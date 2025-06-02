using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Constants;
using PriceTracker.Core.Services;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Configuration;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;
using PriceTracker.Infrastructure.Services;

namespace PriceTracker.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection DataBaseServiceExtensions(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			string dbConnection = configuration.GetConnectionString("DbConnection") ??
				throw new InvalidOperationException("Connection string not found");

			services.AddDbContext<PriceTrackerDbContext>(options =>
				options.UseSqlServer(dbConnection));

			services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
			services.AddDatabaseDeveloperPageExceptionFilter();

			return services;
		}

		public static IServiceCollection AddIdentityServiceExtensions(
			this IServiceCollection services)
		{
			services.AddIdentity<User, IdentityRole>(options =>
			{
				// Email confirmation required
				options.SignIn.RequireConfirmedAccount = true;
				options.SignIn.RequireConfirmedEmail = true;

				// Password requirements
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequiredLength = 8;
				options.Password.RequiredUniqueChars = 1;

				// User settings
				options.User.RequireUniqueEmail = true;

				// Lockout settings
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				// Token settings
				options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
				options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
			})
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<PriceTrackerDbContext>()
			.AddDefaultTokenProviders()
			.AddDefaultUI();

			return services;
		}

		public static IServiceCollection AddEmailServices(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			// Configure email settings
			services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

			// Register email services
			services.AddScoped<IEmailService, EmailService>();
			services.AddScoped<IEmailSender, CustomEmailSender>();

			return services;
		}

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
				Console.WriteLine(Messages.ExternalSeedingEnabled);
				Console.WriteLine(Messages.StrictValidationTemplate, seedingOptions.StrictValidation);
				Console.WriteLine(Messages.ValidationLoggingTemplate, seedingOptions.EnableValidationLogging);
				Console.WriteLine(Messages.MaxErrorsToLogTemplate, seedingOptions.MaxValidationErrorsToLog);

				var enabledSeeders = seedingOptions.EnabledSeeders.Where(kvp => kvp.Value).Select(kvp => kvp.Key);
				Console.WriteLine(Messages.EnabledSeedersTemplate, string.Join(", ", enabledSeeders));
			}
			else
			{
				Console.WriteLine(Messages.DefaultSeedDataUsed);
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

				Console.WriteLine(Messages.FileNameTemplate, enabledSeeder.Key, fileName);

				if (!string.IsNullOrEmpty(fileName))
				{
					var filePath = Path.Combine(dataPath, fileName);
					Console.WriteLine(Messages.FullFilePathTemplate, filePath);
					Console.WriteLine(Messages.FileExistsTemplate, File.Exists(filePath));

					if (!File.Exists(filePath))
					{
						Console.WriteLine(Messages.FileMissingTemplate, filePath);
						missingFiles.Add($"{enabledSeeder.Key}: {filePath}");
					}
					else
					{
						// Check if file has content
						var fileInfo = new FileInfo(filePath);
						Console.WriteLine(Messages.FileSizeTemplate, fileInfo.Length);

						if (fileInfo.Length == 0)
						{
							Console.WriteLine(Messages.FileEmptyTemplate, filePath);
							missingFiles.Add($"{enabledSeeder.Key}: {filePath} (file is empty)");
						}
						else
						{
							Console.WriteLine(Messages.FileOkTemplate, fileName, fileInfo.Length);
							var content = File.ReadAllText(filePath);
							Console.WriteLine(Messages.ContentPreviewTemplate,
								content.Substring(0, Math.Min(50, content.Length)));
						}
					}
				}
				else
				{
					Console.WriteLine(Messages.NoFilenameMappedTemplate, enabledSeeder.Key);
				}
			}

			if (missingFiles.Count != 0)
			{
				Console.WriteLine(Messages.MissingFilesDetectedTemplate, missingFiles.Count);
				foreach (var missing in missingFiles)
				{
					Console.WriteLine(Messages.MissingFileItemTemplate, missing);
				}

				var errorMessage = Messages.RequiredJsonFilesMissing + Environment.NewLine +
								 string.Join(Environment.NewLine, missingFiles.Select(f => $" - {f}")) + Environment.NewLine +
								 Messages.JsonFilesErrorSuffix;

				throw new FileNotFoundException(errorMessage);
			}

			Console.WriteLine(Messages.AllJsonFilesValidationPassed);
		}

		public static void ConfigureMigrationLoggersAsync(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var logger = scope.ServiceProvider.GetRequiredService<IAppLogger>();

			// Set runtime logger for MigrationLogger (new approach)
			MigrationLogger.SetRuntimeLogger(logger);

			// Set logger for MigrationDataHelper (backward compatibility)
			MigrationDataHelper.SetLogger(logger);

			// Log application startup with enhanced information
			logger.LogInformation(Messages.ApplicationStartupMessage);
		}

		public static IServiceCollection AddLoggingConfiguration(
			this IServiceCollection services)
		{
			// Register FileLogger as IAppLogger
			services.AddSingleton<IAppLogger>(provider =>
			{
				var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
				return new FileLogger(logPath, logToConsole: true);
			});

			return services;
		}
	}
}