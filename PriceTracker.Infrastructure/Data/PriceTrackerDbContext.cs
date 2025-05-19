using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

		builder.ApplyConfiguration(new StoreConfiguration());
		builder.ApplyConfiguration(new ProductConfiguration());
		
		builder.ApplyConfiguration(new RoleConfiguration());
		builder.ApplyConfiguration(new UserConfiguration());
		builder.ApplyConfiguration(new UserRoleConfiguration());
		
		builder.ApplyConfiguration(new PriceConfiguration());
		builder.ApplyConfiguration(new ExpenseConfiguration());
		builder.ApplyConfiguration(new MonthlyBudgetConfiguration());
		builder.ApplyConfiguration(new ToDoItemConfiguration());
		builder.ApplyConfiguration(new NotificationConfiguration());
	}
}