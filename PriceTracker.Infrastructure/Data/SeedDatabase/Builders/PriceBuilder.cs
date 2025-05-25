using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Builder for creating validated Price entities
	/// </summary>
	public class PriceBuilder : IBuilder<Price>
	{
		private readonly Price _price;
		private const decimal MaxAllowedPrice = 1000000m;

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
				throw new ValidationException(PriceConstants.ProductRequired);
			}

			if (product.ProductId == 0)
			{
				throw new ValidationException(PriceConstants.ProductIdRequired);
			}

			// Store validations
			if (store == null)
			{
				throw new ValidationException(PriceConstants.StoreRequired);
			}

			if (store.StoreId == 0)
			{
				throw new ValidationException(PriceConstants.StoreIdRequired);
			}

			// Price validations
			if (sellingPrice <= 0)
			{
				throw new ValidationException(string.Format(PriceConstants.InvalidPrice, sellingPrice));
			}

			if (sellingPrice > MaxAllowedPrice)
			{
				throw new ValidationException(
					string.Format(PriceConstants.ExceedsMaxPrice, MaxAllowedPrice, sellingPrice));
			}

			// Date validations
			
			if (!IsValidDate(dateChecked))
			{
				throw new ValidationException(PriceConstants.InvalidDateFormat);
			}

			if (dateChecked > DateTime.Now)
			{
				throw new ValidationException(
					string.Format(PriceConstants.FutureDate, DateTime.Now, dateChecked));
			}					
		}

		private static bool IsValidDate(DateTime date)
		{
			return DateTime.TryParse(date.ToString(), out _);
		}
	}
}
