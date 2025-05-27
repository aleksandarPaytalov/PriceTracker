using System.Text.Json;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataSources
{
	/// <summary>
	/// Implementation of IDataSource that loads data from JSON files
	/// </summary>
	public class JsonDataSource<T> : IDataSource<T> where T : class
	{
		private readonly string _filePath;
		private readonly JsonSerializerOptions _options;

		public JsonDataSource(string filePath)
		{
			_filePath = filePath;
			_options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
		}

		/// <summary>
		/// Loads and deserializes data from a JSON file asynchronously
		/// </summary>
		/// <returns>Collection of deserialized objects</returns>
		public async Task<IEnumerable<T>> LoadDataAsync()
		{
			if (!File.Exists(_filePath))
			{
				throw new FileNotFoundException($"Data file not found: {_filePath}");
			}

			var jsonString = await File.ReadAllTextAsync(_filePath);
			return JsonSerializer.Deserialize<IEnumerable<T>>(jsonString, _options)
				   ?? Enumerable.Empty<T>(); ;
		}
	}
}
