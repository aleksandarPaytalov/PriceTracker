namespace PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels
{
	/// <summary>
	/// Extended DTO for bulk user import scenarios with additional metadata
	/// </summary>
	public class BulkUserJsonDto : UserJsonDto
	{
		/// <summary>
		/// Additional roles to assign to this user (role names)
		/// </summary>
		public List<string> RoleNames { get; set; } = new();

		/// <summary>
		/// Environment restrictions (empty means all environments)
		/// </summary>
		public List<string> Environments { get; set; } = new();

		/// <summary>
		/// Indicates if this is a temporary user (for migration purposes)
		/// </summary>
		public bool IsTemporary { get; set; } = false;

		/// <summary>
		/// Force password change on first login
		/// </summary>
		public bool ForcePasswordChange { get; set; } = false;

		/// <summary>
		/// Additional metadata for bulk operations
		/// </summary>
		public Dictionary<string, object> Metadata { get; set; } = new();
	}
}
