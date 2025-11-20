using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using WebApplication1.Models;
using Microsoft.Extensions.Options;

namespace WebApplication1.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(EmailRequest request)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(new MailboxAddress("", request.ToEmail));
            email.Subject = request.Subject;

            email.Body = new TextPart("plain") { Text = request.Message };

            using var client = new SmtpClient();

            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        }
    }
}
