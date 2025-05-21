/**
 * Scroll Lock Utilities
 * Prevent background scrolling while allowing interaction inside the open menu.
 */

// Lock page scroll except for the open menu
function lockScroll() {
  window.addEventListener('wheel', blockScroll, { passive: false })
  window.addEventListener('touchmove', blockScroll, { passive: false })
}

// Unlock page scroll
function unlockScroll() {
  window.removeEventListener('wheel', blockScroll)
  window.removeEventListener('touchmove', blockScroll)
}

// Scroll blocker handler
function blockScroll(e) {
  const menu = document.querySelector('[data-nav]')
  if (menu && menu.contains(e.target)) {
    // Check if menu can scroll further in the direction of the event
    const atTop = menu.scrollTop === 0 && e.deltaY < 0
    const atBottom = menu.scrollHeight - menu.clientHeight === menu.scrollTop && e.deltaY > 0

    if (!atTop && !atBottom) {
      // Allow scrolling inside the menu if it's not at the scroll boundaries
      return
    }
  }

  // Block scrolling if not inside the menu or if at scroll boundaries
  e.preventDefault()
  e.stopPropagation()
}


/**
 * Navigation Behavior
 * - Compact nav on scroll down
 * - Expand nav on scroll up
 * - Toggle menu open/close with ARIA and focus management
 */
document.addEventListener('DOMContentLoaded', () => {
  const nav = document.querySelector('[data-nav]')
  const toggleButton = document.querySelector('[data-nav-toggle]')

  let lastScrollY = window.scrollY
  let ticking = false

  /**
   * Handles compacting or expanding the nav on scroll.
   */
  function updateNav() {
    const currentY = window.scrollY
    const goingDown = currentY > lastScrollY
    const goingUpFast = currentY < lastScrollY - 10

    if (currentY <= 0) {
      nav.dataset.state = 'default'
    } else if (goingDown && currentY > 100) {
      nav.dataset.state = 'compact'
    } else if (goingUpFast) {
      nav.dataset.state = 'default'
    }

    lastScrollY = currentY
    ticking = false
  }

  /**
   * Scroll event listener with requestAnimationFrame throttling.
   */
  window.addEventListener('scroll', () => {
    if (!ticking) {
      requestAnimationFrame(updateNav)
      ticking = true
    }
  })

  // Run scroll state initialization on page load
  updateNav()

  /**
   * Toggle menu open/close on button click.
   * - Manage ARIA states
   * - Lock or unlock page scrolling
   * - Apply or remove 'inert' on sibling elements to trap focus
   */
  toggleButton?.addEventListener('click', () => {
    const isExpanded = toggleButton.getAttribute('aria-expanded') === 'true'

    // Find all siblings of the nav to apply/remove inert
    const navContainer = nav.parentElement
    const navContainerSiblings = [...navContainer.parentElement.children].filter((el) => el !== navContainer)

    if (isExpanded) {
      toggleButton.setAttribute('aria-expanded', 'false')
      nav.dataset.expanded = 'false'
      unlockScroll()
      navContainerSiblings.forEach((el) => el.removeAttribute('inert'))
    } else {
      toggleButton.setAttribute('aria-expanded', 'true')
      nav.dataset.expanded = 'true'
      lockScroll()
      navContainerSiblings.forEach((el) => el.setAttribute('inert', 'true'))
    }
  })
})