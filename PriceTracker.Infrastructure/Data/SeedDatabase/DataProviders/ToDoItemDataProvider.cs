using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.ToDoItemDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	public class ToDoItemDataProvider : BaseDataProvider<ToDoItem>
	{
		private readonly IRepository<User> _userRepository;
		private readonly Random _random;

		public ToDoItemDataProvider(
			IRepository<ToDoItem> repository,
			IRepository<User> userRepository,
			IDataSource<ToDoItem>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
			_userRepository = userRepository;
			_random = new Random();
		}

		public override IEnumerable<ToDoItem> GetData()
		{
			var tasks = new List<ToDoItem>();

			try
			{
				if (_dataSource != null)
				{
					tasks.AddRange(LoadTasksFromExternalSource());
				}
				else
				{
					tasks.AddRange(LoadDefaultTasks());
				}

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
								LogTaskAdded(task, isDefault: false);
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

		private IEnumerable<ToDoItem> LoadDefaultTasks()
		{
			var tasks = new List<ToDoItem>();

			try
			{
				_logger.LogInformation(LoadingDefaultData);

				var users = LoadUsers();

				foreach (var user in users)
				{
					foreach (var defaultTask in GetDefaultTasksForUser(user))
					{
						try
						{
							if (!TaskExists(defaultTask))
							{
								var task = CreateTask(defaultTask, users);
								if (task != null)
								{
									tasks.Add(task);
									LogTaskAdded(task, isDefault: true);
								}
							}
						}
						catch (Exception ex)
						{
							var identifier = FormatTaskIdentifier(defaultTask);
							LogProcessingError(identifier, ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadDefaultTasks), ex);
			}

			return tasks;
		}

		private List<User> LoadUsers()
		{
			_logger.LogInformation(LoadingRelatedData);
			return _userRepository.AllReadOnly().ToList();
		}

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

		private IEnumerable<ToDoItem> GetDefaultTasksForUser(User user)
		{
			var defaultTasks = new[]
			{
				("Check prices", "Review product prices in all stores", TaskPriority.High),
				("Update budget", "Update monthly budget", TaskPriority.Medium),
				("Review expenses", "Review last month expenses", TaskPriority.Low)
			};

			return defaultTasks.Select(t => new ToDoItem
			{
				UserId = user.Id,
				User = user,
				Title = t.Item1,
				Description = t.Item2,
				Priority = t.Item3,
				TaskStatus = Models.TaskStatus.Pending,
				DueDate = DateTime.Today.AddDays(_random.Next(1, 14)),
				CreatedAt = DateTime.UtcNow
			});
		}

		private bool TaskExists(ToDoItem task)
		{
			return EntityExists(t =>
				t.UserId == task.UserId &&
				t.Title == task.Title &&
				t.CreatedAt.Date == task.CreatedAt.Date);
		}

		private void LogTaskAdded(ToDoItem task, bool isDefault)
		{
			var message = string.Format(
				isDefault ? DefaultTaskAdded
						: TaskAdded,
				task.User?.UserName ?? task.UserId.ToString(),
				task.Title);

			_logger.LogInformation(message);
		}

		private string FormatTaskIdentifier(ToDoItem task)
		{
			return string.Format(
				TaskIdentifier,
				task.Title,
				task.User?.UserName ?? task.UserId.ToString());
		}
	}
}