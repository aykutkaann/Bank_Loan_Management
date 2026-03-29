using BankLoan.Domain.Entities;
using BankLoan.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Repositories
{
    public class LoanCampaignRepository : ILoanCampaignRepository
    {

        private readonly AppDbContext _context;

        public LoanCampaignRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoanCampaign?> GetByIdAsync(Guid id)
        {
            var loanCamp = await _context.LoanCampaigns.FindAsync(id);

            return loanCamp;
        }

        public async Task<IEnumerable<LoanCampaign>> GetActiveCampaignsAsync()
        {
            return await _context.LoanCampaigns
                .Where(c => c.IsActive &&
                            c.StartDate <= DateTime.UtcNow &&
                            c.EndDate >= DateTime.UtcNow)
                .ToListAsync();

        }

        public async Task<IEnumerable<LoanCampaign>> GetAllAsync()
        {
            var loanCamp = await _context.LoanCampaigns.ToListAsync();

            return loanCamp;
        }

        public async Task AddAsync(LoanCampaign loanCampaign)
        {
            await _context.LoanCampaigns.AddAsync(loanCampaign);
        }

        public void Update(LoanCampaign loanCampaign)
        {
            _context.Update(loanCampaign);
        }
    }
}
