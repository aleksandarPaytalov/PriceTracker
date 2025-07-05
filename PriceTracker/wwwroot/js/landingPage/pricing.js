/**
 * Pricing Page Interactive Features
 * Handles copy-to-clipboard, coffee amount selection, and animations
 */

// Wait for DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function () {
    console.log('🎉 Pricing page loaded - Let\'s make it interactive!');

    // Initialize all interactive features
    initializeCopyButtons();
    initializeCoffeeAmounts();
    initializeCounterAnimations();
    initializeScrollAnimations();
    initializeNotifications();
});

/**
 * Initialize copy-to-clipboard functionality for payment details
 */
function initializeCopyButtons() {
    const copyButtons = document.querySelectorAll('.copy-btn');

    copyButtons.forEach(button => {
        button.addEventListener('click', async function (e) {
            e.preventDefault();

            const textToCopy = this.getAttribute('data-copy');
            const originalIcon = this.querySelector('i');
            const originalClass = originalIcon.className;

            try {
                // Use modern Clipboard API if available
                if (navigator.clipboard && window.isSecureContext) {
                    await navigator.clipboard.writeText(textToCopy);
                } else {
                    // Fallback for older browsers
                    fallbackCopyTextToClipboard(textToCopy);
                }

                // Visual feedback for successful copy
                showCopySuccess(this, originalIcon, originalClass);

                // Show notification
                showNotification('Copied to clipboard! ✅', 'success');

            } catch (err) {
                console.error('Failed to copy text: ', err);
                showNotification('Failed to copy. Please try manually selecting the text.', 'error');
            }
        });
    });
}

/**
 * Fallback copy method for older browsers
 * @param {string} text - Text to copy to clipboard
 */
function fallbackCopyTextToClipboard(text) {
    const textArea = document.createElement('textarea');
    textArea.value = text;
    textArea.style.position = 'fixed';
    textArea.style.left = '-999999px';
    textArea.style.top = '-999999px';
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();

    try {
        const successful = document.execCommand('copy');
        if (!successful) {
            throw new Error('Copy command failed');
        }
    } finally {
        document.body.removeChild(textArea);
    }
}

/**
 * Show visual feedback for successful copy operation
 * @param {HTMLElement} button - The copy button element
 * @param {HTMLElement} icon - The icon element
 * @param {string} originalClass - Original icon class
 */
function showCopySuccess(button, icon, originalClass) {
    // Add copied class for styling
    button.classList.add('copied');

    // Change icon to check mark
    icon.className = 'fas fa-check';

    // Reset after 2 seconds
    setTimeout(() => {
        button.classList.remove('copied');
        icon.className = originalClass;
    }, 2000);
}

/**
 * Initialize coffee amount selection functionality
 */
function initializeCoffeeAmounts() {
    const coffeeButtons = document.querySelectorAll('.coffee-amount');

    coffeeButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();

            // Remove active class from all buttons
            coffeeButtons.forEach(btn => btn.classList.remove('active', 'btn-warning'));
            coffeeButtons.forEach(btn => btn.classList.add('btn-outline-warning'));

            // Add active class to clicked button
            this.classList.remove('btn-outline-warning');
            this.classList.add('btn-warning', 'active');

            const amount = this.getAttribute('data-amount');

            if (amount === 'custom') {
                promptCustomAmount();
            } else {
                showNotification(`Thanks for considering a €${amount} coffee! ☕`, 'info');
            }

            // Add bounce animation
            this.style.animation = 'bounce 0.6s ease-in-out';
            setTimeout(() => {
                this.style.animation = '';
            }, 600);
        });
    });
}

/**
 * Prompt user for custom coffee amount
 */
function promptCustomAmount() {
    const customAmount = prompt('Enter your custom coffee amount (€):');

    if (customAmount && !isNaN(customAmount) && parseFloat(customAmount) > 0) {
        showNotification(`Wow! Thanks for considering €${customAmount}! You\'re amazing! 🌟`, 'success');
    } else if (customAmount !== null) {
        showNotification('Please enter a valid amount 😊', 'warning');
    }
}

/**
 * Initialize counter animations for stats
 */
function initializeCounterAnimations() {
    const coffeeCounter = document.getElementById('coffeeCount');

    if (coffeeCounter) {
        // Animate coffee counter on page load
        animateCounter(coffeeCounter, 0, 42, 2000);

        // Add random coffee animation every 30 seconds
        setInterval(() => {
            const currentCount = parseInt(coffeeCounter.textContent);
            const newCount = currentCount + Math.floor(Math.random() * 3) + 1;
            animateCounter(coffeeCounter, currentCount, newCount, 1000);
        }, 30000);
    }
}

/**
 * Animate a counter from start to end value
 * @param {HTMLElement} element - Counter element
 * @param {number} start - Start value
 * @param {number} end - End value
 * @param {number} duration - Animation duration in ms
 */
function animateCounter(element, start, end, duration) {
    const startTime = Date.now();
    const difference = end - start;

    function updateCounter() {
        const elapsed = Date.now() - startTime;
        const progress = Math.min(elapsed / duration, 1);

        // Use easing function for smooth animation
        const easeOutQuart = 1 - Math.pow(1 - progress, 4);
        const current = Math.floor(start + (difference * easeOutQuart));

        element.textContent = current;

        if (progress < 1) {
            requestAnimationFrame(updateCounter);
        } else {
            element.textContent = end;
        }
    }

    requestAnimationFrame(updateCounter);
}

/**
 * Initialize scroll-triggered animations
 */
function initializeScrollAnimations() {
    // Create intersection observer for fade-in animations
    const observerOptions = {
        threshold: 0.2,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    // Add fade-in animation to feature cards
    const featureCards = document.querySelectorAll('.feature-card');
    featureCards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = `opacity 0.6s ease ${index * 0.2}s, transform 0.6s ease ${index * 0.2}s`;
        observer.observe(card);
    });

    // Add fade-in animation to stat cards
    const statCards = document.querySelectorAll('.stat-card');
    statCards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = `opacity 0.6s ease ${index * 0.15}s, transform 0.6s ease ${index * 0.15}s`;
        observer.observe(card);
    });
}

/**
 * Initialize notification system
 */
function initializeNotifications() {
    // Create notification container if it doesn't exist
    if (!document.getElementById('notification-container')) {
        const container = document.createElement('div');
        container.id = 'notification-container';
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            max-width: 350px;
            pointer-events: none;
        `;
        document.body.appendChild(container);
    }
}

/**
 * Show notification message
 * @param {string} message - Notification message
 * @param {string} type - Notification type (success, error, warning, info)
 */
function showNotification(message, type = 'info') {
    const container = document.getElementById('notification-container');
    const notification = document.createElement('div');

    // Set notification styles based on type
    const typeStyles = {
        success: 'background: linear-gradient(135deg, #48bb78, #38a169); color: white;',
        error: 'background: linear-gradient(135deg, #f56565, #e53e3e); color: white;',
        warning: 'background: linear-gradient(135deg, #ed8936, #dd6b20); color: white;',
        info: 'background: linear-gradient(135deg, #4299e1, #3182ce); color: white;'
    };

    notification.style.cssText = `
        ${typeStyles[type]}
        padding: 1rem 1.5rem;
        border-radius: 10px;
        margin-bottom: 10px;
        box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
        transform: translateX(100%);
        transition: all 0.3s ease;
        pointer-events: auto;
        font-weight: 500;
        backdrop-filter: blur(10px);
        border: 1px solid rgba(255, 255, 255, 0.2);
    `;

    notification.textContent = message;
    container.appendChild(notification);

    // Animate in
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
    }, 100);

    // Auto remove after 4 seconds
    setTimeout(() => {
        notification.style.transform = 'translateX(100%)';
        notification.style.opacity = '0';
        setTimeout(() => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 300);
    }, 4000);

    // Click to dismiss
    notification.addEventListener('click', () => {
        notification.style.transform = 'translateX(100%)';
        notification.style.opacity = '0';
        setTimeout(() => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 300);
    });
}

/**
 * Add keyboard navigation support
 */
document.addEventListener('keydown', function (e) {
    // ESC key to dismiss notifications
    if (e.key === 'Escape') {
        const notifications = document.querySelectorAll('#notification-container > div');
        notifications.forEach(notification => {
            notification.click();
        });
    }

    // Enter key on copy buttons
    if (e.key === 'Enter' && e.target.classList.contains('copy-btn')) {
        e.target.click();
    }
});

/**
 * Add smooth scroll behavior for anchor links
 */
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

/**
 * Easter egg: Konami code for extra coffee animation
 */
(function () {
    const konamiCode = [
        'ArrowUp', 'ArrowUp', 'ArrowDown', 'ArrowDown',
        'ArrowLeft', 'ArrowRight', 'ArrowLeft', 'ArrowRight',
        'KeyB', 'KeyA'
    ];
    let userInput = [];

    document.addEventListener('keydown', function (e) {
        userInput.push(e.code);

        if (userInput.length > konamiCode.length) {
            userInput.shift();
        }

        if (userInput.length === konamiCode.length &&
            userInput.every((key, index) => key === konamiCode[index])) {

            // Easter egg activated!
            showNotification('🎉 Konami Code activated! Extra coffee for you! ☕☕☕', 'success');

            // Add extra coffee rain animation
            createCoffeeRain();

            userInput = [];
        }
    });
})();

/**
 * Create coffee rain animation for easter egg
 */
function createCoffeeRain() {
    const coffeeEmojis = ['☕', '🍵', '🥤'];

    for (let i = 0; i < 20; i++) {
        setTimeout(() => {
            const coffee = document.createElement('div');
            coffee.textContent = coffeeEmojis[Math.floor(Math.random() * coffeeEmojis.length)];
            coffee.style.cssText = `
                position: fixed;
                top: -50px;
                left: ${Math.random() * window.innerWidth}px;
                font-size: 2rem;
                z-index: 10000;
                pointer-events: none;
                animation: fall 3s linear forwards;
            `;

            document.body.appendChild(coffee);

            setTimeout(() => {
                if (coffee.parentNode) {
                    coffee.parentNode.removeChild(coffee);
                }
            }, 3000);
        }, i * 200);
    }

    // Add fall animation if not already defined
    if (!document.getElementById('fall-animation')) {
        const style = document.createElement('style');
        style.id = 'fall-animation';
        style.textContent = `
            @keyframes fall {
                to {
                    transform: translateY(${window.innerHeight + 100}px) rotate(360deg);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
}

// Export functions for potential external use
window.PricingPage = {
    showNotification,
    animateCounter,
    createCoffeeRain
};