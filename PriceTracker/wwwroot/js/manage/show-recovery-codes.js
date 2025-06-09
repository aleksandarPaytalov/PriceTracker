// Copy to clipboard functionality
document.addEventListener('DOMContentLoaded', function () {
    const recoveryCodes = document.querySelectorAll('.recovery-code');

    recoveryCodes.forEach(function (codeElement) {
        codeElement.addEventListener('click', function () {
            const code = this.getAttribute('data-code');

            // Modern clipboard API
            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText(code).then(function () {
                    showCopyFeedback(codeElement);
                }).catch(function (err) {
                    console.error('Failed to copy: ', err);
                    fallbackCopyTextToClipboard(code, codeElement);
                });
            } else {
                // Fallback for older browsers
                fallbackCopyTextToClipboard(code, codeElement);
            }
        });
    });

    function fallbackCopyTextToClipboard(text, element) {
        const textArea = document.createElement('textarea');
        textArea.value = text;

        // Avoid scrolling to bottom
        textArea.style.top = '0';
        textArea.style.left = '0';
        textArea.style.position = 'fixed';

        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        try {
            const successful = document.execCommand('copy');
            if (successful) {
                showCopyFeedback(element);
            }
        } catch (err) {
            console.error('Fallback: Oops, unable to copy', err);
        }

        document.body.removeChild(textArea);
    }

    function showCopyFeedback(element) {
        element.classList.add('copied');
        element.textContent = 'Copied!';

        setTimeout(function () {
            element.classList.remove('copied');
            element.textContent = element.getAttribute('data-code');
        }, 1000);
    }
});