import * as bootstrap from 'bootstrap';

const popoverTemplate =
	'<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body"></div></div>';

function initCategoryListWidgets(scope: ParentNode, forumIconLegendContent: string, lastPostTitle: string): void {
	scope.querySelectorAll<HTMLElement>('[data-bs-toggle="tooltip"]').forEach(el => {
		new bootstrap.Tooltip(el);
	});

	scope.querySelectorAll<HTMLElement>('.forum-icon-legend-popvover').forEach(el => {
		new bootstrap.Popover(el, {
			html: true,
			content: forumIconLegendContent,
			trigger: 'focus'
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
	const categoryList = document.getElementById('category-list') as HTMLDivElement;
	const categoryIndexInput = document.getElementById('category-index') as HTMLInputElement;

	if (!categoryList || !categoryIndexInput) {
		return;
	}

	const loadMoreUrl = categoryList.dataset.loadMoreUrl;
	const forumIconLegendContent = categoryList.dataset.forumIconLegendContent || '';
	const lastPostTitle = categoryList.dataset.lastPostTitle || '';

	if (!loadMoreUrl) {
		return;
	}

	let loading = false;
	let observer: IntersectionObserver | null = null;

	// The "N of Total forums shown" alert is rendered by the server as the last
	// element of the current batch, so observing it (instead of measuring scroll
	// position against the page footer) triggers the next load well before the
	// board statistics section comes into view, avoiding the footer-jump flicker.
	function observeMoreButton(): void {
		observer?.disconnect();

		const moreButton = document.getElementById('category-info-more');

		if (!moreButton) {
			return;
		}

		observer = new IntersectionObserver(entries => {
			if (entries[0].isIntersecting && !loading) {
				loadMore();
			}
		}, { rootMargin: '300px' });

		observer.observe(moreButton);
	}

	function loadMore(): void {
		loading = true;

		const categoryIndex = parseInt(categoryIndexInput.value, 10) + 1;
		const separator = loadMoreUrl!.includes('?') ? '&' : '?';
		const url = `${loadMoreUrl}${separator}index=${categoryIndex}`;
		const tokenInput = document.querySelector<HTMLInputElement>('input[name="__RequestVerificationToken"]');

		fetch(url, {
			headers: tokenInput ? { 'RequestVerificationToken': tokenInput.value } : {}
		})
			.then(res => res.text())
			.then(html => {
				categoryList.innerHTML = html;
				categoryIndexInput.value = categoryIndex.toString();

				initCategoryListWidgets(categoryList, forumIconLegendContent, lastPostTitle);

				loading = false;
				observeMoreButton();
			})
			.catch(error => {
				console.error(error);
				loading = false;
			});
	}

	observeMoreButton();
});
