using PriceTracker.Extensions;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.SeedDatabase.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Register SeedingOptions configuration with validation
builder.Services.AddSeedingConfiguration(builder.Configuration);

// Register FileLogger as IAppLogger
builder.Services.AddSingleton<IAppLogger>(provider =>
{
	var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
	return new FileLogger(logPath, logToConsole: true);
});



//Services registration
builder.Services.DataBaseServiceExtensions(builder.Configuration);
builder.Services.AddIdentityServiceExtensions();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Set up logger for MigrationDataHelper before any migrations run
using (var scope = app.Services.CreateScope())
{
	var logger = scope.ServiceProvider.GetRequiredService<IAppLogger>();

	// Set runtime logger for MigrationLogger (new approach)
	MigrationLogger.SetRuntimeLogger(logger);

	// Set logger for MigrationDataHelper (backward compatibility)
	MigrationDataHelper.SetLogger(logger);

	// Log application startup with enhanced information
	logger.LogInformation("PriceTracker application starting up with MigrationLogger support enabled");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await app.RunAsync();