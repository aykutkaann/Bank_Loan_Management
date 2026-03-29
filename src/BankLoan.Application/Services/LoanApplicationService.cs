using BankLoan.Application.Common;
using BankLoan.Domain.Entities;
using BankLoan.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Services
{
    public class LoanApplicationService :ILoanApplicationService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoanEligibilityService _eligibilityService;

        public LoanApplicationService(IUnitOfWork uow, ILoanEligibilityService eligibilityService)
        {
            _uow = uow;
            _eligibilityService = eligibilityService;
        }

        public async Task<EligibilityResult> ApplyAsync(Guid customerId, Guid campaignId, decimal requestedAmount)
        {
            var eligibility = await _eligibilityService.EvaluateAsync(customerId, campaignId, requestedAmount);

            if (!eligibility.IsEligible)
            {
                return eligibility;
            }
            var campaign = await _uow.LoanCampaigns.GetByIdAsync(campaignId);
            var customer = await _uow.Customers.GetByIdAsync(customerId);


            decimal totalRepayment = requestedAmount * (1 + (campaign.InterestRate / 100));
            decimal monthlyInstallment = totalRepayment / campaign.TermMonths;

            var application = new LoanApplication
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                CampaignId = campaignId,
                RequestedAmount = requestedAmount,
                Status = Domain.Enums.LoanStatus.Pending,
                AppliedAt = DateTime.UtcNow,

                CreditScoreAtApply = customer.CreditScore,
                CalculatedMonthly = monthlyInstallment
               
            };

            await _uow.LoanApplications.AddAsync(application);
            await _uow.SaveChangesAsync();

            return eligibility;


        }

        public async Task<LoanApplication?> GetByIdAsync(Guid id)
        {
            return await _uow.LoanApplications.GetByIdAsync(id);
        }

        public async Task<List<LoanApplication>> GetByCustomerIdAsync(Guid customerId)
        {
            var appCustomer = await _uow.LoanApplications.GetByCustomerIdAsync(customerId);

            return appCustomer.ToList();
        }

        public async Task<List<LoanApplication>> GetAllAsync()
        {
            var applications = await _uow.LoanApplications.GetAllAsync();

            return applications.ToList();
        }
    }
}
