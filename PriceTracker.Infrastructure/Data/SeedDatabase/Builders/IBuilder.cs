namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	public interface IBuilder<T> where T : class
	{
		T Build();
	}
}
