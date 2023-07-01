$(document).ready(function () {
    $(".list-group-item-menu, .message").each(function () {

        var isMessageContext = !!$(this).hasClass("message");

        var contextMenu = $(this).find(".context-menu");

        var messageID = $(this).find(".selectionQuoteable").attr("id");

        if (window.matchMedia("only screen and (max-width: 760px)").matches) {

            var el = $(this)[0];

            // listen for the long-press event
            el.addEventListener("long-press",
                function(e) {

                    // stop the event from bubbling up
                    e.preventDefault();

                    if (isMessageContext) {
                        var selectedText = getSelectedMessageText();

                        if (selectedText.length) {
                            var searchItem = contextMenu.find(".item-search");

                            if (searchItem.length) {
                                searchItem.remove();
                            }

                            var selectedItem = contextMenu.find(".item-selected-quoting");

                            if (selectedItem.length) {
                                selectedItem.remove();
                            }

                            var selectedDivider = contextMenu.find(".selected-divider");

                            if (selectedDivider.length) {
                                selectedDivider.remove();
                            }

                            if (contextMenu.data("url")) {
                                contextMenu.prepend('<a href="javascript:goToURL(\'' +
                                    messageID +
                                    "','" +
                                    selectedText +
                                    "','" +
                                    contextMenu.data("url") +
                                    '\')" class="dropdown-item item-selected-quoting"><i class="fas fa-quote-left fa-fw"></i>&nbsp;' +
                                    contextMenu.data("quote") +
                                    "</a>");
                            }

                            contextMenu.prepend(
                                '<a href="javascript:copyToClipBoard(\'' +
                                selectedText +
                                '\')" class="dropdown-item item-search"><i class="fas fa-clipboard fa-fw"></i>&nbsp;' +
                                contextMenu.data("copy") +
                                "</a>");

                            contextMenu.prepend('<div class="dropdown-divider selected-divider"></div>');

                            contextMenu.prepend(
                                '<a href="javascript:searchText(\'' +
                                selectedText +
                                '\')" class="dropdown-item item-search"><i class="fas fa-search fa-fw"></i>&nbsp;' +
                                contextMenu.data("search") +
                                ' "' +
                                selectedText +
                                '"</a>');
                        }
                    }

                    contextMenu.css({
                        display: "block"
                    }).addClass("show").offset({ left: e.detail.pageX, top: e.detail.pageY });
                });
        }

        $(this).on("contextmenu", function (e) {
            if (isMessageContext) {
                var selectedText = getSelectedMessageText();

                if (selectedText.length) {
                    var searchItem = contextMenu.find(".item-search");

                    if (searchItem.length) {
                        searchItem.remove();
                    }

                    var selectedItem = contextMenu.find(".item-selected-quoting");

                    if (selectedItem.length) {
                        selectedItem.remove();
                    }

                    var selectedDivider = contextMenu.find(".selected-divider");

                    if (selectedDivider.length) {
                        selectedDivider.remove();
                    }

                    if (contextMenu.data("url")) {
                        contextMenu.prepend('<a href="javascript:goToURL(\'' +
                            messageID +
                            "','" +
                            selectedText +
                            "','" +
                            contextMenu.data("url") +
                            '\')" class="dropdown-item item-selected-quoting"><i class="fas fa-quote-left fa-fw"></i>&nbsp;' +
                            contextMenu.data("quote") +
                            "</a>");
                    }

                    contextMenu.prepend(
                        '<a href="javascript:copyToClipBoard(\'' +
                        selectedText +
                        '\')" class="dropdown-item item-search"><i class="fas fa-clipboard fa-fw"></i>&nbsp;' +
                        contextMenu.data("copy") +
                        "</a>");

                    contextMenu.prepend('<div class="dropdown-divider selected-divider"></div>');

                    contextMenu.prepend(
                        '<a href="javascript:searchText(\'' +
                        selectedText +
                        '\')" class="dropdown-item item-search"><i class="fas fa-search fa-fw"></i>&nbsp;' +
                        contextMenu.data("search") +
                        ' "' +
                        selectedText +
                        '"</a>');
                }
            }

            contextMenu.removeClass("show").hide();

            contextMenu.css({
                display: "block"
            }).addClass("show").offset({ left: e.pageX, top: e.pageY });
            return false;
        }).on("click", function () {
            contextMenu.removeClass("show").hide();
        });

        $(this).find(".context-menu a").on("click", function (e) {
            if ($(this).data("bs-toggle") !== undefined && $(this).data("bs-toggle") == "confirm") {
                e.preventDefault();

                var link = $(this).attr("href");
                var text = $(this).data("title");
                var title = $(this).html();
                bootbox.confirm({
                        centerVertical: true,
                        title: title,
                        message: text,
                        buttons: {
                            confirm: {
                                label: '<i class="fa fa-check"></i> ' + $(this).data("yes"),
                                className: "btn-success"
                            },
                            cancel: {
                                label: '<i class="fa fa-times"></i> ' + $(this).data("no"),
                                className: "btn-danger"
                            }
                        },
                        callback: function(confirmed) {
                            if (confirmed) {
                                document.location.href = link;
                            }
                        }
                    }
                );
            }

            contextMenu.removeClass("show").hide();
        });

        $("body").click(function () {
            contextMenu.removeClass("show").hide();
        });

    });
});

function goToURL(messageId, input, url) {
    window.location.href = url + "&q=" + messageId + "&text=" + encodeURIComponent(input);
}

function copyToClipBoard(input)
{
    navigator.clipboard.writeText(input);
}

function searchText(input) {
    let a = document.createElement("a");
    a.target = "_blank";
    a.href = "https://www.google.com/search?q=" + encodeURIComponent(input);
    a.click();
}

function getSelectedMessageText() {
    var text = "";
    var sel = window.getSelection();
    if (sel.rangeCount) {
        var container = document.createElement("div");
        for (var i = 0, len = sel.rangeCount; i < len; ++i) {
            container.appendChild(sel.getRangeAt(i).cloneContents());
        }
        text = container.textContent || container.innerText;
    }

    return text.replace(/<p[^>]*>/ig, "\n").replace(/<\/p>|  /ig, "").replace("(", "").replace(")", "")
        .replace("\"", "").replace("'", "").replace("\'", "").replace(";", "").trim();
}
