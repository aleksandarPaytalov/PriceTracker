using Microsoft.AspNetCore.Identity;
using PriceTracker.Infrastructure.Data.Models;
using System;

namespace PriceTracker.Infrastructure.Data.SeedDatabase
{
	/// <summary>
	/// Clean SeedData class containing only default hardcoded data
	/// No validation logic, no JSON logic - pure data definitions
	/// </summary>
	public class SeedData
	{
		public Store Store1 { get; set; } = null!;
		public Store Store2 { get; set; } = null!;
		public Store Store3 { get; set; } = null!;
		public Store Store4 { get; set; } = null!;
		public Store Store5 { get; set; } = null!;
		public Store Store6 { get; set; } = null!;
		public Store Store7 { get; set; } = null!;
		public Store Store8 { get; set; } = null!;

		public Product Product1 { get; set; } = null!;
		public Product Product2 { get; set; } = null!;
		public Product Product3 { get; set; } = null!;
		public Product Product4 { get; set; } = null!;
		public Product Product5 { get; set; } = null!;
		public Product Product6 { get; set; } = null!;
		public Product Product7 { get; set; } = null!;
		public Product Product8 { get; set; } = null!;
		public Product Product9 { get; set; } = null!;

		public Price Price1 { get; set; } = null!;
		public Price Price2 { get; set; } = null!;
		public Price Price3 { get; set; } = null!;
		public Price Price4 { get; set; } = null!;
		public Price Price5 { get; set; } = null!;
		public Price Price6 { get; set; } = null!;
		public Price Price7 { get; set; } = null!;
		public Price Price8 { get; set; } = null!;
		public Price Price9 { get; set; } = null!;

		public Expense Expense1 { get; set; } = null!;
		public Expense Expense2 { get; set; } = null!;
		public Expense Expense3 { get; set; } = null!;
		public Expense Expense4 { get; set; } = null!;
		public Expense Expense5 { get; set; } = null!;
		public Expense Expense6 { get; set; } = null!;

		public ToDoItem Task1 { get; set; } = null!;
		public ToDoItem Task2 { get; set; } = null!;
		public ToDoItem Task3 { get; set; } = null!;

		public Notification Notification1 { get; set; } = null!;
		public Notification Notification2 { get; set; } = null!;
		public Notification Notification3 { get; set; } = null!;

		public MonthlyBudget Budget1 { get; set; } = null!;
		public MonthlyBudget Budget2 { get; set; } = null!;
		public MonthlyBudget Budget3 { get; set; } = null!;

		public IdentityRole AdminRole { get; set; } = null!;
		public IdentityRole UserRole { get; set; } = null!;
		public IdentityRole GuestRole { get; set; } = null!;

		public User Guest { get; set; } = null!;
		public User User { get; set; } = null!;
		public User Administrator { get; set; } = null!;

		public IdentityUserRole<string> AdminUserRole { get; set; } = null!;
		public IdentityUserRole<string> RegularUserRole { get; set; } = null!;
		public IdentityUserRole<string> GuestUserRole { get; set; } = null!;

		public SeedData() { }

		/// <summary>
		/// Initialize all default data - no validation, pure data assignment
		/// </summary>
		public void Initialize()
		{
			SeedStores();
			SeedProducts();
			SeedRoles();

			SeedUsers();
			SeedUserRoles();

			SeedPrices();
			SeedTasks();
			SeedNotifications();
			SeedExpenses();
			SeedBudgets();
		}

		/// <summary>
		/// Default roles - hardcoded, no validation
		/// </summary>
		private void SeedRoles()
		{
			AdminRole = new IdentityRole()
			{
				Id = "c3a2cde1-caac-4a6c-a24f-1b5d35b47f59",
				Name = "Administrator",
				NormalizedName = "ADMINISTRATOR",
				ConcurrencyStamp = Guid.NewGuid().ToString()
			};

			UserRole = new IdentityRole
			{
				Id = "c3a2cde1-caac-4a6c-a24f-1b5d35b47f60",
				Name = "User",
				NormalizedName = "USER",
				ConcurrencyStamp = Guid.NewGuid().ToString()
			};

			GuestRole = new IdentityRole
			{
				Id = "c3a2cde1-caac-4a6c-a24f-1b5d35b47f61",
				Name = "Guest",
				NormalizedName = "GUEST",
				ConcurrencyStamp = Guid.NewGuid().ToString()
			};
		}

		/// <summary>
		/// Default users - hardcoded, no validation, no password hashing
		/// Password hashing will be done in UserBuilder
		/// </summary>
		private void SeedUsers()
		{
			Guest = new User()
			{
				Id = "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
				UserName = "guest@mail.com",
				NormalizedUserName = "GUEST@MAIL.COM",
				Email = "guest@mail.com",
				NormalizedEmail = "GUEST@MAIL.COM",
				EmailConfirmed = true,
				SecurityStamp = Guid.NewGuid().ToString(),
				ConcurrencyStamp = Guid.NewGuid().ToString(),
				CreatedAt = new DateTime(2025, 05, 18)
				// PasswordHash will be set by UserBuilder
			};

			// Regular user
			User = new User()
			{
				Id = "8d1f0bdc-f59a-4c8d-9549-98673c32c25d",
				UserName = "user@mail.com",
				NormalizedUserName = "USER@MAIL.COM",
				Email = "user@mail.com",
				NormalizedEmail = "USER@MAIL.COM",
				EmailConfirmed = true,
				SecurityStamp = Guid.NewGuid().ToString(),
				ConcurrencyStamp = Guid.NewGuid().ToString(),
				CreatedAt = new DateTime(2025, 05, 18)
				// PasswordHash will be set by UserBuilder
			};
			
			//Administrator
			Administrator = new User()
			{
				Id = "56f4b198-3f3e-4f72-9a80-7d903bf24a1e",
				UserName = "admin@mail.com",
				NormalizedUserName = "ADMIN@MAIL.COM",
				Email = "admin@mail.com",
				NormalizedEmail = "ADMIN@MAIL.COM",
				EmailConfirmed = true,
				SecurityStamp = Guid.NewGuid().ToString(),
				ConcurrencyStamp = Guid.NewGuid().ToString(),
				CreatedAt = new DateTime(2025, 05, 18)
				// PasswordHash will be set by UserBuilder
			};
		}

		/// <summary>
		/// Default user-role mappings - hardcoded, no validation
		/// </summary>
		private void SeedUserRoles()
		{
			AdminUserRole = new IdentityUserRole<string> 
			{
				UserId = Administrator.Id,   
				RoleId = AdminRole.Id     
			};

			RegularUserRole = new IdentityUserRole<string>
			{
				UserId = User.Id,
				RoleId = UserRole.Id
			};

			GuestUserRole = new IdentityUserRole<string>
			{
				UserId = Guest.Id,
				RoleId = GuestRole.Id
			};
		}

		/// <summary>
		/// Gets default password for a given user (for Builder usage)
		/// </summary>
		public static string GetDefaultPasswordForUser(string email)
		{
			return email switch
			{
				"admin@mail.com" => "Adin132!",
				"user@mail.com" => "Usr132!",
				"guest@mail.com" => "Qew12!",
				_ => "Default123!"
			};
		}

		private void SeedStores()
		{
			Store1 = new Store()
			{
				StoreId = 1,
				Name = "Kaufland",
			};

			Store2 = new Store()
			{
				StoreId = 2,
				Name = "Lidl"
			};

			Store3 = new Store()
			{
				StoreId = 3,
				Name = "Billa"
			};

			Store4 = new Store()
			{
				StoreId = 4,
				Name = "Metro"
			};

			Store5 = new Store()
			{
				StoreId = 5,
				Name = "BBB"
			};

			Store6 = new Store()
			{
				StoreId = 6,
				Name = "T-Market"
			};

			Store7 = new Store()
			{
				StoreId = 7,
				Name = "Marketplace"
			};

			Store8 = new Store()
			{
				StoreId = 8,
				Name = "Lukoil"
			};
		}
		
		private void SeedProducts()
		{
			Product1 = new Product
			{
				ProductId = 1,
				ProductName = "Яйца",
				Brand = "Нашата ферма",
				Quantity = 10,
				Category = ProductCategory.Food
			};

			Product2 = new Product
			{
				ProductId = 2,
				ProductName = "Кашкавал от краве мляко",
				Brand = "Маджаров",
				Quantity = 400,
				Category = ProductCategory.Food
			};

			Product3 = new Product
			{
				ProductId = 3,
				ProductName = "Сирене",
				Brand = "Лакрима",
				Quantity = 700,
				Category = ProductCategory.Food
			};

			Product4 = new Product
			{
				ProductId = 4,
				ProductName = "Бензин",
				Brand = " А95",
				Quantity = 1,
				Category = ProductCategory.Transportation
			};

			Product5 = new Product
			{
				ProductId = 5,
				ProductName = "Кисело мляко 2%",
				Brand = "Верея",
				Quantity = 400,
				Category = ProductCategory.Food
			};

			Product6 = new Product
			{
				ProductId = 6,
				ProductName = "Филе Елена",
				Brand = "Орехите",
				Quantity = 100,
				Category = ProductCategory.Food
			};

			Product7 = new Product
			{
				ProductId = 7,
				ProductName = "Газ",
				Brand = "Auto-gas",
				Quantity = 1,
				Category = ProductCategory.Transportation
			};

			Product8 = new Product
			{
				ProductId = 8,
				ProductName = "Хляб с лимец",
				Brand = "Добруджа",
				Quantity = 500,
				Category = ProductCategory.Food
			};

			Product9 = new Product
			{
				ProductId = 9,
				ProductName = "Прясно мляко лешник",
				Brand = "Верея",
				Quantity = 1000,
				Category = ProductCategory.Food
			};
		}
		
		private void SeedPrices()
		{
			Price1 = new Price()
			{
				PriceId = 1,
				ProductId = Product1.ProductId,
				StoreId = Store1.StoreId,
				SellingPrice = 4.50m,
				DateChecked = new DateTime(2025, 05, 18)
			};

			Price2 = new Price()
			{
				PriceId = 2,
				ProductId = Product2.ProductId,
				StoreId = Store5.StoreId,
				SellingPrice = 17.59m,
				DateChecked = new DateTime(2025, 05, 18)
			};

			Price3 = new Price()
			{
				PriceId = 3,
				ProductId = Product3.ProductId,
				StoreId = Store1.StoreId,
				SellingPrice = 16.00m,
				DateChecked = new DateTime(2025, 05, 18)
			};

			Price4 = new Price()
			{
				PriceId = 4,
				ProductId = Product4.ProductId,
				StoreId = Store8.StoreId,
				SellingPrice = 2.55m,
				DateChecked = new DateTime(2025, 05, 18)
			};

			Price5 = new Price()
			{
				PriceId = 5,
				ProductId = Product5.ProductId,
				StoreId = Store3.StoreId,
				SellingPrice = 1.55m,
				DateChecked = new DateTime(2025, 05, 18)
			};

			Price6 = new Price()
			{
				PriceId = 6,
				ProductId = Product6.ProductId,
				StoreId = Store1.StoreId,
				SellingPrice = 24.99m,
				DateChecked = new DateTime(2025, 05, 18)
			};

			Price7 = new Price()
			{
				PriceId = 7,
				ProductId = Product7.ProductId,
				StoreId = Store8.StoreId,
				SellingPrice = 1.25m,
				DateChecked = new DateTime(2025, 05, 18)
			};

			Price8 = new Price()
			{
				PriceId = 8,
				ProductId = Product8.ProductId,
				StoreId = Store5.StoreId,
				SellingPrice = 2.50m,
				DateChecked = new DateTime(2025, 05, 18)
			};

			Price9 = new Price()
			{
				PriceId = 9,
				ProductId = Product9.ProductId,
				StoreId = Store3.StoreId,
				SellingPrice = 5.50m,
				DateChecked = new DateTime(2025, 05, 18)
			};
		}

		private void SeedBudgets()
		{
			Budget1 = new MonthlyBudget()
			{
				BudgedId = 1,
				BudgetAmount = 1200,
				Month = Month.April,
				UserId = Administrator.Id
			};

			Budget2 = new MonthlyBudget()
			{
				BudgedId = 2,
				BudgetAmount = 1300,
				Month = Month.May,
				UserId = Administrator.Id
			};

			Budget3 = new MonthlyBudget()
			{
				BudgedId = 3,
				BudgetAmount = 1300,
				Month = Month.June,
				UserId = Administrator.Id
			};
		}

		private void SeedExpenses()
		{
			Expense1 = new Expense()
			{
				ExpenseId = 1,
				AmountSpent = 31.59m,
				DateSpent = new DateTime(2025, 5, 18).AddDays(-1),
				Description = "Заредена газ за пътуване до София",
				ExpenseType = ExpenseType.Car,
				ProductId = Product7.ProductId,
				StoreId = Store8.StoreId,
				UserId = Administrator.Id
			};

			Expense2 = new Expense()
			{
				ExpenseId = 2,
				AmountSpent = 4.50m,
				DateSpent = new DateTime(2025, 5, 18).AddDays(-1),
				Description = "Покупка на яйца размер L",
				ExpenseType = ExpenseType.Food,
				ProductId = Product1.ProductId,
				StoreId = Store1.StoreId,
				UserId = Administrator.Id
			};

			Expense3 = new Expense()
			{
				ExpenseId = 3,
				AmountSpent = 14.50m,
				DateSpent = new DateTime(2025, 5, 18).AddDays(-1),
				Description = "Кашкавал",
				ExpenseType = ExpenseType.Food,
				ProductId = Product2.ProductId,
				StoreId = Store1.StoreId,
				UserId = Administrator.Id
			};

			Expense4 = new Expense()
			{
				ExpenseId = 4,
				AmountSpent = 50.00m,
				DateSpent = new DateTime(2025, 5, 18).AddDays(-1),
				Description = "Бензин",
				ExpenseType = ExpenseType.Car,
				ProductId = Product4.ProductId,
				StoreId = Store8.StoreId,
				UserId = Administrator.Id
			};

			Expense5 = new Expense()
			{
				ExpenseId = 5,
				AmountSpent = 12.50m,
				DateSpent = new DateTime(2025, 5, 18).AddDays(-1),
				Description = "Сирене за баница",
				ExpenseType = ExpenseType.Food,
				ProductId = Product3.ProductId,
				StoreId = Store5.StoreId,
				UserId = Administrator.Id
			};

			Expense6 = new Expense()
			{
				ExpenseId = 6,
				AmountSpent = 12.50m,
				DateSpent = new DateTime(2025, 5, 18).AddDays(-3),
				Description = "Месо за мезе",
				ExpenseType = ExpenseType.Food,
				ProductId = Product6.ProductId,
				StoreId = Store2.StoreId,
				UserId = Administrator.Id
			};
		}
		
		private void SeedNotifications()
		{
			Notification1 = new Notification()
			{
				NotificationId = 1,
				CreatedAt = DateTime.UtcNow,
				IsRead = true,
				Message = "Reminder to buy a food for next two week",
				NotificationTime = DateTime.UtcNow.AddDays(5),
				TaskId = Task1.TaskId, 
				UserId = Administrator.Id
			};

			Notification2 = new Notification()
			{
				NotificationId = 2,
				CreatedAt = DateTime.UtcNow,
				IsRead = false,
				Message = "Reminder to fill up the car with gas and check it, so it is ready for the trip next week.",
				NotificationTime = DateTime.Now.AddDays(5),
				TaskId = Task2.TaskId,
				UserId = Administrator.Id
			};

			Notification3 = new Notification()
			{
				NotificationId = 3,
				CreatedAt = DateTime.UtcNow,
				IsRead = true,
				Message = "Check and pay the bills",
				NotificationTime = DateTime.UtcNow.AddDays(5),
				TaskId = Task3.TaskId,
				UserId = Administrator.Id
			};
		}
		
		private void SeedTasks()
		{
			Task1 = new ToDoItem()
			{
				TaskId = 1,
				CreatedAt = DateTime.UtcNow.AddDays(-1),
				Description = "Buy food for the next two weeks",
				DueDate = DateTime.UtcNow.AddDays(10),
				TaskStatus = Models.TaskStatus.Pending,
				Title = "Shopping",
				UserId = Administrator.Id
			};

			Task2 = new ToDoItem()
			{
				TaskId = 2,
				CreatedAt = DateTime.UtcNow.AddDays(-2),
				Description = "Get the car ready for the trip",
				DueDate = DateTime.UtcNow.AddDays(10),
				TaskStatus = Models.TaskStatus.InProgress,
				Title = "Fill with fuel",
				UserId = Administrator.Id
			};

			Task3 = new ToDoItem()
			{
				TaskId = 3,
				CreatedAt = DateTime.UtcNow.AddDays(-3),
				Description = "Pay the bills for this month.",
				DueDate = DateTime.Now.AddDays(10),
				TaskStatus = Models.TaskStatus.Completed,
				Title = "House bills",
				UserId = Administrator.Id
			};
		}
	}
}
