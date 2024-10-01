using Microsoft.AspNetCore.Authorization;

namespace HelloMVCCore.Infrastructure;

public class SomeAuthorizationRequirement
    : AuthorizationHandler<SomeAuthorizationRequirement>,
        IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SomeAuthorizationRequirement requirement
    )
    {
        if (context.User != null)
        {
            // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-8.0
            // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-8.0
            // https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core
        }
        return Task.CompletedTask;
    }
}
