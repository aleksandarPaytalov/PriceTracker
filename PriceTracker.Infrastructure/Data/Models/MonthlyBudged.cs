using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceTracker.Infrastructure.Data.Models
{
    [Comment("MonthlyBudged Db model")]
    public class MonthlyBudged
    {
        [Key]
        [Comment("Budged identifier")]
		public int BudgedId { get; set; }

        [Required]
        [Comment("User identifier")]
		public int UserId { get; set; }

        [Required]
        [ForeignKey(nameof(UserId))]
        [Comment("User navigation property")]
        public User User { get; set; } = null!;

        [Required]
        [Comment("Total amount of money or budged we have for the current month")]
		public decimal BudgedAmount { get; set; }

        [Required]
        [Comment("Month we spend current budged in")]
		public Month Month { get; set; } 
	}
}
