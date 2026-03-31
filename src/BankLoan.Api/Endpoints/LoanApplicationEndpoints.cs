using BankLoan.Api.Filters;
using BankLoan.Application.DTOs;
using BankLoan.Application.Mappings;
using BankLoan.Application.Services;
using BankLoan.Domain.Entities;
using BankLoan.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankLoan.Api.Endpoints
{
    public static class LoanApplicationEndpoints
    {

        public static void MapApplicationEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/application").WithTags("LoanApplications");

            group.MapPost("/", async ([FromBody] ApplyForLoanRequest request, ILoanApplicationService service, HttpContext context, IUnitOfWork uow) =>
            {

                var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim)) return Results.Unauthorized();


                var appUserId = Guid.Parse(userIdClaim);

                var customer = await uow.Customers.GetByAppUserIdAsync(appUserId);

                if (customer == null)
                    return Results.NotFound(new { Message = "Customer not found." });

                var result = await service.ApplyAsync(customer.Id, request.CampaignId, request.RequestedAmount);

                return result.IsEligible
                        ? Results.Ok(result)
                        : Results.BadRequest(result);


            }).RequireAuthorization(policy => policy.RequireRole("Customer")).AddEndpointFilter<ValidationFilter<ApplyForLoanRequest>>();

            group.MapGet("/", async (ILoanApplicationService service) =>
            {
                var applications = await service.GetAllAsync();
                return Results.Ok(applications.Select(a => a.ToDto()));
            }).RequireAuthorization();




            group.MapGet("/{id:guid}", async (Guid id, ILoanApplicationService service) =>
            {
                var application = await service.GetByIdAsync(id);

                return application is not null
                    ? Results.Ok(application)
                    : Results.NotFound(new { Message = "Application not found." });
            }).RequireAuthorization();
        }

    }
}
