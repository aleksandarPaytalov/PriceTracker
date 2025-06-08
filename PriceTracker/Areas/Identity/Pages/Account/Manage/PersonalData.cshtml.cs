using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PriceTracker.Infrastructure.Data.Models;
using System.Text;
using System.Text.Json;

namespace PriceTracker.Areas.Identity.Pages.Account.Manage
{
	public class PersonalDataModel : PageModel
	{
		private readonly UserManager<User> _userManager;
		private readonly ILogger<PersonalDataModel> _logger;

		// Cached JsonSerializer options for better performance
		private static readonly JsonSerializerOptions JsonOptions = new()
		{
			WriteIndented = true
		};

		public PersonalDataModel(
			UserManager<User> userManager,
			ILogger<PersonalDataModel> logger)
		{
			_userManager = userManager;
			_logger = logger;
		}

		/// <summary>
		/// User's username
		/// </summary>
		public string Username { get; set; } = string.Empty;

		/// <summary>
		/// User's email address
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Whether email is confirmed
		/// </summary>
		public bool IsEmailConfirmed { get; set; }

		/// <summary>
		/// User's phone number
		/// </summary>
		public string? PhoneNumber { get; set; }

		/// <summary>
		/// Whether phone number is confirmed
		/// </summary>
		public bool IsPhoneConfirmed { get; set; }

		/// <summary>
		/// Whether two-factor authentication is enabled
		/// </summary>
		public bool TwoFactorEnabled { get; set; }

		/// <summary>
		/// Account creation date
		/// </summary>
		public DateTime AccountCreated { get; set; }

		/// <summary>
		/// Security level description
		/// </summary>
		public string SecurityLevel { get; set; } = "Basic";

		/// <summary>
		/// Status message for user feedback
		/// </summary>
		[TempData]
		public string StatusMessage { get; set; } = string.Empty;

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			LoadUserData(user);
			return Page();
		}

		/// <summary>
		/// Handles downloading user's personal data as JSON
		/// </summary>
		public async Task<IActionResult> OnPostDownloadPersonalDataAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			_logger.LogInformation("User with ID '{UserId}' requested their personal data download.", _userManager.GetUserId(User));

			// Prepare user data for download
			var personalData = await PreparePersonalDataAsync(user);

			var json = JsonSerializer.Serialize(personalData, JsonOptions);

			var fileName = $"PersonalData-{user.UserName}-{DateTime.UtcNow:yyyyMMdd}.json";
			return File(Encoding.UTF8.GetBytes(json), "application/json", fileName);
		}

		/// <summary>
		/// Handles exporting user data in a structured format
		/// </summary>
		public async Task<IActionResult> OnPostExportDataAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			_logger.LogInformation("User with ID '{UserId}' requested data export.", _userManager.GetUserId(User));

			try
			{
				// Create a more structured export with app-specific data
				var exportData = await PrepareExportDataAsync(user);

				var json = JsonSerializer.Serialize(exportData, JsonOptions);

				var fileName = $"DataExport-{user.UserName}-{DateTime.UtcNow:yyyyMMdd}.json";

				StatusMessage = "Your data export has been generated successfully.";
				return File(Encoding.UTF8.GetBytes(json), "application/json", fileName);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while exporting data for user {UserId}", user.Id);
				StatusMessage = "Error: Unable to export your data. Please try again.";
				return RedirectToPage();
			}
		}

		/// <summary>
		/// Handles permanent deletion of user account and all associated data
		/// </summary>
		public async Task<IActionResult> OnPostDeletePersonalDataAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			// Require the user to be signed in to delete their account
			if (!await _userManager.CheckPasswordAsync(user, Request.Form["Password"]))
			{
				StatusMessage = "Error: Invalid password. Account deletion cancelled.";
				return RedirectToPage();
			}

			_logger.LogWarning("User with ID '{UserId}' requested account deletion.", user.Id);

			try
			{
				// Delete all user-related data before deleting the user
				await DeleteUserRelatedDataAsync(user);

				var result = await _userManager.DeleteAsync(user);
				var userId = await _userManager.GetUserIdAsync(user);

				if (!result.Succeeded)
				{
					var errors = string.Join(", ", result.Errors.Select(e => e.Description));
					throw new InvalidOperationException($"Unexpected error occurred deleting user: {errors}");
				}

				_logger.LogInformation("User with ID '{UserId}' successfully deleted their account.", userId);

				return Redirect("~/");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while deleting account for user {UserId}", user.Id);
				StatusMessage = "Error: Unable to delete account. Please contact support.";
				return RedirectToPage();
			}
		}

		/// <summary>
		/// Loads user data into the page model properties
		/// </summary>
		private void LoadUserData(User user)
		{
			Username = user.UserName ?? string.Empty;
			Email = user.Email ?? string.Empty;
			IsEmailConfirmed = user.EmailConfirmed;
			PhoneNumber = user.PhoneNumber;
			IsPhoneConfirmed = user.PhoneNumberConfirmed;
			TwoFactorEnabled = user.TwoFactorEnabled;

			AccountCreated = user.CreatedAt ?? DateTime.UtcNow;

			// Determine security level based on user settings
			SecurityLevel = CalculateSecurityLevel(user);
		}

		/// <summary>
		/// Prepares user's personal data for download
		/// </summary>
		private async Task<Dictionary<string, object>> PreparePersonalDataAsync(User user)
		{
			var personalDataProps = typeof(User).GetProperties().Where(
				prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

			var personalData = new Dictionary<string, object>();

			foreach (var prop in personalDataProps)
			{
				var value = prop.GetValue(user)?.ToString() ?? "null";
				personalData.Add(prop.Name, value);
			}

			// Add additional computed data
			personalData.Add("AccountCreatedAt", user.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss UTC") ?? "Unknown");
			personalData.Add("SecurityLevel", CalculateSecurityLevel(user));

			// Add external logins
			var logins = await _userManager.GetLoginsAsync(user);
			personalData.Add("ExternalLogins", logins.Select(l => new { l.LoginProvider, l.ProviderDisplayName }));

			return personalData;
		}

		/// <summary>
		/// Prepares structured export data including app-specific information
		/// </summary>
		private async Task<object> PrepareExportDataAsync(User user)
		{
			var basicData = await PreparePersonalDataAsync(user);

			return new
			{
				ExportInfo = new
				{
					ExportedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
					ExportVersion = "1.0",
					ApplicationName = "PriceTracker"
				},
				UserAccount = basicData,
				SecuritySettings = new
				{
					TwoFactorEnabled = user.TwoFactorEnabled,
					EmailConfirmed = user.EmailConfirmed,
					PhoneNumberConfirmed = user.PhoneNumberConfirmed,
					LockoutEnabled = user.LockoutEnabled,
					SecurityLevel = CalculateSecurityLevel(user)
				},
				// You can add more app-specific data here like:
				// PriceTrackingData = await GetUserPriceTrackingDataAsync(user),
				// UserPreferences = await GetUserPreferencesAsync(user),
				// etc.
			};
		}

		/// <summary>
		/// Deletes all user-related data before account deletion
		/// </summary>
		private async Task DeleteUserRelatedDataAsync(User user)
		{
			// This is where you would delete all user-related data from your application
			// For example:
			// - Price tracking data
			// - User preferences
			// - Notifications
			// - etc.

			_logger.LogInformation("Deleting all related data for user {UserId}", user.Id);

			// TODO: Implement deletion of user-related data
			// await _priceTrackingService.DeleteUserDataAsync(user.Id);
			// await _notificationService.DeleteUserNotificationsAsync(user.Id);

			await Task.CompletedTask; // Placeholder
		}

		/// <summary>
		/// Calculates user's security level based on enabled features
		/// </summary>
		private static string CalculateSecurityLevel(User user)
		{
			var score = 0;

			if (user.EmailConfirmed) score += 1;
			if (user.PhoneNumberConfirmed) score += 1;
			if (user.TwoFactorEnabled) score += 2;

			return score switch
			{
				>= 4 => "High",
				>= 2 => "Medium",
				_ => "Basic"
			};
		}
	}
}