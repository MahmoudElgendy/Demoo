using Demo.Api.Contracts.Exceptions;
using Demo.Api.Middlewares.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demo.Api.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class UnhandledExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UnhandledExceptionMiddleware> _logger;

        public UnhandledExceptionMiddleware(RequestDelegate next, ILogger<UnhandledExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                _logger.LogDebug("UnhandledExceptionMiddleware registered.");
                await _next(httpContext);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Unhandeled exception caught.");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            var response = CreateErrorResponseObject(ex, httpContext);
            // todo
        }

        private static IResult CreateErrorResponseObject(Exception ex, HttpContext httpContext)
        {
            switch (ex.GetType().Name)
            {
                case nameof(UniqueKeyException):
                    return TypedResults.Conflict(CreateErrorDetails(ex)); // 409 conflict

                case nameof(DatabaseObjectNotFoundException):
                case "ObjectNotFoundException":
                    return TypedResults.NotFound(CreateErrorDetails(ex)); // 404 not found

                case nameof(PasswordRestrictionException):
                    return TypedResults.Problem(CreateErrorDetails(ex, 401)); // unauthorized

                case nameof(UnauthorizedAccessException):
                    return TypedResults.Problem(CreateErrorDetails(ex, 403)); // forbidden

                default:
                    return TypedResults.Problem(CreateErrorDetails(ex, 500)); // internal server error

            }
        }

        private static ProblemDetails CreateErrorDetails(Exception e, int? statusCode = null)
        {
            return CreateErrorDetails(e.Message, statusCode);
        }
        private static ProblemDetails CreateErrorDetails(string e, int? statusCode = null)
        {
            var pd = new ProblemDetails();
            pd.Status = statusCode;
            pd.Extensions.Add("errors", new[] { e });
            return pd;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UnhandledExceptionMiddleware>();
        }
    }
}
