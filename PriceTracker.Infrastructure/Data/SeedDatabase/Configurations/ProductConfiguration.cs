using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase;
using PriceTracker.Infrastructure.Data.SeedDatabase.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ProductConfigurationConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ProductConstants;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	private readonly IOptions<SeedingOptions> _options;

	public ProductConfiguration(IOptions<SeedingOptions> options)
	{
		_options = options ?? throw new ArgumentNullException(nameof(options));
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
				MigrationLogger.LogInformation(string.Format(LoadedProductsFromJson, validatedProducts.Count()));
			}
			else
			{
				var errorMessage = string.Format(ExternalSourceEnabledButNoData, "products.json");
				MigrationLogger.LogError(errorMessage);
				throw new InvalidOperationException(errorMessage);
			}
		}
		else
		{
			// Use default seed data only if Product seeding is not disabled
			if (!_options.Value.UseExternalSource || _options.Value.EnabledSeeders.GetValueOrDefault("Product", true))
			{
				SeedDefaultData(builder);
				MigrationLogger.LogInformation(UsingDefaultSeedData);
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

			// Load JSON products directly as Product objects
			var jsonProducts = MigrationDataHelper.GetDataFromJson<Product>("products.json");

			if (!jsonProducts.Any())
			{
				MigrationLogger.LogWarning(NoProductsFoundInJson);
				return Enumerable.Empty<Product>();
			}

			// Validate using ProductBuilder - returns only valid items
			var validatedProducts = MigrationDataHelper.ValidateItems(
				jsonProducts,
				ValidateProductWithBuilder,
				"product",
				_options.Value.StrictValidation);

			return validatedProducts;
		}
		catch (Exception ex) when (!(ex is ValidationException))
		{
			MigrationLogger.LogError(string.Format(FailedToLoadProductsFromJson, ex.Message), ex);
			throw new InvalidOperationException(string.Format(ProductLoadingFailed, ex.Message), ex);
		}
	}

	/// <summary>
	/// Validates a Product object using ProductBuilder validation logic
	/// </summary>
	private static Product ValidateProductWithBuilder(Product product)
	{
		// Validate ProductId first - reuse existing constant
		if (product.ProductId <= 0)
		{
			throw new ValidationException(string.Format(InvalidProductId, product.ProductId));
		}

		// Use ProductBuilder for validation
		var productBuilder = new ProductBuilder(
			product.ProductName,
			product.Brand,
			product.Category,
			product.Quantity
		);

		var validatedProduct = productBuilder.Build();
		validatedProduct.ProductId = product.ProductId;

		return validatedProduct;
	}

	/// <summary>
	/// Seeds default data from SeedData class using ProductBuilder validation
	/// </summary>
	private void SeedDefaultData(EntityTypeBuilder<Product> builder)
	{
		try
		{
			ProductBuilder.ResetTracking();

			var data = new SeedData();
			data.Initialize();

			var defaultProducts = new[]
			{
				data.Product1, data.Product2, data.Product3, data.Product4, data.Product5,
				data.Product6, data.Product7, data.Product8, data.Product9
			};

			// Validate default products - should never fail
			var validatedProducts = MigrationDataHelper.ValidateItems(
				defaultProducts,
				ValidateProductWithBuilder,
				"default product",
				strictValidation: true);

			builder.HasData(validatedProducts);
		}
		catch (Exception ex)
		{
			MigrationLogger.LogError(string.Format(FailedToSeedDefaultData, ex.Message), ex);
			throw;
		}
	}
}