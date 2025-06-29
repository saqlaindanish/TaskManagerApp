
// AJAX requent for form submission
$(document).on('submit', '.modal-form', function (e) {
    e.preventDefault();
    var form = $(this);

    $.ajax({
        url: form.attr('action'),
        type: 'POST',
        data: form.serialize(),
        success: function (response, status, xhr) {
            const contentType = xhr.getResponseHeader("Content-Type");

            if (contentType && contentType.includes("application/json")) {
                // Successful - redirect
                if (response.success) {
                    window.location.href = response.redirectUrl;
                }
            }
            else {
                // failed - show updated modal with validation errors
                $('#mainModal .modal-content').html(response);
            }
        },

        error: function (xhr) {
            console.error("Unexpected error:", xhr.responseText);
        }
    });
});