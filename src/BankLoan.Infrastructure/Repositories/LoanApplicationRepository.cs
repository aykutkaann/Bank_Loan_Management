using BankLoan.Domain.Entities;
using BankLoan.Domain.Enums;
using BankLoan.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Repositories
{
    public class LoanApplicationRepository : ILoanApplicationRepository
    {

        private readonly AppDbContext _context;

        public LoanApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoanApplication?> GetByIdAsync(Guid id)
        {
            return await _context.LoanApplications
                .Include(a => a.Customer)
                .Include(a => a.Campaign)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<LoanApplication>> GetByCustomerIdAsync(Guid customerId)
        {
            var customerIdApp = await _context.LoanApplications
                                .Where(c => c.CustomerId == customerId)
                                .ToListAsync();
            
            return customerIdApp;
        }

        public async Task<IEnumerable<LoanApplication>> GetByStatusAsync(LoanStatus status)
        {
            return await _context.LoanApplications
                            .Where(l => l.Status == status)
                            .Include(l => l.Customer)
                            .Include(l => l.Campaign)
                            .ToListAsync();

        }

        public async Task<IEnumerable<LoanApplication>> GetAllAsync()
        {
            return await _context.LoanApplications
                .Include(a => a.Customer)
                .Include(a => a.Campaign)
                .ToListAsync();
        }

        public async Task AddAsync(LoanApplication loanApplication)
        {
            await _context.LoanApplications.AddAsync(loanApplication);
        }

        public void Update(LoanApplication loanApplication)
        {
            _context.LoanApplications.Update(loanApplication);
        }
    }
}
