using gsst.Model.FuelStuff;

namespace gsst.Interfaces
{
    public interface IFuelTypeService
    {
        void AddFuelType(FuelType fuelType);
        FuelType AddFuelType(string name, double price);
        void ChangePrice(int fuelTypeId, double newPrice);
        void DeleteFuelType(int id);
        List<FuelType> GetAllFuelTypes();
        FuelType GetFuelTypeById(int id);
        void UpdateFuelType(int id, string name, double price);
    }
}