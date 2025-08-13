namespace PriceTracker.Core.Models.Dashboard
{
	/// <summary>
	/// Individual widget data for refresh operations
	/// </summary>
	public class DashboardWidgetData
	{
		public object? Budget { get; set; }
		public object? Expenses { get; set; }
		public object? Tasks { get; set; }
		public object? Products { get; set; }
	}
}
