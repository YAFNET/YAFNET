import { PagedResults } from './interfaces/PagedResults';
import * as Utilities from '../forum/utilities';

/**
 * Loads the latest notifications into the notify dropdown (the full list is on the notification page)
 */
export function getNotifyData(pageSize: number): void {
    const placeHolder = document.getElementById('NotifyListPlaceholder') as HTMLElement;
    const pagedResults: PagedResults = {
        UserId: 0,
        PageSize: pageSize,
        PageNumber: 0,
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
            Utilities.empty(placeHolder);

            (document.getElementById('Loader') as HTMLElement).style.display = 'none';

            if (data.attachmentList.length > 0) {
                // each item is already a dropdown-item link, rendered by the server
                placeHolder.innerHTML = data.attachmentList
                    .map((dataItem) => dataItem.fileName)
                    .join('<div class="dropdown-divider"></div>');
            }
        })
        .catch((error) => {
            console.log(error);
            (document.getElementById('Loader') as HTMLElement).style.display = 'none';
            placeHolder.textContent = error.toString();
        });
}
