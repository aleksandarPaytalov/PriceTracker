namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataSources
{
	public interface IDataSource<T> where T : class
	{
		Task<IEnumerable<T>> LoadDataAsync();
	}
}
