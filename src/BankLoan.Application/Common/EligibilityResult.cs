using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Common
{
    public class EligibilityResult
    {
        public bool IsEligible { get; set; }
        public List<string> Reasons { get; set; } = new();
    }
}
