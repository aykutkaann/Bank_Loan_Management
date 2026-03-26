using BankLoan.Domain.Entities;
using BankLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Interfaces
{
    public interface ILoanCampaignRepository
    {
        Task<LoanCampaign?> GetByIdAsync(Guid id);
        Task<IEnumerable<LoanCampaign>> GetActiveCampaignsAsync();

        Task<IEnumerable<LoanCampaign>> GetAllAsync();

        Task AddAsync(LoanCampaign loanCampaign);
        void Update(LoanCampaign loanCampaign);

    }
}
