using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
