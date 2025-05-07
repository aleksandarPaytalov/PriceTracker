using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Constants.DataConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// TodoItem builder class used for data seeding and validation before data being imported in database 
	/// </summary>
	public class ToDoItemBuilder : IBuilder<ToDoItem>
	{
		private readonly ToDoItem _task;

		/// <summary>
		/// Creates a new task with required data
		/// </summary>
		/// <param name="user">The user who owns the task</param>
		/// <param name="title">Task title</param>
		/// <param name="description">Task description (optional)</param>
		/// <param name="dueDate">Task due date (optional)</param>
		/// <param name="priority">Task priority</param>
		/// <param name="status">Task status</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public ToDoItemBuilder(
			User user,
			string title,
			string? description = null,
			DateTime? dueDate = null,
			TaskPriority priority = TaskPriority.Low,
			Models.TaskStatus status = Models.TaskStatus.Pending)
		{
			try
			{
				ValidateTaskInputData(user, title, description, dueDate);

				_task = new ToDoItem
				{
					User = user,
					UserId = user.Id,
					Title = title,
					Description = description,
					DueDate = dueDate,
					Priority = priority,
					TaskStatus = status,
					CreatedAt = DateTime.UtcNow,
					Notifications = new List<Notification>()
				};
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create task: {ex.Message}");
			}
		}

		public ToDoItem Build() => _task;

		private void ValidateTaskInputData(
			User user,
			string title,
			string? description,
			DateTime? dueDate)
		{
			// User validations
			if (user == null)
			{
				throw new ValidationException(TaskConstants.UserRequired);
			}

			if (user.Id == 0)
			{
				throw new ValidationException(TaskConstants.UserIdRequired);
			}

			// Title validations
			if (string.IsNullOrWhiteSpace(title))
			{
				throw new ValidationException(TaskConstants.TitleRequired);
			}

			if (title.Length < taskTitleMinLength || title.Length > taskTitleMaxLength)
			{
				throw new ValidationException(
					string.Format(TaskConstants.InvalidTitleLength,
						taskTitleMinLength,
						taskTitleMaxLength));
			}

			// Description validation (if provided)
			if (description?.Length > taskDescriptionMaxLength)
			{
				throw new ValidationException(
					string.Format(TaskConstants.InvalidDescriptionLength,
						taskDescriptionMaxLength));
			}

			// Due date validation (if provided)
			if (dueDate.HasValue)
			{
				if (!IsValidDate(dueDate.Value))
				{
					throw new ValidationException(TaskConstants.InvalidDueDate);
				}

				if (dueDate.Value < DateTime.Now)
				{
					throw new ValidationException(TaskConstants.DueDateInPast);
				}
			}
		}

		private bool IsValidDate(DateTime date)
		{
			return DateTime.TryParse(date.ToString(), out _);
		}
	}
}