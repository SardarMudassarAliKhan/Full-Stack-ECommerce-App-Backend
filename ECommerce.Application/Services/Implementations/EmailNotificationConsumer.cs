using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ECommerce.Application.Services.Implementations
{
    public class EmailNotificationConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMessageQueueService _queue;

        public EmailNotificationConsumer(IServiceScopeFactory scopeFactory, IMessageQueueService queue)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queue.Subscribe<EmailNotificationDto>("email_notifications", async email =>
            {
                using var scope = _scopeFactory.CreateScope();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();

                Console.WriteLine($"📧 Sending email to: {email.To}");
                await emailSender.SendEmailAsync(email);
            });

            return Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
