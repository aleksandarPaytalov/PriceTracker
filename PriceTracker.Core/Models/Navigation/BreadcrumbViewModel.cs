namespace PriceTracker.Core.Models.Navigation
{
	/// <summary>
	/// ViewModel for the breadcrumb navigation component
	/// </summary>
	public class BreadcrumbViewModel
	{
		/// <summary>
		/// List of breadcrumb items in hierarchical order
		/// </summary>
		public List<BreadcrumbItem> Items { get; set; } = new();

		/// <summary>
		/// Indicates whether to show the breadcrumb navigation
		/// </summary>
		public bool ShowBreadcrumb { get; set; } = true;

		/// <summary>
		/// CSS class for the breadcrumb container
		/// </summary>
		public string ContainerClass { get; set; } = "breadcrumb-container";

		/// <summary>
		/// Separator between breadcrumb items
		/// </summary>
		public string Separator { get; set; } = "/";

		/// <summary>
		/// Maximum number of items to show before truncating
		/// </summary>
		public int MaxItems { get; set; } = 5;

		/// <summary>
		/// Adds a breadcrumb item to the navigation using controller/action
		/// </summary>
		public void AddControllerItem(string text, string? controller = null, string? action = null, object? routeValues = null, string? icon = null, string? tooltip = null)
		{
			Items.Add(new BreadcrumbItem
			{
				Text = text,
				Controller = controller,
				Action = action,
				RouteValues = routeValues,
				Icon = icon,
				Tooltip = tooltip,
				IsActive = false
			});
		}

		/// <summary>
		/// Adds a breadcrumb item with direct URL
		/// </summary>
		public void AddUrlItem(string text, string url, string? icon = null, string? tooltip = null)
		{
			Items.Add(new BreadcrumbItem
			{
				Text = text,
				Url = url,
				Icon = icon,
				Tooltip = tooltip,
				IsActive = false
			});
		}

		/// <summary>
		/// Adds the current page as the final breadcrumb item
		/// </summary>
		public void AddCurrentPage(string text, string? icon = null, string? tooltip = null)
		{
			Items.Add(new BreadcrumbItem
			{
				Text = text,
				Icon = icon,
				Tooltip = tooltip,
				IsActive = true,
				Url = null
			});
		}

		/// <summary>
		/// Creates a standard dashboard breadcrumb
		/// </summary>
		public static BreadcrumbViewModel CreateDashboardBreadcrumb()
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddCurrentPage("Dashboard", "fas fa-tachometer-alt", "Main Dashboard");
			return breadcrumb;
		}

		/// <summary>
		/// Creates a standard two-level breadcrumb (Dashboard > Section)
		/// </summary>
		public static BreadcrumbViewModel CreateSectionBreadcrumb(string sectionName, string? sectionIcon = null, string? controller = null, string? action = null)
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt", "Return to Dashboard");
			breadcrumb.AddCurrentPage(sectionName, sectionIcon);
			return breadcrumb;
		}

		/// <summary>
		/// Creates a standard three-level breadcrumb (Dashboard > Section > Page)
		/// </summary>
		public static BreadcrumbViewModel CreatePageBreadcrumb(string sectionName, string pageName, string? sectionController = null, string? sectionAction = null, string? pageIcon = null)
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt", "Return to Dashboard");

			if (!string.IsNullOrEmpty(sectionController) && !string.IsNullOrEmpty(sectionAction))
			{
				breadcrumb.AddControllerItem(sectionName, sectionController, sectionAction);
			}
			else
			{
				breadcrumb.AddUrlItem(sectionName, "#");
			}

			breadcrumb.AddCurrentPage(pageName, pageIcon);
			return breadcrumb;
		}
	}
}
