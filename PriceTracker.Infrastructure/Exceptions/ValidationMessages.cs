namespace PriceTracker.Infrastructure.Exceptions
{
	internal static class ValidationMessages
	{
		internal static class ConfigurationConstants
		{
			public const string ExternalSourceEnabledButNoData =
				"UseExternalSource is set to true, but {0} is empty, missing, or invalid. " +
				"Either provide valid JSON data or set UseExternalSource to false.";
		}

		internal static class MigrationDataConstants
		{
			// File loading messages
			public const string JsonFileNotFound = "JSON file not found: {0}";
			public const string SuccessfullyLoadedItems = "Successfully loaded {0} items from {1}";
			public const string FailedToLoadData = "Failed to load data from {0}: {1}";

			// Validation messages
			public const string ValidationFailed = "Validation failed for {0} #{1}: {2}";
			public const string UnexpectedValidationError = "Unexpected error validating {0} #{1}: {2}";
			public const string StrictValidationFailed = "Strict validation failed for {0} #{1}: {2}";
			public const string ValidationProcessFailed = "Validation process failed for {0} #{1}: {2}";
			public const string ValidationCompletedWithErrors = "Validation completed for {0}: {1} valid, {2} invalid out of {3} total";
			public const string AllItemsValidatedSuccessfully = "All {0} {1}s validated successfully";
		}

		internal static class UserConstants
		{
			public const string UserNameRequired = "Username is required";
			public const string InvalidUserNameLength = "Username must be between {0} and {1} characters";
			public const string UserNameExistsInSeed = "Username '{0}' is already taken in current seed data";
			public const string UserNameExistsInDb = "Username '{0}' already exists in database";

			public const string EmailRequired = "Email address is required";
			public const string InvalidEmailLength = "Email must be between {0} and {1} characters";
			public const string InvalidEmailFormat = "Invalid email format";
			public const string EmailExistsInSeed = "Email address '{0}' is already taken in current seed data";
			public const string EmailExistsInDb = "Email address '{0}' already exists in database";
			public const string EmailLocalPartTooShort = "Email local part must be at least 3 characters long";
			public const string EmailLocalPartInvalidDot = "Email local part cannot start or end with a dot";
			public const string EmailDomainNotAllowed = "Email domain not allowed";
			public const string EmailContainsForbiddenChars = "Email contains forbidden characters";
			public const string EmailMultipleAtSymbols = "Email cannot contain multiple @ symbols";
			public const string EmailContainsHtmlChars = "Email contains forbidden HTML characters";

			public const string PasswordHashRequired = "Password hash is required";
			public const string InvalidPasswordHashLength = "Password hash must be between {0} and {1} characters";
			public const string SqlInjectionAttempt = "Potential SQL injection attempt detected. Input contains forbidden SQL keywords";
			public const string XssAttempt = "Potential XSS attack attempt detected. Input contains forbidden script patterns";

		}

		internal static class MonthlyBudgetConstants
		{
			public const string UserRequired = "User cannot be null when creating a monthly budget.";
			public const string InvalidAmount = "Budget amount must be greater than zero. Provided value: {0:C}";
			public const string ExceedsMaxAmount = "Budget amount exceeds maximum allowed value of {0:C}. Provided value: {1:C}";
			public const string InvalidMonth = "Invalid month value: {0}";
		}

		internal static class ExpenseConstants
		{
			public const string UserRequired = "User cannot be null when creating an expense.";
			public const string ProductRequired = "Product cannot be null when creating an expense.";
			public const string StoreRequired = "Store cannot be null when creating an expense.";
			public const string InvalidExpenseType = "Invalid expense type value: {0}";
			public const string InvalidAmount = "Amount must be greater than zero. Provided value: {0:C}";
			public const string ExceedsMaxAmount = "Amount cannot exceed {0:C}. Provided value: {1:C}";
			public const string FutureDate = "Date cannot be in the future. Current date: {0:g}, Provided date: {1:g}";
			public const string DescriptionTooLong = "Description length ({0}) exceeds maximum allowed length ({1}).";
		}

		internal static class NotificationConstants
		{
			public const string UserRequired = "User must be specified";
			public const string UserIdRequired = "User ID must be specified";
			public const string TaskRequired = "Task must be specified";
			public const string TaskIdRequired = "Task ID must be specified";
			public const string MessageRequired = "Message cannot be empty";
			public const string MessageTooLong = "Message cannot be longer than {0} characters";
			public const string NotificationTimeMissing = "Notification time must be specified";
			public const string CreatedAtMissing = "Creation time must be specified";
			public const string CreatedAtInFuture = "Creation time cannot be in the future";
			public const string InvalidTimeOrder = "Notification time must be after creation time";
			public const string InvalidNotificationTimeFormat = "Invalid notification time format provided";
			public const string InvalidCreatedAtFormat = "Invalid creation time format provided";
			public const string InvalidIsReadValue = "IsRead must be a valid boolean value (true/false)";
		}

		internal static class ProductConstants
		{
			public const string NameRequired = "Product name is required";
			public const string InvalidNameLength = "Product name must be between {0} and {1} characters";
			public const string BrandRequired = "Brand is required";
			public const string InvalidBrandLength = "Brand must be between {0} and {1} characters";
			public const string InvalidCategory = "Invalid product category: {0}";
			public const string InvalidQuantity = "Quantity cannot be negative";
			public const string InvalidProductId = "Product ID must be a positive number. Provided value: {0}";
		}

		internal static class ProductConfigurationConstants
		{
			public const string LoadedProductsFromJson = "Loaded {0} products from JSON for seeding";
			public const string NoProductsFoundInJson = "No products found in products.json file";
			public const string UsingDefaultSeedData = "Using default seed data for products";
			public const string FailedToLoadProductsFromJson = "Failed to load products from JSON: {0}";
			public const string ProductLoadingFailed = "Product loading failed: {0}";
			public const string FailedToSeedDefaultData = "Failed to seed default product data: {0}";
		}

		internal static class StoreConstants
		{
			public const string NameRequired = "Store name is required";
			public const string InvalidNameLength = "Store name must be between {0} and {1} characters";
			public const string StoreAlreadyExists = "Store with name '{0}' already exists in current seed data";
			public const string StoreAlreadyExistsInDb = "Store with name '{0}' already exists in database";
		}

		internal static class StoreConfigurationConstants
		{
			public const string LoadedStoresFromJson = "Loaded {0} stores from JSON for seeding";
			public const string NoStoresFoundInJson = "No stores found in stores.json file";
			public const string UsingDefaultSeedData = "Using default seed data for stores";
			public const string FailedToLoadStoresFromJson = "Failed to load stores from JSON: {0}";
			public const string StoreLoadingFailed = "Store loading failed: {0}";
			public const string FailedToSeedDefaultData = "Failed to seed default store data: {0}";
			public const string InvalidStoreId = "Store ID must be a positive number. Provided value: {0}";
		}

		internal static class PriceConstants
		{
			public const string ProductRequired = "Product must be specified";
			public const string ProductIdRequired = "Product ID must be specified";
			public const string StoreRequired = "Store must be specified";
			public const string StoreIdRequired = "Store ID must be specified";
			public const string InvalidPrice = "Price must be greater than zero. Provided value: {0:C}";
			public const string ExceedsMaxPrice = "Price exceeds maximum allowed value of {0:C}. Provided value: {1:C}";
			public const string InvalidDateFormat = "Invalid date format provided";
			public const string FutureDate = "Price check date cannot be in the future. Current date: {0:g}, Provided date: {1:g}";

			public const string InvalidPriceId = "Price ID must be a positive number. Provided value: {0}";
			public const string ProductNotFound = "Product with ID {0} not found during price validation";
			public const string StoreNotFound = "Store with ID {0} not found during price validation";
		}

		internal static class PriceConfigurationConstants
		{
			public const string LoadedPricesFromJson = "Loaded {0} prices from JSON for seeding";
			public const string NoPricesFoundInJson = "No prices found in prices.json file";
			public const string UsingDefaultSeedData = "Using default seed data for prices";
			public const string FailedToLoadPricesFromJson = "Failed to load prices from JSON: {0}";
			public const string PriceLoadingFailed = "Price loading failed: {0}";
			public const string FailedToSeedDefaultData = "Failed to seed default price data: {0}";
		}

		internal static class TaskConstants
		{
			public const string UserRequired = "User must be specified";
			public const string UserIdRequired = "User ID must be specified";
			public const string TitleRequired = "Task title is required";
			public const string InvalidTitleLength = "Task title must be between {0} and {1} characters";
			public const string InvalidDescriptionLength = "Task description cannot be longer than {0} characters";
			public const string InvalidDueDate = "Invalid due date format";
			public const string DueDateInPast = "Due date cannot be in the past";
		}
	}
}
