function getNotifyData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById("NotifyListPlaceholder"),
        list = placeHolder.querySelector("ul"),
        yafUserId = placeHolder.dataset.userid,
        pagedResults = {},
        ajaxUrl = "/api/Notify/GetNotifications";

    pagedResults.UserId = yafUserId;
    pagedResults.PageSize = pageSize;
    pagedResults.PageNumber = pageNumber;

    fetch(ajaxUrl,
        {
            method: "POST",
            body: JSON.stringify(pagedResults),
            headers: {
                Accept: "application/json",
                "Content-Type": "application/json;charset=utf-8",
                "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        }).then(res => res.json()).then(data => {

            empty(list);

            document.getElementById("Loader").style.display = "none";

            if (data.attachmentList.length > 0) {
                const markRead = document.getElementById("MarkRead");

                markRead.classList.remove("d-none");
                markRead.classList.add("d-block");

                data.attachmentList.forEach((dataItem) => {
                    var li = document.createElement("li");

                    li.classList.add("list-group-item");
                    li.classList.add("list-group-item-action");
                    li.classList.add("small");
                    li.classList.add("text-wrap");

                    li.style.width = "15rem";

                    li.innerHTML = dataItem.fileName;

                    list.appendChild(li);
                });

                setPageNumber(pageSize,
                    pageNumber,
                    data.totalRecords,
                    document.getElementById("NotifyListPager"),
                    "Notifications",
                    "getNotifyData");

                if (isPageChange) {
                    const toggleBtn = document.querySelector(".notify-toggle"),
                        dropdownEl = new bootstrap.Dropdown(toggleBtn);

                    dropdownEl.toggle();
                }
            }
        }).catch(function (error) {
            console.log(error);
            document.getElementById("Loader").style.display = "none";
            placeHolder.textContent = error;
        });
}