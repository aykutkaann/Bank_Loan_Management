using BankLoan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {

        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);

            builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);

            builder.Property(c => c.NationalId).IsRequired();
            builder.HasIndex(c => c.NationalId).IsUnique();

            builder.Property(c => c.DateOfBirth).IsRequired();

            builder.Property(c => c.MonthlyIncome).IsRequired().HasPrecision(18, 2);

            builder.Property(c => c.EmployerName);

            builder.Property(c => c.CreditScore).IsRequired();

            builder.Property(c => c.PhoneNumber).IsRequired();

            builder.Property(c => c.Address).IsRequired().HasMaxLength(500);

            builder.Property(c => c.CreatedAt).HasColumnType("timestamptz");


            builder.HasOne(c => c.AppUser)
                .WithOne(a => a.Customer)
                .HasForeignKey<Customer>(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            

        }

    }
}
