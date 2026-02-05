using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EcommerceWebsite.Services
{
    /// <summary>
    /// Email service implementation
    /// For production, integrate with SendGrid, MailKit, or SMTP
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            // For development: Log emails instead of sending
            // In production: Integrate with email provider (SendGrid, MailKit, etc.)
            
            _logger.LogInformation($"Sending email to: {toEmail}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Body: {body}");

            // TODO: Implement actual email sending in production
            // Example with MailKit:
            /*
            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["Email:SmtpServer"], 
                int.Parse(_configuration["Email:Port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_configuration["Email:Username"], 
                _configuration["Email:Password"]);
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TechMart", _configuration["Email:FromEmail"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };
            
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            */

            await Task.CompletedTask;
        }

        public async Task SendEmailConfirmationAsync(string toEmail, string confirmationLink)
        {
            var subject = "Confirm Your Email - TechMart";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #2563eb;'>Welcome to TechMart!</h2>
                        <p>Thank you for registering. Please confirm your email address by clicking the button below:</p>
                        <p style='margin: 30px 0;'>
                            <a href='{confirmationLink}' 
                               style='background-color: #2563eb; color: white; padding: 12px 24px; 
                                      text-decoration: none; border-radius: 4px; display: inline-block;'>
                                Confirm Email Address
                            </a>
                        </p>
                        <p>Or copy and paste this link into your browser:</p>
                        <p style='color: #666; font-size: 14px;'>{confirmationLink}</p>
                        <p style='margin-top: 30px; color: #666; font-size: 12px;'>
                            If you didn't create an account, please ignore this email.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetAsync(string toEmail, string resetLink)
        {
            var subject = "Reset Your Password - TechMart";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #2563eb;'>Password Reset Request</h2>
                        <p>We received a request to reset your password. Click the button below to create a new password:</p>
                        <p style='margin: 30px 0;'>
                            <a href='{resetLink}' 
                               style='background-color: #2563eb; color: white; padding: 12px 24px; 
                                      text-decoration: none; border-radius: 4px; display: inline-block;'>
                                Reset Password
                            </a>
                        </p>
                        <p>Or copy and paste this link into your browser:</p>
                        <p style='color: #666; font-size: 14px;'>{resetLink}</p>
                        <p style='margin-top: 30px; color: #666; font-size: 12px;'>
                            This link will expire in 24 hours. If you didn't request a password reset, please ignore this email.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Welcome to TechMart!";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #2563eb;'>Welcome, {userName}!</h2>
                        <p>Your account has been successfully verified. You can now:</p>
                        <ul>
                            <li>Browse thousands of products</li>
                            <li>Create your own listings</li>
                            <li>Track your orders</li>
                            <li>Manage your profile</li>
                        </ul>
                        <p style='margin: 30px 0;'>
                            <a href='https://localhost:5001' 
                               style='background-color: #2563eb; color: white; padding: 12px 24px; 
                                      text-decoration: none; border-radius: 4px; display: inline-block;'>
                                Start Shopping
                            </a>
                        </p>
                        <p style='margin-top: 30px; color: #666; font-size: 14px;'>
                            If you have any questions, please contact our support team.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
