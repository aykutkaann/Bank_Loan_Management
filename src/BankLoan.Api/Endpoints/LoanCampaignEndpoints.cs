using BankLoan.Api.Filters;
using BankLoan.Application.Mappings;
using BankLoan.Application.Services;
using BankLoan.Domain.Entities;
using BankLoan.Domain.Interfaces;

namespace BankLoan.Api.Endpoints
{
    public static class LoanCampaignEndpoints
    {
        public static void MapCampaignEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/campaign").WithTags("LoanCampaigns");

            group.MapGet("/", async (IUnitOfWork unitOfWork) =>
            {
                var campaigns = await unitOfWork.LoanCampaigns.GetAllAsync();

                return Results.Ok(campaigns.Select(a => a.ToDto()));
            });

            group.MapGet("/{id:guid}", async (Guid id, IUnitOfWork unitOfWork) =>
            {
                var campaign = await unitOfWork.LoanCampaigns.GetByIdAsync(id);
                if (campaign == null)
                {
                    return Results.NotFound(new { Message = "Campaign is not found." });
                }

                return Results.Ok(campaign.ToDto());
            });


            group.MapPost("/", async (LoanCampaign campaign, ILoanCampaignService service) =>

            {
                 await service.AddAsync(campaign);
                return Results.Created($"/api/campaign/{campaign.Id}", campaign.ToDto());


            }).RequireAuthorization(policy => policy.RequireRole( "Admin")).AddEndpointFilter<ValidationFilter<LoanCampaign>>();

            group.MapPut("/{id:guid}", async (Guid id, LoanCampaign updatedCampaign, ILoanCampaignService service) =>
            {
                var existingCampaign = await service.GetByIdAsync(id);

                if(existingCampaign == null)
                {
                    return Results.NotFound(new { Message = "Campaign is not found." });
                }

                existingCampaign.Name = updatedCampaign.Name;
                existingCampaign.Description = updatedCampaign.Description;
                existingCampaign.InterestRate = updatedCampaign.InterestRate;
                existingCampaign.MinCreditScore = updatedCampaign.MinCreditScore;
                existingCampaign.TermMonths = updatedCampaign.TermMonths;
                existingCampaign.MinLoanAmount = updatedCampaign.MinLoanAmount;
                existingCampaign.MaxLoanAmount = updatedCampaign.MaxLoanAmount;
                existingCampaign.StartDate = updatedCampaign.StartDate;
                existingCampaign.EndDate = updatedCampaign.EndDate;

                await service.UpdateAsync(existingCampaign);

                return Results.Ok(existingCampaign.ToDto());



            }).RequireAuthorization(policy => policy.RequireRole( "Admin"));

            group.MapDelete("/{id:guid}", async (Guid id, ILoanCampaignService service) =>
            {
                try
                {
                    await service.DeleteAsync(id);
                    return Results.NoContent();

                }
                catch(KeyNotFoundException)
                {
                    return Results.NotFound(new { Message = "Campaign not found for delete." });
                }
            }).RequireAuthorization(policy => policy.RequireRole( "Admin"));






        }
    }
}
