using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationLoggingConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations.IdentityConfiguration
{
	/// <summary>
	/// User configuration with JSON loading, Builder validation, and password hashing orchestration
	/// </summary>
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		private readonly IOptions<SeedingOptions> _options;

		public UserConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public void Configure(EntityTypeBuilder<User> builder)
		{
			UserBuilder.ResetTracking();

			// Configure entity properties
			builder.HasKey(u => u.Id);
			builder.HasIndex(u => u.NormalizedUserName).IsUnique();
			builder.HasIndex(u => u.NormalizedEmail).IsUnique();

			builder.Property(u => u.UserName).HasMaxLength(256);
			builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
			builder.Property(u => u.Email).HasMaxLength(256);
			builder.Property(u => u.NormalizedEmail).HasMaxLength(256);

			// Configure relationships with business entities
			builder.HasMany(u => u.Expenses)
				   .WithOne(e => e.User)
				   .HasForeignKey(e => e.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(u => u.MonthlyBudgets)
				   .WithOne(mb => mb.User)
				   .HasForeignKey(mb => mb.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(u => u.Tasks)
				   .WithOne(t => t.User)
				   .HasForeignKey(t => t.UserId)
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(u => u.Notifications)
				   .WithOne(n => n.User)
				   .HasForeignKey(n => n.UserId)
				   .OnDelete(DeleteBehavior.NoAction);

			// Seed data using JSON + Builder approach with password hashing
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("User", false))
			{
				var validatedUsers = LoadAndValidateUsersFromJsonAsync();

				if (validatedUsers.Any())
				{
					builder.HasData(validatedUsers);
					MigrationLogger.LogInformation(string.Format(
						LoadedFromJsonWithPasswordHashing, validatedUsers.Count()));
				}
				else
				{
					var errorMessage = string.Format(ExternalSourceEnabledButNoData, "users.json");
					MigrationLogger.LogError(errorMessage);
					throw new InvalidOperationException(errorMessage);
				}
			}
			else
			{
				// Use default seed data with Builder validation and password hashing
				if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("User", true))
				{
					var validatedUsers = LoadAndValidateDefaultUsersAsync();
					builder.HasData(validatedUsers);
					MigrationLogger.LogInformation(string.Format(
						UsingDefaultUsersWithPasswordHashing, 
						validatedUsers.Count()));
				}
			}
		}

		/// <summary>
		/// Loads users from JSON and validates them using UserBuilder with password hashing
		/// </summary>
		private IEnumerable<User> LoadAndValidateUsersFromJsonAsync()
		{
			try
			{
				// Clear tracking for new seeding session
				UserBuilder.ResetTracking();

				// Load JSON users directly as UserJsonDto objects
				var jsonUsers = MigrationDataHelper.GetDataFromJson<UserJsonDto>("users.json");

				if (!jsonUsers.Any())
				{
					MigrationLogger.LogWarning(string.Format(
						NoItemsFoundInJson, "users", "users.json"));
					return Enumerable.Empty<User>();
				}

				// Create password hasher for validation
				var passwordHasher = new PasswordHasher<User>();

				// Validate using UserBuilder with password hashing - returns only valid items
				var validatedUsers = MigrationDataHelper.ValidateItems(
					jsonUsers,
					userDto => ValidateUserWithBuilderAsync(userDto, passwordHasher),
					"user",
					_options.Value.StrictValidation);

				return validatedUsers;
			}
			catch (Exception ex) when (!(ex is ValidationException))
			{
				MigrationLogger.LogError(string.Format(
					FailedToLoadFromJson, "users", ex.Message), ex);
				throw new InvalidOperationException(string.Format(
					LoadingFailed, "User", ex.Message), ex);
			}
		}

		/// <summary>
		/// Loads and validates default users using UserBuilder with password hashing
		/// </summary>
		private IEnumerable<User> LoadAndValidateDefaultUsersAsync()
		{
			try
			{
				// Clear tracking for new seeding session
				UserBuilder.ResetTracking();

				// Get default users from SeedData (without password hashes)
				var data = new SeedData();
				data.Initialize();

				var defaultUsersWithPasswords = new[]
				{
					new { User = data.Administrator, Password = SeedData.GetDefaultPasswordForUser(data.Administrator.Email!) },
					new { User = data.User, Password = SeedData.GetDefaultPasswordForUser(data.User.Email!) },
					new { User = data.Guest, Password = SeedData.GetDefaultPasswordForUser(data.Guest.Email!) }
				};

				// Create password hasher
				var passwordHasher = new PasswordHasher<User>();

				// Validate default users using UserBuilder with password hashing
				var validatedUsers = new List<User>();

				foreach (var userWithPassword in defaultUsersWithPasswords)
				{
					try
					{
						var userBuilder = new UserBuilder(userWithPassword.User, userWithPassword.Password, passwordHasher);
						var validatedUser = userBuilder.Build();
						validatedUsers.Add(validatedUser);
					}
					catch (ValidationException ex)
					{
						MigrationLogger.LogError(string.Format(
							FailedToValidateDefaultData, 
							$"user {userWithPassword.User.Email}", 
							ex.Message), 
							ex);

						if (_options.Value.StrictValidation)
						{
							throw;
						}
					}
				}

				return validatedUsers;
			}
			catch (Exception ex)
			{
				MigrationLogger.LogError(string.Format(
					FailedToValidateDefaultData, 
					"user", 
					ex.Message), 
					ex);
				throw;
			}
		}

		/// <summary>
		/// Validates a UserJsonDto using UserBuilder validation logic with password hashing
		/// </summary>
		private static User ValidateUserWithBuilderAsync(UserJsonDto userDto, IPasswordHasher<User> passwordHasher)
		{
			// Use UserBuilder for comprehensive validation and password hashing
			var userBuilder = new UserBuilder(userDto, passwordHasher);
			return userBuilder.Build();
		}
	}
}