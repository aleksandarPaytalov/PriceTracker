document.addEventListener('DOMContentLoaded', function () {
    // Auto-focus on the code input
    const codeInput = document.querySelector('input[name="Input.TwoFactorCode"]');
    if (codeInput) {
        codeInput.focus();

        // Auto-format input (numbers only)
        codeInput.addEventListener('input', function (e) {
            // Only allow numbers
            this.value = this.value.replace(/[^0-9]/g, '');

            // Auto-submit when 6 digits entered (optional)
            if (this.value.length === 6) {
                // Add subtle visual feedback
                this.style.borderColor = '#28a745';
                this.style.backgroundColor = '#f8fff9';
            } else {
                this.style.borderColor = '#667eea';
                this.style.backgroundColor = 'white';
            }
        });

        // Handle paste events
        codeInput.addEventListener('paste', function (e) {
            setTimeout(() => {
                this.value = this.value.replace(/[^0-9]/g, '').substring(0, 6);
            }, 10);
        });
    }

    // Form submission feedback
    document.querySelector('form').addEventListener('submit', function () {
        const submitBtn = this.querySelector('button[type="submit"]');
        submitBtn.innerHTML = '<i class="spinner-border spinner-border-sm me-2"></i>Verifying...';
        submitBtn.disabled = true;
    });
});