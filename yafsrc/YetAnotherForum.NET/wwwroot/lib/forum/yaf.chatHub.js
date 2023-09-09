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

                let toAvatar = control.dataset.toavatar;

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

$(".chat-list-user").on("click", function () {
    $(".chat-list").find(".active").removeClass("active");

    const li = $(this);

    li.addClass("active");

    AddUser(li);
});

function AddUser(li) {
    const avatar = document.getElementsByClassName("img-navbar-avatar")[0].getAttribute("src");
    const userId = parseInt($("#UserId").val());
    const toUserId = li.id;
    const toUserName = li.querySelector(".name").innerText;
    const avatarUrl = li.querySelector(".img-thumbnail").src;

    if (userId !== toUserId) {
        const ctrId = `private_${toUserId}`;

        chatHub.invoke("ConnectAsync", avatar, parseInt(toUserId));

        OpenPrivateChatCard(chatHub, toUserId, ctrId, toUserName, avatarUrl, avatar);
    }
    $("#deleteConversation").on("click", function () {
        const li = $(".chat-list-user.active");

        const deleteId = li.attr("id");

        chatHub.invoke("DeleteConversationAsync", parseInt(deleteId));

        location.reload();
    });
}

function AddMessage(ctrlId, message, dateTime, side, timeSide, msgClass, toAvatar) {

    const divChat = `<div class="direct-chat-msg ${side}"><div class="fs-6 mb-1 clearfix"><span class="text-body-secondary float-${timeSide}"">${dateTime
        }</span></div> <img class="direct-chat-img img-thumbnail rounded" src="${toAvatar
        }" alt="Message User Image"> <div class="direct-chat-text ${msgClass}" >${message}</div> </div>`;

    $(`#${ctrlId}`).find("#divMessage").append(divChat);
}

function OpenPrivateChatCard(chatHub, userId, ctrId, userName, toAvatarUrl, avatarUrl) {

    const div1 = `<div id="${ctrId}" class="card direct-chat" data-toAvatar="${toAvatarUrl}" data-avatar="${avatarUrl
        }"><div class="card-header"><div class="row justify-content-between align-items-center"><div class="col-auto"><h3 class="card-title">${
        userName}</h3></div> <div class="col-auto"><button id="deleteConversation" class="btn btn-danger" type="button"><i class="fas fa-trash fa-fw"></i></button>  </div></div></div> <div class="card-body"> <div id="divMessage" class="direct-chat-messages"></div>  </div>  <div class="card-footer">  <div class="input-group mb-0">    <input type="text" id="txtPrivateMessage" name="message" placeholder="Type Message ..." class="form-control"  />  <button type="button" id="btnSendMessage" class="btn btn-primary"><i class="fas fa-paper-plane fa-fw"></i></button>   </div> </div>`;

    var $div = $(div1);

    // Send Button event in Private Chat
    $("#btnSendMessage").on("click", function () {
        const $text = $div.find("#txtPrivateMessage");

        const msg = $text.val();
        if (msg.length > 0) {
            chatHub.invoke("SendPrivateMessageAsync", parseInt(userId), msg);
            $text.val("");
        }
    });


    // Text card event on Enter Button
    $div.find("#txtPrivateMessage").on("keypress", function () {
        if (e.which === 13) {
            $div.find("#btnSendMessage").trigger("click");
        }
    });


    // Append private chat div inside the main div
    $("#PriChatDiv").empty();
    $("#PriChatDiv").append($div);
}