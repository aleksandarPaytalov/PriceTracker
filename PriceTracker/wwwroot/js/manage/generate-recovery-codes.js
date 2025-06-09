/**
 * Generate Recovery Codes - Simple Interactive Features
 * Basic functionality for generating 2FA recovery codes
 */

document.addEventListener('DOMContentLoaded', function () {
    const generateForm = document.querySelector('.generation-form');
    const generateButton = document.querySelector('.generate-btn');

    // Form submission confirmation
    if (generateForm) {
        generateForm.addEventListener('submit', function (e) {
            if (!confirmCodeGeneration()) {
                e.preventDefault();
                return false;
            }

            // Show loading state
            showLoadingState();
        });
    }

    // Simple button hover effects
    if (generateButton) {
        generateButton.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-2px)';
        });

        generateButton.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0)';
        });
    }

    /**
     * Confirm recovery code generation with user
     * @returns {boolean} - Whether the user confirmed
     */
    function confirmCodeGeneration() {
        const message = `Generate New Recovery Codes?

This will invalidate your current recovery codes and create new ones.

Your authenticator app will continue to work normally.

Continue?`;

        return confirm(message);
    }

    /**
     * Show loading state on the button
     */
    function showLoadingState() {
        if (generateButton) {
            generateButton.disabled = true;
            generateButton.classList.add('btn-loading');
            generateButton.textContent = 'Generating...';
        }
    }

    /**
     * Reset button to normal state (in case of errors)
     */
    function resetButtonState() {
        if (generateButton) {
            generateButton.disabled = false;
            generateButton.classList.remove('btn-loading');
            generateButton.textContent = 'Generate Recovery Codes';
        }
    }

    // Reset button state on page load (in case of navigation back)
    window.addEventListener('load', resetButtonState);

    // Add basic visual feedback to links
    const backLink = document.querySelector('.back-link');
    if (backLink) {
        backLink.addEventListener('mouseenter', function () {
            this.style.transform = 'translateX(-2px)';
        });

        backLink.addEventListener('mouseleave', function () {
            this.style.transform = 'translateX(0)';
        });
    }
});