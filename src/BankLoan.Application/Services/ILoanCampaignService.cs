using BankLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Services
{
    public interface ILoanCampaignService
    {
        Task<IEnumerable<LoanCampaign>> GetAllAsync();

        Task<IEnumerable<LoanCampaign>> GetActiveCampaignAsync();
        Task<LoanCampaign?> GetByIdAsync(Guid id);
        Task AddAsync(LoanCampaign loanCampaign);
        Task UpdateAsync(LoanCampaign loanCampaign);
        Task DeleteAsync(Guid id);
    }
}
