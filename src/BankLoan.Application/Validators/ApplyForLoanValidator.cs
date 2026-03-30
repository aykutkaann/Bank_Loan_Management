using BankLoan.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Validators
{
    public class ApplyForLoanValidator : AbstractValidator<ApplyForLoanRequest>
    {
        public ApplyForLoanValidator()
        {
            RuleFor(x => x.CampaignId)
                .NotEmpty();

            RuleFor(x => x.RequestedAmount)
                .NotEmpty().GreaterThan(0);
        }
    }
}
