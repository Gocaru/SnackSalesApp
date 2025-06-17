using ApiECommerce.Entities;
using ApiECommerce.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularProducts()
        {
            var _products = await _productRepository.GetPopularProductsAsync();
            return Ok(FormatProducts(_products));
        }


        [HttpGet("bestsellers")]
        public async Task<IActionResult> GetBestSellingProducts()
        {
            var _products = await _productRepository.GetPopularProductsAsync();
            return Ok(FormatProducts(_products));
        }


        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var _products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return Ok(FormatProducts(_products));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            var product = await _productRepository.GetProductDetailsAsync(id);

            if (product is null)
            {
                return NotFound($"Product with id={id} not found");
            }

            var productDetail = new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Details,
                product.UrlImage
            };

            return Ok(productDetail);
        }


        private IEnumerable<object> FormatProducts(IEnumerable<Product> products)
        {
            return products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.UrlImage
            });
        }
    }
}
