using System;
using System.Collections.Generic;

namespace TicketProject.Models.Entity;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int IsAdmin { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
