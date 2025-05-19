using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations.IdentityConfiguration
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			// Relation configuration
			builder.HasIndex(u => u.UserName).IsUnique();
			builder.HasIndex(u => u.Email).IsUnique();

			builder.HasMany(u => u.Expenses)
				   .WithOne(e => e.User)
				   .HasForeignKey(e => e.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(u => u.MonthlyBudgets)
				   .WithOne(mb => mb.User)
				   .HasForeignKey(mb => mb.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(u => u.Tasks)
				   .WithOne(t => t.User)
				   .HasForeignKey(t => t.UserId)
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(u => u.Notifications)
				   .WithOne(n => n.User)
				   .HasForeignKey(n => n.UserId)
				   .OnDelete(DeleteBehavior.NoAction);

			var data = new SeedData();

			builder.HasData(
			[
				data.Guest,
				data.User,
				data.Administrator,
			]);
		}
	}
}