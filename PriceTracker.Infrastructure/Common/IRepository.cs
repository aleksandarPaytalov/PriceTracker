namespace PriceTracker.Infrastructure.Common
{
	public interface IRepository<T> where T : class
	{

		IQueryable<T> All();

		IQueryable<T> AllReadOnly();

		Task<T?> GetbyIdAsync(object id);

		Task AddAsync(T entity);

		void Delete(T entity);

		Task<int> SaveChangesAsync();
	}
}
