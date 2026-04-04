using gsst.Interfaces;
using gsst.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Services
{
    public class BonusService : IBonusService
    {
        private readonly AppDbContext _context;

        public BonusService(AppDbContext context)
        {
            _context = context;
        }

        public void AddBonus(int bonusCardId, double amount)
        {
            var bonusCard = _context.BonusCards.Where(c => c.Id == bonusCardId).FirstOrDefault();
            if (bonusCard != null)
            {
                bonusCard.AddBonus(amount);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Bonus card not found");
            }
        }

        public void RemoveBonus(int bonusCardId, double amount)
        {
            var bonusCard = _context.BonusCards.Where(c => c.Id == bonusCardId).FirstOrDefault();
            if (bonusCard != null)
            {
                bonusCard.RemoveBonus(amount);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Bonus card not found");
            }
        }

        public double GetBonusBalance(int bonusCardId)
        {
            var bonusCard = _context.BonusCards.Where(c => c.Id == bonusCardId).FirstOrDefault();
            if (bonusCard != null)
            {
                return bonusCard.BonusBalance;
            }
            else
            {
                throw new Exception("Bonus card not found");
            }
        }

        public bool IsBonusCardValid(int bonusCardId)
        {
            var bonusCard = _context.BonusCards.Where(c => c.Id == bonusCardId).FirstOrDefault();
            return bonusCard != null;
        }

        public BonusCard GetBonusCardById(int bonusCardId)
        {
            var bonusCard = _context.BonusCards.Where(c => c.Id == bonusCardId).FirstOrDefault();
            if (bonusCard != null) return bonusCard;
            else throw new Exception("Bonus card not found");
        }

        public BonusCard CreateBonusCard(string clientName, string barcode)
        {
            var bonusCard = new BonusCard
            {
                ClientName = clientName,
                Barcode = barcode
            };
            _context.BonusCards.Add(bonusCard);
            _context.SaveChanges();
            return bonusCard;
        }

        public void DeleteBonusCard(int bonusCardId)
        {
            var bonusCard = _context.BonusCards.Where(c => c.Id == bonusCardId).FirstOrDefault();
            if (bonusCard != null)
            {
                _context.BonusCards.Remove(bonusCard);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Bonus card not found");
            }
        }

        public void UpdateBonusCard(int bonusCardId, string clientName, string barcode)
        {
            var bonusCard = _context.BonusCards.Where(c => c.Id == bonusCardId).FirstOrDefault();
            if (bonusCard != null)
            {
                bonusCard.ClientName = clientName;
                bonusCard.Barcode = barcode;
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Bonus card not found");
            }
        }

        public BonusCard BonusCardGetByBarcode(string barcode)
        {
            var bonusCard = _context.BonusCards.Where(c => c.Barcode == barcode).FirstOrDefault();
            if (bonusCard != null) return bonusCard;
            else return null;
        }
    }
}
