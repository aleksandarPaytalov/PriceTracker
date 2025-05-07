using PriceTracker.Infrastructure.Constants;
using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Builder for creating validated Expense entities
	/// </summary>
	public class ExpenseBuilder : IBuilder<Expense>
	{
		/// <summary>
		/// Creates a new expense with all required data
		/// </summary>
		/// <param name="user">The user creating the expense</param>
		/// <param name="expenseType">Type of the expense</param>
		/// <param name="product">The product associated with the expense</param>
		/// <param name="store">The store where the expense occurred</param>
		/// <param name="amountSpent">The amount of money spent</param>
		/// <param name="dateSpent">The date when the expense occurred</param>
		/// <param name="description">Optional description of the expense</param>
		/// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
		/// <exception cref="ArgumentException">Thrown when validation fails</exception>
		
		private readonly Expense _expense;
		private const decimal MaxAllowedAmount = 10000m;

		public ExpenseBuilder(
			User user,
			ExpenseType expenseType,
			Product product,
			Store store,
			decimal amountSpent,
			DateTime dateSpent,
			string? description = null)
		{
			try
			{
				ValidateInitialData(user, product, store, amountSpent, dateSpent, expenseType, description);

				_expense = new Expense
				{
					User = user,
					UserId = user.Id,
					ExpenseType = expenseType,
					Product = product,
					ProductId = product.ProductId,
					Store = store,
					StoreId = store.StoreId,
					AmountSpent = amountSpent,
					DateSpent = dateSpent,
					Description = description
				};
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create Expense: {ex.Message}");
			}
		}

		private void ValidateInitialData(
		User user,
		Product product,
		Store store,
		decimal amountSpent,
		DateTime dateSpent,
		ExpenseType expenseType,
		string? description)
		{
			// Null checks with better messages
			if (user == null)
				throw new ArgumentNullException(nameof(user), ExpenseConstants.UserRequired);
			if (product == null)
				throw new ArgumentNullException(nameof(product), ExpenseConstants.ProductRequired);
			if (store == null)
				throw new ArgumentNullException(nameof(store), ExpenseConstants.StoreRequired);

			// Enum validation
			if (!Enum.IsDefined(typeof(ExpenseType), expenseType))
			{
				throw new ArgumentException(String.Format(ExpenseConstants.InvalidExpenseType, expenseType));
			}

			// Amount validations with better messages
			if (amountSpent <= 0)
			{
				throw new ArgumentException(String.Format(ExpenseConstants.InvalidAmount, amountSpent));
			}

			if (amountSpent > MaxAllowedAmount)
			{
				throw new ArgumentException(String.Format(ExpenseConstants.ExceedsMaxAmount, MaxAllowedAmount, amountSpent));
			}

			// Date validations with better messages
			if (dateSpent > DateTime.Now)
			{
				throw new ArgumentException(String.Format(ExpenseConstants.FutureDate, DateTime.Now, dateSpent));
			}

			if (description?.Length > DataConstants.expenseDescriptionMaxLength)
			{
				throw new ArgumentException(
					String.Format(ExpenseConstants.DescriptionTooLong,
						description.Length,
						DataConstants.expenseDescriptionMaxLength));
			}
		}

		public Expense Build() => _expense;
	}

}
