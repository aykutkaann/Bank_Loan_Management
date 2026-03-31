using BankLoan.Application.DTOs;
using BankLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Mappings
{
    public static class MappingExtensions
    {

        public static CustomerDto ToDto(this Customer c) => new CustomerDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            NationalId = c.NationalId,
            DateOfBirth = c.DateOfBirth,
            MonthlyIncome = c.MonthlyIncome,
            EmployerName = c.EmployerName,
            CreditScore = c.CreditScore,
            PhoneNumber  =c.PhoneNumber ,
            Address = c.Address

        };

        public static CampaignDto ToDto(this LoanCampaign l) => new CampaignDto
        {
            Id = l.Id,
            Name = l.Name,
            Description = l.Description,
            InterestRate = l.InterestRate,
            MinCreditScore = l.MinCreditScore,
            MaxLoanAmount = l.MaxLoanAmount,
            MinLoanAmount = l.MinLoanAmount,
            TermMonths = l.TermMonths,
            PaymentFrequency = l.PaymentFrequency,
            IsActive = l.IsActive,
            StartDate = l.StartDate,
            EndDate = l.EndDate
        };

        public static LoanApplicationDto ToDto(this LoanApplication la) => new LoanApplicationDto
        {
            Id =la.Id,
            CustomerName = la.Customer != null ? $"{la.Customer.FirstName} {la.Customer.LastName}"
                                                :"Unknown",
            CampaignName = la.Campaign?.Name ?? "Unknown",
            RequestedAmount = la.RequestedAmount,
            CalculatedMonthly = la.CalculatedMonthly,
            Status = la.Status,
            CreditScoreAtApply = la.CreditScoreAtApply,
            RejectionReason = la.RejectionReason,
            AppliedAt = la.AppliedAt,
            DecisionAt = la.DecisionAt




        };
    }
}
