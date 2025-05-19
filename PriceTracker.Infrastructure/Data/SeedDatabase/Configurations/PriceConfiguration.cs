using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class PriceConfiguration : IEntityTypeConfiguration<Price>
	{
		public void Configure(EntityTypeBuilder<Price> builder)
		{
			// Unique composite index
			builder.HasIndex(p => new { p.ProductId, p.StoreId, p.DateChecked })
				   .IsUnique();

			// Configuration of the relations 
			// We keep the history of the prices
			builder.HasOne(p => p.Product)
				   .WithMany(pr => pr.Prices)
				   .HasForeignKey(p => p.ProductId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(p => p.Store)
				   .WithMany(s => s.Prices)
				   .HasForeignKey(p => p.StoreId)
				   .OnDelete(DeleteBehavior.Restrict);

			var data = new SeedData();

			builder.HasData(
			[
				data.Price1,
				data.Price2,
				data.Price3,
				data.Price4,
				data.Price5,
				data.Price6,
				data.Price7,
				data.Price8,
				data.Price9
			]);
		}
	}
}