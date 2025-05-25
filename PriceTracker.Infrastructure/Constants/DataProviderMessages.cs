namespace PriceTracker.Infrastructure.Constants
{
	internal static class DataProviderMessages
	{
		internal static class BaseDataProviderMessages
		{
			// Information messages
			internal const string NoDataSourceProvided = "No data source provided for {0}";
			internal const string SuccessfullyLoadedRecords = "Successfully loaded {0} records for {1}";
			internal const string StartingToLoadData = "Starting to load data for {0}";
			internal const string FinishedLoadingData = "Finished loading data for {0}. Total count: {1}";

			// Warning messages
			internal const string DataFileNotFound = "Data file not found for {0}: {1}";

			// Error messages
			internal const string ErrorCheckingEntityExistence = "Error checking entity existence in {0}";
			internal const string ErrorLoadingData = "Error loading data for {0}";
			internal const string ErrorProcessingEntity = "Error processing entity in {0}: {1}";
			internal const string CriticalError = "Critical error in {0}.{1}";
		}

		internal static class BaseSeederMessages
		{
			// Information messages
			internal const string StartingSeedOperation = "Starting seed operation for {0}...";
			internal const string SuccessfullyCompletedSeeding = "Successfully completed seeding for {0}";
			internal const string SkippingSeedingDataExists = "Skipping seeding for {0} - data already exists";

			// Error messages
			internal const string ErrorWhileSeeding = "Error while seeding {0}: {1}";
			internal const string FailedToGetJsonFilePath = "Failed to get JSON file path for {0}";
		}

		internal static class SeederServiceConstants
		{
			// Information messages
			internal const string ExternalSeedingDisabled = "External seeding is disabled in configuration";
			internal const string StartingSeeding = "Starting database seeding from external sources...";
			internal const string FoundSeeders = "Found {0} seeders to execute";
			internal const string ExecutingSeeder = "Executing seeder: {0}";
			internal const string SkippingSeeder = "Skipping disabled seeder: {0}";
			internal const string SeedingCompleted = "Database seeding completed successfully";

			// Error messages
			internal const string SeedingError = "Error during database seeding";

			// Constants
			internal const string SeederSuffix = "Seeder";
		}

		internal static class ProductDataProviderConstants
		{
			// Information messages
			internal const string StartingExternalSource = "Starting to load products from external source";
			internal const string ProductAdded = "Added product: {0} ({1})";

			// Entity identification
			internal const string ProductIdentifier = "Name: {0}, Brand: {1}";
		}

		internal static class ProductSeederConstants
		{
			// Information messages
			internal const string SeederDescription = "Product Seeder";
			internal const string CheckingExistingProducts = "Checking if there are existing products in the database";
			internal const string FindingJsonFilePath = "Finding the path to the JSON file for product data";
			internal const string LoadingProductsFromPath = "Loading products from: {0}";
			internal const string UsingFactoryComment = "Using factory pattern for data provider creation";
			internal const string LoadingData = "Loading product data from external source";
			internal const string FoundProductsToImport = "Found {0} products to import";
			internal const string SuccessfullySeededProducts = "Successfully seeded {0} products";
		}

		internal static class StoreDataProviderConstants
		{
			internal const string StartingExternalSource = "Starting to load stores from external source";
			internal const string StoreAdded = "Added store: {0}";
			internal const string StoreIdentifier = "Name: {0}";
		}

		internal static class PriceDataProviderConstants
		{
			internal const string StartingExternalSource = "Starting to load prices from external source";
			internal const string PriceAdded = "Added price: Product={0}, Store={1}, Price={2:C2}";
			internal const string PriceIdentifier = "Product={0}, Store={1}, Date={2:yyyy-MM-dd}";
			internal const string LoadingRelatedData = "Loading related products and stores";
			internal const string NoRelatedDataFound = "No related {0} found with ID: {1}";
		}

		internal static class ExpenseDataProviderConstants
		{
			internal const string StartingExternalSource = "Starting to load expenses from external source";
			internal const string ExpenseAdded = "Added expense: User={0}, Product={1}, Amount={2:C2}";
			internal const string ExpenseIdentifier = "User={0}, Product={1}, Date={2:yyyy-MM-dd}";
			internal const string LoadingRelatedData = "Loading related users, products and stores";
		}

		internal static class ToDoItemDataProviderConstants
		{
			internal const string StartingExternalSource = "Starting to load tasks from external source";
			internal const string TaskAdded = "Added task: User={0}, Title={1}";
			internal const string TaskIdentifier = "Title={0}, User={1}";
			internal const string LoadingRelatedData = "Loading related users";
		}

		internal static class NotificationDataProviderConstants
		{
			internal const string StartingExternalSource = "Starting to load notifications from external source";
			internal const string NotificationAdded = "Added notification: User={0}, Task={1}";
			internal const string NotificationIdentifier = "User={0}, Task={1}, Time={2:yyyy-MM-dd HH:mm}";
			internal const string LoadingRelatedData = "Loading related users and tasks";
		}

		internal static class MonthlyBudgetDataProviderConstants
		{
			internal const string StartingExternalSource = "Starting to load budgets from external source";
			internal const string BudgetAdded = "Added budget: User={0}, Month={1}, Amount={2:C2}";
			internal const string BudgetIdentifier = "User={0}, Month={1}";
			internal const string LoadingRelatedData = "Loading related users";
		}

		internal static class UserDataProviderConstants
		{
			// Information messages
			internal const string StartingExternalSource = "Starting to load users from external source";
			internal const string UserAdded = "Added user: {0} ({1})";

			// Entity identification
			internal const string UserIdentifier = "UserName: {0}, Email: {1}";

			// Default users data
			internal const string AdminUserName = "admin";
			internal const string AdminEmail = "admin@pricetracker.com";
			internal const string AdminPassword = "Admin123#";

			internal const string TestUserName = "test";
			internal const string TestEmail = "test@pricetracker.com";
			internal const string TestPassword = "Test123#";
		}

	}
}
