using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels
{
	/// <summary>
	/// DTO for Role JSON deserialization with validation attributes
	/// </summary>
	public class RoleJsonDto
	{
		/// <summary>
		/// Unique identifier for the role (GUID format)
		/// </summary>
		[Required(ErrorMessage = "Role ID is required")]
		[StringLength(36, MinimumLength = 36, ErrorMessage = "Role ID must be a valid GUID")]
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// Display name of the role
		/// </summary>
		[Required(ErrorMessage = "Role name is required")]
		[StringLength(256, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 256 characters")]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Normalized name for the role (automatically generated if not provided)
		/// </summary>
		[StringLength(256, ErrorMessage = "Normalized name cannot exceed 256 characters")]
		public string? NormalizedName { get; set; }

		/// <summary>
		/// Concurrency stamp for the role (automatically generated if not provided)
		/// </summary>
		public string? ConcurrencyStamp { get; set; }
	}

}
