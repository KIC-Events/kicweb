document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('[data-accordion-toggle]').forEach(button => {
    const accordionId = button.getAttribute('data-accordion-id')

    button.addEventListener('click', () => {
      const expanded = button.getAttribute('aria-expanded') === 'true'
      const shouldOpen = !expanded
      const container = button.parentElement
      console.log('Accordion container:', container)
      const panel = document.getElementById(button.getAttribute('aria-controls'))

      // Close all other accordions in the same group
      document.querySelectorAll(`#${accordionId} .accordion-item`).forEach(element => {
        element.classList.remove('active')
      })
      document.querySelectorAll(`#${accordionId} .accordion-header`).forEach(element => {
        element.setAttribute('aria-expanded', false)
      })
      document.querySelectorAll(`#${accordionId} .accordion-content`).forEach(element => {
        element.setAttribute('aria-hidden', true)
        element.style.maxHeight = '0'
      })

      // Toggle the clicked accordion
      container.classList.toggle('active', shouldOpen)
      button.setAttribute('aria-expanded', !expanded)
      panel.setAttribute('aria-hidden', expanded)

      if (shouldOpen) {
        panel.style.maxHeight = panel.scrollHeight + 'px'
      } else {
        panel.style.maxHeight = '0'
      }
    })
  })
})