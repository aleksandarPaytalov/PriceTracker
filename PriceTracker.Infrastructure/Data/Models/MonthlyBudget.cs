﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceTracker.Infrastructure.Data.Models
{
    [Comment("MonthlyBudget Db model")]
    public class MonthlyBudget
    {
        [Key]
        [Comment("Budged identifier")]
		public int BudgedId { get; set; }

        [Required]
        [Comment("User identifier")]
		public required string UserId { get; set; }

        [Required]
        [ForeignKey(nameof(UserId))]
        [Comment("User navigation property")]
        public virtual User User { get; set; } = null!;

		[Required]
		[Column(TypeName = "decimal(10,2)")]
		[Comment("Total amount of money or budged we have for the current month")]
		public decimal BudgetAmount { get; set; }

        [Required]
        [Comment("Month we spend current budged in")]
		public required Month Month { get; set; } 
	}
}
