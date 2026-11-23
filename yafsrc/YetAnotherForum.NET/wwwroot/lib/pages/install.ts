import * as bootstrap from 'bootstrap';

const myModal = new bootstrap.Modal(document.querySelector('.yafWizard') as Element, {
	backdrop: 'static',
	keyboard: false
});

myModal.show();

// the button shows a spinner while the next step loads: on the submit event, which is only raised when the form is
// valid (on the click the browser may still refuse the form, the button stayed "Loading..." then)
document.querySelectorAll<HTMLFormElement>('.yafWizard form').forEach(form => {
	form.addEventListener('submit', event => {
		const button = event.submitter as HTMLButtonElement | null;

		if (!button || !button.matches('.btn-primary, .btn-info, .btn-success') || button.querySelector('.spinner-border')) {
			return;
		}

		const spinner = document.createElement('span');
		spinner.className = 'spinner-border spinner-border-sm me-1';
		spinner.setAttribute('role', 'status');
		spinner.setAttribute('aria-hidden', 'true');

		button.querySelector('i')?.classList.add('d-none');
		button.prepend(spinner);
	});
});

// a page shown again from the back/forward cache has no spinner
window.addEventListener('pageshow', () => {
	document.querySelectorAll('.yafWizard .spinner-border').forEach(spinner => spinner.remove());
	document.querySelectorAll('.yafWizard button i.d-none').forEach(icon => icon.classList.remove('d-none'));
});
