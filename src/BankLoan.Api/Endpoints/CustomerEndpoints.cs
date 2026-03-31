using BankLoan.Application.Mappings;
using BankLoan.Application.Services;
using BankLoan.Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankLoan.Api.Endpoints
{
    public static class CustomerEndpoints
    {

        public static void MapCustomerEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/customer")
                            .WithTags("Customers")
                            .RequireAuthorization(policy =>
                                    policy.RequireRole("LoanOfficer", "Admin"));

            group.MapGet("/", async (IUnitOfWork unitOfWork) =>
            {
                var customer = await unitOfWork.Customers.GetAllAsync();

                return Results.Ok(customer.Select(c=> c.ToDto()));
                
            });

            group.MapGet("/{id:guid}", async (Guid id, IUnitOfWork unitOfWork) =>
            {
                var customer = await unitOfWork.Customers.GetByIdAsync(id);

                if(customer == null)
                {
                    return Results.NotFound(new {Message = "Customer not found."});
                }

                return Results.Ok(customer.ToDto());
            });

            group.MapGet("/{id:guid}/credit-score", async (Guid id, IUnitOfWork unitOfWork, ICreditScoreService creditScore) =>
            {
                var customer = await unitOfWork.Customers.GetByIdAsync(id);

                if (customer == null)
                {
                    return Results.NotFound(new { Message = "Customer not found." });
                }

                var score = await creditScore.GetScoreByCustomerIdAsync(id);

                return Results.Ok(new
                {
                    CustomerId = id,
                    FullName = $"{customer.FirstName} {customer.LastName}",
                    CreditScore = score
                });
            });
        }
    }
}
