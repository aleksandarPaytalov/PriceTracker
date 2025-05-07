using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Builder for creating validated MonthlyBudget entities
	/// </summary>
	public class MonthlyBudgetBuilder : IBuilder<MonthlyBudget>
	{
		private readonly MonthlyBudget _budget;
		private const decimal MaxAllowedBudget = 1000000m;

		/// <summary>
		/// Creates a new monthly budget with validation
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
				ValidateInitialBudgetData(user, budget, month);

				_budget = new MonthlyBudget
				{
					UserId = user.Id,
					User = user,
					BudgetAmount = budget,
					Month = month
				};
			}
			catch (Exception ex) when (ex is not ValidationException) 
			{
				throw new ValidationException($"Failed to create Monthly budget: {ex.Message}");
			}
			
		}

		/// <summary>
		/// Builds and returns the validated MonthlyBudget instance
		/// </summary>
		/// <returns>A validated MonthlyBudget object</returns>
		public MonthlyBudget Build() => _budget;

		private void ValidateInitialBudgetData(User user, decimal budget, Month month)
		{
			// User validation
			if (user == null)
			{
				throw new ValidationException(MonthlyBudgetConstants.UserRequired);
			}

			// Amount validations
			if (budget <= 0)
			{
				throw new ValidationException(String.Format(MonthlyBudgetConstants.InvalidMonth, budget));
			}

			// Maximum threshold validation
			if (budget > MaxAllowedBudget)
			{
				throw new ValidationException(String.Format(MonthlyBudgetConstants.ExceedsMaxAmount, MaxAllowedBudget, budget));
			}

			// Month enum validation
			if (!Enum.IsDefined(typeof(Month), month))
			{
				throw new ValidationException(String.Format(MonthlyBudgetConstants.InvalidMonth, month));
			}
		}
	}
}
