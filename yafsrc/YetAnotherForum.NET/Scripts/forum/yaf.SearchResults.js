function getSeachResultsData(pageNumber) {
    var searchInput = jQuery(".searchInput").val();
    var searchInputUser = jQuery(".searchUserInput").val();

    // filter options
    var pageSize = jQuery(".resultsPage").val();
    var titleOnly = jQuery(".titleOnly").val();
    var searchWhat = jQuery(".searchWhat").val();

    // Forum Filter
    var searchForum = parseInt(jQuery(".searchForum").val());

    var searchText = "";

    if (searchInput.length && searchInput.length >= 4 || searchInputUser.length && searchInputUser.length >= 4) {

        var replace;

        if (searchInput.length && searchInput.length >= 4) {
            if (titleOnly === "1") {
                // ADD Topic Filter
                if (searchWhat === "0") {
                    // Match all words
                    replace = searchInput;
                    searchText += " Topic: (" + replace.replace(/(^|\s+)/g, "$1+") + ")";
                } else if (searchWhat === "1") {
                    // Match Any Word
                    searchText += " Topic: " + searchInput;
                } else if (searchWhat === "2") {
                    // Match Extact Phrase
                    searchText += " Topic:" + "\"" + searchInput + "\"";
                }
            } else {
                if (searchWhat === "0") {
                    // Match all words
                    replace = searchInput;
                    searchText += "(" + replace.replace(/(^|\s+)/g, "$1+") + ")";
                } else if (searchWhat === "1") {
                    // Match Any Word
                    searchText += "" + searchInput;
                } else if (searchWhat === "2") {
                    // Match Extact Phrase
                    searchText += "" + "\"" + searchInput + "\"";
                }

                searchText += " -Author:" + searchInputUser;
            }
        }

        if (searchInputUser.length && searchInputUser.length >= 4) {

            if (searchInput.length) {
                searchText += "AND Author:" + searchInputUser;
            } else {
                searchText += "+Author:" + searchInputUser;
            }
        }

        var yafUserId = $("#SearchResultsPlaceholder").data("userid");
        var defaultParameters = "{userId:" +
            yafUserId +
            ", forumId:" +
            searchForum +
            ", pageSize:" +
            pageSize +
            ",pageNumber:" +
            pageNumber +
            ",searchInput: '" +
            searchText +
            "'}";

        var ajaxUrl = $("#SearchResultsPlaceholder").data("url") + "YafAjax.asmx/GetSearchResults";

        console.log(defaultParameters);

        $.ajax({
            type: "POST",
            url: ajaxUrl,
            data: defaultParameters,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: (function before() {
                $('#SearchResultsPlaceholder').empty();
                // show loading screen 
                $('#loadModal').modal('show');
            }),
            complete: (function before() {
                // show loading screen 
                $('#loadModal').modal('hide');
            }),
            success: (function success(data) {
                var posted = $("#SearchResultsPlaceholder").data("posted");
                var by = $("#SearchResultsPlaceholder").data("by");
                var lastpost = $("#SearchResultsPlaceholder").data("lastpost");
                var topic = $("#SearchResultsPlaceholder").data("topic");

                if (data.d.SearchResults.length === 0) {
                    var list = $('#SearchResultsPlaceholder');
                    var notext = $("#SearchResultsPlaceholder").data("notext");

                    list.append('<div class="alert alert-warning text-center" role="alert">' +
                        notext +
                        '</div>');

                    $('#SearchResultsPager').empty();

                    $('#loadModal').modal('hide');
                } else {
                    $.each(data.d.SearchResults,
                        function(id, data) {
                            var groupHolder = $('#SearchResultsPlaceholder');

                            groupHolder.append('<div class="row"><div class="card w-100  mb-3">' +
                                '<div class="card-header topicTitle"><h6> ' +
                                '<a title="' +
                                topic +
                                '" href="' +
                                data.TopicUrl +
                                '">' +
                                data.Topic +
                                '</a>&nbsp;' +
                                '<a class="btn btn-primary btn-sm" ' +
                                'title="' +
                                lastpost +
                                '" href="' +
                                data.MessageUrl +
                                '"><i class="fas fa-external-link-square-alt"></i></a>' +
                                ' <small class="text-muted">(' +
                                by +
                                ' ' +
                                data.UserName +
                                ')</small>' +
                                '</h6></div>' +
                                '<div class="card-body">' +
                                '<p class="card-text messageContent">' +
                                data.Message +
                                '</p>' +
                                '</div>' +
                                '<div class="card-footer"> ' +
                                '<small class="text-muted">' +
                                posted +
                                moment(data.Posted).fromNow() +
                                '</small> ' +
                                '</div>' +
                                '</div></div>');
                        });
                    setPageNumber(pageSize, pageNumber, data.d.TotalRecords);
                }
            }),
            error: (function error(request) {
                $("#SearchResultsPlaceholder").html(request.statusText).fadeIn(1000);
            })
        });
    }
}

function setPageNumber(pageSize, pageNumber, total) {
    var pages = Math.ceil(total / pageSize);
    var pagerHolder = $('#SearchResultsPager'),
        pagination = $('<ul class="pagination" />');

    pagerHolder.empty();

    pagination.wrap('<nav aria-label="Search Page Results" />');

    if (pageNumber > 0) {
        pagination.append('<li class="page-item"><a href="javascript:getSeachResultsData(' +
            pageSize +
            ',' +
            (pageNumber - 1) +
            ',' +
            total +
            ')" class="page-link">&laquo;</a></li>');
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
        pagination.append('<li class="page-item"><a href="javascript:getSeachResultsData(' +
            pageSize +
            ',' +
            0 +
            ',' +
            total +
            ');" class="page-link">1</a></li>');
        pagination.append('<li class="page-item disabled"><a class="page-link" href="#" tabindex="-1">...</a></li>');
    }

    for (var i = start; i < end; i++) {
        if (i === pageNumber) {
            pagination.append('<li class="page-item active"><span class="page-link">' + (i + 1) + '</span>');
        } else {
            pagination.append('<li class="page-item"><a href="javascript:getSeachResultsData(' +
                pageSize +
                ',' +
                i +
                ',' +
                total +
                ');" class="page-link">' +
                (i + 1) +
                '</a></li>');
        }
    }

    if (end < pages) {
        pagination.append('<li class="page-item disabled"><a class="page-link" href="#" tabindex="-1">...</a></li>');
        pagination.append('<li class="page-item"><a href="javascript:getSeachResultsData(' +
            pageSize +
            ',' +
            (pages - 1) +
            ',' +
            total +
            ')" class="page-link">' +
            pages +
            '</a></li>');
    }

    if (pageNumber < pages - 1) {
        pagination.append('<li class="page-item"><a href="javascript:getSeachResultsData(' +
            pageSize +
            ',' +
            (pageNumber + 1) +
            ',' +
            total +
            ')" class="page-link">&raquo;</a></li>');
    }

    pagerHolder.append(pagination);
}