namespace PriceTracker.Core.Models.Dashboard
{
	/// <summary>
	/// Dashboard refresh data for AJAX updates
	/// </summary>
	public class DashboardRefreshData
	{
		public bool Success { get; set; }
		public DateTime Timestamp { get; set; }
		public string? ErrorMessage { get; set; }
		public DashboardWidgetData? Widgets { get; set; }
	}
}
