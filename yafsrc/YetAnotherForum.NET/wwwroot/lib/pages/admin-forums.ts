import Sortable from 'sortablejs';
import './admin-edit-form';

// Admin > Forums: the categories and the forums of one level are sorted with drag and drop,
// every drop saves the new order (the server checks the ids)
document.addEventListener('DOMContentLoaded', () => {
	const form = document.querySelector<HTMLFormElement>('.admin-forums');

	if (!form) {
		return;
	}

	const saveUrl = form.dataset.saveUrl ?? '';
	const token = form.querySelector<HTMLInputElement>('input[name="__RequestVerificationToken"]')?.value ?? '';

	const notify = (message: string, type: 'success' | 'danger') => {
		const notifyClass = (window as any).Notify;

		if (notifyClass) {
			const _ = new notifyClass(
				{ message: message, icon: type === 'success' ? 'fa fa-check' : 'fa fa-exclamation-triangle' },
				{ type: type, element: 'body', placement: { from: 'top', align: 'center' }, delay: 3000 });
		}
	};

	const save = async (type: 'categories' | 'forums', list: HTMLElement) => {
		const ids = Array.from(list.children)
			.map(child => Number((child as HTMLElement).dataset[type === 'categories' ? 'categoryId' : 'forumId']))
			.filter(id => id > 0);

		try {
			const response = await fetch(saveUrl, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
				body: JSON.stringify({ type: type, ids: ids })
			});

			if (!response.ok) {
				throw new Error(response.statusText);
			}

			notify(form.dataset.textSaved ?? '', 'success');
		} catch {
			// the page shows the saved order again
			notify(form.dataset.textError ?? '', 'danger');
			setTimeout(() => window.location.reload(), 1500);
		}
	};

	form.querySelectorAll<HTMLElement>('[data-sortable]').forEach(list => {
		const type = list.dataset.sortable === 'categories' ? 'categories' : 'forums';

		Sortable.create(list, {
			// only within the list: a forum is moved to another parent or category on its edit page
			group: { name: `${type}-${Math.random()}`, pull: false, put: false },
			handle: type === 'categories' ? '.category-drag-handle' : '.forum-drag-handle',
			draggable: type === 'categories' ? '.category-card' : '.forum-node',
			animation: 150,
			ghostClass: 'sortable-ghost',
			chosenClass: 'sortable-chosen',
			onEnd: event => {
				if (event.oldIndex !== event.newIndex) {
					save(type, list);
				}
			}
		});
	});
});

// Admin > Delete forum: the topics are moved to another forum, or they are deleted after the name of the forum
// is typed in (the server checks it again)
document.addEventListener('DOMContentLoaded', () => {
	const form = document.querySelector<HTMLFormElement>('.admin-delete-forum');

	if (!form) {
		return;
	}

	const forumId = form.dataset.forumId ?? '';
	const move = form.querySelector<HTMLInputElement>('[data-delete-move]');
	const moveTarget = form.querySelector<HTMLElement>('[data-delete-move-target]');
	const target = form.querySelector<HTMLInputElement>('[data-delete-target]');
	const confirm = form.querySelector<HTMLElement>('[data-delete-confirm]');
	const confirmName = form.querySelector<HTMLInputElement>('[data-confirm-name]');
	const submit = form.querySelector<HTMLButtonElement>('[data-delete-submit]');

	if (!move || !moveTarget || !target || !confirm || !confirmName || !submit) {
		return;
	}

	const expected = (confirmName.dataset.confirmName ?? '').trim();

	const update = () => {
		const moving = move.checked;

		moveTarget.hidden = !moving;
		confirm.hidden = moving;

		const nameMatches = expected.length > 0 && confirmName.value.trim() === expected;
		const targetChosen = target.value !== '' && target.value !== '0' && target.value !== forumId;

		confirmName.classList.toggle('is-valid', nameMatches);
		submit.disabled = moving ? !targetChosen : !nameMatches;
	};

	move.addEventListener('change', update);
	confirmName.addEventListener('input', update);

	// the forum select (choices.js) writes the chosen forum to the hidden field
	document.getElementById('ForumList')?.addEventListener('choice', () => setTimeout(update));

	form.addEventListener('submit', event => {
		if (submit.disabled) {
			event.preventDefault();
		}
	});

	update();
});
