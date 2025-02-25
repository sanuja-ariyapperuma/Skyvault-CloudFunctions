using Dapper;
using Moq;
using skyvault_notification_schedular.Models;
using skyvault_notification_schedular.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skyvault_notification_schedular_tests.Services
{
    public class CustomerRepositoryTests
    {
        private readonly Mock<IDataAccess> _mockDataAccess;
        private readonly CustomerRepository _repository;

        public CustomerRepositoryTests()
        {
            _mockDataAccess = new Mock<IDataAccess>();
            _repository = new CustomerRepository(_mockDataAccess.Object);
        }

        [Fact]
        public async Task GetCustomersWithBirthdayToday_ReturnsExpectedResults()
        {
            // Arrange
            var expectedRecipients = new List<Recipient>
                {
                    new Recipient { Name = "Mr. John Doe", Email = "john.doe@example.com" }
                };

            
            _mockDataAccess.Setup(c => c.QueryAsync<Recipient>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(expectedRecipients);

            // Act
            var result = await _repository.GetCustomersWithBirthdayToday();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedRecipients.First().Name, result.First().Name);
            Assert.Equal(expectedRecipients.First().Email, result.First().Email);
        }

        [Fact]
        public async Task GetCustomersWithPassportExpiryFromSixMonths_ReturnsExpectedResults()
        {
            // Arrange
            var date = "2023-12-01";
            var expectedRecipients = new List<Recipient>
                    {
                        new Recipient { Name = "Mr. Jane Doe", Email = "jane.doe@example.com" }
                    };

            _mockDataAccess.Setup(c => c.QueryAsync<Recipient>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(expectedRecipients);

            // Act
            var result = await _repository.GetCustomersWithPassportExpiryFromSixMonths(date);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedRecipients.First().Name, result.First().Name);
            Assert.Equal(expectedRecipients.First().Email, result.First().Email);
        }

        [Fact]
        public async Task GetCustomersWithVisaExpiryFromThreeMonths_ReturnsExpectedResults()
        {
            // Arrange
            var date = "2023-12-01";
            var expectedRecipients = new List<Recipient>
                    {
                        new Recipient { Name = "Mr. Alex Smith", Email = "alex.smith@example.com" }
                    };

            _mockDataAccess.Setup(c => c.QueryAsync<Recipient>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(expectedRecipients);

            // Act
            var result = await _repository.GetCustomersWithVisaExpiryFromThreeMonths(date);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedRecipients.First().Name, result.First().Name);
            Assert.Equal(expectedRecipients.First().Email, result.First().Email);
        }
    }
}
