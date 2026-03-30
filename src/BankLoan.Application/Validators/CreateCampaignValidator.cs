using BankLoan.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Validators
{
    public class CreateCampaignValidator : AbstractValidator<LoanCampaign>
    {

        public CreateCampaignValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().MaximumLength(200);

            RuleFor(x => x.InterestRate)
                .NotEmpty().GreaterThan(0);

            RuleFor(x => x.MinLoanAmount)
                .NotEmpty().GreaterThan(0);

            RuleFor(x => x.MaxLoanAmount)
                .NotEmpty().GreaterThan(x => x.MinLoanAmount);

            RuleFor(x => x.TermMonths)
                .NotEmpty().GreaterThan(0);

            RuleFor(x => x.StartDate)
                .NotEmpty().LessThanOrEqualTo(x => x.EndDate);

        }
    }
}
