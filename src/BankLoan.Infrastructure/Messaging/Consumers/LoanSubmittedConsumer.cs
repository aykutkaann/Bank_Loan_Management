using BankLoan.Domain.Events;
using BankLoan.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Infrastructure.Messaging.Consumers
{
    public class LoanSubmittedConsumer : IConsumer<LoanApplicationSubmittedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LoanSubmittedConsumer> _logger;

        public LoanSubmittedConsumer(IUnitOfWork unitOfWork, ILogger<LoanSubmittedConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<LoanApplicationSubmittedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation("Processing application: {AppId}", message.ApplicationId);

            var application = await _unitOfWork.LoanApplications.GetByIdAsync(message.ApplicationId);


            if(application != null)
            {
                application.Status = Domain.Enums.LoanStatus.UnderReview;

                _unitOfWork.LoanApplications.Update(application);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Application {AppId} status updated to UnderReview", message.ApplicationId);
            }
        }
    }
}
