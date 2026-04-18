using gsst.Interfaces;
using gsst.Model;
using gsst.Model.User;
using Microsoft.EntityFrameworkCore;
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

        public void ProcessCheckout(Order order, int userId)
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

            foreach (var item in order.Items)
            {
                if (item.Product is Fuel currentFuel)
                {
                    var existingFuel = _db.Fuels.FirstOrDefault(f =>
                        f.Type.Id == currentFuel.Type.Id &&
                        f.Pump.Id == currentFuel.Pump.Id);

                    if (existingFuel != null)
                    {
                        item.Product = existingFuel;
                    }
                    else
                    {
                        _db.Entry(currentFuel.Type).State = EntityState.Unchanged;
                        _db.Entry(currentFuel.Pump).State = EntityState.Unchanged;
                    }
                }
                else if (item.Product is Good good)
                {
                    _db.Entry(good).State = EntityState.Unchanged;
                }
            }

            order.UserId = userId;
            order.Date = DateTime.Now;

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

        public List<Order> GetOrderHistory()
        {
            return _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Items)
                    .ThenInclude(i => (i.Product as Fuel).Type)
                .Include(o => o.Items)
                    .ThenInclude(i => (i.Product as Fuel).Pump)
                .OrderByDescending(o => o.Date)
                .ToList();
        }
    }
}
