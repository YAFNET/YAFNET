function getPaginationData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById("PostAttachmentListPlaceholder"),
        list = document.querySelector("#PostAttachmentListPlaceholder ul"),
        yafUserId = placeHolder.dataset.userid,
        pagedResults = {},
        ajaxUrl = "/api/Attachment/GetAttachments";

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

        document.getElementById("PostAttachmentLoader").style.display = "none";

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
            document.getElementById("AttachmentsListPager"),
            "Attachments",
            "getPaginationData");

        if (isPageChange) {
            const toggleBtn = document.querySelector(".attachments-toggle"),
                dropdownEl = new bootstrap.Dropdown(toggleBtn);

            dropdownEl.toggle();
        }
    }).catch(function (error) {
        console.log(error);
        document.getElementById("PostAttachmentLoader").style.display = "none";
        placeHolder.textContent = error;
    });
}