using gsst.Model;

namespace gsst.Interfaces
{
    public interface IBonusService
    {
        void AddBonus(int bonusCardId, double amount);
        BonusCard BonusCardGetByBarcode(string barcode);
        BonusCard CreateBonusCard(string clientName, string barcode);
        void DeleteBonusCard(int bonusCardId);
        double GetBonusBalance(int bonusCardId);
        BonusCard GetBonusCardById(int bonusCardId);
        bool IsBonusCardValid(int bonusCardId);
        void RemoveBonus(int bonusCardId, double amount);
        void UpdateBonusCard(int bonusCardId, string clientName, string barcode);
    }
}