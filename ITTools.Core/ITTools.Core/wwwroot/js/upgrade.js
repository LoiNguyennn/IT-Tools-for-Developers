function generateRandomSentence() {
    const sentences = [
        "Hoàng Sa, Trường Sa là của Việt Nam",
        "Độc lập - Tự do - Hạnh phúc",
        "Không có gì quý hơn độc lập, tự do",
    ];
    const randomIndex = Math.floor(Math.random() * sentences.length);
    return sentences[randomIndex];
}

$(document).ready(function () {
    const randomSentence = generateRandomSentence();
    $("#random-sentence").text(randomSentence);
    
    const inputField = $('input[type="text"]')
    
    inputField.on('input', function () {
        inputField.removeClass('is-invalid');
    })

    $('.upgrade-account').submit(function (e) {
        e.preventDefault();

        const userInput = inputField.val().trim();
        const val = $('#random-sentence').text().trim();
        
        if (userInput !== val) {
            inputField.addClass('is-invalid');
        } else {
            $.post('/Account/Upgrade', function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Upgraded to Premium!',
                        text: 'You now have full access to premium features.',
                        timer: 1500,
                        showConfirmButton: false
                    }).then(() => {
                        window.location.href = response.redirectUrl;
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Upgrade failed',
                        text: response.errors.join(', ') || 'Please try again.'
                    });
                }
            });
        }
    });
})