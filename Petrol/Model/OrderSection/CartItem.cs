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
            if (obj is not CartItem otherItem || otherItem.Product == null || this.Product == null)
                return false;
            if (this.Product is Fuel thisFuel && otherItem.Product is Fuel otherFuel)
            {
                return thisFuel.Type?.Id == otherFuel.Type?.Id && thisFuel.Pump?.Id == otherFuel.Pump?.Id;
            }
            if (this.Product.Id == 0 && otherItem.Product.Id == 0)
            {
                return this.Product.Name == otherItem.Product.Name;
            }

            return this.Product.Id == otherItem.Product.Id;
        }

        public override int GetHashCode()
        {
            if (Product is Fuel fuel && fuel.Type != null && fuel.Pump != null)
            {
                return HashCode.Combine(fuel.Type.Id, fuel.Pump.Id);
            }
            return Product?.Id.GetHashCode() ?? 0;
        }
    }
}