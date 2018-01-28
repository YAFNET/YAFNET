// Generic Functions
jQuery(document).ready(function () {
    // Numeric Spinner Inputs
    if (jQuery('.searchSimilarTopics').length) {

        jQuery(".searchSimilarTopics").keyup(function () {

            var input = jQuery(".searchSimilarTopics"),
                searchText = input.val(),
                searchPlaceHolder = jQuery("#SearchResultsPlaceholder");

            if (searchText.length && searchText.length >= 5) {

                var yafUserId = searchPlaceHolder.data("userid");
                var defaultParameters = "{userId:" +
                    yafUserId +
                    ",searchInput: '" +
                    searchText +
                    "'}";

                var ajaxUrl = searchPlaceHolder.data("url") + "YafAjax.asmx/GetSimilarTitles";

                $.ajax({
                    type: "POST",
                    url: ajaxUrl,
                    data: defaultParameters,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    beforeSend: (function before() {
                        searchPlaceHolder.empty();
                        searchPlaceHolder.remove("list-group");
                        // show loading screen 
                        $('#loadModal').modal('show');
                    }),
                    complete: (function before() {
                        // show loading screen 
                        $('#loadModal').modal('hide');
                    }),
                    success: (function success(data) {
                        searchPlaceHolder.empty();
                        searchPlaceHolder.remove("list-group");

                        if (data.d.SearchResults.length > 0) {
                            var list = $('<ul class="list-group list-similar" />');
                            searchPlaceHolder.append(list);

                            $.each(data.d.SearchResults,
                                function (id, data) {
                                    list.append('<li class="list-group-item">' +
                                        '<a href="' +
                                        data.TopicUrl +
                                        '" target="_blank">' +
                                        data.Topic +
                                        '</a></li>');
                                });
                        }
                    }),
                    error: (function error(request) {
                        searchPlaceHolder.html(request.statusText).fadeIn(1000);
                    })
                });
            }

        });
    }
});
