using gsst.Interfaces;
using gsst.Model.User;
using gsst.Services;
using Microsoft.EntityFrameworkCore;

namespace GsstUnitTests
{
    [TestClass]
    public sealed class UserServiceTests
    {
        private AppDbContext _context;
        private IUserService _userService;

        [TestInitialize]
        public void TestInit()
        {
            _context = TestDbContextFactory.Create();
            _userService = new UserService(_context);

            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.GetDbConnection().Close();
            _context.Dispose();
        }

        [TestMethod]
        [DoNotParallelize]
        public void AddUser()
        {
            //Arrange
            string password = "TestPassword";
            string fullName = "TestFullName";
            string username = "TestUsername";
            string role = "Cashier";

            //Act
            var result = _userService.CreateUser(fullName, username, password, role);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fullName, result.FullName);
            Assert.AreEqual(username, result.Username);
            Assert.AreEqual(password, result.Password);
            Assert.AreEqual(role, result.Role);
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow("TestFullName", "TestUsername", "", "Cashier")]
        [DataRow("", "TestUsername", "TestPassword", "Cashier")]
        [DataRow("TestFullName", "", "TestPassword", "Cashier")]
        [DataRow("TestFullName", "TestUsername", "TestPassword", "")]
        [DataRow("TestFullName", "TestUsername", "TestPassword", "TestRole")]
        [DataRow("Te", "TestUsername", "TestPassword", "Cashier")]
        [DataRow("TestFullName", "Te", "TestPassword", "Cashier")]
        [DataRow("TestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFulTestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNamelNameestFullNameestFullName", "TestUsername", "Te", "Cashier")]
        [DataRow("TestFullName", "TestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameTestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullName", "TestPassword", "Cashier")]
        [DataRow("TestFullName", "TestUsername", "TestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFuTestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNameestFullNamellNameestFullNameestFullNameestFullName", "Cashier")]
        public void AddUserIncorrect(string fullName, string username, string password, string role)
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _userService.CreateUser(fullName, username, password, role));
        }

        [TestMethod]
        [DoNotParallelize]
        public void AddUserDuplicateUsername()
        {
            //Arrange
            string password = "TestPassword";
            string fullName = "TestFullName";
            string username = "TestUsername";
            string role = "Cashier";
            _userService.CreateUser(fullName, username, password, role);

            //Act & Assert
            Assert.Throws<ArgumentException>(() => _userService.CreateUser(fullName, username, password, role));
        }

        [TestMethod]
        [DoNotParallelize]
        public void GetAllUsers()
        {
            //Arrange
            string password = "TestPassword";
            string fullName = "TestFullName";
            string username = "TestUsername";
            string role = "Cashier";
            _userService.CreateUser(fullName, username, password, role);

            //Act
            var result = _userService.GetAllUsers();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        [DoNotParallelize]
        public void GetUserById()
        {
            //Arrange
            string password = "TestPassword";
            string fullName = "TestFullName";
            string username = "TestUsername";
            string role = "Cashier";
            var user = _userService.CreateUser(fullName, username, password, role);

            //Act
            var result = _userService.GetUserById(user.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fullName, result.FullName);
            Assert.AreEqual(username, result.Username);
            Assert.AreEqual(password, result.Password);
            Assert.AreEqual(role, result.Role);
        }

        [TestMethod]
        [DoNotParallelize]
        public void GetUserByIncorrectId()
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _userService.GetUserById(20));
        }

        [TestMethod]
        [DoNotParallelize]
        public void DeleteUserById()
        {
            //Arrange
            string password = "TestPassword";
            string fullName = "TestFullName";
            string username = "TestUsername";
            string role = "Cashier";
            var user = _userService.CreateUser(fullName, username, password, role);

            //Act
            _userService.DeleteUser(user.Id);

            //Assert
            Assert.Throws<ArgumentException>(() => _userService.GetUserById(user.Id));
        }

        [TestMethod]
        [DoNotParallelize]
        public void DeleteUserByIncorrectId()
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _userService.DeleteUser(20));
        }

        [TestMethod]
        [DoNotParallelize]
        public void UpdateUser()
        {
            //Arrange
            string password = "TestPassword";
            string fullName = "TestFullName";
            string username = "TestUsername";
            string role = "Cashier";
            var user = _userService.CreateUser(fullName, username, password, role);

            //Act
            var result = _userService.UpdateUser(user.Id, "UpdatedFullName", "UpdatedUsername", "UpdatedPassword", "Admin");
            
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UpdatedFullName", result.FullName);
            Assert.AreEqual("UpdatedUsername", result.Username);
            Assert.AreEqual("UpdatedPassword", result.Password);
            Assert.AreEqual("Admin", result.Role);
        }

        [TestMethod]
        [DoNotParallelize]
        public void UpdateUserByIncorrectId()
        {
            //Act & Assert
            Assert.Throws<ArgumentException>(() => _userService.UpdateUser(20, "UpdatedFullName", "UpdatedUsername", "UpdatedPassword", "Admin"));
        }
    }
}
