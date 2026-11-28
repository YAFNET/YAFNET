import * as Utilities from '../forum/utilities';

document.addEventListener('DOMContentLoaded', () => {
	// Numeric Spinner Inputs
	document.querySelectorAll<HTMLInputElement>("input[type='number']").forEach((input) => {
		// create wrapper div if not exist
		if (!input.parentNode || !(input.parentNode as HTMLElement).classList.contains('input-group')) {
			const wrapDiv = document.createElement('div');
			wrapDiv.classList.add('input-group');

			Utilities.wrap(input, wrapDiv);
		}

		// Insert minus button
		const minusButton = document.createElement('button');

		minusButton.classList.add('btn', 'btn-secondary', 'bootstrap-touchspin-down');
		minusButton.type = 'button';
		minusButton.addEventListener('click', touchSpinDown);

		minusButton.innerHTML = '<i class="fa-solid fa-minus"></i>';

		input.parentNode?.insertBefore(minusButton, input);

		// Insert plus button
		const plusButton = document.createElement('button');

		plusButton.classList.add('btn', 'btn-secondary', 'bootstrap-touchspin-up');
		plusButton.type = 'button';
		plusButton.addEventListener('click', touchSpinUp);

		plusButton.innerHTML = '<i class="fa-solid fa-plus"></i>';

		input.parentNode?.append(plusButton);
	});

	function touchSpinDown(this: HTMLButtonElement) {
		const input = this.nextSibling as HTMLInputElement;
		const { min, max } = limits(input);
		const value = parseInt(input.value.trim());

		setValue(input, Math.min(Math.max(isNaN(value) ? min : value - 1, min), max));
	}

	function touchSpinUp(this: HTMLButtonElement) {
		const input = this.previousSibling as HTMLInputElement;
		const { min, max } = limits(input);
		const value = parseInt(input.value.trim());

		setValue(input, Math.max(Math.min(isNaN(value) ? Math.max(min, 0) : value + 1, max), min));
	}

	// the min and max attributes of the input, without them the old defaults (1 and the int maximum, +-720 minutes
	// for the server time correction)
	function limits(input: HTMLInputElement) {
		const serverTime = input.classList.contains('serverTime-Input');

		const attribute = (value: string, fallback: number) => {
			const number = parseInt(value);

			return isNaN(number) ? fallback : number;
		};

		return {
			min: attribute(input.min || (input.dataset.min ?? ''), serverTime ? -720 : 1),
			max: attribute(input.max, serverTime ? 720 : 2147483647)
		};
	}

	// like typing: listeners of the input (e.g. a live preview) get the new value
	function setValue(input: HTMLInputElement, value: number) {
		input.value = value.toString();

		input.dispatchEvent(new Event('input', { bubbles: true }));
		input.dispatchEvent(new Event('change', { bubbles: true }));
	}
});
