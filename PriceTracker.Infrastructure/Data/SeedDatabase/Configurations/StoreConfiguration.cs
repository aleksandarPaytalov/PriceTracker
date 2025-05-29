using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.StoreConfigurationConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class StoreConfiguration : IEntityTypeConfiguration<Store>
	{
		private readonly IOptions<SeedingOptions> _options;

		public StoreConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public void Configure(EntityTypeBuilder<Store> builder)
		{
			// Unique index for the Store name
			builder.HasIndex(s => s.Name).IsUnique();

			// Configuration of the relations
			builder.HasMany(s => s.Prices)
				   .WithOne(p => p.Store)
				   .HasForeignKey(p => p.StoreId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(s => s.Expenses)
				   .WithOne(e => e.Store)
				   .HasForeignKey(e => e.StoreId)
				   .OnDelete(DeleteBehavior.Restrict);

			// Seeding data 
			if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Store", false))
			{
				var validatedStores = LoadAndValidateStoresFromJson();

				if (validatedStores.Any())
				{
					builder.HasData(validatedStores);
					MigrationLogger.LogInformation(string.Format(LoadedStoresFromJson, validatedStores.Count()));
				}
				else
				{
					var errorMessage = string.Format(ExternalSourceEnabledButNoData, "stores.json");
					MigrationLogger.LogError(errorMessage);
					throw new InvalidOperationException(errorMessage);
				}
			}
			else
			{
				// Use default seed data only if Store seeding is not disabled
				if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("Store", true))
				{
					SeedDefaultData(builder);
					MigrationLogger.LogInformation(UsingDefaultSeedData);
				}
			}
		}

		/// <summary>
		/// Loads stores from JSON and validates them using StoreBuilder
		/// </summary>
		private IEnumerable<Store> LoadAndValidateStoresFromJson()
		{
			try
			{
				// Clear tracking for new seeding session
				StoreBuilder.ResetTracking();

				// Load JSON stores directly as Store objects
				var jsonStores = MigrationDataHelper.GetDataFromJson<Store>("stores.json");

				if (!jsonStores.Any())
				{
					MigrationLogger.LogWarning(NoStoresFoundInJson);
					return Enumerable.Empty<Store>();
				}

				// Validate using StoreBuilder - returns only valid items
				var validatedStores = MigrationDataHelper.ValidateItems(
					jsonStores,
					ValidateStoreWithBuilder,
					"store",
					_options.Value.StrictValidation);

				return validatedStores;
			}
			catch (Exception ex) when (!(ex is ValidationException))
			{
				MigrationLogger.LogError(string.Format(FailedToLoadStoresFromJson, ex.Message), ex);
				throw new InvalidOperationException(string.Format(StoreLoadingFailed, ex.Message), ex);
			}
		}

		/// <summary>
		/// Validates a Store object using StoreBuilder validation logic
		/// </summary>
		private static Store ValidateStoreWithBuilder(Store store)
		{
			// Validate StoreId first
			if (store.StoreId <= 0)
			{
				throw new ValidationException(string.Format(InvalidStoreId, store.StoreId));
			}

			// Use StoreBuilder for validation (without repository for seeding)
			var storeBuilder = new StoreBuilder(store.Name);

			var validatedStore = storeBuilder.Build();
			validatedStore.StoreId = store.StoreId;

			return validatedStore;
		}

		/// <summary>
		/// Seeds default data from SeedData class using StoreBuilder validation
		/// </summary>
		private static void SeedDefaultData(EntityTypeBuilder<Store> builder)
		{
			try
			{
				// Clear tracking for new seeding session
				StoreBuilder.ResetTracking();

				var data = new SeedData();
				data.Initialize();

				var defaultStores = new[]
				{
					data.Store1, data.Store2, data.Store3, data.Store4,
					data.Store5, data.Store6, data.Store7, data.Store8
				};

				// Validate default stores - should never fail
				var validatedStores = MigrationDataHelper.ValidateItems(
					defaultStores,
					ValidateStoreWithBuilder,
					"default store",
					strictValidation: true);

				builder.HasData(validatedStores);
			}
			catch (Exception ex)
			{
				MigrationLogger.LogError(string.Format(FailedToSeedDefaultData, ex.Message), ex);
				throw;
			}
		}
	}
}