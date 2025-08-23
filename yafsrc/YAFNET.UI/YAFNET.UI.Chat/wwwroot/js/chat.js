let connection = new signalR.HubConnectionBuilder().withUrl('/allChatHub').configureLogging(signalR.LogLevel.Error).build();

//Disable the send button until connection is established.
document.getElementById('sendButton').disabled = true;

connection.on('ReceiveMessage', function (name, message, dateTime, side, timeSide, msgClass) {
    const divChat = document.createElement('div'),
        toAvatar = document.getElementById('userAvatar').value;

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
    avatarImg.src = toAvatar;

    // Build message body
    const msgDiv = document.createElement('div');
    msgDiv.className = `direct-chat-text ${msgClass}`;
    msgDiv.textContent = message;

    divChat.appendChild(metaDiv);
    divChat.appendChild(avatarImg);
    divChat.appendChild(msgDiv);

    document.getElementById('divMessage').appendChild(divChat);
});

connection.start().then(function () {
    document.getElementById('sendButton').disabled = false;
    
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById('sendButton').addEventListener('click', function (event) {
    const message = document.getElementById('messageInput').value;
    connection.invoke('SendMessage', message).catch(function (err) {
        return console.log(err.toString());
    });
    event.preventDefault();
});