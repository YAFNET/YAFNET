// The admin edit forms: a switch shows (data-shows) or hides (data-hides) the options that depend on it
// (e.g. the moderation options of a forum), the page is not sent to the server for it
document.addEventListener('DOMContentLoaded', () => {
	document.querySelectorAll<HTMLInputElement>('.admin-edit-form input[data-shows], .admin-edit-form input[data-hides]').forEach(input => {
		const update = () => {
			if (input.dataset.shows) {
				document.querySelectorAll<HTMLElement>(input.dataset.shows).forEach(el => el.hidden = !input.checked);
			}

			if (input.dataset.hides) {
				document.querySelectorAll<HTMLElement>(input.dataset.hides).forEach(el => el.hidden = input.checked);
			}
		};

		input.addEventListener('change', update);
		update();
	});
});

// A select with data-set-all sets the value of all selects of the selector (e.g. the access masks of all forums)
document.addEventListener('DOMContentLoaded', () => {
	document.querySelectorAll<HTMLSelectElement>('.admin-edit-form select[data-set-all]').forEach(select => {
		select.addEventListener('change', () => {
			if (select.value === '') {
				return;
			}

			document.querySelectorAll<HTMLSelectElement>(select.dataset.setAll ?? '').forEach(target => target.value = select.value);
		});
	});
});

// A button with data-set-checks sets the switches of its JSON object ({ "#id": true, ... }), e.g. the presets of an access mask
document.addEventListener('DOMContentLoaded', () => {
	document.querySelectorAll<HTMLButtonElement>('.admin-edit-form button[data-set-checks]').forEach(button => {
		button.addEventListener('click', () => {
			const checks = JSON.parse(button.dataset.setChecks ?? '{}') as Record<string, boolean>;

			Object.entries(checks).forEach(([selector, checked]) => {
				const input = document.querySelector<HTMLInputElement>(selector);

				if (input) {
					input.checked = checked;
					input.dispatchEvent(new Event('change', { bubbles: true }));
				}
			});
		});
	});
});

// A button with data-set-values sets the fields of its JSON object ({ "#id": "value", ... }), e.g. the quick durations
// of the board announcement
document.addEventListener('DOMContentLoaded', () => {
	document.querySelectorAll<HTMLButtonElement>('.admin-edit-form button[data-set-values]').forEach(button => {
		button.addEventListener('click', () => {
			const values = JSON.parse(button.dataset.setValues ?? '{}') as Record<string, string>;

			Object.entries(values).forEach(([selector, value]) => {
				const field = document.querySelector<HTMLInputElement | HTMLSelectElement>(selector);

				if (field) {
					field.value = value;
					field.dispatchEvent(new Event('change', { bubbles: true }));
				}
			});
		});
	});
});

// A field with data-preview-html shows its text (html like the board announcement) on the preview element
document.addEventListener('DOMContentLoaded', () => {
	document.querySelectorAll<HTMLTextAreaElement | HTMLInputElement>('.admin-edit-form [data-preview-html]').forEach(field => {
		const preview = document.querySelector<HTMLElement>(field.dataset.previewHtml ?? '');

		if (preview) {
			field.addEventListener('input', () => preview.innerHTML = field.value);
		}
	});
});

// A radio button with data-preview-class sets the class of the preview elements of its JSON object
// ({ "#id": "class prefix" }), the value is added to the prefix (e.g. the type of the board announcement)
document.addEventListener('DOMContentLoaded', () => {
	document.querySelectorAll<HTMLInputElement>('.admin-edit-form input[data-preview-class]').forEach(radio => {
		radio.addEventListener('change', () => {
			if (!radio.checked) {
				return;
			}

			const targets = JSON.parse(radio.dataset.previewClass ?? '{}') as Record<string, string>;

			Object.entries(targets).forEach(([selector, prefix]) => {
				const element = document.querySelector<HTMLElement>(selector);

				if (element) {
					element.className = prefix + radio.value;
				}
			});
		});
	});
});

// A select with data-image-preview shows the image of the selected option (data-image) on the preview image,
// without a selection the preview shows its data-default (e.g. the current logo of the board)
document.addEventListener('DOMContentLoaded', () => {
	document.querySelectorAll<HTMLSelectElement>('.admin-edit-form select[data-image-preview]').forEach(select => {
		const preview = document.querySelector<HTMLImageElement>(select.dataset.imagePreview ?? '');

		if (!preview) {
			return;
		}

		const update = () => {
			const image = select.selectedOptions[0]?.dataset.image;

			preview.src = select.value !== '' && image ? image : preview.dataset.default ?? '';
		};

		select.addEventListener('change', update);
		update();
	});
});

// The style of a role is shown on a preview of the name (the style can have a light and a dark part
// "light!style/dark!style", the preview shows the color of the first part like the names of the forum)
function decodeStyle(style: string): string {
	const parts = style.trim().split('/').map(part => part.split('!')).filter(pair => pair.length > 1);

	if (parts.length === 0) {
		return style;
	}

	return parts[parts.length - 1][1].split(';').find(item => item.toLowerCase().includes('color')) ?? '';
}

document.addEventListener('DOMContentLoaded', () => {
	document.querySelectorAll<HTMLTextAreaElement | HTMLInputElement>('.admin-edit-form [data-style-preview]').forEach(input => {
		const preview = document.querySelector<HTMLElement>(input.dataset.stylePreview ?? '');

		if (!preview) {
			return;
		}

		const update = () => preview.setAttribute('style', decodeStyle(input.value));

		input.addEventListener('input', update);
		update();
	});

	document.querySelectorAll<HTMLInputElement>('.admin-edit-form [data-style-preview-text]').forEach(input => {
		const preview = document.querySelector<HTMLElement>(input.dataset.stylePreviewText ?? '');

		if (!preview) {
			return;
		}

		const fallback = preview.textContent ?? '';

		input.addEventListener('input', () => preview.textContent = input.value.trim() || fallback);
	});
});

// A switch with data-check-all switches all checkboxes of the selector (e.g. the pages of a section), it shows if
// all, none or some of them are checked
document.addEventListener('DOMContentLoaded', () => {
	document.querySelectorAll<HTMLInputElement>('.admin-edit-form input[data-check-all]').forEach(master => {
		const targets = () => Array.from(document.querySelectorAll<HTMLInputElement>(master.dataset.checkAll ?? ''));

		const update = () => {
			const all = targets();
			const checked = all.filter(input => input.checked).length;

			master.checked = all.length > 0 && checked === all.length;
			master.indeterminate = checked > 0 && checked < all.length;
		};

		master.addEventListener('change', () => {
			targets().forEach(input => input.checked = master.checked);
			master.indeterminate = false;
		});

		targets().forEach(input => input.addEventListener('change', update));
		update();
	});
});
