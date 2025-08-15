using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PriceTracker.Controllers;
using PriceTracker.Core.Contracts;
using PriceTracker.Core.Models.Dashboard;
using PriceTracker.Infrastructure.Data.Models;

namespace PriceTracker.Web.Controllers
{
	public class DashboardController : BaseController
	{
		private readonly IDashboardService _dashboardService;
		private readonly UserManager<User> _userManager;
		private readonly ILogger<DashboardController> _logger;

		public DashboardController(
			IDashboardService dashboardService,
			UserManager<User> userManager,
			ILogger<DashboardController> logger)
		{
			_dashboardService = dashboardService;
			_userManager = userManager;
			_logger = logger;
		}

		/// <summary>
		/// Main dashboard landing page for authenticated users
		/// Displays overview of budget, expenses, tasks, and price tracking data
		/// </summary>
		/// <returns>Dashboard view with user-specific data</returns>
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			try
			{
				// Get current user information
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					_logger.LogWarning("User not found when loading dashboard");
					return RedirectToAction("Login", "Account", new { area = "Identity" });
				}

				var userId = user.Id;
				var userName = user.UserName ?? user.Email ?? "User";

				_logger.LogInformation("Loading dashboard for user: {UserId}", userId);

				// Get dashboard data through service layer
				var viewModel = await _dashboardService.GetDashboardDataAsync(userId, userName);

				// Log successful load
				_logger.LogInformation("Successfully loaded dashboard for user: {UserId}", userId);

				return View(viewModel);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading dashboard for user: {UserId}", User.Identity?.Name);

				// Create error view model with fallback data
				var errorViewModel = new DashboardViewModel
				{
					UserId = User.Identity?.Name ?? "Unknown",
					UserDisplayName = User.Identity?.Name ?? "User",
					WelcomeMessage = _dashboardService.GenerateWelcomeMessage(User.Identity?.Name ?? "User"),
					CurrentDate = DateTime.Now,
					LastUpdated = DateTime.Now,
					HasError = true,
					ErrorMessage = "Unable to load dashboard data. Please try again.",
					IsLoading = false
				};

				return View(errorViewModel);
			}
		}

		/// <summary>
		/// API endpoint for refreshing dashboard widgets via AJAX
		/// Returns JSON data for specific widget updates
		/// </summary>
		/// <param name="widgetType">Type of widget to refresh (budget, expenses, tasks, products)</param>
		/// <returns>JSON data for dashboard widget</returns>
		[HttpGet]
		public async Task<IActionResult> RefreshWidget(string widgetType)
		{
			try
			{
				// Get current user information
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					return Json(new { success = false, error = "User not authenticated" });
				}

				if (string.IsNullOrWhiteSpace(widgetType))
				{
					return Json(new { success = false, error = "Widget type is required" });
				}

				_logger.LogDebug("Refreshing widget {WidgetType} for user: {UserId}", widgetType, user.Id);

				// Refresh specific widget through service layer
				var refreshData = await _dashboardService.RefreshWidgetDataAsync(user.Id, widgetType);

				return Json(refreshData);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error refreshing widget {WidgetType} for user: {UserId}",
					widgetType, User.Identity?.Name);

				return Json(new
				{
					success = false,
					error = $"Failed to refresh {widgetType} widget",
					timestamp = DateTime.Now
				});
			}
		}

		/// <summary>
		/// API endpoint for refreshing all dashboard widgets via AJAX
		/// </summary>
		/// <returns>JSON data for all dashboard widgets</returns>
		[HttpGet]
		public async Task<IActionResult> RefreshAll()
		{
			try
			{
				// Get current user information
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					return Json(new { success = false, error = "User not authenticated" });
				}

				_logger.LogDebug("Refreshing all widgets for user: {UserId}", user.Id);

				// Get fresh dashboard data
				var dashboardData = await _dashboardService.GetDashboardDataAsync(user.Id, user.UserName ?? "User");

				var refreshData = new DashboardRefreshData
				{
					Success = !dashboardData.HasError,
					Timestamp = DateTime.Now,
					ErrorMessage = dashboardData.ErrorMessage,
					Widgets = new DashboardWidgetData
					{
						Budget = dashboardData.BudgetOverview,
						Expenses = dashboardData.YearlyExpenses,
						Tasks = dashboardData.TaskOverview,
						Products = dashboardData.TopProducts
					}
				};

				return Json(refreshData);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error refreshing all widgets for user: {UserId}", User.Identity?.Name);

				return Json(new
				{
					success = false,
					error = "Failed to refresh dashboard data",
					timestamp = DateTime.Now
				});
			}
		}

		/// <summary>
		/// API endpoint for getting dashboard configuration
		/// </summary>
		/// <returns>JSON data with user's dashboard configuration</returns>
		[HttpGet]
		public async Task<IActionResult> GetConfiguration()
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					return Json(new { success = false, error = "User not authenticated" });
				}

				var configuration = await _dashboardService.GetDashboardConfigurationAsync(user.Id);
				return Json(new { success = true, configuration });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting dashboard configuration for user: {UserId}", User.Identity?.Name);
				return Json(new { success = false, error = "Failed to get configuration" });
			}
		}

		/// <summary>
		/// API endpoint for updating dashboard configuration
		/// </summary>
		/// <param name="configuration">New configuration settings</param>
		/// <returns>JSON result of the update operation</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateConfiguration([FromBody] DashboardConfigurationViewModel configuration)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					return Json(new { success = false, error = "User not authenticated" });
				}

				if (!ModelState.IsValid)
				{
					return Json(new { success = false, error = "Invalid configuration data" });
				}

				configuration.UserId = user.Id;
				var success = await _dashboardService.UpdateDashboardConfigurationAsync(user.Id, configuration);

				return Json(new { success });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating dashboard configuration for user: {UserId}", User.Identity?.Name);
				return Json(new { success = false, error = "Failed to update configuration" });
			}
		}

		/// <summary>
		/// Partial view for individual dashboard widgets (for AJAX loading)
		/// </summary>
		/// <param name="widgetType">Type of widget to render</param>
		/// <returns>Partial view with widget content</returns>
		[HttpGet]
		public async Task<IActionResult> GetWidget(string widgetType)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					return PartialView("_ErrorWidget", "User not authenticated");
				}

				// Get widget-specific data
				var refreshData = await _dashboardService.RefreshWidgetDataAsync(user.Id, widgetType);

				if (!refreshData.Success)
				{
					return PartialView("_ErrorWidget", refreshData.ErrorMessage);
				}

				// Return appropriate partial view based on widget type
				return widgetType.ToLowerInvariant() switch
				{
					"budget" => PartialView("_BudgetWidget", refreshData.Widgets?.Budget),
					"expenses" => PartialView("_ExpenseWidget", refreshData.Widgets?.Expenses),
					"tasks" => PartialView("_TaskWidget", refreshData.Widgets?.Tasks),
					"products" => PartialView("_ProductWidget", refreshData.Widgets?.Products),
					_ => PartialView("_ErrorWidget", "Unknown widget type")
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting widget {WidgetType} for user: {UserId}",
					widgetType, User.Identity?.Name);
				return PartialView("_ErrorWidget", "Failed to load widget");
			}
		}
	}
}