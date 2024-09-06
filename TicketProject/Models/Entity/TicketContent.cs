using System;
using System.Collections.Generic;

namespace TicketProject.Models.Entity;

public partial class TicketContent
{
    public int TicketContentId { get; set; }

    public int CampaignId { get; set; }

    public string TypeName { get; set; } = null!;

    public int QuantityAvailable { get; set; }

    public int QuantitySold { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
