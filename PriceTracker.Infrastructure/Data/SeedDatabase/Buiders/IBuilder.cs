namespace PriceTracker.Infrastructure.Data.SeedDatabase.Buiders
{
	public interface IBuilder<T> where T : class
	{
		T Build();
	}
}
