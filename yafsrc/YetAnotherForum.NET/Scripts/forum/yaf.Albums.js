function getAlbumImagesData(pageSize, pageNumber, isPageChange) {
    var yafUserID = $("#PostAlbumsListPlaceholder").data("userid");
	var defaultParameters = "{userID:" + yafUserID + ", pageSize:" + pageSize + ",pageNumber:" + pageNumber + "}";

    var ajaxURL = $("#PostAlbumsListPlaceholder").data("url") + "YafAjax.asmx/GetAlbumImages";

	$.ajax({
		type: "POST",
		url: ajaxURL,
		data: defaultParameters,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: (function Success(data, status) {
            $('#PostAlbumsListPlaceholder ul').empty();

            $("#PostAlbumsLoader").hide();

            if (data.d.AttachmentList.length === 0) {
                var list = $('#PostAlbumsListPlaceholder ul');
                var notext = $("#PostAlbumsListPlaceholder").data("notext");

				list.append('<li><em>' + notext + '</em></li>');
			}

            $.each(data.d.AttachmentList, function (id, data) {
                var list = $('#PostAlbumsListPlaceholder ul'),
                    listItem = $('<li class="popupitem" onmouseover="mouseHover(this,true)" onmouseout="mouseHover(this,false)" style="white-space: nowrap; cursor: pointer;" />');

                listItem.attr("onclick", data.OnClick);

                if (data.DataURL) {
                    listItem.attr("title", "<img src=\"" + data.DataURL + "\" style=\"max-width:200px\" />");
                    listItem.attr("data-toggle", "tooltip");
				}

				listItem.append(data.IconImage);

				list.append(listItem);
			});

			setPageNumberAlbums(pageSize, pageNumber, data.d.TotalRecords);

            if (isPageChange) {
                jQuery(".albums-toggle").dropdown('toggle');
                jQuery('[data-toggle="tooltip"]').tooltip({
                    html: true,
                    template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner" style="max-width:250px"></div></div>',
                    placement: 'top'
                });
            }
		}),
		error: (function Error(request, status, error) {
            $("#PostAlbumsLoader").hide();

            $("#PostAlbumsListPlaceholder").html(request.statusText).fadeIn(1000);
		})
	});
}

function setPageNumberAlbums(pageSize, pageNumber, total) {
    var pages = Math.ceil(total / pageSize);
    var pagerHolder = $('#AlbumsListPager'),
        pagination = $('<ul class="pagination pagination-sm" />');

    pagerHolder.empty();

    pagination.wrap('<nav aria-label="Albums Page Results" />');

    if (pageNumber > 0) {
        pagination.append('<li class="page-item"><a href="javascript:getAlbumImagesData(' +
            pageSize +
            ',' +
            (pageNumber - 1) +
            ',' +
            total +
            ',true)" class="page-link">&laquo;</a></li>');
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
            ',' +
            0 +
            ',' +
            total +
            ', true);" class="page-link">1</a></li>');
        pagination.append('<li class="page-item disabled"><a class="page-link" href="#" tabindex="-1">...</a></li>');
    }

    for (var i = start; i < end; i++) {
        if (i === pageNumber) {
            pagination.append('<li class="page-item active"><span class="page-link">' + (i + 1) + '</span>');
        } else {
            pagination.append('<li class="page-item"><a href="javascript:getAlbumImagesData(' +
                pageSize +
                ',' +
                i +
                ',' +
                total +
                ',true);" class="page-link">' +
                (i + 1) +
                '</a></li>');
        }
    }

    if (end < pages) {
        pagination.append('<li class="page-item disabled"><a class="page-link" href="#" tabindex="-1">...</a></li>');
        pagination.append('<li class="page-item"><a href="javascript:getAlbumImagesData(' +
            pageSize +
            ',' +
            (pages - 1) +
            ',' +
            total +
            ',true)" class="page-link">' +
            pages +
            '</a></li>');
    }

    if (pageNumber < pages - 1) {
        pagination.append('<li class="page-item"><a href="javascript:getAlbumImagesData(' +
            pageSize +
            ',' +
            (pageNumber + 1) +
            ',' +
            total +
            ',true)" class="page-link">&raquo;</a></li>');
    }

    pagerHolder.append(pagination);
}