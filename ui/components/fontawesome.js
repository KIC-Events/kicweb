// Wire up the font awesome css
import '@fortawesome/fontawesome-free/css/all.min.css'

// Import the Font Awesome library and specific icons
import { library, dom } from '@fortawesome/fontawesome-svg-core'
import { faChevronDown } from '@fortawesome/free-solid-svg-icons'

// Add the specific icons to the library
// We can add more icons as needed, but this keeps the bundle size smaller
library.add(
  faChevronDown
)

// Enable automatic SVG replacement
// This will replace <i class="fas fa-chevron-up"></i> with the SVG equivalent
dom.watch()
