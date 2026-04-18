using gsst.Interfaces;
using gsst.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace gsst.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public ZReportData GenerateZReport(DateTime shiftStart, DateTime shiftEnd)
        {
            // Отримуємо всі замовлення за вказаний проміжок часу
            var orders = _context.Orders
                .Where(o => o.Date >= shiftStart && o.Date <= shiftEnd)
                .ToList();

            var report = new ZReportData
            {
                ShiftStart = shiftStart,
                ShiftEnd = shiftEnd,
                OrdersCount = orders.Count,
                TotalBonusesSpent = orders.Sum(o => o.BonusSpent),
                TotalSales = orders.Sum(o => o.Total)
            };

            return report;
        }

        public void ExportToJson(ZReportData report, string filePath)
        {
            string jsonContent = JsonConvert.SerializeObject(report, Formatting.Indented);
            File.WriteAllText(filePath, jsonContent);
        }

        public void ExportToTxt(ZReportData report, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("================ Z-REPORT ================");
                writer.WriteLine($"Звітний період (Початок): {report.ShiftStart}");
                writer.WriteLine($"Звітний період (Кінець):  {report.ShiftEnd}");
                writer.WriteLine("------------------------------------------");
                writer.WriteLine($"Кількість чеків:   {report.OrdersCount}");
                writer.WriteLine($"Витрачено бонусів: {report.TotalBonusesSpent:F2} $");
                writer.WriteLine("------------------------------------------");
                writer.WriteLine($"ЗАГАЛЬНА СУМА:     {report.TotalSales:F2} $");
                writer.WriteLine("==========================================");
            }
        }
    }
}