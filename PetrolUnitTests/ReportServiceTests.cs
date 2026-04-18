using gsst.Interfaces;
using gsst.Model;
using gsst.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class ReportServiceTests
    {
        private AppDbContext _context;
        private IReportService _reportService;

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _reportService = new ReportService(_context);

            _context.Orders.RemoveRange(_context.Orders);
            _context.Products.RemoveRange(_context.Products);
            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.GetDbConnection().Close();
            _context.Dispose();
        }

        [TestMethod]
        public void GenerateZReport_WithOrdersInRange_CalculatesCorrectTotals()
        {
            // Arrange
            var shiftStart = new DateTime(2023, 10, 1, 8, 0, 0);
            var shiftEnd = new DateTime(2023, 10, 1, 20, 0, 0);

            var good1 = new Good { Name = "Good 1", Price = 50, BarCode = "111", Image = new byte[0] };
            var good2 = new Good { Name = "Good 2", Price = 30, BarCode = "222", Image = new byte[0] };

            var order1 = new Order { Date = new DateTime(2023, 10, 1, 10, 0, 0), BonusSpent = 10, UserId = 1 };
            order1.Items.Add(new CartItem { Product = good1, Quantity = 1 });

            var order2 = new Order { Date = new DateTime(2023, 10, 1, 15, 0, 0), BonusSpent = 0, UserId = 1 };
            order2.Items.Add(new CartItem { Product = good2, Quantity = 2 });

            var orderOut = new Order { Date = new DateTime(2023, 10, 2, 10, 0, 0), BonusSpent = 5, UserId = 1 };
            orderOut.Items.Add(new CartItem { Product = good1, Quantity = 1 });

            _context.Products.AddRange(good1, good2);
            _context.Orders.AddRange(order1, order2, orderOut);
            _context.SaveChanges();

            // Act
            var report = _reportService.GenerateZReport(shiftStart, shiftEnd);

            // Assert
            Assert.IsNotNull(report);
            Assert.AreEqual(shiftStart, report.ShiftStart);
            Assert.AreEqual(shiftEnd, report.ShiftEnd);
            Assert.AreEqual(2, report.OrdersCount);
            Assert.AreEqual(10, report.TotalBonusesSpent); // 10 + 0
            Assert.AreEqual(100, report.TotalSales); // 40 + 60
        }

        [TestMethod]
        public void GenerateZReport_NoOrdersInRange_ReturnsZeroes()
        {
            // Arrange
            var shiftStart = new DateTime(2023, 10, 1, 8, 0, 0);
            var shiftEnd = new DateTime(2023, 10, 1, 20, 0, 0);

            // Act
            var report = _reportService.GenerateZReport(shiftStart, shiftEnd);

            // Assert
            Assert.IsNotNull(report);
            Assert.AreEqual(0, report.OrdersCount);
            Assert.AreEqual(0, report.TotalBonusesSpent);
            Assert.AreEqual(0, report.TotalSales);
        }

        [TestMethod]
        public void ExportToJson_CreatesValidJsonFile()
        {
            // Arrange
            var report = new ZReportData
            {
                ShiftStart = new DateTime(2023, 10, 1, 8, 0, 0),
                ShiftEnd = new DateTime(2023, 10, 1, 20, 0, 0),
                OrdersCount = 5,
                TotalBonusesSpent = 15.5,
                TotalSales = 500.75
            };

            string tempFilePath = Path.GetTempFileName();

            try
            {
                // Act
                _reportService.ExportToJson(report, tempFilePath);

                // Assert
                Assert.IsTrue(File.Exists(tempFilePath));
                string jsonContent = File.ReadAllText(tempFilePath);

                JObject parsedJson = JObject.Parse(jsonContent);
                Assert.AreEqual(5, (int)parsedJson["OrdersCount"]);
                Assert.AreEqual(15.5, (double)parsedJson["TotalBonusesSpent"]);
                Assert.AreEqual(500.75, (double)parsedJson["TotalSales"]);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [TestMethod]
        public void ExportToTxt_CreatesCorrectlyFormattedTextFile()
        {
            // Arrange
            var report = new ZReportData
            {
                ShiftStart = new DateTime(2023, 10, 1, 8, 0, 0),
                ShiftEnd = new DateTime(2023, 10, 1, 20, 0, 0),
                OrdersCount = 3,
                TotalBonusesSpent = 10,
                TotalSales = 250
            };

            string tempFilePath = Path.GetTempFileName();

            try
            {
                // Act
                _reportService.ExportToTxt(report, tempFilePath);

                // Assert
                Assert.IsTrue(File.Exists(tempFilePath));
                string textContent = File.ReadAllText(tempFilePath);

                Assert.IsTrue(textContent.Contains("================ Z-REPORT ================"));
                Assert.IsTrue(textContent.Contains($"Кількість чеків:   {report.OrdersCount}"));
                Assert.IsTrue(textContent.Contains($"Витрачено бонусів: {report.TotalBonusesSpent:F2} $"));
                Assert.IsTrue(textContent.Contains($"ЗАГАЛЬНА СУМА:     {report.TotalSales:F2} $"));
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
    }
}