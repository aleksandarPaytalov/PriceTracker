using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.PriceConfigurationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.PriceConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class PriceConfiguration : IEntityTypeConfiguration<Price>
	{
		private readonly IOptions<SeedingOptions> _options;

		public PriceConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public void Configure(EntityTypeBuilder<Price> builder)
		{
			// Unique composite index
			builder.HasIndex(p => new { p.ProductId, p.StoreId, p.DateChecked })
				   .IsUnique();

			// Configuration of the relations 
			// We keep the history of the prices
			builder.HasOne(p => p.Product)
				   .WithMany(pr => pr.Prices)
				   .HasForeignKey(p => p.ProductId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(p => p.Store)
				   .WithMany(s => s.Prices)
				   .HasForeignKey(p => p.StoreId)
				   .OnDelete(DeleteBehavior.Restrict);

			// Seeding data 
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Price", false))
			{
				var validatedPrices = LoadAndValidatePricesFromJson();

				if (validatedPrices.Any())
				{
					builder.HasData(validatedPrices);
					MigrationLogger.LogInformation(string.Format(LoadedPricesFromJson, validatedPrices.Count()));
				}
				else
				{
					var errorMessage = string.Format(ExternalSourceEnabledButNoData, "prices.json");
					MigrationLogger.LogError(errorMessage);
					throw new InvalidOperationException(errorMessage);
				}
			}
			else
			{
				// Use default seed data only if Price seeding is not disabled
				if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("Price", true))
				{
					SeedDefaultData(builder);
					MigrationLogger.LogInformation(UsingDefaultSeedData);
				}
			}
		}

		/// <summary>
		/// Loads prices from JSON and validates them using PriceBuilder
		/// </summary>
		private IEnumerable<Price> LoadAndValidatePricesFromJson()
		{
			try
			{
				// Clear tracking for new seeding session
				PriceBuilder.ResetTracking();

				// Load JSON prices directly as Price objects
				var jsonPrices = MigrationDataHelper.GetDataFromJson<Price>("prices.json");

				if (!jsonPrices.Any())
				{
					MigrationLogger.LogWarning(NoPricesFoundInJson);
					return Enumerable.Empty<Price>();
				}

				// We need to get products and stores to validate foreign keys
				var products = GetExistingProducts();
				var stores = GetExistingStores();

				// Validate using PriceBuilder - returns only valid items
				var validatedPrices = MigrationDataHelper.ValidateItems(
					jsonPrices,
					price => ValidatePriceWithBuilder(price, products, stores),
					"price",
					_options.Value.StrictValidation);

				return validatedPrices;
			}
			catch (Exception ex) when (!(ex is ValidationException))
			{
				MigrationLogger.LogError(string.Format(FailedToLoadPricesFromJson, ex.Message), ex);
				throw new InvalidOperationException(string.Format(PriceLoadingFailed, ex.Message), ex);
			}
		}

		/// <summary>
		/// Validates a Price object using PriceBuilder validation logic
		/// </summary>
		private static Price ValidatePriceWithBuilder(
			Price price,
			IEnumerable<Product> products,
			IEnumerable<Store> stores)
		{
			// Validate PriceId first
			if (price.PriceId <= 0)
			{
				throw new ValidationException(string.Format(InvalidPriceId, price.PriceId));
			}

			// Find the referenced product
			var product = products.FirstOrDefault(p => p.ProductId == price.ProductId);
			if (product == null)
			{
				throw new ValidationException(string.Format(ProductNotFound, price.ProductId));
			}

			// Find the referenced store
			var store = stores.FirstOrDefault(s => s.StoreId == price.StoreId);
			if (store == null)
			{
				throw new ValidationException(string.Format(StoreNotFound, price.StoreId));
			}

			// Use PriceBuilder for validation
			var priceBuilder = new PriceBuilder(
				product,
				store,
				price.SellingPrice,
				price.DateChecked
			);

			var validatedPrice = priceBuilder.Build();
			validatedPrice.PriceId = price.PriceId;

			return validatedPrice;
		}

		/// <summary>
		/// Seeds default data from SeedData class using PriceBuilder validation
		/// </summary>
		private static void SeedDefaultData(EntityTypeBuilder<Price> builder)
		{
			try
			{
				PriceBuilder.ResetTracking();

				var data = new SeedData();
				data.Initialize();

				// Get products and stores for validation
				var products = new[]
				{
					data.Product1, data.Product2, data.Product3, data.Product4, data.Product5,
					data.Product6, data.Product7, data.Product8, data.Product9
				};

				var stores = new[]
				{
					data.Store1, data.Store2, data.Store3, data.Store4,
					data.Store5, data.Store6, data.Store7, data.Store8
				};

				var defaultPrices = new[]
				{
					data.Price1, data.Price2, data.Price3, data.Price4, data.Price5,
					data.Price6, data.Price7, data.Price8, data.Price9
				};

				// Validate default prices - should never fail
				var validatedPrices = MigrationDataHelper.ValidateItems(
					defaultPrices,
					price => ValidatePriceWithBuilder(price, products, stores),
					"default price",
					strictValidation: true);

				builder.HasData(validatedPrices);
			}
			catch (Exception ex)
			{
				MigrationLogger.LogError(string.Format(FailedToSeedDefaultData, ex.Message), ex);
				throw;
			}
		}

		/// <summary>
		/// Gets existing products for foreign key validation
		/// This method simulates getting products that would be available during seeding
		/// </summary>
		private IEnumerable<Product> GetExistingProducts()
		{
			// During migration/seeding, we need to get products from the same source
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Product", false))
			{
				// Load products from JSON
				return MigrationDataHelper.GetDataFromJson<Product>("products.json");
			}
			else
			{
				// Use default seed products
				var data = new SeedData();
				data.Initialize();

				return new[]
				{
					data.Product1, data.Product2, data.Product3, data.Product4, data.Product5,
					data.Product6, data.Product7, data.Product8, data.Product9
				};
			}
		}

		/// <summary>
		/// Gets existing stores for foreign key validation
		/// This method simulates getting stores that would be available during seeding
		/// </summary>
		private IEnumerable<Store> GetExistingStores()
		{
			// During migration/seeding, we need to get stores from the same source
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Store", false))
			{
				// Load stores from JSON
				return MigrationDataHelper.GetDataFromJson<Store>("stores.json");
			}
			else
			{
				// Use default seed stores
				var data = new SeedData();
				data.Initialize();

				return new[]
				{
					data.Store1, data.Store2, data.Store3, data.Store4,
					data.Store5, data.Store6, data.Store7, data.Store8
				};
			}
		}
	}
}