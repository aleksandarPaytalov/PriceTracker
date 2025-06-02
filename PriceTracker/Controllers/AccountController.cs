using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PriceTracker.Core.Services;
using PriceTracker.Infrastructure.Data.Models;
using System.Text;

namespace PriceTracker.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly IEmailService _emailService;
		private readonly ILogger<AccountController> _logger;

		public AccountController(
			UserManager<User> userManager,
			IEmailService emailService,
			ILogger<AccountController> logger)
		{
			_userManager = userManager;
			_emailService = emailService;
			_logger = logger;
		}

		[HttpPost]
		public async Task<IActionResult> ResendEmailConfirmation()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound("User not found");
			}

			if (await _userManager.IsEmailConfirmedAsync(user))
			{
				TempData["StatusMessage"] = "Your email is already confirmed.";
				return RedirectToAction("Index", "Home");
			}

			try
			{
				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var encodedCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(code));

				var callbackUrl = Url.Page(
					"/Account/ConfirmEmail",
					pageHandler: null,
					values: new { area = "Identity", userId = user.Id, code = encodedCode },
					protocol: Request.Scheme);

				await _emailService.SendEmailConfirmationAsync(user.Email, callbackUrl);

				TempData["StatusMessage"] = "Email confirmation sent. Please check your email.";
				_logger.LogInformation("Email confirmation resent to {Email}", user.Email);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to resend email confirmation to {Email}", user.Email);
				TempData["ErrorMessage"] = "Failed to send confirmation email. Please try again later.";
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> EmailStatus()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound();
			}

			ViewBag.IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
			ViewBag.Email = user.Email;

			return View();
		}
	}
}