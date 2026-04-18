using gsst.Model.FuelStuff;

namespace gsst.Interfaces
{
    public interface ITanksService
    {
        Tank AddTank(FuelType fuelType, double capacity);
        void ChangeTankFuelType(int oldFuelTypeId, FuelType newFuelType);
        void DeleteTank(int id);
        List<Tank> GetAllTanks();
        List<Tank> GetAvailableTanks(FuelType fuelType);
        Tank GetTankById(int id);
        List<Tank> GetTanksByFuelType(FuelType fuelType);
        bool IsAmountAvailable(FuelType fuelType, double amount);
        bool IsFuelTypeInUse(int fuelTypeId);
        void RefillTank(int tankId, double amount);
        void RefillTanksWithFuelType(FuelType fuelType, double amount);
        void RemoveFuelFromTanks(FuelType fuelType, double amount);
        void UpdateTank(int id, double capacity, double volume);
        void RemoveFuelFromConnectedTanks(IEnumerable<Tank> connectedTanks, FuelType fuelType, double amount);
    }
}