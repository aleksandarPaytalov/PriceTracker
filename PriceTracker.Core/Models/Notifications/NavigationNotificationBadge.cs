namespace PriceTracker.Core.Models.Notifications
{
	/// <summary>
	/// Navigation notification badge data
	/// </summary>
	public class NavigationNotificationBadge
	{
		/// <summary>
		/// Badge identifier (e.g., "tasks", "budget", "prices")
		/// </summary>
		public string BadgeId { get; set; } = string.Empty;

		/// <summary>
		/// Total count to display in badge
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Badge color based on priority (success, warning, danger)
		/// </summary>
		public string BadgeColor { get; set; } = "primary";

		/// <summary>
		/// Tooltip text for the badge
		/// </summary>
		public string TooltipText { get; set; } = string.Empty;

		/// <summary>
		/// Whether badge should be visible
		/// </summary>
		public bool IsVisible { get; set; }

		/// <summary>
		/// Whether badge should pulse/animate
		/// </summary>
		public bool ShouldPulse { get; set; }

		/// <summary>
		/// Maximum count to display before showing "99+"
		/// </summary>
		public int MaxDisplayCount { get; set; } = 99;

		/// <summary>
		/// Gets formatted count string for display
		/// </summary>
		public string DisplayCount => Count > MaxDisplayCount ? $"{MaxDisplayCount}+" : Count.ToString();
	}
}
