using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Infrastructure.Data.Models
{
    [Comment("Store Db model")]
    public class Store
    {
        [Key]
        [Comment("Store identifier")]
		public int StoreId { get; set; }

        [Required]
        [StringLength(DataConstants.storeNameMaxLength)]
        [Comment("Store name")]
        public required string Name { get; set; }

        [Comment("One store can offer many Products with a different prices.")]
		public virtual ICollection<Price> Prices { get; set; } = new List<Price>();

        [Comment("Many Expenses can be made in one Store")]
		public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

	}
}
