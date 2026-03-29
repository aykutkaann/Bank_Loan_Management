using BankLoan.Application.Common;
using BankLoan.Domain.Entities;
using BankLoan.Domain.Enums;
using BankLoan.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Services
{
    public class LoanEligibilityService : ILoanEligibilityService
    {

        private readonly IUnitOfWork _uow;

        public  LoanEligibilityService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EligibilityResult> EvaluateAsync(Guid customerId, Guid campaignId, decimal requestedAmount)
        {
            var result = new EligibilityResult();
            var now = DateTime.UtcNow;


            var customer = await _uow.Customers.GetByIdAsync(customerId);
            var campaign = await _uow.LoanCampaigns.GetByIdAsync(campaignId);


            if(customer == null)
            {
                result.Reasons.Add("Customer not found.");
                result.IsEligible = false;
                return result;
            }

            // Rule 1

            if(campaign == null || !campaign.IsActive || now < campaign.StartDate || now > campaign.EndDate)
            {
                result.Reasons.Add("Campaign is not active or has expired");
            }

            // Rule 2

            if(campaign != null)
            {
                if(requestedAmount < campaign.MinLoanAmount || requestedAmount > campaign.MaxLoanAmount)
                {
                    result.Reasons.Add($"Requested amount must be between {campaign.MinLoanAmount:N2} and {campaign.MaxLoanAmount:N2}");
                }
            }

            // Rule 3

            if(campaign != null)
            {
                if(customer.CreditScore < campaign.MinCreditScore)
                {
                    result.Reasons.Add($"Credit Score {customer.CreditScore} is below minimum required {campaign.MinCreditScore}");
                }
            }

            // Rule 4

            var existingApplications = await _uow.LoanApplications.GetByCustomerIdAsync(customerId);

            bool hasActiveApp = existingApplications.Any(a =>
                         a.CampaignId == campaignId &&
                         (a.Status == LoanStatus.Pending || a.Status == LoanStatus.UnderReview));

            if (hasActiveApp)
            {
                result.Reasons.Add("You already have an active application for this campaign");
            }



            result.IsEligible = result.Reasons.Count == 0;
            return result;

        }


    }
}
