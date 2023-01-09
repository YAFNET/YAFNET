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

                setPageNumberNotify(pageSize, pageNumber, data.totalRecords);

                if (isPageChange) {
                    jQuery(".notify-toggle").dropdown("toggle");
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

function setPageNumberNotify(pageSize, pageNumber, total) {
    var pages = Math.ceil(total / pageSize);
    var pagerHolder = $("#NotifyListPager"),
        pagination = $('<ul class="pagination pagination-sm" />');

    pagerHolder.empty();

    pagination.wrap('<nav aria-label="Attachments Page Results" />');

    if (pageNumber > 0) {
        pagination.append('<li class="page-item"><a href="javascript:getNotifyData(' +
            pageSize +
            "," +
            (pageNumber - 1) +
            "," +
            total +
            ',true)" class="page-link"><i class="fas fa-angle-left"></i></a></li>');
    }

    var start = pageNumber - 2;
    var end = pageNumber + 3;

    if (start < 0) {
        start = 0;
    }

    if (end > pages) {
        end = pages;
    }

    if (start > 0) {
        pagination.append('<li class="page-item"><a href="javascript:getNotifyData(' +
            pageSize +
            "," +
            0 +
            "," +
            total +
            ', true);" class="page-link">1</a></li>');
        pagination.append('<li class="page-item disabled"><a class="page-link" href="#" tabindex="-1">...</a></li>');
    }

    for (var i = start; i < end; i++) {
        if (i === pageNumber) {
            pagination.append('<li class="page-item active"><span class="page-link">' + (i + 1) + "</span>");
        } else {
            pagination.append('<li class="page-item"><a href="javascript:getNotifyData(' +
                pageSize +
                "," +
                i +
                "," +
                total +
                ',true);" class="page-link">' +
                (i + 1) +
                "</a></li>");
        }
    }

    if (end < pages) {
        pagination.append('<li class="page-item disabled"><a class="page-link" href="#" tabindex="-1">...</a></li>');
        pagination.append('<li class="page-item"><a href="javascript:getNotifyData(' +
            pageSize +
            "," +
            (pages - 1) +
            "," +
            total +
            ',true)" class="page-link">' +
            pages +
            "</a></li>");
    }

    if (pageNumber < pages - 1) {
        pagination.append('<li class="page-item"><a href="javascript:getNotifyData(' +
            pageSize +
            "," +
            (pageNumber + 1) +
            "," +
            total +
            ',true)" class="page-link"><i class="fas fa-angle-right"></i></a></li>');
    }

    pagerHolder.append(pagination);
}