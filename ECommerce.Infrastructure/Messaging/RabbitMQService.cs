using ECommerce.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ECommerce.Infrastructure.Messaging
{
    public class RabbitMQService : IMessageQueueService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;

        public RabbitMQService(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost",
                UserName = configuration.GetValue<string>("RabbitMQ:Username") ?? "guest",
                Password = configuration.GetValue<string>("RabbitMQ:Password") ?? "guest",
                Port = configuration.GetValue<int?>("RabbitMQ:Port") ?? 5672
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public Task PublishAsync<T>(string queueName, T message)
        {
            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish("", queueName, null, body);

            Console.WriteLine($"📤 Published message to queue '{queueName}'");
            return Task.CompletedTask;
        }

        public void Subscribe<T>(string queueName, Func<T, Task> handler)
        {
            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
                if (message != null)
                    await handler(message);
            };

            _channel.BasicConsume(queueName, autoAck: true, consumer: consumer);
            Console.WriteLine($"📥 Subscribed to queue '{queueName}'");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
