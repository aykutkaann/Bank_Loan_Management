using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Events
{
    public record LoanApplicationSubmittedEvent
    {
        public Guid ApplicationId { get; init; }
        public Guid CustomerId { get; init; }
        public Guid CampaignId { get; init; }
        public decimal RequestedAmount { get; init; }
    }

}
