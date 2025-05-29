/**
 * Lightbox for Bootstrap 5
 *
 * @file Creates a modal with a lightbox carousel.
 * @module bs5-lightbox
 */

import { Modal, Carousel } from 'bootstrap';
const bootstrap = {
	Modal,
	Carousel
};
class Lightbox {
	constructor(el, options = {}) {
		this.hash = this.randomHash();
		this.settings = Object.assign(Object.assign(Object.assign({}, bootstrap.Modal.Default), bootstrap.Carousel.Default), {
			interval: false,
			target: '[data-toggle="lightbox"]',
			gallery: '',
			size: 'xl',
			constrain: true
		});
		this.settings = Object.assign(Object.assign({}, this.settings), options);
		this.modalOptions = (() => this.setOptionsFromSettings(bootstrap.Modal.Default))();
		this.carouselOptions = (() => this.setOptionsFromSettings(bootstrap.Carousel.Default))();
		if (typeof el === 'string') {
			this.settings.target = el;
			el = document.querySelector(this.settings.target);
		}
		this.el = el;
		this.type = el.dataset.type || '';

		// check for data-size attribute
		if (el.dataset.size) {
			this.settings.size = el.dataset.size;
		}

		this.src = this.getSrc(el);
		this.sources = this.getGalleryItems();
		this.createCarousel();
		this.createModal();
	}
	show() {
		document.body.appendChild(this.modalElement);
		this.modal.show();
	}
	hide() {
		this.modal.hide();
	}
	setOptionsFromSettings(obj) {
		return Object.keys(obj).reduce((p, c) => Object.assign(p, { [c]: this.settings[c] }), {});
	}
	getSrc(el) {
		let src = el.dataset.src || el.dataset.remote || el.href || 'http://via.placeholder.com/1600x900';
		if (el.dataset.type === 'html') {
			return src;
		}
		if (!/\:\/\//.test(src)) {
			src = window.location.origin + src;
		}
		const url = new URL(src);
		if (el.dataset.footer || el.dataset.caption) {
			url.searchParams.set('caption', el.dataset.footer || el.dataset.caption);
		}
		return url.toString();
	}
	getGalleryItems() {
		let galleryTarget;
		if (this.settings.gallery) {
			if (Array.isArray(this.settings.gallery)) {
				return this.settings.gallery;
			}
			galleryTarget = this.settings.gallery;
		} else if (this.el.dataset.gallery) {
			galleryTarget = this.el.dataset.gallery;
		}
		const gallery = galleryTarget
			? [...new Set(Array.from(document.querySelectorAll(`[data-gallery="${galleryTarget}"]`), (v) => `${v.dataset.type ? v.dataset.type : ''}${this.getSrc(v)}`))]
			: [`${this.type ? this.type : ''}${this.src}`];
		return gallery;
	}
	getYoutubeId(src) {
		if (!src) return false;
		const matches = src.match(/^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/);
		return matches && matches[2].length === 11 ? matches[2] : false;
	}
	getYoutubeLink(src) {
		const youtubeId = this.getYoutubeId(src);
		if (!youtubeId) {
			return false;
		}

		const arr = src.split('?');
		let params = arr.length > 1 ? '?' + arr[1] : '';
		
		return `https://www.youtube.com/embed/${youtubeId}${params}`;
	}
	getInstagramEmbed(src) {
		if (/instagram/.test(src)) {
			src += /\/embed$/.test(src) ? '' : '/embed';
			return `<iframe src="${src}" class="start-50 translate-middle-x" style="max-width: 500px" frameborder="0" scrolling="no" allowtransparency="true"></iframe>`;
		}
	}
	isEmbed(src) {
		const regex = new RegExp('(' + Lightbox.allowedEmbedTypes.join('|') + ')');
		const isEmbed = regex.test(src);
		const isImg = /\.(png|jpe?g|gif|svg|webp)/i.test(src) || this.el.dataset.type === 'image';

		return isEmbed || !isImg;
	}
	createCarousel() {
		const template = document.createElement('template');
		const types = Lightbox.allowedMediaTypes.join('|');
		

		const slidesHtml = this.sources
			.map((src, i) => {
				src = src.replace(/\/$/, '');
				const regex = new RegExp(`^(${types})`, 'i');
				const isHtml = /^html/.test(src);
				const isForcedImage = /^image/.test(src);

				if (regex.test(src)) {
					src = src.replace(regex, '');
				}
				const imgClasses = this.settings.constrain ? 'mw-100 mh-100 h-auto w-auto m-auto top-0 end-0 bottom-0 start-0' : 'h-100 w-100';
				const params = new URLSearchParams(src.split('?')[1]);
				let caption = '';
				let url = src;
				if (params.get('caption')) {
					try {
						url = new URL(src);
						url.searchParams.delete('caption');
						url = url.toString();
					} catch (e) {
						url = src;
					}
					caption = `<div class="carousel-caption d-none d-md-block" style="z-index:2"><p class="bg-secondary rounded">${params.get('caption')}</p></div>`;
				}
				let inner = `<img src="${url}" class="d-block ${imgClasses} img-fluid" style="z-index: 1; object-fit: contain;" />`;
				let attributes = '';
				const instagramEmbed = this.getInstagramEmbed(src);
				const youtubeLink = this.getYoutubeLink(src);
				if (this.isEmbed(src) && !isForcedImage) {
					if (youtubeLink) {
						src = youtubeLink;
						attributes = 'title="YouTube video player" frameborder="0" allow="accelerometer autoplay clipboard-write encrypted-media gyroscope picture-in-picture"';
					}
					inner = instagramEmbed || `<img src="${src}" ${attributes} class="d-block mw-100 mh-100 h-auto w-auto m-auto top-0 end-0 bottom-0 start-0 img-fluid" style="z-index: 1; object-fit: contain;" />`;
				}
				if (isHtml) {
					inner = src;
				}
				const spinner = `<div class="position-absolute top-50 start-50 translate-middle text-white"><div class="spinner-border" style="width: 3rem height: 3rem" role="status"></div></div>`;
				return `
				<div class="carousel-item ${!i ? 'active' : ''}" style="min-height: 100px">
					${spinner}
					<div class="ratio ratio-16x9" style="background-color: #000;">${inner}</div>
					${caption}
				</div>`;
			})
			.join('');

        const controlsHtml =
            this.sources.length < 2
                ? ''
                : `
			<button id="#lightboxCarousel-${this.hash
                }-prev" class="carousel-control-prev" type="button" data-bs-target="#lightboxCarousel-${
                this.hash}" data-bs-slide="prev">
				<span class="btn btn-secondary carousel-control-prev-icon" aria-hidden="true"></span>
				<span class="visually-hidden">Previous</span>
			</button>
			<button id="#lightboxCarousel-${this.hash
                }-next" class="carousel-control-next" type="button" data-bs-target="#lightboxCarousel-${
                this.hash}" data-bs-slide="next">
				<span class="btn btn-secondary carousel-control-next-icon" aria-hidden="true"></span>
				<span class="visually-hidden">Next</span>
			</button>`;
    
		let classes = 'lightbox-carousel carousel slide';
		if (this.settings.size === 'fullscreen') {
			classes += ' position-absolute w-100 translate-middle top-50 start-50';
		}
        const indicatorsHtml = `
			<div class="carousel-indicators" style="bottom: -40px">
				${this.sources.map((_, index) => `
					<button type="button" data-bs-target="#lightboxCarousel-${this.hash}" data-bs-slide-to="${index}" class="${index === 0 ? 'active' : ''}" aria-current="${index === 0 ? 'true' : 'false'}" aria-label="Slide ${index + 1}"></button>
				`).join('')}
			</div>`;
		const html = `
			<div id="lightboxCarousel-${this.hash}" class="${classes}" data-bs-ride="carousel" data-bs-interval="${this.carouselOptions.interval}">
			    <div class="carousel-inner">
					${slidesHtml}
				</div>
			    ${indicatorsHtml}
				${controlsHtml}
			</div>`;
		template.innerHTML = html.trim();
		this.carouselElement = template.content.firstChild;
		const carouselOptions = Object.assign(Object.assign({}, this.carouselOptions), { keyboard: false });
		this.carousel = new bootstrap.Carousel(this.carouselElement, carouselOptions);
		const elSrc = this.type && this.type !== 'image' ? this.type + this.src : this.src;
		this.carousel.to(this.findGalleryItemIndex(this.sources, elSrc));
		if (this.carouselOptions.keyboard === true) {
			document.addEventListener('keydown', (e) => {
				if (e.code === 'ArrowLeft') {
					const prev = document.getElementById(`#lightboxCarousel-${this.hash}-prev`);
					if (prev) {
						prev.click();
					}
					return false;
				}
				if (e.code === 'ArrowRight') {
					const next = document.getElementById(`#lightboxCarousel-${this.hash}-next`);
					if (next) {
						next.click();
					}
					return false;
				}
			});
		}
		return this.carousel;
	}
	findGalleryItemIndex(haystack, needle) {
		let index = 0;
		for (const item of haystack) {
			if (item.includes(needle)) {
				return index;
			}
			index++;
		}
		return 0;
	}
	createModal() {
		const template = document.createElement('template');
		const html = `
			<div class="modal lightbox fade" id="lightboxModal-${this.hash}" tabindex="-1" aria-hidden="true">
				<div class="modal-dialog modal-dialog-centered modal-${this.settings.size}">
					<div class="modal-content border-0 bg-transparent">
						<div class="modal-body p-0">
							<button type="button" class="btn-close position-absolute p-3" data-bs-dismiss="modal" aria-label="Close" style="top: -15px;right:-40px"></button>
						</div>
					</div>
				</div>
			</div>`;
		template.innerHTML = html.trim();
		this.modalElement = template.content.firstChild;
		this.modalElement.querySelector('.modal-body').appendChild(this.carouselElement);
		this.modalElement.addEventListener('hidden.bs.modal', () => this.modalElement.remove());
		this.modalElement.querySelector('[data-bs-dismiss]').addEventListener('click', () => this.modal.hide());
		this.modal = new bootstrap.Modal(this.modalElement, this.modalOptions);
		return this.modal;
	}
	randomHash(length = 8) {
		return Array.from({ length }, () => Math.floor(Math.random() * 36).toString(36)).join('');
	}
}
Lightbox.allowedEmbedTypes = ['embed', 'youtube', 'vimeo', 'instagram', 'url'];
Lightbox.allowedMediaTypes = [...Lightbox.allowedEmbedTypes, 'image', 'html'];
Lightbox.defaultSelector = '[data-toggle="lightbox"]';
Lightbox.initialize = function (e) {
	e.preventDefault();
	const lightbox = new Lightbox(this);
	lightbox.show();
};
document.querySelectorAll(Lightbox.defaultSelector).forEach((el) => el.addEventListener('click', Lightbox.initialize));
if (typeof window !== 'undefined' && window.bootstrap) {
	window.bootstrap.Lightbox = Lightbox;
}
export default Lightbox;