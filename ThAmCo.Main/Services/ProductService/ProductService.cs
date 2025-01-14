using System.Net.Http;
using System.Text.Json;
using ThAmCo.Main.Models;
using Microsoft.EntityFrameworkCore;
using Polly;


namespace ThAmCo.Main.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ThirdPartyAPI");
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt))
                .ExecuteAsync(async () =>
                {
                    var response = await _httpClient.GetAsync("products");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<IEnumerable<Product>>(content);
                });
        }

        public Task<Product> GetProductById(int id)
        {
            // Example implementation
            throw new NotImplementedException();
        }

        public Task AddProduct(Product product)
        {
            // Example implementation
            throw new NotImplementedException();
        }
    }
}
