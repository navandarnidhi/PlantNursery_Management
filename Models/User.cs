using System;
using System.Collections.Generic;

namespace PlantNurseryManagement.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    // New fields for enhanced user profile
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PinCode { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    
    // New navigation properties for e-commerce functionality
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
