using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class ExpenseConfiguration : BaseConfiguration<Expense>
	{
		public ExpenseConfiguration(IDataProvider<Expense> dataProvider) : base(dataProvider)
		{
		}

		protected override void ConfigureEntity(EntityTypeBuilder<Expense> builder)
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
		}
	}
}