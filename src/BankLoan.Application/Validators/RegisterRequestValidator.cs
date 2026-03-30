using BankLoan.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(6);

            RuleFor(x => x.FirstName)
                .NotEmpty().MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty().MaximumLength(100);
        }
    }
}
