function getPaginationData(pageSize, pageNumber) {
    var yafUserID = $("#PostAttachmentListPlaceholder").data("userid");
	var defaultParameters = "{userID:" + yafUserID + ", pageSize:" + pageSize + ",pageNumber:" + pageNumber + "}";

	var ajaxURL = $("#PostAttachmentListPlaceholder").data("url") + "YafAjax.asmx/GetAttachments";

	$.ajax({
		type: "POST",
		url: ajaxURL,
		data: defaultParameters,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: (function Success(data, status) {
			$('#PostAttachmentListPlaceholder ul').empty();

			$("#PostAttachmentLoader").hide();

			if (data.d.AttachmentList.length === 0) {
				var list = $('#PostAttachmentListPlaceholder ul');
				var notext = $("#PostAttachmentListPlaceholder").data("notext");

				list.append('<li><em>' + notext + '</em></li>');
			}

			$.each(data.d.AttachmentList, function (id, data) {
			    var list = $('#PostAttachmentListPlaceholder ul'),
			        listItem = $('<li class="popupitem" onmouseover="mouseHover(this,true)" onmouseout="mouseHover(this,false)" style="white-space: nowrap; cursor: pointer;" />');

				listItem.attr("onclick", data.OnClick);

			    if (data.DataURL) {

			        listItem.attr("title", "<img src=\"" + data.DataURL + "\" style=\"max-width:200px\" />");
				    listItem.attr("data-toggle", "tooltip");
				}

				listItem.append(data.IconImage);

				list.append(listItem);
			});

			setPageNumber(pageSize, pageNumber, data.d.TotalRecords);

			jQuery(".attachments-toggle").dropdown('toggle');
		    jQuery('[data-toggle="tooltip"]').tooltip({
		        html: true,
		        template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner" style="max-width:250px"></div></div>',
		        placement: 'top'
		    });
		}),
		error: (function Error(request, status, error) {
			$("#PostAttachmentLoader").hide();

			$("#PostAttachmentListPlaceholder").html(request.statusText).fadeIn(1000);
		})
	});
}

function setPageNumber(pageSize, pageNumber, total) {
	var pages = Math.ceil(total / pageSize);
	var pagerHolder = $('#AttachmentsListPager'),
        pagination = $('<ul class="pagination-sm" />');

	pagerHolder.empty();

	if (pageNumber > 0) {
		pagination.append('<li class="page-item"><a href="javascript:getPaginationData(' + pageSize + ',' + (pageNumber - 1) + ',' + total + ')" class="page-link">&laquo;</a></li>');
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
		pagination.append('<a href="javascript:getPaginationData(' + pageSize + ',' + 0 + ',' + total + ')">1</a>');
		pagination.append('<span>...</span>');
	}

	for (var i = start; i < end; i++) {
		if (i === pageNumber) {
		    pagination.append('<li class="page-item active"><span class="page-link">' + (i + 1) + '</span>');
		} else {
		    pagination.append('<li class="page-item"><a href="javascript:getPaginationData(' + pageSize + ',' + i + ',' + total + ');" class="page-link">' + (i + 1) + '</a></li>');
		}
	}

	if (end < pages) {
		pagination.append('<span>...</span>');
		pagination.append('<li class="page-item"><a href="javascript:getPaginationData(' + pageSize + ',' + (pages - 1) + ',' + total + ')" class="page-link">' + pages + '</a></li>');
	}

	if (pageNumber < pages - 1) {
	    pagination.append('<li class="page-item"><a href="javascript:getPaginationData(' + pageSize + ',' + (pageNumber + 1) + ',' + total + ')" class="page-link">&raquo;</a></li>');
	}

	pagerHolder.append(pagination);
}

function AttachmentsPageSelectCallback(page_index) {
	var Attachments_content = jQuery('#AttachmentsPagerHidden div.result:eq(' + page_index + ')').clone();
	jQuery('#AttachmentsPagerResult').empty().append(Attachments_content);
	return false;
}