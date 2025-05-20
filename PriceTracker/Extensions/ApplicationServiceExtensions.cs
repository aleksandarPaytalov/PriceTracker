using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection DataBaseServiceExtensions(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			string dbConnection = configuration.GetConnectionString("DbConnection") ?? throw new InvalidOperationException("Connection string not found");
			services.AddDbContext<PriceTrackerDbContext>(options => options.UseSqlServer(dbConnection));

			// Register Repository
			services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
			services.AddDatabaseDeveloperPageExceptionFilter();

			return services;
		}

		public static IServiceCollection AddIdentityServiceExtensions(this IServiceCollection services)
		{
			services.AddIdentity<User, IdentityRole>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequiredLength = 6;
			})
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<PriceTrackerDbContext>()
			.AddDefaultTokenProviders()
			.AddDefaultUI();

			return services;
		}

	}
}
