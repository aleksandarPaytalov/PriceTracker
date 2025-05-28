using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
	{
		private readonly IOptions<SeedingOptions> _options;

		public NotificationConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options;
		}

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

			if (!_options.Value.UseExternalSource)
			{
				var data = new SeedData();
				data.Initialize();

				builder.HasData(
				[
					data.Notification1,
					data.Notification2,
					data.Notification3
				]);
			}				
		}
	}
}