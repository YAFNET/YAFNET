import * as Utilities from '../forum/utilities';

document.addEventListener('DOMContentLoaded', () => {

    document.querySelectorAll<HTMLElement>('.list-group-item-menu, .message').forEach(element => {

        const isMessageContext = element.classList.contains('message');

        const contextMenu = element.querySelector<HTMLElement>('.context-menu')!;

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
                        const searchItem = contextMenu.querySelector('.item-search');
                        const selectedItem = contextMenu.querySelector('.item-selected-quoting');
                        const selectedDivider = contextMenu.querySelector('.selected-divider');

                        searchItem?.remove();
                        selectedItem?.remove();
                        selectedDivider?.remove();

                        if (contextMenu.dataset.url) {
                            const link = document.createElement('a');
                            link.classList.add('dropdown-item', 'item-selected-quoting');
                            link.href = `javascript:goToURL('${messageId}','${selectedText}','${contextMenu.dataset.url} ')`;
                            link.innerHTML = `<i class="fas fa-quote-left"></i>&nbsp;${contextMenu.dataset.quote}`;
                            contextMenu.appendChild(link);
                        }

                        const linkSearch = document.createElement('a');
                        linkSearch.classList.add('dropdown-item', 'item-search');
                        linkSearch.href = `javascript:copyToClipBoard('${selectedText}')`;
                        linkSearch.innerHTML = `<i class="fas fa-clipboard"></i>&nbsp;${contextMenu.dataset.copy}`;
                        contextMenu.appendChild(linkSearch);

                        const divider = document.createElement('div');

                        divider.classList.add('dropdown-divider', 'selected-divider');


                        const linkSelected = document.createElement('a');
                        linkSelected.classList.add('dropdown-item','item-search');
                        linkSelected.href = `javascript:searchText('${selectedText}')`;
                        linkSelected.innerHTML = `<i class="fas fa-search"></i>&nbsp;${contextMenu.dataset.search} "${selectedText}"`;
                        contextMenu.appendChild(linkSelected);
                    }
                }

                contextMenu!.style.left = `${(e as any).detail.pageX}px`;
                contextMenu!.style.top = `${(e as any).detail.pageY}px`;
                contextMenu!.style.display = 'block';
                contextMenu!.classList.add('show');
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
                        const searchItem = contextMenu.querySelector('.item-search'),
                            selectedItem = contextMenu.querySelector('.item-selected-quoting'),
                            selectedDivider = contextMenu.querySelector('.selected-divider');

                        if (searchItem != null) {
                            document.querySelectorAll('.item-search').forEach(item => {
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
                            const link = document.createElement('a');

                            link.classList.add('dropdown-item', 'item-selected-quoting');
                            link.href = `javascript:goToURL('${messageId}','${selectedText}','${contextMenu.dataset.url} ')`;
                            link.innerHTML = `<i class="fas fa-quote-left"></i>&nbsp;${contextMenu.dataset.quote}`;

                            contextMenu.appendChild(link);
                        }

                        const linkSearch = document.createElement('a');

                        linkSearch.classList.add('dropdown-item','item-search');
                        linkSearch.href = `javascript:copyToClipBoard('${selectedText}')`;
                        linkSearch.innerHTML = `<i class="fas fa-clipboard"></i>&nbsp;${contextMenu.dataset.copy}`;

                        contextMenu.appendChild(linkSearch);

                        const divider = document.createElement('div');

                        divider.classList.add('dropdown-divider','selected-divider');

                        const linkSelected = document.createElement('a');

                        linkSelected.classList.add('dropdown-item','item-search');
                        linkSelected.href = `javascript:searchText('${selectedText}')`;
                        linkSelected.innerHTML = `<i class="fas fa-search"></i>&nbsp;${contextMenu.dataset.search} "${selectedText}"`;
                        contextMenu.appendChild(linkSelected);
                    }
                }

                contextMenu.style.display = 'block';
                contextMenu.style.left = e.offsetX + 'px';
                contextMenu.style.top = e.offsetY + 'px';

                contextMenu.classList.add('show');

                return false;
            });

        element.addEventListener('click', () => {
            contextMenu?.classList.remove('show');
            contextMenu!.style.display = 'none';
        });

        contextMenu.addEventListener('click', event => {
	        const targetElement = (event.target as HTMLElement);
            if (targetElement.parentElement && targetElement.parentElement.matches('[data-bs-toggle="confirm"]')) {
		        event.preventDefault();
                var button = (event.target as HTMLElement).parentElement!;
		        const text = button.dataset.title!;
		        const yes = button.dataset.yes!;
		        const no = button.dataset.no!;
		        const title = button.innerHTML!;

		        Utilities.bootboxConfirm(button, title, text, yes, no, r => {
			        if (r) {
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
    return text.replace(/<p[^>]*>/ig, '\n').replace(/<\/p>| {2}/ig, '').replace(/["';()]/g, '');
}

const _global = (window /* browser */ || global /* node */) as any;

_global.goToURL = goToURL;
_global.searchText = searchText;