import * as signalR from '@microsoft/signalr';

/**
 * The real-time signal of a published post (ids only, the pages load the html).
 * The page scripts listen to the document event 'yaf:postPublished'.
 */
export interface PostPublishedInfo {
	forumId: number;
	topicId: number;
	messageId: number;
	isNewTopic: boolean;
}

const hasNotifications = document.querySelector('.dropdown-notify') !== null;
const liveUpdates = document.body?.dataset.liveUpdates === 'true';

if (hasNotifications || liveUpdates) {
	const connection = new signalR.HubConnectionBuilder()
		.withUrl('/NotificationHub')
		.withAutomaticReconnect().configureLogging(signalR.LogLevel.Error)
		.build();

	if (hasNotifications) {
		connection.on('newActivityAsync', (alerts: number) => {
			const alert = document.getElementById('notificationAlert') as HTMLElement;
			const notifyLink = document.getElementById('notificationLink') as HTMLElement;

			if (alerts > 0 && notifyLink.classList.contains('d-none')) {
				notifyLink.classList.toggle('d-none');
				alert.classList.toggle('d-none');
			}
		});

		// an achievement was unlocked (e.g. a reaction to a post of the user): the dialog shows it
		connection.on('achievementUnlockedAsync', () => {
			(window as any).yafAchievementUnlocked?.();
		});
	}

	if (liveUpdates) {
		connection.on('postPublishedAsync', (info: PostPublishedInfo) => {
			document.dispatchEvent(new CustomEvent<PostPublishedInfo>('yaf:postPublished', { detail: info }));
		});

		// the groups of a connection are lost on a reconnect (it is a new connection)
		connection.onreconnected(() => {
			connection.invoke('SubscribeAsync').catch(() => { /* no live updates */ });
		});
	}

	connection.start()
		.then(() => liveUpdates ? connection.invoke('SubscribeAsync') : undefined)
		.catch(() => { /* no live updates and notifications without a connection */ });

	document.addEventListener('DOMContentLoaded', () => {
		const alert = document.getElementById('notificationAlert') as HTMLElement;
		const notifyLink = document.getElementById('notificationLink') as HTMLElement;

		if (alert !== null && parseInt(alert.dataset.alerts || '0', 10) > 0) {
			notifyLink.classList.toggle('d-none');
			alert.classList.toggle('d-none');
		}
	});
}
