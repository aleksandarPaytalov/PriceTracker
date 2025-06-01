using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations.IdentityConfiguration
{
	/// <summary>
	/// UserRole configuration with JSON loading, Builder validation, and cross-reference orchestration
	/// </summary>
	public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
	{
		private readonly IOptions<SeedingOptions> _options;

		public UserRoleConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
		{
			UserRoleBuilder.ResetTracking();

			// Configure composite primary key
			builder.HasKey(ur => new { ur.UserId, ur.RoleId });

			// Configure relationships
			builder.HasOne<User>()
				   .WithMany()
				   .HasForeignKey(ur => ur.UserId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne<IdentityRole>()
				   .WithMany()
				   .HasForeignKey(ur => ur.RoleId)
				   .OnDelete(DeleteBehavior.Cascade);

			// Seed data using JSON + Builder approach with cross-reference validation
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("UserRole", false))
			{
				var validatedUserRoles = LoadAndValidateUserRolesFromJson();

				if (validatedUserRoles.Any())
				{
					builder.HasData(validatedUserRoles);
					MigrationLogger.LogInformation($"✅ Loaded {validatedUserRoles.Count()} user-role mappings from JSON with Builder validation");
				}
				else
				{
					var errorMessage = string.Format(ExternalSourceEnabledButNoData, "userroles.json");
					MigrationLogger.LogError(errorMessage);
					throw new InvalidOperationException(errorMessage);
				}
			}
			else
			{
				// Use default seed data with Builder validation
				if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("UserRole", true))
				{
					var validatedUserRoles = LoadAndValidateDefaultUserRoles();
					builder.HasData(validatedUserRoles);
					MigrationLogger.LogInformation($"✅ Using default seed data for user-role mappings with Builder validation: {validatedUserRoles.Count()} mappings");
				}
			}
		}

		/// <summary>
		/// Loads user-role mappings from JSON and validates them using UserRoleBuilder with cross-references
		/// </summary>
		private IEnumerable<IdentityUserRole<string>> LoadAndValidateUserRolesFromJson()
		{
			try
			{
				// Clear tracking for new seeding session
				UserRoleBuilder.ResetTracking();

				// Load JSON user-role mappings directly as UserRoleJsonDto objects
				var jsonUserRoles = MigrationDataHelper.GetDataFromJson<UserRoleJsonDto>("userroles.json");

				if (!jsonUserRoles.Any())
				{
					MigrationLogger.LogWarning("No user-role mappings found in userroles.json file");
					return Enumerable.Empty<IdentityUserRole<string>>();
				}

				// Get available users and roles for cross-reference validation
				var availableUsers = GetAvailableUsers();
				var availableRoles = GetAvailableRoles();

				// Validate using UserRoleBuilder with cross-reference validation - returns only valid items
				var validatedUserRoles = MigrationDataHelper.ValidateItems(
					jsonUserRoles,
					userRoleDto => ValidateUserRoleWithBuilder(userRoleDto, availableUsers, availableRoles),
					"user-role mapping",
					_options.Value.StrictValidation);

				return validatedUserRoles;
			}
			catch (Exception ex) when (!(ex is ValidationException))
			{
				MigrationLogger.LogError($"Failed to load user-role mappings from JSON: {ex.Message}", ex);
				throw new InvalidOperationException($"User-role mapping loading failed: {ex.Message}", ex);
			}
		}

		/// <summary>
		/// Loads and validates default user-role mappings using UserRoleBuilder
		/// </summary>
		private IEnumerable<IdentityUserRole<string>> LoadAndValidateDefaultUserRoles()
		{
			try
			{
				// Clear tracking for new seeding session
				UserRoleBuilder.ResetTracking();

				// Get default user-role mappings from SeedData
				var data = new SeedData();
				data.Initialize();

				var defaultUserRoles = new[]
				{
					data.AdminUserRole,
					data.RegularUserRole,
					data.GuestUserRole
				};

				// Get available users and roles for validation
				var availableUsers = GetDefaultUsers();
				var availableRoles = GetDefaultRoles();

				// Validate default user-role mappings using UserRoleBuilder
				var validatedUserRoles = MigrationDataHelper.ValidateItems(
					defaultUserRoles,
					userRole => ValidateExistingUserRoleWithBuilder(userRole, availableUsers, availableRoles),
					"default user-role mapping",
					strictValidation: true);

				return validatedUserRoles;
			}
			catch (Exception ex)
			{
				MigrationLogger.LogError($"Failed to validate default user-role mapping data: {ex.Message}", ex);
				throw;
			}
		}

		/// <summary>
		/// Validates a UserRoleJsonDto using UserRoleBuilder validation logic with cross-references
		/// </summary>
		private static IdentityUserRole<string> ValidateUserRoleWithBuilder(
			UserRoleJsonDto userRoleDto,
			IEnumerable<User> availableUsers,
			IEnumerable<IdentityRole> availableRoles)
		{
			// Use UserRoleBuilder for comprehensive validation with cross-reference checking
			var userRoleBuilder = new UserRoleBuilder(userRoleDto, availableUsers, availableRoles);
			return userRoleBuilder.Build();
		}

		/// <summary>
		/// Validates an existing IdentityUserRole using UserRoleBuilder validation logic
		/// </summary>
		private static IdentityUserRole<string> ValidateExistingUserRoleWithBuilder(
			IdentityUserRole<string> userRole,
			IEnumerable<User> availableUsers,
			IEnumerable<IdentityRole> availableRoles)
		{
			// Use UserRoleBuilder for validation of existing user-role mappings
			var userRoleBuilder = new UserRoleBuilder(userRole, availableUsers, availableRoles);
			return userRoleBuilder.Build();
		}

		/// <summary>
		/// Gets available users for cross-reference validation
		/// </summary>
		private IEnumerable<User> GetAvailableUsers()
		{
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("User", false))
			{
				RoleBuilder.ResetTracking();

				// Load users from JSON if external source is enabled
				var jsonUsers = MigrationDataHelper.GetDataFromJson<UserJsonDto>("users.json");
				var passwordHasher = new PasswordHasher<User>();

				return jsonUsers.Select(userDto =>
				{
					try
					{
						var userBuilder = new UserBuilder(userDto, passwordHasher);
						return userBuilder.Build();
					}
					catch (ValidationException ex)
					{
						MigrationLogger.LogWarning($"Skipping invalid user during cross-reference validation: {ex.Message}");
						return null;
					}
				}).Where(user => user != null)!;
			}
			else
			{
				RoleBuilder.ResetTracking();

				// Use default users
				return GetDefaultUsers();
			}
		}

		/// <summary>
		/// Gets available roles for cross-reference validation
		/// </summary>
		private IEnumerable<IdentityRole> GetAvailableRoles()
		{
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Role", false))
			{
				RoleBuilder.ResetTracking();

				// Load roles from JSON if external source is enabled
				var jsonRoles = MigrationDataHelper.GetDataFromJson<RoleJsonDto>("roles.json");

				return jsonRoles.Select(roleDto =>
				{
					try
					{
						var roleBuilder = new RoleBuilder(roleDto);
						return roleBuilder.Build();
					}
					catch (ValidationException ex)
					{
						MigrationLogger.LogWarning($"Skipping invalid role during cross-reference validation: {ex.Message}");
						return null;
					}
				}).Where(role => role != null)!;
			}
			else
			{
				RoleBuilder.ResetTracking();

				// Use default roles
				return GetDefaultRoles();
			}
		}

		/// <summary>
		/// Gets default users from SeedData
		/// </summary>
		private static IEnumerable<User> GetDefaultUsers()
		{
			var data = new SeedData();
			data.Initialize();

			var passwordHasher = new PasswordHasher<User>();

			return new[]
			{
				CreateUserWithPassword(data.Administrator, SeedData.GetDefaultPasswordForUser(data.Administrator.Email!), passwordHasher),
				CreateUserWithPassword(data.User, SeedData.GetDefaultPasswordForUser(data.User.Email!), passwordHasher),
				CreateUserWithPassword(data.Guest, SeedData.GetDefaultPasswordForUser(data.Guest.Email!), passwordHasher)
			};
		}

		/// <summary>
		/// Gets default roles from SeedData
		/// </summary>
		private static IEnumerable<IdentityRole> GetDefaultRoles()
		{
			var data = new SeedData();
			data.Initialize();

			return new[]
			{
				data.AdminRole,
				data.UserRole,
				data.GuestRole
			};
		}

		/// <summary>
		/// Creates a user with hashed password for cross-reference validation
		/// </summary>
		private static User CreateUserWithPassword(User user, string password, IPasswordHasher<User> passwordHasher)
		{
			try
			{
				var userBuilder = new UserBuilder(user, password, passwordHasher);
				return userBuilder.Build();
			}
			catch (ValidationException ex)
			{
				MigrationLogger.LogError($"Failed to create user with password for cross-reference: {ex.Message}", ex);
				throw;
			}
		}
	}
}