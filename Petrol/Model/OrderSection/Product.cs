using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Model
{
    public abstract class Product
    {
        private string _name;

        public int Id { get; set; }
        public string Name 
        { 
            get { return _name; }
            set
            {
                if (value.Length <= 0)
                {
                    throw new ArgumentException("Product name cannot be empty");
                }
                if (value.Length > 50)
                {
                    throw new ArgumentException("Product name cannot be longer than 50 characters");
                }
                _name = value;

            } 
        }
        public abstract double Price { get; set; }
    }
}
