// ==========================================
// COMPLETE LoginWith2fa.cshtml.cs (Areas/Identity/Pages/Account/)
// ==========================================

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Areas.Identity.Pages.Account
{
	public class LoginWith2faModel : PageModel
	{
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;
		private readonly ILogger<LoginWith2faModel> _logger;

		public LoginWith2faModel(
			SignInManager<User> signInManager,
			UserManager<User> userManager,
			ILogger<LoginWith2faModel> logger)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
		}

		// ==========================================
		// CRITICAL: Properly bound properties
		// ==========================================

		[BindProperty]
		public InputModel Input { get; set; }

		public bool RememberMe { get; set; }

		public string ReturnUrl { get; set; }

		// ==========================================
		// FIXED: Enhanced InputModel with validation
		// ==========================================
		public class InputModel
		{
			[Required(ErrorMessage = "Please enter your authenticator code.")]
			[StringLength(6, ErrorMessage = "The authenticator code must be exactly {1} digits.", MinimumLength = 6)]
			[DataType(DataType.Text)]
			[Display(Name = "Authenticator code")]
			[RegularExpression(@"^\d{6}$", ErrorMessage = "Please enter exactly 6 digits.")]
			public string TwoFactorCode { get; set; } = string.Empty; // Initialize to prevent null

			[Display(Name = "Remember this machine")]
			public bool RememberMachine { get; set; }
		}

		// ==========================================
		// OnGetAsync with debugging
		// ==========================================
		public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
		{
			_logger.LogInformation("=== 2FA GET REQUEST ===");
			_logger.LogInformation($"RememberMe: {rememberMe}");
			_logger.LogInformation($"ReturnUrl: {returnUrl}");

			// Initialize Input if null
			if (Input == null)
			{
				Input = new InputModel();
				_logger.LogInformation("Input model initialized");
			}

			// Check for TwoFactorUserId cookie
			var twoFactorCookie = Request.Cookies[IdentityConstants.TwoFactorUserIdScheme];
			_logger.LogInformation($"TwoFactorUserId cookie: {(string.IsNullOrEmpty(twoFactorCookie) ? "NULL" : "EXISTS")}");

			// Ensure the user has gone through the username & password screen first
			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

			if (user == null)
			{
				_logger.LogError("Unable to load two-factor authentication user");
				throw new InvalidOperationException($"Unable to load two-factor authentication user.");
			}

			_logger.LogInformation($"2FA user loaded: {user.Email}");

			ReturnUrl = returnUrl;
			RememberMe = rememberMe;

			return Page();
		}

		// ==========================================
		// OnPostAsync with comprehensive debugging
		// ==========================================
		public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
		{
			_logger.LogInformation("=== 2FA POST REQUEST START ===");

			// ==========================================
			// CRITICAL: Check Input object
			// ==========================================
			if (Input == null)
			{
				_logger.LogError("❌ Input object is NULL!");

				// Try to create a new Input and bind manually
				Input = new InputModel();

				// Manual binding attempt
				var twoFactorCodeValue = Request.Form["Input.TwoFactorCode"].FirstOrDefault()
					?? Request.Form["TwoFactorCode"].FirstOrDefault()
					?? Request.Form["Input_TwoFactorCode"].FirstOrDefault();

				var rememberMachineValue = Request.Form["Input.RememberMachine"].FirstOrDefault()
					?? Request.Form["RememberMachine"].FirstOrDefault()
					?? Request.Form["Input_RememberMachine"].FirstOrDefault();

				_logger.LogInformation($"Manual binding attempt:");
				_logger.LogInformation($"  TwoFactorCode from form: '{twoFactorCodeValue}'");
				_logger.LogInformation($"  RememberMachine from form: '{rememberMachineValue}'");

				if (!string.IsNullOrEmpty(twoFactorCodeValue))
				{
					Input.TwoFactorCode = twoFactorCodeValue;
				}

				if (bool.TryParse(rememberMachineValue, out bool rememberMachineParsed))
				{
					Input.RememberMachine = rememberMachineParsed;
				}
			}

			// ==========================================
			// SAFE: Handle potentially null TwoFactorCode
			// ==========================================
			var rawCode = Input?.TwoFactorCode ?? string.Empty;
			var cleanCode = rawCode.Replace(" ", "").Replace("-", "");

			_logger.LogInformation($"Raw code: '{rawCode}'");
			_logger.LogInformation($"Clean code: '{cleanCode}'");
			_logger.LogInformation($"Code length: {cleanCode.Length}");
			_logger.LogInformation($"RememberMe: {rememberMe}");
			_logger.LogInformation($"Input.RememberMachine: {Input?.RememberMachine ?? false}");
			_logger.LogInformation($"ReturnUrl: {returnUrl}");

			// ==========================================
			// DEBUG: Log all form data
			// ==========================================
			_logger.LogInformation("=== FORM DATA ===");
			foreach (var formField in Request.Form)
			{
				_logger.LogInformation($"  {formField.Key} = '{formField.Value}'");
			}

			// ==========================================
			// DEBUG: Check ModelState
			// ==========================================
			_logger.LogInformation($"ModelState.IsValid: {ModelState.IsValid}");
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("ModelState errors:");
				foreach (var modelError in ModelState)
				{
					foreach (var error in modelError.Value.Errors)
					{
						_logger.LogWarning($"  [{modelError.Key}]: {error.ErrorMessage}");
					}
				}
			}

			// ==========================================
			// VALIDATION: Check if code is provided
			// ==========================================
			if (string.IsNullOrWhiteSpace(cleanCode))
			{
				_logger.LogWarning("❌ No 2FA code provided");
				ModelState.AddModelError(nameof(Input.TwoFactorCode), "Please enter your authenticator code.");
				return Page();
			}

			if (cleanCode.Length != 6)
			{
				_logger.LogWarning($"❌ Invalid code length: {cleanCode.Length} (expected 6)");
				ModelState.AddModelError(nameof(Input.TwoFactorCode), "Authenticator code must be exactly 6 digits.");
				return Page();
			}

			returnUrl = returnUrl ?? Url.Content("~/");

			// ==========================================
			// Get and validate user
			// ==========================================
			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				_logger.LogError("❌ GetTwoFactorAuthenticationUserAsync returned null");
				throw new InvalidOperationException($"Unable to load two-factor authentication user.");
			}

			_logger.LogInformation($"✅ 2FA user loaded: {user.Email}");

			// ==========================================
			// Check user's 2FA setup
			// ==========================================
			var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
			var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

			_logger.LogInformation($"Has authenticator key: {!string.IsNullOrEmpty(authenticatorKey)}");
			_logger.LogInformation($"2FA enabled: {isTwoFactorEnabled}");

			if (string.IsNullOrEmpty(authenticatorKey) || !isTwoFactorEnabled)
			{
				_logger.LogError("❌ 2FA not properly configured for user");
				ModelState.AddModelError(string.Empty, "Two-factor authentication is not properly set up.");
				return Page();
			}

			// ==========================================
			// Manual TOTP verification
			// ==========================================
			_logger.LogInformation("=== MANUAL TOTP TEST ===");
			var isValidManual = await _userManager.VerifyTwoFactorTokenAsync(
				user, TokenOptions.DefaultAuthenticatorProvider, cleanCode);
			_logger.LogInformation($"Manual verification: {isValidManual}");

			// ==========================================
			// Official sign-in attempt
			// ==========================================
			_logger.LogInformation("=== SIGN-IN ATTEMPT ===");
			var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
				cleanCode, rememberMe, Input?.RememberMachine ?? false);

			_logger.LogInformation($"SignIn result: Succeeded={result.Succeeded}, IsLockedOut={result.IsLockedOut}");

			if (result.Succeeded)
			{
				_logger.LogInformation("✅ 2FA login successful!");
				return LocalRedirect(returnUrl);
			}
			else if (result.IsLockedOut)
			{
				_logger.LogWarning("❌ User account locked out");
				return RedirectToPage("./Lockout");
			}
			else
			{
				_logger.LogWarning("❌ 2FA login failed");
				ModelState.AddModelError(string.Empty, "Invalid authenticator code. Please check your app and try again.");
				return Page();
			}
		}
	}
}