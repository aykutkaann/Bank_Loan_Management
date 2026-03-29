using BankLoan.Domain.Events;
using BankLoan.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Messaging.Consumers
{
    public class LoanApprovedConsumer : IConsumer<LoanApprovedEvent>
    {

        private readonly ILogger<LoanApprovedConsumer> _logger;

        public LoanApprovedConsumer(ILogger<LoanApprovedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<LoanApprovedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation("Loan approver : Application {AppId} approver by user {UserId}", message.ApplicationId, message.ApprovedUserId);

            await Task.CompletedTask;
        }
    }
}
