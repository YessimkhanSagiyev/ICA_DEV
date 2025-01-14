using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ThAmCo.Main.Models;
using ThAmCo.Main.Services.ProductService;

namespace ThAmCo.Main.Test.Services
{
    [TestClass]
    public class ProductServiceTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private ProductService _productService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Mock HttpMessageHandler
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<Product>
                    {
                        new Product { ProductId = 1, Name = "Laptop", Description = "High-performance laptop", Price = 1200.00m, Stock = 10 },
                        new Product { ProductId = 2, Name = "Smartphone", Description = "Latest model smartphone", Price = 800.00m, Stock = 20 }
                    }))
                });

            // Create a mocked HttpClient using the handlerMock
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.example.com/")
            };

            // Mock IHttpClientFactory
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            // Create an instance of ProductService
            _productService = new ProductService(_httpClientFactoryMock.Object);
        }

        [TestMethod]
        public async Task GetAllProducts_ReturnsProducts()
        {
            // Act
            var products = await _productService.GetAllProducts();
            var productList = products.ToList(); // Convert to List for Count property

            // Assert
            Assert.IsNotNull(productList);
            Assert.AreEqual(2, productList.Count); // Check that 2 products are returned
            Assert.AreEqual("Laptop", productList[0].Name); // Check the first product's name
        }

        [TestMethod]
        public async Task GetAllProducts_ReturnsEmpty_WhenNoProductsAvailable()
        {
            // Arrange: Mock a response with no products
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            _productService = new ProductService(_httpClientFactoryMock.Object);

            // Act
            var products = await _productService.GetAllProducts();
            var productList = products.ToList(); // Convert to List for Count property

            // Assert
            Assert.IsNotNull(productList); // Ensure it's not null
            Assert.AreEqual(0, productList.Count); // Check that no products are returned
        }

        [TestMethod]
        public async Task GetAllProducts_ThrowsException_OnServerError()
        {
            // Arrange: Mock a server error
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(handlerMock.Object);
            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            _productService = new ProductService(_httpClientFactoryMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
            {
                await _productService.GetAllProducts();
            });
        }

        [TestMethod]
        public async Task GetAllProducts_HandlesTimeout()
        {
            // Arrange: Mock a timeout scenario
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException()); // Simulate timeout

            var httpClient = new HttpClient(handlerMock.Object);
            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            _productService = new ProductService(_httpClientFactoryMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
            {
                await _productService.GetAllProducts();
            });
        }
    }
}
