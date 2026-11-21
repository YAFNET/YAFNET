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
