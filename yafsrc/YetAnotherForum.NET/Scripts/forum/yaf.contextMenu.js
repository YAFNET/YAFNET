document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll(".list-group-item-menu, .message").forEach(element => {

        var isMessageContext = !!element.classList.contains("message");

        var contextMenu = element.querySelector(".context-menu");

        var messageId = 0;

        if (element.querySelector(".selectionQuoteable") != null) {
            messageId = element.querySelector(".selectionQuoteable").id;
        }

        if (window.matchMedia("only screen and (max-width: 760px)").matches) {
            
            const el = element;

            // listen for the long-press event
            el.addEventListener("long-press",
                function(e) {

                    // stop the event from bubbling up
                    e.preventDefault();

                    if (isMessageContext) {
                        const selectedText = getSelectedMessageText();

                        if (selectedText.length) {
                            const searchItem = contextMenu.querySelector(".item-search"),
                            selectedItem = contextMenu.querySelector(".item-selected-quoting"),
                            selectedDivider = contextMenu.querySelector(".selected-divider");

                            if (searchItem != null) {
                                document.querySelectorAll(".item-search").forEach(item => {
                                    item.remove();
                                });
                            }

                            if (selectedItem != null) {
                                selectedItem.remove();
                            }

                            if (selectedDivider != null) {
                                selectedDivider.remove();
                            }

                            if (contextMenu.dataset.url) {
                                const link = document.createElement("a");

                                link.classList.add("dropdown-item");
                                link.classList.add("item-selected-quoting");

                                link.href = `javascript:goToURL('${messageId}','${selectedText}','${contextMenu.dataset.url} ')`;

                                link.innerHTML = `<i class="fas fa-quote-left fa-fw"></i>&nbsp;${contextMenu.dataset.quote}`;

                                contextMenu.appendChild(link);
                            }

                            const linkSearch = document.createElement("a");

                            linkSearch.classList.add("dropdown-item");
                            linkSearch.classList.add("item-search");

                            linkSearch.href = `javascript:copyToClipBoard('${selectedText}')`;

                            linkSearch.innerHTML = `<i class="fas fa-clipboard fa-fw"></i>&nbsp;${contextMenu.dataset.copy}`;

                            contextMenu.appendChild(linkSearch);

                            const divider = document.createElement("div");

                            divider.classList.add("dropdown-divider");
                            divider.classList.add("selected-divider");

                            contextMenu.appendChild(linkSearch);

                            const linkSelected = document.createElement("a");

                            linkSelected.classList.add("dropdown-item");
                            linkSelected.classList.add("item-search");

                            linkSelected.href = `javascript:searchText('${selectedText}')`;

                            linkSelected.innerHTML = `<i class="fas fa-search fa-fw"></i>&nbsp;${contextMenu.dataset.search} "${selectedText}"`;

                            contextMenu.appendChild(linkSelected);
                        }
                    }


                    contextMenu.style.left = e.detail.pageX;
                    contextMenu.style.top = e.detail.pageY;
                    contextMenu.style.display = "block";

                    contextMenu.classList.add("show");
                });
        }

        element.addEventListener("contextmenu",
            (e) => {
                e.preventDefault();

                // close other
                document.querySelectorAll(".context-menu").forEach(menu => {
                    menu.style.display = "none";
                    menu.classList.remove("show");
                });

                if (isMessageContext) {
                    const selectedText = getSelectedMessageText();

                    if (selectedText.length) {
                        const searchItem = contextMenu.querySelector(".item-search"),
                            selectedItem = contextMenu.querySelector(".item-selected-quoting"),
                            selectedDivider = contextMenu.querySelector(".selected-divider");

                        if (searchItem != null) {
                            document.querySelectorAll(".item-search").forEach(item => {
                                item.remove();
                            });
                        }


                        if (selectedItem != null) {
                            selectedItem.remove();
                        }

                        if (selectedDivider != null) {
                            selectedDivider.remove();
                        }

                        if (contextMenu.dataset.url) {
                            const link = document.createElement("a");

                            link.classList.add("dropdown-item");
                            link.classList.add("item-selected-quoting");

                            link.href = `javascript:goToURL('${messageId}','${selectedText}','${contextMenu.dataset.url} ')`;

                            link.innerHTML = `<i class="fas fa-quote-left fa-fw"></i>&nbsp;${contextMenu.dataset.quote}`;

                            contextMenu.appendChild(link);
                        }

                        const linkSearch = document.createElement("a");

                        linkSearch.classList.add("dropdown-item");
                        linkSearch.classList.add("item-search");

                        linkSearch.href = `javascript:copyToClipBoard('${selectedText}')`;

                        linkSearch.innerHTML = `<i class="fas fa-clipboard fa-fw"></i>&nbsp;${contextMenu.dataset.copy}`;

                        contextMenu.appendChild(linkSearch);

                        const divider = document.createElement("div");

                        divider.classList.add("dropdown-divider");
                        divider.classList.add("selected-divider");

                        contextMenu.appendChild(linkSearch);

                        const linkSelected = document.createElement("a");

                        linkSelected.classList.add("dropdown-item");
                        linkSelected.classList.add("item-search");

                        linkSelected.href = `javascript:searchText('${selectedText}')`;

                        linkSelected.innerHTML = `<i class="fas fa-search fa-fw"></i>&nbsp;${contextMenu.dataset.search} "${selectedText}"`;

                        contextMenu.appendChild(linkSelected);
                    }
                }

                contextMenu.style.display = "block";
                contextMenu.style.left = e.offsetX + "px";
                contextMenu.style.top = e.offsetY + "px";

                contextMenu.classList.add("show");

                return false;
            });

        element.addEventListener("click",
            () => {
                contextMenu.classList.remove("show");
                contextMenu.style.display = "none";
            });

        element.querySelector(".context-menu a").addEventListener("click", (e) => {
            var a = e.target;
            if (a.dataset.toggle !== undefined && a.dataset.toggle === "confirm") {
                e.preventDefault();

                var link = a.href;

                const text = a.dataset.title,
                    title = a.innerHTML;

                bootbox.confirm({
                        centerVertical: true,
                        title: title,
                        message: text,
                        buttons: {
                            confirm: {
                                label: `<i class="fa fa-check"></i> ${a.dataset.yes}`,
                                className: "btn-success"
                            },
                            cancel: {
                                label: `<i class="fa fa-times"></i> ${a.dataset.no}`,
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

            contextMenu.classList.remove("show");
            contextMenu.style.display = "none";
        });

        contextMenu.addEventListener("click", function (event) {
            if (event.target.parentElement.matches('[data-bs-toggle="confirm"]')) {
                event.preventDefault();
                const button = event.target.parentElement,
                    text = button.dataset.title,
                    title = button.innerHTML;

                bootbox.confirm({
                        centerVertical: true,
                        title: title,
                        message: text,
                        buttons: {
                            confirm: {
                                label: `<i class="fa fa-check"></i> ${button.dataset.yes}`,
                                className: "btn-success"
                            },
                            cancel: {
                                label: `<i class="fa fa-times"></i> ${button.dataset.no}`,
                                className: "btn-danger"
                            }
                        },
                        callback: function (confirmed) {
                            if (confirmed) {
                                button.click();
                            }
                        }
                    }
                );
            }

            contextMenu.classList.remove("show");
            contextMenu.style.display = "none";
        }, false);

        document.querySelector("body").addEventListener("click", () => {
            contextMenu.classList.remove("show");
            contextMenu.style.display = "none";
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
    const a = document.createElement("a");
    a.target = "_blank";
    a.href = `https://www.google.com/search?q=${encodeURIComponent(input)}`;
    a.click();
}

function getSelectedMessageText() {
    var text = "";
    const sel = window.getSelection();
    if (sel.rangeCount) {
        const container = document.createElement("div");
        for (var i = 0, len = sel.rangeCount; i < len; ++i) {
            container.appendChild(sel.getRangeAt(i).cloneContents());
        }
        text = container.textContent || container.innerText;
    }

    return text.replace(/<p[^>]*>/ig, "\n").replace(/<\/p>| {2}/ig, "").replace("(", "").replace(")", "")
        .replace("\"", "").replace("'", "").replace("'", "").replace(";", "").trim();
}
