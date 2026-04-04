using gsst.Interfaces;
using gsst.Model.FuelStuff;
using gsst.Services;
using Microsoft.EntityFrameworkCore;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class TanksServiceTests
    {
        private AppDbContext _context;
        private ITanksService _tanksService;

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _tanksService = new TanksService(_context);

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
        public void AddTank()
        {
            //Arrange
            var fuelType = new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act
            _tanksService.AddTank(fuelType, capacity);

            //Assert
            Assert.IsNotNull(_context.Tanks.FirstOrDefault(x => x.FuelType.Name == fuelType.Name && x.Capacity == capacity));
        }

        [TestMethod]
        public void AddTankInvalidCapacity()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.AddTank(new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 }, -1000));
        }

        [TestMethod]
        public void AddTankInvalidCapacityZero()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.AddTank(new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 }, 0));
        }

        [TestMethod]
        public void AddTankInvalidCapacityNegative()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.AddTank(new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 }, -1000));
        }

        [TestMethod]
        public void AddtankVolumeCorrect()
        {
            //Arrange
            var fuelType = new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity);
            _tanksService.RefillTank(tank.Id, capacity);

            //Assert
            Assert.AreEqual(capacity, _context.Tanks.FirstOrDefault(x => x.FuelType.Name == fuelType.Name && x.Capacity == capacity).Volume);
        }

        [TestMethod]
        public void AddtankIncorrectFuelType()
        {
            //Arrange
            FuelType fuelType = null;
            double capacity = 1000;

            //Act & Assert
            Assert.Throws<ArgumentNullException>(() => _tanksService.AddTank(fuelType, capacity));
        }

        [TestMethod]
        public void RefillTankInvalidAmount()
        {
            //Arrange
            var fuelType = new FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity);
            _tanksService.RefillTank(tank.Id, capacity + 100);

            //Assert
            Assert.AreEqual(capacity, _context.Tanks.FirstOrDefault(x => x.FuelType.Name == fuelType.Name && x.Capacity == capacity).Volume);
        }

        [TestMethod]
        public void RefillTankInCorrect2Amount()
        {
            //Arrange
            var fuelType = new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act & Assert
            var tank = _tanksService.AddTank(fuelType, capacity);

            Assert.Throws<ArgumentException>(() => _tanksService.RefillTank(tank.Id, -50));
        }


        [TestMethod]
        public void RemoveTank()
        {
            //Arrange
            var fuelType = new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity);
            _tanksService.DeleteTank(tank.Id);

            //Assert
            Assert.IsNull(_context.Tanks.FirstOrDefault(x => x.FuelType.Name == fuelType.Name && x.Capacity == capacity));
        }

        [TestMethod]
        public void RemoveTankInvalidId()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.DeleteTank(-1));
        }

        [TestMethod]
        public void RemoveTankNotFound()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.DeleteTank(1));
        }

        [TestMethod]
        public void GetAllTanks()
        {
            //Arrange
            var fuelType = new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity);

            //Assert
            Assert.IsNotNull(_tanksService.GetAllTanks());
        }

        [TestMethod]
        public void GetAllTanksEmpty()
        {
            Assert.IsNotNull(_tanksService.GetAllTanks());
        }

        [TestMethod]
        public void GetTanksByFuelType()
        {
            //Arrange
            var fuelType = new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity);

            //Assert
            Assert.IsNotNull(_tanksService.GetTanksByFuelType(fuelType));
        }

        [TestMethod]
        public void GetTanksByFuelTypeNotFound()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.GetTanksByFuelType(new gsst.Model.FuelStuff.FuelType()));
        }

        [TestMethod]
        public void GetTankById()
        {
            //Arrange
            var fuelType = new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity);

            //Assert
            Assert.IsNotNull(_tanksService.GetTankById(tank.Id));
        }

        [TestMethod]
        public void GetTankByIdInvalidId()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.GetTankById(-1));
        }

        [TestMethod]
        public void GetTankByIdNotFound()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.GetTankById(1));
        }

        [TestMethod]
        public void GetTankByIncorrectId()
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _tanksService.GetTankById(20));
        }

        [TestMethod]
        public void DeleteTankCorrectId()
        {
            //Arrange
            var fuelType = new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity);
            _tanksService.DeleteTank(tank.Id);

            //Assert
            Assert.Throws<ArgumentException>(() => _tanksService.GetTankById(tank.Id));
        }

        [TestMethod]
        public void DeleteTankIncorrectId()
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _tanksService.DeleteTank(20));
        }

        [TestMethod]
        public void DeleteTankInvalidId()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.DeleteTank(-1));
        }

        [TestMethod]
        public void DeleteTankNotFound()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.DeleteTank(1));
        }

        [TestMethod]
        public void UpdateTankCorrectId()
        {
            //Arrange
            var fuelType = new FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity);
            _tanksService.UpdateTank(tank.Id, capacity, capacity);

            //Assert
            Assert.IsNotNull(_tanksService.GetTankById(tank.Id));
        }

        [TestMethod]
        [DataRow(20, 1000, 1000)]
        [DataRow(-1, 1000, 1000)]
        public void UpdateTankIncorrectId(int id, double capacity, double volume)
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _tanksService.UpdateTank(id, capacity, volume));
        }

        [TestMethod]
        [DataRow(1000, 1020)]
        [DataRow(1000, -1)]
        [DataRow(-1, 1000)]
        [DataRow(-1, -1)]
        public void UpdateTankIncorrectValues(double capacity, double volume)
        {
            //Arrange
            var fuelType = new gsst.Model.FuelStuff.FuelType() { Name = "TestFuel", Price = 1.23 };

            //Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                var tank = _tanksService.AddTank(fuelType, capacity);
                _tanksService.UpdateTank(tank.Id, capacity, volume);
            });
        }

        [TestMethod]
        public void ChangeTankFuelTypeCorrectId()
        {
            //Arrange
            var fuelType = new FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;
            var newFuelType = new FuelType() { Name = "TestFuel2", Price = 1.23 };

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity);
            _tanksService.ChangeTankFuelType(tank.Id, newFuelType);

            //Assert
            Assert.IsNotNull(_tanksService.GetTankById(tank.Id));
            Assert.AreEqual(newFuelType, tank.FuelType);
            Assert.AreEqual(0, tank.Volume);
        }

        [TestMethod]
        public void ChangeTankFuelTypeIncorrectId()
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _tanksService.ChangeTankFuelType(20, new FuelType() { Name = "TestFuel", Price = 1.23 }));
        }

        [TestMethod]
        public void ChangeTankFuelTypeToNull()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.ChangeTankFuelType(-1, null));
        }

        [TestMethod]
        public void ChangeTankFuelTypeNull()
        {
            Assert.Throws<ArgumentException>(() => _tanksService.ChangeTankFuelType(-1, null));
        }

        [TestMethod]
        public void IsFuelTypeInUseFalse()
        {
            Assert.IsFalse(_tanksService.IsFuelTypeInUse(new FuelType() { Name = "TestFuel", Price = 1.23 }.Id));
        }

        [TestMethod]
        public void IsFuelTypeInUseTrue()
        {
            //Arrange
            var fuelType = new FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act & Assert
            var tank = _tanksService.AddTank(fuelType, capacity);
            Assert.IsTrue(_tanksService.IsFuelTypeInUse(fuelType.Id));
        }

        [TestMethod]
        public void RemoveFuelFromTanksCorrectFromOneTank()
        {
            //Arrange
            var fuelType = new FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity = 1000;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity);

            _tanksService.RefillTanksWithFuelType(fuelType, 100);

            _tanksService.RemoveFuelFromTanks(fuelType, 100);

            //Assert
            Assert.IsTrue(_tanksService.GetTankById(tank.Id).Volume == 0);
        }

        [TestMethod]
        public void RemoveFuelFromTanksCorrectFromManyTanks()
        {
            //Arrange
            var fuelType = new FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity1 = 50;
            double capacity2 = 50;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity1);
            var tank2 = _tanksService.AddTank(fuelType, capacity2);

            _tanksService.RefillTanksWithFuelType(fuelType, 100);

            _tanksService.RemoveFuelFromTanks(fuelType, 100);

            //Assert
            Assert.IsTrue(_tanksService.GetTankById(tank.Id).Volume == 0);
            Assert.IsTrue(_tanksService.GetTankById(tank2.Id).Volume == 0);
        }

        [TestMethod]
        public void EventLowVolume()
        {
            //Arrange
            var fuelType = new FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity1 = 50;
            double capacity2 = 50;
            bool called1 = false;
            bool called2 = false;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity1);
            var tank2 = _tanksService.AddTank(fuelType, capacity2);

            _tanksService.RefillTanksWithFuelType(fuelType, 100);
            _tanksService.RefillTanksWithFuelType(fuelType, 100);

            tank.LowVolume += (sender, args) => called1 = true;
            tank2.LowVolume += (sender, args) => called2 = true;

            var check1 = tank.Volume;
            var check2 = tank2.Volume;

            _tanksService.RemoveFuelFromTanks(fuelType, 100);

            //Assert
            Assert.IsTrue(called1);
            Assert.IsTrue(called2);
        }

        [TestMethod]
        public void GetAvailableTanks()
        {
            //Arrange
            var fuelType = new FuelType() { Name = "TestFuel", Price = 1.23 };
            double capacity1 = 50;
            double capacity2 = 50;

            var expectedCount = 2;

            //Act
            var tank = _tanksService.AddTank(fuelType, capacity1);
            var tank2 = _tanksService.AddTank(fuelType, capacity2);

            _tanksService.RefillTanksWithFuelType(fuelType, 100);

            //Assert

            Assert.AreEqual(expectedCount, _tanksService.GetAvailableTanks(fuelType).Count());

        }

        [TestMethod]
        public void GetAvailableTanksNoTanks()
        {
            //Arrange
            var fuelType = new FuelType() { Name = "TestFuel", Price = 1.23 };

            //Assert
            Assert.AreEqual(0, _tanksService.GetAvailableTanks(fuelType).Count());
        }

        [TestMethod]
        [DataRow(50, true)]
        [DataRow(100, false)]
        public void IsAmountAvailable(double amount, bool expected)
        {
            //Arrange
            var fuelType = new FuelType() { Name = "TestFuel", Price = 1.23 };

            _tanksService.AddTank(fuelType, 100);
            _tanksService.RefillTanksWithFuelType(fuelType, 70);


            //Act & Assert
            Assert.IsTrue(_tanksService.IsAmountAvailable(fuelType, amount) == expected);
        }
    }
}
