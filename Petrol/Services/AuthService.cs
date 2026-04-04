using gsst.Interfaces;
using gsst.Model.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public User Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and password are required.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                throw new ArgumentException("Invalid username or password.");
            }

            return user;
        }
    }
}
