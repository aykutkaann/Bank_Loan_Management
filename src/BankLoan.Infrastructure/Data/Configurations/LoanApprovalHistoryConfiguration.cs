using BankLoan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Data.Configurations
{
    public class LoanApprovalHistoryConfiguration : IEntityTypeConfiguration<LoanApprovalHistory>
    {

        public void Configure(EntityTypeBuilder<LoanApprovalHistory> builder)
        {
            builder.ToTable("loan_approval_histories");

            builder.HasKey(lo => lo.Id);

            builder.Property(lo => lo.Decision).IsRequired().HasMaxLength(100);

            builder.Property(lo => lo.Notes).HasMaxLength(500);

            builder.Property(lo => lo.Timestamp).HasColumnType("timestamptz");


            builder.HasOne(lo => lo.Application)
                .WithMany(a => a.ApprovalHistories)
                .HasForeignKey(lo => lo.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lo => lo.Officer)
                .WithMany()
                .HasForeignKey(lo => lo.OfficerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
