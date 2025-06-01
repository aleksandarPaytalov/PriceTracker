using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Constants.DataConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.TaskConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.BuilderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Enhanced ToDoItemBuilder with in-memory duplication tracking
	/// </summary>
	public class ToDoItemBuilder : IBuilder<ToDoItem>
	{
		private readonly ToDoItem _task;
		private static readonly HashSet<string> _currentSeedTasks = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new task with enhanced validation including duplication tracking
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
				ValidateTaskInputData(user, title, description, dueDate, priority, status);

				_task = new ToDoItem
				{
					UserId = user.Id,
					Title = title,
					Description = description,
					DueDate = dueDate,
					Priority = priority,
					TaskStatus = status,
					CreatedAt = DateTime.UtcNow,
					Notifications = new List<Notification>()
				};

				// Track in current seed session to prevent duplicates
				var taskKey = $"{user.Id}|{title}|{dueDate?.ToString("yyyy-MM-dd") ?? "no-due"}".ToLower();
				_currentSeedTasks.Add(taskKey);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException(string.Format(FailedToCreateTask, ex.Message));
			}
		}

		/// <summary>
		/// Builds and returns the validated ToDoItem instance
		/// </summary>
		/// <returns>A validated ToDoItem object</returns>
		public ToDoItem Build() => _task;

		/// <summary>
		/// Validates task input data with comprehensive checks
		/// </summary>
		private static void ValidateTaskInputData(
			User user,
			string title,
			string? description,
			DateTime? dueDate,
			TaskPriority priority,
			Models.TaskStatus status)
		{
			// User validations
			ValidateUser(user);

			// Title validations
			ValidateTitle(title);

			// Description validation (if provided)
			ValidateDescription(description);

			// Due date validation (if provided)
			ValidateDueDate(dueDate);

			// Enum validations
			ValidateEnums(priority, status);

			// Business logic validations
			ValidateBusinessLogic(status, dueDate);

			// In-memory duplication check for current seed session
			ValidateTaskUniqueness(user.Id, title, dueDate);
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
		/// Validates task title
		/// </summary>
		private static void ValidateTitle(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
			{
				throw new ValidationException(TitleRequired);
			}

			if (title.Length < taskTitleMinLength || title.Length > taskTitleMaxLength)
			{
				throw new ValidationException(string.Format(InvalidTitleLength, taskTitleMinLength, taskTitleMaxLength));
			}

			// Security validation for title
			if (ContainsForbiddenContent(title))
			{
				throw new ValidationException(TitleContainsForbiddenContent);
			}
		}

		/// <summary>
		/// Validates task description
		/// </summary>
		private static void ValidateDescription(string? description)
		{
			if (description != null)
			{
				if (description.Length < taskDescriptionMinLength)
				{
					throw new ValidationException(string.Format(DescriptionTooShort, taskDescriptionMinLength));
				}

				if (description.Length > taskDescriptionMaxLength)
				{
					throw new ValidationException(string.Format(InvalidDescriptionLength, taskDescriptionMaxLength));
				}

				// Security validation for description
				if (ContainsForbiddenContent(description))
				{
					throw new ValidationException(DescriptionContainsForbiddenContent);
				}
			}
		}

		/// <summary>
		/// Validates due date
		/// </summary>
		private static void ValidateDueDate(DateTime? dueDate)
		{
			if (dueDate.HasValue)
			{
				if (!IsValidDate(dueDate.Value))
				{
					throw new ValidationException(InvalidDueDate);
				}

				if (dueDate.Value < DateTime.Now.Date)
				{
					throw new ValidationException(DueDateInPast);
				}

				// Business rule: due date shouldn't be too far in future
				if (dueDate.Value > DateTime.Now.AddYears(2))
				{
					throw new ValidationException(DueDateTooFarInFuture);
				}
			}
		}

		/// <summary>
		/// Validates enum values
		/// </summary>
		private static void ValidateEnums(TaskPriority priority, Models.TaskStatus status)
		{
			if (!Enum.IsDefined(typeof(TaskPriority), priority))
			{
				throw new ValidationException(string.Format(InvalidPriority, priority));
			}

			if (!Enum.IsDefined(typeof(Models.TaskStatus), status))
			{
				throw new ValidationException(string.Format(InvalidTaskStatus, status));
			}
		}

		/// <summary>
		/// Validates business logic rules
		/// </summary>
		private static void ValidateBusinessLogic(Models.TaskStatus status, DateTime? dueDate)
		{
			// If task is completed but due date is in the future, it might be suspicious
			if (status == Models.TaskStatus.Completed && dueDate.HasValue && dueDate.Value > DateTime.Now)
			{
				// This could be a warning rather than error - tasks can be completed early
				// throw new ValidationException(CompletedTaskWithFutureDueDate);
			}

			// If task is pending but due date is far in the past, it might be problematic
			if (status == Models.TaskStatus.Pending && dueDate.HasValue && dueDate.Value < DateTime.Now.AddDays(-30))
			{
				// This could be a warning - old overdue tasks
				// throw new ValidationException(PendingTaskLongOverdue);
			}
		}

		/// <summary>
		/// Validates task uniqueness in current seeding session
		/// </summary>
		private static void ValidateTaskUniqueness(string userId, string title, DateTime? dueDate)
		{
			var taskKey = $"{userId}|{title}|{dueDate?.ToString("yyyy-MM-dd") ?? "no-due"}".ToLower();

			if (_currentSeedTasks.Contains(taskKey))
			{
				throw new ValidationException(string.Format(DuplicateTaskInSession, userId, title, dueDate?.ToString("yyyy-MM-dd") ?? "no due date"));
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
		/// Checks for forbidden content patterns
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
			_currentSeedTasks.Clear();
		}

		/// <summary>
		/// Get count of currently tracked tasks in this session
		/// </summary>
		public static int GetTrackedTaskCount()
		{
			return _currentSeedTasks.Count;
		}

		/// <summary>
		/// Check if a task combination is already tracked in current session
		/// </summary>
		public static bool IsTaskTracked(string userId, string title, DateTime? dueDate)
		{
			var taskKey = $"{userId}|{title}|{dueDate?.ToString("yyyy-MM-dd") ?? "no-due"}".ToLower();
			return _currentSeedTasks.Contains(taskKey);
		}

		/// <summary>
		/// Get all tracked task keys in current session
		/// </summary>
		public static IEnumerable<string> GetTrackedTaskKeys()
		{
			return _currentSeedTasks.AsEnumerable();
		}
	}
}