namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	public interface IDataProvider<T> where T : class 
	{
		IEnumerable<T> GetData();
	}
}
