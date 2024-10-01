// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showStates() {
    var states = ["Arkansas", "California", "Colorado", "Connecticut","District of Columbia", "Hawaii", "Maine", "Maryland", "Massachusetts",
        "Michigan", "Minnesota", "Nevada", "New Hampshire", "New Jersey", "New Mexico", "New York", "Oregon", "Pennsylvania", "Rhode Island", "Vermont", "Virginia", "Washington"];

    $('#state').on('change', function () {
        if (states.includes($(this).val())) {
            $('#IdOther').show();
        }
    })
    
}
