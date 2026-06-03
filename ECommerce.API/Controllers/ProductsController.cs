using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICacheService _cacheService;

        public ProductsController(IProductService productService, ICacheService cacheService)
        {
            _productService = productService;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            const string cacheKey = "products:all";

            // Try get from cache
            var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey);
            if (cachedProducts != null)
            {
                return Ok(new
                {
                    fromCache = true,
                    data = cachedProducts
                });
            }

            // Get from DB if not cached
            var products = await _productService.GetAllAsync();

            // Cache for 60 minutes
            await _cacheService.SetAsync(cacheKey, products, TimeSpan.FromMinutes(60));

            return Ok(new
            {
                fromCache = false,
                data = products
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var cacheKey = $"product:{id}";

            // Check cache
            var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);
            if (cachedProduct != null)
            {
                return Ok(new
                {
                    fromCache = true,
                    data = cachedProduct
                });
            }

            // Fetch from service
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            // Cache for 60 minutes
            await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(60));

            return Ok(new
            {
                fromCache = false,
                data = product
            });
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductDto dto)
        {
            await _productService.AddAsync(dto);

            // Invalidate cache
            await _cacheService.RemoveAsync("products:all");

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, ProductDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Mismatched product ID.");

            await _productService.UpdateAsync(dto);

            // Invalidate caches
            await _cacheService.RemoveAsync("products:all");
            await _cacheService.RemoveAsync($"product:{id}");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);

            // Invalidate caches
            await _cacheService.RemoveAsync("products:all");
            await _cacheService.RemoveAsync($"product:{id}");

            return NoContent();
        }
    }
}
