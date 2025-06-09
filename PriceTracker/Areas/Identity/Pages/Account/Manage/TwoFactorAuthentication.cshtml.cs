using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Areas.Identity.Pages.Account.Manage
{
	/// <summary>
	/// Page model for managing two-factor authentication settings.
	/// Simplified version focused only on authenticator app functionality.
	/// </summary>
	[Authorize]
	public class TwoFactorAuthenticationModel : PageModel
	{
		#region Private Fields

		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly ILogger<TwoFactorAuthenticationModel> _logger;

		#endregion

		#region Constructor

		public TwoFactorAuthenticationModel(
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			ILogger<TwoFactorAuthenticationModel> logger)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Indicates whether the user has set up an authenticator app.
		/// </summary>
		[Display(Name = "Has Authenticator")]
		public bool HasAuthenticator { get; set; }

		/// <summary>
		/// The number of recovery codes remaining for the user.
		/// Used to alert users when codes are running low.
		/// </summary>
		[Display(Name = "Recovery Codes Left")]
		[Range(0, int.MaxValue, ErrorMessage = "Recovery codes count cannot be negative")]
		public int RecoveryCodesLeft { get; set; }

		/// <summary>
		/// Indicates whether two-factor authentication is enabled for the user.
		/// This is the main toggle for 2FA functionality.
		/// </summary>
		[BindProperty]
		[Display(Name = "Two-Factor Authentication Enabled")]
		public bool Is2faEnabled { get; set; }

		/// <summary>
		/// Indicates whether the current browser/device is remembered for 2FA.
		/// When true, the user won't be prompted for 2FA on this device.
		/// </summary>
		[Display(Name = "Machine Remembered")]
		public bool IsMachineRemembered { get; set; }

		/// <summary>
		/// Status message to display to the user (success, error, info).
		/// Automatically cleared after being displayed.
		/// </summary>
		[TempData]
		public string? StatusMessage { get; set; }

		/// <summary>
		/// Indicates whether the user can track cookies (for 2FA functionality).
		/// Required for 2FA to work properly due to privacy compliance.
		/// </summary>
		public bool CanTrack { get; set; }

		/// <summary>
		/// Indicates whether there were any errors loading the page.
		/// Used to show appropriate error messaging to users.
		/// </summary>
		public bool HasLoadingError { get; set; }

		/// <summary>
		/// User-friendly error message when something goes wrong.
		/// </summary>
		public string? ErrorMessage { get; set; }

		#endregion

		#region HTTP Action Handlers

		/// <summary>
		/// Handles GET requests to display the two-factor authentication page.
		/// Loads the current user's 2FA settings and status with comprehensive error handling.
		/// </summary>
		/// <returns>The page result, NotFound if user cannot be loaded, or error state if data loading fails.</returns>
		public async Task<IActionResult> OnGetAsync()
		{
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();

			try
			{
				_logger.LogInformation("Loading two-factor authentication page for user {UserId}",
					_userManager.GetUserId(User));

				var user = await GetCurrentUserAsync();
				if (user == null)
				{
					return HandleUserNotFound();
				}

				await LoadUserTwoFactorInfoAsync(user);

				stopwatch.Stop();
				_logger.LogInformation("Two-factor authentication page loaded successfully for user {UserId} in {ElapsedMs}ms",
					user.Id, stopwatch.ElapsedMilliseconds);

				return Page();
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				return HandleLoadingError(ex, stopwatch.ElapsedMilliseconds);
			}
		}

		/// <summary>
		/// Handles POST requests to forget the current browser for two-factor authentication.
		/// This will require the user to enter a 2FA code on their next login from this browser.
		/// </summary>
		/// <returns>Redirect to the same page with appropriate status message.</returns>
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
		{
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();

			try
			{
				_logger.LogInformation("Processing forget browser request for user {UserId}",
					_userManager.GetUserId(User));

				var user = await GetCurrentUserAsync();
				if (user == null)
				{
					return HandleUserNotFound();
				}

				await _signInManager.ForgetTwoFactorClientAsync();

				stopwatch.Stop();
				StatusMessage = "Success: The current browser has been forgotten. You will be prompted for your 2FA code on your next login from this browser.";

				_logger.LogInformation("User {UserId} successfully forgot current browser for 2FA in {ElapsedMs}ms",
					user.Id, stopwatch.ElapsedMilliseconds);

				return RedirectToPage();
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				return HandlePostError(ex, stopwatch.ElapsedMilliseconds);
			}
		}

		#endregion

		#region Private Helper Methods

		/// <summary>
		/// Safely retrieves the current user with proper error handling.
		/// </summary>
		/// <returns>The current user or null if not found.</returns>
		private async Task<User?> GetCurrentUserAsync()
		{
			try
			{
				var userId = _userManager.GetUserId(User);
				if (string.IsNullOrEmpty(userId))
				{
					_logger.LogWarning("Unable to determine user ID from claims principal");
					return null;
				}

				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					_logger.LogWarning("Unable to load user with ID '{UserId}' from database", userId);
				}

				return user;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving current user");
				return null;
			}
		}

		/// <summary>
		/// Loads the user's two-factor authentication information from the database.
		/// Includes recovery codes management.
		/// </summary>
		/// <param name="user">The user to load information for.</param>
		private async Task LoadUserTwoFactorInfoAsync(User user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			try
			{
				// Load 2FA status information with individual error handling
				HasAuthenticator = await SafeGetAuthenticatorStatusAsync(user);
				Is2faEnabled = await SafeGetTwoFactorEnabledAsync(user);
				IsMachineRemembered = await SafeGetMachineRememberedStatusAsync(user);
				RecoveryCodesLeft = await SafeGetRecoveryCodesCountAsync(user);

				// Check tracking consent for privacy compliance
				CanTrack = GetTrackingConsentStatus();

				_logger.LogDebug("Loaded 2FA info for user {UserId}: HasAuth={HasAuth}, Is2FA={Is2FA}, Machine={Machine}, Codes={Codes}, CanTrack={CanTrack}",
					user.Id, HasAuthenticator, Is2faEnabled, IsMachineRemembered, RecoveryCodesLeft, CanTrack);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Critical error loading 2FA information for user {UserId}", user.Id);
				SetSafeDefaults();
				throw;
			}
		}

		/// <summary>
		/// Safely retrieves authenticator status with error handling.
		/// </summary>
		private async Task<bool> SafeGetAuthenticatorStatusAsync(User user)
		{
			try
			{
				var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
				return !string.IsNullOrEmpty(authenticatorKey);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Error checking authenticator status for user {UserId}", user.Id);
				return false;
			}
		}

		/// <summary>
		/// Safely retrieves two-factor enabled status with error handling.
		/// </summary>
		private async Task<bool> SafeGetTwoFactorEnabledAsync(User user)
		{
			try
			{
				return await _userManager.GetTwoFactorEnabledAsync(user);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Error checking 2FA enabled status for user {UserId}", user.Id);
				return false;
			}
		}

		/// <summary>
		/// Safely retrieves machine remembered status with error handling.
		/// </summary>
		private async Task<bool> SafeGetMachineRememberedStatusAsync(User user)
		{
			try
			{
				return await _signInManager.IsTwoFactorClientRememberedAsync(user);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Error checking machine remembered status for user {UserId}", user.Id);
				return false;
			}
		}

		/// <summary>
		/// Safely retrieves recovery codes count with error handling.
		/// </summary>
		private async Task<int> SafeGetRecoveryCodesCountAsync(User user)
		{
			try
			{
				return await _userManager.CountRecoveryCodesAsync(user);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Error counting recovery codes for user {UserId}", user.Id);
				return 0;
			}
		}

		/// <summary>
		/// Gets the tracking consent status for privacy compliance.
		/// </summary>
		private bool GetTrackingConsentStatus()
		{
			try
			{
				var consentFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Http.Features.ITrackingConsentFeature>();
				return consentFeature?.CanTrack ?? true;
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Error checking tracking consent status");
				return false; // Err on the side of caution for privacy
			}
		}

		/// <summary>
		/// Sets safe default values when data loading fails.
		/// </summary>
		private void SetSafeDefaults()
		{
			HasAuthenticator = false;
			Is2faEnabled = false;
			IsMachineRemembered = false;
			RecoveryCodesLeft = 0;
			CanTrack = false;
		}

		/// <summary>
		/// Handles the case when a user cannot be found.
		/// </summary>
		private IActionResult HandleUserNotFound()
		{
			var userId = _userManager.GetUserId(User);
			_logger.LogWarning("Unable to load user with ID '{UserId}' for 2FA page", userId);

			return NotFound($"Unable to load user with ID '{userId}'.");
		}

		/// <summary>
		/// Handles errors during page loading.
		/// </summary>
		private IActionResult HandleLoadingError(Exception ex, long elapsedMs)
		{
			var userId = _userManager.GetUserId(User);
			_logger.LogError(ex, "Error loading two-factor authentication page for user {UserId} after {ElapsedMs}ms",
				userId, elapsedMs);

			HasLoadingError = true;
			ErrorMessage = "We're having trouble loading your security settings. Please refresh the page or try again later.";
			StatusMessage = "Error: Unable to load two-factor authentication settings. Please try again.";

			SetSafeDefaults();
			return Page();
		}

		/// <summary>
		/// Handles errors during POST operations.
		/// </summary>
		private IActionResult HandlePostError(Exception ex, long elapsedMs)
		{
			var userId = _userManager.GetUserId(User);
			_logger.LogError(ex, "Error processing forget browser request for user {UserId} after {ElapsedMs}ms",
				userId, elapsedMs);

			StatusMessage = "Error: Unable to forget this browser. Please try again.";
			return RedirectToPage();
		}

		#endregion
	}

	/// <summary>
	/// Navigation constants for manage pages
	/// </summary>
	public static class ManageNavConstants
	{
		public const string GenerateRecoveryCodes = "GenerateRecoveryCodes";

		/// <summary>
		/// Gets a user-friendly display name for a navigation page.
		/// </summary>
		/// <param name="page">The page identifier.</param>
		/// <returns>A human-readable page name.</returns>
		public static string GetDisplayName(string page)
		{
			return page switch
			{
				"Index" => "Profile",
				"Email" => "Email",
				"ChangePassword" => "Password",
				"TwoFactorAuthentication" => "Two-factor authentication",
				"ExternalLogins" => "External logins",
				"PersonalData" => "Personal data",
				"DeletePersonalData" => "Delete data",
				"EnableAuthenticator" => "Enable authenticator",
				GenerateRecoveryCodes => "Recovery codes",
				"Disable2fa" => "Disable 2FA",
				"ResetAuthenticator" => "Reset authenticator",
				_ => page
			};
		}

		/// <summary>
		/// Checks if a page requires two-factor authentication to be enabled.
		/// </summary>
		/// <param name="page">The page identifier to check.</param>
		/// <returns>True if the page requires 2FA to be enabled.</returns>
		public static bool RequiresTwoFactorEnabled(string page)
		{
			return page switch
			{
				GenerateRecoveryCodes => true,
				"Disable2fa" => true,
				"ResetAuthenticator" => true,
				_ => false
			};
		}
	}
}