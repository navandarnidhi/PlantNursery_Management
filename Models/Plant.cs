using System;
using System.Collections.Generic;

namespace PlantNurseryManagement.Models;

public partial class Plant
{
    public int PlantId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public int QuantityAvailable { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
