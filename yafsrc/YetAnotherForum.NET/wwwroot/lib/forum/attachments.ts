import { PagedResults } from './interfaces/PagedResults';
import { setPageNumber } from './paging';
import * as Utilities from '../forum/utilities';

export function getPaginationData(pageSize: number, pageNumber: number, isPageChange: boolean): void {
    const placeHolder = document.getElementById('PostAttachmentListPlaceholder') as HTMLElement;
    const list = document.querySelector('.AttachmentList') as HTMLElement;
    const yafUserId = placeHolder.dataset.userid || '0';
    const pagedResults: PagedResults = {
        UserId: parseInt(yafUserId),
        PageSize: pageSize,
        PageNumber: pageNumber
    };
    const ajaxUrl = '/api/Attachment/GetAttachments';

    fetch(ajaxUrl, {
        method: 'POST',
        body: JSON.stringify(pagedResults),
        headers: {
            Accept: 'application/json',
            "Content-Type": 'application/json;charset=utf-8',
            "RequestVerificationToken": (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value
        }
    }).then(res => res.json()).then(data => {
        list.innerHTML = '';

        document.getElementById('PostAttachmentLoader')!.style.display = 'none';

        if (data.attachmentList.length === 0) {
            const li = document.createElement('div');
            li.classList.add('col p-0');

            const noText = placeHolder.dataset.notext;
            const noAttachmentsText = noText || '';

            li.innerHTML = `<div class="alert alert-info text-break text-center" role="alert"><i class="fa fa-circle-info me-1"></i>${noAttachmentsText}</div></div>`;
            list.appendChild(li);
        }

        let count = 0;

        data.attachmentList.forEach((dataItem: { iconImage: string, onClick: string }) => {
            const li = document.createElement('div');

            li.classList.add('col-6', 'col-sm-4');
            li.style.cursor = 'pointer';

            li.setAttribute('onclick', dataItem.onClick);
            li.innerHTML = dataItem.iconImage;

            list.appendChild(li);
            count++;
        });

        Utilities.renderAttachPreview('.attachments-preview');

        setPageNumber(
            pageSize,
            pageNumber,
            data.totalRecords,
            document.getElementById('AttachmentsListPager') as HTMLDivElement,
            'Attachments',
            'getPaginationData'
        );
    }).catch((error) => {
        console.error(error);
        document.getElementById('PostAttachmentLoader')!.style.display = 'none';
        placeHolder.textContent = error.toString();
    });
}

const _global = (window /* browser */ || global /* node */) as any;
_global.getPaginationData = getPaginationData;