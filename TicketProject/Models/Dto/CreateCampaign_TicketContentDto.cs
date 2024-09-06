namespace TicketProject.Models.Dto
{
    public class CreateCampaign_TicketContentDto
    {
        public string TypeName { get; set; } = string.Empty;
        public int QuantityAvailable { get; set; }
        public int QuantitySold { get; set; }
        public decimal Price { get; set; }
    }
}
