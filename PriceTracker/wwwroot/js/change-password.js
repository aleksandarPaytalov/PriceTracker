/**
 * Change Password Page JavaScript
 * Provides password strength checking, validation, and enhanced user experience
 */

document.addEventListener('DOMContentLoaded', function () {
    // Get form elements
    const oldPassword = document.getElementById('Input_OldPassword');
    const newPassword = document.getElementById('Input_NewPassword');
    const confirmPassword = document.getElementById('Input_ConfirmPassword');
    const strengthBar = document.getElementById('strength-bar');
    const strengthText = document.getElementById('strength-text');
    const matchText = document.getElementById('match-text');
    const submitBtn = document.getElementById('submit-btn');

    /**
     * Checks password strength based on multiple criteria
     * @param {string} password - The password to check
     * @returns {Object} - Object containing strength class, text, and color
     */
    function checkPasswordStrength(password) {
        let strength = 0;

        // Check various password criteria
        if (password.length >= 8) strength += 1;
        if (password.length >= 10) strength += 1;
        if (/[a-z]/.test(password)) strength += 1;
        if (/[A-Z]/.test(password)) strength += 1;
        if (/[0-9]/.test(password)) strength += 1;
        if (/[^A-Za-z0-9]/.test(password)) strength += 1;

        // Define strength levels with corresponding styles
        const strengthLevels = [
            { class: '', text: 'Enter a password to see strength', color: '#e2e8f0' },
            { class: 'strength-weak', text: 'Weak password', color: '#ef4444' },
            { class: 'strength-weak', text: 'Weak password', color: '#ef4444' },
            { class: 'strength-fair', text: 'Fair password', color: '#f59e0b' },
            { class: 'strength-good', text: 'Good password', color: '#10b981' },
            { class: 'strength-good', text: 'Good password', color: '#10b981' },
            { class: 'strength-strong', text: 'Strong password', color: '#059669' }
        ];

        return strengthLevels[Math.min(strength, 6)];
    }

    /**
     * Updates the password strength indicator
     */
    function updatePasswordStrength() {
        if (!newPassword || !strengthBar || !strengthText) return;

        const password = newPassword.value;
        const strength = checkPasswordStrength(password);

        // Update strength bar
        strengthBar.className = 'password-strength-bar ' + strength.class;

        // Update strength text
        strengthText.textContent = strength.text;
        strengthText.style.color = strength.color;

        // Check password match after strength update
        checkPasswordMatch();
    }

    /**
     * Checks if new password and confirm password match
     */
    function checkPasswordMatch() {
        if (!newPassword || !confirmPassword || !matchText) return;

        const newPass = newPassword.value;
        const confirmPass = confirmPassword.value;

        // Handle empty confirm password
        if (confirmPass === '') {
            matchText.textContent = 'Re-enter your new password to confirm';
            matchText.style.color = '#718096';
            return;
        }

        // Check if passwords match
        if (newPass === confirmPass) {
            matchText.textContent = '✓ Passwords match';
            matchText.style.color = '#10b981';
        } else {
            matchText.textContent = '✗ Passwords do not match';
            matchText.style.color = '#ef4444';
        }
    }

    /**
     * Validates that the new password is different from the old password
     * @param {string} oldPass - Current password
     * @param {string} newPass - New password
     * @returns {boolean} - True if passwords are different
     */
    function validatePasswordDifference(oldPass, newPass) {
        if (oldPass === newPass) {
            return false;
        }
        return true;
    }

    /**
     * Validates the entire form before submission
     * @returns {boolean} - True if form is valid
     */
    function validateForm() {
        const oldPass = oldPassword ? oldPassword.value : '';
        const newPass = newPassword ? newPassword.value : '';
        const confirmPass = confirmPassword ? confirmPassword.value : '';

        // Check if old password is provided
        if (oldPass.trim() === '') {
            showError('Please enter your current password.');
            return false;
        }

        // Check new password length
        if (newPass.length < 8) {
            showError('New password must be at least 8 characters long.');
            return false;
        }

        // Check if passwords match
        if (newPass !== confirmPass) {
            showError('New passwords do not match. Please check your entries.');
            return false;
        }

        // Check if new password is different from old password
        if (!validatePasswordDifference(oldPass, newPass)) {
            showError('New password must be different from your current password.');
            return false;
        }

        return true;
    }

    /**
     * Shows an error message to the user
     * @param {string} message - Error message to display
     */
    function showError(message) {
        alert(message);
    }

    /**
     * Shows loading state on submit button
     */
    function showLoadingState() {
        if (submitBtn) {
            submitBtn.textContent = 'Updating Password...';
            submitBtn.disabled = true;
            submitBtn.style.opacity = '0.7';
        }
    }

    /**
     * Resets submit button to normal state
     */
    function resetSubmitButton() {
        if (submitBtn) {
            submitBtn.textContent = 'Update Password';
            submitBtn.disabled = false;
            submitBtn.style.opacity = '1';
        }
    }

    // Event Listeners

    // New password input - check strength and match
    if (newPassword) {
        newPassword.addEventListener('input', updatePasswordStrength);

        // Also check for password similarity with old password
        newPassword.addEventListener('blur', function () {
            const oldPass = oldPassword ? oldPassword.value : '';
            const newPass = this.value;

            if (oldPass && newPass && oldPass === newPass) {
                matchText.textContent = '⚠️ New password should be different from current password';
                matchText.style.color = '#f59e0b';
            }
        });
    }

    // Confirm password input - check match
    if (confirmPassword) {
        confirmPassword.addEventListener('input', checkPasswordMatch);
    }

    // Form submission validation
    if (document.getElementById('change-password-form')) {
        document.getElementById('change-password-form').addEventListener('submit', function (e) {
            // Validate form
            if (!validateForm()) {
                e.preventDefault();
                return false;
            }

            // Show loading state
            showLoadingState();

            // Note: If server-side validation fails, the page will reload and reset the button
            // For successful submissions, the user will be redirected
        });
    }

    // Handle form reset (if validation fails and page reloads)
    window.addEventListener('load', function () {
        resetSubmitButton();
    });
});

/**
 * Toggles password visibility for any password input
 * @param {string} inputId - ID of the password input element
 * @param {HTMLElement} button - The toggle button element
 */
function togglePassword(inputId, button) {
    const input = document.getElementById(inputId);

    if (!input || !button) return;

    // Toggle input type
    const isPassword = input.getAttribute('type') === 'password';
    const newType = isPassword ? 'text' : 'password';

    input.setAttribute('type', newType);

    // Update button text/icon
    button.textContent = isPassword ? '🙈' : '👁️';

    // Add visual feedback
    button.style.transform = 'scale(0.95)';
    setTimeout(() => {
        button.style.transform = 'scale(1)';
    }, 100);

    // Maintain focus on input after toggle
    input.focus();
}