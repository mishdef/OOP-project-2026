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
            if (_context.Products.Find(productId) is Good good)
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
            if (_context.Products.Find(productId) is Good good)
            {
                _context.Products.Remove(good);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public void UpdateProduct(int productId, string name, double price, string barcode, byte[] image)
        {
            if (_context.Products.Find(productId) is Good good)
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
            return _context.Products.Select(x => x as Good).ToList();
        }

        public Good GetProductById(int productId)
        {
            if (_context.Products.Find(productId) is Good good)
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
            var res = GetAllProducts().Where(x => x.BarCode == barCode).FirstOrDefault();
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
            var res = GetAllProducts().Where(x => x.Name == name).FirstOrDefault();
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
                x =>
                x.Name.Contains(prompt) ||
                x.BarCode.Contains(prompt) ||
                x.Price.ToString().Contains(prompt) ||
                x.BarCode.Contains(prompt))
                .ToList();

            if (res != null)
            {
                return res;
            }
            else
            {
                return new List<Good>();
            }
        }
    }
}
