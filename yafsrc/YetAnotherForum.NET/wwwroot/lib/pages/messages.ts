import * as signalR from '@microsoft/signalr';
import scrollIntoView from 'smooth-scroll-into-view-if-needed';

const chatHub = new signalR.HubConnectionBuilder().withUrl('/chatHub').configureLogging(signalR.LogLevel.Error).build();

chatHub.start().then(() => {
	registerEvents();
}).catch(err => {
	console.error(err.toString());
});

chatHub.on('onConnected', (toUserId: string, conversations: Array<{ body: string, dateTime: string, toUserId: number }>) => {
	// load existing conversations
	for (let i = 0; i < conversations.length; i++) {
		const ctrId = `private_${toUserId}`;
		const control = document.getElementById(ctrId) as HTMLElement;
		const avatar = control.dataset.avatar;

		let toAvatar = control.dataset.toAvatar!;
		const currentUserId = parseInt((document.getElementById('UserId') as HTMLInputElement).value);
		let side = 'start';
		let msgClass = 'text-bg-light';
		let timeSide = 'end';
		const message = conversations[i].body;
		const dateTime = conversations[i].dateTime;

		if (currentUserId !== conversations[i].toUserId) {
			side = 'end';
			msgClass = 'text-bg-primary';
			timeSide = 'start';
			toAvatar = avatar!;
		}

		addMessage(ctrId, message, dateTime, side, timeSide, msgClass, toAvatar);
	}
});

chatHub.on('sendPrivateMessage', (toUserId: number, message: string, toAvatar: string, dateTime: string) => {
	const ctrId = `private_${toUserId}`;
	const currentUserId = parseInt((document.getElementById('UserId') as HTMLInputElement).value);

	let side = 'start';
	let msgClass = 'text-bg-light';
	let timeSide = 'end';

	if (currentUserId !== toUserId) {
		side = 'end';
		msgClass = 'text-bg-primary';
		timeSide = 'start';
	}

	addMessage(ctrId, message, dateTime, side, timeSide, msgClass, toAvatar);
});

function registerEvents(): void {
    const li = document.querySelector('.chat-list-user.active') as HTMLLIElement;

	if (li) {
        addMessageUser(li);
	}
}
function addMessageUser(li: HTMLLIElement): void {
	const avatar = document.getElementsByClassName('img-navbar-avatar')[0].getAttribute('src')!;
	const userId = parseInt((document.getElementById('UserId') as HTMLInputElement).value);
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

	document.getElementById('deleteConversation')!.addEventListener('click', () => {
        const active = document.querySelector('.chat-list-user.active') as HTMLElement;

		const deleteId = active!.id;

		chatHub.invoke('DeleteConversationAsync', parseInt(deleteId));

		location.reload();
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

function openPrivateChatCard(chatHub: any, userId: string, ctrId: string, userName: string, toAvatarUrl: string, avatarUrl: string): void {

	// Append private chat div inside the main div
	const priChatDiv = document.getElementById('PriChatDiv') as HTMLDivElement;

	const placeHolder = priChatDiv.dataset.placeholder ?? 'Type a message...';

    const $div: HTMLDivElement = document.createElement('div');

    $div.id = ctrId;
    $div.className = 'card direct-chat';
    $div.dataset.toAvatar = toAvatarUrl;
    $div.dataset.avatar = avatarUrl;
    $div.innerHTML = `<div class="card-header"><div class="row justify-content-between align-items-center">
                          <div class="col-auto"><h3 class="card-title">${userName}</h3></div> 
                          <div class="col-auto"><button id="deleteConversation" class="btn btn-danger" type="button"><i class="fas fa-trash"></i></button>  </div></div></div>
                      <div class="card-body"> <div id="divMessage" class="direct-chat-messages"></div>  </div>
                      <div class="card-footer">  <div class="input-group mb-0">
                          <textarea rows="3" id="txtPrivateMessage" name="message" placeholder="${placeHolder}" class="form-control"></textarea>
						  <button type="button" id="btnSendMessage" class="btn btn-primary"><i class="fas fa-paper-plane"></i></button>
                      </div>`;

    // Text card event on Enter Button
    $div.querySelector<HTMLTextAreaElement>('#txtPrivateMessage')?.addEventListener('keypress', (e: KeyboardEvent) => {
		if (e.key === 'Enter' && e.shiftKey) {
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
            if (msg.length > 0) {
                chatHub.invoke('SendPrivateMessageAsync', parseInt(userId), msg);
                $text.value = '';
            }
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

        addMessageUser(li);
	});
});