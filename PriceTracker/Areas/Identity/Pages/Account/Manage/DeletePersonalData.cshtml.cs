using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Areas.Identity.Pages.Account.Manage
{
	public class DeletePersonalDataModel : PageModel
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly ILogger<DeletePersonalDataModel> _logger;

		public DeletePersonalDataModel(
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			ILogger<DeletePersonalDataModel> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
		}

		/// <summary>
		/// Input model for account deletion
		/// </summary>
		[BindProperty]
		public InputModel Input { get; set; } = new();

		/// <summary>
		/// Input model class for deletion form
		/// </summary>
		public class InputModel
		{
			/// <summary>
			/// User's current password for verification
			/// </summary>
			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; } = string.Empty;
		}

		/// <summary>
		/// Whether password is required for this user
		/// </summary>
		public bool RequirePassword { get; set; }

		/// <summary>
		/// Load the deletion page
		/// </summary>
		public async Task<IActionResult> OnGet()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			RequirePassword = await _userManager.HasPasswordAsync(user);
			return Page();
		}

		/// <summary>
		/// Handle account deletion request
		/// </summary>
		public async Task<IActionResult> OnPostAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			RequirePassword = await _userManager.HasPasswordAsync(user);
			if (RequirePassword)
			{
				if (!await _userManager.CheckPasswordAsync(user, Input.Password))
				{
					ModelState.AddModelError(string.Empty, "Incorrect password.");
					return Page();
				}
			}

			_logger.LogWarning("User with ID '{UserId}' is deleting their account.", user.Id);

			try
			{
				// Delete all user-related data before deleting the user
				await DeleteUserRelatedDataAsync(user);

				// Delete the user account
				var result = await _userManager.DeleteAsync(user);
				var userId = await _userManager.GetUserIdAsync(user);

				if (!result.Succeeded)
				{
					throw new InvalidOperationException($"Unexpected error occurred deleting user.");
				}

				// Sign out the user
				await _signInManager.SignOutAsync();

				_logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

				return Redirect("~/");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while deleting account for user {UserId}", user.Id);
				ModelState.AddModelError(string.Empty, "An error occurred while deleting your account. Please try again.");
				return Page();
			}
		}

		/// <summary>
		/// Delete all user-related data before deleting the account
		/// </summary>
		private async Task DeleteUserRelatedDataAsync(User user)
		{
			_logger.LogInformation("Deleting all related data for user {UserId}", user.Id);

			// This is where you would delete all user-related data from your application
			// For example:
			// - Price tracking data
			// - User preferences  
			// - Notifications
			// - etc.

			// TODO: Implement deletion of user-related data when services are available
			// await _priceTrackingService.DeleteUserDataAsync(user.Id);
			// await _notificationService.DeleteUserNotificationsAsync(user.Id);
			// await _userPreferencesService.DeleteUserPreferencesAsync(user.Id);

			await Task.CompletedTask; // Placeholder
		}
	}
}