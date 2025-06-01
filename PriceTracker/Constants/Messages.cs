namespace PriceTracker.Constants
{
	/// <summary>
	/// Constants for console messages and error messages
	/// </summary>
	public static class Messages
	{
		// Configuration messages
		public const string ExternalSeedingEnabled = "✅ External seeding source enabled with validation";
		public const string DefaultSeedDataUsed = "✅ Using default seed data (external source disabled)";
		public const string AllJsonFilesValidationPassed = "✅ All JSON files validation passed";

		// Configuration details templates
		public const string StrictValidationTemplate = "   - Strict validation: {0}";
		public const string ValidationLoggingTemplate = "   - Validation logging: {0}";
		public const string MaxErrorsToLogTemplate = "   - Max errors to log: {0}";
		public const string EnabledSeedersTemplate = "   - Enabled seeders: {0}";

		// File validation messages
		public const string FileNameTemplate = "🔍 FileName for {0}: {1}";
		public const string FullFilePathTemplate = "🔍 Full file path: {0}";
		public const string FileExistsTemplate = "🔍 File exists: {0}";
		public const string FileSizeTemplate = "🔍 File size: {0} bytes";
		public const string ContentPreviewTemplate = "🔍 Content preview: {0}...";
		public const string FileOkTemplate = "✅ File OK: {0} ({1} bytes)";

		// Warning and error messages
		public const string FileMissingTemplate = "❌ FILE MISSING: {0}";
		public const string FileEmptyTemplate = "❌ FILE EMPTY: {0}";
		public const string NoFilenameMappedTemplate = "⚠️ No filename mapped for seeder: {0}";
		public const string MissingFilesDetectedTemplate = "❌ Missing files detected: {0}";
		public const string MissingFileItemTemplate = "   - {0}";

		// Exception messages
		public const string ConnectionStringNotFound = "Connection string not found";
		public const string RequiredJsonFilesMissing = "Required JSON files are missing or empty:";
		public const string JsonFilesErrorSuffix = "Either provide the missing files or disable the corresponding seeders in EnabledSeeders configuration.";

		// Application startup messages
		public const string ApplicationStartupMessage = "PriceTracker application starting up with MigrationLogger support enabled";
	}

}
