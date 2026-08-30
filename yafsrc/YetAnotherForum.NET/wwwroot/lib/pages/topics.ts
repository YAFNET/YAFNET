import * as bootstrap from 'bootstrap';

const _global = (window /* browser */ || global /* node */) as any;

const popoverTemplate =
	'<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body"></div></div>';

function initTopicRowWidgets(
	scope: ParentNode,
	iconLegendContent: string,
	topicStarterTitle: string,
	lastPostTitle: string): void {
	scope.querySelectorAll<HTMLElement>('[data-bs-toggle="tooltip"]').forEach(el => {
		new bootstrap.Tooltip(el);
	});

	scope.querySelectorAll<HTMLElement>('.topic-icon-legend-popvover').forEach(el => {
		new bootstrap.Popover(el, {
			html: true,
			content: iconLegendContent,
			trigger: 'focus'
		});
	});

	scope.querySelectorAll<HTMLElement>('.topic-starter-popover').forEach(el => {
		new bootstrap.Popover(el, {
			title: topicStarterTitle,
			html: true,
			trigger: 'hover',
			template: popoverTemplate
		});
	});

	scope.querySelectorAll<HTMLElement>('.topic-link-popover').forEach(el => {
		new bootstrap.Popover(el, {
			title: lastPostTitle,
			html: true,
			trigger: 'hover focus',
			template: popoverTemplate
		});
	});
}

document.addEventListener('DOMContentLoaded', () => {
	const container = document.getElementById('topicsContainer') as HTMLDivElement;
	const sentinel = document.getElementById('topicsLoadMoreSentinel') as HTMLDivElement;

	if (!container || !sentinel) {
		return;
	}

	const iconLegendContent = container.dataset.iconLegendContent || '';
	const topicStarterTitle = container.dataset.topicStarterTitle || '';
	const lastPostTitle = container.dataset.lastPostTitle || '';

	initTopicRowWidgets(container, iconLegendContent, topicStarterTitle, lastPostTitle);

	const loadMoreUrl = container.dataset.loadMoreUrl;
	const pageSize = parseInt(container.dataset.pageSize || '0', 10);
	const show = parseInt(container.dataset.show || '0', 10);
	const mode = container.dataset.mode;
	const total = parseInt(container.dataset.total || '0', 10);
	let loadedCount = parseInt(container.dataset.loadedCount || '0', 10);

	if (!loadMoreUrl || !pageSize || loadedCount >= total) {
		sentinel.remove();
		return;
	}

	let nextPage = parseInt(container.dataset.nextPage || '1', 10);
	let loading = false;

	const spinner = sentinel.querySelector<HTMLElement>('.spinner-border');

	const observer = new IntersectionObserver(entries => {
		if (entries[0].isIntersecting && !loading) {
			loadMore();
		}
	}, { rootMargin: '200px' });

	observer.observe(sentinel);

	function loadMore(): void {
		loading = true;
		spinner?.classList.remove('d-none');

		const separator = loadMoreUrl!.includes('?') ? '&' : '?';
		const modeParam = mode !== undefined ? `&mode=${mode}` : '';
		const url = `${loadMoreUrl}${separator}page=${nextPage}&show=${show}&size=${pageSize}${modeParam}`;

		fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
			.then(res => res.text())
			.then(html => {
				spinner?.classList.add('d-none');
				loading = false;

				if (!html.trim()) {
					observer.disconnect();
					sentinel.remove();
					return;
				}

				const template = document.createElement('template');
				template.innerHTML = html.trim();

				const newRows = Array.from(template.content.children) as HTMLElement[];

				newRows.forEach(row => sentinel.parentElement!.insertBefore(row, sentinel));
				newRows.forEach(row => initTopicRowWidgets(row, iconLegendContent, topicStarterTitle, lastPostTitle));

				nextPage++;
				loadedCount += newRows.length;

				if (newRows.length === 0 || loadedCount >= total) {
					observer.disconnect();
					sentinel.remove();
				}
			})
			.catch(error => {
				console.error(error);
				spinner?.classList.add('d-none');
				loading = false;
				observer.disconnect();
			});
	}
});

_global.initTopicRowWidgets = initTopicRowWidgets;
