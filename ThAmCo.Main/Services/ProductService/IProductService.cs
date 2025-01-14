using ThAmCo.Main.Models;

namespace ThAmCo.Main.Services.ProductService
{
    public interface IProductService
    {
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> GetAllProducts();
        Task AddProduct(Product product);
    }
}
