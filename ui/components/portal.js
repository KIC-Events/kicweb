/**
 * This script brings React-style "portals" to vanilla JavaScript.
 * It moves elements (e.g. modals, tooltips, overlays) from their original location in the DOM
 * into a separate container (typically on <body>) to escape layout, overflow, or stacking context issues.
 *
 * Inspired by React portals — learn more: https://reactjs.org/docs/portals.html
 *
 * Usage:
 * Add `data-portal="some-id"` to any element you want moved to a portal container.
 * The element will be moved into <div id="some-id"> on page load — created if it doesn't already exist.
 */

document.addEventListener('DOMContentLoaded', () => {
  const elements = document.querySelectorAll('[data-portal]')
  elements.forEach(element => {
    // Get portal ID from data-portal, default to 'portal'
    const portalId = element.dataset.portal ?? 'portal'

    // Ensure it's a valid ID (basic check)
    if (!/^[a-zA-Z_][\w-]*$/.test(portalId)) {
      throw new Error(
        `Invalid data-portal value '${portalId}' on element: ${element.outerHTML.trim()}\n` +
        `It must be a valid DOM ID (without '#')`
      )
    }

    // Find or create the portal element
    let portal = document.getElementById(portalId)
    if (!portal) {
      portal = document.createElement('div')
      portal.id = portalId
      document.body.appendChild(portal)
    }

    // Move the element into the portal
    portal.appendChild(element)
  })
})