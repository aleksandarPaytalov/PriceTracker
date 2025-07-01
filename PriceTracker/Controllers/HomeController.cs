using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTracker.Core.Models.Email;
using PriceTracker.Core.Services;
using PriceTracker.Models;
using PriceTracker.Models.LandingPage;
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
		_logger.LogInformation("Contact form submission started");

		// ? ADDED: Manual checkbox validation
		if (!model.IsNotRobot)
		{
			ModelState.AddModelError("IsNotRobot", "Please confirm you are not a robot");
		}

		if (!ModelState.IsValid)
		{
			_logger.LogWarning("Contact form validation failed");
			foreach (var error in ModelState)
			{
				_logger.LogWarning($"Validation error in {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
			}
			return View(model);
		}

		// Rest of your method stays the same...
		try
		{
			_logger.LogInformation($"Preparing to send contact email from {model.Email}");

			var contactFormEmail = new ContactFormEmailViewModel
			{
				Name = model.Name,
				Email = model.Email,
				Subject = model.Subject,
				Message = model.Message,
				SubmittedAt = DateTime.Now,
				IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
				UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
			};

			_logger.LogInformation("Calling SendContactFormEmailAsync");

			await _emailService.SendContactFormEmailAsync(contactFormEmail);

			_logger.LogInformation($"Contact form email sent successfully from {model.Email}");

			TempData["ContactSuccess"] = "Your message has been sent successfully! We'll get back to you within 24 hours.";

			return RedirectToAction("Contact");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Error processing contact form from {model.Email}");
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
