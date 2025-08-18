/* ===================================
   API Client - Centralized HTTP Communication
   Handles all API calls for authenticated user features
   =================================== */

/**
 * Centralized API Client for PriceTracker Application
 * Provides consistent error handling, authentication, and request/response processing
 */
class ApiClient {
    constructor() {
        this.baseUrl = window.location.origin;
        this.defaultHeaders = {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        };
        this.requestTimeout = 30000; // 30 seconds
        this.retryAttempts = 3;
        this.retryDelay = 1000; // 1 second

        // Initialize anti-forgery token
        this.initAntiForgeryToken();

        console.log('API Client initialized');
    }

    /**
     * Initialize anti-forgery token for CSRF protection
     */
    initAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        if (token) {
            this.defaultHeaders['RequestVerificationToken'] = token.value;
        }
    }

    /**
     * Create a configured fetch request with timeout and retries
     * @param {string} url - The API endpoint URL
     * @param {object} options - Fetch options
     * @param {number} attempt - Current retry attempt
     * @returns {Promise} Fetch promise
     */
    async createRequest(url, options = {}, attempt = 1) {
        // Merge default headers with custom headers
        const headers = { ...this.defaultHeaders, ...options.headers };

        // Create AbortController for timeout
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), this.requestTimeout);

        const config = {
            ...options,
            headers,
            signal: controller.signal
        };

        try {
            const response = await fetch(url, config);
            clearTimeout(timeoutId);

            // Handle non-2xx responses
            if (!response.ok) {
                throw new ApiError(
                    `HTTP ${response.status}`,
                    response.status,
                    await this.parseErrorResponse(response)
                );
            }

            return response;
        } catch (error) {
            clearTimeout(timeoutId);

            // Handle timeout
            if (error.name === 'AbortError') {
                throw new ApiError('Request timeout', 408, 'The request took too long to complete');
            }

            // Handle network errors with retry logic
            if (attempt < this.retryAttempts && this.shouldRetry(error)) {
                console.warn(`API request failed, retrying (${attempt}/${this.retryAttempts})...`);
                await this.delay(this.retryDelay * attempt);
                return this.createRequest(url, options, attempt + 1);
            }

            throw error;
        }
    }

    /**
     * Parse error response from server
     * @param {Response} response - Failed response object
     * @returns {Promise<string>} Error message
     */
    async parseErrorResponse(response) {
        try {
            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                const errorData = await response.json();
                return errorData.message || errorData.error || 'An error occurred';
            } else {
                return await response.text() || 'An unknown error occurred';
            }
        } catch {
            return 'Failed to parse error response';
        }
    }

    /**
     * Determine if request should be retried
     * @param {Error} error - The error that occurred
     * @returns {boolean} Whether to retry
     */
    shouldRetry(error) {
        // Retry on network errors, not on client errors (4xx)
        return error instanceof TypeError || // Network error
            error.name === 'AbortError' || // Timeout
            (error instanceof ApiError && error.status >= 500); // Server error
    }

    /**
     * Create delay for retry attempts
     * @param {number} ms - Milliseconds to delay
     * @returns {Promise} Delay promise
     */
    delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    /**
     * GET request
     * @param {string} endpoint - API endpoint
     * @param {object} params - Query parameters
     * @param {object} options - Additional fetch options
     * @returns {Promise<any>} Response data
     */
    async get(endpoint, params = {}, options = {}) {
        const url = new URL(endpoint, this.baseUrl);

        // Add query parameters
        Object.keys(params).forEach(key => {
            if (params[key] !== null && params[key] !== undefined) {
                url.searchParams.append(key, params[key]);
            }
        });

        const response = await this.createRequest(url.toString(), {
            method: 'GET',
            ...options
        });

        return this.parseResponse(response);
    }

    /**
     * POST request
     * @param {string} endpoint - API endpoint
     * @param {object} data - Request body data
     * @param {object} options - Additional fetch options
     * @returns {Promise<any>} Response data
     */
    async post(endpoint, data = {}, options = {}) {
        const url = new URL(endpoint, this.baseUrl);

        const response = await this.createRequest(url.toString(), {
            method: 'POST',
            body: JSON.stringify(data),
            ...options
        });

        return this.parseResponse(response);
    }

    /**
     * PUT request
     * @param {string} endpoint - API endpoint
     * @param {object} data - Request body data
     * @param {object} options - Additional fetch options
     * @returns {Promise<any>} Response data
     */
    async put(endpoint, data = {}, options = {}) {
        const url = new URL(endpoint, this.baseUrl);

        const response = await this.createRequest(url.toString(), {
            method: 'PUT',
            body: JSON.stringify(data),
            ...options
        });

        return this.parseResponse(response);
    }

    /**
     * DELETE request
     * @param {string} endpoint - API endpoint
     * @param {object} options - Additional fetch options
     * @returns {Promise<any>} Response data
     */
    async delete(endpoint, options = {}) {
        const url = new URL(endpoint, this.baseUrl);

        const response = await this.createRequest(url.toString(), {
            method: 'DELETE',
            ...options
        });

        return this.parseResponse(response);
    }

    /**
     * Parse response based on content type
     * @param {Response} response - Response object
     * @returns {Promise<any>} Parsed response data
     */
    async parseResponse(response) {
        const contentType = response.headers.get('content-type');

        if (contentType && contentType.includes('application/json')) {
            return await response.json();
        } else if (contentType && contentType.includes('text/')) {
            return await response.text();
        } else {
            // For binary data or other content types
            return await response.blob();
        }
    }

    /**
     * Upload file with progress tracking
     * @param {string} endpoint - Upload endpoint
     * @param {FormData} formData - Form data with file
     * @param {function} onProgress - Progress callback
     * @param {object} options - Additional options
     * @returns {Promise<any>} Upload response
     */
    async uploadFile(endpoint, formData, onProgress = null, options = {}) {
        const url = new URL(endpoint, this.baseUrl);

        // Remove Content-Type header to let browser set it with boundary
        const headers = { ...this.defaultHeaders };
        delete headers['Content-Type'];

        return new Promise((resolve, reject) => {
            const xhr = new XMLHttpRequest();

            // Progress tracking
            if (onProgress && typeof onProgress === 'function') {
                xhr.upload.addEventListener('progress', (event) => {
                    if (event.lengthComputable) {
                        const percentComplete = (event.loaded / event.total) * 100;
                        onProgress(Math.round(percentComplete));
                    }
                });
            }

            // Success handler
            xhr.addEventListener('load', () => {
                if (xhr.status >= 200 && xhr.status < 300) {
                    try {
                        const response = JSON.parse(xhr.responseText);
                        resolve(response);
                    } catch {
                        resolve(xhr.responseText);
                    }
                } else {
                    reject(new ApiError(`Upload failed: HTTP ${xhr.status}`, xhr.status, xhr.responseText));
                }
            });

            // Error handler
            xhr.addEventListener('error', () => {
                reject(new ApiError('Upload failed: Network error', 0, 'Failed to upload file'));
            });

            // Timeout handler
            xhr.addEventListener('timeout', () => {
                reject(new ApiError('Upload failed: Timeout', 408, 'Upload took too long'));
            });

            // Configure request
            xhr.open('POST', url.toString());
            xhr.timeout = this.requestTimeout;

            // Set headers
            Object.keys(headers).forEach(key => {
                xhr.setRequestHeader(key, headers[key]);
            });

            // Send request
            xhr.send(formData);
        });
    }
}

/**
 * Custom API Error class
 */
class ApiError extends Error {
    constructor(message, status = 0, details = null) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
        this.details = details;
    }

    /**
     * Check if error is a client error (4xx)
     * @returns {boolean} True if client error
     */
    isClientError() {
        return this.status >= 400 && this.status < 500;
    }

    /**
     * Check if error is a server error (5xx)
     * @returns {boolean} True if server error
     */
    isServerError() {
        return this.status >= 500;
    }

    /**
     * Check if error is authentication related
     * @returns {boolean} True if auth error
     */
    isAuthError() {
        return this.status === 401 || this.status === 403;
    }

    /**
     * Get user-friendly error message
     * @returns {string} User-friendly message
     */
    getUserMessage() {
        switch (this.status) {
            case 400:
                return 'Invalid request. Please check your input and try again.';
            case 401:
                return 'You need to log in to perform this action.';
            case 403:
                return 'You don\'t have permission to perform this action.';
            case 404:
                return 'The requested resource was not found.';
            case 408:
                return 'The request timed out. Please try again.';
            case 429:
                return 'Too many requests. Please wait a moment and try again.';
            case 500:
                return 'A server error occurred. Please try again later.';
            case 503:
                return 'The service is temporarily unavailable. Please try again later.';
            default:
                return this.details || this.message || 'An unexpected error occurred.';
        }
    }
}

/**
 * Specialized API methods for PriceTracker features
 */
class PriceTrackerApi extends ApiClient {
    constructor() {
        super();
    }

    // === DASHBOARD API METHODS ===

    /**
     * Get dashboard data
     * @returns {Promise<object>} Dashboard data
     */
    async getDashboardData() {
        return this.get('/api/dashboard');
    }

    /**
     * Get budget overview for current month
     * @returns {Promise<object>} Budget overview data
     */
    async getBudgetOverview() {
        return this.get('/api/dashboard/budget-overview');
    }

    /**
     * Get yearly expense summary
     * @param {number} year - Year to fetch data for
     * @returns {Promise<object>} Yearly expense data
     */
    async getYearlyExpenses(year = new Date().getFullYear()) {
        return this.get('/api/dashboard/yearly-expenses', { year });
    }

    /**
     * Get top purchased products
     * @param {number} limit - Number of products to return
     * @returns {Promise<Array>} Top products array
     */
    async getTopProducts(limit = 5) {
        return this.get('/api/dashboard/top-products', { limit });
    }

    /**
     * Get upcoming tasks
     * @param {number} days - Number of days to look ahead
     * @returns {Promise<Array>} Upcoming tasks array
     */
    async getUpcomingTasks(days = 7) {
        return this.get('/api/dashboard/upcoming-tasks', { days });
    }

    // === PRICE TRACKING API METHODS ===

    /**
     * Get price history for a product
     * @param {number} productId - Product ID
     * @param {number} days - Number of days of history
     * @returns {Promise<Array>} Price history data
     */
    async getPriceHistory(productId, days = 30) {
        return this.get(`/api/prices/history/${productId}`, { days });
    }

    /**
     * Get price comparison across stores
     * @param {number} productId - Product ID
     * @returns {Promise<Array>} Price comparison data
     */
    async getPriceComparison(productId) {
        return this.get(`/api/prices/comparison/${productId}`);
    }

    /**
     * Get store recommendations for shopping list
     * @param {Array} productIds - Array of product IDs
     * @returns {Promise<object>} Store recommendations
     */
    async getStoreRecommendations(productIds) {
        return this.post('/api/prices/store-recommendations', { productIds });
    }

    // === BUDGET & EXPENSE API METHODS ===

    /**
     * Get monthly budget data
     * @param {number} year - Year
     * @param {number} month - Month (1-12)
     * @returns {Promise<object>} Monthly budget data
     */
    async getMonthlyBudget(year, month) {
        return this.get('/api/budget/monthly', { year, month });
    }

    /**
     * Get expense categories
     * @returns {Promise<Array>} Expense categories
     */
    async getExpenseCategories() {
        return this.get('/api/expenses/categories');
    }

    /**
     * Add new expense
     * @param {object} expense - Expense data
     * @returns {Promise<object>} Created expense
     */
    async addExpense(expense) {
        return this.post('/api/expenses', expense);
    }

    /**
     * Update expense
     * @param {number} expenseId - Expense ID
     * @param {object} expense - Updated expense data
     * @returns {Promise<object>} Updated expense
     */
    async updateExpense(expenseId, expense) {
        return this.put(`/api/expenses/${expenseId}`, expense);
    }

    /**
     * Delete expense
     * @param {number} expenseId - Expense ID
     * @returns {Promise<boolean>} Success status
     */
    async deleteExpense(expenseId) {
        return this.delete(`/api/expenses/${expenseId}`);
    }

    // === TASK MANAGEMENT API METHODS ===

    /**
     * Get user tasks
     * @param {object} filters - Task filters
     * @returns {Promise<Array>} User tasks
     */
    async getTasks(filters = {}) {
        return this.get('/api/tasks', filters);
    }

    /**
     * Create new task
     * @param {object} task - Task data
     * @returns {Promise<object>} Created task
     */
    async createTask(task) {
        return this.post('/api/tasks', task);
    }

    /**
     * Update task
     * @param {number} taskId - Task ID
     * @param {object} task - Updated task data
     * @returns {Promise<object>} Updated task
     */
    async updateTask(taskId, task) {
        return this.put(`/api/tasks/${taskId}`, task);
    }

    /**
     * Delete task
     * @param {number} taskId - Task ID
     * @returns {Promise<boolean>} Success status
     */
    async deleteTask(taskId) {
        return this.delete(`/api/tasks/${taskId}`);
    }

    /**
     * Mark task as complete
     * @param {number} taskId - Task ID
     * @returns {Promise<object>} Updated task
     */
    async completeTask(taskId) {
        return this.put(`/api/tasks/${taskId}/complete`);
    }

    // === NOTIFICATION API METHODS ===

    /**
     * Get user notifications
     * @param {boolean} unreadOnly - Get only unread notifications
     * @returns {Promise<Array>} Notifications array
     */
    async getNotifications(unreadOnly = false) {
        return this.get('/api/notifications', { unreadOnly });
    }

    /**
     * Mark notification as read
     * @param {number} notificationId - Notification ID
     * @returns {Promise<boolean>} Success status
     */
    async markNotificationRead(notificationId) {
        return this.put(`/api/notifications/${notificationId}/read`);
    }

    /**
     * Mark all notifications as read
     * @returns {Promise<boolean>} Success status
     */
    async markAllNotificationsRead() {
        return this.put('/api/notifications/mark-all-read');
    }

    // === FILE UPLOAD METHODS ===

    /**
     * Upload receipt image
     * @param {File} file - Receipt image file
     * @param {function} onProgress - Progress callback
     * @returns {Promise<object>} Upload result
     */
    async uploadReceipt(file, onProgress = null) {
        const formData = new FormData();
        formData.append('receipt', file);
        return this.uploadFile('/api/receipts/upload', formData, onProgress);
    }

    /**
     * Upload product image
     * @param {File} file - Product image file
     * @param {function} onProgress - Progress callback
     * @returns {Promise<object>} Upload result
     */
    async uploadProductImage(file, onProgress = null) {
        const formData = new FormData();
        formData.append('image', file);
        return this.uploadFile('/api/products/upload-image', formData, onProgress);
    }
}

// Create global API client instance
const api = new PriceTrackerApi();

// Export for module systems and global access
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { PriceTrackerApi, ApiClient, ApiError };
} else {
    window.PriceTrackerApi = PriceTrackerApi;
    window.ApiClient = ApiClient;
    window.ApiError = ApiError;
    window.api = api;
}

// Log API client ready
console.log('PriceTracker API Client ready - Use global "api" object for all API calls');