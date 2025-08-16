using Microsoft.AspNetCore.Http;
using PriceTracker.Core.Models.Navigation;

namespace PriceTracker.Core.Contracts
{
	/// <summary>
	/// Service interface for generating breadcrumb navigation
	/// </summary>
	public interface IBreadcrumbService
	{
		/// <summary>
		/// Generates breadcrumb navigation based on current route context
		/// </summary>
		/// <param name="httpContext">Current HTTP context</param>
		/// <returns>Breadcrumb view model with navigation items</returns>
		BreadcrumbViewModel GenerateBreadcrumb(HttpContext httpContext);

		/// <summary>
		/// Generates breadcrumb navigation for a specific controller and action
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="routeValues">Additional route values</param>
		/// <returns>Breadcrumb view model with navigation items</returns>
		BreadcrumbViewModel GenerateBreadcrumb(string controller, string action, object? routeValues = null);

		/// <summary>
		/// Creates a custom breadcrumb with manual items
		/// </summary>
		/// <param name="items">List of breadcrumb items</param>
		/// <returns>Breadcrumb view model with provided items</returns>
		BreadcrumbViewModel CreateCustomBreadcrumb(List<BreadcrumbItem> items);

		/// <summary>
		/// Gets the display name for a controller
		/// </summary>
		/// <param name="controllerName">Controller name</param>
		/// <returns>Human-readable controller display name</returns>
		string GetControllerDisplayName(string controllerName);

		/// <summary>
		/// Gets the display name for an action
		/// </summary>
		/// <param name="actionName">Action name</param>
		/// <param name="controllerName">Controller name for context</param>
		/// <returns>Human-readable action display name</returns>
		string GetActionDisplayName(string actionName, string controllerName);

		/// <summary>
		/// Gets the icon for a controller
		/// </summary>
		/// <param name="controllerName">Controller name</param>
		/// <returns>Font Awesome icon class</returns>
		string GetControllerIcon(string controllerName);

		/// <summary>
		/// Checks if breadcrumb should be shown for the current route
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <returns>True if breadcrumb should be displayed</returns>
		bool ShouldShowBreadcrumb(string controller, string action);
	}
}
