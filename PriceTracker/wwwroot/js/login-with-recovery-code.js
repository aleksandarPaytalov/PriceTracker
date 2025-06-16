// Обнови JavaScript-a в login-with-recovery-code.js:

/**
 * Format recovery code input - ЗАПАЗВА тирето!
 */
function formatRecoveryCode(input) {
    let value = input.value;

    // Премахни само spaces и нон-алфанумерични символи, НО ЗАПАЗИ тирето
    value = value.replace(/[^a-zA-Z0-9-]/g, '').toUpperCase();

    // Limit to reasonable length
    value = value.substring(0, 16);

    // НЕ добавяй автоматично тире - остави user-a да въведе както иска
    input.value = value;
}

/**
 * Validate recovery code format - очаква формат с тире
 */
function validateRecoveryCode(input) {
    const value = input.value.trim();
    // Очакваме формат като XXXXX-XXXXX (5-5 символа с тире в средата)
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
 * Initialize form validation - НЕ почиствай кода
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

        // НЕ променяй кода - изпрати го точно както е въведен
        return true;
    });
}