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
  document.querySelectorAll('[data-val-required]:not([data-val="false"])').forEach(input => {
    const container = input.closest('.field');
    if (container) {
      container.classList.add('required');
    }
  });

  // Phone number input formatting
  const hasPhoneInput = Boolean(document.querySelectorAll('input[type="tel"]').length);
  if (hasPhoneInput) {
    new Cleave('input[type="tel"]', {
      delimiters: ['(', ') ', '-', ''],
      blocks: [0, 3, 3, 4],
      numericOnly: true
    });
  }

  // Change all input type="number" to be <input type="text" inputmode="numeric" pattern="[0-9]*" />
  document.querySelectorAll('input[type="number"]').forEach(input => {
    input.type = 'text';
    input.setAttribute('inputmode', 'numeric');
  });

  const hasNumericInput = Boolean(document.querySelectorAll('input[type="text"][inputmode="numeric"]').length);
  if (hasNumericInput) {
    new Cleave('input[type="text"][inputmode="numeric"]', {
      numeral: true,
      delimiter: '',
      numeralDecimalScale: 0
    });

  }

  // Add 'has-value' class to inputs and textareas with content
  const inlineFields = document.querySelectorAll('.field--inline input, .field--inline textarea, .field--inline select');

  function sync(el) {
    el.classList.toggle('has-value', !!el.value.trim());
  }

  inlineFields.forEach(el => {
    ['input', 'change'].forEach(evt => el.addEventListener(evt, () => sync(el)));
    // handle autofill and initial values
    requestAnimationFrame(() => sync(el));
  });
});