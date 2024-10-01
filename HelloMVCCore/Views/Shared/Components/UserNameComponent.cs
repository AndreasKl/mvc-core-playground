using HelloMVCCore.Data;
using Microsoft.AspNetCore.Mvc;

namespace HelloMVCCore.Views.Shared.Components;

public class UserNameComponent(ApplicationDbContext applicationDbContext) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var roleNames = applicationDbContext.Roles.Select(r => r.Name).ToList();
        return View(roleNames);
    }
}
