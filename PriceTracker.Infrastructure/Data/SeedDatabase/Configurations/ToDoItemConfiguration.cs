using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public class ToDoItemConfiguration : BaseConfiguration<ToDoItem>
	{
		public ToDoItemConfiguration(IDataProvider<ToDoItem> dataProvider) : base(dataProvider)
		{
		}

		protected override void ConfigureEntity(EntityTypeBuilder<ToDoItem> builder)
		{
			// Relation Configuration
			builder.HasOne(t => t.User)
				   .WithMany(u => u.Tasks)
				   .HasForeignKey(t => t.UserId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(t => t.Notifications)
				   .WithOne(n => n.Task)
				   .HasForeignKey(n => n.TaskId)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}