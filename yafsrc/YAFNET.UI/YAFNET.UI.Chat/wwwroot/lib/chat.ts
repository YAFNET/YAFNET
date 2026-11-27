import * as signalR from '@microsoft/signalr';

type ChatState = 'connecting' | 'connected' | 'reconnecting' | 'offline';

// messages of the same user within this time are one group (ChatRules.GroupTime on the server)
const groupTimeMs = 5 * 60 * 1000;

const list = document.getElementById('ChatMessages') as HTMLElement | null;

if (list) {
	const currentUserId = Number(list.dataset.userId),
		empty = document.getElementById('ChatEmpty'),
		status = document.getElementById('ChatStatus') as HTMLElement,
		form = document.getElementById('ChatForm') as HTMLFormElement | null,
		input = document.getElementById('messageInput') as HTMLInputElement | null,
		sendButton = document.getElementById('sendButton') as HTMLButtonElement | null;

	let connected = false;

	// the times are shown in the time zone and language of the browser (the hub sends UTC)
	const locale = document.documentElement.lang || undefined,
		timeFormat = new Intl.DateTimeFormat(locale, { hour: '2-digit', minute: '2-digit' }),
		dateTimeFormat = new Intl.DateTimeFormat(locale, { dateStyle: 'medium', timeStyle: 'short' }),
		fullFormat = new Intl.DateTimeFormat(locale, { dateStyle: 'full', timeStyle: 'short' });

	const formatTime = (time: HTMLTimeElement): void => {
		const date = new Date(time.dateTime);

		if (Number.isNaN(date.getTime())) {
			return;
		}

		time.textContent = date.toDateString() === new Date().toDateString()
			? timeFormat.format(date)
			: dateTimeFormat.format(date);
		time.title = fullFormat.format(date);
	};

	const isNearBottom = (): boolean => list.scrollHeight - list.scrollTop - list.clientHeight < 80;

	// again in the next frame: messages that arrive together, and the layout after the fonts and avatars are loaded
	const scrollToBottom = (): void => {
		list.scrollTop = list.scrollHeight;
		requestAnimationFrame(() => {
			list.scrollTop = list.scrollHeight;
		});
	};

	const updateSendButton = (): void => {
		if (sendButton && input) {
			sendButton.disabled = !connected || input.value.trim() === '';
		}
	};

	const setState = (state: ChatState): void => {
		status.dataset.state = state;
		status.querySelector('.chat-status-text')!.textContent = status.dataset[`text${state[0].toUpperCase()}${state.slice(1)}`] ?? state;
		connected = state === 'connected';
		updateSendButton();
	};

	const lastMessage = (): HTMLElement | undefined => {
		const messages = list.querySelectorAll<HTMLElement>('.chat-msg');
		return messages[messages.length - 1];
	};

	// the same markup as the page (Areas/Chat/Pages/Index.cshtml), all texts are set as text, not html
	const addMessage = (userId: number, name: string, avatarUrl: string, color: string, message: string, created: string): void => {
		const own = userId === currentUserId,
			previous = lastMessage(),
			createdMs = new Date(created).getTime(),
			continued = !!previous && Number(previous.dataset.userId) === userId
				&& createdMs - new Date(previous.dataset.created ?? '').getTime() >= 0
				&& createdMs - new Date(previous.dataset.created ?? '').getTime() <= groupTimeMs,
			stick = own || isNearBottom();

		const item = document.createElement('div');
		item.className = `chat-msg${own ? ' chat-msg-own' : ''}${continued ? ' chat-msg-continued' : ''}`;
		item.dataset.userId = String(userId);
		item.dataset.created = created;

		if (!own) {
			const avatar = document.createElement('img');
			avatar.className = 'chat-avatar';
			avatar.src = avatarUrl;
			avatar.alt = '';
			avatar.width = 36;
			avatar.height = 36;
			item.appendChild(avatar);
		}

		const body = document.createElement('div');
		body.className = 'chat-msg-body';

		const meta = document.createElement('div');
		meta.className = 'chat-msg-meta';

		if (!own) {
			const nameSpan = document.createElement('span');
			nameSpan.className = 'chat-msg-name';
			nameSpan.textContent = name;
			meta.appendChild(nameSpan);
		}

		const time = document.createElement('time');
		time.dateTime = created;
		meta.appendChild(time);
		formatTime(time);

		const bubble = document.createElement('div');
		bubble.className = 'chat-bubble';
		bubble.style.setProperty('--chat-user-color', color);
		bubble.textContent = message;

		body.appendChild(meta);
		body.appendChild(bubble);
		item.appendChild(body);
		list.appendChild(item);

		empty?.classList.add('d-none');

		// a new message of somebody else does not move the list while the user reads older messages
		if (stick) {
			scrollToBottom();
		}
	};

	const connection = new signalR.HubConnectionBuilder()
		.withUrl(list.dataset.hubUrl || '/AllChatHub')
		.withAutomaticReconnect()
		.configureLogging(signalR.LogLevel.Error)
		.build();

	connection.on('ReceiveMessage', addMessage);
	connection.onreconnecting(() => setState('reconnecting'));
	connection.onreconnected(() => setState('connected'));
	connection.onclose(() => setState('offline'));

	list.querySelectorAll<HTMLTimeElement>('time[datetime]').forEach(formatTime);
	scrollToBottom();

	setState('connecting');

	connection.start()
		.then(() => setState('connected'))
		.catch(err => {
			console.error(err.toString());
			setState('offline');
		});

	const sendMessage = (): void => {
		if (!input || !connected || input.value.trim() === '') {
			return;
		}

		sendButton!.disabled = true;

		// the field is emptied when the message is sent, so the next one can be typed right away
		connection.invoke('SendMessage', input.value)
			.then(() => {
				input.value = '';
				input.focus();
			})
			.catch(err => console.error(err.toString()))
			.finally(updateSendButton);
	};

	// Enter in the field submits the form
	form?.addEventListener('submit', event => {
		event.preventDefault();
		sendMessage();
	});

	input?.addEventListener('input', updateSendButton);
}
