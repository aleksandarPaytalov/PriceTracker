/* ===================================
   Breadcrumb Visibility Control
   Hide breadcrumb on main dashboard page
   =================================== */

document.addEventListener('DOMContentLoaded', function () {
    // Hide breadcrumb when on main dashboard page
    hideBreadcrumbOnDashboardMain();
});

/**
 * Hide breadcrumb navigation when user is on the main dashboard page
 */
function hideBreadcrumbOnDashboardMain() {
    try {
        // Get current URL path
        const currentPath = window.location.pathname.toLowerCase();

        // Check if we're on the main dashboard page
        const isDashboardMain = currentPath === '/dashboard' ||
            currentPath === '/dashboard/' ||
            currentPath === '/dashboard/index';

        // Find breadcrumb container
        const breadcrumbContainer = document.querySelector('.breadcrumb-container');

        if (breadcrumbContainer && isDashboardMain) {
            // Hide the breadcrumb with smooth transition
            breadcrumbContainer.style.display = 'none';

            // Also remove margin from dashboard container to maintain spacing
            const dashboardContainer = document.querySelector('.dashboard-container');
            if (dashboardContainer) {
                dashboardContainer.style.marginTop = '0';
            }
        }
    } catch (error) {
        console.log('Breadcrumb visibility control: No action needed');
    }
}