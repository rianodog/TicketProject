using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    public interface IOrderWriteDao
    {
        Task CreateOrdersAsync(ICollection<Order> order);
        Task<List<OrderItem>> CreateOrderItemAsync(List<OrderItem> orderItems);
    }
}
