using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.NotificationDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	/// <summary>
	/// Provider responsible for loading and managing notification data.
	/// Handles both default and external notification sources.
	/// </summary>
	public class NotificationDataProvider : BaseDataProvider<Notification>
	{
		private readonly IRepository<User> _userRepository;
		private readonly IRepository<ToDoItem> _taskRepository;

		public NotificationDataProvider(
			IRepository<Notification> repository,
			IRepository<User> userRepository,
			IRepository<ToDoItem> taskRepository,
			IDataSource<Notification>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
			_userRepository = userRepository;
			_taskRepository = taskRepository;
		}

		/// <summary>
		/// Retrieves all notification data from external source
		/// </summary>
		/// <returns>Collection of notification entities</returns>
		public override IEnumerable<Notification> GetData()
		{
			var notifications = new List<Notification>();

			try
			{
				notifications.AddRange(LoadNotificationsFromExternalSource());

				_logger.LogInformation(
					string.Format(FinishedLoadingData,
						_typeName,
						notifications.Count));
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(GetData), ex);
			}

			return notifications;
		}

		/// <summary>
		/// Loads notifications from external data source
		/// </summary>
		/// <returns>Collection of notifications from external source</returns>
		private IEnumerable<Notification> LoadNotificationsFromExternalSource()
		{
			var notifications = new List<Notification>();

			try
			{
				_logger.LogInformation(StartingExternalSource);

				var sourceNotifications = LoadFromSourceAsync().Result;
				var (users, tasks) = LoadRelatedData();

				foreach (var notificationData in sourceNotifications)
				{
					try
					{
						if (!NotificationExists(notificationData))
						{
							var notification = CreateNotification(notificationData, users, tasks);
							if (notification != null)
							{
								notifications.Add(notification);
								LogNotificationAdded(notification);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatNotificationIdentifier(notificationData);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadNotificationsFromExternalSource), ex);
			}

			return notifications;
		}

		/// <summary>
		/// Loads all related data needed for notification creation
		/// </summary>
		/// <returns>Tuple containing lists of users and tasks</returns>
		private (List<User> users, List<ToDoItem> tasks) LoadRelatedData()
		{
			_logger.LogInformation(LoadingRelatedData);

			var users = _userRepository.AllReadOnly().ToList();
			var tasks = _taskRepository.AllReadOnly().ToList();

			return (users, tasks);
		}

		/// <summary>
		/// Creates a new notification using the NotificationBuilder
		/// </summary>
		private Notification? CreateNotification(
			Notification notificationData,
			List<User> users,
			List<ToDoItem> tasks)
		{
			try
			{
				var user = users.FirstOrDefault(u => u.Id == notificationData.UserId);
				var task = tasks.FirstOrDefault(t => t.TaskId == notificationData.TaskId);

				if (user == null || task == null) return null;

				return new NotificationBuilder(
					user: user,
					task: task,
					message: notificationData.Message,
					time: notificationData.NotificationTime,
					isRead: notificationData.IsRead,
					createdAt: notificationData.CreatedAt)
					.Build();
			}
			catch (Exception ex)
			{
				var identifier = FormatNotificationIdentifier(notificationData);
				LogProcessingError(identifier, ex);
				return null;
			}
		}

		/// <summary>
		/// Checks if a notification already exists in the database
		/// </summary>
		private bool NotificationExists(Notification notification)
		{
			return EntityExists(n =>
				n.UserId == notification.UserId &&
				n.TaskId == notification.TaskId &&
				n.NotificationTime == notification.NotificationTime);
		}

		/// <summary>
		/// Logs the addition of a new notification
		/// </summary>
		private void LogNotificationAdded(Notification notification)
		{
			var message = string.Format(
				NotificationAdded,
				notification.User?.UserName ?? notification.UserId.ToString(),
				notification.Task?.Title ?? notification.TaskId.ToString());

			_logger.LogInformation(message);
		}

		/// <summary>
		/// Creates a formatted identifier for a notification entity
		/// </summary>
		private static string FormatNotificationIdentifier(Notification notification)
		{
			return FormatNotificationIdentifier(
				notification.UserId,
				notification.TaskId,
				notification.NotificationTime);
		}

		/// <summary>
		/// Creates a formatted identifier using individual notification components
		/// </summary>
		private static string FormatNotificationIdentifier(string userId, int taskId, DateTime time)
		{
			return string.Format(
				NotificationIdentifier,
				userId,
				taskId,
				time);
		}
	}
}