import * as bootstrap from 'bootstrap';
import * as bootbox from '@w8tcha/bootbox';

export function errorLog(x: any): void {
    console.error('An Error has occurred!', x);
}

export function empty(wrap: HTMLElement): void {
	while (wrap.firstChild) {
		wrap.removeChild(wrap.firstChild);
	}
}

export function wrap(el: HTMLElement, wrapper: HTMLElement): void {
	el.parentNode?.insertBefore(wrapper, el);
	wrapper.appendChild(el);
}

// Attachments/Albums Popover Preview
export function renderAttachPreview(previewClass: string): void {
	const attachElements = document.querySelectorAll(previewClass);
	attachElements.forEach((attach) => {
		return new bootstrap.Popover(attach, {
			html: true,
			trigger: 'hover',
			placement: 'bottom',
			content: (): string => `<img src="${(attach as HTMLImageElement).src}" class="img-fluid" />`
		});
	});
}

// Toggle password visibility
export function togglePassword(): void {
	const passwordToggle = document.getElementById('PasswordToggle');
	if (passwordToggle && document.body.contains(passwordToggle)) {
		const icon = passwordToggle.querySelector('i') as HTMLElement;
		const pass = document.querySelector("input[id*='Password']") as HTMLInputElement;
		passwordToggle.addEventListener('click', (event): void => {
			event.preventDefault();
			if (pass.getAttribute('type') === 'text') {
				pass.setAttribute('type', 'password');
				icon.classList.add('fa-eye-slash');
				icon.classList.remove('fa-eye');
			} else if (pass.getAttribute('type') === 'password') {
				pass.setAttribute('type', 'text');
				icon.classList.remove('fa-eye-slash');
				icon.classList.add('fa-eye');
			}
		});
	}
}

// Confirm Dialog
document.addEventListener('click', (event) => {
	const target = event.target as HTMLElement;
	if (target.parentElement && target.parentElement.matches('[data-bs-toggle="confirm"]')) {
		event.preventDefault();
		const button = target.parentElement as HTMLElement;

		const text = button.dataset.title || '',
			yes = button.dataset.yes || '',
			no = button.dataset.no || '',
			title = button.innerHTML;

		button.innerHTML = "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";

		bootboxConfirm(button, title, text, yes, no, (confirmed: boolean) => {
			if (confirmed) {
				button.click();
			}
		});
	}
}, false);

export function bootboxConfirm (button: HTMLElement, title: string, message: string, yes: string, no: string, callback: (result: boolean) => void)  {
	const options: any = {
		message: message,
		centerVertical: true,
		title: title
	};
	options.buttons = {
		cancel: {
			label: `<i class="fa fa-times"></i> ${no}`,
			className: 'btn-danger',
			callback(result: any) {
				callback(false);
				button.innerHTML = title;
			}
		},
		main: {
			label: `<i class="fa fa-check"></i> ${yes}`,
			className: 'btn-success',
			callback(result: any) {
				callback(true);
			}
		}
	};
	bootbox.dialog(options);
};
export function bootboxShareTopic(title: string, message: string, cancel: string, okay: string, value: string) {
	const options: any = {
		message: message,
		centerVertical: true,
		title: title,
		value: value
	};
	options.buttons = {
		cancel: {
			label: cancel
		},
		confirm: {
			label: okay
		}
	};
	options.callback = () => {
		copyToClipBoard(value);
	};
	bootbox.prompt(options);
};

export function copyToClipBoard(input: string): void {
	navigator.clipboard.writeText(input);
}

const _global = (window /* browser */ || global /* node */) as any;

_global.errorLog = errorLog; 
_global.bootboxShareTopic = bootboxShareTopic;
_global.togglePassword = togglePassword;
_global.copyToClipBoard = copyToClipBoard;