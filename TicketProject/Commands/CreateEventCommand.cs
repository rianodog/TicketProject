using MediatR;
using System.ComponentModel.DataAnnotations;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;

namespace TicketProject.Commands
{
    public class CreateEventCommand : IRequest<CreateEventCommand>
    {
        [Required]
        public string? EventName { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Location { get; set; }
        [Required]
        public DateTime EventDate { get; set; }
        [Required]
        public List<CreateTicketDto> Tickets { get; set; } = [];
    }
}
