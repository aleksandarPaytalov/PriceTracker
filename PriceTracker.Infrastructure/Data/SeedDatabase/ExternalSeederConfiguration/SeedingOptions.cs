namespace PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration
{
	public class SeedingOptions
	{
		public bool UseExternalSource { get; set; }
		public string DataFolderPath { get; set; } = string.Empty;

		public string ProductJsonFile { get; set; } = "products.json";
		public string StoreJsonFile { get; set; } = "stores.json";
		public string PriceJsonFile { get; set; } = "prices.json";
		public string ExpenseJsonFile { get; set; } = "expenses.json";
		public string TaskJsonFile { get; set; } = "tasks.json";
		public string NotificationJsonFile { get; set; } = "notifications.json";
		public string BudgetJsonFile { get; set; } = "budgets.json";

		public Dictionary<string, bool> EnabledSeeders { get; set; } = new()
		{
			{ "Product", true },
			{ "Store", true },
			{ "Price", true },
			{ "Expense", false },
			{ "Task", false },
			{ "Notification", false },
			{ "Budget", false }
		};

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
	}
}