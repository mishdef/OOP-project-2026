using gsst.Interfaces;
using gsst.Model;
using gsst.Model.FuelStuff;
using gsst.Services;
using Microsoft.EntityFrameworkCore;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class OrderBuilderTests
    {
        private AppDbContext _context;
        private IBonusService _bonusService;
        private OrderBuilder _orderBuilder;

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _bonusService = new BonusService(_context);
            _orderBuilder = new OrderBuilder(_bonusService);

            _context.BonusCards.Add(new BonusCard { Id = 999, Barcode = "BUILDER", ClientName = "Builder", BonusBalance = 500 });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.GetDbConnection().Close();
            _context.Dispose();
        }

        [TestMethod]
        public void AddItem_NewItem_AddsToOrder()
        {
            var product = new Good { Id = 1, Name = "Test Good", Price = 10 };
            var item = new CartItem { Id = 1, Product = product, Quantity = 2 };

            _orderBuilder.AddItem(item);
            var order = _orderBuilder.Build();

            Assert.AreEqual(1, order.Items.Count);
            Assert.AreEqual(20, order.Total);
        }

        [TestMethod]
        public void AddItem_ExistingItem_IncreasesQuantity()
        {
            var product = new Good { Id = 1, Name = "Test Good", Price = 10 };
            var item1 = new CartItem { Product = product, Quantity = 2 };
            var item2 = new CartItem { Product = product, Quantity = 3 }; 

            _orderBuilder.AddItem(item1);
            _orderBuilder.AddItem(item2);
            var order = _orderBuilder.Build();

            Assert.AreEqual(1, order.Items.Count);
            Assert.AreEqual(5, order.Items.First().Quantity);
            Assert.AreEqual(50, order.Total);
        }

        [TestMethod]
        public void RemoveItem_RemovesFromOrder()
        {
            var product = new Good { Id = 1, Name = "Test Good", Price = 10 };
            var item = new CartItem { Product = product, Quantity = 2 };

            _orderBuilder.AddItem(item);
            _orderBuilder.RemoveItem(item);
            var order = _orderBuilder.Build();

            Assert.AreEqual(0, order.Items.Count);
            Assert.AreEqual(0, order.Total);
        }

        [TestMethod]
        public void AddQuanity_IncreasesItemQuantity()
        {
            var product = new Good { Id = 1, Name = "Test Good", Price = 10 };
            var item = new CartItem { Product = product, Quantity = 2 };

            _orderBuilder.AddItem(item);
            _orderBuilder.AddQuanity(item, 3); 
            var order = _orderBuilder.Build();

            Assert.AreEqual(5, order.Items.First().Quantity);
        }

        [TestMethod]
        public void SetDiscountFromBonus_ValidAmount_AppliesDiscount()
        {
            var product = new Good { Id = 1, Name = "Test Good", Price = 100 };
            _orderBuilder.AddItem(new CartItem { Product = product, Quantity = 1 });

            _orderBuilder.AddBonusCard(999);
            _orderBuilder.SetDiscountFromBonus(20);
            var order = _orderBuilder.Build();

            Assert.AreEqual(20, order.BonusSpent);
            Assert.AreEqual(80, order.Total);
        }

        [TestMethod]
        public void SetDiscountFromBonus_MoreThanTotal_ThrowsException()
        {
            var product = new Good { Id = 1, Name = "Test Good", Price = 10 };
            _orderBuilder.AddItem(new CartItem { Product = product, Quantity = 1 });

            _orderBuilder.AddBonusCard(999);

            Assert.Throws<ArgumentException>(() => _orderBuilder.SetDiscountFromBonus(20));
        }
    }
}
