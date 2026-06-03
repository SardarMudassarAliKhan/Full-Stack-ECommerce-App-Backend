using ECommerce.Application.DTOs;

namespace ECommerce.Application.Services.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(EmailNotificationDto email);
    }
}
