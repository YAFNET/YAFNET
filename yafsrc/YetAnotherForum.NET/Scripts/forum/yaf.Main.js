// Generic Functions
jQuery(document).ready(function () {
    $("a.btn-login,input.btn-login").click(function () {
        // add spinner to button
        $(this).html(
            "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading..."
        );
    });

    // Main Menu
    $(".dropdown-menu a.dropdown-toggle").on("click", function (e) {
        var $el = $(this);
        var $parent = $el.parents(".dropdown-menu");
        var parentDropDown = $parent.prev();
        var $subMenu = $el.next();

        if (!$subMenu.hasClass("show")) {
            $(".dropdown-menu").find(".show").removeClass("show");
        }

        if (!$parent.hasClass("show")) {
            parentDropDown.dropdown("show");
        }

        $subMenu.toggleClass("show");

        $subMenu.css({ "top": $el[0].offsetTop - 10, "left": $el.outerWidth() - 4 });

        e.stopPropagation();
    });


    // Numeric Spinner Inputs
    $("input[type='number']").each(function () {

        if ($(this).hasClass("form-pager")) {
            var holder = $(this).closest(".mb-3");

            $(this).TouchSpin({
                min: holder.data("min"),
                max: holder.data("max")
            });
        } else {
            $(this).TouchSpin({
                max: 2147483647
            });
        }
    });

    jQuery(".serverTime-Input").TouchSpin({
        min: -720,
        max: 720
    });

    $(".yafnet .select2-select").each(function () {
        $(this).select2({
            width: "100%",
            theme: "bootstrap4",
            placeholder: $(this).attr("placeholder")
        });
    });

    if ($(".select2-image-select").length) {
        var selected = $(".select2-image-select").val();

        var groups = {};
        $(".yafnet .select2-image-select option[data-category]").each(function () {
            var sGroup = $.trim($(this).attr("data-category"));
            groups[sGroup] = true;
        });
        $.each(groups, function (c) {
            $(".yafnet .select2-image-select").each(function () {
                $(this).find("option[data-category='" + c + "']").wrapAll('<optgroup label="' + c + '">');
            });
        });

        $(".select2-image-select").val(selected);
    }

    jQuery(".yafnet .select2-image-select").select2({
        width: "100%",
        theme: "bootstrap4",
        allowClear: false,
        dropdownAutoWidth: true,
        templateResult: formatState,
        templateSelection: formatState
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

    $(".form-check > input").addClass("form-check-input");
    $(".form-check li > input").addClass("form-check-input");

    $(".form-check > label").addClass("form-check-label");
    $(".form-check li > label").addClass("form-check-label");

    $(".img-user-posted").on("error",
        function () {
            $(this).parent().parent().hide();
        });
});

document.addEventListener("DOMContentLoaded", function () {
    var popoverTriggerList = [].slice.call(document.querySelectorAll(".thanks-popover"));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl,
            {
                template:
                    '<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body popover-body-scrollable"></div></div>'
            });
    });

    $(".thanks-popover").on("show.bs.popover", function () {
        var messageId = $(this).data("messageid");
        var url = $(this).data("url");
        $.ajax({
            url: url + "/ThankYou/GetThanks/" + messageId,
            type: "POST",
            contentType: "application/json;charset=utf-8",
            cache: true,
            success: function (response) {
                $("#popover-list-" + messageId).html(response.ThanksInfo);
            }
        });
    });

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    [].forEach.call(document.querySelectorAll(".attachedImage"),
        function(imageLink) {
            var messageId = imageLink.parentNode.id;

            imageLink.setAttribute("data-gallery", "#blueimp-gallery-" + messageId);
        });
});