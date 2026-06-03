using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace ECommerce.Infrastructure.Email
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _config;

        public EmailSenderService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(EmailNotificationDto email)
        {
            var smtp = _config.GetSection("SmtpSettings");

            using var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"] ?? "587"))
            {
                Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                EnableSsl = bool.Parse(smtp["EnableSsl"] ?? "true")
            };

            var message = new MailMessage
            {
                From = new MailAddress(smtp["From"] ?? smtp["Username"]),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };
            message.To.Add(email.To);

            await client.SendMailAsync(message);
            Console.WriteLine($"📨 Email sent to {email.To}");
        }
    }
}
