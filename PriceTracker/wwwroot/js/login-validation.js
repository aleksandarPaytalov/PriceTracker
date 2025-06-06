/**
 * Login and Registration Form Validation
 * Enhanced client-side validation with visual feedback
 */

(function ($) {
    'use strict';

    // Form validation configuration
    const ValidationConfig = {
        email: {
            required: true,
            pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
            message: 'Please enter a valid email address'
        },
        password: {
            required: true,
            minLength: 8,
            pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/,
            message: 'Password must meet all requirements'
        }
    };

    /**
     * Input focus effects and validation
     */
    function initializeInputEffects() {
        // Add focus effects for inputs
        $('.input100').each(function () {
            const $input = $(this);

            // Add focus/blur effects
            $input.on('focus', function () {
                hideValidate($input);
            });

            $input.on('blur', function () {
                if ($input.val().trim() !== '') {
                    $input.addClass('has-val');
                } else {
                    $input.removeClass('has-val');
                }

                // Validate on blur
                validateInput($input);
            });

            // Real-time validation for password confirmation
            if ($input.attr('name') === 'Input.ConfirmPassword') {
                $input.on('input', function () {
                    validatePasswordConfirmation();
                });
            }

            // Real-time password strength indicator
            if ($input.attr('name') === 'Input.Password') {
                $input.on('input', function () {
                    updatePasswordStrength($input.val());
                });
            }
        });

        // Initialize input states
        $('.input100').each(function () {
            if ($(this).val().trim() !== '') {
                $(this).addClass('has-val');
            }
        });
    }

    /**
     * Validate individual input
     */
    function validateInput($input) {
        const inputName = $input.attr('name');
        const value = $input.val().trim();
        let isValid = true;
        let message = '';

        // Email validation
        if (inputName === 'Input.Email') {
            if (!value) {
                isValid = false;
                message = 'Email is required';
            } else if (!ValidationConfig.email.pattern.test(value)) {
                isValid = false;
                message = ValidationConfig.email.message;
            }
        }

        // Password validation
        if (inputName === 'Input.Password') {
            if (!value) {
                isValid = false;
                message = 'Password is required';
            } else if (value.length < ValidationConfig.password.minLength) {
                isValid = false;
                message = `Password must be at least ${ValidationConfig.password.minLength} characters long`;
            } else if (!ValidationConfig.password.pattern.test(value)) {
                isValid = false;
                message = ValidationConfig.password.message;
            }
        }

        // Confirm password validation
        if (inputName === 'Input.ConfirmPassword') {
            const password = $('input[name="Input.Password"]').val();
            if (!value) {
                isValid = false;
                message = 'Please confirm your password';
            } else if (value !== password) {
                isValid = false;
                message = 'Passwords do not match';
            }
        }

        if (!isValid) {
            showValidate($input, message);
        } else {
            hideValidate($input);
        }

        return isValid;
    }

    /**
     * Show validation error
     */
    function showValidate($input, message) {
        const $thisAlert = $input.parent();

        $thisAlert.addClass('alert-validate');
        $input.addClass('input-validation-error');

        // Update or create error message
        let $errorSpan = $thisAlert.find('.text-danger');
        if ($errorSpan.length === 0) {
            $errorSpan = $('<span class="text-danger"></span>');
            $thisAlert.append($errorSpan);
        }
        $errorSpan.text(message);
    }

    /**
     * Hide validation error
     */
    function hideValidate($input) {
        const $thisAlert = $input.parent();

        $thisAlert.removeClass('alert-validate');
        $input.removeClass('input-validation-error');
        $thisAlert.find('.text-danger').text('');
    }

    /**
     * Password confirmation validation
     */
    function validatePasswordConfirmation() {
        const $confirmPassword = $('input[name="Input.ConfirmPassword"]');
        const $password = $('input[name="Input.Password"]');

        if ($confirmPassword.length && $password.length) {
            const password = $password.val();
            const confirmPassword = $confirmPassword.val();

            if (confirmPassword && password !== confirmPassword) {
                showValidate($confirmPassword, 'Passwords do not match');
                return false;
            } else if (confirmPassword) {
                hideValidate($confirmPassword);
                return true;
            }
        }
        return true;
    }

    /**
     * Password strength indicator
     */
    function updatePasswordStrength(password) {
        const $strengthIndicator = $('.password-strength-indicator');

        if ($strengthIndicator.length === 0) {
            // Create strength indicator if it doesn't exist
            const strengthHtml = `
                <div class="password-strength-indicator">
                    <div class="strength-bar">
                        <div class="strength-fill"></div>
                    </div>
                    <span class="strength-text">Password strength: <span class="strength-level">Weak</span></span>
                </div>
            `;
            $('.password-requirements').before(strengthHtml);
        }

        const strength = calculatePasswordStrength(password);
        updateStrengthDisplay(strength);
    }

    /**
     * Calculate password strength
     */
    function calculatePasswordStrength(password) {
        let score = 0;
        let level = 'Very Weak';
        let color = '#dc3545';

        if (password.length >= 8) score++;
        if (/[a-z]/.test(password)) score++;
        if (/[A-Z]/.test(password)) score++;
        if (/\d/.test(password)) score++;
        if (/[@$!%*?&]/.test(password)) score++;

        switch (score) {
            case 0:
            case 1:
                level = 'Very Weak';
                color = '#dc3545';
                break;
            case 2:
                level = 'Weak';
                color = '#fd7e14';
                break;
            case 3:
                level = 'Fair';
                color = '#ffc107';
                break;
            case 4:
                level = 'Good';
                color = '#20c997';
                break;
            case 5:
                level = 'Strong';
                color = '#28a745';
                break;
        }

        return { score, level, color, percentage: (score / 5) * 100 };
    }

    /**
     * Update strength display
     */
    function updateStrengthDisplay(strength) {
        const $strengthFill = $('.strength-fill');
        const $strengthLevel = $('.strength-level');

        $strengthFill.css({
            'width': strength.percentage + '%',
            'background-color': strength.color
        });

        $strengthLevel.text(strength.level).css('color', strength.color);
    }

    /**
     * Form submission validation
     */
    function validateForm($form) {
        let isValid = true;

        $form.find('.input100').each(function () {
            if (!validateInput($(this))) {
                isValid = false;
            }
        });

        // Additional password confirmation check for registration
        if ($form.find('input[name="Input.ConfirmPassword"]').length) {
            if (!validatePasswordConfirmation()) {
                isValid = false;
            }
        }

        return isValid;
    }

    /**
     * Initialize form submission handling
     */
    function initializeFormSubmission() {
        $('.login100-form').on('submit', function (e) {
            const $form = $(this);

            if (!validateForm($form)) {
                e.preventDefault();

                // Focus on first invalid input
                const $firstError = $form.find('.input-validation-error').first();
                if ($firstError.length) {
                    $firstError.focus();
                }

                // Show general error message
                showFormError('Please fix the errors above and try again.');
                return false;
            }

            // Show loading state
            const $submitBtn = $form.find('.login100-form-btn');
            $submitBtn.prop('disabled', true).text('Please wait...');
        });
    }

    /**
     * Show form-level error message
     */
    function showFormError(message) {
        let $errorContainer = $('.form-error-message');

        if ($errorContainer.length === 0) {
            $errorContainer = $(`
                <div class="form-error-message alert alert-danger" role="alert">
                    <strong>Error:</strong> <span class="error-text"></span>
                </div>
            `);
            $('.login100-form-title').after($errorContainer);
        }

        $errorContainer.find('.error-text').text(message);
        $errorContainer.show();

        // Auto-hide after 5 seconds
        setTimeout(() => {
            $errorContainer.fadeOut();
        }, 5000);
    }

    /**
     * Initialize accessibility improvements
     */
    function initializeAccessibility() {
        // Add ARIA labels and descriptions
        $('.input100').each(function () {
            const $input = $(this);
            const $label = $input.siblings('.label-input100');

            if ($label.length) {
                const labelText = $label.text();
                $input.attr('aria-label', labelText);
            }
        });

        // Add keyboard navigation support
        $('.login100-form-btn').on('keydown', function (e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                $(this).click();
            }
        });
    }

    /**
     * Initialize all functionality when document is ready
     */
    $(document).ready(function () {
        initializeInputEffects();
        initializeFormSubmission();
        initializeAccessibility();

        console.log('Login/Register form validation initialized successfully');
    });

})(jQuery);

// Additional CSS for password strength indicator and form errors
const additionalCSS = `
<style>
.password-strength-indicator {
    margin: 10px 0;
}

.strength-bar {
    width: 100%;
    height: 4px;
    background-color: #e9ecef;
    border-radius: 2px;
    overflow: hidden;
}

.strength-fill {
    height: 100%;
    width: 0%;
    transition: all 0.3s ease;
    border-radius: 2px;
}

.strength-text {
    font-size: 12px;
    color: #6c757d;
    margin-top: 5px;
    display: block;
}

.password-requirements {
    margin: 15px 0;
}

.requirements-list {
    font-size: 11px;
    margin: 5px 0 0 15px;
    color: #6c757d;
}

.requirements-list li {
    margin-bottom: 2px;
}

.form-error-message {
    margin-bottom: 20px;
    border-radius: 6px;
}

.alert-validate {
    position: relative;
}

.alert-validate::before {
    content: "⚠";
    position: absolute;
    right: 8px;
    top: 50%;
    transform: translateY(-50%);
    color: #dc3545;
    font-size: 16px;
}

@media (max-width: 576px) {
    .password-requirements {
        margin: 10px 0;
    }
    
    .requirements-list {
        font-size: 10px;
    }
}
</style>
`;

// Inject additional CSS
if (typeof document !== 'undefined') {
    document.head.insertAdjacentHTML('beforeend', additionalCSS);
}