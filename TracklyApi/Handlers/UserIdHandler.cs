using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace TracklyApi.Handlers;
public class UserIdRequirement : IAuthorizationRequirement
{
    public UserIdRequirement()
    {
    }
}

public class UserIdHandler : AuthorizationHandler<UserIdRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
    UserIdRequirement requirement)
    {
        var userId = context.User.FindFirst(c => c.Type == "userId");
        if (userId != null)
        {
            int userIdClaim;
            if (int.TryParse(userId.Value, out userIdClaim))
            {
                //get user id from route or query values
                if (context.Resource is HttpContext httpContext)
                {
                    object? requestedUserIdObj;
                    StringValues requestedUserIdQueryStringValue;
                    httpContext.Request.RouteValues.TryGetValue("userId", out requestedUserIdObj);
                    httpContext.Request.Query.TryGetValue("userId", out requestedUserIdQueryStringValue);
                    int requestedUserId;
                    if (int.TryParse((string?)requestedUserIdObj, out requestedUserId) ||
                        int.TryParse(requestedUserIdQueryStringValue.FirstOrDefault(), out requestedUserId))
                    {
                        if (userIdClaim == requestedUserId)
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
        }
        return Task.CompletedTask;
    }
}
