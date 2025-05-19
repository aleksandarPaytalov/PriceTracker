using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
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

			var data = new SeedData();

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