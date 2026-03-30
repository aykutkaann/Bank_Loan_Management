using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.DTOs
{
    public class ApplyForLoanRequest
    {
        public Guid CampaignId { get; set; }
        public decimal RequestedAmount { get; set; }
    }

}
