using Microsoft.AspNetCore.Mvc;
using PriceTracker.Core.Contracts;

namespace PriceTracker.ViewComponents
{
	/// <summary>
	/// View component for providing navigation helper data to views
	/// </summary>
	public class NavigationViewComponent : ViewComponent
	{
		private readonly INavigationHelperService _navigationHelper;
		private readonly ILogger<NavigationViewComponent> _logger;

		public NavigationViewComponent(
			INavigationHelperService navigationHelper,
			ILogger<NavigationViewComponent> logger)
		{
			_navigationHelper = navigationHelper;
			_logger = logger;
		}

		/// <summary>
		/// Provides navigation helper service to the view
		/// This allows the layout to access navigation methods easily
		/// </summary>
		/// <returns>Navigation helper service</returns>
		public IViewComponentResult Invoke()
		{
			try
			{
				// Get current route information
				var (controller, action) = _navigationHelper.GetCurrentRoute(HttpContext);

				_logger.LogDebug("Navigation component invoked for {Controller}/{Action}", controller, action);

				// Create a view model with navigation helper and current route
				var model = new NavigationViewModel
				{
					NavigationHelper = _navigationHelper,
					CurrentController = controller,
					CurrentAction = action,
					IsAuthenticated = _navigationHelper.ShouldShowAuthenticatedNavigation(HttpContext)
				};

				return View(model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in navigation view component");

				// Return minimal model on error
				return View(new NavigationViewModel
				{
					NavigationHelper = _navigationHelper,
					CurrentController = string.Empty,
					CurrentAction = string.Empty,
					IsAuthenticated = false
				});
			}
		}
	}

	/// <summary>
	/// View model for navigation component
	/// </summary>
	public class NavigationViewModel
	{
		public INavigationHelperService NavigationHelper { get; set; } = null!;
		public string CurrentController { get; set; } = string.Empty;
		public string CurrentAction { get; set; } = string.Empty;
		public bool IsAuthenticated { get; set; }

		/// <summary>
		/// Helper method to get menu item CSS class
		/// </summary>
		public string GetMenuCss(string menuKey, string baseClass = "nav-link text-dark")
		{
			return NavigationHelper.GetMenuItemCssClass(menuKey, CurrentController, CurrentAction, baseClass);
		}

		/// <summary>
		/// Helper method to get dropdown toggle CSS class
		/// </summary>
		public string GetDropdownToggleCss(string menuKey, string baseClass = "nav-link dropdown-toggle text-dark")
		{
			return NavigationHelper.GetDropdownToggleCssClass(menuKey, CurrentController, CurrentAction, baseClass);
		}

		/// <summary>
		/// Helper method to get dropdown item CSS class
		/// </summary>
		public string GetDropdownItemCss(string menuKey, string dropdownKey, string baseClass = "dropdown-item")
		{
			return NavigationHelper.GetDropdownItemCssClass(menuKey, dropdownKey, CurrentController, CurrentAction, baseClass);
		}

		/// <summary>
		/// Helper method to check if dropdown should be expanded
		/// </summary>
		public bool ShouldExpandDropdown(string menuKey)
		{
			return NavigationHelper.ShouldDropdownBeExpanded(menuKey, CurrentController, CurrentAction);
		}
	}
}