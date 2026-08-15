/// <reference lib="WebWorker" />

// export empty type because of tsc --isolatedModules flag
export type { };
declare const self: ServiceWorkerGlobalScope;

// Service Worker for PWA

// Update 'version' if you need to refresh the cache
var CACHE_NAME = '{version}';
var offlineUrl = '{offlineRoute}';

// Install event
self.addEventListener('install', (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => {
            // @ts-ignore
            return cache.addAll([offlineUrl, {routes}]);
        })
    );
    self.skipWaiting();
});

// Activate event
self.addEventListener('activate', (event) => {
    event.waitUntil(
        caches.keys().then((cacheNames) => {
            return Promise.all(
                cacheNames.map((cacheName) => {
                    if (cacheName !== CACHE_NAME) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
    self.clients.claim();
});

// Fetch event - Network first, then fallback to cache
self.addEventListener('fetch', (event) => {
    const { request } = event;

    // Skip non-GET requests
    if (request.method !== 'GET') {
        return;
    }

    // Skip non-http(s) requests (e.g. chrome-extension:, blob:)
    if (!/^https?:$/i.test(new URL(request.url).protocol)) {
        return;
    }

    // Page navigations are never cached (see offlineFallback) - only static assets are.
    const isNavigation = request.mode === 'navigate';

    event.respondWith(
	    fetch(request)
	    .then((response) => {
		    // Cache successful, same-origin, uncompressed asset responses only.
		    // Navigation (HTML) responses are intentionally excluded: caching a Response
		    // whose Content-Encoding header (e.g. gzip) no longer matches its already-decoded
		    // body causes Safari on iOS to fail rendering the cached copy and offer it as a
		    // file download instead of the page when it's later served from the fetch() catch handler.
		    if (
			    !isNavigation &&
			    response &&
			    response.ok &&
			    response.type === 'basic' &&
			    !response.headers.has('Content-Encoding')
		    ) {
			    const responseClone = response.clone();
			    caches.open(CACHE_NAME).then((cache) => {
				    cache.put(request, responseClone);
			    });
		    }
		    return response;
	    })
	    .catch(() => offlineFallback(request, isNavigation))
    );
});

async function offlineFallback(request: Request, isNavigation: boolean): Promise<Response> {
    // For page navigations, always fall back to the pre-cached offline page rather than a
    // stale cached copy of the request itself.
    if (isNavigation) {
        const offlinePage = await caches.match(offlineUrl);

        if (offlinePage) {
	        return offlinePage;
        }

        return new Response('Oops! - You\'re offline - Please check your internet connection.',
	        {
		        status: 503,
		        statusText: 'Service Unavailable',
		        headers: new Headers({
			        'Content-Type': 'text/html; charset=utf-8'
		        })
	        });
    }

    // For assets, serve the cached copy if we have one.
    const cached = await caches.match(request);

    if (cached) {
	    return cached;
    }

    return new Response('Oops! - You\'re offline - Please check your internet connection.',
        {
	        status: 503,
	        statusText: 'Service Unavailable',
	        headers: new Headers({
		        'Content-Type': 'text/plain'
	        })
        });
}

// Handle push notifications
self.addEventListener('push', (event) => {

    if (!event.data) {
	    return;
    }

    const data: CustomNotificationOptions = event.data.json();

    const unreadCount = data.unreadCount;

    // Set or clear the badge.
    if (navigator.setAppBadge) {
	    if (unreadCount && unreadCount > 0) {
		    navigator.setAppBadge(unreadCount);
	    } else {
		    navigator.clearAppBadge();
	    }
    }

    // Parse push event data if available
	const notification: CustomNotificationOptions = {
		title: data.title,
		body: data.body,
		icon: data.icon,
        badge: data.badge,
		image: data.image,
		// @ts-ignore
		vibrate: [200, 100, 200],
		tag: `push-notification-${Date.now()}`,
        requireInteraction: true,
        silent: false,
        actions: [
	        {
		        action: data.data.url, title: data.data.action
	        },
	        {
                action: 'close', title: data.data.close
	        }
        ]
	};

    event.waitUntil(
        self.registration.showNotification(data.title!, notification)
    );
});

// Handle notification clicks
self.addEventListener('notificationclick', (event) => {
    const action = event.action;

    event.notification.close();

    event.waitUntil(
		// @ts-ignore
        clients.matchAll({ type: 'window', includeUncontrolled: true }).then((clientList) => {
            // Check if app window already exists
            for (let i = 0; i < clientList.length; i++) {
                const client = clientList[i];
                if (client.url === '/' && 'focus' in client) {
                    return client.focus();
                }
            }
            // If not, open new window
            // @ts-ignore
            if (clients.openWindow) {
                if (action && action !== 'close') {
	                // @ts-ignore
	                return clients.openWindow(action);
                };
            }
        })
    );
});

// Handle notification close
self.addEventListener('notificationclose', (event) => {
});

interface CustomNotificationOptions {
	unreadCount?: number;
    actions?: NotificationAction[];
    title?: string;
	badge?: string;
	body?: string;
	data?: any;
	dir?: NotificationDirection;
	icon?: string;
	image?: string;
	lang?: string;
	renotify?: boolean;
	requireInteraction?: boolean;
	silent?: boolean;
	tag?: string;
	timestamp?: number;
	vibrate?: VibratePattern;
}

interface NotificationAction {
	action: string;
	icon?: string;
	title: string;
}