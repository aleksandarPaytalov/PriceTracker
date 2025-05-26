using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using PriceTracker.Configuration;
using PriceTracker.Infrastructure.Data.Models;
using PriceTracker.Infrastructure.Data.SeedDatabase.Configurations;
using PriceTracker.Infrastructure.Data.SeedDatabase.Configurations.IdentityConfiguration;

public class PriceTrackerDbContext : IdentityDbContext<User>
{
	public PriceTrackerDbContext(DbContextOptions<PriceTrackerDbContext> options)
		: base(options)	{ }


	public DbSet<Product> Products { get; set; } = null!;
	public DbSet<Store> Stores { get; set; } = null!;
	public DbSet<Price> Prices { get; set; } = null!;
	public DbSet<Expense> Expenses { get; set; } = null!;
	public DbSet<MonthlyBudget> MonthlyBudgets { get; set; } = null!;
	public DbSet<ToDoItem> ToDoItems { get; set; } = null!;
	public DbSet<Notification> Notifications { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);		

		var options = this.GetService<IOptions<SeedingOptions>>();

		builder.ApplyConfiguration(new StoreConfiguration(options));
		builder.ApplyConfiguration(new ProductConfiguration(options));
		
		builder.ApplyConfiguration(new RoleConfiguration(options));
		builder.ApplyConfiguration(new UserConfiguration(options));
		builder.ApplyConfiguration(new UserRoleConfiguration(options));
		
		builder.ApplyConfiguration(new PriceConfiguration(options));
		builder.ApplyConfiguration(new ExpenseConfiguration(options));
		builder.ApplyConfiguration(new MonthlyBudgetConfiguration(options));
		builder.ApplyConfiguration(new ToDoItemConfiguration(options));
		builder.ApplyConfiguration(new NotificationConfiguration(options));
	}
}