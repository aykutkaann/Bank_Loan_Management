using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Events
{
    public record LoanApprovedEvent
    (
         Guid ApplicationId,
         Guid ApprovedUserId

    );
}
