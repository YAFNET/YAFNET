import '../prism.js';
import * as Attachments from '../forum/attachments';
import * as Albums from '../forum/albums';
import * as Utilities from '../forum/utilities';
import '../forum/similarTitles';
import '@w8tcha/bs5-lightbox';

const _global = (window /* browser */ || global /* node */) as any;

document.addEventListener('DOMContentLoaded', () => {
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

	document.querySelectorAll<HTMLElement>('.attachedImage').forEach(imageLink => {
		var parentNode = (imageLink.parentNode as HTMLElement);
		const messageId = parentNode?.id as string;

		imageLink.setAttribute('data-gallery', `gallery-${messageId}`);
	});
});