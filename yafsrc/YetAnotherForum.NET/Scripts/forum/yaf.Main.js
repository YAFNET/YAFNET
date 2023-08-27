// Generic Functions
document.addEventListener("DOMContentLoaded", function () {
    if (document.querySelector("a.btn-login,input.btn-login, .btn-spinner") != null) {
        document.querySelector("a.btn-login,input.btn-login, .btn-spinner").click(function () {
            document.querySelector(this).innerHTML = "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";
        });
    }

    // Main Menu
    document.querySelectorAll(".dropdown-menu a.dropdown-toggle").forEach(menu => {
        menu.addEventListener("click", (event) => {
            var $el = menu, $subMenu = $el.nextElementSibling;

            document.querySelectorAll(".dropdown-menu .show").forEach(dropDownMenu => {
                dropDownMenu.classList.remove("show");
            });

            $subMenu.classList.add("show");

            $subMenu.style.top = $el.offsetTop - 10;
            $subMenu.style.left = $el.offsetWidth - 4;

            event.stopPropagation();
        });
    });

    $(".yafnet .select2-select").each(function () {
        $(this).select2({
            width: "100%",
            theme: "bootstrap-5",
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

    $(".yafnet .select2-image-select").each(function () {
        $(this).select2({
            width: "100%",
            theme: "bootstrap-5",
            allowClearing: $(this).data("allow-clear") == "True",
            dropdownAutoWidth: true,
            templateResult: formatState,
            templateSelection: formatState,
            placeholder: $(this).attr("placeholder")
        }).on("select2:select", function (e) {
            if (e.params.data.url) {
                window.location = e.params.data.url;
            }
        });
    });

    if (document.getElementById("PostAttachmentListPlaceholder") != null) {
        const pageSize = 5;
        const pageNumber = 0;
        getPaginationData(pageSize, pageNumber, false);
    }

    if (document.getElementById("SearchResultsPlaceholder") != null && document.querySelector(".searchInput") != null) {
        document.querySelector(".searchInput").addEventListener("keypress", (e) => {
            var code = e.which;
            if (code === 13) {
                e.preventDefault();
                const pageNumberSearch = 0;
                getSearchResultsData(pageNumberSearch);
            }
        });
    }

    // Notify dropdown
    document.querySelector(".dropdown-notify").addEventListener("show.bs.dropdown",
        () => {
            var pageSize = 5;
            var pageNumber = 0;
            getNotifyData(pageSize, pageNumber, false);
        });

    document.querySelectorAll(".form-check > input").forEach(input => { input.classList.add("form-check-input") });
    document.querySelectorAll(".form-check li > input").forEach(input => { input.classList.add("form-check-input") });
    document.querySelectorAll(".form-check > label").forEach(label => { label.classList.add("form-check-label") });
    document.querySelectorAll(".form-check li > label").forEach(label => { label.classList.add("form-check-label") });

    Prism.highlightAll();

    renderAttachPreview(".attachments-preview");

    document.querySelectorAll(".thanks-popover").forEach(thanks => {
        const popover = new bootstrap.Popover(thanks, {
            template: '<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body popover-body-scrollable"></div></div>'
        });

        thanks.addEventListener('show.bs.popover', () => {
            var messageId = thanks.dataset.messageid;
            var url = thanks.dataset.url;

            fetch(url + "/ThankYou/GetThanks/" + messageId, {
                    method: "POST",
                    headers: {
                        'Accept': "application/json",
                        'Content-Type': "application/json;charset=utf-8"
                    }
                }).then(res => res.json())
                .then(response => document.getElementById(`popover-list-${messageId}`).innerHTML = response.ThanksInfo);
        });
    });

    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(toolTip => {
        return new bootstrap.Tooltip(toolTip);
    });

    document.querySelectorAll(".attachedImage").forEach(imageLink => {
        var messageId = imageLink.parentNode.id;

        imageLink.setAttribute("data-gallery", "#blueimp-gallery-" + messageId);
    });
});

