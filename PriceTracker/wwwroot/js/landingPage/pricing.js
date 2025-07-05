/**
 * Pricing Page Interactive Features
 * Clean, minimal version focusing on core functionality
 */

// Wait for DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function () {
    // Initialize all features (removed copy buttons and coffee counter)
    initializeCoffeeAmounts();
    initializeScrollAnimations();
    initializeNotifications();
    initializeSmoothScrolling();
    initializeNavigationHighlighting();
});

/**
 * Initialize coffee amount selection functionality
 */
function initializeCoffeeAmounts() {
    const coffeeButtons = document.querySelectorAll('.coffee-amount');

    coffeeButtons.forEach(function (button) {
        button.addEventListener('click', function (e) {
            e.preventDefault();

            // Remove active class from all buttons
            coffeeButtons.forEach(function (btn) {
                btn.classList.remove('active', 'btn-warning');
                btn.classList.add('btn-outline-warning');
            });

            // Add active class to clicked button
            this.classList.remove('btn-outline-warning');
            this.classList.add('btn-warning', 'active');

            const amount = this.getAttribute('data-amount');

            if (amount === 'custom') {
                promptCustomAmount();
            } else {
                showNotification('Thanks for considering a €' + amount + ' coffee! ☕', 'info');
            }

            // Add bounce animation
            const self = this;
            this.style.animation = 'bounce 0.6s ease-in-out';
            setTimeout(function () {
                self.style.animation = '';
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
        showNotification('Wow! Thanks for considering €' + customAmount + '! You are amazing! 🌟', 'success');
    } else if (customAmount !== null) {
        showNotification('Please enter a valid amount 😊', 'warning');
    }
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

    const observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    // Add fade-in animation to feature cards
    const featureCards = document.querySelectorAll('.feature-card');
    featureCards.forEach(function (card, index) {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'opacity 0.6s ease ' + (index * 0.2) + 's, transform 0.6s ease ' + (index * 0.2) + 's';
        observer.observe(card);
    });

    // Add fade-in animation to stat cards
    const statCards = document.querySelectorAll('.stat-card');
    statCards.forEach(function (card, index) {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'opacity 0.6s ease ' + (index * 0.15) + 's, transform 0.6s ease ' + (index * 0.15) + 's';
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
        container.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 9999; max-width: 350px; pointer-events: none;';
        document.body.appendChild(container);
    }
}

/**
 * Show notification message
 */
function showNotification(message, type) {
    if (!type) type = 'info';

    const container = document.getElementById('notification-container');
    const notification = document.createElement('div');

    // Set notification styles based on type
    const typeStyles = {
        success: 'background: linear-gradient(135deg, #48bb78, #38a169); color: white;',
        error: 'background: linear-gradient(135deg, #f56565, #e53e3e); color: white;',
        warning: 'background: linear-gradient(135deg, #ed8936, #dd6b20); color: white;',
        info: 'background: linear-gradient(135deg, #4299e1, #3182ce); color: white;'
    };

    notification.style.cssText = typeStyles[type] + 'padding: 1rem 1.5rem; border-radius: 10px; margin-bottom: 10px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15); transform: translateX(100%); transition: all 0.3s ease; pointer-events: auto; font-weight: 500; backdrop-filter: blur(10px); border: 1px solid rgba(255, 255, 255, 0.2);';

    notification.textContent = message;
    container.appendChild(notification);

    // Animate in
    setTimeout(function () {
        notification.style.transform = 'translateX(0)';
    }, 100);

    // Auto remove after 4 seconds
    setTimeout(function () {
        notification.style.transform = 'translateX(100%)';
        notification.style.opacity = '0';
        setTimeout(function () {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 300);
    }, 4000);

    // Click to dismiss
    notification.addEventListener('click', function () {
        notification.style.transform = 'translateX(100%)';
        notification.style.opacity = '0';
        setTimeout(function () {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 300);
    });
}

/**
 * Initialize smooth scrolling for navigation links
 */
function initializeSmoothScrolling() {
    const quickNavLinks = document.querySelectorAll('.quick-nav-link');

    quickNavLinks.forEach(function (link) {
        link.addEventListener('click', function (e) {
            e.preventDefault();

            const targetId = this.getAttribute('href').substring(1);
            const targetSection = document.getElementById(targetId);

            if (targetSection) {
                // Calculate offset to account for sticky navigation
                const navElement = document.querySelector('.quick-nav');
                const navHeight = navElement ? navElement.offsetHeight + 20 : 20;
                const targetPosition = targetSection.offsetTop - navHeight;

                // Smooth scroll to target
                window.scrollTo({
                    top: targetPosition,
                    behavior: 'smooth'
                });

                // Update active state
                updateActiveNavLink(this);
            }
        });
    });
}

/**
 * Initialize navigation highlighting based on scroll position
 */
function initializeNavigationHighlighting() {
    const sections = document.querySelectorAll('section[id]');
    const navLinks = document.querySelectorAll('.quick-nav-link');

    if (sections.length === 0 || navLinks.length === 0) return;

    // Throttle scroll events for better performance
    let scrollTimeout;

    window.addEventListener('scroll', function () {
        if (scrollTimeout) {
            clearTimeout(scrollTimeout);
        }

        scrollTimeout = setTimeout(function () {
            const scrollPosition = window.scrollY + 150; // Offset for better UX

            let currentSection = '';

            sections.forEach(function (section) {
                const sectionTop = section.offsetTop;
                const sectionHeight = section.offsetHeight;

                if (scrollPosition >= sectionTop && scrollPosition < sectionTop + sectionHeight) {
                    currentSection = section.getAttribute('id');
                }
            });

            // Update active navigation link
            navLinks.forEach(function (link) {
                link.classList.remove('active');
                if (link.getAttribute('href') === '#' + currentSection) {
                    link.classList.add('active');
                }
            });
        }, 10);
    });

    // Set initial active state
    if (navLinks.length > 0) {
        navLinks[0].classList.add('active');
    }
}

/**
 * Update active navigation link
 */
function updateActiveNavLink(activeLink) {
    const allLinks = document.querySelectorAll('.quick-nav-link');

    allLinks.forEach(function (link) {
        link.classList.remove('active');
    });

    activeLink.classList.add('active');
}

/**
 * Add keyboard navigation support
 */
document.addEventListener('keydown', function (e) {
    // ESC key to dismiss notifications
    if (e.key === 'Escape') {
        const notifications = document.querySelectorAll('#notification-container > div');
        notifications.forEach(function (notification) {
            notification.click();
        });
    }
});

/**
 * Add smooth scroll behavior for anchor links
 */
document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
    // Skip quick-nav links as they have their own handler
    if (!anchor.classList.contains('quick-nav-link')) {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                const navElement = document.querySelector('.quick-nav');
                const navHeight = navElement ? navElement.offsetHeight : 0;
                const targetPosition = target.offsetTop - navHeight - 20;

                window.scrollTo({
                    top: targetPosition,
                    behavior: 'smooth'
                });
            }
        });
    }
});

/**
 * Create coffee rain animation for easter egg
 */
function createCoffeeRain() {
    const coffeeEmojis = ['☕', '🍵', '🥤'];

    for (let i = 0; i < 20; i++) {
        setTimeout(function () {
            const coffee = document.createElement('div');
            coffee.textContent = coffeeEmojis[Math.floor(Math.random() * coffeeEmojis.length)];
            coffee.style.cssText = 'position: fixed; top: -50px; left: ' + (Math.random() * window.innerWidth) + 'px; font-size: 2rem; z-index: 10000; pointer-events: none; animation: fall 3s linear forwards;';

            document.body.appendChild(coffee);

            setTimeout(function () {
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
        style.textContent = '@keyframes fall { to { transform: translateY(' + (window.innerHeight + 100) + 'px) rotate(360deg); opacity: 0; } }';
        document.head.appendChild(style);
    }
}

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
            userInput.every(function (key, index) {
                return key === konamiCode[index];
            })) {

            // Easter egg activated!
            showNotification('🎉 Konami Code activated! Extra coffee for you! ☕☕☕', 'success');

            // Add extra coffee rain animation
            createCoffeeRain();

            userInput = [];
        }
    });
})();

// Export functions for potential external use
window.PricingPage = {
    showNotification: showNotification,
    createCoffeeRain: createCoffeeRain
};