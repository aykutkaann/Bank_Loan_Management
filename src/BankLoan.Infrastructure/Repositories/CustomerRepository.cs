using BankLoan.Domain.Entities;
using BankLoan.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;


        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            return customer;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var customer = await _context.Customers.ToListAsync();


            return customer;
        }

        public async Task<Customer?> GetByAppUserIdAsync(Guid appUserId)
        {
            var appUser = await _context.Customers
                .FirstOrDefaultAsync(c => c.AppUserId == appUserId);

            return appUser;
        }


       public async Task<Customer?> GetByNationalIdAsync(string nationalId)
        {
            var nationalIdUser = await _context.Customers
                .FirstOrDefaultAsync(c => c.NationalId == nationalId);

            return nationalIdUser;
        }



        public async Task AddAsync(Customer customer)
        {
             await _context.Customers.AddAsync(customer);


        }


      public void Delete(Customer customer)
        {
            _context.Customers.Remove(customer);

        }


        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);

        }
    }
}
