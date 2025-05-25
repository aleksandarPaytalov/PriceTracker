using PriceTracker.Extensions;

var builder = WebApplication.CreateBuilder(args);

//Services registration
builder.Services.DataBaseServiceExtensions(builder.Configuration);
builder.Services.AddIdentityServiceExtensions();
builder.Services.AddSeedingServices(builder.Configuration);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

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

if (args.Contains("--seed"))
{
	await app.SeedDatabaseAsync();
}

app.Run();
