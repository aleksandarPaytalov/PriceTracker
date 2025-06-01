using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationLoggingConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.TaskConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
	{
		private readonly IOptions<SeedingOptions> _options;

		public ToDoItemConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public void Configure(EntityTypeBuilder<ToDoItem> builder)
		{
			// Reset tracking for new seeding session
			ToDoItemBuilder.ResetTracking();

			// Relation Configuration
			builder.HasOne(t => t.User)
				   .WithMany(u => u.Tasks)
				   .HasForeignKey(t => t.UserId)
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(t => t.Notifications)
				   .WithOne(n => n.Task)
				   .HasForeignKey(n => n.TaskId)
				   .OnDelete(DeleteBehavior.NoAction);

			// Seeding data 
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Task", false))
			{
				var validatedTasks = LoadAndValidateTasksFromJson();

				if (validatedTasks.Any())
				{
					builder.HasData(validatedTasks);
					MigrationLogger.LogInformation(string.Format(LoadedTasksFromJson, validatedTasks.Count()));
				}
				else
				{
					var errorMessage = string.Format(ExternalSourceEnabledButNoData, "tasks.json");
					MigrationLogger.LogError(errorMessage);
					throw new InvalidOperationException(errorMessage);
				}
			}
			else
			{
				// Use default seed data only if Task seeding is not disabled
				if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("Task", true))
				{
					SeedDefaultData(builder);
					MigrationLogger.LogInformation(UsingDefaultTaskData);
				}
			}
		}

		/// <summary>
		/// Loads tasks from JSON and validates them using ToDoItemBuilder
		/// </summary>
		private IEnumerable<ToDoItem> LoadAndValidateTasksFromJson()
		{
			try
			{
				// Load JSON tasks directly as ToDoItem objects
				var jsonTasks = MigrationDataHelper.GetDataFromJson<ToDoItem>("tasks.json");

				if (!jsonTasks.Any())
				{
					MigrationLogger.LogWarning(NoTasksFoundInJson);
					return Enumerable.Empty<ToDoItem>();
				}

				// We need to get users to validate foreign keys
				var users = GetExistingUsers();

				// Validate using ToDoItemBuilder - returns only valid items
				var validatedTasks = MigrationDataHelper.ValidateItems(
					jsonTasks,
					task => ValidateTaskWithBuilder(task, users),
					"task",
					_options.Value.StrictValidation);

				return validatedTasks;
			}
			catch (Exception ex) when (!(ex is ValidationException))
			{
				MigrationLogger.LogError(string.Format(FailedToLoadTasksFromJson, ex.Message), ex);
				throw new InvalidOperationException(string.Format(TaskLoadingFailed, ex.Message), ex);
			}
		}

		/// <summary>
		/// Validates a ToDoItem object using ToDoItemBuilder validation logic
		/// </summary>
		private static ToDoItem ValidateTaskWithBuilder(
			ToDoItem task,
			IEnumerable<User> users)
		{
			// Validate TaskId first
			if (task.TaskId <= 0)
			{
				throw new ValidationException(string.Format(InvalidTaskId, task.TaskId));
			}

			// Find the referenced user
			var user = users.FirstOrDefault(u => u.Id == task.UserId);
			if (user == null)
			{
				throw new ValidationException(string.Format(UserNotFoundForTask, task.UserId));
			}

			// Use ToDoItemBuilder for validation
			var taskBuilder = new ToDoItemBuilder(
				user,
				task.Title,
				task.Description,
				task.DueDate,
				task.Priority,
				task.TaskStatus
			);

			var validatedTask = taskBuilder.Build();
			validatedTask.TaskId = task.TaskId;

			// Preserve the CreatedAt from JSON if provided, otherwise use builder's value
			if (task.CreatedAt != default(DateTime))
			{
				validatedTask.CreatedAt = task.CreatedAt;
			}

			return validatedTask;
		}

		/// <summary>
		/// Seeds default data from SeedData class using ToDoItemBuilder validation
		/// </summary>
		private static void SeedDefaultData(EntityTypeBuilder<ToDoItem> builder)
		{
			try
			{
				var data = new SeedData();
				data.Initialize();

				// Get users for validation
				var users = new[] { data.Administrator, data.User, data.Guest };

				var defaultTasks = new[]
				{
					data.Task1, data.Task2, data.Task3
				};

				// Validate default tasks - should never fail
				var validatedTasks = MigrationDataHelper.ValidateItems(
					defaultTasks,
					task => ValidateTaskWithBuilder(task, users),
					"default task",
					strictValidation: true);

				builder.HasData(validatedTasks);
			}
			catch (Exception ex)
			{
				MigrationLogger.LogError(string.Format(FailedToSeedDefaultTaskData, ex.Message), ex);
				throw;
			}
		}

		/// <summary>
		/// Gets existing users for foreign key validation
		/// </summary>
		private IEnumerable<User> GetExistingUsers()
		{
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("User", false))
			{
				return MigrationDataHelper.GetDataFromJson<User>("users.json");
			}
			else
			{
				var data = new SeedData();
				data.Initialize();
				return new[] { data.Administrator, data.User, data.Guest };
			}
		}
	}
}