using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	public class ProductDataProviderFactory : IDataProviderFactory<Product>
	{
		private readonly IRepository<Product> _repository;
		private readonly IDataSourceFactory _dataSourceFactory;
		private readonly IAppLogger _logger;

		public ProductDataProviderFactory(
			IRepository<Product> repository,
			IDataSourceFactory dataSourceFactory,
			IAppLogger logger)
		{
			_repository = repository;
			_dataSourceFactory = dataSourceFactory;
			_logger = logger;
		}

		public IDataProvider<Product> CreateDataProvider(string filePath)
		{
			var dataSource = _dataSourceFactory.CreateJsonDataSource<Product>(filePath);
			return new ProductDataProvider(_repository, dataSource, _logger);
		}
	}
}
