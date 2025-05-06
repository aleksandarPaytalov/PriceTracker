using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceTracker.Infrastructure.Data.Models
{
	[Comment("User DB Model")]
    public class User : IdentityUser<int>
    {
		[Comment("The time that the user have been created")]
		[NotMapped]
		public DateTime? CreatedAt { get; set; }

		public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
		public virtual ICollection<MonthlyBudget> MonthlyBudgets { get; set; } = new List<MonthlyBudget>();
		public virtual ICollection<ToDoItem> Tasks { get; set; } = new List<ToDoItem>();
		public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

	}
}
