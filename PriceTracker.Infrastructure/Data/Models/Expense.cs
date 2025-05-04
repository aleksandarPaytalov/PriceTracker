using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceTracker.Infrastructure.Data.Models
{
    [Comment("Expense Db model")]
    public class Expense
    {
        [Key]
        [Comment("Expense identifier")]
		public int ExpenseId { get; set; }

        [Required]
        [Comment("User identifier")]
		public int UserId { get; set; }

        [Required]
        [ForeignKey(nameof(UserId))]
        [Comment("User navigation property")]
        public required virtual User User { get; set; }

        [Required]
        [Comment("Expense type")]
		public ExpenseType ExpenseType { get; set; }

        [Required]
        [Comment("Product identifier")]
		public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        [Comment("Product navigation property")]
        public virtual Product? Product { get; set; }

        [Required]
        [Comment("Store identifier")]
		public int StoreId { get; set; } 
        [ForeignKey(nameof(StoreId))]
        [Comment("Store navigation property")]
        public virtual Store? Store { get; set; }

        [StringLength(DataConstants.expenseDescriptionMaxLength)]
        [Comment("Description for maked expense")]
		public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Comment("Amount of money spent")]
		public decimal AmountSpent { get; set; }

        [Required]
        [Comment("Date when the expense was made")]
		public required DateTime DateSpent { get; set; }
	}
}
