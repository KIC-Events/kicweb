document.addEventListener('DOMContentLoaded', () => {
  const dialog = document.querySelector('dialog')
  if (!dialog) {
    return
  }

  document.documentElement.style.setProperty('overflow', 'hidden')
  dialog.showModal()
  dialog.addEventListener('close', () => {
    document.documentElement.style.setProperty('overflow', 'auto')
  })

  if (dialog.hasAttribute('data-cancel-disabled') && dialog.dataset.cancelDisabled !== 'false') {
    // Prevent the dialog from closing when the user clicks the cancel or hits the escape key
    dialog.addEventListener('cancel', (e) => e.preventDefault())
  } else {
    // Close the dialog when the user clicks outside of it
    dialog.addEventListener('click', (e) => {
      const rect = dialog.getBoundingClientRect();
      const clickedOutside =
        e.clientX < rect.left ||
        e.clientX > rect.right ||
        e.clientY < rect.top ||
        e.clientY > rect.bottom;
  
      if (clickedOutside) {
        dialog.close();
      }
    });
  }
})