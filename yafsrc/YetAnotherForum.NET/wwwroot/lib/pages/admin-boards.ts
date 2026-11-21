// Admin > Boards: a board is only deleted after its name is typed in the delete dialog
// (the name is checked again on the server)
document.addEventListener('DOMContentLoaded', () => {
	document.querySelectorAll<HTMLElement>('.admin-boards-delete').forEach(dialog => {
		const input = dialog.querySelector<HTMLInputElement>('[data-confirm-name]');
		const submit = dialog.querySelector<HTMLButtonElement>('[data-confirm-submit]');

		if (!input || !submit) {
			return;
		}

		const expected = (input.dataset.confirmName ?? '').trim();

		const update = () => {
			const matches = expected.length > 0 && input.value.trim() === expected;

			submit.disabled = !matches;
			input.classList.toggle('is-valid', matches);
		};

		input.addEventListener('input', update);

		// Enter submits only when the name matches
		dialog.querySelector('form')?.addEventListener('submit', event => {
			if (submit.disabled) {
				event.preventDefault();
			}
		});

		dialog.addEventListener('shown.bs.modal', () => input.focus());

		// a closed dialog starts empty again
		dialog.addEventListener('hidden.bs.modal', () => {
			input.value = '';
			update();
		});
	});
});
