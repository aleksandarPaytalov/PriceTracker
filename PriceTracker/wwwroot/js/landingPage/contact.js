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
            submitButton: '#submitBtn',
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
        // Check if contact form exists on page
        if (!$(ContactForm.selectors.form).length) {
            return;
        }

        // Verify all form fields exist
        verifyFormFields();

        // Initialize all form features
        initializeCharacterCounter();
        initializeFieldValidation();
        initializeFormSubmission();
    }

    /**
     * Verify all expected form fields exist
     */
    function verifyFormFields() {
        Object.entries(ContactForm.selectors).forEach(([key, selector]) => {
            const $element = $(selector);
            if ($element.length === 0) {
                console.warn(`Contact Form: ${key} not found (${selector})`);
            }
        });
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
                    // Real-time email validation as user types
                    $field.on('input', debounce(function () {
                        const $currentField = $(this);
                        const value = $currentField.val();
                        if (value && value.trim && value.trim().length > 0) {
                            validateField(fieldName, $currentField);
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
    }

    /**
     * Initialize form submission handling
     */
    function initializeFormSubmission() {
        const $form = $(ContactForm.selectors.form);

        $form.on('submit', function (e) {
            e.preventDefault();
            handleFormSubmission();
        });
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
        const value = fieldName === 'robotCheck' ? $field.is(':checked') : $field.val().trim();

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
        // Required check
        if (!value || value === '') {
            return { valid: false, message: 'Please select a subject' };
        }

        // Check for meaningful selection (not empty or placeholder)
        if (value === 'Select a topic...') {
            return { valid: false, message: 'Please select a valid subject' };
        }

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
        // For select elements, don't add visual validation classes due to styling conflicts
        if ($field.is('select')) {
            $field.removeClass('is-invalid');
            $field.siblings('.invalid-feedback').text('');
        } else {
            $field.removeClass('is-invalid').addClass('is-valid');
            $field.siblings('.invalid-feedback').text('');
        }
    }

    /**
     * Show field validation error
     */
    function showFieldError($field, message) {
        $field.removeClass('is-valid').addClass('is-invalid');
        $field.siblings('.invalid-feedback').text(message);
    }

    /**
     * Clear field validation styling
     */
    function clearFieldValidation($field) {
        $field.removeClass('is-valid is-invalid');
        $field.siblings('.invalid-feedback').text('');
    }

    /**
     * Handle form submission with AJAX
     */
    function handleFormSubmission() {
        // Prevent double submission
        if (ContactForm.state.isSubmitting) {
            return;
        }

        // Clear any previous form-level errors
        clearFormError();

        // Validate all fields client-side first
        let isFormValid = true;

        Object.keys(ContactForm.validation).forEach(fieldName => {
            const selector = getFieldSelector(fieldName);
            const $field = $(selector);

            if ($field.length) {
                const fieldValid = validateField(fieldName, $field);
                if (!fieldValid) {
                    isFormValid = false;
                }
            }
        });

        // Additional form-level validation
        if (isFormValid) {
            isFormValid = performAdditionalFormValidation();
        }

        if (!isFormValid) {
            // Focus on first invalid field
            focusFirstInvalidField();

            // Show form-level error
            showFormError('Please fix the errors above and try again.');
            return;
        }

        // Set submitting state
        ContactForm.state.isSubmitting = true;

        // IMPORTANT: Collect form data BEFORE showing loading state (which disables fields)
        const $form = $(ContactForm.selectors.form);
        const formData = $form.serialize();

        // Now show loading state (this will disable fields but we already have the data)
        showLoadingState();

        // Store start time for minimum loading delay
        const startTime = Date.now();
        const minLoadingDuration = 800; // Minimum 800ms loading time for better UX

        // Submit form via AJAX
        $.ajax({
            url: $form.attr('action') || '/Home/Contact',
            type: 'POST',
            data: formData,
            dataType: 'json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            timeout: 30000, // 30 second timeout
            success: function (response) {
                // Calculate remaining time for minimum loading duration
                const elapsedTime = Date.now() - startTime;
                const remainingTime = Math.max(0, minLoadingDuration - elapsedTime);

                // Ensure minimum loading time for better UX
                setTimeout(() => {
                    handleAjaxSuccess(response);
                }, remainingTime);
            },
            error: function (xhr, status, error) {
                console.error('Contact form submission failed:', {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    error: error
                });

                // Calculate remaining time for minimum loading duration
                const elapsedTime = Date.now() - startTime;
                const remainingTime = Math.max(0, minLoadingDuration - elapsedTime);

                // Ensure minimum loading time even for errors
                setTimeout(() => {
                    handleAjaxError(xhr, status, error);
                }, remainingTime);
            }
        });
    }

    /**
     * Show loading state on submit button
     */
    function showLoadingState() {
        const $submitBtn = $(ContactForm.selectors.submitButton);

        if ($submitBtn.length) {
            // Store original button text
            $submitBtn.data('original-text', $submitBtn.html());

            // Show loading state with spinner
            $submitBtn.html('<i class="fas fa-spinner fa-spin me-2"></i>Sending...')
                .prop('disabled', true)
                .addClass('btn-loading');
        }

        // Disable form inputs during submission
        $(ContactForm.selectors.form + ' input, ' + ContactForm.selectors.form + ' select, ' + ContactForm.selectors.form + ' textarea')
            .prop('disabled', true)
            .addClass('submitting');
    }

    /**
     * Hide loading state on submit button
     */
    function hideLoadingState() {
        const $submitBtn = $(ContactForm.selectors.submitButton);

        if ($submitBtn.length) {
            // Restore original button text
            const originalText = $submitBtn.data('original-text') || 'Send Message';
            $submitBtn.html(originalText)
                .prop('disabled', false)
                .removeClass('btn-loading');
        }

        // Re-enable form inputs
        $(ContactForm.selectors.form + ' input, ' + ContactForm.selectors.form + ' select, ' + ContactForm.selectors.form + ' textarea')
            .prop('disabled', false)
            .removeClass('submitting');
    }

    /**
     * Handle successful AJAX response
     */
    function handleAjaxSuccess(response) {
        // Always clean up loading state first
        hideLoadingState();
        ContactForm.state.isSubmitting = false;

        if (response.success) {
            // Show success message
            showFormSuccess(response.message);

            // Clear the form
            clearForm();

            // Reset validation state
            ContactForm.state.validationErrors = {};

            // Clear any validation styling
            clearAllValidationStyling();

            // Scroll to success message
            scrollToMessage('success');
        } else {
            // Handle server validation errors
            handleServerValidationErrors(response.errors, response.message);
        }
    }

    /**
     * Handle AJAX error response
     */
    function handleAjaxError(xhr, status, error) {
        // Always clean up loading state first
        hideLoadingState();
        ContactForm.state.isSubmitting = false;

        let errorMessage = 'Sorry, there was an error sending your message. Please try again or contact us directly.';
        let serverErrors = null;

        // Try to parse JSON error response
        try {
            if (xhr.responseText && xhr.responseText.trim()) {
                const response = JSON.parse(xhr.responseText);

                if (response.message) {
                    errorMessage = response.message;
                }

                // Check for validation errors
                if (response.errors) {
                    serverErrors = response.errors;
                }
            }
        } catch (e) {
            // Log parsing errors but don't show to user
            console.error('Error parsing server response:', e);
        }

        // Handle specific error types
        if (xhr.status === 400) {
            if (serverErrors) {
                // Handle validation errors
                handleServerValidationErrors(serverErrors, errorMessage);
                return;
            } else {
                errorMessage = 'Invalid form data. Please check your inputs and try again.';
            }
        } else if (xhr.status === 403) {
            errorMessage = 'Access denied. Please refresh the page and try again.';
        } else if (xhr.status === 404) {
            errorMessage = 'Contact form service not found. Please try again later.';
        } else if (xhr.status === 500) {
            errorMessage = 'Server error occurred. Please try again later.';
        } else if (status === 'timeout') {
            errorMessage = 'Request timed out. Please check your connection and try again.';
        } else if (status === 'abort') {
            errorMessage = 'Request was cancelled. Please try again.';
        } else if (xhr.status === 0) {
            errorMessage = 'Network error. Please check your internet connection and try again.';
        }

        showFormError(errorMessage);
        scrollToMessage('error');
    }

    /**
     * Handle server-side validation errors
     */
    function handleServerValidationErrors(errors, message) {
        console.log('🔍 Processing server validation errors:', errors);

        // Show each field error
        Object.keys(errors).forEach(fieldName => {
            const fieldErrors = errors[fieldName];
            const selector = getFieldSelector(fieldName.toLowerCase());
            const $field = $(selector);

            if ($field.length && fieldErrors.length > 0) {
                showFieldError($field, fieldErrors[0]); // Show first error
                console.log(`❌ Server error for ${fieldName}: ${fieldErrors[0]}`);
            }
        });

        // Show general form error
        showFormError(message || 'Please fix the errors above and try again.');

        // Focus on first invalid field
        focusFirstInvalidField();

        // Scroll to first error
        scrollToMessage('error');
    }

    /**
     * Clear the form after successful submission
     */
    function clearForm() {
        console.log('🧹 Clearing form fields...');

        // Clear all input fields
        $(ContactForm.selectors.form + ' input[type="text"], ' + ContactForm.selectors.form + ' input[type="email"]').val('');

        // Clear textarea
        $(ContactForm.selectors.messageField).val('');

        // Reset select dropdown
        $(ContactForm.selectors.subjectField).val('');

        // Uncheck checkbox
        $(ContactForm.selectors.robotCheckbox).prop('checked', false);

        // Update character counter
        $(ContactForm.selectors.messageField).trigger('input');

        console.log('✅ Form cleared successfully');
    }

    /**
     * Clear all validation styling from form
     */
    function clearAllValidationStyling() {
        console.log('🎨 Clearing all validation styling...');

        // Remove validation classes from all fields
        $(ContactForm.selectors.form + ' .form-control, ' + ContactForm.selectors.form + ' .form-select')
            .removeClass('is-valid is-invalid');

        // Clear all error messages
        $(ContactForm.selectors.form + ' .invalid-feedback').text('');

        // Clear any server-side validation messages
        $(ContactForm.selectors.form + ' .text-danger').text('');

        console.log('✅ Validation styling cleared');
    }

    /**
     * Scroll to success or error message
     */
    function scrollToMessage(type) {
        const $target = type === 'success' ? $('#form-success-message') : $('#form-error-message');

        if ($target.length && $target.is(':visible')) {
            $('html, body').animate({
                scrollTop: $target.offset().top - 50
            }, 300);
        }
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
                return '';
            }

            const value = $field.val();
            return value ? value.trim() : '';
        } catch (error) {
            console.error(`Error getting value for ${selector}:`, error);
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
         * Clear all validation errors
         */
        clearValidation: function () {
            Object.keys(ContactForm.validation).forEach(fieldName => {
                const selector = getFieldSelector(fieldName);
                const $field = $(selector);
                if ($field.length) {
                    clearFieldValidation($field);
                }
            });
            ContactForm.state.validationErrors = {};
            clearFormError();
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
        }
    };

})(jQuery);