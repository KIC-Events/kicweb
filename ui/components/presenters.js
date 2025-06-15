import { openDialog } from './dialog'



document.addEventListener('DOMContentLoaded', () => {
  const presenterDialog = document.getElementById('presenters-dialog')
  const presenterDetailsList = document.querySelectorAll('#presenters-dialog .presenter-details')
  let currentPresenterIndex = -1

  const setPresenter = (destination) => {
    let destinationIndex
    if (typeof destination === 'number') {
      destinationIndex = destination
    } else if (destination === 'next') {
      destinationIndex = currentPresenterIndex >= presenterDetailsList.length - 1 ? 0 : currentPresenterIndex + 1
    } else if (destination === 'previous') {
      destinationIndex = currentPresenterIndex <= 0 ? presenterDetailsList.length - 1 : currentPresenterIndex - 1
    } else {
      throw new Error(`Invalid destination type: ${typeof destination}. Must be a number, 'next', or 'previous'.`)
    }

    currentPresenterIndex = destinationIndex
    presenterDetailsList.forEach((detailsElement, i) => {
      detailsElement.style.display = currentPresenterIndex === i ? 'block' : 'none'
    })
  }

  const handlePreviousClick = e => {
    e.stopPropagation()
    e.preventDefault()
    setPresenter('previous')
  }

  const handleNextClick = e => {
    console.log('Next button clicked')
    e.stopPropagation()
    e.preventDefault()
    setPresenter('next')
  }

  const handleKeydown = e => {
    if (e.key === 'ArrowLeft') {
      presenterDialog.querySelector('[data-action="previous"]')?.focus()
      setPresenter('previous')
    } else if (e.key === 'ArrowRight') {
      presenterDialog.querySelector('[data-action="next"]')?.focus()
      setPresenter('next')
    }
  }

  const handleClose = () => presenterDialog.close()

  presenterDialog.querySelector('.dialog-close')?.addEventListener('click', () => {
    presenterDialog.close()
  })

  document.querySelectorAll('.presenters__card').forEach((presenterCard, presenterIndex) => {
    presenterCard.addEventListener('click', () => {
      openDialog(presenterDialog, {
        onOpen() {
          // Add event listeners on dialog open
          presenterDialog.querySelector('[data-action="previous"]').addEventListener('click', handlePreviousClick)
          presenterDialog.querySelector('[data-action="next"]').addEventListener('click', handleNextClick)
          presenterDialog.querySelector('[data-action="close"]').addEventListener('click', handleClose)
          document.addEventListener('keydown', handleKeydown)
        },
        onClose() {
          // Remove event listeners on dialog close
          presenterDialog.querySelector('[data-action="previous"]').removeEventListener('click', handlePreviousClick)
          presenterDialog.querySelector('[data-action="next"]').removeEventListener('click', handleNextClick)
          presenterDialog.querySelector('[data-action="close"]').removeEventListener('click', handleClose)
          document.removeEventListener('keydown', handleKeydown)
        }
      })
      setPresenter(presenterIndex)
    })
  })
})