/**
 * Documentation Page Interactive Features
 * Handles navigation, search, smooth scrolling, and responsive behavior
 */

class DocumentationManager {
    constructor() {
        this.currentSection = 'overview';
        this.searchInput = null;
        this.searchResults = [];
        this.isSearching = false;
        this.searchTimeout = null;

        // Initialize when DOM is ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    /**
     * Initialize all documentation features
     */
    init() {
        this.initializeElements();
        this.bindEvents();
        this.setupMobileNavigation();
        this.setupSmoothScrolling();
        this.setupSearchFunctionality();
        this.setupTableOfContents();
        this.setupBackToTop();
        this.updateActiveStates();

        console.log('Documentation manager initialized successfully');
    }

    /**
     * Cache DOM elements for better performance
     */
    initializeElements() {
        // Navigation elements
        this.sidebar = document.getElementById('documentationSidebar');
        this.sidebarToggle = document.getElementById('sidebarToggle');
        this.sidebarMenu = document.getElementById('sidebarMenu');
        this.menuItems = document.querySelectorAll('.menu-item');

        // Content elements
        this.contentContainer = document.getElementById('documentationContent');
        this.contentSections = document.querySelectorAll('.content-section');
        this.currentSectionBreadcrumb = document.getElementById('currentSection');

        // Search elements
        this.searchInput = document.getElementById('documentationSearch');

        // TOC elements
        this.tocNav = document.getElementById('tocNav');
        this.tocLinks = document.querySelectorAll('.toc-link');

        // Back to top
        this.backToTopBtn = document.getElementById('backToTop');

        // Mobile overlay for sidebar
        this.createMobileOverlay();
    }

    /**
     * Bind all event listeners
     */
    bindEvents() {
        // Sidebar menu navigation
        this.menuItems.forEach(item => {
            item.addEventListener('click', (e) => this.handleMenuClick(e));
        });

        // Mobile sidebar toggle
        if (this.sidebarToggle) {
            this.sidebarToggle.addEventListener('click', () => this.toggleMobileSidebar());
        }

        // Search functionality
        if (this.searchInput) {
            this.searchInput.addEventListener('input', (e) => this.handleSearch(e));
            this.searchInput.addEventListener('keydown', (e) => this.handleSearchKeydown(e));
        }

        // TOC navigation
        this.tocLinks.forEach(link => {
            link.addEventListener('click', (e) => this.handleTocClick(e));
        });

        // Back to top button
        if (this.backToTopBtn) {
            this.backToTopBtn.addEventListener('click', () => this.scrollToTop());
        }

        // Window scroll events
        window.addEventListener('scroll', () => this.throttle(this.handleScroll.bind(this), 100)());

        // Window resize events
        window.addEventListener('resize', () => this.throttle(this.handleResize.bind(this), 250)());

        // Keyboard navigation
        document.addEventListener('keydown', (e) => this.handleKeyboardNavigation(e));

        // Close mobile sidebar when clicking outside
        document.addEventListener('click', (e) => this.handleOutsideClick(e));
    }

    /**
     * Handle menu item clicks and navigation
     */
    handleMenuClick(e) {
        e.preventDefault();

        const menuItem = e.currentTarget;
        const sectionId = menuItem.getAttribute('data-section');

        if (sectionId && sectionId !== this.currentSection) {
            this.navigateToSection(sectionId);

            // Close mobile sidebar after navigation
            if (window.innerWidth <= 768) {
                this.closeMobileSidebar();
            }
        }
    }

    /**
     * Navigate to a specific documentation section
     */
    navigateToSection(sectionId) {
        // Hide current section
        const currentSectionElement = document.getElementById(`${this.currentSection}-content`);
        if (currentSectionElement) {
            currentSectionElement.classList.remove('active');
            currentSectionElement.style.display = 'none';
        }

        // Show new section
        const newSectionElement = document.getElementById(`${sectionId}-content`);
        if (newSectionElement) {
            newSectionElement.style.display = 'block';
            setTimeout(() => {
                newSectionElement.classList.add('active');
            }, 10);
        }

        // Update active states
        this.updateActiveMenuItem(sectionId);
        this.updateBreadcrumb(sectionId);
        this.updateTocForSection(sectionId);

        // Update current section
        this.currentSection = sectionId;

        // Scroll to top of content
        if (this.contentContainer) {
            this.contentContainer.scrollTop = 0;
        }

        // Update URL hash without triggering scroll
        this.updateUrlHash(sectionId);

        // Announce section change for screen readers
        this.announcePageChange(sectionId);
    }

    /**
     * Update active menu item styling
     */
    updateActiveMenuItem(sectionId) {
        // Remove active class from all menu items
        this.menuItems.forEach(item => {
            item.classList.remove('active');
        });

        // Add active class to current menu item
        const activeItem = document.querySelector(`[data-section="${sectionId}"]`);
        if (activeItem) {
            activeItem.classList.add('active');

            // Ensure parent collapse is expanded
            const parentCollapse = activeItem.closest('.collapse');
            if (parentCollapse && !parentCollapse.classList.contains('show')) {
                const toggleButton = document.querySelector(`[data-bs-target="#${parentCollapse.id}"]`);
                if (toggleButton) {
                    toggleButton.click();
                }
            }
        }
    }

    /**
     * Update breadcrumb navigation
     */
    updateBreadcrumb(sectionId) {
        if (this.currentSectionBreadcrumb) {
            const sectionTitle = this.getSectionTitle(sectionId);
            this.currentSectionBreadcrumb.textContent = sectionTitle;
        }
    }

    /**
     * Get human-readable section title from section ID
     */
    getSectionTitle(sectionId) {
        const titleMap = {
            'overview': 'Overview',
            'installation': 'Installation',
            'first-steps': 'First Steps',
            'price-tracking': 'Price Tracking',
            'budget-management': 'Budget Management',
            'task-management': 'Task Management',
            'notifications': 'Notifications',
            'adding-products': 'Adding Products',
            'setting-budgets': 'Setting Budgets',
            'tracking-expenses': 'Tracking Expenses',
            'managing-tasks': 'Managing Tasks',
            'faq': 'FAQ',
            'troubleshooting': 'Troubleshooting',
            'contact': 'Contact Support',
            'privacy-policy': 'Privacy Policy',
            'terms-of-service': 'Terms of Service'
        };

        return titleMap[sectionId] || 'Unknown Section';
    }

    /**
     * Mobile sidebar functionality
     */
    setupMobileNavigation() {
        // Create mobile overlay if it doesn't exist
        if (!document.querySelector('.mobile-sidebar-overlay')) {
            this.createMobileOverlay();
        }
    }

    /**
     * Create mobile overlay element
     */
    createMobileOverlay() {
        const overlay = document.createElement('div');
        overlay.className = 'mobile-sidebar-overlay';
        overlay.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            z-index: 1040;
            opacity: 0;
            visibility: hidden;
            transition: all 0.3s ease;
        `;

        overlay.addEventListener('click', () => this.closeMobileSidebar());
        document.body.appendChild(overlay);
        this.mobileOverlay = overlay;
    }

    /**
     * Toggle mobile sidebar visibility
     */
    toggleMobileSidebar() {
        if (this.sidebar.classList.contains('show')) {
            this.closeMobileSidebar();
        } else {
            this.openMobileSidebar();
        }
    }

    /**
     * Open mobile sidebar
     */
    openMobileSidebar() {
        this.sidebar.classList.add('show');
        document.body.style.overflow = 'hidden';

        if (this.mobileOverlay) {
            this.mobileOverlay.style.opacity = '1';
            this.mobileOverlay.style.visibility = 'visible';
        }

        // Focus first menu item for accessibility
        const firstMenuItem = this.sidebar.querySelector('.menu-item');
        if (firstMenuItem) {
            setTimeout(() => firstMenuItem.focus(), 300);
        }
    }

    /**
     * Close mobile sidebar
     */
    closeMobileSidebar() {
        this.sidebar.classList.remove('show');
        document.body.style.overflow = '';

        if (this.mobileOverlay) {
            this.mobileOverlay.style.opacity = '0';
            this.mobileOverlay.style.visibility = 'hidden';
        }
    }

    /**
     * Handle clicks outside sidebar on mobile
     */
    handleOutsideClick(e) {
        if (window.innerWidth <= 768 &&
            this.sidebar.classList.contains('show') &&
            !this.sidebar.contains(e.target) &&
            !this.sidebarToggle.contains(e.target)) {
            this.closeMobileSidebar();
        }
    }

    /**
     * Setup smooth scrolling for anchor links
     */
    setupSmoothScrolling() {
        // Handle hash links in content
        document.addEventListener('click', (e) => {
            const link = e.target.closest('a[href^="#"]');
            if (link) {
                e.preventDefault();
                const targetId = link.getAttribute('href').substring(1);
                this.scrollToElement(targetId);
            }
        });
    }

    /**
     * Scroll to element with smooth animation
     */
    scrollToElement(elementId, offset = 80) {
        const element = document.getElementById(elementId);
        if (element) {
            const elementPosition = element.offsetTop - offset;

            if (this.contentContainer) {
                this.contentContainer.scrollTo({
                    top: elementPosition,
                    behavior: 'smooth'
                });
            } else {
                window.scrollTo({
                    top: elementPosition,
                    behavior: 'smooth'
                });
            }
        }
    }

    /**
     * Setup search functionality
     */
    setupSearchFunctionality() {
        // Create search content index
        this.buildSearchIndex();
    }

    /**
     * Build searchable content index
     */
    buildSearchIndex() {
        this.searchIndex = [];

        this.menuItems.forEach(item => {
            const sectionId = item.getAttribute('data-section');
            const sectionTitle = item.textContent.trim();
            const sectionElement = document.getElementById(`${sectionId}-content`);

            if (sectionElement) {
                const content = sectionElement.textContent.toLowerCase();

                this.searchIndex.push({
                    id: sectionId,
                    title: sectionTitle,
                    content: content,
                    url: `#${sectionId}`
                });
            }
        });
    }

    /**
     * Handle search input
     */
    handleSearch(e) {
        const query = e.target.value.trim().toLowerCase();

        // Clear previous timeout
        if (this.searchTimeout) {
            clearTimeout(this.searchTimeout);
        }

        // Debounce search
        this.searchTimeout = setTimeout(() => {
            if (query.length >= 2) {
                this.performSearch(query);
            } else {
                this.clearSearchResults();
            }
        }, 300);
    }

    /**
     * Perform search and display results
     */
    performSearch(query) {
        this.isSearching = true;

        const results = this.searchIndex.filter(item => {
            return item.title.toLowerCase().includes(query) ||
                item.content.includes(query);
        });

        this.displaySearchResults(results, query);
    }

    /**
     * Display search results
     */
    displaySearchResults(results, query) {
        // Remove existing search results
        this.clearSearchResults();

        if (results.length === 0) {
            this.showNoResults(query);
            return;
        }

        // Create search results container
        const resultsContainer = document.createElement('div');
        resultsContainer.className = 'search-results';
        resultsContainer.innerHTML = `
            <div class="search-results-header">
                <strong>${results.length} result${results.length !== 1 ? 's' : ''} for "${query}"</strong>
                <button class="btn-close-search" aria-label="Clear search">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="search-results-list">
                ${results.map(result => `
                    <a href="${result.url}" class="search-result-item" data-section="${result.id}">
                        <i class="fas fa-file-text"></i>
                        <span class="result-title">${this.highlightQuery(result.title, query)}</span>
                    </a>
                `).join('')}
            </div>
        `;

        // Insert after search container
        this.searchInput.parentNode.insertAdjacentElement('afterend', resultsContainer);

        // Bind events for search results
        resultsContainer.addEventListener('click', (e) => {
            const resultItem = e.target.closest('.search-result-item');
            const closeBtn = e.target.closest('.btn-close-search');

            if (closeBtn) {
                e.preventDefault();
                this.clearSearchResults();
                this.searchInput.value = '';
            } else if (resultItem) {
                e.preventDefault();
                const sectionId = resultItem.getAttribute('data-section');
                this.navigateToSection(sectionId);
                this.clearSearchResults();
                this.searchInput.value = '';
            }
        });
    }

    /**
     * Highlight search query in text
     */
    highlightQuery(text, query) {
        const regex = new RegExp(`(${query})`, 'gi');
        return text.replace(regex, '<mark>$1</mark>');
    }

    /**
     * Show no results message
     */
    showNoResults(query) {
        const noResultsContainer = document.createElement('div');
        noResultsContainer.className = 'search-results no-results';
        noResultsContainer.innerHTML = `
            <div class="search-results-header">
                <strong>No results found for "${query}"</strong>
                <button class="btn-close-search" aria-label="Clear search">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="no-results-content">
                <p>Try different keywords or browse our sections:</p>
                <a href="#faq" class="btn btn-outline-primary btn-sm me-2">FAQ</a>
                <a href="#contact" class="btn btn-outline-secondary btn-sm">Contact Support</a>
            </div>
        `;

        this.searchInput.parentNode.insertAdjacentElement('afterend', noResultsContainer);

        // Bind close event
        const closeBtn = noResultsContainer.querySelector('.btn-close-search');
        closeBtn.addEventListener('click', () => {
            this.clearSearchResults();
            this.searchInput.value = '';
        });
    }

    /**
     * Clear search results
     */
    clearSearchResults() {
        const existingResults = document.querySelector('.search-results');
        if (existingResults) {
            existingResults.remove();
        }
        this.isSearching = false;
    }

    /**
     * Handle search keyboard navigation
     */
    handleSearchKeydown(e) {
        if (e.key === 'Escape') {
            this.clearSearchResults();
            this.searchInput.value = '';
            this.searchInput.blur();
        }
    }

    /**
     * Setup table of contents
     */
    setupTableOfContents() {
        // TOC will be dynamically updated based on current section
        this.updateTocForSection(this.currentSection);
    }

    /**
     * Update TOC for specific section
     */
    updateTocForSection(sectionId) {
        if (!this.tocNav) return;

        const sectionElement = document.getElementById(`${sectionId}-content`);
        if (!sectionElement) return;

        // Find all headings in the section
        const headings = sectionElement.querySelectorAll('h1, h2, h3, h4, h5, h6');

        if (headings.length === 0) {
            this.tocNav.innerHTML = '<p class="text-muted small">No headings found</p>';
            return;
        }

        // Generate TOC HTML
        const tocHTML = Array.from(headings).map(heading => {
            const id = heading.id || this.generateHeadingId(heading.textContent);
            heading.id = id; // Ensure heading has an ID

            return `
                <li>
                    <a href="#${id}" class="toc-link" data-level="${heading.tagName.toLowerCase()}">
                        ${heading.textContent}
                    </a>
                </li>
            `;
        }).join('');

        this.tocNav.innerHTML = `<ul class="toc-list">${tocHTML}</ul>`;

        // Rebind TOC click events
        this.tocLinks = this.tocNav.querySelectorAll('.toc-link');
        this.tocLinks.forEach(link => {
            link.addEventListener('click', (e) => this.handleTocClick(e));
        });
    }

    /**
     * Generate ID for heading
     */
    generateHeadingId(text) {
        return text.toLowerCase()
            .replace(/[^a-z0-9\s-]/g, '')
            .replace(/\s+/g, '-')
            .replace(/-+/g, '-')
            .trim();
    }

    /**
     * Handle TOC link clicks
     */
    handleTocClick(e) {
        e.preventDefault();
        const targetId = e.target.getAttribute('href').substring(1);
        this.scrollToElement(targetId, 100);

        // Update active TOC link
        this.updateActiveTocLink(e.target);
    }

    /**
     * Update active TOC link
     */
    updateActiveTocLink(activeLink) {
        this.tocLinks.forEach(link => link.classList.remove('active'));
        activeLink.classList.add('active');
    }

    /**
     * Setup back to top functionality
     */
    setupBackToTop() {
        // Initial state
        this.updateBackToTopVisibility();
    }

    /**
     * Scroll to top of page
     */
    scrollToTop() {
        if (this.contentContainer) {
            this.contentContainer.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        } else {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        }
    }

    /**
     * Handle scroll events
     */
    handleScroll() {
        this.updateBackToTopVisibility();
        this.updateActiveTocOnScroll();
    }

    /**
     * Update back to top button visibility
     */
    updateBackToTopVisibility() {
        if (!this.backToTopBtn) return;

        const scrollTop = this.contentContainer ?
            this.contentContainer.scrollTop :
            window.pageYOffset;

        if (scrollTop > 300) {
            this.backToTopBtn.classList.add('visible');
        } else {
            this.backToTopBtn.classList.remove('visible');
        }
    }

    /**
     * Update active TOC link based on scroll position
     */
    updateActiveTocOnScroll() {
        const currentSection = document.getElementById(`${this.currentSection}-content`);
        if (!currentSection || !this.tocLinks.length) return;

        const headings = currentSection.querySelectorAll('h1, h2, h3, h4, h5, h6');
        let activeHeading = null;

        headings.forEach(heading => {
            const rect = heading.getBoundingClientRect();
            if (rect.top <= 150) {
                activeHeading = heading;
            }
        });

        if (activeHeading) {
            const activeLink = document.querySelector(`.toc-link[href="#${activeHeading.id}"]`);
            if (activeLink) {
                this.updateActiveTocLink(activeLink);
            }
        }
    }

    /**
     * Handle window resize
     */
    handleResize() {
        // Close mobile sidebar on desktop
        if (window.innerWidth > 768 && this.sidebar.classList.contains('show')) {
            this.closeMobileSidebar();
        }
    }

    /**
     * Handle keyboard navigation
     */
    handleKeyboardNavigation(e) {
        // Escape key handling
        if (e.key === 'Escape') {
            if (this.sidebar.classList.contains('show')) {
                this.closeMobileSidebar();
            }
            if (this.isSearching) {
                this.clearSearchResults();
                this.searchInput.value = '';
            }
        }

        // Ctrl/Cmd + K for search focus
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            if (this.searchInput) {
                this.searchInput.focus();
                this.searchInput.select();
            }
        }
    }

    /**
     * Update URL hash without triggering scroll
     */
    updateUrlHash(sectionId) {
        if (history.replaceState) {
            history.replaceState(null, null, `#${sectionId}`);
        }
    }

    /**
     * Announce page changes for screen readers
     */
    announcePageChange(sectionId) {
        const announcement = document.createElement('div');
        announcement.setAttribute('aria-live', 'polite');
        announcement.setAttribute('aria-atomic', 'true');
        announcement.className = 'sr-only';
        announcement.textContent = `Navigated to ${this.getSectionTitle(sectionId)} section`;

        document.body.appendChild(announcement);

        setTimeout(() => {
            document.body.removeChild(announcement);
        }, 1000);
    }

    /**
     * Update all active states on initialization
     */
    updateActiveStates() {
        // Check URL hash on load
        const hash = window.location.hash.substring(1);
        if (hash && document.getElementById(`${hash}-content`)) {
            this.navigateToSection(hash);
        }
    }

    /**
     * Utility: Throttle function execution
     */
    throttle(func, delay) {
        let timeoutId;
        let lastExecTime = 0;

        return function (...args) {
            const currentTime = Date.now();

            if (currentTime - lastExecTime > delay) {
                func.apply(this, args);
                lastExecTime = currentTime;
            } else {
                clearTimeout(timeoutId);
                timeoutId = setTimeout(() => {
                    func.apply(this, args);
                    lastExecTime = Date.now();
                }, delay - (currentTime - lastExecTime));
            }
        };
    }
}

// Initialize documentation manager
const documentationManager = new DocumentationManager();

// Additional styles for search results (injected via JavaScript)
const searchStyles = `
    <style>
    .search-results {
        position: absolute;
        top: 100%;
        left: 0;
        right: 0;
        background: var(--doc-bg-primary);
        border: 1px solid var(--doc-border-color);
        border-radius: 0.5rem;
        box-shadow: var(--doc-shadow-lg);
        z-index: 1000;
        max-height: 300px;
        overflow-y: auto;
    }
    
    .search-results-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 0.75rem 1rem;
        border-bottom: 1px solid var(--doc-border-light);
        background-color: var(--doc-bg-secondary);
        font-size: 0.875rem;
    }
    
    .btn-close-search {
        background: none;
        border: none;
        color: var(--doc-text-muted);
        padding: 0.25rem;
        cursor: pointer;
        border-radius: 0.25rem;
        transition: var(--doc-transition-fast);
    }
    
    .btn-close-search:hover {
        color: var(--doc-text-primary);
        background-color: var(--doc-bg-tertiary);
    }
    
    .search-results-list {
        padding: 0.5rem;
    }
    
    .search-result-item {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        padding: 0.75rem;
        color: var(--doc-text-primary);
        text-decoration: none;
        border-radius: 0.375rem;
        transition: var(--doc-transition-fast);
        margin-bottom: 0.25rem;
    }
    
    .search-result-item:hover {
        background-color: var(--doc-bg-secondary);
        color: var(--doc-primary-color);
        text-decoration: none;
    }
    
    .search-result-item i {
        color: var(--doc-text-muted);
        font-size: 0.875rem;
    }
    
    .result-title mark {
        background-color: var(--doc-warning-color);
        color: var(--doc-text-primary);
        padding: 0.125rem 0.25rem;
        border-radius: 0.25rem;
    }
    
    .no-results-content {
        padding: 1rem;
        text-align: center;
        color: var(--doc-text-secondary);
    }
    
    .no-results-content p {
        margin-bottom: 1rem;
    }
    
    .search-container {
        position: relative;
    }
    </style>
`;

// Inject search styles
document.head.insertAdjacentHTML('beforeend', searchStyles);