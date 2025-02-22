using Authorization.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISmtpClient _smtpClient;
        private readonly string _smtpServer = "smtp-relay.brevo.com";
        private readonly int _smtpPort = 587; // Use 465 for SSL
        private readonly string _smtpUsername = "8630e3001@smtp-brevo.com"; // Your Brevo SMTP username
        private readonly string _smtpPassword = "aTRqMb0WwU4kB9yI"; // Your Brevo SMTP password

        public EmailService(ISmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Auth Server", "mirsabbiralam@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            await _smtpClient.ConnectAsync(_smtpServer, _smtpPort, false);
            await _smtpClient.AuthenticateAsync(_smtpUsername, _smtpPassword);
            await _smtpClient.SendAsync(emailMessage);
            await _smtpClient.DisconnectAsync(true);
        }
    }
}
