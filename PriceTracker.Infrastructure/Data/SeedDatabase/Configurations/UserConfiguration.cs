using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class UserConfiguration : BaseConfiguration<User>
	{
		public UserConfiguration(IDataProvider<User> dataProvider) : base(dataProvider)
		{
		}

		protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
		{
			// Relation configuration
			builder.HasIndex(u => u.UserName)
				.IsUnique();

			builder.HasIndex(u => u.Email)
				.IsUnique();

			// Financial data - we keep them for history
			builder.HasMany(u => u.Expenses)
				   .WithOne(e => e.User)
				   .HasForeignKey(e => e.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(u => u.MonthlyBudgets)
				   .WithOne(mb => mb.User)
				   .HasForeignKey(mb => mb.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			// not a critical data - we can remove it
			builder.HasMany(u => u.Tasks)
				   .WithOne(t => t.User)
				   .HasForeignKey(t => t.UserId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(u => u.Notifications)
				   .WithOne(n => n.User)
				   .HasForeignKey(n => n.UserId)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}