using Microsoft.AspNetCore.Mvc;
using PriceTracker.Core.Services;
using PriceTracker.Models;
using System.Diagnostics;

namespace PriceTracker.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
	private readonly IEmailService _emailService;

	public HomeController(ILogger<HomeController> logger, IEmailService emailService)
    {
        _logger = logger;
		_emailService = emailService;
    }

    public IActionResult Index()
    {	
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
