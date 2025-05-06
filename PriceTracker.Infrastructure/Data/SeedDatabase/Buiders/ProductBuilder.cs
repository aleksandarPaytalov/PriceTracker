using PriceTracker.Infrastructure.Constants;
using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Buiders
{
	/// <summary>
	/// Product builder class used for data seeding and validation before data beeing imported in database 
	/// </summary>
	public class ProductBuilder : IBuilder<Product>
	{
		private readonly Product _product = new();

		public ProductBuilder WithName(string name)
		{
			_product.ProductName = name;
			return this;
		}

		public ProductBuilder WithBrand(string brand)
		{
			_product.Brand = brand;
			return this;
		}

		public ProductBuilder WithCategory(ProductCategory category)
		{
			_product.Category = category;
			return this;
		}

		public ProductBuilder WithQuantity(int quantity)
		{
			_product.Quantity = quantity;
			return this;
		}

		public Product Build()
		{
			ValidateProduct();
			return _product;
		}

		private void ValidateProduct()
		{
			// Product Name validations
			if (string.IsNullOrWhiteSpace(_product.ProductName))
			{
				throw new ValidationException("Product name cannot be empty");
			}

			if (_product.ProductName.Length < DataConstants.productNameMinLength ||
				_product.ProductName.Length > DataConstants.productNameMaxLength)
			{
				throw new ValidationException(
					$"Product name must be between {DataConstants.productNameMinLength} and {DataConstants.productNameMaxLength} characters");
			}

			// Check for special symbols in the name
			var forbiddenChars = new[] { "@", "#", "$", "%", "^", "&", "*" };
			if (forbiddenChars.Any(c => _product.ProductName.Contains(c)))
			{
				throw new ValidationException("Product name contains forbidden characters");
			}

			// Check for multiple spaces one after another 
			if (_product.ProductName.Contains("  "))
			{
				throw new ValidationException("Product name cannot contain consecutive spaces");
			}

			// Check for spaces at the start or the end of the ProductName
			if (_product.ProductName != _product.ProductName.Trim())
			{
				throw new ValidationException("Product name cannot have leading or trailing spaces");
			}

			// Brand validations
			if (string.IsNullOrWhiteSpace(_product.Brand))
			{
				throw new ValidationException("Brand cannot be empty");
			}

			if (_product.Brand.Length < DataConstants.productBrandNameMinLength ||
				_product.Brand.Length > DataConstants.productBrandNameMaxLength)
			{
				throw new ValidationException(
					$"Brand must be between {DataConstants.productBrandNameMinLength} and {DataConstants.productBrandNameMaxLength} characters");
			}

			// Check for valid Brand Format
			if (!Regex.IsMatch(_product.Brand, @"^[a-zA-Z0-9\s\-&]+$"))
			{
				throw new ValidationException("Brand can only contain letters, numbers, spaces, hyphens and '&' symbol");
			}

			// Quantity validations
			if (_product.Quantity < 0)
			{
				throw new ValidationException("Quantity cannot be negative");
			}

			if (_product.Quantity > 10000) // Example for maximum quantity
			{
				throw new ValidationException("Quantity exceeds maximum allowed value");
			}

			// Category validations
			if (!Enum.IsDefined(typeof(ProductCategory), _product.Category))
			{
				throw new ValidationException("Invalid product category");
			}

			// Product name format validations
			if (_product.ProductName.All(char.IsDigit))
			{
				throw new ValidationException("Product name cannot contain only numbers");
			}

			// Check for minimum characters in ProductName
			var letterCount = _product.ProductName.Count(char.IsLetter);
			if (letterCount < 2)
			{
				throw new ValidationException("Product name must contain at least 2 letters");
			}

			// Brand format validations
			if (_product.Brand.Length < 2)
			{
				throw new ValidationException("Brand must be at least 2 characters long");
			}

			// Check for two or more same words in ProductName
			var words = _product.ProductName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (words.Length != words.Distinct(StringComparer.OrdinalIgnoreCase).Count())
			{
				throw new ValidationException("Product name contains duplicate words");
			}

			// Check for meaningful names
			var forbiddenNames = new[] { "test", "demo", "sample", "unknown", "undefined" };
			if (forbiddenNames.Any(name =>
				_product.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase)))
			{
				throw new ValidationException("Product name contains forbidden words");
			}

			// Check for match between catery and name 
			ValidateProductNameCategory();

			// Cross-field validations
			if (_product.ProductName.Contains(_product.Brand, StringComparison.OrdinalIgnoreCase))
			{
				//If the name contain the brand inside his name, check if there is additional description 
				var nameWithoutBrand = _product.ProductName
					.Replace(_product.Brand, "", StringComparison.OrdinalIgnoreCase)
					.Trim();

				if (string.IsNullOrWhiteSpace(nameWithoutBrand))
				{
					throw new ValidationException("Product name must contain more information than just the brand");
				}
			}

			// Check for abbreviation
			if (ContainsUnknownAbbreviations(_product.ProductName))
			{
				throw new ValidationException("Product name contains unknown abbreviations");
			}

			// Price validation if there is any
			if (_product.Prices.Any())
			{
				foreach (var price in _product.Prices)
				{
					if (price.SellingPrice <= 0)
					{
						throw new ValidationException("Product price cannot be zero or negative");
					}

					if (price.SellingPrice > 100000) // Примерна максимална стойност
					{
						throw new ValidationException("Product price exceeds maximum allowed value");
					}
				}
			}

			// Connected expenses validation
			if (_product.Expenses.Any())
			{
				foreach (var expense in _product.Expenses)
				{
					if (expense.AmountSpent <= 0)
					{
						throw new ValidationException("Expense amount cannot be zero or negative");
					}

					if (expense.AmountSpent > expense.Product?.Prices.Max(p => p.SellingPrice) * 1.5m)
					{
						throw new ValidationException("Expense amount seems unusually high compared to product price");
					}
				}
			}
		}

		private void ValidateProductNameCategory()
		{
			// Specific rules for different Categories
			switch (_product.Category)
			{
				case ProductCategory.Electronics:
					ValidateElectronicsName();
					break;
				case ProductCategory.Food:
					ValidateFoodName();
					break;
				case ProductCategory.Clothing:
					ValidateClothingName();
					break;
					// More categories can be implemented here if needed
			}
		}

		private void ValidateElectronicsName()
		{
			// Examplary rules for electornics
			var commonElectronicsTerms = new[] { "phone", "laptop", "tv", "computer", "tablet" };
			if (!commonElectronicsTerms.Any(term =>
				_product.ProductName.Contains(term, StringComparison.OrdinalIgnoreCase)))
			{
				throw new ValidationException("Electronics product name should contain relevant term");
			}
		}

		private void ValidateFoodName()
		{
			// Examplary rules for food
			if (_product.ProductName.Contains("electronics", StringComparison.OrdinalIgnoreCase) ||
				_product.ProductName.Contains("clothing", StringComparison.OrdinalIgnoreCase))
			{
				throw new ValidationException("Food product name contains terms from other categories");
			}
		}

		private void ValidateClothingName()
		{
			// Examplary rules for Clothing
			var commonClothingTerms = new[] { "shirt", "pants", "dress", "jacket", "shoes" };
			if (!commonClothingTerms.Any(term =>
				_product.ProductName.Contains(term, StringComparison.OrdinalIgnoreCase)))
			{
				throw new ValidationException("Clothing product name should contain relevant term");
			}
		}

		private bool ContainsUnknownAbbreviations(string name)
		{
			// List with allowed abbreviations
			var knownAbbreviations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"TV", "DVD", "USB", "HD", "SSD", "RAM", "CPU", "GB", "TB", "LCD", "LED"
			};

			// Checking for words thats looks like abbreviations
			var words = name.Split(' ');
			var possibleAbbreviations = words.Where(w =>
				w.Length <= 4 && w.All(char.IsUpper));

			// Check if all abbreviations found are from the allowed List
			return possibleAbbreviations.Any(abbr =>
				!knownAbbreviations.Contains(abbr));
		}
	}
}
