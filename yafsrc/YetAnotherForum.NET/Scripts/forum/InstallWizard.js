(function ($) {
    $(document).ready(function () {
        $(".yafWizard").modal("show", {
            backdrop: "static",
            keyboard: false
        });

        Ladda.bind(".btn-primary,.btn-info");

        $(".selectConnection, .selectTimeZone, .selectCulture").select2({ theme: "bootstrap" });
    });

})(jQuery);