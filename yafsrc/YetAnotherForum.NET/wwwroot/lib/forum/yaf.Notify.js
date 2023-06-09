function getNotifyData(pageSize, pageNumber, isPageChange) {
    var pagedResults = {};

    pagedResults.UserId = 0;
    pagedResults.PageSize = pageSize;
    pagedResults.PageNumber = pageNumber;

    var ajaxURL = "/api/Notify/GetNotifications";

    $.ajax({
        type: "POST",
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        url: ajaxURL,
        data: JSON.stringify(pagedResults),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(data) {
            $("#NotifyListPlaceholder ul").empty();

            $("#Loader").hide();

            if (data.attachmentList.length > 0) {
                $("#MarkRead").removeClass("d-none").addClass("d-block");

                $.each(data.attachmentList,
                    function(id, data) {
                        var list = $("#NotifyListPlaceholder ul"),
                            listItem = $(
                                '<li class="list-group-item list-group-item-action small text-wrap" style="width:15rem;" />');


                        listItem.append(data.fileName);

                        list.append(listItem);
                    });

                if (isPageChange) {
                    $(".notify-toggle").dropdown("toggle");
                }
            }
        },
        error: function (request) {
            console.log(request);
            $("#Loader").hide();

            $("#NotifyListPlaceholder").html(request.statusText).fadeIn(1000);
        }
    });
}