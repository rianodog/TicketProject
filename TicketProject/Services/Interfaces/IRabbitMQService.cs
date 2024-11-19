

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TicketProject.Services.Interfaces
{
    public interface IRabbitMQService : IDisposable
    {
        void AddConsumerAsync(string queue, Func<IModel, BasicDeliverEventArgs, Task> func, bool autoAck = true);
        void AddConsumer(string queue, Action<IModel, BasicDeliverEventArgs> func, bool autoAck = true);
        void CleanQueueForDebug();
        Task PublishMessage(string exchange, string message, string routeKey = "");

    }
}
