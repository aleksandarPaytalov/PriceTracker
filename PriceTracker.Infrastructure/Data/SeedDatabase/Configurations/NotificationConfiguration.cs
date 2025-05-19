using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
	{
		public void Configure(EntityTypeBuilder<Notification> builder)
		{
			// Relations Config
			builder.HasOne(n => n.User)
				   .WithMany(u => u.Notifications)
				   .HasForeignKey(n => n.UserId)
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(n => n.Task)
				   .WithMany(t => t.Notifications)
				   .HasForeignKey(n => n.TaskId)
				   .OnDelete(DeleteBehavior.NoAction);

			var data = new SeedData();

			builder.HasData(
				[
					data.Notification1,
					data.Notification2,
					data.Notification3
				]);
		}
	}
}