using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PriceTracker.Configuration;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.ProductSeederConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeeders
{
	/// <summary>
	/// Seeder for products
	/// </summary>
	public class ProductSeeder(
		IServiceProvider serviceProvider,
		IAppLogger logger,
		IRepository<Product> repository,
		IOptions<SeedingOptions> options,
		IDataProviderFactory<Product> dataProviderFactory)
		: BaseSeeder(serviceProvider, logger)
	{
		private readonly IDataProviderFactory<Product> _dataProviderFactory = dataProviderFactory;
		private readonly IRepository<Product> _repository = repository;
		private readonly SeedingOptions _options = options.Value;

		/// <summary>
		/// Products have high priority (lower the value is -> bigger the priority is)
		/// </summary>
		public override int Priority => 10;

		public override string SeederName => SeederDescription;

		/// <summary>
		/// Check if there are existing products in the database
		/// </summary>
		/// <returns>True if seeding should proceed, false otherwise</returns>
		protected override async Task<bool> ShouldSeedAsync()
		{
			_logger.LogInformation(CheckingExistingProducts);
			return !await _repository.All().AnyAsync();
		}

		/// <summary>
		/// Seeds product data from external JSON file
		/// </summary>
		protected override async Task SeedDataAsync()
		{
			_logger.LogInformation(FindingJsonFilePath);
			var jsonPath = GetJsonFilePath(
				typeof(PriceTrackerDbContext),
				_options.DataFolderPath,
				_options.ProductJsonFile);

			_logger.LogInformation(string.Format(LoadingProductsFromPath, jsonPath));

			_logger.LogInformation(UsingFactoryComment);
			var dataProvider = _dataProviderFactory.CreateDataProvider(jsonPath);

			_logger.LogInformation(LoadingData);
			var products = dataProvider.GetData();

			_logger.LogInformation(string.Format(FoundProductsToImport, products.Count()));

			foreach (var product in products)
			{
				await _repository.AddAsync(product);
			}

			var result = await _repository.SaveChangesAsync();
			_logger.LogInformation(string.Format(SuccessfullySeededProducts, result));
		}
	}
}