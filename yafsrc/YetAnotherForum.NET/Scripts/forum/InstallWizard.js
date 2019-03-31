(function ($) {
	$(document).ready(function() {
		$(".yafWizard").modal("show",
			{
				backdrop: "static",
				keyboard: false
			});

		Ladda.bind(".btn-primary,.btn-info");
	});
})(jQuery);