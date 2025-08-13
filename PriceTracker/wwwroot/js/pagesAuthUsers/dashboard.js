/* ===================================
   Dashboard JavaScript - Phase 1 Placeholder
   Will be enhanced in Phase 2 Step 1-6
   =================================== */

document.addEventListener('DOMContentLoaded', function () {
    console.log('Dashboard loaded successfully');

    // Initialize dashboard functionality
    initializeDashboard();
});

/**
 * Initialize dashboard functionality
 */
function initializeDashboard() {
    // Initialize refresh functionality
    initializeRefreshButton();

    // Initialize quick action buttons
    initializeQuickActions();

    // Initialize widget actions
    initializeWidgetActions();

    // Show task notification badge if needed (placeholder)
    updateTaskNotificationBadge();

    console.log('Dashboard functionality initialized');
}

/**
 * Initialize the refresh dashboard button
 */
function initializeRefreshButton() {
    const refreshButton = document.getElementById('refreshDashboard');
    if (refreshButton) {
        refreshButton.addEventListener('click', function (e) {
            e.preventDefault();
            refreshAllWidgets();
        });
    }
}

/**
 * Initialize quick action buttons
 */
function initializeQuickActions() {
    const quickActionButtons = document.querySelectorAll('.quick-action-btn');
    quickActionButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            // Placeholder - will be enhanced in Phase 2
            console.log('Quick action clicked:', this.querySelector('span').textContent);

            // For now, just show an alert
            e.preventDefault();
            alert('This feature will be implemented in Phase 2');
        });
    });
}

/**
 * Initialize widget action buttons (settings, view details, etc.)
 */
function initializeWidgetActions() {
    const widgetActionButtons = document.querySelectorAll('.widget-actions button');
    widgetActionButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();

            // Placeholder - will be enhanced in Phase 2
            console.log('Widget action clicked');
            alert('Widget settings will be implemented in Phase 2');
        });
    });
}

/**
 * Refresh all dashboard widgets via AJAX
 * Placeholder implementation - will be enhanced in Phase 2
 */
async function refreshAllWidgets() {
    const refreshButton = document.getElementById('refreshDashboard');
    const originalText = refreshButton.innerHTML;

    try {
        // Show loading state
        refreshButton.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>Refreshing...';
        refreshButton.disabled = true;

        // Add loading class to all widgets
        document.querySelectorAll('.dashboard-widget').forEach(widget => {
            widget.classList.add('widget-loading');
        });

        // Simulate API call (will be replaced with actual AJAX in Phase 2)
        await new Promise(resolve => setTimeout(resolve, 1000));

        // For now, just show success message
        console.log('Dashboard refreshed successfully (placeholder)');

        // TODO: Replace with actual AJAX call in Phase 2
        // const response = await fetch('/Dashboard/RefreshAll');
        // const data = await response.json();
        // updateWidgetsWithData(data);

    } catch (error) {
        console.error('Error refreshing dashboard:', error);
        alert('Failed to refresh dashboard. Please try again.');
    } finally {
        // Restore button state
        refreshButton.innerHTML = originalText;
        refreshButton.disabled = false;

        // Remove loading class from all widgets
        document.querySelectorAll('.dashboard-widget').forEach(widget => {
            widget.classList.remove('widget-loading');
        });
    }
}

/**
 * Refresh a specific widget
 * Placeholder implementation - will be enhanced in Phase 2
 */
async function refreshWidget(widgetType) {
    console.log(`Refreshing ${widgetType} widget (placeholder)`);

    try {
        // TODO: Implement actual AJAX call in Phase 2
        // const response = await fetch(`/Dashboard/RefreshWidget?widgetType=${widgetType}`);
        // const data = await response.json();
        // updateSingleWidget(widgetType, data);

        // For now, just log the action
        console.log(`${widgetType} widget refreshed successfully (placeholder)`);

    } catch (error) {
        console.error(`Error refreshing ${widgetType} widget:`, error);
    }
}

/**
 * Update task notification badge
 * Placeholder implementation - will be enhanced in Phase 2
 */
function updateTaskNotificationBadge() {
    const badge = document.getElementById('taskNotificationBadge');
    if (badge) {
        // Placeholder logic - will be replaced with actual task count in Phase 2
        const hasUrgentTasks = false; // This will come from actual data

        if (hasUrgentTasks) {
            badge.style.display = 'flex';
            badge.textContent = '3'; // Placeholder count
        } else {
            badge.style.display = 'none';
        }
    }
}

/**
 * Update widgets with fresh data from server
 * Will be implemented in Phase 2 when AJAX functionality is added
 */
function updateWidgetsWithData(data) {
    // TODO: Implement in Phase 2
    console.log('Updating widgets with data:', data);
}

/**
 * Update a single widget with fresh data
 * Will be implemented in Phase 2 when AJAX functionality is added
 */
function updateSingleWidget(widgetType, data) {
    // TODO: Implement in Phase 2
    console.log(`Updating ${widgetType} widget with data:`, data);
}

/**
 * Handle widget errors
 * Will be enhanced in Phase 2
 */
function handleWidgetError(widgetType, error) {
    console.error(`Error in ${widgetType} widget:`, error);

    // TODO: Implement user-friendly error display in Phase 2
}

/**
 * Utility function to show loading state on elements
 */
function showLoadingState(element) {
    if (element) {
        element.classList.add('widget-loading');
    }
}

/**
 * Utility function to hide loading state on elements
 */
function hideLoadingState(element) {
    if (element) {
        element.classList.remove('widget-loading');
    }
}

// Export functions for use in other modules (Phase 2 preparation)
window.DashboardModule = {
    refreshAllWidgets,
    refreshWidget,
    updateTaskNotificationBadge,
    showLoadingState,
    hideLoadingState
};