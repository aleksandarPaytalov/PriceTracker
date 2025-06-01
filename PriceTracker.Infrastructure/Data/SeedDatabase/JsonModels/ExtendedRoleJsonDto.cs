using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels
{
	/// <summary>
	/// Extended DTO for role import scenarios with additional metadata
	/// </summary>
	public class ExtendedRoleJsonDto : RoleJsonDto
	{
		/// <summary>
		/// Optional description of the role's purpose
		/// </summary>
		[StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
		public string? Description { get; set; }

		/// <summary>
		/// Indicates if this role should be created in all environments
		/// </summary>
		public bool IsSystemRole { get; set; } = true;

		/// <summary>
		/// Environment restrictions (empty means all environments)
		/// </summary>
		public List<string> Environments { get; set; } = new();

		/// <summary>
		/// Role permissions or claims (for future extensibility)
		/// </summary>
		public List<string> Permissions { get; set; } = new();

		/// <summary>
		/// Role hierarchy level (for organizational purposes)
		/// </summary>
		[Range(0, 100, ErrorMessage = "Hierarchy level must be between 0 and 100")]
		public int HierarchyLevel { get; set; } = 0;
	}
}
