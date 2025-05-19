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
		public required string UserId { get; set; }

        [Required]
        [ForeignKey(nameof(UserId))]
        [Comment("User navigation property")]
        public virtual User User { get; set; } = null!;

		[Required]
        [Comment("Expense type")]
		public ExpenseType ExpenseType { get; set; }

        [Required]
        [Comment("Product identifier")]
		public int ProductId { get; set; }

        [Required]
        [ForeignKey(nameof(ProductId))]
        [Comment("Product navigation property")]
        public virtual Product Product { get; set; } = null!;

		[Required]
        [Comment("Store identifier")]
		public int StoreId { get; set; }

        [Required]
        [ForeignKey(nameof(StoreId))]
        [Comment("Store navigation property")]
        public virtual Store Store { get; set; } = null!;

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
