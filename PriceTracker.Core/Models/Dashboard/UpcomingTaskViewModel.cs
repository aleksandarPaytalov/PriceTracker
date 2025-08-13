namespace PriceTracker.Core.Models.Dashboard
{
	/// <summary>
	/// Individual task data for the task overview widget
	/// </summary>
	public class UpcomingTaskViewModel
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public DateTime DueDate { get; set; }
		public string Priority { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public bool IsOverdue { get; set; }
		public int DaysUntilDue { get; set; }
		public string Description { get; set; } = string.Empty;
		public string PriorityColor { get; set; } = string.Empty;
	}
}
