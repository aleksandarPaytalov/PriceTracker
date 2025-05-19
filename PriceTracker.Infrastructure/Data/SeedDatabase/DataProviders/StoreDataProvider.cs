using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.StoreDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	/// <summary>
	/// Provider responsible for loading and managing store data
	/// </summary>
	public class StoreDataProvider : BaseDataProvider<Store>
	{
		public StoreDataProvider(
			IRepository<Store> repository,
			IDataSource<Store>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
		}

		/// <summary>
		/// Main method to retrieve store data
		/// Returns collection of stores from external source or default data
		/// </summary>
		public override IEnumerable<Store> GetData()
		{
			var stores = new List<Store>();

			try
			{
				stores.AddRange(LoadStoresFromExternalSource());

				_logger.LogInformation(string.Format(FinishedLoadingData,
						_typeName,
						stores.Count));
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(GetData), ex);
			}

			return stores;
		}

		/// <summary>
		/// Loads stores from external source
		/// </summary>
		private IEnumerable<Store> LoadStoresFromExternalSource()
		{
			var stores = new List<Store>();

			try
			{
				_logger.LogInformation(StartingExternalSource);

				var sourceStores = LoadFromSourceAsync().Result;
				foreach (var storeData in sourceStores)
				{
					try
					{
						if (!StoreExists(storeData))
						{
							var store = CreateStore(storeData.Name);
							if (store != null)
							{
								stores.Add(store);
								LogStoreAdded(store);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatStoreIdentifier(storeData.Name);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadStoresFromExternalSource), ex);
			}

			return stores;
		}

		/// <summary>
		/// Creates a new Store instance using the builder pattern
		/// </summary>
		private Store? CreateStore(string name)
		{
			try
			{
				return new StoreBuilder(name, _repository).Build();
			}
			catch (Exception ex)
			{
				var identifier = FormatStoreIdentifier(name);
				LogProcessingError(identifier, ex);
				return null;
			}
		}

		/// <summary>
		/// Checks if a store already exists in database
		/// </summary>
		private bool StoreExists(Store store)
		{
			return StoreExists(store.Name);
		}

		/// <summary>
		/// Checks if a store already exists based on name
		/// </summary>
		private bool StoreExists(string name)
		{
			return EntityExists(s => s.Name == name);
		}

		/// <summary>
		/// Logs the addition of a new store to the system
		/// </summary>
		private void LogStoreAdded(Store store)
		{
			var message = string.Format(
				StoreAdded,
				store.Name);

			_logger.LogInformation(message);
		}

		/// <summary>
		/// Creates a formatted identifier string for a store
		/// </summary>
		private static string FormatStoreIdentifier(string name)
		{
			return string.Format(StoreIdentifier, name);
		}
	}
}