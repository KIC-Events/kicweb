/**
 * Opens a dialog element using showModal and sets up proper event listeners.
 * @param {HTMLDialogElement} dialog - The dialog element to open.
 * @param {Object} [options] - Optional configuration for the dialog.
 * @param {boolean} [options.onOpen] - Callback to run when the dialog opens.
 * @param {boolean} [options.onClose] - Callback to run when the dialog closes.
 */
export function openDialog(dialog, options = {}) {
  if (!dialog || dialog.nodeName !== 'DIALOG') {
    throw new Error('Invalid dialog element passed to openDialog()')
  }

  if (dialog.open) {
    console.warn('Dialog is already open, not opening again.')
    return // Already open, abort
  }

  document.documentElement.style.setProperty('overflow', 'hidden')
  dialog.showModal()
  options.onOpen?.()

  const onClose = () => {
    document.documentElement.style.setProperty('overflow', 'auto')
    dialog.removeEventListener('close', onClose)
    options.onClose?.()
  }
  dialog.addEventListener('close', onClose)

  if (dialog.hasAttribute('data-cancel-disabled') && dialog.dataset.cancelDisabled !== 'false') {
    dialog.addEventListener('cancel', e => e.preventDefault())
  } else {
    const onClickOutside = e => {
      const rect = dialog.getBoundingClientRect()
      const clickedOutside =
        e.clientX < rect.left ||
        e.clientX > rect.right ||
        e.clientY < rect.top ||
        e.clientY > rect.bottom

      if (clickedOutside) {
        dialog.close()
        dialog.removeEventListener('click', onClickOutside)
      }
    }
    dialog.addEventListener('click', onClickOutside)
  }
}

document.addEventListener('DOMContentLoaded', () => {
  const dialogs = document.querySelectorAll('dialog[data-open-on-load]')
  if (!dialogs.length) {
    return
  } else if (dialogs.length > 1) {
    console.warn('Multiple dialogs with data-open-on-load found. Only the first will be opened')
  }

  console.log('opening dialog on page load:', dialogs[0].id || 'unnamed dialog')

  openDialog(dialogs[0])
})