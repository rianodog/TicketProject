using System;
using System.Collections.Generic;

namespace TicketProject.Models.Entity;

public partial class SystemLog
{
    public int Id { get; set; }

    public string? Level { get; set; }

    public string? FunctionName { get; set; }

    public string? Message { get; set; }
}
