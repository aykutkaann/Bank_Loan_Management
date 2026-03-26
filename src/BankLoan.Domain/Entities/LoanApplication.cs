using BankLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Entities
{
    public class LoanApplication
    {

        public Guid Id { get; set; }


        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Guid CampaignId { get; set; }
        public LoanCampaign Campaign { get; set; }


        public decimal RequestedAmount { get; set; }
        public decimal CalculatedMonthly { get; set; }


        public LoanStatus  Status  { get; set; }


        public int CreditScoreAtApply { get; set; }
        public string? RejectionReason { get; set; }
        public Guid? ApprovedByUserId { get; set; }

        public AppUser? ApprovedByUser { get; set; }



        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DecisionAt { get; set; }


        public ICollection<LoanApprovalHistory> ApprovalHistories { get; set; } = new HashSet<LoanApprovalHistory>();
    }
}
