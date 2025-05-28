using PriceTracker.Infrastructure.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Helpers
{
	/// <summary>
	/// Helper for reading and validating data during migrations
	/// </summary>
	public static class MigrationDataHelper
	{
		private static IAppLogger? _logger;
		private static readonly JsonSerializerOptions _jsonOptions = new()
		{
			PropertyNameCaseInsensitive = true
		};

		/// <summary>
		/// Sets the logger instance for validation logging
		/// </summary>
		public static void SetLogger(IAppLogger? logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Gets data from JSON file synchronously - appropriate for EF Configuration
		/// </summary>
		/// <typeparam name="T">The type to deserialize to</typeparam>
		/// <param name="fileName">JSON file name</param>
		/// <returns>Collection of deserialized objects</returns>
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

				var jsonString = File.ReadAllText(jsonPath);

				var result = JsonSerializer.Deserialize<IEnumerable<T>>(jsonString, _jsonOptions)
						   ?? Enumerable.Empty<T>();

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
		/// Returns only valid items, logs errors for invalid ones
		/// </summary>
		public static IEnumerable<T> ValidateItems<TDto, T>(
			IEnumerable<TDto> items,
			Func<TDto, T> builderFunction,
			string itemTypeName = "item",
			bool strictValidation = false)
			where T : class
		{
			var validItems = new List<T>();
			var errorCount = 0;
			var processedCount = 0;

			foreach (var item in items)
			{
				processedCount++;
				try
				{
					var validatedItem = builderFunction(item);
					validItems.Add(validatedItem);
				}
				catch (ValidationException ex)
				{
					errorCount++;
					_logger?.LogError($"Validation failed for {itemTypeName} #{processedCount}: {ex.Message}");

					if (strictValidation)
					{
						throw new ValidationException($"Strict validation failed for {itemTypeName} #{processedCount}: {ex.Message}", ex);
					}
				}
				catch (Exception ex)
				{
					errorCount++;
					_logger?.LogError($"Unexpected error validating {itemTypeName} #{processedCount}: {ex.Message}", ex);

					if (strictValidation)
					{
						throw new InvalidOperationException($"Validation process failed for {itemTypeName} #{processedCount}: {ex.Message}", ex);
					}
				}
			}

			if (errorCount > 0)
			{
				_logger?.LogWarning($"Validation completed for {itemTypeName}: {validItems.Count} valid, {errorCount} invalid out of {processedCount} total");
			}
			else
			{
				_logger?.LogInformation($"All {processedCount} {itemTypeName}s validated successfully");
			}

			return validItems;
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
}