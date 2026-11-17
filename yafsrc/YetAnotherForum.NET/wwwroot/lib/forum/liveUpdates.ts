import type { PostPublishedInfo } from './notificationHub';

export type { PostPublishedInfo };

/**
 * Calls the handler for the published posts (real-time updates) the filter accepts,
 * a burst of posts calls it once (with all posts of the burst).
 */
export function onPostPublished(
	handler: (posts: PostPublishedInfo[]) => Promise<void> | void,
	filter: (post: PostPublishedInfo) => boolean = () => true,
	delay = 1500): void {
	let pending: PostPublishedInfo[] = [];
	let timer: number | undefined;
	let running = false;

	const run = async () => {
		timer = undefined;

		if (running) {
			// the next run starts when this one is done
			timer = window.setTimeout(run, delay);
			return;
		}

		const posts = pending;
		pending = [];
		running = true;

		try {
			await handler(posts);
		} catch (error) {
			console.error(error);
		} finally {
			running = false;
		}
	};

	document.addEventListener('yaf:postPublished', event => {
		const post = (event as CustomEvent<PostPublishedInfo>).detail;

		if (!post || !filter(post)) {
			return;
		}

		pending.push(post);

		if (timer === undefined) {
			timer = window.setTimeout(run, delay);
		}
	});
}

/**
 * Loads html of a page handler, null if there is no content (or the request failed).
 */
export async function fetchHtml(url: string): Promise<{ html: string, response: Response } | null> {
	const response = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });

	if (!response.ok || response.status === 204) {
		return null;
	}

	const html = (await response.text()).trim();

	return html ? { html, response } : null;
}

/**
 * Parses html to elements.
 */
export function toElements(html: string): HTMLElement[] {
	const template = document.createElement('template');
	template.innerHTML = html;

	return Array.from(template.content.children) as HTMLElement[];
}

/**
 * Adds query parameters to a url.
 */
export function withParams(url: string, params: Record<string, string | number | undefined>): string {
	const query = Object.entries(params)
		.filter(([, value]) => value !== undefined && value !== '')
		.map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
		.join('&');

	if (!query) {
		return url;
	}

	return `${url}${url.includes('?') ? '&' : '?'}${query}`;
}

/**
 * Highlights a new or changed element for a moment.
 */
export function highlight(element: Element | null | undefined): void {
	if (!element) {
		return;
	}

	element.classList.remove('yaf-live-new');

	// restart the animation
	void (element as HTMLElement).offsetWidth;

	element.classList.add('yaf-live-new');
	element.addEventListener('animationend', () => element.classList.remove('yaf-live-new'), { once: true });
}
