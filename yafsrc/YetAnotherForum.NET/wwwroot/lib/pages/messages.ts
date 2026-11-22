import * as signalR from '@microsoft/signalr';
import scrollIntoView from 'smooth-scroll-into-view-if-needed';

const chatHub = new signalR.HubConnectionBuilder().withUrl('/chatHub').configureLogging(signalR.LogLevel.Error).build();

chatHub.start().then(() => {
	registerEvents();
}).catch(err => {
	console.error(err.toString());
});

function currentUserId(): number {
	return parseInt((document.getElementById('UserId') as HTMLInputElement).value);
}

chatHub.on('onConnected', (toUserId: string, conversations: Array<{ body: string, dateTime: string, toUserId: number }>) => {
	const ctrId = `private_${toUserId}`;
	const control = document.getElementById(ctrId) as HTMLElement | null;

	// another conversation was opened meanwhile
	if (!control) {
		return;
	}

	// the whole conversation is loaded (the conversation can be opened again before it was loaded,
	// the messages were shown twice)
	control.querySelector('#divMessage')?.replaceChildren();

	// load existing conversations
	for (let i = 0; i < conversations.length; i++) {
		const avatar = control.dataset.avatar;

		let toAvatar = control.dataset.toAvatar!;
		let side = 'start';
		let msgClass = 'text-bg-light';
		let timeSide = 'end';
		const message = conversations[i].body;
		const dateTime = conversations[i].dateTime;

		if (currentUserId() !== conversations[i].toUserId) {
			side = 'end';
			msgClass = 'text-bg-primary';
			timeSide = 'start';
			toAvatar = avatar!;
		}

		addMessage(ctrId, message, dateTime, side, timeSide, msgClass, toAvatar);
	}
});

// toUserId is the other user of the conversation, fromUserId the author of the message
chatHub.on('sendPrivateMessage', (toUserId: number, message: string, toAvatar: string, dateTime: string, fromUserId?: number) => {
	const ctrId = `private_${toUserId}`;

	let side = 'start';
	let msgClass = 'text-bg-light';
	let timeSide = 'end';

	const isOwn = fromUserId === undefined ? currentUserId() !== toUserId : fromUserId === currentUserId();

	if (isOwn) {
		side = 'end';
		msgClass = 'text-bg-primary';
		timeSide = 'start';
	}

	addMessage(ctrId, message, dateTime, side, timeSide, msgClass, toAvatar);

	updateConversation(toUserId, message, dateTime, isOwn);
});

// a new message in a conversation that is not open, the conversation is marked as unread
chatHub.on('newPrivateMessage', (fromUserId: number) => {
	const li = document.getElementById(`${fromUserId}`);

	if (!li?.classList.contains('chat-list-user')) {
		return;
	}

	let badge = li.querySelector<HTMLSpanElement>('.chat-unread');

	if (!badge) {
		badge = document.createElement('span');
		badge.className = 'chat-unread badge rounded-pill text-bg-danger ms-2 flex-shrink-0';
		badge.textContent = '0';
		li.append(badge);
	}

	badge.textContent = `${parseInt(badge.textContent ?? '0') + 1}`;
	li.classList.add('unread');

	moveToTop(li);
});

// the conversation with the latest message is shown on top of the list
function moveToTop(li: HTMLElement): void {
	const list = li.parentElement;

	if (list && list.firstElementChild !== li) {
		list.prepend(li);
	}
}

// shows the new message as preview of the conversation (in the list) and moves it on top of the list
function updateConversation(userId: number, message: string, dateTime: string, isOwn: boolean): void {
	const li = document.getElementById(`${userId}`);

	if (!li?.classList.contains('chat-list-user')) {
		return;
	}

	const info = li.querySelector<HTMLElement>('.chat-user-info');

	if (info) {
		const html = document.createElement('div');
		html.innerHTML = message;

		let text = (html.textContent ?? '').replace(/\s+/g, ' ').trim();

		if (text.length > 60) {
			text = `${text.substring(0, 57)}...`;
		}

		const you = (document.getElementById('PriChatDiv') as HTMLElement).dataset.you ?? '';

		let preview = info.querySelector<HTMLElement>('.chat-preview');

		if (!preview) {
			preview = document.createElement('small');
			preview.className = 'chat-preview d-block text-secondary text-truncate';
			info.append(preview);
		}

		preview.textContent = isOwn && you ? `${you} ${text}` : text;

		let time = info.querySelector<HTMLElement>('.chat-time');

		if (!time) {
			time = document.createElement('small');
			time.className = 'chat-time text-secondary text-nowrap ms-1';
			info.querySelector('.name')?.parentElement?.append(time);
		}

		time.textContent = dateTime;
	}

	moveToTop(li);
}

function registerEvents(): void {
    const li = document.querySelector('.chat-list-user.active') as HTMLLIElement;

	if (li) {
        addMessageUser(li);
	}
}
function addMessageUser(li: HTMLLIElement): void {
	const avatar = document.getElementsByClassName('img-navbar-avatar')[0].getAttribute('src')!;
	const userId = currentUserId();
    const toUserId = li.id;
	const toUserName = (li.querySelector('.name') as HTMLSpanElement).outerHTML;
	const avatarUrl = (li.querySelector('.img-thumbnail') as HTMLImageElement).src;

	if (userId !== parseInt(toUserId)) {
		const ctrId = `private_${toUserId}`;

		chatHub.invoke('ConnectAsync', avatar, parseInt(toUserId));

		openPrivateChatCard(chatHub,
			toUserId,
			ctrId,
			toUserName,
			avatarUrl,
			avatar);
	}

	// the dialog of the confirm button (utilities.ts) clicks the button again after it was confirmed
	const deleteButton = document.getElementById('deleteConversation') as HTMLButtonElement;

	deleteButton.addEventListener('click', () => {
		if (!deleteButton.dataset.confirmed) {
			return;
		}

        const active = document.querySelector('.chat-list-user.active') as HTMLElement;

		const deleteId = active!.id;

		chatHub.invoke('DeleteConversationAsync', parseInt(deleteId)).finally(() => {
			location.reload();
		});
	});
}

function addMessage(ctrlId: string, message: string, dateTime: string, side: string, timeSide: string, msgClass: string, toAvatar: string): void {
	const divChat: HTMLDivElement = document.createElement('div');
	divChat.className = `direct-chat-msg ${side}`;
	divChat.innerHTML = `<div class="fs-6 mb-1 clearfix"><span class="text-body-secondary float-${timeSide}">${dateTime}</span>
                         </div> <img class="direct-chat-img img-thumbnail rounded" src="${toAvatar}" alt="Message User Image"> <div class="direct-chat-text ${msgClass}">${message}</div>`;

	const messageContainer = document.getElementById(ctrlId)?.querySelector('#divMessage');
	messageContainer?.append(divChat);

	scrollIntoView(divChat);
}

// escapes a text for the use in a html attribute
function escapeAttribute(text: string): string {
	return text.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

function openPrivateChatCard(chatHub: any, userId: string, ctrId: string, userName: string, toAvatarUrl: string, avatarUrl: string): void {

	// Append private chat div inside the main div
	const priChatDiv = document.getElementById('PriChatDiv') as HTMLDivElement;

	const texts = priChatDiv.dataset;

	const placeHolder = texts.placeholder ?? 'Type a message...';

    const $div: HTMLDivElement = document.createElement('div');

    $div.id = ctrId;
    $div.className = 'card direct-chat';
    $div.dataset.toAvatar = toAvatarUrl;
    $div.dataset.avatar = avatarUrl;
    $div.innerHTML = `<div class="card-header"><div class="row justify-content-between align-items-center">
                          <div class="col-auto"><h3 class="card-title">${userName}</h3></div>
                          <div class="col-auto"><button id="deleteConversation" class="btn btn-danger" type="button"
                                  data-bs-toggle="confirm" data-title="${escapeAttribute(texts.confirm ?? '')}"
                                  data-yes="${escapeAttribute(texts.yes ?? '')}" data-no="${escapeAttribute(texts.no ?? '')}"><i class="fas fa-trash"></i><span class="ms-1 d-none d-md-inline">${escapeAttribute(texts.delete ?? '')}</span></button>  </div></div></div>
                      <div class="card-body"> <div id="divMessage" class="direct-chat-messages"></div>  </div>
                      <div class="card-footer">  <div class="input-group mb-0">
                          <textarea rows="2" id="txtPrivateMessage" name="message" placeholder="${escapeAttribute(placeHolder)}" class="form-control"></textarea>
						  <button type="button" id="btnSendMessage" class="btn btn-primary" title="${escapeAttribute(texts.send ?? '')}"><i class="fas fa-paper-plane"></i></button>
                      </div>`;

    // Enter sends the message, Shift+Enter adds a new line (not while an input method composes a text)
    $div.querySelector<HTMLTextAreaElement>('#txtPrivateMessage')?.addEventListener('keydown', (e: KeyboardEvent) => {
		if (e.key === 'Enter' && !e.shiftKey && !e.isComposing) {
			e.preventDefault();
            $div.querySelector<HTMLButtonElement>('#btnSendMessage')?.click();
        }
    });


    if (priChatDiv) {
        priChatDiv.replaceChildren();
        priChatDiv.append($div);
    }

    // Send Button event in Private Chat
    document.getElementById('btnSendMessage')?.addEventListener('click', () => {
		const $text = $div.querySelector<HTMLTextAreaElement>('#txtPrivateMessage');

        if ($text) {
            const msg = $text.value;
            if (msg.trim().length > 0) {
                chatHub.invoke('SendPrivateMessageAsync', parseInt(userId), msg);
                $text.value = '';
            }

            $text.focus();
        }
    });
}

document.querySelectorAll<HTMLLIElement>('.chat-list-user').forEach(user => {
	user.addEventListener('click', () => {
		const activeUser = document.querySelector<HTMLElement>('.chat-list .active');
		if (activeUser) {
			activeUser.classList.remove('active');
		}

		const li = user;

		li.classList.add('active');

		// opening the conversation marks all messages as read
		li.classList.remove('unread');
		li.querySelector('.chat-unread')?.remove();

        addMessageUser(li);
	});
});

// filter the conversation list by the name of the user
document.getElementById('FilterConversations')?.addEventListener('input', (e: Event) => {
	const filter = (e.target as HTMLInputElement).value.trim().toLowerCase();

	document.querySelectorAll<HTMLLIElement>('.chat-list-user').forEach(li => {
		const name = (li.dataset.name ?? '').toLowerCase();

		li.classList.toggle('d-none', filter.length > 0 && !name.includes(filter));
	});
});

// new conversation, find a user (by the user lookup of the mentions) and open the conversation with the user
const findUser = document.getElementById('FindUser') as HTMLInputElement | null;

if (findUser) {
	const results = document.getElementById('FindUserResults') as HTMLDivElement;
	let timer: number | undefined;
	let lastQuery = '';

	document.getElementById('NewConversation')?.addEventListener('shown.bs.collapse', () => {
		findUser.focus();
	});

	findUser.addEventListener('input', () => {
		window.clearTimeout(timer);

		timer = window.setTimeout(() => {
			const query = findUser.value.trim();

			if (query === lastQuery) {
				return;
			}

			lastQuery = query;

			if (query.length < 3) {
				results.replaceChildren();
				return;
			}

			const token = (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement)?.value;

			fetch(`/api/User/GetMentionUsers?users=${encodeURIComponent(query)}`, {
				method: 'GET',
				headers: {
					Accept: 'application/json',
					RequestVerificationToken: token
				}
			}).then(response => response.ok ? response.json() : [])
				.then((users: Array<{ id: number, name: string, avatar: string }>) => {
					// a newer search was started meanwhile
					if (query !== lastQuery) {
						return;
					}

					results.replaceChildren();

					const found = users.filter(u => u.id !== currentUserId());

					if (found.length === 0) {
						const none = document.createElement('div');
						none.className = 'list-group-item text-secondary small';
						none.textContent = findUser.dataset.none ?? '';
						results.append(none);
						return;
					}

					found.forEach(u => {
						const link = document.createElement('a');
						link.className = 'list-group-item list-group-item-action d-flex align-items-center';
						link.href = (findUser.dataset.url ?? '').replace('__USERID__', `${u.id}`);

						const img = document.createElement('img');
						img.className = 'img-thumbnail rounded chat-avatar me-2 flex-shrink-0 object-fit-cover';
						img.src = u.avatar;
						img.alt = '';

						const name = document.createElement('span');
						name.className = 'text-truncate';
						name.textContent = u.name;

						link.append(img, name);
						results.append(link);
					});
				}).catch(() => {
					results.replaceChildren();
				});
		}, 300);
	});
}
