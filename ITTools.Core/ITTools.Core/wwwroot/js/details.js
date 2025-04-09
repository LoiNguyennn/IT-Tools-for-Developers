$(document).ready(function () {
    $(document).on("click", ".remove-favorite", function (e) {
        e.preventDefault();
        e.stopPropagation();
    
        const target = $(this);
        const toolId = target.data('tool-id');
    
        console.log("CLICKED");
    
        $.post('/Tools/ToggleFavorite', { id: toolId }, function (response) {
            if (response.success) {
                // Remove the entire row
                target.closest('.d-flex').fadeOut(300, function () {
                    $(this).remove();
                });
            }
        });
    });
})
