namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	public interface IDataSource<T> where T : class
	{
		Task<IEnumerable<T>> LoadDataAsync();
	}
}
