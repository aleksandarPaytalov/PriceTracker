using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class ProductConfiguration : BaseConfiguration<Product>
	{
		public ProductConfiguration(IDataProvider<Product> dataProvider) : base(dataProvider)
		{
		}

		protected override void ConfigureEntity(EntityTypeBuilder<Product> builder)
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
		}
	}
}