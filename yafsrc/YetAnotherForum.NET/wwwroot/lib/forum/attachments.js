function getPaginationData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById('PostAttachmentListPlaceholder'),
        list = document.querySelector('.AttachmentList'),
        yafUserId = placeHolder.dataset.userid,
        pagedResults = {},
        ajaxUrl = '/api/Attachment/GetAttachments';

    pagedResults.UserId = yafUserId;
    pagedResults.PageSize = pageSize;
    pagedResults.PageNumber = pageNumber;

    fetch(ajaxUrl, {
        method: 'POST',
        body: JSON.stringify(pagedResults),
        headers: {
            Accept: 'application/json',
            "Content-Type": 'application/json;charset=utf-8',
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    }).then(res => res.json()).then(data => {

        list.innerHTML = '';

        document.getElementById('PostAttachmentLoader').style.display = 'none';

        if (data.attachmentList.length === 0) {
            const li = document.createElement('div');

            li.classList.add('col');

            const noText = placeHolder.dataset.notext;

            if (noText) {
	           noAttachmentsText = noText;
            }

            li.innerHTML = `<div class="alert alert-info text-break" role="alert" style="white-space:normal;width:200px">${noAttachmentsText}</div>`;

            list.appendChild(li);
        }

        var count = 0;

        data.attachmentList.forEach((dataItem) => {
            var li = document.createElement('div');

            li.classList.add('col-6');
            li.classList.add('col-sm-4');

            li.style.cursor = 'pointer';

            li.setAttribute('onclick', dataItem.onClick);

            li.innerHTML = dataItem.iconImage;

            list.appendChild(li);
            count++;
        });

        renderAttachPreview('.attachments-preview');

        setPageNumber(pageSize,
            pageNumber,
            data.totalRecords,
            document.getElementById('AttachmentsListPager'),
            'Attachments',
            'getPaginationData');
    }).catch(function (error) {
        console.log(error);
        document.getElementById('PostAttachmentLoader').style.display = 'none';
        placeHolder.textContent = error;
    });
}