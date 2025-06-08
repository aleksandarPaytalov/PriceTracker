/**
 * Personal Data Management - Interactive Features
 * Handles user interactions for the personal data page
 */

class PersonalDataManager {
    constructor() {
        this.initializeEventListeners();
        this.initializeAnimations();
        this.initializeDeleteConfirmation();
    }

    /**
     * Initialize all event listeners
     */
    initializeEventListeners() {
        // Delete confirmation checkbox
        const confirmCheckbox = document.getElementById('confirm-delete');
        const deleteButton = document.getElementById('delete-button');
        
        if (confirmCheckbox && deleteButton) {
            confirmCheckbox.addEventListener('change', (e) => {
                this.toggleDeleteButton(e.target.checked);
            });
        }

        // Form submission confirmations
        this.initializeFormConfirmations();
        
        // Animated counters for stats
        this.initializeStatCounters();
        
        // Tooltip enhancements
        this.initializeTooltips();
    }

    /**
     * Toggle delete button based on confirmation checkbox
     * @param {boolean} isChecked - Whether the checkbox is checked
     */
    toggleDeleteButton(isChecked) {
        const deleteButton = document.getElementById('delete-button');
        if (!deleteButton) return;

        if (isChecked) {
            deleteButton.disabled = false;
            deleteButton.style.transform = 'scale(1)';
            deleteButton.style.opacity = '1';
            this.addButtonPulse(deleteButton);
        } else {
            deleteButton.disabled = true;
            deleteButton.style.transform = 'scale(0.95)';
            deleteButton.style.opacity = '0.5';
            this.removeButtonPulse(deleteButton);
        }
    }

    /**
     * Add pulsing animation to delete button when enabled
     * @param {HTMLElement} button - The delete button element
     */
    addButtonPulse(button) {
        button.style.animation = 'dangerPulse 2s ease-in-out infinite';
    }

    /**
     * Remove pulsing animation from delete button
     * @param {HTMLElement} button - The delete button element
     */
    removeButtonPulse(button) {
        button.style.animation = 'none';
    }

    /**
     * Initialize form submission confirmations
     */
    initializeFormConfirmations() {
        // Download data confirmation
        const downloadForm = document.querySelector('form[asp-page-handler="DownloadPersonalData"]');
        if (downloadForm) {
            downloadForm.addEventListener('submit', (e) => {
                this.showDownloadProgress();
            });
        }

        // Export data confirmation
        const exportForm = document.querySelector('form[asp-page-handler="ExportData"]');
        if (exportForm) {
            exportForm.addEventListener('submit', (e) => {
                this.showExportProgress();
            });
        }

        // Delete account confirmation
        const deleteForm = document.getElementById('delete-form');
        if (deleteForm) {
            deleteForm.addEventListener('submit', (e) => {
                if (!this.confirmAccountDeletion()) {
                    e.preventDefault();
                }
            });
        }
    }

    /**
     * Show download progress indication
     */
    showDownloadProgress() {
        const downloadBtn = document.querySelector('.btn-download');
        if (downloadBtn) {
            const originalText = downloadBtn.innerHTML;
            downloadBtn.innerHTML = '<span>⏳</span> Preparing Download...';
            downloadBtn.disabled = true;

            // Reset button after a delay
            setTimeout(() => {
                downloadBtn.innerHTML = originalText;
                downloadBtn.disabled = false;
            }, 3000);
        }
    }

    /**
     * Show export progress indication
     */
    showExportProgress() {
        const exportBtn = document.querySelector('.btn-export');
        if (exportBtn) {
            const originalText = exportBtn.innerHTML;
            exportBtn.innerHTML = '<span>⏳</span> Exporting Data...';
            exportBtn.disabled = true;

            // Reset button after a delay
            setTimeout(() => {
                exportBtn.innerHTML = originalText;
                exportBtn.disabled = false;
            }, 3000);
        }
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
     * Initialize entrance animations for elements
     */
    initializeAnimations() {
        // Observe elements for scroll-triggered animations
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.animationPlayState = 'running';
                    observer.unobserve(entry.target);
                }
            });
        }, observerOptions);

        // Observe animated elements
        const animatedElements = document.querySelectorAll('.info-card, .action-card, .stat-card');
        animatedElements.forEach((el, index) => {
            el.style.animationDelay = `${index * 0.1}s`;
            el.style.animationPlayState = 'paused';
            observer.observe(el);
        });
    }

    /**
     * Initialize delete confirmation flow
     */
    initializeDeleteConfirmation() {
        const confirmCheckbox = document.getElementById('confirm-delete');
        const deleteButton = document.getElementById('delete-button');

        if (confirmCheckbox && deleteButton) {
            // Add enhanced styling when checkbox is focused
            confirmCheckbox.addEventListener('focus', () => {
                confirmCheckbox.parentElement.style.boxShadow = '0 0 0 2px rgba(255, 154, 158, 0.3)';
            });

            confirmCheckbox.addEventListener('blur', () => {
                confirmCheckbox.parentElement.style.boxShadow = 'none';
            });

            // Add click animation to checkbox
            confirmCheckbox.addEventListener('change', () => {
                confirmCheckbox.parentElement.style.transform = 'scale(0.95)';
                setTimeout(() => {
                    confirmCheckbox.parentElement.style.transform = 'scale(1)';
                }, 150);
            });
        }
    }

    /**
     * Initialize animated counters for statistics
     */
    initializeStatCounters() {
        const statValues = document.querySelectorAll('.stat-value');
        
        statValues.forEach(statValue => {
            const text = statValue.textContent;
            const number = parseInt(text);
            
            if (!isNaN(number) && number > 0) {
                this.animateCounter(statValue, 0, number, 1000);
            }
        });
    }

    /**
     * Animate counter from start to end value
     * @param {HTMLElement} element - Element to animate
     * @param {number} start - Starting value
     * @param {number} end - Ending value
     * @param {number} duration - Animation duration in ms
     */
    animateCounter(element, start, end, duration) {
        const startTime = performance.now();
        
        const updateCounter = (currentTime) => {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            // Easing function for smooth animation
            const easeOut = 1 - Math.pow(1 - progress, 3);
            const currentValue = Math.floor(start + (end - start) * easeOut);
            
            element.textContent = currentValue.toLocaleString();
            
            if (progress < 1) {
                requestAnimationFrame(updateCounter);
            } else {
                element.textContent = end.toLocaleString();
            }
        };
        
        requestAnimationFrame(updateCounter);
    }

    /**
     * Initialize tooltips for enhanced user experience
     */
    initializeTooltips() {
        const elements = document.querySelectorAll('[title]');
        
        elements.forEach(element => {
            element.addEventListener('mouseenter', (e) => {
                this.showTooltip(e.target, e.target.getAttribute('title'));
            });
            
            element.addEventListener('mouseleave', () => {
                this.hideTooltip();
            });
        });
    }

    /**
     * Show custom tooltip
     * @param {HTMLElement} target - Target element
     * @param {string} text - Tooltip text
     */
    showTooltip(target, text) {
        const existingTooltip = document.querySelector('.custom-tooltip');
        if (existingTooltip) {
            existingTooltip.remove();
        }

        const tooltip = document.createElement('div');
        tooltip.className = 'custom-tooltip';
        tooltip.textContent = text;
        
        // Style the tooltip
        Object.assign(tooltip.style, {
            position: 'absolute',
            background: 'linear-gradient(135deg, #667eea, #764ba2)',
            color: 'white',
            padding: '8px 12px',
            borderRadius: '8px',
            fontSize: '12px',
            fontWeight: '500',
            zIndex: '1000',
            whiteSpace: 'nowrap',
            boxShadow: '0 4px 12px rgba(0,0,0,0.2)',
            animation: 'tooltipFadeIn 0.2s ease-out'
        });

        document.body.appendChild(tooltip);

        // Position the tooltip
        const rect = target.getBoundingClientRect();
        const tooltipRect = tooltip.getBoundingClientRect();
        
        tooltip.style.left = `${rect.left + (rect.width - tooltipRect.width) / 2}px`;
        tooltip.style.top = `${rect.top - tooltipRect.height - 8}px`;
    }

    /**
     * Hide custom tooltip
     */
    hideTooltip() {
        const tooltip = document.querySelector('.custom-tooltip');
        if (tooltip) {
            tooltip.style.animation = 'tooltipFadeOut 0.2s ease-out';
            setTimeout(() => {
                tooltip.remove();
            }, 200);
        }
    }

    /**
     * Add dynamic styles for enhanced interactions
     */
    addDynamicStyles() {
        const style = document.createElement('style');
        style.textContent = `
            @keyframes dangerPulse {
                0%, 100% {
                    box-shadow: 0 4px 20px rgba(255, 154, 158, 0.4);
                }
                50% {
                    box-shadow: 0 8px 40px rgba(255, 154, 158, 0.8);
                }
            }

            @keyframes tooltipFadeIn {
                from {
                    opacity: 0;
                    transform: translateY(5px);
                }
                to {
                    opacity: 1;
                    transform: translateY(0);
                }
            }

            @keyframes tooltipFadeOut {
                from {
                    opacity: 1;
                    transform: translateY(0);
                }
                to {
                    opacity: 0;
                    transform: translateY(5px);
                }
            }

            .info-card {
                animation: slideInFromLeft 0.6s ease-out both;
            }

            .action-card {
                animation: slideInFromRight 0.6s ease-out both;
            }

            .stat-card {
                animation: scaleInBounce 0.6s ease-out both;
            }

            @keyframes slideInFromLeft {
                from {
                    opacity: 0;
                    transform: translateX(-30px);
                }
                to {
                    opacity: 1;
                    transform: translateX(0);
                }
            }

            @keyframes slideInFromRight {
                from {
                    opacity: 0;
                    transform: translateX(30px);
                }
                to {
                    opacity: 1;
                    transform: translateX(0);
                }
            }

            @keyframes scaleInBounce {
                from {
                    opacity: 0;
                    transform: scale(0.8);
                }
                to {
                    opacity: 1;
                    transform: scale(1);
                }
            }
        `;
        document.head.appendChild(style);
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const personalDataManager = new PersonalDataManager();
    personalDataManager.addDynamicStyles();
    
    // Add some visual polish
    setTimeout(() => {
        document.querySelectorAll('.info-card').forEach((card, index) => {
            setTimeout(() => {
                card.style.transform = 'translateY(0)';
                card.style.opacity = '1';
            }, index * 100);
        });
    }, 300);
});

// Export for potential external usage
window.PersonalDataManager = PersonalDataManager;