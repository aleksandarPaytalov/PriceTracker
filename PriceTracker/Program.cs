using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data;
using PriceTracker.Infrastructure.Data.Models;

var builder = WebApplication.CreateBuilder(args);

string dbConnection = builder.Configuration.GetConnectionString("DbConnection") ?? throw new InvalidOperationException("Connection string not found");
builder.Services.AddDbContext<PriceTrackerDbContext>(options => options.UseSqlServer(dbConnection));

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<PriceTrackerDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Repository pattern
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

app.Run();
