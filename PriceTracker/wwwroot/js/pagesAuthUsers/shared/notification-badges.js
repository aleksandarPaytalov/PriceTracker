/* ===================================
   Notification Badges Management
   =================================== */

/**
 * Notification Badge Manager
 * Handles real-time updates, animations, and interactions for navigation badges
 */
class NotificationBadgeManager {
    constructor() {
        this.refreshInterval = 30000; // 30 seconds
        this.refreshTimer = null;
        this.badges = new Map();
        this.isRefreshing = false;

        this.init();
    }

    /**
     * Initialize the badge manager
     */
    init() {
        this.loadExistingBadges();
        this.setupEventListeners();
        this.startAutoRefresh();

        console.log('NotificationBadgeManager initialized');
    }

    /**
     * Load existing badges from the DOM
     */
    loadExistingBadges() {
        document.querySelectorAll('.notification-badge').forEach(badge => {
            const badgeId = this.getBadgeId(badge);
            if (badgeId) {
                this.badges.set(badgeId, {
                    element: badge,
                    count: parseInt(badge.textContent) || 0,
                    visible: badge.style.display !== 'none'
                });
            }
        });

        console.log(`Loaded ${this.badges.size} existing badges`);
    }

    /**
     * Setup event listeners for badge interactions
     */
    setupEventListeners() {
        // Listen for badge clicks to mark notifications as read
        document.addEventListener('click', (event) => {
            const badge = event.target.closest('.notification-badge');
            if (badge) {
                this.handleBadgeClick(badge, event);
            }
        });

        // Listen for navigation changes
        document.addEventListener('visibilitychange', () => {
            if (!document.hidden) {
                this.refreshBadges();
            }
        });

        // Listen for focus events to refresh badges
        window.addEventListener('focus', () => {
            this.refreshBadges();
        });
    }

    /**
     * Start automatic badge refresh
     */
    startAutoRefresh() {
        if (this.refreshTimer) {
            clearInterval(this.refreshTimer);
        }

        this.refreshTimer = setInterval(() => {
            this.refreshBadges();
        }, this.refreshInterval);
    }

    /**
     * Stop automatic badge refresh
     */
    stopAutoRefresh() {
        if (this.refreshTimer) {
            clearInterval(this.refreshTimer);
            this.refreshTimer = null;
        }
    }

    /**
     * Refresh all notification badges from server
     */
    async refreshBadges() {
        if (this.isRefreshing) {
            return;
        }

        try {
            this.isRefreshing = true;

            // TODO: Replace with actual API call in Phase 2
            // const response = await fetch('/api/notifications/badges', {
            //     method: 'GET',
            //     headers: {
            //         'Content-Type': 'application/json'
            //     }
            // });

            // if (!response.ok) {
            //     throw new Error(`HTTP error! status: ${response.status}`);
            // }

            // const badgeData = await response.json();
            // this.updateBadges(badgeData);

            // Placeholder for Phase 2 - simulate API response
            console.log('Badge refresh scheduled for Phase 2 implementation');

        } catch (error) {
            console.error('Error refreshing notification badges:', error);
        } finally {
            this.isRefreshing = false;
        }
    }

    /**
     * Update badges with new data from server
     */
    updateBadges(badgeData) {
        if (!badgeData || !Array.isArray(badgeData)) {
            console.warn('Invalid badge data received');
            return;
        }

        badgeData.forEach(data => {
            this.updateSingleBadge(data.badgeId, data);
        });
    }

    /**
     * Update a single badge with new data
     */
    updateSingleBadge(badgeId, data) {
        const badgeInfo = this.badges.get(badgeId);
        if (!badgeInfo) {
            console.warn(`Badge not found: ${badgeId}`);
            return;
        }

        const { element } = badgeInfo;
        const oldCount = badgeInfo.count;
        const newCount = data.count || 0;

        // Update count if changed
        if (oldCount !== newCount) {
            this.animateCountChange(element, oldCount, newCount);
            badgeInfo.count = newCount;
        }

        // Update visibility
        const shouldShow = data.isVisible && newCount > 0;
        if (badgeInfo.visible !== shouldShow) {
            this.toggleBadgeVisibility(element, shouldShow);
            badgeInfo.visible = shouldShow;
        }

        // Update badge color if changed
        if (data.badgeColor) {
            this.updateBadgeColor(element, data.badgeColor);
        }

        // Update tooltip if changed
        if (data.tooltipText) {
            this.updateBadgeTooltip(element, data.tooltipText);
        }

        // Update pulse animation
        if (data.shouldPulse) {
            element.classList.add('pulse');
        } else {
            element.classList.remove('pulse');
        }
    }

    /**
     * Animate count change with smooth transition
     */
    animateCountChange(element, oldCount, newCount) {
        element.classList.add('updating');

        // Animate the number change
        const duration = 600;
        const startTime = performance.now();

        const animate = (currentTime) => {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);

            const currentCount = Math.round(oldCount + (newCount - oldCount) * progress);
            element.textContent = this.formatCount(currentCount);

            if (progress < 1) {
                requestAnimationFrame(animate);
            } else {
                element.classList.remove('updating');
            }
        };

        requestAnimationFrame(animate);
    }

    /**
     * Toggle badge visibility with animation
     */
    toggleBadgeVisibility(element, show) {
        if (show) {
            element.style.display = '';
            element.classList.remove('hidden');
            element.classList.add('fade-in');

            setTimeout(() => {
                element.classList.remove('fade-in');
            }, 500);
        } else {
            element.classList.add('hidden');

            setTimeout(() => {
                element.style.display = 'none';
            }, 300);
        }
    }

    /**
     * Update badge color
     */
    updateBadgeColor(element, newColor) {
        // Remove existing color classes
        const colorClasses = ['bg-danger', 'bg-warning', 'bg-info', 'bg-success', 'bg-primary', 'bg-secondary'];
        element.classList.remove(...colorClasses);

        // Add new color class
        element.classList.add(`bg-${newColor}`);
    }

    /**
     * Update badge tooltip
     */
    updateBadgeTooltip(element, tooltipText) {
        element.setAttribute('title', tooltipText);
        element.setAttribute('data-bs-original-title', tooltipText);

        // Update Bootstrap tooltip if it exists
        const tooltip = bootstrap.Tooltip.getInstance(element);
        if (tooltip) {
            tooltip.setContent({ '.tooltip-inner': tooltipText });
        }
    }

    /**
     * Handle badge click events
     */
    handleBadgeClick(badge, event) {
        event.preventDefault();
        event.stopPropagation();

        const badgeId = this.getBadgeId(badge);
        if (badgeId) {
            console.log(`Badge clicked: ${badgeId}`);

            // TODO: Implement mark as read functionality in Phase 2
            // this.markNotificationsAsRead(badgeId);
        }
    }

    /**
     * Mark notifications as read for a specific badge
     */
    async markNotificationsAsRead(badgeId) {
        try {
            // TODO: Implement API call in Phase 2
            // const response = await fetch(`/api/notifications/mark-read/${badgeId}`, {
            //     method: 'POST',
            //     headers: {
            //         'Content-Type': 'application/json'
            //     }
            // });

            // if (response.ok) {
            //     this.updateSingleBadge(badgeId, { count: 0, isVisible: false });
            // }

            console.log(`Mark as read functionality scheduled for Phase 2: ${badgeId}`);

        } catch (error) {
            console.error(`Error marking notifications as read for ${badgeId}:`, error);
        }
    }

    /**
     * Get badge ID from element
     */
    getBadgeId(element) {
        // Try to find the badge ID from parent nav item
        const navItem = element.closest('.nav-item');
        if (navItem) {
            const link = navItem.querySelector('a');
            if (link) {
                const text = link.textContent.trim().toLowerCase();
                if (text.includes('task')) return 'tasks';
                if (text.includes('budget')) return 'budget';
                if (text.includes('price')) return 'prices';
            }
        }

        return null;
    }

    /**
     * Format count for display (99+ for counts over 99)
     */
    formatCount(count) {
        return count > 99 ? '99+' : count.toString();
    }

    /**
     * Get total notification count across all badges
     */
    getTotalCount() {
        let total = 0;
        this.badges.forEach(badge => {
            if (badge.visible) {
                total += badge.count;
            }
        });
        return total;
    }

    /**
     * Check if there are any critical notifications
     */
    hasCriticalNotifications() {
        for (const [badgeId, badge] of this.badges) {
            if (badge.visible && badge.element.classList.contains('critical')) {
                return true;
            }
        }
        return false;
    }

    /**
     * Destroy the badge manager
     */
    destroy() {
        this.stopAutoRefresh();
        this.badges.clear();
        console.log('NotificationBadgeManager destroyed');
    }
}

// Global badge manager instance
let badgeManager = null;

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    // Only initialize for authenticated users
    if (document.querySelector('.notification-badge')) {
        badgeManager = new NotificationBadgeManager();
    }
});

// Clean up on page unload
window.addEventListener('beforeunload', function () {
    if (badgeManager) {
        badgeManager.destroy();
    }
});

// Export for use in other modules
window.NotificationBadgeManager = NotificationBadgeManager;