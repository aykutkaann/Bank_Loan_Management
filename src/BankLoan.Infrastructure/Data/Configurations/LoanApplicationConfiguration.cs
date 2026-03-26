using BankLoan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Data.Configurations
{
    public class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
    {

        public void Configure(EntityTypeBuilder<LoanApplication> builder)
        {
            builder.ToTable("loan_applications");

            builder.HasKey(lap => lap.Id);

            builder.Property(lap => lap.RequestedAmount).HasPrecision(18, 2);

            builder.Property(lap => lap.CalculatedMonthly).HasPrecision(18, 2);

            builder.Property(lap => lap.Status).HasDefaultValue(BankLoan.Domain.Enums.LoanStatus.Pending).IsRequired();

            builder.Property(lap => lap.CreditScoreAtApply).IsRequired();

            builder.Property(lap => lap.RejectionReason);

            builder.Property(lap => lap.AppliedAt).HasColumnType("timestamptz");

            builder.Property(lap => lap.DecisionAt).HasColumnType("timestamptz");



            builder.HasOne(lap => lap.Customer)
                .WithMany(c => c.LoanApplications)
                .HasForeignKey(lap => lap.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lap => lap.Campaign)
                .WithMany(ca => ca.LoanApplications)
                .HasForeignKey(lap => lap.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

          

            builder.HasOne(lap => lap.ApprovedByUser)
                .WithMany()
                .HasForeignKey(lap => lap.ApprovedByUserId)
                .OnDelete(DeleteBehavior.Restrict);




        }
    }
}
