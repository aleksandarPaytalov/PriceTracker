namespace PriceTracker.Core.Models.Dashboard
{
	/// <summary>
	/// Dashboard configuration and user preferences
	/// </summary>
	public class DashboardConfigurationViewModel
	{
		public string UserId { get; set; } = string.Empty;
		public bool ShowBudgetWidget { get; set; } = true;
		public bool ShowExpenseWidget { get; set; } = true;
		public bool ShowTaskWidget { get; set; } = true;
		public bool ShowProductWidget { get; set; } = true;
		public string DefaultTimeRange { get; set; } = "current_month";
		public string PreferredCurrency { get; set; } = "USD";
		public bool EnableNotifications { get; set; } = true;
		public bool EnableEmailAlerts { get; set; } = false;
		public int RefreshIntervalMinutes { get; set; } = 15;
		public DateTime LastModified { get; set; }
	}
}
