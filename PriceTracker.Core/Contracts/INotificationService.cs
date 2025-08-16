using PriceTracker.Core.Models.Notifications;

namespace PriceTracker.Core.Contracts
{
	/// <summary>
	/// Service interface for managing notifications and badges
	/// </summary>
	public interface INotificationService
	{
		/// <summary>
		/// Gets complete navigation notification summary for a user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>Navigation notification summary with all badge data</returns>
		Task<NavigationNotificationSummary> GetNavigationNotificationSummaryAsync(string userId);

		/// <summary>
		/// Gets task-related notification data for a user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>Task notification data</returns>
		Task<TaskNotificationData> GetTaskNotificationDataAsync(string userId);

		/// <summary>
		/// Gets budget-related notification data for a user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>Budget notification data</returns>
		Task<BudgetNotificationData> GetBudgetNotificationDataAsync(string userId);

		/// <summary>
		/// Gets price tracking notification data for a user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>Price notification data</returns>
		Task<PriceNotificationData> GetPriceNotificationDataAsync(string userId);

		/// <summary>
		/// Gets navigation badges for the specified user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>List of navigation badges to display</returns>
		Task<List<NavigationNotificationBadge>> GetNavigationBadgesAsync(string userId);

		/// <summary>
		/// Gets badge data for a specific navigation section
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <param name="badgeId">Badge identifier (tasks, budget, prices)</param>
		/// <returns>Navigation badge data or null if no notifications</returns>
		Task<NavigationNotificationBadge?> GetNavigationBadgeAsync(string userId, string badgeId);

		/// <summary>
		/// Checks if user has any critical notifications requiring immediate attention
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>True if critical notifications exist</returns>
		Task<bool> HasCriticalNotificationsAsync(string userId);

		/// <summary>
		/// Gets total notification count for a user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>Total number of notifications</returns>
		Task<int> GetTotalNotificationCountAsync(string userId);

		/// <summary>
		/// Marks notifications as read for a specific category
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <param name="notificationType">Type of notifications to mark as read</param>
		/// <returns>Success status</returns>
		Task<bool> MarkNotificationsAsReadAsync(string userId, NotificationType notificationType);

		/// <summary>
		/// Updates notification preferences for a user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <param name="preferences">Notification preferences</param>
		/// <returns>Success status</returns>
		Task<bool> UpdateNotificationPreferencesAsync(string userId, NotificationPreferences preferences);

		/// <summary>
		/// Gets notification preferences for a user
		/// </summary>
		/// <param name="userId">User identifier</param>
		/// <returns>User's notification preferences</returns>
		Task<NotificationPreferences> GetNotificationPreferencesAsync(string userId);
	}
}