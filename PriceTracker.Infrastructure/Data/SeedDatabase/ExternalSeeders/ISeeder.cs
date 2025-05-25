namespace PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeeders
{
	/// <summary>
	/// Interface for all classes that seed data
	/// </summary>
	public interface ISeeder
	{
		/// <summary>
		/// Priority of the seeder, determining the order of execution
		/// </summary>
		int Priority { get; }

		/// <summary>
		/// The name of the seeder, used for logging
		/// </summary>
		string SeederName { get; }

		/// <summary>
		/// Data seeding
		/// </summary>
		Task SeedAsync();
	}
}
