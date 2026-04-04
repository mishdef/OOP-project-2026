using gsst.Interfaces;
using gsst.Model.FuelStuff;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Model
{
    public class Fuel : Product
    {
        public FuelType Type { get; set; }

        public Pump Pump { get; set; }

        public override double Price
        {
            get
            {
                return Type?.Price ?? 0;
            }
            set
            {
                if (Type != null)
                {
                    Type.Price = value;
                }
            }
        }
    }
}
