using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Domain.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event) where T : class;


    }
}
