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

        private User _user;

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _bonusService = new BonusService(_context);
            _orderService = new OrderService(
                _context,
                new List<IOrderProcessor>() { new FuelOrderProcessor(new PumpService(_context, new TanksService(_context))) },
                _bonusService);

            _user = new UserService(_context).CreateUser("Test User", "testuser", "testpassword", "Admin");

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
            var good = new Good { Name = "Water", Price = 10, BarCode = "111" };
            _context.Goods.Add(good);
            _context.SaveChanges(); 

            var order = new Order();
            order.Items.Add(new CartItem { Product = good, Quantity = 2 });

            _orderService.ProcessCheckout(order, _user.Id);

            var savedOrders = _orderService.GetAllOrders().ToList();
            Assert.AreEqual(1, savedOrders.Count);
            Assert.AreEqual(20, savedOrders.First().Total);
        }

        [TestMethod]
        public void ProcessCheckout_WithBonusCard_NoBonusSpent_AddsBonusPoints()
        {
            var good = new Good { Name = "Snack", Price = 100, BarCode = "222" };
            _context.Goods.Add(good);
            _context.SaveChanges();

            var order = new Order { BonusCardId = 777, BonusSpent = 0 };
            order.Items.Add(new CartItem { Product = good, Quantity = 1 });

            double oldBalance = _bonusService.GetBonusBalance(777);
            double expectedBonus = order.Total / 100 * SettingsService.Settings.BonusRate;

            _orderService.ProcessCheckout(order, _user.Id);

            double newBalance = _bonusService.GetBonusBalance(777);
            Assert.AreEqual(oldBalance + expectedBonus, newBalance);
        }

        [TestMethod]
        public void ProcessCheckout_WithBonusCard_WithBonusSpent_RemovesBonusPoints()
        {
            var good = new Good { Name = "Coffee", Price = 100, BarCode = "333" };
            _context.Goods.Add(good);
            _context.SaveChanges();

            var order = new Order { BonusCardId = 777, BonusSpent = 50 };
            order.Items.Add(new CartItem { Product = good, Quantity = 1 });

            double oldBalance = _bonusService.GetBonusBalance(777);

            _orderService.ProcessCheckout(order, _user.Id);

            double newBalance = _bonusService.GetBonusBalance(777);
            Assert.AreEqual(oldBalance - 50, newBalance);
            Assert.AreEqual(50, order.Total);
        }
    }
}
