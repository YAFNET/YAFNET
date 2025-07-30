import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
	.withUrl('/NotificationHub')
	.withAutomaticReconnect().configureLogging(signalR.LogLevel.Error)
	.build();

connection.on('newActivityAsync', (alerts: number) => {
	const alert = document.getElementById('notificationAlert') as HTMLElement;
	const notifyLink = document.getElementById('notificationLink') as HTMLElement;

	if (alerts > 0 && notifyLink.classList.contains('d-none')) {
		notifyLink.classList.toggle('d-none');
		alert.classList.toggle('d-none');
	}
});

connection.start();

document.addEventListener('DOMContentLoaded', () => {
	const alert = document.getElementById('notificationAlert') as HTMLElement;
	const notifyLink = document.getElementById('notificationLink') as HTMLElement;

	if (alert !== null && parseInt(alert.dataset.alerts || '0', 10) > 0) {
		notifyLink.classList.toggle('d-none');
		alert.classList.toggle('d-none');
	}
});