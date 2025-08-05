import $ from 'jquery';
import Cleave from 'cleave.js';
import 'cleave.js/dist/addons/cleave-phone.us';
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

  // Phone number input formatting
  new Cleave('input[type="tel"]', {
    delimiters: ['(', ') ', '-', ''],
    blocks: [0, 3, 3, 4],
    numericOnly: true
  });

  // Change all input type="number" to be <input type="text" inputmode="numeric" pattern="[0-9]*" />
  document.querySelectorAll('input[type="number"]').forEach(input => {
    input.type = 'text';
    input.setAttribute('inputmode', 'numeric');
  });
  new Cleave('input[type="text"][inputmode="numeric"]', {
    numeral: true,
    delimiter: '',
    numeralDecimalScale: 0
  });
});