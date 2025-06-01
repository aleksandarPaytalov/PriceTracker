using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels
{
	/// <summary>
	/// DTO for batch user-role operations
	/// </summary>
	public class BatchUserRoleJsonDto
	{
		/// <summary>
		/// User ID for the mappings
		/// </summary>
		[Required(ErrorMessage = "User ID is required")]
		public string UserId { get; set; } = string.Empty;

		/// <summary>
		/// List of role IDs to assign to the user
		/// </summary>
		[Required(ErrorMessage = "At least one role ID is required")]
		[MinLength(1, ErrorMessage = "At least one role ID must be provided")]
		public List<string> RoleIds { get; set; } = new();

		/// <summary>
		/// Operation type (Add, Remove, Replace)
		/// </summary>
		public string Operation { get; set; } = "Add";
	}
}
