$(document).ready(function () {
    // Search functionality
    $("#search-tools").on("keyup", function () {
        var keyword = $(this).val().toLowerCase();
        $(".tool-card").each(function () {
            var toolName = $(this).find(".card-title").text().toLowerCase();
            var toolDescription = $(this).find(".card-text").text().toLowerCase();
            if (toolName.includes(keyword) || toolDescription.includes(keyword)) {
                $(this).closest(".col-md-4").show();
            } else {
                $(this).closest(".col-md-4").hide();
            }
        });
    });

    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Handle favorite icon click - prevent navigation
    $(document).on("click", ".favorite-icon-container", function (e) {
        e.preventDefault();
        e.stopPropagation();

        const toolId = $(this).data("tool-id");
        console.log("Favorite clicked for tool ID:", toolId);

        // Add your favorite logic here
        // For example:
        // $.post('/Tools/ToggleFavorite', { id: toolId }, function(response) {
        //    console.log("Favorite toggled", response);
        // });
    });

    // Handle edit icon click - navigate to edit page
    $(document).on("click", ".edit-icon-container", function (e) {
        e.preventDefault();
        e.stopPropagation();

        const editUrl = $(this).data("edit-url");
        window.location.href = editUrl;
    });

    // Handle card click for navigation to details
    $(document).on("click", ".tool-card", function (e) {
        const toolId = $(this).data("tool-id");
        window.location.href = `/Tools/Details/${toolId}`;
    });

    // Tool selection handlers
    $('.select-tool').click(function () {
        const toolName = $(this).data('tool');
        $('#toolName').val(toolName);
        $('#result').text('');
    });

    // Form submission handler
    $('#toolForm').submit(function (e) {
        e.preventDefault();
        const toolName = $('#toolName').val();
        const input = $('#input').val();
        if (!toolName) {
            alert('Please select a tool first');
            return;
        }

        $.ajax({
            url: '@Url.Action("Execute", "Tools")',
            type: 'POST',
            data: {
                toolName: toolName,
                input: input
            },
            success: function (response) {
                $('#result').text(response.result);
            },
            error: function (xhr) {
                $('#result').text('Error: ' + (xhr.responseJSON?.result || xhr.statusText));
            }
        });
    });
});