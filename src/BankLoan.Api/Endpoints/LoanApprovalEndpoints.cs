using BankLoan.Domain.Events;
using BankLoan.Domain.Interfaces;
using MassTransit;
using System.Security.Claims;

namespace BankLoan.Api.Endpoints
{
    public static class LoanApprovalEndpoints
    {
        public static void MapApprovalEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/applications").WithTags("LoanApproval")
                .RequireAuthorization(policy => policy.RequireRole("LoanOfficer", "Admin"));

            group.MapPost("/{id:guid}/approve", async (Guid id, ClaimsPrincipal user, IUnitOfWork uow, IPublishEndpoint publish) =>
            {
                var application = await uow.LoanApplications.GetByIdAsync(id);

                if (application == null)
                    return Results.NotFound(new { Message = "Application not found." });

                if (application.Status != Domain.Enums.LoanStatus.UnderReview)
                    return Results.BadRequest(new { Message = "Only applications in 'UnderReview' status can be approved." });

                var officerId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                application.Status = Domain.Enums.LoanStatus.Approved;
                application.ApprovedByUserId = officerId;
                application.DecisionAt = DateTime.UtcNow;

                uow.LoanApplications.Update(application);
                await uow.SaveChangesAsync();

                await publish.Publish(new LoanApprovedEvent { ApplicationId = application.Id, ApprovedUserId = officerId });

                return Results.Ok(new { Message = "Application approved successfully." });
            });

            group.MapPost("/{id:guid}/reject", async (Guid id, RejectRequest request, ClaimsPrincipal user, IUnitOfWork uow, IPublishEndpoint publish) =>
            {
                var application = await uow.LoanApplications.GetByIdAsync(id);

                if (application == null)
                    return Results.NotFound(new { Message = "Application not found." });

                if (application.Status != Domain.Enums.LoanStatus.UnderReview)
                    return Results.BadRequest(new { Message = "Only applications in 'UnderReview' status can be rejected." });

                application.Status = Domain.Enums.LoanStatus.Rejected;
                application.RejectionReason = request.Reason;
                application.DecisionAt = DateTime.UtcNow;

                uow.LoanApplications.Update(application);
                await uow.SaveChangesAsync();

                await publish.Publish(new LoanRejectedEvent { ApplicationId = application.Id, RejectionReason = request.Reason });

                return Results.Ok(new { Message = "Application rejected." });
            });
        }
    }

    public record RejectRequest(string Reason);
}
