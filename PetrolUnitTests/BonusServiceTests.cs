using gsst.Interfaces;
using gsst.Model;
using gsst.Services;
using Microsoft.EntityFrameworkCore;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class BonusServiceTests
    {
        private AppDbContext _context ;
        private IBonusService _bonusService;

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _bonusService = new BonusService(_context);

            var card = _context.BonusCards.First();
            card.BonusBalance = 0;
            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.GetDbConnection().Close();
            _context.Dispose();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        [TestMethod]
        public void AddBonus()
        {
            //Arrange
            int bonusCardId = _context.BonusCards.First().Id;
            double amount = 100;

            //Act
            _bonusService.AddBonus(bonusCardId, amount);

            //Assert
            Assert.AreEqual(amount, _context.BonusCards.First().BonusBalance);
        }

        [TestMethod]
        public void AddBonusNegative()
        {
            //Arrange
            int bonusCardId = _context.BonusCards.First().Id;
            double amount = -100;

            //Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _bonusService.AddBonus(bonusCardId, amount)
                );
        }

        [TestMethod]
        public void AddBonusZero()
        {
            //Arrange
            int bonusCardId = _context.BonusCards.First().Id;
            double amount = 0;

            //Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _bonusService.AddBonus(bonusCardId, amount)
                );
        }

        [TestMethod]
        public void RemoveBonus()
        {
            //Arrange
            int bonusCardId = _context.BonusCards.First().Id;
            double amount = 100;
            _bonusService.AddBonus(bonusCardId, amount);

            //Act
            _bonusService.RemoveBonus(bonusCardId, amount);

            //Assert
            Assert.AreEqual(0, _context.BonusCards.First().BonusBalance);
        }

        [TestMethod]
        public void RemoveBonusNegative()
        {
            //Arrange
            int bonusCardId = _context.BonusCards.First().Id;
            double amount = -100;

            //Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _bonusService.RemoveBonus(bonusCardId, amount)
                );
        }

        [TestMethod]
        public void RemoveBonusZero()
        {
            //Arrange
            int bonusCardId = _context.BonusCards.First().Id;
            double amount = 0;

            //Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _bonusService.RemoveBonus(bonusCardId, amount)
                );
        }

        [TestMethod]
        public void RemoveBonusMoreThanBalance()
        {
            //Arrange
            int bonusCardId = _context.BonusCards.First().Id;
            double amount = 1000;

            //Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _bonusService.RemoveBonus(bonusCardId, amount)
                );
        }

        [TestMethod]
        public void GetBonusBalance()
        {
            //Arrange
            int bonusCardId = _context.BonusCards.First().Id;

            //Act
            var balance = _bonusService.GetBonusBalance(bonusCardId);

            //Assert
            Assert.IsNotNull(balance);
        }

        [TestMethod]
        public void IsBonusCardValid1()
        {
            //Arrange
            int bonusCardId = _context.BonusCards.First().Id;

            //Act
            var isValid = _bonusService.IsBonusCardValid(bonusCardId);

            //Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsBonusCardValid2()
        {
            //Arrange
            int bonusCardId = 42;

            //Act
            var isValid = _bonusService.IsBonusCardValid(bonusCardId);

            //Assert
            Assert.IsFalse(isValid);
        }
    }
}
