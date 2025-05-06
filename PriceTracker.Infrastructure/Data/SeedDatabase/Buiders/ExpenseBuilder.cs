using PriceTracker.Infrastructure.Constants;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Buiders
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
				throw new ArgumentNullException(nameof(user), "User cannot be null when creating an expense");
			if (product == null)
				throw new ArgumentNullException(nameof(product), "Product cannot be null when creating an expense");
			if (store == null)
				throw new ArgumentNullException(nameof(store), "Store cannot be null when creating an expense");

			// Enum validation
			if (!Enum.IsDefined(typeof(ExpenseType), expenseType))
			{
				throw new ArgumentException($"Invalid expense type value: {expenseType}", nameof(expenseType));
			}

			// Amount validations with better messages
			if (amountSpent <= 0)
			{
				throw new ArgumentException(
					$"Amount must be greater than zero. Provided value: {amountSpent:C}",
					nameof(amountSpent));
			}

			if (amountSpent > MaxAllowedAmount)
			{
				throw new ArgumentException(
					$"Amount cannot exceed {MaxAllowedAmount:C}. Provided value: {amountSpent:C}",
					nameof(amountSpent));
			}

			// Date validations with better messages
			if (dateSpent > DateTime.Now)
			{
				throw new ArgumentException(
					$"Date cannot be in the future. Current date: {DateTime.Now:g}, Provided date: {dateSpent:g}",
					nameof(dateSpent));
			}

			if (description?.Length > DataConstants.expenseDescriptionMaxLength)
			{
				throw new ArgumentException(
					$"Description length ({description.Length}) exceeds maximum allowed length ({DataConstants.expenseDescriptionMaxLength})",
					nameof(description));
			}
		}


		public Expense Build() => _expense;
	}

}
