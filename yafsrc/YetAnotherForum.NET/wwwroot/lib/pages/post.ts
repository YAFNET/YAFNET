import * as bootstrap from 'bootstrap';
import '../prism.js';
import * as Attachments from '../forum/attachments';
import * as Albums from '../forum/albums';
import * as Utilities from '../forum/utilities';
import '../forum/similarTitles';
import Lightbox from '@w8tcha/bs5-lightbox';

const _global = (window /* browser */ || global /* node */) as any;

document.addEventListener('DOMContentLoaded', () => {
	// Gallery
	document.querySelectorAll<HTMLElement>('[data-toggle="lightbox"]').forEach(element => {
		element.addEventListener('click', Lightbox.initialize);
	});

	_global.Prism.highlightAll();

	const postAttachmentListPlaceholder = document.getElementById('PostAttachmentListPlaceholder');
	if (postAttachmentListPlaceholder !== null) {
		const pageSize = 5;
		const pageNumber = 0;
		Attachments.getPaginationData(pageSize, pageNumber, false);
	}

	// Render Album Images DropDown
	const postAlbumsListPlaceholder = document.getElementById('PostAlbumsListPlaceholder');
	if (postAlbumsListPlaceholder !== null) {
		const pageSize = 5;
		const pageNumber = 0;
		Albums.getAlbumImagesData(pageSize, pageNumber, false);
	}

	Utilities.renderAttachPreview('.attachments-preview');

	document.querySelectorAll<HTMLElement>('.thanks-popover').forEach(thanks => {
		const popover = new bootstrap.Popover(thanks, {
			template: '<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body popover-body-scrollable"></div></div>'
		});

		thanks.addEventListener('show.bs.popover', () => {
			const messageId = thanks.dataset.messageid as string;

			fetch(`/api/ThankYou/GetThanks/${messageId}`, {
					method: 'POST',
					headers: {
						'Accept': 'application/json',
						'Content-Type': 'application/json;charset=utf-8',
						'RequestVerificationToken': (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value
					}
				})
				.then(res => res.json())
				.then(response => {
					const popoverList = document.getElementById(`popover-list-${messageId}`);
					if (popoverList) {
						popoverList.innerHTML = response.thanksInfo;
					}
				});
		});
	});

	document.querySelectorAll<HTMLElement>('.attachedImage').forEach(imageLink => {
		var parentNode = (imageLink.parentNode as HTMLElement);
		const messageId = parentNode?.id as string;

		imageLink.setAttribute('data-gallery', `gallery-${messageId}`);
	});
});