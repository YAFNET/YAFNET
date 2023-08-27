function getAlbumImagesData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById("PostAlbumsListPlaceholder"),
        list = placeHolder.querySelector("ul"),
        yafUserId = placeHolder.dataset.userid,
        pagedResults = {},
        ajaxUrl = placeHolder.dataset.url + "api/Album/GetAlbumImages";

    pagedResults.UserId = yafUserId;
    pagedResults.PageSize = pageSize;
    pagedResults.PageNumber = pageNumber;

    fetch(ajaxUrl, {
        method: "POST",
        body: JSON.stringify(pagedResults),
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json;charset=utf-8"
        }
    }).then(res => res.json()).then(data => {

        empty(list);

        document.getElementById("PostAlbumsLoader").style.display = "none";

        if (data.AttachmentList.length === 0) {
            const noText = placeHolder.dataset.notext;

            const li = document.createElement("li");

            li.innerHTML = `<li><div class="alert alert-info text-break" role="alert" style="white-space:normal">${noText}</div></li>`;

            list.appendChild(li);
        }

        data.AttachmentList.forEach((dataItem) => {
            var li = document.createElement("li");

            li.classList.add("list-group-item");
            li.classList.add("list-group-item-action");

            li.style.whiteSpace = "nowrap";
            li.style.cursor = "pointer";
            li.setAttribute("onclick", dataItem.OnClick);

            li.innerHTML = dataItem.IconImage;

            list.appendChild(li);
        });

        renderAttachPreview(".attachments-preview");

        setPageNumber(pageSize,
            pageNumber,
            data.TotalRecords,
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