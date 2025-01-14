using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThAmCo.Main.DTOs;
using ThAmCo.Main.Models;
using ThAmCo.Main.Services.ProductService;

namespace ThAmCo.Main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();

            var response = products.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null)
                return NotFound();

            var response = new ProductResponseDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProduct([FromBody] ProductResponseDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock
            };

            await _productService.AddProduct(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }
    }
}
