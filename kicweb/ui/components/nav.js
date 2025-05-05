import './nav.style.scss';

document.addEventListener('DOMContentLoaded', () => {
  const nav = document.querySelector('[data-nav]');
  let lastScrollY = window.scrollY;
  let ticking = false;

  function updateNav() {
    const currentY = window.scrollY;
    const goingDown = currentY > lastScrollY;
    const goingUpFast = currentY < lastScrollY - 10;

    if (currentY <= 0) {
      nav.dataset.state = 'default';
    } else if (goingDown && currentY > 100) {
      nav.dataset.state = 'compact';
    } else if (goingUpFast) {
      nav.dataset.state = 'default';
    }

    lastScrollY = currentY;
    ticking = false;
  }

  window.addEventListener('scroll', () => {
    if (!ticking) {
      requestAnimationFrame(updateNav);
      ticking = true;
    }
  });

  updateNav(); // run once on load
});