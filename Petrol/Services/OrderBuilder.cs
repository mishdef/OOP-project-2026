using gsst.Interfaces;
using gsst.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Services
{
    public class OrderBuilder : IOrderBuilder
    {
        private readonly Order _order;
        private readonly IBonusService _bonusService;

        public OrderBuilder(IBonusService bonusService)
        {
            _order = new Order();
            _bonusService = bonusService;
        }

        public void AddItem(CartItem item)
        {
            foreach (var orderItem in _order)
            {
                if (orderItem.Equals(item))
                {
                    orderItem.Quantity += item.Quantity;
                    return;
                }
            }
            _order.Items.Add(item);
        }
        public void RemoveItem(CartItem item)
        {
            _order.Items.Remove(item);
        }

        public void AddQuanity(CartItem item, int quantity)
        {
            if (quantity > 0)
            {
                item.Quantity += quantity;
            }
        }

        public void AddBonusCard(int id)
        {
            if (_bonusService.IsBonusCardValid(id))
            {
                _order.BonusCardId = id;
            }
        }

        public void SetDiscountFromBonus(double amount)
        {
            if (_order.BonusCardId == null) throw new ArgumentException("No bonus card");
            if (amount < 0) throw new ArgumentException("Amount must be greater than 0");
            if (amount > _bonusService.GetBonusBalance(_order.BonusCardId ?? 0)) throw new ArgumentException("Not enough balance");
            if (amount > _order.Total) throw new ArgumentException("Amount must be less than total price");

            _order.BonusSpent = amount;
        }

        public void RemoveBonusCard()
        {
            _order.BonusCardId = null;
            _order.BonusSpent = 0;
        }

        public Order Build()
        {
            return _order;
        }
    }
}
