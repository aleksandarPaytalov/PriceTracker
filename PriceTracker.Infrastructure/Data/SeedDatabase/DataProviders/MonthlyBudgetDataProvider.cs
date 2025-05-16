using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.MonthlyBudgetDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	/// <summary>
	/// Provider responsible for loading and managing monthly budget data
	/// </summary>
	public class MonthlyBudgetDataProvider : BaseDataProvider<MonthlyBudget>
	{
		private readonly IRepository<User> _userRepository;
		private readonly Random _random;

		public MonthlyBudgetDataProvider(
			IRepository<MonthlyBudget> repository,
			IRepository<User> userRepository,
			IDataSource<MonthlyBudget>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
			_userRepository = userRepository;
			_random = new Random();
		}

		/// <summary>
		/// Main method to retrieve budget data
		/// Returns collection of monthly budgets from external source or default data
		/// </summary>
		public override IEnumerable<MonthlyBudget> GetData()
		{
			var budgets = new List<MonthlyBudget>();

			try
			{
				if (_dataSource != null)
				{
					budgets.AddRange(LoadBudgetsFromExternalSource());
				}
				else
				{
					budgets.AddRange(LoadDefaultBudgets());
				}

				_logger.LogInformation(
					string.Format(FinishedLoadingData,
						_typeName,
						budgets.Count));
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(GetData), ex);
			}

			return budgets;
		}

		/// <summary>
		/// Loads budgets from external source (e.g., JSON file)
		/// </summary>
		private IEnumerable<MonthlyBudget> LoadBudgetsFromExternalSource()
		{
			var budgets = new List<MonthlyBudget>();

			try
			{
				_logger.LogInformation(StartingExternalSource);

				var sourceBudgets = LoadFromSourceAsync().Result;
				var users = LoadUsers();

				foreach (var budgetData in sourceBudgets)
				{
					try
					{
						if (!BudgetExists(budgetData))
						{
							var budget = CreateBudget(budgetData, users);
							if (budget != null)
							{
								budgets.Add(budget);
								LogBudgetAdded(budget, isDefault: false);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatBudgetIdentifier(budgetData);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadBudgetsFromExternalSource), ex);
			}

			return budgets;
		}

		/// <summary>
		/// Creates default budgets when no external source is available
		/// </summary>
		private IEnumerable<MonthlyBudget> LoadDefaultBudgets()
		{
			var budgets = new List<MonthlyBudget>();

			try
			{
				_logger.LogInformation(LoadingDefaultData);

				var users = LoadUsers();
				var currentMonth = (Month)DateTime.Now.Month;
				var nextMonth = currentMonth == Month.December ? Month.January : currentMonth + 1;

				foreach (var user in users)
				{
					try
					{
						// Creating a budget for the current month
						var currentBudget = GenerateDefaultBudget(user, currentMonth);
						if (!BudgetExists(currentBudget))
						{
							var budget = CreateBudget(currentBudget, users);
							if (budget != null)
							{
								budgets.Add(budget);
								LogBudgetAdded(budget, isDefault: true);
							}
						}

						// Creating a budget for the next month
						var nextBudget = GenerateDefaultBudget(user, nextMonth);
						if (!BudgetExists(nextBudget))
						{
							var budget = CreateBudget(nextBudget, users);
							if (budget != null)
							{
								budgets.Add(budget);
								LogBudgetAdded(budget, isDefault: true);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatBudgetIdentifier(user.Id, currentMonth);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadDefaultBudgets), ex);
			}

			return budgets;
		}

		/// <summary>
		/// Loads all users from the database
		/// </summary>
		private List<User> LoadUsers()
		{
			_logger.LogInformation(LoadingRelatedData);
			return _userRepository.AllReadOnly().ToList();
		}

		/// <summary>
		/// Creates a new MonthlyBudget instance using the builder pattern
		/// </summary>
		private MonthlyBudget? CreateBudget(MonthlyBudget budgetData, List<User> users)
		{
			try
			{
				var user = users.FirstOrDefault(u => u.Id == budgetData.UserId);
				if (user == null) return null;

				return new MonthlyBudgetBuilder(
					user: user,
					budget: budgetData.BudgetAmount,
					month: budgetData.Month)
					.Build();
			}
			catch (Exception ex)
			{
				var identifier = FormatBudgetIdentifier(budgetData);
				LogProcessingError(identifier, ex);
				return null;
			}
		}

		/// <summary>
		/// Generates a default budget with random amount for testing purposes
		/// </summary>
		private MonthlyBudget GenerateDefaultBudget(User user, Month month)
		{
			// Generating a random budget for the next month 
			var randomBudget = _random.Next(100000, 500000) / 100.0m;

			return new MonthlyBudget
			{
				UserId = user.Id,
				User = user,
				Month = month,
				BudgetAmount = randomBudget
			};
		}

		/// <summary>
		/// Checks if a budget already exists for specific user and month
		/// </summary>
		private bool BudgetExists(MonthlyBudget budget)
		{
			return EntityExists(b =>
				b.UserId == budget.UserId &&
				b.Month == budget.Month);
		}

		/// <summary>
		/// Add message in log if budget is successfully added
		/// </summary>
		private void LogBudgetAdded(MonthlyBudget budget, bool isDefault)
		{
			var message = string.Format(
				isDefault ? DefaultBudgetAdded
						: BudgetAdded,
				budget.User?.UserName ?? budget.UserId.ToString(),
				budget.Month,
				budget.BudgetAmount);

			_logger.LogInformation(message);
		}

		/// <summary>
		/// Formats a unique identifier for a budget using its associated budget entity
		/// </summary>
		private string FormatBudgetIdentifier(MonthlyBudget budget)
		{
			return FormatBudgetIdentifier(budget.UserId, budget.Month);
		}

		/// <summary>
		/// Formats a unique identifier for a budget using individual components
		/// </summary>
		private string FormatBudgetIdentifier(int userId, Month month)
		{
			return string.Format(
				BudgetIdentifier,
				userId,
				month);
		}
	}
}