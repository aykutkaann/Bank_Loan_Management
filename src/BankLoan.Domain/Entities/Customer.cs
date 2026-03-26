using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }


        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalId { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string? EmployerName { get; set; }
        public int CreditScore { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }


        public ICollection<LoanApplication> LoanApplications { get; set; } = new HashSet<LoanApplication>();


    }
}
