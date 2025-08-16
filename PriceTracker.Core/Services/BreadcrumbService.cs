using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PriceTracker.Core.Contracts;
using PriceTracker.Core.Models.Navigation;

namespace PriceTracker.Core.Services
{
	/// <summary>
	/// Service for generating breadcrumb navigation based on route context
	/// </summary>
	public class BreadcrumbService : IBreadcrumbService
	{
		private readonly ILogger<BreadcrumbService> _logger;

		// Controller display name mappings
		private readonly Dictionary<string, string> _controllerDisplayNames = new()
		{
			{ "Dashboard", "Dashboard" },
			{ "PriceTracking", "Price Tracking" },
			{ "Budget", "Budget Management" },
			{ "Expense", "Expenses" },
			{ "Task", "Task Management" },
			{ "Product", "Products" },
			{ "Store", "Stores" },
			{ "Report", "Reports" },
			{ "Profile", "Profile" },
			{ "Settings", "Settings" },
			{ "Home", "Home" },
			{ "Land", "Home" }
		};

		// Controller icon mappings
		private readonly Dictionary<string, string> _controllerIcons = new()
		{
			{ "Dashboard", "fas fa-tachometer-alt" },
			{ "PriceTracking", "fas fa-chart-line" },
			{ "Budget", "fas fa-wallet" },
			{ "Expense", "fas fa-receipt" },
			{ "Task", "fas fa-tasks" },
			{ "Product", "fas fa-box" },
			{ "Store", "fas fa-store" },
			{ "Report", "fas fa-chart-bar" },
			{ "Profile", "fas fa-user" },
			{ "Settings", "fas fa-cog" },
			{ "Home", "fas fa-home" },
			{ "Land", "fas fa-home" }
		};

		// Action display name mappings
		private readonly Dictionary<string, Dictionary<string, string>> _actionDisplayNames = new()
		{
			["Dashboard"] = new() { ["Index"] = "Overview" },
			["PriceTracking"] = new()
			{
				["Index"] = "Price Overview",
				["Products"] = "Products",
				["Stores"] = "Stores",
				["History"] = "Price History",
				["Compare"] = "Price Comparison"
			},
			["Budget"] = new()
			{
				["Index"] = "Budget Overview",
				["Monthly"] = "Monthly Budget",
				["Analysis"] = "Budget Analysis"
			},
			["Expense"] = new()
			{
				["Index"] = "All Expenses",
				["Add"] = "Add Expense",
				["Edit"] = "Edit Expense",
				["Details"] = "Expense Details"
			},
			["Task"] = new()
			{
				["Index"] = "My Tasks",
				["Create"] = "Create Task",
				["Edit"] = "Edit Task",
				["Details"] = "Task Details"
			},
			["Product"] = new()
			{
				["Index"] = "All Products",
				["Details"] = "Product Details",
				["Track"] = "Track Product"
			},
			["Store"] = new()
			{
				["Index"] = "All Stores",
				["Details"] = "Store Details"
			},
			["Report"] = new()
			{
				["Index"] = "Reports",
				["Budget"] = "Budget Reports",
				["Price"] = "Price Reports"
			}
		};

		// Routes where breadcrumb should be hidden
		private readonly HashSet<(string Controller, string Action)> _hideBreadcrumbRoutes = new()
		{
			("Land", "Index"),
			("Land", "Contact"),
			("Land", "About"),
			("Account", "Login"),
			("Account", "Register"),
			("Account", "Logout")
		};

		public BreadcrumbService(ILogger<BreadcrumbService> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Generates breadcrumb navigation based on current route context
		/// </summary>
		public BreadcrumbViewModel GenerateBreadcrumb(HttpContext httpContext)
		{
			try
			{
				var routeData = httpContext.Request.RouteValues;
				var controller = routeData["controller"]?.ToString() ?? string.Empty;
				var action = routeData["action"]?.ToString() ?? string.Empty;

				_logger.LogDebug("Generating breadcrumb for {Controller}/{Action}", controller, action);

				return GenerateBreadcrumb(controller, action, routeData);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating breadcrumb from HTTP context");
				return new BreadcrumbViewModel { ShowBreadcrumb = false };
			}
		}

		/// <summary>
		/// Generates breadcrumb navigation for a specific controller and action
		/// </summary>
		public BreadcrumbViewModel GenerateBreadcrumb(string controller, string action, object? routeValues = null)
		{
			try
			{
				// Check if breadcrumb should be hidden for this route
				if (!ShouldShowBreadcrumb(controller, action))
				{
					return new BreadcrumbViewModel { ShowBreadcrumb = false };
				}

				var breadcrumb = new BreadcrumbViewModel();

				// Generate breadcrumb based on controller and action
				switch (controller.ToLowerInvariant())
				{
					case "dashboard":
						return CreateDashboardBreadcrumb(action);

					case "pricetracking":
						return CreatePriceTrackingBreadcrumb(action, routeValues);

					case "budget":
						return CreateBudgetBreadcrumb(action, routeValues);

					case "expense":
						return CreateExpenseBreadcrumb(action, routeValues);

					case "task":
						return CreateTaskBreadcrumb(action, routeValues);

					case "product":
						return CreateProductBreadcrumb(action, routeValues);

					case "store":
						return CreateStoreBreadcrumb(action, routeValues);

					case "report":
						return CreateReportBreadcrumb(action, routeValues);

					default:
						return CreateDefaultBreadcrumb(controller, action);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating breadcrumb for {Controller}/{Action}", controller, action);
				return new BreadcrumbViewModel { ShowBreadcrumb = false };
			}
		}

		/// <summary>
		/// Creates a custom breadcrumb with manual items
		/// </summary>
		public BreadcrumbViewModel CreateCustomBreadcrumb(List<BreadcrumbItem> items)
		{
			return new BreadcrumbViewModel
			{
				Items = items,
				ShowBreadcrumb = items.Any()
			};
		}

		/// <summary>
		/// Gets the display name for a controller
		/// </summary>
		public string GetControllerDisplayName(string controllerName)
		{
			return _controllerDisplayNames.GetValueOrDefault(controllerName, controllerName);
		}

		/// <summary>
		/// Gets the display name for an action
		/// </summary>
		public string GetActionDisplayName(string actionName, string controllerName)
		{
			if (_actionDisplayNames.TryGetValue(controllerName, out var actionMappings))
			{
				return actionMappings.GetValueOrDefault(actionName, actionName);
			}
			return actionName;
		}

		/// <summary>
		/// Gets the icon for a controller
		/// </summary>
		public string GetControllerIcon(string controllerName)
		{
			return _controllerIcons.GetValueOrDefault(controllerName, "fas fa-folder");
		}

		/// <summary>
		/// Checks if breadcrumb should be shown for the current route
		/// </summary>
		public bool ShouldShowBreadcrumb(string controller, string action)
		{
			return !_hideBreadcrumbRoutes.Contains((controller, action));
		}

		#region Private Helper Methods

		private BreadcrumbViewModel CreateDashboardBreadcrumb(string action)
		{
			if (action.ToLowerInvariant() == "index")
			{
				return BreadcrumbViewModel.CreateDashboardBreadcrumb();
			}

			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt");
			breadcrumb.AddCurrentPage(GetActionDisplayName(action, "Dashboard"));
			return breadcrumb;
		}

		private BreadcrumbViewModel CreatePriceTrackingBreadcrumb(string action, object? routeValues)
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt");

			if (action.ToLowerInvariant() == "index")
			{
				breadcrumb.AddCurrentPage("Price Tracking", "fas fa-chart-line");
			}
			else
			{
				breadcrumb.AddControllerItem("Price Tracking", "PriceTracking", "Index", null, "fas fa-chart-line");
				breadcrumb.AddCurrentPage(GetActionDisplayName(action, "PriceTracking"));
			}

			return breadcrumb;
		}

		private BreadcrumbViewModel CreateBudgetBreadcrumb(string action, object? routeValues)
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt");

			if (action.ToLowerInvariant() == "index")
			{
				breadcrumb.AddCurrentPage("Budget Management", "fas fa-wallet");
			}
			else
			{
				breadcrumb.AddControllerItem("Budget Management", "Budget", "Index", null, "fas fa-wallet");
				breadcrumb.AddCurrentPage(GetActionDisplayName(action, "Budget"));
			}

			return breadcrumb;
		}

		private BreadcrumbViewModel CreateExpenseBreadcrumb(string action, object? routeValues)
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt");
			breadcrumb.AddControllerItem("Budget Management", "Budget", "Index", null, "fas fa-wallet");

			if (action.ToLowerInvariant() == "index")
			{
				breadcrumb.AddCurrentPage("Expenses", "fas fa-receipt");
			}
			else
			{
				breadcrumb.AddControllerItem("Expenses", "Expense", "Index", null, "fas fa-receipt");
				breadcrumb.AddCurrentPage(GetActionDisplayName(action, "Expense"));
			}

			return breadcrumb;
		}

		private BreadcrumbViewModel CreateTaskBreadcrumb(string action, object? routeValues)
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt");

			if (action.ToLowerInvariant() == "index")
			{
				breadcrumb.AddCurrentPage("Task Management", "fas fa-tasks");
			}
			else
			{
				breadcrumb.AddControllerItem("Task Management", "Task", "Index", null, "fas fa-tasks");
				breadcrumb.AddCurrentPage(GetActionDisplayName(action, "Task"));
			}

			return breadcrumb;
		}

		private BreadcrumbViewModel CreateProductBreadcrumb(string action, object? routeValues)
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt");
			breadcrumb.AddControllerItem("Price Tracking", "PriceTracking", "Index", null, "fas fa-chart-line");

			if (action.ToLowerInvariant() == "index")
			{
				breadcrumb.AddCurrentPage("Products", "fas fa-box");
			}
			else
			{
				breadcrumb.AddControllerItem("Products", "Product", "Index", null, "fas fa-box");
				breadcrumb.AddCurrentPage(GetActionDisplayName(action, "Product"));
			}

			return breadcrumb;
		}

		private BreadcrumbViewModel CreateStoreBreadcrumb(string action, object? routeValues)
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt");
			breadcrumb.AddControllerItem("Price Tracking", "PriceTracking", "Index", null, "fas fa-chart-line");

			if (action.ToLowerInvariant() == "index")
			{
				breadcrumb.AddCurrentPage("Stores", "fas fa-store");
			}
			else
			{
				breadcrumb.AddControllerItem("Stores", "Store", "Index", null, "fas fa-store");
				breadcrumb.AddCurrentPage(GetActionDisplayName(action, "Store"));
			}

			return breadcrumb;
		}

		private BreadcrumbViewModel CreateReportBreadcrumb(string action, object? routeValues)
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt");

			if (action.ToLowerInvariant() == "index")
			{
				breadcrumb.AddCurrentPage("Reports", "fas fa-chart-bar");
			}
			else
			{
				breadcrumb.AddControllerItem("Reports", "Report", "Index", null, "fas fa-chart-bar");
				breadcrumb.AddCurrentPage(GetActionDisplayName(action, "Report"));
			}

			return breadcrumb;
		}

		private BreadcrumbViewModel CreateDefaultBreadcrumb(string controller, string action)
		{
			var breadcrumb = new BreadcrumbViewModel();
			breadcrumb.AddControllerItem("Dashboard", "Dashboard", "Index", null, "fas fa-tachometer-alt");

			var controllerDisplayName = GetControllerDisplayName(controller);
			var actionDisplayName = GetActionDisplayName(action, controller);
			var controllerIcon = GetControllerIcon(controller);

			if (action.ToLowerInvariant() == "index")
			{
				breadcrumb.AddCurrentPage(controllerDisplayName, controllerIcon);
			}
			else
			{
				breadcrumb.AddControllerItem(controllerDisplayName, controller, "Index", null, controllerIcon);
				breadcrumb.AddCurrentPage(actionDisplayName);
			}

			return breadcrumb;
		}

		#endregion
	}
}