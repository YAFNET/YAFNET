import * as bootstrap from 'bootstrap';
import { fetchHtml, highlight, onPostPublished } from '../forum/liveUpdates';

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

/**
 * Real-time updates: the active discussions and the last posts of the forums are loaded again when a post is published.
 */
function initLiveUpdates(forumIconLegendContent: string, lastPostTitle: string, isLoading: () => boolean): void {
	if (document.body.dataset.liveUpdates !== 'true') {
		return;
	}

	const activeDiscussions = document.getElementById('active-discussions');
	const categoryList = document.getElementById('category-list');

	onPostPublished(async posts => {
		const topics = new Set(posts.map(post => post.topicId.toString()));
		const forums = new Set(posts.map(post => post.forumId.toString()));

		const activeUrl = activeDiscussions?.dataset.refreshUrl;

		if (activeDiscussions && activeUrl) {
			const result = await fetchHtml(activeUrl);

			if (result) {
				activeDiscussions.innerHTML = result.html;

				initCategoryListWidgets(activeDiscussions, forumIconLegendContent, lastPostTitle);

				activeDiscussions.querySelectorAll<HTMLElement>('[data-topic-id]').forEach(item => {
					if (topics.has(item.dataset.topicId!)) {
						highlight(item);
					}
				});
			}
		}

		const categoriesUrl = categoryList?.dataset.refreshUrl;

		// not while more forums are loaded (the list is replaced)
		if (categoryList && categoriesUrl && !isLoading()) {
			const result = await fetchHtml(categoriesUrl);

			if (result) {
				categoryList.innerHTML = result.html;

				initCategoryListWidgets(categoryList, forumIconLegendContent, lastPostTitle);

				categoryList.querySelectorAll<HTMLElement>('[data-forum-id]').forEach(row => {
					if (forums.has(row.dataset.forumId!)) {
						highlight(row.querySelector('.card-post-last') ?? row);
					}
				});

				document.dispatchEvent(new CustomEvent('yaf:categoryListRefreshed'));
			}
		}
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

	let loading = false;

	initLiveUpdates(forumIconLegendContent, lastPostTitle, () => loading);

	if (!loadMoreUrl) {
		return;
	}

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

	// the "N of Total forums shown" alert is new after a refresh of the list
	document.addEventListener('yaf:categoryListRefreshed', () => observeMoreButton());

	observeMoreButton();
});
