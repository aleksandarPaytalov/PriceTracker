/**
 * Contact Form JavaScript Module
 * Handles form validation, submission, and user interactions
 * 
 * Dependencies: jQuery, Bootstrap 5
 */

(function ($) {
    'use strict';

    // Contact form configuration
    const ContactForm = {
        // Form selectors
        selectors: {
            form: '#contactForm',
            nameField: 'input[name="Name"]',
            emailField: 'input[name="Email"]',
            subjectField: 'select[name="Subject"]',  // Fixed: changed from input to select
            messageField: 'textarea[name="Message"]',
            robotCheckbox: '#robotCheck',
            submitButton: '#submitBtn',  // Fixed: changed from #submitButton to #submitBtn
            charCounter: '#charCounter'
        },

        // Validation configuration
        validation: {
            name: {
                required: true,
                minLength: 2,
                maxLength: 100,
                pattern: /^[a-zA-Z\s\-'\.]+$/,
                message: 'Name must be 2-100 characters and contain only letters, spaces, hyphens, apostrophes, and periods'
            },
            email: {
                required: true,
                pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                message: 'Please enter a valid email address'
            },
            subject: {
                required: true,
                message: 'Please select a subject'
            },
            message: {
                required: true,
                minLength: 10,
                maxLength: 2000,
                message: 'Message must be between 10-2000 characters'
            },
            robotCheck: {
                required: true,
                message: 'Please confirm you are not a robot'
            }
        },

        // Form state
        state: {
            isSubmitting: false,
            validationErrors: {}
        }
    };

    /**
     * Initialize the contact form functionality
     */
    function initializeContactForm() {
        console.log('🚀 Contact Form Module: Initializing...');

        // Check if contact form exists on page
        if (!$(ContactForm.selectors.form).length) {
            console.log('📋 Contact Form Module: Form not found, skipping initialization');
            return;
        }

        console.log('✅ Contact Form Module: Form found, setting up functionality');

        // Verify all form fields exist
        verifyFormFields();

        // Clear any existing server-side validation messages on load
        clearAllServerSideValidation();

        // Initialize all form features
        initializeCharacterCounter();
        initializeFieldValidation();
        initializeFormSubmission();

        console.log('🎉 Contact Form Module: Initialization complete');
    }

    /**
     * Clear all server-side validation messages on form initialization
     */
    function clearAllServerSideValidation() {
        console.log('🧹 Clearing any existing server-side validation messages...');

        // Clear all ASP.NET Core validation spans
        $('span[data-valmsg-for]').text('');
        $('span.text-danger.small').text('');
        $('span.field-validation-error').text('');

        // Clear validation summary if present
        $('.validation-summary-errors ul').empty();
        $('.validation-summary-errors').hide();

        console.log('✅ Server-side validation messages cleared');
    }

    /**
     * Verify all expected form fields exist
     */
    function verifyFormFields() {
        console.log('🔍 Verifying form fields...');

        let allFieldsFound = true;

        Object.entries(ContactForm.selectors).forEach(([key, selector]) => {
            const $element = $(selector);
            if ($element.length > 0) {
                console.log(`✅ ${key}: found (${selector})`);
            } else {
                console.warn(`⚠️  ${key}: NOT found (${selector})`);
                allFieldsFound = false;
            }
        });

        if (allFieldsFound) {
            console.log('🎉 All form fields verified successfully');
        } else {
            console.warn('⚠️  Some form fields are missing - this may cause functionality issues');
        }

        return allFieldsFound;
    }

    /**
     * Initialize character counter for message field with enhanced feedback
     */
    function initializeCharacterCounter() {
        const $messageField = $(ContactForm.selectors.messageField);
        const $charCounter = $(ContactForm.selectors.charCounter);

        if ($messageField.length && $charCounter.length) {
            // Update counter on input
            $messageField.on('input', function () {
                const currentLength = $(this).val().length;
                const maxLength = ContactForm.validation.message.maxLength;
                const minLength = ContactForm.validation.message.minLength;

                $charCounter.text(`${currentLength}/${maxLength} characters`);

                // Add visual feedback based on character count
                if (currentLength > maxLength) {
                    $charCounter.removeClass('text-muted text-warning text-success').addClass('text-danger');
                    $charCounter.html(`<i class="fas fa-exclamation-triangle me-1"></i>${currentLength}/${maxLength} characters (too many)`);
                } else if (currentLength > maxLength * 0.9) {
                    $charCounter.removeClass('text-muted text-danger text-success').addClass('text-warning');
                    $charCounter.html(`<i class="fas fa-exclamation me-1"></i>${currentLength}/${maxLength} characters (almost at limit)`);
                } else if (currentLength >= minLength) {
                    $charCounter.removeClass('text-muted text-warning text-danger').addClass('text-success');
                    $charCounter.html(`<i class="fas fa-check me-1"></i>${currentLength}/${maxLength} characters`);
                } else {
                    $charCounter.removeClass('text-warning text-danger text-success').addClass('text-muted');
                    $charCounter.html(`${currentLength}/${maxLength} characters (minimum ${minLength})`);
                }
            });

            // Initialize counter on page load
            $messageField.trigger('input');

            console.log('📝 Enhanced character counter initialized');
        }
    }

    /**
     * Initialize real-time field validation
     */
    function initializeFieldValidation() {
        // Add validation listeners to all form fields
        Object.keys(ContactForm.validation).forEach(fieldName => {
            const selector = getFieldSelector(fieldName);
            const $field = $(selector);

            if ($field.length) {
                // Validate on blur (when user leaves field)
                $field.on('blur', function () {
                    validateField(fieldName, $(this));
                });

                // Clear validation on focus
                $field.on('focus', function () {
                    clearFieldValidation($(this));
                });

                // Real-time validation for specific fields
                if (fieldName === 'email') {
                    // Real-time email validation as user types (with safe value retrieval)
                    $field.on('input', debounce(function () {
                        const value = getFieldValue(ContactForm.selectors.emailField);
                        if (value.length > 0) {
                            console.log(`📧 Real-time email validation for: "${value}"`);
                            validateField(fieldName, $(this));
                        }
                    }, 500));
                }

                if (fieldName === 'message') {
                    // Real-time message length validation
                    $field.on('input', function () {
                        validateField(fieldName, $(this));
                    });
                }

                // Special handling for select dropdown and checkbox
                if (fieldName === 'subject') {
                    $field.on('change', function () {
                        console.log(`📋 Subject changed to: "${$(this).val()}"`);
                        validateField(fieldName, $(this));
                    });

                    // Also validate on focus out for dropdown
                    $field.on('blur', function () {
                        console.log(`📋 Subject blur event with value: "${$(this).val()}"`);
                        validateField(fieldName, $(this));
                    });
                }

                if (fieldName === 'robotCheck') {
                    $field.on('change', function () {
                        validateField(fieldName, $(this));
                    });
                }
            }
        });

        console.log('✅ Field validation listeners initialized');
    }

    /**
     * Initialize form submission handling
     */
    function initializeFormSubmission() {
        const $form = $(ContactForm.selectors.form);

        $form.on('submit', function (e) {
            e.preventDefault();

            // Clear any existing server-side validation messages
            clearAllServerSideValidation();

            handleFormSubmission();
        });

        console.log('📤 Form submission handler initialized');
    }

    /**
     * Get jQuery selector for field name
     */
    function getFieldSelector(fieldName) {
        switch (fieldName) {
            case 'name': return ContactForm.selectors.nameField;
            case 'email': return ContactForm.selectors.emailField;
            case 'subject': return ContactForm.selectors.subjectField;
            case 'message': return ContactForm.selectors.messageField;
            case 'robotCheck': return ContactForm.selectors.robotCheckbox;
            default: return null;
        }
    }

    /**
     * Validate individual field with comprehensive rules
     */
    function validateField(fieldName, $field) {
        const config = ContactForm.validation[fieldName];

        // Safe value retrieval to prevent trim() errors
        let value;
        if (fieldName === 'robotCheck') {
            value = $field.is(':checked');
        } else {
            // Use safe value retrieval instead of direct .val().trim()
            const rawValue = $field.val();
            value = rawValue ? rawValue.trim() : '';
        }

        let isValid = true;
        let errorMessage = '';

        // Specific validation logic for each field type
        switch (fieldName) {
            case 'name':
                isValid = validateNameField(value, config);
                if (!isValid.valid) errorMessage = isValid.message;
                break;

            case 'email':
                isValid = validateEmailField(value, config);
                if (!isValid.valid) errorMessage = isValid.message;
                break;

            case 'subject':
                isValid = validateSubjectField(value, config);
                if (!isValid.valid) errorMessage = isValid.message;
                break;

            case 'message':
                isValid = validateMessageField(value, config);
                if (!isValid.valid) errorMessage = isValid.message;
                break;

            case 'robotCheck':
                isValid = validateRobotCheckbox(value, config);
                if (!isValid.valid) errorMessage = isValid.message;
                break;

            default:
                isValid = { valid: false, message: 'Unknown field' };
                errorMessage = isValid.message;
        }

        // Update field validation state
        if (isValid.valid) {
            showFieldSuccess($field);
            delete ContactForm.state.validationErrors[fieldName];
        } else {
            showFieldError($field, errorMessage);
            ContactForm.state.validationErrors[fieldName] = errorMessage;
        }

        return isValid.valid;
    }

    /**
     * Validate name field
     */
    function validateNameField(value, config) {
        // Required check
        if (!value) {
            return { valid: false, message: 'Name is required' };
        }

        // Length validation
        if (value.length < config.minLength) {
            return { valid: false, message: `Name must be at least ${config.minLength} characters` };
        }

        if (value.length > config.maxLength) {
            return { valid: false, message: `Name must not exceed ${config.maxLength} characters` };
        }

        // Pattern validation (letters, spaces, hyphens, apostrophes, periods)
        if (!config.pattern.test(value)) {
            return { valid: false, message: 'Name can only contain letters, spaces, hyphens, apostrophes, and periods' };
        }

        return { valid: true };
    }

    /**
     * Validate email field with comprehensive email format checking
     */
    function validateEmailField(value, config) {
        // Required check
        if (!value) {
            return { valid: false, message: 'Email address is required' };
        }

        // Basic format validation
        if (!config.pattern.test(value)) {
            return { valid: false, message: 'Please enter a valid email address' };
        }

        // Additional email format checks
        if (value.length > 254) {
            return { valid: false, message: 'Email address is too long' };
        }

        // Check for common email issues
        if (value.includes('..') || value.startsWith('.') || value.endsWith('.')) {
            return { valid: false, message: 'Email address format is invalid' };
        }

        // Split email into local and domain parts
        const parts = value.split('@');
        if (parts.length !== 2) {
            return { valid: false, message: 'Email address must contain exactly one @ symbol' };
        }

        const [localPart, domainPart] = parts;

        // Validate local part (before @)
        if (localPart.length === 0 || localPart.length > 64) {
            return { valid: false, message: 'Email address format is invalid' };
        }

        // Validate domain part (after @)
        if (domainPart.length === 0 || domainPart.length > 255) {
            return { valid: false, message: 'Email domain is invalid' };
        }

        // Domain should have at least one dot
        if (!domainPart.includes('.')) {
            return { valid: false, message: 'Email domain must contain a valid domain extension' };
        }

        return { valid: true };
    }

    /**
     * Validate subject field (dropdown selection)
     */
    function validateSubjectField(value, config) {
        console.log(`🔍 Validating subject field with value: "${value}"`);

        // Required check - empty value means "Select a topic..." is selected
        if (!value || value === '' || value === 'Select a topic...') {
            console.log('❌ Subject validation failed: no valid selection');
            return { valid: false, message: 'Please select a subject' };
        }

        console.log('✅ Subject validation passed');
        return { valid: true };
    }

    /**
     * Validate message field with character count and content validation
     */
    function validateMessageField(value, config) {
        // Required check
        if (!value) {
            return { valid: false, message: 'Message is required' };
        }

        // Length validation
        if (value.length < config.minLength) {
            return { valid: false, message: `Message must be at least ${config.minLength} characters` };
        }

        if (value.length > config.maxLength) {
            return { valid: false, message: `Message must not exceed ${config.maxLength} characters` };
        }

        // Check for meaningful content (not just spaces or repeated characters)
        const meaningfulContent = value.replace(/\s+/g, ' ').trim();
        if (meaningfulContent.length < config.minLength) {
            return { valid: false, message: 'Please enter a meaningful message' };
        }

        // Check for excessive repetition of characters
        const repeatedChars = /(.)\1{10,}/;
        if (repeatedChars.test(value)) {
            return { valid: false, message: 'Message contains too many repeated characters' };
        }

        return { valid: true };
    }

    /**
     * Validate anti-spam checkbox
     */
    function validateRobotCheckbox(isChecked, config) {
        if (!isChecked) {
            return { valid: false, message: 'Please confirm you are not a robot' };
        }

        return { valid: true };
    }

    /**
     * Show field validation success
     */
    function showFieldSuccess($field) {
        // Clear server-side validation messages to prevent duplicates
        clearServerSideValidation($field);

        // For select elements, don't add visual validation classes due to styling conflicts
        if ($field.is('select')) {
            $field.removeClass('is-invalid');
            $field.siblings('.invalid-feedback').text('');
            console.log('✅ Select field valid (visual styling skipped)');
        } else {
            $field.removeClass('is-invalid').addClass('is-valid');
            $field.siblings('.invalid-feedback').text('');
        }
    }

    /**
     * Show field validation error
     */
    function showFieldError($field, message) {
        // Clear server-side validation messages to prevent duplicates
        clearServerSideValidation($field);

        // For select elements, don't add visual validation classes due to styling conflicts
        if ($field.is('select')) {
            $field.removeClass('is-valid');
            $field.siblings('.invalid-feedback').text(message);
            console.log(`❌ Select field invalid: ${message} (visual styling skipped)`);
        } else {
            $field.removeClass('is-valid').addClass('is-invalid');
            $field.siblings('.invalid-feedback').text(message);
        }
    }

    /**
     * Clear field validation styling
     */
    function clearFieldValidation($field) {
        // Clear both client-side and server-side validation
        clearServerSideValidation($field);

        // For select elements, only remove validation classes (no visual styling issues)
        if ($field.is('select')) {
            $field.removeClass('is-valid is-invalid');
            $field.siblings('.invalid-feedback').text('');
            console.log('🧹 Select field validation cleared (no styling conflicts)');
        } else {
            $field.removeClass('is-valid is-invalid');
            $field.siblings('.invalid-feedback').text('');
        }
    }

    /**
     * Clear server-side validation messages to prevent duplicates
     */
    function clearServerSideValidation($field) {
        // Find and clear ASP.NET Core validation span
        const fieldName = $field.attr('name');
        if (fieldName) {
            // Clear validation summary for this field
            $(`span[data-valmsg-for="${fieldName}"]`).text('');

            // Also try alternative selector patterns
            $field.siblings('span.text-danger').text('');
            $field.siblings('span.field-validation-error').text('');
            $field.siblings('.text-danger.small').text('');
        }
    }

    /**
     * Handle form submission with complete validation
     */
    function handleFormSubmission() {
        console.log('📝 Form submission started');

        // Prevent double submission
        if (ContactForm.state.isSubmitting) {
            console.log('⚠️  Form already submitting, ignoring');
            return;
        }

        // Clear any previous form-level errors
        clearFormError();

        // Validate all fields
        let isFormValid = true;
        const validationResults = {};

        console.log('🔍 Starting field validation...');

        Object.keys(ContactForm.validation).forEach(fieldName => {
            const selector = getFieldSelector(fieldName);
            const $field = $(selector);

            if ($field.length) {
                console.log(`🔸 Validating ${fieldName}...`);
                const fieldValid = validateField(fieldName, $field);
                validationResults[fieldName] = fieldValid;
                console.log(`${fieldValid ? '✅' : '❌'} ${fieldName}: ${fieldValid ? 'valid' : 'invalid'}`);

                if (!fieldValid) {
                    isFormValid = false;
                }
            } else {
                console.warn(`⚠️  Field ${fieldName} not found with selector: ${selector}`);
            }
        });

        // Additional form-level validation
        if (isFormValid) {
            console.log('🔍 Running additional form validation...');
            isFormValid = performAdditionalFormValidation();
        }

        if (!isFormValid) {
            console.log('❌ Form validation failed');
            console.log('Validation errors:', ContactForm.state.validationErrors);

            // Focus on first invalid field
            focusFirstInvalidField();

            // Show form-level error
            showFormError('Please fix the errors above and try again.');
            return;
        }

        console.log('✅ Form validation passed, preparing submission');

        // All validation passed - ready for AJAX submission
        console.log('🎉 Form ready for AJAX submission (to be implemented in next sub-step)');

        // For now, just show success message to indicate validation worked
        showFormSuccess('✅ Form validation successful! (AJAX submission will be implemented next)');
    }

    /**
     * Perform additional form-level validation checks
     */
    function performAdditionalFormValidation() {
        let isValid = true;

        try {
            // Safely get field values with null checking
            const email = getFieldValue(ContactForm.selectors.emailField);
            const message = getFieldValue(ContactForm.selectors.messageField);
            const subject = getFieldValue(ContactForm.selectors.subjectField);

            console.log('📋 Form values for additional validation:', { email, message: message.substring(0, 50) + '...', subject });

            // Basic spam detection (only if fields have values)
            if (message && (isSpamContent(message) || isSpamContent(subject))) {
                ContactForm.state.validationErrors['spam'] = 'Content appears to be spam';
                isValid = false;
                console.log('🚨 Spam content detected');
            }

            // Check for obviously fake emails (only if email has value)
            if (email && isFakeEmail(email)) {
                ContactForm.state.validationErrors['email'] = 'Please use a valid email address';
                showFieldError($(ContactForm.selectors.emailField), 'Please use a valid email address');
                isValid = false;
                console.log('🚨 Fake email detected:', email);
            }

        } catch (error) {
            console.error('❌ Error in additional form validation:', error);
            // Don't fail the form submission due to additional validation errors
            console.log('⚠️  Continuing with basic validation only');
        }

        return isValid;
    }

    /**
     * Safely get field value with null checking
     */
    function getFieldValue(selector) {
        try {
            const $field = $(selector);
            if ($field.length === 0) {
                console.warn(`⚠️  Field not found: ${selector}`);
                return '';
            }

            const value = $field.val();
            return value ? value.trim() : '';
        } catch (error) {
            console.error(`❌ Error getting value for ${selector}:`, error);
            return '';
        }
    }

    /**
     * Basic spam content detection with null safety
     */
    function isSpamContent(content) {
        // Return false if content is empty or undefined
        if (!content || typeof content !== 'string') {
            return false;
        }

        const spamKeywords = [
            'viagra', 'casino', 'lottery', 'winner', 'congratulations',
            'click here', 'make money', 'work from home', 'guarantee',
            'free money', 'act now', 'limited time'
        ];

        const lowercaseContent = content.toLowerCase();

        // Check for excessive spam keywords
        let spamCount = 0;
        spamKeywords.forEach(keyword => {
            if (lowercaseContent.includes(keyword)) {
                spamCount++;
            }
        });

        // If more than 2 spam keywords, likely spam
        if (spamCount > 2) return true;

        // Check for excessive links
        const linkCount = (content.match(/http/gi) || []).length;
        if (linkCount > 3) return true;

        // Check for excessive repetition
        const words = content.split(/\s+/);
        if (words.length > 10) {
            const uniqueWords = new Set(words.map(w => w.toLowerCase()));
            if (uniqueWords.size / words.length < 0.3) return true;
        }

        return false;
    }

    /**
     * Check for obviously fake email addresses with null safety
     */
    function isFakeEmail(email) {
        // Return false if email is empty or undefined
        if (!email || typeof email !== 'string') {
            return false;
        }

        const fakePatterns = [
            /^test@/i,
            /^fake@/i,
            /^noreply@/i,
            /@test\./i,
            /@fake\./i,
            /@example\./i,
            /@localhost/i,
            /^.{1,2}@/,  // Very short local part
            /@.{1,2}\./  // Very short domain
        ];

        return fakePatterns.some(pattern => pattern.test(email));
    }

    /**
     * Focus on the first invalid field
     */
    function focusFirstInvalidField() {
        const $firstInvalid = $('.is-invalid').first();
        if ($firstInvalid.length) {
            $firstInvalid.focus();

            // Scroll to field if needed
            $('html, body').animate({
                scrollTop: $firstInvalid.offset().top - 100
            }, 300);
        }
    }

    /**
     * Show form-level error message
     */
    function showFormError(message) {
        let $errorContainer = $('#form-error-message');

        // Create error container if it doesn't exist
        if (!$errorContainer.length) {
            $errorContainer = $('<div id="form-error-message" class="alert alert-danger mt-3" role="alert"></div>');
            $(ContactForm.selectors.form).prepend($errorContainer);
        }

        $errorContainer.html(`<i class="fas fa-exclamation-triangle me-2"></i>${message}`).show();

        // Scroll to error message
        $('html, body').animate({
            scrollTop: $errorContainer.offset().top - 50
        }, 300);
    }

    /**
     * Show form-level success message
     */
    function showFormSuccess(message) {
        let $successContainer = $('#form-success-message');

        // Create success container if it doesn't exist
        if (!$successContainer.length) {
            $successContainer = $('<div id="form-success-message" class="alert alert-success mt-3" role="alert"></div>');
            $(ContactForm.selectors.form).prepend($successContainer);
        }

        $successContainer.html(`<i class="fas fa-check-circle me-2"></i>${message}`).show();

        // Hide any error messages
        $('#form-error-message').hide();
    }

    /**
     * Clear form-level error messages
     */
    function clearFormError() {
        $('#form-error-message').hide();
        $('#form-success-message').hide();
    }

    /**
     * Debounce function to limit API calls
     */
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    /**
     * Utility function to capitalize first letter
     */
    function capitalizeFirst(str) {
        return str.charAt(0).toUpperCase() + str.slice(1);
    }

    // Initialize when document is ready
    $(document).ready(function () {
        initializeContactForm();
    });

    // Expose public API if needed
    window.ContactForm = {
        /**
         * Validate entire form and return results
         */
        validate: function () {
            const results = {
                isValid: true,
                errors: {},
                fieldResults: {}
            };

            Object.keys(ContactForm.validation).forEach(fieldName => {
                const selector = getFieldSelector(fieldName);
                const $field = $(selector);
                if ($field.length) {
                    const fieldValid = validateField(fieldName, $field);
                    results.fieldResults[fieldName] = fieldValid;
                    if (!fieldValid) {
                        results.isValid = false;
                        results.errors[fieldName] = ContactForm.state.validationErrors[fieldName];
                    }
                }
            });

            return results;
        },

        /**
         * Get current validation state
         */
        getValidationState: function () {
            return {
                errors: { ...ContactForm.state.validationErrors },
                isSubmitting: ContactForm.state.isSubmitting
            };
        },

        /**
         * Clear all validation errors (both client and server-side)
         */
        clearValidation: function () {
            // Clear client-side validation
            Object.keys(ContactForm.validation).forEach(fieldName => {
                const selector = getFieldSelector(fieldName);
                const $field = $(selector);
                if ($field.length) {
                    clearFieldValidation($field);
                }
            });
            ContactForm.state.validationErrors = {};
            clearFormError();

            // Clear server-side validation
            clearAllServerSideValidation();

            console.log('🧹 All validation cleared (client and server-side)');
        },

        /**
         * Manually trigger validation for specific field
         */
        validateField: function (fieldName) {
            const selector = getFieldSelector(fieldName);
            const $field = $(selector);
            if ($field.length) {
                return validateField(fieldName, $field);
            }
            return false;
        },

        /**
         * Clear only server-side validation messages
         */
        clearServerSideValidation: function () {
            clearAllServerSideValidation();
        }
    };

})(jQuery);