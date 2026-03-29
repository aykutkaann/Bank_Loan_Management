using BankLoan.Application.Common;
using BankLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Services
{
    public interface ILoanApplicationService
    {
        Task<EligibilityResult> ApplyAsync(Guid customerId, Guid campaignId, decimal requestedAmount);
        Task<LoanApplication?> GetByIdAsync(Guid id);
        Task<List<LoanApplication>> GetByCustomerIdAsync(Guid customerId);

        Task<List<LoanApplication>> GetAllAsync();


    }
}
