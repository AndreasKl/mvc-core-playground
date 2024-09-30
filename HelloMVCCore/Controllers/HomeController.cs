using System.Diagnostics;
using HelloMVCCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using HelloMVCCore.Models;
using Microsoft.AspNetCore.Authorization;

namespace HelloMVCCore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(new SampleViewModel(FirstName: "Andreas", LastName: "Kluth"));
    }

    [Authorize(Roles = "User", Policy = "SomePolicy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}