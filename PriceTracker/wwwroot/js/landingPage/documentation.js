/**
 * Documentation Page JavaScript
 * Handles smooth scrolling, TOC highlighting, back-to-top, and FAQ interactions
 * Enhanced with mobile-first responsive features
 * Total: ~180 lines with mobile optimizations
 */

document.addEventListener('DOMContentLoaded', function () {
    // Initialize all functionality
    initSmoothScrolling();
    initBackToTop();
    initTOCHighlighting();
    initFAQAccordion();
    initCopyButtons();
    initMobileOptimizations();
    initResponsiveFeatures();
});

/**
 * Mobile-specific optimizations
 */
function initMobileOptimizations() {
    // Detect mobile device
    const isMobile = window.innerWidth <= 768;

    if (isMobile) {
        // Add mobile-specific classes
        document.body.classList.add('mobile-device');

        // Optimize touch interactions
        const touchElements = document.querySelectorAll('.toc-link, .faq-question, .btn-primary');
        touchElements.forEach(element => {
            element.style.minHeight = '44px';
            element.style.display = 'flex';
            element.style.alignItems = 'center';
        });

        // Collapse TOC on mobile initially (except first load)
        const toc = document.querySelector('nav.bg-gray-50');
        if (toc && window.location.hash === '') {
            setTimeout(() => {
                toc.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }, 500);
        }
    }
}

/**
 * Responsive features that adapt to screen size changes
 */
function initResponsiveFeatures() {
    let resizeTimeout;

    window.addEventListener('resize', function () {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(function () {
            handleResponsiveChanges();
        }, 150);
    });

    // Initial call
    handleResponsiveChanges();
}

/**
 * Handle responsive layout changes
 */
function handleResponsiveChanges() {
    const isMobile = window.innerWidth <= 768;
    const isTablet = window.innerWidth > 768 && window.innerWidth <= 1024;

    // Adjust back-to-top button behavior based on screen size
    const backToTopBtn = document.getElementById('backToTop');
    if (backToTopBtn) {
        if (isMobile) {
            backToTopBtn.style.bottom = '1rem';
            backToTopBtn.style.right = '1rem';
            backToTopBtn.style.width = '3rem';
            backToTopBtn.style.height = '3rem';
        } else {
            backToTopBtn.style.bottom = '2rem';
            backToTopBtn.style.right = '2rem';
            backToTopBtn.style.width = 'auto';
            backToTopBtn.style.height = 'auto';
        }
    }

    // Adjust TOC behavior
    const toc = document.querySelector('nav.bg-gray-50');
    if (toc) {
        if (isMobile) {
            toc.style.position = 'static';
            toc.style.marginBottom = '2rem';
        } else {
            toc.style.position = '';
            toc.style.marginBottom = '';
        }
    }

    // Recalculate section highlighting for mobile
    if (isMobile) {
        updateActiveSection();
    }
}

/**
 * Smooth scrolling for anchor links
 */
function initSmoothScrolling() {
    const tocLinks = document.querySelectorAll('nav a[href^="#"]');

    tocLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();

            const targetId = this.getAttribute('href').substring(1);
            const targetSection = document.getElementById(targetId);

            if (targetSection) {
                targetSection.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });

                // Update URL without jumping
                history.pushState(null, null, `#${targetId}`);
            }
        });
    });
}

/**
 * Back to top button functionality
 */
function initBackToTop() {
    const backToTopBtn = document.getElementById('backToTop');

    if (!backToTopBtn) return;

    // Show/hide button based on scroll position
    window.addEventListener('scroll', function () {
        if (window.pageYOffset > 300) {
            backToTopBtn.classList.remove('opacity-0', 'pointer-events-none');
            backToTopBtn.classList.add('opacity-100');
        } else {
            backToTopBtn.classList.add('opacity-0', 'pointer-events-none');
            backToTopBtn.classList.remove('opacity-100');
        }
    });

    // Smooth scroll to top when clicked
    backToTopBtn.addEventListener('click', function () {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });
}

/**
 * Highlight current section in table of contents
 */
function initTOCHighlighting() {
    const sections = document.querySelectorAll('section[id]');
    const tocLinks = document.querySelectorAll('nav a[href^="#"]');

    if (sections.length === 0 || tocLinks.length === 0) return;

    function updateActiveSection() {
        let currentSection = '';
        const scrollPos = window.pageYOffset + 100; // Offset for better UX

        sections.forEach(section => {
            const sectionTop = section.offsetTop;
            const sectionHeight = section.offsetHeight;

            if (scrollPos >= sectionTop && scrollPos < sectionTop + sectionHeight) {
                currentSection = section.getAttribute('id');
            }
        });

        // Update TOC link styles
        tocLinks.forEach(link => {
            const href = link.getAttribute('href').substring(1);

            if (href === currentSection) {
                link.classList.remove('text-blue-600', 'hover:text-blue-800');
                link.classList.add('text-blue-800', 'font-medium', 'bg-blue-50', 'rounded', 'px-2', '-mx-2');
            } else {
                link.classList.add('text-blue-600', 'hover:text-blue-800');
                link.classList.remove('text-blue-800', 'font-medium', 'bg-blue-50', 'rounded', 'px-2', '-mx-2');
            }
        });
    }

    // Throttle scroll events for better performance
    let scrollTimeout;
    window.addEventListener('scroll', function () {
        if (scrollTimeout) {
            clearTimeout(scrollTimeout);
        }

        scrollTimeout = setTimeout(updateActiveSection, 10);
    });

    // Initial call
    updateActiveSection();
}

/**
 * FAQ accordion functionality
 */
function initFAQAccordion() {
    const faqItems = document.querySelectorAll('#faq .bg-gray-50');

    faqItems.forEach(item => {
        const question = item.querySelector('h4');
        const answer = item.querySelector('p');

        if (!question || !answer) return;

        // Add cursor pointer and interaction styling
        question.style.cursor = 'pointer';
        question.classList.add('transition-colors', 'hover:text-blue-700');

        // Add expand/collapse icon
        const icon = document.createElement('i');
        icon.className = 'fas fa-chevron-down text-sm ml-2 transition-transform';
        question.appendChild(icon);

        // Initially hide answers (except first one for better UX)
        const isFirst = Array.from(faqItems).indexOf(item) === 0;
        if (!isFirst) {
            answer.style.display = 'none';
            icon.style.transform = 'rotate(-90deg)';
        }

        // Click handler for toggling
        question.addEventListener('click', function () {
            const isVisible = answer.style.display !== 'none';

            if (isVisible) {
                answer.style.display = 'none';
                icon.style.transform = 'rotate(-90deg)';
                item.classList.remove('ring-2', 'ring-blue-200');
            } else {
                answer.style.display = 'block';
                icon.style.transform = 'rotate(0deg)';
                item.classList.add('ring-2', 'ring-blue-200');
            }
        });
    });
}

/**
 * Copy button functionality for code snippets
 */
function initCopyButtons() {
    const codeBlocks = document.querySelectorAll('pre, code');

    codeBlocks.forEach(block => {
        // Only add copy button to multi-line code blocks
        if (block.textContent.trim().split('\n').length > 1) {
            const wrapper = document.createElement('div');
            wrapper.className = 'relative';

            block.parentNode.insertBefore(wrapper, block);
            wrapper.appendChild(block);

            const copyBtn = document.createElement('button');
            copyBtn.className = 'absolute top-2 right-2 px-2 py-1 bg-gray-700 text-white text-xs rounded hover:bg-gray-600 transition-colors';
            copyBtn.innerHTML = '<i class="fas fa-copy mr-1"></i>Copy';

            copyBtn.addEventListener('click', function () {
                navigator.clipboard.writeText(block.textContent).then(() => {
                    copyBtn.innerHTML = '<i class="fas fa-check mr-1"></i>Copied!';
                    copyBtn.classList.remove('bg-gray-700', 'hover:bg-gray-600');
                    copyBtn.classList.add('bg-green-600');

                    setTimeout(() => {
                        copyBtn.innerHTML = '<i class="fas fa-copy mr-1"></i>Copy';
                        copyBtn.classList.add('bg-gray-700', 'hover:bg-gray-600');
                        copyBtn.classList.remove('bg-green-600');
                    }, 2000);
                });
            });

            wrapper.appendChild(copyBtn);
        }
    });
}