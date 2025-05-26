using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using PriceTracker.Configuration;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Configurations.IdentityConfiguration
{
	public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
	{
		private readonly IOptions<SeedingOptions> _options;

		public UserRoleConfiguration(IOptions<SeedingOptions> options)
		{
			_options = options;
		}

		public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
		{
			if (!_options.Value.UseExternalSource)
			{
				var data = new SeedData();
				data.Initialize();

				builder.HasData(
				[
					data.AdminUserRole,
					data.RegularUserRole,
					data.GuestUserRole
				]);
			}
			
		}
	}
}
