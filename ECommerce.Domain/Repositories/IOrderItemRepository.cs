using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Repositories
{
    public interface IOrderItemRepository
    {
        Task<OrderItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task AddAsync(OrderItem orderItem);
        Task UpdateAsync(OrderItem orderItem);
        Task DeleteAsync(Guid id);

        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId);
    }
}
