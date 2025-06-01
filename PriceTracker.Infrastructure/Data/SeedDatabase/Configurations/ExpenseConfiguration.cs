using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationLoggingConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ExpenseConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
	{
		private readonly IOptions<SeedingOptions> _options;

		public ExpenseConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public void Configure(EntityTypeBuilder<Expense> builder)
		{
			// Reset tracking for new seeding session
			ExpenseBuilder.ResetTracking();

			// Relations Config
			// We keep the expenses for tracking/accounting
			builder.HasOne(e => e.User)
				   .WithMany(u => u.Expenses)
				   .HasForeignKey(e => e.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(e => e.Product)
				   .WithMany(p => p.Expenses)
				   .HasForeignKey(e => e.ProductId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(e => e.Store)
				   .WithMany(s => s.Expenses)
				   .HasForeignKey(e => e.StoreId)
				   .OnDelete(DeleteBehavior.Restrict);

			// Seeding data 
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Expense", false))
			{
				var validatedExpenses = LoadAndValidateExpensesFromJson();

				if (validatedExpenses.Any())
				{
					builder.HasData(validatedExpenses);
					MigrationLogger.LogInformation(string.Format(LoadedExpensesFromJson, validatedExpenses.Count()));
				}
				else
				{
					var errorMessage = string.Format(ExternalSourceEnabledButNoData, "expenses.json");
					MigrationLogger.LogError(errorMessage);
					throw new InvalidOperationException(errorMessage);
				}
			}
			else
			{
				// Use default seed data only if Expense seeding is not disabled
				if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("Expense", true))
				{
					SeedDefaultData(builder);
					MigrationLogger.LogInformation(UsingDefaultExpenseData);
				}
			}
		}

		/// <summary>
		/// Loads expenses from JSON and validates them using ExpenseBuilder
		/// </summary>
		private IEnumerable<Expense> LoadAndValidateExpensesFromJson()
		{
			try
			{
				// Load JSON expenses directly as Expense objects
				var jsonExpenses = MigrationDataHelper.GetDataFromJson<Expense>("expenses.json");

				if (!jsonExpenses.Any())
				{
					MigrationLogger.LogWarning(NoExpensesFoundInJson);
					return Enumerable.Empty<Expense>();
				}

				// We need to get users, products and stores to validate foreign keys
				var users = GetExistingUsers();
				var products = GetExistingProducts();
				var stores = GetExistingStores();

				// Validate using ExpenseBuilder - returns only valid items
				var validatedExpenses = MigrationDataHelper.ValidateItems(
					jsonExpenses,
					expense => ValidateExpenseWithBuilder(expense, users, products, stores),
					"expense",
					_options.Value.StrictValidation);

				return validatedExpenses;
			}
			catch (Exception ex) when (!(ex is ValidationException))
			{
				MigrationLogger.LogError(string.Format(FailedToLoadExpensesFromJson, ex.Message), ex);
				throw new InvalidOperationException(string.Format(ExpenseLoadingFailed, ex.Message), ex);
			}
		}

		/// <summary>
		/// Validates an Expense object using ExpenseBuilder validation logic
		/// </summary>
		private static Expense ValidateExpenseWithBuilder(
			Expense expense,
			IEnumerable<User> users,
			IEnumerable<Product> products,
			IEnumerable<Store> stores)
		{
			// Validate ExpenseId first
			if (expense.ExpenseId <= 0)
			{
				throw new ValidationException(string.Format(InvalidExpenseId, expense.ExpenseId));
			}

			// Find the referenced user
			var user = users.FirstOrDefault(u => u.Id == expense.UserId);
			if (user == null)
			{
				throw new ValidationException(string.Format(UserNotFound, expense.UserId));
			}

			// Find the referenced product
			var product = products.FirstOrDefault(p => p.ProductId == expense.ProductId);
			if (product == null)
			{
				throw new ValidationException(string.Format(ProductNotFoundForExpense, expense.ProductId));
			}

			// Find the referenced store
			var store = stores.FirstOrDefault(s => s.StoreId == expense.StoreId);
			if (store == null)
			{
				throw new ValidationException(string.Format(StoreNotFoundForExpense, expense.StoreId));
			}

			// Use ExpenseBuilder for validation
			var expenseBuilder = new ExpenseBuilder(
				user,
				expense.ExpenseType,
				product,
				store,
				expense.AmountSpent,
				expense.DateSpent,
				expense.Description
			);

			var validatedExpense = expenseBuilder.Build();
			validatedExpense.ExpenseId = expense.ExpenseId;

			return validatedExpense;
		}

		/// <summary>
		/// Seeds default data from SeedData class using ExpenseBuilder validation
		/// </summary>
		private static void SeedDefaultData(EntityTypeBuilder<Expense> builder)
		{
			try
			{
				var data = new SeedData();
				data.Initialize();

				// Get related entities for validation
				var users = new[] { data.Administrator, data.User, data.Guest };
				var products = new[]
				{
					data.Product1, data.Product2, data.Product3, data.Product4, data.Product5,
					data.Product6, data.Product7, data.Product8, data.Product9
				};
				var stores = new[]
				{
					data.Store1, data.Store2, data.Store3, data.Store4,
					data.Store5, data.Store6, data.Store7, data.Store8
				};

				var defaultExpenses = new[]
				{
					data.Expense1, data.Expense2, data.Expense3,
					data.Expense4, data.Expense5, data.Expense6
				};

				// Validate default expenses - should never fail
				var validatedExpenses = MigrationDataHelper.ValidateItems(
					defaultExpenses,
					expense => ValidateExpenseWithBuilder(expense, users, products, stores),
					"default expense",
					strictValidation: true);

				builder.HasData(validatedExpenses);
			}
			catch (Exception ex)
			{
				MigrationLogger.LogError(string.Format(FailedToSeedDefaultExpenseData, ex.Message), ex);
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

		/// <summary>
		/// Gets existing products for foreign key validation
		/// </summary>
		private IEnumerable<Product> GetExistingProducts()
		{
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Product", false))
			{
				return MigrationDataHelper.GetDataFromJson<Product>("products.json");
			}
			else
			{
				var data = new SeedData();
				data.Initialize();
				return new[]
				{
					data.Product1, data.Product2, data.Product3, data.Product4, data.Product5,
					data.Product6, data.Product7, data.Product8, data.Product9
				};
			}
		}

		/// <summary>
		/// Gets existing stores for foreign key validation
		/// </summary>
		private IEnumerable<Store> GetExistingStores()
		{
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Store", false))
			{
				return MigrationDataHelper.GetDataFromJson<Store>("stores.json");
			}
			else
			{
				var data = new SeedData();
				data.Initialize();
				return new[]
				{
					data.Store1, data.Store2, data.Store3, data.Store4,
					data.Store5, data.Store6, data.Store7, data.Store8
				};
			}
		}
	}
}