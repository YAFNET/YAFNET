document.addEventListener('DOMContentLoaded', function () {
    if (document.querySelector('.searchSimilarTopics') != null) {

        const input = document.querySelector('.searchSimilarTopics');

        // one search after the typing (a search per key ran into the request limit), only the newest answer is shown
        let timer = null;
        let request = 0;

        input.addEventListener('keyup', () => {
            clearTimeout(timer);
            timer = setTimeout(searchSimilar, 500);
        });

        function searchSimilar() {
            const placeHolder = document.getElementById('SearchResultsPlaceholder'),
                ajaxUrl = placeHolder.dataset.url + 'api/Search/GetSimilarTitles',
                searchText = input.value,
                current = ++request;

            if (searchText.length && searchText.length >= 4) {

                const searchTopic = {};
                searchTopic.ForumId = 0;
                searchTopic.PageSize = 0;
                searchTopic.Page = 0;
                searchTopic.SearchTerm = searchText;

                fetch(ajaxUrl,
                    {
                        method: 'POST',
                        body: JSON.stringify(searchTopic),
                        headers: {
                            Accept: 'application/json',
                            "Content-Type": 'application/json;charset=utf-8'
                        }
                    }).then(res => res.json()).then(data => {
                        if (current !== request) {
                            return;
                        }

                        empty(placeHolder);

                        placeHolder.classList.remove('list-group');

                        if (data.TotalRecords > 0) {
                            var list = document.createElement('ul');

                            list.classList.add('list-group','list-similar');

                            if (data.SearchResults.length > 0) {
                                data.SearchResults.forEach((dataItem) => {
                                    var li = document.createElement('li');

                                    li.classList.add('list-group-item');
                                    li.classList.add('list-group-item-action');

                                    li.innerHTML = `<a href="${dataItem.TopicUrl}" target="_blank">${dataItem.Topic}</a>`;

                                    list.appendChild(li);
                                });
                            }

                            placeHolder.appendChild(list);
                        }
                }).catch(function (error) {
                    // the error is not shown as the similar topics (it was the text of a script error)
                    console.log(error);
                    empty(placeHolder);
                });
            }
        }
    }
});