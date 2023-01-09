(function ($) {
    $(document).ready(function() {
        $(".yafWizard").modal("show",
            {
                backdrop: "static",
                keyboard: false
            });

        $(".form-check > input").addClass("form-check-input");
        $(".form-check li > input").addClass("form-check-input");

        $(".form-check > label").addClass("form-check-label");
        $(".form-check li > label").addClass("form-check-label");

        $(".btn-primary,.btn-info").click(function () {
            // add spinner to button
            $(this).html(
                "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading..."
            );
        });
    });
})(jQuery);