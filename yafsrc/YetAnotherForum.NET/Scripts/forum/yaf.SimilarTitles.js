// Generic Functions
jQuery(document).ready(function () {
    if (jQuery(".searchSimilarTopics").length) {

        jQuery(".searchSimilarTopics").keyup(function () {

            var input = jQuery(".searchSimilarTopics"),
                searchText = input.val(),
                searchPlaceHolder = jQuery("#SearchResultsPlaceholder");

            if (searchText.length && searchText.length >= 4) {

                var searchTopic = {};
                searchTopic.ForumId = 0;
                searchTopic.PageSize = 0;
                searchTopic.Page = 0;
                searchTopic.SearchTerm = searchText;

                var ajaxUrl = searchPlaceHolder.data("url") + "api/Search/GetSimilarTitles";

                $.ajax({
                    type: "POST",
                    url: ajaxUrl,
                    dataType: "json",
                    data: JSON.stringify(searchTopic),
                    contentType: "application/json; charset=utf-8",
                    beforeSend: function() {
                        searchPlaceHolder.empty();
                        searchPlaceHolder.remove("list-group");
                    },
                    complete: function() {
                    },
                    success: function (data) {
                        searchPlaceHolder.empty();
                        searchPlaceHolder.remove("list-group");

                        if (data.TotalRecords > 0) {
                            var list = $('<ul class="list-group list-similar" />');
                            searchPlaceHolder.append(list);

                            $.each(data.SearchResults,
                                function (id, data) {
                                    list.append('<li class="list-group-item">' +
                                        '<a href="' +
                                        data.TopicUrl +
                                        '" target="_blank">' +
                                        data.Topic +
                                        "</a></li>");
                                });
                        }
                    },
                    error: function (request) {
                        searchPlaceHolder.html(request.statusText).fadeIn(1000);
                    }
                });
            }

        });
    }
});
