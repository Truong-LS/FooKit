using Microsoft.AspNetCore.Mvc.Filters;
using MyProject.Domain.Exceptions;
using System.Security.Claims;

namespace MyProject.API.Filters
{
    public class OwnerActionFilter : IActionFilter
    {
        private readonly ILogger<OwnerActionFilter> _logger;

        public OwnerActionFilter(ILogger<OwnerActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // 1. Get UserId from JWT Token
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 2. Get the resource/post ID from route (e.g., api/posts/{id})
            if (context.ActionArguments.TryGetValue("id", out var id))
            {
                // ASSUMPTION: Call the database to check whether this resource belongs to the user
                // Here it's mocked, in reality you can inject a service here
                bool isOwner = true; // Logic: _service.CheckOwner(id, userId);

                if (!isOwner)
                {
                    // Throw 403 error - your middleware will catch and return RFC 7807 response
                    throw new ForbiddenAccessException("You do not have permission to modify this content.");
                }
            }

            // LOGGING: Audit who did what
            var actionName = context.ActionDescriptor.DisplayName;
            _logger.LogInformation($"[AUDIT LOG] User {userId} called Action {actionName} at {DateTime.Now}");
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}