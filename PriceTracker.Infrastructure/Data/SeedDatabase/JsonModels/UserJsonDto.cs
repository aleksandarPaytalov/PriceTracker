using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels
{
	/// <summary>
	/// DTO for User JSON deserialization with plain text password and validation attributes
	/// </summary>
	public class UserJsonDto
	{
		/// <summary>
		/// Unique identifier for the user (GUID format)
		/// </summary>
		[Required(ErrorMessage = "User ID is required")]
		[StringLength(36, MinimumLength = 36, ErrorMessage = "User ID must be a valid GUID")]
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// Username for the user
		/// </summary>
		[Required(ErrorMessage = "Username is required")]
		[StringLength(256, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 256 characters")]
		public string UserName { get; set; } = string.Empty;

		/// <summary>
		/// Normalized username (automatically generated if not provided)
		/// </summary>
		[StringLength(256, ErrorMessage = "Normalized username cannot exceed 256 characters")]
		public string? NormalizedUserName { get; set; }

		/// <summary>
		/// Email address for the user
		/// </summary>
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		[StringLength(256, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 256 characters")]
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Normalized email (automatically generated if not provided)
		/// </summary>
		[StringLength(256, ErrorMessage = "Normalized email cannot exceed 256 characters")]
		public string? NormalizedEmail { get; set; }

		/// <summary>
		/// Indicates if the email is confirmed
		/// </summary>
		public bool EmailConfirmed { get; set; } = true;

		/// <summary>
		/// Plain text password that will be hashed by UserBuilder
		/// </summary>
		[Required(ErrorMessage = "Password is required")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
		public string Password { get; set; } = string.Empty;

		/// <summary>
		/// Security stamp (automatically generated if not provided)
		/// </summary>
		public string? SecurityStamp { get; set; }

		/// <summary>
		/// Concurrency stamp (automatically generated if not provided)
		/// </summary>
		public string? ConcurrencyStamp { get; set; }

		/// <summary>
		/// User creation timestamp (defaults to current UTC time if not provided)
		/// </summary>
		public DateTime? CreatedAt { get; set; }
	}
}
