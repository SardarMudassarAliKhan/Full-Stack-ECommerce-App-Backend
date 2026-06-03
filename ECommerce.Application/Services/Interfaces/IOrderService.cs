using ECommerce.Application.DTOs;

namespace ECommerce.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task<OrderDto> AddAsync(OrderDto dto);
        Task UpdateAsync(OrderDto dto);
        Task DeleteAsync(Guid id);
    }
}
