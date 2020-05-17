// Generic Functions
jQuery(document).ready(function () {
    $("a.btn-login,input.btn-login").click(function () {
        // add spinner to button
        $(this).html(
            "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading..."
        );
    });

    // Main Menu
    $(".dropdown-menu a.dropdown-toggle").on("click", function () {
		var $el = $(this);
		var $parent = $(this).offsetParent(".dropdown-menu");
		if (!$(this).next().hasClass("show")) {
			$(this).parents(".dropdown-menu").first().find(".show").removeClass("show");
		}
		var $subMenu = $(this).next(".dropdown-menu");
		$subMenu.toggleClass("show");

		$(this).parent("li").toggleClass("show");

		$(this).parents("li.nav-item.dropdown.show").on("hidden.bs.dropdown", function () {
			$(".dropdown-menu .show").removeClass("show");
		});

		if (!$parent.parent().hasClass("navbar-nav")) {
			$el.next().css({ "top": $el[0].offsetTop, "left": $parent.outerWidth() - 4 });
		}

		return false;
	});

    // Numeric Spinner Inputs
    jQuery("input[type='number']").TouchSpin({
        max: 2147483647
    });

    jQuery(".serverTime-Input").TouchSpin({
        min: -720,
        max: 720
    });

    $(".yafnet .select2-select").each(function () {
        $(this).select2({
            theme: "bootstrap4",
            dropdownAutoWidth: true,
            width: "style",
            placeholder: $(this).attr("placeholder")
        });
    });

    jQuery(".yafnet .select2-image-select").select2({
        theme: "bootstrap4",
        dropdownAutoWidth: true,
        templateResult: formatState,
        templateSelection: formatState,
        width: "style"
    });

    jQuery(".thanks-popover").popover({
        template: '<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-header"></h3><div class="popover-body popover-body-scrollable"></div></div>'
    });

    jQuery('[data-toggle="tooltip"]').tooltip();

    // Convert user posted image to modal images
    jQuery(".img-user-posted").each(function () {
        var image = jQuery(this);

        

        if (image.parents(".selectionQuoteable").length && image.parent().attr("class") !== "card-body py-0") {

            var messageId = image.parents(".selectionQuoteable")[0].id;

            if (!image.parents("a").length) {
                image.wrap('<a href="' + image.attr("src") + '" title="' + image.attr("alt") + '" data-gallery="#blueimp-gallery-' + messageId + '" />');
            }
        }
    });

    jQuery(".attachedImage").each(function () {
        var imageLink = jQuery(this);

        var messageId = imageLink.parents(".selectionQuoteable")[0].id;

        imageLink.attr("data-gallery", "#blueimp-gallery-" + messageId);
    });

    // Show caps lock info on password fields
    jQuery("input[type='password']").keypress(function (e) {
        var s = String.fromCharCode(e.which);
        if (s.toUpperCase() === s && s.toLowerCase() !== s && !e.shiftKey) {
            jQuery(".CapsLockWarning").show();
        }
        else {
            jQuery(".CapsLockWarning").hide();
        }
    });

    if (jQuery("#PostAttachmentListPlaceholder").length) {
        var pageSize = 5;
        var pageNumber = 0;
        getPaginationData(pageSize, pageNumber, false);
    }

    if (jQuery("#SearchResultsPlaceholder").length) {

         jQuery(".searchInput").keypress(function (e) {

            var code = e.which;

            if (code === 13) {
                e.preventDefault();

                var pageNumberSearch = 0;
                getSearchResultsData(pageNumberSearch);
            }

         });
    }

    // Notify dropdown
    $(".dropdown-notify").on("show.bs.dropdown",
        function() {
            var pageSize = 5;
            var pageNumber = 0;
            getNotifyData(pageSize, pageNumber, false);
        });
});