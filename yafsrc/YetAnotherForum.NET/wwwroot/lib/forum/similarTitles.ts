import * as Utilities from '../forum/utilities';

document.addEventListener('DOMContentLoaded', () => {
	if (document.querySelector('.searchSimilarTopics') != null) {

		const input = document.querySelector('.searchSimilarTopics') as HTMLInputElement;

		// the api allows only a few requests per minute: search when the typing pauses, not on every key
		let timer: number | undefined;

		input.addEventListener('keyup', () => {
			window.clearTimeout(timer);
			timer = window.setTimeout(searchSimilar, 500);
		});

		function searchSimilar() {

			const placeHolder = document.getElementById('SearchResultsPlaceholder')!,
				searchText = input.value;

			if (searchText.length && searchText.length >= 4) {

				const searchTopic: { ForumId: number; PageSize: number; Page: number; SearchTerm: string } = {
					ForumId: 0,
					PageSize: 0,
					Page: 0,
					SearchTerm: searchText
				};

				const ajaxUrl = '/api/Search/GetSimilarTitles';

				fetch(ajaxUrl,
					{
						method: 'POST',
						body: JSON.stringify(searchTopic),
						headers: {
							Accept: 'application/json',
							"Content-Type": 'application/json;charset=utf-8',
							"RequestVerificationToken": (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value
						}
					}).then(res => {
						if (!res.ok) {
							throw new Error(res.statusText);
						}

						return res.json();
					}).then(data => {

					// a newer text was typed while this search ran
					if (input.value !== searchText) {
						return;
					}


					Utilities.empty(placeHolder);

					placeHolder.classList.remove('list-group');

					if (data.totalRecords > 0) {
						var list = document.createElement('ul');

						// header
						var liHeader = document.createElement('li');
						liHeader.classList.add('list-group-item', 'fw-bold')
						liHeader.innerHTML = placeHolder.dataset.title!;
						list.appendChild(liHeader);

						list.classList.add('list-group','list-similar');

						if (data.searchResults.length > 0) {
							data.searchResults.forEach((dataItem: any) => {
								var li = document.createElement('li');

								li.classList.add('list-group-item','list-group-item-action');

								li.innerHTML = `<a href="${dataItem.topicUrl}" target="_blank">${dataItem.topic}</a>`;

								list.appendChild(li);
							});
						}

						placeHolder.appendChild(list);
					}
				}).catch(error => {
					console.log(error);
					Utilities.empty(placeHolder);
				});
			}
		}
	}
});