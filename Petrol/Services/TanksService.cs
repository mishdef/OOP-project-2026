using gsst.Interfaces;
using gsst.Model.FuelStuff;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Services
{
    public class TanksService : ITanksService
    {
        private readonly AppDbContext _context;

        public TanksService(AppDbContext context)
        {
            _context = context;
        }

        public Tank AddTank(FuelType fuelType, double capacity)
        {
            var tank = new Tank()
            {
                FuelType = fuelType,
                Capacity = capacity,
                Volume = 0
            };
            _context.Tanks.Add(tank);
            _context.SaveChanges();

            return tank;
        }

        public List<Tank> GetAllTanks()
        {
            return _context.Tanks.ToList();
        }

        public List<Tank> GetTanksByFuelType(FuelType fuelType)
        {
            if (fuelType == null) throw new ArgumentNullException("FuelType cannot be null");
            var tanks = _context.Tanks.Where(x => x.FuelType == fuelType).ToList();
            if (tanks == null) throw new ArgumentException("Tanks not found");
            if (tanks.Count == 0) throw new ArgumentException("Tanks not found");
            return tanks;
        }

        public Tank GetTankById(int id)
        {
            if (id < 0) throw new ArgumentException("Id must be greater than 0 or equal to 0");
            var tank = _context.Tanks.Find(id);
            if (tank == null) throw new ArgumentException("Tank not found");
            return tank;
        }

        public void DeleteTank(int id)
        {
            var tank = _context.Tanks.Find(id);
            if (id < 0) throw new ArgumentException("Id must be greater than 0 or equal to 0");
            if (tank == null)
            {
                throw new ArgumentException("Tank not found");
            }
            _context.Tanks.Remove(tank);
            _context.SaveChanges();
        }

        public void UpdateTank(int id, double capacity, double volume)
        {
            if (id < 0) throw new ArgumentException("Id must be greater than 0 or equal to 0");
            if (volume > capacity) throw new ArgumentException("Volume cannot be greater than capacity");
            var tank = _context.Tanks.Find(id);
            if (tank == null) throw new ArgumentException("Tank not found");
            tank.Capacity = capacity;
            tank.Volume = volume;
            _context.SaveChanges();
        }

        public void ChangeTankFuelType(int tankId, FuelType newFuelType)
        {
            var tank = _context.Tanks.FirstOrDefault(x => x.Id == tankId);

            if (tank == null)
            {
                throw new ArgumentException("Tank not found");
            }

            if (newFuelType == null)
            {
                throw new ArgumentException("New fuel type cannot be null");
            }

            if (tank.FuelType == newFuelType)
            {
                return;
            }

            tank.FuelType = newFuelType;
            tank.Volume = 0;
            _context.SaveChanges();
        }

        public bool IsFuelTypeInUse(int fuelTypeId)
        {
            return _context.Tanks.Any(x => x.FuelType.Id == fuelTypeId);
        }

        public void RemoveFuelFromTanks(FuelType fuelType, double amount)
        {
            if (fuelType == null) throw new ArgumentNullException("FuelType cannot be null");
            if (amount < 0) throw new ArgumentException("Amount must be greater than 0 or equal to 0");
            if (_context.Tanks.Count() == 0) throw new ArgumentException("Tanks not found");
            if (_context.Tanks.Any(x => x.FuelType == fuelType) == false) throw new ArgumentException("Fuel type not found");

            var tanks = _context.Tanks.Where(x => x.FuelType == fuelType).ToList();
            var totalAvailable = tanks.Sum(t => t.Volume);

            if (totalAvailable < amount)
                throw new InvalidOperationException("Not enough fuel in tanks!");

            var amountLeftToRemove = amount;

            do
            {
                foreach (Tank tank in tanks)
                {
                    if (tank.Volume >= amountLeftToRemove)
                    {
                        tank.Volume -= amountLeftToRemove;
                        amountLeftToRemove = 0;
                        break;
                    }
                    else
                    {
                        amountLeftToRemove -= tank.Volume;
                        tank.Volume = 0;
                    }
                }
            } while (amountLeftToRemove > 0);

            _context.SaveChanges();
        }

        public void RefillTanksWithFuelType(FuelType fuelType, double amount)
        {
            var leftToFill = amount;

            List<Tank> tanks = _context.Tanks.Where(x => x.FuelType == fuelType).ToList();
            foreach (Tank tank in tanks)
            {
                var spaceAvailable = tank.Capacity - tank.Volume;
                if (leftToFill > spaceAvailable)
                {
                    tank.Volume = tank.Capacity;
                    leftToFill -= spaceAvailable;
                }
                else
                {
                    tank.Volume += leftToFill;
                    leftToFill = 0;
                }
            }

            _context.SaveChanges();
        }

        public void RefillTank(int tankId, double amount)
        {
            var tank = _context.Tanks.Find(tankId);

            if (tank == null)
            {
                throw new Exception("Tank not found");
            }

            tank.Volume += amount;

            _context.SaveChanges();
        }

        public List<Tank> GetAvailableTanks(FuelType fuelType)
        {
            return _context.Tanks.Where(x => x.FuelType == fuelType && x.Volume > 0).ToList();
        }

        public bool IsAmountAvailable(FuelType fuelType, double amount)
        {
            var tanks = _context.Tanks.Where(x => x.FuelType == fuelType).ToList();
            var totalAvailable = tanks.Sum(t => t.Volume);

            return totalAvailable >= amount;
        }
    }
}
