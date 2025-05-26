using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Configuration;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
	{
		private readonly IOptions<SeedingOptions> _options;

		public ToDoItemConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options;
		}

		public void Configure(EntityTypeBuilder<ToDoItem> builder)
		{
			// Relation Configuration
			builder.HasOne(t => t.User)
				   .WithMany(u => u.Tasks)
				   .HasForeignKey(t => t.UserId)
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(t => t.Notifications)
				   .WithOne(n => n.Task)
				   .HasForeignKey(n => n.TaskId)
				   .OnDelete(DeleteBehavior.NoAction);

			if (!_options.Value.UseExternalSource)
			{
				var data = new SeedData();
				data.Initialize();

				builder.HasData(
				[
					data.Task1,
					data.Task2,
					data.Task3
				]);
			}				
		}
	}
}