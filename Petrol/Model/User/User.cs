using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace gsst.Model.User
{
    public class User
    {
        private string _role = UserRoles.Cashier;
        private string _fullName = null!;
        private string _username = null!;
        private string _password = null!;

        public int Id { get; set; }
        public string FullName { 
            get { return _fullName; }
            set
            {
                if (value.Length < 3)
                {
                    throw new ArgumentException("Full name must be at least 3 characters long");
                }
                if (value.Length > 50)
                {
                    throw new ArgumentException("Full name must be at most 50 characters long");
                }
                _fullName = value;
            }
        }
        public string Username { 
            get { return _username; }
            set
            {
                if (value.Length < 3)
                {
                    throw new ArgumentException("Username must be at least 3 characters long");
                }
                if (value.Length > 50)
                {
                    throw new ArgumentException("Username must be at most 50 characters long");
                }
                if (!Regex.IsMatch(value, @"^[a-zA-Z0-9]+$"))
                {
                    throw new ArgumentException("Username must contain only letters and numbers");
                }
                _username = value;
            }
        }
        public string Password 
        { 
            get { return _password; }
            set
            {
                if (value.Length < 4)
                {
                    throw new ArgumentException("Password must be at least 4 characters long");
                }
                if (value.Length > 32)
                {
                    throw new ArgumentException("Password must be at most 32 characters long");
                }
                _password = value;
            }
        }
        public string Role 
        {
            get { return _role; }
            set 
            {
                if (value != UserRoles.Admin && value != UserRoles.Cashier)
                {
                    throw new ArgumentException($"Role must be either '{UserRoles.Admin}' or '{UserRoles.Cashier}'");
                }
                _role = value;
            } 
        }
    }
}
