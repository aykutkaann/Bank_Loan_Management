using BankLoan.Application.Services;
using BankLoan.Domain.Entities;
using BankLoan.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BankLoan.UnitTests.Services
{
    public class CreditScoreServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<ILoanCampaignRepository> _campaignRepoMock;
        private readonly CreditScoreService _service;

        public CreditScoreServiceTests()
        {
            _customerRepoMock = new Mock<ICustomerRepository>();
            _campaignRepoMock = new Mock<ILoanCampaignRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            _uowMock.Setup(u => u.Customers).Returns(_customerRepoMock.Object);
            _uowMock.Setup(u => u.LoanCampaigns).Returns(_campaignRepoMock.Object);

            _service = new CreditScoreService(_uowMock.Object);
        }

        [Fact]
        public async Task GetScoreByCustomerIdAsync_CustomerExists_ShouldReturnCreditScore()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer
            {
                Id = customerId,
                FirstName = "Ali",
                LastName = "Yilmaz",
                NationalId = "10000000001",
                CreditScore = 820,
                MonthlyIncome = 25000,
                PhoneNumber = "5550000000",
                Address = "Istanbul",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _customerRepoMock.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync(customer);

            // Act
            var score = await _service.GetScoreByCustomerIdAsync(customerId);

            // Assert
            score.Should().Be(820);
        }

        [Fact]
        public async Task GetScoreByCustomerIdAsync_CustomerNotFound_ShouldThrowException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerRepoMock.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync((Customer?)null);

            // Act
            var act = async () => await _service.GetScoreByCustomerIdAsync(customerId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*Customer*");
        }

        [Fact]
        public async Task CheckEligibilityAsync_ScoreAboveMinimum_ShouldReturnTrue()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();

            _customerRepoMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(new Customer
                {
                    Id = customerId,
                    CreditScore = 750,
                    FirstName = "Test",
                    LastName = "User",
                    NationalId = "10000000001",
                    PhoneNumber = "555",
                    Address = "Istanbul",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });

            _campaignRepoMock.Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(new LoanCampaign
                {
                    Id = campaignId,
                    Name = "Test",
                    Description = "Test",
                    MinCreditScore = 600,
                    MinLoanAmount = 5000,
                    MaxLoanAmount = 50000,
                    InterestRate = 2.5m,
                    TermMonths = 12,
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddYears(1)
                });

            // Act
            var result = await _service.CheckEligibilityAsync(customerId, campaignId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckEligibilityAsync_ScoreBelowMinimum_ShouldReturnFalse()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();

            _customerRepoMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(new Customer
                {
                    Id = customerId,
                    CreditScore = 400,
                    FirstName = "Test",
                    LastName = "User",
                    NationalId = "10000000001",
                    PhoneNumber = "555",
                    Address = "Istanbul",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });

            _campaignRepoMock.Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(new LoanCampaign
                {
                    Id = campaignId,
                    Name = "Test",
                    Description = "Test",
                    MinCreditScore = 600,
                    MinLoanAmount = 5000,
                    MaxLoanAmount = 50000,
                    InterestRate = 2.5m,
                    TermMonths = 12,
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddYears(1)
                });

            // Act
            var result = await _service.CheckEligibilityAsync(customerId, campaignId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CheckEligibilityAsync_CampaignNotFound_ShouldThrowException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();

            _customerRepoMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(new Customer
                {
                    Id = customerId,
                    CreditScore = 750,
                    FirstName = "Test",
                    LastName = "User",
                    NationalId = "10000000001",
                    PhoneNumber = "555",
                    Address = "Istanbul",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });

            _campaignRepoMock.Setup(r => r.GetByIdAsync(campaignId)).ReturnsAsync((LoanCampaign?)null);

            // Act
            var act = async () => await _service.CheckEligibilityAsync(customerId, campaignId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*Campaign*");
        }
    }
}
