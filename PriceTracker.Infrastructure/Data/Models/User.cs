using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceTracker.Infrastructure.Data.Models
{
	[Comment("User DB Model")]
    public class User
    {
		[Key]
		[Comment("User Identifier")]
		public int UserId { get; set; }

		//Must be Unique and should be configured later in the fluent API
		[Required]
		[StringLength(DataConstants.userNameMaxLength)]
		[Comment("UserName")]
		public required string UserName { get; set; }

		//Must be Unique and should be configured later in the fluent API 
		[Required]
		[EmailAddress]
		[StringLength(DataConstants.emailAddressMaxLength)]
		[Comment("User emailAdress")]
		public required string Email { get; set; }

		[Required]
		[StringLength(DataConstants.passwordHashMaxLength)]
		[Comment("PasswordHash of the User")]
		public required string PasswordHash { get; set; }

		[Comment("The time that the user have been created")]
		[NotMapped]
		public DateTime? CreatedAt { get; set; }

		public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
		public virtual ICollection<MonthlyBudget> MonthlyBudgets { get; set; } = new List<MonthlyBudget>();
		public virtual ICollection<ToDoItem> Tasks { get; set; } = new List<ToDoItem>();
		public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

	}
}
