import Choices from './assets/scripts/choices.js';
import * as bootstrap from 'bootstrap';

import { Options } from 'choices.js';
import { ReturnParams } from '../forum/interfaces/ReturnParams';

const _global = (window /* browser */ || global /* node */) as any;

_global.Choices = Choices;

// Generic Functions
document.addEventListener('DOMContentLoaded', () => {
	_global.loadSelectMenus();
});

_global.loadSelectMenus = (): void => {
	document.querySelectorAll<HTMLElement>('[data-bs-toggle="tooltip"]').forEach(toolTip => {
		return new bootstrap.Tooltip(toolTip);
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
						item({ classNames }: { classNames: { item: string; highlightedState: string; itemSelectable: string; button: Array<string>; placeholder: string } }, data: { label: string; customProperties?: string; placeholder: string;highlighted?: boolean; id: string; value: string; active?: boolean; disabled?: boolean }) {
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
										? `<button type="button" class="${String(classNames.button.join(' '))
									}" aria-label="Remove item: '${String(data.value)
									}'" data-button="">Remove item</button>`
									: '')}
                                 </div>
                                `
							);
						},
						choice({ classNames }: { classNames: Record<string, string> }, data: { disabled: boolean; id: string; value: string; groupId: number; label: string; customProperties: string }) {
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


_global.createTagsSelectTemplates = function(this: any, template: (templateString: string) => string) {
	const config: Options = this.config;
	const removeItemButton = config.removeItemButton;

	return {
		item({ classNames }: { classNames: { item: string; highlightedState: string; itemSelectable: string; button: Array<string> } }, data: { label: string; customProperties?: string; highlighted?: boolean; id: string; value: string; active?: boolean; disabled?: boolean }) {
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
                        ${String(removeItemButton ? `<button type="button" class="${String(classNames.button.join(' '))}" aria-label="Remove item: '${String(data.value)}'" data-button="">Remove item</button>` : '')}
                     </div>
                    `
			);
		}
	};
}

_global.createForumSelectTemplates =  function (this: any, template: (html: string) => string) {
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


_global.loadForumChoiceOptions = (params: ReturnParams, url: string, selectedForumId: string) => fetch(
		url,
		{
			method: 'POST',
			body: JSON.stringify(params),
			headers: {
				"RequestVerificationToken": (document.querySelector('input[name="__RequestVerificationToken"]') as
					HTMLInputElement).value,
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

_global.loadChoiceOptions = (params: ReturnParams, url: string) => fetch(
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