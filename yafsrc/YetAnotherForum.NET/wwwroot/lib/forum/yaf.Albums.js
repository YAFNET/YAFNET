function getAlbumImagesData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById("PostAlbumsListPlaceholder"),
        list = document.querySelector("#PostAlbumsListPlaceholder ul"),
        yafUserId = placeHolder.dataset.userid,
        pagedResults = {},
        ajaxUrl = "/api/Album/GetAlbumImages";

    pagedResults.UserId = yafUserId;
    pagedResults.PageSize = pageSize;
    pagedResults.PageNumber = pageNumber;

    fetch(ajaxUrl, {
        method: "POST",
        body: JSON.stringify(pagedResults),
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json;charset=utf-8",
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    }).then(res => res.json()).then(data => {

        empty(list);

        document.getElementById("PostAlbumsLoader").style.display = "none";

        if (data.attachmentList.length === 0) {
            const noText = placeHolder.dataset.notext;

            const li = document.createElement("li");

            li.innerHTML = `<li><div class="alert alert-info text-break" role="alert" style="white-space:normal">${noText}</div></li>`;

            list.appendChild(li);
        }

        data.attachmentList.forEach((dataItem) => {
            var li = document.createElement("li");

            li.classList.add("list-group-item");
            li.classList.add("list-group-item-action");

            li.style.whiteSpace = "nowrap";
            li.style.cursor = "pointer";
            li.setAttribute("onclick", dataItem.onClick);

            li.innerHTML = dataItem.iconImage;

            list.appendChild(li);
        });

        renderAttachPreview(".attachments-preview");

        setPageNumber(pageSize,
            pageNumber,
            data.totalRecords,
            document.getElementById("AlbumsListPager"),
            "Album Images",
            "getAlbumImagesData");

        if (isPageChange) {
            const toggleBtn = document.querySelector(".albums-toggle"),
                dropdownEl = new bootstrap.Dropdown(toggleBtn);

            dropdownEl.toggle();
        }
    }).catch(function (error) {
        console.log(error);
        document.getElementById("PostAlbumsLoader").style.display = "none";
        placeHolder.textContent = error;
    });
}