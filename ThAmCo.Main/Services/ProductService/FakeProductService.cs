
using ThAmCo.Main.Models;

namespace ThAmCo.Main.Services.ProductService
{
    public class FakeProductService : IProductService
    {
       private readonly List<Product> _products;

        public FakeProductService()
        {
            // Predefined products
            _products = new List<Product>
            {
                new Product { ProductId = 1, Name = "Laptop", Description = "High-performance laptop", Price = 1200.00m, Stock = 10 },
                new Product { ProductId = 2, Name = "Smartphone", Description = "Latest model smartphone", Price = 800.00m, Stock = 20 },
                new Product { ProductId = 3, Name = "Headphones", Description = "Noise-cancelling headphones", Price = 150.00m, Stock = 15 }
            };
        }

        public Task<Product> GetProductById(int id)
        {
            return Task.FromResult(_products.FirstOrDefault(p => p.ProductId == id));
        }

        public Task<IEnumerable<Product>> GetAllProducts()
        {
            return Task.FromResult(_products.AsEnumerable());
        }

        public Task AddProduct(Product product)
        {
            product.ProductId = _products.Count + 1;
            _products.Add(product);
            return Task.CompletedTask;
        }
    }
}
