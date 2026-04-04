using gsst.Interfaces;
using gsst.Model;
using gsst.Model.OrderProcessors;
using gsst.Model.User;
using gsst.Services;
using Microsoft.EntityFrameworkCore;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class OrderServiceTests
    {
        private AppDbContext _context;
        private IBonusService _bonusService;
        private IOrderService _orderService;

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _bonusService = new BonusService(_context);
            _orderService = new OrderService(
                _context,
                new List<IOrderProcessor>() { new FuelOrderProcessor(new PumpService(_context, new TanksService(_context))) },
                _bonusService);

            _context.BonusCards.Add(new BonusCard { Id = 777, Barcode = "ORDER_CARD", ClientName = "Order Tester", BonusBalance = 100 });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.GetDbConnection().Close();
            _context.Dispose();
        }

        [TestMethod]
        public void ProcessCheckout_NoBonusCard_SavesOrder()
        {
            var order = new Order();
            order.Items.Add(new CartItem { Product = new Good { Name = "Water", Price = 10, BarCode = "111" }, Quantity = 2 });

            _orderService.ProcessCheckout(order);

            var savedOrders = _orderService.GetAllOrders().ToList();
            Assert.AreEqual(1, savedOrders.Count);
            Assert.AreEqual(20, savedOrders.First().Total);
        }

        [TestMethod]
        public void ProcessCheckout_WithBonusCard_NoBonusSpent_AddsBonusPoints()
        {
            var order = new Order { BonusCardId = 777, BonusSpent = 0 };
            order.Items.Add(new CartItem { Product = new Good { Name = "Snack", Price = 100, BarCode = "222" }, Quantity = 1 });

            double oldBalance = _bonusService.GetBonusBalance(777);
            double expectedBonus = order.Total / 100 * SettingsService.Settings.BonusRate;

            _orderService.ProcessCheckout(order);

            double newBalance = _bonusService.GetBonusBalance(777);
            Assert.AreEqual(oldBalance + expectedBonus, newBalance);
        }

        [TestMethod]
        public void ProcessCheckout_WithBonusCard_WithBonusSpent_RemovesBonusPoints()
        {
            var order = new Order { BonusCardId = 777, BonusSpent = 50 };
            order.Items.Add(new CartItem { Product = new Good { Name = "Coffee", Price = 100, BarCode = "333" }, Quantity = 1 });

            double oldBalance = _bonusService.GetBonusBalance(777);

            _orderService.ProcessCheckout(order);

            double newBalance = _bonusService.GetBonusBalance(777);
            Assert.AreEqual(oldBalance - 50, newBalance);
            Assert.AreEqual(50, order.Total); 
        }
    }
}
