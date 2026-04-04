using gsst.Model;
using gsst.Services;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class SettingsTests
    {
        [TestMethod]
        public void SettingsModel_SetValidBonusRate_SetsValue()
        {
            var settings = new SettingsModel();
            settings.BonusRate = 15;
            Assert.AreEqual(15, settings.BonusRate);
        }

        [TestMethod]
        public void SettingsModel_SetBonusRateBelowZero_ThrowsException()
        {
            var settings = new SettingsModel();
            Assert.Throws<Exception>(() => settings.BonusRate = -1);
        }

        [TestMethod]
        public void SettingsModel_SetBonusRateAbove100_ThrowsException()
        {
            var settings = new SettingsModel();
            Assert.Throws<Exception>(() => settings.BonusRate = 101);
        }

        [TestMethod]
        public void SettingsModel_Clone_CreatesDeepCopy()
        {
            var original = new SettingsModel { BonusRate = 20, ConnectionString = "Data Source=test.db" };
            var clone = (SettingsModel)original.Clone();

            Assert.AreNotSame(original, clone);
            Assert.AreEqual(original.BonusRate, clone.BonusRate);
            Assert.AreEqual(original.ConnectionString, clone.ConnectionString);
        }

        [TestMethod]
        public void SettingsService_SettingsAreInitialized()
        {
            Assert.IsNotNull(SettingsService.Settings);
            Assert.IsNotNull(SettingsService.Settings.ConnectionString);
            Assert.IsTrue(SettingsService.Settings.BonusRate >= 0);
        }
    }
}