

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources
{
	public class DataSourceFactory : IDataSourceFactory
	{
		public IDataSource<T> CreateJsonDataSource<T>(string filePath) where T : class
		{
			return new JsonDataSource<T>(filePath);
		}
	}
}
