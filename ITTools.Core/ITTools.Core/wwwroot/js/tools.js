$(document).ready(function () {
    // Search functionality
    $("#search-tools").on("keyup", function () {
        const keyword = $(this).val().toLowerCase().trim();
        const toolSection = $(".tools-section");

        let visibleCount = 0;

        $(".tool-card").each(function () {
            const toolName = $(this).find(".card-title").text().toLowerCase();
            const toolDescription = $(this).find(".card-text").text().toLowerCase();
            const isMatch = toolName.includes(keyword) || toolDescription.includes(keyword);

            $(this).closest(".col-md-4").toggle(isMatch);
            if (isMatch) visibleCount++;
        });

        // Toggle the whole section based on how many cards matched
        toolSection.toggle(visibleCount > 0);

        // Update title accordingly
        if (visibleCount === 1) {
            $(".section-title").text("Tool");
        } else {
            $(".section-title").text("Tools");
        }
    });

    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Handle favorite icon click - prevent navigation
    $(document).on("click", ".favorite-icon-container", function (e) {
        e.preventDefault();
        e.stopPropagation();

        const toolId = $(this).data("tool-id");
        const favoriteIcon = $(this).find(".favorite-toggle");
        const isFavorited = favoriteIcon.hasClass('fa-solid');
        const toolCard = $(this).closest(".col-md-4");

        // Gửi yêu cầu AJAX tới ToggleFavorite
        $.ajax({
            type: 'POST',
            url: '/Tools/ToggleFavorite',
            data: { id: toolId },
            success: function (response) {
                if (response.success) {
                    var $favoriteContainer = $("#favorite-tools-container");
                    var $toolsContainer = $("#tools-container");
                    var $favoriteSection = $("#favorite-tools-section");

                    if (response.type === 1) {
                        // Tool was favorited
                        Swal.fire({
                            position: "top-end",
                            icon: "success",
                            title: "Favorited tool!",
                            showConfirmButton: false,
                            timer: 1000
                        });

                        // Cập nhật biểu tượng và tooltip
                        favoriteIcon.removeClass('fa-regular').addClass('fa-solid');
                        favoriteIcon.attr('title', 'Unfavorite').tooltip('dispose').tooltip();

                        // Nếu phần yêu thích không tồn tại, tạo mới
                        if ($favoriteSection.length === 0) {
                            var favoriteSectionHtml = `
                                <div class="tools-section mb-4" id="favorite-tools-section">
                                    <h3 class="section-title">Your Favorite Tools</h3>
                                    <hr class="section-divider" />
                                    <div class="row" id="favorite-tools-container"></div>
                                </div>`;
                            $("#tools-container").closest('.tools-section').before(favoriteSectionHtml);
                            $favoriteContainer = $("#favorite-tools-container");
                            $favoriteSection = $("#favorite-tools-section");
                        }

                        // Di chuyển công cụ vào phần yêu thích
                        const clonedCard = toolCard.clone(true);
                        toolCard.fadeOut(300, function () {
                            clonedCard.prependTo($favoriteContainer).hide().fadeIn(300);
                            toolCard.remove();
                        });
                    } else {
                        // Tool was unfavorited
                        Swal.fire({
                            position: "top-end",
                            icon: "success",
                            title: "Unfavorited tool!",
                            showConfirmButton: false,
                            timer: 1000
                        });

                        // Cập nhật biểu tượng và tooltip
                        favoriteIcon.removeClass('fa-solid').addClass('fa-regular');
                        favoriteIcon.attr('title', 'Add to favorites').tooltip('dispose').tooltip();

                        // Clone the card before removing it
                        const clonedCard = toolCard.clone(true);

                        // Xóa công cụ khỏi phần yêu thích
                        toolCard.fadeOut(300, function () {
                            // Di chuyển công cụ về phần công cụ thông thường
                            clonedCard.appendTo($toolsContainer).hide().fadeIn(300);
                            toolCard.remove();

                            // Kiểm tra xem còn công cụ yêu thích nào không
                            if ($favoriteContainer.children().length === 0) {
                                $favoriteSection.fadeOut(300, function () {
                                    $favoriteSection.remove();
                                });
                            }
                        });
                    }
                } else {
                    // Khôi phục trạng thái biểu tượng nếu thất bại
                    Swal.fire({
                        icon: "error",
                        title: "Oops!",
                        text: "Unable to update favorite status.",
                    });
                }
            },
            error: function (xhr) {
                // Nếu unauthorized, hiển thị thông báo đăng nhập
                if (xhr.status === 401) {
                    Swal.fire({
                        icon: "warning",
                        title: "You need to log in!",
                        text: "Please sign in to save favorites.",
                        confirmButtonText: "Log In",
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = "/Account/Login";
                        }
                    });
                } else {
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