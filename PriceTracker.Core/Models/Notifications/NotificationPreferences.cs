namespace PriceTracker.Core.Models.Notifications
{
	/// <summary>
	/// User notification preferences
	/// </summary>
	public class NotificationPreferences
	{
		public string UserId { get; set; } = string.Empty;
		public bool EnableTaskNotifications { get; set; } = true;
		public bool EnableBudgetNotifications { get; set; } = true;
		public bool EnablePriceNotifications { get; set; } = true;
		public bool EnableEmailNotifications { get; set; } = false;
		public bool EnablePushNotifications { get; set; } = true;
		public int TaskReminderHours { get; set; } = 24;
		public int BudgetWarningPercentage { get; set; } = 80;
		public bool EnableCriticalAlertsOnly { get; set; } = false;
		public DateTime LastModified { get; set; } = DateTime.Now;
	}
}
