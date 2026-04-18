using gsst.Model;

namespace gsst.Interfaces
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAllOrders();
        void ProcessCheckout(Order order, int userId);
        void ProcessFuelItems(IEnumerable<CartItem> items);
        List<Order> GetOrderHistory();
    }
}