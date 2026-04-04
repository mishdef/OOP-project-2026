using gsst.Model;

namespace gsst.Interfaces
{
    public interface IGoodsService
    {
        Good AddProduct(string name, double price, string barcode, byte[] image);
        void ChangePrice(int productId, double newPrice);
        void DeleteProduct(int productId);
        List<Good> GetAllProducts();
        Good GetProductByBarCode(string barCode);
        Good GetProductById(int productId);
        Good GetProductByName(string name);
        List<Good> GetProductsByPrompt(string prompt);
        void UpdateProduct(int productId, string name, double price, string barcode, byte[] image);
    }
}