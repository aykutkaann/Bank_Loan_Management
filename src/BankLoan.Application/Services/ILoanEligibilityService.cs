using BankLoan.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Services
{
    public interface ILoanEligibilityService
    {

        Task<EligibilityResult> EvaluateAsync(Guid customerId, Guid campaignId, decimal requestedAmount);

    }
}
