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
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationLoggingConstants;

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
					MigrationLogger.LogInformation(string.Format(
						LoadedFromJsonWithValidation, 
						validatedUserRoles.Count(), "user-role mappings"));
				}
				else
				{
					var errorMessage = string.Format(
						ExternalSourceEnabledButNoData, "userroles.json");
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
					MigrationLogger.LogInformation(string.Format(
						UsingDefaultUserRoleMappings, validatedUserRoles.Count()));
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
					MigrationLogger.LogWarning(string.Format(
						NoItemsFoundInJson, 
						"user-role mappings", "userroles.json"));
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
				MigrationLogger.LogError(string.Format(
					FailedToLoadFromJson, 
					"user-role mappings", ex.Message), ex);
				throw new InvalidOperationException(string.Format(
					LoadingFailed, 
					"User-role mapping", ex.Message), ex);
			}
		}

		/// <summary>
		/// Loads default user-role mappings - simplified
		/// </summary>
		private static IEnumerable<IdentityUserRole<string>> LoadAndValidateDefaultUserRoles()
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
				MigrationLogger.LogError(string.Format(
					FailedToValidateDefaultData, 
					"user-role mapping", ex.Message), ex);
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