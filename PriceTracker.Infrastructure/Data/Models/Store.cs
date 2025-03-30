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
        public string Name { get; set; } = string.Empty;
	}
}
