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
            for (var i = 0; i < conversations.length; i++) {

                var ctrId = 'private_' + toUserId;

                var avatar = $('#' + ctrId).data("avatar");
                var toAvatar = $('#' + ctrId).data("toavatar");

                var currUserId = parseInt($('#UserId').val());
                var side = 'start';
                var msgClass = "text-bg-light";
                var timeSide = 'end';
                var message = conversations[i].body;
                var dateTime = conversations[i].dateTime;

                if (currUserId !== conversations[i].toUserId) {
                    side = 'end';
                    msgClass = "text-bg-primary";
                    timeSide = 'start';
                    toAvatar = avatar;
                }

                AddMessage(ctrId, message, dateTime, side, timeSide, msgClass, toAvatar);
            }
        });

    chatHub.on("sendPrivateMessage",
        function (toUserId, message, toAvatar, dateTime) {

            var ctrId = 'private_' + toUserId;

            var currentUserId = parseInt($('#UserId').val());
            var side = 'start';
            var msgClass = "text-bg-light";
            var timeSide = 'end';

            if (currentUserId !== toUserId) {
                side = 'end';
                msgClass = "text-bg-primary";
                timeSide = 'start';
            }

            AddMessage(ctrId, message, dateTime, side, timeSide, msgClass, toAvatar);
        });
}
function registerEvents() {

    var li = $('.chat-list-user.active');

    AddUser(li);
}

$(".chat-list-user").on("click", function () {
    $(".chat-list").find(".active").removeClass("active");

    var li = $(this);

    li.addClass("active");

    AddUser(li);
});

function AddUser(li) {
    let avatar = document.getElementsByClassName("img-navbar-avatar")[0].getAttribute("src");
    var userId = parseInt($('#UserId').val());
    var toUserId = li.attr('id');
    var toUserName = li.find(".name")[0].innerText;
    var avatarUrl = li.find(".img-thumbnail").attr("src");

    if (userId !== toUserId) {
        var ctrId = 'private_' + toUserId;

        chatHub.invoke("ConnectAsync", avatar, parseInt(toUserId));

        OpenPrivateChatCard(chatHub, toUserId, ctrId, toUserName, avatarUrl, avatar);
    }
    $("#deleteConversation").on("click", function () {
        var li = $('.chat-list-user.active');

        var deleteId = li.attr('id');

        chatHub.invoke("DeleteConversationAsync", parseInt(deleteId));

        location.reload();
    });
}

function AddMessage(ctrlId, message, dateTime, side, timeSide, msgClass, toAvatar) {

    var divChat = '<div class="direct-chat-msg ' +
        side +
        '">' +
        '<div class="fs-6 mb-1 clearfix">' +
        '<span class="text-body-secondary float-' +
        timeSide +
        '"">' +
        dateTime +
        '</span>' +
        '</div>' +
        ' <img class="direct-chat-img img-thumbnail rounded" src="' +
        toAvatar +
        '" alt="Message User Image">' +
        ' <div class="direct-chat-text ' +
        msgClass +
        '" >' +
        message +
        '</div> </div>';

    $('#' + ctrlId).find('#divMessage').append(divChat);
}

function OpenPrivateChatCard(chatHub, userId, ctrId, userName, toAvatarUrl, avatarUrl) {

    var div1 = '<div id="' +
        ctrId +
        '" class="card direct-chat" data-toAvatar="' +
        toAvatarUrl +
        '" data-avatar="' +
        avatarUrl +
        '">' +
        '<div class="card-header">' +
        '<div class="row justify-content-between align-items-center">' +
        '<div class="col-auto"><h3 class="card-title">' +
        userName +
        '</h3></div>' +
        ' <div class="col-auto">' +
        '<button id="deleteConversation" class="btn btn-danger" type="button"><i class="fas fa-trash fa-fw"></i></button>' +
        '  </div>' +
        '</div></div>' +
        ' <div class="card-body">' +
        ' <div id="divMessage" class="direct-chat-messages"></div>' +
        '  </div>' +
        '  <div class="card-footer">' +
        '  <div class="input-group mb-0">' +
        '    <input type="text" id="txtPrivateMessage" name="message" placeholder="Type Message ..." class="form-control"  />' +
        '  <button type="button" id="btnSendMessage" class="btn btn-primary"><i class="fas fa-paper-plane fa-fw"></i></button>' +
        '   </div>' +
        ' </div>';

    var $div = $(div1);

    // Send Button event in Private Chat
    $("#btnSendMessage").on("click", function () {
        var $text = $div.find("#txtPrivateMessage");

        var msg = $text.val();
        if (msg.length > 0) {
            chatHub.invoke("SendPrivateMessageAsync", parseInt(userId), msg);
            $text.val('');
        }
    });


    // Text card event on Enter Button
    $div.find("#txtPrivateMessage").on("keypress", function () {
        if (e.which === 13) {
            $div.find("#btnSendMessage").trigger("click");
        }
    });


    // Append private chat div inside the main div
    $('#PriChatDiv').empty();
    $('#PriChatDiv').append($div);
}