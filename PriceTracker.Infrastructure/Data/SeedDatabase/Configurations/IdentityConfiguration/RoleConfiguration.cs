using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Configuration;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations.IdentityConfiguration
{
	public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
	{
		private readonly IOptions<SeedingOptions> _options;

		public RoleConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options;
		}

		public void Configure(EntityTypeBuilder<IdentityRole> builder)
		{
			if (!_options.Value.UseExternalSource)
			{
				var data = new SeedData();
				data.Initialize();

				builder.HasData(
				[
					data.AdminRole,
				data.UserRole,
				data.GuestRole
				]);
			}			
		}
	}
}
