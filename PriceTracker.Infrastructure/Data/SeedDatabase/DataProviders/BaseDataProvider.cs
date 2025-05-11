using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseDataProviderMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders
{
	public abstract class BaseDataProvider<T> : IDataProvider<T> where T : class
	{
		protected readonly IRepository<T> _repository;
		protected readonly IDataSource<T>? _dataSource;
		protected readonly IAppLogger _logger;
		protected readonly string _typeName;

		protected BaseDataProvider(
			IRepository<T> repository,
			IDataSource<T>? dataSource = null,
			IAppLogger? logger = null)
		{
			_repository = repository;
			_dataSource = dataSource;
			_logger = logger ?? new FileLogger();
			_typeName = typeof(T).Name;
		}

		public abstract IEnumerable<T> GetData();

		protected bool EntityExists(Func<T, bool> predicate)
		{
			try
			{
				return _repository.AllReadOnly().Any(predicate);
			}
			catch (Exception ex)
			{
				_logger.LogError(
					string.Format(ErrorCheckingEntityExistence, _typeName),
					ex);
				return false;
			}
		}

		protected async Task<IEnumerable<T>> LoadFromSourceAsync()
		{
			if (_dataSource == null)
			{
				_logger.LogInformation(
					string.Format(NoDataSourceProvided, _typeName));
				return Enumerable.Empty<T>();
			}

			try
			{
				_logger.LogInformation(
					string.Format(StartingToLoadData, _typeName));

				var data = await _dataSource.LoadDataAsync();

				_logger.LogInformation(
					string.Format(SuccessfullyLoadedRecords,
						data.Count(),
						_typeName));

				return data;
			}
			catch (FileNotFoundException ex)
			{
				_logger.LogWarning(
					string.Format(DataFileNotFound,
						_typeName,
						ex.Message));
				return Enumerable.Empty<T>();
			}
			catch (Exception ex)
			{
				_logger.LogError(
					string.Format(ErrorLoadingData, _typeName),
					ex);
				return Enumerable.Empty<T>();
			}
		}

		protected void LogProcessingError(string entityIdentifier, Exception ex)
		{
			_logger.LogError(
				string.Format(ErrorProcessingEntity,
					_typeName,
					entityIdentifier),
				ex);
		}

		protected void LogCriticalError(string methodName, Exception ex)
		{
			_logger.LogError(
				string.Format(CriticalError,
					_typeName,
					methodName),
				ex);
		}
	}
}