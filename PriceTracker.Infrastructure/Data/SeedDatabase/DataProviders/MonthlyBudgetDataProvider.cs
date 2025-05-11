using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.MonthlyBudgetDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
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

		private List<User> LoadUsers()
		{
			_logger.LogInformation(LoadingRelatedData);
			return _userRepository.AllReadOnly().ToList();
		}

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

		private bool BudgetExists(MonthlyBudget budget)
		{
			return EntityExists(b =>
				b.UserId == budget.UserId &&
				b.Month == budget.Month);
		}

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

		private string FormatBudgetIdentifier(MonthlyBudget budget)
		{
			return FormatBudgetIdentifier(budget.UserId, budget.Month);
		}

		private string FormatBudgetIdentifier(int userId, Month month)
		{
			return string.Format(
				BudgetIdentifier,
				userId,
				month);
		}
	}
}