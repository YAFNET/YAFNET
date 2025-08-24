﻿document.addEventListener('DOMContentLoaded', function () {
    if (document.querySelector('.searchSimilarTopics') != null) {

        const input = document.querySelector('.searchSimilarTopics');

        input.addEventListener('keyup', () => {

            const placeHolder = document.getElementById('SearchResultsPlaceholder'),
                ajaxUrl = placeHolder.dataset.url + 'api/Search/GetSimilarTitles',
                searchText = input.value;

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

                        empty(placeHolder);

                        placeHolder.classList.remove('list-group');

                        if (data.TotalRecords > 0) {
                            var list = document.createElement('ul');

                            list.classList.add('list-group','list-similar');

                            if (data.SearchResults.length > 0) {
                                const markRead = document.getElementById('MarkRead');

                                markRead.classList.remove('d-none');
                                markRead.classList.add('d-block');

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
                    console.log(error);
                    placeHolder.textContent = error;
                });
            }

        });
    }
});