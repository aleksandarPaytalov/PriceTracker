namespace PriceTracker.Core.Models.Dashboard
{
	/// <summary>
	/// Monthly expense data for charts
	/// </summary>
	public class MonthlyExpenseData
	{
		public string Month { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public int MonthNumber { get; set; }
		public bool IsCurrentMonth { get; set; }
	}
}
