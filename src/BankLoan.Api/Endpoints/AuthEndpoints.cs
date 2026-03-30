using BankLoan.Api.Filters;
using BankLoan.Application.DTOs;
using BankLoan.Application.Services;
using System.Security.Claims;

namespace BankLoan.Api.Endpoints
{
    public static class AuthEndpoints
    {

        public static void MapAuthEndpoint(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/auth/").WithTags("Auth");


            group.MapPost("/register", async (RegisterDto registerDto, IAuthService authService) =>
            {

                try
                {
                    var result = await authService.RegisterAsync(registerDto);
                    return Results.Ok(result);
                }
                catch(Exception err)
                {
                    return Results.BadRequest(new { message = err.Message });
                }

            }).AddEndpointFilter<ValidationFilter<RegisterDto>>();


            group.MapPost("/login", async (LoginDto loginDto, IAuthService authService) =>
            {
                try
                {
                    var result = await authService.LoginAsync(loginDto);
                    return Results.Ok(result);
                }
                catch (Exception err)
                {
                    return Results.BadRequest(new { message = err.Message });

                }
            }).AddEndpointFilter<ValidationFilter<LoginDto>>();


            group.MapPost("/refresh", async (RefreshTokenRequest request, IAuthService authService) =>
            {
                try
                {
                    var result = await authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);

                    return Results.Ok(result);
                }
                catch (Exception err)
                {
                    return Results.BadRequest(new { message = err.Message });
                }
            });


            group.MapPost("/logout", async (HttpContext httpContext, IAuthService authService) =>
            {
                var email = httpContext.User.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(email))
                {
                    return Results.Unauthorized();
                }

                await authService.LogoutAsync(email);

                return Results.Ok(new { message = "Logout isSuccessfull" });

            }).RequireAuthorization();
        }
    }
}
