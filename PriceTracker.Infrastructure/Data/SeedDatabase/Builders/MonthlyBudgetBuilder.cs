using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.MonthlyBudgetConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.BuilderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Enhanced MonthlyBudgetBuilder with in-memory duplication tracking
	/// </summary>
	public class MonthlyBudgetBuilder : IBuilder<MonthlyBudget>
	{
		private readonly MonthlyBudget _budget;
		private const decimal MaxAllowedBudget = 1000000m;
		private static readonly HashSet<string> _currentSeedBudgets = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new monthly budget with enhanced validation including duplication tracking
		/// </summary>
		/// <param name="user">The user associated with the budget</param>
		/// <param name="budget">The amount of the monthly budget</param>
		/// <param name="month">The month for which the budget is set</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public MonthlyBudgetBuilder(
			User user,
			decimal budget,
			Month month)
		{
			try
			{
				ValidateBudgetInputData(user, budget, month);

				_budget = new MonthlyBudget
				{
					UserId = user.Id,
					BudgetAmount = budget,
					Month = month
				};

				// Track in current seed session to prevent duplicates (UserId + Month is unique)
				var budgetKey = $"{user.Id}|{(int)month}".ToLower();
				_currentSeedBudgets.Add(budgetKey);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException(string.Format(FailedToCreateMonthlyBudget, ex.Message));
			}
		}

		/// <summary>
		/// Builds and returns the validated MonthlyBudget instance
		/// </summary>
		/// <returns>A validated MonthlyBudget object</returns>
		public MonthlyBudget Build() => _budget;

		/// <summary>
		/// Validates budget input data with comprehensive checks
		/// </summary>
		private static void ValidateBudgetInputData(User user, decimal budget, Month month)
		{
			// User validation
			if (user == null)
			{
				throw new ValidationException(UserRequired);
			}

			if (string.IsNullOrWhiteSpace(user.Id))
			{
				throw new ValidationException(UserIdRequired);
			}

			// Amount validations
			if (budget <= 0)
			{
				throw new ValidationException(string.Format(InvalidAmount, budget));
			}

			// Maximum threshold validation
			if (budget > MaxAllowedBudget)
			{
				throw new ValidationException(string.Format(ExceedsMaxAmount, MaxAllowedBudget, budget));
			}

			// Month enum validation
			if (!Enum.IsDefined(typeof(Month), month))
			{
				throw new ValidationException(string.Format(InvalidMonth, month));
			}

			// Business rules validation
			ValidateBusinessRules(budget, month);

			// In-memory duplication check for current seed session
			ValidateBudgetUniqueness(user.Id, month);
		}

		/// <summary>
		/// Validates business rules for budget
		/// </summary>
		private static void ValidateBusinessRules(decimal budget, Month month)
		{
			// Validate reasonable budget amounts
			if (budget < 10m)
			{
				throw new ValidationException(string.Format(BudgetTooLow, budget));
			}

			// Validate month is reasonable (allow past, current, and future months within reason)
			var currentMonth = (Month)DateTime.Now.Month;
			var currentYear = DateTime.Now.Year;

			// Allow reasonable range - previous year to next year
			var monthDifference = Math.Abs((int)month - (int)currentMonth);
			if (monthDifference > 11) // More than 11 months difference is questionable
			{
				// This is just a warning-level validation, not critical
				// Could log a warning instead of throwing exception
			}
		}

		/// <summary>
		/// Validates budget uniqueness in current seeding session
		/// </summary>
		private static void ValidateBudgetUniqueness(string userId, Month month)
		{
			var budgetKey = $"{userId}|{(int)month}".ToLower();

			if (_currentSeedBudgets.Contains(budgetKey))
			{
				throw new ValidationException(string.Format(DuplicateBudgetInSession, userId, month));
			}
		}

		/// <summary>
		/// Clear tracking collections for new seeding session
		/// Call this before starting a new migration or seeding operation
		/// </summary>
		public static void ResetTracking()
		{
			_currentSeedBudgets.Clear();
		}

		/// <summary>
		/// Get count of currently tracked budgets in this session
		/// </summary>
		public static int GetTrackedBudgetCount()
		{
			return _currentSeedBudgets.Count;
		}

		/// <summary>
		/// Check if a budget combination is already tracked in current session
		/// </summary>
		public static bool IsBudgetTracked(string userId, Month month)
		{
			var budgetKey = $"{userId}|{(int)month}".ToLower();
			return _currentSeedBudgets.Contains(budgetKey);
		}

		/// <summary>
		/// Get all tracked budget keys in current session
		/// </summary>
		public static IEnumerable<string> GetTrackedBudgetKeys()
		{
			return _currentSeedBudgets.AsEnumerable();
		}
	}
}