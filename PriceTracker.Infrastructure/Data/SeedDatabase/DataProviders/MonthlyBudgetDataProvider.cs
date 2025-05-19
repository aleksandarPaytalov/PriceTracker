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

		public MonthlyBudgetDataProvider(
			IRepository<MonthlyBudget> repository,
			IRepository<User> userRepository,
			IDataSource<MonthlyBudget>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
			_userRepository = userRepository;
		}

		/// <summary>
		/// Main method to retrieve budget data
		/// Returns collection of monthly budgets from external source
		/// </summary>
		public override IEnumerable<MonthlyBudget> GetData()
		{
			var budgets = new List<MonthlyBudget>();

			try
			{
				budgets.AddRange(LoadBudgetsFromExternalSource());

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
								LogBudgetAdded(budget);
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
		private void LogBudgetAdded(MonthlyBudget budget)
		{
			var message = string.Format(
				BudgetAdded,
				budget.User?.UserName ?? budget.UserId.ToString(),
				budget.Month,
				budget.BudgetAmount);

			_logger.LogInformation(message);
		}

		/// <summary>
		/// Formats a unique identifier for a budget using its associated budget entity
		/// </summary>
		private static string FormatBudgetIdentifier(MonthlyBudget budget)
		{
			return FormatBudgetIdentifier(budget.UserId, budget.Month);
		}

		/// <summary>
		/// Formats a unique identifier for a budget using individual components
		/// </summary>
		private static string FormatBudgetIdentifier(string userId, Month month)
		{
			return string.Format(
				BudgetIdentifier,
				userId,
				month);
		}
	}
}