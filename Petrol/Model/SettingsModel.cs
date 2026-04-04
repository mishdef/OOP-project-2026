using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Model
{
    public class SettingsModel : ICloneable
    {
        private int _bonusRate = 5;

        public int BonusRate 
        { 
            get => _bonusRate; 
            set {
                if (value > 100)
                {
                    throw new Exception("Bonus rate cannot be more than 100%");
                }
                if (value < 0)
                {
                    throw new Exception("Bonus rate cannot be less than 0%");
                }
                _bonusRate = value;
            }
        }

        public string ConnectionString { get; set; } = "Data Source=petrol.db";

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
