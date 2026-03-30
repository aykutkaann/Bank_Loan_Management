using BankLoan.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _context;

        private  ICustomerRepository _customerRepository;
        private  ILoanApplicationRepository _loanApplicationRepo;
        private  ILoanCampaignRepository _loanCampaignRepo;


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public ICustomerRepository Customers => _customerRepository ??= new CustomerRepository(_context);


        public ILoanApplicationRepository LoanApplications => _loanApplicationRepo ??= new LoanApplicationRepository(_context);

        public ILoanCampaignRepository LoanCampaigns => _loanCampaignRepo ??= new LoanCampaignRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        


    }
}
