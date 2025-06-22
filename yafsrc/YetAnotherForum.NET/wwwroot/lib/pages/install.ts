import * as bootstrap from 'bootstrap';

const myModal = new bootstrap.Modal(document.querySelector('.yafWizard') as Element, {
	backdrop: 'static',
	keyboard: false
});

myModal.show();

document.querySelectorAll<HTMLButtonElement>('.btn-primary, .btn-info').forEach(button => {
	button.addEventListener('click', () => {
		// code…
		button.innerHTML =
			"<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";
	});
});