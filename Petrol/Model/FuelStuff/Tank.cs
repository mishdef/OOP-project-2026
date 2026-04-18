using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Model.FuelStuff
{
    public class Tank
    {
        private double _capacity;
        private double _volume = 0;


        public event EventHandler LowVolume;


        public int Id { get; set; }

        public FuelType FuelType { get; set; }

        public double Capacity 
        {
            get
            {
                if (FuelType == null)
                {
                    throw new ArgumentNullException("Fuel type is not set");
                }
                return _capacity;
            }
            set
            {
                if (FuelType == null)
                {
                    throw new ArgumentNullException("Fuel type is not set");
                }
                if (value < 10)
                {
                    throw new ArgumentException("Capacity must be greater than 10");
                }
                
                _capacity = value;
            }
        }

        public double Volume
        {
            get
            {
                if (FuelType == null) throw new ArgumentNullException("Fuel type is not set");

                return _volume;
            }
            set
            {
                if (FuelType == null) throw new ArgumentNullException("Fuel type is not set");

                double oldVolume = _volume;

                if (value > Capacity) _volume = Capacity;
                else if (value < 0) throw new ArgumentException("Volume must be greater than 0");
                else _volume = value;

                double threshold = Capacity / 10;

                if (oldVolume >= threshold && _volume < threshold)
                {
                    LowVolume?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public List<Pump> ConnectedPumps { get; set; } = new List<Pump>();
    }
}
