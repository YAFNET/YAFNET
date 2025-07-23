import * as bootstrap from 'bootstrap';
import * as bootbox from '@w8tcha/bootbox';
import { Options } from 'choices.js';
import { Exception } from 'sass';
import { ReturnParams } from './interfaces/ReturnParams';


export function createTagsSelectTemplates(this: any, template: (templateString: string) => string) {
	const config: Options = this.config;
	const removeItemButton = config.removeItemButton;

	return {
		item({ classNames }: { classNames: { item: string; highlightedState: string; itemSelectable: string; button: string } }, data: { label: string; customProperties?: string; highlighted?: boolean; id: string; value: string; active?: boolean; disabled?: boolean }) {
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
					: classNames.itemSelectable)} badge text-bg-primary fs-6 m-1"
                          data-item data-id="${String(data.id)}" data-value="${String(data.value)}"
                          ${String(removeItemButton ? 'data-deletable' : '')}
                          ${String(data.active ? 'aria-selected="true"' : '')} ${String(data.disabled
					? 'aria-disabled="true"'
					: '')}>
                        <i class="fas fa-tag align-middle me-1"></i>${String(label)}
                        ${String(removeItemButton ? `<button type="button" class="${String(classNames.button)}" aria-label="Remove item: '${String(data.value)}'" data-button="">Remove item</button>` : '')}
                     </div>
                    `
			);
		}
	};
}

export function createForumSelectTemplates(this: any, template: (html: string) => string) {
	const config: Options = this.config;
	const itemSelectText = config.itemSelectText;

	return {
		item({ classNames }: { classNames: Record<string, string> }, data: { highlighted: boolean; placeholder: boolean; id: string; value: string; active: boolean; disabled: boolean; label: string }) {
			return template(
				`
                                 <div class="${String(classNames.item)} ${String(data.highlighted
					? classNames.highlightedState
					: classNames.itemSelectable)} ${String(data.placeholder ? classNames.placeholder : '')}"
                                      data-item data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(data.active ? 'aria-selected="true"' : '')} ${String(data.disabled
					? 'aria-disabled="true"'
					: '')}>
                                    <span><i class="fas fa-comments text-secondary me-1"></i>${String(data.label)
				}</span>
                                 </div>
                                `
			);
		},
		choice({ classNames }: { classNames: Record<string, string> }, data: { disabled: boolean; id: string; value: string; groupId: number; label: string }) {
			return template(
				`
                                 <div class="${String(classNames.item)} ${String(classNames.itemChoice)} ${String(
					data.disabled ? classNames.itemDisabled : classNames.itemSelectable)}"
                                      data-select-text="${String(itemSelectText)}" data-choice ${String(data.disabled
					? 'data-choice-disabled aria-disabled="true"'
					: 'data-choice-selectable')}
                                      data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(data.groupId > 0 ? 'role="treeitem"' : 'role="option"')}>
                                      <span><i class="fas fa-comments text-secondary me-1"></i>${String(
					data.label)}</span>
                                 </div>
                                 `
			);
		},
		choiceGroup({ classNames }: { classNames: Record<string, string> }, data: { disabled: boolean; id: string; value: string; groupId: number; label: string }) {
			return template(`
                     <div class="${String(classNames.item)} fw-bold text-secondary"
                          data-select-text="${String(itemSelectText)}" data-choice ${String(data.disabled
					? 'data-choice-disabled aria-disabled="true"'
					: 'data-choice-selectable')}
                          data-id="${String(data.id)}" data-value="${String(data.value)}"
                          ${String(data.groupId > 0 ? 'role="treeitem"' : 'role="option"')}>
                          <span><i class="fas fa-folder text-warning me-1"></i>${String(data.label)}</span>
                     </div>
                     `);
		}
	};
}



export function loadForumChoiceOptions(params: ReturnParams, url:string, selectedForumId: string) {
	return fetch(
		url,
		{
			method: 'POST',
			body: JSON.stringify(params),
			headers: {
				"RequestVerificationToken": (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value,
				"Accept": 'application/json',
				"Content-Type": 'application/json;charset=utf-8'
			}
		}).then(response => response.json())
		.then(data => data.results.map((group: any) => ({
			value: group.id,
			label: group.text,
			choices: group.children.map((forum: any) => {
				const selectedId = parseInt(selectedForumId);
				return {
					value: forum.id,
					label: forum.text,
					selected: selectedId > 0 && selectedForumId == forum.id,
					customProperties: {
						page: params.Page,
						total: data.total,
						url: forum.url
					}
				};
			})
		})));
}

export function loadChoiceOptions(params: ReturnParams, url:string) {
	return fetch(
		url,
		{
			method: 'POST',
			body: JSON.stringify(params),
			headers: {
				"RequestVerificationToken": (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value,
				"Accept": 'application/json',
				"Content-Type": 'application/json;charset=utf-8'
			}
		}).then(response => response.json())
		.then(data => data.results.map((result: any) => ({
			value: result.id,
			label: result.text,
			customProperties: {
				page: params.Page,
				total: data.total
			}
		})));
}

export function errorLog(x: Exception): void {
    console.error('An Error has occurred!', x);
}

export function empty(wrap: HTMLElement): void {
	while (wrap.firstChild) {
		wrap.removeChild(wrap.firstChild);
	}
}

export function wrap(el: HTMLElement, wrapper: HTMLElement): void {
	el.parentNode?.insertBefore(wrapper, el);
	wrapper.appendChild(el);
}

// Attachments/Albums Popover Preview
export function renderAttachPreview(previewClass: string): void {
	const attachElements = document.querySelectorAll(previewClass);
	attachElements.forEach((attach) => {
		return new bootstrap.Popover(attach, {
			html: true,
			trigger: 'hover',
			placement: 'bottom',
			content: (): string => `<img src="${(attach as HTMLImageElement).src}" class="img-fluid" />`
		});
	});
}

// Toggle password visibility
export function togglePassword(): void {
	const passwordToggle = document.getElementById('PasswordToggle');
	if (passwordToggle && document.body.contains(passwordToggle)) {
		const icon = passwordToggle.querySelector('i') as HTMLElement;
		const pass = document.querySelector("input[id*='Password']") as HTMLInputElement;
		passwordToggle.addEventListener('click', (event): void => {
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
document.addEventListener('click', (event) => {
	const target = event.target as HTMLElement;
	if (target.parentElement && target.parentElement.matches('[data-bs-toggle="confirm"]')) {
		event.preventDefault();
		const button = target.parentElement as HTMLElement;

		const text = button.dataset.title || '',
			yes = button.dataset.yes || '',
			no = button.dataset.no || '',
			title = button.innerHTML;

		button.innerHTML = "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";

		bootboxConfirm(button, title, text, yes, no, (confirmed: boolean) => {
			if (confirmed) {
				button.click();
			}
		});
	}
}, false);

export function bootboxConfirm (button: HTMLElement, title: string, message: string, yes: string, no: string, callback: (result: boolean) => void)  {
	const options: any = {
		message: message,
		centerVertical: true,
		title: title
	};
	options.buttons = {
		cancel: {
			label: `<i class="fa fa-times"></i> ${no}`,
			className: 'btn-danger',
			callback(result: any) {
				callback(false);
				button.innerHTML = title;
			}
		},
		main: {
			label: `<i class="fa fa-check"></i> ${yes}`,
			className: 'btn-success',
			callback(result: any) {
				callback(true);
			}
		}
	};
	bootbox.dialog(options);
};
export function bootboxShareTopic(title: string, message: string, cancel: string, okay: string, value: string) {
	const options: any = {
		message: message,
		centerVertical: true,
		title: title,
		value: value
	};
	options.buttons = {
		cancel: {
			label: cancel
		},
		confirm: {
			label: okay
		}
	};
	options.callback = () => {
		copyToClipBoard(value);
	};
	bootbox.prompt(options);
};

export function copyToClipBoard(input: string): void {
	navigator.clipboard.writeText(input);
}

const _global = (window /* browser */ || global /* node */) as any;

_global.createTagsSelectTemplates = createTagsSelectTemplates;
_global.createForumSelectTemplates = createForumSelectTemplates;
_global.loadForumChoiceOptions = loadForumChoiceOptions;
_global.loadChoiceOptions = loadChoiceOptions;
_global.errorLog = errorLog; 
_global.bootboxShareTopic = bootboxShareTopic;
_global.togglePassword = togglePassword;
_global.copyToClipBoard = copyToClipBoard;