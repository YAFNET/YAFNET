import '@popperjs/core';
import 'bootstrap';

import '@w8tcha/bs5-lightbox';

// Custom JS imports
import DarkEditable from '@w8tcha/dark-editable';

if (document.querySelector('.album-caption')) {
	document.querySelectorAll<HTMLElement>('.album-caption').forEach(el => {
		const _ = new DarkEditable(el);
	});
}

if (document.querySelector('.album-image-caption')) {
	document.querySelectorAll<HTMLElement>('.album-image-caption').forEach(el => {
		const _ = new DarkEditable(el);
	});
}
// The button copies the bb code of an image ([ALBUMIMG]id[/ALBUMIMG]) to the clipboard, it shows "copied" for a moment
document.querySelectorAll<HTMLButtonElement>('.album-copy').forEach(button => {
	button.addEventListener('click', async () => {
		const text = button.dataset.copy ?? '';

		try {
			await navigator.clipboard.writeText(text);
		} catch {
			// no clipboard (e.g. not https): the code is shown to copy it by hand
			window.prompt(button.title, text);
			return;
		}

		const icon = button.innerHTML;
		const title = button.title;

		button.innerHTML = '<i class="fa fa-check"></i>';
		button.title = button.dataset.copied ?? title;
		button.classList.add('btn-success');

		setTimeout(() => {
			button.innerHTML = icon;
			button.title = title;
			button.classList.remove('btn-success');
		}, 1500);
	});
});
