using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class NotificationConfiguration : BaseConfiguration<Notification>
	{
		public NotificationConfiguration(IDataProvider<Notification> dataProvider) : base(dataProvider)
		{
		}

		protected override void ConfigureEntity(EntityTypeBuilder<Notification> builder)
		{
			// Relations Config
			builder.HasOne(n => n.User)
				   .WithMany(u => u.Notifications)
				   .HasForeignKey(n => n.UserId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(n => n.Task)
				   .WithMany(t => t.Notifications)
				   .HasForeignKey(n => n.TaskId)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}