using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Data.Models;

public class PriceTrackerDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
	private readonly IEnumerable<IEntityTypeConfiguration<User>> _userConfigurations;
	private readonly IEnumerable<IEntityTypeConfiguration<Store>> _storeConfigurations;
	private readonly IEnumerable<IEntityTypeConfiguration<Product>> _productConfigurations;
	private readonly IEnumerable<IEntityTypeConfiguration<Price>> _priceConfigurations;
	private readonly IEnumerable<IEntityTypeConfiguration<Expense>> _expenseConfigurations;
	private readonly IEnumerable<IEntityTypeConfiguration<MonthlyBudget>> _budgetConfigurations;
	private readonly IEnumerable<IEntityTypeConfiguration<ToDoItem>> _todoConfigurations;
	private readonly IEnumerable<IEntityTypeConfiguration<Notification>> _notificationConfigurations;

	public PriceTrackerDbContext(
		DbContextOptions<PriceTrackerDbContext> options,
		IEnumerable<IEntityTypeConfiguration<User>> userConfigurations,
		IEnumerable<IEntityTypeConfiguration<Store>> storeConfigurations,
		IEnumerable<IEntityTypeConfiguration<Product>> productConfigurations,
		IEnumerable<IEntityTypeConfiguration<Price>> priceConfigurations,
		IEnumerable<IEntityTypeConfiguration<Expense>> expenseConfigurations,
		IEnumerable<IEntityTypeConfiguration<MonthlyBudget>> budgetConfigurations,
		IEnumerable<IEntityTypeConfiguration<ToDoItem>> todoConfigurations,
		IEnumerable<IEntityTypeConfiguration<Notification>> notificationConfigurations)
		: base(options)
	{
		_userConfigurations = userConfigurations;
		_storeConfigurations = storeConfigurations;
		_productConfigurations = productConfigurations;
		_priceConfigurations = priceConfigurations;
		_expenseConfigurations = expenseConfigurations;
		_budgetConfigurations = budgetConfigurations;
		_todoConfigurations = todoConfigurations;
		_notificationConfigurations = notificationConfigurations;
	}

	public DbSet<Product> Products { get; set; }
	public DbSet<Store> Stores { get; set; }
	public DbSet<Price> Prices { get; set; }
	public DbSet<Expense> Expenses { get; set; }
	public DbSet<MonthlyBudget> MonthlyBudgets { get; set; }
	public DbSet<ToDoItem> ToDoItems { get; set; }
	public DbSet<Notification> Notifications { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		foreach (var configuration in _userConfigurations)
		{
			configuration.Configure(builder.Entity<User>());
		}
		foreach (var configuration in _storeConfigurations)
		{
			configuration.Configure(builder.Entity<Store>());
		}
		foreach (var configuration in _productConfigurations)
		{
			configuration.Configure(builder.Entity<Product>());
		}
		foreach (var configuration in _priceConfigurations)
		{
			configuration.Configure(builder.Entity<Price>());
		}
		foreach (var configuration in _expenseConfigurations)
		{
			configuration.Configure(builder.Entity<Expense>());
		}
		foreach (var configuration in _budgetConfigurations)
		{
			configuration.Configure(builder.Entity<MonthlyBudget>());
		}
		foreach (var configuration in _todoConfigurations)
		{
			configuration.Configure(builder.Entity<ToDoItem>());
		}
		foreach (var configuration in _notificationConfigurations)
		{
			configuration.Configure(builder.Entity<Notification>());
		}
		
		base.OnModelCreating(builder);
	}
}