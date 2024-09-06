using System;
using System.Collections.Generic;

namespace TicketProject.Models.Entity;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int TicketContentId { get; set; }

    public int UserId { get; set; }

    public int OrderItemId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual TicketContent TicketContent { get; set; } = null!;

    public virtual OrderItem TicketNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
