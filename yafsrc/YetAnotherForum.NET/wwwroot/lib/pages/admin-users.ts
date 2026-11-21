// Admin > Users: the delete dialog shows the user of the clicked button (button or context menu)
document.addEventListener('DOMContentLoaded', () => {
	const dialog = document.getElementById('DeleteUserDialog');

	if (!dialog) {
		return;
	}

	dialog.addEventListener('show.bs.modal', event => {
		const button = (event as any).relatedTarget as HTMLElement | null;

		if (!button) {
			return;
		}

		const name = dialog.querySelector<HTMLElement>('[data-delete-user-name]');
		const id = dialog.querySelector<HTMLInputElement>('[data-delete-user-id]');

		if (name) {
			name.textContent = button.dataset.userName ?? '';
		}

		if (id) {
			id.value = button.dataset.userId ?? '0';
		}
	});
});
