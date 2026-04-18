using gsst.Interfaces;
using gsst.Model.FuelStuff;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace gsst.Services
{
    public class PumpService : IPumpService
    {
        private readonly AppDbContext _context;
        private readonly ITanksService _tankService;

        public event Action<int, bool>? PumpStateChanged;

        private static readonly object _dbLock = new object();

        public PumpService(AppDbContext dbContext, ITanksService tankService)
        {
            _context = dbContext;
            _tankService = tankService;
        }

        public bool IsFuelAvailableOnPump(int pumpId, int fuelTypeId, double requestedAmount)
        {
            var pump = _context.Pumps
                .Include(p => p.ConnectedTanks)
                .ThenInclude(t => t.FuelType)
                .FirstOrDefault(p => p.Id == pumpId);

            if (pump == null) return false;

            var availableVolume = pump.ConnectedTanks
                .Where(t => t.FuelType?.Id == fuelTypeId)
                .Sum(t => t.Volume);

            return availableVolume >= requestedAmount;
        }

        public async Task StartPumpAsync(Pump pump, FuelType type, double quantity)
        {
            PumpStateChanged?.Invoke(pump.Id, false);
            try
            {
                lock (_dbLock)
                {
                    var dbPump = _context.Pumps
                        .Include(p => p.ConnectedTanks)
                        .ThenInclude(t => t.FuelType)
                        .FirstOrDefault(p => p.Id == pump.Id);

                    if (dbPump == null) throw new Exception("Pump not found");

                    var availableTanks = dbPump.ConnectedTanks.Where(x => x.FuelType?.Id == type.Id).ToList();
                    if (availableTanks.Count == 0)
                        throw new Exception("No tanks available for this fuel type on this pump");

                    if (quantity > availableTanks.Sum(x => x.Volume))
                        throw new Exception("Not enough fuel in connected tanks");

                    if (dbPump.Status == PumpStatus.Busy)
                        throw new Exception("Pump is already busy");

                    dbPump.Status = PumpStatus.Busy;
                    _context.SaveChanges();

                    _tankService.RemoveFuelFromConnectedTanks(dbPump.ConnectedTanks, type, quantity);
                }

                int timeToPump = (int)(quantity * 1000);
                if (timeToPump < 10000) timeToPump = 10000;
                await Task.Delay(timeToPump);

                lock (_dbLock)
                {
                    var dbPump = _context.Pumps.Find(pump.Id);
                    if (dbPump != null)
                    {
                        dbPump.Status = PumpStatus.Free;
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                lock (_dbLock)
                {
                    var pumpToFree = _context.Pumps.Find(pump.Id);
                    if (pumpToFree != null && pumpToFree.Status == PumpStatus.Busy)
                    {
                        pumpToFree.Status = PumpStatus.Free;
                        _context.SaveChanges();
                    }
                }
                throw;
            }
            finally
            {
                PumpStateChanged?.Invoke(pump.Id, true);
            }
        }

        public Pump AddPump(string pumpName, List<Tank> connectedTanks)
        {
            var pump = new Pump()
            {
                Name = pumpName,
                ConnectedTanks = connectedTanks,
                Status = PumpStatus.Free
            };
            _context.Pumps.Add(pump);
            _context.SaveChanges();

            return pump; 
        }

        public List<Pump> GetAllPumps()
        {
            return _context.Pumps.ToList();
        }

        public void DeletePump(int id)
        {
            if (id < 0) throw new ArgumentException("Id must be greater than 0 or equal to 0");

            var pump = _context.Pumps.Find(id);
            if (pump == null) throw new ArgumentException("Pump not found");

            _context.Pumps.Remove(pump);
            _context.SaveChanges();
        }

        public void UpdatePump(int id, string pumpName, List<Tank> connectedTanks)
        {
            if (id < 0) throw new ArgumentException("Id must be greater than 0 or equal to 0");

            var pump = _context.Pumps.Find(id);

            if (pump == null) throw new ArgumentException("Pump not found");

            pump.Name = pumpName;
            pump.ConnectedTanks = connectedTanks;
            _context.SaveChanges();
        }

        public Pump GetPumpById(int id)
        {
            if (id < 0) throw new ArgumentException("Id must be greater than 0 or equal to 0");
            
            var pump = _context.Pumps.Find(id);
            if (pump == null) throw new ArgumentException("Pump not found");

            return pump;
        }

        public void ChangePumpStatus(int id, PumpStatus status)
        {
            if (id < 0) throw new ArgumentException("Id must be greater than 0 or equal to 0");

            var pump = _context.Pumps.Find(id);
            if (pump == null) throw new ArgumentException("Pump not found");
            pump.Status = status;
            _context.SaveChanges();
        }

        public List<FuelType> GetFuelTypesForPump(int pumpId)
        {
            if (pumpId < 0) throw new ArgumentException("Id must be greater than 0 or equal to 0");

            var pump = _context.Pumps
                .Include(p => p.ConnectedTanks)
                    .ThenInclude(t => t.FuelType)
                .FirstOrDefault(p => p.Id == pumpId);

            if (pump == null || pump.ConnectedTanks == null)
            {
                return new List<FuelType>();
            }

            var types = pump.ConnectedTanks.Select(x => x.FuelType).DistinctBy(x => x.Id).ToList();

            return types;
        }
    }
}
