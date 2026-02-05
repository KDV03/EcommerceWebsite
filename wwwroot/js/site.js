// TechMart E-Commerce - Modern JavaScript
// Enhanced interactions and animations

document.addEventListener('DOMContentLoaded', function() {
    console.log('🛒 TechMart E-Commerce - Modern Interface Loaded');
    
    // ========== NAVBAR SCROLL EFFECT ==========
    const navbar = document.querySelector('.navbar');
    let lastScroll = 0;
    
    window.addEventListener('scroll', () => {
        const currentScroll = window.pageYOffset;
        
        if (currentScroll > 50) {
            navbar.classList.add('scrolled');
        } else {
            navbar.classList.remove('scrolled');
        }
        
        lastScroll = currentScroll;
    });
    
    // ========== SMOOTH SCROLL FOR ANCHOR LINKS ==========
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
    
    // ========== SEARCH FORM VALIDATION ==========
    const searchForms = document.querySelectorAll('.search-form');
    searchForms.forEach(form => {
        const input = form.querySelector('input[type="text"]');
        const button = form.querySelector('.search-button');
        
        form.addEventListener('submit', function(e) {
            if (input && input.value.trim() === '') {
                e.preventDefault();
                
                // Add shake animation
                input.style.animation = 'shake 0.5s';
                input.focus();
                
                setTimeout(() => {
                    input.style.animation = '';
                }, 500);
            }
        });
        
        // Add search icon animation on focus
        if (input && button) {
            input.addEventListener('focus', () => {
                button.style.transform = 'translateY(-50%) scale(1.1)';
            });
            
            input.addEventListener('blur', () => {
                button.style.transform = 'translateY(-50%) scale(1)';
            });
        }
    });
    
    // ========== PRODUCT CARD HOVER EFFECTS ==========
    const productCards = document.querySelectorAll('.product-card');
    productCards.forEach(card => {
        // Add smooth hover effect
        card.addEventListener('mouseenter', function() {
            this.style.transition = 'all 0.35s cubic-bezier(0.34, 1.56, 0.64, 1)';
        });
        
        // Add tilt effect on mouse move
        card.addEventListener('mousemove', function(e) {
            const rect = this.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            
            const centerX = rect.width / 2;
            const centerY = rect.height / 2;
            
            const rotateX = (y - centerY) / 30;
            const rotateY = (centerX - x) / 30;
            
            // Subtle tilt effect
            const image = this.querySelector('.product-image');
            if (image) {
                image.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) scale(1.02)`;
            }
        });
        
        card.addEventListener('mouseleave', function() {
            const image = this.querySelector('.product-image');
            if (image) {
                image.style.transform = 'perspective(1000px) rotateX(0) rotateY(0) scale(1)';
            }
        });
    });
    
    // ========== CATEGORY CARD INTERACTIONS ==========
    const categoryCards = document.querySelectorAll('.category-card');
    categoryCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            // Add ripple effect
            const ripple = document.createElement('div');
            ripple.className = 'ripple-effect';
            this.appendChild(ripple);
            
            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
    
    // ========== IMAGE LAZY LOADING ==========
    const images = document.querySelectorAll('img');
    images.forEach(img => {
        // Fade in when loaded
        img.style.opacity = '0';
        img.style.transition = 'opacity 0.5s ease-in-out';
        
        img.addEventListener('load', function() {
            this.style.opacity = '1';
        });
        
        // Fallback for errors
        img.addEventListener('error', function() {
            this.src = '/images/placeholder.jpg';
            this.style.opacity = '0.7';
        });
    });
    
    // ========== FORM ENHANCEMENTS ==========
    const formInputs = document.querySelectorAll('.form-input');
    formInputs.forEach(input => {
        // Add floating label effect
        input.addEventListener('focus', function() {
            this.parentElement.classList.add('focused');
        });
        
        input.addEventListener('blur', function() {
            if (this.value === '') {
                this.parentElement.classList.remove('focused');
            }
        });
        
        // Check if input has value on load
        if (input.value !== '') {
            input.parentElement.classList.add('focused');
        }
    });
    
    // ========== INTERSECTION OBSERVER FOR ANIMATIONS ==========
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);
    
    // Observe elements that should animate on scroll
    const animatedElements = document.querySelectorAll('.section-title, .feature-item');
    animatedElements.forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(30px)';
        el.style.transition = 'opacity 0.6s ease-out, transform 0.6s ease-out';
        observer.observe(el);
    });
    
    // ========== PRICE FORMATTER ==========
    const priceElements = document.querySelectorAll('.product-price, .discounted-price, .current-price');
    priceElements.forEach(el => {
        // Add subtle number animation on load
        const originalPrice = el.textContent;
        if (originalPrice.includes('$')) {
            const price = parseFloat(originalPrice.replace('$', ''));
            if (!isNaN(price)) {
                animateValue(el, 0, price, 800);
            }
        }
    });
    
    // ========== TOOLTIP INITIALIZATION ==========
    const tooltipElements = document.querySelectorAll('[data-tooltip]');
    tooltipElements.forEach(el => {
        el.addEventListener('mouseenter', function(e) {
            const tooltip = document.createElement('div');
            tooltip.className = 'custom-tooltip';
            tooltip.textContent = this.dataset.tooltip;
            document.body.appendChild(tooltip);
            
            const rect = this.getBoundingClientRect();
            tooltip.style.left = rect.left + (rect.width / 2) - (tooltip.offsetWidth / 2) + 'px';
            tooltip.style.top = rect.top - tooltip.offsetHeight - 10 + 'px';
            
            setTimeout(() => tooltip.classList.add('show'), 10);
        });
        
        el.addEventListener('mouseleave', function() {
            const tooltips = document.querySelectorAll('.custom-tooltip');
            tooltips.forEach(t => {
                t.classList.remove('show');
                setTimeout(() => t.remove(), 300);
            });
        });
    });
    
    // ========== NEWSLETTER FORM (if exists) ==========
    const newsletterForm = document.querySelector('.newsletter-form');
    if (newsletterForm) {
        newsletterForm.addEventListener('submit', function(e) {
            e.preventDefault();
            const email = this.querySelector('input[type="email"]').value;
            
            // Show success message
            const successMsg = document.createElement('div');
            successMsg.className = 'success-message';
            successMsg.textContent = 'Thanks for subscribing! 🎉';
            this.appendChild(successMsg);
            
            setTimeout(() => {
                successMsg.classList.add('show');
            }, 10);
            
            // Reset form after delay
            setTimeout(() => {
                this.reset();
                successMsg.remove();
            }, 3000);
        });
    }
    
    // ========== BUTTON RIPPLE EFFECT ==========
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('click', function(e) {
            const ripple = document.createElement('span');
            ripple.className = 'button-ripple';
            
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            
            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            
            this.appendChild(ripple);
            
            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
    
    // ========== STOCK QUANTITY PULSE ==========
    const stockIndicators = document.querySelectorAll('.stock-available');
    stockIndicators.forEach(indicator => {
        const pulse = () => {
            indicator.style.transform = 'scale(1.05)';
            setTimeout(() => {
                indicator.style.transform = 'scale(1)';
            }, 200);
        };
        
        // Pulse every 3 seconds
        setInterval(pulse, 3000);
    });
});

// ========== UTILITY FUNCTIONS ==========

// Animate number values
function animateValue(element, start, end, duration) {
    let startTimestamp = null;
    const step = (timestamp) => {
        if (!startTimestamp) startTimestamp = timestamp;
        const progress = Math.min((timestamp - startTimestamp) / duration, 1);
        const value = progress * (end - start) + start;
        element.textContent = '$' + value.toFixed(2);
        if (progress < 1) {
            window.requestAnimationFrame(step);
        }
    };
    window.requestAnimationFrame(step);
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

// Update URL without page refresh
function updateQueryString(key, value) {
    const url = new URL(window.location);
    if (value) {
        url.searchParams.set(key, value);
    } else {
        url.searchParams.delete(key);
    }
    window.history.pushState({}, '', url);
}

// Debounce function for performance
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

// Add custom CSS animations
const style = document.createElement('style');
style.textContent = `
    @keyframes shake {
        0%, 100% { transform: translateX(0); }
        10%, 30%, 50%, 70%, 90% { transform: translateX(-5px); }
        20%, 40%, 60%, 80% { transform: translateX(5px); }
    }
    
    .ripple-effect {
        position: absolute;
        border-radius: 50%;
        background: rgba(6, 182, 212, 0.3);
        width: 20px;
        height: 20px;
        animation: ripple 0.6s ease-out;
        pointer-events: none;
    }
    
    @keyframes ripple {
        to {
            transform: scale(20);
            opacity: 0;
        }
    }
    
    .button-ripple {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.6);
        transform: scale(0);
        animation: button-ripple 0.6s ease-out;
        pointer-events: none;
    }
    
    @keyframes button-ripple {
        to {
            transform: scale(2);
            opacity: 0;
        }
    }
    
    .custom-tooltip {
        position: fixed;
        background: rgba(15, 23, 42, 0.95);
        color: white;
        padding: 0.5rem 1rem;
        border-radius: 0.5rem;
        font-size: 0.875rem;
        font-weight: 600;
        z-index: 10000;
        pointer-events: none;
        opacity: 0;
        transform: translateY(5px);
        transition: all 0.3s ease;
    }
    
    .custom-tooltip.show {
        opacity: 1;
        transform: translateY(0);
    }
    
    .success-message {
        background: linear-gradient(135deg, #10B981 0%, #059669 100%);
        color: white;
        padding: 1rem;
        border-radius: 0.75rem;
        margin-top: 1rem;
        font-weight: 700;
        text-align: center;
        opacity: 0;
        transform: translateY(-10px);
        transition: all 0.3s ease;
    }
    
    .success-message.show {
        opacity: 1;
        transform: translateY(0);
    }
`;
document.head.appendChild(style);

console.log('✨ Enhanced interactions loaded successfully!');
