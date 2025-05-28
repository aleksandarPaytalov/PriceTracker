using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using PriceTracker.Infrastructure.Common;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	private readonly IOptions<SeedingOptions> _options;
	private readonly IAppLogger? _logger;

	public ProductConfiguration(IOptions<SeedingOptions> options, IAppLogger? logger = null)
	{
		_options = options ?? throw new ArgumentNullException(nameof(options));
		_logger = logger;
	}

	public void Configure(EntityTypeBuilder<Product> builder)
	{
		// Configuration of the relations
		builder.HasMany(p => p.Prices)
			   .WithOne(pr => pr.Product)
			   .HasForeignKey(pr => pr.ProductId)
			   .OnDelete(DeleteBehavior.Restrict);

		builder.HasMany(p => p.Expenses)
			   .WithOne(e => e.Product)
			   .HasForeignKey(e => e.ProductId)
			   .OnDelete(DeleteBehavior.Restrict);

		// Seeding data 
		if (_options.Value.UseExternalSource && _options.Value.EnabledSeeders.GetValueOrDefault("Product", false))
		{
			var validatedProducts = LoadAndValidateProductsFromJson();

			if (validatedProducts.Any())
			{
				builder.HasData(validatedProducts);
				_logger?.LogInformation($"Successfully loaded and validated {validatedProducts.Count()} products from JSON");
			}
			else
			{
				var errorMessage = string.Format(ExternalSourceEnabledButNoData, "products.json");
				_logger?.LogError(errorMessage);
				throw new InvalidOperationException(errorMessage);
			}
		}
		else
		{
			// Use default seed data only if Product seeding is not disabled
			if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("Product", true))
			{
				SeedDefaultData(builder);
				_logger?.LogInformation("Using default seed data for products");
			}
		}
	}

	/// <summary>
	/// Loads products from JSON and validates them using ProductBuilder
	/// </summary>
	private IEnumerable<Product> LoadAndValidateProductsFromJson()
	{
		try
		{
			// Clear tracking for new seeding session
			ProductBuilder.ResetTracking();

			var jsonProducts = MigrationDataHelper.GetDataFromJson<ProductJsonDto>("products.json");

			if (!jsonProducts.Any())
			{
				_logger?.LogWarning("No products found in products.json file");
				return Enumerable.Empty<Product>();
			}

			// Use the validation helper for consistent validation logic
			var validationResult = MigrationDataHelper.ValidateItems(
				jsonProducts,
				BuildProductFromDto,
				"product");

			// Handle validation results based on configuration
			if (validationResult.HasErrors)
			{
				var errorSummary = string.Join(Environment.NewLine,
					validationResult.ValidationErrors.Take(_options.Value.MaxValidationErrorsToLog)
					.Select(e => $"Item #{e.ItemIndex}: {e.ErrorMessage}"));

				if (_options.Value.StrictValidation)
				{
					throw new ValidationException(
						$"Product validation failed with {validationResult.InvalidCount} errors. " +
						$"Success rate: {validationResult.ValidationSuccessRate:F1}%{Environment.NewLine}{errorSummary}");
				}

				_logger?.LogWarning(
					$"Continuing with {validationResult.ValidCount} valid products despite {validationResult.InvalidCount} validation errors. " +
					$"Success rate: {validationResult.ValidationSuccessRate:F1}%{Environment.NewLine}{errorSummary}");
			}

			_logger?.LogInformation($"Successfully loaded and validated {validationResult.ValidCount} products from JSON");
			return validationResult.ValidItems;
		}
		catch (Exception ex) when (!(ex is ValidationException))
		{
			var errorMessage = $"Failed to load and validate products from JSON: {ex.Message}";
			_logger?.LogError(errorMessage, ex);
			throw new InvalidOperationException(errorMessage, ex);
		}
	}

	/// <summary>
	/// Builds a Product entity from ProductJsonDto using ProductBuilder validation
	/// </summary>
	private static Product BuildProductFromDto(ProductJsonDto dto)
	{
		// Validate ProductId first
		if (dto.ProductId <= 0)
		{
			throw new ValidationException($"Product ID must be a positive number. Provided value: {dto.ProductId}");
		}

		var productBuilder = new ProductBuilder(
			dto.ProductName,
			dto.Brand,
			(ProductCategory)dto.Category,
			dto.Quantity
		);

		var product = productBuilder.Build();

		// Set the ID from JSON (important for seeding)
		product.ProductId = dto.ProductId;

		return product;
	}

	/// <summary>
	/// Seeds default data from SeedData class using ProductBuilder validation
	/// </summary>
	private void SeedDefaultData(EntityTypeBuilder<Product> builder)
	{
		try
		{
			// Clear tracking for new seeding session
			ProductBuilder.ResetTracking();

			var data = new SeedData();
			data.Initialize();

			// Validate default products using ProductBuilder as well
			var defaultProducts = new[]
			{
				data.Product1, data.Product2, data.Product3, data.Product4, data.Product5,
				data.Product6, data.Product7, data.Product8, data.Product9
			};

			var validatedProducts = new List<Product>();

			foreach (var product in defaultProducts)
			{
				try
				{
					// Validate using ProductBuilder
					var productBuilder = new ProductBuilder(
						product.ProductName,
						product.Brand,
						product.Category,
						product.Quantity
					);

					var validatedProduct = productBuilder.Build();
					validatedProduct.ProductId = product.ProductId;
					validatedProducts.Add(validatedProduct);
				}
				catch (ValidationException ex)
				{
					_logger?.LogError($"Default product validation failed for '{product.ProductName}': {ex.Message}", ex);
					throw; // Re-throw for default data - this should never happen
				}
			}

			builder.HasData(validatedProducts);
		}
		catch (Exception ex)
		{
			_logger?.LogError($"Failed to seed default product data: {ex.Message}", ex);
			throw;
		}
	}
}

/// <summary>
/// DTO class for deserializing products from JSON
/// </summary>
public class ProductJsonDto
{
	public int ProductId { get; set; }
	public string ProductName { get; set; } = string.Empty;
	public string Brand { get; set; } = string.Empty;
	public int Quantity { get; set; }
	public int Category { get; set; }
}