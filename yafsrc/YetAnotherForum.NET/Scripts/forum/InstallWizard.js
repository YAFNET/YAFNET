(function ($) {
    $(document).ready(function () {
        $(".yafWizard").modal("show", {
            backdrop: "static",
            keyboard: false
        });

        Ladda.bind(".buttonPrimary,.buttonInfo");

        $(".selectConnection, .selectTimeZone, .selectCulture").select2({ theme: "bootstrap" });
    });

})(jQuery);