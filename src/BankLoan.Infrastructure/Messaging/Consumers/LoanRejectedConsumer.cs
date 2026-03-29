using BankLoan.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Messaging.Consumers
{
    public class LoanRejectedConsumer :IConsumer<LoanRejectedEvent>
    {
        private readonly ILogger<LoanRejectedConsumer> _logger;


        public  LoanRejectedConsumer(ILogger<LoanRejectedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<LoanRejectedEvent> context)
        {
            var message = context.Message;


            _logger.LogInformation("Loan rejected: Application {AppId} rejection reason {Reason}", message.ApplicationId, message.RejectionReason);

            await Task.CompletedTask;
        }

    }
}
