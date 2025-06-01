namespace PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration
{
	/// <summary>
	/// Password validation requirements for JSON user data
	/// </summary>
	public class PasswordRequirements
	{
		/// <summary>
		/// Minimum password length
		/// </summary>
		public int MinLength { get; set; } = 6;

		/// <summary>
		/// Maximum password length
		/// </summary>
		public int MaxLength { get; set; } = 100;

		/// <summary>
		/// Require at least one digit
		/// </summary>
		public bool RequireDigit { get; set; } = true;

		/// <summary>
		/// Require at least one lowercase letter
		/// </summary>
		public bool RequireLowercase { get; set; } = true;

		/// <summary>
		/// Require at least one uppercase letter
		/// </summary>
		public bool RequireUppercase { get; set; } = true;

		/// <summary>
		/// Require at least one special character
		/// </summary>
		public bool RequireNonAlphanumeric { get; set; } = true;

		/// <summary>
		/// List of forbidden passwords that should not be accepted
		/// </summary>
		public List<string> ForbiddenPasswords { get; set; } = new()
		{
			"password", "123456", "qwerty", "admin", "user", "test",
			"welcome", "login", "default", "temp", "guest"
		};

		/// <summary>
		/// Forbidden password patterns (regex patterns)
		/// </summary>
		public List<string> ForbiddenPatterns { get; set; } = new()
		{
			@"^(.)\1{2,}$", // Repeated characters (aaa, 111, etc.)
			@"^(012|123|234|345|456|567|678|789|890|abc|bcd|cde|def|efg
				|fgh|ghi|hij|ijk|jkl|klm|lmn|mno|nop|opq|pqr|qrs
				|rst|stu|tuv|uvw|vwx|wxy|xyz)+$", // Sequential characters
		};

		/// <summary>
		/// Check password against common password dictionaries
		/// </summary>
		public bool CheckAgainstCommonPasswords { get; set; } = true;

		/// <summary>
		/// Maximum number of consecutive identical characters allowed
		/// </summary>
		public int MaxConsecutiveIdenticalChars { get; set; } = 2;
	}
}
