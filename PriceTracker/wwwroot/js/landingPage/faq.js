/**
 * FAQ Page Interactive Functionality
 * Handles accordion, search, filtering, and feedback features
 */

class FAQManager {
    constructor() {
        this.faqData = [];
        this.filteredData = [];
        this.currentCategory = 'all';
        this.searchTerm = '';
        this.isLoading = false;
        
        this.init();
    }

    /**
     * Initialize FAQ functionality
     */
    init() {
        this.bindEvents();
        this.loadFAQData();
        this.hideLoadingSpinner();
    }

    /**
     * Bind all event listeners
     */
    bindEvents() {
        // Search functionality
        const searchInput = document.getElementById('faqSearch');
        const clearSearchBtn = document.getElementById('clearSearch');
        
        if (searchInput) {
            searchInput.addEventListener('input', (e) => this.handleSearch(e.target.value));
            searchInput.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    this.handleSearch(e.target.value);
                }
            });
        }
        
        if (clearSearchBtn) {
            clearSearchBtn.addEventListener('click', () => this.clearSearch());
        }

        // Handle mobile responsiveness
        window.addEventListener('resize', () => this.handleResize());
        
        // Handle browser back/forward navigation
        window.addEventListener('popstate', () => this.handleURLChange());
    }

    /**
     * Load FAQ data (in real app, this would be from API)
     */
    loadFAQData() {
        this.showLoadingSpinner();
        
        // Simulate API delay
        setTimeout(() => {
            this.faqData = this.getStaticFAQData();
            this.filteredData = [...this.faqData];
            this.renderCategories();
            this.renderFAQItems();
            this.hideLoadingSpinner();
        }, 500);
    }

    /**
     * Static FAQ data - in production, fetch from API
     */
    getStaticFAQData() {
        return [
            // Getting Started
            {
                id: 1,
                category: 'getting-started',
                categoryName: 'Getting Started',
                question: 'How do I create my first account?',
                answer: `
                    <p>Creating your account is simple and takes just a few minutes:</p>
                    <ol>
                        <li>Click the "Register" button on the homepage</li>
                        <li>Fill in your email address and create a secure password</li>
                        <li>Verify your email address by clicking the link we send you</li>
                        <li>Complete your profile setup with basic information</li>
                        <li>Start adding your first products and stores!</li>
                    </ol>
                    <p>You can also sign up quickly using your Google account for faster access.</p>
                `
            },
            {
                id: 2,
                category: 'getting-started',
                categoryName: 'Getting Started',
                question: 'What should I do after creating my account?',
                answer: `
                    <p>Welcome! Here's what to do next:</p>
                    <ul>
                        <li><strong>Set up your first budget:</strong> Go to Budget Management and create a monthly budget</li>
                        <li><strong>Add stores:</strong> Include your favorite grocery stores and shopping locations</li>
                        <li><strong>Start tracking prices:</strong> Add products you buy regularly to begin price comparison</li>
                        <li><strong>Create tasks:</strong> Set up your first to-do items to organize your shopping</li>
                    </ul>
                    <p>The dashboard will show your progress and help you get the most value from the app.</p>
                `
            },
            {
                id: 3,
                category: 'getting-started',
                categoryName: 'Getting Started',
                question: 'How do I navigate the main dashboard?',
                answer: `
                    <p>Your dashboard is divided into key sections:</p>
                    <ul>
                        <li><strong>Budget Overview:</strong> Shows your monthly spending progress</li>
                        <li><strong>Yearly Summary:</strong> Displays annual expense trends</li>
                        <li><strong>Top Products:</strong> Lists your most purchased items</li>
                        <li><strong>Task Overview:</strong> Shows upcoming and overdue tasks</li>
                    </ul>
                    <p>Use the navigation menu to access Price Tracking, Budget Management, and Task Management features.</p>
                `
            },

            // Price Tracking
            {
                id: 4,
                category: 'price-tracking',
                categoryName: 'Price Tracking',
                question: 'How do I add a new product for price tracking?',
                answer: `
                    <p>Adding products is quick and easy:</p>
                    <ol>
                        <li>Go to "Price Tracking" in the main menu</li>
                        <li>Click "Add New Product"</li>
                        <li>Enter the product name, brand, and category</li>
                        <li>Add the current price and select the store</li>
                        <li>Click "Save" to start tracking</li>
                    </ol>
                    <p>You can add the same product from multiple stores to compare prices effectively.</p>
                `
            },
            {
                id: 5,
                category: 'price-tracking',
                categoryName: 'Price Tracking',
                question: 'How does store recommendation work?',
                answer: `
                    <p>Our smart recommendation system analyzes your shopping list:</p>
                    <ul>
                        <li>Compares current prices across all your tracked stores</li>
                        <li>Calculates total cost for your entire shopping list</li>
                        <li>Suggests the store with the lowest total cost</li>
                        <li>Shows potential savings compared to other stores</li>
                    </ul>
                    <p>The recommendations update automatically as prices change, ensuring you always get the best deals.</p>
                `
            },
            {
                id: 6,
                category: 'price-tracking',
                categoryName: 'Price Tracking',
                question: 'Can I see price history for products?',
                answer: `
                    <p>Yes! Price history helps you make informed decisions:</p>
                    <ul>
                        <li>View interactive charts showing price trends over time</li>
                        <li>See seasonal patterns and identify best buying opportunities</li>
                        <li>Compare price changes across different stores</li>
                        <li>Set price alerts for when items reach your target price</li>
                    </ul>
                    <p>Access price history by clicking any product in your tracking list.</p>
                `
            },

            // Budget Management
            {
                id: 7,
                category: 'budget-management',
                categoryName: 'Budget Management',
                question: 'How do I set up my monthly budget?',
                answer: `
                    <p>Setting up your budget helps control spending:</p>
                    <ol>
                        <li>Navigate to "Budget Management"</li>
                        <li>Click "Create New Budget"</li>
                        <li>Set your total monthly budget amount</li>
                        <li>Allocate amounts to different categories (groceries, entertainment, etc.)</li>
                        <li>Save your budget to start tracking against it</li>
                    </ol>
                    <p>You can adjust budget categories and amounts anytime as your needs change.</p>
                `
            },
            {
                id: 8,
                category: 'budget-management',
                categoryName: 'Budget Management',
                question: 'What happens when I exceed my budget?',
                answer: `
                    <p>We help you stay aware of overspending:</p>
                    <ul>
                        <li><strong>Automatic alerts:</strong> Get notifications when you reach 80% of your budget</li>
                        <li><strong>Visual warnings:</strong> Budget progress bars turn red when exceeded</li>
                        <li><strong>Detailed reports:</strong> See exactly which categories caused overspending</li>
                        <li><strong>Suggestions:</strong> Get tips for reducing expenses in high-spending areas</li>
                    </ul>
                    <p>These alerts help you make informed decisions about future purchases.</p>
                `
            },

            // Task Management
            {
                id: 9,
                category: 'task-management',
                categoryName: 'Task Management',
                question: 'How do I create and organize tasks?',
                answer: `
                    <p>Task management keeps your shopping and budgeting organized:</p>
                    <ol>
                        <li>Go to "Task Management" section</li>
                        <li>Click "Add New Task"</li>
                        <li>Enter task description and set due date</li>
                        <li>Choose priority level (High, Medium, Low)</li>
                        <li>Assign to categories like "Shopping," "Budget Review," etc.</li>
                    </ol>
                    <p>Tasks appear on your dashboard with upcoming deadlines highlighted.</p>
                `
            },
            {
                id: 10,
                category: 'task-management',
                categoryName: 'Task Management',
                question: 'How do task notifications work?',
                answer: `
                    <p>Stay on top of important tasks with smart notifications:</p>
                    <ul>
                        <li><strong>3-day alerts:</strong> Get notified when tasks are due in 3 days</li>
                        <li><strong>Same-day reminders:</strong> Receive alerts on the due date</li>
                        <li><strong>Overdue warnings:</strong> See highlighted overdue tasks</li>
                        <li><strong>Dashboard summary:</strong> View all upcoming tasks at a glance</li>
                    </ul>
                    <p>Customize notification timing in your profile settings.</p>
                `
            },

            // Technical Support
            {
                id: 11,
                category: 'technical-support',
                categoryName: 'Technical Support',
                question: 'Why can\'t I log in to my account?',
                answer: `
                    <p>Login issues can usually be resolved quickly:</p>
                    <ul>
                        <li><strong>Check your credentials:</strong> Ensure email and password are correct</li>
                        <li><strong>Reset password:</strong> Use "Forgot Password" if needed</li>
                        <li><strong>Clear browser cache:</strong> Sometimes cached data causes issues</li>
                        <li><strong>Try different browser:</strong> Test in incognito/private mode</li>
                        <li><strong>Check email verification:</strong> Ensure your account is verified</li>
                    </ul>
                    <p>If problems persist, contact our support team for assistance.</p>
                `
            },
            {
                id: 12,
                category: 'technical-support',
                categoryName: 'Technical Support',
                question: 'Which browsers are supported?',
                answer: `
                    <p>Our app works best with modern browsers:</p>
                    <ul>
                        <li><strong>Recommended:</strong> Chrome 90+, Firefox 88+, Safari 14+, Edge 90+</li>
                        <li><strong>Mobile:</strong> iOS Safari 14+, Chrome Mobile 90+</li>
                        <li><strong>Features requiring modern support:</strong> Charts, notifications, offline sync</li>
                    </ul>
                    <p>For the best experience, keep your browser updated to the latest version.</p>
                `
            },

            // Account & Privacy
            {
                id: 13,
                category: 'account-privacy',
                categoryName: 'Account & Privacy',
                question: 'How is my data protected?',
                answer: `
                    <p>We take your privacy and security seriously:</p>
                    <ul>
                        <li><strong>Encryption:</strong> All data is encrypted in transit and at rest</li>
                        <li><strong>No selling:</strong> We never sell or share your personal data</li>
                        <li><strong>Secure hosting:</strong> Data stored on secure, certified servers</li>
                        <li><strong>Regular backups:</strong> Your data is backed up daily</li>
                        <li><strong>GDPR compliant:</strong> Full compliance with privacy regulations</li>
                    </ul>
                    <p>Read our Privacy Policy for complete details on data handling.</p>
                `
            },
            {
                id: 14,
                category: 'account-privacy',
                categoryName: 'Account & Privacy',
                question: 'Can I delete my account and data?',
                answer: `
                    <p>You have full control over your account:</p>
                    <ol>
                        <li>Go to Profile Settings</li>
                        <li>Scroll to "Account Management" section</li>
                        <li>Click "Delete Account"</li>
                        <li>Confirm deletion and provide feedback (optional)</li>
                        <li>All your data will be permanently removed within 30 days</li>
                    </ol>
                    <p>Note: This action is irreversible. Consider exporting your data first if needed.</p>
                `
            }
        ];
    }

    /**
     * Render category filter buttons
     */
    renderCategories() {
        const categoriesContainer = document.getElementById('faqCategories');
        if (!categoriesContainer) return;

        // Get unique categories
        const categories = [...new Set(this.faqData.map(item => item.category))];
        const categoryCounts = this.getCategoryCounts();

        let categoriesHTML = `
            <h3 class="categories-title">Browse by Category</h3>
            <div class="category-list">
                <button class="category-button ${this.currentCategory === 'all' ? 'active' : ''}" 
                        data-category="all">
                    All FAQs
                    <span class="category-count">${this.faqData.length}</span>
                </button>
        `;

        categories.forEach(category => {
            const categoryItem = this.faqData.find(item => item.category === category);
            const count = categoryCounts[category] || 0;
            
            categoriesHTML += `
                <button class="category-button ${this.currentCategory === category ? 'active' : ''}" 
                        data-category="${category}">
                    ${categoryItem.categoryName}
                    <span class="category-count">${count}</span>
                </button>
            `;
        });

        categoriesHTML += '</div>';
        categoriesContainer.innerHTML = categoriesHTML;

        // Bind category click events
        categoriesContainer.addEventListener('click', (e) => {
            if (e.target.classList.contains('category-button')) {
                this.handleCategoryFilter(e.target.dataset.category);
            }
        });
    }

    /**
     * Get count of items per category
     */
    getCategoryCounts() {
        const counts = {};
        this.faqData.forEach(item => {
            counts[item.category] = (counts[item.category] || 0) + 1;
        });
        return counts;
    }

    /**
     * Render FAQ items
     */
    renderFAQItems() {
        const itemsContainer = document.getElementById('faqItems');
        const noResultsContainer = document.getElementById('noResults');
        
        if (!itemsContainer) return;

        if (this.filteredData.length === 0) {
            itemsContainer.style.display = 'none';
            if (noResultsContainer) {
                noResultsContainer.style.display = 'block';
            }
            return;
        }

        itemsContainer.style.display = 'flex';
        if (noResultsContainer) {
            noResultsContainer.style.display = 'none';
        }

        const itemsHTML = this.filteredData.map(item => `
            <div class="faq-item" data-id="${item.id}">
                <button class="faq-question" aria-expanded="false" aria-controls="answer-${item.id}">
                    <h3 class="question-text">${item.question}</h3>
                    <i class="fas fa-chevron-down question-icon"></i>
                </button>
                <div class="faq-answer" id="answer-${item.id}">
                    <div class="answer-content">
                        ${item.answer}
                        <div class="faq-feedback">
                            <p class="feedback-question">Was this helpful?</p>
                            <div class="feedback-buttons">
                                <button class="feedback-btn helpful" data-feedback="yes" data-id="${item.id}">
                                    <i class="fas fa-thumbs-up me-1"></i>Yes
                                </button>
                                <button class="feedback-btn not-helpful" data-feedback="no" data-id="${item.id}">
                                    <i class="fas fa-thumbs-down me-1"></i>No
                                </button>
                            </div>
                            <a href="/Home/Contact" class="need-help-link">Still need help?</a>
                        </div>
                    </div>
                </div>
            </div>
        `).join('');

        itemsContainer.innerHTML = itemsHTML;

        // Bind accordion and feedback events
        this.bindFAQEvents();
    }

    /**
     * Bind FAQ item events (accordion and feedback)
     */
    bindFAQEvents() {
        const faqItems = document.querySelectorAll('.faq-item');
        
        faqItems.forEach(item => {
            const questionBtn = item.querySelector('.faq-question');
            const answerDiv = item.querySelector('.faq-answer');
            const feedbackBtns = item.querySelectorAll('.feedback-btn');

            // Accordion functionality
            if (questionBtn && answerDiv) {
                questionBtn.addEventListener('click', () => {
                    this.toggleAccordion(questionBtn, answerDiv);
                });
            }

            // Feedback functionality
            feedbackBtns.forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.stopPropagation();
                    this.handleFeedback(btn.dataset.id, btn.dataset.feedback);
                });
            });
        });
    }

    /**
     * Toggle accordion open/close
     */
    toggleAccordion(questionBtn, answerDiv) {
        const isActive = questionBtn.classList.contains('active');
        
        // Close all other accordions
        document.querySelectorAll('.faq-question.active').forEach(btn => {
            if (btn !== questionBtn) {
                btn.classList.remove('active');
                btn.setAttribute('aria-expanded', 'false');
                btn.parentElement.querySelector('.faq-answer').classList.remove('active');
            }
        });

        // Toggle current accordion
        if (isActive) {
            questionBtn.classList.remove('active');
            questionBtn.setAttribute('aria-expanded', 'false');
            answerDiv.classList.remove('active');
        } else {
            questionBtn.classList.add('active');
            questionBtn.setAttribute('aria-expanded', 'true');
            answerDiv.classList.add('active');
            
            // Smooth scroll to question
            setTimeout(() => {
                questionBtn.scrollIntoView({ 
                    behavior: 'smooth', 
                    block: 'start',
                    inline: 'nearest'
                });
            }, 100);
        }
    }

    /**
     * Handle search functionality
     */
    handleSearch(searchTerm) {
        this.searchTerm = searchTerm.toLowerCase().trim();
        this.updateClearButton();
        this.filterData();
        this.renderFAQItems();
        this.updateURL();
    }

    /**
     * Clear search input
     */
    clearSearch() {
        const searchInput = document.getElementById('faqSearch');
        if (searchInput) {
            searchInput.value = '';
            this.handleSearch('');
        }
    }

    /**
     * Update clear search button visibility
     */
    updateClearButton() {
        const clearBtn = document.getElementById('clearSearch');
        if (clearBtn) {
            if (this.searchTerm.length > 0) {
                clearBtn.classList.add('show');
            } else {
                clearBtn.classList.remove('show');
            }
        }
    }

    /**
     * Handle category filtering
     */
    handleCategoryFilter(category) {
        this.currentCategory = category;
        
        // Update active category button
        document.querySelectorAll('.category-button').forEach(btn => {
            btn.classList.remove('active');
        });
        document.querySelector(`[data-category="${category}"]`).classList.add('active');

        this.filterData();
        this.renderFAQItems();
        this.updateURL();
    }

    /**
     * Filter data based on search and category
     */
    filterData() {
        this.filteredData = this.faqData.filter(item => {
            const matchesCategory = this.currentCategory === 'all' || item.category === this.currentCategory;
            const matchesSearch = this.searchTerm === '' || 
                item.question.toLowerCase().includes(this.searchTerm) ||
                item.answer.toLowerCase().includes(this.searchTerm) ||
                item.categoryName.toLowerCase().includes(this.searchTerm);
            
            return matchesCategory && matchesSearch;
        });
    }

    /**
     * Handle feedback submission
     */
    handleFeedback(itemId, feedback) {
        // In production, send to API
        console.log(`Feedback for item ${itemId}: ${feedback}`);
        
        // Visual feedback
        const item = document.querySelector(`[data-id="${itemId}"]`);
        const feedbackBtns = item.querySelectorAll('.feedback-btn');
        
        feedbackBtns.forEach(btn => {
            btn.style.opacity = '0.5';
            btn.disabled = true;
        });

        // Show thank you message
        const feedbackSection = item.querySelector('.faq-feedback');
        const thankYouMessage = document.createElement('div');
        thankYouMessage.className = 'feedback-thank-you';
        thankYouMessage.innerHTML = '<p style="color: var(--faq-success); font-size: 0.875rem; margin: 0;">Thank you for your feedback!</p>';
        
        setTimeout(() => {
            feedbackSection.appendChild(thankYouMessage);
        }, 300);

        // In production, you would send this to your backend:
        // this.sendFeedbackToAPI(itemId, feedback);
    }

    /**
     * Show loading spinner
     */
    showLoadingSpinner() {
        const spinner = document.getElementById('loadingSpinner');
        if (spinner) {
            spinner.style.display = 'block';
        }
        this.isLoading = true;
    }

    /**
     * Hide loading spinner
     */
    hideLoadingSpinner() {
        const spinner = document.getElementById('loadingSpinner');
        if (spinner) {
            spinner.style.display = 'none';
        }
        this.isLoading = false;
    }

    /**
     * Handle responsive behavior
     */
    handleResize() {
        // Adjust layout for mobile if needed
        if (window.innerWidth < 768) {
            this.closeMobileMenus();
        }
    }

    /**
     * Close mobile menus
     */
    closeMobileMenus() {
        // Close any open accordions on mobile
        document.querySelectorAll('.faq-question.active').forEach(btn => {
            btn.classList.remove('active');
            btn.setAttribute('aria-expanded', 'false');
            btn.parentElement.querySelector('.faq-answer').classList.remove('active');
        });
    }

    /**
     * Update URL with current state (for bookmarking/sharing)
     */
    updateURL() {
        const params = new URLSearchParams();
        
        if (this.currentCategory !== 'all') {
            params.set('category', this.currentCategory);
        }
        
        if (this.searchTerm !== '') {
            params.set('search', this.searchTerm);
        }

        const newURL = params.toString() ? 
            `${window.location.pathname}?${params.toString()}` : 
            window.location.pathname;
            
        history.replaceState({}, '', newURL);
    }

    /**
     * Handle URL changes (back/forward navigation)
     */
    handleURLChange() {
        const params = new URLSearchParams(window.location.search);
        const category = params.get('category') || 'all';
        const search = params.get('search') || '';

        this.currentCategory = category;
        this.searchTerm = search;

        // Update UI
        const searchInput = document.getElementById('faqSearch');
        if (searchInput) {
            searchInput.value = search;
        }

        this.filterData();
        this.renderCategories();
        this.renderFAQItems();
        this.updateClearButton();
    }
}

// Initialize FAQ functionality when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new FAQManager();
});

// Export for use in other modules if needed
if (typeof module !== 'undefined' && module.exports) {
    module.exports = FAQManager;
}