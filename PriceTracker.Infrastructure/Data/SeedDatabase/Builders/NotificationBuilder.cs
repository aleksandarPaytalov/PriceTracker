using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;
using static PriceTracker.Infrastructure.Constants.DataConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Builder for creating validated Notification entities
	/// </summary>
	public class NotificationBuilder : IBuilder<Notification>
	{
		/// <summary>
		/// Creates a new notification with required data
		/// </summary>
		/// <param name="user">The user associated with the notification</param>
		/// <param name="task">The task associated with the notification</param>
		/// <param name="message">The notification message</param>
		/// <param name="time">Time when the notification should be shown</param>
		/// <param name="isRead">Indicates if the notification has been read</param>
		/// <param name="createdAt">Creation time of the notification</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		
		private readonly Notification _notification;

		public NotificationBuilder(
			User user,
			ToDoItem task,
			string message,
			DateTime time,
			bool isRead,
			DateTime createdAt
			)
		{
			try
			{
				ValidateNotificationInputs(user, task, message, time, isRead, createdAt);

				_notification = new Notification
				{
					User = user,
					UserId = user.Id,
					Task = task,
					TaskId = task.TaskId,
					Message = message,
					NotificationTime = time,
					IsRead = isRead,  
					CreatedAt = createdAt
				};

			}
			catch (InvalidCastException)
			{
				throw new ValidationException(NotificationConstants.InvalidIsReadValue);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create Notification: {ex.Message}");
			}
		}


		public Notification Build() => _notification;

		private void ValidateNotificationInputs(
			User user,
			ToDoItem task,
			string message,
			DateTime time,
			bool isRead,
			DateTime createdAt)
		{
			// User validations
			if (user == null)
			{
				throw new ValidationException(NotificationConstants.UserRequired);
			}

			if (user.Id == 0)
			{
				throw new ValidationException(NotificationConstants.UserIdRequired);
			}

			// Task validations
			if (task == null)
			{
				throw new ValidationException(NotificationConstants.TaskRequired);
			}

			if (task.TaskId == 0)
			{
				throw new ValidationException(NotificationConstants.TaskIdRequired);
			}

			// Message validations
			if (string.IsNullOrWhiteSpace(message))
			{
				throw new ValidationException(NotificationConstants.MessageRequired);
			}

			if (message.Length > notificationMessageMaxLength)
			{
				throw new ValidationException(
					string.Format(NotificationConstants.MessageTooLong, notificationMessageMaxLength));
			}

			// Time validations
			if (!IsValidDate(time))
			{
				throw new ValidationException(NotificationConstants.InvalidNotificationTimeFormat);
			}

			if (time == default)
			{
				throw new ValidationException(NotificationConstants.NotificationTimeMissing);
			}

			// CreatedAt validations
			if (!IsValidDate(createdAt))
			{
				throw new ValidationException(NotificationConstants.InvalidCreatedAtFormat);
			}

			if (createdAt == default)
			{
				throw new ValidationException(NotificationConstants.CreatedAtMissing);
			}

			if (createdAt > DateTime.Now)
			{
				throw new ValidationException(NotificationConstants.CreatedAtInFuture);
			}

			// Logical time order validation
			if (time < createdAt)
			{
				throw new ValidationException(NotificationConstants.InvalidTimeOrder);
			}
		}

		private bool IsValidDate(DateTime date)
		{
			return DateTime.TryParse(date.ToString(), out _);
		}
	}
}
