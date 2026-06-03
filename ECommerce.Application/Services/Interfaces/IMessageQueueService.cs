namespace ECommerce.Application.Services.Interfaces
{
    public interface IMessageQueueService
    {
        Task PublishAsync<T>(string queueName, T message);
        void Subscribe<T>(string queueName, Func<T, Task> handler);
    }
}
