using System;

namespace PlantNurseryManagement.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int UserId { get; set; }
        public int PlantId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Plant Plant { get; set; } = null!;
    }
} 