namespace PriceTracker.Configuration
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
			{ "Store", false },
			{ "Price", false },
			{ "Expense", false },
			{ "Task", false },
			{ "Notification", false },
			{ "Budget", false }
		};
	}
}