using Microsoft.Extensions.Options;
using PriceTracker.Configuration;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeeders;
using static PriceTracker.Infrastructure.Constants.DataProviderMessages.SeederServiceConstants;
namespace PriceTracker.Infrastructure.Data.SeedDatabase.Services;

/// <summary>
/// Service responsible for executing all seeders
/// </summary>
public class SeederService : ISeederService
{
	private readonly IEnumerable<ISeeder> _seeders;
	private readonly IAppLogger _logger;
	private readonly SeedingOptions _options;

	public SeederService(
		IEnumerable<ISeeder> seeders,
		IAppLogger logger,
		IOptions<SeedingOptions> options)
	{
		_seeders = seeders;
		_logger = logger;
		_options = options.Value;
	}

	/// <summary>
	/// Executes all registered seeders in the correct order
	/// </summary>
	public async Task SeedAllAsync()
	{
		if (!_options.UseExternalSource)
		{
			_logger.LogInformation(ExternalSeedingDisabled);
			return;
		}

		_logger.LogInformation(StartingSeeding);

		try
		{
			// Sort seeders by priority (lower value = higher priority)
			var orderedSeeders = _seeders
				.OrderBy(s => s.Priority)
				.ToList();

			_logger.LogInformation(string.Format(FoundSeeders, orderedSeeders.Count));

			foreach (var seeder in orderedSeeders)
			{
				var seederName = seeder.GetType().Name.Replace(SeederSuffix, string.Empty);

				// Check if the seeder is enabled in the configuration
				if (_options.EnabledSeeders.TryGetValue(seederName, out bool isEnabled) && isEnabled)
				{
					_logger.LogInformation(string.Format(ExecutingSeeder, seeder.SeederName));
					await seeder.SeedAsync();
				}
				else
				{
					_logger.LogInformation(string.Format(SkippingSeeder, seeder.SeederName));
				}
			}

			_logger.LogInformation(SeedingCompleted);
		}
		catch (Exception ex)
		{
			_logger.LogError(SeedingError, ex);
			throw;
		}
	}
}
