using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.ExpenseDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	/// <summary>
	/// Provider responsible for loading and managing expense data.
	/// Handles both default and external data sources for expenses.
	/// </summary>
	public class ExpenseDataProvider : BaseDataProvider<Expense>
	{
		private readonly IRepository<User> _userRepository;
		private readonly IRepository<Product> _productRepository;
		private readonly IRepository<Store> _storeRepository;
		private readonly Random _random;

		public ExpenseDataProvider(
			IRepository<Expense> repository,
			IRepository<User> userRepository,
			IRepository<Product> productRepository,
			IRepository<Store> storeRepository,
			IDataSource<Expense>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
			_userRepository = userRepository;
			_productRepository = productRepository;
			_storeRepository = storeRepository;
			_random = new Random();
		}

		/// <summary>
		/// Retrieves all expense data either from external source or generates default data
		/// </summary>
		/// <returns>Collection of expense entities</returns>
		public override IEnumerable<Expense> GetData()
		{
			var expenses = new List<Expense>();

			try
			{
				if (_dataSource != null)
				{
					expenses.AddRange(LoadExpensesFromExternalSource());
				}
				else
				{
					expenses.AddRange(LoadDefaultExpenses());
				}

				_logger.LogInformation(
					string.Format(FinishedLoadingData,
						_typeName,
						expenses.Count));
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(GetData), ex);
			}

			return expenses;
		}

		/// <summary>
		/// Loads expenses from an external data source
		/// </summary>
		/// <returns>Collection of expenses from external source</returns>
		private IEnumerable<Expense> LoadExpensesFromExternalSource()
		{
			var expenses = new List<Expense>();

			try
			{
				_logger.LogInformation(StartingExternalSource);

				var sourceExpenses = LoadFromSourceAsync().Result;
				var (users, products, stores) = LoadRelatedData();

				foreach (var expenseData in sourceExpenses)
				{
					try
					{
						if (!ExpenseExists(expenseData))
						{
							var expense = CreateExpense(expenseData, users, products, stores);
							if (expense != null)
							{
								expenses.Add(expense);
								LogExpenseAdded(expense, isDefault: false);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatExpenseIdentifier(expenseData);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadExpensesFromExternalSource), ex);
			}

			return expenses;
		}

		/// <summary>
		/// Creates default expense records when no external source is available
		/// </summary>
		private IEnumerable<Expense> LoadDefaultExpenses()
		{
			var expenses = new List<Expense>();

			try
			{
				_logger.LogInformation(LoadingDefaultData);

				var (users, products, stores) = LoadRelatedData();

				// Generating couple of expenses for each user
				foreach (var user in users)
				{
					foreach (var product in products.Take(3)) // We take the first three products for example
					{
						try
						{
							var defaultExpense = GenerateDefaultExpense(user, product, stores.First());
							if (!ExpenseExists(defaultExpense))
							{
								var expense = CreateExpense(defaultExpense, users, products, stores);
								if (expense != null)
								{
									expenses.Add(expense);
									LogExpenseAdded(expense, isDefault: true);
								}
							}
						}
						catch (Exception ex)
						{
							var identifier = FormatExpenseIdentifier(user.Id, product.ProductId, DateTime.Today);
							LogProcessingError(identifier, ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadDefaultExpenses), ex);
			}

			return expenses;
		}

		/// <summary>
		/// Loads all related data needed for expense creation
		/// </summary>
		/// <returns>Tuple containing lists of users, products, and stores</returns>
		private (List<User> users, List<Product> products, List<Store> stores) LoadRelatedData()
		{
			_logger.LogInformation(LoadingRelatedData);

			var users = _userRepository.AllReadOnly().ToList();
			var products = _productRepository.AllReadOnly().ToList();
			var stores = _storeRepository.AllReadOnly().ToList();

			return (users, products, stores);
		}

		/// <summary>
		/// Creates a new expense entity using the builder pattern
		/// </summary>
		private Expense? CreateExpense(
			Expense expenseData,
			List<User> users,
			List<Product> products,
			List<Store> stores)
		{
			try
			{
				var user = users.FirstOrDefault(u => u.Id == expenseData.UserId);
				var product = products.FirstOrDefault(p => p.ProductId == expenseData.ProductId);
				var store = stores.FirstOrDefault(s => s.StoreId == expenseData.StoreId);

				if (user == null || product == null || store == null)
				{
					return null;
				}

				return new ExpenseBuilder(
					user: user,
					expenseType: expenseData.ExpenseType,
					product: product,
					store: store,
					amountSpent: expenseData.AmountSpent,
					dateSpent: expenseData.DateSpent,
					description: expenseData.Description)
					.Build();
			}
			catch (Exception ex)
			{
				var identifier = FormatExpenseIdentifier(expenseData);
				LogProcessingError(identifier, ex);
				return null;
			}
		}

		/// <summary>
		/// Generates a default expense with random amount for testing purposes
		/// </summary>
		private Expense GenerateDefaultExpense(User user, Product product, Store store)
		{
			var randomAmount = _random.Next(1000, 100000) / 100.0m; 

			return new Expense
			{
				UserId = user.Id,
				User = user,
				ProductId = product.ProductId,
				Product = product,
				StoreId = store.StoreId,
				Store = store,
				ExpenseType = (ExpenseType)_random.Next(1, 10),
				AmountSpent = randomAmount,
				DateSpent = DateTime.Today.AddDays(-_random.Next(0, 30)),
				Description = $"Test expense for {product.ProductName}"
			};
		}

		/// <summary>
		/// Checks if an expense already exists in the database
		/// </summary>
		private bool ExpenseExists(Expense expense)
		{
			return EntityExists(e =>
				e.UserId == expense.UserId &&
				e.ProductId == expense.ProductId &&
				e.StoreId == expense.StoreId &&
				e.DateSpent == expense.DateSpent);
		}

		/// <summary>
		/// Logs the addition of a new expense to the system
		/// </summary>
		private void LogExpenseAdded(Expense expense, bool isDefault)
		{
			var message = string.Format(
				isDefault ? DefaultExpenseAdded
						 : ExpenseAdded,
				expense.User?.UserName ?? expense.UserId.ToString(),
				expense.Product?.ProductName ?? expense.ProductId.ToString(),
				expense.AmountSpent);

			_logger.LogInformation(message);
		}

		/// <summary>
		/// Creates a formatted identifier for an expense entity
		/// </summary>
		private string FormatExpenseIdentifier(Expense expense)
		{
			return FormatExpenseIdentifier(
				expense.UserId,
				expense.ProductId,
				expense.DateSpent);
		}

		/// /// <summary>
		/// Creates a formatted identifier using individual expense components
		/// </summary>
		private string FormatExpenseIdentifier(int userId, int productId, DateTime date)
		{
			return string.Format(
				ExpenseIdentifier,
				userId,
				productId,
				date);
		}
	}
}