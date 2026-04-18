using System;

namespace gsst.Model
{
    public class ZReportData
    {
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
        public double TotalSales { get; set; }
        public double TotalBonusesSpent { get; set; }
        public int OrdersCount { get; set; }
    }
}