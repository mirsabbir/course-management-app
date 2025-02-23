using Authorization.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Authorization.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISmtpClient _smtpClient;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpServer = "smtp-relay.brevo.com";
        private readonly int _smtpPort = 587; // Use 465 for SSL
        private readonly string _smtpUsername = "8630e3001@smtp-brevo.com"; // Your Brevo SMTP username
        private readonly string _smtpPassword = "aTRqMb0WwU4kB9yI"; // Your Brevo SMTP password

        public EmailService(
            ISmtpClient smtpClient,
            ILogger<EmailService> logger)
        {
            _smtpClient = smtpClient;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            _logger.LogInformation("Entering SendEmailAsync method. Preparing to send email to: {Email} with subject: {Subject}", email, subject);

            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Auth Server", "mirsabbiralam@gmail.com"));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart("plain") { Text = message };

                _logger.LogInformation("Connecting to SMTP server {SmtpServer}:{SmtpPort}...", _smtpServer, _smtpPort);

                await _smtpClient.ConnectAsync(_smtpServer, _smtpPort, false);
                _logger.LogInformation("Successfully connected to SMTP server {SmtpServer}:{SmtpPort}. Authenticating...", _smtpServer, _smtpPort);

                await _smtpClient.AuthenticateAsync(_smtpUsername, _smtpPassword);
                _logger.LogInformation("Successfully authenticated with SMTP server.");

                _logger.LogInformation("Sending email to {Email} with subject: {Subject}...", email, subject);
                await _smtpClient.SendAsync(emailMessage);

                _logger.LogInformation("Email successfully sent to {Email} with subject: {Subject}.", email, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending email to {Email} with subject: {Subject}.", email, subject);
                throw;  // Optionally rethrow or handle the exception
            }
            finally
            {
                await _smtpClient.DisconnectAsync(true);
                _logger.LogInformation("Disconnected from SMTP server.");
            }
        }

    }
}
