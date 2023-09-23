function getSearchResultsData(pageNumber) {
    var searchInput = document.querySelector(".searchInput").value,
        searchInputUser = document.querySelector(".searchUserInput").value,
        searchInputTag = document.querySelector(".searchTagInput").value,
        placeHolder = document.getElementById("SearchResultsPlaceholder"),
        ajaxUrl = "api/Search/GetSearchResults",
        loadModal = new bootstrap.Modal("#loadModal");

    var useDisplayName = document.querySelector(".searchUserInput").dataset.display === "True";

    // filter options
    var pageSize = document.querySelector(".resultsPage").value,
        titleOnly = document.querySelector(".titleOnly").value,
        searchWhat = document.querySelector(".searchWhat").value;

    var minimumLength = placeHolder.dataset.minimum;

    // Forum Filter
    var searchForum = document.getElementById("Input_ForumListSelected").value === ""
        ? 0
        : parseInt(document.getElementById("Input_ForumListSelected").value);

    var searchText = "";

    if (searchInput.length && searchInput.length >= minimumLength ||
        searchInputUser.length && searchInputUser.length >= minimumLength ||
        searchInputTag.length && searchInputTag.length >= minimumLength) {

        var replace;

        if (searchInput.length && searchInput.length >= minimumLength) {
            if (titleOnly === "1") {
                // ADD Topic Filter
                if (searchWhat === "0") {
                    // Match all words
                    replace = searchInput;
                    searchText += ` Topic: (${replace.replace(/(^|\s+)/g, "$1+")})`;
                } else if (searchWhat === "1") {
                    // Match Any Word
                    searchText += ` Topic: ${searchInput}`;
                } else if (searchWhat === "2") {
                    // Match Exact Phrase
                    searchText += ` Topic:"${searchInput}"`;
                }
            } else {
                if (searchWhat === "0") {
                    // Match all words
                    replace = searchInput;
                    searchText += `(${replace.replace(/(^|\s+)/g, "$1+")})`;
                } else if (searchWhat === "1") {
                    // Match Any Word
                    searchText += `${searchInput}`;
                } else if (searchWhat === "2") {
                    // Match Exact Phrase
                    searchText += `"${searchInput}"`;
                }
            }
        }

        if (searchInputUser.length && searchInputUser.length >= minimumLength) {
            var author = useDisplayName ? "AuthorDisplay" : "Author";

            if (searchText.length) searchText += " ";

            if (searchInput.length) {
                searchText += `AND ${author}:${searchInputUser}`;
            } else {
                searchText = `+${author}:${searchInputUser}`;
            }
        }

        if (searchInputTag.length && searchInputTag.length >= minimumLength) {
            if (searchText.length) searchText += " ";

            if (searchInput.length) {
                searchText += `AND TopicTags:${searchInputTag}`;
            } else {
                searchText = `+TopicTags:${searchInputTag}`;
            }
        }

        var searchTopic = {};
        searchTopic.ForumId = searchForum;
        searchTopic.PageSize = pageSize;
        searchTopic.Page = pageNumber;
        searchTopic.SearchTerm = searchText;

        empty(placeHolder);

        // show loading screen 
        loadModal.show();

        fetch(ajaxUrl,
            {
                method: "POST",
                body: JSON.stringify(searchTopic),
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json;charset=utf-8",
                    "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            }).then(res => res.json()).then(data => {
                document.getElementById("loadModal").addEventListener("shown.bs.modal",
                    () => {
                        loadModal.hide();
                    });

                var posted = placeHolder.dataset.posted,
                    by = placeHolder.dataset.by,
                    lastPost = placeHolder.dataset.lastpost,
                    topic = placeHolder.dataset.topic;

                if (data.searchResults.length === 0) {
                    loadModal.hide();

                    const noText = placeHolder.dataset.notext;

                    const div = document.createElement("div");

                    div.innerHTML = `<div class="alert alert-warning text-center mt-3" role="alert">${noText}</div>`;

                    placeHolder.appendChild(div);

                    empty(document.getElementById("SearchResultsPagerTop"));
                    empty(document.getElementById("SearchResultsPagerBottom"));

                } else {
                    loadModal.hide();

                    data.searchResults.forEach((dataItem) => {
                        var item = document.createElement("div");

                        var tags = " ";

                        if (dataItem.topicTags) {
                            const topicTags = dataItem.topicTags.split(",");

                            topicTags.forEach((d) => {
                                tags += `<span class='badge text-bg-secondary me-1'><i class='fas fa-tag me-1'></i>${d
                                    }</span>`;
                            });
                        }

                        item.innerHTML =
                            `<div class="row"><div class="col"><div class="card border-0 w-100 mb-3"><div class="card-header bg-transparent border-top border-bottom-0 px-0 pb-0 pt-4 topicTitle"><h5> <a title="${topic}" href="${dataItem.topicUrl}">${dataItem.topic}</a>&nbsp;<a title="${lastPost}" href="${dataItem.messageUrl
                            }"><i class="fas fa-external-link-alt"></i></a> <small class="text-body-secondary">(<a href="${dataItem.forumUrl}">${dataItem.forumName
                            }</a>)</small></h5></div><div class="card-body px-0"><h6 class="card-subtitle mb-2 text-body-secondary">${data.Description
                            }</h6><p class="card-text messageContent">${dataItem.message
                            }</p></div><div class="card-footer bg-transparent border-top-0 px-0 py-2"> <small class="text-body-secondary"><span class="fa-stack"><i class="fa fa-calendar-day fa-stack-1x text-secondary"></i><i class="fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse"></i> <i class="fa fa-clock fa-badge text-secondary"></i> </span>${posted} ${dataItem.posted} <i class="fa fa-user fa-fw text-secondary"></i>${by} ${useDisplayName
                                ? dataItem.userDisplayName
                                : dataItem.userName}${tags}</small> </div></div></div></div>`;

                        placeHolder.appendChild(item);
                    });

                    setSearchPageNumber(pageSize, pageNumber, data.totalRecords);
                }
            }).catch(function (error) {
                console.log(error);
                document.getElementById("SearchResultsPlaceholder").style.display = "none";
                placeHolder.textContent = error;
            });
    }
}

function setSearchPageNumber(pageSize, pageNumber, total) {
    const pages = Math.ceil(total / pageSize),
        pagerHolderTop = document.getElementById("SearchResultsPagerTop"),
        pagerHolderBottom = document.getElementById("SearchResultsPagerBottom"),
        pagination = document.createElement("ul"),
        paginationNavTop = document.createElement("nav"),
        paginationNavBottom = document.createElement("nav");

    paginationNavTop.setAttribute("aria-label", "Search Page Results");
    paginationNavBottom.setAttribute("aria-label", "Search Page Results");

    pagination.classList.add("pagination");

    empty(pagerHolderTop);
    empty(pagerHolderBottom);

    if (pageNumber > 0) {
        const page = document.createElement("li");

        page.classList.add("page-item");

        page.innerHTML =
            `<a href="javascript:getSearchResultsData(${pageNumber - 1
            })" class="page-link"><i class="fas fas fa-angle-left" aria-hidden="true"></i></a>`;

        pagination.appendChild(page);
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
        let page = document.createElement("li");

        page.classList.add("page-item");
        page.innerHTML = `<a href="javascript:getSearchResultsData(${0});" class="page-link">1</a>`;

        pagination.appendChild(page);

        page = document.createElement("li");

        page.classList.add("page-item");
        page.classList.add("disabled");

        page.innerHTML = '<a class="page-link" href="#" tabindex="-1">...</a>';

        pagination.appendChild(page);
    }

    for (var i = start; i < end; i++) {
        if (i === pageNumber) {
            const page = document.createElement("li");

            page.classList.add("page-item");
            page.classList.add("active");

            page.innerHTML = `<span class="page-link">${i + 1}</span>`;

            pagination.appendChild(page);
        } else {
            const page = document.createElement("li");

            page.classList.add("page-item");
            page.innerHTML = `<a href="javascript:getSearchResultsData(${i});" class="page-link">${i + 1}</a>`;

            pagination.appendChild(page);
        }
    }

    if (end < pages) {
        let page = document.createElement("li");

        page.classList.add("page-item");
        page.classList.add("disabled");
        page.innerHTML = '<a class="page-link" href="#" tabindex="-1">...</a>';

        pagination.appendChild(page);

        page = document.createElement("li");

        page.classList.add("page-item");
        page.innerHTML = `<a href="javascript:getSearchResultsData(${pages - 1})" class="page-link">${pages}</a>`;

        pagination.appendChild(page);
    }

    if (pageNumber < pages - 1) {
        const page = document.createElement("li");

        page.classList.add("page-item");
        page.innerHTML =
            `<a href="javascript:getSearchResultsData(${pageNumber + 1
            })" class="page-link"><i class="fas fas fa-angle-right" aria-hidden="true"></i></a>`;

        pagination.appendChild(page);
    }

    paginationNavTop.appendChild(pagination);

    paginationNavBottom.innerHTML = paginationNavTop.innerHTML;

    pagerHolderTop.appendChild(paginationNavTop);
    pagerHolderBottom.appendChild(paginationNavBottom);
}