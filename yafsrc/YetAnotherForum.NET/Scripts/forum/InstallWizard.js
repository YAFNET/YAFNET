(function ($) {
	$(document).ready(function() {
        $(".yafWizard").modal("show",
            {
                backdrop: "static",
                keyboard: false
            });

        $(".btn-primary,.btn-info").click(function () {
            // add spinner to button
            $(this).html(
                "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading..."
            );
        });
	});
})(jQuery);