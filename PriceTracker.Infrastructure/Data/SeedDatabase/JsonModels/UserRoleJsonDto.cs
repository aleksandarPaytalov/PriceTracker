using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels
{
	/// <summary>
	/// DTO for UserRole JSON deserialization with validation attributes
	/// </summary>
	public class UserRoleJsonDto
	{
		/// <summary>
		/// User ID for the user-role mapping (GUID format)
		/// </summary>
		[Required(ErrorMessage = "User ID is required for user-role mapping")]
		[StringLength(36, MinimumLength = 36, ErrorMessage = "User ID must be a valid GUID")]
		public string UserId { get; set; } = string.Empty;

		/// <summary>
		/// Role ID for the user-role mapping (GUID format)
		/// </summary>
		[Required(ErrorMessage = "Role ID is required for user-role mapping")]
		[StringLength(36, MinimumLength = 36, ErrorMessage = "Role ID must be a valid GUID")]
		public string RoleId { get; set; } = string.Empty;
	}
}
