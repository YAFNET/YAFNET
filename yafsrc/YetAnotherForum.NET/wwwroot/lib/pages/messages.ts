import * as signalR from '@microsoft/signalr';

const chatHub = new signalR.HubConnectionBuilder().withUrl('/chatHub').configureLogging(signalR.LogLevel.Error).build();

chatHub.start().then(() => {
	registerEvents();
}).catch(err => {
	console.error(err.toString());
});

function currentUserId(): number {
	return parseInt((document.getElementById('UserId') as HTMLInputElement).value);
}

chatHub.on('onConnected', (toUserId: string, conversations: Array<{ body: string, dateTime: string, created: string, fromUserId: number }>) => {
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
	conversations.forEach(message => {
		addMessage(ctrId, message.body, message.dateTime, message.created, message.fromUserId === currentUserId(),
			message.fromUserId, control.dataset.toAvatar ?? '');
	});

	scrollToEnd(ctrId);
});

// toUserId is the other user of the conversation, fromUserId the author of the message,
// created the time in UTC (older servers send only the formatted time)
chatHub.on('sendPrivateMessage', (toUserId: number, message: string, avatar: string, dateTime: string, fromUserId?: number, created?: string) => {
	const ctrId = `private_${toUserId}`;

	const isOwn = fromUserId === undefined ? currentUserId() !== toUserId : fromUserId === currentUserId();

	addMessage(ctrId, message, dateTime, created ?? new Date().toISOString(), isOwn,
		fromUserId ?? (isOwn ? currentUserId() : toUserId), decodeHtml(avatar));

	scrollToEnd(ctrId);

	updateConversation(toUserId, message, dateTime, isOwn);
});

// the message was not sent (the other user does not accept private messages from the user):
// the text stays in the text box and a message is shown
chatHub.on('privateMessageNotSent', (toUserId: number) => {
	const control = document.getElementById(`private_${toUserId}`);
	const text = control?.querySelector<HTMLTextAreaElement>('#txtPrivateMessage');

	if (!control || !text) {
		return;
	}

	if (text.value.length === 0) {
		text.value = control.dataset.pending ?? '';
	}

	let alert = control.querySelector<HTMLDivElement>('#privateMessageNotSent');

	if (!alert) {
		alert = document.createElement('div');
		alert.id = 'privateMessageNotSent';
		alert.className = 'alert alert-warning small py-2 mb-2';
		alert.setAttribute('role', 'alert');
		text.closest('.input-group')?.before(alert);
	}

	alert.textContent = (document.getElementById('PriChatDiv') as HTMLElement).dataset.notSent ?? '';
});

// the number of the unread messages in the header
function changeUnreadTotal(change: number): void {
	const total = document.getElementById('UnreadCount');

	if (!total) {
		return;
	}

	const count = Math.max(0, parseInt(total.textContent ?? '0') + change);

	total.textContent = `${count}`;
	total.classList.toggle('text-bg-danger', count > 0);
	total.classList.toggle('text-bg-secondary', count === 0);
}

// decodes the html encoded text (the avatar url of the hub is html encoded)
function decodeHtml(text: string): string {
	const area = document.createElement('textarea');
	area.innerHTML = text;
	return area.value;
}

// a conversation with a user that is not in the list yet (the first message of the user)
function addConversation(userId: number, name: string, avatar: string): HTMLLIElement | null {
	let list = document.querySelector<HTMLUListElement>('.chat-list');

	if (!list) {
		// the page had no conversations: the list replaces the text
		const body = document.querySelector<HTMLElement>('.my-messages .card .card-body');

		if (!body) {
			return null;
		}

		list = document.createElement('ul');
		list.className = 'list-group list-group-flush chat-list mb-0';
		body.replaceChildren(list);
	}

	const li = document.createElement('li');
	li.id = `${userId}`;
	li.className = 'chat-list-user list-group-item list-group-item-action d-flex align-items-center';
	li.dataset.name = name;

	const img = document.createElement('img');
	img.className = 'img-thumbnail rounded chat-avatar me-2 flex-shrink-0 object-fit-cover';
	img.src = decodeHtml(avatar);
	img.alt = name;

	const info = document.createElement('div');
	info.className = 'chat-user-info flex-grow-1';

	const line = document.createElement('div');
	line.className = 'd-flex justify-content-between align-items-baseline';

	const label = document.createElement('span');
	label.className = 'name text-truncate';
	label.textContent = name;

	line.append(label);
	info.append(line);
	li.append(img, info);
	list.prepend(li);

	bindConversation(li);

	return li;
}

// a new message in a conversation that is not open, the conversation is marked as unread
chatHub.on('newPrivateMessage', (fromUserId: number, name?: string, avatar?: string) => {
	let li = document.getElementById(`${fromUserId}`);

	if (!li?.classList.contains('chat-list-user')) {
		li = name ? addConversation(fromUserId, name, avatar ?? '') : null;
	}

	if (!li) {
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

	changeUnreadTotal(1);

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
	} else {
		// no conversation is open (the user has no conversations): the page is connected to get
		// the first message of another user
		const avatar = document.getElementsByClassName('img-navbar-avatar')[0]?.getAttribute('src') ?? '';

		chatHub.invoke('ConnectAsync', avatar, 0);
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
			avatar,
			li.dataset.profile ?? '',
			li.dataset.lastVisit ?? '');
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

// messages of the same user within this time are one group (like the chat)
const groupTimeMs = 5 * 60 * 1000;

// the times are shown in the time zone and language of the browser
const locale = document.documentElement.lang || undefined,
	timeFormat = new Intl.DateTimeFormat(locale, { hour: '2-digit', minute: '2-digit' }),
	dateTimeFormat = new Intl.DateTimeFormat(locale, { dateStyle: 'medium', timeStyle: 'short' }),
	fullFormat = new Intl.DateTimeFormat(locale, { dateStyle: 'full', timeStyle: 'short' });

// a message of the conversation, the same bubbles as the chat:
// the messages of the other user on the left with the avatar, the own messages on the right
function addMessage(ctrlId: string, message: string, dateTime: string, created: string, isOwn: boolean, fromUserId: number, avatar: string): void {
	const container = document.getElementById(ctrlId)?.querySelector<HTMLElement>('#divMessage');

	if (!container) {
		return;
	}

	const previous = container.lastElementChild as HTMLElement | null,
		date = new Date(created),
		continued = !!previous && Number(previous.dataset.userId) === fromUserId
			&& date.getTime() - new Date(previous.dataset.created ?? '').getTime() >= 0
			&& date.getTime() - new Date(previous.dataset.created ?? '').getTime() <= groupTimeMs;

	const item = document.createElement('div');
	item.className = `chat-msg${isOwn ? ' chat-msg-own' : ''}${continued ? ' chat-msg-continued' : ''}`;
	item.dataset.userId = `${fromUserId}`;
	item.dataset.created = created;

	if (!isOwn) {
		const img = document.createElement('img');
		img.className = 'chat-avatar';
		img.src = avatar;
		img.alt = '';
		img.width = 36;
		img.height = 36;
		item.append(img);
	}

	const body = document.createElement('div');
	body.className = 'chat-msg-body';

	const time = document.createElement('time');
	time.className = 'chat-msg-meta';

	if (Number.isNaN(date.getTime())) {
		time.textContent = dateTime;
	} else {
		time.dateTime = created;
		time.textContent = date.toDateString() === new Date().toDateString() ? timeFormat.format(date) : dateTimeFormat.format(date);
		time.title = fullFormat.format(date);
	}

	// the message is formatted on the server (bb codes)
	const bubble = document.createElement('div');
	bubble.className = 'chat-bubble';
	bubble.innerHTML = message;

	body.append(time, bubble);
	item.append(body);
	container.append(item);
}

// shows the latest message
function scrollToEnd(ctrlId: string): void {
	const container = document.getElementById(ctrlId)?.querySelector<HTMLElement>('#divMessage');

	if (!container) {
		return;
	}

	container.scrollTop = container.scrollHeight;

	// again after the images of the messages are loaded
	requestAnimationFrame(() => {
		container.scrollTop = container.scrollHeight;
	});
}

// escapes a text for the use in a html attribute
function escapeAttribute(text: string): string {
	return text.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

function openPrivateChatCard(chatHub: any, userId: string, ctrId: string, userName: string, toAvatarUrl: string, avatarUrl: string, profileUrl: string, lastVisit: string): void {

	// Append private chat div inside the main div
	const priChatDiv = document.getElementById('PriChatDiv') as HTMLDivElement;

	const texts = priChatDiv.dataset;

	const placeHolder = texts.placeholder ?? 'Type a message...';

    const $div: HTMLDivElement = document.createElement('div');

    $div.id = ctrId;
    $div.className = 'card direct-chat';
    $div.dataset.toAvatar = toAvatarUrl;
    $div.dataset.avatar = avatarUrl;
    const title = profileUrl ? `<a href="${escapeAttribute(profileUrl)}" class="text-reset text-decoration-none">${userName}</a>` : userName;

    $div.innerHTML = `<div class="card-header d-flex align-items-center gap-2">
                          <img class="chat-header-avatar img-thumbnail rounded object-fit-cover flex-shrink-0" src="${escapeAttribute(toAvatarUrl)}" alt="">
                          <div class="flex-grow-1 min-w-0"><h3 class="card-title h6 mb-0 text-truncate">${title}</h3>
                          <small class="text-body-secondary chat-last-visit">${escapeAttribute(lastVisit)}</small></div>
                          <div class="dropdown"><button class="btn btn-sm btn-link text-body-secondary" type="button" data-bs-toggle="dropdown" aria-expanded="false"
                                  title="${escapeAttribute(texts.options ?? '')}" aria-label="${escapeAttribute(texts.options ?? '')}"><i class="fas fa-ellipsis-vertical"></i></button>
                          <ul class="dropdown-menu dropdown-menu-end"><li><button id="deleteConversation" class="dropdown-item text-danger" type="button"
                                  data-bs-toggle="confirm" data-title="${escapeAttribute(texts.confirm ?? '')}"
                                  data-yes="${escapeAttribute(texts.yes ?? '')}" data-no="${escapeAttribute(texts.no ?? '')}"><i class="fas fa-trash me-1"></i>${escapeAttribute(texts.delete ?? '')}</button></li></ul></div></div>
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
				// the text is shown again if the message was not sent
				$div.dataset.pending = msg;
				$div.querySelector('#privateMessageNotSent')?.remove();

                chatHub.invoke('SendPrivateMessageAsync', parseInt(userId), msg);
                $text.value = '';
            }

            $text.focus();
        }
    });
}

function bindConversation(user: HTMLLIElement): void {
	user.addEventListener('click', () => {
		const activeUser = document.querySelector<HTMLElement>('.chat-list .active');
		if (activeUser) {
			activeUser.classList.remove('active');
		}

		const li = user;

		li.classList.add('active');

		// opening the conversation marks all messages as read
		const badge = li.querySelector('.chat-unread');

		if (badge) {
			changeUnreadTotal(-parseInt(badge.textContent ?? '0'));
			badge.remove();
		}

		li.classList.remove('unread');

        addMessageUser(li);
	});
}

document.querySelectorAll<HTMLLIElement>('.chat-list-user').forEach(user => bindConversation(user));

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
