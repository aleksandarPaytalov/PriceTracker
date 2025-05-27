namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataSources
{
	public interface IDataSourceFactory
	{
		IDataSource<T> CreateJsonDataSource<T>(string filePath) where T : class;
	}
}