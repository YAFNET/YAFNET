var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").withAutomaticReconnect().build();

connection.on("newActivityAsync", (alerts) => {
    var alert = document.getElementById("notificationAlert");
    var notifyLink = document.getElementById("notificationLink");

    if (alerts > 0 && notifyLink.classList.contains('d-none')) {
        notifyLink.classList.toggle('d-none');
        alert.classList.toggle('d-none');
    }
});

connection.start();

document.addEventListener('DOMContentLoaded',
    function () {
        var alert = document.getElementById("notificationAlert");
        var notifyLink = document.getElementById("notificationLink");

        if (alert !== null && alert.dataset.alerts > 0) {
            notifyLink.classList.toggle('d-none');
            alert.classList.toggle('d-none');
        }
    });