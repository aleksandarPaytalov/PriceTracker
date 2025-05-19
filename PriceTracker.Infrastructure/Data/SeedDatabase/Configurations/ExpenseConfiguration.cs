using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
	{
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

			var data = new SeedData();

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