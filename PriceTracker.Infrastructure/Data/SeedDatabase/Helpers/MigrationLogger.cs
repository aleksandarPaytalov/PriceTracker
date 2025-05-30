using PriceTracker.Infrastructure.Common;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Helpers
{
	/// <summary>
	/// Static logger helper for migration and configuration contexts
	/// Handles both runtime DI logger and migration-time fallback logger
	/// </summary>
	public static class MigrationLogger
	{
		private static IAppLogger? _runtimeLogger;
		private static IAppLogger? _migrationLogger;

		/// <summary>
		/// Sets the runtime logger (called from Program.cs during app startup)
		/// </summary>
		public static void SetRuntimeLogger(IAppLogger? logger)
		{
			_runtimeLogger = logger;
		}

		/// <summary>
		/// Gets appropriate logger - runtime logger if available, otherwise migration logger
		/// </summary>
		private static IAppLogger GetLogger()
		{
			// Try runtime logger first (when app is running)
			if (_runtimeLogger != null)
			{
				return _runtimeLogger;
			}

			// Create migration logger if needed (during add-migration)
			if (_migrationLogger == null)
			{
				var migrationLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "migration-logs");
				_migrationLogger = new FileLogger(migrationLogPath, logToConsole: true);
				_migrationLogger.LogInformation("Created migration logger for EF Tools context");
			}

			return _migrationLogger;
		}

		public static void LogInformation(string message)
		{
			GetLogger().LogInformation(message);
		}

		public static void LogWarning(string message)
		{
			GetLogger().LogWarning(message);
		}

		public static void LogError(string message, Exception? ex = null)
		{
			GetLogger().LogError(message, ex);
		}
	}
}