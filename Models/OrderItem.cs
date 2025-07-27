using System;

namespace PlantNurseryManagement.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int PlantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        
        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual Plant Plant { get; set; } = null!;
    }
} 