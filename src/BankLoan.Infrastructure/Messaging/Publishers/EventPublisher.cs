using BankLoan.Domain.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Messaging.Publishers
{
    public class EventPublisher :IEventPublisher
    {

        private readonly IPublishEndpoint _publish;

        public EventPublisher(IPublishEndpoint publish)
        {
            _publish = publish;
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
           await _publish.Publish(@event);
        }
    }
}
