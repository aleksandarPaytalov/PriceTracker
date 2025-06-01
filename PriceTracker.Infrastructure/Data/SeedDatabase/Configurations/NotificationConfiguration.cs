using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationLoggingConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.NotificationConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
	{
		private readonly IOptions<SeedingOptions> _options;

		public NotificationConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public void Configure(EntityTypeBuilder<Notification> builder)
		{
			// Reset tracking for new seeding session
			NotificationBuilder.ResetTracking();

			// Relations Config
			builder.HasOne(n => n.User)
				   .WithMany(u => u.Notifications)
				   .HasForeignKey(n => n.UserId)
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(n => n.Task)
				   .WithMany(t => t.Notifications)
				   .HasForeignKey(n => n.TaskId)
				   .OnDelete(DeleteBehavior.NoAction);

			// Seeding data 
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Notification", false))
			{
				var validatedNotifications = LoadAndValidateNotificationsFromJson();

				if (validatedNotifications.Any())
				{
					builder.HasData(validatedNotifications);
					MigrationLogger.LogInformation(string.Format(LoadedNotificationsFromJson, validatedNotifications.Count()));
				}
				else
				{
					var errorMessage = string.Format(ExternalSourceEnabledButNoData, "notifications.json");
					MigrationLogger.LogError(errorMessage);
					throw new InvalidOperationException(errorMessage);
				}
			}
			else
			{
				// Use default seed data only if Notification seeding is not disabled
				if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("Notification", true))
				{
					SeedDefaultData(builder);
					MigrationLogger.LogInformation(UsingDefaultNotificationData);
				}
			}
		}

		/// <summary>
		/// Loads notifications from JSON and validates them using NotificationBuilder
		/// </summary>
		private IEnumerable<Notification> LoadAndValidateNotificationsFromJson()
		{
			try
			{
				// Load JSON notifications directly as Notification objects
				var jsonNotifications = MigrationDataHelper.GetDataFromJson<Notification>("notifications.json");

				if (!jsonNotifications.Any())
				{
					MigrationLogger.LogWarning(NoNotificationsFoundInJson);
					return Enumerable.Empty<Notification>();
				}

				// We need to get users and tasks to validate foreign keys
				var users = GetExistingUsers();
				var tasks = GetExistingTasks();

				// Validate using NotificationBuilder - returns only valid items
				var validatedNotifications = MigrationDataHelper.ValidateItems(
					jsonNotifications,
					notification => ValidateNotificationWithBuilder(notification, users, tasks),
					"notification",
					_options.Value.StrictValidation);

				return validatedNotifications;
			}
			catch (Exception ex) when (!(ex is ValidationException))
			{
				MigrationLogger.LogError(string.Format(FailedToLoadNotificationsFromJson, ex.Message), ex);
				throw new InvalidOperationException(string.Format(NotificationLoadingFailed, ex.Message), ex);
			}
		}

		/// <summary>
		/// Validates a Notification object using NotificationBuilder validation logic
		/// </summary>
		private static Notification ValidateNotificationWithBuilder(
			Notification notification,
			IEnumerable<User> users,
			IEnumerable<ToDoItem> tasks)
		{
			// Validate NotificationId first
			if (notification.NotificationId <= 0)
			{
				throw new ValidationException(string.Format(InvalidNotificationId, notification.NotificationId));
			}

			// Find the referenced user
			var user = users.FirstOrDefault(u => u.Id == notification.UserId);
			if (user == null)
			{
				throw new ValidationException(string.Format(UserNotFoundForNotification, notification.UserId));
			}

			// Find the referenced task
			var task = tasks.FirstOrDefault(t => t.TaskId == notification.TaskId);
			if (task == null)
			{
				throw new ValidationException(string.Format(TaskNotFoundForNotification, notification.TaskId));
			}

			// Use NotificationBuilder for validation
			var notificationBuilder = new NotificationBuilder(
				user,
				task,
				notification.Message,
				notification.NotificationTime,
				notification.IsRead,
				notification.CreatedAt
			);

			var validatedNotification = notificationBuilder.Build();
			validatedNotification.NotificationId = notification.NotificationId;

			return validatedNotification;
		}

		/// <summary>
		/// Seeds default data from SeedData class using NotificationBuilder validation
		/// </summary>
		private static void SeedDefaultData(EntityTypeBuilder<Notification> builder)
		{
			try
			{
				var data = new SeedData();
				data.Initialize();

				// Get related entities for validation
				var users = new[] { data.Administrator, data.User, data.Guest };
				var tasks = new[] { data.Task1, data.Task2, data.Task3 };

				var defaultNotifications = new[]
				{
					data.Notification1, data.Notification2, data.Notification3
				};

				// Validate default notifications - should never fail
				var validatedNotifications = MigrationDataHelper.ValidateItems(
					defaultNotifications,
					notification => ValidateNotificationWithBuilder(notification, users, tasks),
					"default notification",
					strictValidation: true);

				builder.HasData(validatedNotifications);
			}
			catch (Exception ex)
			{
				MigrationLogger.LogError(string.Format(FailedToSeedDefaultNotificationData, ex.Message), ex);
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

		/// <summary>
		/// Gets existing tasks for foreign key validation
		/// </summary>
		private IEnumerable<ToDoItem> GetExistingTasks()
		{
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Task", false))
			{
				return MigrationDataHelper.GetDataFromJson<ToDoItem>("tasks.json");
			}
			else
			{
				var data = new SeedData();
				data.Initialize();
				return new[] { data.Task1, data.Task2, data.Task3 };
			}
		}
	}
}