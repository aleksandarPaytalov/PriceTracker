using PriceTracker.Infrastructure.Constants;
using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ExpenseConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.BuilderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Enhanced ExpenseBuilder with in-memory duplication tracking
	/// </summary>
	public class ExpenseBuilder : IBuilder<Expense>
	{
		private readonly Expense _expense;
		private const decimal MaxAllowedAmount = 10000m;
		private static readonly HashSet<string> _currentSeedExpenses = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new expense with enhanced validation including duplication tracking
		/// </summary>
		/// <param name="user">The user creating the expense</param>
		/// <param name="expenseType">Type of the expense</param>
		/// <param name="product">The product associated with the expense</param>
		/// <param name="store">The store where the expense occurred</param>
		/// <param name="amountSpent">The amount of money spent</param>
		/// <param name="dateSpent">The date when the expense occurred</param>
		/// <param name="description">Optional description of the expense</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
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
				ValidateExpenseInputData(user, expenseType, product, store, amountSpent, dateSpent, description);

				_expense = new Expense
				{
					UserId = user.Id,
					ProductId = product.ProductId,
					StoreId = store.StoreId,
					AmountSpent = amountSpent,
					DateSpent = dateSpent,
					ExpenseType = expenseType,
					Description = description
				};

				// Track in current seed session to prevent duplicates
				var expenseKey = $"{user.Id}|{product.ProductId}|{store.StoreId}|{dateSpent:yyyy-MM-dd HH:mm:ss}|{amountSpent}".ToLower();
				_currentSeedExpenses.Add(expenseKey);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException(string.Format(FailedToCreateExpense, ex.Message));
			}
		}

		/// <summary>
		/// Builds and returns the validated Expense instance
		/// </summary>
		/// <returns>A validated Expense object</returns>
		public Expense Build() => _expense;

		/// <summary>
		/// Validates expense input data with comprehensive checks
		/// </summary>
		private static void ValidateExpenseInputData(
			User user,
			ExpenseType expenseType,
			Product product,
			Store store,
			decimal amountSpent,
			DateTime dateSpent,
			string? description)
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

			// Product validation
			if (product == null)
			{
				throw new ValidationException(ProductRequired);
			}

			if (product.ProductId <= 0)
			{
				throw new ValidationException(ProductIdRequired);
			}

			// Store validation
			if (store == null)
			{
				throw new ValidationException(StoreRequired);
			}

			if (store.StoreId <= 0)
			{
				throw new ValidationException(StoreIdRequired);
			}

			// Expense type validation
			if (!Enum.IsDefined(typeof(ExpenseType), expenseType))
			{
				throw new ValidationException(string.Format(InvalidExpenseType, expenseType));
			}

			// Amount validations
			if (amountSpent <= 0)
			{
				throw new ValidationException(string.Format(InvalidAmount, amountSpent));
			}

			if (amountSpent > MaxAllowedAmount)
			{
				throw new ValidationException(string.Format(ExceedsMaxAmount, MaxAllowedAmount, amountSpent));
			}

			// Date validations
			if (dateSpent > DateTime.Now)
			{
				throw new ValidationException(string.Format(FutureDate, DateTime.Now, dateSpent));
			}

			if (!IsValidDate(dateSpent))
			{
				throw new ValidationException(InvalidDateFormat);
			}

			// Description validation
			if (description?.Length > DataConstants.expenseDescriptionMaxLength)
			{
				throw new ValidationException(string.Format(DescriptionTooLong, description.Length, DataConstants.expenseDescriptionMaxLength));
			}

			// Security validation for description
			if (!string.IsNullOrEmpty(description) && ContainsForbiddenContent(description))
			{
				throw new ValidationException(DescriptionContainsForbiddenContent);
			}

			// In-memory duplication check for current seed session
			ValidateExpenseUniqueness(user.Id, product.ProductId, store.StoreId, dateSpent, amountSpent);
		}

		/// <summary>
		/// Validates expense uniqueness in current seeding session
		/// </summary>
		private static void ValidateExpenseUniqueness(string userId, int productId, int storeId, DateTime dateSpent, decimal amountSpent)
		{
			var expenseKey = $"{userId}|{productId}|{storeId}|{dateSpent:yyyy-MM-dd HH:mm:ss}|{amountSpent}".ToLower();

			if (_currentSeedExpenses.Contains(expenseKey))
			{
				throw new ValidationException(string.Format(DuplicateExpenseInSession, userId, productId, storeId, dateSpent, amountSpent));
			}
		}

		/// <summary>
		/// Validates if date is in correct format
		/// </summary>
		private static bool IsValidDate(DateTime date)
		{
			return DateTime.TryParse(date.ToString(), out _);
		}

		/// <summary>
		/// Checks for forbidden content patterns
		/// </summary>
		private static bool ContainsForbiddenContent(string content)
		{
			if (string.IsNullOrEmpty(content)) return false;

			var forbiddenPatterns = new[]
			{
				"<script", "javascript:", "vbscript:", "onload=", "onerror=",
				"onclick=", "onmouseover=", "alert(", "eval(", "document.cookie",
				"<iframe", "<object", "<embed", "data:text/html", "data:text/javascript",
				"<", ">", "src=", "href=", "style=", "expression(", "url(",
				"select ", "insert ", "update ", "delete ", "drop ", "union ",
				"exec ", "execute ", "--", "/*", "*/", "@@", "char", "nchar",
				"varchar", "nvarchar", "table", "database", "sysobjects", "syscolumns"
			};

			return forbiddenPatterns.Any(pattern =>
				content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Clear tracking collections for new seeding session
		/// Call this before starting a new migration or seeding operation
		/// </summary>
		public static void ResetTracking()
		{
			_currentSeedExpenses.Clear();
		}

		/// <summary>
		/// Get count of currently tracked expenses in this session
		/// </summary>
		public static int GetTrackedExpenseCount()
		{
			return _currentSeedExpenses.Count;
		}

		/// <summary>
		/// Check if an expense combination is already tracked in current session
		/// </summary>
		public static bool IsExpenseTracked(string userId, int productId, int storeId, DateTime dateSpent, decimal amountSpent)
		{
			var expenseKey = $"{userId}|{productId}|{storeId}|{dateSpent:yyyy-MM-dd HH:mm:ss}|{amountSpent}".ToLower();
			return _currentSeedExpenses.Contains(expenseKey);
		}

		/// <summary>
		/// Get all tracked expense keys in current session
		/// </summary>
		public static IEnumerable<string> GetTrackedExpenseKeys()
		{
			return _currentSeedExpenses.AsEnumerable();
		}
	}
}