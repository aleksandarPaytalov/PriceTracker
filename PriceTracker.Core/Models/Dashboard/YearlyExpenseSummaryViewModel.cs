namespace PriceTracker.Core.Models.Dashboard
{
	public class YearlyExpenseSummaryViewModel
	{
		public decimal TotalYearlyExpenses { get; set; }
		public int CurrentYear { get; set; }
		public List<MonthlyExpenseData> MonthlyData { get; set; } = new();
		public List<CategoryExpenseData> TopCategories { get; set; } = new();
		public decimal AverageMonthlySpending { get; set; }
		public string HighestSpendingMonth { get; set; } = string.Empty;
	}
}
