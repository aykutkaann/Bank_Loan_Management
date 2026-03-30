using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace BankLoan.Api.Middleware
{
    public class ExceptionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception err)
            {
                _logger.LogError(err, "An Error occured: {Message}", err.Message);
                await HandleExceptionAsync(context, err);

            }
        }

        
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound, //404
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, //401
                ArgumentException => (int)HttpStatusCode.BadRequest, // 400
                InvalidOperationException => (int)HttpStatusCode.BadRequest, // 400
                _ => (int)HttpStatusCode.InternalServerError //500

            };

            var response = new
            {
                Code = context.Response.StatusCode,
                Message = context.Response.StatusCode == 500 ? "An internal error occured." : exception.Message,
            };

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}
