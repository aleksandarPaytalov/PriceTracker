using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class PriceConfiguration : BaseConfiguration<Price>
	{
		public PriceConfiguration(IDataProvider<Price> dataProvider) : base(dataProvider)
		{
		}

		protected override void ConfigureEntity(EntityTypeBuilder<Price> builder)
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
		}
	}
}