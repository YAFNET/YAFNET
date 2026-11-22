import './admin-edit-form';

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

// Admin > Users: the bulk actions of the selected users, the bar shows the number of the selected users and the
// dialogs the names (only the users of the shown page can be selected)
document.addEventListener('DOMContentLoaded', () => {
	const bar = document.getElementById('BulkBar');

	if (!bar) {
		return;
	}

	const boxes = () => Array.from(document.querySelectorAll<HTMLInputElement>('.bulk-select'));
	const selected = () => boxes().filter(box => box.checked);
	const count = bar.querySelector<HTMLElement>('[data-bulk-count]');
	const master = document.getElementById('BulkSelectAll') as HTMLInputElement | null;

	const update = () => {
		const checked = selected();
		const text = (count?.dataset.template ?? '{0}').replace('{0}', checked.length.toString());

		bar.hidden = checked.length === 0;

		if (count) {
			count.textContent = text;
		}

		document.querySelectorAll<HTMLElement>('[data-bulk-summary]').forEach(summary => summary.textContent = text);

		document.querySelectorAll<HTMLElement>('[data-bulk-names]').forEach(list => {
			list.replaceChildren(...checked.map(box => {
				const item = document.createElement('li');

				item.textContent = box.dataset.userName ?? '';

				return item;
			}));
		});

		if (master) {
			const all = boxes();

			master.checked = all.length > 0 && checked.length === all.length;
			master.indeterminate = checked.length > 0 && checked.length < all.length;
			master.disabled = all.length === 0;
		}
	};

	// the switch "select all" sets the boxes without a change event of them
	document.addEventListener('change', event => {
		const target = event.target as HTMLElement;

		if (target.matches('.bulk-select, #BulkSelectAll')) {
			update();
		}
	});

	bar.querySelector('[data-bulk-clear]')?.addEventListener('click', () => {
		boxes().forEach(box => box.checked = false);
		update();
	});

	update();
});
