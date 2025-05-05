using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Infrastructure.Data
{
    public class PriceTrackerDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
		public PriceTrackerDbContext(DbContextOptions<PriceTrackerDbContext> options)
			: base(options)
		{
			
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
			builder.Entity<User>()
				.HasIndex(u => u.UserName)
				.IsUnique();

			builder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();


			base.OnModelCreating(builder);
		}
	}
}
