function createTagsSelectTemplates(template) {
    const removeItemButton = this.config.removeItemButton;

    return {
        item: function ({ classNames }, data) {
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
                    : classNames.itemSelectable)} badge text-bg-primary fs-6 m-1""
                          data-item data-id="${String(data.id)}" data-value="${String(data.value)}"
                          ${String(removeItemButton ? 'data-deletable' : '')}
                          ${String(data.active ? 'aria-selected="true"' : '')} ${String(data.disabled
                        ? 'aria-disabled="true"'
                        : '')}>
                        <i class="fas fa-fw fa-tag align-middle me-1"></i>${String(label)}
                        ${String(removeItemButton ? `<button type="button" class="${String(classNames.button)}" aria-label="Remove item: '${String(data.value)}'" data-button="">Remove item</button>` : '')}
                     </div>
                    `
            );
        }
    };
}

function createForumSelectTemplates(template) {
    var itemSelectText = this.config.itemSelectText;

    return {
        item: function ({ classNames }, data) {
	        return template(
		        `
                                 <div class="${String(classNames.item)} ${String(data.highlighted
			        ? classNames.highlightedState
			        : classNames.itemSelectable)} ${String(data.placeholder ? classNames.placeholder : '')}"
                                      data-item data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(data.active ? 'aria-selected="true"' : '')} ${String(data.disabled
			        ? 'aria-disabled="true"'
			        : '')}>
                                    <span><i class="fas fa-fw fa-comments text-secondary me-1"></i>${String(data.label)
		        }</span>
                                 </div>
                                `
	        );
        },
        choice: function ({ classNames }, data) {
            return template(
                `
                                 <div class="${String(classNames.item)} ${String(classNames.itemChoice)} ${String(
                    data.disabled ? classNames.itemDisabled : classNames.itemSelectable)}"
                                      data-select-text="${String(itemSelectText)}" data-choice ${String(data.disabled
                        ? 'data-choice-disabled aria-disabled="true"'
                        : 'data-choice-selectable')}
                                      data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(data.groupId > 0 ? 'role="treeitem"' : 'role="option"')}>
                                      <span><i class="fas fa-comments fa-fw text-secondary me-1"></i>${String(
                            data.label)}</span>
                                 </div>
                                 `
            );
        },
        choiceGroup: function ({ classNames }, data) {
	        return template(`
                     <div class="${String(classNames.item)} fw-bold text-secondary"
                          data-select-text="${String(itemSelectText)}" data-choice ${String(data.disabled
                ? 'data-choice-disabled aria-disabled="true"'
                : 'data-choice-selectable')}
                          data-id="${String(data.id)}" data-value="${String(data.value)}"
                          ${String(data.groupId > 0 ? 'role="treeitem"' : 'role="option"')}>
                          <span><i class="fas fa-fw fa-folder text-warning me-1"></i>${String(data.label)}</span>
                     </div>
                     `);
        }
    };
}

function loadForumChoiceOptions(params, url, selectedForumId) {
    return fetch(
        url,
        {
            method: 'POST',
            body: JSON.stringify(params),
            headers: {
                "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value,
                "Accept": 'application/json',
                "Content-Type": 'application/json;charset=utf-8'
            }
        }).then(function (response) {
            return response.json();
        })
        .then(function (data) {
            return data.results.map(function (group) {
                return {
                    value: group.id,
                    label: group.text,
                    choices: group.children.map(function (forum) {
                        const selectedId = parseInt(selectedForumId);
                        return {
                            value: forum.id,
                            label: forum.text,
                            selected: selectedId > 0 && selectedId == forum.id,
                            customProperties: {
                                page: params.Page,
                                total: data.total,
                                url: forum.url
                            }
                        }
                    })
                };
            });
        });
}

function loadChoiceOptions(params, url) {
    return fetch(
        url,
        {
            method: 'POST',
            body: JSON.stringify(params),
            headers: {
                "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value,
                "Accept": 'application/json',
                "Content-Type": 'application/json;charset=utf-8'
            }
        }).then(function (response) {
            return response.json();
        })
        .then(function (data) {
            return data.results.map(function (result) {

                return {
                    value: result.id,
                    label: result.text,
                    customProperties: {
                        page: params.Page,
                        total: data.total
                    }
                };
            });
        });
}

function errorLog(x) {
    console.log('An Error has occurred!');
    console.log(x.responseText);
    console.log(x.status);
}

function wrap(el, wrapper) {
    el.parentNode.insertBefore(wrapper, el);
    wrapper.appendChild(el);
}

function empty(wrap) {
    while (wrap.firstChild) wrap.removeChild(wrap.firstChild);
}

// Attachments/Albums Popover Preview
function renderAttachPreview(previewClass) {
    document.querySelectorAll(previewClass).forEach(attach => {
        return new bootstrap.Popover(attach,
            {
                html: true,
                trigger: 'hover',
                placement: 'bottom',
                content: function () { return `<img src="${attach.src}" class="img-fluid" />`; }
            });
    });
}

// Toggle password visibility
function togglePassword() {

    if (document.body.contains(document.getElementById('PasswordToggle'))) {
        const passwordToggle = document.getElementById('PasswordToggle');
        var icon = passwordToggle.querySelector('i'),
            pass = document.querySelector("input[id*='Password']");
        passwordToggle.addEventListener('click', function (event) {
            event.preventDefault();
            if (pass.getAttribute('type') === 'text') {
                pass.setAttribute('type', 'password');
                icon.classList.add('fa-eye-slash');
                icon.classList.remove('fa-eye');
            } else if (pass.getAttribute('type') === 'password') {
                pass.setAttribute('type', 'text');
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye');
            }
        });
    }
}

// Confirm Dialog
document.addEventListener('click', function (event) {
    if (event.target.parentElement && event.target.parentElement.matches('[data-bs-toggle="confirm"]')) {
        event.preventDefault();
        var button = event.target.parentElement;

        const text = button.dataset.title,
            yes = button.dataset.yes,
            no = button.dataset.no,
            title = button.innerHTML;

        button.innerHTML = "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";

        bootboxConfirm(button, title, text, yes, no, function (r) {
            if (r) {
                button.click();
            }
        });
    }
}, false);

var bootboxConfirm = function (button, title, message, yes, no, callback) {
    const options = {
        message: message,
        centerVertical: true,
        title: title
    };
    options.buttons = {
        cancel: {
            label: `<i class="fa fa-times"></i> ${no}`,
            className: 'btn-danger',
            callback: function (result) {
                callback(false);
                button.innerHTML = title;
            }
        },
        main: {
            label: `<i class="fa fa-check"></i> ${yes}`,
            className: 'btn-success',
            callback: function (result) {
                callback(true);
            }
        }
    };
    bootbox.dialog(options);
};

document.addEventListener('DOMContentLoaded', function () {

    if (document.querySelector('.btn-scroll') != null) {
        // Scroll top button
        var scrollToTopBtn = document.querySelector('.btn-scroll'), rootElement = document.documentElement;

        function handleScroll() {
            const scrollTotal = rootElement.scrollHeight - rootElement.clientHeight;
            if ((rootElement.scrollTop / scrollTotal) > 0.15) {
                // Show button
                scrollToTopBtn.classList.add('show-btn-scroll');
            } else {
                // Hide button
                scrollToTopBtn.classList.remove('show-btn-scroll');
            }
        }

        function scrollToTop(e) {
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

    togglePassword();
})