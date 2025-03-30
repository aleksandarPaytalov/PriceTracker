using System.ComponentModel;

namespace PriceTracker.Infrastructure.Constants
{
	public static class DataConstants
	{
		/// <summary>
		/// Users table data constants
		/// </summary>
		public const int userNameMinLength = 3;
		public const int userNameMaxLength = 50;

		public const int emailAddressMinLength = 5;
		public const int emailAddressMaxLength = 100;

		public const int passwordHashMinLength = 10;
		public const int passwordHashMaxLength = 255;

		/// <summary>
		/// Stores table data constants
		/// </summary>
		public const int storeNameMinLength = 2;
		public const int storeNameMaxLength = 100;

		/// <summary>
		/// Stores table data constants
		/// </summary>
	}
}
