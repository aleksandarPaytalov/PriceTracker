using Microsoft.AspNetCore.Mvc;
using PriceTracker.Core.Contracts;
using PriceTracker.Core.Models.Navigation;

namespace PriceTracker.ViewComponents
{
	/// <summary>
	/// View component for rendering breadcrumb navigation
	/// </summary>
	public class BreadcrumbViewComponent : ViewComponent
	{
		private readonly IBreadcrumbService _breadcrumbService;
		private readonly ILogger<BreadcrumbViewComponent> _logger;

		public BreadcrumbViewComponent(
			IBreadcrumbService breadcrumbService,
			ILogger<BreadcrumbViewComponent> logger)
		{
			_breadcrumbService = breadcrumbService;
			_logger = logger;
		}

		/// <summary>
		/// Invokes the breadcrumb view component
		/// Can be called with no parameters (auto-generate), custom breadcrumb model, or controller/action
		/// </summary>
		/// <param name="breadcrumb">Optional custom breadcrumb view model</param>
		/// <param name="controller">Optional controller name for manual generation</param>
		/// <param name="action">Optional action name for manual generation</param>
		/// <param name="routeValues">Optional route values for manual generation</param>
		/// <returns>Breadcrumb view with navigation</returns>
		public IViewComponentResult Invoke(
			BreadcrumbViewModel? breadcrumb = null,
			string? controller = null,
			string? action = null,
			object? routeValues = null)
		{
			try
			{
				BreadcrumbViewModel result;

				if (breadcrumb != null)
				{
					// Custom breadcrumb provided
					_logger.LogDebug("Using custom breadcrumb with {ItemCount} items", breadcrumb.Items.Count);
					result = breadcrumb;
				}
				else if (!string.IsNullOrEmpty(controller) && !string.IsNullOrEmpty(action))
				{
					// Manual controller/action provided
					_logger.LogDebug("Generating breadcrumb for {Controller}/{Action}", controller, action);
					result = _breadcrumbService.GenerateBreadcrumb(controller, action, routeValues);
				}
				else
				{
					// Auto-generate from current route
					_logger.LogDebug("Auto-generating breadcrumb from current route");
					result = _breadcrumbService.GenerateBreadcrumb(HttpContext);
				}

				_logger.LogDebug("Generated breadcrumb with {ItemCount} items, ShowBreadcrumb: {ShowBreadcrumb}",
					result.Items.Count, result.ShowBreadcrumb);

				return View(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating breadcrumb in view component");

				// Return empty breadcrumb on error
				return View(new BreadcrumbViewModel { ShowBreadcrumb = false });
			}
		}
	}
}