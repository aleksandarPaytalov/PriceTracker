using Microsoft.EntityFrameworkCore;

namespace PriceTracker.Infrastructure.Common
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly PriceTrackerDbContext _context;
		private readonly DbSet<T> _dbSet;

		public Repository(PriceTrackerDbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}
		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public IQueryable<T> All()
		{
			return _dbSet;
		}

		public IQueryable<T> AllReadOnly()
		{
			return _dbSet.AsNoTracking();
		}

		public void Delete(T entity)
		{
			_dbSet.Remove(entity);
		}

		public async Task<T?> GetbyIdAsync(object id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
