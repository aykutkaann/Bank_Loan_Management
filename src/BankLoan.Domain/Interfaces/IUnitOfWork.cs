using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        ICustomerRepository Customers { get; }
        ILoanCampaignRepository LoanCampaigns { get; }
        ILoanApplicationRepository LoanApplications { get; }
        
        Task<int> SaveChangesAsync();
    }
}
