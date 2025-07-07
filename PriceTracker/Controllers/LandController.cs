using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTracker.Core.Models.Email;
using PriceTracker.Core.Services;
using PriceTracker.Models.LandingPage;

namespace PriceTracker.Controllers
{
	[AllowAnonymous]
	public class LandController : Controller
	{
		private readonly ILogger<LandController> _logger;
		private readonly IEmailService _emailService;

		public LandController(ILogger<LandController> logger, IEmailService emailService)
		{
			_logger = logger;
			_emailService = emailService;
		}

		[HttpGet]
		[Route("/")]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		[Route("/pricing")]
		public IActionResult Pricing()
		{
			ViewBag.AppName = "PriceTracker"; 
			ViewBag.CurrentYear = DateTime.Now.Year;

			return View();
		}

		[HttpGet]
		[Route("/contact")]
		public IActionResult Contact()
		{
			return View(new ContactViewModel());
		}

		[HttpPost]
		[Route("/contact")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Contact(ContactViewModel model)
		{
			_logger.LogInformation("Contact form submission started");

			// Check if this is an AJAX request
			bool isAjaxRequest = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

			_logger.LogInformation($"Request type: {(isAjaxRequest ? "AJAX" : "Regular")}");

			// Manual checkbox validation
			if (!model.IsNotRobot)
			{
				ModelState.AddModelError("IsNotRobot", "Please confirm you are not a robot");
			}

			// If validation fails
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Contact form validation failed");
				foreach (var error in ModelState)
				{
					_logger.LogWarning($"Validation error in {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
				}

				// Return different responses based on request type
				if (isAjaxRequest)
				{
					// Return JSON response for AJAX requests
					var errors = ModelState
						.Where(ms => ms.Value!.Errors.Count > 0)
						.ToDictionary(
							kvp => kvp.Key,
							kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
						);

					Response.StatusCode = 400; // Bad Request
					return Json(new
					{
						success = false,
						message = "Please fix the errors below and try again.",
						errors = errors
					});
				}
				else
				{
					// Return view for regular requests (fallback)
					return View(model);
				}
			}

			// Process the form submission
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

				// Return different responses based on request type
				if (isAjaxRequest)
				{
					// Return JSON success response for AJAX requests
					return Json(new
					{
						success = true,
						message = "Your message has been sent successfully! We'll get back to you within 24 hours.",
						data = new
						{
							submittedAt = contactFormEmail.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss"),
							name = model.Name,
							email = model.Email,
							subject = model.Subject
						}
					});
				}
				else
				{
					// Traditional redirect for regular requests (fallback)
					TempData["ContactSuccess"] = "Your message has been sent successfully! We'll get back to you within 24 hours.";
					return RedirectToAction("Contact");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error processing contact form from {model.Email}");

				// Return different responses based on request type
				if (isAjaxRequest)
				{
					// Return JSON error response for AJAX requests
					Response.StatusCode = 500; // Internal Server Error
					return Json(new
					{
						success = false,
						message = "Sorry, there was an error sending your message. Please try again or contact us directly.",
						error = "SERVER_ERROR"
					});
				}
				else
				{
					// Traditional error handling for regular requests (fallback)
					ModelState.AddModelError("", "Sorry, there was an error sending your message. Please try again or contact us directly.");
					return View(model);
				}
			}
		}

		[HttpGet]
		[Route("/documentation")]
		public IActionResult Documentation()
		{
			return View();
		}

		[HttpGet]
		[Route("/about")]
		public IActionResult About()
		{
			return View();
		}

		[HttpGet]
		[Route("/faq")]
		public IActionResult FAQ()
		{
			ViewData["Title"] = "Frequently Asked Questions";
			return View();
		}
	}
}
