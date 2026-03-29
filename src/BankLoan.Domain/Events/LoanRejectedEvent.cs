using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Events
{
    public record LoanRejectedEvent
    (
         Guid ApplicationId,
         string RejectionReason
    );
}
