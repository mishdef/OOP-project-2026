namespace gsst.Interfaces
{
    public interface IStatisticsService
    {
        double AveraageMoneySpent();
        double AverageFuelSales();
        double CalcucateTotalSales();
        double CalcucateTotalSalesForDate(DateTime date);
        List<string> GetMostPopularFuelTypes(int topN);
        List<string> GetMostPopularProducts(int topN);
        List<string> GetMostPopularPumps(int topN);
        double TotalFuelSales();
        double TotalMoneySpentPerDate(DateTime date);
    }
}