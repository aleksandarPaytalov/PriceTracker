/**
 * Format recovery code input - keep the dash
 */
function formatRecoveryCode(input) {
    let value = input.value;

    value = value.replace(/[^a-zA-Z0-9-]/g, '').toUpperCase();

    // Limit to reasonable length
    value = value.substring(0, 16);

    input.value = value;
}

/**
 * Validate recovery code format - expect a format with dash
 */
function validateRecoveryCode(input) {
    const value = input.value.trim();
    const isValidFormat = /^[A-Z0-9]{5}-[A-Z0-9]{5}$/.test(value) || value.length === 0;

    // Update input styling based on validation
    if (value.length === 0) {
        // Empty state
        input.classList.remove('is-valid', 'is-invalid');
        updateInputFeedback(input, '', 'neutral');
    } else if (isValidFormat) {
        // Valid state
        input.classList.add('is-valid');
        input.classList.remove('is-invalid');
        updateInputFeedback(input, '✓ Recovery code format looks correct', 'success');
    } else {
        // Invalid state
        input.classList.add('is-invalid');
        input.classList.remove('is-valid');
        updateInputFeedback(input, '⚠ Expected format: XXXXX-XXXXX (5 chars, dash, 5 chars)', 'warning');
    }

    return isValidFormat || value.length === 0;
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

        const code = recoveryInput.value.trim();

        // Validate format
        if (!/^[A-Z0-9]{5}-[A-Z0-9]{5}$/.test(code)) {
            e.preventDefault();
            showValidationError('Please enter the recovery code in the correct format: XXXXX-XXXXX (including the dash)');
            recoveryInput.focus();
            return false;
        }

        return true;
    });
}