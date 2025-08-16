using Microsoft.AspNetCore.Http;

namespace PriceTracker.Core.Contracts
{
	/// <summary>
	/// Interface for navigation and menu state management
	/// </summary>
	public interface INavigationHelperService
	{
		/// <summary>
		/// Determines if a main menu item should be active
		/// </summary>
		/// <param name="menuKey">Menu identifier (dashboard, price-tracking, budget, tasks)</param>
		/// <param name="currentController">Current controller name</param>
		/// <param name="currentAction">Current action name</param>
		/// <returns>True if menu item should be highlighted as active</returns>
		bool IsMenuItemActive(string menuKey, string currentController, string currentAction);

		/// <summary>
		/// Determines if a dropdown item should be active
		/// </summary>
		/// <param name="menuKey">Parent menu identifier</param>
		/// <param name="dropdownKey">Dropdown item identifier</param>
		/// <param name="currentController">Current controller name</param>
		/// <param name="currentAction">Current action name</param>
		/// <returns>True if dropdown item should be highlighted as active</returns>
		bool IsDropdownItemActive(string menuKey, string dropdownKey, string currentController, string currentAction);

		/// <summary>
		/// Gets CSS classes for menu item based on active state
		/// </summary>
		/// <param name="menuKey">Menu identifier</param>
		/// <param name="currentController">Current controller name</param>
		/// <param name="currentAction">Current action name</param>
		/// <param name="baseClass">Base CSS classes</param>
		/// <returns>CSS classes with active state if applicable</returns>
		string GetMenuItemCssClass(string menuKey, string currentController, string currentAction, string baseClass = "nav-link text-dark");

		/// <summary>
		/// Gets CSS classes for dropdown item based on active state
		/// </summary>
		/// <param name="menuKey">Parent menu identifier</param>
		/// <param name="dropdownKey">Dropdown item identifier</param>
		/// <param name="currentController">Current controller name</param>
		/// <param name="currentAction">Current action name</param>
		/// <param name="baseClass">Base CSS classes</param>
		/// <returns>CSS classes with active state if applicable</returns>
		string GetDropdownItemCssClass(string menuKey, string dropdownKey, string currentController, string currentAction, string baseClass = "dropdown-item");

		/// <summary>
		/// Gets CSS classes for dropdown toggle based on any child being active
		/// </summary>
		/// <param name="menuKey">Menu identifier</param>
		/// <param name="currentController">Current controller name</param>
		/// <param name="currentAction">Current action name</param>
		/// <param name="baseClass">Base CSS classes</param>
		/// <returns>CSS classes with active state if applicable</returns>
		string GetDropdownToggleCssClass(string menuKey, string currentController, string currentAction, string baseClass = "nav-link dropdown-toggle text-dark");

		/// <summary>
		/// Determines if a dropdown should be expanded (show) based on current route
		/// </summary>
		/// <param name="menuKey">Menu identifier</param>
		/// <param name="currentController">Current controller name</param>
		/// <param name="currentAction">Current action name</param>
		/// <returns>True if dropdown should be expanded</returns>
		bool ShouldDropdownBeExpanded(string menuKey, string currentController, string currentAction);

		/// <summary>
		/// Gets route information from HttpContext
		/// </summary>
		/// <param name="httpContext">Current HTTP context</param>
		/// <returns>Current controller and action names</returns>
		(string controller, string action) GetCurrentRoute(HttpContext httpContext);

		/// <summary>
		/// Gets breadcrumb trail for current route
		/// </summary>
		/// <param name="currentController">Current controller name</param>
		/// <param name="currentAction">Current action name</param>
		/// <returns>List of breadcrumb items</returns>
		List<string> GetBreadcrumbTrail(string currentController, string currentAction);

		/// <summary>
		/// Gets user-friendly display name for menu keys
		/// </summary>
		/// <param name="menuKey">Menu identifier</param>
		/// <returns>User-friendly menu name</returns>
		string GetMenuDisplayName(string menuKey);

		/// <summary>
		/// Gets user-friendly display name for actions
		/// </summary>
		/// <param name="action">Action name</param>
		/// <param name="controller">Controller name for context</param>
		/// <returns>User-friendly action name</returns>
		string GetActionDisplayName(string action, string controller);

		/// <summary>
		/// Checks if user should see authenticated navigation
		/// </summary>
		/// <param name="httpContext">Current HTTP context</param>
		/// <returns>True if authenticated navigation should be shown</returns>
		bool ShouldShowAuthenticatedNavigation(HttpContext httpContext);
	}
}