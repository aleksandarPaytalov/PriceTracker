using PriceTracker.Infrastructure.Constants;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Buiders
{
	/// <summary>
	/// UserBuilder class used for data seeding and validation before data beeing imported in database 
	/// </summary>
	public class UserBuilder : IBuilder<User>
	{
		private readonly User _user = new();

		public UserBuilder WithUsername(string username)
		{
			_user.UserName = username;
			return this;
		}

		public UserBuilder WithEmail(string email)
		{
			_user.Email = email;
			return this;
		}

		public UserBuilder WithPasswordHash(string passwordHash)
		{
			_user.PasswordHash = passwordHash;
			return this;
		}

		public User Build()
		{
			ValidateUser();
			return _user;
		}

		private void ValidateUser()
		{
			// Username validations
			if (string.IsNullOrEmpty(_user.UserName))
			{
				throw new InvalidOperationException("Username cannot be empty");
			}

			if (_user.UserName.Length < DataConstants.userNameMinLength ||
				_user.UserName.Length > DataConstants.userNameMaxLength)
			{
				throw new InvalidOperationException(
					$"Username must be between {DataConstants.userNameMinLength} and {DataConstants.userNameMaxLength} characters");
			}

			if (_user.UserName.Contains(" "))
			{
				throw new InvalidOperationException("Username cannot contain spaces");
			}

			// Email validations
			if (string.IsNullOrEmpty(_user.Email))
			{
				throw new InvalidOperationException("Email cannot be empty");
			}

			if (_user.Email.Length > DataConstants.emailAddressMaxLength)
			{
				throw new InvalidOperationException(
					$"Email cannot be longer than {DataConstants.emailAddressMaxLength} characters");
			}

			if (!_user.Email.Contains("@") || !_user.Email.Contains("."))
			{
				throw new InvalidOperationException("Invalid email format");
			}

			// Password validations
			if (string.IsNullOrEmpty(_user.PasswordHash))
			{
				throw new InvalidOperationException("Password hash cannot be empty");
			}

			if (_user.PasswordHash.Length < DataConstants.passwordHashMinLength ||
				_user.PasswordHash.Length > DataConstants.passwordHashMaxLength)
			{
				throw new InvalidOperationException(
					$"Password hash must be between {DataConstants.passwordHashMinLength} and {DataConstants.passwordHashMaxLength} characters");
			}

			// Common data validations
			var forbiddenCharacters = new[] { ";", "'", "\"", "\\", "--" };
			if (forbiddenCharacters.Any(c => _user.UserName.Contains(c) || _user.Email.Contains(c)))
			{
				throw new InvalidOperationException("Input contains forbidden characters");
			}

			// SQL Injection prevention
			if (_user.UserName.ToLower().Contains("select") ||
				_user.UserName.ToLower().Contains("insert") ||
				_user.UserName.ToLower().Contains("update") ||
				_user.UserName.ToLower().Contains("delete") ||
				_user.UserName.ToLower().Contains("drop"))
			{
				throw new InvalidOperationException("Username contains forbidden SQL keywords");
			}

			// XSS prevention
			if (_user.UserName.Contains("<") || _user.UserName.Contains(">") ||
				_user.Email.Contains("<") || _user.Email.Contains(">"))
			{
				throw new InvalidOperationException("Input contains forbidden HTML characters");
			}

			// Length ratio validation
			if (_user.UserName.Distinct().Count() < 2)
			{
				throw new InvalidOperationException("Username must contain at least 2 different characters");
			}

			// Case validation
			if (_user.UserName.All(char.IsUpper) || _user.UserName.All(char.IsLower))
			{
				throw new InvalidOperationException("Username must contain both upper and lower case characters");
			}

			// Special username requirements
			if (_user.UserName.StartsWith(".") || _user.UserName.EndsWith(".") ||
				_user.UserName.StartsWith("_") || _user.UserName.EndsWith("_"))
			{
				throw new InvalidOperationException("Username cannot start or end with dots or underscores");
			}

			// Consecutive special characters
			if (_user.UserName.Contains("..") || _user.UserName.Contains("__"))
			{
				throw new InvalidOperationException("Username cannot contain consecutive special characters");
			}

			// Number only validation
			if (_user.UserName.All(char.IsDigit))
			{
				throw new InvalidOperationException("Username cannot contain only numbers");
			}

			// First character validation
			if (char.IsDigit(_user.UserName[0]))
			{
				throw new InvalidOperationException("Username cannot start with a number");
			}

			// Simple profanity check (можете да разширите списъка)
			var profanityList = new[] { "admin", "root", "system", "moderator" };
			if (profanityList.Any(word =>
				_user.UserName.ToLower().Contains(word.ToLower())))
			{
				throw new InvalidOperationException("Username contains forbidden words");
			}

			// Domain validation for email
			var emailParts = _user.Email.Split('@');
			if (emailParts.Length != 2)
			{
				throw new InvalidOperationException("Invalid email format");
			}

			var domain = emailParts[1];
			var forbiddenDomains = new[] { "temp.com", "temporary.com", "disposable.com" };
			if (forbiddenDomains.Any(d => domain.EndsWith(d, StringComparison.OrdinalIgnoreCase)))
			{
				throw new InvalidOperationException("Email domain not allowed");
			}

			// Local part of email validation
			var localPart = emailParts[0];
			if (localPart.Length < 3)
			{
				throw new InvalidOperationException("Email local part must be at least 3 characters long");
			}

			if (localPart.StartsWith(".") || localPart.EndsWith("."))
			{
				throw new InvalidOperationException("Email local part cannot start or end with a dot");
			}

			// Special email validation
			if (_user.Email.Count(c => c == '@') > 1)
			{
				throw new InvalidOperationException("Email cannot contain multiple @ characters");
			}

			// Relationship validations
			if (_user.UserName.ToLower() == emailParts[0].ToLower())
			{
				throw new InvalidOperationException("Username cannot be the same as email local part");
			}
		}
	}
}
