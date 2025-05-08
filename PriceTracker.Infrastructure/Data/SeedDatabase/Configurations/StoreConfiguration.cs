using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class StoreConfiguration : BaseConfiguration<Store>
	{
		public StoreConfiguration(IDataProvider<Store> dataProvider) : base(dataProvider)
		{
		}

		protected override void ConfigureEntity(EntityTypeBuilder<Store> builder)
		{
			// Unique index for the Store name
			builder.HasIndex(s => s.Name)
				   .IsUnique();

			// Configuration of the relations
			builder.HasMany(s => s.Prices)
				   .WithOne(p => p.Store)
				   .HasForeignKey(p => p.StoreId)
				   .OnDelete(DeleteBehavior.Restrict);  

			builder.HasMany(s => s.Expenses)
				   .WithOne(e => e.Store)
				   .HasForeignKey(e => e.StoreId)
				   .OnDelete(DeleteBehavior.Restrict);  
		}
	}
}