import * as Utilities from '../forum/utilities';
import { Modal } from 'bootstrap';

const _global = (window /* browser */ || global /* node */) as any;

const searchTypeForums = '2';

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

		// no pager for a single page
		if (pages <= 1) {
			return;
		}

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

			// the gap only when pages are left out (not between the first and the second page)
			if (start > 1) {
				page = document.createElement('li');

				page.classList.add('page-item');
				page.classList.add('disabled');

				page.innerHTML = '<a class="page-link" href="#" tabindex="-1">...</a>';

				pagination.appendChild(page);
			}
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
			let page: HTMLLIElement;

			// the gap only when pages are left out (not between the page before the last one and the last one)
			if (end < pages - 1) {
				page = document.createElement('li');

				page.classList.add('page-item');
				page.classList.add('disabled');
				page.innerHTML = '<a class="page-link" href="#" tabindex="-1">...</a>';

				pagination.appendChild(page);
			}

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

// adds a required filter (user name or tag) to the search: the value is searched as phrase (a name can
// contain spaces), the filter is combined with the other search terms and filters
function addSearchFilter(searchText: string, field: string, value: string): string {
	const filter = `${field}:"${value.replace(/["\\]/g, '')}"`;

	return searchText.length ? `${searchText} AND ${filter}` : `+${filter}`;
}

function getSearchType(): string {
	return (document.querySelector('.searchType:checked') as HTMLInputElement).value;
}

// the search for forums only uses the search text, the other filters are for posts
function setSearchOptionsState(): void {
	const searchForums = getSearchType() === searchTypeForums;

	document.querySelectorAll<HTMLInputElement>('.searchUserInput, .searchTagInput, .searchWhat, .titleOnly')
		.forEach(element => {
			element.disabled = searchForums;
		});

	if (searchForums) {
		_global.forumsSelect?.disable();
	} else {
		_global.forumsSelect?.enable();
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
		const pageSize = parseInt((document.querySelector('.resultsPage:checked') as HTMLInputElement).value);
		const titleOnly = (document.querySelector('.titleOnly:checked') as HTMLInputElement).value;
		const searchWhat = (document.querySelector('.searchWhat:checked') as HTMLInputElement).value;
		const searchType = getSearchType();

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

				searchText = addSearchFilter(searchText, author, searchInputUser);
			}

			if (searchInputTag.length && searchInputTag.length >= parseInt(minimumLength)) {
				searchText = addSearchFilter(searchText, 'TopicTags', searchInputTag);
			}

			const searchTopic: { ForumId: number; PageSize: number; Page: number; SearchTerm: string; SearchType: number } = {
				ForumId: searchForum,
				PageSize: pageSize,
				Page: pageNumber,

				// the names of the forums are not searched with the search index, only the plain text is used
				SearchTerm: searchType === searchTypeForums ? searchInput : searchText,
				SearchType: parseInt(searchType)
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

				const by = placeHolder.dataset.by || '';
				const lastPost = placeHolder.dataset.lastpost || '';
				const topic = placeHolder.dataset.topic || '';
				const forum = placeHolder.dataset.forum || '';
				const results = placeHolder.dataset.results || '';

				const summary = document.getElementById('SearchResultsSummary') as HTMLElement;

				if (data.searchResults.length === 0) {
					loadModal.hide();

					const noText = placeHolder.dataset.notext || '';

					const div: HTMLDivElement = document.createElement('div');
					div.innerHTML = `<div class="search-empty" role="alert"><i class="fas fa-search fa-2x mb-2"></i><div>${noText}</div></div>`;

					placeHolder.appendChild(div);

					Utilities.empty(summary);
					Utilities.empty(document.getElementById('SearchResultsPagerTop') as HTMLElement);
					Utilities.empty(document.getElementById('SearchResultsPagerBottom') as HTMLElement);

				} else {
					loadModal.hide();

					summary.innerHTML = `${results} <span class="badge rounded-pill text-bg-secondary">${data.totalRecords}</span>`;

					const list: HTMLDivElement = document.createElement('div');
					list.classList.add('search-results', 'card', 'mb-3');

					data.searchResults.forEach((dataItem: any) => {
						const item: HTMLElement = document.createElement('article');
						item.classList.add('search-result');

						if (searchType === searchTypeForums) {
							item.innerHTML = `
                <a class="search-result-title" title="${forum}" href="${dataItem.forumUrl}"><i class="fas fa-comments me-1"></i>${dataItem.forumName}</a>
                <div class="search-result-meta">${dataItem.description}</div>`;

							list.appendChild(item);

							return;
						}

						let tags = '';

						if (dataItem.topicTags) {
							dataItem.topicTags.split(',').forEach((tag: string) => {
								tags += `<span class="badge search-result-tag"><i class="fas fa-tag me-1"></i>${tag}</span>`;
							});
						}

						const userName = useDisplayName ? dataItem.userDisplayName : dataItem.userName;

						// the author like the user links of the board (a guest without a link)
						const author = dataItem.userUrl
							? `<a href="${dataItem.userUrl}" class="btn btn-sm btn-outline-secondary search-result-user"${dataItem.userStyle ? ` style="${dataItem.userStyle}"` : ''}>${userName}</a>`
							: `<span class="search-result-user-guest">${userName}</span>`;

						item.innerHTML = `
                <div class="search-result-header">
                    <a class="search-result-title" title="${topic}" href="${dataItem.topicUrl}">${dataItem.topic}</a>
                    <a class="search-result-jump" title="${lastPost}" aria-label="${lastPost}" href="${dataItem.messageUrl}"><i class="fas fa-arrow-up-right-from-square"></i></a>
                </div>
                <div class="search-result-meta">
                    <a class="search-result-forum" title="${forum}" href="${dataItem.forumUrl}"><i class="fas fa-comments me-1"></i>${dataItem.forumName}</a>
                    <span class="search-result-author">${by} ${author}</span>
                    <span class="search-result-date"><i class="far fa-clock me-1"></i>${dataItem.posted}</span>
                    ${tags}
                </div>
                ${dataItem.description ? `<div class="search-result-description">${dataItem.description}</div>` : ''}
                <div class="search-result-message messageContent">${dataItem.message}</div>`;

						list.appendChild(item);
					});

					placeHolder.appendChild(list);

					setSearchPageNumber(pageSize, pageNumber, data.totalRecords);

					// Gallery
					document.querySelectorAll<HTMLElement>('[data-toggle="lightbox"]').forEach(element => {
						element.addEventListener('click', _global.bootstrap.Lightbox.initialize);
					});
				}
			}).catch((error: any) => {
				console.log(error);

				// the loading dialog is closed (also while it is still opening) and the error is shown in the
				// results, the next search shows its results again
				document.getElementById('loadModal')?.addEventListener('shown.bs.modal', () => loadModal.hide(), { once: true });
				loadModal.hide();

				Utilities.empty(document.getElementById('SearchResultsPagerTop') as HTMLElement);
				Utilities.empty(document.getElementById('SearchResultsPagerBottom') as HTMLElement);

				placeHolder.textContent = error;
			});
		}
	}


//////////////
// Enter in the text, the user or the tag field searches (it did nothing in the user and the tag field,
// a search by the user or the tag alone is possible)
document.querySelectorAll<HTMLInputElement>('.searchInput, .searchUserInput, .searchTagInput').forEach(input => {
	input.addEventListener('keypress', (event: KeyboardEvent) => {
		if (event.key === 'Enter') {
			event.preventDefault();
			const pageNumberSearch = 0;
			getSearchResultsData(pageNumberSearch);
		}
	});
});

document.querySelectorAll('.searchType, .searchWhat, .titleOnly, .resultsPage').forEach(element => {
	element.addEventListener('change', () => {
		setSearchOptionsState();
		getSearchResultsData(0);
	});
});

document.addEventListener('DOMContentLoaded', () => {
	setSearchOptionsState();

	var url = new URL(window.location.href);

	if (url.searchParams.has('search') ||
		url.searchParams.has('forum') ||
		url.searchParams.has('postedby') ||
		url.searchParams.has('tag')) {
		getSearchResultsData(0);
	}
});

_global.getSearchResultsData = getSearchResultsData;