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

			// Seed data - simplified approach without cross-reference validation
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("UserRole", false))
			{
				var validatedUserRoles = LoadAndValidateUserRolesFromJson();

				if (validatedUserRoles.Any())
				{
					builder.HasData(validatedUserRoles);
					MigrationLogger.LogInformation($"✅ Loaded {validatedUserRoles.Count()} user-role mappings from JSON");
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
				// Use default seed data
				if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("UserRole", true))
				{
					var validatedUserRoles = LoadAndValidateDefaultUserRoles();
					builder.HasData(validatedUserRoles);
					MigrationLogger.LogInformation($"✅ Using default user-role mappings: {validatedUserRoles.Count()} mappings");
				}
			}
		}

		/// <summary>
		/// Loads user-role mappings from JSON - simplified without cross-reference validation
		/// </summary>
		private IEnumerable<IdentityUserRole<string>> LoadAndValidateUserRolesFromJson()
		{
			try
			{
				var jsonUserRoles = MigrationDataHelper.GetDataFromJson<UserRoleJsonDto>("userroles.json");

				if (!jsonUserRoles.Any())
				{
					MigrationLogger.LogWarning("No user-role mappings found in userroles.json file");
					return Enumerable.Empty<IdentityUserRole<string>>();
				}

				// Simple validation without cross-reference checks
				var validatedUserRoles = MigrationDataHelper.ValidateItems(
					jsonUserRoles,
					ValidateUserRoleWithBuilder,
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
		/// Loads default user-role mappings - simplified
		/// </summary>
		private IEnumerable<IdentityUserRole<string>> LoadAndValidateDefaultUserRoles()
		{
			try
			{
				var data = new SeedData();
				data.Initialize();

				var defaultUserRoles = new[]
				{
					data.AdminUserRole,
					data.RegularUserRole,
					data.GuestUserRole
				};

				// Simple validation without cross-reference checks
				var validatedUserRoles = MigrationDataHelper.ValidateItems(
					defaultUserRoles,
					ValidateExistingUserRoleWithBuilder,
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
		/// Simple validation without cross-reference checks
		/// EF will handle foreign key constraint violations automatically
		/// </summary>
		private static IdentityUserRole<string> ValidateUserRoleWithBuilder(UserRoleJsonDto userRoleDto)
		{
			var userRoleBuilder = new UserRoleBuilder(userRoleDto);
			return userRoleBuilder.Build();
		}

		/// <summary>
		/// Simple validation for existing user-role mappings
		/// EF will handle foreign key constraint violations automatically
		/// </summary>
		private static IdentityUserRole<string> ValidateExistingUserRoleWithBuilder(IdentityUserRole<string> userRole)
		{
			var userRoleBuilder = new UserRoleBuilder(userRole);
			return userRoleBuilder.Build();
		}
	}
}