function searchEmployees(query) {
    if (query.length >= 3) {
        $.ajax({
            url: '@Url.Action("Search", "Employee")',
            type: 'GET',
            data: { searchQuery: query },
            success: function (data) {
                // Update the search results on the page
                if (data && data.length > 0) {
                    // Display the search results in a dropdown or any other format
                } else {
                    // Display a message indicating no results found
                }
            }
        });
    }
}
