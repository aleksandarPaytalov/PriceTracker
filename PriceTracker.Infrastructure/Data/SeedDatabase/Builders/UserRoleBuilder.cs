using Microsoft.AspNetCore.Identity;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	public class UserRoleBuilder : IBuilder<IdentityUserRole<string>>
	{
		private readonly IdentityUserRole<string> _userRole;
		private static readonly HashSet<string> _existingUserRolePairs = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates user-role mapping from JSON DTO with basic validation
		/// </summary>
		public UserRoleBuilder(UserRoleJsonDto userRoleDto)
		{
			try
			{
				ValidateUserRoleJsonDto(userRoleDto);
				_userRole = CreateUserRoleFromDto(userRoleDto);

				// Track this mapping to prevent duplicates in current session
				var userRolePair = $"{userRoleDto.UserId}|{userRoleDto.RoleId}";
				_existingUserRolePairs.Add(userRolePair);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create user-role mapping from JSON: {ex.Message}");
			}
		}

		/// <summary>
		/// Creates user-role mapping from individual parameters
		/// </summary>
		public UserRoleBuilder(string userId, string roleId)
		{
			try
			{
				var userRoleDto = new UserRoleJsonDto
				{
					UserId = userId,
					RoleId = roleId
				};

				ValidateUserRoleJsonDto(userRoleDto);
				_userRole = CreateUserRoleFromDto(userRoleDto);

				// Track this mapping to prevent duplicates in current session
				var userRolePair = $"{userId}|{roleId}";
				_existingUserRolePairs.Add(userRolePair);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create user-role mapping: {ex.Message}");
			}
		}

		/// <summary>
		/// Creates user-role mapping from existing IdentityUserRole
		/// </summary>
		public UserRoleBuilder(IdentityUserRole<string> existingUserRole)
		{
			try
			{
				if (existingUserRole == null)
				{
					throw new ArgumentNullException(nameof(existingUserRole), UserRoleConstants.UserRoleRequired);
				}

				ValidateExistingUserRole(existingUserRole);
				_userRole = existingUserRole;

				// Track this mapping to prevent duplicates in current session
				var userRolePair = $"{existingUserRole.UserId}|{existingUserRole.RoleId}";
				_existingUserRolePairs.Add(userRolePair);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to validate existing user-role mapping: {ex.Message}");
			}
		}

		public IdentityUserRole<string> Build() => _userRole;

		/// <summary>
		/// Basic validation without cross-reference checks
		/// </summary>
		private static void ValidateUserRoleJsonDto(UserRoleJsonDto userRoleDto)
		{
			if (userRoleDto == null)
			{
				throw new ValidationException(UserRoleConstants.UserRoleRequired);
			}

			// Standard model validation
			var validationResults = new List<ValidationResult>();
			var validationContext = new ValidationContext(userRoleDto);

			if (!Validator.TryValidateObject(userRoleDto, validationContext, validationResults, true))
			{
				var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
				throw new ValidationException($"User-role mapping validation failed: {errors}");
			}

			// Basic validation
			ValidateUserId(userRoleDto.UserId);
			ValidateRoleId(userRoleDto.RoleId);

			// Check for duplicates in current seeding session BEFORE adding to constructor
			CheckDuplicateInSession(userRoleDto.UserId, userRoleDto.RoleId);
		}

		/// <summary>
		/// Validates existing IdentityUserRole
		/// </summary>
		private static void ValidateExistingUserRole(IdentityUserRole<string> userRole)
		{
			ValidateUserId(userRole.UserId);
			ValidateRoleId(userRole.RoleId);

			// Check for duplicates in current seeding session BEFORE adding to constructor
			CheckDuplicateInSession(userRole.UserId, userRole.RoleId);
		}

		/// <summary>
		/// Validates user ID
		/// </summary>
		private static void ValidateUserId(string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ValidationException(UserRoleConstants.UserIdRequired);
			}

			if (!Guid.TryParse(userId, out _))
			{
				throw new ValidationException(string.Format(UserRoleConstants.InvalidUserIdFormat, userId));
			}

			if (ContainsForbiddenContent(userId))
			{
				throw new ValidationException(string.Format(UserRoleConstants.UserIdContainsForbiddenContent, userId));
			}
		}

		/// <summary>
		/// Validates role ID
		/// </summary>
		private static void ValidateRoleId(string roleId)
		{
			if (string.IsNullOrWhiteSpace(roleId))
			{
				throw new ValidationException(UserRoleConstants.RoleIdRequired);
			}

			if (!Guid.TryParse(roleId, out _))
			{
				throw new ValidationException(string.Format(UserRoleConstants.InvalidRoleIdFormat, roleId));
			}

			if (ContainsForbiddenContent(roleId))
			{
				throw new ValidationException(string.Format(UserRoleConstants.RoleIdContainsForbiddenContent, roleId));
			}
		}

		/// <summary>
		/// Checks for duplicates without adding to tracking (done in constructor)
		/// </summary>
		private static void CheckDuplicateInSession(string userId, string roleId)
		{
			var userRolePair = $"{userId}|{roleId}";

			if (_existingUserRolePairs.Contains(userRolePair))
			{
				throw new ValidationException(string.Format(UserRoleConstants.DuplicateUserRoleMapping, userId, roleId));
			}
		}

		/// <summary>
		/// Basic security check for forbidden content
		/// </summary>
		private static bool ContainsForbiddenContent(string content)
		{
			if (string.IsNullOrEmpty(content)) return false;

			var forbiddenPatterns = new[]
			{
				"<script", "javascript:", "select ", "insert ", "update ",
				"delete ", "drop ", "union ", "exec ", "execute ",
				"--", "/*", "*/", "@@"
			};

			return forbiddenPatterns.Any(pattern =>
				content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Creates IdentityUserRole from validated DTO
		/// </summary>
		private static IdentityUserRole<string> CreateUserRoleFromDto(UserRoleJsonDto userRoleDto)
		{
			return new IdentityUserRole<string>
			{
				UserId = userRoleDto.UserId,
				RoleId = userRoleDto.RoleId
			};
		}

		/// <summary>
		/// Clear tracking collections for new seeding session
		/// </summary>
		public static void ResetTracking()
		{
			_existingUserRolePairs.Clear();
		}

		/// <summary>
		/// Get count of currently tracked user-role mappings
		/// </summary>
		public static int GetTrackedUserRoleCount()
		{
			return _existingUserRolePairs.Count;
		}

		/// <summary>
		/// Check if a user-role mapping is already tracked
		/// </summary>
		public static bool IsUserRoleMappingTracked(string userId, string roleId)
		{
			var userRolePair = $"{userId}|{roleId}";
			return _existingUserRolePairs.Contains(userRolePair);
		}
	}
}