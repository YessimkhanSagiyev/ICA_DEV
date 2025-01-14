using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ThAmCo.Main.Models;

namespace ThAmCo.Main.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient client, IConfiguration configuration)
        {
            // Retrieve the base URL from the configuration and ensure it's set
            var baseUrl = configuration["WebServices:UnderCutters:BaseURL"] ?? throw new ArgumentNullException(nameof(configuration), "Base URL is not configured.");
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient = client;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            const string uri = "api/product"; // Use a consistent API endpoint
            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch products. Status Code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Product>>(content) 
                ?? throw new InvalidOperationException("Failed to deserialize the product data.");
        }

        public async Task<Product> GetProductById(int id)
        {
            var response = await _httpClient.GetAsync($"api/product/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch the product with ID {id}. Status Code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Product>(content) 
                ?? throw new InvalidOperationException("Failed to deserialize the product data.");
        }

        public async Task AddProduct(Product product)
        {
            var content = new StringContent(JsonSerializer.Serialize(product), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/product", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to add the product. Status Code: {response.StatusCode}");
            }
        }
    }
}
