using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations.IdentityConfiguration
{
	/// <summary>
	/// Role configuration with JSON loading and Builder validation orchestration
	/// </summary>
	public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
	{
		private readonly IOptions<SeedingOptions> _options;

		public RoleConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public void Configure(EntityTypeBuilder<IdentityRole> builder)
		{
			RoleBuilder.ResetTracking();

			// Configure entity properties
			builder.HasKey(r => r.Id);
			builder.HasIndex(r => r.NormalizedName).IsUnique();
			builder.Property(r => r.Name).HasMaxLength(256);
			builder.Property(r => r.NormalizedName).HasMaxLength(256);

			// Seed data using JSON + Builder approach
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Role", false))
			{
				var validatedRoles = LoadAndValidateRolesFromJson();

				if (validatedRoles.Any())
				{
					builder.HasData(validatedRoles);
					MigrationLogger.LogInformation($"✅ Loaded {validatedRoles.Count()} roles from JSON with Builder validation");
				}
				else
				{
					var errorMessage = string.Format(ExternalSourceEnabledButNoData, "roles.json");
					MigrationLogger.LogError(errorMessage);
					throw new InvalidOperationException(errorMessage);
				}
			}
			else
			{
				// Use default seed data with Builder validation
				if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("Role", true))
				{
					var validatedRoles = LoadAndValidateDefaultRoles();
					builder.HasData(validatedRoles);
					MigrationLogger.LogInformation($"✅ Using default seed data for roles with Builder validation: {validatedRoles.Count()} roles");
				}
			}
		}

		/// <summary>
		/// Loads roles from JSON and validates them using RoleBuilder
		/// </summary>
		private IEnumerable<IdentityRole> LoadAndValidateRolesFromJson()
		{
			try
			{
				// Clear tracking for new seeding session
				RoleBuilder.ResetTracking();

				// Load JSON roles directly as RoleJsonDto objects
				var jsonRoles = MigrationDataHelper.GetDataFromJson<RoleJsonDto>("roles.json");

				if (!jsonRoles.Any())
				{
					MigrationLogger.LogWarning("No roles found in roles.json file");
					return Enumerable.Empty<IdentityRole>();
				}

				// Validate using RoleBuilder - returns only valid items
				var validatedRoles = MigrationDataHelper.ValidateItems(
					jsonRoles,
					ValidateRoleWithBuilder,
					"role",
					_options.Value.StrictValidation);

				return validatedRoles;
			}
			catch (Exception ex) when (!(ex is ValidationException))
			{
				MigrationLogger.LogError($"Failed to load roles from JSON: {ex.Message}", ex);
				throw new InvalidOperationException($"Role loading failed: {ex.Message}", ex);
			}
		}

		/// <summary>
		/// Loads and validates default roles using RoleBuilder
		/// </summary>
		private IEnumerable<IdentityRole> LoadAndValidateDefaultRoles()
		{
			try
			{
				// Clear tracking for new seeding session
				RoleBuilder.ResetTracking();

				// Get default roles from SeedData
				var data = new SeedData();
				data.Initialize();

				var defaultRoles = new[]
				{
					data.AdminRole,
					data.UserRole,
					data.GuestRole
				};

				// Validate default roles using RoleBuilder - should never fail
				var validatedRoles = MigrationDataHelper.ValidateItems(
					defaultRoles,
					ValidateExistingRoleWithBuilder,
					"default role",
					strictValidation: true);

				return validatedRoles;
			}
			catch (Exception ex)
			{
				MigrationLogger.LogError($"Failed to validate default role data: {ex.Message}", ex);
				throw;
			}
		}

		/// <summary>
		/// Validates a RoleJsonDto using RoleBuilder validation logic
		/// </summary>
		private static IdentityRole ValidateRoleWithBuilder(RoleJsonDto roleDto)
		{
			// Use RoleBuilder for comprehensive validation
			var roleBuilder = new RoleBuilder(roleDto);
			return roleBuilder.Build();
		}

		/// <summary>
		/// Validates an existing IdentityRole using RoleBuilder validation logic
		/// </summary>
		private static IdentityRole ValidateExistingRoleWithBuilder(IdentityRole role)
		{
			// Use RoleBuilder for validation of existing roles
			var roleBuilder = new RoleBuilder(role);
			return roleBuilder.Build();
		}
	}
}