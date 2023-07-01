function getAlbumImagesData(pageSize, pageNumber, isPageChange) {
    var pagedResults = {};

    pagedResults.UserId = 0;
    pagedResults.PageSize = pageSize;
    pagedResults.PageNumber = pageNumber;

    var ajaxURL = CKEDITOR.basePath.replace("js/ckeditor/", "") + "api/Album/GetAlbumImages";

	$.ajax({
        url: ajaxURL,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        type: "POST",
        data: JSON.stringify(pagedResults),
		contentType: "application/json; charset=utf-8",
		success: function(data) {
            $("#PostAlbumsListPlaceholder ul").empty();

            $("#PostAlbumsLoader").hide();

            if (data.attachmentList.length === 0) {
                var list = $("#PostAlbumsListPlaceholder ul");
                var notext = $("#PostAlbumsListPlaceholder").data("notext");

                list.append('<li><div class="alert alert-info text-break" role="alert" style="white-space:normal">' + notext + "</div></li>");
            }

            $.each(data.attachmentList, function (id, data) {
                var list = $("#PostAlbumsListPlaceholder ul"),
                    listItem = $('<li class="list-group-item list-group-item-action" style="white-space: nowrap; cursor: pointer;" />');

                listItem.attr("onclick", data.onClick);

                listItem.append(data.iconImage);

                list.append(listItem);
            });

            setPageNumberAlbums(pageSize, pageNumber, data.totalRecords);

            if (isPageChange) {
                $(".albums-toggle").dropdown("toggle");
            }

            var tooltipAlbumsTriggerList = [].slice.call(document.querySelectorAll("#PostAlbumsListPlaceholder ul li"));
            var tooltipAlbumsList = tooltipAlbumsTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl, {
                    html: true,
                    template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner" style="max-width:250px"></div></div>',
                    placement: "top"
                });
            });
        },
        error: function(request, status, error) {
            console.log(request);
            console.log(error);
            $("#PostAlbumsLoader").hide();

            $("#PostAlbumsListPlaceholder").html(request.statusText).fadeIn(1000);
        }
	});
}

function setPageNumberAlbums(pageSize, pageNumber, total) {
    var pages = Math.ceil(total / pageSize);
    var pagerHolder = $("#AlbumsListPager"),
        pagination = $('<ul class="pagination pagination-sm" />');

    pagerHolder.empty();

    pagination.wrap('<nav aria-label="Albums Page Results" />');

    if (pageNumber > 0) {
        pagination.append('<li class="page-item"><a href="javascript:getAlbumImagesData(' +
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
        pagination.append('<li class="page-item"><a href="javascript:getAlbumImagesData(' +
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
            pagination.append('<li class="page-item"><a href="javascript:getAlbumImagesData(' +
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
        pagination.append('<li class="page-item"><a href="javascript:getAlbumImagesData(' +
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
        pagination.append('<li class="page-item"><a href="javascript:getAlbumImagesData(' +
            pageSize +
            "," +
            (pageNumber + 1) +
            "," +
            total +
            ',true)" class="page-link"><i class="fas fa-angle-right"></i></a></li>');
    }

    pagerHolder.append(pagination);
}