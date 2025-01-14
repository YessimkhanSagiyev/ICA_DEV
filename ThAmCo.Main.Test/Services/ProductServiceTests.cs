using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ThAmCo.Main.Models;
using ThAmCo.Main.Services.ProductService;
using Microsoft.Extensions.Configuration;

namespace ThAmCo.Main.Test.Services
{
    [TestClass]
    public class ProductServiceTests
    {
        private Mock<HttpMessageHandler> _handlerMock;
        private Mock<IConfiguration> _configurationMock;
        private ProductService _productService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Mock HttpMessageHandler
            _handlerMock = new Mock<HttpMessageHandler>();

            _handlerMock
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

            // Mock IConfiguration
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock
                .Setup(c => c["WebServices:UnderCutters:BaseURL"])
                .Returns("http://undercutters.azurewebsites.net/");

            // Create HttpClient with mocked handler
            var httpClient = new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new Uri("http://undercutters.azurewebsites.net/")
            };

            // Initialize ProductService
            _productService = new ProductService(httpClient, _configurationMock.Object);
        }

        [TestMethod]
        public async Task GetAllProducts_ReturnsProducts()
        {
            // Act
            var products = await _productService.GetAllProducts();
            var productList = products.ToList();

            // Assert
            Assert.IsNotNull(productList);
            Assert.AreEqual(2, productList.Count);
            Assert.AreEqual("Laptop", productList[0].Name);
        }

        [TestMethod]
        public async Task GetAllProducts_ReturnsEmpty_WhenNoProductsAvailable()
        {
            // Arrange: Mock a response with no products
            _handlerMock
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

            // Act
            var products = await _productService.GetAllProducts();
            var productList = products.ToList();

            // Assert
            Assert.IsNotNull(productList);
            Assert.AreEqual(0, productList.Count);
        }

        [TestMethod]
        public async Task GetAllProducts_ThrowsException_OnServerError()
        {
            // Arrange: Mock a server error
            _handlerMock
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
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException());

            // Act & Assert
            await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
            {
                await _productService.GetAllProducts();
            });
        }
    }
}
