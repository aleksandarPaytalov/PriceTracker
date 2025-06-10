/**
 * Login with Recovery Code - Enhanced User Experience
 * Provides client-side validation, formatting, and user feedback
 */

document.addEventListener('DOMContentLoaded', function () {
    // Get form elements
    const recoveryForm = document.querySelector('form');
    const recoveryInput = document.querySelector('.recovery-input');
    const submitButton = document.querySelector('.btn-auth');

    // Initialize functionality
    initializeRecoveryInput();
    initializeFormValidation();
    initializeSubmissionFeedback();
    initializeAccessibilityFeatures();
});

/**
 * Initialize recovery code input functionality
 */
function initializeRecoveryInput() {
    const recoveryInput = document.querySelector('.recovery-input');
    if (!recoveryInput) return;

    // Auto-focus on the recovery input
    recoveryInput.focus();

    // Format input as user types
    recoveryInput.addEventListener('input', function (e) {
        formatRecoveryCode(e.target);
        validateRecoveryCode(e.target);
    });

    // Handle paste events
    recoveryInput.addEventListener('paste', function (e) {
        setTimeout(() => {
            formatRecoveryCode(e.target);
            validateRecoveryCode(e.target);
        }, 10);
    });

    // Clear formatting on focus for better UX
    recoveryInput.addEventListener('focus', function () {
        this.select(); // Select all text for easy replacement
    });

    // Validate on blur
    recoveryInput.addEventListener('blur', function () {
        validateRecoveryCode(this);
    });
}

/**
 * Format recovery code input
 * @param {HTMLInputElement} input - Recovery code input element
 */
function formatRecoveryCode(input) {
    let value = input.value;

    // Remove any non-alphanumeric characters and convert to uppercase
    value = value.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();

    // Limit to reasonable length (most recovery codes are 8-16 characters)
    value = value.substring(0, 16);

    // Add visual separation for better readability (every 4 characters)
    if (value.length > 4) {
        value = value.match(/.{1,4}/g).join('-');
    }

    input.value = value;
}

/**
 * Validate recovery code format
 * @param {HTMLInputElement} input - Recovery code input element
 */
function validateRecoveryCode(input) {
    const value = input.value.replace(/[^a-zA-Z0-9]/g, ''); // Remove formatting
    const isValid = value.length >= 6; // Minimum reasonable length

    // Update input styling based on validation
    if (value.length === 0) {
        // Empty state
        input.classList.remove('is-valid', 'is-invalid');
        updateInputFeedback(input, '', 'neutral');
    } else if (isValid) {
        // Valid state
        input.classList.add('is-valid');
        input.classList.remove('is-invalid');
        updateInputFeedback(input, '✓ Recovery code format looks good', 'success');
    } else {
        // Invalid state
        input.classList.add('is-invalid');
        input.classList.remove('is-valid');
        updateInputFeedback(input, '⚠ Recovery code seems too short', 'warning');
    }

    return isValid;
}

/**
 * Update input feedback message
 * @param {HTMLInputElement} input - Input element
 * @param {string} message - Feedback message
 * @param {string} type - Message type (success, warning, error, neutral)
 */
function updateInputFeedback(input, message, type) {
    let feedbackElement = input.parentNode.querySelector('.input-feedback');

    if (!feedbackElement) {
        feedbackElement = document.createElement('div');
        feedbackElement.className = 'input-feedback form-text mt-1';
        input.parentNode.appendChild(feedbackElement);
    }

    feedbackElement.textContent = message;
    feedbackElement.className = `input-feedback form-text mt-1 text-${type === 'success' ? 'success' : type === 'warning' ? 'warning' : type === 'error' ? 'danger' : 'muted'}`;
}

/**
 * Initialize form validation
 */
function initializeFormValidation() {
    const recoveryForm = document.querySelector('form');
    if (!recoveryForm) return;

    recoveryForm.addEventListener('submit', function (e) {
        const recoveryInput = document.querySelector('.recovery-input');

        if (!recoveryInput) return;

        const cleanValue = recoveryInput.value.replace(/[^a-zA-Z0-9]/g, '');

        // Validate recovery code
        if (cleanValue.length < 6) {
            e.preventDefault();
            showValidationError('Please enter a valid recovery code (at least 6 characters).');
            recoveryInput.focus();
            return false;
        }

        // If validation passes, show loading state
        showLoadingState();
    });
}

/**
 * Show validation error message
 * @param {string} message - Error message to display
 */
function showValidationError(message) {
    // Remove existing error messages
    const existingErrors = document.querySelectorAll('.validation-error-message');
    existingErrors.forEach(error => error.remove());

    // Create new error message
    const errorElement = document.createElement('div');
    errorElement.className = 'validation-error-message alert alert-danger mt-3';
    errorElement.innerHTML = `<strong>Validation Error:</strong> ${message}`;

    // Insert error message before the form
    const form = document.querySelector('form');
    form.parentNode.insertBefore(errorElement, form);

    // Auto-remove after 5 seconds
    setTimeout(() => {
        errorElement.remove();
    }, 5000);

    // Add shake animation to form
    const authCard = document.querySelector('.auth-card');
    authCard.style.animation = 'shake 0.5s ease-in-out';
    setTimeout(() => {
        authCard.style.animation = '';
    }, 500);
}

/**
 * Initialize submission feedback
 */
function initializeSubmissionFeedback() {
    const submitButton = document.querySelector('.btn-auth');
    if (!submitButton) return;

    // Store original button text
    submitButton.setAttribute('data-original-text', submitButton.innerHTML);

    // Add hover effect
    submitButton.addEventListener('mouseenter', function () {
        if (!this.disabled) {
            this.style.transform = 'translateY(-2px)';
        }
    });

    submitButton.addEventListener('mouseleave', function () {
        if (!this.disabled) {
            this.style.transform = 'translateY(0)';
        }
    });
}

/**
 * Show loading state during form submission
 */
function showLoadingState() {
    const submitButton = document.querySelector('.btn-auth');
    const recoveryInput = document.querySelector('.recovery-input');

    if (submitButton) {
        submitButton.disabled = true;
        submitButton.classList.add('btn-loading');
        submitButton.innerHTML = '<i class="spinner-border spinner-border-sm me-2"></i>Verifying Recovery Code...';
    }

    if (recoveryInput) {
        recoveryInput.disabled = true;
    }

    // Show progress message
    showProgressMessage('Verifying your recovery code...', 'info');
}

/**
 * Reset form to normal state (useful for errors)
 */
function resetFormState() {
    const submitButton = document.querySelector('.btn-auth');
    const recoveryInput = document.querySelector('.recovery-input');

    if (submitButton) {
        submitButton.disabled = false;
        submitButton.classList.remove('btn-loading');
        const originalText = submitButton.getAttribute('data-original-text');
        submitButton.innerHTML = originalText || 'Sign In with Recovery Code';
    }

    if (recoveryInput) {
        recoveryInput.disabled = false;
        recoveryInput.focus();
    }
}

/**
 * Show progress message
 * @param {string} message - Progress message
 * @param {string} type - Message type
 */
function showProgressMessage(message, type = 'info') {
    const progressElement = document.createElement('div');
    progressElement.className = `progress-message alert alert-${type} mt-3`;
    progressElement.innerHTML = `<i class="bi bi-clock me-2"></i>${message}`;

    const form = document.querySelector('form');
    form.parentNode.insertBefore(progressElement, form.nextSibling);

    // Remove after 10 seconds (form submission should complete by then)
    setTimeout(() => {
        progressElement.remove();
    }, 10000);
}

/**
 * Initialize accessibility features
 */
function initializeAccessibilityFeatures() {
    const recoveryInput = document.querySelector('.recovery-input');
    if (!recoveryInput) return;

    // Add ARIA attributes for better screen reader support
    recoveryInput.setAttribute('aria-describedby', 'recovery-code-help');
    recoveryInput.setAttribute('autocomplete', 'one-time-code');

    // Add keyboard shortcuts
    document.addEventListener('keydown', function (e) {
        // Focus recovery input with Ctrl/Cmd + R
        if ((e.ctrlKey || e.metaKey) && e.key === 'r') {
            e.preventDefault();
            recoveryInput.focus();
        }

        // Submit form with Ctrl/Cmd + Enter
        if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
            e.preventDefault();
            document.querySelector('form').submit();
        }
    });

    // Add visual feedback for keyboard navigation
    recoveryInput.addEventListener('focus', function () {
        this.parentNode.style.boxShadow = '0 0 0 3px rgba(102, 126, 234, 0.2)';
    });

    recoveryInput.addEventListener('blur', function () {
        this.parentNode.style.boxShadow = '';
    });
}

/**
 * Add dynamic CSS for additional animations
 */
function addDynamicStyles() {
    const style = document.createElement('style');
    style.textContent = `
        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-5px); }
            75% { transform: translateX(5px); }
        }

        .form-control.is-valid {
            border-color: #28a745;
            box-shadow: 0 0 0 3px rgba(40, 167, 69, 0.1);
        }

        .form-control.is-invalid {
            border-color: #dc3545;
            box-shadow: 0 0 0 3px rgba(220, 53, 69, 0.1);
        }

        .text-success { color: #28a745 !important; }
        .text-warning { color: #ffc107 !important; }

        .progress-message {
            animation: slideInDown 0.3s ease-out;
        }

        .validation-error-message {
            animation: slideInDown 0.3s ease-out;
        }

        @keyframes slideInDown {
            from {
                opacity: 0;
                transform: translateY(-10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
    `;
    document.head.appendChild(style);
}

// Initialize dynamic styles when script loads
addDynamicStyles();

// Handle page visibility change to reset form if user comes back
document.addEventListener('visibilitychange', function () {
    if (!document.hidden) {
        resetFormState();
    }
});

// Export functions for potential external use
window.LoginWithRecoveryCode = {
    formatRecoveryCode,
    validateRecoveryCode,
    showLoadingState,
    resetFormState
};