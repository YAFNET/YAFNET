import '../prism.js';
import * as Attachments from '../forum/attachments';
import * as Albums from '../forum/albums';
import * as Utilities from '../forum/utilities';
import '../forum/similarTitles';
import Lightbox from '@w8tcha/bs5-lightbox';
import * as bootstrap from 'bootstrap';
import { fetchHtml, highlight, onPostPublished, toElements, withParams } from '../forum/liveUpdates';

const _global = (window /* browser */ || global /* node */) as any;

/**
 * Initializes the scripts of posts added by the real-time updates (the page initializes its posts on load).
 */
function initPostWidgets(scope: HTMLElement, hoverCardDelay: number | undefined): void {
	_global.Prism.highlightAllUnder(scope);

	scope.querySelectorAll<HTMLElement>('.attachments-preview').forEach(attach => {
		new bootstrap.Popover(attach, {
			html: true,
			trigger: 'hover',
			placement: 'bottom',
			content: (): string => `<img src="${(attach as HTMLImageElement).src}" class="img-fluid" />`
		});
	});

	scope.querySelectorAll<HTMLElement>('.attachedImage').forEach(imageLink => {
		const messageId = (imageLink.parentNode as HTMLElement)?.id as string;

		imageLink.setAttribute('data-gallery', `gallery-${messageId}`);
	});

	scope.querySelectorAll<HTMLElement>(Lightbox.defaultSelector).forEach(element => {
		element.addEventListener('click', Lightbox.initialize);
	});

	scope.querySelectorAll<HTMLElement>('[data-bs-toggle="tooltip"]').forEach(element => {
		new bootstrap.Tooltip(element);
	});

	if (hoverCardDelay !== undefined && typeof _global.userCardContent === 'function') {
		scope.querySelectorAll<HTMLElement>('.hc-user').forEach(element => {
			_global.userCardContent(element, hoverCardDelay);
		});
	}

	_global.initContextMenus?.(scope);
}

/**
 * Real-time updates: a reply on the last page shown is added below the posts, otherwise a notice links to it.
 */
function initLivePosts(): void {
	const livePosts = document.getElementById('live-posts');

	if (!livePosts || document.body.dataset.liveUpdates !== 'true') {
		return;
	}

	const topicId = parseInt(livePosts.dataset.topicId || '0', 10);
	const page = parseInt(livePosts.dataset.page || '0', 10);
	const newPostsUrl = livePosts.dataset.newPostsUrl!;
	const hoverCardDelay = livePosts.dataset.hoverCardDelay !== undefined
		? parseInt(livePosts.dataset.hoverCardDelay, 10)
		: undefined;

	let isLastPage = livePosts.dataset.lastPage === 'true';
	let lastMessageId = parseInt(livePosts.dataset.lastMessageId || '0', 10);
	let notice: HTMLElement | null = null;

	const showNotice = (messageId: number) => {
		const url = (livePosts.dataset.postUrl || '').replace('2147483647', messageId.toString());

		if (!notice) {
			notice = document.createElement('div');
			notice.className = 'alert alert-info d-flex align-items-center justify-content-between yaf-live-notice';
			notice.setAttribute('role', 'status');

			const link = document.createElement('a');
			link.className = 'alert-link';
			link.textContent = livePosts.dataset.newReplyText || '';
			notice.appendChild(link);

			const close = document.createElement('button');
			close.type = 'button';
			close.className = 'btn-close';
			close.setAttribute('aria-label', 'Close');
			close.addEventListener('click', () => {
				notice?.remove();
				notice = null;
			});
			notice.appendChild(close);

			livePosts.after(notice);
		}

		notice.querySelector('a')!.href = url;
		highlight(notice);
	};

	onPostPublished(async posts => {
		const newest = Math.max(...posts.map(post => post.messageId));

		if (newest <= lastMessageId) {
			return;
		}

		if (!isLastPage) {
			showNotice(newest);
			return;
		}

		const result = await fetchHtml(withParams(newPostsUrl, { pageIndex: page, after: lastMessageId }));

		if (result?.response.headers.get('X-Topic-More-Pages') === 'true') {
			// the page is full, the next posts are on the next page
			isLastPage = false;
		}

		if (!result) {
			if (!isLastPage) {
				showNotice(newest);
			}

			return;
		}

		const elements = toElements(result.html);

		// the posts are siblings after the posts toolbar, the new ones go after the last post shown
		const lastPost = document.getElementById(`post${lastMessageId}`)?.closest('form');

		let anchor: Element = lastPost ?? livePosts;

		elements.forEach(element => {
			anchor.after(element);
			anchor = element;

			initPostWidgets(element, hoverCardDelay);
			highlight(element.querySelector('.card') ?? element);
		});

		elements.forEach(element => {
			element.querySelectorAll<HTMLElement>('[id^="post"]').forEach(postAnchor => {
				const id = parseInt(postAnchor.id.substring(4), 10);

				if (!isNaN(id) && id > lastMessageId) {
					lastMessageId = id;
				}
			});
		});

		if (!isLastPage) {
			showNotice(newest);
		}
	}, post => post.topicId === topicId);
}

document.addEventListener('DOMContentLoaded', () => {
	_global.Prism.highlightAll();

	initLivePosts();

	const postAttachmentListPlaceholder = document.getElementById('PostAttachmentListPlaceholder');
	if (postAttachmentListPlaceholder !== null) {
		const pageSize = 5;
		const pageNumber = 0;
		Attachments.getPaginationData(pageSize, pageNumber, false);
	}

	// Render Album Images DropDown
	const postAlbumsListPlaceholder = document.getElementById('PostAlbumsListPlaceholder');
	if (postAlbumsListPlaceholder !== null) {
		const pageSize = 5;
		const pageNumber = 0;
		Albums.getAlbumImagesData(pageSize, pageNumber, false);
	}

	Utilities.renderAttachPreview('.attachments-preview');

	document.querySelectorAll<HTMLElement>('.attachedImage').forEach(imageLink => {
		var parentNode = (imageLink.parentNode as HTMLElement);
		const messageId = parentNode?.id as string;

		imageLink.setAttribute('data-gallery', `gallery-${messageId}`);
	});
});