namespace PriceTracker.Core.Models.Notifications
{
	/// <summary>
	/// Price tracking notification data
	/// </summary>
	public class PriceNotificationData
	{
		/// <summary>
		/// Number of price drop alerts
		/// </summary>
		public int PriceDropAlerts { get; set; }

		/// <summary>
		/// Number of target price reached notifications
		/// </summary>
		public int TargetPriceReached { get; set; }

		/// <summary>
		/// Number of price increase warnings
		/// </summary>
		public int PriceIncreaseWarnings { get; set; }

		/// <summary>
		/// Number of back in stock notifications
		/// </summary>
		public int BackInStockNotifications { get; set; }

		/// <summary>
		/// Total price alerts requiring attention
		/// </summary>
		public int TotalPriceAlerts => PriceDropAlerts + TargetPriceReached + PriceIncreaseWarnings + BackInStockNotifications;

		/// <summary>
		/// Gets appropriate badge color based on alert type
		/// </summary>
		public string GetBadgeColor()
		{
			if (PriceDropAlerts > 0 || TargetPriceReached > 0) return "success";
			if (PriceIncreaseWarnings > 0) return "warning";
			if (BackInStockNotifications > 0) return "info";
			return "secondary";
		}

		/// <summary>
		/// Gets tooltip text for price notifications
		/// </summary>
		public string GetTooltipText()
		{
			var parts = new List<string>();

			if (PriceDropAlerts > 0) parts.Add($"{PriceDropAlerts} price drops");
			if (TargetPriceReached > 0) parts.Add($"{TargetPriceReached} target prices reached");
			if (PriceIncreaseWarnings > 0) parts.Add($"{PriceIncreaseWarnings} price increases");
			if (BackInStockNotifications > 0) parts.Add($"{BackInStockNotifications} back in stock");

			return parts.Any() ? string.Join(", ", parts) : "No price alerts";
		}
	}
}
