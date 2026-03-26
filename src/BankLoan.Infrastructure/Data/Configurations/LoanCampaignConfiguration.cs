using BankLoan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Data.Configurations
{
    public class LoanCampaignConfiguration : IEntityTypeConfiguration<LoanCampaign>
    {

        public void Configure(EntityTypeBuilder<LoanCampaign> builder)
        {
            builder.ToTable("loan_campaigns");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Name).IsRequired().HasMaxLength(200);

            builder.Property(l => l.Description).IsRequired().HasMaxLength(1000);

            builder.Property(l => l.InterestRate).IsRequired().HasPrecision(5, 2);

            builder.Property(l => l.MinCreditScore).IsRequired();

            builder.Property(l => l.MaxLoanAmount).IsRequired().HasPrecision(18, 2);

            builder.Property(l => l.MinLoanAmount).IsRequired().HasPrecision(18, 2);

            builder.Property(l => l.TermMonths).IsRequired();

            builder.Property(l => l.PaymentFrequency).IsRequired().HasDefaultValue(BankLoan.Domain.Enums.PaymentFrequency.Monthly);

            builder.Property(l => l.IsActive).HasDefaultValue(true);

            builder.Property(l => l.StartDate).HasColumnType("timestamptz");
            builder.Property(l => l.EndDate).HasColumnType("timestamptz");
            builder.Property(l => l.CreatedAt).HasColumnType("timestamptz");

           



        }
    }
}
