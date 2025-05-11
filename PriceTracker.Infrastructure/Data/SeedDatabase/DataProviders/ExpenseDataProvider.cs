using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.ExpenseDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
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

		private (List<User> users, List<Product> products, List<Store> stores) LoadRelatedData()
		{
			_logger.LogInformation(LoadingRelatedData);

			var users = _userRepository.AllReadOnly().ToList();
			var products = _productRepository.AllReadOnly().ToList();
			var stores = _storeRepository.AllReadOnly().ToList();

			return (users, products, stores);
		}

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

		private bool ExpenseExists(Expense expense)
		{
			return EntityExists(e =>
				e.UserId == expense.UserId &&
				e.ProductId == expense.ProductId &&
				e.StoreId == expense.StoreId &&
				e.DateSpent == expense.DateSpent);
		}

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

		private string FormatExpenseIdentifier(Expense expense)
		{
			return FormatExpenseIdentifier(
				expense.UserId,
				expense.ProductId,
				expense.DateSpent);
		}

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