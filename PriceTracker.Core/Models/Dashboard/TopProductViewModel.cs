namespace PriceTracker.Core.Models.Dashboard
{
	/// <summary>
	/// Top product data for the Top 5 Products widget
	/// </summary>
	public class TopProductViewModel
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public string Brand { get; set; } = string.Empty;
		public int PurchaseCount { get; set; }
		public decimal LastPrice { get; set; }
		public decimal AveragePrice { get; set; }
		public string? ImageUrl { get; set; }
		public DateTime LastPurchased { get; set; }
		public string Category { get; set; } = string.Empty;
	}
}
