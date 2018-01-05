// Generic Functions
jQuery(document).ready(function () {
    // Numeric Spinner Inputs
    jQuery("input[type='number']").TouchSpin({
        max: 2147483647
    });

    jQuery(".serverTime-Input").TouchSpin({
        min: -720,
        max: 720
    });

    jQuery(".yafnet-bs .standardSelectMenu").select2({
        theme: "bootstrap",
        dropdownAutoWidth: true,
        width: 'style'
    });

    jQuery(".yafnet-bs .selectpicker").select2({
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
});

if (document.addEventListener) document.addEventListener("click", function (e) { window.event = e; }, true);
if (document.addEventListener) document.addEventListener("mouseover", function(e) { window.event = e; }, true);