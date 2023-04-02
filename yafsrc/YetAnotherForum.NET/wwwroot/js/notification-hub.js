"use strict";

var hubName = "notification-hub";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/" + hubName)
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

var myNotificationManager = $.extend({}, myNotificationManager, {
    messageListeners: {
        notificationProcessMessages: []
    }
});

connection.on("receiveNotificationMessage", function (message) {
    if (!myNotificationManager.messageListeners.notificationProcessMessages) {
        return;
    }

    myNotificationManager.messageListeners.notificationProcessMessages.forEach(function (fn) {
        fn({
            message: message
        });
    });
});

connection.start().then(function () {
    abp.log.info(hubName + " connected.");
}).catch(function (err) {
    abp.log.error(err.toString());
});