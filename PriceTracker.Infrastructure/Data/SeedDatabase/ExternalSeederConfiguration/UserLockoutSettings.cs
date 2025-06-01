namespace PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration
{
	/// <summary>
	/// User lockout settings for seeded users
	/// </summary>
	public class UserLockoutSettings
	{
		/// <summary>
		/// Enable lockout for seeded users
		/// </summary>
		public bool LockoutEnabled { get; set; } = false;

		/// <summary>
		/// Maximum failed access attempts before lockout
		/// </summary>
		public int MaxFailedAccessAttempts { get; set; } = 5;

		/// <summary>
		/// Default lockout duration in minutes
		/// </summary>
		public int DefaultLockoutTimeSpanMinutes { get; set; } = 15;
	}
}
