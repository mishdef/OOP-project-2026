using gsst.Interfaces;
using gsst.Model.FuelStuff;
using gsst.Services;
using Microsoft.EntityFrameworkCore;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class PumpServiceTests
    {
        private AppDbContext _context;
        private ITanksService _tanksService;
        private IPumpService _pumpService;

        private Tank Tank1 { get; set; }
        private Tank Tank2 { get; set; }
        private FuelType FuelType1 { get; set; }
        private FuelType FuelType2 { get; set; }

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _tanksService = new TanksService(_context);
            _pumpService = new PumpService(_context, _tanksService);

            FuelType1 = new FuelType() { Name = "TestFuel", Price = 1.23 };
            FuelType2 = new FuelType() { Name = "TestFuel2", Price = 1.2 };

            _context.Tanks.RemoveRange(_context.Tanks);
            _context.Pumps.RemoveRange(_context.Pumps);
            _context.SaveChanges();

            Tank1 = _tanksService.AddTank(FuelType1, 100);
            Tank2 = _tanksService.AddTank(FuelType2, 100);

            _tanksService.RefillTanksWithFuelType(FuelType1, 100);
            _tanksService.RefillTanksWithFuelType(FuelType2, 100);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.GetDbConnection().Close();
            _context.Dispose();
        }

        [TestMethod]
        public void AddPumpTest()
        {
            //Arrange
            _pumpService.AddPump("Pump 1", new List<Tank> { Tank1 });

            //Act
            Assert.AreEqual(1, _context.Pumps.Count());


            //Arrange
            _pumpService.AddPump("Pump 2", new List<Tank> { Tank1, Tank2 });

            //Act
            Assert.AreEqual(2, _context.Pumps.Count());
        }

        [TestMethod]
        public void AddPumpTestIncorrect()
        {
            //Act + Assert
            Assert.Throws<ArgumentException>(() => _pumpService.AddPump("", new List<Tank> { Tank1 }));
            Assert.Throws<ArgumentException>(() => _pumpService.AddPump("Pumpdasdkljasdjaskdjaskldjaskldjaslkdjalsdjaslkdjlkasjdl", new List<Tank> { }));
        }

        [TestMethod]
        public void GetPumpsTest()
        {
            //Arrange
            _pumpService.AddPump("Pump 1", new List<Tank> { Tank1 });
            _pumpService.AddPump("Pump 2", new List<Tank> { Tank1, Tank2 });

            //Act
            Assert.AreEqual(2, _pumpService.GetAllPumps().Count());
        }

        [TestMethod]
        public void GetPumpByIdTest()
        {
            //Arrange
            var pump = _pumpService.AddPump("Pump 1", new List<Tank> { Tank1 });
            _pumpService.AddPump("Pump 2", new List<Tank> { Tank1, Tank2 });

            //Act
            Assert.AreEqual("Pump 1", _pumpService.GetPumpById(pump.Id).Name);
        }

        [TestMethod]
        public void GetFuelTypesForPumpTest()
        {
            //Arrange
            _pumpService.AddPump("Pump 1", new List<Tank> { Tank1 });
            var pump = _pumpService.AddPump("Pump 2", new List<Tank> { Tank1, Tank2 });

            var res = _pumpService.GetFuelTypesForPump(pump.Id);

            //Act
            Assert.AreEqual(2, res.Count());
        }

        [TestMethod]
        public void GetFuelTypesForPumpTestIncorrect()
        {
            //Act + Assert
            Assert.Throws<ArgumentException>(() => _pumpService.GetFuelTypesForPump(-4));
        }

        [TestMethod]
        public void DeletePumpTest()
        {
            //Arrange
            var pump = _pumpService.AddPump("Pump 1", new List<Tank> { Tank1 });
            _pumpService.AddPump("Pump 2", new List<Tank> { Tank1, Tank2 });

            //Act
            _pumpService.DeletePump(pump.Id);

            //Assert
            Assert.AreEqual(1, _pumpService.GetAllPumps().Count());
        }

        [TestMethod]
        public void DeletePumpTestIncorrect()
        {
            //Arrange
            var pump = _pumpService.AddPump("Pump 1", new List<Tank> { Tank1 });
            _pumpService.AddPump("Pump 2", new List<Tank> { Tank1, Tank2 });

            //Act + Assert
            Assert.Throws<ArgumentException>(() => _pumpService.DeletePump(-4));
            Assert.Throws<ArgumentException>(() => _pumpService.DeletePump(pump.Id + 5));
        }

        [TestMethod]
        public void UpdatePumpTest()
        {
            //Arrange
            var pump = _pumpService.AddPump("Pump 1", new List<Tank> { Tank1 });
            _pumpService.AddPump("Pump 2", new List<Tank> { Tank1, Tank2 });

            //Act
            _pumpService.UpdatePump(pump.Id, "Pump 3", new List<Tank> { Tank1, Tank2 });

            //Assert
            Assert.AreEqual("Pump 3", _pumpService.GetPumpById(pump.Id).Name);
        }

        [TestMethod]
        public void UpdatePumpTestIncorrect()
        {
            //Arrange
            var pump = _pumpService.AddPump("Pump 1", new List<Tank> { Tank1 });
            _pumpService.AddPump("Pump 2", new List<Tank> { Tank1, Tank2 });

            //Act + Assert
            Assert.Throws<ArgumentException>(() => _pumpService.UpdatePump(-4, "Pump 3", new List<Tank> { Tank1, Tank2 }));
            Assert.Throws<ArgumentException>(() => _pumpService.UpdatePump(pump.Id + 5, "Pump 3", new List<Tank> { Tank1, Tank2 }));
        }

        [TestMethod]
        public void ChangePumpStatusTest()
        {
            //Arrange
            var pump = _pumpService.AddPump("Pump 1", new List<Tank> { Tank1 });
            _pumpService.AddPump("Pump 2", new List<Tank> { Tank1, Tank2 });

            //Act
            _pumpService.ChangePumpStatus(pump.Id, PumpStatus.Busy);

            //Assert
            Assert.AreEqual(PumpStatus.Busy, _pumpService.GetPumpById(pump.Id).Status);
        }

        [TestMethod]
        public void ChangePumpStatusTestIncorrect()
        {
            //Arrange
            var pump = _pumpService.AddPump("Pump 1", new List<Tank> { Tank1 });
            _pumpService.AddPump("Pump 2", new List<Tank> { Tank1, Tank2 });

            //Act + Assert
            Assert.Throws<ArgumentException>(() => _pumpService.ChangePumpStatus(-4, PumpStatus.Busy));
            Assert.Throws<ArgumentException>(() => _pumpService.ChangePumpStatus(pump.Id + 5, PumpStatus.Busy));
        }

        [TestMethod]
        public async Task StartPumpAsyncTest()
        {
            // Arrange
            var fuelType = new FuelType { Name = "TestFuelAsync", Price = 10 };
            _context.FuelTypes.Add(fuelType);

            var tank = new Tank { FuelType = fuelType, Capacity = 1000, Volume = 500 };
            _context.Tanks.Add(tank);
            _context.SaveChanges();

            var pump = _pumpService.AddPump("PumpAsync", new List<Tank> { tank });

            // Act
            await _pumpService.StartPumpAsync(pump, pump.ConnectedTanks.First().FuelType, 10);

            // Assert
            Assert.AreEqual(PumpStatus.Free, _pumpService.GetPumpById(pump.Id).Status);
        }

        [TestMethod]
        public async Task StartPumpAsyncTestNotEnoughFuel()
        {
            // Arrange
            var fuelType = new FuelType { Name = "TestFuelAsync2", Price = 10 };
            _context.FuelTypes.Add(fuelType);

            var tank = new Tank { FuelType = fuelType, Capacity = 1000, Volume = 50 };
            _context.Tanks.Add(tank);
            _context.SaveChanges();

            var pump = _pumpService.AddPump("PumpAsync2", new List<Tank> { tank });

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _pumpService.StartPumpAsync(pump, pump.ConnectedTanks.First().FuelType, 1000);
            });
        }
    }
}
