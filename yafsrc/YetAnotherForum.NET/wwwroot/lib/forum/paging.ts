import * as Utilities from '../forum/utilities';

export function setPageNumber(pageSize: number, pageNumber: number, total: number, pagerHolder: HTMLDivElement, label: string, javascriptFunction: string) {
    const pages = Math.ceil(total / pageSize),
        pagination = document.createElement('ul'),
        paginationNav = document.createElement('nav');

    paginationNav.setAttribute('aria-label', label + ' Page Results');

    pagination.classList.add('pagination');
    pagination.classList.add('pagination-sm');

    Utilities.empty(pagerHolder);

    if (pageNumber > 0) {
        const page = document.createElement('li');

        page.classList.add('page-item');

        page.innerHTML = `<a href="javascript:${javascriptFunction}(${pageSize},${pageNumber - 1},${total},true)" class="page-link"><i class="fas fa-angle-left"></i></a>`;

        pagination.appendChild(page);
    }
    var start = pageNumber - 2;
    var end = pageNumber + 3;
    if (start < 0) {
        start = 0;
    }
    if (end > pages) {
        end = pages;
    }
    if (start > 0) {
        let page = document.createElement('li');

        page.classList.add('page-item');
        page.innerHTML = `<a href="javascript:${javascriptFunction}(${pageSize},${0},${total}, true);" class="page-link">1</a>`;

        pagination.appendChild(page);

        page = document.createElement('li');

        page.classList.add('page-item');
        page.classList.add('disabled');

        page.innerHTML = '<a class="page-link" href="#" tabindex="-1">...</a>';

        pagination.appendChild(page);
    }
    for (var i = start; i < end; i++) {

        if (i === pageNumber) {
            const page = document.createElement('li');

            page.classList.add('page-item');
            page.classList.add('active');

            page.innerHTML = `<span class="page-link">${i + 1}</span>`;

            pagination.appendChild(page);
        } else {
            const page = document.createElement('li');

            page.classList.add('page-item');
            page.innerHTML = `<a href="javascript:${javascriptFunction}(${pageSize},${i},${total},true);" class="page-link">${i + 1}</a>`;

            pagination.appendChild(page);
        }
    }
    if (end < pages) {
        let page = document.createElement('li');

        page.classList.add('page-item');
        page.classList.add('disabled');
        page.innerHTML = '<a class="page-link" href="#" tabindex="-1">...</a>';

        pagination.appendChild(page);

        page = document.createElement('li');

        page.classList.add('page-item');
        page.innerHTML = `<a href="javascript:${javascriptFunction}(${pageSize},${pages - 1},${total},true)" class="page-link">${pages}</a>`;

        pagination.appendChild(page);
    }
    if (pageNumber < pages - 1) {
        const page = document.createElement('li');

        page.classList.add('page-item');
        page.innerHTML = `<a href="javascript:${javascriptFunction}(${pageSize},${pageNumber + 1},${total},true)" class="page-link"><i class="fas fa-angle-right"></i></a>`;

        pagination.appendChild(page);
    }

    paginationNav.appendChild(pagination);
    pagerHolder.appendChild(paginationNav);
}