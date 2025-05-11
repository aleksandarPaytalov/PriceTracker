using Microsoft.AspNetCore.Identity;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.UserDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
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

		private bool UserExists(User user)
		{
			return UserExists(user.Email!);
		}

		private bool UserExists(string email)
		{
			return EntityExists(u => u.Email == email);
		}

		private void LogUserAdded(User user, bool isDefault)
		{
			var message = string.Format(
				isDefault ? DefaultUserAdded
						: UserAdded,
				user.UserName,
				user.Email);

			_logger.LogInformation(message);
		}

		private string FormatUserIdentifier(User user)
		{
			return FormatUserIdentifier(user.UserName!, user.Email!);
		}

		private string FormatUserIdentifier(string userName, string email)
		{
			return string.Format(
				UserIdentifier,
				userName,
				email);
		}
	}
}