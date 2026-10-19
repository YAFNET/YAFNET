import * as Utilities from '../forum/utilities';

document.addEventListener('DOMContentLoaded', () => {

    document.querySelectorAll<HTMLElement>('.list-group-item-menu, .message, .row-menu').forEach(element => {

        const isMessageContext = element.classList.contains('message');

        const contextMenu = element.querySelector<HTMLElement>('.context-menu');

        if (!contextMenu) {
	        return;
        }

        let messageId = 0;

        const quoteableElement = element.querySelector('.selectionQuoteable');
        if (quoteableElement != null) {
            messageId = parseInt(quoteableElement.id, 10);
        }

        if (window.matchMedia('only screen and (max-width: 760px)').matches) {

            const el = element;

            // listen for the long-press event
            el.addEventListener('long-press', (e: Event) => {
                e.preventDefault();

                if (isMessageContext && contextMenu) {
                    const selectedText = getSelectedMessageText();

                    if (selectedText.length) {
                        addSelectedTextItems(contextMenu, messageId, selectedText);
                    }
                }

                showContextMenu(contextMenu!, (e as any).detail.pageX, (e as any).detail.pageY);
            });
        }

        element.addEventListener('contextmenu',
            (e) => {
                e.preventDefault();

                // close other
                document.querySelectorAll<HTMLElement>('.context-menu').forEach(menu => {
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

        element.addEventListener('click', () => {
            contextMenu?.classList.remove('show');
            contextMenu!.style.display = 'none';
        });

        contextMenu.addEventListener('click', event => {
	        const button = (event.target as HTMLElement).closest<HTMLElement>('[data-bs-toggle="confirm"]');

	        // the click after the dialog was confirmed is handled by the document (utilities.ts)
            if (button && !button.dataset.confirmed) {
		        event.preventDefault();

		        // only this dialog, not the one of the document
		        event.stopPropagation();

		        const text = button.dataset.title!;
		        const yes = button.dataset.yes!;
		        const no = button.dataset.no!;
		        const title = button.innerHTML!;

		        Utilities.bootboxConfirm(button, title, text, yes, no, r => {
			        if (r) {
				        button.dataset.confirmed = 'true';
				        button.click();
			        }
			        else {
				        contextMenu.classList.remove('show');
				        contextMenu.style.display = 'none';
			        }
		        });

		        contextMenu.classList.remove('show');
		        contextMenu.style.display = 'none';
	        }
        }, false);

        document.querySelector<HTMLBodyElement>('body')!.addEventListener('click', () => {
            contextMenu.classList.remove('show');
            contextMenu.style.display = 'none';
        });

    });
});

// shows the menu at the position of the click (the position in the page), the menu is positioned in its
// offset parent (the offset of the event is the one in the clicked element, e.g. a quote of the post)
function showContextMenu(contextMenu: HTMLElement, pageX: number, pageY: number): void {
    contextMenu.style.display = 'block';

    const parent = (contextMenu.offsetParent as HTMLElement | null) ?? document.body,
        rect = parent.getBoundingClientRect();

    contextMenu.style.left = `${pageX - (rect.left + window.scrollX + parent.clientLeft)}px`;
    contextMenu.style.top = `${pageY - (rect.top + window.scrollY + parent.clientTop)}px`;

    contextMenu.classList.add('show');
}

export function goToURL(messageId: number, input: string, url: string): void {
    window.location.href = `${url}&q=${messageId}&text=${encodeURIComponent(input)}`;
}

export function searchText(input: string): void {
    const a = document.createElement('a');
    a.target = '_blank';
    a.href = `https://www.google.com/search?q=${encodeURIComponent(input)}`;
    a.click();
}

function getSelectedMessageText(): string {
    let text = '';
    const sel: Selection | null = window.getSelection();
    if (sel?.rangeCount) {
        const container = document.createElement('div');
        for (let i = 0; i < sel.rangeCount; ++i) {
            container.appendChild(sel.getRangeAt(i).cloneContents());
        }
        text = container.textContent?.trim() ?? '';
    }
    return text.replace(/<p[^>]*>/ig, '\n').replace(/<\/p>| {2}/ig, '');
}

/**
 * Adds the menu items for the selected text (quote, copy, search).
 * The text comes from a post (of another user): it is only used as text and as parameter of the
 * click handlers, never as html or inside a "javascript:" link.
 */
function addSelectedTextItems(contextMenu: HTMLElement, messageId: number, selectedText: string): void {
    contextMenu.querySelectorAll('.item-search, .item-selected-quoting, .selected-divider').forEach(item => {
        item.remove();
    });

    const url = contextMenu.dataset.url;

    if (url) {
        addMenuItem(contextMenu, 'item-selected-quoting', 'fa-quote-left', contextMenu.dataset.quote ?? '',
            () => goToURL(messageId, selectedText, url));
    }

    addMenuItem(contextMenu, 'item-search', 'fa-clipboard', contextMenu.dataset.copy ?? '',
        () => Utilities.copyToClipBoard(selectedText));

    addMenuItem(contextMenu, 'item-search', 'fa-search', `${contextMenu.dataset.search ?? ''} "${selectedText}"`,
        () => searchText(selectedText));
}

function addMenuItem(contextMenu: HTMLElement, cssClass: string, icon: string, text: string, onClick: () => void): void {
    const link = document.createElement('a');
    link.classList.add('dropdown-item', cssClass);
    link.href = '#';

    const iconElement = document.createElement('i');
    iconElement.classList.add('fas', icon);

    link.append(iconElement, ` ${text}`);

    link.addEventListener('click', event => {
        event.preventDefault();
        onClick();
    });

    contextMenu.appendChild(link);
}

const _global = (window /* browser */ || global /* node */) as any;

_global.goToURL = goToURL;
_global.searchText = searchText;