namespace PriceTracker.Core.Models.Navigation
{
	/// <summary>
	/// Represents a single breadcrumb item in the navigation hierarchy
	/// </summary>
	public class BreadcrumbItem
	{
		/// <summary>
		/// Display text for the breadcrumb item
		/// </summary>
		public string Text { get; set; } = string.Empty;

		/// <summary>
		/// URL for the breadcrumb item (null for current page)
		/// </summary>
		public string? Url { get; set; }

		/// <summary>
		/// Icon class for the breadcrumb item (Font Awesome)
		/// </summary>
		public string? Icon { get; set; }

		/// <summary>
		/// Indicates if this is the current/active page
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Controller name for generating URLs
		/// </summary>
		public string? Controller { get; set; }

		/// <summary>
		/// Action name for generating URLs
		/// </summary>
		public string? Action { get; set; }

		/// <summary>
		/// Route values for generating URLs
		/// </summary>
		public object? RouteValues { get; set; }

		/// <summary>
		/// Tooltip text for the breadcrumb item
		/// </summary>
		public string? Tooltip { get; set; }
	}
}
