namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources
{
	public interface IDataSource<T> where T : class
	{
		Task<IEnumerable<T>> LoadDataAsync();
	}
}
