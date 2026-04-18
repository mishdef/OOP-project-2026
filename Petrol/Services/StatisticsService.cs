using gsst.Interfaces;
using gsst.Model;
using gsst.Model.FuelStuff;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Services
{
    public class StatisticsService : IStatisticsService
    {
        private AppDbContext _context;

        public StatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public double TotalFuelSales()
        {
            return _context.CartItems
                .Include(i => i.Product)
                .AsEnumerable()
                .Where(i => i.Product is Fuel)
                .Sum(i => i.Subtotal); 
        }

        public double AverageFuelSales()
        {
            var fuelItems = _context.CartItems
                .Include(i => i.Product)
                .AsEnumerable()
                .Where(i => i.Product is Fuel)
                .ToList();

            return fuelItems.Any() ? fuelItems.Average(i => i.Subtotal) : 0;
        }

        public double AveraageMoneySpent()
        {
            var orders = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .AsEnumerable();

            return orders.Any() ? orders.Average(o => o.Total) : 0;
        }

        public double TotalMoneySpentPerDate(DateTime date)
        {
            var orders = _context.Orders
                .Where(o => o.Date.Date == date.Date)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .AsEnumerable();

            return orders.Any() ? orders.Average(o => o.Total) : 0;
        }

        public List<string> GetMostPopularFuelTypes(int topN)
        {
            return _context.CartItems
                           .Include(i => i.Product)
                           .AsEnumerable()
                           .Where(i => i.Product is Fuel)
                           .Select(i => new { FuelType = ((Fuel)i.Product).Type?.Name ?? "Unknown", i.Quantity })
                           .GroupBy(x => x.FuelType)
                           .OrderByDescending(g => g.Sum(x => x.Quantity))
                           .Take(topN)
                           .Select(g => g.Key)
                           .ToList();
        }

        public List<string> GetMostPopularPumps(int topN)
        {
            return _context.CartItems
                           .Include(i => i.Product)
                           .AsEnumerable()
                           .Where(i => i.Product is Fuel)
                           .Select(i => new { PumpName = ((Fuel)i.Product).Pump?.Name ?? "Unknown", i.Quantity })
                           .GroupBy(x => x.PumpName)
                           .OrderByDescending(g => g.Sum(x => x.Quantity))
                           .Take(topN)
                           .Select(g => g.Key)
                           .ToList();
        }
        public double CalcucateTotalSales()
        {
            return _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .AsEnumerable()
                .Sum(o => o.Total);
        }

        public double CalcucateTotalSalesForDate(DateTime date)
        {
            return _context.Orders
                .Where(o => o.Date.Date == date.Date)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .AsEnumerable()
                .Sum(o => o.Total);
        }

        public List<string> GetMostPopularProducts(int topN)
        {
            return _context.Orders
                           .Include(o => o.Items)
                               .ThenInclude(i => i.Product)
                           .AsEnumerable()
                           .SelectMany(o => o.Items)
                           .Where(i => i.Product is Good)
                           .GroupBy(i => i.Product?.Name ?? "Unknown")
                           .OrderByDescending(g => g.Sum(i => i.Quantity))
                           .Take(topN)
                           .Select(g => g.Key)
                           .ToList();
        }
    }
}
