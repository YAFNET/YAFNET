var connection = new signalR.HubConnectionBuilder().withUrl('/allChatHub').configureLogging(signalR.LogLevel.Error).build();

//Disable the send button until connection is established.
document.getElementById('sendButton').disabled = true;

connection.on('ReceiveMessage', function (name, message, dateTime, side, timeSide, msgClass) {
    const divChat = document.createElement('div'),
        toAvatar = document.getElementById('userAvatar').value;

    divChat.className = `direct-chat-msg ${side}`;
    divChat.innerHTML = `<div class="fs-6 mb-1 clearfix"><span class="text-body-secondary float-${timeSide}"">${dateTime
        }</span></div> <img class="direct-chat-img img-thumbnail rounded" src="${toAvatar
        }" alt="${name}"> <div class="direct-chat-text ${msgClass}" >${message}</div>`;

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