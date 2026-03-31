using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Events
{
    public record LoanRejectedEvent
    {
        public Guid ApplicationId { get; init; }
        public string RejectionReason { get; init; } = string.Empty;
    }
}
