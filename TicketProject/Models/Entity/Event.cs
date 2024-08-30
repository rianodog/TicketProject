using System;
using System.Collections.Generic;

namespace TicketProject.Models.Entity;

public partial class Event
{
    public int EventId { get; set; }

    public string? EventName { get; set; }

    public string? Description { get; set; }

    public string? Location { get; set; }

    public DateTime? EventDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
