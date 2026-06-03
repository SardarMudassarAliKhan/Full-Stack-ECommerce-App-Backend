using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock
            });
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p == null) return null;

            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock
            };
        }

        public async Task<ProductDto> AddAsync(ProductDto dto)
        {
            var entity = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock
            };

            await _repository.AddAsync(entity);
            dto.Id = entity.Id;
            return dto;
        }

        public async Task UpdateAsync(ProductDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null) return;

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.Stock = dto.Stock;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);
    }
}
