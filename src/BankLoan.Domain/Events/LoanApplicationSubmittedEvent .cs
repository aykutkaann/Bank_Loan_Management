using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Events
{
    public record LoanApplicationSubmittedEvent
    (
         Guid ApplicationId,
         Guid CustomerId,
         Guid CampaignId,
         decimal RequestedAmount
        
     );   
    
}
