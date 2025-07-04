using System;
using System.Collections.Generic;

namespace PlantNurseryManagement.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int PlantId { get; set; }

    public int Quantity { get; set; }

    public DateTime BookingDate { get; set; }

    public string Status { get; set; } = null!;

    public int? AdminId { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Plant Plant { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
