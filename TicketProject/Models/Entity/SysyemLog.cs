using System;
using System.Collections.Generic;

namespace TicketProject.Models.Entity;

public partial class SysyemLog
{
    public int Id { get; set; }

    public string? Level { get; set; }

    public string? FunctionName { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }
}
