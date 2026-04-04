using gsst.Model;

namespace gsst.Interfaces
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAllOrders();
        void ProcessCheckout(Order order);
        void ProcessFuelItems(IEnumerable<CartItem> items);
    }
}