namespace PriceTracker.Core.Models.Dashboard
{
	/// <summary>
	/// Task overview widget data - placeholder for Phase 2 implementation
	/// </summary>
	public class TaskOverviewViewModel
	{
		public int TasksDueThisWeek { get; set; }
		public int TasksDueNext3Days { get; set; }
		public int OpenTasks { get; set; }
		public int InProgressTasks { get; set; }
		public int CompletedTasks { get; set; }
		public int OverdueTasks { get; set; }
		public List<UpcomingTaskViewModel> UpcomingTasks { get; set; } = new();
		public double CompletionRate { get; set; }
	}
}
