using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationLoggingConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.MonthlyBudgetConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class MonthlyBudgetConfiguration : IEntityTypeConfiguration<MonthlyBudget>
	{
		private readonly IOptions<SeedingOptions> _options;

		public MonthlyBudgetConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public void Configure(EntityTypeBuilder<MonthlyBudget> builder)
		{
			// Reset tracking for new seeding session
			MonthlyBudgetBuilder.ResetTracking();

			// Unique composite index
			builder.HasIndex(mb => new { mb.UserId, mb.Month })
				   .IsUnique();

			// Configuration of the relations
			// We keep history for tracking the monthly budget
			builder.HasOne(mb => mb.User)
				   .WithMany(u => u.MonthlyBudgets)
				   .HasForeignKey(mb => mb.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			// Seeding data 
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Budget", false))
			{
				var validatedBudgets = LoadAndValidateBudgetsFromJson();

				if (validatedBudgets.Any())
				{
					builder.HasData(validatedBudgets);
					MigrationLogger.LogInformation(string.Format(LoadedBudgetsFromJson, validatedBudgets.Count()));
				}
				else
				{
					var errorMessage = string.Format(ExternalSourceEnabledButNoData, "budgets.json");
					MigrationLogger.LogError(errorMessage);
					throw new InvalidOperationException(errorMessage);
				}
			}
			else
			{
				// Use default seed data only if Budget seeding is not disabled
				if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("Budget", true))
				{
					SeedDefaultData(builder);
					MigrationLogger.LogInformation(UsingDefaultBudgetData);
				}
			}
		}

		/// <summary>
		/// Loads monthly budgets from JSON and validates them using MonthlyBudgetBuilder
		/// </summary>
		private IEnumerable<MonthlyBudget> LoadAndValidateBudgetsFromJson()
		{
			try
			{
				// Load JSON budgets directly as MonthlyBudget objects
				var jsonBudgets = MigrationDataHelper.GetDataFromJson<MonthlyBudget>("budgets.json");

				if (!jsonBudgets.Any())
				{
					MigrationLogger.LogWarning(NoBudgetsFoundInJson);
					return Enumerable.Empty<MonthlyBudget>();
				}

				// We need to get users to validate foreign keys
				var users = GetExistingUsers();

				// Validate using MonthlyBudgetBuilder - returns only valid items
				var validatedBudgets = MigrationDataHelper.ValidateItems(
					jsonBudgets,
					budget => ValidateBudgetWithBuilder(budget, users),
					"monthly budget",
					_options.Value.StrictValidation);

				return validatedBudgets;
			}
			catch (Exception ex) when (!(ex is ValidationException))
			{
				MigrationLogger.LogError(string.Format(FailedToLoadBudgetsFromJson, ex.Message), ex);
				throw new InvalidOperationException(string.Format(BudgetLoadingFailed, ex.Message), ex);
			}
		}

		/// <summary>
		/// Validates a MonthlyBudget object using MonthlyBudgetBuilder validation logic
		/// </summary>
		private static MonthlyBudget ValidateBudgetWithBuilder(
			MonthlyBudget budget,
			IEnumerable<User> users)
		{
			// Validate BudgetId first
			if (budget.BudgedId <= 0)
			{
				throw new ValidationException(string.Format(InvalidBudgetId, budget.BudgedId));
			}

			// Find the referenced user
			var user = users.FirstOrDefault(u => u.Id == budget.UserId);
			if (user == null)
			{
				throw new ValidationException(string.Format(UserNotFoundForBudget, budget.UserId));
			}

			// Use MonthlyBudgetBuilder for validation
			var budgetBuilder = new MonthlyBudgetBuilder(
				user,
				budget.BudgetAmount,
				budget.Month
			);

			var validatedBudget = budgetBuilder.Build();
			validatedBudget.BudgedId = budget.BudgedId;

			return validatedBudget;
		}

		/// <summary>
		/// Seeds default data from SeedData class using MonthlyBudgetBuilder validation
		/// </summary>
		private static void SeedDefaultData(EntityTypeBuilder<MonthlyBudget> builder)
		{
			try
			{
				var data = new SeedData();
				data.Initialize();

				// Get users for validation
				var users = new[] { data.Administrator, data.User, data.Guest };

				var defaultBudgets = new[]
				{
					data.Budget1, data.Budget2, data.Budget3
				};

				// Validate default budgets - should never fail
				var validatedBudgets = MigrationDataHelper.ValidateItems(
					defaultBudgets,
					budget => ValidateBudgetWithBuilder(budget, users),
					"default monthly budget",
					strictValidation: true);

				builder.HasData(validatedBudgets);
			}
			catch (Exception ex)
			{
				MigrationLogger.LogError(string.Format(FailedToSeedDefaultBudgetData, ex.Message), ex);
				throw;
			}
		}

		/// <summary>
		/// Gets existing users for foreign key validation
		/// </summary>
		private IEnumerable<User> GetExistingUsers()
		{
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("User", false))
			{
				return MigrationDataHelper.GetDataFromJson<User>("users.json");
			}
			else
			{
				var data = new SeedData();
				data.Initialize();
				return new[] { data.Administrator, data.User, data.Guest };
			}
		}
	}
}