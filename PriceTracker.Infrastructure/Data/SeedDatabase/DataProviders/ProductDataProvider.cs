using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.ProductDataProviderConstants;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	public class ProductDataProvider : BaseDataProvider<Product>
	{
		public ProductDataProvider(
			IRepository<Product> repository,
			IDataSource<Product>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
		}

		public override IEnumerable<Product> GetData()
		{
			var products = new List<Product>();

			try
			{
				// External source loading if available
				if (_dataSource != null)
				{
					products.AddRange(LoadProductsFromExternalSource());
				}
				else
				{
					// Loading of Default data if External source is not available  
					products.AddRange(LoadDefaultProducts());
				}

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
								LogProductAdded(product, isDefault: false);
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

		private IEnumerable<Product> LoadDefaultProducts()
		{
			var products = new List<Product>();

			try
			{
				_logger.LogInformation(LoadingDefaultData);

				foreach (var defaultProduct in GetDefaultProductData())
				{
					try
					{
						if (!ProductExists(defaultProduct.name, defaultProduct.brand))
						{
							var product = CreateProduct(
								defaultProduct.name,
								defaultProduct.brand,
								defaultProduct.category,
								defaultProduct.quantity);

							if (product != null)
							{
								products.Add(product);
								LogProductAdded(product, isDefault: true);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatProductIdentifier(defaultProduct.name, defaultProduct.brand);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadDefaultProducts), ex);
			}

			return products;
		}

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

		private bool ProductExists(Product product)
		{
			return ProductExists(product.ProductName, product.Brand);
		}

		private bool ProductExists(string name, string brand)
		{
			return EntityExists(p =>
				p.ProductName == name &&
				p.Brand == brand);
		}

		private void LogProductAdded(Product product, bool isDefault)
		{
			var message = string.Format(
				isDefault ? DefaultProductAdded
						 : ProductAdded,
				product.ProductName,
				product.Brand);

			_logger.LogInformation(message);
		}

		private string FormatProductIdentifier(string name, string brand)
		{
			return string.Format(ProductIdentifier, name, brand);
		}

		private IEnumerable<(string name, string brand, ProductCategory category, int quantity)>
			GetDefaultProductData()
		{
			return new[]
			{
				("Milk 3%", "Vereia", ProductCategory.Food, 1),
				("White Bread", "Sofia Mel", ProductCategory.Food, 1),
				("Coca Cola 2L", "Coca Cola", ProductCategory.Food, 1),
				("Laptop XPS 13", "Dell", ProductCategory.Electronics, 1),
				("iPhone 15", "Apple", ProductCategory.Electronics, 1),
				("Running Shoes", "Nike", ProductCategory.Clothing, 1)
			};
		}
	}
}