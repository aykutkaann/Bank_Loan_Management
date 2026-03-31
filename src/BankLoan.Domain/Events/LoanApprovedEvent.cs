using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Events
{
    public record LoanApprovedEvent
    {
        public Guid ApplicationId { get; init; }
        public Guid ApprovedUserId { get; init; }
    }
}
