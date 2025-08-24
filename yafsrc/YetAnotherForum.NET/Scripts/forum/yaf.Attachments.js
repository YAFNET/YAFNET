function getPaginationData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById('PostAttachmentListPlaceholder'),
        list = placeHolder.querySelector('.AttachmentList'),
        yafUserId = placeHolder.dataset.userid,
        pagedResults = {},
        ajaxUrl = placeHolder.dataset.url + 'api/Attachment/GetAttachments';

    pagedResults.UserId = yafUserId;
    pagedResults.PageSize = pageSize;
    pagedResults.PageNumber = pageNumber;

    fetch(ajaxUrl, {
        method: 'POST',
        body: JSON.stringify(pagedResults),
        headers: {
            Accept: 'application/json',
            "Content-Type": 'application/json;charset=utf-8'
        }
    }).then(res => res.json()).then(data => {

	    list.innerHTML = '';

        document.getElementById('PostAttachmentLoader').style.display = 'none';

        if (data.AttachmentList.length === 0) {
            const li = document.createElement('div');

            li.classList.add('col');

            const noText = placeHolder.dataset.notext;
            const noAttachmentsText = noText || '';

            li.innerHTML = `<div class="alert alert-info text-break" role="alert">${noAttachmentsText}</div>`;

            list.appendChild(li);
        }

        data.AttachmentList.forEach((dataItem) => {
            var li = document.createElement('div');

            li.classList.add('col-6');
            li.classList.add('col-sm-4');

            li.style.cursor = 'pointer';

            li.setAttribute('onclick', dataItem.OnClick);

            li.innerHTML = dataItem.IconImage;

            list.appendChild(li);
        });

        renderAttachPreview('.attachments-preview');

        setPageNumber(pageSize,
            pageNumber,
            data.TotalRecords,
            document.getElementById('AttachmentsListPager'),
            'Attachments',
            'getPaginationData');
    }).catch(function (error) {
        console.log(error);
        document.getElementById('PostAttachmentLoader').style.display = 'none';
        placeHolder.textContent = error;
    });
}