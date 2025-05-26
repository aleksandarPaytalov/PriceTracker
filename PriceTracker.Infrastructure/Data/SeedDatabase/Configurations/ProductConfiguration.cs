using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Configuration;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.ConfigurationConstants;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	private readonly IOptions<SeedingOptions>? _options;

	public ProductConfiguration(IOptions<SeedingOptions>? options = null)
	{
		_options = options;
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
		if (ShouldUseExternalSource())
		{
			// Use JSON data via existing JsonDataSource
			var jsonProducts = MigrationDataHelper.GetDataFromJson<Product>("products.json");
			if (jsonProducts.Any())
			{
				builder.HasData(jsonProducts);
			}
			else
			{
				// Throw error if JSON is empty or invalid when external source is expected
				throw new InvalidOperationException(
					string.Format(ExternalSourceEnabledButNoData, "products.json"));
			}
		}
		else
		{
			// Use default seed data
			SeedDefaultData(builder);
		}
	}

	/// <summary>
	/// Seeds default data from SeedData class
	/// </summary>
	private static void SeedDefaultData(EntityTypeBuilder<Product> builder)
	{
		var data = new SeedData();
		data.Initialize();

		builder.HasData(
		[
			data.Product1,
				data.Product2,
				data.Product3,
				data.Product4,
				data.Product5,
				data.Product6,
				data.Product7,
				data.Product8,
				data.Product9,
			]);
	}

	/// <summary>
	/// Determines if external source should be used - FIXED LOGIC
	/// </summary>
	private bool ShouldUseExternalSource()
	{
		// ONLY check configuration, ignore file existence
		return _options?.Value.UseExternalSource == true;
	}
}