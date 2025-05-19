using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.ToDoItemDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	/// <summary>
	/// Provider responsible for loading and managing todo items data
	/// </summary>
	public class ToDoItemDataProvider : BaseDataProvider<ToDoItem>
	{
		private readonly IRepository<User> _userRepository;
		public ToDoItemDataProvider(
			IRepository<ToDoItem> repository,
			IRepository<User> userRepository,
			IDataSource<ToDoItem>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
			_userRepository = userRepository;
		}

		/// <summary>
		/// Main method to retrieve todo items data
		/// Returns collection of todo items from external source
		/// </summary>
		public override IEnumerable<ToDoItem> GetData()
		{
			var tasks = new List<ToDoItem>();

			try
			{
				tasks.AddRange(LoadTasksFromExternalSource());

				_logger.LogInformation(
					string.Format(FinishedLoadingData,
						_typeName,
						tasks.Count));
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(GetData), ex);
			}

			return tasks;
		}

		/// <summary>
		/// Loads tasks from external source
		/// </summary>
		private IEnumerable<ToDoItem> LoadTasksFromExternalSource()
		{
			var tasks = new List<ToDoItem>();

			try
			{
				_logger.LogInformation(StartingExternalSource);

				var sourceTasks = LoadFromSourceAsync().Result;
				var users = LoadUsers();

				foreach (var taskData in sourceTasks)
				{
					try
					{
						if (!TaskExists(taskData))
						{
							var task = CreateTask(taskData, users);
							if (task != null)
							{
								tasks.Add(task);
								LogTaskAdded(task);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatTaskIdentifier(taskData);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadTasksFromExternalSource), ex);
			}

			return tasks;
		}

		/// <summary>
		/// Get a collection of users
		/// </summary>
		private List<User> LoadUsers()
		{
			_logger.LogInformation(LoadingRelatedData);
			return [.. _userRepository.AllReadOnly()];
		}

		/// <summary>
		/// Creates a new Task instance using the builder pattern
		/// </summary>
		private ToDoItem? CreateTask(ToDoItem taskData, List<User> users)
		{
			try
			{
				var user = users.FirstOrDefault(u => u.Id == taskData.UserId);
				if (user == null) return null;

				return new ToDoItemBuilder(
					user: user,
					title: taskData.Title,
					description: taskData.Description,
					dueDate: taskData.DueDate,
					priority: taskData.Priority,
					status: taskData.TaskStatus)
					.Build();
			}
			catch (Exception ex)
			{
				var identifier = FormatTaskIdentifier(taskData);
				LogProcessingError(identifier, ex);
				return null;
			}
		}

		/// <summary>
		/// Determines if a task with identical UserId, Title and Creation Date already exists in the system.
		/// </summary>
		private bool TaskExists(ToDoItem task)
		{
			return EntityExists(t =>
				t.UserId == task.UserId &&
				t.Title == task.Title &&
				t.CreatedAt.Date == task.CreatedAt.Date);
		}

		/// <summary>
		/// Logs the addition of a new task to the system, with different messages for default and custom tasks.
		/// </summary>
		private void LogTaskAdded(ToDoItem task)
		{
			var message = string.Format(
				TaskAdded,
				task.User?.UserName ?? task.UserId.ToString(),
				task.Title);

			_logger.LogInformation(message);
		}

		/// <summary>
		/// Creates a formatted string identifier for a task using its title and user information.
		/// </summary>
		private static string FormatTaskIdentifier(ToDoItem task)
		{
			return string.Format(
				TaskIdentifier,
				task.Title,
				task.User?.UserName ?? task.UserId.ToString());
		}
	}
}