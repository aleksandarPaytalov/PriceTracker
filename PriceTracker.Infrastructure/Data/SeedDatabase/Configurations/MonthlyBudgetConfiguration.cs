using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Configuration;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class MonthlyBudgetConfiguration : IEntityTypeConfiguration<MonthlyBudget>
	{
		private readonly IOptions<SeedingOptions> _options;

		public MonthlyBudgetConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options;
		}

		public void Configure(EntityTypeBuilder<MonthlyBudget> builder)
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

			if (!_options.Value.UseExternalSource)
			{
				var data = new SeedData();
				data.Initialize();

				builder.HasData(
				[
					data.Budget1,
					data.Budget2,
					data.Budget3
				]);
			}
				
		}
	}
}