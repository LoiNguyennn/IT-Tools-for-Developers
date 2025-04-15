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

        // Show/hide section titles based on visible cards
        $(".tools-section").each(function () {
            const visibleCards = $(this).find(".col-md-4:visible").length;
            if (visibleCards === 0) {
                $(this).hide();
            } else {
                $(this).show();
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
        const favoriteIcon = $(this).find(".favorite-toggle");
        const isFavorited = favoriteIcon.hasClass('fa-solid');
        const toolCard = $(this).closest(".tool-card");

        // Toggle classes correctly
        favoriteIcon.toggleClass('fa-solid fa-regular');

        // Update title for tooltip
        favoriteIcon.attr('title', isFavorited ? 'Favorite' : 'Unfavorite');

        // If you're using Bootstrap tooltips, you must also refresh it
        favoriteIcon.tooltip('dispose').tooltip();

        $.ajax({
            type: 'POST',
            url: '/Tools/ToggleFavorite',
            data: { id: toolId },
            success: function (response) {
                if (response.type === 1) {
                    // Tool was favorited
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Favorited tool!",
                        showConfirmButton: false,
                        timer: 1000
                    });

                    // Optional: Move the tool card to the favorites section without refreshing
                    if ($("#favorite-tools-container").length) {
                        const card = toolCard.closest(".col-md-4");
                        card.fadeOut(300, function () {
                            card.detach().prependTo("#favorite-tools-container").fadeIn(300);
                        });
                    }
                } else {
                    // Tool was unfavorited
                    // Optional: Move the tool card to the other tools section
                    if ($("#tools-container").length) {
                        const card = toolCard.closest(".col-md-4");
                        card.fadeOut(300, function () {
                            card.detach().appendTo("#tools-container").fadeIn(300);
                        });
                    }
                }
            },
            error: function (xhr) {
                // If unauthorized, revert icon and show login alert
                if (xhr.status === 401) {
                    favoriteIcon.toggleClass('fa-solid fa-regular'); // revert toggle
                    Swal.fire({
                        icon: "warning",
                        title: "You need to log in!",
                        text: "Please sign in to save favorites.",
                        confirmButtonText: "Log In",
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = "/Account/Login"; // or your login route
                        }
                    });
                } else {
                    // Optional: handle other errors
                    Swal.fire({
                        icon: "error",
                        title: "Oops!",
                        text: "Something went wrong.",
                    });
                }
            }
        });
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
            url: '/Tools/Execute',
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