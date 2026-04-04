using gsst.Interfaces;
using gsst.Model.FuelStuff;
using gsst.Services;
using Microsoft.EntityFrameworkCore;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class FuelTypeServiceTests
    {
        private AppDbContext _context;
        private ITanksService _tanksService;
        private IFuelTypeService _fuelTypeService;

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _tanksService = new TanksService(_context);
            _fuelTypeService = new FuelTypeService(_context, _tanksService);

            _context.FuelTypes.RemoveRange(_context.FuelTypes);
            _context.Tanks.RemoveRange(_context.Tanks);
            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.GetDbConnection().Close();
            _context.Dispose();
        }

        [TestMethod]
        public void AddFuelType()
        {
            //Arrange
            var fuelType = new FuelType()
            {
                Name = "TestFuel",
                Price = 1.23
            };

            //Act
            _fuelTypeService.AddFuelType(fuelType);

            //Assert
            Assert.IsNotNull(_context.FuelTypes.FirstOrDefault(x => x.Name == fuelType.Name));
        }

        [TestMethod]
        public void AddFuelTypeInvalidName()
        {
            Assert.Throws<ArgumentException>(() => 
                new FuelType() 
                { 
                    Name = "", 
                    Price = 1.23 
                }
            );
        }

        [TestMethod]
        public void AddFuelTypeInvalidPrice()
        {
            Assert.Throws<ArgumentException>(() =>
                new FuelType()
                {
                    Name = "TestFuel",
                    Price = -1
                }
            );
        }

        [TestMethod]
        public void AddFuelTypeDuplicateName()
        {
            //Arrange
            var fuelType = new FuelType()
            {
                Name = "TestFuel",
                Price = 1.23
            };
            _fuelTypeService.AddFuelType(fuelType);

            //Act
            Assert.Throws<ArgumentException>(() => _fuelTypeService.AddFuelType(fuelType));
        }

        [TestMethod]
        public void AddFuelTypeSecondMethod()
        {
            //Arrange
            //Act
            var fuelType = _fuelTypeService.AddFuelType("TestFuel", 1.23);

            //Assert
            Assert.IsNotNull(_context.FuelTypes.FirstOrDefault(x => x.Name == fuelType.Name));
        }

        [TestMethod]
        public void AddFuelTypeSecondMethodInvalidName()
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _fuelTypeService.AddFuelType("", 1.23));
        }

        [TestMethod]
        public void AddFuelTypeSecondMethodInvalidPrice()
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _fuelTypeService.AddFuelType("TestFuel", -1));
        }

        [TestMethod]
        public void RemoveFuelType()
        {
            //Arrange
            var fuelType = new FuelType()
            {
                Name = "TestFuel",
                Price = 1.23
            };
            _fuelTypeService.AddFuelType(fuelType);

            //Act
            _fuelTypeService.DeleteFuelType(fuelType.Id);

            //Assert
            Assert.IsNull(_context.FuelTypes.FirstOrDefault(x => x.Name == fuelType.Name));
        }

        [TestMethod]
        public void RemoveFuelTypeNotFound()
        {
            //Act & Assert
            Assert.Throws<Exception>(() => _fuelTypeService.DeleteFuelType(999));
        }

        [TestMethod]
        public void GetFuelTypeById()
        {
            //Arrange
            var fuelType = new FuelType()
            {
                Name = "TestFuel",
                Price = 1.23
            };
            _fuelTypeService.AddFuelType(fuelType);

            //Act
            var result = _fuelTypeService.GetFuelTypeById(fuelType.Id);

            //Assert
            Assert.AreEqual(fuelType, result);
        }

        [TestMethod]
        public void GetFuelTypeByIdNotFound()
        {
            //Act & Assert
            Assert.Throws<Exception>(() => _fuelTypeService.GetFuelTypeById(999));
        }

        [TestMethod]
        public void GetAllFuelTypes()
        {
            //Arrange
            var fuelType = new FuelType()
            {
                Name = "TestFuel",
                Price = 1.23
            };
            _fuelTypeService.AddFuelType(fuelType);

            //Act
            var result = _fuelTypeService.GetAllFuelTypes();

            //Assert
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void UpdateFuelType()
        {
            //Arrange
            var fuelType = new FuelType()
            {
                Name = "TestFuel",
                Price = 1.23
            };
            _fuelTypeService.AddFuelType(fuelType);

            //Act
            _fuelTypeService.UpdateFuelType(fuelType.Id, "UpdatedTestFuel", 2.34);

            //Assert
            var updatedFuelType = _context.FuelTypes.Find(fuelType.Id);
            Assert.AreEqual("UpdatedTestFuel", updatedFuelType.Name);
            Assert.AreEqual(2.34, updatedFuelType.Price);
        }

        [TestMethod]
        public void UpdateFuelTypeNotFound()
        {
            //Act & Assert
            Assert.Throws<Exception>(() => _fuelTypeService.UpdateFuelType(999, "UpdatedTestFuel", 2.34));
        }

        [TestMethod]
        public void ChangePrice()
        {
            //Arrange
            var fuelType = new FuelType()
            {
                Name = "TestFuel",
                Price = 1.23
            };
            _fuelTypeService.AddFuelType(fuelType);

            //Act
            _fuelTypeService.ChangePrice(fuelType.Id, 2.34);

            //Assert
            var updatedFuelType = _context.FuelTypes.Find(fuelType.Id);
            Assert.AreEqual(2.34, updatedFuelType.Price);
        }

        [TestMethod]
        public void ChangePriceNotFound()
        {
            //Act & Assert
            Assert.Throws<Exception>(() => _fuelTypeService.ChangePrice(999, 2.34));
        }
    }
}
