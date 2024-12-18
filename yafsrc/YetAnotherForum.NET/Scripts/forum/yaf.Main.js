﻿// Generic Functions
document.addEventListener('DOMContentLoaded', function () {
    if (document.querySelector('a.btn-login,input.btn-login, .btn-spinner') != null) {
        document.querySelector('a.btn-login,input.btn-login, .btn-spinner').addEventListener('click', () => {
            document.querySelector(this).innerHTML = "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";
        });
    }

    // Gallery
    for (const el of document.querySelectorAll('[data-toggle="lightbox"]')) {
        const lightBox = window.bootstrap.Lightbox;
        el.addEventListener('click', lightBox.initialize);
    }

    // Main Menu
    document.querySelectorAll('.dropdown-menu a.dropdown-toggle').forEach(menu => {
        menu.addEventListener('click', (event) => {
            var $el = menu, $subMenu = $el.nextElementSibling;

            document.querySelectorAll('.dropdown-menu .show').forEach(dropDownMenu => {
                dropDownMenu.classList.remove('show');
            });

            $subMenu.classList.add('show');

            $subMenu.style.top = $el.offsetTop - 10;
            $subMenu.style.left = $el.offsetWidth - 4;

            event.stopPropagation();
        });
    });

    document.querySelectorAll('.yafnet .select2-select').forEach(select => {
	    const choice = new window.Choices(select,
		    {
			    allowHTML: true,
			    shouldSort: false,
			    placeholderValue: select.getAttribute('placeholder'),
			    classNames: {
				    containerOuter: ["choices", "w-100"]
			    }
		    });
    });

    document.querySelectorAll('.yafnet .select2-image-select').forEach(select => {
        var selectedValue = select.value;
        var groups = new Array();
        document.querySelectorAll('.yafnet .select2-image-select option[data-category]').forEach(option => {
            var group = option.dataset.category.trim();

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

        const choice = new window.Choices(select,
	        {
		        classNames: {
			        containerOuter: ["choices", "w-100"]
		        },
		        allowHTML: true,
		        shouldSort: false,
		        removeItemButton: select.dataset.allowClear === 'True',
		        placeholderValue: select.getAttribute('placeholder'),
		        callbackOnCreateTemplates: function(template) {
			        var itemSelectText = this.config.itemSelectText;
			        const removeItemButton = this.config.removeItemButton;

			        return {
				        item: function({ classNames }, data) {
					        var label = data.label;
					        var json;

					        if (data.customProperties) {
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
							        : classNames.itemSelectable)}"
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
				        choice: function({ classNames }, data) {
					        var label = data.label;
					        var json;

					        if (data.customProperties) {
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
	        }
        );

        choice.passedElement.element.addEventListener('choice', function (event) {
            var json;

            if (event.detail.customProperties) {
                try {
                    json = JSON.parse(event.detail.customProperties);
                } catch (e) {
                    json = event.detail.customProperties;
                }

                if (json.url !== undefined) {
                    window.location = json.url;
                }
            }
        });
    });

    if (document.getElementById('PostAttachmentListPlaceholder') != null) {
        const pageSize = 5;
        const pageNumber = 0;
        getPaginationData(pageSize, pageNumber, false);
    }

    // Render Album Images DropDown
    if (document.getElementById('PostAlbumsListPlaceholder') != null) {
        const pageSize = 5;
        const pageNumber = 0;
        getAlbumImagesData(pageSize, pageNumber, false);
    }

    if (document.getElementById('SearchResultsPlaceholder') != null && document.querySelector('.searchInput') != null) {
        document.querySelector('.searchInput').addEventListener('keypress', (e) => {
            var code = e.which;
            if (code === 13) {
                e.preventDefault();
                const pageNumberSearch = 0;
                getSearchResultsData(pageNumberSearch);
            }
        });
    }

    // Notify dropdown
    if (document.querySelector('.dropdown-notify') != null) {
        document.querySelector('.dropdown-notify').addEventListener('show.bs.dropdown',
            () => {
                var pageSize = 5;
                var pageNumber = 0;
                getNotifyData(pageSize, pageNumber, false);
            });
    }

    document.querySelectorAll('.form-check > input').forEach(input => { input.classList.add('form-check-input') });
    document.querySelectorAll('.form-check li > input').forEach(input => { input.classList.add('form-check-input') });
    document.querySelectorAll('.form-check > label').forEach(label => { label.classList.add('form-check-label') });
    document.querySelectorAll('.form-check li > label').forEach(label => { label.classList.add('form-check-label') });

    Prism.highlightAll();

    renderAttachPreview('.attachments-preview');

    document.querySelectorAll('.thanks-popover').forEach(thanks => {
        const popover = new bootstrap.Popover(thanks, {
            template: '<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body popover-body-scrollable"></div></div>'
        });

        thanks.addEventListener('show.bs.popover', () => {
            var messageId = thanks.dataset.messageid;
            var url = thanks.dataset.url;

            fetch(url + '/ThankYou/GetThanks/' + messageId, {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json;charset=utf-8'
                    }
                }).then(res => res.json())
                .then(response => document.getElementById(`popover-list-${messageId}`).innerHTML = response.ThanksInfo);
        });
    });

    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(toolTip => {
        return new bootstrap.Tooltip(toolTip);
    });

    document.querySelectorAll('.attachedImage').forEach(imageLink => {
        var messageId = imageLink.parentNode.id;

        imageLink.setAttribute('data-gallery', `gallery-${messageId}`);
    });

    const quickReplyDialog = document.getElementById('QuickReplyDialog');

    if (quickReplyDialog) {
	    const quickReply = document.getElementById('quickReply');

        moveDialogToCard(quickReplyDialog, quickReply);

        quickReplyDialog.addEventListener('show.bs.modal',
            _ => {
	            const body = quickReply.querySelector('.modal-body'),
                    footer = quickReply.querySelector('.quick-reply-footer');

	            footer.classList.add('modal-footer');
                footer.classList.remove('quick-reply-footer');
                footer.classList.remove('mt-3');

                const copy = quickReplyDialog.querySelector('.modal-content');

		        copy.append(body);
		        copy.append(footer);
            });

        quickReplyDialog.addEventListener('hide.bs.modal',
            _ => {
                moveDialogToCard(quickReplyDialog, quickReply);
	        });
    }

    function moveDialogToCard(quickReplyDialog, quickReply) {
	    const body = quickReplyDialog.querySelector('.modal-body'),
		    footer = quickReplyDialog.querySelector('.modal-footer');

	    footer.classList.add('mt-3');
        footer.classList.add('quick-reply-footer');
	    footer.classList.remove('modal-footer');

	    const copy = quickReply.querySelector('.card-body');

	    copy.append(body);
	    copy.append(footer);
    }

});