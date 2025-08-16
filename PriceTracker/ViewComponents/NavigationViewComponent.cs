using Microsoft.AspNetCore.Mvc;
using PriceTracker.Core.Contracts;
using PriceTracker.Core.Models.Notifications;

namespace PriceTracker.ViewComponents
{
	/// <summary>
	/// View component for providing navigation helper data to views
	/// </summary>
	public class NavigationViewComponent : ViewComponent
	{
		private readonly INavigationHelperService _navigationHelper;
		private readonly INotificationService _notificationService;
		private readonly ILogger<NavigationViewComponent> _logger;

		public NavigationViewComponent(
			INavigationHelperService navigationHelper,
			INotificationService notificationService,
			ILogger<NavigationViewComponent> logger)
		{
			_navigationHelper = navigationHelper;
			_notificationService = notificationService;
			_logger = logger;
		}

		/// <summary>
		/// Provides navigation helper service and notification data to the view
		/// This allows the layout to access navigation methods and badges easily
		/// </summary>
		/// <returns>Navigation view model with notification badges</returns>
		public async Task<IViewComponentResult> InvokeAsync()
		{
			try
			{
				// Get current route information
				var (controller, action) = _navigationHelper.GetCurrentRoute(HttpContext);
				var isAuthenticated = _navigationHelper.ShouldShowAuthenticatedNavigation(HttpContext);

				_logger.LogDebug("Navigation component invoked for {Controller}/{Action}", controller, action);

				// Create base model
				var model = new NavigationViewModel
				{
					NavigationHelper = _navigationHelper,
					CurrentController = controller,
					CurrentAction = action,
					IsAuthenticated = isAuthenticated
				};

				// Load notification badges for authenticated users
				if (isAuthenticated && HttpContext.User.Identity?.IsAuthenticated == true)
				{
					try
					{
						// Get user ID (this will need to be adapted based on your user management setup)
						var userId = HttpContext.User.Identity.Name ?? string.Empty;

						if (!string.IsNullOrEmpty(userId))
						{
							model.NotificationSummary = await _notificationService.GetNavigationNotificationSummaryAsync(userId);
							model.NotificationBadges = model.NotificationSummary.GetNavigationBadges();

							_logger.LogDebug("Loaded {BadgeCount} notification badges for user: {UserId}",
								model.NotificationBadges.Count, userId);
						}
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Failed to load notification badges for user, continuing without badges");
						// Continue with empty notification data rather than failing completely
					}
				}

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
					IsAuthenticated = false,
					NotificationSummary = new NavigationNotificationSummary(),
					NotificationBadges = new List<NavigationNotificationBadge>()
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
		public NavigationNotificationSummary NotificationSummary { get; set; } = new();
		public List<NavigationNotificationBadge> NotificationBadges { get; set; } = new();

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

		/// <summary>
		/// Gets notification badge for a specific menu section
		/// </summary>
		public NavigationNotificationBadge? GetNotificationBadge(string badgeId)
		{
			return NotificationBadges.FirstOrDefault(b => b.BadgeId.Equals(badgeId, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Checks if a specific badge should be visible
		/// </summary>
		public bool ShouldShowBadge(string badgeId)
		{
			var badge = GetNotificationBadge(badgeId);
			return badge?.IsVisible == true && badge.Count > 0;
		}

		/// <summary>
		/// Gets badge HTML attributes for a specific badge
		/// </summary>
		public string GetBadgeAttributes(string badgeId)
		{
			var badge = GetNotificationBadge(badgeId);
			if (badge == null || !badge.IsVisible) return "style=\"display: none;\"";

			var classes = new List<string> { "notification-badge", $"bg-{badge.BadgeColor}" };

			if (badge.ShouldPulse) classes.Add("pulse");
			if (badge.Count > badge.MaxDisplayCount) classes.Add("high-count");
			if (badge.BadgeColor == "danger" && badge.ShouldPulse) classes.Add("critical");

			var attributes = new List<string>
			{
				$"class=\"{string.Join(" ", classes)}\"",
				!string.IsNullOrEmpty(badge.TooltipText) ? $"title=\"{badge.TooltipText}\"" : "",
				!string.IsNullOrEmpty(badge.TooltipText) ? "data-bs-toggle=\"tooltip\"" : ""
			};

			return string.Join(" ", attributes.Where(a => !string.IsNullOrEmpty(a)));
		}

		/// <summary>
		/// Gets the display count for a specific badge
		/// </summary>
		public string GetBadgeDisplayCount(string badgeId)
		{
			var badge = GetNotificationBadge(badgeId);
			return badge?.DisplayCount ?? "0";
		}

		/// <summary>
		/// Gets total notification count across all badges
		/// </summary>
		public int GetTotalNotificationCount()
		{
			return NotificationSummary.GetTotalNotificationCount();
		}

		/// <summary>
		/// Checks if there are any critical notifications
		/// </summary>
		public bool HasCriticalNotifications()
		{
			return NotificationSummary.HasCriticalNotifications();
		}
	}
}