using Microsoft.AspNetCore.Identity;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Builder for creating validated IdentityUserRole entities from JSON or default data
	/// </summary>
	public class UserRoleBuilder : IBuilder<IdentityUserRole<string>>
	{
		private readonly IdentityUserRole<string> _userRole;
		private static readonly HashSet<string> _existingUserRolePairs = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new user-role mapping from JSON DTO with full validation
		/// </summary>
		/// <param name="userRoleDto">JSON DTO containing user-role mapping data</param>
		/// <param name="availableUsers">Available users for validation (optional)</param>
		/// <param name="availableRoles">Available roles for validation (optional)</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public UserRoleBuilder(
			UserRoleJsonDto userRoleDto,
			IEnumerable<User>? availableUsers = null,
			IEnumerable<IdentityRole>? availableRoles = null)
		{
			try
			{
				ValidateUserRoleJsonDto(userRoleDto, availableUsers, availableRoles);
				_userRole = CreateUserRoleFromDto(userRoleDto);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create user-role mapping from JSON: {ex.Message}");
			}
		}

		/// <summary>
		/// Creates a new user-role mapping from individual parameters with validation
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="roleId">Role ID</param>
		/// <param name="availableUsers">Available users for validation (optional)</param>
		/// <param name="availableRoles">Available roles for validation (optional)</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public UserRoleBuilder(
			string userId,
			string roleId,
			IEnumerable<User>? availableUsers = null,
			IEnumerable<IdentityRole>? availableRoles = null)
		{
			try
			{
				var userRoleDto = new UserRoleJsonDto
				{
					UserId = userId,
					RoleId = roleId
				};

				ValidateUserRoleJsonDto(userRoleDto, availableUsers, availableRoles);
				_userRole = CreateUserRoleFromDto(userRoleDto);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create user-role mapping: {ex.Message}");
			}
		}

		/// <summary>
		/// Creates a new user-role mapping from existing IdentityUserRole with validation
		/// </summary>
		/// <param name="existingUserRole">Existing IdentityUserRole to validate</param>
		/// <param name="availableUsers">Available users for validation (optional)</param>
		/// <param name="availableRoles">Available roles for validation (optional)</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public UserRoleBuilder(
			IdentityUserRole<string> existingUserRole,
			IEnumerable<User>? availableUsers = null,
			IEnumerable<IdentityRole>? availableRoles = null)
		{
			try
			{
				if (existingUserRole == null)
				{
					throw new ArgumentNullException(nameof(existingUserRole), UserRoleConstants.UserRoleRequired);
				}

				ValidateExistingUserRole(existingUserRole, availableUsers, availableRoles);
				_userRole = existingUserRole;
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to validate existing user-role mapping: {ex.Message}");
			}
		}

		/// <summary>
		/// Builds and returns the validated IdentityUserRole
		/// </summary>
		/// <returns>A validated IdentityUserRole object</returns>
		public IdentityUserRole<string> Build() => _userRole;

		/// <summary>
		/// Validates user-role JSON DTO with comprehensive checks
		/// </summary>
		/// <param name="userRoleDto">User-role DTO to validate</param>
		/// <param name="availableUsers">Available users for reference validation</param>
		/// <param name="availableRoles">Available roles for reference validation</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateUserRoleJsonDto(
			UserRoleJsonDto userRoleDto,
			IEnumerable<User>? availableUsers,
			IEnumerable<IdentityRole>? availableRoles)
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

			// Business logic validation
			ValidateUserId(userRoleDto.UserId);
			ValidateRoleId(userRoleDto.RoleId);

			// Reference validation (if collections provided)
			if (availableUsers != null)
			{
				ValidateUserExists(userRoleDto.UserId, availableUsers);
			}

			if (availableRoles != null)
			{
				ValidateRoleExists(userRoleDto.RoleId, availableRoles);
			}

			// Check for duplicates in current seeding session
			ValidateUserRoleUniqueness(userRoleDto.UserId, userRoleDto.RoleId);
		}

		/// <summary>
		/// Validates existing IdentityUserRole
		/// </summary>
		/// <param name="userRole">User-role mapping to validate</param>
		/// <param name="availableUsers">Available users for reference validation</param>
		/// <param name="availableRoles">Available roles for reference validation</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateExistingUserRole(
			IdentityUserRole<string> userRole,
			IEnumerable<User>? availableUsers,
			IEnumerable<IdentityRole>? availableRoles)
		{
			ValidateUserId(userRole.UserId);
			ValidateRoleId(userRole.RoleId);

			// Reference validation (if collections provided)
			if (availableUsers != null)
			{
				ValidateUserExists(userRole.UserId, availableUsers);
			}

			if (availableRoles != null)
			{
				ValidateRoleExists(userRole.RoleId, availableRoles);
			}

			ValidateUserRoleUniqueness(userRole.UserId, userRole.RoleId);
		}

		/// <summary>
		/// Validates user ID
		/// </summary>
		/// <param name="userId">User ID to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateUserId(string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ValidationException(UserRoleConstants.UserIdRequired);
			}

			// Validate GUID format
			if (!Guid.TryParse(userId, out _))
			{
				throw new ValidationException(string.Format(UserRoleConstants.InvalidUserIdFormat, userId));
			}

			// Security validation
			if (ContainsForbiddenContent(userId))
			{
				throw new ValidationException(string.Format(UserRoleConstants.UserIdContainsForbiddenContent, userId));
			}
		}

		/// <summary>
		/// Validates role ID
		/// </summary>
		/// <param name="roleId">Role ID to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateRoleId(string roleId)
		{
			if (string.IsNullOrWhiteSpace(roleId))
			{
				throw new ValidationException(UserRoleConstants.RoleIdRequired);
			}

			// Validate GUID format
			if (!Guid.TryParse(roleId, out _))
			{
				throw new ValidationException(string.Format(UserRoleConstants.InvalidRoleIdFormat, roleId));
			}

			// Security validation
			if (ContainsForbiddenContent(roleId))
			{
				throw new ValidationException(string.Format(UserRoleConstants.RoleIdContainsForbiddenContent, roleId));
			}
		}

		/// <summary>
		/// Validates that user exists in the available users collection
		/// </summary>
		/// <param name="userId">User ID to check</param>
		/// <param name="availableUsers">Available users collection</param>
		/// <exception cref="ValidationException">Thrown when user not found</exception>
		private static void ValidateUserExists(string userId, IEnumerable<User> availableUsers)
		{
			var usersList = availableUsers.ToList();
			if (!usersList.Any(u => u.Id == userId))
			{
				var availableUserIds = string.Join(", ", usersList.Select(u => u.Id).Take(5));
				throw new ValidationException(
					string.Format(UserRoleConstants.UserNotFound, userId, availableUserIds));
			}
		}

		/// <summary>
		/// Validates that role exists in the available roles collection
		/// </summary>
		/// <param name="roleId">Role ID to check</param>
		/// <param name="availableRoles">Available roles collection</param>
		/// <exception cref="ValidationException">Thrown when role not found</exception>
		private static void ValidateRoleExists(string roleId, IEnumerable<IdentityRole> availableRoles)
		{
			var rolesList = availableRoles.ToList();
			if (!rolesList.Any(r => r.Id == roleId))
			{
				var availableRoleIds = string.Join(", ", rolesList.Select(r => r.Id).Take(5));
				throw new ValidationException(
					string.Format(UserRoleConstants.RoleNotFound, roleId, availableRoleIds));
			}
		}

		/// <summary>
		/// Validates user-role mapping uniqueness in current seeding session
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="roleId">Role ID</param>
		/// <exception cref="ValidationException">Thrown when duplicate found</exception>
		private static void ValidateUserRoleUniqueness(string userId, string roleId)
		{
			var userRolePair = $"{userId}|{roleId}";

			if (_existingUserRolePairs.Contains(userRolePair))
			{
				throw new ValidationException(string.Format(UserRoleConstants.DuplicateUserRoleMapping, userId, roleId));
			}

			_existingUserRolePairs.Add(userRolePair);
		}

		/// <summary>
		/// Checks for forbidden content patterns in string data
		/// </summary>
		/// <param name="content">Content to check</param>
		/// <returns>True if forbidden content found</returns>
		private static bool ContainsForbiddenContent(string content)
		{
			if (string.IsNullOrEmpty(content)) return false;

			var forbiddenPatterns = new[]
			{
				"<script", "javascript:", "vbscript:", "onload=", "onerror=",
				"onclick=", "onmouseover=", "alert(", "eval(", "document.cookie",
				"select ", "insert ", "update ", "delete ", "drop ", "union ",
				"exec ", "execute ", "--", "/*", "*/", "@@"
			};

			return forbiddenPatterns.Any(pattern =>
				content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Creates IdentityUserRole from validated DTO
		/// </summary>
		/// <param name="userRoleDto">Validated user-role DTO</param>
		/// <returns>IdentityUserRole entity</returns>
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
		/// Call this before starting a new migration or seeding operation
		/// </summary>
		public static void ResetTracking()
		{
			_existingUserRolePairs.Clear();
		}

		/// <summary>
		/// Get count of currently tracked user-role mappings in this session
		/// </summary>
		public static int GetTrackedUserRoleCount()
		{
			return _existingUserRolePairs.Count;
		}

		/// <summary>
		/// Check if a user-role mapping is already tracked in current session
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="roleId">Role ID</param>
		/// <returns>True if already tracked, false otherwise</returns>
		public static bool IsUserRoleMappingTracked(string userId, string roleId)
		{
			var userRolePair = $"{userId}|{roleId}";
			return _existingUserRolePairs.Contains(userRolePair);
		}

		/// <summary>
		/// Get all tracked user-role pairs in current session
		/// </summary>
		/// <returns>Collection of tracked user-role pairs</returns>
		public static IEnumerable<string> GetTrackedUserRolePairs()
		{
			return _existingUserRolePairs.AsEnumerable();
		}
	}
}