jQuery(document).ready(function () {
    jQuery(".custom-file-input").on("change",
        function () {
            var fileName = $(this)[0].files[0].name;
            $(this).next(".custom-file-label").html(fileName);
        });
});