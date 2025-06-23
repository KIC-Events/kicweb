document.addEventListener('DOMContentLoaded', () => {
  const nav = document.querySelector('[data-nav]');
  if (!nav) return;

  nav.querySelectorAll('[data-dropdown-toggle]').forEach(button => {
    const menuId = button.getAttribute('aria-controls');
    if (!menuId) return;

    const menu = document.getElementById(menuId);
    if (!menu || !menu.hasAttribute('data-dropdown')) return;

    const container = button.parentElement;

    function openMenu() {
      if (window.getComputedStyle(container).position === 'static') {
        container.style.position = 'relative';
      }

      menu.classList.add('show');
      button.setAttribute('aria-expanded', 'true');
      button.classList.add('open');

      // Focus first item
      const focusables = menu.querySelectorAll('a, button, [tabindex]:not([tabindex="-1"])');
      if (focusables.length) {
        focusables[0].focus();
      }
    }

    function closeMenu() {
      menu.classList.remove('show');
      button.setAttribute('aria-expanded', 'false');
      button.classList.remove('open');
    }

    // Toggle on click
    button.addEventListener('click', e => {
      e.stopPropagation();

      const isOpen = menu.classList.contains('show');
      if (isOpen) {
        closeMenu();
        button.focus();
      } else {
        openMenu();
      }
    });

    // Keyboard open: Enter or Space ONLY
    button.addEventListener('keydown', e => {
      if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        openMenu();
      }
    });

    // Keyboard nav within menu
    menu.addEventListener('keydown', e => {
      const items = [...menu.querySelectorAll('a, button, [tabindex]:not([tabindex="-1"])')];
      const index = items.indexOf(document.activeElement);

      if (e.key === 'ArrowDown') {
        e.preventDefault();
        const next = items[index + 1] || items[0];
        next.focus();
      }

      if (e.key === 'ArrowUp') {
        e.preventDefault();
        const prev = items[index - 1] || items[items.length - 1];
        prev.focus();
      }

      if (e.key === 'Escape') {
        e.preventDefault();
        closeMenu();
        button.focus();
      }

      if (e.key === 'Tab') {
        // Allow natural focus movement but close the menu
        closeMenu();
      }
    });
  });

  // Close all on outside click
  document.addEventListener('click', () => {
    document.querySelectorAll('[data-dropdown]').forEach(menu => menu.classList.remove('show'));
    document.querySelectorAll('[data-dropdown-toggle]').forEach(button => {
      button.setAttribute('aria-expanded', 'false');
      button.classList.remove('open');
    });
  });
});