import * as bootstrap from 'bootstrap';
import * as Utilities from '../forum/utilities';

import {serialize} from '../form-serialize/index.js';

const _global = (window /* browser */ || global /* node */) as any;

document.addEventListener('DOMContentLoaded',
	() => {
		var placeholderElement = (document.getElementById('modal-placeholder') as HTMLDivElement);

		if (placeholderElement) {
			document.querySelectorAll<HTMLElement>('button[data-bs-toggle="ajax-modal"],a[data-bs-toggle="ajax-modal"]').forEach(
				button => {
					button.addEventListener('click',
						(event) => {
							event.preventDefault();
							const url = button.dataset.url!;

							fetch(url).then(res => res.text()).then(data => {
								placeholderElement.innerHTML = data;

								loadModal(_global.dialog = new bootstrap.Modal(placeholderElement.querySelector('.modal')!),
									placeholderElement);
							}).catch(error => {
								console.log(error);
							});
						});
				});
		}

		if (document.querySelector('[data-bs-save="quickReply"]') !== null) {
			const replyButton = document.querySelector<HTMLButtonElement>('[data-bs-save="quickReply"]') as HTMLButtonElement;
			replyButton.addEventListener('click', (event) => {
				replyButton.innerHTML =
					"<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";
				replyButton.classList.add('disabled');

				event.preventDefault();

				const form = document.getElementById('quickReply')?.querySelector('form') as HTMLFormElement;
				const actionUrl = form.action;

				var formData: string = serialize(form,
					{
						hash: true
					});

				fetch(actionUrl, {
					method: 'POST',
					body: formData,
					headers: {
						'Accept': 'application/json',
						'Content-Type': 'application/json;charset=utf-8',
						'RequestVerificationToken': (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value
					}
				}).then(res => res.json())
					.then((response: any) => {
						if (response) {
							if (response.messageType) {
								_global.showModalNotify(response.messageType, response.message, '#quickReply form');
							} else {
								window.location.href = response;
							}
						} else {
							window.location.href = window.location.pathname + window.location.search;
						}
					});
			});
		}
	});
function loadModal(modal: any, placeholderElement: HTMLDivElement) {
	modal.show();

	modal._element.addEventListener('shown.bs.modal', (event: Event) => {
		const target = event.target as HTMLElement;
		if (target.id === 'LoginBox') {
			Utilities.togglePassword();

			const form = document.querySelector('.modal.show')?.querySelector('form') as HTMLFormElement;
			form.addEventListener('submit', (e: Event) => {
				if (!form.checkValidity()) {
					e.preventDefault();
					e.stopPropagation();
				}

				form.classList.add('was-validated');
			}, false);
		} else {
			_global.dialogFunctions(event);
		}
	});

	if (placeholderElement.querySelector('[data-bs-save="modal"]')) {
		placeholderElement.querySelector('[data-bs-save="modal"]')!.addEventListener('click',
			(event) => {
				event.preventDefault();

				const form = document.querySelector('.modal.show')!.querySelector('form') as HTMLFormElement;
				const actionUrl = form.action;

				var formData: string = serialize(form,
					{
						hash: true
					});

				fetch(actionUrl, {
						method: 'POST',
						body: formData,
						headers: {
							'Accept': 'application/json',
							'Content-Type': 'application/json;charset=utf-8',
							'RequestVerificationToken': (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value
						}
					}).then(res => res.json())
					.then((response: any) => {
						if (response) {
							if (response.messageType) {
								_global.showModalNotify(response.messageType, response.message, '.modal.show form');
							} else {
								window.location.href = response;
							}
						} else {
							window.location.href = window.location.pathname + window.location.search;
						}
					});
			});
	}

	if (placeholderElement.querySelector('[data-bs-save="editModal"]')) {
		placeholderElement.querySelector('[data-bs-save="editModal"]')!.addEventListener('click',
			(event) => {
				event.preventDefault();
				event.stopPropagation();

				const form = document.querySelector('.modal.show')?.querySelector('form') as HTMLFormElement;
				const actionUrl = form.action;

				form.classList.add('was-validated');

				if (!form.checkValidity()) {
					return;
				}

				var formData: string = serialize(form,
					{
						hash: true
					});

				fetch(actionUrl,
						{
							method: 'POST',
							body: formData,
							headers: {
								'Accept': 'application/json',
								'Content-Type': 'application/json;charset=utf-8',
								'RequestVerificationToken': (document.querySelector(
									'input[name="__RequestVerificationToken"]') as HTMLInputElement).value
							}
						}).then(res => res.json())
					.then(response => {
						if (response) {
							if (response.messageType) {
								_global.showModalNotify(response.messageType, response.message, '.modal.show form');
							} else {
								window.location.href = response;
							}
						} else {
							window.location.href = window.location.pathname + window.location.search;
						}
					}).catch(() => {
						window.location.href = window.location.pathname + window.location.search;
					});
			});
	}

	if (placeholderElement.querySelector('[data-bs-save="importModal"]')) {
		placeholderElement.querySelector('[data-bs-save="importModal"]')!.addEventListener('click',
			(event) => {
				event.preventDefault();
				event.stopPropagation();

				var form = document.querySelector('.modal.show')!.querySelector<HTMLFormElement>('form')!;
				const actionUrl = form.action,
					formData = new FormData(),
					fileInput = (document.getElementById('Import') as HTMLInputElement)!;

				if (fileInput.files) {
					formData.append('file', fileInput.files[0]);

					fetch(actionUrl,
							{
								method: 'POST',
								body: formData,
								headers: {
									'RequestVerificationToken': document.querySelector<HTMLInputElement>('input[name="__RequestVerificationToken"]')!.value
								}
							}).then(res => res.json())
						.then(response => {
							if (response) {
								if (response.messageType) {
									_global.showModalNotify(response.messageType, response.message, '.modal.show form');
								} else {
									window.location.href = response;
								}
							} else {
								window.location.href = window.location.pathname + window.location.search;
							}
						}).catch(() => {
							window.location.href = window.location.pathname + window.location.search;
						});
				}
			});
	}
}