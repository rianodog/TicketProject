using System;
using System.Collections.Generic;

namespace TicketProject.Models.Entity;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int EventId { get; set; }

    public string TicketType { get; set; } = null!;

    public decimal Price { get; set; }

    public int QuantityAvailable { get; set; }

    public int? QuantitySold { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
