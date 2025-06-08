/**
 * Profile Index - Simple Interactive Features
 * Basic functionality for profile management
 */

class ProfileIndexManager {
    constructor() {
        this.profileForm = document.getElementById('profile-form');
        this.updateButton = document.getElementById('update-profile-button');
        this.phoneInput = document.querySelector('input[type="tel"], input[name*="PhoneNumber"]');
        this.usernameInput = document.querySelector('input[name*="Username"]');

        this.initializeEventListeners();
        this.initializeAnimations();
        this.initializeProfileAvatar();
        this.setupFormValidation();
    }

    /**
     * Initialize basic event listeners
     */
    initializeEventListeners() {
        // Form submission with loading state
        if (this.profileForm) {
            this.profileForm.addEventListener('submit', (e) => {
                this.handleFormSubmission(e);
            });
        }

        // Real-time phone number formatting
        if (this.phoneInput) {
            this.phoneInput.addEventListener('input', (e) => {
                this.formatPhoneNumber(e.target);
            });

            this.phoneInput.addEventListener('focus', () => {
                this.showFieldInfo('phone');
            });

            this.phoneInput.addEventListener('blur', () => {
                this.hideFieldInfo();
            });
        }

        // Username field info (since it's disabled)
        if (this.usernameInput) {
            this.usernameInput.addEventListener('click', () => {
                this.showNotification('Username cannot be changed after registration', 'info');
            });
        }

        // Button hover effects
        if (this.updateButton) {
            this.updateButton.addEventListener('mouseenter', () => {
                this.updateButton.style.transform = 'translateY(-3px)';
            });

            this.updateButton.addEventListener('mouseleave', () => {
                this.updateButton.style.transform = 'translateY(0)';
            });
        }

        // Auto-save indication
        this.setupAutoSaveIndicator();
    }

    /**
     * Handle form submission with loading state
     * @param {Event} e - Submit event
     */
    handleFormSubmission(e) {
        if (!this.validateForm()) {
            e.preventDefault();
            return;
        }

        // Show loading state
        this.setLoadingState(true);

        // Allow form to submit naturally, but provide user feedback
        this.showNotification('Updating profile...', 'info');

        // Reset loading state after a delay (in case of errors)
        setTimeout(() => {
            this.setLoadingState(false);
        }, 5000);
    }

    /**
     * Set loading state for the form
     * @param {boolean} isLoading - Whether form is loading
     */
    setLoadingState(isLoading) {
        if (!this.updateButton) return;

        if (isLoading) {
            this.updateButton.disabled = true;
            this.updateButton.classList.add('btn-loading');
            this.updateButton.setAttribute('data-original-text', this.updateButton.textContent);
            this.updateButton.textContent = 'Updating...';
        } else {
            this.updateButton.disabled = false;
            this.updateButton.classList.remove('btn-loading');
            const originalText = this.updateButton.getAttribute('data-original-text');
            if (originalText) {
                this.updateButton.textContent = originalText;
            }
        }
    }

    /**
     * Format phone number input
     * @param {HTMLInputElement} input - Phone input element
     */
    formatPhoneNumber(input) {
        let value = input.value.replace(/\D/g, ''); // Remove non-digits

        // Format as (XXX) XXX-XXXX for US numbers
        if (value.length >= 6) {
            value = `(${value.slice(0, 3)}) ${value.slice(3, 6)}-${value.slice(6, 10)}`;
        } else if (value.length >= 3) {
            value = `(${value.slice(0, 3)}) ${value.slice(3)}`;
        }

        input.value = value;
    }

    /**
     * Show field information
     * @param {string} fieldType - Type of field
     */
    showFieldInfo(fieldType) {
        // Remove existing info
        this.hideFieldInfo();

        let message = '';
        let parent = null;

        switch (fieldType) {
            case 'phone':
                message = 'ℹ️ Enter your phone number for account security and notifications';
                parent = this.phoneInput?.closest('.form-floating');
                break;
        }

        if (message && parent) {
            const info = document.createElement('div');
            info.className = 'field-info';
            info.innerHTML = message;
            parent.insertAdjacentElement('afterend', info);
        }
    }

    /**
     * Hide field information
     */
    hideFieldInfo() {
        const existingInfo = document.querySelector('.field-info');
        if (existingInfo) {
            existingInfo.style.animation = 'slideOutUp 0.3s ease-out';
            setTimeout(() => {
                existingInfo.remove();
            }, 300);
        }
    }

    /**
     * Setup form validation
     */
    setupFormValidation() {
        if (this.phoneInput) {
            this.phoneInput.addEventListener('blur', () => {
                this.validatePhoneNumber();
            });
        }
    }

    /**
     * Validate phone number
     * @returns {boolean} - Whether phone number is valid
     */
    validatePhoneNumber() {
        if (!this.phoneInput) return true;

        const phoneValue = this.phoneInput.value.replace(/\D/g, '');

        // Remove existing error
        const existingError = this.phoneInput.parentNode.querySelector('.text-danger');
        if (existingError) {
            existingError.remove();
        }

        // Validate if not empty
        if (phoneValue && phoneValue.length !== 10) {
            const error = document.createElement('span');
            error.className = 'text-danger';
            error.textContent = 'Please enter a valid 10-digit phone number';
            this.phoneInput.parentNode.appendChild(error);
            return false;
        }

        return true;
    }

    /**
     * Validate entire form
     * @returns {boolean} - Whether form is valid
     */
    validateForm() {
        const isPhoneValid = this.validatePhoneNumber();
        return isPhoneValid;
    }

    /**
     * Initialize profile avatar
     */
    initializeProfileAvatar() {
        // Create avatar if it doesn't exist
        let avatar = document.querySelector('.profile-avatar');
        if (!avatar) {
            const profileHeader = this.createProfileHeader();
            this.profileForm?.parentNode.insertBefore(profileHeader, this.profileForm);
        }
    }

    /**
     * Create profile header with avatar
     * @returns {HTMLElement} - Profile header element
     */
    createProfileHeader() {
        const username = this.usernameInput?.value || 'User';
        const initials = this.getInitials(username);

        const header = document.createElement('div');
        header.className = 'profile-header';
        header.innerHTML = `
            <div class="profile-avatar">${initials}</div>
            <div class="profile-title">Profile Settings</div>
            <div class="profile-subtitle">Manage your account information</div>
        `;

        return header;
    }

    /**
     * Get user initials for avatar
     * @param {string} name - User name
     * @returns {string} - User initials
     */
    getInitials(name) {
        return name
            .split(' ')
            .map(part => part.charAt(0).toUpperCase())
            .slice(0, 2)
            .join('');
    }

    /**
     * Setup auto-save indicator
     */
    setupAutoSaveIndicator() {
        const inputs = this.profileForm?.querySelectorAll('input:not([disabled])');
        if (!inputs) return;

        let changeTimeout;

        inputs.forEach(input => {
            input.addEventListener('input', () => {
                clearTimeout(changeTimeout);
                this.showChangeIndicator();

                changeTimeout = setTimeout(() => {
                    this.hideChangeIndicator();
                }, 2000);
            });
        });
    }

    /**
     * Show change indicator
     */
    showChangeIndicator() {
        if (!this.updateButton) return;

        const indicator = document.querySelector('.change-indicator');
        if (indicator) return;

        const span = document.createElement('span');
        span.className = 'change-indicator';
        span.innerHTML = ' <small style="color: #f59e0b;">●</small>';
        this.updateButton.appendChild(span);
    }

    /**
     * Hide change indicator
     */
    hideChangeIndicator() {
        const indicator = document.querySelector('.change-indicator');
        if (indicator) {
            indicator.remove();
        }
    }

    /**
     * Initialize basic animations
     */
    initializeAnimations() {
        // Simple fade-in for form elements
        const formElements = document.querySelectorAll('.form-floating, .form-actions');
        formElements.forEach((element, index) => {
            element.style.opacity = '0';
            element.style.transform = 'translateY(20px)';

            setTimeout(() => {
                element.style.transition = 'all 0.6s ease-out';
                element.style.opacity = '1';
                element.style.transform = 'translateY(0)';
            }, index * 100);
        });

        // Add focus animations
        const inputs = document.querySelectorAll('.form-control');
        inputs.forEach(input => {
            input.addEventListener('focus', () => {
                input.style.transform = 'translateY(-2px)';
            });

            input.addEventListener('blur', () => {
                input.style.transform = 'translateY(0)';
            });
        });
    }

    /**
     * Show simple notification message
     * @param {string} message - Message to show
     * @param {string} type - Type of message (success, error, warning, info)
     */
    showNotification(message, type = 'info') {
        // Remove existing notification
        const existing = document.querySelector('.simple-notification');
        if (existing) {
            existing.remove();
        }

        // Create notification
        const notification = document.createElement('div');
        notification.className = `simple-notification ${type}`;
        notification.textContent = message;

        // Style the notification
        const colors = {
            success: '#10b981',
            error: '#dc2626',
            warning: '#f59e0b',
            info: '#3b82f6'
        };

        Object.assign(notification.style, {
            position: 'fixed',
            top: '20px',
            right: '20px',
            background: colors[type] || colors.info,
            color: 'white',
            padding: '12px 16px',
            borderRadius: '8px',
            boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
            zIndex: '1000',
            fontWeight: '500',
            fontSize: '14px',
            maxWidth: '300px',
            animation: 'slideInRight 0.3s ease-out'
        });

        document.body.appendChild(notification);

        // Auto-remove after 4 seconds
        setTimeout(() => {
            notification.style.animation = 'slideOutRight 0.3s ease-out';
            setTimeout(() => {
                notification.remove();
            }, 300);
        }, 4000);
    }

    /**
     * Add dynamic styles for animations
     */
    addDynamicStyles() {
        const style = document.createElement('style');
        style.textContent = `
            @keyframes slideInRight {
                from {
                    opacity: 0;
                    transform: translateX(100px);
                }
                to {
                    opacity: 1;
                    transform: translateX(0);
                }
            }

            @keyframes slideOutRight {
                from {
                    opacity: 1;
                    transform: translateX(0);
                }
                to {
                    opacity: 0;
                    transform: translateX(100px);
                }
            }

            @keyframes slideOutUp {
                from {
                    opacity: 1;
                    transform: translateY(0);
                }
                to {
                    opacity: 0;
                    transform: translateY(-20px);
                }
            }

            .simple-notification {
                font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
            }

            .change-indicator {
                animation: pulse 1.5s ease-in-out infinite;
            }
        `;
        document.head.appendChild(style);
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const profileManager = new ProfileIndexManager();
    profileManager.addDynamicStyles();

    // Handle status messages if they exist
    const statusMessageData = document.getElementById('status-message-data');
    if (statusMessageData) {
        const message = statusMessageData.getAttribute('data-message');
        const type = statusMessageData.getAttribute('data-type') || 'info';
        if (message) {
            profileManager.showNotification(message, type);
        }
    }

    // Add wrapper class to body for consistent styling
    document.body.classList.add('profile-index-page');
});

// Export for potential external usage
window.ProfileIndexManager = ProfileIndexManager;