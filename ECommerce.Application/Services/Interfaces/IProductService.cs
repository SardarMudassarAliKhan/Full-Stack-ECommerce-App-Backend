using ECommerce.Application.DTOs;

namespace ECommerce.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<ProductDto> AddAsync(ProductDto dto);
        Task UpdateAsync(ProductDto dto);
        Task DeleteAsync(Guid id);
    }
}
