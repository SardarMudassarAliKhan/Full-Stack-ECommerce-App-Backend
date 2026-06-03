using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository repository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository)
        {
            _repository = repository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _repository.GetAllAsync();
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            });
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var o = await _repository.GetByIdAsync(id);
            if (o == null) return null;

            return new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }

        public async Task<OrderDto> AddAsync(OrderDto dto)
        {
            var entity = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = dto.OrderDate
            };

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null) continue;

                entity.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                });
            }

            entity.CalculateTotal();
            await _repository.AddAsync(entity);

            dto.Id = entity.Id;
            dto.TotalAmount = entity.TotalAmount;
            return dto;
        }

        public async Task UpdateAsync(OrderDto dto)
        {
            var order = await _repository.GetByIdAsync(dto.Id);
            if (order == null) return;

            order.Status = dto.Status;
            await _repository.UpdateAsync(order);
        }

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);
    }
}
