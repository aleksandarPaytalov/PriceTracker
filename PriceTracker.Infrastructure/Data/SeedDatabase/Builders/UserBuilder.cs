using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Common;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Constants.DataConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// User builder class used for data seeding and validation before data being imported in database 
	/// </summary>
	public class UserBuilder : IBuilder<User>
	{
		private readonly User _user;
		private readonly IRepository<User>? _userRepository;
		private static readonly HashSet<string> _existingUsernames = new(StringComparer.OrdinalIgnoreCase);
		private static readonly HashSet<string> _existingEmails = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new user with required data
		/// </summary>
		/// <param name="userName">Username</param>
		/// <param name="email">Email address</param>
		/// <param name="passwordHash">Hashed password</param>
		/// <param name="userRepository">Optional repository for checking existing users in database</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public UserBuilder(
			string userName,
			string email,
			string passwordHash,
			IRepository<User>? userRepository = null)
		{
			_userRepository = userRepository;

			try
			{
				ValidateUserInputData(userName, email, passwordHash);

				_user = new User
				{
					UserName = userName,
					Email = email,
					PasswordHash = passwordHash,
					CreatedAt = DateTime.UtcNow,
					Expenses = new List<Expense>(),
					MonthlyBudgets = new List<MonthlyBudget>(),
					Tasks = new List<ToDoItem>(),
					Notifications = new List<Notification>()
				};

				_existingUsernames.Add(userName);
				_existingEmails.Add(email);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create user: {ex.Message}");
			}
		}
		public User Build() => _user;

		private void ValidateUserInputData(
			string userName,
			string email,
			string passwordHash)
		{
			// Username validations
			if (string.IsNullOrWhiteSpace(userName))
			{
				throw new ValidationException(UserConstants.UserNameRequired);
			}

			if (userName.Length < userNameMinLength || userName.Length > userNameMaxLength)
			{
				throw new ValidationException(
					string.Format(UserConstants.InvalidUserNameLength,
						userNameMinLength,
						userNameMaxLength));
			}

			// Check for duplicate username in current seed
			if (_existingUsernames.Contains(userName))
			{
				throw new ValidationException(
					string.Format(UserConstants.UserNameExistsInSeed, userName));
			}

			// Check for duplicate username in database
			if (_userRepository != null)
			{
				var usernameExists = _userRepository
					.AllReadOnly()
					.Any(u => u.UserName!.Equals(userName, StringComparison.CurrentCultureIgnoreCase));

				if (usernameExists)
				{
					throw new ValidationException(
						string.Format(UserConstants.UserNameExistsInDb, userName));
				}
			}

			// Email validations
			if (string.IsNullOrWhiteSpace(email))
			{
				throw new ValidationException(UserConstants.EmailRequired);
			}

			if (email.Length < emailAddressMinLength || email.Length > emailAddressMaxLength)
			{
				throw new ValidationException(
					string.Format(UserConstants.InvalidEmailLength,
						emailAddressMinLength,
						emailAddressMaxLength));
			}

			if (!IsValidEmail(email))
			{
				throw new ValidationException(UserConstants.InvalidEmailFormat);
			}

			// Split email validation
			var emailParts = email.Split('@');
			if (emailParts.Length != 2)
			{
				throw new ValidationException(UserConstants.InvalidEmailFormat);
			}

			// Local part validations
			var localPart = emailParts[0];
			if (localPart.Length < 3)
			{
				throw new ValidationException(UserConstants.EmailLocalPartTooShort);
			}

			if (localPart.StartsWith('.') || localPart.EndsWith('.'))
			{
				throw new ValidationException(UserConstants.EmailLocalPartInvalidDot);
			}

			// Domain validations
			var domain = emailParts[1];
			var forbiddenDomains = new[] { "temp.com", "temporary.com", "disposable.com" };
			if (forbiddenDomains.Any(d => domain.EndsWith(d, StringComparison.OrdinalIgnoreCase)))
			{
				throw new ValidationException(UserConstants.EmailDomainNotAllowed);
			}

			// Multiple @ check
			if (email.Count(c => c == '@') > 1)
			{
				throw new ValidationException(UserConstants.EmailMultipleAtSymbols);
			}

			// Special characters validation
			var forbiddenCharacters = new[] { ";", "'", "\"", "\\", "--" };
			if (forbiddenCharacters.Any(c => email.Contains(c)))
			{
				throw new ValidationException(UserConstants.EmailContainsForbiddenChars);
			}

			// XSS prevention
			if (email.Contains('<') || email.Contains('>'))
			{
				throw new ValidationException(UserConstants.EmailContainsHtmlChars);
			}

			// XSS Protection
			var xssPatterns = new[]
			{
				"<script", "javascript:", "vbscript:", "onload=",
				"onerror=", "onclick=", "onmouseover=",
				"alert(", "eval(", "document.cookie",
				"<iframe", "<object", "<embed",
				"data:text/html", "data:text/javascript",
				"<", ">", "src=", "href=",
				"style=", "expression(", "url("
			};

			if (xssPatterns.Any(pattern =>
				userName.ToLower().Contains(pattern.ToLower()) ||
				email.ToLower().Contains(pattern.ToLower())))
			{
				throw new ValidationException(UserConstants.XssAttempt);
			}



			// SQL Injection Protection
			var sqlKeywords = new[]
			{
				"select", "insert", "update", "delete", "drop",
				"union", "exec", "execute", "sp_", "xp_",
				"--", ";", "/*", "*/", "@@", "@",
				"char", "nchar", "varchar", "nvarchar",
				"table", "database", "sysobjects", "syscolumns"
			};

			if (sqlKeywords.Any(keyword =>
				userName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
				email.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)))
			{
				throw new ValidationException(UserConstants.SqlInjectionAttempt);
			}

			// Check for duplicate email in current seed
			if (_existingEmails.Contains(email))
			{
				throw new ValidationException(
					string.Format(UserConstants.EmailExistsInSeed, email));
			}

			// Check for duplicate email in database
			if (_userRepository != null)
			{
				var emailExists = _userRepository
					.AllReadOnly()
					.Any(u => u.Email!.Equals(email, StringComparison.CurrentCultureIgnoreCase));

				if (emailExists)
				{
					throw new ValidationException(
						string.Format(UserConstants.EmailExistsInDb, email));
				}
			}

			// Password hash validations
			if (string.IsNullOrWhiteSpace(passwordHash))
			{
				throw new ValidationException(UserConstants.PasswordHashRequired);
			}

			if (passwordHash.Length < passwordHashMinLength || passwordHash.Length > passwordHashMaxLength)
			{
				throw new ValidationException(
					string.Format(UserConstants.InvalidPasswordHashLength,
						passwordHashMinLength,
						passwordHashMaxLength));
			}
		}

		private static bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}
	}
}