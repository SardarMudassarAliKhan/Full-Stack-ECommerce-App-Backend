using ECommerce.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Domain.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Range(0, double.MaxValue, ErrorMessage = "Total amount cannot be negative.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; private set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Navigation Properties
        public Customer? Customer { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        // Business Logic
        public void CalculateTotal()
        {
            TotalAmount = Items.Sum(i => i.TotalPrice);
        }

        public void MarkAsCompleted() => Status = OrderStatus.Completed;
        public void MarkAsCancelled() => Status = OrderStatus.Cancelled;
    }
}
