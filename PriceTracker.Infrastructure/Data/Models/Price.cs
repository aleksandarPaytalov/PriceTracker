using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceTracker.Infrastructure.Data.Models
{
    [Comment("Price Db model")]
    public class Price
    {
		[Key]
		[Comment("Price identifier")]
		public int PriceId { get; set; }

		[Required]
		[Comment("Product identifier")]
		public int ProductId { get; set; }

		[Required]
		[ForeignKey(nameof(ProductId))]
		[Comment("Product navigation property")]
		public Product Product { get; set; } = null!;

		[Required]
		[Comment("Store identifier")]
		public int StoreId { get; set; }

		[Required]
		[ForeignKey(nameof(StoreId))]
		[Comment("Store navigation property")]
		public Store Store { get; set; } = null!;

		[Required]
		[Comment("Current price of a product in the store")]
		public decimal SellingPrice { get; set; }

		[Comment("The date of the record for the price on a product")]
		public DateTime? DateChecked { get; set; }

	}
}
