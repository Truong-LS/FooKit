using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyProject.Domain.Exceptions;

namespace MyProject.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // 1. Log the error for developers
            _logger.LogError(exception, "System Error: {Message}", exception.Message);

            // 2. Default to 500 (Internal Server Error)
            var statusCode = StatusCodes.Status500InternalServerError;
            var title = "Internal server error.";
            // Hide exception details in production
            var detail = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment() 
                ? exception.Message 
                : "An unexpected error occurred. Please try again later.";
            Dictionary<string, string[]>? validationErrors = null;

            // 3. Classify exception types thrown from the Service layer
            switch (exception)
            {
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    title = "Resource not found.";
                    break;

                case BadRequestException:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = "Invalid request data.";
                    break;

                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Unauthorized access.";
                    break;

                case ConflictException conflictEx:
                    statusCode = StatusCodes.Status409Conflict;
                    title = "Data conflict.";
                    break;

                case ForbiddenAccessException:
                    statusCode = StatusCodes.Status403Forbidden;
                    title = "Access forbidden.";
                    break;
                        
                case UnauthenticatedException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Authentication failed.";
                    break;

                case FluentValidation.ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = "Invalid input data.";
                    detail = "One or more validation errors occurred.";

                    validationErrors = validationEx.Errors
                        .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                        .ToDictionary(g => g.Key, g => g.ToArray());

                    break;
            }

            // 4. Create standard RFC 7807 response (Problem Details)
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Type = $"https://httpstatuses.com/{statusCode}"
            };

            if (validationErrors != null)
            {
                problemDetails.Extensions.Add("errors", validationErrors);
            }

            // 5. Return response to client
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // Tell .NET that the exception has been handled
        }
    }
}