using Microsoft.AspNetCore.Identity;
using PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Builder for creating validated IdentityRole entities from JSON or default data
	/// </summary>
	public class RoleBuilder : IBuilder<IdentityRole>
	{
		private readonly IdentityRole _role;
		private static readonly HashSet<string> _existingRoleNames = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new role from JSON DTO with full validation
		/// </summary>
		/// <param name="roleDto">JSON DTO containing role data</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public RoleBuilder(RoleJsonDto roleDto)
		{
			try
			{
				ValidateRoleJsonDto(roleDto);
				_role = CreateRoleFromDto(roleDto);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create role from JSON: {ex.Message}");
			}
		}

		/// <summary>
		/// Creates a new role from individual parameters with validation
		/// </summary>
		/// <param name="id">Role ID</param>
		/// <param name="name">Role name</param>
		/// <param name="normalizedName">Normalized role name (optional - auto-generated if null)</param>
		/// <param name="concurrencyStamp">Concurrency stamp (optional - auto-generated if null)</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public RoleBuilder(string id, string name, string? normalizedName = null, string? concurrencyStamp = null)
		{
			try
			{
				var roleDto = new RoleJsonDto
				{
					Id = id,
					Name = name,
					NormalizedName = normalizedName,
					ConcurrencyStamp = concurrencyStamp
				};

				ValidateRoleJsonDto(roleDto);
				_role = CreateRoleFromDto(roleDto);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create role: {ex.Message}");
			}
		}

		/// <summary>
		/// Creates a new role from existing IdentityRole with validation
		/// </summary>
		/// <param name="existingRole">Existing IdentityRole to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public RoleBuilder(IdentityRole existingRole)
		{
			try
			{
				if (existingRole == null)
				{
					throw new ArgumentNullException(nameof(existingRole), RoleConstants.RoleRequired);
				}

				ValidateExistingRole(existingRole);
				_role = existingRole;
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to validate existing role: {ex.Message}");
			}
		}

		/// <summary>
		/// Builds and returns the validated IdentityRole
		/// </summary>
		/// <returns>A validated IdentityRole object</returns>
		public IdentityRole Build() => _role;

		/// <summary>
		/// Validates role JSON DTO with comprehensive checks
		/// </summary>
		/// <param name="roleDto">Role DTO to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateRoleJsonDto(RoleJsonDto roleDto)
		{
			if (roleDto == null)
			{
				throw new ValidationException(RoleConstants.RoleRequired);
			}

			// Standard model validation
			var validationResults = new List<ValidationResult>();
			var validationContext = new ValidationContext(roleDto);

			if (!Validator.TryValidateObject(roleDto, validationContext, validationResults, true))
			{
				var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
				throw new ValidationException($"Role validation failed: {errors}");
			}

			// Business logic validation
			ValidateRoleId(roleDto.Id);
			ValidateRoleName(roleDto.Name);

			// Auto-generate normalized name if not provided
			if (string.IsNullOrEmpty(roleDto.NormalizedName))
			{
				roleDto.NormalizedName = roleDto.Name.ToUpperInvariant();
			}

			// Auto-generate concurrency stamp if not provided
			if (string.IsNullOrEmpty(roleDto.ConcurrencyStamp))
			{
				roleDto.ConcurrencyStamp = Guid.NewGuid().ToString();
			}

			// Validate normalized name
			ValidateNormalizedRoleName(roleDto.NormalizedName);

			// Check for duplicates in current seeding session
			ValidateRoleUniqueness(roleDto.Name);
		}

		/// <summary>
		/// Validates existing IdentityRole
		/// </summary>
		/// <param name="role">Role to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateExistingRole(IdentityRole role)
		{
			ValidateRoleId(role.Id);
			ValidateRoleName(role.Name!);

			if (string.IsNullOrEmpty(role.NormalizedName))
			{
				role.NormalizedName = role.Name!.ToUpperInvariant();
			}

			if (string.IsNullOrEmpty(role.ConcurrencyStamp))
			{
				role.ConcurrencyStamp = Guid.NewGuid().ToString();
			}

			ValidateNormalizedRoleName(role.NormalizedName);
			ValidateRoleUniqueness(role.Name!);
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
				throw new ValidationException(RoleConstants.RoleIdRequired);
			}

			// Validate GUID format
			if (!Guid.TryParse(roleId, out _))
			{
				throw new ValidationException(string.Format(RoleConstants.InvalidRoleIdFormat, roleId));
			}
		}

		/// <summary>
		/// Validates role name with security checks
		/// </summary>
		/// <param name="roleName">Role name to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateRoleName(string roleName)
		{
			if (string.IsNullOrWhiteSpace(roleName))
			{
				throw new ValidationException(RoleConstants.RoleNameRequired);
			}

			// Length validation
			if (roleName.Length < 2 || roleName.Length > 256)
			{
				throw new ValidationException(string.Format(RoleConstants.InvalidRoleNameLength, 2, 256));
			}

			// Security validation - check for forbidden patterns
			if (ContainsForbiddenContent(roleName))
			{
				throw new ValidationException(string.Format(RoleConstants.RoleNameContainsForbiddenContent, roleName));
			}

			// Business rules validation
			if (!IsValidRoleNameFormat(roleName))
			{
				throw new ValidationException(string.Format(RoleConstants.InvalidRoleNameFormat, roleName));
			}
		}

		/// <summary>
		/// Validates normalized role name
		/// </summary>
		/// <param name="normalizedName">Normalized name to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateNormalizedRoleName(string normalizedName)
		{
			if (string.IsNullOrWhiteSpace(normalizedName))
			{
				throw new ValidationException(RoleConstants.NormalizedNameRequired);
			}

			// Ensure it's actually normalized (uppercase)
			if (normalizedName != normalizedName.ToUpperInvariant())
			{
				throw new ValidationException(RoleConstants.NormalizedNameNotUppercase);
			}
		}

		/// <summary>
		/// Validates role name uniqueness in current seeding session
		/// </summary>
		/// <param name="roleName">Role name to check</param>
		/// <exception cref="ValidationException">Thrown when duplicate found</exception>
		private static void ValidateRoleUniqueness(string roleName)
		{
			if (_existingRoleNames.Contains(roleName))
			{
				throw new ValidationException(string.Format(RoleConstants.DuplicateRoleName, roleName));
			}

			_existingRoleNames.Add(roleName);
		}

		/// <summary>
		/// Checks if role name format is valid
		/// </summary>
		/// <param name="roleName">Role name to check</param>
		/// <returns>True if format is valid</returns>
		private static bool IsValidRoleNameFormat(string roleName)
		{
			// Role names should contain only letters, numbers, spaces, and common punctuation
			// No special characters that could be used for attacks
			return roleName.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_');
		}

		/// <summary>
		/// Checks for forbidden content patterns in role data
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
				"<iframe", "<object", "<embed", "data:text/html", "data:text/javascript",
				"<", ">", "src=", "href=", "style=", "expression(", "url(",
				"select ", "insert ", "update ", "delete ", "drop ", "union ",
				"exec ", "execute ", "--", "/*", "*/", "@@", "char", "nchar",
				"varchar", "nvarchar", "table", "database", "sysobjects", "syscolumns"
			};

			return forbiddenPatterns.Any(pattern =>
				content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Creates IdentityRole from validated DTO
		/// </summary>
		/// <param name="roleDto">Validated role DTO</param>
		/// <returns>IdentityRole entity</returns>
		private static IdentityRole CreateRoleFromDto(RoleJsonDto roleDto)
		{
			return new IdentityRole
			{
				Id = roleDto.Id,
				Name = roleDto.Name,
				NormalizedName = roleDto.NormalizedName,
				ConcurrencyStamp = roleDto.ConcurrencyStamp
			};
		}

		/// <summary>
		/// Clear tracking collections for new seeding session
		/// Call this before starting a new migration or seeding operation
		/// </summary>
		public static void ResetTracking()
		{
			_existingRoleNames.Clear();
		}

		/// <summary>
		/// Get count of currently tracked roles in this session
		/// </summary>
		public static int GetTrackedRoleCount()
		{
			return _existingRoleNames.Count;
		}

		/// <summary>
		/// Check if a role name is already tracked in current session
		/// </summary>
		/// <param name="roleName">Role name to check</param>
		/// <returns>True if already tracked, false otherwise</returns>
		public static bool IsRoleNameTracked(string roleName)
		{
			return _existingRoleNames.Contains(roleName);
		}
	}
}