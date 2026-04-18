using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace gsst.Model
{
    public class Order : IEnumerable<CartItem>
    {
        public int Id { get; set; }
        public List<CartItem> Items { get; set; } = new ();

        public int? BonusCardId { get; set; }
        public double BonusSpent { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public User.User User { get; set; } = null!;


        public double Total
        {
            get
            {
                double total = 0;
                foreach (var item in Items)
                {
                    total += item.Subtotal;
                }
                return Math.Round(total - BonusSpent, 2);
            }
        }

        public Order() { }

        public IEnumerator<CartItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
