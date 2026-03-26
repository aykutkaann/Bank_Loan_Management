using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Enums
{
   public enum LoanStatus
    {
        Pending,
        UnderReview,
        Approved,
        Rejected,
        Cancelled
    }
}
