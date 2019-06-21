(function ($) {
	$(document).ready(function() {
		$(".yafWizard").modal("show",
			{
				backdrop: "static",
				keyboard: false
            });

        $(".custom-control input").addClass("custom-control-input");
        $(".custom-control label").addClass("custom-control-label");

		Ladda.bind(".btn-primary,.btn-info");
	});
})(jQuery);