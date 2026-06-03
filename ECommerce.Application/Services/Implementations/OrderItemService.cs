using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.Services.Implementations
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _repository;

        public OrderItemService(IOrderItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(Guid orderId)
        {
            var items = await _repository.GetByOrderIdAsync(orderId);
            return items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                OrderId = i.OrderId,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            });
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                OrderId = i.OrderId,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            });
        }
    }

}
