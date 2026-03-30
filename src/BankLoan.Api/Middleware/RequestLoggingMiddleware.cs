using System.Diagnostics;

namespace BankLoan.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var request = context.Request;

            _logger.LogInformation("HTTP Request information: {Method} {Path} started.",
                request.Method, request.Path);

            await _next(context);

            stopwatch.Stop();


            var response = context.Response;

            var statusCode = context.Response.StatusCode;

            if (statusCode >= 500)
                _logger.LogError("HTTP Response Information: {Method} {Path} finished in {ElapsedMilliseconds}ms with status {StatusCode}",
                request.Method, request.Path, stopwatch.ElapsedMilliseconds, response.StatusCode);
            else if (statusCode >= 400)
                _logger.LogWarning("HTTP Response Information: {Method} {Path} finished in {ElapsedMilliseconds}ms with status {StatusCode}",
                request.Method, request.Path, stopwatch.ElapsedMilliseconds, response.StatusCode);
            else
                _logger.LogInformation("HTTP Response Information: {Method} {Path} finished in {ElapsedMilliseconds}ms with status {StatusCode}",
                request.Method, request.Path, stopwatch.ElapsedMilliseconds, response.StatusCode);

        }

    }
}
