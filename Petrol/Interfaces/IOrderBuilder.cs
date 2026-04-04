using gsst.Model;

namespace gsst.Interfaces
{
    internal interface IOrderBuilder
    {
        void AddBonusCard(int id);
        void AddItem(CartItem item);
        void AddQuanity(CartItem item, int quantity);
        Order Build();
        void RemoveBonusCard();
        void RemoveItem(CartItem item);
    }
}