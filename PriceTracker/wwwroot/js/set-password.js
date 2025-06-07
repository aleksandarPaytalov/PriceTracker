document.addEventListener('DOMContentLoaded', function () {
    const newPassword = document.getElementById('Input_NewPassword'); // ✅ Променено
    const confirmPassword = document.getElementById('Input_ConfirmPassword'); // ✅ Променено
    const strengthBar = document.getElementById('strength-bar');
    const strengthText = document.getElementById('strength-text');
    const matchText = document.getElementById('match-text');
    const submitBtn = document.getElementById('submit-btn');

    // Останалия код остава същия...
    // Password strength checker
    function checkPasswordStrength(password) {
        let strength = 0;
        let feedback = [];

        if (password.length >= 8) strength += 1;
        if (password.length >= 10) strength += 1;
        if (/[a-z]/.test(password)) strength += 1;
        if (/[A-Z]/.test(password)) strength += 1;
        if (/[0-9]/.test(password)) strength += 1;
        if (/[^A-Za-z0-9]/.test(password)) strength += 1;

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

    // Update password strength
    if (newPassword) {
        newPassword.addEventListener('input', function () {
            const password = this.value;
            const strength = checkPasswordStrength(password);

            strengthBar.className = 'password-strength-bar ' + strength.class;
            strengthText.textContent = strength.text;
            strengthText.style.color = strength.color;

            checkPasswordMatch();
        });
    }

    // Check password match
    function checkPasswordMatch() {
        const newPass = newPassword ? newPassword.value : '';
        const confirmPass = confirmPassword ? confirmPassword.value : '';

        if (confirmPass === '') {
            matchText.textContent = 'Re-enter your password to confirm';
            matchText.style.color = '#718096';
            return;
        }

        if (newPass === confirmPass) {
            matchText.textContent = '✓ Passwords match';
            matchText.style.color = '#10b981';
        } else {
            matchText.textContent = '✗ Passwords do not match';
            matchText.style.color = '#ef4444';
        }
    }

    if (confirmPassword) {
        confirmPassword.addEventListener('input', checkPasswordMatch);
    }

    // Form validation
    document.getElementById('set-password-form').addEventListener('submit', function (e) {
        const newPass = newPassword ? newPassword.value : '';
        const confirmPass = confirmPassword ? confirmPassword.value : '';

        if (newPass !== confirmPass) {
            e.preventDefault();
            alert('Passwords do not match. Please check your entries.');
            return false;
        }

        if (newPass.length < 8) {
            e.preventDefault();
            alert('Password must be at least 8 characters long.');
            return false;
        }

        // Show loading state
        if (submitBtn) {
            submitBtn.textContent = 'Setting Password...';
            submitBtn.disabled = true;
        }
    });
});

// Toggle password visibility
function togglePassword(inputId, button) {
    const input = document.getElementById(inputId);
    if (input) {
        const type = input.getAttribute('type') === 'password' ? 'text' : 'password';
        input.setAttribute('type', type);
        button.textContent = type === 'password' ? '👁️' : '🙈';
    }
}