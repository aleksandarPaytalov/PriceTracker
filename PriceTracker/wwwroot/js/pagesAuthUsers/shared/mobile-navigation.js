/* ===================================
   Mobile Navigation Manager
   =================================== */

/**
 * Mobile Navigation Manager
 * Handles hamburger menu, dropdowns, and touch interactions
 */
class MobileNavigationManager {
    constructor() {
        this.isSmallScreen = window.innerWidth < 768;
        this.navbar = document.querySelector('.navbar');
        this.navbarToggler = document.querySelector('.navbar-toggler');
        this.navbarCollapse = document.querySelector('.navbar-collapse');
        this.dropdowns = document.querySelectorAll('.nav-item.dropdown');
        this.touchStartY = 0;
        this.touchEndY = 0;

        this.init();
    }

    /**
     * Initialize mobile navigation functionality
     */
    init() {
        this.setupEventListeners();
        this.handleResize();
        this.setupTouchEvents();
        this.setupDropdownBehavior();
        this.setupAccessibility();

        console.log('MobileNavigationManager initialized');
    }

    /**
     * Setup event listeners for mobile navigation
     */
    setupEventListeners() {
        // Hamburger menu toggle
        if (this.navbarToggler) {
            this.navbarToggler.addEventListener('click', (e) => {
                this.toggleMobileMenu(e);
            });
        }

        // Window resize handler
        window.addEventListener('resize', () => {
            this.handleResize();
        });

        // Close menu when clicking outside
        document.addEventListener('click', (e) => {
            this.handleOutsideClick(e);
        });

        // Handle escape key
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.closeMobileMenu();
                this.closeAllDropdowns();
            }
        });

        // Handle dropdown toggles in mobile
        this.dropdowns.forEach(dropdown => {
            const toggle = dropdown.querySelector('.dropdown-toggle');
            if (toggle) {
                toggle.addEventListener('click', (e) => {
                    if (this.isSmallScreen) {
                        e.preventDefault();
                        this.toggleDropdown(dropdown);
                    }
                });
            }
        });

        // Handle orientation change
        window.addEventListener('orientationchange', () => {
            setTimeout(() => {
                this.handleResize();
                this.adjustForOrientation();
            }, 100);
        });
    }

    /**
     * Setup touch events for better mobile experience
     */
    setupTouchEvents() {
        if (this.navbarCollapse) {
            // Swipe up to close menu
            this.navbarCollapse.addEventListener('touchstart', (e) => {
                this.touchStartY = e.touches[0].clientY;
            }, { passive: true });

            this.navbarCollapse.addEventListener('touchend', (e) => {
                this.touchEndY = e.changedTouches[0].clientY;
                this.handleSwipe();
            }, { passive: true });

            // Prevent scrolling behind menu when open
            this.navbarCollapse.addEventListener('touchmove', (e) => {
                if (this.isMenuOpen() && this.isSmallScreen) {
                    e.preventDefault();
                }
            }, { passive: false });
        }

        // Add touch-friendly hover effects
        const navLinks = document.querySelectorAll('.nav-link, .dropdown-item');
        navLinks.forEach(link => {
            link.addEventListener('touchstart', () => {
                link.classList.add('touch-hover');
            });

            link.addEventListener('touchend', () => {
                setTimeout(() => {
                    link.classList.remove('touch-hover');
                }, 150);
            });
        });
    }

    /**
     * Setup dropdown behavior for mobile
     */
    setupDropdownBehavior() {
        this.dropdowns.forEach(dropdown => {
            const dropdownMenu = dropdown.querySelector('.dropdown-menu');
            const toggle = dropdown.querySelector('.dropdown-toggle');

            if (dropdownMenu && toggle) {
                // Handle dropdown item clicks
                const dropdownItems = dropdownMenu.querySelectorAll('.dropdown-item');
                dropdownItems.forEach(item => {
                    item.addEventListener('click', () => {
                        if (this.isSmallScreen) {
                            // Close dropdown after item selection on mobile
                            this.closeDropdown(dropdown);
                            // Close main menu after navigation
                            setTimeout(() => {
                                this.closeMobileMenu();
                            }, 300);
                        }
                    });
                });
            }
        });
    }

    /**
     * Setup accessibility features
     */
    setupAccessibility() {
        // Add ARIA labels
        if (this.navbarToggler) {
            this.navbarToggler.setAttribute('aria-label', 'Toggle navigation menu');
        }

        // Add role attributes
        const dropdownMenus = document.querySelectorAll('.dropdown-menu');
        dropdownMenus.forEach(menu => {
            menu.setAttribute('role', 'menu');
        });

        const dropdownItems = document.querySelectorAll('.dropdown-item');
        dropdownItems.forEach(item => {
            item.setAttribute('role', 'menuitem');
        });

        // Keyboard navigation
        this.setupKeyboardNavigation();
    }

    /**
     * Setup keyboard navigation for accessibility
     */
    setupKeyboardNavigation() {
        const focusableElements = this.navbarCollapse?.querySelectorAll(
            'a, button, [tabindex]:not([tabindex="-1"])'
        );

        if (focusableElements) {
            focusableElements.forEach((element, index) => {
                element.addEventListener('keydown', (e) => {
                    if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
                        e.preventDefault();
                        const direction = e.key === 'ArrowDown' ? 1 : -1;
                        const nextIndex = (index + direction + focusableElements.length) % focusableElements.length;
                        focusableElements[nextIndex].focus();
                    }
                });
            });
        }
    }

    /**
     * Toggle mobile menu open/closed
     */
    toggleMobileMenu(event) {
        if (event) {
            event.preventDefault();
        }

        if (this.isMenuOpen()) {
            this.closeMobileMenu();
        } else {
            this.openMobileMenu();
        }
    }

    /**
     * Open mobile menu
     */
    openMobileMenu() {
        if (this.navbarCollapse) {
            this.navbarCollapse.classList.add('show');
            this.navbarToggler?.setAttribute('aria-expanded', 'true');

            // Prevent body scroll
            if (this.isSmallScreen) {
                document.body.style.overflow = 'hidden';
            }

            // Focus first menu item
            const firstMenuItem = this.navbarCollapse.querySelector('.nav-link');
            if (firstMenuItem) {
                setTimeout(() => {
                    firstMenuItem.focus();
                }, 100);
            }

            this.announceToScreenReader('Navigation menu opened');
        }
    }

    /**
     * Close mobile menu
     */
    closeMobileMenu() {
        if (this.navbarCollapse) {
            this.navbarCollapse.classList.remove('show');
            this.navbarToggler?.setAttribute('aria-expanded', 'false');

            // Restore body scroll
            document.body.style.overflow = '';

            // Close any open dropdowns
            this.closeAllDropdowns();

            // Return focus to toggle button
            if (this.navbarToggler) {
                this.navbarToggler.focus();
            }

            this.announceToScreenReader('Navigation menu closed');
        }
    }

    /**
     * Check if mobile menu is open
     */
    isMenuOpen() {
        return this.navbarCollapse?.classList.contains('show') || false;
    }

    /**
     * Toggle dropdown menu
     */
    toggleDropdown(dropdown) {
        const dropdownMenu = dropdown.querySelector('.dropdown-menu');
        const toggle = dropdown.querySelector('.dropdown-toggle');

        if (dropdownMenu && toggle) {
            const isOpen = dropdownMenu.classList.contains('show');

            // Close other dropdowns first
            this.closeAllDropdowns();

            if (!isOpen) {
                dropdownMenu.classList.add('show');
                toggle.setAttribute('aria-expanded', 'true');
                this.announceToScreenReader(`${toggle.textContent.trim()} menu opened`);
            }
        }
    }

    /**
     * Close specific dropdown
     */
    closeDropdown(dropdown) {
        const dropdownMenu = dropdown.querySelector('.dropdown-menu');
        const toggle = dropdown.querySelector('.dropdown-toggle');

        if (dropdownMenu && toggle) {
            dropdownMenu.classList.remove('show');
            toggle.setAttribute('aria-expanded', 'false');
        }
    }

    /**
     * Close all dropdowns
     */
    closeAllDropdowns() {
        this.dropdowns.forEach(dropdown => {
            this.closeDropdown(dropdown);
        });
    }

    /**
     * Handle outside click to close menus
     */
    handleOutsideClick(event) {
        if (!this.navbar?.contains(event.target)) {
            if (this.isMenuOpen()) {
                this.closeMobileMenu();
            }
        }
    }

    /**
     * Handle swipe gestures
     */
    handleSwipe() {
        const swipeThreshold = 100;
        const swipeDistance = this.touchStartY - this.touchEndY;

        // Swipe up to close menu
        if (swipeDistance > swipeThreshold && this.isMenuOpen()) {
            this.closeMobileMenu();
        }
    }

    /**
     * Handle window resize
     */
    handleResize() {
        const wasSmallScreen = this.isSmallScreen;
        this.isSmallScreen = window.innerWidth < 768;

        // Close menu when transitioning to desktop
        if (wasSmallScreen && !this.isSmallScreen) {
            this.closeMobileMenu();
            document.body.style.overflow = '';
        }

        // Update dropdown behavior
        this.updateDropdownBehavior();
    }

    /**
     * Update dropdown behavior based on screen size
     */
    updateDropdownBehavior() {
        this.dropdowns.forEach(dropdown => {
            const toggle = dropdown.querySelector('.dropdown-toggle');
            const dropdownMenu = dropdown.querySelector('.dropdown-menu');

            if (toggle && dropdownMenu) {
                if (this.isSmallScreen) {
                    // Mobile behavior: click to toggle
                    dropdownMenu.classList.remove('show');
                    toggle.setAttribute('aria-expanded', 'false');
                } else {
                    // Desktop behavior: hover
                    // Bootstrap handles this automatically
                }
            }
        });
    }

    /**
     * Adjust layout for orientation changes
     */
    adjustForOrientation() {
        if (this.isSmallScreen && window.orientation !== undefined) {
            // Adjust menu height for landscape orientation
            if (Math.abs(window.orientation) === 90) { // Landscape
                if (this.navbarCollapse) {
                    this.navbarCollapse.style.maxHeight = 'calc(100vh - 120px)';
                }
            } else { // Portrait
                if (this.navbarCollapse) {
                    this.navbarCollapse.style.maxHeight = '';
                }
            }
        }
    }

    /**
     * Announce changes to screen readers
     */
    announceToScreenReader(message) {
        const announcement = document.createElement('div');
        announcement.setAttribute('aria-live', 'polite');
        announcement.setAttribute('aria-atomic', 'true');
        announcement.className = 'sr-only';
        announcement.textContent = message;

        document.body.appendChild(announcement);

        setTimeout(() => {
            document.body.removeChild(announcement);
        }, 1000);
    }

    /**
     * Get current screen size category
     */
    getScreenSize() {
        const width = window.innerWidth;

        if (width < 576) return 'xs';
        if (width < 768) return 'sm';
        if (width < 992) return 'md';
        if (width < 1200) return 'lg';
        return 'xl';
    }

    /**
     * Check if device supports touch
     */
    isTouchDevice() {
        return 'ontouchstart' in window || navigator.maxTouchPoints > 0;
    }

    /**
     * Destroy mobile navigation manager
     */
    destroy() {
        this.closeMobileMenu();
        document.body.style.overflow = '';

        // Remove event listeners would go here
        // (In a real app, you'd store references to clean up)

        console.log('MobileNavigationManager destroyed');
    }
}

// Global mobile navigation manager
let mobileNavManager = null;

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    mobileNavManager = new MobileNavigationManager();
});

// Clean up on page unload
window.addEventListener('beforeunload', function () {
    if (mobileNavManager) {
        mobileNavManager.destroy();
    }
});

// Export for external use
window.MobileNavigationManager = MobileNavigationManager;

// Additional utility functions for mobile navigation

/**
 * Check if current device is mobile
 */
function isMobileDevice() {
    return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
}

/**
 * Get viewport dimensions
 */
function getViewportSize() {
    return {
        width: Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0),
        height: Math.max(document.documentElement.clientHeight || 0, window.innerHeight || 0)
    };
}

/**
 * Debounce function for resize events
 */
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Export utility functions
window.MobileNavigationUtils = {
    isMobileDevice,
    getViewportSize,
    debounce
};