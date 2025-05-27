using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Constants.DataConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Enhanced ProductBuilder with in-memory duplication tracking
	/// </summary>
	public class ProductBuilder : IBuilder<Product>
	{
		private readonly Product _product;
		private static readonly HashSet<string> _currentSeedProducts = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new product with enhanced validation including duplication tracking
		/// </summary>
		/// <param name="productName">Product name</param>
		/// <param name="brandName">Product brand</param>
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

				// Track in current seed session to prevent duplicates
				var productKey = $"{productName}|{brandName}".ToLower();
				_currentSeedProducts.Add(productKey);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create product: {ex.Message}");
			}
		}

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

			// In-memory duplication check for current seed session
			var productKey = $"{productName}|{brandName}".ToLower();
			if (_currentSeedProducts.Contains(productKey))
			{
				throw new ValidationException(
					$"Duplicate product in current seed session: '{productName}' by '{brandName}'");
			}
		}

		public Product Build() => _product;

		/// <summary>
		/// Clear tracking collections for new seeding session
		/// Call this before starting a new migration or seeding operation
		/// </summary>
		public static void ResetTracking()
		{
			_currentSeedProducts.Clear();
		}

		/// <summary>
		/// Get count of currently tracked products in this session
		/// </summary>
		public static int GetTrackedProductCount()
		{
			return _currentSeedProducts.Count;
		}
	}
}

