using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.PriceConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Builder for creating validated Price entities
	/// </summary>
	public class PriceBuilder : IBuilder<Price>
	{
		private readonly Price _price;
		private const decimal MaxAllowedPrice = 1000000m;
		private static readonly HashSet<string> _currentSeedPrices = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new price with required data
		/// </summary>
		/// <param name="product">The product associated with the price</param>
		/// <param name="store">The store where the price is valid</param>
		/// <param name="sellingPrice">The selling price of the product</param>
		/// <param name="dateChecked">Optional date when the price was checked</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public PriceBuilder(
			Product product,
			Store store,
			decimal sellingPrice,
			DateTime? dateChecked = null)
		{
			try
			{
				var actualDateChecked = dateChecked ?? DateTime.Now;
				ValidatePriceInputData(product, store, sellingPrice, actualDateChecked);

				_price = new Price
				{
					ProductId = product.ProductId,
					StoreId = store.StoreId,
					SellingPrice = sellingPrice,
					DateChecked = actualDateChecked
				};

				// Track combination for counting purposes
				var priceKey = $"{product.ProductId}|{store.StoreId}|{actualDateChecked:yyyy-MM-dd}".ToLower();
				_currentSeedPrices.Add(priceKey);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create price: {ex.Message}");
			}
		}

		public Price Build() => _price;

		private static void ValidatePriceInputData(
			Product product,
			Store store,
			decimal sellingPrice,
			DateTime dateChecked)
		{
			// Product validations
			if (product == null)
			{
				throw new ValidationException(ProductRequired);
			}

			if (product.ProductId == 0)
			{
				throw new ValidationException(ProductIdRequired);
			}

			// Store validations
			if (store == null)
			{
				throw new ValidationException(StoreRequired);
			}

			if (store.StoreId == 0)
			{
				throw new ValidationException(StoreIdRequired);
			}

			// Price validations
			if (sellingPrice <= 0)
			{
				throw new ValidationException(string.Format(InvalidPrice, sellingPrice));
			}

			if (sellingPrice > MaxAllowedPrice)
			{
				throw new ValidationException(
					string.Format(ExceedsMaxPrice, MaxAllowedPrice, sellingPrice));
			}

			// Date validations
			if (!IsValidDate(dateChecked))
			{
				throw new ValidationException(InvalidDateFormat);
			}

			if (dateChecked > DateTime.Now)
			{
				throw new ValidationException(
					string.Format(FutureDate, DateTime.Now, dateChecked));
			}
		}

		private static bool IsValidDate(DateTime date)
		{
			return DateTime.TryParse(date.ToString(), out _);
		}

		/// <summary>
		/// Clear tracking collections for new seeding session
		/// Call this before starting a new migration or seeding operation
		/// </summary>
		public static void ResetTracking()
		{
			_currentSeedPrices.Clear();
		}

		/// <summary>
		/// Get count of currently tracked price combinations in this session
		/// </summary>
		public static int GetTrackedPriceCount()
		{
			return _currentSeedPrices.Count;
		}
	}
}