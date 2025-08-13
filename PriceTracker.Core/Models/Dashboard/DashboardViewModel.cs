namespace PriceTracker.Core.Models.Dashboard
{
	public class DashboardViewModel
	{
		/// <summary>
		/// Personalized welcome message for the user
		/// </summary>
		public string WelcomeMessage { get; set; } = string.Empty;

		/// <summary>
		/// Current date and time for display
		/// </summary>
		public DateTime CurrentDate { get; set; }

		/// <summary>
		/// User identifier for the current dashboard
		/// </summary>
		public string UserId { get; set; } = string.Empty;

		/// <summary>
		/// Display name for the current user
		/// </summary>
		public string UserDisplayName { get; set; } = string.Empty;

		/// <summary>
		/// Indicates if user has budget data to display
		/// </summary>
		public bool HasBudgetData { get; set; }

		/// <summary>
		/// Indicates if user has expense data to display
		/// </summary>
		public bool HasExpenseData { get; set; }

		/// <summary>
		/// Indicates if user has task data to display  
		/// </summary>
		public bool HasTaskData { get; set; }

		/// <summary>
		/// Indicates if user has price tracking data to display
		/// </summary>
		public bool HasPriceData { get; set; }

		/// <summary>
		/// Indicates if there was an error loading dashboard data
		/// </summary>
		public bool HasError { get; set; }

		/// <summary>
		/// Error message to display if HasError is true
		/// </summary>
		public string? ErrorMessage { get; set; }

		/// <summary>
		/// Budget overview data - will be implemented in Phase 2 Step 3
		/// </summary>
		public BudgetOverviewViewModel? BudgetOverview { get; set; }

		/// <summary>
		/// Yearly expense summary data - will be implemented in Phase 2 Step 4
		/// </summary>
		public YearlyExpenseSummaryViewModel? YearlyExpenses { get; set; }

		/// <summary>
		/// Top 5 most purchased products - will be implemented in Phase 2 Step 5
		/// </summary>
		public List<TopProductViewModel>? TopProducts { get; set; }

		/// <summary>
		/// Task overview data - will be implemented in Phase 2 Step 6
		/// </summary>
		public TaskOverviewViewModel? TaskOverview { get; set; }

		/// <summary>
		/// Timestamp when the dashboard data was last loaded
		/// </summary>
		public DateTime LastUpdated { get; set; }

		/// <summary>
		/// Indicates if the dashboard is currently loading data
		/// </summary>
		public bool IsLoading { get; set; }
	}
}
