namespace PriceTracker.Core.Models.Dashboard
{
	/// <summary>
	/// Category expense data for charts
	/// </summary>
	public class CategoryExpenseData
	{
		public string Category { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public decimal Percentage { get; set; }
		public string Color { get; set; } = string.Empty;
		public int ItemCount { get; set; }
	}
}
