$(document).ready(function () {
    // Event listener for delete buttons to populate modal
    $(document).on('click', '.delete-product-btn', function () {
        const productId = $(this).data('id');
        $('#productIdToDelete').val(productId); // Set the ID in the hidden input
    });
});