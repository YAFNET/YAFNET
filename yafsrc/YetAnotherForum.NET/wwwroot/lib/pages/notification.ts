import * as bootstrap from 'bootstrap';

function initNotificationRowWidgets(scope: ParentNode): void {
	scope.querySelectorAll<HTMLElement>('[data-bs-toggle="tooltip"]').forEach(el => {
		new bootstrap.Tooltip(el);
	});
}

document.addEventListener('DOMContentLoaded', () => {
	const container = document.getElementById('notificationsContainer') as HTMLUListElement;
	const sentinel = document.getElementById('notificationsLoadMoreSentinel') as HTMLLIElement;
	const emptyState = document.getElementById('notificationsEmpty');

	if (!container || !sentinel) {
		return;
	}

	// Note: tooltips for the initially server-rendered rows are already
	// handled by the site-wide [data-bs-toggle="tooltip"] init in forumExtensions.ts.
	// initNotificationRowWidgets() below is only for rows inserted afterwards via AJAX.

	const loadMoreUrl = container.dataset.loadMoreUrl;
	const pageSize = parseInt(container.dataset.pageSize || '0', 10);
	let nextPage = 1;
	let loading = false;
	let observer: IntersectionObserver | null = null;

	const spinner = sentinel.querySelector<HTMLElement>('.spinner-border');

	const filterCheckboxes = document.querySelectorAll<HTMLInputElement>(
		'#notificationFilters input[type="checkbox"]');

	function getFilterParams(): string {
		let params = '';

		filterCheckboxes.forEach(checkbox => {
			params += `&${checkbox.name}=${checkbox.checked}`;
		});

		return params;
	}

	function stopObserving(): void {
		observer?.disconnect();
		observer = null;
		sentinel.classList.add('d-none');
	}

	function observeSentinel(): void {
		stopObserving();
		sentinel.classList.remove('d-none');

		observer = new IntersectionObserver(entries => {
			if (entries[0].isIntersecting && !loading) {
				loadMore();
			}
		}, { rootMargin: '200px' });

		observer.observe(sentinel);
	}

	function appendRows(html: string): HTMLElement[] {
		const template = document.createElement('template');
		template.innerHTML = html.trim();

		const newRows = Array.from(template.content.children) as HTMLElement[];

		newRows.forEach(row => sentinel.parentElement!.insertBefore(row, sentinel));
		newRows.forEach(row => initNotificationRowWidgets(row));

		return newRows;
	}

	function loadMore(): void {
		if (!loadMoreUrl) {
			return;
		}

		loading = true;
		spinner?.classList.remove('d-none');

		const url = `${loadMoreUrl}?page=${nextPage}&size=${pageSize}${getFilterParams()}`;

		fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
			.then(res => res.text())
			.then(html => {
				spinner?.classList.add('d-none');
				loading = false;

				if (!html.trim()) {
					stopObserving();
					return;
				}

				const newRows = appendRows(html);

				nextPage++;

				if (newRows.length < pageSize) {
					stopObserving();
				}
			})
			.catch(error => {
				console.error(error);
				spinner?.classList.add('d-none');
				loading = false;
				stopObserving();
			});
	}

	function reload(): void {
		if (!loadMoreUrl || loading) {
			return;
		}

		loading = true;
		spinner?.classList.remove('d-none');
		sentinel.classList.remove('d-none');

		const url = `${loadMoreUrl}?page=0&size=${pageSize}${getFilterParams()}`;

		fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
			.then(res => res.text())
			.then(html => {
				loading = false;
				spinner?.classList.add('d-none');

				container.querySelectorAll(':scope > li:not(#notificationsLoadMoreSentinel)').forEach(el => el.remove());

				nextPage = 1;

				const trimmed = html.trim();

				if (!trimmed) {
					container.classList.add('d-none');
					emptyState?.classList.remove('d-none');
					stopObserving();
					return;
				}

				emptyState?.classList.add('d-none');
				container.classList.remove('d-none');

				const newRows = appendRows(trimmed);

				if (newRows.length < pageSize) {
					stopObserving();
				} else {
					observeSentinel();
				}
			})
			.catch(error => {
				console.error(error);
				loading = false;
				spinner?.classList.add('d-none');
				stopObserving();
			});
	}

	filterCheckboxes.forEach(checkbox => {
		checkbox.addEventListener('change', reload);
	});

	const loadedCount = parseInt(container.dataset.loadedCount || '0', 10);

	if (!loadMoreUrl || !pageSize || loadedCount < pageSize) {
		stopObserving();
	} else {
		observeSentinel();
	}
});
