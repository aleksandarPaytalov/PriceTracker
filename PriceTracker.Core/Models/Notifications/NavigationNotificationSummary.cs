namespace PriceTracker.Core.Models.Notifications
{
	/// <summary>
	/// Complete navigation notification summary
	/// </summary>
	public class NavigationNotificationSummary
	{
		/// <summary>
		/// Task-related notifications
		/// </summary>
		public TaskNotificationData Tasks { get; set; } = new();

		/// <summary>
		/// Budget-related notifications
		/// </summary>
		public BudgetNotificationData Budget { get; set; } = new();

		/// <summary>
		/// Price tracking notifications
		/// </summary>
		public PriceNotificationData Prices { get; set; } = new();

		/// <summary>
		/// Gets all navigation badges for the current user
		/// </summary>
		public List<NavigationNotificationBadge> GetNavigationBadges()
		{
			var badges = new List<NavigationNotificationBadge>();

			// Task badge
			if (Tasks.TotalUrgentTasks > 0)
			{
				badges.Add(new NavigationNotificationBadge
				{
					BadgeId = "tasks",
					Count = Tasks.TotalUrgentTasks,
					BadgeColor = Tasks.GetBadgeColor(),
					TooltipText = Tasks.GetTooltipText(),
					IsVisible = true,
					ShouldPulse = Tasks.OverdueTasks > 0
				});
			}

			// Budget badge
			if (Budget.TotalBudgetWarnings > 0)
			{
				badges.Add(new NavigationNotificationBadge
				{
					BadgeId = "budget",
					Count = Budget.TotalBudgetWarnings,
					BadgeColor = Budget.GetBadgeColor(),
					TooltipText = Budget.GetTooltipText(),
					IsVisible = true,
					ShouldPulse = Budget.OverspentCategories > 0
				});
			}

			// Price tracking badge
			if (Prices.TotalPriceAlerts > 0)
			{
				badges.Add(new NavigationNotificationBadge
				{
					BadgeId = "prices",
					Count = Prices.TotalPriceAlerts,
					BadgeColor = Prices.GetBadgeColor(),
					TooltipText = Prices.GetTooltipText(),
					IsVisible = true,
					ShouldPulse = Prices.PriceDropAlerts > 0
				});
			}

			return badges;
		}

		/// <summary>
		/// Gets total notification count across all categories
		/// </summary>
		public int GetTotalNotificationCount()
		{
			return Tasks.TotalUrgentTasks + Budget.TotalBudgetWarnings + Prices.TotalPriceAlerts;
		}

		/// <summary>
		/// Checks if any critical notifications exist
		/// </summary>
		public bool HasCriticalNotifications()
		{
			return Tasks.OverdueTasks > 0 || Budget.OverspentCategories > 0;
		}
	}
}
