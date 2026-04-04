using gsst.Interfaces;
using gsst.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly IBonusService _bonusService;
        private readonly IEnumerable<IOrderProcessor> _orderProcessors;

        public OrderService(AppDbContext db, IEnumerable<IOrderProcessor> orderProcessors, IBonusService bonusService)
        {
            _db = db;
            _orderProcessors = orderProcessors;
            _bonusService = bonusService;
        }

        public void ProcessCheckout(Order order)
        {
            if (order.BonusCardId.HasValue || order.BonusCardId != null)
            {
                if (order.BonusSpent > 0)
                {
                    _bonusService.RemoveBonus(order.BonusCardId.Value, order.BonusSpent);
                }
                else
                {
                    var bonusPoints = order.Total / 100 * SettingsService.Settings.BonusRate;

                    _bonusService.AddBonus(order.BonusCardId.Value, bonusPoints);
                }
            }

            _db.Orders.Add(order);
            _db.SaveChanges();

            ProcessFuelItems(order.Items);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _db.Orders;
        }

        public async void ProcessFuelItems(IEnumerable<CartItem> items)
        {
            var fuelItems = items.Where(x => x.Product is Fuel).ToList();

            foreach (var item in fuelItems)
            {
                var processor = _orderProcessors.FirstOrDefault(p => p.CanProcess(item.Product));

                if (processor != null)
                {
                    processor.ProcessAsync(item.Product, item.Quantity);
                }
                else
                {
                    // для продукта нет обработчика
                    // можно какой-то прикол прикрутить
                }
            }
        }
    }
}
