using gsst.Interfaces;
using gsst.Services;
using Microsoft.EntityFrameworkCore;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class GoodsServiceTests
    {
        private AppDbContext _context;
        private IGoodsService _goodsService;

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _goodsService = new GoodsService(_context);

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
        public void AddProduct()
        {
            //Arrange
            string name = "TestProduct";
            double price = 9.99;
            string barcode = "1234567890";
            byte[] image = new byte[] { 0xFF, 0xD8, 0xFF };

            //Act
            var result = _goodsService.AddProduct(name, price, barcode, image);

            //Assert
            var product = _goodsService.GetProductById(result.Id);
            Assert.IsNotNull(product);
            Assert.AreEqual(price, product.Price);
            Assert.AreEqual(barcode, product.BarCode);
            Assert.AreEqual(image, product.Image);
        }

        [TestMethod]
        [DataRow("", 9.99, "1234567890", new byte[] { 0xFF, 0xD8, 0xFF })]
        [DataRow("TestProduct", -1, "1234567890", new byte[] { 0xFF, 0xD8, 0xFF })]
        [DataRow("TestProTestProductTestProductTestProductTestProductTestProductTestProductTestProductTestProductTestProductTestProductduct", 9.99, "1234567890", new byte[] { 0xFF, 0xD8, 0xFF })]
        public void AddProductIncorrect(string name, double price, string barcode, byte[] image)
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _goodsService.AddProduct(name, price, barcode, image));
        }

        [TestMethod]
        public void GetProductById()
        {
            //Arrange
            string name = "TestProduct";
            double price = 9.99;
            string barcode = "1234567890";
            byte[] image = new byte[] { 0xFF, 0xD8, 0xFF };

            //Act
            var result = _goodsService.AddProduct(name, price, barcode, image);
            var product = _goodsService.GetProductById(result.Id);

            //Assert
            Assert.IsNotNull(product);
            Assert.AreEqual(price, product.Price);
            Assert.AreEqual(barcode, product.BarCode);
            Assert.AreEqual(image, product.Image);
        }

        [TestMethod]
        public void GetProductByIdNotFound()
        {
            //Act & Assert
            Assert.Throws<Exception>(() => _goodsService.GetProductById(1));
        }

        [TestMethod]
        public void GetAllProducts()
        {
            //Arrange
            string name = "TestProduct";
            double price = 9.99;
            string barcode = "1234567890";
            byte[] image = new byte[] { 0xFF, 0xD8, 0xFF };

            //Act
            var result = _goodsService.AddProduct(name, price, barcode, image);
            var products = _goodsService.GetAllProducts();

            //Assert
            Assert.IsNotNull(products);
            Assert.AreEqual(1, products.Count);
            Assert.AreEqual(price, products[0].Price);
            Assert.AreEqual(barcode, products[0].BarCode);
            Assert.AreEqual(image, products[0].Image);
        }

        [TestMethod]
        public void RemoveProduct()
        {
            //Arrange
            string name = "TestProduct";
            double price = 9.99;
            string barcode = "1234567890";
            byte[] image = new byte[] { 0xFF, 0xD8, 0xFF };

            //Act
            var result = _goodsService.AddProduct(name, price, barcode, image);
            var product = _goodsService.GetProductsByPrompt(barcode);
            _goodsService.DeleteProduct(product[0].Id);

            //Assert
            Assert.Throws<Exception>(() => _goodsService.GetProductById(result.Id));
        }

        [TestMethod]
        public void RemoveProductNotFound()
        {
            //Act & Assert
            Assert.Throws<Exception>(() => _goodsService.DeleteProduct(1));
        }

        [TestMethod]
        public void UpdateProduct()
        {
            //Arrange
            string name = "TestProduct";
            double price = 9.99;
            string barcode = "1234567890";
            byte[] image = new byte[] { 0xFF, 0xD8, 0xFF };

            //Act
            var result = _goodsService.AddProduct(name, price, barcode, image);
            _goodsService.UpdateProduct(result.Id, "UpdatedTestProduct", 19.99, "1234567890", new byte[] { 0xFF, 0xD8, 0xFF });

            //Assert
            var product = _goodsService.GetProductById(result.Id);
            Assert.IsNotNull(product);
            Assert.AreEqual("UpdatedTestProduct", product.Name);
            Assert.AreEqual(19.99, product.Price);
            Assert.AreEqual("1234567890", product.BarCode);
        }

        [TestMethod]
        public void UpdateProductNotFound()
        {
            //Act & Assert
            Assert.Throws<Exception>(() => _goodsService.UpdateProduct(1, "UpdatedTestProduct", 19.99, "1234567890", new byte[] { 0xFF, 0xD8, 0xFF }));
        }

        [TestMethod]
        public void ChangePrice()
        {
            //Arrange
            string name = "TestProduct";
            double price = 9.99;
            string barcode = "1234567890";
            byte[] image = new byte[] { 0xFF, 0xD8, 0xFF };

            //Act
            var result = _goodsService.AddProduct(name, price, barcode, image);
            _goodsService.ChangePrice(result.Id, 19.99);

            //Assert
            var product = _goodsService.GetProductById(result.Id);
            Assert.IsNotNull(product);
            Assert.AreEqual(19.99, product.Price);
        }

        [TestMethod]
        public void ChangePriceNotFound()
        {
            //Act & Assert
            Assert.Throws<Exception>(() => _goodsService.ChangePrice(1, 19.99));
        }

        [TestMethod]
        public void GetProductsByPrompt()
        {
            //Arrange
            string name = "TestProduct";
            double price = 9.99;
            string barcode = "1234567890";
            byte[] image = new byte[] { 0xFF, 0xD8, 0xFF };

            //Act
            var result = _goodsService.AddProduct(name, price, barcode, image);
            var products = _goodsService.GetProductsByPrompt(barcode);

            //Assert
            Assert.IsNotNull(products);
            Assert.AreEqual(1, products.Count);
            Assert.AreEqual(price, products[0].Price);
            Assert.AreEqual(barcode, products[0].BarCode);
            Assert.AreEqual(image, products[0].Image);
        }

        [TestMethod]
        public void GetProductsByPromptNotFound()
        {
            //Act
            var products = _goodsService.GetProductsByPrompt("1234567890");

            //Assert
            Assert.IsNotNull(products);
            Assert.AreEqual(0, products.Count);
        }
    }
}
