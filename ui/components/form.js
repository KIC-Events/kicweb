import $ from 'jquery';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';
import TomSelect from 'tom-select';
import 'tom-select/dist/css/tom-select.css';

document.addEventListener('DOMContentLoaded', () => {
  // Initialize Tom Select only after DOM is ready
  document.querySelectorAll('select[multiple]').forEach(select => {
    new TomSelect(select, {
      plugins: ['remove_button'],
    });
  });

  // Re-parse validation after initializing Tom Select
  $.validator.unobtrusive.parse(document);

  // Apply 'required' styling based on data attributes
  document.querySelectorAll('[data-val-required]').forEach(input => {
    const container = input.closest('.field');
    if (container) {
      container.classList.add('required');
    }
  });
});