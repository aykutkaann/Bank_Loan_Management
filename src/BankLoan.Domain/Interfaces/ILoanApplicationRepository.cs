using BankLoan.Domain.Entities;
using BankLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Interfaces
{
    public interface ILoanApplicationRepository
    {

        Task<LoanApplication?> GetByIdAsync(Guid id);

        Task<IEnumerable<LoanApplication>> GetByCustomerIdAsync(Guid customerId);

        Task<IEnumerable<LoanApplication>> GetByStatusAsync(LoanStatus status);

        Task<IEnumerable<LoanApplication>> GetAllAsync();

        Task AddAsync(LoanApplication loanApplication);

        void Update(LoanApplication loanApplication);
    } 
}
