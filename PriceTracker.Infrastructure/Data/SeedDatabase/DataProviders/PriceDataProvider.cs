using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.PriceDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	/// <summary>
	/// Provider responsible for loading and managing price data
	/// </summary>
	public class PriceDataProvider : BaseDataProvider<Price>
	{
		private readonly IRepository<Product> _productRepository;
		private readonly IRepository<Store> _storeRepository;
		private readonly Random _random;

		public PriceDataProvider(
			IRepository<Price> repository,
			IRepository<Product> productRepository,
			IRepository<Store> storeRepository,
			IDataSource<Price>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
			_productRepository = productRepository;
			_storeRepository = storeRepository;
			_random = new Random();
		}

		/// <summary>
		/// Main method to retrieve price data.
		/// Returns collection of prices from external source or default data
		/// </summary>
		public override IEnumerable<Price> GetData()
		{
			var prices = new List<Price>();

			try
			{
				if (_dataSource != null)
				{
					prices.AddRange(LoadPricesFromExternalSource());
				}
				else
				{
					prices.AddRange(LoadDefaultPrices());
				}

				_logger.LogInformation(
					string.Format(FinishedLoadingData,
						_typeName,
						prices.Count));
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(GetData), ex);
			}

			return prices;
		}

		/// <summary>
		/// Loads prices from external source
		/// </summary>
		private IEnumerable<Price> LoadPricesFromExternalSource()
		{
			var prices = new List<Price>();

			try
			{
				_logger.LogInformation(StartingExternalSource);

				var sourcePrices = LoadFromSourceAsync().Result;
				var (products, stores) = LoadRelatedData();

				foreach (var priceData in sourcePrices)
				{
					try
					{
						if (!PriceExists(priceData))
						{
							var price = CreatePrice(priceData, products, stores);
							if (price != null)
							{
								prices.Add(price);
								LogPriceAdded(price, isDefault: false);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatPriceIdentifier(priceData);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadPricesFromExternalSource), ex);
			}

			return prices;
		}

		/// <summary>
		/// Creates default prices when no external source is available
		/// </summary>
		private IEnumerable<Price> LoadDefaultPrices()
		{
			var prices = new List<Price>();

			try
			{
				_logger.LogInformation(LoadingDefaultData);

				var (products, stores) = LoadRelatedData();

				// Generate prices for each product in each store
				foreach (var product in products)
				{
					foreach (var store in stores)
					{
						try
						{
							var defaultPrice = GenerateDefaultPrice(product, store);
							if (!PriceExists(defaultPrice))
							{
								var price = CreatePrice(defaultPrice, products, stores);
								if (price != null)
								{
									prices.Add(price);
									LogPriceAdded(price, isDefault: true);
								}
							}
						}
						catch (Exception ex)
						{
							var identifier = FormatPriceIdentifier(product.ProductId, store.StoreId, DateTime.Today);
							LogProcessingError(identifier, ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadDefaultPrices), ex);
			}

			return prices;
		}

		/// <summary>
		/// Loads related products and stores data
		/// </summary>
		private (List<Product> products, List<Store> stores) LoadRelatedData()
		{
			_logger.LogInformation(LoadingRelatedData);

			var products = _productRepository.AllReadOnly().ToList();
			var stores = _storeRepository.AllReadOnly().ToList();

			return (products, stores);
		}

		/// <summary>
		/// Creates a new Price instance using the builder pattern
		/// </summary>
		private Price? CreatePrice(Price priceData, List<Product> products, List<Store> stores)
		{
			try
			{
				var product = products.FirstOrDefault(p => p.ProductId == priceData.ProductId);
				var store = stores.FirstOrDefault(s => s.StoreId == priceData.StoreId);

				if (product == null)
				{
					_logger.LogWarning(string.Format(NoRelatedDataFound, "product", priceData.ProductId));
					return null;
				}

				if (store == null)
				{
					_logger.LogWarning(string.Format(NoRelatedDataFound, "store", priceData.StoreId));
					return null;	
				}

				return new PriceBuilder(
					product: product,
					store: store,
					sellingPrice: priceData.SellingPrice,
					dateChecked: priceData.DateChecked)
					.Build();
			}
			catch (Exception ex)
			{
				var identifier = FormatPriceIdentifier(priceData);
				LogProcessingError(identifier, ex);
				return null;
			}
		}

		/// <summary>
		/// Generates a default price with random amount for testing purposes
		/// </summary>
		private Price GenerateDefaultPrice(Product product, Store store)
		{
			// Generate random price in range 1 to 1000 lv.
			var randomPrice = _random.Next(100, 100000) / 100.0m;

			return new Price
			{
				ProductId = product.ProductId,
				Product = product,
				StoreId = store.StoreId,
				Store = store,
				SellingPrice = randomPrice,
				DateChecked = DateTime.Today
			};
		}

		/// <summary>
		/// Checks if a price record already exists in the database
		/// </summary>
		private bool PriceExists(Price price)
		{
			return EntityExists(p =>
				p.ProductId == price.ProductId &&
				p.StoreId == price.StoreId &&
				p.DateChecked == price.DateChecked);
		}

		/// <summary>
		/// Logs the addition of a new price record to the system
		/// </summary>
		private void LogPriceAdded(Price price, bool isDefault)
		{
			var message = string.Format(
				isDefault ? DefaultPriceAdded
						 : PriceAdded,
				price.Product?.ProductName ?? price.ProductId.ToString(),
				price.Store?.Name ?? price.StoreId.ToString(),
				price.SellingPrice);

			_logger.LogInformation(message);
		}

		/// <summary>
		/// Creates a formatted identifier for a price entity
		/// </summary>
		private string FormatPriceIdentifier(Price price)
		{
			return FormatPriceIdentifier(
				price.ProductId,
				price.StoreId,
				price.DateChecked ?? DateTime.Today);
		}

		/// <summary>
		/// Creates a formatted identifier using individual price components
		/// </summary>
		private string FormatPriceIdentifier(int productId, int storeId, DateTime date)
		{
			return string.Format(
				PriceIdentifier,
				productId,
				storeId,
				date);
		}
	}
}