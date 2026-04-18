using gsst.Interfaces;
using gsst.Model;

namespace gsst.Services
{
    public class GoodsService : IGoodsService
    {
        event EventHandler NewProductAdded;

        private readonly AppDbContext _context;

        public GoodsService(AppDbContext context)
        {
            _context = context;
        }

        public Good AddProduct(string name, double price, string barcode, byte[] image)
        {
            var product = new Good()
            {
                Name = name,
                Price = price,
                BarCode = barcode,
                Image = image
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            return product;
        }

        public void ChangePrice(int productId, double newPrice)
        {
            if (_context.Products.Find(productId) is Good good && !good.IsDeleted)
            {
                good.Price = newPrice;
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Product not found");
            }

        }

        public void DeleteProduct(int productId)
        {
            if (_context.Products.Find(productId) is Good good && !good.IsDeleted)
            {
                good.IsDeleted = true;
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public void UpdateProduct(int productId, string name, double price, string barcode, byte[] image)
        {
            if (_context.Products.Find(productId) is Good good && !good.IsDeleted)
            {
                good.Name = name;
                good.Price = price;
                good.BarCode = barcode;
                good.Image = image;
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public List<Good> GetAllProducts()
        {
            return _context.Goods.ToList();
        }

        public Good GetProductById(int productId)
        {
            if (_context.Products.Find(productId) is Good good && !good.IsDeleted)
            {
                return good;
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public Good GetProductByBarCode(string barCode)
        {
            var res = GetAllProducts().Where(x => x.BarCode == barCode && !x.IsDeleted).FirstOrDefault();
            if (res != null)
            {
                return res;
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public Good GetProductByName(string name)
        {
            var res = GetAllProducts().Where(x => x.Name == name && !x.IsDeleted).FirstOrDefault();
            if (res != null)
            {
                return res;
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public List<Good> GetProductsByPrompt(string prompt)
        {
            var res = GetAllProducts()
                .Where(
                x => !x.IsDeleted &&
                (x.Name.Contains(prompt) ||
                x.BarCode.Contains(prompt) ||
                x.Price.ToString().Contains(prompt)))
                .ToList();

            return res ?? new List<Good>();
        }
    }
}
