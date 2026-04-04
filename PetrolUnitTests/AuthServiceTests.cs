using gsst.Interfaces;
using gsst.Services;
using Microsoft.EntityFrameworkCore;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class AuthServiceTests
    {
        private AppDbContext _context;
        private IAuthService _authService;
        private IUserService _userService;

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _authService = new AuthService(_context);
            _userService = new UserService(_context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.GetDbConnection().Close();
            _context.Dispose();
        }

        [TestMethod]
        public void LoginTestValid()
        {
            var username = "testuser";
            var password = "testpassword";
            _userService.CreateUser("Test User", username, password, "Admin");

            var user = _authService.Login(username, password);

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void LoginTestInvalid()
        {
            var username = "invalid";
            var password = "invalid";

            Assert.Throws<ArgumentException>(() => _authService.Login(username, password));
        }

        [TestMethod]
        public void LoginTestNull()
        {
            string username = null;
            string password = null;

            Assert.Throws<ArgumentException>(() => _authService.Login(username, password));
        }

        [TestMethod]
        public void LoginTestEmpty()
        {
            string username = string.Empty;
            string password = string.Empty;

            Assert.Throws<ArgumentException>(() => _authService.Login(username, password));
        }

        [TestMethod]
        public void LoginTestWhitespace()
        {
            string username = "   ";
            string password = "   ";

            Assert.Throws<ArgumentException>(() => _authService.Login(username, password));
        }
    }
}