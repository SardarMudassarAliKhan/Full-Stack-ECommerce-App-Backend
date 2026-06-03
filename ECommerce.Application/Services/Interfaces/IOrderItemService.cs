using ECommerce.Application.DTOs;

namespace ECommerce.Application.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<OrderItemDto>> GetAllAsync();
    }
}
