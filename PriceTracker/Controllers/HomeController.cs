using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models;
using PriceTracker.Models.LandingPage;
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

	[AllowAnonymous]
	public IActionResult About()
	{
		return View();
	}

	public IActionResult LandingOption1()
	{
		return View();
	}

	// Add these methods to your existing HomeController.cs

	[HttpGet]
	[AllowAnonymous]
	public IActionResult Contact()
	{
		return View(new ContactViewModel());
	}

	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Contact(ContactViewModel model)
	{
		if (!ModelState.IsValid)
		{
			// Return the view with validation errors
			return View(model);
		}

		try
		{
			// TODO: In the next steps, we'll implement email sending here
			// For now, we'll simulate success
			await Task.Delay(1000); // Simulate processing time

			// Add success message to TempData for display
			TempData["ContactSuccess"] = "Your message has been sent successfully! We'll get back to you within 24 hours.";

			// Redirect to avoid resubmission
			return RedirectToAction("Contact");
		}
		catch (Exception ex)
		{
			// Log the error (you'll want to use proper logging)
			// _logger.LogError(ex, "Error sending contact form email");

			// Add error message
			ModelState.AddModelError("", "Sorry, there was an error sending your message. Please try again or contact us directly.");
			return View(model);
		}
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
