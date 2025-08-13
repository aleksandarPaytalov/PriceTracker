namespace PriceTracker.Core.Models.Dashboard
{
	public class BudgetOverviewViewModel
	{
		public decimal PlannedBudget { get; set; }
		public decimal TotalSpent { get; set; }
		public decimal RemainingAmount { get; set; }
		public int ProgressPercentage { get; set; }
		public string ProgressColor { get; set; } = "success"; // success, warning, danger
		public string CurrentMonth { get; set; } = string.Empty;
		public bool IsOverBudget { get; set; }
		public decimal DailyBudgetRemaining { get; set; }
	}
}
