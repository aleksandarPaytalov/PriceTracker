/**
 * Two-Factor Authentication - Account Management JavaScript
 * Simple functionality for 2FA management without recovery codes
 */

class TwoFactorAuthManager {
    constructor() {
        this.forgetForm = document.querySelector('.forget-form');
        this.dangerButtons = document.querySelectorAll('.btn-danger-custom');

        this.initializeEventListeners();
        this.initializeAnimations();
        this.setupFormValidation();
    }

    /**
     * Initialize event listeners
     */
    initializeEventListeners() {
        // Forget device form confirmation
        if (this.forgetForm) {
            this.forgetForm.addEventListener('submit', (e) => {
                if (!this.confirmForgetDevice()) {
                    e.preventDefault();
                }
            });
        }

        // Disable 2FA confirmation
        this.dangerButtons.forEach(button => {
            if (button.href && button.href.includes('Disable2fa')) {
                button.addEventListener('click', (e) => {
                    if (!this.confirmDisable2FA()) {
                        e.preventDefault();
                    }
                });
            }
        });

        // Recovery codes generation confirmation
        const recoveryCodes = document.querySelectorAll('a[href*="GenerateRecoveryCodes"]');
        recoveryCodes.forEach(button => {
            button.addEventListener('click', (e) => {
                if (!this.confirmGenerateRecoveryCodes()) {
                    e.preventDefault();
                }
            });
        });

        // Button hover effects
        this.setupButtonEffects();

        // Status message auto-hide
        this.handleStatusMessages();

        // Recovery codes alerts enhancement
        this.enhanceRecoveryAlerts();
    }

    /**
     * Confirm forget device action
     * @returns {boolean} - Whether user confirmed
     */
    confirmForgetDevice() {
        return confirm(
            'Are you sure you want to forget this device?\n\n' +
            'You will need to enter a verification code from your authenticator app the next time you sign in from this device.'
        );
    }

    /**
     * Confirm disable 2FA action
     * @returns {boolean} - Whether user confirmed
     */
    confirmDisable2FA() {
        return confirm(
            '🚨 DISABLE TWO-FACTOR AUTHENTICATION 🚨\n\n' +
            'This will reduce your account security!\n\n' +
            'Are you sure you want to disable two-factor authentication?\n\n' +
            'You will only be protected by your password after this change.'
        );
    }

    /**
     * Confirm generate recovery codes action
     * @returns {boolean} - Whether user confirmed
     */
    confirmGenerateRecoveryCodes() {
        return confirm(
            '🔑 GENERATE NEW RECOVERY CODES 🔑\n\n' +
            'This will invalidate all your existing recovery codes!\n\n' +
            'Make sure to save the new codes in a secure location.\n\n' +
            'Do you want to continue?'
        );
    }

    /**
     * Setup button hover effects
     */
    setupButtonEffects() {
        const buttons = document.querySelectorAll('.btn-custom');

        buttons.forEach(button => {
            // Add ripple effect on click
            button.addEventListener('click', (e) => {
                this.createRippleEffect(button, e);
            });

            // Enhanced hover animations
            button.addEventListener('mouseenter', () => {
                if (!button.classList.contains('btn-danger-custom')) {
                    button.style.transform = 'translateY(-3px) scale(1.02)';
                }
            });

            button.addEventListener('mouseleave', () => {
                button.style.transform = 'translateY(0) scale(1)';
            });
        });

        // Special effects for danger buttons
        this.dangerButtons.forEach(button => {
            button.addEventListener('mouseenter', () => {
                button.style.animation = 'dangerPulse 1s ease-in-out infinite';
            });

            button.addEventListener('mouseleave', () => {
                button.style.animation = 'none';
            });
        });
    }

    /**
     * Create ripple effect on button click
     * @param {HTMLElement} button - Button element
     * @param {Event} e - Click event
     */
    createRippleEffect(button, e) {
        const ripple = document.createElement('span');
        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;

        ripple.style.cssText = `
            position: absolute;
            border-radius: 50%;
            background: rgba(255, 255, 255, 0.4);
            transform: scale(0);
            animation: ripple 0.6s linear;
            width: ${size}px;
            height: ${size}px;
            left: ${x}px;
            top: ${y}px;
            pointer-events: none;
        `;

        button.style.position = 'relative';
        button.appendChild(ripple);

        setTimeout(() => {
            ripple.remove();
        }, 600);
    }

    /**
     * Setup form validation
     */
    setupFormValidation() {
        // Add loading states to form submissions
        const forms = document.querySelectorAll('form');
        forms.forEach(form => {
            form.addEventListener('submit', (e) => {
                const submitButton = form.querySelector('button[type="submit"]');
                if (submitButton) {
                    this.setButtonLoading(submitButton, true);
                }
            });
        });
    }

    /**
     * Set button loading state
     * @param {HTMLElement} button - Button element
     * @param {boolean} isLoading - Loading state
     */
    setButtonLoading(button, isLoading) {
        if (isLoading) {
            button.dataset.originalText = button.innerHTML;
            button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processing...';
            button.disabled = true;
        } else {
            button.innerHTML = button.dataset.originalText || button.innerHTML;
            button.disabled = false;
        }
    }

    /**
     * Handle status messages
     */
    handleStatusMessages() {
        const statusMessages = document.querySelectorAll('.alert');

        statusMessages.forEach(message => {
            if (message.textContent.trim()) {
                // Auto-hide after 5 seconds
                setTimeout(() => {
                    this.hideStatusMessage(message);
                }, 5000);

                // Add close button if not present
                if (!message.querySelector('.btn-close')) {
                    this.addCloseButton(message);
                }
            }
        });
    }

    /**
     * Add close button to status message
     * @param {HTMLElement} message - Status message element
     */
    addCloseButton(message) {
        const closeButton = document.createElement('button');
        closeButton.type = 'button';
        closeButton.className = 'btn-close';
        closeButton.setAttribute('aria-label', 'Close');
        closeButton.style.cssText = `
            position: absolute;
            top: 10px;
            right: 10px;
            background: none;
            border: none;
            color: currentColor;
            opacity: 0.7;
            cursor: pointer;
            font-size: 1.2rem;
        `;
        closeButton.innerHTML = '×';
        closeButton.addEventListener('click', () => this.hideStatusMessage(message));

        message.style.position = 'relative';
        message.appendChild(closeButton);
    }

    /**
     * Hide status message with animation
     * @param {HTMLElement} message - Message to hide
     */
    hideStatusMessage(message) {
        message.style.transition = 'all 0.3s ease-out';
        message.style.opacity = '0';
        message.style.transform = 'translateY(-10px)';

        setTimeout(() => {
            if (message.parentNode) {
                message.remove();
            }
        }, 300);
    }

    /**
     * Enhance recovery alerts with interactive features
     */
    enhanceRecoveryAlerts() {
        const recoveryAlerts = document.querySelectorAll('.recovery-alert');

        recoveryAlerts.forEach(alert => {
            // Add pulse animation for critical alerts
            if (alert.classList.contains('danger')) {
                alert.style.animation = 'recoveryCriticalPulse 2s ease-in-out infinite';
            }

            // Add click to dismiss functionality
            alert.style.cursor = 'pointer';
            alert.title = 'Click to dismiss';

            alert.addEventListener('click', () => {
                this.hideStatusMessage(alert);
            });

            // Add hover effects
            alert.addEventListener('mouseenter', () => {
                alert.style.transform = 'translateY(-2px)';
            });

            alert.addEventListener('mouseleave', () => {
                alert.style.transform = 'translateY(0)';
            });
        });

        // Add counter animation to recovery count
        const recoveryCount = document.querySelector('.recovery-count-large');
        if (recoveryCount) {
            this.animateRecoveryCount(recoveryCount);
        }
    }

    /**
     * Animate recovery count number
     * @param {HTMLElement} element - Recovery count element
     */
    animateRecoveryCount(element) {
        const finalCount = parseInt(element.textContent);
        if (isNaN(finalCount)) return;

        let currentCount = 0;
        const increment = finalCount / 20;
        const timer = setInterval(() => {
            currentCount += increment;
            if (currentCount >= finalCount) {
                currentCount = finalCount;
                clearInterval(timer);
            }
            element.textContent = Math.floor(currentCount).toString();
        }, 50);
    }

    /**
     * Initialize page animations
     */
    initializeAnimations() {
        // Animate elements on page load
        const animatedElements = [
            '.status-card',
            '.recovery-alert',
            '.recovery-section',
            '.authenticator-section',
            '.management-section',
            '.consent-required'
        ];

        animatedElements.forEach((selector, index) => {
            const elements = document.querySelectorAll(selector);
            elements.forEach(element => {
                element.style.opacity = '0';
                element.style.transform = 'translateY(30px)';

                setTimeout(() => {
                    element.style.transition = 'all 0.6s ease-out';
                    element.style.opacity = '1';
                    element.style.transform = 'translateY(0)';
                }, (index + 1) * 200);
            });
        });

        // Add success pulse animation to enabled status
        const successIcons = document.querySelectorAll('.status-icon.success');
        successIcons.forEach(icon => {
            icon.style.animation = 'successPulse 2s ease-in-out infinite';
        });

        // Add special animation to recovery status card
        const recoveryStatusCard = document.querySelector('.recovery-status-card');
        if (recoveryStatusCard) {
            recoveryStatusCard.style.animation = 'recoveryCardPulse 3s ease-in-out infinite';
        }
    }

    /**
     * Show notification
     * @param {string} message - Message to show
     * @param {string} type - Type (success, error, warning, info)
     */
    showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;

        const colors = {
            success: '#10b981',
            error: '#dc2626',
            warning: '#f59e0b',
            info: '#3b82f6'
        };

        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: ${colors[type]};
            color: white;
            padding: 12px 20px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            z-index: 1000;
            font-weight: 500;
            max-width: 300px;
            animation: slideInRight 0.3s ease-out;
        `;

        notification.textContent = message;
        document.body.appendChild(notification);

        setTimeout(() => {
            notification.style.animation = 'slideOutRight 0.3s ease-out';
            setTimeout(() => notification.remove(), 300);
        }, 4000);
    }

    /**
     * Add dynamic styles
     */
    addDynamicStyles() {
        const style = document.createElement('style');
        style.textContent = `
            @keyframes ripple {
                to {
                    transform: scale(2);
                    opacity: 0;
                }
            }

            @keyframes dangerPulse {
                0%, 100% {
                    box-shadow: 0 4px 20px rgba(220, 38, 38, 0.3);
                }
                50% {
                    box-shadow: 0 8px 40px rgba(220, 38, 38, 0.6);
                }
            }

            @keyframes recoveryCriticalPulse {
                0%, 100% {
                    box-shadow: 0 4px 20px rgba(220, 38, 38, 0.2);
                    transform: scale(1);
                }
                50% {
                    box-shadow: 0 8px 40px rgba(220, 38, 38, 0.4);
                    transform: scale(1.02);
                }
            }

            @keyframes recoveryCardPulse {
                0%, 100% {
                    box-shadow: 0 4px 20px rgba(16, 185, 129, 0.2);
                }
                50% {
                    box-shadow: 0 8px 40px rgba(16, 185, 129, 0.4);
                }
            }

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

            .btn-custom {
                overflow: hidden;
            }

            .notification {
                font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
            }

            .recovery-alert {
                transition: all 0.3s ease-out;
            }
        `;
        document.head.appendChild(style);
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const twoFactorManager = new TwoFactorAuthManager();
    twoFactorManager.addDynamicStyles();

    // Add page-specific enhancements
    document.body.classList.add('two-factor-auth-page');
});

// Export for potential external usage
window.TwoFactorAuthManager = TwoFactorAuthManager;