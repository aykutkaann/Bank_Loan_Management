using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Services
{
    public interface ICreditScoreService
    {
        Task<int> GetScoreByCustomerIdAsync(Guid customerId);
        Task<bool> CheckEligibilityAsync(Guid customerId, Guid campaignId);
    }
}
