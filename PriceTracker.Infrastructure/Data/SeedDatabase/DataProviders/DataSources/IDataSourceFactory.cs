namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources
{
	public interface IDataSourceFactory
	{
		IDataSource<T> CreateJsonDataSource<T>(string filePath) where T : class;
	}
}