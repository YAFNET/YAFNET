// JS Dependencies: Popper, Bootstrap
import '@popperjs/core';
import * as bootstrap from 'bootstrap';
import * as signalR from '@microsoft/signalr';
import * as Utilities from './forum/utilities';
import * as Attachments from './forum/attachments';
import * as Albums from './forum/albums';
import * as Notifications from './forum/notifications';
import Choices from './choices/assets/scripts/choices.js';
import Lightbox from './bs5-lightbox/index.js';
import './prism.js';
import Notify from '@w8tcha/bootstrap-notify';

import * as bootbox from '@w8tcha/bootbox';
import './forum/bootstrap-touchspin';
import './forum/hoverCard';
import 'long-press-event';

import './forum/similarTitles';
import './forum/paging';
import './forum/modals';
import './forum/notificationHub';
import './forum/contextMenu';
import './form-serialize/index.js';

const _global = (window /* browser */ || global /* node */) as any;

_global.bootstrap = bootstrap;
_global.Choices = Choices;
_global.Notify = Notify;
_global.loadSelectMenus = loadSelectMenus;

// Generic Functions
document.addEventListener('DOMContentLoaded', () => {
    const loginButton = document.querySelector<HTMLElement>('a.btn-login,input.btn-login, .btn-spinner')!;
    if (loginButton) {
        loginButton.addEventListener('click', (event) => {
            const button = event.target as HTMLElement;
            button.innerHTML = "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";
            button.classList.add('disabled');
        });
    }

    // Gallery
    document.querySelectorAll<HTMLElement>('[data-toggle="lightbox"]').forEach(element => {
	    element.addEventListener('click', Lightbox.initialize);
    });

    loadSelectMenus();

    const postAttachmentListPlaceholder = document.getElementById('PostAttachmentListPlaceholder');
    if (postAttachmentListPlaceholder !== null) {
        const pageSize = 5;
        const pageNumber = 0;
        Attachments.getPaginationData(pageSize, pageNumber, false);
    }

    // Render Album Images DropDown
    const postAlbumsListPlaceholder = document.getElementById('PostAlbumsListPlaceholder');
    if (postAlbumsListPlaceholder !== null) {
        const pageSize = 5;
        const pageNumber = 0;
        Albums.getAlbumImagesData(pageSize, pageNumber, false);
    }

    // Notify dropdown
    const dropdownNotify = document.querySelector('.dropdown-notify');
    if (dropdownNotify) {
        dropdownNotify.addEventListener('show.bs.dropdown', () => {
            const pageSize = 5;
            const pageNumber = 0;
            Notifications.getNotifyData(pageSize, pageNumber, false);
        });
    }

    _global.Prism.highlightAll();

    Utilities.renderAttachPreview('.attachments-preview');

    document.querySelectorAll<HTMLElement>('.thanks-popover').forEach(thanks => {
        const popover = new bootstrap.Popover(thanks, {
            template: '<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body popover-body-scrollable"></div></div>'
        });

        thanks.addEventListener('show.bs.popover', () => {
            const messageId = thanks.dataset.messageid as string;

            fetch(`/api/ThankYou/GetThanks/${messageId}`, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json;charset=utf-8',
                    'RequestVerificationToken': (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value
                }
            })
                .then(res => res.json())
                .then(response => {
                    const popoverList = document.getElementById(`popover-list-${messageId}`);
                    if (popoverList) {
                        popoverList.innerHTML = response.thanksInfo;
                    }
                });
        });
    });

    document.querySelectorAll<HTMLElement>('.attachedImage').forEach(imageLink => {
	    var parentNode = (imageLink.parentNode as HTMLElement);
        const messageId = parentNode?.id as string;

        imageLink.setAttribute('data-gallery', `gallery-${messageId}`);
    });

    const scrollToTopBtn = document.querySelector<HTMLButtonElement>('.btn-scroll');
    const rootElement = document.documentElement;

    if (scrollToTopBtn) {
	    function handleScroll(): void {
		    const scrollTotal = rootElement.scrollHeight - rootElement.clientHeight;
		    if ((rootElement.scrollTop / scrollTotal) > 0.15) {
			    // Show button
			    scrollToTopBtn!.classList.add('show-btn-scroll');
		    } else {
			    // Hide button
			    scrollToTopBtn!.classList.remove('show-btn-scroll');
		    }
	    }

	    function scrollToTop(e: MouseEvent): void {
		    e.preventDefault();

		    // Scroll to top logic
		    rootElement.scrollTo({
			    top: 0,
			    behavior: 'smooth'
		    });
	    }

	    scrollToTopBtn.addEventListener('click', scrollToTop);
	    document.addEventListener('scroll', handleScroll);
    }
});

function loadSelectMenus(): void {
    document.querySelectorAll<HTMLElement>('[data-bs-toggle="tooltip"]').forEach(toolTip => {
        return new bootstrap.Tooltip(toolTip);
    });

    document.querySelectorAll<HTMLElement>('.dropdown-menu a.dropdown-toggle').forEach(menu => {
        menu.addEventListener('click', (event: MouseEvent) => {
            const $el = menu as HTMLElement;
            const $subMenu = $el.nextElementSibling as HTMLElement;

            document.querySelectorAll<HTMLElement>('.dropdown-menu .show').forEach(dropDownMenu => {
                dropDownMenu.classList.remove('show');
            });

            if ($subMenu) {
                $subMenu.classList.add('show');

                $subMenu.style.top = `${$el.offsetTop - 10}px`;
                $subMenu.style.left = `${$el.offsetWidth - 4}px`;
            }

            event.stopPropagation();
        });
    });

    document.querySelectorAll<HTMLElement>('.yafnet .select2-select').forEach(select => {
        const choice = new Choices(select, {
            allowHTML: true,
            shouldSort: false,
            placeholderValue: select.getAttribute('placeholder') || '',
            classNames: { containerOuter: ['choices', 'w-100'] }
        });
    });



    document.querySelectorAll<HTMLSelectElement>('.yafnet .select2-image-select').forEach(select => {
        var selectedValue = select.value;
        var groups = new Array();
        document.querySelectorAll<HTMLOptionElement>('.yafnet .select2-image-select option[data-category]').forEach(option => {
            var group = option.dataset.category!.trim();

            if (groups.indexOf(group) === -1) {
                groups.push(group);
            }
        });

        groups.forEach(group => {
            document.querySelectorAll('.yafnet .select2-image-select').forEach(s => {

                var optionGroups = new Array();
                s.querySelectorAll(`option[data-category='${group}']`).forEach(option => {
                    if (optionGroups.indexOf(option) === -1) {
                        optionGroups.push(option);
                    }
                });

                const optionGroupElement = document.createElement('optgroup');
                optionGroupElement.label = group;

                optionGroups.forEach(option => {
                    option.replaceWith(optionGroupElement);

                    optionGroupElement.appendChild(option);
                });
            });
        });
        select.value = selectedValue;

        const choice = new Choices(select,
            {
                classNames: { containerOuter: ['choices', 'w-100'] },
                allowHTML: true,
                shouldSort: false,
                removeItemButton: select.dataset.allowClear === 'true',
                placeholderValue: select.getAttribute('placeholder'),
                callbackOnCreateTemplates: function (template: (html: string) => string) {
                    var itemSelectText = this.config.itemSelectText;
                    const removeItemButton = this.config.removeItemButton;

                    return {
                        item({ classNames }: { classNames: { item: string; highlightedState: string; itemSelectable: string; button: string; placeholder: string } }, data: { label: string; customProperties?: string; placeholder: string;highlighted?: boolean; id: string; value: string; active?: boolean; disabled?: boolean }) {
		                    let label = data.label;

		                    if (data.customProperties) {
			                    let json: any;
			                    try {
				                    json = JSON.parse(data.customProperties);
			                    } catch (e) {
				                    json = data.customProperties;
			                    }

			                    label = json.label === undefined ? data.label : json.label;
		                    }

		                    return template(
			                    `
                                 <div class="${String(classNames.item)} ${String(data.highlighted
				                    ? classNames.highlightedState
				                    : classNames.itemSelectable)} ${String(data.placeholder ? classNames.placeholder : '')}"
                                      data-item data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(removeItemButton ? 'data-deletable' : '')}
                                      ${String(data.active ? 'aria-selected="true"' : '')} ${String(data.disabled
				                    ? 'aria-disabled="true"'
				                    : '')}>
                                    ${String(label)}
                                    ${String(removeItemButton
				                    ? `<button type="button" class="${String(classNames.button)
				                    }" aria-label="Remove item: '${String(data.value)
				                    }'" data-button="">Remove item</button>`
				                    : '')}
                                 </div>
                                `
		                    );
	                    },
                        choice({ classNames }: { classNames: Record<string, string> }, data: { disabled: boolean; id: string; value: string; groupId: number; label: string, customProperties: string }) {
                            let label = data.label;

                            if (data.customProperties) {
	                            let json: any;
	                            try {
                                    json = JSON.parse(data.customProperties);
                                } catch (e) {
                                    json = data.customProperties;
                                }

                                label = json.label === undefined ? data.label : json.label;
                            }

                            return template(
                                `
                                 <div class="${String(classNames.item)} ${String(classNames.itemChoice)} ${String(
                                    data.disabled ? classNames.itemDisabled : classNames.itemSelectable)}"
                                      data-select-text="${String(itemSelectText)}" data-choice ${String(data.disabled
                                        ? 'data-choice-disabled aria-disabled="true"'
                                        : 'data-choice-selectable')}
                                      data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(data.groupId > 0 ? 'role="treeitem"' : 'role="option"')}>
                                      ${String(label)}
                                 </div>
                                 `
                            );
                        }
                    };
                }
            });

        choice.passedElement.element.addEventListener('choice',
	        (event: Event) => {
		        const customEvent = event as CustomEvent;

		        if (customEvent.detail?.customProperties) {
			        let json: any;
			        try {
				        json = JSON.parse(customEvent.detail.customProperties);
			        } catch (e) {
				        json = customEvent.detail.customProperties;
			        }

			        if (json.url !== undefined) {
				        window.location.href = json.url;
			        }
		        }
	        }
        );
    });
}