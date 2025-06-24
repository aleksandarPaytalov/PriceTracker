using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models;
using System.Diagnostics;

namespace PriceTracker.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

	[AllowAnonymous]
    public IActionResult Index()
    {
		if (IsUserAuthenticated)
		{
			return View();
		}

		else
		{
			return View("LandingPage");
		}
	}

	public IActionResult LandingOption1()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
