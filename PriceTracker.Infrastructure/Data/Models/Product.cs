using Microsoft.EntityFrameworkCore;
using PriceTracker.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Infrastructure.Data.Models
{
	[Comment("Product Db model")]
    public class Product
    {
		[Key]
		[Comment("Product identifier")]
		public int ProductId { get; set; }

		[Required]
		[Comment("Product quantity")]
		public int Quantity { get; set; }

		[Required]
		[StringLength(DataConstants.productNameMaxLength)]
		[Comment("Product name")]
		public string ProductName { get; set; } = string.Empty;

		[Required]
		[StringLength(DataConstants.productBrandNameMaxLength)]
		[Comment("Product brand")]
		public string Brand { get; set; } = string.Empty;

		[Required]
		[StringLength(DataConstants.productCategoryMaxLength)]
		[Comment("Product category")]
		public ProductCategory Category { get; set; }
	}
}
