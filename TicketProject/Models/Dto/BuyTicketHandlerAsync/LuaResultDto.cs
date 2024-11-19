using System.Text.Json.Serialization;
using static TicketProject.Models.Enums;

namespace TicketProject.Models.Dto.BuyTicketHandlerAsync
{
    public class LuaResultDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResultStatus Status { get; set; }
        public string Result { get; set; } = string.Empty;
    }
}
