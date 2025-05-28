using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
	{
		private readonly IOptions<SeedingOptions> _options;

		public ExpenseConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options;
		}

		public void Configure(EntityTypeBuilder<Expense> builder)
		{
			// Relations Config
			// We keep the expenses for tacking/accounting
			builder.HasOne(e => e.User)
				   .WithMany(u => u.Expenses)
				   .HasForeignKey(e => e.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(e => e.Product)
				   .WithMany(p => p.Expenses)
				   .HasForeignKey(e => e.ProductId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(e => e.Store)
				   .WithMany(s => s.Expenses)
				   .HasForeignKey(e => e.StoreId)
				   .OnDelete(DeleteBehavior.Restrict);

			if (!_options.Value.UseExternalSource)
			{
				var data = new SeedData();
				data.Initialize();

				builder.HasData(
				[
					data.Expense1,
					data.Expense2,
					data.Expense3,
					data.Expense4,
					data.Expense5,
					data.Expense6,
				]);
			}			
		}
	}
}