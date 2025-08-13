using PriceTracker.Core.Models.Dashboard;

namespace PriceTracker.Core.Contracts
{
	/// <summary>
	/// Service interface for dashboard operations
	/// Provides abstraction for dashboard data retrieval and business logic
	/// </summary>
	public interface IDashboardService
	{
		/// <summary>
		/// Gets complete dashboard data for the specified user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <param name="userName">User's display name or email</param>
		/// <returns>Complete dashboard view model with all widget data</returns>
		Task<DashboardViewModel> GetDashboardDataAsync(string userId, string userName);

		/// <summary>
		/// Generates personalized welcome message based on time of day and user info
		/// </summary>
		/// <param name="userName">User's display name or email</param>
		/// <returns>Contextual greeting message</returns>
		string GenerateWelcomeMessage(string userName);

		/// <summary>
		/// Gets budget overview data for the current month
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>Budget overview data or null if no budget data exists</returns>
		Task<BudgetOverviewViewModel?> GetBudgetOverviewAsync(string userId);

		/// <summary>
		/// Gets yearly expense summary data for charts and analysis
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <param name="year">Year to retrieve data for (defaults to current year)</param>
		/// <returns>Yearly expense summary data or null if no expense data exists</returns>
		Task<YearlyExpenseSummaryViewModel?> GetYearlyExpenseSummaryAsync(string userId, int? year = null);

		/// <summary>
		/// Gets top 5 most purchased products for the user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <param name="topCount">Number of top products to retrieve (default 5)</param>
		/// <returns>List of top products or empty list if no purchase data exists</returns>
		Task<List<TopProductViewModel>> GetTopProductsAsync(string userId, int topCount = 5);

		/// <summary>
		/// Gets task overview data including counts and upcoming tasks
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>Task overview data or null if no task data exists</returns>
		Task<TaskOverviewViewModel?> GetTaskOverviewAsync(string userId);

		/// <summary>
		/// Refreshes specific dashboard widget data for AJAX updates
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <param name="widgetType">Type of widget to refresh (budget, expenses, tasks, products)</param>
		/// <returns>Refresh data result with success status and updated data</returns>
		Task<DashboardRefreshData> RefreshWidgetDataAsync(string userId, string widgetType);

		/// <summary>
		/// Checks if user has any data in the system for dashboard display
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>Data availability status for different dashboard sections</returns>
		Task<(bool hasBudget, bool hasExpenses, bool hasTasks, bool hasProducts)> CheckDataAvailabilityAsync(string userId);

		/// <summary>
		/// Gets dashboard configuration settings for the user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>User's dashboard preferences and settings</returns>
		Task<DashboardConfigurationViewModel?> GetDashboardConfigurationAsync(string userId);

		/// <summary>
		/// Updates dashboard configuration settings for the user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <param name="configuration">New configuration settings</param>
		/// <returns>Success status of the update operation</returns>
		Task<bool> UpdateDashboardConfigurationAsync(string userId, DashboardConfigurationViewModel configuration);
	}

}