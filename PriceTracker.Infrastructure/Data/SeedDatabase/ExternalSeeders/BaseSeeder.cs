using PriceTracker.Infrastructure.Common;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.BaseSeederMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeeders
{
	/// <summary>
	/// Base class for all seeders, providing common functionality
	/// </summary>
	public abstract class BaseSeeder : ISeeder
	{
		protected readonly IAppLogger _logger;
		protected readonly IServiceProvider _serviceProvider;

		protected BaseSeeder(IServiceProvider serviceProvider, IAppLogger logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		/// <summary>
		/// Priority of the seeder - lower value means higher priority
		/// </summary>
		public abstract int Priority { get; }

		/// <summary>
		/// The name of the seeder
		/// </summary>
		public abstract string SeederName { get; }

		/// <summary>
		/// Method for seeding data
		/// </summary>
		public virtual async Task SeedAsync()
		{
			_logger.LogInformation(string.Format(StartingSeedOperation, SeederName));

			try
			{
				// Check if existing data exists
				if (await ShouldSeedAsync())
				{
					await SeedDataAsync();
					_logger.LogInformation(string.Format(SuccessfullyCompletedSeeding, SeederName));
				}
				else
				{
					_logger.LogInformation(string.Format(SkippingSeedingDataExists, SeederName));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(string.Format(ErrorWhileSeeding, SeederName, ex.Message), ex);
				throw;
			}
		}

		/// <summary>
		/// Checks whether seeding should be executed
		/// </summary>
		protected abstract Task<bool> ShouldSeedAsync();

		/// <summary>
		/// Implementation of seeding for a specific type
		/// </summary>
		protected abstract Task SeedDataAsync();

		/// <summary>
		/// Returns the full path to a JSON file
		/// </summary>
		protected string GetJsonFilePath(Type infrastructureType, string relativeDataPath, string fileName)
		{
			try
			{
				var infrastructureAssembly = infrastructureType.Assembly;
				var infrastructurePath = Path.GetDirectoryName(infrastructureAssembly.Location)!;
				return Path.Combine(infrastructurePath, relativeDataPath, fileName);
			}
			catch (Exception ex)
			{
				_logger.LogError(string.Format(FailedToGetJsonFilePath, fileName), ex);
				throw;
			}
		}
	}
}
