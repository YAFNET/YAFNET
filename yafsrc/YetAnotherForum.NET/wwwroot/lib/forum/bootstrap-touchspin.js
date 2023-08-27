document.addEventListener("DOMContentLoaded", function () {
    // Numeric Spinner Inputs
    document.querySelectorAll("input[type='number']").forEach(input => {
        // create wrapper div if not exist
        if (!input.parentNode.classList.contains("input-group")) {
            const wrapDiv = document.createElement("div");
            wrapDiv.classList.add("input-group");

            wrap(input, wrapDiv);
        }

        // Insert minus button
        const minusButton = document.createElement("button");

        minusButton.classList.add("btn");
        minusButton.classList.add("btn-secondary");
        minusButton.classList.add("bootstrap-touchspin-down");
        minusButton.type = "button";
        minusButton.addEventListener("click", touchSpinDown);

        minusButton.innerHTML = '<i class="fa-solid fa-minus"></i>';

        input.parentNode.insertBefore(minusButton, input);

        // Insert plus button
        const plusButton = document.createElement("button");

        plusButton.classList.add("btn");
        plusButton.classList.add("btn-secondary");
        plusButton.classList.add("bootstrap-touchspin-up");
        plusButton.type = "button";
        plusButton.addEventListener("click", touchSpinUp);

        plusButton.innerHTML = '<i class="fa-solid fa-plus"></i>';

        input.parentNode.insertBefore(plusButton, input.nextSibling);
    });

    function touchSpinDown() {
        const btn = this,
            input = btn.nextSibling,
            oldValue = input.value.trim();

        let newVal, minValue = 1;

        if (input.classList.contains("form-control-days")) {
            minValue = input.dataset.min;
        }
        else if (input.classList.contains("serverTime-Input")) {
            minValue = -720;
        }

        if (oldValue > minValue) {
            newVal = parseInt(oldValue) - 1;
        } else {
            newVal = minValue;
        }

        input.value = newVal;
    }

    function touchSpinUp() {
        const btn = this,
            input = btn.previousSibling,
            oldValue = input.value.trim();

        let maxValue = 2147483647;

        if (input.classList.contains("serverTime-Input")) {
            maxValue = -720;
        }

        if (oldValue <= maxValue) {
            const newVal = parseInt(oldValue) + 1;

            input.value = newVal;
        }
    }
});