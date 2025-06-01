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

		// ================================
		// BUILDER ERROR MESSAGES
		// ================================

		internal static class BuilderConstants
		{
			// Common builder error messages
			public const string FailedToCreateEntity = "Failed to create {0}: {1}";
			public const string ValidationFailed = "{0} validation failed: {1}";

			// User builder specific
			public const string FailedToCreateUserFromJson = "Failed to create user from JSON: {0}";
			public const string FailedToCreateUser = "Failed to create user: {0}";
			public const string FailedToValidateExistingUser = "Failed to validate and hash password for existing user: {0}";
			public const string UserValidationFailed = "User validation failed: {0}";
			public const string PasswordMustContain = "Password must contain: {0}";

			// Role builder specific
			public const string FailedToCreateRoleFromJson = "Failed to create role from JSON: {0}";
			public const string FailedToCreateRole = "Failed to create role: {0}";
			public const string FailedToValidateExistingRole = "Failed to validate existing role: {0}";
			public const string RoleValidationFailed = "Role validation failed: {0}";

			// UserRole builder specific
			public const string FailedToCreateUserRoleFromJson = "Failed to create user-role mapping from JSON: {0}";
			public const string FailedToCreateUserRole = "Failed to create user-role mapping: {0}";
			public const string FailedToValidateExistingUserRole = "Failed to validate existing user-role mapping: {0}";
			public const string UserRoleValidationFailed = "User-role mapping validation failed: {0}";

			// Product builder specific
			public const string FailedToCreateProduct = "Failed to create product: {0}";
			public const string DuplicateProductInSession = "Duplicate product in current seed session: '{0}' by '{1}'";

			// Store builder specific
			public const string FailedToCreateStore = "Failed to create store: {0}";

			// Price builder specific
			public const string FailedToCreatePrice = "Failed to create price: {0}";

			// Expense builder specific
			public const string FailedToCreateExpense = "Failed to create Expense: {0}";

			// MonthlyBudget builder specific
			public const string FailedToCreateMonthlyBudget = "Failed to create Monthly budget: {0}";

			// Notification builder specific
			public const string FailedToCreateNotification = "Failed to create Notification: {0}";

			// ToDoItem builder specific
			public const string FailedToCreateTask = "Failed to create task: {0}";
		}

		// ================================
		// CONFIGURATION LOGGING MESSAGES
		// ================================

		internal static class ConfigurationLoggingConstants
		{
			// Success messages
			public const string LoadedFromJsonWithValidation = "✅ Loaded {0} {1} from JSON with Builder validation";
			public const string LoadedFromJsonWithPasswordHashing = "✅ Loaded {0} users from JSON with Builder validation and password hashing";
			public const string UsingDefaultSeedDataWithValidation = "✅ Using default seed data for {0} with Builder validation: {1} {0}";
			public const string UsingDefaultUsersWithPasswordHashing = "✅ Using default seed data for users with Builder validation and password hashing: {0} users";

			// Warning messages
			public const string NoItemsFoundInJson = "No {0} found in {1} file";
			public const string UsingDefaultSeedData = "Using default seed data for {0}";

			// Error messages
			public const string FailedToLoadFromJson = "Failed to load {0} from JSON: {1}";
			public const string FailedToValidateDefaultData = "Failed to validate default {0} data: {1}";
			public const string FailedToSeedDefaultData = "Failed to seed default {0} data: {1}";
			public const string LoadingFailed = "{0} loading failed: {1}";
		}

		internal static class UserConstants
		{
			public const string UserRequired = "User is required";
			public const string UserIdRequired = "User ID is required";
			public const string InvalidUserIdFormat = "User ID must be a valid GUID format. Provided value: {0}";
			public const string UserNameRequired = "Username is required";
			public const string InvalidUserNameLength = "Username must be between {0} and {1} characters";
			public const string UserNameContainsForbiddenContent = "Username contains forbidden content: {0}";
			public const string DuplicateUserName = "Username '{0}' is already taken in current seed data";

			public const string EmailRequired = "Email address is required";
			public const string InvalidEmailLength = "Email must be between {0} and {1} characters";
			public const string InvalidEmailFormat = "Invalid email format";
			public const string EmailContainsForbiddenContent = "Email contains forbidden content: {0}";
			public const string DuplicateEmail = "Email address '{0}' is already taken in current seed data";
			public const string EmailLocalPartTooShort = "Email local part must be at least 3 characters long";
			public const string EmailLocalPartInvalidDot = "Email local part cannot start or end with a dot";
			public const string EmailDomainNotAllowed = "Email domain not allowed";
			public const string EmailMultipleAtSymbols = "Email cannot contain multiple @ symbols";

			public const string PasswordRequired = "Password is required";
			public const string InvalidPasswordLength = "Password must be between {0} and {1} characters";
			public const string SqlInjectionAttempt = "Potential SQL injection attempt detected. Input contains forbidden SQL keywords";
			public const string XssAttempt = "Potential XSS attack attempt detected. Input contains forbidden script patterns";
		}

		internal static class RoleConstants
		{
			public const string RoleRequired = "Role is required";
			public const string RoleIdRequired = "Role ID is required";
			public const string InvalidRoleIdFormat = "Role ID must be a valid GUID format. Provided value: {0}";
			public const string RoleNameRequired = "Role name is required";
			public const string InvalidRoleNameLength = "Role name must be between {0} and {1} characters";
			public const string RoleNameContainsForbiddenContent = "Role name contains forbidden content: {0}";
			public const string InvalidRoleNameFormat = "Role name contains invalid characters: {0}";
			public const string NormalizedNameRequired = "Normalized role name is required";
			public const string NormalizedNameNotUppercase = "Normalized role name must be uppercase";
			public const string DuplicateRoleName = "Role name '{0}' is already taken in current seed data";
		}

		internal static class UserRoleConstants
		{
			public const string UserRoleRequired = "User-role mapping is required";
			public const string UserIdRequired = "User ID is required for user-role mapping";
			public const string RoleIdRequired = "Role ID is required for user-role mapping";
			public const string InvalidUserIdFormat = "User ID must be a valid GUID format. Provided value: {0}";
			public const string InvalidRoleIdFormat = "Role ID must be a valid GUID format. Provided value: {0}";
			public const string UserIdContainsForbiddenContent = "User ID contains forbidden content: {0}";
			public const string RoleIdContainsForbiddenContent = "Role ID contains forbidden content: {0}";
			public const string DuplicateUserRoleMapping = "User-role mapping already exists: User '{0}' -> Role '{1}'";
			public const string UserNotFound = "User with ID '{0}' not found in available users. Available IDs: {1}";
			public const string RoleNotFound = "Role with ID '{0}' not found in available roles. Available IDs: {1}";
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