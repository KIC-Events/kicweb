document.addEventListener('DOMContentLoaded', () => {
  const accordions = document.querySelectorAll('.accordion')

  accordions.forEach(accordion => {
    const toggles = Array.from(accordion.querySelectorAll('[data-accordion-toggle]'))

    const closeAll = () => {
      toggles.forEach(toggle => {
        toggle.setAttribute('aria-expanded', 'false')
        const panel = document.getElementById(toggle.getAttribute('aria-controls'))
        if (panel) {
          panel.setAttribute('inert', '')
          panel.setAttribute('aria-hidden', 'true')
          panel.style.maxHeight = '0'
          toggle.closest('.accordion-item')?.classList.remove('active')
        }
      })
    }

    const open = (toggle) => {
      const panel = document.getElementById(toggle.getAttribute('aria-controls'))
      if (panel) {
        panel.style.maxHeight = (panel.scrollHeight / 16) + 'rem'
        toggle.setAttribute('aria-expanded', 'true')
        panel.setAttribute('aria-hidden', 'false')
        panel.removeAttribute('inert')
        toggle.closest('.accordion-item')?.classList.add('active')
      }
    }

    toggles.forEach((toggle, index) => {
      toggle.addEventListener('click', () => {
        const isExpanded = toggle.getAttribute('aria-expanded') === 'true'
        closeAll()
        if (!isExpanded) {
          open(toggle)
        }
      })

      toggle.addEventListener('keydown', (e) => {
        switch (e.key) {
          case 'Enter':
          case ' ':
            e.preventDefault()
            toggle.click()
            break
          case 'ArrowDown':
            e.preventDefault()
            if (index < toggles.length - 1) {
              toggles[index + 1].focus()
            }
            break
          case 'ArrowUp':
            e.preventDefault()
            if (index > 0) {
              toggles[index - 1].focus()
            }
            break
          case 'Home':
            e.preventDefault()
            toggles[0].focus()
            break
          case 'End':
            e.preventDefault()
            toggles[toggles.length - 1].focus()
            break
        }
      })
    })

    // On resize, update the maxHeight of any open panel
    window.addEventListener('resize', () => {
      toggles.forEach(toggle => {
        if (toggle.getAttribute('aria-expanded') === 'true') {
          const panel = document.getElementById(toggle.getAttribute('aria-controls'))
          if (panel) {
            panel.style.maxHeight = (panel.scrollHeight / 16) + 'rem'
          }
        }
      })
    })
  })
})