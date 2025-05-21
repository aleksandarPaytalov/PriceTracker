using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Constants.DataConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Product builder class used for data seeding and validation before data beeing imported in database 
	/// </summary>
	public class ProductBuilder : IBuilder<Product>
	{
		private readonly Product _product;

		/// <summary>
		/// Creates a new product with required data
		/// </summary>
		/// <param name="name">Product name</param>
		/// <param name="brand">Product brand</param>
		/// <param name="category">Product category</param>
		/// <param name="quantity">Product quantity</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public ProductBuilder(
			string productName,
			string brandName,
			ProductCategory category,
			int quantity)
		{
			try
			{
				ValidateProductInputData(productName, brandName, category, quantity);

				_product = new Product
				{
					ProductName = productName,
					Brand = brandName,
					Category = category,
					Quantity = quantity,
					Prices = new List<Price>(),
					Expenses = new List<Expense>()
				};
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create product: {ex.Message}");
			}
		}

		public Product Build() => _product;
		

		private static void ValidateProductInputData(
			string productName,
			string brandName,
			ProductCategory category,
			int quantity)
		{
			// Name validations
			if (string.IsNullOrWhiteSpace(productName))
			{
				throw new ValidationException(ProductConstants.NameRequired);
			}

			if (productName.Length < productNameMinLength || productName.Length > productNameMaxLength)
			{
				throw new ValidationException(
					string.Format(ProductConstants.InvalidNameLength,
						productNameMinLength,
						productNameMaxLength));
			}

			if (string.IsNullOrWhiteSpace(brandName))
			{
				throw new ValidationException(ProductConstants.BrandRequired);
			}

			if (brandName.Length < productBrandNameMinLength || brandName.Length > productBrandNameMaxLength)
			{
				throw new ValidationException(
					string.Format(ProductConstants.InvalidBrandLength,
						productBrandNameMinLength,
						productBrandNameMaxLength));
			}

			// Category validation
			if (!Enum.IsDefined(typeof(ProductCategory), category))
			{
				throw new ValidationException(
					string.Format(ProductConstants.InvalidCategory, category));
			}

			// Quantity validation
			if (quantity < 0)
			{
				throw new ValidationException(ProductConstants.InvalidQuantity);
			}
		}
	}
}
