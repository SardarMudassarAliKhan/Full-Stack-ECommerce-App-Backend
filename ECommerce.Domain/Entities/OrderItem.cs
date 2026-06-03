using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be non-negative.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        // Navigation Property
        public Order? Order { get; set; }

        // Computed Property (Not mapped to DB)
        [NotMapped]
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
