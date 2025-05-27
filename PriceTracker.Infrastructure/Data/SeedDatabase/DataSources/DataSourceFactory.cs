using PriceTracker.Infrastructure.Data.SeedDatabase.DataSources;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataSources
{
	public class DataSourceFactory : IDataSourceFactory
	{
		public IDataSource<T> CreateJsonDataSource<T>(string filePath) where T : class
		{
			return new JsonDataSource<T>(filePath);
		}
	}
}
