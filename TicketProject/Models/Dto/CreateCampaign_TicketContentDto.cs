using System.ComponentModel.DataAnnotations;
using static TicketProject.Models.Enums;

namespace TicketProject.Models.Dto
{
    public class CreateCampaign_TicketContentDto
    {
        [Required]
        [Range(0, 1, ErrorMessage = "票券類型欄位格式錯誤")]
        public TicketType TypeName { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "票券數量欄位格式錯誤")]
        public int QuantityAvailable { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "票券價格欄位格式錯誤")]
        public decimal Price { get; set; }
    }
}
