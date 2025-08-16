namespace PriceTracker.Core.Models.Notifications
{
	// <summary>
	/// Task-related notification data
	/// </summary>
	public class TaskNotificationData
	{
		/// <summary>
		/// Number of overdue tasks
		/// </summary>
		public int OverdueTasks { get; set; }

		/// <summary>
		/// Number of tasks due today
		/// </summary>
		public int TasksDueToday { get; set; }

		/// <summary>
		/// Number of tasks due this week
		/// </summary>
		public int TasksDueThisWeek { get; set; }

		/// <summary>
		/// Number of high priority tasks
		/// </summary>
		public int HighPriorityTasks { get; set; }

		/// <summary>
		/// Total urgent tasks requiring attention
		/// </summary>
		public int TotalUrgentTasks => OverdueTasks + TasksDueToday + HighPriorityTasks;

		/// <summary>
		/// Gets appropriate badge color based on urgency
		/// </summary>
		public string GetBadgeColor()
		{
			if (OverdueTasks > 0) return "danger";
			if (TasksDueToday > 0) return "warning";
			if (HighPriorityTasks > 0) return "info";
			return "secondary";
		}

		/// <summary>
		/// Gets tooltip text for task notifications
		/// </summary>
		public string GetTooltipText()
		{
			var parts = new List<string>();

			if (OverdueTasks > 0) parts.Add($"{OverdueTasks} overdue");
			if (TasksDueToday > 0) parts.Add($"{TasksDueToday} due today");
			if (HighPriorityTasks > 0) parts.Add($"{HighPriorityTasks} high priority");

			return parts.Any() ? string.Join(", ", parts) : "No urgent tasks";
		}
	}
}
