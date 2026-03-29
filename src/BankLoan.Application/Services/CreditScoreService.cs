using BankLoan.Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Services
{
    public class CreditScoreService : ICreditScoreService
    {

        private readonly IUnitOfWork _uow;

        public CreditScoreService(IUnitOfWork uow)
        {
            _uow = uow;

        }

        public async Task<int> GetScoreByCustomerIdAsync(Guid customerId)
        {
            var customer = await _uow.Customers.GetByIdAsync(customerId);

            if (customer == null)
                throw new KeyNotFoundException("Customer Not Found");

            return customer.CreditScore;

        }

        public async Task<bool> CheckEligibilityAsync(Guid customerId, Guid campaignId)
        {
            var customerScore = await GetScoreByCustomerIdAsync(customerId);
            var campaign = await _uow.LoanCampaigns.GetByIdAsync(campaignId);


            if (campaign == null)
                throw new KeyNotFoundException("Campaign Not Found");

            return customerScore >= campaign.MinCreditScore;
        }



    }
}
