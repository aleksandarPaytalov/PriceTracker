using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Configuration;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		private readonly IOptions<SeedingOptions> _options;
		public ProductConfiguration(IOptions<SeedingOptions> options)
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

			if (!_options.Value.UseExternalSource)
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
		}
	}
}