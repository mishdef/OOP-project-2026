using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace gsst.Model
{
    public class CartItem
    {
        public int Id { get; set; }

        public Product? Product { get; set; }

        public double Quantity { get; set; }

        public double Subtotal
        {
            get
            {
                if (Product == null) return 0;
                return Math.Round(Product.Price * Quantity, 2);
            }
        }

        public override bool Equals(object? obj)
        {
            return Product.Id == ((CartItem)obj).Product.Id;
        }
    }
}