import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder().withUrl('/allChatHub').configureLogging(signalR.LogLevel.Error).build();

const sendButton = (document.getElementById('sendButton') as HTMLInputElement)!;

//Disable the send button until connection is established.
sendButton.disabled = true;

const currentUserId = Number((document.getElementById('userId') as HTMLInputElement).value);

connection.on('ReceiveMessage', (userId: number, name: string, avatarUrl: string, color: string, message: string, dateTime: string) => {
	// the message is sent to all users, only the sender shows it as their own message
	const isOwn = userId === currentUserId,
		side = isOwn ? 'end' : 'start',
		timeSide = isOwn ? 'start' : 'end';

	const divChat = document.createElement('div');

	divChat.className = `direct-chat-msg ${side}`;

	// Build meta (date/time) bar
	const metaDiv = document.createElement('div');
	metaDiv.className = 'fs-6 mb-1 clearfix';
	const metaSpan = document.createElement('span');
	metaSpan.className = `text-body-secondary float-${timeSide}`;
	metaSpan.textContent = dateTime;
	metaDiv.appendChild(metaSpan);

	// Build avatar img safely
	const avatarImg = document.createElement('img');
	avatarImg.className = 'direct-chat-img img-thumbnail rounded';
	avatarImg.alt = name;
	avatarImg.src = avatarUrl;

	// Build message body
	const msgDiv = document.createElement('div');
	msgDiv.className = isOwn ? 'direct-chat-text text-bg-primary' : 'direct-chat-text';
	if (!isOwn) {
		msgDiv.style.backgroundColor = color;
	}
	msgDiv.textContent = message;

	divChat.appendChild(metaDiv);
	divChat.appendChild(avatarImg);
	divChat.appendChild(msgDiv);

	document.getElementById('divMessage')!.appendChild(divChat);
});

connection.start().then(() => {
	sendButton.disabled = false;
    
}).catch(err => console.error(err.toString()));

const messageInput = document.getElementById('messageInput') as HTMLInputElement;

const sendMessage = (): void => {
	const message = messageInput.value;

	if (sendButton.disabled || message.trim() === '') {
		return;
	}

	// the field is emptied when the message is sent, so the next one can be typed right away
	connection.invoke('SendMessage', message).then(() => {
		messageInput.value = '';
		messageInput.focus();
	}).catch(err => console.log(err.toString()));
};

sendButton.addEventListener('click', event => {
	event.preventDefault();
	sendMessage();
});

messageInput.addEventListener('keydown', event => {
	if (event.key === 'Enter' && !event.isComposing) {
		event.preventDefault();
		sendMessage();
	}
});