document.addEventListener('DOMContentLoaded', function () {

    document.querySelectorAll('.list-group-item-menu, .message').forEach(element => {

        var isMessageContext = !!element.classList.contains('message');

        var contextMenu = element.querySelector('.context-menu');

        // rows without a context menu (e.g. the task manager and language lists of the admin)
        if (contextMenu == null) {
            return;
        }

        var messageId = 0;

        if (element.querySelector('.selectionQuoteable') != null) {
            messageId = element.querySelector('.selectionQuoteable').id;
        }

        if (window.matchMedia('only screen and (max-width: 760px)').matches) {
            
            const el = element;

            // listen for the long-press event
            el.addEventListener('long-press',
                function(e) {

                    // stop the event from bubbling up
                    e.preventDefault();

                    if (isMessageContext) {
                        const selectedText = getSelectedMessageText();

                        if (selectedText.length) {
                            addSelectedTextItems(contextMenu, messageId, selectedText);
                        }
                    }


                    showContextMenu(contextMenu, e.detail.pageX, e.detail.pageY);
                });
        }

        element.addEventListener('contextmenu',
            (e) => {
                e.preventDefault();

                // close other
                document.querySelectorAll('.context-menu').forEach(menu => {
                    menu.style.display = 'none';
                    menu.classList.remove('show');
                });

                if (isMessageContext) {
                    const selectedText = getSelectedMessageText();

                    if (selectedText.length) {
                        addSelectedTextItems(contextMenu, messageId, selectedText);
                    }
                }

                showContextMenu(contextMenu, e.pageX, e.pageY);

                return false;
            });

        element.addEventListener('click',
            () => {
                contextMenu.classList.remove('show');
                contextMenu.style.display = 'none';
            });

        // a context menu without links
        contextMenu.querySelector('a')?.addEventListener('click', (e) => {
            var a = e.target;
            if (a.dataset.toggle !== undefined && a.dataset.toggle === 'confirm') {
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
                                className: 'btn-success'
                            },
                            cancel: {
                                label: `<i class="fa fa-times"></i> ${a.dataset.no}`,
                                className: 'btn-danger'
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

            contextMenu.classList.remove('show');
            contextMenu.style.display = 'none';
        });

        contextMenu.addEventListener('click', function (event) {
            // the link or its icon or text is clicked (not the click of the confirmed link)
            const confirmButton = event.target.closest('[data-bs-toggle="confirm"]');

            if (confirmButton && event.isTrusted) {
                event.preventDefault();
                const button = confirmButton,
                    text = button.dataset.title,
                    title = button.innerHTML;

                bootbox.confirm({
                        centerVertical: true,
                        title: title,
                        message: text,
                        buttons: {
                            confirm: {
                                label: `<i class="fa fa-check"></i> ${button.dataset.yes}`,
                                className: 'btn-success'
                            },
                            cancel: {
                                label: `<i class="fa fa-times"></i> ${button.dataset.no}`,
                                className: 'btn-danger'
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

            contextMenu.classList.remove('show');
            contextMenu.style.display = 'none';
        }, false);

        document.querySelector('body').addEventListener('click', () => {
            contextMenu.classList.remove('show');
            contextMenu.style.display = 'none';
        });

    });
});

// shows the menu at the position of the click (the position in the page), the menu is positioned in its
// offset parent (the offset of the event is the one in the clicked element, e.g. a quote of the post)
function showContextMenu(contextMenu, pageX, pageY) {
    contextMenu.style.display = 'block';

    const parent = contextMenu.offsetParent || document.body,
        rect = parent.getBoundingClientRect();

    contextMenu.style.left = `${pageX - (rect.left + window.scrollX + parent.clientLeft)}px`;
    contextMenu.style.top = `${pageY - (rect.top + window.scrollY + parent.clientTop)}px`;

    contextMenu.classList.add('show');
}

function goToURL(messageId, input, url) {
    window.location.href = url + '&q=' + messageId + '&text=' + encodeURIComponent(input);
}

function copyToClipBoard(input)
{
    navigator.clipboard.writeText(input);
}

function searchText(input) {
    const a = document.createElement('a');
    a.target = '_blank';
    a.href = `https://www.google.com/search?q=${encodeURIComponent(input)}`;
    a.click();
}

function getSelectedMessageText() {
    var text = '';
    const sel = window.getSelection();
    if (sel.rangeCount) {
        const container = document.createElement('div');
        for (var i = 0, len = sel.rangeCount; i < len; ++i) {
            container.appendChild(sel.getRangeAt(i).cloneContents());
        }
        text = container.textContent || container.innerText;
    }

    // the text is only used as text (not as html or inside a link), quotes are kept
    return text.replace(/<p[^>]*>/ig, '\n').replace(/<\/p>| {2}/ig, '').trim();
}

/**
 * Adds the menu items for the selected text (quote, copy, search).
 * The text comes from a post (of another user): it is only used as text and as parameter of the
 * click handlers, never as html or inside a "javascript:" link.
 */
function addSelectedTextItems(contextMenu, messageId, selectedText) {
    contextMenu.querySelectorAll('.item-search, .item-selected-quoting, .selected-divider').forEach(item => {
        item.remove();
    });

    const url = contextMenu.dataset.url;

    if (url) {
        addMenuItem(contextMenu, 'item-selected-quoting', 'fa-quote-left', contextMenu.dataset.quote || '',
            () => goToURL(messageId, selectedText, url));
    }

    addMenuItem(contextMenu, 'item-search', 'fa-clipboard', contextMenu.dataset.copy || '',
        () => copyToClipBoard(selectedText));

    addMenuItem(contextMenu, 'item-search', 'fa-search', `${contextMenu.dataset.search || ''} "${selectedText}"`,
        () => searchText(selectedText));
}

function addMenuItem(contextMenu, cssClass, icon, text, onClick) {
    const link = document.createElement('a');
    link.classList.add('dropdown-item', cssClass);
    link.href = '#';

    const iconElement = document.createElement('i');
    iconElement.classList.add('fas', icon);

    link.append(iconElement, ` ${text}`);

    link.addEventListener('click', event => {
        event.preventDefault();
        onClick();
    });

    contextMenu.appendChild(link);
}
