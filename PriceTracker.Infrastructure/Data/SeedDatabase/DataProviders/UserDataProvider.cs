using Microsoft.AspNetCore.Identity;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.UserDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	/// <summary>
	/// Provider responsible for loading and managing user data.
	/// Handles both default and external user sources.
	/// </summary>
	public class UserDataProvider : BaseDataProvider<User>
	{
		private readonly IPasswordHasher<User> _passwordHasher;

		public UserDataProvider(
			IRepository<User> repository,
			IPasswordHasher<User> passwordHasher,
			IDataSource<User>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
			_passwordHasher = passwordHasher;
		}

		/// <summary>
		/// Retrieves all user data either from external source or generates default users
		/// </summary>
		/// <returns>Collection of user entities</returns>
		public override IEnumerable<User> GetData()
		{
			var users = new List<User>();

			try
			{
				if (_dataSource != null)
				{
					users.AddRange(LoadUsersFromExternalSource());
				}
				else
				{
					users.AddRange(LoadDefaultUsers());
				}

				_logger.LogInformation(
					string.Format(FinishedLoadingData,
						_typeName,
						users.Count));
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(GetData), ex);
			}

			return users;
		}

		/// <summary>
		/// Loads users from external data source
		/// </summary>
		/// <returns>Collection of users from external source</returns>
		private IEnumerable<User> LoadUsersFromExternalSource()
		{
			var users = new List<User>();

			try
			{
				_logger.LogInformation(StartingExternalSource);

				var sourceUsers = LoadFromSourceAsync().Result;
				foreach (var userData in sourceUsers)
				{
					try
					{
						if (!UserExists(userData))
						{
							var user = CreateUser(
								userData.UserName!,
								userData.Email!,
								TestPassword); // We use test password for external users

							if (user != null)
							{
								users.Add(user);
								LogUserAdded(user, isDefault: false);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatUserIdentifier(userData);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadUsersFromExternalSource), ex);
			}

			return users;
		}

		/// <summary>
		/// Creates default users when no external source is available
		/// </summary>
		/// <returns>Collection of default users (admin and test user)</returns>
		private IEnumerable<User> LoadDefaultUsers()
		{
			var users = new List<User>();

			try
			{
				_logger.LogInformation(LoadingDefaultData);

				// Creating admin user
				if (!UserExists(AdminEmail))
				{
					var adminUser = CreateUser(
						AdminUserName,
						AdminEmail,
						AdminPassword);

					if (adminUser != null)
					{
						users.Add(adminUser);
						LogUserAdded(adminUser, isDefault: true);
					}
				}

				// Creating test/regular user
				if (!UserExists(TestEmail))
				{
					var testUser = CreateUser(
						TestUserName,
						TestEmail,
						TestPassword);

					if (testUser != null)
					{
						users.Add(testUser);
						LogUserAdded(testUser, isDefault: true);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadDefaultUsers), ex);
			}

			return users;
		}

		/// <summary>
		/// Creates a new user using the UserBuilder
		/// </summary>
		private User? CreateUser(string userName, string email, string password)
		{
			try
			{
				var hashedPassword = _passwordHasher.HashPassword(null!, password);

				return new UserBuilder(
					userName: userName,
					email: email,
					passwordHash: hashedPassword,
					_repository)
					.Build();
			}
			catch (Exception ex)
			{
				var identifier = FormatUserIdentifier(userName, email);
				LogProcessingError(identifier, ex);
				return null;
			}
		}

		/// <summary>
		/// Checks if a user already exists in the database
		/// </summary>
		private bool UserExists(User user)
		{
			return UserExists(user.Email!);
		}

		/// <summary>
		/// Checks if a user exists by email
		/// </summary>
		private bool UserExists(string email)
		{
			return EntityExists(u => u.Email == email);
		}

		/// <summary>
		/// Logs the addition of a new user
		/// </summary>
		private void LogUserAdded(User user, bool isDefault)
		{
			var message = string.Format(
				isDefault ? DefaultUserAdded
						: UserAdded,
				user.UserName,
				user.Email);

			_logger.LogInformation(message);
		}

		/// <summary>
		/// Creates a formatted identifier for a user
		/// </summary>
		private string FormatUserIdentifier(User user)
		{
			return FormatUserIdentifier(user.UserName!, user.Email!);
		}

		/// <summary>
		/// Creates a formatted identifier using individual user components
		/// </summary>
		private string FormatUserIdentifier(string userName, string email)
		{
			return string.Format(
				UserIdentifier,
				userName,
				email);
		}
	}
}