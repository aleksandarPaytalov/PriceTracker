using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataSources;
using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Helpers
{
	/// <summary>
	/// Enhanced helper for reading and validating data during migrations
	/// </summary>
	public static class MigrationDataHelper
	{
		private static readonly IDataSourceFactory _dataSourceFactory = new DataSourceFactory();
		private static IAppLogger? _logger;

		/// <summary>
		/// Sets the logger instance for validation logging
		/// </summary>
		public static void SetLogger(IAppLogger? logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Gets data from JSON file using existing infrastructure (Synchronous version for EF Configuration)
		/// </summary>
		public static IEnumerable<T> GetDataFromJson<T>(string fileName) where T : class
		{
			try
			{
				var jsonPath = GetJsonFilePath(fileName);
				if (!File.Exists(jsonPath))
				{
					_logger?.LogWarning($"JSON file not found: {jsonPath}");
					return Enumerable.Empty<T>();
				}

				var dataSource = _dataSourceFactory.CreateJsonDataSource<T>(jsonPath);
				var result = dataSource.LoadDataAsync().GetAwaiter().GetResult();

				_logger?.LogInformation($"Successfully loaded {result.Count()} items from {fileName}");
				return result;
			}
			catch (Exception ex)
			{
				_logger?.LogError($"Failed to load data from {fileName}: {ex.Message}", ex);
				return Enumerable.Empty<T>();
			}
		}

		/// <summary>
		/// Gets data from JSON file asynchronously with validation support
		/// </summary>
		public static async Task<IEnumerable<T>> GetDataFromJsonAsync<T>(string fileName) where T : class
		{
			try
			{
				var jsonPath = GetJsonFilePath(fileName);
				if (!File.Exists(jsonPath))
				{
					_logger?.LogWarning($"JSON file not found: {jsonPath}");
					return Enumerable.Empty<T>();
				}

				var dataSource = _dataSourceFactory.CreateJsonDataSource<T>(jsonPath);
				var result = await dataSource.LoadDataAsync();

				_logger?.LogInformation($"Successfully loaded {result.Count()} items from {fileName}");
				return result;
			}
			catch (Exception ex)
			{
				_logger?.LogError($"Failed to load data from {fileName}: {ex.Message}", ex);
				return Enumerable.Empty<T>();
			}
		}

		/// <summary>
		/// Validates a collection of items using a builder function
		/// </summary>
		public static ValidationResult<T> ValidateItems<TDto, T>(
			IEnumerable<TDto> items,
			Func<TDto, T> builderFunction,
			string itemTypeName = "item")
			where T : class
		{
			var validItems = new List<T>();
			var validationErrors = new List<ValidationError>();
			var processedCount = 0;

			foreach (var item in items)
			{
				processedCount++;
				try
				{
					var validatedItem = builderFunction(item);
					validItems.Add(validatedItem);

					_logger?.LogInformation($"Successfully validated {itemTypeName} #{processedCount}");
				}
				catch (ValidationException ex)
				{
					var error = new ValidationError
					{
						ItemIndex = processedCount,
						ErrorMessage = ex.Message,
						ItemData = item?.ToString() ?? "null"
					};

					validationErrors.Add(error);
					_logger?.LogError($"Validation failed for {itemTypeName} #{processedCount}: {ex.Message}");
				}
				catch (Exception ex)
				{
					var error = new ValidationError
					{
						ItemIndex = processedCount,
						ErrorMessage = $"Unexpected error: {ex.Message}",
						ItemData = item?.ToString() ?? "null"
					};

					validationErrors.Add(error);
					_logger?.LogError($"Unexpected error validating {itemTypeName} #{processedCount}: {ex.Message}", ex);
				}
			}

			var result = new ValidationResult<T>
			{
				ValidItems = validItems,
				ValidationErrors = validationErrors,
				TotalProcessed = processedCount,
				ValidCount = validItems.Count,
				InvalidCount = validationErrors.Count
			};

			_logger?.LogInformation($"Validation summary for {itemTypeName}: {result.ValidCount} valid, {result.InvalidCount} invalid out of {result.TotalProcessed} total");

			return result;
		}

		/// <summary>
		/// Checks if JSON file exists for external source usage
		/// </summary>
		public static bool HasJsonData(string fileName)
		{
			var jsonPath = GetJsonFilePath(fileName);
			return File.Exists(jsonPath);
		}

		/// <summary>
		/// Gets the full path to JSON file
		/// </summary>
		private static string GetJsonFilePath(string fileName)
		{
			var basePath = AppDomain.CurrentDomain.BaseDirectory;
			return Path.Combine(basePath, "Data", "SeedDatabase", "JsonData", fileName);
		}
	}

	/// <summary>
	/// Result of validation operation
	/// </summary>
	public class ValidationResult<T> where T : class
	{
		public IEnumerable<T> ValidItems { get; set; } = Enumerable.Empty<T>();
		public IEnumerable<ValidationError> ValidationErrors { get; set; } = Enumerable.Empty<ValidationError>();
		public int TotalProcessed { get; set; }
		public int ValidCount { get; set; }
		public int InvalidCount { get; set; }

		public bool HasErrors => ValidationErrors.Any();
		public bool IsFullyValid => InvalidCount == 0;
		public double ValidationSuccessRate => TotalProcessed > 0 ? (double)ValidCount / TotalProcessed * 100 : 0;
	}

	/// <summary>
	/// Information about a validation error
	/// </summary>
	public class ValidationError
	{
		public int ItemIndex { get; set; }
		public string ErrorMessage { get; set; } = string.Empty;
		public string ItemData { get; set; } = string.Empty;
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	}
}