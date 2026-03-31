using BankLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.DTOs
{
    public class CampaignDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal InterestRate { get; set; }
        public int MinCreditScore { get; set; }
        public decimal MaxLoanAmount { get; set; }
        public decimal MinLoanAmount { get; set; }
        public int TermMonths { get; set; }
        public PaymentFrequency PaymentFrequency  { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
