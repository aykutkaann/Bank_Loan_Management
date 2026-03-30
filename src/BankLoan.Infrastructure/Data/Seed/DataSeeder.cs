using BankLoan.Domain.Entities;
using BankLoan.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Data.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 1- Roles Seed
            string[] roles = { "Admin", "LoanOfficer", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            // 2- Admin & Officer Seed
            var adminEmail = "admin@bank.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            var officerEmail = "officer@bank.com";
            if (await userManager.FindByEmailAsync(officerEmail) == null)
            {
                var officerUser = new AppUser
                {
                    UserName = officerEmail,
                    Email = officerEmail,
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(officerUser, "Officer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(officerUser, "LoanOfficer");
                }
            }

            // 3- Customer Seed
            if (!await context.Customers.AnyAsync())
            {
                var customerData = new[]
                {
                    new { Name = "Ali", Surname = "Yilmaz", Email = "ali.yilmaz@bank.com", TCKN = "10000000001", Income = 25000m, Score = 820, Dob = new DateOnly(1990, 3, 15) },
                    new { Name = "Zeynep", Surname = "Kara", Email = "zeynep.kara@bank.com", TCKN = "10000000002", Income = 15000m, Score = 680, Dob = new DateOnly(1988, 7, 22) },
                    new { Name = "Mehmet", Surname = "Demir", Email = "mehmet.demir@bank.com", TCKN = "10000000003", Income = 8000m, Score = 520, Dob = new DateOnly(1995, 1, 10) },
                    new { Name = "Ayse", Surname = "Celik", Email = "ayse.celik@bank.com", TCKN = "10000000004", Income = 45000m, Score = 900, Dob = new DateOnly(1985, 11, 5) },
                    new { Name = "Burak", Surname = "Ozkan", Email = "burak.ozkan@bank.com", TCKN = "10000000005", Income = 6000m, Score = 350, Dob = new DateOnly(1998, 6, 30) },
                    new { Name = "Deniz", Surname = "Arslan", Email = "deniz.arslan@bank.com", TCKN = "10000000006", Income = 12000m, Score = 600, Dob = new DateOnly(1992, 4, 18) },
                    new { Name = "Selin", Surname = "Tas", Email = "selin.tas@bank.com", TCKN = "10000000007", Income = 20000m, Score = 750, Dob = new DateOnly(1993, 9, 25) },
                    new { Name = "Can", Surname = "Aydin", Email = "can.aydin@bank.com", TCKN = "10000000008", Income = 10000m, Score = 480, Dob = new DateOnly(1997, 12, 2) }
                };

                foreach (var data in customerData)
                {
                    var user = new AppUser { UserName = data.Email, Email = data.Email, EmailConfirmed = true };
                    var result = await userManager.CreateAsync(user, "Customer123!");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Customer");

                        context.Customers.Add(new Customer
                        {
                            AppUserId = user.Id,
                            FirstName = data.Name,
                            LastName = data.Surname,
                            NationalId = data.TCKN,
                            MonthlyIncome = data.Income,
                            CreditScore = data.Score,
                            DateOfBirth = data.Dob,
                            PhoneNumber = "5550000000",
                            Address = "Istanbul, Turkey",
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
                await context.SaveChangesAsync();
            }

            // 4- Campaign Seed
            if (!await context.LoanCampaigns.AnyAsync())
            {
                context.LoanCampaigns.AddRange(
                    new LoanCampaign { Name = "Home Starter 2026", Description = "Affordable home loans for first-time buyers", InterestRate = 1.29m, MinCreditScore = 700, MinLoanAmount = 100000, MaxLoanAmount = 2000000, TermMonths = 60, PaymentFrequency = PaymentFrequency.Monthly, IsActive = true, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddYears(1), CreatedAt = DateTime.UtcNow },
                    new LoanCampaign { Name = "Quick Cash Express", Description = "Fast cash for urgent needs", InterestRate = 2.99m, MinCreditScore = 500, MinLoanAmount = 5000, MaxLoanAmount = 50000, TermMonths = 12, PaymentFrequency = PaymentFrequency.Monthly, IsActive = true, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddYears(1), CreatedAt = DateTime.UtcNow },
                    new LoanCampaign { Name = "Auto Loan Premium", Description = "Premium vehicle financing with low rates", InterestRate = 1.79m, MinCreditScore = 650, MinLoanAmount = 50000, MaxLoanAmount = 500000, TermMonths = 36, PaymentFrequency = PaymentFrequency.Biweekly, IsActive = true, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddYears(1), CreatedAt = DateTime.UtcNow },
                    new LoanCampaign { Name = "Education Support", Description = "Low-interest loans for education expenses", InterestRate = 0.99m, MinCreditScore = 600, MinLoanAmount = 10000, MaxLoanAmount = 200000, TermMonths = 48, PaymentFrequency = PaymentFrequency.Monthly, IsActive = true, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddYears(1), CreatedAt = DateTime.UtcNow },
                    new LoanCampaign { Name = "Emergency Micro Loan", Description = "Small emergency loans with quick approval", InterestRate = 3.49m, MinCreditScore = 400, MinLoanAmount = 1000, MaxLoanAmount = 10000, TermMonths = 6, PaymentFrequency = PaymentFrequency.Weekly, IsActive = true, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddYears(1), CreatedAt = DateTime.UtcNow }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
