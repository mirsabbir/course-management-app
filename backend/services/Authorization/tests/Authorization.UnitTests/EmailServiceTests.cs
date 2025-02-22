using Authorization.Infrastructure.Services;
using MailKit.Net.Smtp;
using MimeKit;
using Moq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Authorization.UnitTests
{
    public class EmailServiceTests
    {
        [Fact]
        public async Task SendEmailAsync_ShouldSendEmail_WithCorrectDetails()
        {
            // Arrange
            var mockSmtpClient = new Mock<ISmtpClient>();

            // Setup the mock behavior to avoid exceptions
            mockSmtpClient.Setup(m => m.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), default))
                          .Returns(Task.CompletedTask);

            mockSmtpClient.Setup(m => m.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                          .Returns(Task.CompletedTask);

            mockSmtpClient.Setup(m => m.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), default))
                          .ReturnsAsync("Message Sent");


            mockSmtpClient.Setup(m => m.DisconnectAsync(It.IsAny<bool>(), default))
                          .Returns(Task.CompletedTask);

            var emailService = new EmailService(mockSmtpClient.Object);

            var recipient = "test@example.com";
            var subject = "Test Subject";
            var message = "Test Message";

            // Act
            await emailService.SendEmailAsync(recipient, subject, message);

            // Assert: Verify that the SMTP client was used correctly
            mockSmtpClient.Verify(m => m.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), default), Times.Once);
            mockSmtpClient.Verify(m => m.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), default), Times.Once);
            mockSmtpClient.Verify(m => m.SendAsync(It.Is<MimeMessage>(msg =>
                msg.To.ToString().Contains(recipient) &&
                msg.Subject == subject &&
                ((TextPart)msg.Body).Text == message
            ), It.IsAny<CancellationToken>(), null), Times.Once);
            mockSmtpClient.Verify(m => m.DisconnectAsync(It.IsAny<bool>(), default), Times.Once);
        }
    }
}
