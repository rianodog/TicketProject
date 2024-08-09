using TicketProject.Models.Entity;

namespace TicketProject.Services.Interfaces
{
    public interface IOrderService
    {
        void CreateOrder(Order order);

        void UpdateOrder(Order order);

        void DeleteOrder(int id);

        IEnumerable<Order> GetAllOrders();

        Order GetOrderById(int id);
    }
}
