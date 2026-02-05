using System.Threading.Tasks;

namespace EcommerceWebsite.Services
{
    /// <summary>
    /// Email service interface for sending notifications
    /// </summary>
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task SendEmailConfirmationAsync(string toEmail, string confirmationLink);
        Task SendPasswordResetAsync(string toEmail, string resetLink);
        Task SendWelcomeEmailAsync(string toEmail, string userName);
    }
}
