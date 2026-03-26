using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Entities
{
    public  class LoanApprovalHistory
    {

        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OfficerId { get; set; }
        public string Decision { get; set; }
        public string? Notes { get; set; }
        public DateTime Timestamp { get; set; }

        public LoanApplication Application { get; set; }
        public AppUser Officer { get; set; }
    }
}
