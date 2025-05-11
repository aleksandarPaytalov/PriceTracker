using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.StoreDataProviderConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	public class StoreDataProvider : BaseDataProvider<Store>
	{
		public StoreDataProvider(
			IRepository<Store> repository,
			IDataSource<Store>? dataSource = null,
			IAppLogger? logger = null)
			: base(repository, dataSource, logger)
		{
		}

		public override IEnumerable<Store> GetData()
		{
			var stores = new List<Store>();

			try
			{
				if (_dataSource != null)
				{
					stores.AddRange(LoadStoresFromExternalSource());
				}
				else
				{
					stores.AddRange(LoadDefaultStores());
				}

				_logger.LogInformation(
					string.Format(FinishedLoadingData,
						_typeName,
						stores.Count));
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(GetData), ex);
			}

			return stores;
		}

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
								LogStoreAdded(store, isDefault: false);
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

		private IEnumerable<Store> LoadDefaultStores()
		{
			var stores = new List<Store>();

			try
			{
				_logger.LogInformation(LoadingDefaultData);

				foreach (var storeName in GetDefaultStoreData())
				{
					try
					{
						if (!StoreExists(storeName))
						{
							var store = CreateStore(storeName);
							if (store != null)
							{
								stores.Add(store);
								LogStoreAdded(store, isDefault: true);
							}
						}
					}
					catch (Exception ex)
					{
						var identifier = FormatStoreIdentifier(storeName);
						LogProcessingError(identifier, ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogCriticalError(nameof(LoadDefaultStores), ex);
			}

			return stores;
		}

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

		private bool StoreExists(Store store)
		{
			return StoreExists(store.Name);
		}

		private bool StoreExists(string name)
		{
			return EntityExists(s => s.Name == name);
		}

		private void LogStoreAdded(Store store, bool isDefault)
		{
			var message = string.Format(
				isDefault ? DefaultStoreAdded
						 : StoreAdded,
				store.Name);

			_logger.LogInformation(message);
		}

		private string FormatStoreIdentifier(string name)
		{
			return string.Format(StoreIdentifier, name);
		}

		private IEnumerable<string> GetDefaultStoreData()
		{
			return new[]
			{
				"Kaufland",
				"Lidl",
				"Billa",
				"Metro",
				"BBB",
				"T-Market"
			};
		}
	}
}