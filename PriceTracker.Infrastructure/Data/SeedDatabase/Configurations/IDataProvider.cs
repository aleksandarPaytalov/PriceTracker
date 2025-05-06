namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public interface IDataProvider<T> where T : class 
	{
		IEnumerable<T> GetData();
	}
}
