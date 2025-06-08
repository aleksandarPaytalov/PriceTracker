/**
 * Delete Personal Data - Simple Interactive Features
 * Basic functionality for account deletion
 */

class DeletePersonalDataManager {
    constructor() {
        this.deleteForm = document.getElementById('delete-user');
        this.deleteButton = document.querySelector('.btn-danger');

        this.initializeEventListeners();
        this.initializeAnimations();
    }

    /**
     * Initialize basic event listeners
     */
    initializeEventListeners() {
        // Form submission confirmation
        if (this.deleteForm) {
            this.deleteForm.addEventListener('submit', (e) => {
                if (!this.confirmAccountDeletion()) {
                    e.preventDefault();
                }
            });
        }

        // Button hover effects
        if (this.deleteButton) {
            this.deleteButton.addEventListener('mouseenter', () => {
                this.deleteButton.style.transform = 'translateY(-3px)';
            });

            this.deleteButton.addEventListener('mouseleave', () => {
                this.deleteButton.style.transform = 'translateY(0)';
            });
        }

        // Warning when user tries to leave
        window.addEventListener('beforeunload', (e) => {
            const passwordField = document.querySelector('input[type="password"]');
            if (passwordField && passwordField.value.length > 0) {
                e.preventDefault();
                e.returnValue = 'Are you sure you want to leave? Your progress will be lost.';
                return e.returnValue;
            }
        });
    }

    /**
     * Confirm account deletion with user
     * @returns {boolean} - Whether the user confirmed
     */
    confirmAccountDeletion() {
        const message = `
🚨 PERMANENT ACCOUNT DELETION 🚨

This action will permanently delete your account and ALL associated data:
• Personal information
• Price tracking data  
• Preferences and settings
• All historical data

This action CANNOT be undone!

Are you absolutely sure you want to proceed?
        `.trim();

        return confirm(message);
    }

    /**
     * Initialize basic animations
     */
    initializeAnimations() {
        // Simple fade-in for cards
        const cards = document.querySelectorAll('.warning-card, .deletion-form-section');
        cards.forEach((card, index) => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(20px)';

            setTimeout(() => {
                card.style.transition = 'all 0.6s ease-out';
                card.style.opacity = '1';
                card.style.transform = 'translateY(0)';
            }, index * 200);
        });
    }

    /**
     * Show simple notification message
     * @param {string} message - Message to show
     * @param {string} type - Type of message (success, error, warning)
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

            .simple-notification {
                font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
            }
        `;
        document.head.appendChild(style);
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const deleteManager = new DeletePersonalDataManager();
    deleteManager.addDynamicStyles();

    // Add visual feedback to forms
    const formInputs = document.querySelectorAll('.form-control');
    formInputs.forEach(input => {
        input.addEventListener('focus', () => {
            input.style.transform = 'scale(1.02)';
        });

        input.addEventListener('blur', () => {
            input.style.transform = 'scale(1)';
        });
    });
});

// Export for potential external usage
window.DeletePersonalDataManager = DeletePersonalDataManager;