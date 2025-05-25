using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Configuration;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders;
using PriceTracker.Infrastructure.Data.SeedDatabase.DataProviders.DataSources;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeeders;
using PriceTracker.Infrastructure.Data.SeedDatabase.Services;

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

		public static IServiceCollection AddIdentityServiceExtensions(
			this IServiceCollection services)
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

		/// <summary>
		/// Registers the services needed for seeding data from external sources
		/// </summary>
		public static IServiceCollection AddSeedingServices(this IServiceCollection services, IConfiguration configuration)
		{
			// Registration of seeding configuration
			services.Configure<SeedingOptions>(
				configuration.GetSection("SeedingOptions"));

			services.AddSingleton<IAppLogger, FileLogger>();
			services.AddScoped<IDataSourceFactory, DataSourceFactory>();
			services.AddScoped<IDataProviderFactory<Product>, ProductDataProviderFactory>();
			services.AddScoped<ISeederService, SeederService>();

			// Registration of all seeders
			services.AddScoped<ISeeder, ProductSeeder>();
			// TODO: Add other seeders as needed			

			return services;
		}

		/// <summary>
		/// Seeds the database from an external JSON source
		/// </summary>
		public static async Task SeedDatabaseAsync(this IHost host)
		{
			using var scope = host.Services.CreateScope();
			var seederService = scope.ServiceProvider.GetRequiredService<ISeederService>();
			await seederService.SeedAllAsync();
		}
	}
}
