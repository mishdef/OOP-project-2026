using gsst.Model;
using System;

namespace gsst.Interfaces
{
    public interface IReportService
    {
        ZReportData GenerateZReport(DateTime shiftStart, DateTime shiftEnd);
        void ExportToJson(ZReportData report, string filePath);
        void ExportToTxt(ZReportData report, string filePath);
    }
}