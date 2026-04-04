using gsst.Interfaces;
using gsst.Model;
using gsst.Model.FuelStuff;
using gsst.Services;
using Microsoft.EntityFrameworkCore;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class StatisticsServiceTests
    {
        private AppDbContext _context;
        private IStatisticsService _statsService;

        private static readonly DateTime Date1 = new DateTime(2023, 1, 1);
        private static readonly DateTime Date2 = new DateTime(2023, 1, 2);

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _statsService = new StatisticsService(_context);

            var fuelType1 = new FuelType { Name = "A-99", Price = 50.0 };
            var fuelType2 = new FuelType { Name = "Diesel", Price = 40.0 };
            var pump1 = new Pump { Name = "Pump #1" };
            var pump2 = new Pump { Name = "Pump #2" };
            var fuel1 = new Fuel { Name = "A-99 Premium", Type = fuelType1, Pump = pump1 };
            var fuel2 = new Fuel { Name = "Diesel Extra", Type = fuelType2, Pump = pump2 };
            var good1 = new Good { Name = "Coffee", Price = 30.0, BarCode = "C123", Image = new byte[0] };
            var good2 = new Good { Name = "Tea", Price = 20.0, BarCode = "T123", Image = new byte[0] };

            var order1 = new Order { Date = Date1 };
            order1.Items.Add(new CartItem { Product = fuel1, Quantity = 10 });
            order1.Items.Add(new CartItem { Product = good1, Quantity = 2 });

            var order2 = new Order { Date = Date1 };
            order2.Items.Add(new CartItem { Product = fuel2, Quantity = 5 });

            var order3 = new Order { Date = Date2 };
            order3.Items.Add(new CartItem { Product = good2, Quantity = 3 });

            _context.Orders.AddRange(order1, order2, order3);
            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.GetDbConnection().Close();
            _context.Dispose();
        }

        [TestMethod]
        public void TotalFuelSales_CalculatesCorrectly()
        {
            var result = _statsService.TotalFuelSales();
            Assert.AreEqual(700, result);
        }

        [TestMethod]
        public void AverageFuelSales_CalculatesCorrectly()
        {
            var result = _statsService.AverageFuelSales();
            Assert.AreEqual(350, result);
        }

        [TestMethod]
        public void AveraageMoneySpent_CalculatesCorrectly()
        {
            var result = _statsService.AveraageMoneySpent();
            Assert.AreEqual(273.33, Math.Round(result, 2));
        }

        [TestMethod]
        public void CalcucateTotalSales_CalculatesCorrectly()
        {
            var result = _statsService.CalcucateTotalSales();
            Assert.AreEqual(820, result);
        }

        [TestMethod]
        public void CalcucateTotalSalesForDate_CalculatesCorrectly()
        {
            var result = _statsService.CalcucateTotalSalesForDate(Date1);
            Assert.AreEqual(760, result);
        }

        [TestMethod]
        public void TotalMoneySpentPerDate_CalculatesCorrectly()
        {
            var result = _statsService.TotalMoneySpentPerDate(Date1);
            Assert.AreEqual(380, result);
        }

        [TestMethod]
        public void GetMostPopularFuelTypes_ReturnsCorrectOrder()
        {
            var popular = _statsService.GetMostPopularFuelTypes(2);

            Assert.AreEqual(2, popular.Count);
            Assert.AreEqual("A-99", popular[0]);
            Assert.AreEqual("Diesel", popular[1]);
        }

        [TestMethod]
        public void GetMostPopularPumps_ReturnsCorrectOrder()
        {
            var popular = _statsService.GetMostPopularPumps(1);

            Assert.AreEqual(1, popular.Count);
            Assert.AreEqual("Pump #1", popular[0]);
        }

        [TestMethod]
        public void GetMostPopularProducts_ReturnsCorrectOrder()
        {
            var popular = _statsService.GetMostPopularProducts(3);

            Assert.AreEqual(3, popular.Count);
            Assert.AreEqual("A-99 Premium", popular[0]);
            Assert.AreEqual("Diesel Extra", popular[1]);
            Assert.AreEqual("Tea", popular[2]);
        }
    }
}
