using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations
{
	public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : class
	{
		protected readonly IDataProvider<T> _dataProvider;
		protected BaseConfiguration(IDataProvider<T> dataProvider)
		{
			_dataProvider = dataProvider;
		}
		
		public void Configure(EntityTypeBuilder<T> builder)
		{
			ConfigureEntity(builder);
			SeedData(builder);
		}

		protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);

		protected virtual void SeedData(EntityTypeBuilder<T> builder)
		{
			var data = _dataProvider.GetData();
			if (data.Any())
			{
				builder.HasData(data);
			}
		}
	}
}
