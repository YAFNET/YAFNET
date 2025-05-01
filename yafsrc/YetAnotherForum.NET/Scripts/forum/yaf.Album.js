function getAlbumImagesData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById('PostAlbumsListPlaceholder'),
        list = placeHolder.querySelector('.AlbumsList'),
        yafUserId = placeHolder.dataset.userid,
        pagedResults = {},
        ajaxUrl = placeHolder.dataset.url + 'api/Album/GetAlbumImages';

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

        document.getElementById('PostAlbumsLoader').style.display = 'none';

        data.AttachmentList.forEach((dataItem) => {
            var li = document.createElement('div');

            li.classList.add('col-6');
            li.classList.add('col-sm-4');
            li.classList.add('p-1');

            li.style.cursor = 'pointer';

            li.setAttribute('onclick', dataItem.OnClick);

            li.innerHTML = dataItem.IconImage;

            list.appendChild(li);
        });

        renderAttachPreview('.attachments-preview');

        setPageNumber(pageSize,
            pageNumber,
            data.TotalRecords,
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