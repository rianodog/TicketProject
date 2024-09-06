using System;
using System.Collections.Generic;

namespace TicketProject.Models.Entity;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int TicketContentId { get; set; }

    public int Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Ticket? Ticket { get; set; }

    public virtual TicketContent TicketContent { get; set; } = null!;
}
