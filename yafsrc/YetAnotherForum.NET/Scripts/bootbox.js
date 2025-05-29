/*! @preserve
 * bootbox.js
 * version: 6.0.4
 * author: Nick Payne <nick@kurai.co.uk>
 * license: MIT
 * http://bootboxjs.com/
 */
(function (root, factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // AMD
        define(['bootstrap'], factory);
    } else if (typeof exports === 'object') {
        // Node, CommonJS-like
        module.exports = factory(require('bootstrap'));
    } else {
        // Browser globals (root is window)
        root.bootbox = factory(root.bootstrap);
    }
}(this,
    function init($, undefined) {
        'use strict';

        const exports = {};

        const VERSION = '6.0.4';
        exports.VERSION = VERSION;

        const locales = {
            'en': {
                OK: 'OK',
                CANCEL: 'Cancel',
                CONFIRM: 'OK'
            }
        };

        const templates = {
            dialog:
                '<div class="bootbox modal" tabindex="-1" role="dialog" aria-hidden="true"><div class="modal-dialog"><div class="modal-content"><div class="modal-body"><div class="bootbox-body"></div></div></div></div></div>',
            header: '<div class="modal-header"><h5 class="modal-title"></h5></div>',
            footer: '<div class="modal-footer"></div>',
            closeButton:
                '<button type="button" class="bootbox-close-button close btn-close" aria-hidden="true" aria-label="Close"></button>',
            form: '<form class="bootbox-form"></form>',
            button: '<button type="button" class="btn"></button>',
            option: '<option value=""></option>',
            promptMessage: '<div class="bootbox-prompt-message"></div>',
            inputs: {
                text: '<input class="bootbox-input bootbox-input-text form-control" autocomplete="off" type="text" />',
                textarea: '<textarea class="bootbox-input bootbox-input-textarea form-control"></textarea>',
                email:
                    '<input class="bootbox-input bootbox-input-email form-control" autocomplete="off" type="email" />',
                select: '<select class="bootbox-input bootbox-input-select form-select"></select>',
                checkbox:
                    '<div class="form-check checkbox"><label class="form-check-label"><input class="form-check-input bootbox-input bootbox-input-checkbox" type="checkbox" /></label></div>',
                radio:
                    '<div class="form-check radio"><label class="form-check-label"><input class="form-check-input bootbox-input bootbox-input-radio" type="radio" name="bootbox-radio" /></label></div>',
                date: '<input class="bootbox-input bootbox-input-date form-control" autocomplete="off" type="date" />',
                time: '<input class="bootbox-input bootbox-input-time form-control" autocomplete="off" type="time" />',
                number:
                    '<input class="bootbox-input bootbox-input-number form-control" autocomplete="off" type="number" />',
                password:
                    '<input class="bootbox-input bootbox-input-password form-control" autocomplete="off" type="password" />',
                range:
                    '<input class="bootbox-input bootbox-input-range form-control-range" autocomplete="off" type="range" />'
            }
        };


        const defaults = {
            // Default language used when generating buttons for alert, confirm, and prompt dialogs
            locale: 'en',
            // Show backdrop or not. Default to static so user has to interact with dialog
            backdrop: 'static',
            // Animate the modal in/out
            animate: true,
            // Additional class string applied to the top level dialog
            className: null,
            // Whether or not to include a close button
            closeButton: true,
            // Show the dialog immediately by default
            show: true,
            // Dialog container
            container: 'body',
            // Default value (used by the prompt helper)
            value: '',
            // Default input type (used by the prompt helper)
            inputType: 'text',
            // Custom error message to report if prompt fails validation
            errorMessage: null,
            // Switch button order from cancel/confirm (default) to confirm/cancel
            swapButtonOrder: false,
            // Center modal vertically in page
            centerVertical: false,
            // Append "multiple" property to the select when using the "prompt" helper
            multiple: false,
            // Automatically scroll modal content when height exceeds viewport height
            scrollable: false,
            // Whether or not to destroy the modal on hide
            reusable: false,
            // The element which triggered the dialog
            relatedTarget: null,
            // The size of the modal to generate
            size: null,
            // A unique identifier for this modal
            id: null
        };


        // PUBLIC FUNCTIONS
        // *************************************************************************************************************

        /**
         * Return all currently registered locales, or a specific locale if "name" is defined
         * @param {string} [name]
         * @returns {(Object|Object[])} An array of the available locale objects, or a single locale object if {name} is not null
         */
        exports.locales = function(name) {
            return name ? locales[name] : locales;
        };


        /**
         * Register localized strings for the OK, CONFIRM, and CANCEL buttons
         * @param {string} name - The key used to identify the new locale in the locales array
         * @param {Object} values - An object containing the localized string for each of the OK, CANCEL, and CONFIRM properties of a locale
         * @returns The updated bootbox object
         */
        exports.addLocale = function(name, values) {
            ['OK', 'CANCEL', 'CONFIRM'].forEach((v, _) => {
                if (!values[v]) {
                    throw new Error(`Please supply a translation for "${v}"`);
                }
            });

            locales[name] = {
                OK: values.OK,
                CANCEL: values.CANCEL,
                CONFIRM: values.CONFIRM
            };

            return exports;
        };


        /**
         * Remove a previously-registered locale
         * @param {string} name - The key identifying the locale to remove
         * @returns The updated bootbox object
         */
        exports.removeLocale = function(name) {
            if (name !== 'en') {
                delete locales[name];
            } else {
                throw new Error('"en" is used as the default and fallback locale and cannot be removed.');
            }

            return exports;
        };


        /**
         * Set the default locale
         * @param {string} name - The key identifying the locale to set as the default locale for all future bootbox calls 
         * @returns The updated bootbox object
         */
        exports.setLocale = function(name) {
            return exports.setDefaults('locale', name);
        };


        /**
         * Override default value(s) of Bootbox.
         * @returns The updated bootbox object
         */
        exports.setDefaults = function() {
            let values = {};

            if (arguments.length === 2) {
                // Allow passing of single key/value...
                values[arguments[0]] = arguments[1];
            } else {
                // ... and as an object too
                values = arguments[0];
            }

            Object.assign(defaults, values);

            return exports;
        };


        /**
         * Hides all currently active Bootbox modals
         * @returns The current bootbox object
         */
        exports.hideAll = function() {
            document.querySelectorAll('.bootbox').forEach(box => {
                bootstrap.Modal.getInstance(box).hide();
            });

            return exports;
        };


        /**
         * Allows the base init() function to be overridden
         * @param {function} _$ - A function to be called when the bootbox instance is created
         * @returns The current bootbox object
         */
        exports.init = function(_$) {
            return init(_$ || $);
        };


        // CORE HELPER FUNCTIONS
        // *************************************************************************************************************

        /**
         * The core dialog helper function, which can be used to create any custom Bootstrap modal. 
         * @param {Object} options - An object used to configure the various properties which define a Bootbox dialog
         * @returns A jQuery object upon which Bootstrap's modal function has been called
         */
        exports.dialog = function(options) {
            if (bootstrap.Modal === undefined) {
                throw new Error(
                    '"bootstrap.Modal" is not defined; please double check you have included the Bootstrap JavaScript library. See https://getbootstrap.com/docs/5.3/getting-started/introduction/ for more details.'
                );
            }

            options = sanitize(options);

            if (bootstrap.Modal.VERSION) {
                options.fullBootstrapVersion = bootstrap.Modal.VERSION;
                let i = options.fullBootstrapVersion.indexOf('.');
                options.bootstrap = options.fullBootstrapVersion.substring(0, i);
            } else {
                // Assuming version 2.3.2, as that was the last "supported" 2.x version
                options.bootstrap = '2';
                options.fullBootstrapVersion = '2.3.2';
                console.warn(
                    'Bootbox will *mostly* work with Bootstrap 2, but we do not officially support it. Please upgrade, if possible.');
            }

            let dialog = generateElement(templates.dialog);
            let innerDialog = dialog.querySelector('.modal-dialog');
            let body = dialog.querySelector('.modal-body');
            let header = generateElement(templates.header);
            let footer = generateElement(templates.footer);
            let buttons = options.buttons;

            let callbacks = {
                onEscape: options.onEscape
            };

            if (typeof options.message === 'string') {
                body.querySelector('.bootbox-body').innerHTML = options.message;
            } else {
                body.querySelector('.bootbox-body').append(options.message);
            }

            // Only attempt to create buttons if at least one has been defined in the options object
            if (getKeyLength(options.buttons) > 0) {

                for (const [key, b] of Object.entries(buttons)) {
                    let button = generateElement(templates.button);
                    button.dataset.bbHandler = key;

                    var classNames = b.className.split(' ');
                    classNames.forEach(name => { button.classList.add(name);});

                    switch (key) {
                    case 'ok':
                    case 'confirm':
                        button.classList.add('bootbox-accept');
                        break;

                    case 'cancel':
                        button.classList.add('bootbox-cancel');
                        break;
                    }

                    button.innerHTML = b.label;

                    if (b.id) {
                        button.setAttribute({ 'id': b.id });
                    }

                    if (b.disabled === true) {
                        button.disabled = true;
                    }

                    footer.append(button);

                    callbacks[key] = b.callback;
                }

                body.after(footer);
            }

            if (options.animate === true) {
                dialog.classList.add('fade');
            }

            if (options.className) {
                options.className.split(' ').forEach(name => { dialog.classList.add(name); });
            }

            if (options.id) {
                dialog.setAttribute({ 'id': options.id });
            }

            if (options.size) {
                // Requires Bootstrap 3.1.0 or higher
                if (options.fullBootstrapVersion.substring(0, 3) < '3.1') {
                    console.warn(
                        `"size" requires Bootstrap 3.1.0 or higher. You appear to be using ${options
                        .fullBootstrapVersion}. Please upgrade to use this option.`);
                }

                switch (options.size) {
                case 'small':
                case 'sm':
                    innerDialog.classList.add('modal-sm');
                    break;

                case 'large':
                case 'lg':
                    innerDialog.classList.add('modal-lg');
                    break;

                case 'extra-large':
                case 'xl':
                    innerDialog.classList.add('modal-xl');

                    // Requires Bootstrap 4.2.0 or higher
                    if (options.fullBootstrapVersion.substring(0, 3) < '4.2') {
                        console.warn(
                            `Using size "xl"/"extra-large" requires Bootstrap 4.2.0 or higher. You appear to be using ${
                            options.fullBootstrapVersion
                            }. Please upgrade to use this option.`);
                    }
                    break;
                }
            }

            if (options.scrollable) {
                innerDialog.classList.add('modal-dialog-scrollable');

                // Requires Bootstrap 4.3.0 or higher
                if (options.fullBootstrapVersion.substring(0, 3) < '4.3') {
                    console.warn(
                        `Using "scrollable" requires Bootstrap 4.3.0 or higher. You appear to be using ${options
                        .fullBootstrapVersion}. Please upgrade to use this option.`);
                }
            }

            if (options.title || options.closeButton) {
                if (options.title) {
                    header.querySelector('.modal-title').innerHTML = options.title;
                } else {
                    header.classList.add('border-0');
                }

                if (options.closeButton) {
                    let closeButton = generateElement(templates.closeButton);
                    if (options.bootstrap < 5) {
                        closeButton.innerHTML = '&#215;';
                    }

                    /* Note: the close button for Bootstrap 5+ does not contain content */
                    if (options.bootstrap < 4) {
                        /* Bootstrap 3 and under */
                        header.prepend(closeButton);
                    } else {
                        header.append(closeButton);
                    }
                }

                body.before(header);
            }

            if (options.centerVertical) {
                innerDialog.classList.add('modal-dialog-centered');

                // Requires Bootstrap 4.0.0-beta.3 or higher
                if (options.fullBootstrapVersion < '4.0.0') {
                    console.warn(
                        `"centerVertical" requires Bootstrap 4.0.0-beta.3 or higher. You appear to be using ${
                        options.fullBootstrapVersion}. Please upgrade to use this option.`);
                }
            }

            // Bootstrap event listeners; these handle extra setup & teardown required after the underlying modal has performed certain actions.

            if (!options.reusable) {
                // make sure we unbind any listeners once the dialog has definitively been dismissed
                dialog.addEventListener('hide.bs.modal',
                    function() {

                        dialog.removeEventListener('escape.close.bb', null);
                        dialog.removeEventListener('click', null);
                    },
                    { once: true });

                dialog.addEventListener('hidden.bs.modal',
                    function() {

                        // Ensure we don't accidentally intercept hidden events triggered by children of the current dialog. 
                        // We shouldn't need to handle this anymore, now that Bootstrap namespaces its events, but still worth doing.
                        dialog.remove();

                        dialog = null;


                    },
                    { once: true });
            }

            if (options.onHide) {
                if (typeof options.onHide === 'function') {
                    dialog.addEventListener('hide.bs.modal', options.onHide);
                } else {
                    throw new Error('Argument supplied to "onHide" must be a function');
                }
            }

            if (options.onHidden) {
                if (typeof options.onHidden === 'function') {
                    dialog.addEventListener('hidden.bs.modal', options.onHidden);
                } else {
                    throw new Error('Argument supplied to "onHidden" must be a function');
                }
            }

            if (options.onShow) {
                if (typeof options.onShow === 'function') {
                    addEventListener(dialog, 'show.bs.modal', options.onShow);
                } else {
                    throw new Error('Argument supplied to "onShow" must be a function');
                }
            }

            dialog.addEventListener('shown.bs.modal', { dialog: dialog }, focusPrimaryButton);

            if (options.onShown) {
                if (typeof options.onShown === 'function') {
                    addEventListener(dialog, 'shown.bs.modal', options.onShown);
                } else {
                    throw new Error('Argument supplied to "onShown" must be a function');
                }
            }

            // Bootbox event listeners; used to decouple some behaviours from their respective triggers

            if (options.backdrop === true) {
                let startedOnBody = false;

                // Prevents the event from propagating to the backdrop, when something inside the dialog is clicked
                addEventListener(dialog,
                    'mousedown',
                    function(e) {
                        e.stopPropagation();

                        startedOnBody = true;
                    });

                // A boolean true/false according to the Bootstrap docs should show a dialog the user can dismiss by clicking on the background.
                // We always only ever pass static/false to the actual $.modal function because with "true" we can't trap this event (the .modal-backdrop swallows it).
                // However, we still want to sort-of respect true and invoke the escape mechanism instead
                addEventListener(dialog, 'click.dismiss.bs.modal', function (e) {
                    if (startedOnBody || e.target !== e.currentTarget) {
                        return;
                    }
                    trigger(dialog, 'escape.close.bb');
                });
            }

            dialog.addEventListener('escape.close.bb',
                function(e) {
                    // The if() statement looks redundant but it isn't; without it, if we *didn't* have an onEscape handler then processCallback would automatically dismiss the dialog
                    if (callbacks.onEscape) {
                       processCallback(e, dialog, callbacks.onEscape);
                    }
                });

            dialog.addEventListener('click',
                (e) => {
					if (e.target.nodeName.toLowerCase() === 'button' && !e.target.classList.contains('disabled')) {
                        const callbackKey = e.target.dataset.bbHandler;
						
                        if (callbackKey !== undefined) {

                            // Only process callbacks for buttons we recognize:
                            processCallback(e, dialog, callbacks[callbackKey]);
                        }
                    }
                });


            document.addEventListener('click',
                (e) => {
                    if (e.target.closest('.bootbox-close-button')) {
                        processCallback(e, dialog, callbacks.onEscape);
                    }
                });

            dialog.addEventListener('keyup',
                function(e) {
                    if (e.which === 27) {
                        trigger(dialog, 'escape.close.bb');
                    }
                });

            /*
            The remainder of this method simply deals with adding our dialog element to the DOM, augmenting it with 
            Bootstrap's modal functionality and then giving the resulting object back to our caller
            */

            if (typeof options.container === 'object') {
               options.container.append(dialog);
            } else {
                document.querySelector(options.container).append(dialog);
            }
            

            const modal = new bootstrap.Modal(dialog,
                {
                    backdrop: options.backdrop,
                    keyboard: false,
                    show: false
                });

            if (options.show) {
                modal.show(options.relatedTarget);
            }

            return dialog;
        };


        /**
         * Helper function to simulate the native alert() behavior. **NOTE**: This is non-blocking, so any code that must happen after the alert is dismissed should be placed within the callback function for this alert.
         * @returns  A jQuery object upon which Bootstrap's modal function has been called
         */
        exports.alert = function() {
            let options;

            options = mergeDialogOptions('alert', ['ok'], ['message', 'callback'], arguments);

            // @TODO: can this move inside exports.dialog when we're iterating over each button and checking its button.callback value instead?
            if (options.callback && typeof options.callback !== 'function') {
                throw new Error('alert requires the "callback" property to be a function when provided');
            }

            // Override the ok and escape callback to make sure they just invoke the single user-supplied one (if provided)
            options.buttons.ok.callback = options.onEscape = function() {
                if (typeof options.callback === 'function') {
                    return options.callback.call(this);
                }

                return true;
            };

            return exports.dialog(options);
        };


        /**
         * Helper function to simulate the native confirm() behavior. **NOTE**: This is non-blocking, so any code that must happen after the confirm is dismissed should be placed within the callback function for this confirm.
         * @returns A jQuery object upon which Bootstrap's modal function has been called
         */
        exports.confirm = function() {
            let options;

            options = mergeDialogOptions('confirm', ['cancel', 'confirm'], ['message', 'callback'], arguments);

            // confirm specific validation; they don't make sense without a callback so make sure it's present
            if (typeof options.callback !== 'function') {
                throw new Error('confirm requires a callback');
            }

            // Overrides; undo anything the user tried to set they shouldn't have
            options.buttons.cancel.callback = options.onEscape = function() {
                return options.callback.call(this, false);
            };

            options.buttons.confirm.callback = function() {
                return options.callback.call(this, true);
            };

            return exports.dialog(options);
        };


        /**
         * Helper function to simulate the native prompt() behavior. **NOTE**: This is non-blocking, so any code that must happen after the prompt is dismissed should be placed within the callback function for this prompt.
         * @returns A jQuery object upon which Bootstrap's modal function has been called
         */
        exports.prompt = function() {
            let options;
            let promptDialog;
            let form;
            let input;
            let shouldShow;
            let inputOptions;

            // We have to create our form first, otherwise its value is undefined when gearing up our options.
            // @TODO this could be solved by allowing message to be a function instead...
            form = generateElement(templates.form);

            // prompt defaults are more complex than others in that users can override more defaults
            options = mergeDialogOptions('prompt', ['cancel', 'confirm'], ['title', 'callback'], arguments);

            if (!options.value) {
                options.value = defaults.value;
            }

            if (!options.inputType) {
                options.inputType = defaults.inputType;
            }

            // Capture the user's 'show' value; we always set this to false before spawning the dialog to give us a chance to attach some handlers to it, but we need to make sure we respect a preference not to show it
            shouldShow = (options.show === undefined) ? defaults.show : options.show;

            // This is required prior to calling the dialog builder below - we need to add an event handler just before the prompt is shown
            options.show = false;

            // Handles the 'cancel' action
            options.buttons.cancel.callback = options.onEscape = function() {
                return options.callback.call(this, null);
            };

            // Prompt submitted - extract the prompt value. This requires a bit of work, given the different input types available.
            options.buttons.confirm.callback = function() {
	            let value;

	            if (options.inputType === 'checkbox') {
		            const checkedInputs = Array.from(input.querySelectorAll('input[type="checkbox"]:checked'));

		            value = Array.from(checkedInputs).map(function(e) {
			            return e.value;
		            });

		            if (options.required === true && checkedInputs.length === 0) {
			            // prevents button callback from being called if no checkboxes have been checked
			            return false;
		            }
	            } else if (options.inputType === 'radio') {
		            value = input.querySelector('input[type="radio"]:checked').value;
	            } else {
		            value = input.value;
	            }

	            return options.callback.call(this, value);
            };

            // prompt-specific validation
            if (!options.title) {
                throw new Error('prompt requires a title');
            }

            if (typeof options.callback !== 'function') {
                throw new Error('prompt requires a callback');
            }

            if (!templates.inputs[options.inputType]) {
                throw new Error('Invalid prompt type');
            }

            // Create the input based on the supplied type
            input = generateElement(templates.inputs[options.inputType]);

            switch (options.inputType) {
            case 'text':
            case 'textarea':
            case 'email':
            case 'password':
                input.value = options.value;

                if (options.placeholder) {
                    input.setAttribute('placeholder', options.placeholder);
                }

                if (options.pattern) {
                    input.setAttribute('pattern', options.pattern);
                }

                if (options.maxlength) {
                    input.setAttribute('maxlength', options.maxlength);
                }

                if (options.required) {
                    input.required = true;
                }

                if (options.rows && !isNaN(parseInt(options.rows))) {
                    if (options.inputType === 'textarea') {
                        input.setAttribute('rows', options.rows);
                    }
                }
                break;

            case 'date':
            case 'time':
            case 'number':
            case 'range':
                input.value = options.value;

                if (options.placeholder) {
                    input.setAttribute('placeholder', options.placeholder);
                }

                if (options.pattern) {
                    input.setAttribute('pattern', options.pattern);
                } else {
                    if (options.inputType === 'date') {
                        // Add the ISO-8601 short date format as a fallback for browsers without native type="date" support
                        input.setAttribute('pattern', '\d{4}-\d{2}-\d{2}');
                    } else if (options.inputType === 'time') {
                        // Add an HH:MM pattern as a fallback for browsers without native type="time" support
                        input.setAttribute('pattern', '\d{2}:\d{2}');
                    }
                }

                if (options.required) {
                    input.required = true;
                }

                
                if (options.step) {
	                if (options.step === 'any' || (!isNaN(options.step) && parseFloat(options.step) > 0)) {
		                input.setAttribute('step', options.step);
	                } else {
		                throw new Error(
			                '"step" must be a valid positive number or the value "any". See https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#attr-step for more information.');
	                }
                }

                if (minAndMaxAreValid(options.inputType, options.min, options.max)) {
                    if (options.min !== undefined) {
                        input.setAttribute('min', options.min);
                    }
                    if (options.max !== undefined) {
                        input.setAttribute('max', options.max);
                    }
                }
                break;

            case 'select':
                var groups = {};
                inputOptions = options.inputOptions || [];

                if (!Array.isArray(inputOptions)) {
                    throw new Error('Please pass an array of input options');
                }

                if (!inputOptions.length) {
                    throw new Error('prompt with "inputType" set to "select" requires at least one option');
                }

                if (options.required) {
                    input.required = true;
                }

                if (options.multiple) {
                    input.multiple = true;
                }

                for (const [, option] of Object.entries(inputOptions)) {
                    // Assume the element to attach to is the input...
                    let elem = input;

                    if (option.value === undefined || option.text === undefined) {
                        throw new Error('each option needs a "value" property and a "text" property');
                    }

                    // ... but override that element if this option sits in a group

                    if (option.group) {

                        // Initialise group if necessary
                        if (!groups[option.group]) {
                            var groupElement = generateElement('<optgroup />');
                            groupElement.setAttribute('label', option.group);

                            groups[option.group] = groupElement;
                        }

                        elem = groups[option.group];
                    }

                    let o = generateElement(templates.option);

                    o.setAttribute('value', option.value);
                    o.textContent = option.text;

                   

                    elem.append(o);
                }

                for (const [key, group] of Object.entries(groups)) {
                    input.append(group);
                }

                // Safe to set a select's value as per a normal input
                input.value = options.value;
                if (options.bootstrap < 5) {
                    input.classList.remove('form-select');
                    input.classList.add('form-control');
                }
                break;

            case 'checkbox':
                var checkboxValues = Array.isArray(options.value) ? options.value : [options.value];
                inputOptions = options.inputOptions || [];

                if (!inputOptions.length) {
                    throw new Error('prompt with "inputType" set to "checkbox" requires at least one option');
                }

                // Checkboxes have to nest within a containing element, so they break the rules a bit and we end up re-assigning our 'input' element to this container instead
                input = generateElement('<div class="bootbox-checkbox-list"></div>');

                for (const [_, option] of Object.entries(inputOptions)) {
                    if (option.value === undefined || option.text === undefined) {
                        throw new Error('each option needs a "value" property and a "text" property');
                    }

                    let checkbox = generateElement(templates.inputs[options.inputType]);

                    checkbox.querySelector('input').setAttribute('value', option.value);
                    checkbox.querySelector('label').append(`\n${option.text}`);

                    // We've ensured values is an array, so we can always iterate over it
                    for (const [key, value] of Object.entries(checkboxValues)) {
                        if (value === option.value) {
                            checkbox.querySelector('input').checked = true;
                        }
                    }

                    input.append(checkbox);
                }
                break;

            case 'radio':
                // Make sure that value is not an array (only a single radio can ever be checked)
                if (options.value !== undefined && Array.isArray(options.value)) {
                    throw new Error(
                        'prompt with "inputType" set to "radio" requires a single, non-array value for "value"');
                }

                inputOptions = options.inputOptions || [];

                if (!inputOptions.length) {
                    throw new Error('prompt with "inputType" set to "radio" requires at least one option');
                }

                // Radiobuttons have to nest within a containing element, so they break the rules a bit and we end up re-assigning our 'input' element to this container instead
                input = generateElement('<div class="bootbox-radiobutton-list"></div>');

                // Radiobuttons should always have an initial checked input checked in a "group".
                // If value is undefined or doesn't match an input option, select the first radiobutton
                var checkFirstRadio = true;

                for (const [_, option] of Object.entries(inputOptions)) {
                    if (option.value === undefined || option.text === undefined) {
                        throw new Error('each option needs a "value" property and a "text" property');
                    }

                    let radio = generateElement(templates.inputs[options.inputType]);

                    radio.querySelector('input').setAttribute('value', option.value);
                    radio.querySelector('label').append(`\n${option.text}`);

                    if (options.value !== undefined) {
                        if (option.value === options.value) {
                            radio.querySelector('input').checked = true;
                            checkFirstRadio = false;
                        }
                    }

                    input.append(radio);
                }

                if (checkFirstRadio) {
                    input.querySelector('input[type="radio"]').setAttribute('checked', true);
                }
                break;
            }

            // Now place it in our form
            form.append(input);

            form.addEventListener('submit',
                function(e) {
                    e.preventDefault();
                    // Fix for SammyJS (or similar JS routing library) hijacking the form post.
                    e.stopPropagation();

                    // @TODO can we actually click *the* button object instead?
                    // e.g. buttons.confirm.click() or similar
                    promptDialog.querySelector('.bootbox-accept').click();
                });

            if (options.message && options.message.trim() !== '') {
                // Add the form to whatever content the user may have added.
                let message = generateElement(templates.promptMessage).innerHTML = options.message;
                form.prepend(message);
                options.message = form;
            } else {
                options.message = form;
            }

            // Generate the dialog
            promptDialog = exports.dialog(options);

            // Clear the existing handler focusing the submit button...
            promptDialog.removeEventListener('shown.bs.modal', focusPrimaryButton);

            // ...and replace it with one focusing our input, if possible
            promptDialog.addEventListener('shown.bs.modal',
                function() {
                    // Need the closure here since input isn't can object otherwise
                    input.focus();
                });

            const modal = new bootstrap.Modal(promptDialog);

            if (shouldShow === true) {
                modal.show();
            }

            return promptDialog;
        };

        function deepExtend(out, ...arguments_) {
            if (!out) {
                return {};
            }

            for (const obj of arguments_) {
                if (!obj) {
                    continue;
                }

                for (const [key, value] of Object.entries(obj)) {
                    switch (Object.prototype.toString.call(value)) {
                    case '[object Object]':
                        out[key] = out[key] || {};
                        out[key] = deepExtend(out[key], value);
                        break;
                    case '[object Array]':
                        out[key] = deepExtend(new Array(value.length), value);
                        break;
                    default:
                        out[key] = value;
                    }
                }
            }

            return out;
        }

        // INTERNAL FUNCTIONS
        // *************************************************************************************************************

        // Map a flexible set of arguments into a single returned object.
        // If args.length is already one just return it, otherwise use the properties argument to map the unnamed args to object properties.
        // So in the latter case:
        //
        //    mapArguments(["foo", $.noop], ["message", "callback"])
        //  
        //  results in
        //
        //    { message: "foo", callback: $.noop }
        //
        function mapArguments(args, properties) {
            const argsLength = args.length;
            let options = {};

            if (argsLength < 1 || argsLength > 2) {
                throw new Error('Invalid argument length');
            }

            if (argsLength === 2 || typeof args[0] === 'string') {
                options[properties[0]] = args[0];
                options[properties[1]] = args[1];
            } else {
                options = args[0];
            }

            return options;
        }


        // Merge a set of default dialog options with user supplied arguments
        function mergeArguments(defaults, args, properties) {
            return deepExtend(
                // Ensure the target is an empty, unreferenced object
                {},
                // The base options object for this type of dialog (often just buttons)
                defaults,
                // 'args' could be an object or array; if it's an array properties will map it to a proper options object
                mapArguments(args, properties)
            );
        }


        // This entry-level method makes heavy use of composition to take a simple range of inputs and return valid options suitable for passing to bootbox.dialog
        function mergeDialogOptions(className, labels, properties, args) {
            let locale;
            if (args && args[0]) {
                locale = args[0].locale || defaults.locale;
                const swapButtons = args[0].swapButtonOrder || defaults.swapButtonOrder;

                if (swapButtons) {
                    labels = labels.reverse();
                }
            }

            // Build up a base set of dialog properties
            const baseOptions = {
                className: `bootbox-${className}`,
                buttons: createLabels(labels, locale)
            };

            // Ensure the buttons properties generated, *after* merging with user args are still valid against the supplied labels
            return validateButtons(
                // Merge the generated base properties with user supplied arguments
                mergeArguments(
                    baseOptions,
                    args,
                    // If args.length > 1, properties specify how each arg maps to an object key
                    properties
                ),
                labels
            );
        }


        // Checks each button object to see if key is valid. 
        // This function will only be called by the alert, confirm, and prompt helpers. 
        function validateButtons(options, buttons) {
            const allowedButtons = {};

            for (const [key, value] of Object.entries(buttons)) {
                allowedButtons[value] = true;
            }

            for (const [key, value] of Object.entries(options.buttons)) {
                if (allowedButtons[key] === undefined) {
                    throw new Error(`button key "${key}" is not allowed (options are ${buttons.join(' ')})`);
                }
            }

            return options;
        }


        // From a given list of arguments, return a suitable object of button labels.
        // All this does is normalise the given labels and translate them where possible.
        // e.g. "ok", "confirm" -> { ok: "OK", cancel: "Annuleren" }
        function createLabels(labels, locale) {
            const buttons = {};

            for (let i = 0, j = labels.length; i < j; i++) {
                const argument = labels[i];
                const key = argument.toLowerCase();
                const value = argument.toUpperCase();

                buttons[key] = {
                    label: getText(value, locale)
                };
            }

            return buttons;
        }


        // Get localized text from a locale. Defaults to 'en' locale if no locale provided or a non-registered locale is requested
        function getText(key, locale) {
            const labels = locales[locale];

            return labels ? labels[key] : locales.en[key];
        }


        // Filter and tidy up any user supplied parameters to this dialog.
        // Also looks for any shorthands used and ensures that the options which are returned are all normalized properly
        function sanitize(options) {
            let buttons;
            let total;

            if (typeof options !== 'object') {
                throw new Error('Please supply an object of options');
            }

            if (!options.message) {
                throw new Error('"message" option must not be null or an empty string.');
            }

            // Make sure any supplied options take precedence over defaults
            options = Object.assign({}, defaults, options);


            // Make sure backdrop is either true, false, or 'static'
            if (!options.backdrop) {
                options.backdrop = (options.backdrop === false || options.backdrop === 0) ? false : 'static';
            } else {
                options.backdrop = typeof options.backdrop === 'string' && options.backdrop.toLowerCase() === 'static' ? 'static' : true;
            }

            // No buttons is still a valid dialog but it's cleaner to always have a buttons object to iterate over, even if it's empty
            if (!options.buttons) {
                options.buttons = {};
            }

            buttons = options.buttons;

            total = getKeyLength(buttons);

            var index = 0;

            for (var [key, button] of Object.entries(buttons)) {
                if (typeof button === 'function') {
                    // Short form, assume value is our callback. Since button isn't an object it isn't a reference either so re-assign it
                    button = buttons[key] = {
                        callback: button
                    };
                }

                // Before any further checks, make sure button is the correct type
                if (Object.prototype.toString
                    .call(button)
                    .replace(/^\[object (.+)\]$/, '$1')
                    .toLowerCase() !==
                    'object') {
                    throw new Error(`button with key "${key}" must be an object`);
                }

                if (!button.label) {
                    // The lack of an explicit label means we'll assume the key is good enough
                    button.label = key;
                }

                if (!button.className) {
                    let isPrimary = false;
                    if (options.swapButtonOrder) {
                        isPrimary = index === 0;
                    } else {
                        isPrimary = index === total - 1;
                    }

                    if (total <= 2 && isPrimary) {
                        // always add a primary to the main option in a one or two-button dialog
                        button.className = 'btn-primary';
                    } else {
                        // adding both classes allows us to target both BS3 and BS4+ without needing to check the version
                        button.className = 'btn-secondary btn-default';
                    }
                }

                index++;
            }

            return options;
        }


        // Returns a count of the properties defined on the object
        function getKeyLength(obj) {
            return Object.keys(obj).length;
        }

        function focusPrimaryButton(e) {
            trigger(e.data.dialog.querySelector('.bootbox-accept').first(), 'focus');
        }


        //  Handle the invoked dialog callback
        function processCallback(e, dialog, callback) {
            e.stopPropagation();
            e.preventDefault();

            // By default we assume a callback will get rid of the dialog, although it is given the opportunity to override this
            // If the callback can be invoked and it *explicitly returns false*, then we'll set a flag to keep the dialog active...
            const preserveDialog = typeof callback === 'function' && callback.call(dialog, e) === false;

            // ... otherwise we'll bin it
            if (!preserveDialog && dialog) {
                bootstrap.Modal.getInstance(dialog).hide();
            }
        }

        // Validate `min` and `max` values based on the current `inputType` value
        function minAndMaxAreValid(type, min, max) {
            let result = false;
            let minValid = true;
            let maxValid = true;

            if (type === 'date') {
                if (min !== undefined && !(minValid = dateIsValid(min))) {
                    console.warn(
                        'Browsers which natively support the "date" input type expect date values to be of the form "YYYY-MM-DD" (see ISO-8601 https://www.iso.org/iso-8601-date-and-time-format.html). Bootbox does not enforce this rule, but your min value may not be enforced by this browser.');
                } else if (max !== undefined && !(maxValid = dateIsValid(max))) {
                    console.warn(
                        'Browsers which natively support the "date" input type expect date values to be of the form "YYYY-MM-DD" (see ISO-8601 https://www.iso.org/iso-8601-date-and-time-format.html). Bootbox does not enforce this rule, but your max value may not be enforced by this browser.');
                }
            } else if (type === 'time') {
                if (min !== undefined && !(minValid = timeIsValid(min))) {
                    throw new Error(
                        '"min" is not a valid time. See https://www.w3.org/TR/2012/WD-html-markup-20120315/datatypes.html#form.data.time for more information.');
                } else if (max !== undefined && !(maxValid = timeIsValid(max))) {
                    throw new Error(
                        '"max" is not a valid time. See https://www.w3.org/TR/2012/WD-html-markup-20120315/datatypes.html#form.data.time for more information.');
                }
            } else {
                if (min !== undefined && isNaN(min)) {
                    minValid = false;
                    throw new Error(
                        '"min" must be a valid number. See https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#attr-min for more information.');
                }

                if (max !== undefined && isNaN(max)) {
                    maxValid = false;
                    throw new Error(
                        '"max" must be a valid number. See https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#attr-max for more information.');
                }
            }

            if (minValid && maxValid) {
                if (max < min) {
                    throw new Error('"max" must be greater than or equal to "min". See https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#attr-max for more information.');
                } else {
                    result = true;
                }
            }

            return result;
        }

        function timeIsValid(value) {
            return /([01][0-9]|2[0-3]):[0-5][0-9]?:[0-5][0-9]/.test(value);
        }

        function dateIsValid(value) {
            return /(\d{4})-(\d{2})-(\d{2})/.test(value);
        }
        function trigger(el, eventType) {
            if (typeof eventType === 'string' && typeof el[eventType] === 'function') {
                el[eventType]();
            } else {
                const event =
                    typeof eventType === 'string' ? new Event(eventType, { bubbles: true }) : eventType;
                el.dispatchEvent(event);
            }
        }

        // helper Class
        function generateElement(html) {
            const template = document.createElement('template');
            template.innerHTML = html.trim();
            return template.content.children[0];
        }
        function addEventListener(el, eventName, eventHandler, selector) {
            if (selector) {
                const wrappedHandler = (e) => {
                    if (!e.target) return;
                    const el = e.target.closest(selector);
                    if (el) {
                        eventHandler.call(el, e);
                    }
                };
                el.addEventListener(eventName, wrappedHandler);
                return wrappedHandler;
            } else {
                const wrappedHandler = (e) => {
                    eventHandler.call(el, e);
                };
                el.addEventListener(eventName, wrappedHandler);
                return wrappedHandler;
            }
        }

        //  The Bootbox object
        return exports;
    }));