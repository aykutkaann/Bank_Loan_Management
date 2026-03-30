using BankLoan.Application.Common;
using BankLoan.Application.Services;
using BankLoan.Domain.Entities;
using BankLoan.Domain.Enums;
using BankLoan.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BankLoan.UnitTests.Services
{
    public class LoanApplicationServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILoanEligibilityService> _eligibilityMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<ILoanCampaignRepository> _campaignRepoMock;
        private readonly Mock<ILoanApplicationRepository> _applicationRepoMock;
        private readonly LoanApplicationService _service;

        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _campaignId = Guid.NewGuid();

        public LoanApplicationServiceTests()
        {
            _customerRepoMock = new Mock<ICustomerRepository>();
            _campaignRepoMock = new Mock<ILoanCampaignRepository>();
            _applicationRepoMock = new Mock<ILoanApplicationRepository>();
            _eligibilityMock = new Mock<ILoanEligibilityService>();
            _uowMock = new Mock<IUnitOfWork>();

            _uowMock.Setup(u => u.Customers).Returns(_customerRepoMock.Object);
            _uowMock.Setup(u => u.LoanCampaigns).Returns(_campaignRepoMock.Object);
            _uowMock.Setup(u => u.LoanApplications).Returns(_applicationRepoMock.Object);

            _service = new LoanApplicationService(_uowMock.Object, _eligibilityMock.Object);
        }

        [Fact]
        public async Task ApplyAsync_Eligible_ShouldCreateApplicationAndSave()
        {
            // Arrange
            _eligibilityMock.Setup(e => e.EvaluateAsync(_customerId, _campaignId, 20000))
                .ReturnsAsync(new EligibilityResult { IsEligible = true, Reasons = new() });

            _campaignRepoMock.Setup(r => r.GetByIdAsync(_campaignId))
                .ReturnsAsync(new LoanCampaign
                {
                    Id = _campaignId,
                    Name = "Test",
                    Description = "Test",
                    InterestRate = 2.5m,
                    TermMonths = 12,
                    MinCreditScore = 600,
                    MinLoanAmount = 5000,
                    MaxLoanAmount = 50000,
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddYears(1)
                });

            _customerRepoMock.Setup(r => r.GetByIdAsync(_customerId))
                .ReturnsAsync(new Customer
                {
                    Id = _customerId,
                    CreditScore = 750,
                    FirstName = "Ali",
                    LastName = "Yilmaz",
                    NationalId = "10000000001",
                    PhoneNumber = "555",
                    Address = "Istanbul",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });

            // Act
            var result = await _service.ApplyAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeTrue();
            _applicationRepoMock.Verify(r => r.AddAsync(It.IsAny<LoanApplication>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ApplyAsync_NotEligible_ShouldNotCreateApplication()
        {
            // Arrange
            _eligibilityMock.Setup(e => e.EvaluateAsync(_customerId, _campaignId, 20000))
                .ReturnsAsync(new EligibilityResult
                {
                    IsEligible = false,
                    Reasons = new List<string> { "Credit score too low" }
                });

            // Act
            var result = await _service.ApplyAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain("Credit score too low");
            _applicationRepoMock.Verify(r => r.AddAsync(It.IsAny<LoanApplication>()), Times.Never);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ApplyAsync_Eligible_ShouldSnapshotCreditScore()
        {
            // Arrange
            LoanApplication? savedApp = null;

            _eligibilityMock.Setup(e => e.EvaluateAsync(_customerId, _campaignId, 20000))
                .ReturnsAsync(new EligibilityResult { IsEligible = true, Reasons = new() });

            _campaignRepoMock.Setup(r => r.GetByIdAsync(_campaignId))
                .ReturnsAsync(new LoanCampaign
                {
                    Id = _campaignId,
                    Name = "Test",
                    Description = "Test",
                    InterestRate = 2.5m,
                    TermMonths = 12,
                    MinCreditScore = 600,
                    MinLoanAmount = 5000,
                    MaxLoanAmount = 50000,
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddYears(1)
                });

            _customerRepoMock.Setup(r => r.GetByIdAsync(_customerId))
                .ReturnsAsync(new Customer
                {
                    Id = _customerId,
                    CreditScore = 820,
                    FirstName = "Ali",
                    LastName = "Yilmaz",
                    NationalId = "10000000001",
                    PhoneNumber = "555",
                    Address = "Istanbul",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });

            _applicationRepoMock.Setup(r => r.AddAsync(It.IsAny<LoanApplication>()))
                .Callback<LoanApplication>(app => savedApp = app);

            // Act
            await _service.ApplyAsync(_customerId, _campaignId, 20000);

            // Assert
            savedApp.Should().NotBeNull();
            savedApp!.CreditScoreAtApply.Should().Be(820);
            savedApp.Status.Should().Be(LoanStatus.Pending);
            savedApp.CustomerId.Should().Be(_customerId);
            savedApp.CampaignId.Should().Be(_campaignId);
            savedApp.RequestedAmount.Should().Be(20000);
        }

        [Fact]
        public async Task ApplyAsync_Eligible_ShouldCalculateMonthlyInstallment()
        {
            // Arrange
            LoanApplication? savedApp = null;

            _eligibilityMock.Setup(e => e.EvaluateAsync(_customerId, _campaignId, 10000))
                .ReturnsAsync(new EligibilityResult { IsEligible = true, Reasons = new() });

            _campaignRepoMock.Setup(r => r.GetByIdAsync(_campaignId))
                .ReturnsAsync(new LoanCampaign
                {
                    Id = _campaignId,
                    Name = "Test",
                    Description = "Test",
                    InterestRate = 2.0m,  // 2%
                    TermMonths = 12,
                    MinCreditScore = 600,
                    MinLoanAmount = 5000,
                    MaxLoanAmount = 50000,
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddYears(1)
                });

            _customerRepoMock.Setup(r => r.GetByIdAsync(_customerId))
                .ReturnsAsync(new Customer
                {
                    Id = _customerId,
                    CreditScore = 750,
                    FirstName = "Test",
                    LastName = "User",
                    NationalId = "10000000001",
                    PhoneNumber = "555",
                    Address = "Istanbul",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });

            _applicationRepoMock.Setup(r => r.AddAsync(It.IsAny<LoanApplication>()))
                .Callback<LoanApplication>(app => savedApp = app);

            // Act
            await _service.ApplyAsync(_customerId, _campaignId, 10000);

            // Assert — 10000 * (1 + 2/100) / 12 = 10200 / 12 = 850
            savedApp.Should().NotBeNull();
            savedApp!.CalculatedMonthly.Should().Be(850m);
        }

        [Fact]
        public async Task GetByIdAsync_Exists_ShouldReturnApplication()
        {
            // Arrange
            var appId = Guid.NewGuid();
            var app = new LoanApplication { Id = appId, CustomerId = _customerId, CampaignId = _campaignId };
            _applicationRepoMock.Setup(r => r.GetByIdAsync(appId)).ReturnsAsync(app);

            // Act
            var result = await _service.GetByIdAsync(appId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(appId);
        }

        [Fact]
        public async Task GetByIdAsync_NotExists_ShouldReturnNull()
        {
            // Arrange
            var appId = Guid.NewGuid();
            _applicationRepoMock.Setup(r => r.GetByIdAsync(appId)).ReturnsAsync((LoanApplication?)null);

            // Act
            var result = await _service.GetByIdAsync(appId);

            // Assert
            result.Should().BeNull();
        }
    }
}
