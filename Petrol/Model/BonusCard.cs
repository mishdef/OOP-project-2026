using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Model
{
    public class BonusCard
    {
        private double _bonusBalance;

        public int Id { get; set; }
        public string ClientName { get; set; }
        public string Barcode { get; set; }
        public double BonusBalance { 
            get
            { 
                return _bonusBalance; 
            } 
            set
            { 
                if (value < 0) throw new ArgumentException("Amount must be greater than 0");
                _bonusBalance = value;
            } 
        }

        public void AddBonus(double amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than 0");
            BonusBalance += amount;
        }

        public void RemoveBonus(double amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than 0");
            if (amount > BonusBalance) throw new ArgumentException("Not enough balance");
            BonusBalance -= amount;
        }
    }
}
