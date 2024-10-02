using System.Diagnostics;
using HelloMVCCore.Data;
using HelloMVCCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HelloMVCCore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View(new SampleViewModel(FirstName: "Andreas", LastName: "Kluth"));
    }

    [HttpPost]
    public async Task<IActionResult> SetThing(int thingID, string thingRole)
    {
        var applicationUser = await _userManager.GetUserAsync(User);
        if (applicationUser == null)
        {
            return NotFound();
        }

        applicationUser.CurrentThingID = thingID;
        applicationUser.CurrentThingRole = thingRole;
        var result = await _userManager.UpdateAsync(applicationUser);
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        return RedirectToAction("Index");
    }

    [Authorize(Roles = "User", Policy = "SomePolicy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
