using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.ProductDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	/// <summary>
	/// Provider responsible for loading and managing product data
	/// </summary>
	public class ProductDataProvider : BaseDataProvider<Product>
	{
		public ProductDataProvider(
			IRepository<Product> repository,
			IDataSource<Product>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
		}

		/// <summary>
		/// Main method to retrieve product data.
		/// Returns collection of products from external source or default data
		/// </summary>
		public override IEnumerable<Product> GetData()
		{
			var products = new List<Product>();

			try
			{
				// External source loading if available
				products.AddRange(LoadProductsFromExternalSource());

				_logger.LogInformation(
					string.Format(FinishedLoadingData,
						_typeName,
						products.Count));
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(GetData), ex);
			}

			return products;
		}

		/// <summary>
		/// Loads products from external source
		/// </summary>
		private IEnumerable<Product> LoadProductsFromExternalSource()
		{
			var products = new List<Product>();

			try
			{
				_logger.LogInformation(StartingExternalSource);

				var sourceProducts = LoadFromSourceAsync().Result;
				foreach (var productData in sourceProducts)
				{
					try
					{
						if (!ProductExists(productData))
						{
							var product = CreateProduct(
								productData.ProductName,
								productData.Brand,
								productData.Category,
								productData.Quantity);

							if (product != null)
							{
								products.Add(product);
								LogProductAdded(product);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatProductIdentifier(productData.ProductName, productData.Brand);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadProductsFromExternalSource), ex);
			}

			return products;
		}	

		/// <summary>
		/// Creates a new product instance using the ProductBuilder
		/// </summary>
		private Product? CreateProduct(string name, string brand, ProductCategory category, int quantity)
		{
			try
			{
				return new ProductBuilder(
					productName: name,
					brandName: brand,
					category: category,
					quantity: quantity)
					.Build();
			}
			catch (Exception ex)
			{
				var identifier = FormatProductIdentifier(name, brand);
				LogProcessingError(identifier, ex);
				return null;
			}
		}

		/// <summary>
		/// Checks if a product already exists in the database
		/// </summary>
		private bool ProductExists(Product product)
		{
			return ProductExists(product.ProductName, product.Brand);
		}

		/// <summary>
		/// Checks if a product already exists based on name and brand
		/// </summary>
		private bool ProductExists(string name, string brand)
		{
			return EntityExists(p =>
				p.ProductName == name &&
				p.Brand == brand);
		}

		/// <summary>
		/// Logs the addition of a new product to the system
		/// </summary>
		private void LogProductAdded(Product product)
		{
			var message = string.Format(
				ProductAdded,
				product.ProductName,
				product.Brand);

			_logger.LogInformation(message);
		}

		/// <summary>
		/// Creates a formatted identifier string for a product
		/// </summary>
		private static string FormatProductIdentifier(string name, string brand)
		{
			return string.Format(ProductIdentifier, name, brand);
		}
	}
}