using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.NotificationDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
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

		public override IEnumerable<Notification> GetData()
		{
			var notifications = new List<Notification>();

			try
			{
				if (_dataSource != null)
				{
					notifications.AddRange(LoadNotificationsFromExternalSource());
				}
				else
				{
					notifications.AddRange(LoadDefaultNotifications());
				}

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
								LogNotificationAdded(notification, isDefault: false);
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

		private IEnumerable<Notification> LoadDefaultNotifications()
		{
			var notifications = new List<Notification>();

			try
			{
				_logger.LogInformation(LoadingDefaultData);

				var (users, tasks) = LoadRelatedData();

				// Making one notification for eaach task
				foreach (var task in tasks)
				{
					try
					{
						var defaultNotification = GenerateDefaultNotification(task);
						if (!NotificationExists(defaultNotification))
						{
							var notification = CreateNotification(defaultNotification, users, tasks);
							if (notification != null)
							{
								notifications.Add(notification);
								LogNotificationAdded(notification, isDefault: true);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatNotificationIdentifier(
							task.UserId,
							task.TaskId,
							DateTime.Now);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadDefaultNotifications), ex);
			}

			return notifications;
		}

		private (List<User> users, List<ToDoItem> tasks) LoadRelatedData()
		{
			_logger.LogInformation(LoadingRelatedData);

			var users = _userRepository.AllReadOnly().ToList();
			var tasks = _taskRepository.AllReadOnly().ToList();

			return (users, tasks);
		}

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

		private Notification GenerateDefaultNotification(ToDoItem task)
		{
			return new Notification
			{
				UserId = task.UserId,
				User = task.User,
				TaskId = task.TaskId,
				Task = task,
				Message = $"Reminder: {task.Title} is due soon",
				NotificationTime = task.DueDate?.AddHours(-24) ?? DateTime.Now.AddDays(1),
				IsRead = false,
				CreatedAt = DateTime.UtcNow
			};
		}

		private bool NotificationExists(Notification notification)
		{
			return EntityExists(n =>
				n.UserId == notification.UserId &&
				n.TaskId == notification.TaskId &&
				n.NotificationTime == notification.NotificationTime);
		}

		private void LogNotificationAdded(Notification notification, bool isDefault)
		{
			var message = string.Format(
				isDefault ? DefaultNotificationAdded
						: NotificationAdded,
				notification.User?.UserName ?? notification.UserId.ToString(),
				notification.Task?.Title ?? notification.TaskId.ToString());

			_logger.LogInformation(message);
		}

		private string FormatNotificationIdentifier(Notification notification)
		{
			return FormatNotificationIdentifier(
				notification.UserId,
				notification.TaskId,
				notification.NotificationTime);
		}

		private string FormatNotificationIdentifier(int userId, int taskId, DateTime time)
		{
			return string.Format(
				NotificationIdentifier,
				userId,
				taskId,
				time);
		}
	}
}