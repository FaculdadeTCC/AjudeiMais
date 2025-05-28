$(document).ready(function () {
    // Event listener for delete buttons to populate modal
    $(document).on('click', '.delete-category-btn', function () {
        const categoryId = $(this).data('id');
        $('#categoryIdToDelete').val(categoryId); // Set the ID in the hidden input
    });
});