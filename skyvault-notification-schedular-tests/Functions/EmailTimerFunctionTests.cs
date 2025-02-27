using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using skyvault_notification_schedular.Functions;
using skyvault_notification_schedular.Models;
using skyvault_notification_schedular.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skyvault_notification_schedular_tests.Functions
{
    public class EmailTimerFunctionTests
    {
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly Mock<ILogger<EmailTimerFunction>> _loggerMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ITemplateRepository> _templateRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly EmailTimerFunction _emailTimerFunction;

        public EmailTimerFunctionTests()
        {
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerMock = new Mock<ILogger<EmailTimerFunction>>();
            _loggerFactoryMock.Setup(factory => factory.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);

            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _templateRepositoryMock = new Mock<ITemplateRepository>();
            _emailServiceMock = new Mock<IEmailService>();

            _emailTimerFunction = new EmailTimerFunction(
                _loggerFactoryMock.Object,
                _customerRepositoryMock.Object,
                _templateRepositoryMock.Object,
                _emailServiceMock.Object
            );
        }

        [Fact]
        public async Task RunAsync_Should_LogError_When_ExceptionThrown()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithBirthdayToday()).ThrowsAsync(new Exception("Test exception"));

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), 
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task RunAsync_Should_Execute_All_Tasks()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithBirthdayToday()).ReturnsAsync(new List<Recipient>());
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithPassportExpiryFromSixMonths(It.IsAny<string>())).ReturnsAsync(new List<Recipient>());
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithVisaExpiryFromThreeMonths(It.IsAny<string>())).ReturnsAsync(new List<Recipient>());

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.GetCustomersWithBirthdayToday(), Times.Once);
            _customerRepositoryMock.Verify(repo => repo.GetCustomersWithPassportExpiryFromSixMonths(It.IsAny<string>()), Times.Once);
            _customerRepositoryMock.Verify(repo => repo.GetCustomersWithVisaExpiryFromThreeMonths(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public async Task RunAsync_ShouldLogInformation_WhenNoBirthdayNotifications()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithBirthdayToday()).ReturnsAsync(new List<Recipient>());
            _templateRepositoryMock.Setup(repo => repo.GetEmailContent(NotificationTypeEnum.Birthday)).ReturnsAsync("http://example.com/birthday.jpg");

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No birthday notifications to send today")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
        [Fact]
        public async Task RunAsync_ShouldSendBirthdayNotifications_WhenRecipientsExist()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            var recipients = new List<Recipient> { new Recipient { Name = "John Doe", Email = "john@example.com" } };
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithBirthdayToday()).ReturnsAsync(recipients);
            _templateRepositoryMock.Setup(repo => repo.GetEmailContent(NotificationTypeEnum.Birthday)).ReturnsAsync("http://example.com/birthday.jpg");

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _emailServiceMock.Verify(service => service.SendEmailAsync(recipients, "Greetings from Travel Channel (Private) Limited"), Times.Once);
        }
        [Fact]
        public async Task RunAsync_ShouldLogError_WhenNoBirthdayImageURLFound()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            var recipients = new List<Recipient> { new Recipient { Name = "John Doe", Email = "john@example.com" } };
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithBirthdayToday()).ReturnsAsync(recipients);
            _templateRepositoryMock.Setup(repo => repo.GetEmailContent(NotificationTypeEnum.Birthday)).ReturnsAsync((string)null);

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No birthday image URL found")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
        [Fact]
        public async Task SendPassportExpirationNotification_ShouldLogInformation_WhenNoRecipients()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithPassportExpiryFromSixMonths(It.IsAny<string>())).ReturnsAsync(new List<Recipient>());
            _templateRepositoryMock.Setup(repo => repo.GetEmailContent(NotificationTypeEnum.PassportExpiration)).ReturnsAsync("Passport expiration message");

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No passport expiry notifications to send today")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
        [Fact]
        public async Task SendPassportExpirationNotification_ShouldLogError_WhenNoMessageFound()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            var recipients = new List<Recipient> { new Recipient { Name = "John Doe", Email = "john@example.com" } };
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithPassportExpiryFromSixMonths(It.IsAny<string>())).ReturnsAsync(recipients);
            _templateRepositoryMock.Setup(repo => repo.GetEmailContent(NotificationTypeEnum.PassportExpiration)).ReturnsAsync((string)null);

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No passport expiry message found")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
        [Fact]
        public async Task SendPassportExpirationNotification_ShouldSendEmails_WhenRecipientsExist()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            var recipients = new List<Recipient> { new Recipient { Name = "John Doe", Email = "john@example.com" } };
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithPassportExpiryFromSixMonths(It.IsAny<string>())).ReturnsAsync(recipients);
            _templateRepositoryMock.Setup(repo => repo.GetEmailContent(NotificationTypeEnum.PassportExpiration)).ReturnsAsync("Passport expiration message");

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _emailServiceMock.Verify(service => service.SendEmailAsync(recipients, "Passport Expiry Reminder - Travel Channel (Private) Limited"), Times.Once);
        }
        [Fact]
        public async Task SendVisaExpirationNotification_ShouldLogInformation_WhenNoRecipients()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithVisaExpiryFromThreeMonths(It.IsAny<string>())).ReturnsAsync(new List<Recipient>());
            _templateRepositoryMock.Setup(repo => repo.GetEmailContent(NotificationTypeEnum.VisaExpiration)).ReturnsAsync("Visa expiration message");

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No visa expiry notifications to send today")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
        [Fact]
        public async Task SendVisaExpirationNotification_ShouldLogError_WhenNoMessageFound()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            var recipients = new List<Recipient> { new Recipient { Name = "John Doe", Email = "john@example.com" } };
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithVisaExpiryFromThreeMonths(It.IsAny<string>())).ReturnsAsync(recipients);
            _templateRepositoryMock.Setup(repo => repo.GetEmailContent(NotificationTypeEnum.VisaExpiration)).ReturnsAsync((string)null);

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No visa expiry message found")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
        [Fact]
        public async Task SendVisaExpirationNotification_ShouldLogError_WhenNoCountryNamePlaceholderFound()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            var recipients = new List<Recipient> { new Recipient { Name = "John Doe", Email = "john@example.com" } };
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithVisaExpiryFromThreeMonths(It.IsAny<string>())).ReturnsAsync(recipients);
            _templateRepositoryMock.Setup(repo => repo.GetEmailContent(NotificationTypeEnum.VisaExpiration)).ReturnsAsync("message without the placeholder");

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No country_name found in visa expiry message.")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
        [Fact]
        public async Task SendVisaExpirationNotification_ShouldSendEmails_WhenRecipientsExist()
        {
            // Arrange
            var timerInfo = new TimerInfo();
            var recipients = new List<Recipient> { new Recipient { Name = "John Doe", Email = "john@example.com" } };
            _customerRepositoryMock.Setup(repo => repo.GetCustomersWithVisaExpiryFromThreeMonths(It.IsAny<string>())).ReturnsAsync(recipients);
            _templateRepositoryMock.Setup(repo => repo.GetEmailContent(NotificationTypeEnum.VisaExpiration)).ReturnsAsync("sometext country_name sometext");

            // Act
            await _emailTimerFunction.RunAsync(timerInfo);

            // Assert
            _emailServiceMock.Verify(service => service.SendEmailAsync(recipients, "Visa Expiry Reminder - Travel Channel (Private) Limited"), Times.Once);
        }
    }
}
