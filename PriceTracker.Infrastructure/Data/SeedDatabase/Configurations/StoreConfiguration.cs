using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class StoreConfiguration : IEntityTypeConfiguration<Store>
	{
		private readonly IOptions<SeedingOptions> _options;

		public StoreConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options;
		}

		public void Configure(EntityTypeBuilder<Store> builder)
		{
			// Unique index for the Store name
			builder.HasIndex(s => s.Name).IsUnique();

			// Configuration of the relations
			builder.HasMany(s => s.Prices)
				   .WithOne(p => p.Store)
				   .HasForeignKey(p => p.StoreId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(s => s.Expenses)
				   .WithOne(e => e.Store)
				   .HasForeignKey(e => e.StoreId)
				   .OnDelete(DeleteBehavior.Restrict);

			if (!_options.Value.UseExternalSource)
			{
				var data = new SeedData();
				data.Initialize();

				builder.HasData(
				[
					data.Store1,
					data.Store2,
					data.Store3,
					data.Store4,
					data.Store5,
					data.Store6,
					data.Store7,
					data.Store8,
				]);
			}		
		}
	}
}