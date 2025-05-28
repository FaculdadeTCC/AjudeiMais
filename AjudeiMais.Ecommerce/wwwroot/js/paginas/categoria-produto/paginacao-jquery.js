$(document).ready(function () {
    const categoriesPerPage = 5; // Number of categories to display per page (you can adjust this)
    let $tableRows = $('#categoryTableBody tr'); // All category rows initially rendered
    let $filteredRows = $tableRows; // Currently filtered rows, starts with all rows
    let currentPage = 1; // Current active page

    // Function to display rows for the current page
    function displayRowsForPage() {
        // Hide all rows first
        $tableRows.hide();

        const startIndex = (currentPage - 1) * categoriesPerPage;
        const endIndex = startIndex + categoriesPerPage;

        // Show only the rows for the current page from the filtered set
        $filteredRows.slice(startIndex, endIndex).show();

        renderPaginationControls(); // Update pagination controls based on current state
    }

    // Function to render pagination controls
    function renderPaginationControls() {
        const totalPages = Math.ceil($filteredRows.length / categoriesPerPage);
        const $paginationControls = $('#paginationControls');
        $paginationControls.empty(); // Clear existing pagination links

        if (totalPages <= 1) {
            return; // No pagination needed if 1 or fewer pages
        }

        // Previous button
        $paginationControls.append(`
            <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="${currentPage - 1}">Anterior</a>
            </li>
        `);

        // Page numbers (displaying a limited range for better UX)
        const maxPageButtons = 5; // Max buttons to show at once
        let startPage = Math.max(1, currentPage - Math.floor(maxPageButtons / 2));
        let endPage = Math.min(totalPages, startPage + maxPageButtons - 1);

        // Adjust startPage if we're near the end to ensure `maxPageButtons` are shown
        if (endPage - startPage + 1 < maxPageButtons) {
            startPage = Math.max(1, endPage - maxPageButtons + 1);
        }

        for (let i = startPage; i <= endPage; i++) {
            $paginationControls.append(`
                <li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>
            `);
        }

        // Next button
        $paginationControls.append(`
            <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="${currentPage + 1}">Próxima</a>
            </li>
        `);

        // Add click event for pagination links
        $paginationControls.find('.page-link').on('click', function (e) {
            e.preventDefault(); // Prevent default link behavior (page reload)
            const newPage = parseInt($(this).data('page'));

            // Only navigate if it's a valid and different page
            if (!isNaN(newPage) && newPage > 0 && newPage <= totalPages && newPage !== currentPage) {
                currentPage = newPage;
                displayRowsForPage(); // Re-render table for the new page
            }
        });
    }

    // Search functionality
    $('#searchInput').on('keyup', function () {
        const searchText = $(this).val().toLowerCase().trim();

        // Filter rows based on the data-search-term attribute
        $filteredRows = $tableRows.filter(function () {
            const searchTerm = $(this).data('search-term');
            // If the search term is empty, all rows are considered filtered
            if (searchText === '') {
                return true;
            }
            // Otherwise, check if the row's search term includes the input
            return searchTerm && searchTerm.includes(searchText);
        });

        currentPage = 1; // Always reset to the first page when a search is performed
        displayRowsForPage(); // Re-render the table with filtered results
    });

    // Initial display of categories when the page loads
    // This will show the first page of categories and generate initial pagination controls
    displayRowsForPage();
});

