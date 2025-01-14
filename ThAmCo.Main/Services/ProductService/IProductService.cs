using ThAmCo.Main.Models;

namespace ThAmCo.Main.Services.ProductService
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
        Task AddProduct(Product product);
    }
}
