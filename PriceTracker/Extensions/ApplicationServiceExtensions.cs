using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.ExternalSeederConfiguration;

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
		/// Configures seeding options and validates JSON files if external source is enabled
		/// </summary>
		public static IServiceCollection AddSeedingConfiguration(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			// Register the configuration
			services.Configure<SeedingOptions>(configuration.GetSection("SeedingOptions"));

			// Get the seeding options for validation
			var seedingOptions = configuration.GetSection("SeedingOptions").Get<SeedingOptions>();

			if (seedingOptions?.UseExternalSource == true)
			{
				ValidateJsonFiles(seedingOptions);

				// Log configuration summary
				Console.WriteLine($"✅ External seeding source enabled with validation");
				Console.WriteLine($"   - Strict validation: {seedingOptions.StrictValidation}");
				Console.WriteLine($"   - Validation logging: {seedingOptions.EnableValidationLogging}");
				Console.WriteLine($"   - Max errors to log: {seedingOptions.MaxValidationErrorsToLog}");

				var enabledSeeders = seedingOptions.EnabledSeeders.Where(kvp => kvp.Value).Select(kvp => kvp.Key);
				Console.WriteLine($"   - Enabled seeders: {string.Join(", ", enabledSeeders)}");
			}
			else
			{
				Console.WriteLine("✅ Using default seed data (external source disabled)");
			}

			return services;
		}

		private static void ValidateJsonFiles(SeedingOptions options)
		{
			var basePath = AppDomain.CurrentDomain.BaseDirectory;
			var dataPath = Path.Combine(basePath, "Data", "SeedDatabase", "JsonData");
			var missingFiles = new List<string>();

			foreach (var enabledSeeder in options.EnabledSeeders.Where(kvp => kvp.Value))
			{
				var fileName = enabledSeeder.Key.ToLower() switch
				{
					"product" => options.ProductJsonFile,
					"store" => options.StoreJsonFile,
					"price" => options.PriceJsonFile,
					"expense" => options.ExpenseJsonFile,
					"task" => options.TaskJsonFile,
					"notification" => options.NotificationJsonFile,
					"budget" => options.BudgetJsonFile,
					"role" => options.RoleJsonFile,
					"user" => options.UserJsonFile,
					"userrole" => options.UserRoleJsonFile,
					_ => null
				};

				Console.WriteLine($"🔍 FileName for {enabledSeeder.Key}: {fileName}");

				if (!string.IsNullOrEmpty(fileName))
				{
					var filePath = Path.Combine(dataPath, fileName);
					Console.WriteLine($"🔍 Full file path: {filePath}");
					Console.WriteLine($"🔍 File exists: {File.Exists(filePath)}");

					if (!File.Exists(filePath))
					{
						Console.WriteLine($"❌ FILE MISSING: {filePath}");
						missingFiles.Add($"{enabledSeeder.Key}: {filePath}");
					}
					else
					{
						// Check if file has content
						var fileInfo = new FileInfo(filePath);
						Console.WriteLine($"🔍 File size: {fileInfo.Length} bytes");

						if (fileInfo.Length == 0)
						{
							Console.WriteLine($"❌ FILE EMPTY: {filePath}");
							missingFiles.Add($"{enabledSeeder.Key}: {filePath} (file is empty)");
						}
						else
						{
							Console.WriteLine($"✅ File OK: {fileName} ({fileInfo.Length} bytes)");
							var content = File.ReadAllText(filePath);
							Console.WriteLine($"🔍 Content preview: {content.Substring(0, Math.Min(50, content.Length))}...");
						}
					}
				}
				else
				{
					Console.WriteLine($"⚠️ No filename mapped for seeder: {enabledSeeder.Key}");
				}
			}

			if (missingFiles.Count != 0)
			{
				Console.WriteLine($"❌ Missing files detected: {missingFiles.Count}");
				foreach (var missing in missingFiles)
				{
					Console.WriteLine($"   - {missing}");
				}

				var errorMessage = "Required JSON files are missing or empty:" + Environment.NewLine +
								 string.Join(Environment.NewLine, missingFiles.Select(f => $" - {f}")) + Environment.NewLine +
								 "Either provide the missing files or disable the corresponding seeders in EnabledSeeders configuration.";

				throw new FileNotFoundException(errorMessage);
			}
			Console.WriteLine("✅ All JSON files validation passed");
		}
	}
}
