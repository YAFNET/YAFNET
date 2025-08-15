import * as Utilities from '../forum/utilities';
import { Modal } from 'bootstrap';

const _global = (window /* browser */ || global /* node */) as any;

function setSearchPageNumber(pageSize: number, pageNumber: number, total: number): void {
		const pages = Math.ceil(total / pageSize),
			pagerHolderTop = document.getElementById('SearchResultsPagerTop') as HTMLDivElement,
			pagerHolderBottom = document.getElementById('SearchResultsPagerBottom') as HTMLDivElement,
			pagination: HTMLUListElement = document.createElement('ul'),
			paginationNavTop: HTMLElement = document.createElement('nav'),
			paginationNavBottom: HTMLElement = document.createElement('nav');

		paginationNavTop.setAttribute('aria-label', 'Search Page Results');
		paginationNavBottom.setAttribute('aria-label', 'Search Page Results');

		pagination.classList.add('pagination');

		Utilities.empty(pagerHolderTop);
		Utilities.empty(pagerHolderBottom);

		if (pageNumber > 0) {
			const page: HTMLLIElement = document.createElement('li');

			page.classList.add('page-item');

			page.innerHTML =
				`<a href="javascript:getSearchResultsData(${pageNumber - 1
				})" class="page-link"><i class="fas fas fa-angle-left" aria-hidden="true"></i></a>`;

			pagination.appendChild(page);
		}

		let start = pageNumber - 2;
		let end = pageNumber + 3;

		if (start < 0) {
			start = 0;
		}

		if (end > pages) {
			end = pages;
		}

		if (start > 0) {
			let page: HTMLLIElement = document.createElement('li');

			page.classList.add('page-item');
			page.innerHTML = `<a href="javascript:getSearchResultsData(${0});" class="page-link">1</a>`;

			pagination.appendChild(page);

			page = document.createElement('li');

			page.classList.add('page-item');
			page.classList.add('disabled');

			page.innerHTML = '<a class="page-link" href="#" tabindex="-1">...</a>';

			pagination.appendChild(page);
		}

		for (let i = start; i < end; i++) {
			if (i === pageNumber) {
				const page: HTMLLIElement = document.createElement('li');

				page.classList.add('page-item');
				page.classList.add('active');

				page.innerHTML = `<span class="page-link">${i + 1}</span>`;

				pagination.appendChild(page);
			} else {
				const page: HTMLLIElement = document.createElement('li');

				page.classList.add('page-item');
				page.innerHTML = `<a href="javascript:getSearchResultsData(${i});" class="page-link">${i + 1}</a>`;

				pagination.appendChild(page);
			}
		}

		if (end < pages) {
			let page: HTMLLIElement = document.createElement('li');

			page.classList.add('page-item');
			page.classList.add('disabled');
			page.innerHTML = '<a class="page-link" href="#" tabindex="-1">...</a>';

			pagination.appendChild(page);

			page = document.createElement('li');

			page.classList.add('page-item');
			page.innerHTML = `<a href="javascript:getSearchResultsData(${pages - 1})" class="page-link">${pages}</a>`;

			pagination.appendChild(page);
		}

		if (pageNumber < pages - 1) {
			const page: HTMLLIElement = document.createElement('li');

			page.classList.add('page-item');
			page.innerHTML =
				`<a href="javascript:getSearchResultsData(${pageNumber + 1
				})" class="page-link"><i class="fas fas fa-angle-right" aria-hidden="true"></i></a>`;

			pagination.appendChild(page);
		}

		paginationNavTop.appendChild(pagination);

		paginationNavBottom.innerHTML = paginationNavTop.innerHTML;

		if (pagerHolderTop) {
			pagerHolderTop.appendChild(paginationNavTop);
		}
		if (pagerHolderBottom) {
			pagerHolderBottom.appendChild(paginationNavBottom);
		}
	}

function getSearchResultsData(pageNumber: number): void {
		const searchInput = (document.querySelector('.searchInput') as HTMLInputElement).value;
		const searchInputUser = (document.querySelector('.searchUserInput') as HTMLInputElement).value;
		const searchInputTag = (document.querySelector('.searchTagInput') as HTMLInputElement).value;
		const placeHolder = document.getElementById('SearchResultsPlaceholder') as HTMLDivElement;
		const ajaxUrl = '/api/Search/GetSearchResults';
		const loadModal: bootstrap.Modal = new Modal('#loadModal');

		const useDisplayName =
			(document.querySelector('.searchUserInput') as HTMLInputElement).dataset.display === 'True';

		// filter options
		const pageSize = parseInt((document.querySelector('.resultsPage') as HTMLInputElement).value);
		const titleOnly = (document.querySelector('.titleOnly') as HTMLInputElement).value;
		const searchWhat = (document.querySelector('.searchWhat') as HTMLInputElement).value;

		const minimumLength = placeHolder.dataset.minimum as string;

		// Forum Filter
		const searchForum = (document.getElementById('Input_ForumListSelected') as HTMLInputElement).value === ''
			? 0
			: parseInt((document.getElementById('Input_ForumListSelected') as HTMLInputElement).value);

		let searchText = '';

		if (searchInput.length && searchInput.length >= parseInt(minimumLength) ||
			searchInputUser.length && searchInputUser.length >= parseInt(minimumLength) ||
			searchInputTag.length && searchInputTag.length >= parseInt(minimumLength)) {

			let replace: string;

			if (searchInput.length && searchInput.length >= parseInt(minimumLength)) {
				if (titleOnly === '1') {
					// ADD Topic Filter
					if (searchWhat === '0') {
						// Match all words
						replace = searchInput;
						searchText += ` Topic: (${replace.replace(/(^|\s+)/g, '$1+')})`;
					} else if (searchWhat === '1') {
						// Match Any Word
						searchText += ` Topic: ${searchInput}`;
					} else if (searchWhat === '2') {
						// Match Exact Phrase
						searchText += ` Topic:"${searchInput}"`;
					}
				} else {
					if (searchWhat === '0') {
						// Match all words
						replace = searchInput;
						searchText += `(${replace.replace(/(^|\s+)/g, '$1+')})`;
					} else if (searchWhat === '1') {
						// Match Any Word
						searchText += `${searchInput}`;
					} else if (searchWhat === '2') {
						// Match Exact Phrase
						searchText += `"${searchInput}"`;
					}
				}
			}

			if (searchInputUser.length && searchInputUser.length >= parseInt(minimumLength)) {
				const author: string = useDisplayName ? 'AuthorDisplay' : 'Author';

				if (searchText.length) searchText += ' ';

				if (searchInput.length) {
					searchText += `AND ${author}:${searchInputUser}`;
				} else {
					searchText = `+${author}:${searchInputUser}`;
				}
			}

			if (searchInputTag.length && searchInputTag.length >= parseInt(minimumLength)) {
				if (searchText.length) searchText += ' ';

				if (searchInput.length) {
					searchText += `AND TopicTags:${searchInputTag}`;
				} else {
					searchText = `+TopicTags:${searchInputTag}`;
				}
			}

			const searchTopic: { ForumId: number; PageSize: number; Page: number; SearchTerm: string } = {
				ForumId: searchForum,
				PageSize: pageSize,
				Page: pageNumber,
				SearchTerm: searchText
			};

			Utilities.empty(placeHolder);

			// show loading screen 
			loadModal.show();

			fetch(ajaxUrl,
				{
					method: 'POST',
					body: JSON.stringify(searchTopic),
					headers: {
						Accept: 'application/json',
						"Content-Type": 'application/json;charset=utf-8',
						"RequestVerificationToken": (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value
					}
				}).then(res => res.json()).then(data => {
				document.getElementById('loadModal')?.addEventListener('shown.bs.modal',
					() => {
						loadModal.hide();
					});

				const posted = placeHolder.dataset.posted || '';
				const by = placeHolder.dataset.by || '';
				const lastPost = placeHolder.dataset.lastpost || '';
				const topic = placeHolder.dataset.topic || '';

				if (data.searchResults.length === 0) {
					loadModal.hide();

					const noText = placeHolder.dataset.notext || '';

					const div: HTMLDivElement = document.createElement('div');
					div.innerHTML = `<div class="alert alert-warning text-center mt-3" role="alert">${noText}</div>`;

					placeHolder.appendChild(div);

					Utilities.empty(document.getElementById('SearchResultsPagerTop') as HTMLElement);
					Utilities.empty(document.getElementById('SearchResultsPagerBottom') as HTMLElement);

				} else {
					loadModal.hide();

					data.searchResults.forEach((dataItem: any) => {
						const item: HTMLDivElement = document.createElement('div');
						let tags = ' ';

						if (dataItem.topicTags) {
							const topicTags: string[] = dataItem.topicTags.split(',');

							topicTags.forEach((d: string) => {
								tags += `<span class='badge text-bg-secondary me-1'><i class='fas fa-tag me-1'></i>${d
									}</span>`;
							});
						}

						item.innerHTML = `
                <div class="row">
                    <div class="col">
                        <div class="card border-0 w-100 mb-3">
                            <div class="card-header bg-transparent border-top border-bottom-0 px-0 pb-0 pt-4 topicTitle">
                                <h5>
                                    <a title="${topic}" href="${dataItem.topicUrl}">${dataItem.topic}</a>&nbsp;
                                    <a title="${lastPost}" href="${dataItem.messageUrl
							}"><i class="fas fa-external-link-alt"></i></a>
                                    <small class="text-body-secondary">(<a href="${dataItem.forumUrl}">${dataItem
							.forumName}</a>)</small>
                                </h5>
                            </div>
                            <div class="card-body px-0">
                                <h6 class="card-subtitle mb-2 text-body-secondary">${dataItem.description}</h6>
                                <p class="card-text messageContent">${dataItem.message}</p>
                            </div>
                            <div class="card-footer bg-transparent border-top-0 px-0 py-2">
                                <small class="text-body-secondary">
                                    <span class="fa-stack">
                                        <i class="fa fa-calendar-day fa-stack-1x text-secondary"></i>
                                        <i class="fa fa-clock fa-badge text-secondary"></i>
                                    </span>
                                    ${posted} ${dataItem.posted} <i class="fa fa-user text-secondary"></i>${by} ${
							useDisplayName ? dataItem.userDisplayName : dataItem.userName}${tags}
                                </small>
                            </div>
                        </div>
                    </div>
                </div>`;

						placeHolder.appendChild(item);
					});

					setSearchPageNumber(pageSize, pageNumber, data.totalRecords);

					// Gallery
					document.querySelectorAll<HTMLElement>('[data-toggle="lightbox"]').forEach(element => {
						element.addEventListener('click', _global.bootstrap.Lightbox.initialize);
					});
				}
			}).catch((error: any) => {
				console.log(error);
				document.getElementById('SearchResultsPlaceholder')!.style.display = 'none';
				placeHolder.textContent = error;
			});
		}
	}


//////////////
const searchInput = document.querySelector('.searchInput') as HTMLInputElement;

searchInput.addEventListener('keypress', (event: KeyboardEvent) => {
	if (event.key === 'Enter') {
		event.preventDefault();
		const pageNumberSearch = 0;
		getSearchResultsData(pageNumberSearch);
	}
});

document.addEventListener('DOMContentLoaded', () => {
	var url = new URL(window.location.href);

	if (url.searchParams.has('search') ||
		url.searchParams.has('forum') ||
		url.searchParams.has('postedBy') ||
		url.searchParams.has('tag')) {
		getSearchResultsData(0);
	}
});