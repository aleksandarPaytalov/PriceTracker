using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class MonthlyBudgetConfiguration : BaseConfiguration<MonthlyBudget>
	{
		public MonthlyBudgetConfiguration(IDataProvider<MonthlyBudget> dataProvider) : base(dataProvider)
		{
		}

		protected override void ConfigureEntity(EntityTypeBuilder<MonthlyBudget> builder)
		{
			// Unique composite index
			builder.HasIndex(mb => new { mb.UserId, mb.Month })
				   .IsUnique();

			// Configuration of the relations
			// We keep history for tracking the monthly budget
			builder.HasOne(mb => mb.User)
				   .WithMany(u => u.MonthlyBudgets)
				   .HasForeignKey(mb => mb.UserId)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}
}