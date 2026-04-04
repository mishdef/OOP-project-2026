using gsst.Interfaces;
using gsst.Model.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User CreateUser(string fullName, string username, string password, string role)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                throw new ArgumentException("Username already exists.");
            }
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("All fields are required.");
            }
            var user = new User
            {
                FullName = fullName,
                Username = username,
                Password = password,
                Role = role
            };
            _context.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User GetUserById(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            return user;
        }

        public User UpdateUser(int id, string fullName, string username, string password, string role)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            user.FullName = fullName;
            user.Username = username;
            user.Password = password;
            user.Role = role;
            _context.SaveChanges();
            return user;
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
