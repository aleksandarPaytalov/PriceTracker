namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	public interface IDataProviderFactory<T> where T : class
	{
		IDataProvider<T> CreateDataProvider(string filePath);
	}
}
