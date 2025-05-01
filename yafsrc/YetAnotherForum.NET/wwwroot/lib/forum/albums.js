function getAlbumImagesData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById('PostAlbumsListPlaceholder'),
        list = document.querySelector('.AlbumsList'),
        yafUserId = placeHolder.dataset.userid,
        pagedResults = {},
        ajaxUrl = '/api/Album/GetAlbumImages';

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

        document.getElementById('PostAlbumsLoader').style.display = 'none';

        data.attachmentList.forEach((dataItem) => {
            var li = document.createElement('div');

            li.classList.add('col-6');
            li.classList.add('col-sm-4');
            li.classList.add('p-1');

            li.style.cursor = 'pointer';

            li.setAttribute('onclick', dataItem.onClick);

            li.innerHTML = dataItem.iconImage;

            list.appendChild(li);
        });

        renderAttachPreview('.attachments-preview');

        setPageNumber(pageSize,
            pageNumber,
            data.totalRecords,
            document.getElementById('AlbumsListPager'),
            'Album Images',
            'getAlbumImagesData');

        if (isPageChange && document.querySelector('.albums-toggle') != null) {
            const toggleBtn = document.querySelector('.albums-toggle'),
                dropdownEl = new bootstrap.Dropdown(toggleBtn);

            dropdownEl.toggle();
        }
    }).catch(function (error) {
        console.log(error);
        document.getElementById('PostAlbumsLoader').style.display = 'none';
        placeHolder.textContent = error;
    });
}