namespace PriceTracker.Core.Models.Notifications
{
	/// <summary>
	/// Budget-related notification data
	/// </summary>
	public class BudgetNotificationData
	{
		/// <summary>
		/// Number of budget categories overspent
		/// </summary>
		public int OverspentCategories { get; set; }

		/// <summary>
		/// Number of categories approaching limit (80%+)
		/// </summary>
		public int ApproachingLimitCategories { get; set; }

		/// <summary>
		/// Number of unusual spending alerts
		/// </summary>
		public int UnusualSpendingAlerts { get; set; }

		/// <summary>
		/// Number of pending budget reviews
		/// </summary>
		public int PendingBudgetReviews { get; set; }

		/// <summary>
		/// Total budget warnings requiring attention
		/// </summary>
		public int TotalBudgetWarnings => OverspentCategories + ApproachingLimitCategories + UnusualSpendingAlerts;

		/// <summary>
		/// Gets appropriate badge color based on severity
		/// </summary>
		public string GetBadgeColor()
		{
			if (OverspentCategories > 0) return "danger";
			if (ApproachingLimitCategories > 0) return "warning";
			if (UnusualSpendingAlerts > 0) return "info";
			return "success";
		}

		/// <summary>
		/// Gets tooltip text for budget notifications
		/// </summary>
		public string GetTooltipText()
		{
			var parts = new List<string>();

			if (OverspentCategories > 0) parts.Add($"{OverspentCategories} overspent");
			if (ApproachingLimitCategories > 0) parts.Add($"{ApproachingLimitCategories} near limit");
			if (UnusualSpendingAlerts > 0) parts.Add($"{UnusualSpendingAlerts} spending alerts");

			return parts.Any() ? string.Join(", ", parts) : "Budget on track";
		}
	}
}
