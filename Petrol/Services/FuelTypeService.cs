using gsst.Interfaces;
using gsst.Model.FuelStuff;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Services
{
    public class FuelTypeService : IFuelTypeService
    {
        private readonly AppDbContext _context;
        private readonly ITanksService _tanksService;

        public FuelTypeService(AppDbContext context, ITanksService tanksService)
        {
            _context = context;
            _tanksService = tanksService;
        }


        public FuelType GetFuelTypeById(int id)
        {
            var fuelType = _context.FuelTypes.Find(id);
            if (fuelType == null)
            {
                throw new Exception("Fuel type not found");
            }
            return fuelType;
        }

        public void AddFuelType(FuelType fuelType)
        {
            if (_context.FuelTypes.Any(x => x.Id == fuelType.Id) || _context.FuelTypes.Any(x => x.Name == fuelType.Name))
            {
                throw new ArgumentException("Fuel type already exists");
            }

            _context.FuelTypes.Add(fuelType);
            _context.SaveChanges();
        }

        public FuelType AddFuelType(string name, double price)
        {
            if (_context.FuelTypes.Any(x => x.Name == name))
            {
                throw new ArgumentException("Fuel type already exists");
            }

            var fuelType = new FuelType { Name = name, Price = price };

            _context.FuelTypes.Add(fuelType);
            _context.SaveChanges();

            return fuelType;
        }

        public void UpdateFuelType(int id, string name, double price)
        {
            var fuelType = _context.FuelTypes.Find(id);
            if (fuelType == null)
            {
                throw new Exception("Fuel type not found");
            }

            fuelType.Name = name;
            fuelType.Price = price;
            _context.SaveChanges();
        }

        public void DeleteFuelType(int id)
        {
            if (_tanksService.IsFuelTypeInUse(id))
            {
                throw new Exception("Cannot delete fuel type because it is in use by a tank.");
            }

            var fuelType = _context.FuelTypes.Find(id);
            if (fuelType == null)
            {
                throw new Exception("Fuel type not found");
            }

            fuelType.IsDeleted = true;
            _context.SaveChanges();
        }

        public List<FuelType> GetAllFuelTypes()
        {
            return _context.FuelTypes.Where(f => !f.IsDeleted).ToList();
        }

        public void ChangePrice(int fuelTypeId, double newPrice)
        {
            var fuelType = _context.FuelTypes.Find(fuelTypeId);
            if (fuelType != null)
            {
                fuelType.Price = newPrice;
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Fuel type not found");
            }
        }
    }
}
