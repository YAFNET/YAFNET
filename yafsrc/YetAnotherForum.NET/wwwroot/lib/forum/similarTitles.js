document.addEventListener('DOMContentLoaded', function () {
    if (document.querySelector('.searchSimilarTopics') != null) {

        const input = document.querySelector('.searchSimilarTopics');

        input.addEventListener('keyup', () => {

            const placeHolder = document.getElementById('SearchResultsPlaceholder'),
                searchText = input.value;

            if (searchText.length && searchText.length >= 4) {

                const searchTopic = {};
                searchTopic.ForumId = 0;
                searchTopic.PageSize = 0;
                searchTopic.Page = 0;
                searchTopic.SearchTerm = searchText;

                const ajaxUrl = '/api/Search/GetSimilarTitles';

                fetch(ajaxUrl,
                    {
                        method: 'POST',
                        body: JSON.stringify(searchTopic),
                        headers: {
                            Accept: 'application/json',
                            "Content-Type": 'application/json;charset=utf-8',
                            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
                        }
                    }).then(res => res.json()).then(data => {

                        empty(placeHolder);

                        placeHolder.classList.remove('list-group');

                        if (data.totalRecords > 0) {
                            var list = document.createElement('ul');

                            list.classList.add('list-group');
                            list.classList.add('list-similar');

                            if (data.searchResults.length > 0) {
                                const markRead = document.getElementById('MarkRead');

                                markRead.classList.remove('d-none');
                                markRead.classList.add('d-block');

                                data.searchResults.forEach((dataItem) => {
                                    var li = document.createElement('li');

                                    li.classList.add('list-group-item');
                                    li.classList.add('list-group-item-action');

                                    li.innerHTML = `<a href="${dataItem.topicUrl}" target="_blank">${dataItem.topic}</a>`;

                                    list.appendChild(li);
                                });
                            }

                            placeHolder.appendChild(list);
                        }
                    }).catch(function (error) {
                        console.log(error);
                        placeHolder.textContent = error;
                    });
            }

        });
    }
});