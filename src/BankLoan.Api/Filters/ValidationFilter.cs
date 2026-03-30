using FluentValidation;

namespace BankLoan.Api.Filters
{
    public class ValidationFilter<T> : IEndpointFilter where T : class
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validator is null)
                return await next(context);

            var argument = context.Arguments.OfType<T>().FirstOrDefault();

            if (argument is null)
                return await next(context);

            var result = await validator.ValidateAsync(argument);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return Results.BadRequest(new { Errors = errors });
            }

            return await next(context);
        }
    }
}
