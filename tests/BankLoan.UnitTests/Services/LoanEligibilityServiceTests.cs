using BankLoan.Application.Services;
using BankLoan.Domain.Entities;
using BankLoan.Domain.Enums;
using BankLoan.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BankLoan.UnitTests.Services
{
    public class LoanEligibilityServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<ILoanCampaignRepository> _campaignRepoMock;
        private readonly Mock<ILoanApplicationRepository> _applicationRepoMock;
        private readonly LoanEligibilityService _service;

        // Test data
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _campaignId = Guid.NewGuid();

        public LoanEligibilityServiceTests()
        {
            _customerRepoMock = new Mock<ICustomerRepository>();
            _campaignRepoMock = new Mock<ILoanCampaignRepository>();
            _applicationRepoMock = new Mock<ILoanApplicationRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            _uowMock.Setup(u => u.Customers).Returns(_customerRepoMock.Object);
            _uowMock.Setup(u => u.LoanCampaigns).Returns(_campaignRepoMock.Object);
            _uowMock.Setup(u => u.LoanApplications).Returns(_applicationRepoMock.Object);

            _service = new LoanEligibilityService(_uowMock.Object);
        }

        private Customer CreateCustomer(int creditScore = 750, decimal income = 20000m)
        {
            return new Customer
            {
                Id = _customerId,
                FirstName = "Test",
                LastName = "User",
                NationalId = "10000000001",
                CreditScore = creditScore,
                MonthlyIncome = income,
                PhoneNumber = "5550000000",
                Address = "Istanbul",
                DateOfBirth = new DateOnly(1990, 1, 1),
                CreatedAt = DateTime.UtcNow
            };
        }

        private LoanCampaign CreateCampaign(int minScore = 600, decimal minAmount = 5000, decimal maxAmount = 50000)
        {
            return new LoanCampaign
            {
                Id = _campaignId,
                Name = "Test Campaign",
                Description = "Test",
                InterestRate = 2.5m,
                MinCreditScore = minScore,
                MinLoanAmount = minAmount,
                MaxLoanAmount = maxAmount,
                TermMonths = 12,
                PaymentFrequency = PaymentFrequency.Monthly,
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddYears(1),
                CreatedAt = DateTime.UtcNow
            };
        }

        private void SetupMocks(Customer customer, LoanCampaign campaign, List<LoanApplication>? existingApps = null)
        {
            _customerRepoMock.Setup(r => r.GetByIdAsync(_customerId)).ReturnsAsync(customer);
            _campaignRepoMock.Setup(r => r.GetByIdAsync(_campaignId)).ReturnsAsync(campaign);
            _applicationRepoMock.Setup(r => r.GetByCustomerIdAsync(_customerId))
                .ReturnsAsync(existingApps ?? new List<LoanApplication>());
        }

        // ========== ELIGIBLE SCENARIOS ==========

        [Fact]
        public async Task EvaluateAsync_AllRulesPass_ShouldBeEligible()
        {
            // Arrange
            var customer = CreateCustomer(creditScore: 750);
            var campaign = CreateCampaign(minScore: 600, minAmount: 5000, maxAmount: 50000);
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Fact]
        public async Task EvaluateAsync_CreditScoreExactlyAtMinimum_ShouldBeEligible()
        {
            // Arrange
            var customer = CreateCustomer(creditScore: 600);
            var campaign = CreateCampaign(minScore: 600);
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeTrue();
        }

        [Fact]
        public async Task EvaluateAsync_AmountAtMinBoundary_ShouldBeEligible()
        {
            // Arrange
            var customer = CreateCustomer(creditScore: 750);
            var campaign = CreateCampaign(minAmount: 5000, maxAmount: 50000);
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 5000);

            // Assert
            result.IsEligible.Should().BeTrue();
        }

        [Fact]
        public async Task EvaluateAsync_AmountAtMaxBoundary_ShouldBeEligible()
        {
            // Arrange
            var customer = CreateCustomer(creditScore: 750);
            var campaign = CreateCampaign(minAmount: 5000, maxAmount: 50000);
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 50000);

            // Assert
            result.IsEligible.Should().BeTrue();
        }

        // ========== REJECTION SCENARIOS ==========

        [Fact]
        public async Task EvaluateAsync_CustomerNotFound_ShouldNotBeEligible()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.GetByIdAsync(_customerId)).ReturnsAsync((Customer?)null);
            _campaignRepoMock.Setup(r => r.GetByIdAsync(_campaignId)).ReturnsAsync(CreateCampaign());

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain("Customer not found.");
        }

        [Fact]
        public async Task EvaluateAsync_CampaignNotActive_ShouldNotBeEligible()
        {
            // Arrange
            var customer = CreateCustomer();
            var campaign = CreateCampaign();
            campaign.IsActive = false;
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain("Campaign is not active or has expired");
        }

        [Fact]
        public async Task EvaluateAsync_CampaignExpired_ShouldNotBeEligible()
        {
            // Arrange
            var customer = CreateCustomer();
            var campaign = CreateCampaign();
            campaign.EndDate = DateTime.UtcNow.AddDays(-1); // expired yesterday
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain("Campaign is not active or has expired");
        }

        [Fact]
        public async Task EvaluateAsync_CampaignNotStartedYet_ShouldNotBeEligible()
        {
            // Arrange
            var customer = CreateCustomer();
            var campaign = CreateCampaign();
            campaign.StartDate = DateTime.UtcNow.AddDays(5); // starts in 5 days
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain("Campaign is not active or has expired");
        }

        [Fact]
        public async Task EvaluateAsync_AmountBelowMinimum_ShouldNotBeEligible()
        {
            // Arrange
            var customer = CreateCustomer();
            var campaign = CreateCampaign(minAmount: 5000, maxAmount: 50000);
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 1000);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain(r => r.Contains("Requested amount must be between"));
        }

        [Fact]
        public async Task EvaluateAsync_AmountAboveMaximum_ShouldNotBeEligible()
        {
            // Arrange
            var customer = CreateCustomer();
            var campaign = CreateCampaign(minAmount: 5000, maxAmount: 50000);
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 100000);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain(r => r.Contains("Requested amount must be between"));
        }

        [Fact]
        public async Task EvaluateAsync_CreditScoreBelowMinimum_ShouldNotBeEligible()
        {
            // Arrange
            var customer = CreateCustomer(creditScore: 400);
            var campaign = CreateCampaign(minScore: 600);
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain(r => r.Contains("Credit Score"));
        }

        [Fact]
        public async Task EvaluateAsync_HasPendingApplicationForSameCampaign_ShouldNotBeEligible()
        {
            // Arrange
            var customer = CreateCustomer();
            var campaign = CreateCampaign();
            var existingApps = new List<LoanApplication>
            {
                new LoanApplication
                {
                    Id = Guid.NewGuid(),
                    CustomerId = _customerId,
                    CampaignId = _campaignId,
                    Status = LoanStatus.Pending,
                    RequestedAmount = 10000,
                    CreditScoreAtApply = 750,
                    AppliedAt = DateTime.UtcNow
                }
            };
            SetupMocks(customer, campaign, existingApps);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain("You already have an active application for this campaign");
        }

        [Fact]
        public async Task EvaluateAsync_HasUnderReviewApplicationForSameCampaign_ShouldNotBeEligible()
        {
            // Arrange
            var customer = CreateCustomer();
            var campaign = CreateCampaign();
            var existingApps = new List<LoanApplication>
            {
                new LoanApplication
                {
                    Id = Guid.NewGuid(),
                    CustomerId = _customerId,
                    CampaignId = _campaignId,
                    Status = LoanStatus.UnderReview,
                    RequestedAmount = 10000,
                    CreditScoreAtApply = 750,
                    AppliedAt = DateTime.UtcNow
                }
            };
            SetupMocks(customer, campaign, existingApps);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeFalse();
        }

        [Fact]
        public async Task EvaluateAsync_HasApprovedApplicationForSameCampaign_ShouldBeEligible()
        {
            // Arrange — previous application was already approved, so new one is allowed
            var customer = CreateCustomer();
            var campaign = CreateCampaign();
            var existingApps = new List<LoanApplication>
            {
                new LoanApplication
                {
                    Id = Guid.NewGuid(),
                    CustomerId = _customerId,
                    CampaignId = _campaignId,
                    Status = LoanStatus.Approved,
                    RequestedAmount = 10000,
                    CreditScoreAtApply = 750,
                    AppliedAt = DateTime.UtcNow
                }
            };
            SetupMocks(customer, campaign, existingApps);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 20000);

            // Assert
            result.IsEligible.Should().BeTrue();
        }

        // ========== MULTIPLE FAILURES ==========

        [Fact]
        public async Task EvaluateAsync_MultipleRulesFail_ShouldReturnAllReasons()
        {
            // Arrange — low score + amount out of range + inactive campaign
            var customer = CreateCustomer(creditScore: 300);
            var campaign = CreateCampaign(minScore: 600, minAmount: 5000, maxAmount: 50000);
            campaign.IsActive = false;
            SetupMocks(customer, campaign);

            // Act
            var result = await _service.EvaluateAsync(_customerId, _campaignId, 100000);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Count.Should().BeGreaterThanOrEqualTo(2);
        }
    }
}
