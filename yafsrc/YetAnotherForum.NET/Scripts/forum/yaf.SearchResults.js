function getSearchResultsData(pageNumber) {
    var searchInput = jQuery(".searchInput").val();
    var searchInputUser = jQuery(".searchUserInput").val();
    var useDisplayName = jQuery(".searchUserInput").data("display") === "True";

    // filter options
    var pageSize = jQuery(".resultsPage").val();
    var titleOnly = jQuery(".titleOnly").val();
    var searchWhat = jQuery(".searchWhat").val();

    var minimumLength = jQuery("#SearchResultsPlaceholder").data("minimum");

    // Forum Filter
    var searchForum = parseInt(jQuery(".searchForum").val());

    var searchText = "";

    if (searchInput.length && searchInput.length >= minimumLength || searchInputUser.length && searchInputUser.length >= 3) {

        var replace;

        if (searchInput.length && searchInput.length >= minimumLength) {
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
                    // Match Exact Phrase
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
                    // Match Exact Phrase
                    searchText += "" + "\"" + searchInput + "\"";
                }
//                searchText += " -Author:" + searchInputUser;
            }
        }

        if (searchInputUser.length && searchInputUser.length >= 3) {
            var author = useDisplayName ? "AuthorDisplay" : "Author";

            if (searchText.length) searchText += " ";
        
            if (searchInput.length) {
                searchText += "AND " + author + ":" + searchInputUser;
            } else {
                searchText = "+" + author + ":" + searchInputUser;
            }
        }

        var searchTopic = {};
        searchTopic.ForumId = searchForum;
        searchTopic.UserId = $("#SearchResultsPlaceholder").data("userid");
        searchTopic.PageSize = pageSize;
        searchTopic.Page = pageNumber;
        searchTopic.SearchTerm = searchText;

        var ajaxUrl = $("#SearchResultsPlaceholder").data("url") + "api/Search/GetSearchResults";

        $.ajax({
            type: "POST",
            url: ajaxUrl,
            data: JSON.stringify(searchTopic),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function() {
                $("#SearchResultsPlaceholder").empty();
                // show loading screen 
                $("#loadModal").modal("show");
            },
            complete: function() {
                // hide loading screen 
                $("#loadModal").modal("hide");
            },
            success: function(data) {
                $("#loadModal").on("shown.bs.modal",
                    function () {
                        $("#loadModal").modal("hide");
                    });
                var posted = $("#SearchResultsPlaceholder").data("posted");
                var by = $("#SearchResultsPlaceholder").data("by");
                var lastpost = $("#SearchResultsPlaceholder").data("lastpost");
                var topic = $("#SearchResultsPlaceholder").data("topic");
                if (data.SearchResults.length === 0) {
                    var list = $("#SearchResultsPlaceholder");
                    var notext = $("#SearchResultsPlaceholder").data("notext");

                    list.append('<div class="alert alert-warning text-center mt-3" role="alert">' +
                        notext +
                        "</div>");

                    $("#SearchResultsPagerTop, #SearchResultsPagerBottom").empty();
                    
                } else {
                    $.each(data.SearchResults,
                        function(id, data) {
                            var groupHolder = $("#SearchResultsPlaceholder");

                            groupHolder.append('<div class="row"><div class="col"><div class="card border-0 w-100 mb-3">' +
                                '<div class="card-header bg-transparent border-top border-bottom-0 px-0 pb-0 pt-4 topicTitle"><h5> ' +
                                '<a title="' +
                                topic +
                                '" href="' +
                                data.TopicUrl +
                                '">' +
                                data.Topic +
                                "</a>&nbsp;" +
                                "<a " +
                                'title="' +
                                lastpost +
                                '" href="' +
                                data.MessageUrl +
                                '"><i class="fas fa-external-link-alt"></i></a>' +
                                ' <small class="text-muted">(<a href="' +
                                data.ForumUrl +
                                '">' +
                                data.ForumName +
                                "</a>)</small>" +
                                "</h5></div>" +
                                '<div class="card-body px-0">' +
                                '<h6 class="card-subtitle mb-2 text-muted">' +
                                data.Description +
                                "</h6>"+ 
                                '<p class="card-text messageContent">' +
                                data.Message +
                                "</p>" +
                                "</div>" +
                                '<div class="card-footer bg-transparent border-top-0 px-0 py-2"> ' +
                                '<small class="text-muted">' +
                                '<span class="fa-stack">' +
                                '<i class="fa fa-calendar-day fa-stack-1x text-secondary"></i>' +
                                '<i class="fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse"></i> ' +
                                '<i class="fa fa-clock fa-badge text-secondary"></i> ' +
                                "</span>" +
                                posted + " " +
                                moment(data.Posted).fromNow() +
                                " " + 
                                '<i class="fa fa-user fa-fw text-secondary"></i>' +
                                by +
                                " " +
                                (useDisplayName ? data.UserDisplayName : data.UserName) +
                                "</small> " +
                                "</div>" +
                                "</div></div></div>");
                        });
                    setPageNumber(pageSize, pageNumber, data.TotalRecords);
                }
            },
            error: function(request) {
                console.log(request);
                $("#SearchResultsPlaceholder").html(request.responseText).fadeIn(1000);
            }
        });
    }
}

function setPageNumber(pageSize, pageNumber, total) {
    var pages = Math.ceil(total / pageSize);
    var pagerHolder = $("#SearchResultsPagerTop, #SearchResultsPagerBottom"),
        pagination = $('<ul class="pagination" />');

    pagerHolder.empty();

    pagination.wrap('<nav aria-label="Search Page Results" />');

    if (pageNumber > 0) {
        pagination.append('<li class="page-item"><a href="javascript:getSearchResultsData(' +
            (pageNumber - 1) +
            ')" class="page-link"><i class="fas fas fa-angle-left" aria-hidden="true"></i></a></li>');
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
        pagination.append('<li class="page-item"><a href="javascript:getSearchResultsData(' +
            0 +
            ');" class="page-link">1</a></li>');
        pagination.append('<li class="page-item disabled"><a class="page-link" href="#" tabindex="-1">...</a></li>');
    }

    for (var i = start; i < end; i++) {
        if (i === pageNumber) {
            pagination.append('<li class="page-item active"><span class="page-link">' + (i + 1) + "</span>");
        } else {
            pagination.append('<li class="page-item"><a href="javascript:getSearchResultsData(' +
                i +
                ');" class="page-link">' +
                (i + 1) +
                "</a></li>");
        }
    }

    if (end < pages) {
        pagination.append('<li class="page-item disabled"><a class="page-link" href="#" tabindex="-1">...</a></li>');
        pagination.append('<li class="page-item"><a href="javascript:getSearchResultsData(' +
            (pages - 1) +
            ')" class="page-link">' +
            pages +
            "</a></li>");
    }

    if (pageNumber < pages - 1) {
        pagination.append('<li class="page-item"><a href="javascript:getSearchResultsData(' +
            (pageNumber + 1) +
            ')" class="page-link"><i class="fas fas fa-angle-right" aria-hidden="true"></i></a></li>');
    }

    pagerHolder.append(pagination);
}