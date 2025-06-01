using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Constants.DataConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.NotificationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.BuilderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Enhanced NotificationBuilder with in-memory duplication tracking
	/// </summary>
	public class NotificationBuilder : IBuilder<Notification>
	{
		private readonly Notification _notification;
		private static readonly HashSet<string> _currentSeedNotifications = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new notification with enhanced validation including duplication tracking
		/// </summary>
		/// <param name="user">The user associated with the notification</param>
		/// <param name="task">The task associated with the notification</param>
		/// <param name="message">The notification message</param>
		/// <param name="time">Time when the notification should be shown</param>
		/// <param name="isRead">Indicates if the notification has been read</param>
		/// <param name="createdAt">Creation time of the notification</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public NotificationBuilder(
			User user,
			ToDoItem task,
			string message,
			DateTime time,
			bool isRead,
			DateTime createdAt)
		{
			try
			{
				ValidateNotificationInputs(user, task, message, time, createdAt, isRead);

				_notification = new Notification
				{
					UserId = user.Id,
					TaskId = task.TaskId,
					Message = message,
					NotificationTime = time,
					IsRead = isRead,
					CreatedAt = createdAt
				};

				// Track in current seed session to prevent duplicates
				var notificationKey = $"{user.Id}|{task.TaskId}|{time:yyyy-MM-dd HH:mm:ss}".ToLower();
				_currentSeedNotifications.Add(notificationKey);
			}
			catch (InvalidCastException)
			{
				throw new ValidationException(InvalidIsReadValue);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException(string.Format(FailedToCreateNotification, ex.Message));
			}
		}

		/// <summary>
		/// Builds and returns the validated Notification instance
		/// </summary>
		/// <returns>A validated Notification object</returns>
		public Notification Build() => _notification;

		/// <summary>
		/// Validates all notification input data with comprehensive checks
		/// </summary>
		private static void ValidateNotificationInputs(
			User user,
			ToDoItem task,
			string message,
			DateTime time,
			DateTime createdAt,
			bool isRead)
		{
			// User validations
			ValidateUser(user);

			// Task validations
			ValidateTask(task);

			// Message validations
			ValidateMessage(message);

			// Time validations
			ValidateTimeInputs(time, createdAt);

			// Business logic validations
			ValidateBusinessLogic(user, task, time, createdAt);

			// In-memory duplication check for current seed session
			ValidateNotificationUniqueness(user.Id, task.TaskId, time);
		}

		/// <summary>
		/// Validates user data
		/// </summary>
		private static void ValidateUser(User user)
		{
			if (user == null)
			{
				throw new ValidationException(UserRequired);
			}

			if (string.IsNullOrWhiteSpace(user.Id))
			{
				throw new ValidationException(UserIdRequired);
			}
		}

		/// <summary>
		/// Validates task data
		/// </summary>
		private static void ValidateTask(ToDoItem task)
		{
			if (task == null)
			{
				throw new ValidationException(TaskRequired);
			}

			if (task.TaskId <= 0)
			{
				throw new ValidationException(TaskIdRequired);
			}
		}

		/// <summary>
		/// Validates message content
		/// </summary>
		private static void ValidateMessage(string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				throw new ValidationException(MessageRequired);
			}

			if (message.Length < notificationMessageMinLength)
			{
				throw new ValidationException(string.Format(MessageTooShort, notificationMessageMinLength));
			}

			if (message.Length > notificationMessageMaxLength)
			{
				throw new ValidationException(string.Format(MessageTooLong, notificationMessageMaxLength));
			}

			// Security validation for message content
			if (ContainsForbiddenContent(message))
			{
				throw new ValidationException(MessageContainsForbiddenContent);
			}
		}

		/// <summary>
		/// Validates time inputs
		/// </summary>
		private static void ValidateTimeInputs(DateTime time, DateTime createdAt)
		{
			// Notification time validations
			if (!IsValidDate(time))
			{
				throw new ValidationException(InvalidNotificationTimeFormat);
			}

			if (time == default)
			{
				throw new ValidationException(NotificationTimeMissing);
			}

			// CreatedAt validations
			if (!IsValidDate(createdAt))
			{
				throw new ValidationException(InvalidCreatedAtFormat);
			}

			if (createdAt == default)
			{
				throw new ValidationException(CreatedAtMissing);
			}

			if (createdAt > DateTime.Now)
			{
				throw new ValidationException(CreatedAtInFuture);
			}

			// Logical time order validation
			if (time < createdAt)
			{
				throw new ValidationException(InvalidTimeOrder);
			}
		}

		/// <summary>
		/// Validates business logic rules
		/// </summary>
		private static void ValidateBusinessLogic(User user, ToDoItem task, DateTime time, DateTime createdAt)
		{
			// Ensure user owns the task
			if (task.UserId != user.Id)
			{
				throw new ValidationException(string.Format(TaskDoesNotBelongToUser, task.TaskId, user.Id));
			}

			// Validate notification timing makes sense
			if (task.DueDate.HasValue && time > task.DueDate.Value.AddDays(1))
			{
				throw new ValidationException(NotificationAfterTaskDue);
			}

			// Validate reasonable notification timing
			var timeDifference = time - createdAt;
			if (timeDifference.TotalDays > 365) // More than a year in future
			{
				throw new ValidationException(NotificationTooFarInFuture);
			}

			// Validate notification is not too old
			if (createdAt < DateTime.Now.AddYears(-1))
			{
				throw new ValidationException(NotificationTooOld);
			}
		}

		/// <summary>
		/// Validates notification uniqueness in current seeding session
		/// </summary>
		private static void ValidateNotificationUniqueness(string userId, int taskId, DateTime time)
		{
			var notificationKey = $"{userId}|{taskId}|{time:yyyy-MM-dd HH:mm:ss}".ToLower();

			if (_currentSeedNotifications.Contains(notificationKey))
			{
				throw new ValidationException(string.Format(DuplicateNotificationInSession, userId, taskId, time));
			}
		}

		/// <summary>
		/// Validates if date is in correct format
		/// </summary>
		private static bool IsValidDate(DateTime date)
		{
			return DateTime.TryParse(date.ToString(), out _);
		}

		/// <summary>
		/// Checks for forbidden content patterns in message
		/// </summary>
		private static bool ContainsForbiddenContent(string content)
		{
			if (string.IsNullOrEmpty(content)) return false;

			var forbiddenPatterns = new[]
			{
				"<script", "javascript:", "vbscript:", "onload=", "onerror=",
				"onclick=", "onmouseover=", "alert(", "eval(", "document.cookie",
				"<iframe", "<object", "<embed", "data:text/html", "data:text/javascript",
				"<", ">", "src=", "href=", "style=", "expression(", "url(",
				"select ", "insert ", "update ", "delete ", "drop ", "union ",
				"exec ", "execute ", "--", "/*", "*/", "@@", "char", "nchar",
				"varchar", "nvarchar", "table", "database", "sysobjects", "syscolumns"
			};

			return forbiddenPatterns.Any(pattern =>
				content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Clear tracking collections for new seeding session
		/// Call this before starting a new migration or seeding operation
		/// </summary>
		public static void ResetTracking()
		{
			_currentSeedNotifications.Clear();
		}

		/// <summary>
		/// Get count of currently tracked notifications in this session
		/// </summary>
		public static int GetTrackedNotificationCount()
		{
			return _currentSeedNotifications.Count;
		}

		/// <summary>
		/// Check if a notification combination is already tracked in current session
		/// </summary>
		public static bool IsNotificationTracked(string userId, int taskId, DateTime time)
		{
			var notificationKey = $"{userId}|{taskId}|{time:yyyy-MM-dd HH:mm:ss}".ToLower();
			return _currentSeedNotifications.Contains(notificationKey);
		}

		/// <summary>
		/// Get all tracked notification keys in current session
		/// </summary>
		public static IEnumerable<string> GetTrackedNotificationKeys()
		{
			return _currentSeedNotifications.AsEnumerable();
		}
	}
}