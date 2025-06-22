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
		const btn = this;
		const input = btn.nextSibling as HTMLInputElement;
		const oldValue = input.value.trim();

		let minValue = 1;

		if (input.classList.contains('form-control-days')) {
			minValue = parseInt(input.dataset.min ?? '1');
		} else if (input.classList.contains('serverTime-Input')) {
			minValue = -720;
		}

		const newVal = parseInt(oldValue) > minValue ? parseInt(oldValue) - 1 : minValue;

		input.value = newVal.toString();
	}

	function touchSpinUp(this: HTMLButtonElement) {
		const btn = this;
		const input = btn.previousSibling as HTMLInputElement;
		const oldValue = input.value.trim();

		let maxValue = 2147483647;

		if (input.classList.contains('serverTime-Input')) {
			maxValue = -720;
		}

		if (parseInt(oldValue) <= maxValue) {
			const newVal = parseInt(oldValue) + 1;
			input.value = newVal.toString();
		}
	}
});
