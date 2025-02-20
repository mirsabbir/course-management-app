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
        private readonly string _smtpServer = "smtp-relay.brevo.com";
        private readonly int _smtpPort = 587; // Use 465 for SSL
        private readonly string _smtpUsername = "8630e3001@smtp-brevo.com"; // Your Brevo SMTP username
        private readonly string _smtpPassword = "aTRqMb0WwU4kB9yI"; // Your Brevo SMTP password

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            // Set the sender and recipient
            emailMessage.From.Add(new MailboxAddress("Auth Server", "mirsabbiralam@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));

            // Set the subject and body
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            // Send the email
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, false);
                await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
