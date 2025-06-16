// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
namespace PriceTracker.Areas.Identity.Pages.Account
{
    public class LoginWithRecoveryCodeModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginWithRecoveryCodeModel> _logger;

        public LoginWithRecoveryCodeModel(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<LoginWithRecoveryCodeModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [BindProperty]
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;

            return Page();
        }

		// Replace your existing OnPostAsync method in LoginWithRecoveryCode.cshtml.cs with this:

		// Замени OnPostAsync метода в LoginWithRecoveryCode.cshtml.cs:

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Model state invalid for recovery code login");
				return Page();
			}

			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				_logger.LogError("Unable to load two-factor authentication user for recovery code login");
				throw new InvalidOperationException($"Unable to load two-factor authentication user.");
			}

			try
			{
				var originalCode = Input.RecoveryCode;

				// ВАЖНО: НЕ почиствай кода! ASP.NET Identity очаква точния формат от базата
				// Само премахни излишни spaces в началото/края
				var recoveryCode = originalCode.Trim();

				_logger.LogInformation("=== RECOVERY CODE DEBUG START ===");
				_logger.LogInformation("User ID: {UserId}", user.Id);
				_logger.LogInformation("Original Code: '{OriginalCode}'", originalCode);
				_logger.LogInformation("Trimmed Code: '{TrimmedCode}'", recoveryCode);
				_logger.LogInformation("Code Length: {Length}", recoveryCode.Length);

				// Проверка на recovery codes
				var remainingCodesBefore = await _userManager.CountRecoveryCodesAsync(user);
				_logger.LogInformation("Recovery Codes Before: {RemainingCodes}", remainingCodesBefore);

				if (remainingCodesBefore == 0)
				{
					_logger.LogWarning("User {UserId} has no recovery codes remaining", user.Id);
					ModelState.AddModelError(string.Empty, "You have no recovery codes remaining.");
					return Page();
				}

				// Опитай се да влезеш с точния код (С тире ако са въведени)
				_logger.LogInformation("About to call TwoFactorRecoveryCodeSignInAsync with: '{Code}'", recoveryCode);
				var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

				// Проверка на recovery codes след опита
				var remainingCodesAfter = await _userManager.CountRecoveryCodesAsync(user);
				_logger.LogInformation("Recovery Codes After: {RemainingCodes}", remainingCodesAfter);
				_logger.LogInformation("Result.Succeeded: {Succeeded}", result.Succeeded);
				_logger.LogInformation("=== RECOVERY CODE DEBUG END ===");

				if (result.Succeeded)
				{
					_logger.LogInformation("SUCCESS: User {UserId} logged in with recovery code", user.Id);
					return LocalRedirect(returnUrl ?? Url.Content("~/"));
				}

				if (result.IsLockedOut)
				{
					_logger.LogWarning("LOCKOUT: User {UserId} account locked out", user.Id);
					return RedirectToPage("./Lockout");
				}

				if (result.IsNotAllowed)
				{
					_logger.LogWarning("NOT ALLOWED: User {UserId} sign-in not allowed", user.Id);
					ModelState.AddModelError(string.Empty, "Sign-in not allowed. Please verify your account.");
					return Page();
				}

				// Анализ защо не успя
				if (remainingCodesBefore > remainingCodesAfter)
				{
					_logger.LogWarning("ALREADY USED: Valid recovery code but already consumed");
					ModelState.AddModelError(string.Empty, "This recovery code has already been used.");
				}
				else
				{
					_logger.LogWarning("INVALID CODE: Recovery code was not recognized");
					ModelState.AddModelError(string.Empty, "Invalid recovery code. Please include the dash (e.g., TWQDK-V5C2F).");
				}

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "EXCEPTION during recovery code login for user {UserId}", user?.Id);
				ModelState.AddModelError(string.Empty, "An error occurred during login.");
				return Page();
			}
		}
	}
}
