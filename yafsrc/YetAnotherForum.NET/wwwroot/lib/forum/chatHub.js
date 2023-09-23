function startChat() {
    window.chatHub = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    chatHub.start().then(function() {
        registerEvents(chatHub);
    }).catch(function(err) {
        return console.error(err.toString());
    });

    chatHub.on("onConnected",
        function (toUserId, conversations) {

            // load existing conversations
            for (let i = 0; i < conversations.length; i++) {

                const ctrId = `private_${toUserId}`,

                    control = document.getElementById(ctrId),
                    avatar = control.dataset.avatar;

                let toAvatar = control.dataset.toAvatar;

                const currentUserId = parseInt(document.getElementById("UserId").value);
                let side = "start";
                let msgClass = "text-bg-light";
                let timeSide = "end";
                const message = conversations[i].body;
                const dateTime = conversations[i].dateTime;

                if (currentUserId !== conversations[i].toUserId) {
                    side = "end";
                    msgClass = "text-bg-primary";
                    timeSide = "start";
                    toAvatar = avatar;
                }

                AddMessage(ctrId, message, dateTime, side, timeSide, msgClass, toAvatar);
            }
        });

    chatHub.on("sendPrivateMessage",
        function (toUserId, message, toAvatar, dateTime) {

            const ctrId = `private_${toUserId}`,
                currentUserId = parseInt(document.getElementById("UserId").value);

            var side = "start";
            var msgClass = "text-bg-light";
            var timeSide = "end";

            if (currentUserId !== toUserId) {
                side = "end";
                msgClass = "text-bg-primary";
                timeSide = "start";
            }

            AddMessage(ctrId, message, dateTime, side, timeSide, msgClass, toAvatar);
        });
}
function registerEvents() {

    const li = document.querySelector(".chat-list-user.active");

    AddUser(li);
}


document.querySelectorAll(".chat-list-user").forEach(user => {
    user.addEventListener("click", () => {
        document.querySelector(".chat-list").querySelector(".active").classList.remove("active");

        const li = user;

        li.classList.add("active");

        AddUser(li);

    });
});

function AddUser(li) {
    const avatar = document.getElementsByClassName("img-navbar-avatar")[0].getAttribute("src");
    const userId = parseInt(document.getElementById("UserId").value);
    const toUserId = li.id;
    const toUserName = li.querySelector(".name").innerText;
    const avatarUrl = li.querySelector(".img-thumbnail").src;

    if (userId !== toUserId) {
        const ctrId = `private_${toUserId}`;

        chatHub.invoke("ConnectAsync", avatar, parseInt(toUserId));

        OpenPrivateChatCard(chatHub, toUserId, ctrId, toUserName, avatarUrl, avatar);
    }

    document.getElementById("deleteConversation").addEventListener("click", () => {
        const active = document.querySelector(".chat-list-user.active");

        const deleteId = active.id;

        chatHub.invoke("DeleteConversationAsync", parseInt(deleteId));

        location.reload();
    });
}

function AddMessage(ctrlId, message, dateTime, side, timeSide, msgClass, toAvatar) {

    const divChat = document.createElement("div");
    divChat.className = `direct-chat-msg ${side}`;
    divChat.innerHTML = `<div class="fs-6 mb-1 clearfix"><span class="text-body-secondary float-${timeSide}"">${dateTime
        }</span></div> <img class="direct-chat-img img-thumbnail rounded" src="${toAvatar
        }" alt="Message User Image"> <div class="direct-chat-text ${msgClass}" >${message}</div>`;

    document.getElementById(`${ctrlId}`).querySelector("#divMessage").append(divChat);
}

function OpenPrivateChatCard(chatHub, userId, ctrId, userName, toAvatarUrl, avatarUrl) {

    const $div = document.createElement("div");

    $div.id = ctrId;
    $div.className = "card direct-chat";
    $div.dataset.toAvatar = toAvatarUrl;
    $div.dataset.avatar = avatarUrl;
    $div.innerHTML = `<div class="card-header"><div class="row justify-content-between align-items-center">
                          <div class="col-auto"><h3 class="card-title">${userName}</h3></div> 
                          <div class="col-auto"><button id="deleteConversation" class="btn btn-danger" type="button"><i class="fas fa-trash fa-fw"></i></button>  </div></div></div>
                      <div class="card-body"> <div id="divMessage" class="direct-chat-messages"></div>  </div>
                      <div class="card-footer">  <div class="input-group mb-0">
                          <input type="text" id="txtPrivateMessage" name="message" placeholder="Type Message ..." class="form-control"  />
                          <button type="button" id="btnSendMessage" class="btn btn-primary"><i class="fas fa-paper-plane fa-fw"></i></button>
                      </div>`;

    // Text card event on Enter Button
    $div.querySelector("#txtPrivateMessage").addEventListener("keypress", (e) => {
        if (e.which === 13) {
            $div.querySelector("#btnSendMessage").click();
        }
    });


    // Append private chat div inside the main div
    document.getElementById("PriChatDiv").replaceChildren();
    document.getElementById("PriChatDiv").append($div);



    // Send Button event in Private Chat
    document.getElementById("btnSendMessage").addEventListener("click", () => {
        const $text = $div.querySelector("#txtPrivateMessage");

        const msg = $text.value;
        if (msg.length > 0) {
            chatHub.invoke("SendPrivateMessageAsync", parseInt(userId), msg);
            $text.value = "";
        }
    });
}