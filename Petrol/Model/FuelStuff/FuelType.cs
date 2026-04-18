using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Model.FuelStuff
{
    public class FuelType
    {
        private string _name;
        private double _price;


        public int Id { get; set; }

        public string Name 
        {
            get
            {
                return _name;
            }
            set
            {
                if (value.IsNullOrEmpty())
                {
                    throw new ArgumentException("Fuel type name cannot be empty");
                }
                if (value.Length > 20)
                {
                    throw new ArgumentException("Fuel type name cannot be longer than 20 characters");
                }
                _name = value;
            }
        }

        public double Price 
        {
            get
            {
                return _price;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Fuel type price cannot be negative");
                }
                _price = value;
            }
        }

        public bool IsDeleted { get; set; } = false;
    }
}
