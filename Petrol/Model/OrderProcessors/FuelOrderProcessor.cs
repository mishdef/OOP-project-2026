using gsst.Interfaces;
using gsst.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Model.OrderProcessors
{
    public class FuelOrderProcessor : IOrderProcessor
    {
        private readonly IPumpService _pumpService;

        public FuelOrderProcessor(IPumpService pumpService)
        {
            _pumpService = pumpService;
        }

        public bool CanProcess(Product product)
        {
            return product is Fuel;
        }

        public void ProcessAsync(Product product, double quantity)
        {
            if (product is Fuel fuel)
            {
                if (fuel.Pump.IsAvailable())
                {
                    Task.Run(() => _pumpService.StartPumpAsync(fuel.Pump, fuel.Type, quantity));
                }
                else
                {
                    throw new Exception("Pump is not available");
                }
            }
        }
    }
}
