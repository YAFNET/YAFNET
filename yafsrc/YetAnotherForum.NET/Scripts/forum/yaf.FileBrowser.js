jQuery(document).ready(function () {
    jQuery(".form-file-input").on("change",
        function () {
            var fileName = $(this)[0].files[0].name;
            $(this).next(".form-file-label").html(fileName);
        });
});