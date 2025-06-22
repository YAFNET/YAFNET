import * as bootstrap from 'bootstrap';
import { PagedResults } from './interfaces/PagedResults';

export function getNotifyData(
    pageSize: number,
    pageNumber: number,
    isPageChange: boolean
): void {
    const placeHolder = document.getElementById('NotifyListPlaceholder') as HTMLElement;
    const list = placeHolder.querySelector('ul') as HTMLUListElement;
    const yafUserId = placeHolder.dataset.userid ?? '';
    const pagedResults: PagedResults = {
        UserId: parseInt(yafUserId),
        PageSize: pageSize,
        PageNumber: pageNumber,
    };
    const ajaxUrl = '/api/Notify/GetNotifications';

    fetch(ajaxUrl, {
        method: 'POST',
        body: JSON.stringify(pagedResults),
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json;charset=utf-8',
            'RequestVerificationToken': (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement)?.value ?? '',
        },
    })
        .then((res) => res.json())
        .then((data: { attachmentList: { fileName: string }[]; totalRecords: number }) => {
            empty(list);

            (document.getElementById('Loader') as HTMLElement).style.display = 'none';

            if (data.attachmentList.length > 0) {
                const markRead = document.getElementById('MarkRead') as HTMLElement;
                markRead.classList.remove('d-none');
                markRead.classList.add('d-block');

                data.attachmentList.forEach((dataItem) => {
                    const li = document.createElement('li');

                    li.classList.add('list-group-item', 'list-group-item-action', 'small', 'text-wrap');
                    li.style.width = '15rem';

                    li.innerHTML = dataItem.fileName;
                    list.appendChild(li);
                });

                setPageNumber(
                    pageSize,
                    pageNumber,
                    data.totalRecords,
                    document.getElementById('NotifyListPager') as HTMLElement,
                    'Notifications',
                    'getNotifyData'
                );

                if (isPageChange) {
                    const toggleBtn = document.querySelector('.notify-toggle') as HTMLElement;
                    const dropdownEl = new bootstrap.Dropdown(toggleBtn);
                    dropdownEl.toggle();
                }
            }
        })
        .catch((error) => {
            console.log(error);
            (document.getElementById('Loader') as HTMLElement).style.display = 'none';
            placeHolder.textContent = error.toString();
        });
}
