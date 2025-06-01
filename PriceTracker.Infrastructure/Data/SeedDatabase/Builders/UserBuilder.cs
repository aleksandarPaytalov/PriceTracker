using Microsoft.AspNetCore.Identity;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.JsonModels;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Enhanced UserBuilder for creating validated User entities with password hashing support
	/// </summary>
	public class UserBuilder : IBuilder<User>
	{
		private readonly User _user;
		private static readonly HashSet<string> _existingUserNames = new(StringComparer.OrdinalIgnoreCase);
		private static readonly HashSet<string> _existingEmails = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new user from JSON DTO with full validation and password hashing
		/// </summary>
		/// <param name="userDto">JSON DTO containing user data with plain text password</param>
		/// <param name="passwordHasher">Password hasher for securing passwords</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public UserBuilder(UserJsonDto userDto, IPasswordHasher<User> passwordHasher)
		{
			try
			{
				ValidateUserJsonDto(userDto);
				_user = CreateUserFromDto(userDto, passwordHasher);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create user from JSON: {ex.Message}");
			}
		}

		/// <summary>
		/// Creates a new user from individual parameters with validation and password hashing
		/// </summary>
		/// <param name="id">User ID</param>
		/// <param name="userName">Username</param>
		/// <param name="email">Email address</param>
		/// <param name="password">Plain text password (will be hashed)</param>
		/// <param name="passwordHasher">Password hasher</param>
		/// <param name="emailConfirmed">Email confirmation status</param>
		/// <param name="createdAt">Creation timestamp</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public UserBuilder(
			string id,
			string userName,
			string email,
			string password,
			IPasswordHasher<User> passwordHasher,
			bool emailConfirmed = true,
			DateTime? createdAt = null)
		{
			try
			{
				var userDto = new UserJsonDto
				{
					Id = id,
					UserName = userName,
					Email = email,
					Password = password,
					EmailConfirmed = emailConfirmed,
					CreatedAt = createdAt ?? DateTime.UtcNow
				};

				ValidateUserJsonDto(userDto);
				_user = CreateUserFromDto(userDto, passwordHasher);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create user: {ex.Message}");
			}
		}

		/// <summary>
		/// Creates a new user from existing User with password hashing
		/// </summary>
		/// <param name="existingUser">Existing User (without password hash)</param>
		/// <param name="password">Plain text password to hash</param>
		/// <param name="passwordHasher">Password hasher</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public UserBuilder(User existingUser, string password, IPasswordHasher<User> passwordHasher)
		{
			try
			{
				if (existingUser == null)
				{
					throw new ArgumentNullException(nameof(existingUser), UserConstants.UserRequired);
				}

				ValidateExistingUser(existingUser, password);
				_user = existingUser;

				// Hash the password
				_user.PasswordHash = passwordHasher.HashPassword(_user, password);

				// Ensure required fields are set
				EnsureUserFieldsComplete(_user);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to validate and hash password for existing user: {ex.Message}");
			}
		}

		/// <summary>
		/// Builds and returns the validated User with hashed password
		/// </summary>
		/// <returns>A validated User object with hashed password</returns>
		public User Build() => _user;

		/// <summary>
		/// Validates user JSON DTO with comprehensive checks
		/// </summary>
		/// <param name="userDto">User DTO to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateUserJsonDto(UserJsonDto userDto)
		{
			if (userDto == null)
			{
				throw new ValidationException(UserConstants.UserRequired);
			}

			// Standard model validation
			var validationResults = new List<ValidationResult>();
			var validationContext = new ValidationContext(userDto);

			if (!Validator.TryValidateObject(userDto, validationContext, validationResults, true))
			{
				var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
				throw new ValidationException($"User validation failed: {errors}");
			}

			// Business logic validation
			ValidateUserId(userDto.Id);
			ValidateUserName(userDto.UserName);
			ValidateEmail(userDto.Email);
			ValidatePassword(userDto.Password);

			// Auto-generate normalized values if not provided
			if (string.IsNullOrEmpty(userDto.NormalizedUserName))
			{
				userDto.NormalizedUserName = userDto.UserName.ToUpperInvariant();
			}

			if (string.IsNullOrEmpty(userDto.NormalizedEmail))
			{
				userDto.NormalizedEmail = userDto.Email.ToUpperInvariant();
			}

			// Auto-generate stamps if not provided
			if (string.IsNullOrEmpty(userDto.SecurityStamp))
			{
				userDto.SecurityStamp = Guid.NewGuid().ToString();
			}

			if (string.IsNullOrEmpty(userDto.ConcurrencyStamp))
			{
				userDto.ConcurrencyStamp = Guid.NewGuid().ToString();
			}

			// Set default created date if not provided
			if (!userDto.CreatedAt.HasValue)
			{
				userDto.CreatedAt = DateTime.UtcNow;
			}

			// Validate uniqueness
			ValidateUserUniqueness(userDto.UserName, userDto.Email);
		}

		/// <summary>
		/// Validates existing User
		/// </summary>
		/// <param name="user">User to validate</param>
		/// <param name="password">Password to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateExistingUser(User user, string password)
		{
			ValidateUserId(user.Id);
			ValidateUserName(user.UserName!);
			ValidateEmail(user.Email!);
			ValidatePassword(password);
			ValidateUserUniqueness(user.UserName!, user.Email!);
		}

		/// <summary>
		/// Validates user ID
		/// </summary>
		/// <param name="userId">User ID to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateUserId(string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ValidationException(UserConstants.UserIdRequired);
			}

			// Validate GUID format
			if (!Guid.TryParse(userId, out _))
			{
				throw new ValidationException(string.Format(UserConstants.InvalidUserIdFormat, userId));
			}
		}

		/// <summary>
		/// Validates username with security checks
		/// </summary>
		/// <param name="userName">Username to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateUserName(string userName)
		{
			if (string.IsNullOrWhiteSpace(userName))
			{
				throw new ValidationException(UserConstants.UserNameRequired);
			}

			// Length validation
			if (userName.Length < 3 || userName.Length > 256)
			{
				throw new ValidationException(string.Format(UserConstants.InvalidUserNameLength, 3, 256));
			}

			// Security validation
			if (ContainsForbiddenContent(userName))
			{
				throw new ValidationException(string.Format(UserConstants.UserNameContainsForbiddenContent, userName));
			}
		}

		/// <summary>
		/// Validates email address with comprehensive checks
		/// </summary>
		/// <param name="email">Email to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				throw new ValidationException(UserConstants.EmailRequired);
			}

			// Length validation
			if (email.Length < 5 || email.Length > 256)
			{
				throw new ValidationException(string.Format(UserConstants.InvalidEmailLength, 5, 256));
			}

			// Format validation
			if (!IsValidEmail(email))
			{
				throw new ValidationException(UserConstants.InvalidEmailFormat);
			}

			// Security validation
			if (ContainsForbiddenContent(email))
			{
				throw new ValidationException(string.Format(UserConstants.EmailContainsForbiddenContent, email));
			}

			// Business rules validation
			ValidateEmailBusinessRules(email);
		}

		/// <summary>
		/// Validates email business rules
		/// </summary>
		/// <param name="email">Email to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidateEmailBusinessRules(string email)
		{
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
			var forbiddenDomains = new[] { "temp.com", "temporary.com", "disposable.com", "fake.com" };
			if (forbiddenDomains.Any(d => domain.EndsWith(d, StringComparison.OrdinalIgnoreCase)))
			{
				throw new ValidationException(UserConstants.EmailDomainNotAllowed);
			}

			// Multiple @ check
			if (email.Count(c => c == '@') > 1)
			{
				throw new ValidationException(UserConstants.EmailMultipleAtSymbols);
			}
		}

		/// <summary>
		/// Validates password strength
		/// </summary>
		/// <param name="password">Password to validate</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		private static void ValidatePassword(string password)
		{
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ValidationException(UserConstants.PasswordRequired);
			}

			// Length validation
			if (password.Length < 6 || password.Length > 100)
			{
				throw new ValidationException(string.Format(UserConstants.InvalidPasswordLength, 6, 100));
			}

			// Strength validation (configurable rules)
			var issues = new List<string>();

			if (!password.Any(char.IsDigit))
			{
				issues.Add("at least one digit");
			}

			if (!password.Any(char.IsLower))
			{
				issues.Add("at least one lowercase letter");
			}

			if (!password.Any(char.IsUpper))
			{
				issues.Add("at least one uppercase letter");
			}

			if (!password.Any(c => !char.IsLetterOrDigit(c)))
			{
				issues.Add("at least one special character");
			}

			// Common weak passwords
			var weakPasswords = new[] { "password", "123456", "qwerty", "admin", "user", "test" };
			if (weakPasswords.Any(weak => password.Contains(weak, StringComparison.OrdinalIgnoreCase)))
			{
				issues.Add("no common weak patterns");
			}

			if (issues.Any())
			{
				throw new ValidationException($"Password must contain: {string.Join(", ", issues)}");
			}
		}

		/// <summary>
		/// Validates user uniqueness in current seeding session
		/// </summary>
		/// <param name="userName">Username to check</param>
		/// <param name="email">Email to check</param>
		/// <exception cref="ValidationException">Thrown when duplicate found</exception>
		private static void ValidateUserUniqueness(string userName, string email)
		{
			// Check username uniqueness
			if (_existingUserNames.Contains(userName))
			{
				throw new ValidationException(string.Format(UserConstants.DuplicateUserName, userName));
			}

			// Check email uniqueness  
			if (_existingEmails.Contains(email))
			{
				throw new ValidationException(string.Format(UserConstants.DuplicateEmail, email));
			}

			_existingUserNames.Add(userName);
			_existingEmails.Add(email);
		}

		/// <summary>
		/// Validates email format using MailAddress
		/// </summary>
		/// <param name="email">Email to validate</param>
		/// <returns>True if valid email format</returns>
		private static bool IsValidEmail(string email)
		{
			try
			{
				var addr = new MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Checks for forbidden content patterns
		/// </summary>
		/// <param name="content">Content to check</param>
		/// <returns>True if forbidden content found</returns>
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
		/// Creates User from validated DTO with password hashing
		/// </summary>
		/// <param name="userDto">Validated user DTO</param>
		/// <param name="passwordHasher">Password hasher</param>
		/// <returns>User entity with hashed password</returns>
		private static User CreateUserFromDto(UserJsonDto userDto, IPasswordHasher<User> passwordHasher)
		{
			var user = new User
			{
				Id = userDto.Id,
				UserName = userDto.UserName,
				NormalizedUserName = userDto.NormalizedUserName,
				Email = userDto.Email,
				NormalizedEmail = userDto.NormalizedEmail,
				EmailConfirmed = userDto.EmailConfirmed,
				SecurityStamp = userDto.SecurityStamp ?? Guid.NewGuid().ToString(),
				ConcurrencyStamp = userDto.ConcurrencyStamp ?? Guid.NewGuid().ToString(),
				CreatedAt = userDto.CreatedAt
			};

			// Hash the password
			user.PasswordHash = passwordHasher.HashPassword(user, userDto.Password);

			return user;
		}

		/// <summary>
		/// Ensures all required user fields are properly set
		/// </summary>
		/// <param name="user">User to complete</param>
		private static void EnsureUserFieldsComplete(User user)
		{
			if (string.IsNullOrEmpty(user.NormalizedUserName))
			{
				user.NormalizedUserName = user.UserName!.ToUpperInvariant();
			}

			if (string.IsNullOrEmpty(user.NormalizedEmail))
			{
				user.NormalizedEmail = user.Email!.ToUpperInvariant();
			}

			if (string.IsNullOrEmpty(user.SecurityStamp))
			{
				user.SecurityStamp = Guid.NewGuid().ToString();
			}

			if (string.IsNullOrEmpty(user.ConcurrencyStamp))
			{
				user.ConcurrencyStamp = Guid.NewGuid().ToString();
			}

			if (!user.CreatedAt.HasValue)
			{
				user.CreatedAt = DateTime.UtcNow;
			}
		}

		/// <summary>
		/// Clear tracking collections for new seeding session
		/// Call this before starting a new migration or seeding operation
		/// </summary>
		public static void ResetTracking()
		{
			_existingUserNames.Clear();
			_existingEmails.Clear();
		}

		/// <summary>
		/// Get count of currently tracked users in this session
		/// </summary>
		public static int GetTrackedUserCount()
		{
			return _existingUserNames.Count;
		}

		/// <summary>
		/// Check if a username is already tracked in current session
		/// </summary>
		/// <param name="userName">Username to check</param>
		/// <returns>True if already tracked, false otherwise</returns>
		public static bool IsUserNameTracked(string userName)
		{
			return _existingUserNames.Contains(userName);
		}

		/// <summary>
		/// Check if an email is already tracked in current session
		/// </summary>
		/// <param name="email">Email to check</param>
		/// <returns>True if already tracked, false otherwise</returns>
		public static bool IsEmailTracked(string email)
		{
			return _existingEmails.Contains(email);
		}
	}
}