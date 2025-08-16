using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PriceTracker.Core.Contracts;

namespace PriceTracker.Core.Services
{
	/// <summary>
	/// Service for navigation and menu state management
	/// </summary>
	public class NavigationHelperService : INavigationHelperService
	{
		private readonly ILogger<NavigationHelperService> _logger;

		// Controller groupings for menu highlighting
		private readonly Dictionary<string, HashSet<string>> _menuControllerMappings = new()
		{
			["dashboard"] = new() { "dashboard" },
			["price-tracking"] = new() { "pricetracking", "product", "store", "price" },
			["budget"] = new() { "budget", "expense", "monthlybudget" },
			["tasks"] = new() { "task", "todo", "notification" },
			["reports"] = new() { "report", "analytics" }
		};

		// Action groupings for dropdown highlighting
		private readonly Dictionary<string, Dictionary<string, HashSet<string>>> _dropdownActionMappings = new()
		{
			["price-tracking"] = new()
			{
				["track-products"] = new() { "index", "search", "track", "add" },
				["stores"] = new() { "stores", "storedetails" },
				["price-history"] = new() { "history", "trends", "charts" },
				["price-reports"] = new() { "reports", "analytics", "comparison" }
			},
			["budget"] = new()
			{
				["add-expense"] = new() { "add", "create", "new" },
				["monthly-budget"] = new() { "monthly", "budget", "plan" },
				["all-expenses"] = new() { "index", "list", "all" },
				["budget-reports"] = new() { "reports", "analysis", "charts" }
			},
			["tasks"] = new()
			{
				["create-task"] = new() { "create", "add", "new" },
				["my-tasks"] = new() { "index", "list", "all" },
				["notifications"] = new() { "notifications", "alerts", "reminders" },
				["task-analytics"] = new() { "analytics", "reports", "stats" }
			}
		};

		public NavigationHelperService(ILogger<NavigationHelperService> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Determines if a main menu item should be active
		/// </summary>
		public bool IsMenuItemActive(string menuKey, string currentController, string currentAction)
		{
			try
			{
				var normalizedController = currentController.ToLowerInvariant();
				var menuKeyLower = menuKey.ToLowerInvariant();

				if (_menuControllerMappings.TryGetValue(menuKeyLower, out var controllers))
				{
					return controllers.Contains(normalizedController);
				}

				// Fallback: direct match
				return string.Equals(menuKeyLower, normalizedController, StringComparison.OrdinalIgnoreCase);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error determining menu item active state for {MenuKey}, {Controller}/{Action}",
					menuKey, currentController, currentAction);
				return false;
			}
		}

		/// <summary>
		/// Determines if a dropdown item should be active
		/// </summary>
		public bool IsDropdownItemActive(string menuKey, string dropdownKey, string currentController, string currentAction)
		{
			try
			{
				var menuKeyLower = menuKey.ToLowerInvariant();
				var dropdownKeyLower = dropdownKey.ToLowerInvariant();
				var normalizedController = currentController.ToLowerInvariant();
				var normalizedAction = currentAction.ToLowerInvariant();

				// First check if we're in the right menu section
				if (!IsMenuItemActive(menuKey, currentController, currentAction))
				{
					return false;
				}

				// Then check specific dropdown actions
				if (_dropdownActionMappings.TryGetValue(menuKeyLower, out var dropdownMappings))
				{
					if (dropdownMappings.TryGetValue(dropdownKeyLower, out var actions))
					{
						return actions.Contains(normalizedAction);
					}
				}

				return false;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error determining dropdown item active state for {MenuKey}/{DropdownKey}, {Controller}/{Action}",
					menuKey, dropdownKey, currentController, currentAction);
				return false;
			}
		}

		/// <summary>
		/// Gets CSS classes for menu item based on active state
		/// </summary>
		public string GetMenuItemCssClass(string menuKey, string currentController, string currentAction, string baseClass = "nav-link text-dark")
		{
			var isActive = IsMenuItemActive(menuKey, currentController, currentAction);
			return isActive ? $"{baseClass} active" : baseClass;
		}

		/// <summary>
		/// Gets CSS classes for dropdown item based on active state
		/// </summary>
		public string GetDropdownItemCssClass(string menuKey, string dropdownKey, string currentController, string currentAction, string baseClass = "dropdown-item")
		{
			var isActive = IsDropdownItemActive(menuKey, dropdownKey, currentController, currentAction);
			return isActive ? $"{baseClass} active" : baseClass;
		}

		/// <summary>
		/// Gets CSS classes for dropdown toggle based on any child being active
		/// </summary>
		public string GetDropdownToggleCssClass(string menuKey, string currentController, string currentAction, string baseClass = "nav-link dropdown-toggle text-dark")
		{
			var isActive = IsMenuItemActive(menuKey, currentController, currentAction);
			return isActive ? $"{baseClass} active" : baseClass;
		}

		/// <summary>
		/// Determines if a dropdown should be expanded (show) based on current route
		/// </summary>
		public bool ShouldDropdownBeExpanded(string menuKey, string currentController, string currentAction)
		{
			return IsMenuItemActive(menuKey, currentController, currentAction);
		}

		/// <summary>
		/// Gets route information from HttpContext
		/// </summary>
		public (string controller, string action) GetCurrentRoute(HttpContext httpContext)
		{
			try
			{
				var routeData = httpContext.Request.RouteValues;
				var controller = routeData["controller"]?.ToString() ?? string.Empty;
				var action = routeData["action"]?.ToString() ?? string.Empty;

				return (controller, action);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting current route from HttpContext");
				return (string.Empty, string.Empty);
			}
		}

		/// <summary>
		/// Gets breadcrumb trail for current route
		/// </summary>
		public List<string> GetBreadcrumbTrail(string currentController, string currentAction)
		{
			var trail = new List<string> { "Dashboard" };

			try
			{
				var normalizedController = currentController.ToLowerInvariant();

				// Add main section
				foreach (var (menuKey, controllers) in _menuControllerMappings)
				{
					if (controllers.Contains(normalizedController) && menuKey != "dashboard")
					{
						trail.Add(GetMenuDisplayName(menuKey));
						break;
					}
				}

				// Add current page if not index
				if (!string.Equals(currentAction, "index", StringComparison.OrdinalIgnoreCase))
				{
					trail.Add(GetActionDisplayName(currentAction, currentController));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating breadcrumb trail for {Controller}/{Action}", currentController, currentAction);
			}

			return trail;
		}

		/// <summary>
		/// Gets user-friendly display name for menu keys
		/// </summary>
		public string GetMenuDisplayName(string menuKey)
		{
			return menuKey.ToLowerInvariant() switch
			{
				"dashboard" => "Dashboard",
				"price-tracking" => "Price Tracking",
				"budget" => "Budget Management",
				"tasks" => "Task Management",
				"reports" => "Reports",
				_ => menuKey.Replace("-", " ").Replace("_", " ")
			};
		}

		/// <summary>
		/// Gets user-friendly display name for actions
		/// </summary>
		public string GetActionDisplayName(string action, string controller)
		{
			return action.ToLowerInvariant() switch
			{
				"index" => "Overview",
				"create" => "Create",
				"add" => "Add",
				"edit" => "Edit",
				"details" => "Details",
				"delete" => "Delete",
				"search" => "Search",
				"history" => "History",
				"reports" => "Reports",
				"analytics" => "Analytics",
				_ => action.Replace("-", " ").Replace("_", " ")
			};
		}

		/// <summary>
		/// Checks if user should see authenticated navigation
		/// </summary>
		public bool ShouldShowAuthenticatedNavigation(HttpContext httpContext)
		{
			return httpContext.User.Identity?.IsAuthenticated == true;
		}
	}
}