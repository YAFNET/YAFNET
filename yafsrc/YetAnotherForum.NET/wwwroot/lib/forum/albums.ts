import * as bootstrap from 'bootstrap';
import { PagedResults } from './interfaces/PagedResults';
import { renderAttachPreview } from './utilities';
import { setPageNumber } from './paging';

export function getAlbumImagesData(
    pageSize: number,
    pageNumber: number,
    isPageChange: boolean
): void {
    const placeHolder = document.getElementById('PostAlbumsListPlaceholder') as HTMLElement;
    const list = document.querySelector('.AlbumsList') as HTMLElement;
    const yafUserId = placeHolder.dataset.userid || '0';
    const ajaxUrl = '/api/Album/GetAlbumImages';

    const pagedResults: PagedResults = {
        UserId: parseInt(yafUserId),
        PageSize: pageSize,
        PageNumber: pageNumber,
    };

    fetch(ajaxUrl, {
        method: 'POST',
        body: JSON.stringify(pagedResults),
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json;charset=utf-8',
            'RequestVerificationToken': (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement)?.value || '',
        },
    })
        .then((res) => res.json())
        .then((data: { attachmentList: { onClick: string; iconImage: string }[]; totalRecords: number }) => {
            list.innerHTML = '';

            (document.getElementById('PostAlbumsLoader') as HTMLElement).style.display = 'none';

            data.attachmentList.forEach((dataItem) => {
                const li = document.createElement('div');

                li.classList.add('col-6', 'col-sm-4', 'p-1');
                li.style.cursor = 'pointer';
                li.setAttribute('onclick', dataItem.onClick);
                li.innerHTML = dataItem.iconImage;

                list.appendChild(li);
            });

            renderAttachPreview('.attachments-preview');

            setPageNumber(
                pageSize,
                pageNumber,
                data.totalRecords,
                document.getElementById('AlbumsListPager') as HTMLDivElement,
                'Album Images',
                'getAlbumImagesData'
            );

            if (isPageChange && document.querySelector('.albums-toggle') !== null) {
                const toggleBtn = document.querySelector('.albums-toggle') as HTMLElement;
                const dropdownEl = new bootstrap.Dropdown(toggleBtn);
                dropdownEl.toggle();
            }
        })
        .catch((error: Error) => {
            console.error(error);
            (document.getElementById('PostAlbumsLoader') as HTMLElement).style.display = 'none';
            placeHolder.textContent = error.message;
        });
}

const _global = (window /* browser */ || global /* node */) as any;
_global.getAlbumImagesData = getAlbumImagesData;