using BankLoan.Domain.Entities;
using BankLoan.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Services
{
    public class LoanCampaignService : ILoanCampaignService
    {
        private readonly IUnitOfWork _uow;

        public LoanCampaignService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<LoanCampaign>> GetAllAsync()
        {
            var campaign = await _uow.LoanCampaigns.GetAllAsync();

            return campaign.ToList();
        }

        public async Task<IEnumerable<LoanCampaign>> GetActiveCampaignAsync()
        {
            return await _uow.LoanCampaigns.GetActiveCampaignsAsync();
            
        }

        public async Task<LoanCampaign?> GetByIdAsync(Guid id)
        {
            return await _uow.LoanCampaigns.GetByIdAsync(id);
           
        }

        public async Task AddAsync(LoanCampaign loanCampaign)
        {
            await _uow.LoanCampaigns.AddAsync(loanCampaign);

            await _uow.SaveChangesAsync();

            
        }
        

        public async Task UpdateAsync(LoanCampaign loanCampaign)
        {
             _uow.LoanCampaigns.Update(loanCampaign);

            await _uow.SaveChangesAsync();

            
        }

        public async Task DeleteAsync(Guid id)
        {
            var campaign = await _uow.LoanCampaigns.GetByIdAsync(id);

           if(campaign is null)
            {
                throw new Exception("Campaign not found.");
            }

            campaign.IsActive = false;

            await UpdateAsync(campaign);
        }





    }
}
