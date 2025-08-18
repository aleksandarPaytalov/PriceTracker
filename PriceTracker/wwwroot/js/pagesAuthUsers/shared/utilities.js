/* ===================================
   Utilities - Common Helper Functions
   Reusable functions for all authenticated user features
   =================================== */

/**
 * Utility functions for PriceTracker application
 * Provides common functionality for date formatting, currency, validation, etc.
 */
class PriceTrackerUtils {
    constructor() {
        // Initialize user locale settings
        this.userLocale = this.detectUserLocale();
        this.currency = this.detectUserCurrency();
        this.timezone = this.detectUserTimezone();

        console.log('PriceTracker Utilities initialized', {
            locale: this.userLocale,
            currency: this.currency,
            timezone: this.timezone
        });
    }

    // === LOCALE & SETTINGS DETECTION ===

    /**
     * Detect user's preferred locale
     * @returns {string} Locale string (e.g., 'en-US', 'bg-BG')
     */
    detectUserLocale() {
        // Check for saved preference first
        const saved = localStorage.getItem('pt_user_locale');
        if (saved) return saved;

        // Fall back to browser locale
        return navigator.language || navigator.userLanguage || 'en-US';
    }

    /**
     * Detect user's preferred currency
     * @returns {string} Currency code (e.g., 'USD', 'BGN', 'EUR')
     */
    detectUserCurrency() {
        // Check for saved preference first
        const saved = localStorage.getItem('pt_user_currency');
        if (saved) return saved;

        // Map common locales to currencies
        const currencyMap = {
            'bg': 'BGN',
            'bg-BG': 'BGN',
            'en-US': 'USD',
            'en-GB': 'GBP',
            'de-DE': 'EUR',
            'fr-FR': 'EUR',
            'es-ES': 'EUR',
            'it-IT': 'EUR'
        };

        const locale = this.userLocale.toLowerCase();
        return currencyMap[locale] || currencyMap[locale.split('-')[0]] || 'USD';
    }

    /**
     * Detect user's timezone
     * @returns {string} Timezone string
     */
    detectUserTimezone() {
        try {
            return Intl.DateTimeFormat().resolvedOptions().timeZone || 'UTC';
        } catch {
            return 'UTC';
        }
    }

    // === DATE & TIME UTILITIES ===

    /**
     * Format date for display
     * @param {Date|string|number} date - Date to format
     * @param {object} options - Formatting options
     * @returns {string} Formatted date string
     */
    formatDate(date, options = {}) {
        const defaultOptions = {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        };

        const finalOptions = { ...defaultOptions, ...options };
        const dateObj = this.parseDate(date);

        try {
            return new Intl.DateTimeFormat(this.userLocale, finalOptions).format(dateObj);
        } catch {
            return dateObj.toLocaleDateString();
        }
    }

    /**
     * Format date and time for display
     * @param {Date|string|number} date - Date to format
     * @param {object} options - Formatting options
     * @returns {string} Formatted datetime string
     */
    formatDateTime(date, options = {}) {
        const defaultOptions = {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        };

        const finalOptions = { ...defaultOptions, ...options };
        return this.formatDate(date, finalOptions);
    }

    /**
     * Format time only
     * @param {Date|string|number} date - Date to format
     * @param {object} options - Formatting options
     * @returns {string} Formatted time string
     */
    formatTime(date, options = {}) {
        const defaultOptions = {
            hour: '2-digit',
            minute: '2-digit'
        };

        const finalOptions = { ...defaultOptions, ...options };
        return this.formatDate(date, finalOptions);
    }

    /**
     * Get relative time (e.g., "2 hours ago", "in 3 days")
     * @param {Date|string|number} date - Date to compare
     * @returns {string} Relative time string
     */
    formatRelativeTime(date) {
        const dateObj = this.parseDate(date);
        const now = new Date();
        const diffMs = dateObj.getTime() - now.getTime();
        const diffSec = Math.round(diffMs / 1000);
        const diffMin = Math.round(diffSec / 60);
        const diffHour = Math.round(diffMin / 60);
        const diffDay = Math.round(diffHour / 24);

        // Use Intl.RelativeTimeFormat if available
        if (typeof Intl !== 'undefined' && Intl.RelativeTimeFormat) {
            try {
                const rtf = new Intl.RelativeTimeFormat(this.userLocale, { numeric: 'auto' });

                if (Math.abs(diffSec) < 60) return rtf.format(diffSec, 'second');
                if (Math.abs(diffMin) < 60) return rtf.format(diffMin, 'minute');
                if (Math.abs(diffHour) < 24) return rtf.format(diffHour, 'hour');
                if (Math.abs(diffDay) < 30) return rtf.format(diffDay, 'day');

                return this.formatDate(dateObj);
            } catch {
                // Fall through to manual implementation
            }
        }

        // Manual relative time formatting
        const abs = Math.abs;
        if (abs(diffSec) < 60) return diffSec >= 0 ? 'in a moment' : 'just now';
        if (abs(diffMin) < 60) return diffMin >= 0 ? `in ${diffMin} min` : `${abs(diffMin)} min ago`;
        if (abs(diffHour) < 24) return diffHour >= 0 ? `in ${diffHour}h` : `${abs(diffHour)}h ago`;
        if (abs(diffDay) < 7) return diffDay >= 0 ? `in ${diffDay} days` : `${abs(diffDay)} days ago`;

        return this.formatDate(dateObj);
    }

    /**
     * Parse various date formats into Date object
     * @param {Date|string|number} date - Date to parse
     * @returns {Date} Parsed date object
     */
    parseDate(date) {
        if (date instanceof Date) return date;
        if (typeof date === 'string' || typeof date === 'number') {
            const parsed = new Date(date);
            if (!isNaN(parsed.getTime())) return parsed;
        }
        return new Date(); // Default to now
    }

    /**
     * Check if date is today
     * @param {Date|string|number} date - Date to check
     * @returns {boolean} True if date is today
     */
    isToday(date) {
        const dateObj = this.parseDate(date);
        const today = new Date();
        return dateObj.toDateString() === today.toDateString();
    }

    /**
     * Check if date is within specified days from now
     * @param {Date|string|number} date - Date to check
     * @param {number} days - Number of days
     * @returns {boolean} True if within range
     */
    isWithinDays(date, days) {
        const dateObj = this.parseDate(date);
        const now = new Date();
        const diffTime = Math.abs(dateObj - now);
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        return diffDays <= days;
    }

    // === CURRENCY & NUMBER UTILITIES ===

    /**
     * Format currency amount
     * @param {number} amount - Amount to format
     * @param {string} currency - Currency code (optional)
     * @param {object} options - Formatting options
     * @returns {string} Formatted currency string
     */
    formatCurrency(amount, currency = null, options = {}) {
        const finalCurrency = currency || this.currency;
        const defaultOptions = {
            style: 'currency',
            currency: finalCurrency,
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        };

        const finalOptions = { ...defaultOptions, ...options };

        try {
            return new Intl.NumberFormat(this.userLocale, finalOptions).format(amount);
        } catch {
            // Fallback formatting
            return `${finalCurrency} ${this.formatNumber(amount, { minimumFractionDigits: 2 })}`;
        }
    }

    /**
     * Format number with locale-specific formatting
     * @param {number} number - Number to format
     * @param {object} options - Formatting options
     * @returns {string} Formatted number string
     */
    formatNumber(number, options = {}) {
        try {
            return new Intl.NumberFormat(this.userLocale, options).format(number);
        } catch {
            return number.toLocaleString();
        }
    }

    /**
     * Format percentage
     * @param {number} value - Value between 0 and 1 (or 0-100 if isPercent is true)
     * @param {boolean} isPercent - Whether input is already a percentage
     * @param {number} decimals - Number of decimal places
     * @returns {string} Formatted percentage string
     */
    formatPercentage(value, isPercent = false, decimals = 1) {
        const percentage = isPercent ? value : value * 100;
        try {
            return new Intl.NumberFormat(this.userLocale, {
                style: 'percent',
                minimumFractionDigits: decimals,
                maximumFractionDigits: decimals
            }).format(isPercent ? value / 100 : value);
        } catch {
            return `${percentage.toFixed(decimals)}%`;
        }
    }

    /**
     * Parse currency string to number
     * @param {string} currencyString - Currency string to parse
     * @returns {number} Parsed number
     */
    parseCurrency(currencyString) {
        if (typeof currencyString === 'number') return currencyString;

        // Remove currency symbols and non-numeric characters except decimal point
        const cleaned = currencyString
            .replace(/[^\d.-]/g, '')
            .replace(/^-+/, '-'); // Keep only the first minus sign

        return parseFloat(cleaned) || 0;
    }

    /**
     * Calculate percentage change
     * @param {number} oldValue - Original value
     * @param {number} newValue - New value
     * @returns {number} Percentage change
     */
    calculatePercentageChange(oldValue, newValue) {
        if (oldValue === 0) return newValue === 0 ? 0 : 100;
        return ((newValue - oldValue) / oldValue) * 100;
    }

    // === VALIDATION UTILITIES ===

    /**
     * Validate email address
     * @param {string} email - Email to validate
     * @returns {boolean} True if valid email
     */
    isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    /**
     * Validate phone number (basic validation)
     * @param {string} phone - Phone number to validate
     * @returns {boolean} True if valid phone
     */
    isValidPhone(phone) {
        const phoneRegex = /^[\+]?[\d\s\-\(\)]{10,}$/;
        return phoneRegex.test(phone);
    }

    /**
     * Validate currency amount
     * @param {string|number} amount - Amount to validate
     * @param {number} min - Minimum value
     * @param {number} max - Maximum value
     * @returns {boolean} True if valid amount
     */
    isValidAmount(amount, min = 0, max = Infinity) {
        const numericAmount = typeof amount === 'string' ? this.parseCurrency(amount) : amount;
        return !isNaN(numericAmount) && numericAmount >= min && numericAmount <= max;
    }

    /**
     * Validate date range
     * @param {Date|string} startDate - Start date
     * @param {Date|string} endDate - End date
     * @returns {boolean} True if valid range
     */
    isValidDateRange(startDate, endDate) {
        const start = this.parseDate(startDate);
        const end = this.parseDate(endDate);
        return !isNaN(start.getTime()) && !isNaN(end.getTime()) && start <= end;
    }

    // === DOM UTILITIES ===

    /**
     * Safely select DOM element
     * @param {string} selector - CSS selector
     * @param {Element} context - Context element (optional)
     * @returns {Element|null} Selected element
     */
    $(selector, context = document) {
        try {
            return context.querySelector(selector);
        } catch {
            return null;
        }
    }

    /**
     * Safely select multiple DOM elements
     * @param {string} selector - CSS selector
     * @param {Element} context - Context element (optional)
     * @returns {NodeList} Selected elements
     */
    $$(selector, context = document) {
        try {
            return context.querySelectorAll(selector);
        } catch {
            return [];
        }
    }

    /**
     * Show element with animation
     * @param {Element} element - Element to show
     * @param {string} animation - Animation class
     */
    showElement(element, animation = 'animate-fade-in') {
        if (!element) return;

        element.style.display = '';
        element.classList.remove('d-none', 'hidden');
        element.classList.add(animation);
    }

    /**
     * Hide element with animation
     * @param {Element} element - Element to hide
     * @param {string} hiddenClass - Class to add when hidden
     */
    hideElement(element, hiddenClass = 'd-none') {
        if (!element) return;

        element.classList.add(hiddenClass);
    }

    /**
     * Toggle element visibility
     * @param {Element} element - Element to toggle
     * @param {boolean} show - Force show/hide (optional)
     */
    toggleElement(element, show = null) {
        if (!element) return;

        const isHidden = element.classList.contains('d-none') ||
            element.classList.contains('hidden') ||
            element.style.display === 'none';

        const shouldShow = show !== null ? show : isHidden;

        if (shouldShow) {
            this.showElement(element);
        } else {
            this.hideElement(element);
        }
    }

    /**
     * Create and show loading state
     * @param {Element} container - Container element
     * @param {string} message - Loading message
     */
    showLoading(container, message = 'Loading...') {
        if (!container) return;

        const loadingHtml = `
            <div class="loading-state d-flex flex-column align-items-center justify-content-center p-4">
                <div class="spinner-border text-primary mb-3" role="status">
                    <span class="visually-hidden">${message}</span>
                </div>
                <p class="text-muted mb-0">${message}</p>
            </div>
        `;

        container.innerHTML = loadingHtml;
    }

    /**
     * Create and show error state
     * @param {Element} container - Container element
     * @param {string} message - Error message
     * @param {function} retryCallback - Retry function (optional)
     */
    showError(container, message, retryCallback = null) {
        if (!container) return;

        const retryButton = retryCallback ?
            `<button class="btn btn-outline-primary mt-2" onclick="(${retryCallback.toString()})()">
                <i class="fas fa-redo me-1"></i>Try Again
            </button>` : '';

        const errorHtml = `
            <div class="error-state text-center p-4">
                <i class="fas fa-exclamation-triangle text-warning mb-3" style="font-size: 2rem;"></i>
                <h6 class="text-dark mb-2">Oops! Something went wrong</h6>
                <p class="text-muted mb-0">${message}</p>
                ${retryButton}
            </div>
        `;

        container.innerHTML = errorHtml;
    }

    // === EVENT UTILITIES ===

    /**
     * Debounce function execution
     * @param {function} func - Function to debounce
     * @param {number} wait - Wait time in milliseconds
     * @param {boolean} immediate - Execute immediately on first call
     * @returns {function} Debounced function
     */
    debounce(func, wait, immediate = false) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                timeout = null;
                if (!immediate) func(...args);
            };
            const callNow = immediate && !timeout;
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
            if (callNow) func(...args);
        };
    }

    /**
     * Throttle function execution
     * @param {function} func - Function to throttle
     * @param {number} limit - Time limit in milliseconds
     * @returns {function} Throttled function
     */
    throttle(func, limit) {
        let inThrottle;
        return function executedFunction(...args) {
            if (!inThrottle) {
                func.apply(this, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }

    // === STORAGE UTILITIES ===

    /**
     * Safely get item from localStorage
     * @param {string} key - Storage key
     * @param {any} defaultValue - Default value if not found
     * @returns {any} Stored value or default
     */
    getStorageItem(key, defaultValue = null) {
        try {
            const item = localStorage.getItem(key);
            return item ? JSON.parse(item) : defaultValue;
        } catch {
            return defaultValue;
        }
    }

    /**
     * Safely set item in localStorage
     * @param {string} key - Storage key
     * @param {any} value - Value to store
     * @returns {boolean} Success status
     */
    setStorageItem(key, value) {
        try {
            localStorage.setItem(key, JSON.stringify(value));
            return true;
        } catch {
            return false;
        }
    }

    /**
     * Remove item from localStorage
     * @param {string} key - Storage key
     */
    removeStorageItem(key) {
        try {
            localStorage.removeItem(key);
        } catch {
            // Silently fail
        }
    }

    // === NOTIFICATION UTILITIES ===

    /**
     * Show toast notification
     * @param {string} message - Notification message
     * @param {string} type - Notification type (success, error, warning, info)
     * @param {number} duration - Duration in milliseconds
     */
    showToast(message, type = 'info', duration = 5000) {
        // Create toast container if it doesn't exist
        let container = this.$('#toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'position-fixed top-0 end-0 p-3';
            container.style.zIndex = '1060';
            document.body.appendChild(container);
        }

        // Create toast element
        const toast = document.createElement('div');
        toast.className = `toast show align-items-center text-white bg-${type === 'error' ? 'danger' : type} border-0`;
        toast.setAttribute('role', 'alert');

        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        `;

        container.appendChild(toast);

        // Auto-remove toast
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, duration);

        // Handle close button
        const closeBtn = toast.querySelector('.btn-close');
        if (closeBtn) {
            closeBtn.addEventListener('click', () => {
                if (toast.parentNode) {
                    toast.parentNode.removeChild(toast);
                }
            });
        }
    }

    // === COMMON CALCULATIONS ===

    /**
     * Calculate budget progress percentage
     * @param {number} spent - Amount spent
     * @param {number} budget - Total budget
     * @returns {number} Progress percentage
     */
    calculateBudgetProgress(spent, budget) {
        if (budget === 0) return 0;
        return Math.min((spent / budget) * 100, 100);
    }

    /**
     * Calculate savings from price comparison
     * @param {number} currentPrice - Current price
     * @param {number} bestPrice - Best available price
     * @returns {object} Savings amount and percentage
     */
    calculateSavings(currentPrice, bestPrice) {
        const amount = currentPrice - bestPrice;
        const percentage = currentPrice > 0 ? (amount / currentPrice) * 100 : 0;

        return {
            amount: Math.max(amount, 0),
            percentage: Math.max(percentage, 0),
            hasSavings: amount > 0
        };
    }

    /**
     * Generate random ID
     * @param {number} length - ID length
     * @returns {string} Random ID
     */
    generateId(length = 8) {
        const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        let result = '';
        for (let i = 0; i < length; i++) {
            result += chars.charAt(Math.floor(Math.random() * chars.length));
        }
        return result;
    }

    /**
     * Deep clone object
     * @param {any} obj - Object to clone
     * @returns {any} Cloned object
     */
    deepClone(obj) {
        if (obj === null || typeof obj !== 'object') return obj;
        if (obj instanceof Date) return new Date(obj.getTime());
        if (obj instanceof Array) return obj.map(item => this.deepClone(item));
        if (obj instanceof Object) {
            const cloned = {};
            Object.keys(obj).forEach(key => {
                cloned[key] = this.deepClone(obj[key]);
            });
            return cloned;
        }
    }
}

// Create global utilities instance
const utils = new PriceTrackerUtils();

// Export for module systems and global access
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { PriceTrackerUtils };
} else {
    window.PriceTrackerUtils = PriceTrackerUtils;
    window.utils = utils;
}

// Log utilities ready
console.log('PriceTracker Utilities ready - Use global "utils" object for helper functions');