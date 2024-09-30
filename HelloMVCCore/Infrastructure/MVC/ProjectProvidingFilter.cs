using HelloMVCCore.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HelloMVCCore.Infrastructure.MVC;

public class ProjectProvidingFilter(ILogger<ProjectProvidingFilter> logger, ApplicationDbContext db) : IActionFilter
{
    private const string RunOnceMarker = nameof(ProjectProvidingFilter);
    private const string ContentKey = $"{nameof(ProjectProvidingFilter)}_content";

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Items[RunOnceMarker] != null)
        {
            return;
        }

        context.HttpContext.Items[RunOnceMarker] = true;
        if (!(context.HttpContext.User.Identity?.IsAuthenticated ?? false))
        {
            return;
        }

        var users = db.Users;
        var names = Enumerable.Aggregate(users, "", (current, user) => current + user.UserName + ", ");
        logger.LogInformation(message: "Found the following accounts: {names}", names);
        context.HttpContext.Items[ContentKey] = users;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}