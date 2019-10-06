function getPaginationData(pageSize, pageNumber, isPageChange) {
    var yafUserID = $("#PostAttachmentListPlaceholder").data("userid");

    var pagedResults = {};

    pagedResults.UserId = yafUserID;
    pagedResults.PageSize = pageSize;
    pagedResults.PageNumber = pageNumber;

    var ajaxURL = $("#PostAttachmentListPlaceholder").data("url") + "api/Attachment/GetAttachments";

	$.ajax({
		type: "POST",
		url: ajaxURL,
        data: JSON.stringify(pagedResults),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: (function Success(data, status) {
			$("#PostAttachmentListPlaceholder ul").empty();

			$("#PostAttachmentLoader").hide();

			if (data.AttachmentList.length === 0) {
				var list = $("#PostAttachmentListPlaceholder ul");
				var notext = $("#PostAttachmentListPlaceholder").data("notext");

                list.append('<li><div class="alert alert-info text-break" role="alert" style="white-space:normal">' + notext + "</div></li>");
			}

            $.each(data.AttachmentList, function (id, data) {
                var list = $("#PostAttachmentListPlaceholder ul"),
                    listItem = $('<li class="list-group-item" style="white-space: nowrap; cursor: pointer;" />');

                listItem.attr("onclick", data.OnClick);

                if (data.DataURL) {
                    listItem.attr("title", "<img src=\"" + data.DataURL + "\" class=\"img-thumbnail\" />");
                    listItem.attr("data-toggle", "tooltip");
                }

				listItem.append(data.IconImage);

				list.append(listItem);
            });

            setPageNumberAttach(pageSize, pageNumber, data.TotalRecords);

            if (isPageChange) {
                jQuery(".attachments-toggle").dropdown("toggle");
            }

            jQuery("#PostAttachmentListPlaceholder ul li").tooltip({
                html: true,
                template:
                    '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner" style="max-width:250px"></div></div>',
                placement: "top"
            });
        }),
		error: (function Error(request, status, error) {
			$("#PostAttachmentLoader").hide();

			$("#PostAttachmentListPlaceholder").html(request.statusText).fadeIn(1000);
		})
	});
}

function setPageNumberAttach(pageSize, pageNumber, total) {
    var pages = Math.ceil(total / pageSize);
    var pagerHolder = $("#AttachmentsListPager"),
        pagination = $('<ul class="pagination pagination-sm" />');

    pagerHolder.empty();

    pagination.wrap('<nav aria-label="Attachments Page Results" />');

    if (pageNumber > 0) {
        pagination.append('<li class="page-item"><a href="javascript:getPaginationData(' +
            pageSize +
            "," +
            (pageNumber - 1) +
            "," +
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
        pagination.append('<li class="page-item"><a href="javascript:getPaginationData(' +
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
            pagination.append('<li class="page-item"><a href="javascript:getPaginationData(' +
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
        pagination.append('<li class="page-item"><a href="javascript:getPaginationData(' +
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
        pagination.append('<li class="page-item"><a href="javascript:getPaginationData(' +
            pageSize +
            "," +
            (pageNumber + 1) +
            "," +
            total +
            ',true)" class="page-link">&raquo;</a></li>');
    }

    pagerHolder.append(pagination);
}