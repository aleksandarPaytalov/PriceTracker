using Microsoft.Extensions.Options;
using PriceTracker.Extensions;
using PriceTracker.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Services registration
builder.Services.AddLoggingConfiguration();
builder.Services.AddSeedingConfiguration(builder.Configuration);
builder.Services.DataBaseServiceExtensions(builder.Configuration);
builder.Services.AddIdentityServiceExtensions(builder.Configuration);
builder.Services.AddEmailServices(builder.Configuration);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure migration loggers
app.ConfigureMigrationLoggersAsync();

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