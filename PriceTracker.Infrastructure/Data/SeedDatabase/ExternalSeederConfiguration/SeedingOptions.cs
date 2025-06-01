using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;

/// <summary>
/// Comprehensive seeding options supporting both business entities and Identity data
/// </summary>
public class SeedingOptions
{
	/// <summary>
	/// Enable external JSON source for seeding (applies to all enabled seeders)
	/// </summary>
	public bool UseExternalSource { get; set; } = false;

	/// <summary>
	/// Base path for data folder (automatically resolved if empty)
	/// </summary>
	public string DataFolderPath { get; set; } = string.Empty;

	// ================================
	// BUSINESS ENTITY JSON FILES
	// ================================

	/// <summary>
	/// JSON file name for product data
	/// </summary>
	public string ProductJsonFile { get; set; } = "products.json";

	/// <summary>
	/// JSON file name for store data
	/// </summary>
	public string StoreJsonFile { get; set; } = "stores.json";

	/// <summary>
	/// JSON file name for price data
	/// </summary>
	public string PriceJsonFile { get; set; } = "prices.json";

	/// <summary>
	/// JSON file name for expense data
	/// </summary>
	public string ExpenseJsonFile { get; set; } = "expenses.json";

	/// <summary>
	/// JSON file name for task data
	/// </summary>
	public string TaskJsonFile { get; set; } = "tasks.json";

	/// <summary>
	/// JSON file name for notification data
	/// </summary>
	public string NotificationJsonFile { get; set; } = "notifications.json";

	/// <summary>
	/// JSON file name for budget data
	/// </summary>
	public string BudgetJsonFile { get; set; } = "budgets.json";

	// ================================
	// IDENTITY JSON FILES
	// ================================

	/// <summary>
	/// JSON file name for role data
	/// </summary>
	public string RoleJsonFile { get; set; } = "roles.json";

	/// <summary>
	/// JSON file name for user data (with plain text passwords)
	/// </summary>
	public string UserJsonFile { get; set; } = "users.json";

	/// <summary>
	/// JSON file name for user-role mapping data
	/// </summary>
	public string UserRoleJsonFile { get; set; } = "userroles.json";

	// ================================
	// SEEDER ENABLEMENT CONFIGURATION
	// ================================

	/// <summary>
	/// Controls which seeders are enabled
	/// Key: Seeder name, Value: Enabled status
	/// </summary>
	public Dictionary<string, bool> EnabledSeeders { get; set; } = new()
		{
			// Business entity seeders
			{ "Product", true },
			{ "Store", true },
			{ "Price", true },
			{ "Expense", false },
			{ "Task", false },
			{ "Notification", false },
			{ "Budget", false },
			
			// Identity seeders
			{ "Role", true },
			{ "User", true },
			{ "UserRole", true }
		};

	// ================================
	// VALIDATION CONFIGURATION
	// ================================

	/// <summary>
	/// If true, validation errors will stop the migration process.
	/// If false, invalid items will be skipped with warnings logged.
	/// </summary>
	public bool StrictValidation { get; set; } = true;

	/// <summary>
	/// If true, detailed validation logs will be written to console and log files
	/// </summary>
	public bool EnableValidationLogging { get; set; } = true;

	/// <summary>
	/// Maximum number of validation errors to log before stopping (prevents log spam)
	/// </summary>
	public int MaxValidationErrorsToLog { get; set; } = 50;

	// ================================
	// IDENTITY-SPECIFIC CONFIGURATION
	// ================================

	/// <summary>
	/// Enable comprehensive password validation for JSON user data
	/// When true, validates password strength according to PasswordRequirements
	/// When false, accepts any password from JSON (useful for bulk imports with temporary passwords)
	/// </summary>
	public bool EnablePasswordValidation { get; set; } = true;

	/// <summary>
	/// Password validation requirements for JSON user data
	/// </summary>
	public PasswordRequirements PasswordRequirements { get; set; } = new();

	/// <summary>
	/// Enable role hierarchy validation (if using ExtendedRoleJsonDto)
	/// </summary>
	public bool EnableRoleHierarchyValidation { get; set; } = false;

	/// <summary>
	/// Enable environment-specific seeding for Identity data
	/// </summary>
	public bool EnableEnvironmentAwareIdentitySeeding { get; set; } = true;

	/// <summary>
	/// Allow creation of temporary users (useful for migration scenarios)
	/// </summary>
	public bool AllowTemporaryUsers { get; set; } = false;

	/// <summary>
	/// Force email confirmation for all seeded users
	/// </summary>
	public bool ForceEmailConfirmation { get; set; } = true;

	/// <summary>
	/// Default user lock out settings for seeded users
	/// </summary>
	public UserLockoutSettings DefaultLockoutSettings { get; set; } = new();

	// ================================
	// BULK OPERATION CONFIGURATION
	// ================================

	/// <summary>
	/// Enable batch processing for large datasets
	/// </summary>
	public bool EnableBatchProcessing { get; set; } = false;

	/// <summary>
	/// Batch size for bulk operations (users/roles)
	/// </summary>
	public int BatchSize { get; set; } = 100;

	/// <summary>
	/// Enable parallel processing for validation (use with caution)
	/// </summary>
	public bool EnableParallelValidation { get; set; } = false;

	/// <summary>
	/// Maximum number of parallel validation threads
	/// </summary>
	public int MaxParallelThreads { get; set; } = Environment.ProcessorCount;

	// ================================
	// DEVELOPMENT AND TESTING
	// ================================

	/// <summary>
	/// Enable detailed performance logging for seeding operations
	/// </summary>
	public bool EnablePerformanceLogging { get; set; } = false;

	/// <summary>
	/// Enable validation caching (improves performance for large datasets)
	/// </summary>
	public bool EnableValidationCaching { get; set; } = true;

	/// <summary>
	/// Validate JSON schema before processing (additional safety layer)
	/// </summary>
	public bool EnableJsonSchemaValidation { get; set; } = false;
}