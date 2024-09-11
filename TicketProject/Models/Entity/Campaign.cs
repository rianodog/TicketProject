using System;
using System.Collections.Generic;

namespace TicketProject.Models.Entity;

public partial class Campaign
{
    public int CampaignId { get; set; }

    public string CampaignName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Location { get; set; } = null!;

    public DateTime CampaignDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<TicketContent> TicketContents { get; set; } = new List<TicketContent>();
}
