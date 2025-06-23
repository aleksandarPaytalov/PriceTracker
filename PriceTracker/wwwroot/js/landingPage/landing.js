const CONFIG = {
    SCROLL_THRESHOLD: 50,
    ANIMATION_DELAY: 100
};

let navbar, navToggle, navMenu, navLinks, featureCards;

let isMenuOpen = false;
let isScrolled = false;

function smoothScrollTo(targetId) {
    const target = document.getElementById(targetId);
    if (target) {
        const offsetTop = target.offsetTop - navbar.offsetHeight;
        window.scrollTo({
            top: offsetTop,
            behavior: 'smooth'
        });
    }
}

function isInViewport(element) {
    const rect = element.getBoundingClientRect();
    return rect.top < window.innerHeight && rect.bottom > 0;
}

function toggleMobileMenu() {
    isMenuOpen = !isMenuOpen;
    navMenu.classList.toggle('active', isMenuOpen);
    navToggle.classList.toggle('active', isMenuOpen);

    document.body.style.overflow = isMenuOpen ? 'hidden' : '';
}

function handleNavClick(event) {
    const href = event.target.getAttribute('href');

    if (href && href.startsWith('#')) {
        event.preventDefault();
        const targetId = href.substring(1);
        smoothScrollTo(targetId);

        if (isMenuOpen) {
            toggleMobileMenu();
        }
    }
}

function handleScroll() {
    const scrolled = window.scrollY > CONFIG.SCROLL_THRESHOLD;

    if (scrolled !== isScrolled) {
        isScrolled = scrolled;
        navbar.classList.toggle('scrolled', scrolled);
    }

    animateOnScroll();
}

function handleOutsideClick(event) {
    if (isMenuOpen &&
        !navMenu.contains(event.target) &&
        !navToggle.contains(event.target)) {
        toggleMobileMenu();
    }
}

function animateOnScroll() {
    featureCards.forEach((card, index) => {
        if (isInViewport(card) && !card.classList.contains('animated')) {
            setTimeout(() => {
                card.style.opacity = '1';
                card.style.transform = 'translateY(0)';
                card.classList.add('animated');
            }, index * CONFIG.ANIMATION_DELAY);
        }
    });
}

function prepareAnimations() {
    featureCards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'all 0.6s ease-out';
    });
}

function initButtonEffects() {
    const buttons = document.querySelectorAll('.btn');

    buttons.forEach(button => {
        button.addEventListener('mouseenter', () => {
            button.style.transform = 'translateY(-2px)';
        });

        button.addEventListener('mouseleave', () => {
            button.style.transform = 'translateY(0)';
        });

        button.addEventListener('click', () => {
            button.style.transform = 'scale(0.95)';
            setTimeout(() => {
                button.style.transform = '';
            }, 150);
        });
    });
}

function initHeroImageAnimation() {
    const heroImage = document.querySelector('.hero-image');
    if (heroImage) {
        heroImage.style.animation = 'heroFloat 6s ease-in-out infinite';
    }
}

function handleKeydown(event) {
    if (event.key === 'Escape' && isMenuOpen) {
        toggleMobileMenu();
    }

    if (event.key === 'Enter' && event.target === navToggle) {
        event.preventDefault();
        toggleMobileMenu();
    }
}

function cacheElements() {
    navbar = document.getElementById('navbar');
    navToggle = document.getElementById('nav-toggle');
    navMenu = document.getElementById('nav-menu');
    navLinks = document.querySelectorAll('.nav-link');
    featureCards = document.querySelectorAll('.feature-card');
}

function addEventListeners() {
    if (navToggle) {
        navToggle.addEventListener('click', toggleMobileMenu);
    }

    navLinks.forEach(link => {
        link.addEventListener('click', handleNavClick);
    });

    window.addEventListener('scroll', handleScroll);

    document.addEventListener('click', handleOutsideClick);

    document.addEventListener('keydown', handleKeydown);

    window.addEventListener('resize', () => {
        if (window.innerWidth > 768 && isMenuOpen) {
            toggleMobileMenu();
        }
    });
}

function addCustomAnimations() {
    const style = document.createElement('style');
    style.textContent = `
        @keyframes heroFloat {
            0%, 100% { transform: translateY(0px); }
            50% { transform: translateY(-10px); }
        }
    `;
    document.head.appendChild(style);
}

function initLandingPage() {
    try {
        cacheElements();

        if (!navbar) {
            console.warn('Navigation elements not found');
            return;
        }

        addCustomAnimations();

        addEventListeners();

        prepareAnimations();

        initButtonEffects();

        initHeroImageAnimation();

        handleScroll();

        console.log('Landing page initialized successfully');

    } catch (error) {
        console.error('Failed to initialize landing page:', error);
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initLandingPage);
} else {
    initLandingPage();
}

document.addEventListener('visibilitychange', () => {
    if (document.hidden) {
        document.body.style.animationPlayState = 'paused';
    } else {
        document.body.style.animationPlayState = 'running';
    }
});

window.addEventListener('beforeunload', () => {
    document.body.style.overflow = '';
});

if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        toggleMobileMenu,
        smoothScrollTo,
        handleScroll
    };
}