using BankLoan.Domain.Entities;
using BankLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.DTOs
{
    public class LoanApplicationDto
    {
        public Guid Id { get; set; }

        public string  CustomerName { get; set; }

        public string CampaignName { get; set; }


        public decimal RequestedAmount { get; set; }
        public decimal CalculatedMonthly { get; set; }


        public LoanStatus Status { get; set; }


        public int CreditScoreAtApply { get; set; }
        public string? RejectionReason { get; set; }


        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DecisionAt { get; set; }


    }
}
