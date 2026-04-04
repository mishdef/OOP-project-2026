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

        
        public double Total 
        { 
            get {
                double total = 0;
                foreach (var item in Items)
                {
                    total += item.Subtotal;
                }
                return total - BonusSpent;
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
