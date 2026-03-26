using BankLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Interfaces
{
    public interface ICustomerRepository
    {

        Task<Customer?> GetByIdAsync(Guid id);

        Task<IEnumerable<Customer>> GetAllAsync();

        Task<Customer?> GetByAppUserIdAsync(Guid appUserId);

        Task<Customer?> GetByNationalIdAsync(string nationalId);

        Task AddAsync(Customer customer);

        void Delete(Customer customer);

        void Update(Customer customer);
    }
}
