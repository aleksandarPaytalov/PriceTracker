using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Helpers
{
	/// <summary>
	/// Helper for reading data during migrations using existing data source infrastructure
	/// </summary>
	public static class MigrationDataHelper
	{
		private static readonly IDataSourceFactory _dataSourceFactory = new DataSourceFactory();

		/// <summary>
		/// Gets data from JSON file using existing infrastructure
		/// </summary>
		public static IEnumerable<T> GetDataFromJson<T>(string fileName) where T : class
		{
			try
			{
				var jsonPath = GetJsonFilePath(fileName);
				if (!File.Exists(jsonPath))
				{
					return Enumerable.Empty<T>();
				}

				var dataSource = _dataSourceFactory.CreateJsonDataSource<T>(jsonPath);
				return dataSource.LoadDataAsync().GetAwaiter().GetResult();
			}
			catch
			{
				return Enumerable.Empty<T>();
			}
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