using gsst.Model.FuelStuff;

namespace gsst.Interfaces
{
    public interface IPumpService
    {
        Pump AddPump(string pumpName, List<Tank> connectedTanks);
        void ChangePumpStatus(int id, PumpStatus status);
        void DeletePump(int id);
        List<Pump> GetAllPumps();
        List<FuelType> GetFuelTypesForPump(int pumpId);
        Pump GetPumpById(int id);
        Task StartPumpAsync(Pump pump, FuelType type, double quantity);
        void UpdatePump(int id, string pumpName, List<Tank> connectedTanks);
    }
}