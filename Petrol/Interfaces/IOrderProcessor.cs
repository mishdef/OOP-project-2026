using gsst.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Interfaces
{
    public interface IOrderProcessor
    {
        bool CanProcess(Product product);

        void ProcessAsync(Product product, double quantity);
    }
}
