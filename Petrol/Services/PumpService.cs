using gsst.Interfaces;
using gsst.Model.FuelStuff;
using Microsoft.EntityFrameworkCore;

namespace gsst.Services
{
    public class PumpService : IPumpService
    {
        private readonly AppDbContext _context;
        private readonly ITanksService _tankService;

        public PumpService(AppDbContext dbContext, ITanksService tankService)
        {
            _context = dbContext;
            _tankService = tankService;
        }

        public async Task StartPumpAsync(Pump pump, FuelType type, double quantity)
        {
            var availablePumps = pump.ConnectedTanks.Where(x => x.FuelType == type).ToList();

            if (availablePumps == null || availablePumps.Count == 0) throw new Exception("No tanks available for this fuel type");

            if (quantity > availablePumps.Sum(x => x.Volume)) throw new Exception("Not enough fuel in tanks");

                if (pump.Status == PumpStatus.Busy) throw new Exception("Pump is busy");
                var fuelTypes = GetFuelTypesForPump(pump.Id);
                if (fuelTypes == null) throw new Exception("Pump is not connected to any tanks");

                await Task.Run(() =>
                {
                    int timeToPump = (int)(quantity * 1000);
                    pump.Status = PumpStatus.Busy;
                    Thread.Sleep(timeToPump);
                    _tankService.RemoveFuelFromTanks(type, quantity);
                    pump.Status = PumpStatus.Free;

                });
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

            var types = pump.ConnectedTanks.Select(x => x.FuelType).Distinct().ToList();

            return types;
        }
    }
}
