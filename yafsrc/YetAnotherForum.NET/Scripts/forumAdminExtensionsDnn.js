(function(root, factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        define([ "bootstrap" ], factory);
    } else if (typeof exports === "object") {
        module.exports = factory(require("bootstrap"));
    } else {
        root.bootbox = factory(root.bootstrap);
    }
})(this, function init($, undefined) {
    "use strict";
    const exports = {};
    const VERSION = "6.0.0";
    exports.VERSION = VERSION;
    const locales = {
        en: {
            OK: "OK",
            CANCEL: "Cancel",
            CONFIRM: "OK"
        }
    };
    const templates = {
        dialog: '<div class="bootbox modal" tabindex="-1" role="dialog" aria-hidden="true"><div class="modal-dialog"><div class="modal-content"><div class="modal-body"><div class="bootbox-body"></div></div></div></div></div>',
        header: '<div class="modal-header"><h5 class="modal-title"></h5></div>',
        footer: '<div class="modal-footer"></div>',
        closeButton: '<button type="button" class="bootbox-close-button close btn-close" aria-hidden="true" aria-label="Close"></button>',
        form: '<form class="bootbox-form"></form>',
        button: '<button type="button" class="btn"></button>',
        option: '<option value=""></option>',
        promptMessage: '<div class="bootbox-prompt-message"></div>',
        inputs: {
            text: '<input class="bootbox-input bootbox-input-text form-control" autocomplete="off" type="text" />',
            textarea: '<textarea class="bootbox-input bootbox-input-textarea form-control"></textarea>',
            email: '<input class="bootbox-input bootbox-input-email form-control" autocomplete="off" type="email" />',
            select: '<select class="bootbox-input bootbox-input-select form-select"></select>',
            checkbox: '<div class="form-check checkbox"><label class="form-check-label"><input class="form-check-input bootbox-input bootbox-input-checkbox" type="checkbox" /></label></div>',
            radio: '<div class="form-check radio"><label class="form-check-label"><input class="form-check-input bootbox-input bootbox-input-radio" type="radio" name="bootbox-radio" /></label></div>',
            date: '<input class="bootbox-input bootbox-input-date form-control" autocomplete="off" type="date" />',
            time: '<input class="bootbox-input bootbox-input-time form-control" autocomplete="off" type="time" />',
            number: '<input class="bootbox-input bootbox-input-number form-control" autocomplete="off" type="number" />',
            password: '<input class="bootbox-input bootbox-input-password form-control" autocomplete="off" type="password" />',
            range: '<input class="bootbox-input bootbox-input-range form-control-range" autocomplete="off" type="range" />'
        }
    };
    const defaults = {
        locale: "en",
        backdrop: "static",
        animate: true,
        className: null,
        closeButton: true,
        show: true,
        container: "body",
        value: "",
        inputType: "text",
        errorMessage: null,
        swapButtonOrder: false,
        centerVertical: false,
        multiple: false,
        scrollable: false,
        reusable: false,
        relatedTarget: null,
        size: null,
        id: null
    };
    exports.locales = function(name) {
        return name ? locales[name] : locales;
    };
    exports.addLocale = function(name, values) {
        [ "OK", "CANCEL", "CONFIRM" ].forEach(v => {
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
    exports.removeLocale = function(name) {
        if (name !== "en") {
            delete locales[name];
        } else {
            throw new Error('"en" is used as the default and fallback locale and cannot be removed.');
        }
        return exports;
    };
    exports.setLocale = function(name) {
        return exports.setDefaults("locale", name);
    };
    exports.setDefaults = function() {
        let values = {};
        if (arguments.length === 2) {
            values[arguments[0]] = arguments[1];
        } else {
            values = arguments[0];
        }
        Object.assign(defaults, values);
        return exports;
    };
    exports.hideAll = function() {
        document.querySelectorAll(".bootbox").forEach(box => {
            bootstrap.Modal.getInstance(box).hide();
        });
        return exports;
    };
    exports.init = function(_$) {
        return init(_$ || $);
    };
    exports.dialog = function(options) {
        if (bootstrap.Modal === undefined) {
            throw new Error('"bootstrap.Modal" is not defined; please double check you have included the Bootstrap JavaScript library. See https://getbootstrap.com/docs/5.3/getting-started/introduction/ for more details.');
        }
        options = sanitize(options);
        if (bootstrap.Modal.VERSION) {
            options.fullBootstrapVersion = bootstrap.Modal.VERSION;
            let i = options.fullBootstrapVersion.indexOf(".");
            options.bootstrap = options.fullBootstrapVersion.substring(0, i);
        } else {
            options.bootstrap = "2";
            options.fullBootstrapVersion = "2.3.2";
            console.warn("Bootbox will *mostly* work with Bootstrap 2, but we do not officially support it. Please upgrade, if possible.");
        }
        let dialog = generateElement(templates.dialog);
        let innerDialog = dialog.querySelector(".modal-dialog");
        let body = dialog.querySelector(".modal-body");
        let header = generateElement(templates.header);
        let footer = generateElement(templates.footer);
        let buttons = options.buttons;
        let callbacks = {
            onEscape: options.onEscape
        };
        if (typeof options.message === "string") {
            body.querySelector(".bootbox-body").innerHTML = options.message;
        } else {
            body.querySelector(".bootbox-body").append(options.message);
        }
        if (getKeyLength(options.buttons) > 0) {
            for (const [ key, b ] of Object.entries(buttons)) {
                let button = generateElement(templates.button);
                button.dataset.bbHandler = key;
                var classNames = b.className.split(" ");
                classNames.forEach(name => {
                    button.classList.add(name);
                });
                switch (key) {
                  case "ok":
                  case "confirm":
                    button.classList.add("bootbox-accept");
                    break;

                  case "cancel":
                    button.classList.add("bootbox-cancel");
                    break;
                }
                button.innerHTML = b.label;
                if (b.id) {
                    button.setAttribute({
                        id: b.id
                    });
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
            dialog.classList.add("fade");
        }
        if (options.className) {
            options.className.split(" ").forEach(name => {
                dialog.classList.add(name);
            });
        }
        if (options.id) {
            dialog.setAttribute({
                id: options.id
            });
        }
        if (options.size) {
            if (options.fullBootstrapVersion.substring(0, 3) < "3.1") {
                console.warn(`"size" requires Bootstrap 3.1.0 or higher. You appear to be using ${options.fullBootstrapVersion}. Please upgrade to use this option.`);
            }
            switch (options.size) {
              case "small":
              case "sm":
                innerDialog.classList.add("modal-sm");
                break;

              case "large":
              case "lg":
                innerDialog.classList.add("modal-lg");
                break;

              case "extra-large":
              case "xl":
                innerDialog.classList.add("modal-xl");
                if (options.fullBootstrapVersion.substring(0, 3) < "4.2") {
                    console.warn(`Using size "xl"/"extra-large" requires Bootstrap 4.2.0 or higher. You appear to be using ${options.fullBootstrapVersion}. Please upgrade to use this option.`);
                }
                break;
            }
        }
        if (options.scrollable) {
            innerDialog.classList.add("modal-dialog-scrollable");
            if (options.fullBootstrapVersion.substring(0, 3) < "4.3") {
                console.warn(`Using "scrollable" requires Bootstrap 4.3.0 or higher. You appear to be using ${options.fullBootstrapVersion}. Please upgrade to use this option.`);
            }
        }
        if (options.title || options.closeButton) {
            if (options.title) {
                header.querySelector(".modal-title").innerHTML = options.title;
            } else {
                header.classList.add("border-0");
            }
            if (options.closeButton) {
                let closeButton = generateElement(templates.closeButton);
                if (options.bootstrap < 5) {
                    closeButton.innerHTML = "&#215;";
                }
                if (options.bootstrap < 4) {
                    header.prepend(closeButton);
                } else {
                    header.append(closeButton);
                }
            }
            body.before(header);
        }
        if (options.centerVertical) {
            innerDialog.classList.add("modal-dialog-centered");
            if (options.fullBootstrapVersion < "4.0.0") {
                console.warn(`"centerVertical" requires Bootstrap 4.0.0-beta.3 or higher. You appear to be using ${options.fullBootstrapVersion}. Please upgrade to use this option.`);
            }
        }
        if (!options.reusable) {
            dialog.addEventListener("hide.bs.modal", function() {
                dialog.removeEventListener("escape.close.bb", null);
                dialog.removeEventListener("click", null);
            }, {
                once: true
            });
            dialog.addEventListener("hidden.bs.modal", function() {
                dialog.remove();
                dialog = null;
            }, {
                once: true
            });
        }
        if (options.onHide) {
            if (typeof options.onHide === "function") {
                dialog.addEventListener("hide.bs.modal", options.onHide);
            } else {
                throw new Error('Argument supplied to "onHide" must be a function');
            }
        }
        if (options.onHidden) {
            if (typeof options.onHidden === "function") {
                dialog.addEventListener("hidden.bs.modal", options.onHidden);
            } else {
                throw new Error('Argument supplied to "onHidden" must be a function');
            }
        }
        if (options.onShow) {
            if (typeof options.onShow === "function") {
                addEventListener(dialog, "show.bs.modal", options.onShow);
            } else {
                throw new Error('Argument supplied to "onShow" must be a function');
            }
        }
        dialog.addEventListener("shown.bs.modal", {
            dialog: dialog
        }, focusPrimaryButton);
        if (options.onShown) {
            if (typeof options.onShown === "function") {
                addEventListener(dialog, "shown.bs.modal", options.onShown);
            } else {
                throw new Error('Argument supplied to "onShown" must be a function');
            }
        }
        if (options.backdrop === true) {
            let startedOnBody = false;
            addEventListener(dialog, "mousedown", function(e) {
                e.stopPropagation();
                startedOnBody = true;
            });
            addEventListener(dialog, "click.dismiss.bs.modal", function(e) {
                if (startedOnBody || e.target !== e.currentTarget) {
                    return;
                }
                trigger(dialog, "escape.close.bb");
            });
        }
        dialog.addEventListener("escape.close.bb", function(e) {
            if (callbacks.onEscape) {
                processCallback(e, dialog, callbacks.onEscape);
            }
        });
        dialog.addEventListener("click", e => {
            if (e.target.nodeName.toLowerCase() === "button" && !e.target.classList.contains("disabled")) {
                const callbackKey = e.target.dataset.bbHandler;
                if (callbackKey !== undefined) {
                    processCallback(e, dialog, callbacks[callbackKey]);
                }
            }
        });
        document.addEventListener("click", e => {
            if (e.target.closest(".bootbox-close-button")) {
                processCallback(e, dialog, callbacks.onEscape);
            }
        });
        dialog.addEventListener("keyup", function(e) {
            if (e.which === 27) {
                trigger(dialog, "escape.close.bb");
            }
        });
        if (typeof options.container === "object") {
            options.container.append(dialog);
        } else {
            document.querySelector(options.container).append(dialog);
        }
        const modal = new bootstrap.Modal(dialog, {
            backdrop: options.backdrop,
            keyboard: false,
            show: false
        });
        if (options.show) {
            modal.show(options.relatedTarget);
        }
        return dialog;
    };
    exports.alert = function() {
        let options;
        options = mergeDialogOptions("alert", [ "ok" ], [ "message", "callback" ], arguments);
        if (options.callback && typeof options.callback !== "function") {
            throw new Error('alert requires the "callback" property to be a function when provided');
        }
        options.buttons.ok.callback = options.onEscape = function() {
            if (typeof options.callback === "function") {
                return options.callback.call(this);
            }
            return true;
        };
        return exports.dialog(options);
    };
    exports.confirm = function() {
        let options;
        options = mergeDialogOptions("confirm", [ "cancel", "confirm" ], [ "message", "callback" ], arguments);
        if (typeof options.callback !== "function") {
            throw new Error("confirm requires a callback");
        }
        options.buttons.cancel.callback = options.onEscape = function() {
            return options.callback.call(this, false);
        };
        options.buttons.confirm.callback = function() {
            return options.callback.call(this, true);
        };
        return exports.dialog(options);
    };
    exports.prompt = function() {
        let options;
        let promptDialog;
        let form;
        let input;
        let shouldShow;
        let inputOptions;
        form = generateElement(templates.form);
        options = mergeDialogOptions("prompt", [ "cancel", "confirm" ], [ "title", "callback" ], arguments);
        if (!options.value) {
            options.value = defaults.value;
        }
        if (!options.inputType) {
            options.inputType = defaults.inputType;
        }
        shouldShow = options.show === undefined ? defaults.show : options.show;
        options.show = false;
        options.buttons.cancel.callback = options.onEscape = function() {
            return options.callback.call(this, null);
        };
        options.buttons.confirm.callback = function() {
            let value;
            if (options.inputType === "checkbox") {
                value = Array.from(input.querySelectorAll('input[type="checkbox"]:checked')).map(function(e) {
                    return e.value;
                });
            } else if (options.inputType === "radio") {
                value = input.querySelector('input[type="radio"]:checked').value;
            } else {
                value = input.value;
            }
            return options.callback.call(this, value);
        };
        if (!options.title) {
            throw new Error("prompt requires a title");
        }
        if (typeof options.callback !== "function") {
            throw new Error("prompt requires a callback");
        }
        if (!templates.inputs[options.inputType]) {
            throw new Error("Invalid prompt type");
        }
        input = generateElement(templates.inputs[options.inputType]);
        switch (options.inputType) {
          case "text":
          case "textarea":
          case "email":
          case "password":
            input.value = options.value;
            if (options.placeholder) {
                input.setAttribute("placeholder", options.placeholder);
            }
            if (options.pattern) {
                input.setAttribute("pattern", options.pattern);
            }
            if (options.maxlength) {
                input.setAttribute("maxlength", options.maxlength);
            }
            if (options.required) {
                input.required = true;
            }
            if (options.rows && !isNaN(parseInt(options.rows))) {
                if (options.inputType === "textarea") {
                    input.setAttribute("rows", options.rows);
                }
            }
            break;

          case "date":
          case "time":
          case "number":
          case "range":
            input.value = options.value;
            if (options.placeholder) {
                input.setAttribute("placeholder", options.placeholder);
            }
            if (options.pattern) {
                input.setAttribute("pattern", options.pattern);
            } else {
                if (options.inputType === "date") {
                    input.setAttribute("pattern", "d{4}-d{2}-d{2}");
                } else if (options.inputType === "time") {
                    input.setAttribute("pattern", "d{2}:d{2}");
                }
            }
            if (options.required) {
                input.required = true;
            }
            if (options.inputType !== "date") {
                if (options.step) {
                    if (options.step === "any" || !isNaN(options.step) && parseFloat(options.step) > 0) {
                        input.setAttribute("step", options.step);
                    } else {
                        throw new Error('"step" must be a valid positive number or the value "any". See https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#attr-step for more information.');
                    }
                }
            }
            if (minAndMaxAreValid(options.inputType, options.min, options.max)) {
                if (options.min !== undefined) {
                    input.setAttribute("min", options.min);
                }
                if (options.max !== undefined) {
                    input.setAttribute("max", options.max);
                }
            }
            break;

          case "select":
            var groups = {};
            inputOptions = options.inputOptions || [];
            if (!Array.isArray(inputOptions)) {
                throw new Error("Please pass an array of input options");
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
            for (const [ , option ] of Object.entries(inputOptions)) {
                let elem = input;
                if (option.value === undefined || option.text === undefined) {
                    throw new Error('each option needs a "value" property and a "text" property');
                }
                if (option.group) {
                    if (!groups[option.group]) {
                        var groupElement = generateElement("<optgroup />");
                        groupElement.setAttribute("label", option.group);
                        groups[option.group] = groupElement;
                    }
                    elem = groups[option.group];
                }
                let o = generateElement(templates.option);
                o.setAttribute("value", option.value);
                o.textContent = option.text;
                elem.append(o);
            }
            for (const [ key, group ] of Object.entries(groups)) {
                input.append(group);
            }
            input.value = options.value;
            if (options.bootstrap < 5) {
                input.classList.remove("form-select");
                input.classList.add("form-control");
            }
            break;

          case "checkbox":
            var checkboxValues = Array.isArray(options.value) ? options.value : [ options.value ];
            inputOptions = options.inputOptions || [];
            if (!inputOptions.length) {
                throw new Error('prompt with "inputType" set to "checkbox" requires at least one option');
            }
            input = generateElement('<div class="bootbox-checkbox-list"></div>');
            for (const [ _, option ] of Object.entries(inputOptions)) {
                if (option.value === undefined || option.text === undefined) {
                    throw new Error('each option needs a "value" property and a "text" property');
                }
                let checkbox = generateElement(templates.inputs[options.inputType]);
                checkbox.querySelector("input").setAttribute("value", option.value);
                checkbox.querySelector("label").append(`\n${option.text}`);
                for (const [ key, value ] of Object.entries(checkboxValues)) {
                    if (value === option.value) {
                        checkbox.querySelector("input").checked = true;
                    }
                }
                input.append(checkbox);
            }
            break;

          case "radio":
            if (options.value !== undefined && Array.isArray(options.value)) {
                throw new Error('prompt with "inputType" set to "radio" requires a single, non-array value for "value"');
            }
            inputOptions = options.inputOptions || [];
            if (!inputOptions.length) {
                throw new Error('prompt with "inputType" set to "radio" requires at least one option');
            }
            input = generateElement('<div class="bootbox-radiobutton-list"></div>');
            var checkFirstRadio = true;
            for (const [ _, option ] of Object.entries(inputOptions)) {
                if (option.value === undefined || option.text === undefined) {
                    throw new Error('each option needs a "value" property and a "text" property');
                }
                let radio = generateElement(templates.inputs[options.inputType]);
                radio.querySelector("input").setAttribute("value", option.value);
                radio.querySelector("label").append(`\n${option.text}`);
                if (options.value !== undefined) {
                    if (option.value === options.value) {
                        radio.querySelector("input").checked = true;
                        checkFirstRadio = false;
                    }
                }
                input.append(radio);
            }
            if (checkFirstRadio) {
                input.querySelector('input[type="radio"]').setAttribute("checked", true);
            }
            break;
        }
        form.append(input);
        form.addEventListener("submit", function(e) {
            e.preventDefault();
            e.stopPropagation();
            promptDialog.querySelector(".bootbox-accept").click();
        });
        if (options.message && options.message.trim() !== "") {
            let message = generateElement(templates.promptMessage).innerHTML = options.message;
            form.prepend(message);
            options.message = form;
        } else {
            options.message = form;
        }
        promptDialog = exports.dialog(options);
        promptDialog.removeEventListener("shown.bs.modal", focusPrimaryButton);
        promptDialog.addEventListener("shown.bs.modal", function() {
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
            for (const [ key, value ] of Object.entries(obj)) {
                switch (Object.prototype.toString.call(value)) {
                  case "[object Object]":
                    out[key] = out[key] || {};
                    out[key] = deepExtend(out[key], value);
                    break;

                  case "[object Array]":
                    out[key] = deepExtend(new Array(value.length), value);
                    break;

                  default:
                    out[key] = value;
                }
            }
        }
        return out;
    }
    function mapArguments(args, properties) {
        const argsLength = args.length;
        let options = {};
        if (argsLength < 1 || argsLength > 2) {
            throw new Error("Invalid argument length");
        }
        if (argsLength === 2 || typeof args[0] === "string") {
            options[properties[0]] = args[0];
            options[properties[1]] = args[1];
        } else {
            options = args[0];
        }
        return options;
    }
    function mergeArguments(defaults, args, properties) {
        return deepExtend({}, defaults, mapArguments(args, properties));
    }
    function mergeDialogOptions(className, labels, properties, args) {
        let locale;
        if (args && args[0]) {
            locale = args[0].locale || defaults.locale;
            const swapButtons = args[0].swapButtonOrder || defaults.swapButtonOrder;
            if (swapButtons) {
                labels = labels.reverse();
            }
        }
        const baseOptions = {
            className: `bootbox-${className}`,
            buttons: createLabels(labels, locale)
        };
        return validateButtons(mergeArguments(baseOptions, args, properties), labels);
    }
    function validateButtons(options, buttons) {
        const allowedButtons = {};
        for (const [ key, value ] of Object.entries(buttons)) {
            allowedButtons[value] = true;
        }
        for (const [ key, value ] of Object.entries(options.buttons)) {
            if (allowedButtons[key] === undefined) {
                throw new Error(`button key "${key}" is not allowed (options are ${buttons.join(" ")})`);
            }
        }
        return options;
    }
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
    function getText(key, locale) {
        const labels = locales[locale];
        return labels ? labels[key] : locales.en[key];
    }
    function sanitize(options) {
        let buttons;
        let total;
        if (typeof options !== "object") {
            throw new Error("Please supply an object of options");
        }
        if (!options.message) {
            throw new Error('"message" option must not be null or an empty string.');
        }
        options = Object.assign({}, defaults, options);
        if (!options.backdrop) {
            options.backdrop = options.backdrop === false || options.backdrop === 0 ? false : "static";
        } else {
            options.backdrop = typeof options.backdrop === "string" && options.backdrop.toLowerCase() === "static" ? "static" : true;
        }
        if (!options.buttons) {
            options.buttons = {};
        }
        buttons = options.buttons;
        total = getKeyLength(buttons);
        var index = 0;
        for (var [ key, button ] of Object.entries(buttons)) {
            if (typeof button === "function") {
                button = buttons[key] = {
                    callback: button
                };
            }
            if (Object.prototype.toString.call(button).replace(/^\[object (.+)\]$/, "$1").toLowerCase() !== "object") {
                throw new Error(`button with key "${key}" must be an object`);
            }
            if (!button.label) {
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
                    button.className = "btn-primary";
                } else {
                    button.className = "btn-secondary btn-default";
                }
            }
            index++;
        }
        return options;
    }
    function getKeyLength(obj) {
        return Object.keys(obj).length;
    }
    function focusPrimaryButton() {
        trigger(de.data.dialog.querySelector(".bootbox-accept").first(), "focus");
    }
    function processCallback(e, dialog, callback) {
        e.stopPropagation();
        e.preventDefault();
        const preserveDialog = typeof callback === "function" && callback.call(dialog, e) === false;
        if (!preserveDialog && dialog) {
            bootstrap.Modal.getInstance(dialog).hide();
        }
    }
    function minAndMaxAreValid(type, min, max) {
        let result = false;
        let minValid = true;
        let maxValid = true;
        if (type === "date") {
            if (min !== undefined && !(minValid = dateIsValid(min))) {
                console.warn('Browsers which natively support the "date" input type expect date values to be of the form "YYYY-MM-DD" (see ISO-8601 https://www.iso.org/iso-8601-date-and-time-format.html). Bootbox does not enforce this rule, but your min value may not be enforced by this browser.');
            } else if (max !== undefined && !(maxValid = dateIsValid(max))) {
                console.warn('Browsers which natively support the "date" input type expect date values to be of the form "YYYY-MM-DD" (see ISO-8601 https://www.iso.org/iso-8601-date-and-time-format.html). Bootbox does not enforce this rule, but your max value may not be enforced by this browser.');
            }
        } else if (type === "time") {
            if (min !== undefined && !(minValid = timeIsValid(min))) {
                throw new Error('"min" is not a valid time. See https://www.w3.org/TR/2012/WD-html-markup-20120315/datatypes.html#form.data.time for more information.');
            } else if (max !== undefined && !(maxValid = timeIsValid(max))) {
                throw new Error('"max" is not a valid time. See https://www.w3.org/TR/2012/WD-html-markup-20120315/datatypes.html#form.data.time for more information.');
            }
        } else {
            if (min !== undefined && isNaN(min)) {
                minValid = false;
                throw new Error('"min" must be a valid number. See https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#attr-min for more information.');
            }
            if (max !== undefined && isNaN(max)) {
                maxValid = false;
                throw new Error('"max" must be a valid number. See https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#attr-max for more information.');
            }
        }
        if (minValid && maxValid) {
            if (max <= min) {
                throw new Error('"max" must be greater than "min". See https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#attr-max for more information.');
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
        if (typeof eventType === "string" && typeof el[eventType] === "function") {
            el[eventType]();
        } else {
            const event = typeof eventType === "string" ? new Event(eventType, {
                bubbles: true
            }) : eventType;
            el.dispatchEvent(event);
        }
    }
    function generateElement(html) {
        const template = document.createElement("template");
        template.innerHTML = html.trim();
        return template.content.children[0];
    }
    function addEventListener(el, eventName, eventHandler, selector) {
        if (selector) {
            const wrappedHandler = e => {
                if (!e.target) {
                    return;
                }
                const el = e.target.closest(selector);
                if (el) {
                    eventHandler.call(el, e);
                }
            };
            el.addEventListener(eventName, wrappedHandler);
            return wrappedHandler;
        } else {
            const wrappedHandler = e => {
                eventHandler.call(el, e);
            };
            el.addEventListener(eventName, wrappedHandler);
            return wrappedHandler;
        }
    }
    return exports;
});

function Notify(content, options) {
    String.format = function() {
        var args = arguments;
        const string = arguments[0];
        return string.replace(/(\{\{\d\}\}|\{\d\})/g, function(str) {
            if (str.substring(0, 2) === "{{") return str;
            const num = parseInt(str.match(/\d/)[0]);
            return args[num + 1];
        });
    };
    function isDuplicateNotification(notification) {
        var isDupe = false;
        document.querySelectorAll('[data-notify="container"]').forEach(container => {
            const $el = container, title = $el.querySelector('[data-notify="title"]').innerHTML.trim(), message = $el.querySelector('[data-notify="message"]').innerHTML.trim();
            const isSameTitle = title === `${notification.settings.content.title}`.trim(), isSameMsg = message === `${notification.settings.content.message}`.trim();
            if (isSameTitle && isSameMsg) {
                isDupe = true;
            }
            return !isDupe;
        });
        return isDupe;
    }
    const defaults = {
        element: "body",
        type: "info",
        allow_dismiss: true,
        allow_duplicates: true,
        newest_on_top: true,
        showProgressbar: false,
        placement: {
            from: "top",
            align: "right"
        },
        delay: 5e3,
        timer: 1e3,
        mouse_over: "pause",
        animate: {
            enter: "animated fadeInDown",
            exit: "animated fadeOutUp"
        },
        onShow: null,
        onShown: null,
        onClose: null,
        onClosed: null,
        onClick: null,
        icon_type: "class",
        template: [ '<div data-notify="container" class="toast fade m-3" role="alert" aria-live="assertive" aria-atomic="true">', '<div class="toast-header">', '<span data-notify="icon" class="me-2 text-{0}"></span>', '<strong class="me-auto fw-bold" data-notify="title">{1}</strong>', '<button type="button" class="ms-2 mb-1 btn-close" data-bs-dismiss="toast" data-notify="dismiss" aria-label="Close">', "</button>", "</div>", '<div class="toast-body" data-notify="message">', "{2}", '<div class="progress" role="progressbar" data-notify="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100">', '<div class="progress-bar bg-{0}" style="width: 0%;"></div>', "</div>", "</div>" ].join("")
    };
    const contentObj = {
        content: {
            message: typeof content === "object" ? content.message : content,
            title: content.title ? content.title : "",
            icon: content.icon ? content.icon : ""
        }
    };
    options = deepExtend({}, contentObj, options);
    this.settings = deepExtend({}, defaults, options);
    this._defaults = defaults;
    this.animations = {
        start: "webkitAnimationStart oanimationstart MSAnimationStart animationstart",
        end: "webkitAnimationEnd oanimationend MSAnimationEnd animationend"
    };
    if (typeof this.settings.offset === "number") {
        this.settings.offset = {
            x: this.settings.offset,
            y: this.settings.offset
        };
    }
    if (this.settings.allow_duplicates || !this.settings.allow_duplicates && !isDuplicateNotification(this)) {
        this.init();
    }
}

extend(Notify.prototype, {
    init: function() {
        var self = this;
        this.buildNotify();
        if (this.settings.content.icon) {
            this.setIcon();
        }
        this.placement();
        this.bind();
        this.notify = {
            $ele: this.$ele,
            close: function() {
                self.close();
            }
        };
    },
    update: function(command, update) {
        var commands = {};
        if (typeof command === "string") {
            commands[command] = update;
        } else {
            commands = command;
        }
        for (var cmd in commands) {
            switch (cmd) {
              default:
                this.$ele.querySelector(`[data-notify="${cmd}"]`).innerHTML = commands[cmd];
            }
        }
    },
    buildNotify: function() {
        const content = this.settings.content;
        const div = document.createElement("div");
        div.innerHTML = String.format(this.settings.template, this.settings.type, content.title, content.message, content.url, content.target);
        this.$ele = div.firstChild;
        this.$ele.dataset.notifyPosition = this.settings.placement.from + "-" + this.settings.placement.align;
        this.$ele.dataset.bsDelay = this.settings.delay;
        if (!this.settings.allow_dismiss) {
            this.$ele.querySelector('[data-notify="dismiss"]').style.display = "none";
        }
        if (this.settings.delay <= 0 && !this.settings.showProgressbar || !this.settings.showProgressbar) {
            if (this.$ele.querySelector('[data-notify="progressbar"]') != null) {
                this.$ele.querySelector('[data-notify="progressbar"]').remove();
            }
        }
    },
    setIcon: function() {
        if (this.settings.icon_type.toLowerCase() === "class") {
            this.$ele.querySelector('[data-notify="icon"]').className += ` ${this.settings.content.icon}`;
        } else {
            if (this.$ele.querySelector('[data-notify="icon"]').nodeName === "IMG") {
                const image = this.$ele.querySelector('[data-notify="icon"]');
                image.src = this.settings.content.icon;
                image.className = "me-2";
            } else {
                const image = document.createElement("img");
                image.src = `${this.settings.content.icon}`;
                image.alt = "Notify Icon";
                image.className = "me-2";
                this.$ele.querySelector('[data-notify="icon"]').append(image);
            }
        }
    },
    placement: function() {
        var self = this;
        this.$ele.className += ` ${this.settings.animate.enter}`;
        const toast = new bootstrap.Toast(this.$ele);
        toast.show();
        const pre = [ "webkit-", "moz-", "o-", "ms-", "" ];
        pre.forEach(prefix => {
            self.cssText += prefix + "AnimationIterationCount: " + 1;
        });
        if (document.querySelector(".toast-container") == null) {
            const container = document.createElement("div");
            container.className = "toast-container position-fixed";
            switch (this.settings.placement.from) {
              case "top":
                container.className += " top-0";
                break;

              case "bottom":
                container.className += " bottom-0";
                break;
            }
            switch (this.settings.placement.align) {
              case "left":
                container.className += " start-0";
                break;

              case "right":
                container.className += " end-0";
                break;

              case "center":
                container.className += " start-50 translate-middle-x";
                break;
            }
            document.querySelector(this.settings.element).append(container);
        }
        if (this.settings.newest_on_top === true) {
            document.querySelector(".toast-container").prepend(this.$ele);
        } else {
            document.querySelector(".toast-container").append(this.$ele);
        }
        if (typeof self.settings.onShow === "function") {
            self.settings.onShow.call(this.$ele);
        }
    },
    bind: function() {
        var self = this;
        if (this.$ele.querySelector('[data-notify="dismiss"]') != null) {
            this.$ele.querySelector('[data-notify="dismiss"]').addEventListener("click", () => {
                self.close();
            });
        }
        if (typeof self.settings.onClick === "function") {
            this.$ele.addEventListener("click", event => {
                if (event.target !== self.$ele.querySelector('[data-notify="dismiss"]')) {
                    self.settings.onClick.call(this, event);
                }
            });
        }
        this.$ele.addEventListener("mouseover", () => {
            this.$ele.dataset.hover = "true";
        });
        this.$ele.addEventListener("mouseout", () => {
            this.$ele.dataset.hover = "false";
        });
        this.$ele.dataset.hover = "false";
        if (this.settings.delay > 0) {
            self.$ele.dataset.notifyDelay = self.settings.delay;
            var timer = setInterval(function() {
                const delay = parseInt(self.$ele.dataset.notifyDelay) - self.settings.timer;
                if (self.$ele.dataset.hover === "false" && self.settings.mouse_over === "pause" || self.settings.mouse_over !== "pause") {
                    const percent = (self.settings.delay - delay) / self.settings.delay * 100;
                    self.$ele.dataset.notifyDelay = delay;
                    if (self.settings.showProgressbar) {
                        const div = self.$ele.querySelector('[data-notify="progressbar"] > div');
                        self.$ele.querySelector('[data-notify="progressbar"]').setAttribute("aria-valuenow", percent);
                        div.style.width = percent + "%";
                    }
                }
                if (delay <= -self.settings.timer) {
                    clearInterval(timer);
                    self.close();
                }
            }, self.settings.timer);
        }
    },
    close: function() {
        const self = this;
        this.$ele.dataset.closing = "true";
        this.$ele.className = `toast ${this.settings.animate.exit}`;
        if (typeof self.settings.onClose === "function") {
            self.settings.onClose.call(this.$ele);
        }
        self.$ele.remove();
    }
});

function extend(a, b) {
    for (let key in b) if (b.hasOwnProperty(key)) a[key] = b[key];
    return a;
}

function deepExtend(out, ...arguments_) {
    if (!out) {
        return {};
    }
    for (const obj of arguments_) {
        if (!obj) {
            continue;
        }
        for (const [ key, value ] of Object.entries(obj)) {
            switch (Object.prototype.toString.call(value)) {
              case "[object Object]":
                out[key] = out[key] || {};
                out[key] = deepExtend(out[key], value);
                break;

              case "[object Array]":
                out[key] = deepExtend(new Array(value.length), value);
                break;

              default:
                out[key] = value;
            }
        }
    }
    return out;
}

var DarkEditable = function(m) {
    "use strict";
    var _ = Object.defineProperty;
    var g = (m, r, a) => r in m ? _(m, r, {
        enumerable: !0,
        configurable: !0,
        writable: !0,
        value: a
    }) : m[r] = a;
    var i = (m, r, a) => g(m, typeof r != "symbol" ? r + "" : r, a);
    var r = document.createElement("style");
    r.textContent = `.dark-editable-element{border-bottom:dashed 1px #0088cc;text-decoration:none;cursor:pointer}.dark-editable-element-disabled{border-bottom:none;cursor:default}.dark-editable-element-empty{font-style:italic;color:#d14}.dark-editable{max-width:none}.dark-editable-loader{font-size:5px;left:50%;top:50%;width:1em;height:1em;border-radius:50%;position:relative;text-indent:-9999em;-webkit-animation:load5 1.1s infinite ease;animation:load5 1.1s infinite ease;-webkit-transform:translateZ(0);-ms-transform:translateZ(0);transform:translateZ(0)}@-webkit-keyframes load5{0%,to{box-shadow:0 -2.6em #000,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #00000080,-1.8em -1.8em #000000b3}12.5%{box-shadow:0 -2.6em #000000b3,1.8em -1.8em #000,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #00000080}25%{box-shadow:0 -2.6em #00000080,1.8em -1.8em #000000b3,2.5em 0 #000,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}37.5%{box-shadow:0 -2.6em #0003,1.8em -1.8em #00000080,2.5em 0 #000000b3,1.75em 1.75em #000,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}50%{box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #00000080,1.75em 1.75em #000000b3,0 2.5em #000,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}62.5%{box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #00000080,0 2.5em #000000b3,-1.8em 1.8em #000,-2.6em 0 #0003,-1.8em -1.8em #0003}75%{box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #00000080,-1.8em 1.8em #000000b3,-2.6em 0 #000,-1.8em -1.8em #0003}87.5%{box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #00000080,-2.6em 0 #000000b3,-1.8em -1.8em #000}}@keyframes load5{0%,to{box-shadow:0 -2.6em #000,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #00000080,-1.8em -1.8em #000000b3}12.5%{box-shadow:0 -2.6em #000000b3,1.8em -1.8em #000,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #00000080}25%{box-shadow:0 -2.6em #00000080,1.8em -1.8em #000000b3,2.5em 0 #000,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}37.5%{box-shadow:0 -2.6em #0003,1.8em -1.8em #00000080,2.5em 0 #000000b3,1.75em 1.75em #000,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}50%{box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #00000080,1.75em 1.75em #000000b3,0 2.5em #000,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}62.5%{box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #00000080,0 2.5em #000000b3,-1.8em 1.8em #000,-2.6em 0 #0003,-1.8em -1.8em #0003}75%{box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #00000080,-1.8em 1.8em #000000b3,-2.6em 0 #000,-1.8em -1.8em #0003}87.5%{box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #00000080,-2.6em 0 #000000b3,-1.8em -1.8em #000}}
/*$vite$:1*/`, document.head.appendChild(r);
    class a {
        constructor(e) {
            i(this, "context");
            if (this.constructor === a) throw new Error("It's abstract class");
            this.context = e;
        }
        event_show() {
            if (this.context.typeElement.hideError(), !this.context.typeElement.element) throw new Error("Element is missing!");
            this.context.typeElement.element.value = this.context.getValue(), this.context.element.dispatchEvent(new CustomEvent("show"));
        }
        event_shown() {
            this.context.element.dispatchEvent(new CustomEvent("shown"));
        }
        event_hide() {
            this.context.element.dispatchEvent(new CustomEvent("hide"));
        }
        event_hidden() {
            this.context.element.dispatchEvent(new CustomEvent("hidden"));
        }
        init() {
            throw new Error("Method `init` not define!");
        }
        enable() {
            throw new Error("Method `enable` not define!");
        }
        disable() {
            throw new Error("Method `disable` not define!");
        }
        hide() {
            throw new Error("Method `hide` not define!");
        }
    }
    class p extends a {
        constructor() {
            super(...arguments);
            i(this, "popover", null);
        }
        init() {
            this.popover = new m.Popover(this.context.element, {
                container: "body",
                content: this.context.typeElement.create(),
                html: !0,
                customClass: "dark-editable",
                title: this.context.options.title
            }), this.context.element.addEventListener("show.bs.popover", () => {
                this.event_show();
            }), this.context.element.addEventListener("shown.bs.popover", () => {
                this.event_shown();
            }), this.context.element.addEventListener("hide.bs.popover", () => {
                this.event_hide();
            }), this.context.element.addEventListener("hidden.bs.popover", () => {
                this.event_hidden();
            }), document.addEventListener("click", t => {
                const s = t.target;
                if (this.popover && s === this.popover.tip || s === this.context.element) return;
                let n = s.parentNode;
                for (;n; ) {
                    if (n === this.popover.tip) return;
                    n = n.parentNode;
                }
                this.hide();
            });
        }
        enable() {
            this.popover && this.popover.enable();
        }
        disable() {
            this.popover && this.popover.disable();
        }
        hide() {
            this.popover && this.popover.hide();
        }
    }
    class u extends a {
        init() {
            const e = () => {
                if (!this.context.options.disabled) {
                    const t = this.context.typeElement.create();
                    this.event_show(), this.context.element.removeEventListener("click", e), 
                    this.context.element.innerHTML = "", this.context.element.append(t), 
                    this.event_shown();
                }
            };
            this.context.element.addEventListener("click", e);
        }
        enable() {}
        disable() {}
        hide() {
            this.event_hide(), this.context.element.innerHTML = this.context.getValue(), 
            setTimeout(() => {
                this.init(), this.event_hidden();
            }, 100);
        }
    }
    class c {
        constructor(e) {
            i(this, "context");
            i(this, "element", null);
            i(this, "error", null);
            i(this, "form", null);
            i(this, "load", null);
            i(this, "buttons", {
                success: null,
                cancel: null
            });
            if (this.constructor === c) throw new Error("It's abstract class");
            this.context = e;
        }
        create() {
            throw new Error("Method `create` not define!");
        }
        createContainer(e) {
            const t = document.createElement("div");
            return this.element = e, this.error = this.createContainerError(), this.form = this.createContainerForm(), 
            this.load = this.createContainerLoad(), this.form.append(e, this.load), 
            this.buttons.success = null, this.buttons.cancel = null, this.context.options.showbuttons && (this.buttons.success = this.createButtonSuccess(), 
            this.buttons.cancel = this.createButtonCancel(), this.form.append(this.buttons.success, this.buttons.cancel)), 
            t.append(this.error, this.form), t;
        }
        createContainerError() {
            const e = document.createElement("div");
            return e.classList.add("text-danger", "fst-italic", "mb-2", "fw-bold"), 
            e.style.display = "none", e;
        }
        createContainerForm() {
            const e = document.createElement("form");
            return e.classList.add("d-flex", "align-items-start"), e.style.gap = "10px", 
            e.addEventListener("submit", async t => {
                t.preventDefault();
                const s = this.getValue();
                if (this.context.options.send && this.context.options.id && this.context.options.url && this.context.getValue() !== s) {
                    this.showLoad();
                    let n;
                    try {
                        const o = await this.ajax(s);
                        o.ok ? n = await this.context.success(o, s) : n = await this.context.error(o, s) || `${o.status} ${o.statusText}`;
                    } catch (o) {
                        console.error(o), n = o;
                    }
                    n ? (this.setError(n), this.showError()) : (this.setError(""), 
                    this.hideError(), this.context.setValue(this.getValue()), this.context.modeElement.hide(), 
                    this.initText()), this.hideLoad();
                } else this.context.setValue(this.getValue()), this.context.modeElement.hide(), 
                this.initText();
                this.context.element.dispatchEvent(new CustomEvent("save"));
            }), e;
        }
        createContainerLoad() {
            const e = document.createElement("div");
            e.style.display = "none", e.style.position = "absolute", e.style.background = "white", 
            e.style.width = "100%", e.style.height = "100%", e.style.top = "0", 
            e.style.left = "0";
            const t = document.createElement("div");
            return t.classList.add("dark-editable-loader"), e.append(t), e;
        }
        createButton() {
            const e = document.createElement("button");
            return e.type = "button", e.classList.add("btn", "btn-sm"), e;
        }
        createButtonSuccess() {
            const e = this.createButton();
            return e.type = "submit", e.classList.add("btn-success"), e.innerHTML = '<i class="fa-solid fa-check"></i>', 
            e;
        }
        createButtonCancel() {
            const e = this.createButton();
            return e.classList.add("btn-danger"), e.innerHTML = '<i class="fa-solid fa-times"></i>', 
            e.addEventListener("click", () => {
                this.context.modeElement.hide();
            }), e;
        }
        hideLoad() {
            this.load && (this.load.style.display = "none");
        }
        showLoad() {
            this.load && (this.load.style.display = "block");
        }
        ajax(e) {
            var o;
            let t = this.context.options.url;
            if (!t) throw new Error("URL is required!");
            if (!this.context.options.id) throw new Error("pk is required!");
            if (!this.context.options.name) throw new Error("Name is required!");
            const s = new FormData();
            if (s.append("id", this.context.options.id), s.append("name", this.context.options.name), 
            s.append("value", e), ((o = this.context.options.ajaxOptions) == null ? void 0 : o.method) === "GET") {
                const d = [];
                s.forEach((y, v) => {
                    d.push(`${v}=${y}`);
                }), t += "?" + d.join("&");
            }
            const n = {
                ...this.context.options.ajaxOptions
            };
            return n.body = s, fetch(t, n);
        }
        async successResponse(e, t) {}
        async errorResponse(e, t) {}
        setError(e) {
            this.error && (this.error.innerHTML = e);
        }
        showError() {
            this.error && (this.error.style.display = "block");
        }
        hideError() {
            this.error && (this.error.style.display = "none");
        }
        createElement(e) {
            const t = document.createElement(e);
            return t.classList.add("form-control"), this.context.options.required && (t.required = this.context.options.required), 
            this.context.options.placeholder && (t.placeholder = this.context.options.placeholder), 
            this.context.options.showbuttons || t.addEventListener("change", () => {
                this.form && this.form.dispatchEvent(new Event("submit"));
            }), this.add_focus(t), t;
        }
        add_focus(e) {
            this.context.element.addEventListener("shown", function() {
                e.focus();
            });
        }
        initText() {
            return this.context.getValue() === "" ? (this.context.element.innerHTML = this.context.options.emptytext || "", 
            !0) : (this.context.element.innerHTML = this.context.getValue(), !1);
        }
        initOptions() {}
        getValue() {
            return this.element ? this.element.value : "";
        }
    }
    class x extends c {
        create() {
            const e = this.createElement("input");
            return e.type = typeof this.context.options.type == "string" ? this.context.options.type : "text", 
            this.createContainer(e);
        }
    }
    class b extends c {
        create() {
            const e = this.createElement("textarea");
            return this.createContainer(e);
        }
    }
    class f extends c {
        create() {
            const e = this.createElement("select");
            return this.context.options.source && Array.isArray(this.context.options.source) && this.context.options.source.forEach(t => {
                const s = document.createElement("option");
                s.value = t.value, s.innerHTML = t.text, e.append(s);
            }), this.createContainer(e);
        }
        initText() {
            if (this.context.element.innerHTML = this.context.options.emptytext || "", 
            this.context.getValue() !== "" && this.context.options.source && Array.isArray(this.context.options.source) && this.context.options.source.length > 0) for (let e = 0; e < this.context.options.source.length; e++) {
                const t = this.context.options.source[e];
                if (t.value == this.context.getValue()) return this.context.element.innerHTML = t.text, 
                !1;
            }
            return !0;
        }
        initOptions() {
            this.context.get_opt("source", []), this.context.options && typeof this.context.options.source == "string" && this.context.options.source !== "" && (this.context.options.source = JSON.parse(this.context.options.source));
        }
    }
    class l extends c {
        create() {
            const e = this.createElement("input");
            return e.type = "date", this.createContainer(e);
        }
        initText() {
            return this.context.getValue() === "" ? (this.context.element.innerHTML = this.context.options.emptytext || "", 
            !0) : (this.context.element.innerHTML = this.context.getValue(), !1);
        }
        initOptions() {
            this.context.get_opt("format", "YYYY-MM-DD"), this.context.get_opt("viewformat", "YYYY-MM-DD");
        }
    }
    class w extends l {
        create() {
            const e = this.createElement("input");
            return e.type = "datetime-local", this.createContainer(e);
        }
        initOptions() {
            this.context.get_opt("format", "YYYY-MM-DD HH:mm"), this.context.get_opt("viewformat", "YYYY-MM-DD HH:mm"), 
            this.context.setValue(this.context.getValue());
        }
    }
    class E {
        constructor(e, t = {}) {
            i(this, "element");
            i(this, "options");
            i(this, "typeElement");
            i(this, "modeElement");
            this.element = e, this.options = {
                ...t
            }, this.init_options(), this.typeElement = this.route_type(), this.typeElement.initOptions(), 
            this.modeElement = this.route_mode(), this.modeElement.init(), this.setValue(this.element.innerHTML), 
            this.init_style(), this.options.disabled && this.disable(), this.element.dispatchEvent(new CustomEvent("init"));
        }
        get_opt(e, t) {
            var s, n;
            this.options[e] = ((s = this.element.dataset) == null ? void 0 : s[e]) ?? ((n = this.options) == null ? void 0 : n[e]) ?? t;
        }
        get_opt_bool(e, t) {
            if (this.get_opt(e, t), typeof this.options[e] != "boolean") {
                if (this.options[e] === "true") {
                    this.options[e] = !0;
                    return;
                }
                if (this.options[e] === "false") {
                    this.options[e] = !1;
                    return;
                }
                this.options[e] = t;
            }
        }
        init_options() {
            var e, t, s, n, o;
            this.get_opt("value", this.element.innerHTML), this.get_opt("name", this.element.id), 
            this.get_opt("id", null), this.get_opt("title", ""), this.get_opt("type", "text"), 
            this.get_opt("emptytext", "Empty"), this.get_opt("placeholder", this.element.getAttribute("placeholder")), 
            this.get_opt("mode", "popup"), this.get_opt("url", null), this.get_opt("ajaxOptions", {}), 
            this.options.ajaxOptions = Object.assign({
                method: "POST",
                dataType: "text",
                headers: {
                    RequestVerificationToken: (e = document.querySelector('input[name="__RequestVerificationToken"]')) == null ? void 0 : e.value
                }
            }, this.options.ajaxOptions), this.get_opt_bool("send", !0), this.get_opt_bool("disabled", !1), 
            this.get_opt_bool("required", !1), this.get_opt_bool("showbuttons", !0), 
            (t = this.options) != null && t.success && typeof ((s = this.options) == null ? void 0 : s.success) == "function" && (this.success = this.options.success), 
            (n = this.options) != null && n.error && typeof ((o = this.options) == null ? void 0 : o.error) == "function" && (this.error = this.options.error);
        }
        init_text() {
            const e = "dark-editable-element-empty";
            this.element.classList.remove(e), this.typeElement.initText() && this.element.classList.add(e);
        }
        init_style() {
            this.element.classList.add("dark-editable-element");
        }
        route_mode() {
            switch (this.options.mode) {
              default:
                throw new Error(`Mode ${this.options.mode} not found!`);

              case "popup":
                return new p(this);

              case "inline":
                return new u(this);
            }
        }
        route_type() {
            if (this.options.type && typeof this.options.type != "string") return new this.options.type(this);
            switch (this.options.type) {
              case "text":
              case "password":
              case "email":
              case "url":
              case "tel":
              case "number":
              case "range":
              case "time":
                return new x(this);

              case "textarea":
                return new b(this);

              case "select":
                return new f(this);

              case "date":
                return new l(this);

              case "datetime":
                return new w(this);
            }
            throw new Error("Undefined type");
        }
        async success(e, t) {
            return await this.typeElement.successResponse(e, t);
        }
        async error(e, t) {
            return await this.typeElement.errorResponse(e, t);
        }
        enable() {
            this.options.disabled = !1, this.element.classList.remove("dark-editable-element-disabled"), 
            this.modeElement.enable();
        }
        disable() {
            this.options.disabled = !0, this.element.classList.add("dark-editable-element-disabled"), 
            this.modeElement.enable();
        }
        setValue(e) {
            this.options.value = e, this.init_text();
        }
        getValue() {
            return this.options.value ?? "";
        }
    }
    return E;
}(bootstrap);

document.addEventListener("DOMContentLoaded", function() {
    document.querySelectorAll("input[type='number']").forEach(input => {
        if (!input.parentNode.classList.contains("input-group")) {
            const wrapDiv = document.createElement("div");
            wrapDiv.classList.add("input-group");
            wrap(input, wrapDiv);
        }
        const minusButton = document.createElement("button");
        minusButton.classList.add("btn");
        minusButton.classList.add("btn-secondary");
        minusButton.classList.add("bootstrap-touchspin-down");
        minusButton.type = "button";
        minusButton.addEventListener("click", touchSpinDown);
        minusButton.innerHTML = '<i class="fa-solid fa-minus"></i>';
        input.parentNode.insertBefore(minusButton, input);
        const plusButton = document.createElement("button");
        plusButton.classList.add("btn");
        plusButton.classList.add("btn-secondary");
        plusButton.classList.add("bootstrap-touchspin-up");
        plusButton.type = "button";
        plusButton.addEventListener("click", touchSpinUp);
        plusButton.innerHTML = '<i class="fa-solid fa-plus"></i>';
        input.parentNode.append(plusButton);
    });
    function wrap(el, wrapper) {
        el.parentNode.insertBefore(wrapper, el);
        wrapper.appendChild(el);
    }
    function touchSpinDown() {
        const btn = this, input = btn.nextSibling, oldValue = input.value.trim();
        let newVal, minValue = 1;
        if (input.classList.contains("form-control-days")) {
            minValue = input.dataset.min;
        } else if (input.classList.contains("serverTime-Input")) {
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
        const btn = this, input = btn.previousSibling, oldValue = input.value.trim();
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

(function(global, factory) {
    typeof exports === "object" && typeof module !== "undefined" ? module.exports = factory() : typeof define === "function" && define.amd ? define(factory) : (global = typeof globalThis !== "undefined" ? globalThis : global || self, 
    global.Choices = factory());
})(this, function() {
    "use strict";
    var extendStatics = function(d, b) {
        extendStatics = Object.setPrototypeOf || {
            __proto__: []
        } instanceof Array && function(d, b) {
            d.__proto__ = b;
        } || function(d, b) {
            for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p];
        };
        return extendStatics(d, b);
    };
    function __extends(d, b) {
        if (typeof b !== "function" && b !== null) throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() {
            this.constructor = d;
        }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, 
        new __());
    }
    var __assign = function() {
        __assign = Object.assign || function __assign(t) {
            for (var s, i = 1, n = arguments.length; i < n; i++) {
                s = arguments[i];
                for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p)) t[p] = s[p];
            }
            return t;
        };
        return __assign.apply(this, arguments);
    };
    function __spreadArray(to, from, pack) {
        if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
            if (ar || !(i in from)) {
                if (!ar) ar = Array.prototype.slice.call(from, 0, i);
                ar[i] = from[i];
            }
        }
        return to.concat(ar || Array.prototype.slice.call(from));
    }
    typeof SuppressedError === "function" ? SuppressedError : function(error, suppressed, message) {
        var e = new Error(message);
        return e.name = "SuppressedError", e.error = error, e.suppressed = suppressed, 
        e;
    };
    var ActionType = {
        ADD_CHOICE: "ADD_CHOICE",
        REMOVE_CHOICE: "REMOVE_CHOICE",
        FILTER_CHOICES: "FILTER_CHOICES",
        ACTIVATE_CHOICES: "ACTIVATE_CHOICES",
        CLEAR_CHOICES: "CLEAR_CHOICES",
        ADD_GROUP: "ADD_GROUP",
        ADD_ITEM: "ADD_ITEM",
        REMOVE_ITEM: "REMOVE_ITEM",
        HIGHLIGHT_ITEM: "HIGHLIGHT_ITEM"
    };
    var EventType = {
        showDropdown: "showDropdown",
        hideDropdown: "hideDropdown",
        change: "change",
        choice: "choice",
        search: "search",
        addItem: "addItem",
        removeItem: "removeItem",
        highlightItem: "highlightItem",
        highlightChoice: "highlightChoice",
        unhighlightItem: "unhighlightItem"
    };
    var KeyCodeMap = {
        TAB_KEY: 9,
        SHIFT_KEY: 16,
        BACK_KEY: 46,
        DELETE_KEY: 8,
        ENTER_KEY: 13,
        A_KEY: 65,
        ESC_KEY: 27,
        UP_KEY: 38,
        DOWN_KEY: 40,
        PAGE_UP_KEY: 33,
        PAGE_DOWN_KEY: 34
    };
    var ObjectsInConfig = [ "fuseOptions", "classNames" ];
    var PassedElementTypes = {
        Text: "text",
        SelectOne: "select-one",
        SelectMultiple: "select-multiple"
    };
    var addChoice = function(choice) {
        return {
            type: ActionType.ADD_CHOICE,
            choice: choice
        };
    };
    var removeChoice = function(choice) {
        return {
            type: ActionType.REMOVE_CHOICE,
            choice: choice
        };
    };
    var filterChoices = function(results) {
        return {
            type: ActionType.FILTER_CHOICES,
            results: results
        };
    };
    var activateChoices = function(active) {
        return {
            type: ActionType.ACTIVATE_CHOICES,
            active: active
        };
    };
    var addGroup = function(group) {
        return {
            type: ActionType.ADD_GROUP,
            group: group
        };
    };
    var addItem = function(item) {
        return {
            type: ActionType.ADD_ITEM,
            item: item
        };
    };
    var removeItem$1 = function(item) {
        return {
            type: ActionType.REMOVE_ITEM,
            item: item
        };
    };
    var highlightItem = function(item, highlighted) {
        return {
            type: ActionType.HIGHLIGHT_ITEM,
            item: item,
            highlighted: highlighted
        };
    };
    var getRandomNumber = function(min, max) {
        return Math.floor(Math.random() * (max - min) + min);
    };
    var generateChars = function(length) {
        return Array.from({
            length: length
        }, function() {
            return getRandomNumber(0, 36).toString(36);
        }).join("");
    };
    var generateId = function(element, prefix) {
        var id = element.id || element.name && "".concat(element.name, "-").concat(generateChars(2)) || generateChars(4);
        id = id.replace(/(:|\.|\[|\]|,)/g, "");
        id = "".concat(prefix, "-").concat(id);
        return id;
    };
    var getAdjacentEl = function(startEl, selector, direction) {
        if (direction === void 0) {
            direction = 1;
        }
        var prop = "".concat(direction > 0 ? "next" : "previous", "ElementSibling");
        var sibling = startEl[prop];
        while (sibling) {
            if (sibling.matches(selector)) {
                return sibling;
            }
            sibling = sibling[prop];
        }
        return null;
    };
    var isScrolledIntoView = function(element, parent, direction) {
        if (direction === void 0) {
            direction = 1;
        }
        var isVisible;
        if (direction > 0) {
            isVisible = parent.scrollTop + parent.offsetHeight >= element.offsetTop + element.offsetHeight;
        } else {
            isVisible = element.offsetTop >= parent.scrollTop;
        }
        return isVisible;
    };
    var sanitise = function(value) {
        if (typeof value !== "string") {
            if (value === null || value === undefined) {
                return "";
            }
            if (typeof value === "object") {
                if ("raw" in value) {
                    return sanitise(value.raw);
                }
                if ("trusted" in value) {
                    return value.trusted;
                }
            }
            return value;
        }
        return value.replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;").replace(/'/g, "&#039;").replace(/"/g, "&quot;");
    };
    var strToEl = function() {
        var tmpEl = document.createElement("div");
        return function(str) {
            tmpEl.innerHTML = str.trim();
            var firstChild = tmpEl.children[0];
            while (tmpEl.firstChild) {
                tmpEl.removeChild(tmpEl.firstChild);
            }
            return firstChild;
        };
    }();
    var resolveNoticeFunction = function(fn, value) {
        return typeof fn === "function" ? fn(sanitise(value), value) : fn;
    };
    var resolveStringFunction = function(fn) {
        return typeof fn === "function" ? fn() : fn;
    };
    var unwrapStringForRaw = function(s) {
        if (typeof s === "string") {
            return s;
        }
        if (typeof s === "object") {
            if ("trusted" in s) {
                return s.trusted;
            }
            if ("raw" in s) {
                return s.raw;
            }
        }
        return "";
    };
    var unwrapStringForEscaped = function(s) {
        if (typeof s === "string") {
            return s;
        }
        if (typeof s === "object") {
            if ("escaped" in s) {
                return s.escaped;
            }
            if ("trusted" in s) {
                return s.trusted;
            }
        }
        return "";
    };
    var escapeForTemplate = function(allowHTML, s) {
        return allowHTML ? unwrapStringForEscaped(s) : sanitise(s);
    };
    var setElementHtml = function(el, allowHtml, html) {
        el.innerHTML = escapeForTemplate(allowHtml, html);
    };
    var sortByAlpha = function(_a, _b) {
        var value = _a.value, _c = _a.label, label = _c === void 0 ? value : _c;
        var value2 = _b.value, _d = _b.label, label2 = _d === void 0 ? value2 : _d;
        return unwrapStringForRaw(label).localeCompare(unwrapStringForRaw(label2), [], {
            sensitivity: "base",
            ignorePunctuation: true,
            numeric: true
        });
    };
    var sortByRank = function(a, b) {
        return a.rank - b.rank;
    };
    var dispatchEvent = function(element, type, customArgs) {
        if (customArgs === void 0) {
            customArgs = null;
        }
        var event = new CustomEvent(type, {
            detail: customArgs,
            bubbles: true,
            cancelable: true
        });
        return element.dispatchEvent(event);
    };
    var diff = function(a, b) {
        var aKeys = Object.keys(a).sort();
        var bKeys = Object.keys(b).sort();
        return aKeys.filter(function(i) {
            return bKeys.indexOf(i) < 0;
        });
    };
    var getClassNames = function(ClassNames) {
        return Array.isArray(ClassNames) ? ClassNames : [ ClassNames ];
    };
    var getClassNamesSelector = function(option) {
        if (option && Array.isArray(option)) {
            return option.map(function(item) {
                return ".".concat(item);
            }).join("");
        }
        return ".".concat(option);
    };
    var addClassesToElement = function(element, className) {
        var _a;
        (_a = element.classList).add.apply(_a, getClassNames(className));
    };
    var removeClassesFromElement = function(element, className) {
        var _a;
        (_a = element.classList).remove.apply(_a, getClassNames(className));
    };
    var parseCustomProperties = function(customProperties) {
        if (typeof customProperties !== "undefined") {
            try {
                return JSON.parse(customProperties);
            } catch (e) {
                return customProperties;
            }
        }
        return {};
    };
    var updateClassList = function(item, add, remove) {
        var itemEl = item.itemEl;
        if (itemEl) {
            removeClassesFromElement(itemEl, remove);
            addClassesToElement(itemEl, add);
        }
    };
    var Dropdown = function() {
        function Dropdown(_a) {
            var element = _a.element, type = _a.type, classNames = _a.classNames;
            this.element = element;
            this.classNames = classNames;
            this.type = type;
            this.isActive = false;
        }
        Dropdown.prototype.show = function() {
            addClassesToElement(this.element, this.classNames.activeState);
            this.element.setAttribute("aria-expanded", "true");
            this.isActive = true;
            return this;
        };
        Dropdown.prototype.hide = function() {
            removeClassesFromElement(this.element, this.classNames.activeState);
            this.element.setAttribute("aria-expanded", "false");
            this.isActive = false;
            return this;
        };
        return Dropdown;
    }();
    var Container = function() {
        function Container(_a) {
            var element = _a.element, type = _a.type, classNames = _a.classNames, position = _a.position;
            this.element = element;
            this.classNames = classNames;
            this.type = type;
            this.position = position;
            this.isOpen = false;
            this.isFlipped = false;
            this.isDisabled = false;
            this.isLoading = false;
        }
        Container.prototype.shouldFlip = function(dropdownPos, dropdownHeight) {
            var shouldFlip = false;
            if (this.position === "auto") {
                shouldFlip = this.element.getBoundingClientRect().top - dropdownHeight >= 0 && !window.matchMedia("(min-height: ".concat(dropdownPos + 1, "px)")).matches;
            } else if (this.position === "top") {
                shouldFlip = true;
            }
            return shouldFlip;
        };
        Container.prototype.setActiveDescendant = function(activeDescendantID) {
            this.element.setAttribute("aria-activedescendant", activeDescendantID);
        };
        Container.prototype.removeActiveDescendant = function() {
            this.element.removeAttribute("aria-activedescendant");
        };
        Container.prototype.open = function(dropdownPos, dropdownHeight) {
            addClassesToElement(this.element, this.classNames.openState);
            this.element.setAttribute("aria-expanded", "true");
            this.isOpen = true;
            if (this.shouldFlip(dropdownPos, dropdownHeight)) {
                addClassesToElement(this.element, this.classNames.flippedState);
                this.isFlipped = true;
            }
        };
        Container.prototype.close = function() {
            removeClassesFromElement(this.element, this.classNames.openState);
            this.element.setAttribute("aria-expanded", "false");
            this.removeActiveDescendant();
            this.isOpen = false;
            if (this.isFlipped) {
                removeClassesFromElement(this.element, this.classNames.flippedState);
                this.isFlipped = false;
            }
        };
        Container.prototype.addFocusState = function() {
            addClassesToElement(this.element, this.classNames.focusState);
        };
        Container.prototype.removeFocusState = function() {
            removeClassesFromElement(this.element, this.classNames.focusState);
        };
        Container.prototype.enable = function() {
            removeClassesFromElement(this.element, this.classNames.disabledState);
            this.element.removeAttribute("aria-disabled");
            if (this.type === PassedElementTypes.SelectOne) {
                this.element.setAttribute("tabindex", "0");
            }
            this.isDisabled = false;
        };
        Container.prototype.disable = function() {
            addClassesToElement(this.element, this.classNames.disabledState);
            this.element.setAttribute("aria-disabled", "true");
            if (this.type === PassedElementTypes.SelectOne) {
                this.element.setAttribute("tabindex", "-1");
            }
            this.isDisabled = true;
        };
        Container.prototype.wrap = function(element) {
            var el = this.element;
            var parentNode = element.parentNode;
            if (parentNode) {
                if (element.nextSibling) {
                    parentNode.insertBefore(el, element.nextSibling);
                } else {
                    parentNode.appendChild(el);
                }
            }
            el.appendChild(element);
        };
        Container.prototype.unwrap = function(element) {
            var el = this.element;
            var parentNode = el.parentNode;
            if (parentNode) {
                parentNode.insertBefore(element, el);
                parentNode.removeChild(el);
            }
        };
        Container.prototype.addLoadingState = function() {
            addClassesToElement(this.element, this.classNames.loadingState);
            this.element.setAttribute("aria-busy", "true");
            this.isLoading = true;
        };
        Container.prototype.removeLoadingState = function() {
            removeClassesFromElement(this.element, this.classNames.loadingState);
            this.element.removeAttribute("aria-busy");
            this.isLoading = false;
        };
        return Container;
    }();
    var Input = function() {
        function Input(_a) {
            var element = _a.element, type = _a.type, classNames = _a.classNames, preventPaste = _a.preventPaste;
            this.element = element;
            this.type = type;
            this.classNames = classNames;
            this.preventPaste = preventPaste;
            this.isFocussed = this.element.isEqualNode(document.activeElement);
            this.isDisabled = element.disabled;
            this._onPaste = this._onPaste.bind(this);
            this._onInput = this._onInput.bind(this);
            this._onFocus = this._onFocus.bind(this);
            this._onBlur = this._onBlur.bind(this);
        }
        Object.defineProperty(Input.prototype, "placeholder", {
            set: function(placeholder) {
                this.element.placeholder = placeholder;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(Input.prototype, "value", {
            get: function() {
                return this.element.value;
            },
            set: function(value) {
                this.element.value = value;
            },
            enumerable: false,
            configurable: true
        });
        Input.prototype.addEventListeners = function() {
            var el = this.element;
            el.addEventListener("paste", this._onPaste);
            el.addEventListener("input", this._onInput, {
                passive: true
            });
            el.addEventListener("focus", this._onFocus, {
                passive: true
            });
            el.addEventListener("blur", this._onBlur, {
                passive: true
            });
        };
        Input.prototype.removeEventListeners = function() {
            var el = this.element;
            el.removeEventListener("input", this._onInput);
            el.removeEventListener("paste", this._onPaste);
            el.removeEventListener("focus", this._onFocus);
            el.removeEventListener("blur", this._onBlur);
        };
        Input.prototype.enable = function() {
            var el = this.element;
            el.removeAttribute("disabled");
            this.isDisabled = false;
        };
        Input.prototype.disable = function() {
            var el = this.element;
            el.setAttribute("disabled", "");
            this.isDisabled = true;
        };
        Input.prototype.focus = function() {
            if (!this.isFocussed) {
                this.element.focus();
            }
        };
        Input.prototype.blur = function() {
            if (this.isFocussed) {
                this.element.blur();
            }
        };
        Input.prototype.clear = function(setWidth) {
            if (setWidth === void 0) {
                setWidth = true;
            }
            this.element.value = "";
            if (setWidth) {
                this.setWidth();
            }
            return this;
        };
        Input.prototype.setWidth = function() {
            var element = this.element;
            element.style.minWidth = "".concat(element.placeholder.length + 1, "ch");
            element.style.width = "".concat(element.value.length + 1, "ch");
        };
        Input.prototype.setActiveDescendant = function(activeDescendantID) {
            this.element.setAttribute("aria-activedescendant", activeDescendantID);
        };
        Input.prototype.removeActiveDescendant = function() {
            this.element.removeAttribute("aria-activedescendant");
        };
        Input.prototype._onInput = function() {
            if (this.type !== PassedElementTypes.SelectOne) {
                this.setWidth();
            }
        };
        Input.prototype._onPaste = function(event) {
            if (this.preventPaste) {
                event.preventDefault();
            }
        };
        Input.prototype._onFocus = function() {
            this.isFocussed = true;
        };
        Input.prototype._onBlur = function() {
            this.isFocussed = false;
        };
        return Input;
    }();
    var SCROLLING_SPEED = 4;
    var List = function() {
        function List(_a) {
            var element = _a.element;
            this.element = element;
            this.scrollPos = this.element.scrollTop;
            this.height = this.element.offsetHeight;
        }
        List.prototype.prepend = function(node) {
            var child = this.element.firstElementChild;
            if (child) {
                this.element.insertBefore(node, child);
            } else {
                this.element.append(node);
            }
        };
        List.prototype.scrollToTop = function() {
            this.element.scrollTop = 0;
        };
        List.prototype.scrollToChildElement = function(element, direction) {
            var _this = this;
            if (!element) {
                return;
            }
            var listHeight = this.element.offsetHeight;
            var listScrollPosition = this.element.scrollTop + listHeight;
            var elementHeight = element.offsetHeight;
            var elementPos = element.offsetTop + elementHeight;
            var destination = direction > 0 ? this.element.scrollTop + elementPos - listScrollPosition : element.offsetTop;
            requestAnimationFrame(function() {
                _this._animateScroll(destination, direction);
            });
        };
        List.prototype._scrollDown = function(scrollPos, strength, destination) {
            var easing = (destination - scrollPos) / strength;
            var distance = easing > 1 ? easing : 1;
            this.element.scrollTop = scrollPos + distance;
        };
        List.prototype._scrollUp = function(scrollPos, strength, destination) {
            var easing = (scrollPos - destination) / strength;
            var distance = easing > 1 ? easing : 1;
            this.element.scrollTop = scrollPos - distance;
        };
        List.prototype._animateScroll = function(destination, direction) {
            var _this = this;
            var strength = SCROLLING_SPEED;
            var choiceListScrollTop = this.element.scrollTop;
            var continueAnimation = false;
            if (direction > 0) {
                this._scrollDown(choiceListScrollTop, strength, destination);
                if (choiceListScrollTop < destination) {
                    continueAnimation = true;
                }
            } else {
                this._scrollUp(choiceListScrollTop, strength, destination);
                if (choiceListScrollTop > destination) {
                    continueAnimation = true;
                }
            }
            if (continueAnimation) {
                requestAnimationFrame(function() {
                    _this._animateScroll(destination, direction);
                });
            }
        };
        return List;
    }();
    var WrappedElement = function() {
        function WrappedElement(_a) {
            var element = _a.element, classNames = _a.classNames;
            this.element = element;
            this.classNames = classNames;
            this.isDisabled = false;
        }
        Object.defineProperty(WrappedElement.prototype, "isActive", {
            get: function() {
                return this.element.dataset.choice === "active";
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(WrappedElement.prototype, "dir", {
            get: function() {
                return this.element.dir;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(WrappedElement.prototype, "value", {
            get: function() {
                return this.element.value;
            },
            set: function(value) {
                this.element.setAttribute("value", value);
                this.element.value = value;
            },
            enumerable: false,
            configurable: true
        });
        WrappedElement.prototype.conceal = function() {
            var el = this.element;
            addClassesToElement(el, this.classNames.input);
            el.hidden = true;
            el.tabIndex = -1;
            var origStyle = el.getAttribute("style");
            if (origStyle) {
                el.setAttribute("data-choice-orig-style", origStyle);
            }
            el.setAttribute("data-choice", "active");
        };
        WrappedElement.prototype.reveal = function() {
            var el = this.element;
            removeClassesFromElement(el, this.classNames.input);
            el.hidden = false;
            el.removeAttribute("tabindex");
            var origStyle = el.getAttribute("data-choice-orig-style");
            if (origStyle) {
                el.removeAttribute("data-choice-orig-style");
                el.setAttribute("style", origStyle);
            } else {
                el.removeAttribute("style");
            }
            el.removeAttribute("data-choice");
        };
        WrappedElement.prototype.enable = function() {
            this.element.removeAttribute("disabled");
            this.element.disabled = false;
            this.isDisabled = false;
        };
        WrappedElement.prototype.disable = function() {
            this.element.setAttribute("disabled", "");
            this.element.disabled = true;
            this.isDisabled = true;
        };
        WrappedElement.prototype.triggerEvent = function(eventType, data) {
            dispatchEvent(this.element, eventType, data || {});
        };
        return WrappedElement;
    }();
    var WrappedInput = function(_super) {
        __extends(WrappedInput, _super);
        function WrappedInput() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        return WrappedInput;
    }(WrappedElement);
    var coerceBool = function(arg, defaultValue) {
        if (defaultValue === void 0) {
            defaultValue = true;
        }
        return typeof arg === "undefined" ? defaultValue : !!arg;
    };
    var stringToHtmlClass = function(input) {
        if (typeof input === "string") {
            input = input.split(" ").filter(function(s) {
                return s.length;
            });
        }
        if (Array.isArray(input) && input.length) {
            return input;
        }
        return undefined;
    };
    var mapInputToChoice = function(value, allowGroup, allowRawString) {
        if (allowRawString === void 0) {
            allowRawString = true;
        }
        if (typeof value === "string") {
            var sanitisedValue = sanitise(value);
            var userValue = allowRawString || sanitisedValue === value ? value : {
                escaped: sanitisedValue,
                raw: value
            };
            var result_1 = mapInputToChoice({
                value: value,
                label: userValue,
                selected: true
            }, false);
            return result_1;
        }
        var groupOrChoice = value;
        if ("choices" in groupOrChoice) {
            if (!allowGroup) {
                throw new TypeError("optGroup is not allowed");
            }
            var group = groupOrChoice;
            var choices = group.choices.map(function(e) {
                return mapInputToChoice(e, false);
            });
            var result_2 = {
                id: 0,
                value: group.value,
                label: unwrapStringForRaw(group.label) || group.value,
                active: !!choices.length,
                disabled: !!group.disabled,
                choices: choices
            };
            return result_2;
        }
        var choice = groupOrChoice;
        var result = {
            id: 0,
            group: null,
            score: 0,
            rank: 0,
            value: choice.value,
            label: choice.label || choice.value,
            active: coerceBool(choice.active),
            selected: coerceBool(choice.selected, false),
            disabled: coerceBool(choice.disabled, false),
            placeholder: coerceBool(choice.placeholder, false),
            highlighted: false,
            labelClass: stringToHtmlClass(choice.labelClass),
            labelDescription: choice.labelDescription,
            customProperties: choice.customProperties
        };
        return result;
    };
    var isHtmlInputElement = function(e) {
        return e.tagName === "INPUT";
    };
    var isHtmlSelectElement = function(e) {
        return e.tagName === "SELECT";
    };
    var isHtmlOption = function(e) {
        return e.tagName === "OPTION";
    };
    var isHtmlOptgroup = function(e) {
        return e.tagName === "OPTGROUP";
    };
    var WrappedSelect = function(_super) {
        __extends(WrappedSelect, _super);
        function WrappedSelect(_a) {
            var element = _a.element, classNames = _a.classNames, template = _a.template, extractPlaceholder = _a.extractPlaceholder;
            var _this = _super.call(this, {
                element: element,
                classNames: classNames
            }) || this;
            _this.template = template;
            _this.extractPlaceholder = extractPlaceholder;
            return _this;
        }
        Object.defineProperty(WrappedSelect.prototype, "placeholderOption", {
            get: function() {
                return this.element.querySelector('option[value=""]') || this.element.querySelector("option[placeholder]");
            },
            enumerable: false,
            configurable: true
        });
        WrappedSelect.prototype.addOptions = function(choices) {
            var _this = this;
            var fragment = document.createDocumentFragment();
            choices.forEach(function(obj) {
                var choice = obj;
                if (choice.element) {
                    return;
                }
                var option = _this.template(choice);
                fragment.appendChild(option);
                choice.element = option;
            });
            this.element.appendChild(fragment);
        };
        WrappedSelect.prototype.optionsAsChoices = function() {
            var _this = this;
            var choices = [];
            this.element.querySelectorAll(":scope > option, :scope > optgroup").forEach(function(e) {
                if (isHtmlOption(e)) {
                    choices.push(_this._optionToChoice(e));
                } else if (isHtmlOptgroup(e)) {
                    choices.push(_this._optgroupToChoice(e));
                }
            });
            return choices;
        };
        WrappedSelect.prototype._optionToChoice = function(option) {
            if (!option.hasAttribute("value") && option.hasAttribute("placeholder")) {
                option.setAttribute("value", "");
                option.value = "";
            }
            return {
                id: 0,
                group: null,
                score: 0,
                rank: 0,
                value: option.value,
                label: option.label,
                element: option,
                active: true,
                selected: this.extractPlaceholder ? option.selected : option.hasAttribute("selected"),
                disabled: option.disabled,
                highlighted: false,
                placeholder: this.extractPlaceholder && (!option.value || option.hasAttribute("placeholder")),
                labelClass: typeof option.dataset.labelClass !== "undefined" ? stringToHtmlClass(option.dataset.labelClass) : undefined,
                labelDescription: typeof option.dataset.labelDescription !== "undefined" ? option.dataset.labelDescription : undefined,
                customProperties: parseCustomProperties(option.dataset.customProperties)
            };
        };
        WrappedSelect.prototype._optgroupToChoice = function(optgroup) {
            var _this = this;
            var options = optgroup.querySelectorAll("option");
            var choices = Array.from(options).map(function(option) {
                return _this._optionToChoice(option);
            });
            return {
                id: 0,
                label: optgroup.label || "",
                value: optgroup.getAttribute("value") || "",
                element: optgroup,
                active: !!choices.length,
                disabled: optgroup.disabled,
                choices: choices
            };
        };
        return WrappedSelect;
    }(WrappedElement);
    var DEFAULT_CLASSNAMES = {
        containerOuter: [ "choices" ],
        containerInner: [ "choices__inner" ],
        input: [ "choices__input" ],
        inputCloned: [ "choices__input--cloned", "form-control" ],
        list: [ "choices__list" ],
        listItems: [ "choices__list--multiple" ],
        listSingle: [ "choices__list--single" ],
        listDropdown: [ "choices__list--dropdown" ],
        item: [ "choices__item" ],
        itemSelectable: [ "choices__item--selectable" ],
        itemDisabled: [ "choices__item--disabled" ],
        itemChoice: [ "choices__item--choice" ],
        description: [ "choices__description" ],
        placeholder: [ "choices__placeholder" ],
        group: [ "choices__group" ],
        groupHeading: [ "choices__heading" ],
        button: [ "choices__button btn-close" ],
        activeState: [ "is-active" ],
        focusState: [ "is-focused" ],
        openState: [ "is-open" ],
        disabledState: [ "is-disabled" ],
        highlightedState: [ "is-highlighted" ],
        selectedState: [ "is-selected" ],
        flippedState: [ "is-flipped" ],
        loadingState: [ "is-loading" ],
        notice: [ "choices__notice" ],
        addChoice: [ "choices__item--selectable", "add-choice" ],
        noResults: [ "has-no-results" ],
        noChoices: [ "has-no-choices" ]
    };
    var DEFAULT_CONFIG = {
        items: [],
        choices: [],
        silent: false,
        renderChoiceLimit: -1,
        maxItemCount: -1,
        closeDropdownOnSelect: "auto",
        singleModeForMultiSelect: false,
        addChoices: false,
        addItems: true,
        addItemFilter: function(value) {
            return !!value && value !== "";
        },
        removeItems: true,
        removeItemButton: false,
        removeItemButtonAlignLeft: false,
        editItems: false,
        allowHTML: false,
        allowHtmlUserInput: false,
        duplicateItemsAllowed: true,
        delimiter: ",",
        paste: true,
        searchEnabled: true,
        searchChoices: true,
        searchFloor: 1,
        searchResultLimit: 4,
        searchFields: [ "label", "value" ],
        position: "auto",
        resetScrollPosition: true,
        shouldSort: true,
        shouldSortItems: false,
        sorter: sortByAlpha,
        shadowRoot: null,
        placeholder: true,
        placeholderValue: null,
        searchPlaceholderValue: null,
        prependValue: null,
        appendValue: null,
        renderSelectedChoices: "auto",
        loadingText: "Loading...",
        noResultsText: "No results found",
        noChoicesText: "No choices to choose from",
        itemSelectText: "Press to select",
        uniqueItemText: "Only unique values can be added",
        customAddItemText: "Only values matching specific conditions can be added",
        addItemText: function(value) {
            return 'Press Enter to add <b>"'.concat(value, '"</b>');
        },
        removeItemIconText: function() {
            return "Remove item";
        },
        removeItemLabelText: function(value) {
            return "Remove item: ".concat(value);
        },
        maxItemText: function(maxItemCount) {
            return "Only ".concat(maxItemCount, " values can be added");
        },
        valueComparer: function(value1, value2) {
            return value1 === value2;
        },
        fuseOptions: {
            includeScore: true
        },
        labelId: "",
        callbackOnInit: null,
        callbackOnCreateTemplates: null,
        classNames: DEFAULT_CLASSNAMES,
        appendGroupInSearch: false
    };
    var removeItem = function(item) {
        var itemEl = item.itemEl;
        if (itemEl) {
            itemEl.remove();
            item.itemEl = undefined;
        }
    };
    function items(s, action, context) {
        var state = s;
        var update = true;
        switch (action.type) {
          case ActionType.ADD_ITEM:
            {
                action.item.selected = true;
                var el = action.item.element;
                if (el) {
                    el.selected = true;
                    el.setAttribute("selected", "");
                }
                state.push(action.item);
                break;
            }

          case ActionType.REMOVE_ITEM:
            {
                action.item.selected = false;
                var el = action.item.element;
                if (el) {
                    el.selected = false;
                    el.removeAttribute("selected");
                    var select = el.parentElement;
                    if (select && isHtmlSelectElement(select) && select.type === PassedElementTypes.SelectOne) {
                        select.value = "";
                    }
                }
                removeItem(action.item);
                state = state.filter(function(choice) {
                    return choice.id !== action.item.id;
                });
                break;
            }

          case ActionType.REMOVE_CHOICE:
            {
                removeItem(action.choice);
                state = state.filter(function(item) {
                    return item.id !== action.choice.id;
                });
                break;
            }

          case ActionType.HIGHLIGHT_ITEM:
            {
                var highlighted = action.highlighted;
                var item = state.find(function(obj) {
                    return obj.id === action.item.id;
                });
                if (item && item.highlighted !== highlighted) {
                    item.highlighted = highlighted;
                    if (context) {
                        updateClassList(item, highlighted ? context.classNames.highlightedState : context.classNames.selectedState, highlighted ? context.classNames.selectedState : context.classNames.highlightedState);
                    }
                }
                break;
            }

          default:
            {
                update = false;
                break;
            }
        }
        return {
            state: state,
            update: update
        };
    }
    function groups(s, action) {
        var state = s;
        var update = true;
        switch (action.type) {
          case ActionType.ADD_GROUP:
            {
                state.push(action.group);
                break;
            }

          case ActionType.CLEAR_CHOICES:
            {
                state = [];
                break;
            }

          default:
            {
                update = false;
                break;
            }
        }
        return {
            state: state,
            update: update
        };
    }
    function choices(s, action, context) {
        var state = s;
        var update = true;
        switch (action.type) {
          case ActionType.ADD_CHOICE:
            {
                state.push(action.choice);
                break;
            }

          case ActionType.REMOVE_CHOICE:
            {
                action.choice.choiceEl = undefined;
                if (action.choice.group) {
                    action.choice.group.choices = action.choice.group.choices.filter(function(obj) {
                        return obj.id !== action.choice.id;
                    });
                }
                state = state.filter(function(obj) {
                    return obj.id !== action.choice.id;
                });
                break;
            }

          case ActionType.ADD_ITEM:
          case ActionType.REMOVE_ITEM:
            {
                action.item.choiceEl = undefined;
                break;
            }

          case ActionType.FILTER_CHOICES:
            {
                var scoreLookup_1 = [];
                action.results.forEach(function(result) {
                    scoreLookup_1[result.item.id] = result;
                });
                state.forEach(function(choice) {
                    var result = scoreLookup_1[choice.id];
                    if (result !== undefined) {
                        choice.score = result.score;
                        choice.rank = result.rank;
                        choice.active = true;
                    } else {
                        choice.score = 0;
                        choice.rank = 0;
                        choice.active = false;
                    }
                    if (context && context.appendGroupInSearch) {
                        choice.choiceEl = undefined;
                    }
                });
                break;
            }

          case ActionType.ACTIVATE_CHOICES:
            {
                state.forEach(function(choice) {
                    choice.active = action.active;
                    if (context && context.appendGroupInSearch) {
                        choice.choiceEl = undefined;
                    }
                });
                break;
            }

          case ActionType.CLEAR_CHOICES:
            {
                state = [];
                break;
            }

          default:
            {
                update = false;
                break;
            }
        }
        return {
            state: state,
            update: update
        };
    }
    var reducers = {
        groups: groups,
        items: items,
        choices: choices
    };
    var Store = function() {
        function Store(context) {
            this._state = this.defaultState;
            this._listeners = [];
            this._txn = 0;
            this._context = context;
        }
        Object.defineProperty(Store.prototype, "defaultState", {
            get: function() {
                return {
                    groups: [],
                    items: [],
                    choices: []
                };
            },
            enumerable: false,
            configurable: true
        });
        Store.prototype.changeSet = function(init) {
            return {
                groups: init,
                items: init,
                choices: init
            };
        };
        Store.prototype.reset = function() {
            this._state = this.defaultState;
            var changes = this.changeSet(true);
            if (this._txn) {
                this._changeSet = changes;
            } else {
                this._listeners.forEach(function(l) {
                    return l(changes);
                });
            }
        };
        Store.prototype.subscribe = function(onChange) {
            this._listeners.push(onChange);
            return this;
        };
        Store.prototype.dispatch = function(action) {
            var _this = this;
            var state = this._state;
            var hasChanges = false;
            var changes = this._changeSet || this.changeSet(false);
            Object.keys(reducers).forEach(function(key) {
                var stateUpdate = reducers[key](state[key], action, _this._context);
                if (stateUpdate.update) {
                    hasChanges = true;
                    changes[key] = true;
                    state[key] = stateUpdate.state;
                }
            });
            if (hasChanges) {
                if (this._txn) {
                    this._changeSet = changes;
                } else {
                    this._listeners.forEach(function(l) {
                        return l(changes);
                    });
                }
            }
        };
        Store.prototype.withTxn = function(func) {
            this._txn++;
            try {
                func();
            } finally {
                this._txn = Math.max(0, this._txn - 1);
                if (!this._txn) {
                    var changeSet_1 = this._changeSet;
                    if (changeSet_1) {
                        this._changeSet = undefined;
                        this._listeners.forEach(function(l) {
                            return l(changeSet_1);
                        });
                    }
                }
            }
        };
        Object.defineProperty(Store.prototype, "state", {
            get: function() {
                return this._state;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(Store.prototype, "items", {
            get: function() {
                return this.state.items;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(Store.prototype, "highlightedActiveItems", {
            get: function() {
                return this.items.filter(function(item) {
                    return item.active && item.highlighted;
                });
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(Store.prototype, "choices", {
            get: function() {
                return this.state.choices;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(Store.prototype, "activeChoices", {
            get: function() {
                return this.choices.filter(function(choice) {
                    return choice.active;
                });
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(Store.prototype, "searchableChoices", {
            get: function() {
                return this.choices.filter(function(choice) {
                    return !choice.disabled && !choice.placeholder;
                });
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(Store.prototype, "groups", {
            get: function() {
                return this.state.groups;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(Store.prototype, "activeGroups", {
            get: function() {
                var _this = this;
                return this.state.groups.filter(function(group) {
                    var isActive = group.active && !group.disabled;
                    var hasActiveOptions = _this.state.choices.some(function(choice) {
                        return choice.active && !choice.disabled;
                    });
                    return isActive && hasActiveOptions;
                }, []);
            },
            enumerable: false,
            configurable: true
        });
        Store.prototype.inTxn = function() {
            return this._txn > 0;
        };
        Store.prototype.getChoiceById = function(id) {
            return this.activeChoices.find(function(choice) {
                return choice.id === id;
            });
        };
        Store.prototype.getGroupById = function(id) {
            return this.groups.find(function(group) {
                return group.id === id;
            });
        };
        return Store;
    }();
    var NoticeTypes = {
        noChoices: "no-choices",
        noResults: "no-results",
        addChoice: "add-choice",
        generic: ""
    };
    function _defineProperty(e, r, t) {
        return (r = _toPropertyKey(r)) in e ? Object.defineProperty(e, r, {
            value: t,
            enumerable: !0,
            configurable: !0,
            writable: !0
        }) : e[r] = t, e;
    }
    function ownKeys(e, r) {
        var t = Object.keys(e);
        if (Object.getOwnPropertySymbols) {
            var o = Object.getOwnPropertySymbols(e);
            r && (o = o.filter(function(r) {
                return Object.getOwnPropertyDescriptor(e, r).enumerable;
            })), t.push.apply(t, o);
        }
        return t;
    }
    function _objectSpread2(e) {
        for (var r = 1; r < arguments.length; r++) {
            var t = null != arguments[r] ? arguments[r] : {};
            r % 2 ? ownKeys(Object(t), !0).forEach(function(r) {
                _defineProperty(e, r, t[r]);
            }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(e, Object.getOwnPropertyDescriptors(t)) : ownKeys(Object(t)).forEach(function(r) {
                Object.defineProperty(e, r, Object.getOwnPropertyDescriptor(t, r));
            });
        }
        return e;
    }
    function _toPrimitive(t, r) {
        if ("object" != typeof t || !t) return t;
        var e = t[Symbol.toPrimitive];
        if (void 0 !== e) {
            var i = e.call(t, r || "default");
            if ("object" != typeof i) return i;
            throw new TypeError("@@toPrimitive must return a primitive value.");
        }
        return ("string" === r ? String : Number)(t);
    }
    function _toPropertyKey(t) {
        var i = _toPrimitive(t, "string");
        return "symbol" == typeof i ? i : i + "";
    }
    function isArray(value) {
        return !Array.isArray ? getTag(value) === "[object Array]" : Array.isArray(value);
    }
    const INFINITY = 1 / 0;
    function baseToString(value) {
        if (typeof value == "string") {
            return value;
        }
        let result = value + "";
        return result == "0" && 1 / value == -INFINITY ? "-0" : result;
    }
    function toString(value) {
        return value == null ? "" : baseToString(value);
    }
    function isString(value) {
        return typeof value === "string";
    }
    function isNumber(value) {
        return typeof value === "number";
    }
    function isBoolean(value) {
        return value === true || value === false || isObjectLike(value) && getTag(value) == "[object Boolean]";
    }
    function isObject(value) {
        return typeof value === "object";
    }
    function isObjectLike(value) {
        return isObject(value) && value !== null;
    }
    function isDefined(value) {
        return value !== undefined && value !== null;
    }
    function isBlank(value) {
        return !value.trim().length;
    }
    function getTag(value) {
        return value == null ? value === undefined ? "[object Undefined]" : "[object Null]" : Object.prototype.toString.call(value);
    }
    const EXTENDED_SEARCH_UNAVAILABLE = "Extended search is not available";
    const INCORRECT_INDEX_TYPE = "Incorrect 'index' type";
    const LOGICAL_SEARCH_INVALID_QUERY_FOR_KEY = key => `Invalid value for key ${key}`;
    const PATTERN_LENGTH_TOO_LARGE = max => `Pattern length exceeds max of ${max}.`;
    const MISSING_KEY_PROPERTY = name => `Missing ${name} property in key`;
    const INVALID_KEY_WEIGHT_VALUE = key => `Property 'weight' in key '${key}' must be a positive integer`;
    const hasOwn = Object.prototype.hasOwnProperty;
    class KeyStore {
        constructor(keys) {
            this._keys = [];
            this._keyMap = {};
            let totalWeight = 0;
            keys.forEach(key => {
                let obj = createKey(key);
                this._keys.push(obj);
                this._keyMap[obj.id] = obj;
                totalWeight += obj.weight;
            });
            this._keys.forEach(key => {
                key.weight /= totalWeight;
            });
        }
        get(keyId) {
            return this._keyMap[keyId];
        }
        keys() {
            return this._keys;
        }
        toJSON() {
            return JSON.stringify(this._keys);
        }
    }
    function createKey(key) {
        let path = null;
        let id = null;
        let src = null;
        let weight = 1;
        let getFn = null;
        if (isString(key) || isArray(key)) {
            src = key;
            path = createKeyPath(key);
            id = createKeyId(key);
        } else {
            if (!hasOwn.call(key, "name")) {
                throw new Error(MISSING_KEY_PROPERTY("name"));
            }
            const name = key.name;
            src = name;
            if (hasOwn.call(key, "weight")) {
                weight = key.weight;
                if (weight <= 0) {
                    throw new Error(INVALID_KEY_WEIGHT_VALUE(name));
                }
            }
            path = createKeyPath(name);
            id = createKeyId(name);
            getFn = key.getFn;
        }
        return {
            path: path,
            id: id,
            weight: weight,
            src: src,
            getFn: getFn
        };
    }
    function createKeyPath(key) {
        return isArray(key) ? key : key.split(".");
    }
    function createKeyId(key) {
        return isArray(key) ? key.join(".") : key;
    }
    function get(obj, path) {
        let list = [];
        let arr = false;
        const deepGet = (obj, path, index) => {
            if (!isDefined(obj)) {
                return;
            }
            if (!path[index]) {
                list.push(obj);
            } else {
                let key = path[index];
                const value = obj[key];
                if (!isDefined(value)) {
                    return;
                }
                if (index === path.length - 1 && (isString(value) || isNumber(value) || isBoolean(value))) {
                    list.push(toString(value));
                } else if (isArray(value)) {
                    arr = true;
                    for (let i = 0, len = value.length; i < len; i += 1) {
                        deepGet(value[i], path, index + 1);
                    }
                } else if (path.length) {
                    deepGet(value, path, index + 1);
                }
            }
        };
        deepGet(obj, isString(path) ? path.split(".") : path, 0);
        return arr ? list : list[0];
    }
    const MatchOptions = {
        includeMatches: false,
        findAllMatches: false,
        minMatchCharLength: 1
    };
    const BasicOptions = {
        isCaseSensitive: false,
        includeScore: false,
        keys: [],
        shouldSort: true,
        sortFn: (a, b) => a.score === b.score ? a.idx < b.idx ? -1 : 1 : a.score < b.score ? -1 : 1
    };
    const FuzzyOptions = {
        location: 0,
        threshold: .6,
        distance: 100
    };
    const AdvancedOptions = {
        useExtendedSearch: false,
        getFn: get,
        ignoreLocation: false,
        ignoreFieldNorm: false,
        fieldNormWeight: 1
    };
    var Config = _objectSpread2(_objectSpread2(_objectSpread2(_objectSpread2({}, BasicOptions), MatchOptions), FuzzyOptions), AdvancedOptions);
    const SPACE = /[^ ]+/g;
    function norm(weight = 1, mantissa = 3) {
        const cache = new Map();
        const m = Math.pow(10, mantissa);
        return {
            get(value) {
                const numTokens = value.match(SPACE).length;
                if (cache.has(numTokens)) {
                    return cache.get(numTokens);
                }
                const norm = 1 / Math.pow(numTokens, .5 * weight);
                const n = parseFloat(Math.round(norm * m) / m);
                cache.set(numTokens, n);
                return n;
            },
            clear() {
                cache.clear();
            }
        };
    }
    class FuseIndex {
        constructor({
            getFn = Config.getFn,
            fieldNormWeight = Config.fieldNormWeight
        } = {}) {
            this.norm = norm(fieldNormWeight, 3);
            this.getFn = getFn;
            this.isCreated = false;
            this.setIndexRecords();
        }
        setSources(docs = []) {
            this.docs = docs;
        }
        setIndexRecords(records = []) {
            this.records = records;
        }
        setKeys(keys = []) {
            this.keys = keys;
            this._keysMap = {};
            keys.forEach((key, idx) => {
                this._keysMap[key.id] = idx;
            });
        }
        create() {
            if (this.isCreated || !this.docs.length) {
                return;
            }
            this.isCreated = true;
            if (isString(this.docs[0])) {
                this.docs.forEach((doc, docIndex) => {
                    this._addString(doc, docIndex);
                });
            } else {
                this.docs.forEach((doc, docIndex) => {
                    this._addObject(doc, docIndex);
                });
            }
            this.norm.clear();
        }
        add(doc) {
            const idx = this.size();
            if (isString(doc)) {
                this._addString(doc, idx);
            } else {
                this._addObject(doc, idx);
            }
        }
        removeAt(idx) {
            this.records.splice(idx, 1);
            for (let i = idx, len = this.size(); i < len; i += 1) {
                this.records[i].i -= 1;
            }
        }
        getValueForItemAtKeyId(item, keyId) {
            return item[this._keysMap[keyId]];
        }
        size() {
            return this.records.length;
        }
        _addString(doc, docIndex) {
            if (!isDefined(doc) || isBlank(doc)) {
                return;
            }
            let record = {
                v: doc,
                i: docIndex,
                n: this.norm.get(doc)
            };
            this.records.push(record);
        }
        _addObject(doc, docIndex) {
            let record = {
                i: docIndex,
                $: {}
            };
            this.keys.forEach((key, keyIndex) => {
                let value = key.getFn ? key.getFn(doc) : this.getFn(doc, key.path);
                if (!isDefined(value)) {
                    return;
                }
                if (isArray(value)) {
                    let subRecords = [];
                    const stack = [ {
                        nestedArrIndex: -1,
                        value: value
                    } ];
                    while (stack.length) {
                        const {
                            nestedArrIndex,
                            value
                        } = stack.pop();
                        if (!isDefined(value)) {
                            continue;
                        }
                        if (isString(value) && !isBlank(value)) {
                            let subRecord = {
                                v: value,
                                i: nestedArrIndex,
                                n: this.norm.get(value)
                            };
                            subRecords.push(subRecord);
                        } else if (isArray(value)) {
                            value.forEach((item, k) => {
                                stack.push({
                                    nestedArrIndex: k,
                                    value: item
                                });
                            });
                        } else;
                    }
                    record.$[keyIndex] = subRecords;
                } else if (isString(value) && !isBlank(value)) {
                    let subRecord = {
                        v: value,
                        n: this.norm.get(value)
                    };
                    record.$[keyIndex] = subRecord;
                }
            });
            this.records.push(record);
        }
        toJSON() {
            return {
                keys: this.keys,
                records: this.records
            };
        }
    }
    function createIndex(keys, docs, {
        getFn = Config.getFn,
        fieldNormWeight = Config.fieldNormWeight
    } = {}) {
        const myIndex = new FuseIndex({
            getFn: getFn,
            fieldNormWeight: fieldNormWeight
        });
        myIndex.setKeys(keys.map(createKey));
        myIndex.setSources(docs);
        myIndex.create();
        return myIndex;
    }
    function parseIndex(data, {
        getFn = Config.getFn,
        fieldNormWeight = Config.fieldNormWeight
    } = {}) {
        const {
            keys,
            records
        } = data;
        const myIndex = new FuseIndex({
            getFn: getFn,
            fieldNormWeight: fieldNormWeight
        });
        myIndex.setKeys(keys);
        myIndex.setIndexRecords(records);
        return myIndex;
    }
    function computeScore$1(pattern, {
        errors = 0,
        currentLocation = 0,
        expectedLocation = 0,
        distance = Config.distance,
        ignoreLocation = Config.ignoreLocation
    } = {}) {
        const accuracy = errors / pattern.length;
        if (ignoreLocation) {
            return accuracy;
        }
        const proximity = Math.abs(expectedLocation - currentLocation);
        if (!distance) {
            return proximity ? 1 : accuracy;
        }
        return accuracy + proximity / distance;
    }
    function convertMaskToIndices(matchmask = [], minMatchCharLength = Config.minMatchCharLength) {
        let indices = [];
        let start = -1;
        let end = -1;
        let i = 0;
        for (let len = matchmask.length; i < len; i += 1) {
            let match = matchmask[i];
            if (match && start === -1) {
                start = i;
            } else if (!match && start !== -1) {
                end = i - 1;
                if (end - start + 1 >= minMatchCharLength) {
                    indices.push([ start, end ]);
                }
                start = -1;
            }
        }
        if (matchmask[i - 1] && i - start >= minMatchCharLength) {
            indices.push([ start, i - 1 ]);
        }
        return indices;
    }
    const MAX_BITS = 32;
    function search(text, pattern, patternAlphabet, {
        location = Config.location,
        distance = Config.distance,
        threshold = Config.threshold,
        findAllMatches = Config.findAllMatches,
        minMatchCharLength = Config.minMatchCharLength,
        includeMatches = Config.includeMatches,
        ignoreLocation = Config.ignoreLocation
    } = {}) {
        if (pattern.length > MAX_BITS) {
            throw new Error(PATTERN_LENGTH_TOO_LARGE(MAX_BITS));
        }
        const patternLen = pattern.length;
        const textLen = text.length;
        const expectedLocation = Math.max(0, Math.min(location, textLen));
        let currentThreshold = threshold;
        let bestLocation = expectedLocation;
        const computeMatches = minMatchCharLength > 1 || includeMatches;
        const matchMask = computeMatches ? Array(textLen) : [];
        let index;
        while ((index = text.indexOf(pattern, bestLocation)) > -1) {
            let score = computeScore$1(pattern, {
                currentLocation: index,
                expectedLocation: expectedLocation,
                distance: distance,
                ignoreLocation: ignoreLocation
            });
            currentThreshold = Math.min(score, currentThreshold);
            bestLocation = index + patternLen;
            if (computeMatches) {
                let i = 0;
                while (i < patternLen) {
                    matchMask[index + i] = 1;
                    i += 1;
                }
            }
        }
        bestLocation = -1;
        let lastBitArr = [];
        let finalScore = 1;
        let binMax = patternLen + textLen;
        const mask = 1 << patternLen - 1;
        for (let i = 0; i < patternLen; i += 1) {
            let binMin = 0;
            let binMid = binMax;
            while (binMin < binMid) {
                const score = computeScore$1(pattern, {
                    errors: i,
                    currentLocation: expectedLocation + binMid,
                    expectedLocation: expectedLocation,
                    distance: distance,
                    ignoreLocation: ignoreLocation
                });
                if (score <= currentThreshold) {
                    binMin = binMid;
                } else {
                    binMax = binMid;
                }
                binMid = Math.floor((binMax - binMin) / 2 + binMin);
            }
            binMax = binMid;
            let start = Math.max(1, expectedLocation - binMid + 1);
            let finish = findAllMatches ? textLen : Math.min(expectedLocation + binMid, textLen) + patternLen;
            let bitArr = Array(finish + 2);
            bitArr[finish + 1] = (1 << i) - 1;
            for (let j = finish; j >= start; j -= 1) {
                let currentLocation = j - 1;
                let charMatch = patternAlphabet[text.charAt(currentLocation)];
                if (computeMatches) {
                    matchMask[currentLocation] = +!!charMatch;
                }
                bitArr[j] = (bitArr[j + 1] << 1 | 1) & charMatch;
                if (i) {
                    bitArr[j] |= (lastBitArr[j + 1] | lastBitArr[j]) << 1 | 1 | lastBitArr[j + 1];
                }
                if (bitArr[j] & mask) {
                    finalScore = computeScore$1(pattern, {
                        errors: i,
                        currentLocation: currentLocation,
                        expectedLocation: expectedLocation,
                        distance: distance,
                        ignoreLocation: ignoreLocation
                    });
                    if (finalScore <= currentThreshold) {
                        currentThreshold = finalScore;
                        bestLocation = currentLocation;
                        if (bestLocation <= expectedLocation) {
                            break;
                        }
                        start = Math.max(1, 2 * expectedLocation - bestLocation);
                    }
                }
            }
            const score = computeScore$1(pattern, {
                errors: i + 1,
                currentLocation: expectedLocation,
                expectedLocation: expectedLocation,
                distance: distance,
                ignoreLocation: ignoreLocation
            });
            if (score > currentThreshold) {
                break;
            }
            lastBitArr = bitArr;
        }
        const result = {
            isMatch: bestLocation >= 0,
            score: Math.max(.001, finalScore)
        };
        if (computeMatches) {
            const indices = convertMaskToIndices(matchMask, minMatchCharLength);
            if (!indices.length) {
                result.isMatch = false;
            } else if (includeMatches) {
                result.indices = indices;
            }
        }
        return result;
    }
    function createPatternAlphabet(pattern) {
        let mask = {};
        for (let i = 0, len = pattern.length; i < len; i += 1) {
            const char = pattern.charAt(i);
            mask[char] = (mask[char] || 0) | 1 << len - i - 1;
        }
        return mask;
    }
    class BitapSearch {
        constructor(pattern, {
            location = Config.location,
            threshold = Config.threshold,
            distance = Config.distance,
            includeMatches = Config.includeMatches,
            findAllMatches = Config.findAllMatches,
            minMatchCharLength = Config.minMatchCharLength,
            isCaseSensitive = Config.isCaseSensitive,
            ignoreLocation = Config.ignoreLocation
        } = {}) {
            this.options = {
                location: location,
                threshold: threshold,
                distance: distance,
                includeMatches: includeMatches,
                findAllMatches: findAllMatches,
                minMatchCharLength: minMatchCharLength,
                isCaseSensitive: isCaseSensitive,
                ignoreLocation: ignoreLocation
            };
            this.pattern = isCaseSensitive ? pattern : pattern.toLowerCase();
            this.chunks = [];
            if (!this.pattern.length) {
                return;
            }
            const addChunk = (pattern, startIndex) => {
                this.chunks.push({
                    pattern: pattern,
                    alphabet: createPatternAlphabet(pattern),
                    startIndex: startIndex
                });
            };
            const len = this.pattern.length;
            if (len > MAX_BITS) {
                let i = 0;
                const remainder = len % MAX_BITS;
                const end = len - remainder;
                while (i < end) {
                    addChunk(this.pattern.substr(i, MAX_BITS), i);
                    i += MAX_BITS;
                }
                if (remainder) {
                    const startIndex = len - MAX_BITS;
                    addChunk(this.pattern.substr(startIndex), startIndex);
                }
            } else {
                addChunk(this.pattern, 0);
            }
        }
        searchIn(text) {
            const {
                isCaseSensitive,
                includeMatches
            } = this.options;
            if (!isCaseSensitive) {
                text = text.toLowerCase();
            }
            if (this.pattern === text) {
                let result = {
                    isMatch: true,
                    score: 0
                };
                if (includeMatches) {
                    result.indices = [ [ 0, text.length - 1 ] ];
                }
                return result;
            }
            const {
                location,
                distance,
                threshold,
                findAllMatches,
                minMatchCharLength,
                ignoreLocation
            } = this.options;
            let allIndices = [];
            let totalScore = 0;
            let hasMatches = false;
            this.chunks.forEach(({
                pattern,
                alphabet,
                startIndex
            }) => {
                const {
                    isMatch,
                    score,
                    indices
                } = search(text, pattern, alphabet, {
                    location: location + startIndex,
                    distance: distance,
                    threshold: threshold,
                    findAllMatches: findAllMatches,
                    minMatchCharLength: minMatchCharLength,
                    includeMatches: includeMatches,
                    ignoreLocation: ignoreLocation
                });
                if (isMatch) {
                    hasMatches = true;
                }
                totalScore += score;
                if (isMatch && indices) {
                    allIndices = [ ...allIndices, ...indices ];
                }
            });
            let result = {
                isMatch: hasMatches,
                score: hasMatches ? totalScore / this.chunks.length : 1
            };
            if (hasMatches && includeMatches) {
                result.indices = allIndices;
            }
            return result;
        }
    }
    class BaseMatch {
        constructor(pattern) {
            this.pattern = pattern;
        }
        static isMultiMatch(pattern) {
            return getMatch(pattern, this.multiRegex);
        }
        static isSingleMatch(pattern) {
            return getMatch(pattern, this.singleRegex);
        }
        search() {}
    }
    function getMatch(pattern, exp) {
        const matches = pattern.match(exp);
        return matches ? matches[1] : null;
    }
    class ExactMatch extends BaseMatch {
        constructor(pattern) {
            super(pattern);
        }
        static get type() {
            return "exact";
        }
        static get multiRegex() {
            return /^="(.*)"$/;
        }
        static get singleRegex() {
            return /^=(.*)$/;
        }
        search(text) {
            const isMatch = text === this.pattern;
            return {
                isMatch: isMatch,
                score: isMatch ? 0 : 1,
                indices: [ 0, this.pattern.length - 1 ]
            };
        }
    }
    class InverseExactMatch extends BaseMatch {
        constructor(pattern) {
            super(pattern);
        }
        static get type() {
            return "inverse-exact";
        }
        static get multiRegex() {
            return /^!"(.*)"$/;
        }
        static get singleRegex() {
            return /^!(.*)$/;
        }
        search(text) {
            const index = text.indexOf(this.pattern);
            const isMatch = index === -1;
            return {
                isMatch: isMatch,
                score: isMatch ? 0 : 1,
                indices: [ 0, text.length - 1 ]
            };
        }
    }
    class PrefixExactMatch extends BaseMatch {
        constructor(pattern) {
            super(pattern);
        }
        static get type() {
            return "prefix-exact";
        }
        static get multiRegex() {
            return /^\^"(.*)"$/;
        }
        static get singleRegex() {
            return /^\^(.*)$/;
        }
        search(text) {
            const isMatch = text.startsWith(this.pattern);
            return {
                isMatch: isMatch,
                score: isMatch ? 0 : 1,
                indices: [ 0, this.pattern.length - 1 ]
            };
        }
    }
    class InversePrefixExactMatch extends BaseMatch {
        constructor(pattern) {
            super(pattern);
        }
        static get type() {
            return "inverse-prefix-exact";
        }
        static get multiRegex() {
            return /^!\^"(.*)"$/;
        }
        static get singleRegex() {
            return /^!\^(.*)$/;
        }
        search(text) {
            const isMatch = !text.startsWith(this.pattern);
            return {
                isMatch: isMatch,
                score: isMatch ? 0 : 1,
                indices: [ 0, text.length - 1 ]
            };
        }
    }
    class SuffixExactMatch extends BaseMatch {
        constructor(pattern) {
            super(pattern);
        }
        static get type() {
            return "suffix-exact";
        }
        static get multiRegex() {
            return /^"(.*)"\$$/;
        }
        static get singleRegex() {
            return /^(.*)\$$/;
        }
        search(text) {
            const isMatch = text.endsWith(this.pattern);
            return {
                isMatch: isMatch,
                score: isMatch ? 0 : 1,
                indices: [ text.length - this.pattern.length, text.length - 1 ]
            };
        }
    }
    class InverseSuffixExactMatch extends BaseMatch {
        constructor(pattern) {
            super(pattern);
        }
        static get type() {
            return "inverse-suffix-exact";
        }
        static get multiRegex() {
            return /^!"(.*)"\$$/;
        }
        static get singleRegex() {
            return /^!(.*)\$$/;
        }
        search(text) {
            const isMatch = !text.endsWith(this.pattern);
            return {
                isMatch: isMatch,
                score: isMatch ? 0 : 1,
                indices: [ 0, text.length - 1 ]
            };
        }
    }
    class FuzzyMatch extends BaseMatch {
        constructor(pattern, {
            location = Config.location,
            threshold = Config.threshold,
            distance = Config.distance,
            includeMatches = Config.includeMatches,
            findAllMatches = Config.findAllMatches,
            minMatchCharLength = Config.minMatchCharLength,
            isCaseSensitive = Config.isCaseSensitive,
            ignoreLocation = Config.ignoreLocation
        } = {}) {
            super(pattern);
            this._bitapSearch = new BitapSearch(pattern, {
                location: location,
                threshold: threshold,
                distance: distance,
                includeMatches: includeMatches,
                findAllMatches: findAllMatches,
                minMatchCharLength: minMatchCharLength,
                isCaseSensitive: isCaseSensitive,
                ignoreLocation: ignoreLocation
            });
        }
        static get type() {
            return "fuzzy";
        }
        static get multiRegex() {
            return /^"(.*)"$/;
        }
        static get singleRegex() {
            return /^(.*)$/;
        }
        search(text) {
            return this._bitapSearch.searchIn(text);
        }
    }
    class IncludeMatch extends BaseMatch {
        constructor(pattern) {
            super(pattern);
        }
        static get type() {
            return "include";
        }
        static get multiRegex() {
            return /^'"(.*)"$/;
        }
        static get singleRegex() {
            return /^'(.*)$/;
        }
        search(text) {
            let location = 0;
            let index;
            const indices = [];
            const patternLen = this.pattern.length;
            while ((index = text.indexOf(this.pattern, location)) > -1) {
                location = index + patternLen;
                indices.push([ index, location - 1 ]);
            }
            const isMatch = !!indices.length;
            return {
                isMatch: isMatch,
                score: isMatch ? 0 : 1,
                indices: indices
            };
        }
    }
    const searchers = [ ExactMatch, IncludeMatch, PrefixExactMatch, InversePrefixExactMatch, InverseSuffixExactMatch, SuffixExactMatch, InverseExactMatch, FuzzyMatch ];
    const searchersLen = searchers.length;
    const SPACE_RE = / +(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)/;
    const OR_TOKEN = "|";
    function parseQuery(pattern, options = {}) {
        return pattern.split(OR_TOKEN).map(item => {
            let query = item.trim().split(SPACE_RE).filter(item => item && !!item.trim());
            let results = [];
            for (let i = 0, len = query.length; i < len; i += 1) {
                const queryItem = query[i];
                let found = false;
                let idx = -1;
                while (!found && ++idx < searchersLen) {
                    const searcher = searchers[idx];
                    let token = searcher.isMultiMatch(queryItem);
                    if (token) {
                        results.push(new searcher(token, options));
                        found = true;
                    }
                }
                if (found) {
                    continue;
                }
                idx = -1;
                while (++idx < searchersLen) {
                    const searcher = searchers[idx];
                    let token = searcher.isSingleMatch(queryItem);
                    if (token) {
                        results.push(new searcher(token, options));
                        break;
                    }
                }
            }
            return results;
        });
    }
    const MultiMatchSet = new Set([ FuzzyMatch.type, IncludeMatch.type ]);
    class ExtendedSearch {
        constructor(pattern, {
            isCaseSensitive = Config.isCaseSensitive,
            includeMatches = Config.includeMatches,
            minMatchCharLength = Config.minMatchCharLength,
            ignoreLocation = Config.ignoreLocation,
            findAllMatches = Config.findAllMatches,
            location = Config.location,
            threshold = Config.threshold,
            distance = Config.distance
        } = {}) {
            this.query = null;
            this.options = {
                isCaseSensitive: isCaseSensitive,
                includeMatches: includeMatches,
                minMatchCharLength: minMatchCharLength,
                findAllMatches: findAllMatches,
                ignoreLocation: ignoreLocation,
                location: location,
                threshold: threshold,
                distance: distance
            };
            this.pattern = isCaseSensitive ? pattern : pattern.toLowerCase();
            this.query = parseQuery(this.pattern, this.options);
        }
        static condition(_, options) {
            return options.useExtendedSearch;
        }
        searchIn(text) {
            const query = this.query;
            if (!query) {
                return {
                    isMatch: false,
                    score: 1
                };
            }
            const {
                includeMatches,
                isCaseSensitive
            } = this.options;
            text = isCaseSensitive ? text : text.toLowerCase();
            let numMatches = 0;
            let allIndices = [];
            let totalScore = 0;
            for (let i = 0, qLen = query.length; i < qLen; i += 1) {
                const searchers = query[i];
                allIndices.length = 0;
                numMatches = 0;
                for (let j = 0, pLen = searchers.length; j < pLen; j += 1) {
                    const searcher = searchers[j];
                    const {
                        isMatch,
                        indices,
                        score
                    } = searcher.search(text);
                    if (isMatch) {
                        numMatches += 1;
                        totalScore += score;
                        if (includeMatches) {
                            const type = searcher.constructor.type;
                            if (MultiMatchSet.has(type)) {
                                allIndices = [ ...allIndices, ...indices ];
                            } else {
                                allIndices.push(indices);
                            }
                        }
                    } else {
                        totalScore = 0;
                        numMatches = 0;
                        allIndices.length = 0;
                        break;
                    }
                }
                if (numMatches) {
                    let result = {
                        isMatch: true,
                        score: totalScore / numMatches
                    };
                    if (includeMatches) {
                        result.indices = allIndices;
                    }
                    return result;
                }
            }
            return {
                isMatch: false,
                score: 1
            };
        }
    }
    const registeredSearchers = [];
    function register(...args) {
        registeredSearchers.push(...args);
    }
    function createSearcher(pattern, options) {
        for (let i = 0, len = registeredSearchers.length; i < len; i += 1) {
            let searcherClass = registeredSearchers[i];
            if (searcherClass.condition(pattern, options)) {
                return new searcherClass(pattern, options);
            }
        }
        return new BitapSearch(pattern, options);
    }
    const LogicalOperator = {
        AND: "$and",
        OR: "$or"
    };
    const KeyType = {
        PATH: "$path",
        PATTERN: "$val"
    };
    const isExpression = query => !!(query[LogicalOperator.AND] || query[LogicalOperator.OR]);
    const isPath = query => !!query[KeyType.PATH];
    const isLeaf = query => !isArray(query) && isObject(query) && !isExpression(query);
    const convertToExplicit = query => ({
        [LogicalOperator.AND]: Object.keys(query).map(key => ({
            [key]: query[key]
        }))
    });
    function parse(query, options, {
        auto = true
    } = {}) {
        const next = query => {
            let keys = Object.keys(query);
            const isQueryPath = isPath(query);
            if (!isQueryPath && keys.length > 1 && !isExpression(query)) {
                return next(convertToExplicit(query));
            }
            if (isLeaf(query)) {
                const key = isQueryPath ? query[KeyType.PATH] : keys[0];
                const pattern = isQueryPath ? query[KeyType.PATTERN] : query[key];
                if (!isString(pattern)) {
                    throw new Error(LOGICAL_SEARCH_INVALID_QUERY_FOR_KEY(key));
                }
                const obj = {
                    keyId: createKeyId(key),
                    pattern: pattern
                };
                if (auto) {
                    obj.searcher = createSearcher(pattern, options);
                }
                return obj;
            }
            let node = {
                children: [],
                operator: keys[0]
            };
            keys.forEach(key => {
                const value = query[key];
                if (isArray(value)) {
                    value.forEach(item => {
                        node.children.push(next(item));
                    });
                }
            });
            return node;
        };
        if (!isExpression(query)) {
            query = convertToExplicit(query);
        }
        return next(query);
    }
    function computeScore(results, {
        ignoreFieldNorm = Config.ignoreFieldNorm
    }) {
        results.forEach(result => {
            let totalScore = 1;
            result.matches.forEach(({
                key,
                norm,
                score
            }) => {
                const weight = key ? key.weight : null;
                totalScore *= Math.pow(score === 0 && weight ? Number.EPSILON : score, (weight || 1) * (ignoreFieldNorm ? 1 : norm));
            });
            result.score = totalScore;
        });
    }
    function transformMatches(result, data) {
        const matches = result.matches;
        data.matches = [];
        if (!isDefined(matches)) {
            return;
        }
        matches.forEach(match => {
            if (!isDefined(match.indices) || !match.indices.length) {
                return;
            }
            const {
                indices,
                value
            } = match;
            let obj = {
                indices: indices,
                value: value
            };
            if (match.key) {
                obj.key = match.key.src;
            }
            if (match.idx > -1) {
                obj.refIndex = match.idx;
            }
            data.matches.push(obj);
        });
    }
    function transformScore(result, data) {
        data.score = result.score;
    }
    function format(results, docs, {
        includeMatches = Config.includeMatches,
        includeScore = Config.includeScore
    } = {}) {
        const transformers = [];
        if (includeMatches) transformers.push(transformMatches);
        if (includeScore) transformers.push(transformScore);
        return results.map(result => {
            const {
                idx
            } = result;
            const data = {
                item: docs[idx],
                refIndex: idx
            };
            if (transformers.length) {
                transformers.forEach(transformer => {
                    transformer(result, data);
                });
            }
            return data;
        });
    }
    class Fuse {
        constructor(docs, options = {}, index) {
            this.options = _objectSpread2(_objectSpread2({}, Config), options);
            if (this.options.useExtendedSearch && !true) {
                throw new Error(EXTENDED_SEARCH_UNAVAILABLE);
            }
            this._keyStore = new KeyStore(this.options.keys);
            this.setCollection(docs, index);
        }
        setCollection(docs, index) {
            this._docs = docs;
            if (index && !(index instanceof FuseIndex)) {
                throw new Error(INCORRECT_INDEX_TYPE);
            }
            this._myIndex = index || createIndex(this.options.keys, this._docs, {
                getFn: this.options.getFn,
                fieldNormWeight: this.options.fieldNormWeight
            });
        }
        add(doc) {
            if (!isDefined(doc)) {
                return;
            }
            this._docs.push(doc);
            this._myIndex.add(doc);
        }
        remove(predicate = () => false) {
            const results = [];
            for (let i = 0, len = this._docs.length; i < len; i += 1) {
                const doc = this._docs[i];
                if (predicate(doc, i)) {
                    this.removeAt(i);
                    i -= 1;
                    len -= 1;
                    results.push(doc);
                }
            }
            return results;
        }
        removeAt(idx) {
            this._docs.splice(idx, 1);
            this._myIndex.removeAt(idx);
        }
        getIndex() {
            return this._myIndex;
        }
        search(query, {
            limit = -1
        } = {}) {
            const {
                includeMatches,
                includeScore,
                shouldSort,
                sortFn,
                ignoreFieldNorm
            } = this.options;
            let results = isString(query) ? isString(this._docs[0]) ? this._searchStringList(query) : this._searchObjectList(query) : this._searchLogical(query);
            computeScore(results, {
                ignoreFieldNorm: ignoreFieldNorm
            });
            if (shouldSort) {
                results.sort(sortFn);
            }
            if (isNumber(limit) && limit > -1) {
                results = results.slice(0, limit);
            }
            return format(results, this._docs, {
                includeMatches: includeMatches,
                includeScore: includeScore
            });
        }
        _searchStringList(query) {
            const searcher = createSearcher(query, this.options);
            const {
                records
            } = this._myIndex;
            const results = [];
            records.forEach(({
                v: text,
                i: idx,
                n: norm
            }) => {
                if (!isDefined(text)) {
                    return;
                }
                const {
                    isMatch,
                    score,
                    indices
                } = searcher.searchIn(text);
                if (isMatch) {
                    results.push({
                        item: text,
                        idx: idx,
                        matches: [ {
                            score: score,
                            value: text,
                            norm: norm,
                            indices: indices
                        } ]
                    });
                }
            });
            return results;
        }
        _searchLogical(query) {
            const expression = parse(query, this.options);
            const evaluate = (node, item, idx) => {
                if (!node.children) {
                    const {
                        keyId,
                        searcher
                    } = node;
                    const matches = this._findMatches({
                        key: this._keyStore.get(keyId),
                        value: this._myIndex.getValueForItemAtKeyId(item, keyId),
                        searcher: searcher
                    });
                    if (matches && matches.length) {
                        return [ {
                            idx: idx,
                            item: item,
                            matches: matches
                        } ];
                    }
                    return [];
                }
                const res = [];
                for (let i = 0, len = node.children.length; i < len; i += 1) {
                    const child = node.children[i];
                    const result = evaluate(child, item, idx);
                    if (result.length) {
                        res.push(...result);
                    } else if (node.operator === LogicalOperator.AND) {
                        return [];
                    }
                }
                return res;
            };
            const records = this._myIndex.records;
            const resultMap = {};
            const results = [];
            records.forEach(({
                $: item,
                i: idx
            }) => {
                if (isDefined(item)) {
                    let expResults = evaluate(expression, item, idx);
                    if (expResults.length) {
                        if (!resultMap[idx]) {
                            resultMap[idx] = {
                                idx: idx,
                                item: item,
                                matches: []
                            };
                            results.push(resultMap[idx]);
                        }
                        expResults.forEach(({
                            matches
                        }) => {
                            resultMap[idx].matches.push(...matches);
                        });
                    }
                }
            });
            return results;
        }
        _searchObjectList(query) {
            const searcher = createSearcher(query, this.options);
            const {
                keys,
                records
            } = this._myIndex;
            const results = [];
            records.forEach(({
                $: item,
                i: idx
            }) => {
                if (!isDefined(item)) {
                    return;
                }
                let matches = [];
                keys.forEach((key, keyIndex) => {
                    matches.push(...this._findMatches({
                        key: key,
                        value: item[keyIndex],
                        searcher: searcher
                    }));
                });
                if (matches.length) {
                    results.push({
                        idx: idx,
                        item: item,
                        matches: matches
                    });
                }
            });
            return results;
        }
        _findMatches({
            key,
            value,
            searcher
        }) {
            if (!isDefined(value)) {
                return [];
            }
            let matches = [];
            if (isArray(value)) {
                value.forEach(({
                    v: text,
                    i: idx,
                    n: norm
                }) => {
                    if (!isDefined(text)) {
                        return;
                    }
                    const {
                        isMatch,
                        score,
                        indices
                    } = searcher.searchIn(text);
                    if (isMatch) {
                        matches.push({
                            score: score,
                            key: key,
                            value: text,
                            idx: idx,
                            norm: norm,
                            indices: indices
                        });
                    }
                });
            } else {
                const {
                    v: text,
                    n: norm
                } = value;
                const {
                    isMatch,
                    score,
                    indices
                } = searcher.searchIn(text);
                if (isMatch) {
                    matches.push({
                        score: score,
                        key: key,
                        value: text,
                        norm: norm,
                        indices: indices
                    });
                }
            }
            return matches;
        }
    }
    Fuse.version = "7.0.0";
    Fuse.createIndex = createIndex;
    Fuse.parseIndex = parseIndex;
    Fuse.config = Config;
    {
        Fuse.parseQuery = parse;
    }
    {
        register(ExtendedSearch);
    }
    var SearchByFuse = function() {
        function SearchByFuse(config) {
            this._haystack = [];
            this._fuseOptions = __assign(__assign({}, config.fuseOptions), {
                keys: __spreadArray([], config.searchFields, true),
                includeMatches: true
            });
        }
        SearchByFuse.prototype.index = function(data) {
            this._haystack = data;
            if (this._fuse) {
                this._fuse.setCollection(data);
            }
        };
        SearchByFuse.prototype.reset = function() {
            this._haystack = [];
            this._fuse = undefined;
        };
        SearchByFuse.prototype.isEmptyIndex = function() {
            return !this._haystack.length;
        };
        SearchByFuse.prototype.search = function(needle) {
            if (!this._fuse) {
                {
                    this._fuse = new Fuse(this._haystack, this._fuseOptions);
                }
            }
            var results = this._fuse.search(needle);
            return results.map(function(value, i) {
                return {
                    item: value.item,
                    score: value.score || 0,
                    rank: i + 1
                };
            });
        };
        return SearchByFuse;
    }();
    function getSearcher(config) {
        {
            return new SearchByFuse(config);
        }
    }
    var isEmptyObject = function(obj) {
        for (var prop in obj) {
            if (Object.prototype.hasOwnProperty.call(obj, prop)) {
                return false;
            }
        }
        return true;
    };
    var assignCustomProperties = function(el, choice, withCustomProperties) {
        var dataset = el.dataset;
        var customProperties = choice.customProperties, labelClass = choice.labelClass, labelDescription = choice.labelDescription;
        if (labelClass) {
            dataset.labelClass = getClassNames(labelClass).join(" ");
        }
        if (labelDescription) {
            dataset.labelDescription = labelDescription;
        }
        if (withCustomProperties && customProperties) {
            if (typeof customProperties === "string") {
                dataset.customProperties = customProperties;
            } else if (typeof customProperties === "object" && !isEmptyObject(customProperties)) {
                dataset.customProperties = JSON.stringify(customProperties);
            }
        }
    };
    var addAriaLabel = function(docRoot, id, element) {
        var label = id && docRoot.querySelector("label[for='".concat(id, "']"));
        var text = label && label.innerText;
        if (text) {
            element.setAttribute("aria-label", text);
        }
    };
    var templates = {
        containerOuter: function(_a, dir, isSelectElement, isSelectOneElement, searchEnabled, passedElementType, labelId) {
            var containerOuter = _a.classNames.containerOuter;
            var div = document.createElement("div");
            addClassesToElement(div, containerOuter);
            div.dataset.type = passedElementType;
            if (dir) {
                div.dir = dir;
            }
            if (isSelectOneElement) {
                div.tabIndex = 0;
            }
            if (isSelectElement) {
                div.setAttribute("role", searchEnabled ? "combobox" : "listbox");
                if (searchEnabled) {
                    div.setAttribute("aria-autocomplete", "list");
                } else if (!labelId) {
                    addAriaLabel(this._docRoot, this.passedElement.element.id, div);
                }
                div.setAttribute("aria-haspopup", "true");
                div.setAttribute("aria-expanded", "false");
            }
            if (labelId) {
                div.setAttribute("aria-labelledby", labelId);
            }
            return div;
        },
        containerInner: function(_a) {
            var containerInner = _a.classNames.containerInner;
            var div = document.createElement("div");
            addClassesToElement(div, containerInner);
            return div;
        },
        itemList: function(_a, isSelectOneElement) {
            var searchEnabled = _a.searchEnabled, _b = _a.classNames, list = _b.list, listSingle = _b.listSingle, listItems = _b.listItems;
            var div = document.createElement("div");
            addClassesToElement(div, list);
            addClassesToElement(div, isSelectOneElement ? listSingle : listItems);
            if (this._isSelectElement && searchEnabled) {
                div.setAttribute("role", "listbox");
            }
            return div;
        },
        placeholder: function(_a, value) {
            var allowHTML = _a.allowHTML, placeholder = _a.classNames.placeholder;
            var div = document.createElement("div");
            addClassesToElement(div, placeholder);
            setElementHtml(div, allowHTML, value);
            return div;
        },
        item: function(_a, choice, removeItemButton) {
            var allowHTML = _a.allowHTML, removeItemButtonAlignLeft = _a.removeItemButtonAlignLeft, removeItemIconText = _a.removeItemIconText, removeItemLabelText = _a.removeItemLabelText, _b = _a.classNames, item = _b.item, button = _b.button, highlightedState = _b.highlightedState, itemSelectable = _b.itemSelectable, placeholder = _b.placeholder;
            var rawValue = unwrapStringForRaw(choice.value);
            var div = document.createElement("div");
            addClassesToElement(div, item);
            if (choice.labelClass) {
                var spanLabel = document.createElement("span");
                setElementHtml(spanLabel, allowHTML, choice.label);
                addClassesToElement(spanLabel, choice.labelClass);
                div.appendChild(spanLabel);
            } else {
                setElementHtml(div, allowHTML, choice.label);
            }
            div.dataset.item = "";
            div.dataset.id = choice.id;
            div.dataset.value = rawValue;
            assignCustomProperties(div, choice, true);
            if (choice.disabled || this.containerOuter.isDisabled) {
                div.setAttribute("aria-disabled", "true");
            }
            if (this._isSelectElement) {
                div.setAttribute("aria-selected", "true");
                div.setAttribute("role", "option");
            }
            if (choice.placeholder) {
                addClassesToElement(div, placeholder);
                div.dataset.placeholder = "";
            }
            addClassesToElement(div, choice.highlighted ? highlightedState : itemSelectable);
            if (removeItemButton) {
                if (choice.disabled) {
                    removeClassesFromElement(div, itemSelectable);
                }
                div.dataset.deletable = "";
                var removeButton = document.createElement("button");
                removeButton.type = "button";
                addClassesToElement(removeButton, button);
                setElementHtml(removeButton, true, resolveNoticeFunction(removeItemIconText, choice.value));
                var REMOVE_ITEM_LABEL = resolveNoticeFunction(removeItemLabelText, choice.value);
                if (REMOVE_ITEM_LABEL) {
                    removeButton.setAttribute("aria-label", REMOVE_ITEM_LABEL);
                }
                removeButton.dataset.button = "";
                if (removeItemButtonAlignLeft) {
                    div.insertAdjacentElement("afterbegin", removeButton);
                } else {
                    div.appendChild(removeButton);
                }
            }
            return div;
        },
        choiceList: function(_a, isSelectOneElement) {
            var list = _a.classNames.list;
            var div = document.createElement("div");
            addClassesToElement(div, list);
            if (!isSelectOneElement) {
                div.setAttribute("aria-multiselectable", "true");
            }
            div.setAttribute("role", "listbox");
            return div;
        },
        choiceGroup: function(_a, _b) {
            var allowHTML = _a.allowHTML, _c = _a.classNames, group = _c.group, groupHeading = _c.groupHeading, itemDisabled = _c.itemDisabled;
            var id = _b.id, label = _b.label, disabled = _b.disabled;
            var rawLabel = unwrapStringForRaw(label);
            var div = document.createElement("div");
            addClassesToElement(div, group);
            if (disabled) {
                addClassesToElement(div, itemDisabled);
            }
            div.setAttribute("role", "group");
            div.dataset.group = "";
            div.dataset.id = id;
            div.dataset.value = rawLabel;
            if (disabled) {
                div.setAttribute("aria-disabled", "true");
            }
            var heading = document.createElement("div");
            addClassesToElement(heading, groupHeading);
            setElementHtml(heading, allowHTML, label || "");
            div.appendChild(heading);
            return div;
        },
        choice: function(_a, choice, selectText, groupName) {
            var allowHTML = _a.allowHTML, _b = _a.classNames, item = _b.item, itemChoice = _b.itemChoice, itemSelectable = _b.itemSelectable, selectedState = _b.selectedState, itemDisabled = _b.itemDisabled, description = _b.description, placeholder = _b.placeholder;
            var label = choice.label;
            var rawValue = unwrapStringForRaw(choice.value);
            var div = document.createElement("div");
            div.id = choice.elementId;
            addClassesToElement(div, item);
            addClassesToElement(div, itemChoice);
            if (groupName && typeof label === "string") {
                label = escapeForTemplate(allowHTML, label);
                label += " (".concat(groupName, ")");
                label = {
                    trusted: label
                };
            }
            var describedBy = div;
            if (choice.labelClass) {
                var spanLabel = document.createElement("span");
                setElementHtml(spanLabel, allowHTML, label);
                addClassesToElement(spanLabel, choice.labelClass);
                describedBy = spanLabel;
                div.appendChild(spanLabel);
            } else {
                setElementHtml(div, allowHTML, label);
            }
            if (choice.labelDescription) {
                var descId = "".concat(choice.elementId, "-description");
                describedBy.setAttribute("aria-describedby", descId);
                var spanDesc = document.createElement("span");
                setElementHtml(spanDesc, allowHTML, choice.labelDescription);
                spanDesc.id = descId;
                addClassesToElement(spanDesc, description);
                div.appendChild(spanDesc);
            }
            if (choice.selected) {
                addClassesToElement(div, selectedState);
            }
            if (choice.placeholder) {
                addClassesToElement(div, placeholder);
            }
            div.setAttribute("role", choice.group ? "treeitem" : "option");
            div.dataset.choice = "";
            div.dataset.id = choice.id;
            div.dataset.value = rawValue;
            if (selectText) {
                div.dataset.selectText = selectText;
            }
            if (choice.group) {
                div.dataset.groupId = "".concat(choice.group.id);
            }
            assignCustomProperties(div, choice, false);
            if (choice.disabled) {
                addClassesToElement(div, itemDisabled);
                div.dataset.choiceDisabled = "";
                div.setAttribute("aria-disabled", "true");
            } else {
                addClassesToElement(div, itemSelectable);
                div.dataset.choiceSelectable = "";
            }
            return div;
        },
        input: function(_a, placeholderValue) {
            var _b = _a.classNames, input = _b.input, inputCloned = _b.inputCloned, labelId = _a.labelId;
            var inp = document.createElement("input");
            inp.type = "search";
            addClassesToElement(inp, input);
            addClassesToElement(inp, inputCloned);
            inp.autocomplete = "off";
            inp.autocapitalize = "off";
            inp.spellcheck = false;
            inp.setAttribute("aria-autocomplete", "list");
            if (placeholderValue) {
                inp.setAttribute("aria-label", placeholderValue);
            } else if (!labelId) {
                addAriaLabel(this._docRoot, this.passedElement.element.id, inp);
            }
            return inp;
        },
        dropdown: function(_a) {
            var _b = _a.classNames, list = _b.list, listDropdown = _b.listDropdown;
            var div = document.createElement("div");
            addClassesToElement(div, list);
            addClassesToElement(div, listDropdown);
            div.setAttribute("aria-expanded", "false");
            return div;
        },
        notice: function(_a, innerHTML, type) {
            var _b = _a.classNames, item = _b.item, itemChoice = _b.itemChoice, addChoice = _b.addChoice, noResults = _b.noResults, noChoices = _b.noChoices, noticeItem = _b.notice;
            if (type === void 0) {
                type = NoticeTypes.generic;
            }
            var notice = document.createElement("div");
            setElementHtml(notice, true, innerHTML);
            addClassesToElement(notice, item);
            addClassesToElement(notice, itemChoice);
            addClassesToElement(notice, noticeItem);
            switch (type) {
              case NoticeTypes.addChoice:
                addClassesToElement(notice, addChoice);
                break;

              case NoticeTypes.noResults:
                addClassesToElement(notice, noResults);
                break;

              case NoticeTypes.noChoices:
                addClassesToElement(notice, noChoices);
                break;
            }
            if (type === NoticeTypes.addChoice) {
                notice.dataset.choiceSelectable = "";
                notice.dataset.choice = "";
            }
            return notice;
        },
        option: function(choice) {
            var labelValue = unwrapStringForRaw(choice.label);
            var opt = new Option(labelValue, choice.value, false, choice.selected);
            assignCustomProperties(opt, choice, true);
            opt.disabled = choice.disabled;
            if (choice.selected) {
                opt.setAttribute("selected", "");
            }
            return opt;
        }
    };
    var IS_IE11 = "-ms-scroll-limit" in document.documentElement.style && "-ms-ime-align" in document.documentElement.style;
    var USER_DEFAULTS = {};
    var parseDataSetId = function(element) {
        if (!element) {
            return undefined;
        }
        return element.dataset.id ? parseInt(element.dataset.id, 10) : undefined;
    };
    var selectableChoiceIdentifier = "[data-choice-selectable]";
    var Choices = function() {
        function Choices(element, userConfig) {
            if (element === void 0) {
                element = "[data-choice]";
            }
            if (userConfig === void 0) {
                userConfig = {};
            }
            var _this = this;
            this.initialisedOK = undefined;
            this._hasNonChoicePlaceholder = false;
            this._lastAddedChoiceId = 0;
            this._lastAddedGroupId = 0;
            var defaults = Choices.defaults;
            this.config = __assign(__assign(__assign({}, defaults.allOptions), defaults.options), userConfig);
            ObjectsInConfig.forEach(function(key) {
                _this.config[key] = __assign(__assign(__assign({}, defaults.allOptions[key]), defaults.options[key]), userConfig[key]);
            });
            var config = this.config;
            if (!config.silent) {
                this._validateConfig();
            }
            var docRoot = config.shadowRoot || document.documentElement;
            this._docRoot = docRoot;
            var passedElement = typeof element === "string" ? docRoot.querySelector(element) : element;
            if (!passedElement || typeof passedElement !== "object" || !(isHtmlInputElement(passedElement) || isHtmlSelectElement(passedElement))) {
                if (!passedElement && typeof element === "string") {
                    throw TypeError("Selector ".concat(element, " failed to find an element"));
                }
                throw TypeError("Expected one of the following types text|select-one|select-multiple");
            }
            var elementType = passedElement.type;
            var isText = elementType === PassedElementTypes.Text;
            if (isText || config.maxItemCount !== 1) {
                config.singleModeForMultiSelect = false;
            }
            if (config.singleModeForMultiSelect) {
                elementType = PassedElementTypes.SelectMultiple;
            }
            var isSelectOne = elementType === PassedElementTypes.SelectOne;
            var isSelectMultiple = elementType === PassedElementTypes.SelectMultiple;
            var isSelect = isSelectOne || isSelectMultiple;
            this._elementType = elementType;
            this._isTextElement = isText;
            this._isSelectOneElement = isSelectOne;
            this._isSelectMultipleElement = isSelectMultiple;
            this._isSelectElement = isSelectOne || isSelectMultiple;
            this._canAddUserChoices = isText && config.addItems || isSelect && config.addChoices;
            if (typeof config.renderSelectedChoices !== "boolean") {
                config.renderSelectedChoices = config.renderSelectedChoices === "always" || isSelectOne;
            }
            if (config.closeDropdownOnSelect === "auto") {
                config.closeDropdownOnSelect = isText || isSelectOne || config.singleModeForMultiSelect;
            } else {
                config.closeDropdownOnSelect = coerceBool(config.closeDropdownOnSelect);
            }
            if (config.placeholder) {
                if (config.placeholderValue) {
                    this._hasNonChoicePlaceholder = true;
                } else if (passedElement.dataset.placeholder) {
                    this._hasNonChoicePlaceholder = true;
                    config.placeholderValue = passedElement.dataset.placeholder;
                }
            }
            if (userConfig.addItemFilter && typeof userConfig.addItemFilter !== "function") {
                var re = userConfig.addItemFilter instanceof RegExp ? userConfig.addItemFilter : new RegExp(userConfig.addItemFilter);
                config.addItemFilter = re.test.bind(re);
            }
            if (this._isTextElement) {
                this.passedElement = new WrappedInput({
                    element: passedElement,
                    classNames: config.classNames
                });
            } else {
                var selectEl = passedElement;
                this.passedElement = new WrappedSelect({
                    element: selectEl,
                    classNames: config.classNames,
                    template: function(data) {
                        return _this._templates.option(data);
                    },
                    extractPlaceholder: config.placeholder && !this._hasNonChoicePlaceholder
                });
            }
            this.initialised = false;
            this._store = new Store(config);
            this._currentValue = "";
            config.searchEnabled = !isText && config.searchEnabled || isSelectMultiple;
            this._canSearch = config.searchEnabled;
            this._isScrollingOnIe = false;
            this._highlightPosition = 0;
            this._wasTap = true;
            this._placeholderValue = this._generatePlaceholderValue();
            this._baseId = generateId(passedElement, "choices-");
            this._direction = passedElement.dir;
            if (!this._direction) {
                var elementDirection = window.getComputedStyle(passedElement).direction;
                var documentDirection = window.getComputedStyle(document.documentElement).direction;
                if (elementDirection !== documentDirection) {
                    this._direction = elementDirection;
                }
            }
            this._idNames = {
                itemChoice: "item-choice"
            };
            this._templates = defaults.templates;
            this._render = this._render.bind(this);
            this._onFocus = this._onFocus.bind(this);
            this._onBlur = this._onBlur.bind(this);
            this._onKeyUp = this._onKeyUp.bind(this);
            this._onKeyDown = this._onKeyDown.bind(this);
            this._onInput = this._onInput.bind(this);
            this._onClick = this._onClick.bind(this);
            this._onTouchMove = this._onTouchMove.bind(this);
            this._onTouchEnd = this._onTouchEnd.bind(this);
            this._onMouseDown = this._onMouseDown.bind(this);
            this._onMouseOver = this._onMouseOver.bind(this);
            this._onFormReset = this._onFormReset.bind(this);
            this._onSelectKey = this._onSelectKey.bind(this);
            this._onEnterKey = this._onEnterKey.bind(this);
            this._onEscapeKey = this._onEscapeKey.bind(this);
            this._onDirectionKey = this._onDirectionKey.bind(this);
            this._onDeleteKey = this._onDeleteKey.bind(this);
            if (this.passedElement.isActive) {
                if (!config.silent) {
                    console.warn("Trying to initialise Choices on element already initialised", {
                        element: element
                    });
                }
                this.initialised = true;
                this.initialisedOK = false;
                return;
            }
            this.init();
            this._initialItems = this._store.items.map(function(choice) {
                return choice.value;
            });
        }
        Object.defineProperty(Choices, "defaults", {
            get: function() {
                return Object.preventExtensions({
                    get options() {
                        return USER_DEFAULTS;
                    },
                    get allOptions() {
                        return DEFAULT_CONFIG;
                    },
                    get templates() {
                        return templates;
                    }
                });
            },
            enumerable: false,
            configurable: true
        });
        Choices.prototype.init = function() {
            if (this.initialised || this.initialisedOK !== undefined) {
                return;
            }
            this._searcher = getSearcher(this.config);
            this._loadChoices();
            this._createTemplates();
            this._createElements();
            this._createStructure();
            if (this._isTextElement && !this.config.addItems || this.passedElement.element.hasAttribute("disabled") || !!this.passedElement.element.closest("fieldset:disabled")) {
                this.disable();
            } else {
                this.enable();
                this._addEventListeners();
            }
            this._initStore();
            this.initialised = true;
            this.initialisedOK = true;
            var callbackOnInit = this.config.callbackOnInit;
            if (typeof callbackOnInit === "function") {
                callbackOnInit.call(this);
            }
        };
        Choices.prototype.destroy = function() {
            if (!this.initialised) {
                return;
            }
            this._removeEventListeners();
            this.passedElement.reveal();
            this.containerOuter.unwrap(this.passedElement.element);
            this._store._listeners = [];
            this.clearStore(false);
            this._stopSearch();
            this._templates = Choices.defaults.templates;
            this.initialised = false;
            this.initialisedOK = undefined;
        };
        Choices.prototype.enable = function() {
            if (this.passedElement.isDisabled) {
                this.passedElement.enable();
            }
            if (this.containerOuter.isDisabled) {
                this._addEventListeners();
                this.input.enable();
                this.input.element.focus();
                this.containerOuter.enable();
            }
            return this;
        };
        Choices.prototype.disable = function() {
            if (!this.passedElement.isDisabled) {
                this.passedElement.disable();
            }
            if (!this.containerOuter.isDisabled) {
                this._removeEventListeners();
                this.input.disable();
                this.containerOuter.disable();
            }
            return this;
        };
        Choices.prototype.highlightItem = function(item, runEvent) {
            if (runEvent === void 0) {
                runEvent = true;
            }
            if (!item || !item.id) {
                return this;
            }
            var choice = this._store.items.find(function(c) {
                return c.id === item.id;
            });
            if (!choice || choice.highlighted) {
                return this;
            }
            this._store.dispatch(highlightItem(choice, true));
            if (runEvent) {
                this.passedElement.triggerEvent(EventType.highlightItem, this._getChoiceForOutput(choice));
            }
            return this;
        };
        Choices.prototype.unhighlightItem = function(item, runEvent) {
            if (runEvent === void 0) {
                runEvent = true;
            }
            if (!item || !item.id) {
                return this;
            }
            var choice = this._store.items.find(function(c) {
                return c.id === item.id;
            });
            if (!choice || !choice.highlighted) {
                return this;
            }
            this._store.dispatch(highlightItem(choice, false));
            if (runEvent) {
                this.passedElement.triggerEvent(EventType.unhighlightItem, this._getChoiceForOutput(choice));
            }
            return this;
        };
        Choices.prototype.highlightAll = function() {
            var _this = this;
            this._store.withTxn(function() {
                _this._store.items.forEach(function(item) {
                    if (!item.highlighted) {
                        _this._store.dispatch(highlightItem(item, true));
                        _this.passedElement.triggerEvent(EventType.highlightItem, _this._getChoiceForOutput(item));
                    }
                });
            });
            return this;
        };
        Choices.prototype.unhighlightAll = function() {
            var _this = this;
            this._store.withTxn(function() {
                _this._store.items.forEach(function(item) {
                    if (item.highlighted) {
                        _this._store.dispatch(highlightItem(item, false));
                        _this.passedElement.triggerEvent(EventType.highlightItem, _this._getChoiceForOutput(item));
                    }
                });
            });
            return this;
        };
        Choices.prototype.removeActiveItemsByValue = function(value) {
            var _this = this;
            this._store.withTxn(function() {
                _this._store.items.filter(function(item) {
                    return item.value === value;
                }).forEach(function(item) {
                    return _this._removeItem(item);
                });
            });
            return this;
        };
        Choices.prototype.removeActiveItems = function(excludedId) {
            var _this = this;
            this._store.withTxn(function() {
                _this._store.items.filter(function(_a) {
                    var id = _a.id;
                    return id !== excludedId;
                }).forEach(function(item) {
                    return _this._removeItem(item);
                });
            });
            return this;
        };
        Choices.prototype.removeHighlightedItems = function(runEvent) {
            var _this = this;
            if (runEvent === void 0) {
                runEvent = false;
            }
            this._store.withTxn(function() {
                _this._store.highlightedActiveItems.forEach(function(item) {
                    _this._removeItem(item);
                    if (runEvent) {
                        _this._triggerChange(item.value);
                    }
                });
            });
            return this;
        };
        Choices.prototype.showDropdown = function(preventInputFocus) {
            var _this = this;
            if (this.dropdown.isActive) {
                return this;
            }
            if (preventInputFocus === undefined) {
                preventInputFocus = !this._canSearch;
            }
            requestAnimationFrame(function() {
                _this.dropdown.show();
                var rect = _this.dropdown.element.getBoundingClientRect();
                _this.containerOuter.open(rect.bottom, rect.height);
                if (!preventInputFocus) {
                    _this.input.focus();
                }
                _this.passedElement.triggerEvent(EventType.showDropdown);
            });
            return this;
        };
        Choices.prototype.hideDropdown = function(preventInputBlur) {
            var _this = this;
            if (!this.dropdown.isActive) {
                return this;
            }
            requestAnimationFrame(function() {
                _this.dropdown.hide();
                _this.containerOuter.close();
                if (!preventInputBlur && _this._canSearch) {
                    _this.input.removeActiveDescendant();
                    _this.input.blur();
                }
                _this.passedElement.triggerEvent(EventType.hideDropdown);
            });
            return this;
        };
        Choices.prototype.getValue = function(valueOnly) {
            var _this = this;
            var values = this._store.items.map(function(item) {
                return valueOnly ? item.value : _this._getChoiceForOutput(item);
            });
            return this._isSelectOneElement || this.config.singleModeForMultiSelect ? values[0] : values;
        };
        Choices.prototype.setValue = function(items) {
            var _this = this;
            if (!this.initialisedOK) {
                this._warnChoicesInitFailed("setValue");
                return this;
            }
            this._store.withTxn(function() {
                items.forEach(function(value) {
                    if (value) {
                        _this._addChoice(mapInputToChoice(value, false));
                    }
                });
            });
            this._searcher.reset();
            return this;
        };
        Choices.prototype.setChoiceByValue = function(value) {
            var _this = this;
            if (!this.initialisedOK) {
                this._warnChoicesInitFailed("setChoiceByValue");
                return this;
            }
            if (this._isTextElement) {
                return this;
            }
            this._store.withTxn(function() {
                var choiceValue = Array.isArray(value) ? value : [ value ];
                choiceValue.forEach(function(val) {
                    return _this._findAndSelectChoiceByValue(val);
                });
                _this.unhighlightAll();
            });
            this._searcher.reset();
            return this;
        };
        Choices.prototype.setChoices = function(choicesArrayOrFetcher, value, label, replaceChoices, clearSearchFlag, replaceItems) {
            var _this = this;
            if (choicesArrayOrFetcher === void 0) {
                choicesArrayOrFetcher = [];
            }
            if (value === void 0) {
                value = "value";
            }
            if (label === void 0) {
                label = "label";
            }
            if (replaceChoices === void 0) {
                replaceChoices = false;
            }
            if (clearSearchFlag === void 0) {
                clearSearchFlag = true;
            }
            if (replaceItems === void 0) {
                replaceItems = false;
            }
            if (!this.initialisedOK) {
                this._warnChoicesInitFailed("setChoices");
                return this;
            }
            if (!this._isSelectElement) {
                throw new TypeError("setChoices can't be used with INPUT based Choices");
            }
            if (typeof value !== "string" || !value) {
                throw new TypeError("value parameter must be a name of 'value' field in passed objects");
            }
            if (typeof choicesArrayOrFetcher === "function") {
                var fetcher_1 = choicesArrayOrFetcher(this);
                if (typeof Promise === "function" && fetcher_1 instanceof Promise) {
                    return new Promise(function(resolve) {
                        return requestAnimationFrame(resolve);
                    }).then(function() {
                        return _this._handleLoadingState(true);
                    }).then(function() {
                        return fetcher_1;
                    }).then(function(data) {
                        return _this.setChoices(data, value, label, replaceChoices, clearSearchFlag, replaceItems);
                    }).catch(function(err) {
                        if (!_this.config.silent) {
                            console.error(err);
                        }
                    }).then(function() {
                        return _this._handleLoadingState(false);
                    }).then(function() {
                        return _this;
                    });
                }
                if (!Array.isArray(fetcher_1)) {
                    throw new TypeError(".setChoices first argument function must return either array of choices or Promise, got: ".concat(typeof fetcher_1));
                }
                return this.setChoices(fetcher_1, value, label, false);
            }
            if (!Array.isArray(choicesArrayOrFetcher)) {
                throw new TypeError(".setChoices must be called either with array of choices with a function resulting into Promise of array of choices");
            }
            this.containerOuter.removeLoadingState();
            this._store.withTxn(function() {
                if (clearSearchFlag) {
                    _this._isSearching = false;
                }
                if (replaceChoices) {
                    _this.clearChoices(true, replaceItems);
                }
                var isDefaultValue = value === "value";
                var isDefaultLabel = label === "label";
                choicesArrayOrFetcher.forEach(function(groupOrChoice) {
                    if ("choices" in groupOrChoice) {
                        var group = groupOrChoice;
                        if (!isDefaultLabel) {
                            group = __assign(__assign({}, group), {
                                label: group[label],
                                value: group[value]
                            });
                        }
                        _this._addGroup(mapInputToChoice(group, true));
                    } else {
                        var choice = groupOrChoice;
                        if (!isDefaultLabel || !isDefaultValue) {
                            choice = __assign(__assign({}, choice), {
                                value: choice[value],
                                label: choice[label]
                            });
                        }
                        var choiceFull = mapInputToChoice(choice, false);
                        _this._addChoice(choiceFull);
                        if (choiceFull.placeholder && !_this._hasNonChoicePlaceholder) {
                            _this._placeholderValue = unwrapStringForEscaped(choiceFull.label);
                        }
                    }
                });
                _this.unhighlightAll();
            });
            this._searcher.reset();
            return this;
        };
        Choices.prototype.refresh = function(withEvents, selectFirstOption, deselectAll) {
            var _this = this;
            if (withEvents === void 0) {
                withEvents = false;
            }
            if (selectFirstOption === void 0) {
                selectFirstOption = false;
            }
            if (deselectAll === void 0) {
                deselectAll = false;
            }
            if (!this._isSelectElement) {
                if (!this.config.silent) {
                    console.warn("refresh method can only be used on choices backed by a <select> element");
                }
                return this;
            }
            this._store.withTxn(function() {
                var choicesFromOptions = _this.passedElement.optionsAsChoices();
                var existingItems = {};
                if (!deselectAll) {
                    _this._store.items.forEach(function(choice) {
                        if (choice.id && choice.active && choice.selected) {
                            existingItems[choice.value] = true;
                        }
                    });
                }
                _this.clearStore(false);
                var updateChoice = function(choice) {
                    if (deselectAll) {
                        _this._store.dispatch(removeItem$1(choice));
                    } else if (existingItems[choice.value]) {
                        choice.selected = true;
                    }
                };
                choicesFromOptions.forEach(function(groupOrChoice) {
                    if ("choices" in groupOrChoice) {
                        groupOrChoice.choices.forEach(updateChoice);
                        return;
                    }
                    updateChoice(groupOrChoice);
                });
                _this._addPredefinedChoices(choicesFromOptions, selectFirstOption, withEvents);
                if (_this._isSearching) {
                    _this._searchChoices(_this.input.value);
                }
            });
            return this;
        };
        Choices.prototype.removeChoice = function(value) {
            var choice = this._store.choices.find(function(c) {
                return c.value === value;
            });
            if (!choice) {
                return this;
            }
            this._clearNotice();
            this._store.dispatch(removeChoice(choice));
            this._searcher.reset();
            if (choice.selected) {
                this.passedElement.triggerEvent(EventType.removeItem, this._getChoiceForOutput(choice));
            }
            return this;
        };
        Choices.prototype.clearChoices = function(clearOptions, clearItems) {
            var _this = this;
            if (clearOptions === void 0) {
                clearOptions = true;
            }
            if (clearItems === void 0) {
                clearItems = false;
            }
            if (clearOptions) {
                if (clearItems) {
                    this.passedElement.element.replaceChildren("");
                } else {
                    this.passedElement.element.querySelectorAll(":not([selected])").forEach(function(el) {
                        el.remove();
                    });
                }
            }
            this.itemList.element.replaceChildren("");
            this.choiceList.element.replaceChildren("");
            this._clearNotice();
            this._store.withTxn(function() {
                var items = clearItems ? [] : _this._store.items;
                _this._store.reset();
                items.forEach(function(item) {
                    _this._store.dispatch(addChoice(item));
                    _this._store.dispatch(addItem(item));
                });
            });
            this._searcher.reset();
            return this;
        };
        Choices.prototype.clearStore = function(clearOptions) {
            if (clearOptions === void 0) {
                clearOptions = true;
            }
            this.clearChoices(clearOptions, true);
            this._stopSearch();
            this._lastAddedChoiceId = 0;
            this._lastAddedGroupId = 0;
            return this;
        };
        Choices.prototype.clearInput = function() {
            var shouldSetInputWidth = !this._isSelectOneElement;
            this.input.clear(shouldSetInputWidth);
            this._stopSearch();
            return this;
        };
        Choices.prototype._validateConfig = function() {
            var config = this.config;
            var invalidConfigOptions = diff(config, DEFAULT_CONFIG);
            if (invalidConfigOptions.length) {
                console.warn("Unknown config option(s) passed", invalidConfigOptions.join(", "));
            }
            if (config.allowHTML && config.allowHtmlUserInput) {
                if (config.addItems) {
                    console.warn("Warning: allowHTML/allowHtmlUserInput/addItems all being true is strongly not recommended and may lead to XSS attacks");
                }
                if (config.addChoices) {
                    console.warn("Warning: allowHTML/allowHtmlUserInput/addChoices all being true is strongly not recommended and may lead to XSS attacks");
                }
            }
        };
        Choices.prototype._render = function(changes) {
            if (changes === void 0) {
                changes = {
                    choices: true,
                    groups: true,
                    items: true
                };
            }
            if (this._store.inTxn()) {
                return;
            }
            if (this._isSelectElement) {
                if (changes.choices || changes.groups) {
                    this._renderChoices();
                }
            }
            if (changes.items) {
                this._renderItems();
            }
        };
        Choices.prototype._renderChoices = function() {
            var _this = this;
            if (!this._canAddItems()) {
                return;
            }
            var _a = this, config = _a.config, isSearching = _a._isSearching;
            var _b = this._store, activeGroups = _b.activeGroups, activeChoices = _b.activeChoices;
            var renderLimit = 0;
            if (isSearching && config.searchResultLimit > 0) {
                renderLimit = config.searchResultLimit;
            } else if (config.renderChoiceLimit > 0) {
                renderLimit = config.renderChoiceLimit;
            }
            if (this._isSelectElement) {
                var backingOptions = activeChoices.filter(function(choice) {
                    return !choice.element;
                });
                if (backingOptions.length) {
                    this.passedElement.addOptions(backingOptions);
                }
            }
            var fragment = document.createDocumentFragment();
            var renderableChoices = function(choices) {
                return choices.filter(function(choice) {
                    return !choice.placeholder && (isSearching ? !!choice.rank : config.renderSelectedChoices || !choice.selected);
                });
            };
            var selectableChoices = false;
            var renderChoices = function(choices, withinGroup, groupLabel) {
                if (isSearching) {
                    choices.sort(sortByRank);
                } else if (config.shouldSort) {
                    choices.sort(config.sorter);
                }
                var choiceLimit = choices.length;
                choiceLimit = !withinGroup && renderLimit && choiceLimit > renderLimit ? renderLimit : choiceLimit;
                choiceLimit--;
                choices.every(function(choice, index) {
                    var dropdownItem = choice.choiceEl || _this._templates.choice(config, choice, config.itemSelectText, groupLabel);
                    choice.choiceEl = dropdownItem;
                    fragment.appendChild(dropdownItem);
                    if (isSearching || !choice.selected) {
                        selectableChoices = true;
                    }
                    return index < choiceLimit;
                });
            };
            if (activeChoices.length) {
                if (config.resetScrollPosition) {
                    requestAnimationFrame(function() {
                        return _this.choiceList.scrollToTop();
                    });
                }
                if (!this._hasNonChoicePlaceholder && !isSearching && this._isSelectOneElement) {
                    renderChoices(activeChoices.filter(function(choice) {
                        return choice.placeholder && !choice.group;
                    }), false, undefined);
                }
                if (activeGroups.length && !isSearching) {
                    if (config.shouldSort) {
                        activeGroups.sort(config.sorter);
                    }
                    renderChoices(activeChoices.filter(function(choice) {
                        return !choice.placeholder && !choice.group;
                    }), false, undefined);
                    activeGroups.forEach(function(group) {
                        var groupChoices = renderableChoices(group.choices);
                        if (groupChoices.length) {
                            if (group.label) {
                                var dropdownGroup = group.groupEl || _this._templates.choiceGroup(_this.config, group);
                                group.groupEl = dropdownGroup;
                                dropdownGroup.remove();
                                fragment.appendChild(dropdownGroup);
                            }
                            renderChoices(groupChoices, true, config.appendGroupInSearch && isSearching ? group.label : undefined);
                        }
                    });
                } else {
                    renderChoices(renderableChoices(activeChoices), false, undefined);
                }
            }
            if (!selectableChoices && (isSearching || !fragment.children.length || !config.renderSelectedChoices)) {
                if (!this._notice) {
                    this._notice = {
                        text: resolveStringFunction(isSearching ? config.noResultsText : config.noChoicesText),
                        type: isSearching ? NoticeTypes.noResults : NoticeTypes.noChoices
                    };
                }
                fragment.replaceChildren("");
            }
            this._renderNotice(fragment);
            this.choiceList.element.replaceChildren(fragment);
            if (selectableChoices) {
                this._highlightChoice();
            }
        };
        Choices.prototype._renderItems = function() {
            var _this = this;
            var items = this._store.items || [];
            var itemList = this.itemList.element;
            var config = this.config;
            var fragment = document.createDocumentFragment();
            var itemFromList = function(item) {
                return itemList.querySelector('[data-item][data-id="'.concat(item.id, '"]'));
            };
            var addItemToFragment = function(item) {
                var el = item.itemEl;
                if (el && el.parentElement) {
                    return;
                }
                el = itemFromList(item) || _this._templates.item(config, item, config.removeItemButton);
                item.itemEl = el;
                fragment.appendChild(el);
            };
            items.forEach(addItemToFragment);
            var addedItems = !!fragment.childNodes.length;
            if (this._isSelectOneElement) {
                var existingItems = itemList.children.length;
                if (addedItems || existingItems > 1) {
                    var placeholder = itemList.querySelector(getClassNamesSelector(config.classNames.placeholder));
                    if (placeholder) {
                        placeholder.remove();
                    }
                } else if (!addedItems && !existingItems && this._placeholderValue) {
                    addedItems = true;
                    addItemToFragment(mapInputToChoice({
                        selected: true,
                        value: "",
                        label: this._placeholderValue,
                        placeholder: true
                    }, false));
                }
            }
            if (addedItems) {
                itemList.append(fragment);
                if (config.shouldSortItems && !this._isSelectOneElement) {
                    items.sort(config.sorter);
                    items.forEach(function(item) {
                        var el = itemFromList(item);
                        if (el) {
                            el.remove();
                            fragment.append(el);
                        }
                    });
                    itemList.append(fragment);
                }
            }
            if (this._isTextElement) {
                this.passedElement.value = items.map(function(_a) {
                    var value = _a.value;
                    return value;
                }).join(config.delimiter);
            }
        };
        Choices.prototype._displayNotice = function(text, type, openDropdown) {
            if (openDropdown === void 0) {
                openDropdown = true;
            }
            var oldNotice = this._notice;
            if (oldNotice && (oldNotice.type === type && oldNotice.text === text || oldNotice.type === NoticeTypes.addChoice && (type === NoticeTypes.noResults || type === NoticeTypes.noChoices))) {
                if (openDropdown) {
                    this.showDropdown(true);
                }
                return;
            }
            this._clearNotice();
            this._notice = text ? {
                text: text,
                type: type
            } : undefined;
            this._renderNotice();
            if (openDropdown && text) {
                this.showDropdown(true);
            }
        };
        Choices.prototype._clearNotice = function() {
            if (!this._notice) {
                return;
            }
            var noticeElement = this.choiceList.element.querySelector(getClassNamesSelector(this.config.classNames.notice));
            if (noticeElement) {
                noticeElement.remove();
            }
            this._notice = undefined;
        };
        Choices.prototype._renderNotice = function(fragment) {
            var noticeConf = this._notice;
            if (noticeConf) {
                var notice = this._templates.notice(this.config, noticeConf.text, noticeConf.type);
                if (fragment) {
                    fragment.append(notice);
                } else {
                    this.choiceList.prepend(notice);
                }
            }
        };
        Choices.prototype._getChoiceForOutput = function(choice, keyCode) {
            return {
                id: choice.id,
                highlighted: choice.highlighted,
                labelClass: choice.labelClass,
                labelDescription: choice.labelDescription,
                customProperties: choice.customProperties,
                disabled: choice.disabled,
                active: choice.active,
                label: choice.label,
                placeholder: choice.placeholder,
                value: choice.value,
                groupValue: choice.group ? choice.group.label : undefined,
                element: choice.element,
                keyCode: keyCode
            };
        };
        Choices.prototype._triggerChange = function(value) {
            if (value === undefined || value === null) {
                return;
            }
            this.passedElement.triggerEvent(EventType.change, {
                value: value
            });
        };
        Choices.prototype._handleButtonAction = function(element) {
            var _this = this;
            var items = this._store.items;
            if (!items.length || !this.config.removeItems || !this.config.removeItemButton) {
                return;
            }
            var id = element && parseDataSetId(element.parentElement);
            var itemToRemove = id && items.find(function(item) {
                return item.id === id;
            });
            if (!itemToRemove) {
                return;
            }
            this._store.withTxn(function() {
                _this._removeItem(itemToRemove);
                _this._triggerChange(itemToRemove.value);
                if (_this._isSelectOneElement && !_this._hasNonChoicePlaceholder) {
                    var placeholderChoice = (_this.config.shouldSort ? _this._store.choices.reverse() : _this._store.choices).find(function(choice) {
                        return choice.placeholder;
                    });
                    if (placeholderChoice) {
                        _this._addItem(placeholderChoice);
                        _this.unhighlightAll();
                        if (placeholderChoice.value) {
                            _this._triggerChange(placeholderChoice.value);
                        }
                    }
                }
            });
        };
        Choices.prototype._handleItemAction = function(element, hasShiftKey) {
            var _this = this;
            if (hasShiftKey === void 0) {
                hasShiftKey = false;
            }
            var items = this._store.items;
            if (!items.length || !this.config.removeItems || this._isSelectOneElement) {
                return;
            }
            var id = parseDataSetId(element);
            if (!id) {
                return;
            }
            items.forEach(function(item) {
                if (item.id === id && !item.highlighted) {
                    _this.highlightItem(item);
                } else if (!hasShiftKey && item.highlighted) {
                    _this.unhighlightItem(item);
                }
            });
            this.input.focus();
        };
        Choices.prototype._handleChoiceAction = function(element) {
            var _this = this;
            var id = parseDataSetId(element);
            var choice = id && this._store.getChoiceById(id);
            if (!choice || choice.disabled) {
                return false;
            }
            var hasActiveDropdown = this.dropdown.isActive;
            if (!choice.selected) {
                if (!this._canAddItems()) {
                    return true;
                }
                this._store.withTxn(function() {
                    _this._addItem(choice, true, true);
                    _this.clearInput();
                    _this.unhighlightAll();
                });
                this._triggerChange(choice.value);
            }
            if (hasActiveDropdown && this.config.closeDropdownOnSelect) {
                this.hideDropdown(true);
                this.containerOuter.element.focus();
            }
            return true;
        };
        Choices.prototype._handleBackspace = function(items) {
            var config = this.config;
            if (!config.removeItems || !items.length) {
                return;
            }
            var lastItem = items[items.length - 1];
            var hasHighlightedItems = items.some(function(item) {
                return item.highlighted;
            });
            if (config.editItems && !hasHighlightedItems && lastItem) {
                this.input.value = lastItem.value;
                this.input.setWidth();
                this._removeItem(lastItem);
                this._triggerChange(lastItem.value);
            } else {
                if (!hasHighlightedItems) {
                    this.highlightItem(lastItem, false);
                }
                this.removeHighlightedItems(true);
            }
        };
        Choices.prototype._loadChoices = function() {
            var _a;
            var _this = this;
            var config = this.config;
            if (this._isTextElement) {
                this._presetChoices = config.items.map(function(e) {
                    return mapInputToChoice(e, false);
                });
                if (this.passedElement.value) {
                    var elementItems = this.passedElement.value.split(config.delimiter).map(function(e) {
                        return mapInputToChoice(e, false, _this.config.allowHtmlUserInput);
                    });
                    this._presetChoices = this._presetChoices.concat(elementItems);
                }
                this._presetChoices.forEach(function(choice) {
                    choice.selected = true;
                });
            } else if (this._isSelectElement) {
                this._presetChoices = config.choices.map(function(e) {
                    return mapInputToChoice(e, true);
                });
                var choicesFromOptions = this.passedElement.optionsAsChoices();
                if (choicesFromOptions) {
                    (_a = this._presetChoices).push.apply(_a, choicesFromOptions);
                }
            }
        };
        Choices.prototype._handleLoadingState = function(setLoading) {
            if (setLoading === void 0) {
                setLoading = true;
            }
            var el = this.itemList.element;
            if (setLoading) {
                this.disable();
                this.containerOuter.addLoadingState();
                if (this._isSelectOneElement) {
                    el.replaceChildren(this._templates.placeholder(this.config, this.config.loadingText));
                } else {
                    this.input.placeholder = this.config.loadingText;
                }
            } else {
                this.enable();
                this.containerOuter.removeLoadingState();
                if (this._isSelectOneElement) {
                    el.replaceChildren("");
                    this._render();
                } else {
                    this.input.placeholder = this._placeholderValue || "";
                }
            }
        };
        Choices.prototype._handleSearch = function(value) {
            if (!this.input.isFocussed) {
                return;
            }
            if (value !== null && typeof value !== "undefined" && value.length >= this.config.searchFloor) {
                var resultCount = this.config.searchChoices ? this._searchChoices(value) : 0;
                if (resultCount !== null) {
                    this.passedElement.triggerEvent(EventType.search, {
                        value: value,
                        resultCount: resultCount
                    });
                }
            } else if (this._store.choices.some(function(option) {
                return !option.active;
            })) {
                this._stopSearch();
            }
        };
        Choices.prototype._canAddItems = function() {
            var config = this.config;
            var maxItemCount = config.maxItemCount, maxItemText = config.maxItemText;
            if (!config.singleModeForMultiSelect && maxItemCount > 0 && maxItemCount <= this._store.items.length) {
                this.choiceList.element.replaceChildren("");
                this._notice = undefined;
                this._displayNotice(typeof maxItemText === "function" ? maxItemText(maxItemCount) : maxItemText, NoticeTypes.addChoice);
                return false;
            }
            if (this._notice && this._notice.type === NoticeTypes.addChoice) {
                this._clearNotice();
            }
            return true;
        };
        Choices.prototype._canCreateItem = function(value) {
            var config = this.config;
            var canAddItem = true;
            var notice = "";
            if (canAddItem && typeof config.addItemFilter === "function" && !config.addItemFilter(value)) {
                canAddItem = false;
                notice = resolveNoticeFunction(config.customAddItemText, value);
            }
            if (canAddItem) {
                var foundChoice = this._store.choices.find(function(choice) {
                    return config.valueComparer(choice.value, value);
                });
                if (foundChoice) {
                    if (this._isSelectElement) {
                        this._displayNotice("", NoticeTypes.addChoice);
                        return false;
                    }
                    if (!config.duplicateItemsAllowed) {
                        canAddItem = false;
                        notice = resolveNoticeFunction(config.uniqueItemText, value);
                    }
                }
            }
            if (canAddItem) {
                notice = resolveNoticeFunction(config.addItemText, value);
            }
            if (notice) {
                this._displayNotice(notice, NoticeTypes.addChoice);
            }
            return canAddItem;
        };
        Choices.prototype._searchChoices = function(value) {
            var newValue = value.trim().replace(/\s{2,}/, " ");
            if (!newValue.length || newValue === this._currentValue) {
                return null;
            }
            var searcher = this._searcher;
            if (searcher.isEmptyIndex()) {
                searcher.index(this._store.searchableChoices);
            }
            var results = searcher.search(newValue);
            this._currentValue = newValue;
            this._highlightPosition = 0;
            this._isSearching = true;
            var notice = this._notice;
            var noticeType = notice && notice.type;
            if (noticeType !== NoticeTypes.addChoice) {
                if (!results.length) {
                    this._displayNotice(resolveStringFunction(this.config.noResultsText), NoticeTypes.noResults);
                } else {
                    this._clearNotice();
                }
            }
            this._store.dispatch(filterChoices(results));
            return results.length;
        };
        Choices.prototype._stopSearch = function() {
            if (this._isSearching) {
                this._currentValue = "";
                this._isSearching = false;
                this._clearNotice();
                this._store.dispatch(activateChoices(true));
                this.passedElement.triggerEvent(EventType.search, {
                    value: "",
                    resultCount: 0
                });
            }
        };
        Choices.prototype._addEventListeners = function() {
            var documentElement = this._docRoot;
            var outerElement = this.containerOuter.element;
            var inputElement = this.input.element;
            documentElement.addEventListener("touchend", this._onTouchEnd, true);
            outerElement.addEventListener("keydown", this._onKeyDown, true);
            outerElement.addEventListener("mousedown", this._onMouseDown, true);
            documentElement.addEventListener("click", this._onClick, {
                passive: true
            });
            documentElement.addEventListener("touchmove", this._onTouchMove, {
                passive: true
            });
            this.dropdown.element.addEventListener("mouseover", this._onMouseOver, {
                passive: true
            });
            if (this._isSelectOneElement) {
                outerElement.addEventListener("focus", this._onFocus, {
                    passive: true
                });
                outerElement.addEventListener("blur", this._onBlur, {
                    passive: true
                });
            }
            inputElement.addEventListener("keyup", this._onKeyUp, {
                passive: true
            });
            inputElement.addEventListener("input", this._onInput, {
                passive: true
            });
            inputElement.addEventListener("focus", this._onFocus, {
                passive: true
            });
            inputElement.addEventListener("blur", this._onBlur, {
                passive: true
            });
            if (inputElement.form) {
                inputElement.form.addEventListener("reset", this._onFormReset, {
                    passive: true
                });
            }
            this.input.addEventListeners();
        };
        Choices.prototype._removeEventListeners = function() {
            var documentElement = this._docRoot;
            var outerElement = this.containerOuter.element;
            var inputElement = this.input.element;
            documentElement.removeEventListener("touchend", this._onTouchEnd, true);
            outerElement.removeEventListener("keydown", this._onKeyDown, true);
            outerElement.removeEventListener("mousedown", this._onMouseDown, true);
            documentElement.removeEventListener("click", this._onClick);
            documentElement.removeEventListener("touchmove", this._onTouchMove);
            this.dropdown.element.removeEventListener("mouseover", this._onMouseOver);
            if (this._isSelectOneElement) {
                outerElement.removeEventListener("focus", this._onFocus);
                outerElement.removeEventListener("blur", this._onBlur);
            }
            inputElement.removeEventListener("keyup", this._onKeyUp);
            inputElement.removeEventListener("input", this._onInput);
            inputElement.removeEventListener("focus", this._onFocus);
            inputElement.removeEventListener("blur", this._onBlur);
            if (inputElement.form) {
                inputElement.form.removeEventListener("reset", this._onFormReset);
            }
            this.input.removeEventListeners();
        };
        Choices.prototype._onKeyDown = function(event) {
            var keyCode = event.keyCode;
            var hasActiveDropdown = this.dropdown.isActive;
            var wasPrintableChar = event.key.length === 1 || event.key.length === 2 && event.key.charCodeAt(0) >= 55296 || event.key === "Unidentified";
            if (!this._isTextElement && !hasActiveDropdown && keyCode !== KeyCodeMap.ESC_KEY && keyCode !== KeyCodeMap.TAB_KEY && keyCode !== KeyCodeMap.SHIFT_KEY) {
                this.showDropdown();
                if (!this.input.isFocussed && wasPrintableChar) {
                    this.input.value += event.key;
                    if (event.key === " ") {
                        event.preventDefault();
                    }
                }
            }
            switch (keyCode) {
              case KeyCodeMap.A_KEY:
                return this._onSelectKey(event, this.itemList.element.hasChildNodes());

              case KeyCodeMap.ENTER_KEY:
                return this._onEnterKey(event, hasActiveDropdown);

              case KeyCodeMap.ESC_KEY:
                return this._onEscapeKey(event, hasActiveDropdown);

              case KeyCodeMap.UP_KEY:
              case KeyCodeMap.PAGE_UP_KEY:
              case KeyCodeMap.DOWN_KEY:
              case KeyCodeMap.PAGE_DOWN_KEY:
                return this._onDirectionKey(event, hasActiveDropdown);

              case KeyCodeMap.DELETE_KEY:
              case KeyCodeMap.BACK_KEY:
                return this._onDeleteKey(event, this._store.items, this.input.isFocussed);
            }
        };
        Choices.prototype._onKeyUp = function() {
            this._canSearch = this.config.searchEnabled;
        };
        Choices.prototype._onInput = function() {
            var value = this.input.value;
            if (!value) {
                if (this._isTextElement) {
                    this.hideDropdown(true);
                } else {
                    this._stopSearch();
                }
                return;
            }
            if (!this._canAddItems()) {
                return;
            }
            if (this._canSearch) {
                this._handleSearch(value);
            }
            if (!this._canAddUserChoices) {
                return;
            }
            this._canCreateItem(value);
            if (this._isSelectElement) {
                this._highlightPosition = 0;
                this._highlightChoice();
            }
        };
        Choices.prototype._onSelectKey = function(event, hasItems) {
            if ((event.ctrlKey || event.metaKey) && hasItems) {
                this._canSearch = false;
                var shouldHightlightAll = this.config.removeItems && !this.input.value && this.input.element === document.activeElement;
                if (shouldHightlightAll) {
                    this.highlightAll();
                }
            }
        };
        Choices.prototype._onEnterKey = function(event, hasActiveDropdown) {
            var _this = this;
            var value = this.input.value;
            var target = event.target;
            event.preventDefault();
            if (target && target.hasAttribute("data-button")) {
                this._handleButtonAction(target);
                return;
            }
            if (!hasActiveDropdown) {
                if (this._isSelectElement || this._notice) {
                    this.showDropdown();
                }
                return;
            }
            var highlightedChoice = this.dropdown.element.querySelector(getClassNamesSelector(this.config.classNames.highlightedState));
            if (highlightedChoice && this._handleChoiceAction(highlightedChoice)) {
                return;
            }
            if (!target || !value) {
                this.hideDropdown(true);
                return;
            }
            if (!this._canAddItems()) {
                return;
            }
            var addedItem = false;
            this._store.withTxn(function() {
                addedItem = _this._findAndSelectChoiceByValue(value, true);
                if (!addedItem) {
                    if (!_this._canAddUserChoices) {
                        return;
                    }
                    if (!_this._canCreateItem(value)) {
                        return;
                    }
                    _this._addChoice(mapInputToChoice(value, false, _this.config.allowHtmlUserInput), true, true);
                    addedItem = true;
                }
                _this.clearInput();
                _this.unhighlightAll();
            });
            if (!addedItem) {
                return;
            }
            this._triggerChange(value);
            if (this.config.closeDropdownOnSelect) {
                this.hideDropdown(true);
            }
        };
        Choices.prototype._onEscapeKey = function(event, hasActiveDropdown) {
            if (hasActiveDropdown) {
                event.stopPropagation();
                this.hideDropdown(true);
                this._stopSearch();
                this.containerOuter.element.focus();
            }
        };
        Choices.prototype._onDirectionKey = function(event, hasActiveDropdown) {
            var keyCode = event.keyCode;
            if (hasActiveDropdown || this._isSelectOneElement) {
                this.showDropdown();
                this._canSearch = false;
                var directionInt = keyCode === KeyCodeMap.DOWN_KEY || keyCode === KeyCodeMap.PAGE_DOWN_KEY ? 1 : -1;
                var skipKey = event.metaKey || keyCode === KeyCodeMap.PAGE_DOWN_KEY || keyCode === KeyCodeMap.PAGE_UP_KEY;
                var nextEl = void 0;
                if (skipKey) {
                    if (directionInt > 0) {
                        nextEl = this.dropdown.element.querySelector("".concat(selectableChoiceIdentifier, ":last-of-type"));
                    } else {
                        nextEl = this.dropdown.element.querySelector(selectableChoiceIdentifier);
                    }
                } else {
                    var currentEl = this.dropdown.element.querySelector(getClassNamesSelector(this.config.classNames.highlightedState));
                    if (currentEl) {
                        nextEl = getAdjacentEl(currentEl, selectableChoiceIdentifier, directionInt);
                    } else {
                        nextEl = this.dropdown.element.querySelector(selectableChoiceIdentifier);
                    }
                }
                if (nextEl) {
                    if (!isScrolledIntoView(nextEl, this.choiceList.element, directionInt)) {
                        this.choiceList.scrollToChildElement(nextEl, directionInt);
                    }
                    this._highlightChoice(nextEl);
                }
                event.preventDefault();
            }
        };
        Choices.prototype._onDeleteKey = function(event, items, hasFocusedInput) {
            if (!this._isSelectOneElement && !event.target.value && hasFocusedInput) {
                this._handleBackspace(items);
                event.preventDefault();
            }
        };
        Choices.prototype._onTouchMove = function() {
            if (this._wasTap) {
                this._wasTap = false;
            }
        };
        Choices.prototype._onTouchEnd = function(event) {
            var target = (event || event.touches[0]).target;
            var touchWasWithinContainer = this._wasTap && this.containerOuter.element.contains(target);
            if (touchWasWithinContainer) {
                var containerWasExactTarget = target === this.containerOuter.element || target === this.containerInner.element;
                if (containerWasExactTarget) {
                    if (this._isTextElement) {
                        this.input.focus();
                    } else if (this._isSelectMultipleElement) {
                        this.showDropdown();
                    }
                }
                event.stopPropagation();
            }
            this._wasTap = true;
        };
        Choices.prototype._onMouseDown = function(event) {
            var target = event.target;
            if (!(target instanceof HTMLElement)) {
                return;
            }
            if (IS_IE11 && this.choiceList.element.contains(target)) {
                var firstChoice = this.choiceList.element.firstElementChild;
                this._isScrollingOnIe = this._direction === "ltr" ? event.offsetX >= firstChoice.offsetWidth : event.offsetX < firstChoice.offsetLeft;
            }
            if (target === this.input.element) {
                return;
            }
            var item = target.closest("[data-button],[data-item],[data-choice]");
            if (item instanceof HTMLElement) {
                if ("button" in item.dataset) {
                    this._handleButtonAction(item);
                } else if ("item" in item.dataset) {
                    this._handleItemAction(item, event.shiftKey);
                } else if ("choice" in item.dataset) {
                    this._handleChoiceAction(item);
                }
            }
            event.preventDefault();
        };
        Choices.prototype._onMouseOver = function(_a) {
            var target = _a.target;
            if (target instanceof HTMLElement && "choice" in target.dataset) {
                this._highlightChoice(target);
            }
        };
        Choices.prototype._onClick = function(_a) {
            var target = _a.target;
            var containerOuter = this.containerOuter;
            var clickWasWithinContainer = containerOuter.element.contains(target);
            if (clickWasWithinContainer) {
                if (!this.dropdown.isActive && !containerOuter.isDisabled) {
                    if (this._isTextElement) {
                        if (document.activeElement !== this.input.element) {
                            this.input.focus();
                        }
                    } else {
                        this.showDropdown();
                        containerOuter.element.focus();
                    }
                } else if (this._isSelectOneElement && target !== this.input.element && !this.dropdown.element.contains(target)) {
                    this.hideDropdown();
                }
            } else {
                containerOuter.removeFocusState();
                this.hideDropdown(true);
                this.unhighlightAll();
            }
        };
        Choices.prototype._onFocus = function(_a) {
            var target = _a.target;
            var containerOuter = this.containerOuter;
            var focusWasWithinContainer = target && containerOuter.element.contains(target);
            if (!focusWasWithinContainer) {
                return;
            }
            var targetIsInput = target === this.input.element;
            if (this._isTextElement) {
                if (targetIsInput) {
                    containerOuter.addFocusState();
                }
            } else if (this._isSelectMultipleElement) {
                if (targetIsInput) {
                    this.showDropdown(true);
                    containerOuter.addFocusState();
                }
            } else {
                containerOuter.addFocusState();
                if (targetIsInput) {
                    this.showDropdown(true);
                }
            }
        };
        Choices.prototype._onBlur = function(_a) {
            var target = _a.target;
            var containerOuter = this.containerOuter;
            var blurWasWithinContainer = target && containerOuter.element.contains(target);
            if (blurWasWithinContainer && !this._isScrollingOnIe) {
                if (target === this.input.element) {
                    containerOuter.removeFocusState();
                    this.hideDropdown(true);
                    if (this._isTextElement || this._isSelectMultipleElement) {
                        this.unhighlightAll();
                    }
                } else if (target === this.containerOuter.element) {
                    containerOuter.removeFocusState();
                    if (!this._canSearch) {
                        this.hideDropdown(true);
                    }
                }
            } else {
                this._isScrollingOnIe = false;
                this.input.element.focus();
            }
        };
        Choices.prototype._onFormReset = function() {
            var _this = this;
            this._store.withTxn(function() {
                _this.clearInput();
                _this.hideDropdown();
                _this.refresh(false, false, true);
                if (_this._initialItems.length) {
                    _this.setChoiceByValue(_this._initialItems);
                }
            });
        };
        Choices.prototype._highlightChoice = function(el) {
            if (el === void 0) {
                el = null;
            }
            var choices = Array.from(this.dropdown.element.querySelectorAll(selectableChoiceIdentifier));
            if (!choices.length) {
                return;
            }
            var passedEl = el;
            var highlightedState = this.config.classNames.highlightedState;
            var highlightedChoices = Array.from(this.dropdown.element.querySelectorAll(getClassNamesSelector(highlightedState)));
            highlightedChoices.forEach(function(choice) {
                removeClassesFromElement(choice, highlightedState);
                choice.setAttribute("aria-selected", "false");
            });
            if (passedEl) {
                this._highlightPosition = choices.indexOf(passedEl);
            } else {
                if (choices.length > this._highlightPosition) {
                    passedEl = choices[this._highlightPosition];
                } else {
                    passedEl = choices[choices.length - 1];
                }
                if (!passedEl) {
                    passedEl = choices[0];
                }
            }
            addClassesToElement(passedEl, highlightedState);
            passedEl.setAttribute("aria-selected", "true");
            this.passedElement.triggerEvent(EventType.highlightChoice, {
                el: passedEl
            });
            if (this.dropdown.isActive) {
                this.input.setActiveDescendant(passedEl.id);
                this.containerOuter.setActiveDescendant(passedEl.id);
            }
        };
        Choices.prototype._addItem = function(item, withEvents, userTriggered) {
            if (withEvents === void 0) {
                withEvents = true;
            }
            if (userTriggered === void 0) {
                userTriggered = false;
            }
            if (!item.id) {
                throw new TypeError("item.id must be set before _addItem is called for a choice/item");
            }
            if (this.config.singleModeForMultiSelect || this._isSelectOneElement) {
                this.removeActiveItems(item.id);
            }
            this._store.dispatch(addItem(item));
            if (withEvents) {
                this.passedElement.triggerEvent(EventType.addItem, this._getChoiceForOutput(item));
                if (userTriggered) {
                    this.passedElement.triggerEvent(EventType.choice, this._getChoiceForOutput(item));
                }
            }
        };
        Choices.prototype._removeItem = function(item) {
            if (!item.id) {
                return;
            }
            this._store.dispatch(removeItem$1(item));
            var notice = this._notice;
            if (notice && notice.type === NoticeTypes.noChoices) {
                this._clearNotice();
            }
            this.passedElement.triggerEvent(EventType.removeItem, this._getChoiceForOutput(item));
        };
        Choices.prototype._addChoice = function(choice, withEvents, userTriggered) {
            if (withEvents === void 0) {
                withEvents = true;
            }
            if (userTriggered === void 0) {
                userTriggered = false;
            }
            if (choice.id) {
                throw new TypeError("Can not re-add a choice which has already been added");
            }
            var config = this.config;
            if (!config.duplicateItemsAllowed && this._store.choices.find(function(c) {
                return config.valueComparer(c.value, choice.value);
            })) {
                return;
            }
            this._lastAddedChoiceId++;
            choice.id = this._lastAddedChoiceId;
            choice.elementId = "".concat(this._baseId, "-").concat(this._idNames.itemChoice, "-").concat(choice.id);
            var prependValue = config.prependValue, appendValue = config.appendValue;
            if (prependValue) {
                choice.value = prependValue + choice.value;
            }
            if (appendValue) {
                choice.value += appendValue.toString();
            }
            if ((prependValue || appendValue) && choice.element) {
                choice.element.value = choice.value;
            }
            this._clearNotice();
            this._store.dispatch(addChoice(choice));
            if (choice.selected) {
                this._addItem(choice, withEvents, userTriggered);
            }
        };
        Choices.prototype._addGroup = function(group, withEvents) {
            var _this = this;
            if (withEvents === void 0) {
                withEvents = true;
            }
            if (group.id) {
                throw new TypeError("Can not re-add a group which has already been added");
            }
            this._store.dispatch(addGroup(group));
            if (!group.choices) {
                return;
            }
            this._lastAddedGroupId++;
            group.id = this._lastAddedGroupId;
            group.choices.forEach(function(item) {
                item.group = group;
                if (group.disabled) {
                    item.disabled = true;
                }
                _this._addChoice(item, withEvents);
            });
        };
        Choices.prototype._createTemplates = function() {
            var _this = this;
            var callbackOnCreateTemplates = this.config.callbackOnCreateTemplates;
            var userTemplates = {};
            if (typeof callbackOnCreateTemplates === "function") {
                userTemplates = callbackOnCreateTemplates.call(this, strToEl, escapeForTemplate, getClassNames);
            }
            var templating = {};
            Object.keys(this._templates).forEach(function(name) {
                if (name in userTemplates) {
                    templating[name] = userTemplates[name].bind(_this);
                } else {
                    templating[name] = _this._templates[name].bind(_this);
                }
            });
            this._templates = templating;
        };
        Choices.prototype._createElements = function() {
            var templating = this._templates;
            var _a = this, config = _a.config, isSelectOneElement = _a._isSelectOneElement;
            var position = config.position, classNames = config.classNames;
            var elementType = this._elementType;
            this.containerOuter = new Container({
                element: templating.containerOuter(config, this._direction, this._isSelectElement, isSelectOneElement, config.searchEnabled, elementType, config.labelId),
                classNames: classNames,
                type: elementType,
                position: position
            });
            this.containerInner = new Container({
                element: templating.containerInner(config),
                classNames: classNames,
                type: elementType,
                position: position
            });
            this.input = new Input({
                element: templating.input(config, this._placeholderValue),
                classNames: classNames,
                type: elementType,
                preventPaste: !config.paste
            });
            this.choiceList = new List({
                element: templating.choiceList(config, isSelectOneElement)
            });
            this.itemList = new List({
                element: templating.itemList(config, isSelectOneElement)
            });
            this.dropdown = new Dropdown({
                element: templating.dropdown(config),
                classNames: classNames,
                type: elementType
            });
        };
        Choices.prototype._createStructure = function() {
            var _a = this, containerInner = _a.containerInner, containerOuter = _a.containerOuter, passedElement = _a.passedElement;
            var dropdownElement = this.dropdown.element;
            passedElement.conceal();
            containerInner.wrap(passedElement.element);
            containerOuter.wrap(containerInner.element);
            if (this._isSelectOneElement) {
                this.input.placeholder = this.config.searchPlaceholderValue || "";
            } else {
                if (this._placeholderValue) {
                    this.input.placeholder = this._placeholderValue;
                }
                this.input.setWidth();
            }
            containerOuter.element.appendChild(containerInner.element);
            containerOuter.element.appendChild(dropdownElement);
            containerInner.element.appendChild(this.itemList.element);
            dropdownElement.appendChild(this.choiceList.element);
            if (!this._isSelectOneElement) {
                containerInner.element.appendChild(this.input.element);
            } else if (this.config.searchEnabled) {
                dropdownElement.insertBefore(this.input.element, dropdownElement.firstChild);
            }
            this._highlightPosition = 0;
            this._isSearching = false;
        };
        Choices.prototype._initStore = function() {
            var _this = this;
            this._store.subscribe(this._render).withTxn(function() {
                _this._addPredefinedChoices(_this._presetChoices, _this._isSelectOneElement && !_this._hasNonChoicePlaceholder, false);
            });
            if (!this._store.choices.length || this._isSelectOneElement && this._hasNonChoicePlaceholder) {
                this._render();
            }
        };
        Choices.prototype._addPredefinedChoices = function(choices, selectFirstOption, withEvents) {
            var _this = this;
            if (selectFirstOption === void 0) {
                selectFirstOption = false;
            }
            if (withEvents === void 0) {
                withEvents = true;
            }
            if (selectFirstOption) {
                var noSelectedChoices = choices.findIndex(function(choice) {
                    return choice.selected;
                }) === -1;
                if (noSelectedChoices) {
                    choices.some(function(choice) {
                        if (choice.disabled || "choices" in choice) {
                            return false;
                        }
                        choice.selected = true;
                        return true;
                    });
                }
            }
            choices.forEach(function(item) {
                if ("choices" in item) {
                    if (_this._isSelectElement) {
                        _this._addGroup(item, withEvents);
                    }
                } else {
                    _this._addChoice(item, withEvents);
                }
            });
        };
        Choices.prototype._findAndSelectChoiceByValue = function(value, userTriggered) {
            var _this = this;
            if (userTriggered === void 0) {
                userTriggered = false;
            }
            var foundChoice = this._store.choices.find(function(choice) {
                return _this.config.valueComparer(choice.value, value);
            });
            if (foundChoice && !foundChoice.disabled && !foundChoice.selected) {
                this._addItem(foundChoice, true, userTriggered);
                return true;
            }
            return false;
        };
        Choices.prototype._generatePlaceholderValue = function() {
            var config = this.config;
            if (!config.placeholder) {
                return null;
            }
            if (this._hasNonChoicePlaceholder) {
                return config.placeholderValue;
            }
            if (this._isSelectElement) {
                var placeholderOption = this.passedElement.placeholderOption;
                return placeholderOption ? placeholderOption.text : null;
            }
            return null;
        };
        Choices.prototype._warnChoicesInitFailed = function(caller) {
            if (this.config.silent) {
                return;
            }
            if (!this.initialised) {
                throw new TypeError("".concat(caller, " called on a non-initialised instance of Choices"));
            } else if (!this.initialisedOK) {
                throw new TypeError("".concat(caller, " called for an element which has multiple instances of Choices initialised on it"));
            }
        };
        Choices.version = "11.1.0";
        return Choices;
    }();
    return Choices;
});

!function() {
    "use strict";
    var t = {
        d: function(e, n) {
            for (var a in n) t.o(n, a) && !t.o(e, a) && Object.defineProperty(e, a, {
                enumerable: !0,
                get: n[a]
            });
        },
        o: function(t, e) {
            return Object.prototype.hasOwnProperty.call(t, e);
        }
    }, e = {};
    t.d(e, {
        default: function() {
            return d;
        }
    });
    var n = window.bootstrap;
    function a(t) {
        return a = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function(t) {
            return typeof t;
        } : function(t) {
            return t && "function" == typeof Symbol && t.constructor === Symbol && t !== Symbol.prototype ? "symbol" : typeof t;
        }, a(t);
    }
    function r(t) {
        return function(t) {
            if (Array.isArray(t)) return i(t);
        }(t) || function(t) {
            if ("undefined" != typeof Symbol && null != t[Symbol.iterator] || null != t["@@iterator"]) return Array.from(t);
        }(t) || o(t) || function() {
            throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.");
        }();
    }
    function o(t, e) {
        if (t) {
            if ("string" == typeof t) return i(t, e);
            var n = {}.toString.call(t).slice(8, -1);
            return "Object" === n && t.constructor && (n = t.constructor.name), 
            "Map" === n || "Set" === n ? Array.from(t) : "Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n) ? i(t, e) : void 0;
        }
    }
    function i(t, e) {
        (null == e || e > t.length) && (e = t.length);
        for (var n = 0, a = Array(e); n < e; n++) a[n] = t[n];
        return a;
    }
    function s(t, e) {
        for (var n = 0; n < e.length; n++) {
            var a = e[n];
            a.enumerable = a.enumerable || !1, a.configurable = !0, "value" in a && (a.writable = !0), 
            Object.defineProperty(t, l(a.key), a);
        }
    }
    function l(t) {
        var e = function(t, e) {
            if ("object" != a(t) || !t) return t;
            var n = t[Symbol.toPrimitive];
            if (void 0 !== n) {
                var r = n.call(t, e || "default");
                if ("object" != a(r)) return r;
                throw new TypeError("@@toPrimitive must return a primitive value.");
            }
            return ("string" === e ? String : Number)(t);
        }(t, "string");
        return "symbol" == a(e) ? e : e + "";
    }
    var c = {
        Modal: n.Modal,
        Carousel: n.Carousel
    }, u = function() {
        function t(e) {
            var n = this, a = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : {};
            !function(t, e) {
                if (!(t instanceof e)) throw new TypeError("Cannot call a class as a function");
            }(this, t), this.hash = this.randomHash(), this.settings = Object.assign(Object.assign(Object.assign({}, c.Modal.Default), c.Carousel.Default), {
                interval: !1,
                target: '[data-toggle="lightbox"]',
                gallery: "",
                size: "xl",
                constrain: !0
            }), this.settings = Object.assign(Object.assign({}, this.settings), a), 
            this.modalOptions = n.setOptionsFromSettings(c.Modal.Default), this.carouselOptions = n.setOptionsFromSettings(c.Carousel.Default), 
            "string" == typeof e && (this.settings.target = e, e = document.querySelector(this.settings.target)), 
            this.el = e, this.type = e.dataset.type || "", this.src = this.getSrc(e), 
            this.sources = this.getGalleryItems(), this.createCarousel(), this.createModal();
        }
        return e = t, n = [ {
            key: "show",
            value: function() {
                document.body.appendChild(this.modalElement), this.modal.show();
            }
        }, {
            key: "hide",
            value: function() {
                this.modal.hide();
            }
        }, {
            key: "setOptionsFromSettings",
            value: function(t) {
                var e = this;
                return Object.keys(t).reduce(function(t, n) {
                    return Object.assign(t, (a = {}, r = n, o = e.settings[n], (r = l(r)) in a ? Object.defineProperty(a, r, {
                        value: o,
                        enumerable: !0,
                        configurable: !0,
                        writable: !0
                    }) : a[r] = o, a));
                    var a, r, o;
                }, {});
            }
        }, {
            key: "getSrc",
            value: function(t) {
                var e = t.dataset.src || t.dataset.remote || t.href || "http://via.placeholder.com/1600x900";
                if ("html" === t.dataset.type) return e;
                /\:\/\//.test(e) || (e = window.location.origin + e);
                var n = new URL(e);
                return (t.dataset.footer || t.dataset.caption) && n.searchParams.set("caption", t.dataset.footer || t.dataset.caption), 
                n.toString();
            }
        }, {
            key: "getGalleryItems",
            value: function() {
                var t, e = this;
                if (this.settings.gallery) {
                    if (Array.isArray(this.settings.gallery)) return this.settings.gallery;
                    t = this.settings.gallery;
                } else this.el.dataset.gallery && (t = this.el.dataset.gallery);
                return t ? r(new Set(Array.from(document.querySelectorAll('[data-gallery="'.concat(t, '"]')), function(t) {
                    return "".concat(t.dataset.type ? t.dataset.type : "").concat(e.getSrc(t));
                }))) : [ "".concat(this.type ? this.type : "").concat(this.src) ];
            }
        }, {
            key: "getYoutubeId",
            value: function(t) {
                if (!t) return !1;
                var e = t.match(/^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/);
                return !(!e || 11 !== e[2].length) && e[2];
            }
        }, {
            key: "getYoutubeLink",
            value: function(t) {
                var e = this.getYoutubeId(t);
                if (!e) return !1;
                var n = t.split("?"), a = n.length > 1 ? "?" + n[1] : "";
                return "https://www.youtube.com/embed/".concat(e).concat(a);
            }
        }, {
            key: "getInstagramEmbed",
            value: function(t) {
                if (/instagram/.test(t)) return t += /\/embed$/.test(t) ? "" : "/embed", 
                '<iframe src="'.concat(t, '" class="start-50 translate-middle-x" style="max-width: 500px" frameborder="0" scrolling="no" allowtransparency="true"></iframe>');
            }
        }, {
            key: "isEmbed",
            value: function(e) {
                var n = new RegExp("(" + t.allowedEmbedTypes.join("|") + ")").test(e), a = /\.(png|jpe?g|gif|svg|webp)/i.test(e) || "image" === this.el.dataset.type;
                return n || !a;
            }
        }, {
            key: "createCarousel",
            value: function() {
                var e = this, n = document.createElement("template"), a = t.allowedMediaTypes.join("|"), r = this.sources.map(function(t, n) {
                    t = t.replace(/\/$/, "");
                    var r = new RegExp("^(".concat(a, ")"), "i"), o = /^html/.test(t), i = /^image/.test(t);
                    r.test(t) && (t = t.replace(r, ""));
                    var s = e.settings.constrain ? "mw-100 mh-100 h-auto w-auto m-auto top-0 end-0 bottom-0 start-0" : "h-100 w-100", l = new URLSearchParams(t.split("?")[1]), c = "", u = t;
                    if (l.get("caption")) {
                        try {
                            (u = new URL(t)).searchParams.delete("caption"), u = u.toString();
                        } catch (e) {
                            u = t;
                        }
                        c = '<div class="carousel-caption d-none d-md-block" style="z-index:2"><p class="bg-secondary rounded">'.concat(l.get("caption"), "</p></div>");
                    }
                    var d = '<img src="'.concat(u, '" class="d-block ').concat(s, ' img-fluid" style="z-index: 1; object-fit: contain;" />'), h = "", m = e.getInstagramEmbed(t), b = e.getYoutubeLink(t);
                    return e.isEmbed(t) && !i && (b && (t = b, h = 'title="YouTube video player" frameborder="0" allow="accelerometer autoplay clipboard-write encrypted-media gyroscope picture-in-picture"'), 
                    d = m || '<img src="'.concat(t, '" ').concat(h, ' class="d-block mw-100 mh-100 h-auto w-auto m-auto top-0 end-0 bottom-0 start-0 img-fluid" style="z-index: 1; object-fit: contain;" />')), 
                    o && (d = t), '\n\t\t\t\t<div class="carousel-item '.concat(n ? "" : "active", '" style="min-height: 100px">\n\t\t\t\t\t').concat('<div class="position-absolute top-50 start-50 translate-middle text-white"><div class="spinner-border" style="width: 3rem height: 3rem" role="status"></div></div>', '\n\t\t\t\t\t<div class="ratio ratio-16x9" style="background-color: #000;">').concat(d, "</div>\n\t\t\t\t\t").concat(c, "\n\t\t\t\t</div>");
                }).join(""), o = this.sources.length < 2 ? "" : '\n\t\t\t<button id="#lightboxCarousel-'.concat(this.hash, '-prev" class="carousel-control-prev" type="button" data-bs-target="#lightboxCarousel-').concat(this.hash, '" data-bs-slide="prev">\n\t\t\t\t<span class="btn btn-secondary carousel-control-prev-icon" aria-hidden="true"></span>\n\t\t\t\t<span class="visually-hidden">Previous</span>\n\t\t\t</button>\n\t\t\t<button id="#lightboxCarousel-').concat(this.hash, '-next" class="carousel-control-next" type="button" data-bs-target="#lightboxCarousel-').concat(this.hash, '" data-bs-slide="next">\n\t\t\t\t<span class="btn btn-secondary carousel-control-next-icon" aria-hidden="true"></span>\n\t\t\t\t<span class="visually-hidden">Next</span>\n\t\t\t</button>'), i = "lightbox-carousel carousel slide";
                "fullscreen" === this.settings.size && (i += " position-absolute w-100 translate-middle top-50 start-50");
                var s = '\n\t\t\t<div class="carousel-indicators" style="bottom: -40px">\n\t\t\t\t'.concat(this.sources.map(function(t, n) {
                    return '\n\t\t\t\t\t<button type="button" data-bs-target="#lightboxCarousel-'.concat(e.hash, '" data-bs-slide-to="').concat(n, '" class="').concat(0 === n ? "active" : "", '" aria-current="').concat(0 === n ? "true" : "false", '" aria-label="Slide ').concat(n + 1, '"></button>\n\t\t\t\t');
                }).join(""), "\n\t\t\t</div>"), l = '\n\t\t\t<div id="lightboxCarousel-'.concat(this.hash, '" class="').concat(i, '" data-bs-ride="carousel" data-bs-interval="').concat(this.carouselOptions.interval, '">\n\t\t\t    <div class="carousel-inner">\n\t\t\t\t\t').concat(r, "\n\t\t\t\t</div>\n\t\t\t    ").concat(s, "\n\t\t\t\t").concat(o, "\n\t\t\t</div>");
                n.innerHTML = l.trim(), this.carouselElement = n.content.firstChild;
                var u = Object.assign(Object.assign({}, this.carouselOptions), {
                    keyboard: !1
                });
                this.carousel = new c.Carousel(this.carouselElement, u);
                var d = this.type && "image" !== this.type ? this.type + this.src : this.src;
                return this.carousel.to(this.findGalleryItemIndex(this.sources, d)), 
                !0 === this.carouselOptions.keyboard && document.addEventListener("keydown", function(t) {
                    if ("ArrowLeft" === t.code) {
                        var n = document.getElementById("#lightboxCarousel-".concat(e.hash, "-prev"));
                        return n && n.click(), !1;
                    }
                    if ("ArrowRight" === t.code) {
                        var a = document.getElementById("#lightboxCarousel-".concat(e.hash, "-next"));
                        return a && a.click(), !1;
                    }
                }), this.carousel;
            }
        }, {
            key: "findGalleryItemIndex",
            value: function(t, e) {
                var n, a = 0, r = function(t, e) {
                    var n = "undefined" != typeof Symbol && t[Symbol.iterator] || t["@@iterator"];
                    if (!n) {
                        if (Array.isArray(t) || (n = o(t)) || e && t && "number" == typeof t.length) {
                            n && (t = n);
                            var a = 0, r = function() {};
                            return {
                                s: r,
                                n: function() {
                                    return a >= t.length ? {
                                        done: !0
                                    } : {
                                        done: !1,
                                        value: t[a++]
                                    };
                                },
                                e: function(t) {
                                    throw t;
                                },
                                f: r
                            };
                        }
                        throw new TypeError("Invalid attempt to iterate non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.");
                    }
                    var i, s = !0, l = !1;
                    return {
                        s: function() {
                            n = n.call(t);
                        },
                        n: function() {
                            var t = n.next();
                            return s = t.done, t;
                        },
                        e: function(t) {
                            l = !0, i = t;
                        },
                        f: function() {
                            try {
                                s || null == n.return || n.return();
                            } finally {
                                if (l) throw i;
                            }
                        }
                    };
                }(t);
                try {
                    for (r.s(); !(n = r.n()).done; ) {
                        if (n.value.includes(e)) return a;
                        a++;
                    }
                } catch (t) {
                    r.e(t);
                } finally {
                    r.f();
                }
                return 0;
            }
        }, {
            key: "createModal",
            value: function() {
                var t = this, e = document.createElement("template"), n = '\n\t\t\t<div class="modal lightbox fade" id="lightboxModal-'.concat(this.hash, '" tabindex="-1" aria-hidden="true">\n\t\t\t\t<div class="modal-dialog modal-dialog-centered modal-').concat(this.settings.size, '">\n\t\t\t\t\t<div class="modal-content border-0 bg-transparent">\n\t\t\t\t\t\t<div class="modal-body p-0">\n\t\t\t\t\t\t\t<button type="button" class="btn-close position-absolute p-3" data-bs-dismiss="modal" aria-label="Close" style="top: -15px;right:-40px"></button>\n\t\t\t\t\t\t</div>\n\t\t\t\t\t</div>\n\t\t\t\t</div>\n\t\t\t</div>');
                return e.innerHTML = n.trim(), this.modalElement = e.content.firstChild, 
                this.modalElement.querySelector(".modal-body").appendChild(this.carouselElement), 
                this.modalElement.addEventListener("hidden.bs.modal", function() {
                    return t.modalElement.remove();
                }), this.modalElement.querySelector("[data-bs-dismiss]").addEventListener("click", function() {
                    return t.modal.hide();
                }), this.modal = new c.Modal(this.modalElement, this.modalOptions), 
                this.modal;
            }
        }, {
            key: "randomHash",
            value: function() {
                var t = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : 8;
                return Array.from({
                    length: t
                }, function() {
                    return Math.floor(36 * Math.random()).toString(36);
                }).join("");
            }
        } ], n && s(e.prototype, n), a && s(e, a), Object.defineProperty(e, "prototype", {
            writable: !1
        }), e;
        var e, n, a;
    }();
    u.allowedEmbedTypes = [ "embed", "youtube", "vimeo", "instagram", "url" ], u.allowedMediaTypes = [].concat(r(u.allowedEmbedTypes), [ "image", "html" ]), 
    u.defaultSelector = '[data-toggle="lightbox"]', u.initialize = function(t) {
        t.preventDefault(), new u(this).show();
    }, document.querySelectorAll(u.defaultSelector).forEach(function(t) {
        return t.addEventListener("click", u.initialize);
    }), "undefined" != typeof window && window.bootstrap && (window.bootstrap.Lightbox = u);
    var d = u;
    window.Lightbox = e.default;
}();

function userCardContent(pop, delay) {
    const popover = new bootstrap.Popover(pop, {
        delay: {
            show: delay,
            hide: 100
        },
        trigger: "hover",
        html: true,
        content: "&nbsp;",
        template: '<div class="popover" role="tooltip"><div class="popover-arrow"></div><div class="popover-body p-2"></div></div>'
    });
    pop.addEventListener("show.bs.popover", () => {
        if (popover._newContent != undefined) {
            return;
        }
        fetch(pop.dataset.hovercard, {
            method: "GET",
            headers: {
                Accept: "application/json",
                "Content-Type": "application/json;charset=utf-8"
            }
        }).then(res => res.json()).then(profileData => {
            var content = (profileData.Avatar ? `<img src="${profileData.Avatar}" class="rounded mx-auto d-block" style="width:75px" alt="" />` : "") + '<ul class="list-unstyled m-0">' + (profileData.Location ? `<li class="px-2 py-1"><i class="fas fa-home me-1"></i>${profileData.Location}</li>` : "") + (profileData.Rank ? `<li class="px-2 py-1"><i class="fas fa-graduation-cap me-1"></i>${profileData.Rank}</li>` : "") + (profileData.Interests ? `<li class="px-2 py-1"><i class="fas fa-running me-1"></i>${profileData.Interests}</li>` : "") + (profileData.Joined ? `<li class="px-2 py-1"><i class="fas fa-user-check me-1"></i>${profileData.Joined}</li>` : "") + (profileData.HomePage ? `<li class="px-2 py-1"><i class="fas fa-globe me-1"></i><a href="${profileData.HomePage}" target="_blank">${profileData.HomePage}</a></li>` : "") + '<li class="px-2 py-1"><i class="far fa-comment me-1"></i>' + profileData.Posts + "</li>" + "</ul>";
            popover.setContent({
                ".popover-body": content
            });
        }).catch(function(error) {
            console.log(error);
        });
    });
}

var _self = "undefined" != typeof window ? window : "undefined" != typeof WorkerGlobalScope && self instanceof WorkerGlobalScope ? self : {}, Prism = function(e) {
    var n = /(?:^|\s)lang(?:uage)?-([\w-]+)(?=\s|$)/i, t = 0, r = {}, a = {
        manual: e.Prism && e.Prism.manual,
        disableWorkerMessageHandler: e.Prism && e.Prism.disableWorkerMessageHandler,
        util: {
            encode: function e(n) {
                return n instanceof i ? new i(n.type, e(n.content), n.alias) : Array.isArray(n) ? n.map(e) : n.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/\u00a0/g, " ");
            },
            type: function(e) {
                return Object.prototype.toString.call(e).slice(8, -1);
            },
            objId: function(e) {
                return e.__id || Object.defineProperty(e, "__id", {
                    value: ++t
                }), e.__id;
            },
            clone: function e(n, t) {
                var r, i;
                switch (t = t || {}, a.util.type(n)) {
                  case "Object":
                    if (i = a.util.objId(n), t[i]) return t[i];
                    for (var l in r = {}, t[i] = r, n) n.hasOwnProperty(l) && (r[l] = e(n[l], t));
                    return r;

                  case "Array":
                    return i = a.util.objId(n), t[i] ? t[i] : (r = [], t[i] = r, 
                    n.forEach(function(n, a) {
                        r[a] = e(n, t);
                    }), r);

                  default:
                    return n;
                }
            },
            getLanguage: function(e) {
                for (;e; ) {
                    var t = n.exec(e.className);
                    if (t) return t[1].toLowerCase();
                    e = e.parentElement;
                }
                return "none";
            },
            setLanguage: function(e, t) {
                e.className = e.className.replace(RegExp(n, "gi"), ""), e.classList.add("language-" + t);
            },
            currentScript: function() {
                if ("undefined" == typeof document) return null;
                if (document.currentScript && "SCRIPT" === document.currentScript.tagName) return document.currentScript;
                try {
                    throw new Error();
                } catch (r) {
                    var e = (/at [^(\r\n]*\((.*):[^:]+:[^:]+\)$/i.exec(r.stack) || [])[1];
                    if (e) {
                        var n = document.getElementsByTagName("script");
                        for (var t in n) if (n[t].src == e) return n[t];
                    }
                    return null;
                }
            },
            isActive: function(e, n, t) {
                for (var r = "no-" + n; e; ) {
                    var a = e.classList;
                    if (a.contains(n)) return !0;
                    if (a.contains(r)) return !1;
                    e = e.parentElement;
                }
                return !!t;
            }
        },
        languages: {
            plain: r,
            plaintext: r,
            text: r,
            txt: r,
            extend: function(e, n) {
                var t = a.util.clone(a.languages[e]);
                for (var r in n) t[r] = n[r];
                return t;
            },
            insertBefore: function(e, n, t, r) {
                var i = (r = r || a.languages)[e], l = {};
                for (var o in i) if (i.hasOwnProperty(o)) {
                    if (o == n) for (var s in t) t.hasOwnProperty(s) && (l[s] = t[s]);
                    t.hasOwnProperty(o) || (l[o] = i[o]);
                }
                var u = r[e];
                return r[e] = l, a.languages.DFS(a.languages, function(n, t) {
                    t === u && n != e && (this[n] = l);
                }), l;
            },
            DFS: function e(n, t, r, i) {
                i = i || {};
                var l = a.util.objId;
                for (var o in n) if (n.hasOwnProperty(o)) {
                    t.call(n, o, n[o], r || o);
                    var s = n[o], u = a.util.type(s);
                    "Object" !== u || i[l(s)] ? "Array" !== u || i[l(s)] || (i[l(s)] = !0, 
                    e(s, t, o, i)) : (i[l(s)] = !0, e(s, t, null, i));
                }
            }
        },
        plugins: {},
        highlightAll: function(e, n) {
            a.highlightAllUnder(document, e, n);
        },
        highlightAllUnder: function(e, n, t) {
            var r = {
                callback: t,
                container: e,
                selector: 'code[class*="language-"], [class*="language-"] code, code[class*="lang-"], [class*="lang-"] code'
            };
            a.hooks.run("before-highlightall", r), r.elements = Array.prototype.slice.apply(r.container.querySelectorAll(r.selector)), 
            a.hooks.run("before-all-elements-highlight", r);
            for (var i, l = 0; i = r.elements[l++]; ) a.highlightElement(i, !0 === n, r.callback);
        },
        highlightElement: function(n, t, r) {
            var i = a.util.getLanguage(n), l = a.languages[i];
            a.util.setLanguage(n, i);
            var o = n.parentElement;
            o && "pre" === o.nodeName.toLowerCase() && a.util.setLanguage(o, i);
            var s = {
                element: n,
                language: i,
                grammar: l,
                code: n.textContent
            };
            function u(e) {
                s.highlightedCode = e, a.hooks.run("before-insert", s), s.element.innerHTML = s.highlightedCode, 
                a.hooks.run("after-highlight", s), a.hooks.run("complete", s), r && r.call(s.element);
            }
            if (a.hooks.run("before-sanity-check", s), (o = s.element.parentElement) && "pre" === o.nodeName.toLowerCase() && !o.hasAttribute("tabindex") && o.setAttribute("tabindex", "0"), 
            !s.code) return a.hooks.run("complete", s), void (r && r.call(s.element));
            if (a.hooks.run("before-highlight", s), s.grammar) if (t && e.Worker) {
                var c = new Worker(a.filename);
                c.onmessage = function(e) {
                    u(e.data);
                }, c.postMessage(JSON.stringify({
                    language: s.language,
                    code: s.code,
                    immediateClose: !0
                }));
            } else u(a.highlight(s.code, s.grammar, s.language)); else u(a.util.encode(s.code));
        },
        highlight: function(e, n, t) {
            var r = {
                code: e,
                grammar: n,
                language: t
            };
            if (a.hooks.run("before-tokenize", r), !r.grammar) throw new Error('The language "' + r.language + '" has no grammar.');
            return r.tokens = a.tokenize(r.code, r.grammar), a.hooks.run("after-tokenize", r), 
            i.stringify(a.util.encode(r.tokens), r.language);
        },
        tokenize: function(e, n) {
            var t = n.rest;
            if (t) {
                for (var r in t) n[r] = t[r];
                delete n.rest;
            }
            var a = new s();
            return u(a, a.head, e), o(e, a, n, a.head, 0), function(e) {
                for (var n = [], t = e.head.next; t !== e.tail; ) n.push(t.value), 
                t = t.next;
                return n;
            }(a);
        },
        hooks: {
            all: {},
            add: function(e, n) {
                var t = a.hooks.all;
                t[e] = t[e] || [], t[e].push(n);
            },
            run: function(e, n) {
                var t = a.hooks.all[e];
                if (t && t.length) for (var r, i = 0; r = t[i++]; ) r(n);
            }
        },
        Token: i
    };
    function i(e, n, t, r) {
        this.type = e, this.content = n, this.alias = t, this.length = 0 | (r || "").length;
    }
    function l(e, n, t, r) {
        e.lastIndex = n;
        var a = e.exec(t);
        if (a && r && a[1]) {
            var i = a[1].length;
            a.index += i, a[0] = a[0].slice(i);
        }
        return a;
    }
    function o(e, n, t, r, s, g) {
        for (var f in t) if (t.hasOwnProperty(f) && t[f]) {
            var h = t[f];
            h = Array.isArray(h) ? h : [ h ];
            for (var d = 0; d < h.length; ++d) {
                if (g && g.cause == f + "," + d) return;
                var v = h[d], p = v.inside, m = !!v.lookbehind, y = !!v.greedy, k = v.alias;
                if (y && !v.pattern.global) {
                    var x = v.pattern.toString().match(/[imsuy]*$/)[0];
                    v.pattern = RegExp(v.pattern.source, x + "g");
                }
                for (var b = v.pattern || v, w = r.next, A = s; w !== n.tail && !(g && A >= g.reach); A += w.value.length, 
                w = w.next) {
                    var P = w.value;
                    if (n.length > e.length) return;
                    if (!(P instanceof i)) {
                        var E, S = 1;
                        if (y) {
                            if (!(E = l(b, A, e, m)) || E.index >= e.length) break;
                            var L = E.index, O = E.index + E[0].length, C = A;
                            for (C += w.value.length; L >= C; ) C += (w = w.next).value.length;
                            if (A = C -= w.value.length, w.value instanceof i) continue;
                            for (var j = w; j !== n.tail && (C < O || "string" == typeof j.value); j = j.next) S++, 
                            C += j.value.length;
                            S--, P = e.slice(A, C), E.index -= A;
                        } else if (!(E = l(b, 0, P, m))) continue;
                        L = E.index;
                        var N = E[0], _ = P.slice(0, L), M = P.slice(L + N.length), W = A + P.length;
                        g && W > g.reach && (g.reach = W);
                        var I = w.prev;
                        if (_ && (I = u(n, I, _), A += _.length), c(n, I, S), w = u(n, I, new i(f, p ? a.tokenize(N, p) : N, k, N)), 
                        M && u(n, w, M), S > 1) {
                            var T = {
                                cause: f + "," + d,
                                reach: W
                            };
                            o(e, n, t, w.prev, A, T), g && T.reach > g.reach && (g.reach = T.reach);
                        }
                    }
                }
            }
        }
    }
    function s() {
        var e = {
            value: null,
            prev: null,
            next: null
        }, n = {
            value: null,
            prev: e,
            next: null
        };
        e.next = n, this.head = e, this.tail = n, this.length = 0;
    }
    function u(e, n, t) {
        var r = n.next, a = {
            value: t,
            prev: n,
            next: r
        };
        return n.next = a, r.prev = a, e.length++, a;
    }
    function c(e, n, t) {
        for (var r = n.next, a = 0; a < t && r !== e.tail; a++) r = r.next;
        n.next = r, r.prev = n, e.length -= a;
    }
    if (e.Prism = a, i.stringify = function e(n, t) {
        if ("string" == typeof n) return n;
        if (Array.isArray(n)) {
            var r = "";
            return n.forEach(function(n) {
                r += e(n, t);
            }), r;
        }
        var i = {
            type: n.type,
            content: e(n.content, t),
            tag: "span",
            classes: [ "token", n.type ],
            attributes: {},
            language: t
        }, l = n.alias;
        l && (Array.isArray(l) ? Array.prototype.push.apply(i.classes, l) : i.classes.push(l)), 
        a.hooks.run("wrap", i);
        var o = "";
        for (var s in i.attributes) o += " " + s + '="' + (i.attributes[s] || "").replace(/"/g, "&quot;") + '"';
        return "<" + i.tag + ' class="' + i.classes.join(" ") + '"' + o + ">" + i.content + "</" + i.tag + ">";
    }, !e.document) return e.addEventListener ? (a.disableWorkerMessageHandler || e.addEventListener("message", function(n) {
        var t = JSON.parse(n.data), r = t.language, i = t.code, l = t.immediateClose;
        e.postMessage(a.highlight(i, a.languages[r], r)), l && e.close();
    }, !1), a) : a;
    var g = a.util.currentScript();
    function f() {
        a.manual || a.highlightAll();
    }
    if (g && (a.filename = g.src, g.hasAttribute("data-manual") && (a.manual = !0)), 
    !a.manual) {
        var h = document.readyState;
        "loading" === h || "interactive" === h && g && g.defer ? document.addEventListener("DOMContentLoaded", f) : window.requestAnimationFrame ? window.requestAnimationFrame(f) : window.setTimeout(f, 16);
    }
    return a;
}(_self);

"undefined" != typeof module && module.exports && (module.exports = Prism), "undefined" != typeof global && (global.Prism = Prism);

Prism.languages.markup = {
    comment: {
        pattern: /<!--(?:(?!<!--)[\s\S])*?-->/,
        greedy: !0
    },
    prolog: {
        pattern: /<\?[\s\S]+?\?>/,
        greedy: !0
    },
    doctype: {
        pattern: /<!DOCTYPE(?:[^>"'[\]]|"[^"]*"|'[^']*')+(?:\[(?:[^<"'\]]|"[^"]*"|'[^']*'|<(?!!--)|<!--(?:[^-]|-(?!->))*-->)*\]\s*)?>/i,
        greedy: !0,
        inside: {
            "internal-subset": {
                pattern: /(^[^\[]*\[)[\s\S]+(?=\]>$)/,
                lookbehind: !0,
                greedy: !0,
                inside: null
            },
            string: {
                pattern: /"[^"]*"|'[^']*'/,
                greedy: !0
            },
            punctuation: /^<!|>$|[[\]]/,
            "doctype-tag": /^DOCTYPE/i,
            name: /[^\s<>'"]+/
        }
    },
    cdata: {
        pattern: /<!\[CDATA\[[\s\S]*?\]\]>/i,
        greedy: !0
    },
    tag: {
        pattern: /<\/?(?!\d)[^\s>\/=$<%]+(?:\s(?:\s*[^\s>\/=]+(?:\s*=\s*(?:"[^"]*"|'[^']*'|[^\s'">=]+(?=[\s>]))|(?=[\s/>])))+)?\s*\/?>/,
        greedy: !0,
        inside: {
            tag: {
                pattern: /^<\/?[^\s>\/]+/,
                inside: {
                    punctuation: /^<\/?/,
                    namespace: /^[^\s>\/:]+:/
                }
            },
            "special-attr": [],
            "attr-value": {
                pattern: /=\s*(?:"[^"]*"|'[^']*'|[^\s'">=]+)/,
                inside: {
                    punctuation: [ {
                        pattern: /^=/,
                        alias: "attr-equals"
                    }, {
                        pattern: /^(\s*)["']|["']$/,
                        lookbehind: !0
                    } ]
                }
            },
            punctuation: /\/?>/,
            "attr-name": {
                pattern: /[^\s>\/]+/,
                inside: {
                    namespace: /^[^\s>\/:]+:/
                }
            }
        }
    },
    entity: [ {
        pattern: /&[\da-z]{1,8};/i,
        alias: "named-entity"
    }, /&#x?[\da-f]{1,8};/i ]
}, Prism.languages.markup.tag.inside["attr-value"].inside.entity = Prism.languages.markup.entity, 
Prism.languages.markup.doctype.inside["internal-subset"].inside = Prism.languages.markup, 
Prism.hooks.add("wrap", function(a) {
    "entity" === a.type && (a.attributes.title = a.content.replace(/&amp;/, "&"));
}), Object.defineProperty(Prism.languages.markup.tag, "addInlined", {
    value: function(a, e) {
        var s = {};
        s["language-" + e] = {
            pattern: /(^<!\[CDATA\[)[\s\S]+?(?=\]\]>$)/i,
            lookbehind: !0,
            inside: Prism.languages[e]
        }, s.cdata = /^<!\[CDATA\[|\]\]>$/i;
        var t = {
            "included-cdata": {
                pattern: /<!\[CDATA\[[\s\S]*?\]\]>/i,
                inside: s
            }
        };
        t["language-" + e] = {
            pattern: /[\s\S]+/,
            inside: Prism.languages[e]
        };
        var n = {};
        n[a] = {
            pattern: RegExp("(<__[^>]*>)(?:<!\\[CDATA\\[(?:[^\\]]|\\](?!\\]>))*\\]\\]>|(?!<!\\[CDATA\\[)[^])*?(?=</__>)".replace(/__/g, function() {
                return a;
            }), "i"),
            lookbehind: !0,
            greedy: !0,
            inside: t
        }, Prism.languages.insertBefore("markup", "cdata", n);
    }
}), Object.defineProperty(Prism.languages.markup.tag, "addAttribute", {
    value: function(a, e) {
        Prism.languages.markup.tag.inside["special-attr"].push({
            pattern: RegExp("(^|[\"'\\s])(?:" + a + ")\\s*=\\s*(?:\"[^\"]*\"|'[^']*'|[^\\s'\">=]+(?=[\\s>]))", "i"),
            lookbehind: !0,
            inside: {
                "attr-name": /^[^\s=]+/,
                "attr-value": {
                    pattern: /=[\s\S]+/,
                    inside: {
                        value: {
                            pattern: /(^=\s*(["']|(?!["'])))\S[\s\S]*(?=\2$)/,
                            lookbehind: !0,
                            alias: [ e, "language-" + e ],
                            inside: Prism.languages[e]
                        },
                        punctuation: [ {
                            pattern: /^=/,
                            alias: "attr-equals"
                        }, /"|'/ ]
                    }
                }
            }
        });
    }
}), Prism.languages.html = Prism.languages.markup, Prism.languages.mathml = Prism.languages.markup, 
Prism.languages.svg = Prism.languages.markup, Prism.languages.xml = Prism.languages.extend("markup", {}), 
Prism.languages.ssml = Prism.languages.xml, Prism.languages.atom = Prism.languages.xml, 
Prism.languages.rss = Prism.languages.xml;

!function(s) {
    var e = /(?:"(?:\\(?:\r\n|[\s\S])|[^"\\\r\n])*"|'(?:\\(?:\r\n|[\s\S])|[^'\\\r\n])*')/;
    s.languages.css = {
        comment: /\/\*[\s\S]*?\*\//,
        atrule: {
            pattern: RegExp("@[\\w-](?:[^;{\\s\"']|\\s+(?!\\s)|" + e.source + ")*?(?:;|(?=\\s*\\{))"),
            inside: {
                rule: /^@[\w-]+/,
                "selector-function-argument": {
                    pattern: /(\bselector\s*\(\s*(?![\s)]))(?:[^()\s]|\s+(?![\s)])|\((?:[^()]|\([^()]*\))*\))+(?=\s*\))/,
                    lookbehind: !0,
                    alias: "selector"
                },
                keyword: {
                    pattern: /(^|[^\w-])(?:and|not|only|or)(?![\w-])/,
                    lookbehind: !0
                }
            }
        },
        url: {
            pattern: RegExp("\\burl\\((?:" + e.source + "|(?:[^\\\\\r\n()\"']|\\\\[^])*)\\)", "i"),
            greedy: !0,
            inside: {
                function: /^url/i,
                punctuation: /^\(|\)$/,
                string: {
                    pattern: RegExp("^" + e.source + "$"),
                    alias: "url"
                }
            }
        },
        selector: {
            pattern: RegExp("(^|[{}\\s])[^{}\\s](?:[^{};\"'\\s]|\\s+(?![\\s{])|" + e.source + ")*(?=\\s*\\{)"),
            lookbehind: !0
        },
        string: {
            pattern: e,
            greedy: !0
        },
        property: {
            pattern: /(^|[^-\w\xA0-\uFFFF])(?!\s)[-_a-z\xA0-\uFFFF](?:(?!\s)[-\w\xA0-\uFFFF])*(?=\s*:)/i,
            lookbehind: !0
        },
        important: /!important\b/i,
        function: {
            pattern: /(^|[^-a-z0-9])[-a-z0-9]+(?=\()/i,
            lookbehind: !0
        },
        punctuation: /[(){};:,]/
    }, s.languages.css.atrule.inside.rest = s.languages.css;
    var t = s.languages.markup;
    t && (t.tag.addInlined("style", "css"), t.tag.addAttribute("style", "css"));
}(Prism);

Prism.languages.clike = {
    comment: [ {
        pattern: /(^|[^\\])\/\*[\s\S]*?(?:\*\/|$)/,
        lookbehind: !0,
        greedy: !0
    }, {
        pattern: /(^|[^\\:])\/\/.*/,
        lookbehind: !0,
        greedy: !0
    } ],
    string: {
        pattern: /(["'])(?:\\(?:\r\n|[\s\S])|(?!\1)[^\\\r\n])*\1/,
        greedy: !0
    },
    "class-name": {
        pattern: /(\b(?:class|extends|implements|instanceof|interface|new|trait)\s+|\bcatch\s+\()[\w.\\]+/i,
        lookbehind: !0,
        inside: {
            punctuation: /[.\\]/
        }
    },
    keyword: /\b(?:break|catch|continue|do|else|finally|for|function|if|in|instanceof|new|null|return|throw|try|while)\b/,
    boolean: /\b(?:false|true)\b/,
    function: /\b\w+(?=\()/,
    number: /\b0x[\da-f]+\b|(?:\b\d+(?:\.\d*)?|\B\.\d+)(?:e[+-]?\d+)?/i,
    operator: /[<>]=?|[!=]=?=?|--?|\+\+?|&&?|\|\|?|[?*/~^%]/,
    punctuation: /[{}[\];(),.:]/
};

Prism.languages.javascript = Prism.languages.extend("clike", {
    "class-name": [ Prism.languages.clike["class-name"], {
        pattern: /(^|[^$\w\xA0-\uFFFF])(?!\s)[_$A-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\.(?:constructor|prototype))/,
        lookbehind: !0
    } ],
    keyword: [ {
        pattern: /((?:^|\})\s*)catch\b/,
        lookbehind: !0
    }, {
        pattern: /(^|[^.]|\.\.\.\s*)\b(?:as|assert(?=\s*\{)|async(?=\s*(?:function\b|\(|[$\w\xA0-\uFFFF]|$))|await|break|case|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally(?=\s*(?:\{|$))|for|from(?=\s*(?:['"]|$))|function|(?:get|set)(?=\s*(?:[#\[$\w\xA0-\uFFFF]|$))|if|implements|import|in|instanceof|interface|let|new|null|of|package|private|protected|public|return|static|super|switch|this|throw|try|typeof|undefined|var|void|while|with|yield)\b/,
        lookbehind: !0
    } ],
    function: /#?(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*(?:\.\s*(?:apply|bind|call)\s*)?\()/,
    number: {
        pattern: RegExp("(^|[^\\w$])(?:NaN|Infinity|0[bB][01]+(?:_[01]+)*n?|0[oO][0-7]+(?:_[0-7]+)*n?|0[xX][\\dA-Fa-f]+(?:_[\\dA-Fa-f]+)*n?|\\d+(?:_\\d+)*n|(?:\\d+(?:_\\d+)*(?:\\.(?:\\d+(?:_\\d+)*)?)?|\\.\\d+(?:_\\d+)*)(?:[Ee][+-]?\\d+(?:_\\d+)*)?)(?![\\w$])"),
        lookbehind: !0
    },
    operator: /--|\+\+|\*\*=?|=>|&&=?|\|\|=?|[!=]==|<<=?|>>>?=?|[-+*/%&|^!=<>]=?|\.{3}|\?\?=?|\?\.?|[~:]/
}), Prism.languages.javascript["class-name"][0].pattern = /(\b(?:class|extends|implements|instanceof|interface|new)\s+)[\w.\\]+/, 
Prism.languages.insertBefore("javascript", "keyword", {
    regex: {
        pattern: RegExp("((?:^|[^$\\w\\xA0-\\uFFFF.\"'\\])\\s]|\\b(?:return|yield))\\s*)/(?:(?:\\[(?:[^\\]\\\\\r\n]|\\\\.)*\\]|\\\\.|[^/\\\\\\[\r\n])+/[dgimyus]{0,7}|(?:\\[(?:[^[\\]\\\\\r\n]|\\\\.|\\[(?:[^[\\]\\\\\r\n]|\\\\.|\\[(?:[^[\\]\\\\\r\n]|\\\\.)*\\])*\\])*\\]|\\\\.|[^/\\\\\\[\r\n])+/[dgimyus]{0,7}v[dgimyus]{0,7})(?=(?:\\s|/\\*(?:[^*]|\\*(?!/))*\\*/)*(?:$|[\r\n,.;:})\\]]|//))"),
        lookbehind: !0,
        greedy: !0,
        inside: {
            "regex-source": {
                pattern: /^(\/)[\s\S]+(?=\/[a-z]*$)/,
                lookbehind: !0,
                alias: "language-regex",
                inside: Prism.languages.regex
            },
            "regex-delimiter": /^\/|\/$/,
            "regex-flags": /^[a-z]+$/
        }
    },
    "function-variable": {
        pattern: /#?(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*[=:]\s*(?:async\s*)?(?:\bfunction\b|(?:\((?:[^()]|\([^()]*\))*\)|(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*)\s*=>))/,
        alias: "function"
    },
    parameter: [ {
        pattern: /(function(?:\s+(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*)?\s*\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\))/,
        lookbehind: !0,
        inside: Prism.languages.javascript
    }, {
        pattern: /(^|[^$\w\xA0-\uFFFF])(?!\s)[_$a-z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*=>)/i,
        lookbehind: !0,
        inside: Prism.languages.javascript
    }, {
        pattern: /(\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\)\s*=>)/,
        lookbehind: !0,
        inside: Prism.languages.javascript
    }, {
        pattern: /((?:\b|\s|^)(?!(?:as|async|await|break|case|catch|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally|for|from|function|get|if|implements|import|in|instanceof|interface|let|new|null|of|package|private|protected|public|return|set|static|super|switch|this|throw|try|typeof|undefined|var|void|while|with|yield)(?![$\w\xA0-\uFFFF]))(?:(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*\s*)\(\s*|\]\s*\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\)\s*\{)/,
        lookbehind: !0,
        inside: Prism.languages.javascript
    } ],
    constant: /\b[A-Z](?:[A-Z_]|\dx?)*\b/
}), Prism.languages.insertBefore("javascript", "string", {
    hashbang: {
        pattern: /^#!.*/,
        greedy: !0,
        alias: "comment"
    },
    "template-string": {
        pattern: /`(?:\\[\s\S]|\$\{(?:[^{}]|\{(?:[^{}]|\{[^}]*\})*\})+\}|(?!\$\{)[^\\`])*`/,
        greedy: !0,
        inside: {
            "template-punctuation": {
                pattern: /^`|`$/,
                alias: "string"
            },
            interpolation: {
                pattern: /((?:^|[^\\])(?:\\{2})*)\$\{(?:[^{}]|\{(?:[^{}]|\{[^}]*\})*\})+\}/,
                lookbehind: !0,
                inside: {
                    "interpolation-punctuation": {
                        pattern: /^\$\{|\}$/,
                        alias: "punctuation"
                    },
                    rest: Prism.languages.javascript
                }
            },
            string: /[\s\S]+/
        }
    },
    "string-property": {
        pattern: /((?:^|[,{])[ \t]*)(["'])(?:\\(?:\r\n|[\s\S])|(?!\2)[^\\\r\n])*\2(?=\s*:)/m,
        lookbehind: !0,
        greedy: !0,
        alias: "property"
    }
}), Prism.languages.insertBefore("javascript", "operator", {
    "literal-property": {
        pattern: /((?:^|[,{])[ \t]*)(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*:)/m,
        lookbehind: !0,
        alias: "property"
    }
}), Prism.languages.markup && (Prism.languages.markup.tag.addInlined("script", "javascript"), 
Prism.languages.markup.tag.addAttribute("on(?:abort|blur|change|click|composition(?:end|start|update)|dblclick|error|focus(?:in|out)?|key(?:down|up)|load|mouse(?:down|enter|leave|move|out|over|up)|reset|resize|scroll|select|slotchange|submit|unload|wheel)", "javascript")), 
Prism.languages.js = Prism.languages.javascript;

(function(window, document) {
    "use strict";
    var timer = null;
    var hasPointerEvents = "PointerEvent" in window || window.navigator && "msPointerEnabled" in window.navigator;
    var isTouch = "ontouchstart" in window || navigator.MaxTouchPoints > 0 || navigator.msMaxTouchPoints > 0;
    var mouseDown = hasPointerEvents ? "pointerdown" : isTouch ? "touchstart" : "mousedown";
    var mouseUp = hasPointerEvents ? "pointerup" : isTouch ? "touchend" : "mouseup";
    var mouseMove = hasPointerEvents ? "pointermove" : isTouch ? "touchmove" : "mousemove";
    var mouseLeave = hasPointerEvents ? "pointerleave" : isTouch ? "touchleave" : "mouseleave";
    var startX = 0;
    var startY = 0;
    var maxDiffX = 10;
    var maxDiffY = 10;
    if (typeof window.CustomEvent !== "function") {
        window.CustomEvent = function(event, params) {
            params = params || {
                bubbles: false,
                cancelable: false,
                detail: undefined
            };
            var evt = document.createEvent("CustomEvent");
            evt.initCustomEvent(event, params.bubbles, params.cancelable, params.detail);
            return evt;
        };
        window.CustomEvent.prototype = window.Event.prototype;
    }
    window.requestAnimFrame = function() {
        return window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.oRequestAnimationFrame || window.msRequestAnimationFrame || function(callback) {
            window.setTimeout(callback, 1e3 / 60);
        };
    }();
    function requestTimeout(fn, delay) {
        if (!window.requestAnimationFrame && !window.webkitRequestAnimationFrame && !(window.mozRequestAnimationFrame && window.mozCancelRequestAnimationFrame) && !window.oRequestAnimationFrame && !window.msRequestAnimationFrame) return window.setTimeout(fn, delay);
        var start = new Date().getTime();
        var handle = {};
        var loop = function() {
            var current = new Date().getTime();
            var delta = current - start;
            if (delta >= delay) {
                fn.call();
            } else {
                handle.value = requestAnimFrame(loop);
            }
        };
        handle.value = requestAnimFrame(loop);
        return handle;
    }
    function clearRequestTimeout(handle) {
        if (handle) {
            window.cancelAnimationFrame ? window.cancelAnimationFrame(handle.value) : window.webkitCancelAnimationFrame ? window.webkitCancelAnimationFrame(handle.value) : window.webkitCancelRequestAnimationFrame ? window.webkitCancelRequestAnimationFrame(handle.value) : window.mozCancelRequestAnimationFrame ? window.mozCancelRequestAnimationFrame(handle.value) : window.oCancelRequestAnimationFrame ? window.oCancelRequestAnimationFrame(handle.value) : window.msCancelRequestAnimationFrame ? window.msCancelRequestAnimationFrame(handle.value) : clearTimeout(handle);
        }
    }
    function fireLongPressEvent(originalEvent) {
        clearLongPressTimer();
        originalEvent = unifyEvent(originalEvent);
        var allowClickEvent = this.dispatchEvent(new CustomEvent("long-press", {
            bubbles: true,
            cancelable: true,
            detail: {
                clientX: originalEvent.clientX,
                clientY: originalEvent.clientY,
                offsetX: originalEvent.offsetX,
                offsetY: originalEvent.offsetY,
                pageX: originalEvent.pageX,
                pageY: originalEvent.pageY
            },
            clientX: originalEvent.clientX,
            clientY: originalEvent.clientY,
            offsetX: originalEvent.offsetX,
            offsetY: originalEvent.offsetY,
            pageX: originalEvent.pageX,
            pageY: originalEvent.pageY,
            screenX: originalEvent.screenX,
            screenY: originalEvent.screenY
        }));
        if (!allowClickEvent) {
            document.addEventListener("click", function suppressEvent(e) {
                document.removeEventListener("click", suppressEvent, true);
                cancelEvent(e);
            }, true);
        }
    }
    function unifyEvent(e) {
        if (e.changedTouches !== undefined) {
            return e.changedTouches[0];
        }
        return e;
    }
    function startLongPressTimer(e) {
        clearLongPressTimer(e);
        var el = e.target;
        var longPressDelayInMs = parseInt(getNearestAttribute(el, "data-long-press-delay", "1500"), 10);
        timer = requestTimeout(fireLongPressEvent.bind(el, e), longPressDelayInMs);
    }
    function clearLongPressTimer(e) {
        clearRequestTimeout(timer);
        timer = null;
    }
    function cancelEvent(e) {
        e.stopImmediatePropagation();
        e.preventDefault();
        e.stopPropagation();
    }
    function mouseDownHandler(e) {
        startX = e.clientX;
        startY = e.clientY;
        startLongPressTimer(e);
    }
    function mouseMoveHandler(e) {
        var diffX = Math.abs(startX - e.clientX);
        var diffY = Math.abs(startY - e.clientY);
        if (diffX >= maxDiffX || diffY >= maxDiffY) {
            clearLongPressTimer(e);
        }
    }
    function getNearestAttribute(el, attributeName, defaultValue) {
        while (el && el !== document.documentElement) {
            var attributeValue = el.getAttribute(attributeName);
            if (attributeValue) {
                return attributeValue;
            }
            el = el.parentNode;
        }
        return defaultValue;
    }
    document.addEventListener(mouseUp, clearLongPressTimer, true);
    document.addEventListener(mouseLeave, clearLongPressTimer, true);
    document.addEventListener(mouseMove, mouseMoveHandler, true);
    document.addEventListener("wheel", clearLongPressTimer, true);
    document.addEventListener("scroll", clearLongPressTimer, true);
    document.addEventListener("contextmenu", clearLongPressTimer, true);
    document.addEventListener(mouseDown, mouseDownHandler, true);
})(window, document);

function createTagsSelectTemplates(template) {
    const removeItemButton = this.config.removeItemButton;
    return {
        item: function({
            classNames
        }, data) {
            var label = data.label;
            var json;
            if (data.customProperties) {
                try {
                    json = JSON.parse(data.customProperties);
                } catch (e) {
                    json = data.customProperties;
                }
                label = json.label === undefined ? data.label : json.label;
            }
            return template(`
                     <div class="${String(classNames.item)} ${String(data.highlighted ? classNames.highlightedState : classNames.itemSelectable)} badge text-bg-primary fs-6 m-1""
                          data-item data-id="${String(data.id)}" data-value="${String(data.value)}"
                          ${String(removeItemButton ? "data-deletable" : "")}
                          ${String(data.active ? 'aria-selected="true"' : "")} ${String(data.disabled ? 'aria-disabled="true"' : "")}>
                        <i class="fas fa-fw fa-tag align-middle me-1"></i>${String(label)}
                        ${String(removeItemButton ? `<button type="button" class="${String(classNames.button)}" aria-label="Remove item: '${String(data.value)}'" data-button="">Remove item</button>` : "")}
                     </div>
                    `);
        }
    };
}

function createForumSelectTemplates(template) {
    var itemSelectText = this.config.itemSelectText;
    return {
        item: function({
            classNames
        }, data) {
            return template(`
                                 <div class="${String(classNames.item)} ${String(data.highlighted ? classNames.highlightedState : classNames.itemSelectable)}"
                                      data-item data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(data.active ? 'aria-selected="true"' : "")} ${String(data.disabled ? 'aria-disabled="true"' : "")}>
                                    <span><i class="fas fa-fw fa-comments text-secondary me-1"></i>${String(data.label)}</span>
                                 </div>
                                `);
        },
        choice: function({
            classNames
        }, data) {
            return template(`
                                 <div class="${String(classNames.item)} ${String(classNames.itemChoice)} ${String(data.disabled ? classNames.itemDisabled : classNames.itemSelectable)}"
                                      data-select-text="${String(itemSelectText)}" data-choice ${String(data.disabled ? 'data-choice-disabled aria-disabled="true"' : "data-choice-selectable")}
                                      data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(data.groupId > 0 ? 'role="treeitem"' : 'role="option"')}>
                                      <span><i class="fas fa-comments fa-fw text-secondary me-1"></i>${String(data.label)}</span>
                                 </div>
                                 `);
        },
        choiceGroup: function({
            classNames
        }, data) {
            return template(`
                     <div class="${String(classNames.item)} fw-bold text-secondary"
                          data-select-text="${String(itemSelectText)}" data-choice ${String(data.disabled ? 'data-choice-disabled aria-disabled="true"' : "data-choice-selectable")}
                          data-id="${String(data.id)}" data-value="${String(data.value)}"
                          ${String(data.groupId > 0 ? 'role="treeitem"' : 'role="option"')}>
                          <span><i class="fas fa-fw fa-folder text-warning me-1"></i>${String(data.label)}</span>
                     </div>
                     `);
        }
    };
}

function loadForumChoiceOptions(params, url, selectedForumId) {
    return fetch(url, {
        method: "POST",
        body: JSON.stringify(params),
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json;charset=utf-8"
        }
    }).then(function(response) {
        return response.json();
    }).then(function(data) {
        return data.Results.map(function(group) {
            return {
                value: group.id,
                label: group.text,
                choices: group.children.map(function(forum) {
                    const selectedId = parseInt(selectedForumId);
                    return {
                        value: forum.id,
                        label: forum.text,
                        selected: selectedId > 0 && selectedId == forum.id,
                        customProperties: {
                            page: params.Page,
                            total: data.Total,
                            url: forum.url
                        }
                    };
                })
            };
        });
    });
}

function loadChoiceOptions(params, url) {
    return fetch(url, {
        method: "POST",
        body: JSON.stringify(params),
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json;charset=utf-8"
        }
    }).then(function(response) {
        return response.json();
    }).then(function(data) {
        return data.Results.map(function(result) {
            return {
                value: result.id,
                label: result.text,
                customProperties: {
                    page: params.Page,
                    total: data.Total
                }
            };
        });
    });
}

function errorLog(x) {
    console.log("An Error has occurred!");
    console.log(x.responseText);
    console.log(x.status);
}

function wrap(el, wrapper) {
    el.parentNode.insertBefore(wrapper, el);
    wrapper.appendChild(el);
}

function empty(wrap) {
    while (wrap.firstChild) wrap.removeChild(wrap.firstChild);
}

function deepExtend(out, ...arguments_) {
    if (!out) {
        return {};
    }
    for (const obj of arguments_) {
        if (!obj) {
            continue;
        }
        for (const [ key, value ] of Object.entries(obj)) {
            switch (Object.prototype.toString.call(value)) {
              case "[object Object]":
                out[key] = out[key] || {};
                out[key] = deepExtend(out[key], value);
                break;

              case "[object Array]":
                out[key] = deepExtend(new Array(value.length), value);
                break;

              default:
                out[key] = value;
            }
        }
    }
    return out;
}

function renderAttachPreview(previewClass) {
    document.querySelectorAll(previewClass).forEach(attach => {
        return new bootstrap.Popover(attach, {
            html: true,
            trigger: "hover",
            placement: "bottom",
            content: function() {
                return `<img src="${attach.src}" class="img-fluid" />`;
            }
        });
    });
}

document.addEventListener("click", function(event) {
    if (event.target.parentElement && event.target.parentElement.matches('a[data-bs-toggle="confirm"]')) {
        event.preventDefault();
        var button = event.target.parentElement;
        var link = button.href;
        const text = button.dataset.title;
        var title = button.innerHTML;
        button.innerHTML = "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";
        bootbox.confirm({
            centerVertical: true,
            title: title,
            message: text,
            buttons: {
                confirm: {
                    label: `<i class="fa fa-check"></i> ${button.dataset.yes}`,
                    className: "btn-success"
                },
                cancel: {
                    label: `<i class="fa fa-times"></i> ${button.dataset.no}`,
                    className: "btn-danger"
                }
            },
            callback: function(confirmed) {
                if (confirmed) {
                    document.location.href = link;
                } else {
                    button.innerHTML = title;
                }
            }
        });
    }
}, false);

document.addEventListener("DOMContentLoaded", function() {
    if (document.querySelector(".btn-scroll") != null) {
        var scrollToTopBtn = document.querySelector(".btn-scroll"), rootElement = document.documentElement;
        function handleScroll() {
            const scrollTotal = rootElement.scrollHeight - rootElement.clientHeight;
            if (rootElement.scrollTop / scrollTotal > .15) {
                scrollToTopBtn.classList.add("show-btn-scroll");
            } else {
                scrollToTopBtn.classList.remove("show-btn-scroll");
            }
        }
        function scrollToTop(e) {
            e.preventDefault();
            rootElement.scrollTo({
                top: 0,
                behavior: "smooth"
            });
        }
        scrollToTopBtn.addEventListener("click", scrollToTop);
        document.addEventListener("scroll", handleScroll);
    }
    if (document.body.contains(document.getElementById("PasswordToggle"))) {
        const passwordToggle = document.getElementById("PasswordToggle");
        var icon = passwordToggle.querySelector("i"), pass = document.querySelector("input[id*='Password']");
        passwordToggle.addEventListener("click", function(event) {
            event.preventDefault();
            if (pass.getAttribute("type") === "text") {
                pass.setAttribute("type", "password");
                icon.classList.add("fa-eye-slash");
                icon.classList.remove("fa-eye");
            } else if (pass.getAttribute("type") === "password") {
                pass.setAttribute("type", "text");
                icon.classList.remove("fa-eye-slash");
                icon.classList.add("fa-eye");
            }
        });
    }
});

function getAlbumImagesData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById("PostAlbumsListPlaceholder"), list = placeHolder.querySelector("ul"), yafUserId = placeHolder.dataset.userid, pagedResults = {}, ajaxUrl = placeHolder.dataset.url + "api/Album/GetAlbumImages";
    pagedResults.UserId = yafUserId;
    pagedResults.PageSize = pageSize;
    pagedResults.PageNumber = pageNumber;
    fetch(ajaxUrl, {
        method: "POST",
        body: JSON.stringify(pagedResults),
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json;charset=utf-8"
        }
    }).then(res => res.json()).then(data => {
        empty(list);
        document.getElementById("PostAlbumsLoader").style.display = "none";
        data.AttachmentList.forEach(dataItem => {
            var li = document.createElement("li");
            li.classList.add("list-group-item");
            li.classList.add("list-group-item-action");
            li.style.whiteSpace = "nowrap";
            li.style.cursor = "pointer";
            li.setAttribute("onclick", dataItem.OnClick);
            li.innerHTML = dataItem.IconImage;
            list.appendChild(li);
        });
        renderAttachPreview(".attachments-preview");
        setPageNumber(pageSize, pageNumber, data.TotalRecords, document.getElementById("AlbumsListPager"), "Album Images", "getAlbumImagesData");
        if (isPageChange && document.querySelector(".albums-toggle") != null) {
            const toggleBtn = document.querySelector(".albums-toggle"), dropdownEl = new bootstrap.Dropdown(toggleBtn);
            dropdownEl.toggle();
        }
    }).catch(function(error) {
        console.log(error);
        document.getElementById("PostAlbumsLoader").style.display = "none";
        placeHolder.textContent = error;
    });
}

function getNotifyData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById("NotifyListPlaceholder"), list = placeHolder.querySelector("ul"), yafUserId = placeHolder.dataset.userid, pagedResults = {}, ajaxUrl = placeHolder.dataset.url + "api/Notify/GetNotifications";
    pagedResults.UserId = yafUserId;
    pagedResults.PageSize = pageSize;
    pagedResults.PageNumber = pageNumber;
    fetch(ajaxUrl, {
        method: "POST",
        body: JSON.stringify(pagedResults),
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json;charset=utf-8"
        }
    }).then(res => res.json()).then(data => {
        empty(list);
        document.getElementById("Loader").style.display = "none";
        if (data.AttachmentList.length > 0) {
            const markRead = document.getElementById("MarkRead");
            markRead.classList.remove("d-none");
            markRead.classList.add("d-block");
            data.AttachmentList.forEach(dataItem => {
                var li = document.createElement("li");
                li.classList.add("list-group-item");
                li.classList.add("list-group-item-action");
                li.classList.add("small");
                li.classList.add("text-wrap");
                li.style.width = "15rem";
                li.innerHTML = dataItem.FileName;
                list.appendChild(li);
            });
            setPageNumber(pageSize, pageNumber, data.TotalRecords, document.getElementById("NotifyListPager"), "Notifications", "getNotifyData");
            if (isPageChange) {
                const toggleBtn = document.querySelector(".notify-toggle"), dropdownEl = new bootstrap.Dropdown(toggleBtn);
                dropdownEl.toggle();
            }
        }
    }).catch(function(error) {
        console.log(error);
        document.getElementById("Loader").style.display = "none";
        placeHolder.textContent = error;
    });
}

function setPageNumber(pageSize, pageNumber, total, pagerHolder, label, javascriptFunction) {
    const pages = Math.ceil(total / pageSize), pagination = document.createElement("ul"), paginationNav = document.createElement("nav");
    paginationNav.setAttribute("aria-label", label + " Page Results");
    pagination.classList.add("pagination");
    pagination.classList.add("pagination-sm");
    empty(pagerHolder);
    if (pageNumber > 0) {
        const page = document.createElement("li");
        page.classList.add("page-item");
        page.innerHTML = `<a href="javascript:${javascriptFunction}(${pageSize},${pageNumber - 1},${total},true)" class="page-link"><i class="fas fa-angle-left"></i></a>`;
        pagination.appendChild(page);
    }
    var start = pageNumber - 2;
    var end = pageNumber + 3;
    if (start < 0) {
        start = 0;
    }
    if (end > pages) {
        end = pages;
    }
    if (start > 0) {
        let page = document.createElement("li");
        page.classList.add("page-item");
        page.innerHTML = `<a href="javascript:${javascriptFunction}(${pageSize},${0},${total}, true);" class="page-link">1</a>`;
        pagination.appendChild(page);
        page = document.createElement("li");
        page.classList.add("page-item");
        page.classList.add("disabled");
        page.innerHTML = '<a class="page-link" href="#" tabindex="-1">...</a>';
        pagination.appendChild(page);
    }
    for (var i = start; i < end; i++) {
        if (i === pageNumber) {
            const page = document.createElement("li");
            page.classList.add("page-item");
            page.classList.add("active");
            page.innerHTML = `<span class="page-link">${i + 1}</span>`;
            pagination.appendChild(page);
        } else {
            const page = document.createElement("li");
            page.classList.add("page-item");
            page.innerHTML = `<a href="javascript:${javascriptFunction}(${pageSize},${i},${total},true);" class="page-link">${i + 1}</a>`;
            pagination.appendChild(page);
        }
    }
    if (end < pages) {
        let page = document.createElement("li");
        page.classList.add("page-item");
        page.classList.add("disabled");
        page.innerHTML = '<a class="page-link" href="#" tabindex="-1">...</a>';
        pagination.appendChild(page);
        page = document.createElement("li");
        page.classList.add("page-item");
        page.innerHTML = `<a href="javascript:${javascriptFunction}(${pageSize},${pages - 1},${total},true)" class="page-link">${pages}</a>`;
        pagination.appendChild(page);
    }
    if (pageNumber < pages - 1) {
        const page = document.createElement("li");
        page.classList.add("page-item");
        page.innerHTML = `<a href="javascript:${javascriptFunction}(${pageSize},${pageNumber + 1},${total},true)" class="page-link"><i class="fas fa-angle-right"></i></a>`;
        pagination.appendChild(page);
    }
    paginationNav.appendChild(pagination);
    pagerHolder.appendChild(paginationNav);
}

document.addEventListener("DOMContentLoaded", function() {
    if (document.querySelector("a.btn-login,input.btn-login, .btn-spinner") != null) {
        document.querySelector("a.btn-login,input.btn-login, .btn-spinner").addEventListener("click", () => {
            document.querySelector(this).innerHTML = "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";
        });
    }
    for (const el of document.querySelectorAll('[data-toggle="lightbox"]')) {
        const lightBox = window.bootstrap.Lightbox;
        el.addEventListener("click", lightBox.initialize);
    }
    document.querySelectorAll(".dropdown-menu a.dropdown-toggle").forEach(menu => {
        menu.addEventListener("click", event => {
            var $el = menu, $subMenu = $el.nextElementSibling;
            document.querySelectorAll(".dropdown-menu .show").forEach(dropDownMenu => {
                dropDownMenu.classList.remove("show");
            });
            $subMenu.classList.add("show");
            $subMenu.style.top = $el.offsetTop - 10;
            $subMenu.style.left = $el.offsetWidth - 4;
            event.stopPropagation();
        });
    });
    document.querySelectorAll(".yafnet .select2-select").forEach(select => {
        const choice = new window.Choices(select, {
            allowHTML: true,
            shouldSort: false,
            placeholderValue: select.getAttribute("placeholder"),
            classNames: {
                containerOuter: [ "choices", "w-100" ]
            }
        });
    });
    document.querySelectorAll(".yafnet .select2-image-select").forEach(select => {
        var selectedValue = select.value;
        var groups = new Array();
        document.querySelectorAll(".yafnet .select2-image-select option[data-category]").forEach(option => {
            var group = option.dataset.category.trim();
            if (groups.indexOf(group) === -1) {
                groups.push(group);
            }
        });
        groups.forEach(group => {
            document.querySelectorAll(".yafnet .select2-image-select").forEach(s => {
                var optionGroups = new Array();
                s.querySelectorAll(`option[data-category='${group}']`).forEach(option => {
                    if (optionGroups.indexOf(option) === -1) {
                        optionGroups.push(option);
                    }
                });
                const optionGroupElement = document.createElement("optgroup");
                optionGroupElement.label = group;
                optionGroups.forEach(option => {
                    option.replaceWith(optionGroupElement);
                    optionGroupElement.appendChild(option);
                });
            });
        });
        select.value = selectedValue;
        const choice = new window.Choices(select, {
            classNames: {
                containerOuter: [ "choices", "w-100" ]
            },
            allowHTML: true,
            shouldSort: false,
            removeItemButton: select.dataset.allowClear === "True",
            placeholderValue: select.getAttribute("placeholder"),
            callbackOnCreateTemplates: function(template) {
                var itemSelectText = this.config.itemSelectText;
                const removeItemButton = this.config.removeItemButton;
                return {
                    item: function({
                        classNames
                    }, data) {
                        var label = data.label;
                        var json;
                        if (data.customProperties) {
                            try {
                                json = JSON.parse(data.customProperties);
                            } catch (e) {
                                json = data.customProperties;
                            }
                            label = json.label === undefined ? data.label : json.label;
                        }
                        return template(`
                                 <div class="${String(classNames.item)} ${String(data.highlighted ? classNames.highlightedState : classNames.itemSelectable)}"
                                      data-item data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(removeItemButton ? "data-deletable" : "")}
                                      ${String(data.active ? 'aria-selected="true"' : "")} ${String(data.disabled ? 'aria-disabled="true"' : "")}>
                                    ${String(label)}
                                    ${String(removeItemButton ? `<button type="button" class="${String(classNames.button)}" aria-label="Remove item: '${String(data.value)}'" data-button="">Remove item</button>` : "")}
                                 </div>
                                `);
                    },
                    choice: function({
                        classNames
                    }, data) {
                        var label = data.label;
                        var json;
                        if (data.customProperties) {
                            try {
                                json = JSON.parse(data.customProperties);
                            } catch (e) {
                                json = data.customProperties;
                            }
                            label = json.label === undefined ? data.label : json.label;
                        }
                        return template(`
                                 <div class="${String(classNames.item)} ${String(classNames.itemChoice)} ${String(data.disabled ? classNames.itemDisabled : classNames.itemSelectable)}"
                                      data-select-text="${String(itemSelectText)}" data-choice ${String(data.disabled ? 'data-choice-disabled aria-disabled="true"' : "data-choice-selectable")}
                                      data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(data.groupId > 0 ? 'role="treeitem"' : 'role="option"')}>
                                      ${String(label)}
                                 </div>
                                 `);
                    }
                };
            }
        });
        choice.passedElement.element.addEventListener("choice", function(event) {
            var json;
            if (event.detail.customProperties) {
                try {
                    json = JSON.parse(event.detail.customProperties);
                } catch (e) {
                    json = event.detail.customProperties;
                }
                if (json.url !== undefined) {
                    window.location = json.url;
                }
            }
        });
    });
    if (document.getElementById("PostAttachmentListPlaceholder") != null) {
        const pageSize = 5;
        const pageNumber = 0;
        getPaginationData(pageSize, pageNumber, false);
    }
    if (document.getElementById("PostAlbumsListPlaceholder") != null) {
        const pageSize = 5;
        const pageNumber = 0;
        getAlbumImagesData(pageSize, pageNumber, false);
    }
    if (document.getElementById("SearchResultsPlaceholder") != null && document.querySelector(".searchInput") != null) {
        document.querySelector(".searchInput").addEventListener("keypress", e => {
            var code = e.which;
            if (code === 13) {
                e.preventDefault();
                const pageNumberSearch = 0;
                getSearchResultsData(pageNumberSearch);
            }
        });
    }
    if (document.querySelector(".dropdown-notify") != null) {
        document.querySelector(".dropdown-notify").addEventListener("show.bs.dropdown", () => {
            var pageSize = 5;
            var pageNumber = 0;
            getNotifyData(pageSize, pageNumber, false);
        });
    }
    document.querySelectorAll(".form-check > input").forEach(input => {
        input.classList.add("form-check-input");
    });
    document.querySelectorAll(".form-check li > input").forEach(input => {
        input.classList.add("form-check-input");
    });
    document.querySelectorAll(".form-check > label").forEach(label => {
        label.classList.add("form-check-label");
    });
    document.querySelectorAll(".form-check li > label").forEach(label => {
        label.classList.add("form-check-label");
    });
    Prism.highlightAll();
    renderAttachPreview(".attachments-preview");
    document.querySelectorAll(".thanks-popover").forEach(thanks => {
        const popover = new bootstrap.Popover(thanks, {
            template: '<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body popover-body-scrollable"></div></div>'
        });
        thanks.addEventListener("show.bs.popover", () => {
            var messageId = thanks.dataset.messageid;
            var url = thanks.dataset.url;
            fetch(url + "/ThankYou/GetThanks/" + messageId, {
                method: "POST",
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json;charset=utf-8"
                }
            }).then(res => res.json()).then(response => document.getElementById(`popover-list-${messageId}`).innerHTML = response.ThanksInfo);
        });
    });
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(toolTip => {
        return new bootstrap.Tooltip(toolTip);
    });
    document.querySelectorAll(".attachedImage").forEach(imageLink => {
        var messageId = imageLink.parentNode.id;
        imageLink.setAttribute("data-gallery", `gallery-${messageId}`);
    });
    const quickReplyDialog = document.getElementById("QuickReplyDialog");
    if (quickReplyDialog) {
        const quickReply = document.getElementById("quickReply");
        moveDialogToCard(quickReplyDialog, quickReply);
        quickReplyDialog.addEventListener("show.bs.modal", _ => {
            const body = quickReply.querySelector(".modal-body"), footer = quickReply.querySelector(".quick-reply-footer");
            footer.classList.add("modal-footer");
            footer.classList.remove("quick-reply-footer");
            footer.classList.remove("mt-3");
            const copy = quickReplyDialog.querySelector(".modal-content");
            copy.append(body);
            copy.append(footer);
        });
        quickReplyDialog.addEventListener("hide.bs.modal", _ => {
            moveDialogToCard(quickReplyDialog, quickReply);
        });
    }
    function moveDialogToCard(quickReplyDialog, quickReply) {
        const body = quickReplyDialog.querySelector(".modal-body"), footer = quickReplyDialog.querySelector(".modal-footer");
        footer.classList.add("mt-3");
        footer.classList.add("quick-reply-footer");
        footer.classList.remove("modal-footer");
        const copy = quickReply.querySelector(".card-body");
        copy.append(body);
        copy.append(footer);
    }
});

document.addEventListener("DOMContentLoaded", function() {
    document.querySelectorAll(".list-group-item-menu, .message").forEach(element => {
        var isMessageContext = !!element.classList.contains("message");
        var contextMenu = element.querySelector(".context-menu");
        var messageId = 0;
        if (element.querySelector(".selectionQuoteable") != null) {
            messageId = element.querySelector(".selectionQuoteable").id;
        }
        if (window.matchMedia("only screen and (max-width: 760px)").matches) {
            const el = element;
            el.addEventListener("long-press", function(e) {
                e.preventDefault();
                if (isMessageContext) {
                    const selectedText = getSelectedMessageText();
                    if (selectedText.length) {
                        const searchItem = contextMenu.querySelector(".item-search"), selectedItem = contextMenu.querySelector(".item-selected-quoting"), selectedDivider = contextMenu.querySelector(".selected-divider");
                        if (searchItem != null) {
                            document.querySelectorAll(".item-search").forEach(item => {
                                item.remove();
                            });
                        }
                        if (selectedItem != null) {
                            selectedItem.remove();
                        }
                        if (selectedDivider != null) {
                            selectedDivider.remove();
                        }
                        if (contextMenu.dataset.url) {
                            const link = document.createElement("a");
                            link.classList.add("dropdown-item");
                            link.classList.add("item-selected-quoting");
                            link.href = `javascript:goToURL('${messageId}','${selectedText}','${contextMenu.dataset.url} ')`;
                            link.innerHTML = `<i class="fas fa-quote-left fa-fw"></i>&nbsp;${contextMenu.dataset.quote}`;
                            contextMenu.appendChild(link);
                        }
                        const linkSearch = document.createElement("a");
                        linkSearch.classList.add("dropdown-item");
                        linkSearch.classList.add("item-search");
                        linkSearch.href = `javascript:copyToClipBoard('${selectedText}')`;
                        linkSearch.innerHTML = `<i class="fas fa-clipboard fa-fw"></i>&nbsp;${contextMenu.dataset.copy}`;
                        contextMenu.appendChild(linkSearch);
                        const divider = document.createElement("div");
                        divider.classList.add("dropdown-divider");
                        divider.classList.add("selected-divider");
                        contextMenu.appendChild(linkSearch);
                        const linkSelected = document.createElement("a");
                        linkSelected.classList.add("dropdown-item");
                        linkSelected.classList.add("item-search");
                        linkSelected.href = `javascript:searchText('${selectedText}')`;
                        linkSelected.innerHTML = `<i class="fas fa-search fa-fw"></i>&nbsp;${contextMenu.dataset.search} "${selectedText}"`;
                        contextMenu.appendChild(linkSelected);
                    }
                }
                contextMenu.style.left = e.detail.pageX;
                contextMenu.style.top = e.detail.pageY;
                contextMenu.style.display = "block";
                contextMenu.classList.add("show");
            });
        }
        element.addEventListener("contextmenu", e => {
            e.preventDefault();
            document.querySelectorAll(".context-menu").forEach(menu => {
                menu.style.display = "none";
                menu.classList.remove("show");
            });
            if (isMessageContext) {
                const selectedText = getSelectedMessageText();
                if (selectedText.length) {
                    const searchItem = contextMenu.querySelector(".item-search"), selectedItem = contextMenu.querySelector(".item-selected-quoting"), selectedDivider = contextMenu.querySelector(".selected-divider");
                    if (searchItem != null) {
                        document.querySelectorAll(".item-search").forEach(item => {
                            item.remove();
                        });
                    }
                    if (selectedItem != null) {
                        selectedItem.remove();
                    }
                    if (selectedDivider != null) {
                        selectedDivider.remove();
                    }
                    if (contextMenu.dataset.url) {
                        const link = document.createElement("a");
                        link.classList.add("dropdown-item");
                        link.classList.add("item-selected-quoting");
                        link.href = `javascript:goToURL('${messageId}','${selectedText}','${contextMenu.dataset.url} ')`;
                        link.innerHTML = `<i class="fas fa-quote-left fa-fw"></i>&nbsp;${contextMenu.dataset.quote}`;
                        contextMenu.appendChild(link);
                    }
                    const linkSearch = document.createElement("a");
                    linkSearch.classList.add("dropdown-item");
                    linkSearch.classList.add("item-search");
                    linkSearch.href = `javascript:copyToClipBoard('${selectedText}')`;
                    linkSearch.innerHTML = `<i class="fas fa-clipboard fa-fw"></i>&nbsp;${contextMenu.dataset.copy}`;
                    contextMenu.appendChild(linkSearch);
                    const divider = document.createElement("div");
                    divider.classList.add("dropdown-divider");
                    divider.classList.add("selected-divider");
                    contextMenu.appendChild(linkSearch);
                    const linkSelected = document.createElement("a");
                    linkSelected.classList.add("dropdown-item");
                    linkSelected.classList.add("item-search");
                    linkSelected.href = `javascript:searchText('${selectedText}')`;
                    linkSelected.innerHTML = `<i class="fas fa-search fa-fw"></i>&nbsp;${contextMenu.dataset.search} "${selectedText}"`;
                    contextMenu.appendChild(linkSelected);
                }
            }
            contextMenu.style.display = "block";
            contextMenu.style.left = e.offsetX + "px";
            contextMenu.style.top = e.offsetY + "px";
            contextMenu.classList.add("show");
            return false;
        });
        element.addEventListener("click", () => {
            contextMenu.classList.remove("show");
            contextMenu.style.display = "none";
        });
        element.querySelector(".context-menu a").addEventListener("click", e => {
            var a = e.target;
            if (a.dataset.toggle !== undefined && a.dataset.toggle === "confirm") {
                e.preventDefault();
                var link = a.href;
                const text = a.dataset.title, title = a.innerHTML;
                bootbox.confirm({
                    centerVertical: true,
                    title: title,
                    message: text,
                    buttons: {
                        confirm: {
                            label: `<i class="fa fa-check"></i> ${a.dataset.yes}`,
                            className: "btn-success"
                        },
                        cancel: {
                            label: `<i class="fa fa-times"></i> ${a.dataset.no}`,
                            className: "btn-danger"
                        }
                    },
                    callback: function(confirmed) {
                        if (confirmed) {
                            document.location.href = link;
                        }
                    }
                });
            }
            contextMenu.classList.remove("show");
            contextMenu.style.display = "none";
        });
        contextMenu.addEventListener("click", function(event) {
            if (event.target.parentElement.matches('[data-bs-toggle="confirm"]')) {
                event.preventDefault();
                const button = event.target.parentElement, text = button.dataset.title, title = button.innerHTML;
                bootbox.confirm({
                    centerVertical: true,
                    title: title,
                    message: text,
                    buttons: {
                        confirm: {
                            label: `<i class="fa fa-check"></i> ${button.dataset.yes}`,
                            className: "btn-success"
                        },
                        cancel: {
                            label: `<i class="fa fa-times"></i> ${button.dataset.no}`,
                            className: "btn-danger"
                        }
                    },
                    callback: function(confirmed) {
                        if (confirmed) {
                            button.click();
                        }
                    }
                });
            }
            contextMenu.classList.remove("show");
            contextMenu.style.display = "none";
        }, false);
        document.querySelector("body").addEventListener("click", () => {
            contextMenu.classList.remove("show");
            contextMenu.style.display = "none";
        });
    });
});

function goToURL(messageId, input, url) {
    window.location.href = url + "&q=" + messageId + "&text=" + encodeURIComponent(input);
}

function copyToClipBoard(input) {
    navigator.clipboard.writeText(input);
}

function searchText(input) {
    const a = document.createElement("a");
    a.target = "_blank";
    a.href = `https://www.google.com/search?q=${encodeURIComponent(input)}`;
    a.click();
}

function getSelectedMessageText() {
    var text = "";
    const sel = window.getSelection();
    if (sel.rangeCount) {
        const container = document.createElement("div");
        for (var i = 0, len = sel.rangeCount; i < len; ++i) {
            container.appendChild(sel.getRangeAt(i).cloneContents());
        }
        text = container.textContent || container.innerText;
    }
    return text.replace(/<p[^>]*>/gi, "\n").replace(/<\/p>| {2}/gi, "").replace("(", "").replace(")", "").replace('"', "").replace("'", "").replace("'", "").replace(";", "").trim();
}