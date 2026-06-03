using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace ECommerce.Infrastructure.BackgroundServices
{
    public class EmailNotificationConsumer : BackgroundService
    {
        private readonly IMessageQueueService _queue;
        private readonly IEmailSenderService _emailSender;

        public EmailNotificationConsumer(IMessageQueueService queue, IEmailSenderService emailSender)
        {
            _queue = queue;
            _emailSender = emailSender;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queue.Subscribe<EmailNotificationDto>("email_notifications_queue", async email =>
            {
                Console.WriteLine($"📧 Sending email to: {email.To}");
                await _emailSender.SendEmailAsync(email);
            });

            return Task.CompletedTask;
        }
    }

}
