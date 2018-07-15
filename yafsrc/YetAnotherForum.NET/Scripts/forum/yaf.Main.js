// Generic Functions
jQuery(document).ready(function () {
    // Cookie alert
    if (!getCookie("YAF-AcceptCookies")) {
        jQuery(".cookiealert").addClass("show");
    }

    $(".acceptcookies").click(function () {
        setCookie("YAF-AcceptCookies", true, 60);
        jQuery(".cookiealert").removeClass("show");
    });

    // Numeric Spinner Inputs
    jQuery("input[type='number']").TouchSpin({
        max: 2147483647
    });

    jQuery(".serverTime-Input").TouchSpin({
        min: -720,
        max: 720
    });

    jQuery(".yaf-net .standardSelectMenu").select2({
        theme: "bootstrap",
        dropdownAutoWidth: true,
        width: 'style'
    });

    jQuery(".yaf-net .selectpicker").select2({
        theme: "bootstrap",
        dropdownAutoWidth: true,
        templateResult: formatState,
        templateSelection: formatState,
        width: 'style'
    });

    jQuery('[data-toggle="tooltip"]').tooltip();

    // Convert user posted image to modal images
    jQuery(".postContainer .UserPostedImage,.postContainer_Alt .UserPostedImage, .previewPostContent .UserPostedImage").each(function () {
        var image = jQuery(this);

        if (!image.parents('a').length) {
            image.wrap('<a href="' + image.attr("src") + '" title="' + image.attr("alt") + '" data-gallery />');
        }
    });

    // Show caps lock info on password fields
    jQuery("input[type='password']").keypress(function (e) {
        var s = String.fromCharCode(e.which);
        if (s.toUpperCase() === s && s.toLowerCase() !== s && !e.shiftKey) {
            jQuery('.CapsLockWarning').show();
        }
        else {
            jQuery('.CapsLockWarning').hide();
        }
    });

    if (jQuery('#PostAttachmentListPlaceholder').length) {
        var pageSize = 5;
        var pageNumber = 0;
        getPaginationData(pageSize, pageNumber, false);
    }

    if (jQuery('#SearchResultsPlaceholder').length) {

         jQuery(".searchInput").keypress(function (e) {

            var code = e.which;

            if (code === 13) {
                e.preventDefault();

                var pageNumberSearch = 0;
                getSeachResultsData(pageNumberSearch);
            }

        });
    }
});

if (document.addEventListener) document.addEventListener("click", function (e) { window.event = e; }, true);
if (document.addEventListener) document.addEventListener("mouseover", function(e) { window.event = e; }, true);