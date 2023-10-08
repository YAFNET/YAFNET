(function(root, factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        define([ "jquery" ], factory);
    } else if (typeof exports === "object") {
        module.exports = factory(require("jquery"));
    } else {
        root.bootbox = factory(root.jQuery);
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
        $.each([ "OK", "CANCEL", "CONFIRM" ], function(_, v) {
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
        $.extend(defaults, values);
        return exports;
    };
    exports.hideAll = function() {
        document.querySelectorAll(".bootbox").forEach(box => {
            box.modal("hide");
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
        body.querySelector(".bootbox-body").innerHTML = options.message;
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
                    button.prop({
                        disabled: true
                    });
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
                    closeButton.innerHTML = "&times;";
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
            dialog.addEventListener("hide.bs.modal", {
                dialog: dialog
            }, unbindModal, {
                once: true
            });
            dialog.addEventListener("hidden.bs.modal", {
                dialog: dialog
            }, destroyModal, {
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
                dialog.on("show.bs.modal", options.onShow);
            } else {
                throw new Error('Argument supplied to "onShow" must be a function');
            }
        }
        dialog.addEventListener("shown.bs.modal", {
            dialog: dialog
        }, focusPrimaryButton);
        if (options.onShown) {
            if (typeof options.onShown === "function") {
                dialog.on("shown.bs.modal", options.onShown);
            } else {
                throw new Error('Argument supplied to "onShown" must be a function');
            }
        }
        if (options.backdrop === true) {
            let startedOnBody = false;
            dialog.on("mousedown", ".modal-content", function(e) {
                e.stopPropagation();
                startedOnBody = true;
            });
            dialog.on("click.dismiss.bs.modal", function(e) {
                if (startedOnBody || e.target !== e.currentTarget) {
                    return;
                }
                dialog.trigger("escape.close.bb");
            });
        }
        dialog.addEventListener("escape.close.bb", function(e) {
            if (callbacks.onEscape) {
                processCallback(e, dialog, callbacks.onEscape);
            }
        });
        document.addEventListener("click", e => {
            if (e.target.closest(".modal-footer button:not(.disabled)")) {
                const callbackKey = e.target.closest(".modal-footer button:not(.disabled)").dataset.bbHandler;
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
                dialog.trigger("escape.close.bb");
            }
        });
        document.querySelector(options.container).append(dialog);
        const modal = new bootstrap.Modal(dialog, {
            backdrop: options.backdrop,
            keyboard: false,
            show: false
        });
        if (options.show) {
            modal.show(options.relatedTarget);
        }
        return modal;
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
                value = input.querySelector("input:checked").map(function(e) {
                    return e.value;
                }).get();
            } else if (options.inputType === "radio") {
                value = input.querySelector("input:checked").value;
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
                input.prop({
                    required: true
                });
            }
            if (options.rows && !isNaN(parseInt(options.rows))) {
                if (options.inputType === "textarea") {
                    input.setAttribute({
                        rows: options.rows
                    });
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
                input.prop({
                    required: true
                });
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
            let groups = {};
            inputOptions = options.inputOptions || [];
            if (!$.isArray(inputOptions)) {
                throw new Error("Please pass an array of input options");
            }
            if (!inputOptions.length) {
                throw new Error('prompt with "inputType" set to "select" requires at least one option');
            }
            if (options.required) {
                input.prop({
                    required: true
                });
            }
            if (options.multiple) {
                input.prop({
                    multiple: true
                });
            }
            for (const [ key, value ] of Object.entries(inputOptions)) {
                let elem = input;
                if (option.value === undefined || option.text === undefined) {
                    throw new Error('each option needs a "value" property and a "text" property');
                }
                if (option.group) {
                    if (!groups[option.group]) {
                        groups[option.group] = generateElement("<optgroup />").setAttribute("label", option.group);
                    }
                    elem = groups[option.group];
                }
                let o = generateElement(templates.option);
                o.setAttribute("value", option.value).text(option.text);
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
            let checkboxValues = $.isArray(options.value) ? options.value : [ options.value ];
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
                        checkbox.querySelector("input").prop("checked", true);
                    }
                }
                input.append(checkbox);
            }
            break;

          case "radio":
            if (options.value !== undefined && $.isArray(options.value)) {
                throw new Error('prompt with "inputType" set to "radio" requires a single, non-array value for "value"');
            }
            inputOptions = options.inputOptions || [];
            if (!inputOptions.length) {
                throw new Error('prompt with "inputType" set to "radio" requires at least one option');
            }
            input = generateElement('<div class="bootbox-radiobutton-list"></div>');
            let checkFirstRadio = true;
            for (const [ _, option ] of Object.entries(inputOptions)) {
                if (option.value === undefined || option.text === undefined) {
                    throw new Error('each option needs a "value" property and a "text" property');
                }
                let radio = generateElement(templates.inputs[options.inputType]);
                radio.querySelector("input").setAttribute("value", option.value);
                radio.querySelector("label").append(`\n${option.text}`);
                if (options.value !== undefined) {
                    if (option.value === options.value) {
                        radio.querySelector("input").prop("checked", true);
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
        promptDialog._element.removeEventListener("shown.bs.modal", focusPrimaryButton);
        promptDialog._element.addEventListener("shown.bs.modal", function() {
            input.focus();
        });
        const modal = new bootstrap.Modal(promptDialog._element);
        if (shouldShow === true) {
            modal.show();
        }
        return promptDialog;
    };
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
    function focusPrimaryButton(e) {
        e.data.dialog.querySelector(".bootbox-accept").first().trigger("focus");
    }
    function destroyModal(e) {
        if (e.target === e.data.dialog[0]) {
            e.data.dialog.remove();
        }
    }
    function unbindModal(e) {
        if (e.target === e.data.dialog[0]) {
            e.data.dialog.removeEventListener("escape.close.bb");
            e.data.dialog.removeEventListener("click");
        }
    }
    function processCallback(e, dialog, callback) {
        e.stopPropagation();
        e.preventDefault();
        const preserveDialog = typeof callback === "function" && callback.call(dialog, e) === false;
        if (!preserveDialog) {
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
    function generateElement(html) {
        const template = document.createElement("template");
        template.innerHTML = html.trim();
        return template.content.children[0];
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
        input.parentNode.insertBefore(plusButton, input.nextSibling);
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

(function webpackUniversalModuleDefinition(root, factory) {
    if (typeof exports === "object" && typeof module === "object") module.exports = factory(); else if (typeof define === "function" && define.amd) define([], factory); else if (typeof exports === "object") exports["Choices"] = factory(); else root["Choices"] = factory();
})(window, function() {
    return function() {
        "use strict";
        var __webpack_modules__ = {
            282: function(__unused_webpack_module, exports, __webpack_require__) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.clearChoices = exports.activateChoices = exports.filterChoices = exports.addChoice = void 0;
                var constants_1 = __webpack_require__(883);
                var addChoice = function(_a) {
                    var value = _a.value, label = _a.label, id = _a.id, groupId = _a.groupId, disabled = _a.disabled, elementId = _a.elementId, customProperties = _a.customProperties, placeholder = _a.placeholder, keyCode = _a.keyCode;
                    return {
                        type: constants_1.ACTION_TYPES.ADD_CHOICE,
                        value: value,
                        label: label,
                        id: id,
                        groupId: groupId,
                        disabled: disabled,
                        elementId: elementId,
                        customProperties: customProperties,
                        placeholder: placeholder,
                        keyCode: keyCode
                    };
                };
                exports.addChoice = addChoice;
                var filterChoices = function(results) {
                    return {
                        type: constants_1.ACTION_TYPES.FILTER_CHOICES,
                        results: results
                    };
                };
                exports.filterChoices = filterChoices;
                var activateChoices = function(active) {
                    if (active === void 0) {
                        active = true;
                    }
                    return {
                        type: constants_1.ACTION_TYPES.ACTIVATE_CHOICES,
                        active: active
                    };
                };
                exports.activateChoices = activateChoices;
                var clearChoices = function() {
                    return {
                        type: constants_1.ACTION_TYPES.CLEAR_CHOICES
                    };
                };
                exports.clearChoices = clearChoices;
            },
            783: function(__unused_webpack_module, exports, __webpack_require__) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.addGroup = void 0;
                var constants_1 = __webpack_require__(883);
                var addGroup = function(_a) {
                    var value = _a.value, id = _a.id, active = _a.active, disabled = _a.disabled;
                    return {
                        type: constants_1.ACTION_TYPES.ADD_GROUP,
                        value: value,
                        id: id,
                        active: active,
                        disabled: disabled
                    };
                };
                exports.addGroup = addGroup;
            },
            464: function(__unused_webpack_module, exports, __webpack_require__) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.highlightItem = exports.removeItem = exports.addItem = void 0;
                var constants_1 = __webpack_require__(883);
                var addItem = function(_a) {
                    var value = _a.value, label = _a.label, id = _a.id, choiceId = _a.choiceId, groupId = _a.groupId, customProperties = _a.customProperties, placeholder = _a.placeholder, keyCode = _a.keyCode;
                    return {
                        type: constants_1.ACTION_TYPES.ADD_ITEM,
                        value: value,
                        label: label,
                        id: id,
                        choiceId: choiceId,
                        groupId: groupId,
                        customProperties: customProperties,
                        placeholder: placeholder,
                        keyCode: keyCode
                    };
                };
                exports.addItem = addItem;
                var removeItem = function(id, choiceId) {
                    return {
                        type: constants_1.ACTION_TYPES.REMOVE_ITEM,
                        id: id,
                        choiceId: choiceId
                    };
                };
                exports.removeItem = removeItem;
                var highlightItem = function(id, highlighted) {
                    return {
                        type: constants_1.ACTION_TYPES.HIGHLIGHT_ITEM,
                        id: id,
                        highlighted: highlighted
                    };
                };
                exports.highlightItem = highlightItem;
            },
            137: function(__unused_webpack_module, exports, __webpack_require__) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.setIsLoading = exports.resetTo = exports.clearAll = void 0;
                var constants_1 = __webpack_require__(883);
                var clearAll = function() {
                    return {
                        type: constants_1.ACTION_TYPES.CLEAR_ALL
                    };
                };
                exports.clearAll = clearAll;
                var resetTo = function(state) {
                    return {
                        type: constants_1.ACTION_TYPES.RESET_TO,
                        state: state
                    };
                };
                exports.resetTo = resetTo;
                var setIsLoading = function(isLoading) {
                    return {
                        type: constants_1.ACTION_TYPES.SET_IS_LOADING,
                        isLoading: isLoading
                    };
                };
                exports.setIsLoading = setIsLoading;
            },
            373: function(__unused_webpack_module, exports, __webpack_require__) {
                var __spreadArray = this && this.__spreadArray || function(to, from, pack) {
                    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
                        if (ar || !(i in from)) {
                            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
                            ar[i] = from[i];
                        }
                    }
                    return to.concat(ar || Array.prototype.slice.call(from));
                };
                var __importDefault = this && this.__importDefault || function(mod) {
                    return mod && mod.__esModule ? mod : {
                        default: mod
                    };
                };
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                var deepmerge_1 = __importDefault(__webpack_require__(996));
                var fuse_js_1 = __importDefault(__webpack_require__(221));
                var choices_1 = __webpack_require__(282);
                var groups_1 = __webpack_require__(783);
                var items_1 = __webpack_require__(464);
                var misc_1 = __webpack_require__(137);
                var components_1 = __webpack_require__(520);
                var constants_1 = __webpack_require__(883);
                var defaults_1 = __webpack_require__(789);
                var utils_1 = __webpack_require__(799);
                var reducers_1 = __webpack_require__(655);
                var store_1 = __importDefault(__webpack_require__(744));
                var templates_1 = __importDefault(__webpack_require__(686));
                var IS_IE11 = "-ms-scroll-limit" in document.documentElement.style && "-ms-ime-align" in document.documentElement.style;
                var USER_DEFAULTS = {};
                var Choices = function() {
                    function Choices(element, userConfig) {
                        var _a;
                        if (element === void 0) {
                            element = "[data-choice]";
                        }
                        if (userConfig === void 0) {
                            userConfig = {};
                        }
                        var _this = this;
                        if (userConfig.allowHTML === undefined) {
                            console.warn("Deprecation warning: allowHTML will default to false in a future release. To render HTML in Choices, you will need to set it to true. Setting allowHTML will suppress this message.");
                        }
                        this.config = deepmerge_1.default.all([ defaults_1.DEFAULT_CONFIG, Choices.defaults.options, userConfig ], {
                            arrayMerge: function(_, sourceArray) {
                                return __spreadArray([], sourceArray, true);
                            }
                        });
                        var invalidConfigOptions = (0, utils_1.diff)(this.config, defaults_1.DEFAULT_CONFIG);
                        if (invalidConfigOptions.length) {
                            console.warn("Unknown config option(s) passed", invalidConfigOptions.join(", "));
                        }
                        var passedElement = typeof element === "string" ? document.querySelector(element) : element;
                        if (!(passedElement instanceof HTMLInputElement || passedElement instanceof HTMLSelectElement)) {
                            throw TypeError("Expected one of the following types text|select-one|select-multiple");
                        }
                        this._isTextElement = passedElement.type === constants_1.TEXT_TYPE;
                        this._isSelectOneElement = passedElement.type === constants_1.SELECT_ONE_TYPE;
                        this._isSelectMultipleElement = passedElement.type === constants_1.SELECT_MULTIPLE_TYPE;
                        this._isSelectElement = this._isSelectOneElement || this._isSelectMultipleElement;
                        this.config.searchEnabled = this._isSelectMultipleElement || this.config.searchEnabled;
                        if (![ "auto", "always" ].includes("".concat(this.config.renderSelectedChoices))) {
                            this.config.renderSelectedChoices = "auto";
                        }
                        if (userConfig.addItemFilter && typeof userConfig.addItemFilter !== "function") {
                            var re = userConfig.addItemFilter instanceof RegExp ? userConfig.addItemFilter : new RegExp(userConfig.addItemFilter);
                            this.config.addItemFilter = re.test.bind(re);
                        }
                        if (this._isTextElement) {
                            this.passedElement = new components_1.WrappedInput({
                                element: passedElement,
                                classNames: this.config.classNames,
                                delimiter: this.config.delimiter
                            });
                        } else {
                            this.passedElement = new components_1.WrappedSelect({
                                element: passedElement,
                                classNames: this.config.classNames,
                                template: function(data) {
                                    return _this._templates.option(data);
                                }
                            });
                        }
                        this.initialised = false;
                        this._store = new store_1.default();
                        this._initialState = reducers_1.defaultState;
                        this._currentState = reducers_1.defaultState;
                        this._prevState = reducers_1.defaultState;
                        this._currentValue = "";
                        this._canSearch = !!this.config.searchEnabled;
                        this._isScrollingOnIe = false;
                        this._highlightPosition = 0;
                        this._wasTap = true;
                        this._placeholderValue = this._generatePlaceholderValue();
                        this._baseId = (0, utils_1.generateId)(this.passedElement.element, "choices-");
                        this._direction = this.passedElement.dir;
                        if (!this._direction) {
                            var elementDirection = window.getComputedStyle(this.passedElement.element).direction;
                            var documentDirection = window.getComputedStyle(document.documentElement).direction;
                            if (elementDirection !== documentDirection) {
                                this._direction = elementDirection;
                            }
                        }
                        this._idNames = {
                            itemChoice: "item-choice"
                        };
                        if (this._isSelectElement) {
                            this._presetGroups = this.passedElement.optionGroups;
                            this._presetOptions = this.passedElement.options;
                        }
                        this._presetChoices = this.config.choices;
                        this._presetItems = this.config.items;
                        if (this.passedElement.value && this._isTextElement) {
                            var splitValues = this.passedElement.value.split(this.config.delimiter);
                            this._presetItems = this._presetItems.concat(splitValues);
                        }
                        if (this.passedElement.options) {
                            var choicesFromOptions = this.passedElement.optionsAsChoices();
                            (_a = this._presetChoices).push.apply(_a, choicesFromOptions);
                        }
                        this._render = this._render.bind(this);
                        this._onFocus = this._onFocus.bind(this);
                        this._onBlur = this._onBlur.bind(this);
                        this._onKeyUp = this._onKeyUp.bind(this);
                        this._onKeyDown = this._onKeyDown.bind(this);
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
                            if (!this.config.silent) {
                                console.warn("Trying to initialise Choices on element already initialised", {
                                    element: element
                                });
                            }
                            this.initialised = true;
                            return;
                        }
                        this.init();
                    }
                    Object.defineProperty(Choices, "defaults", {
                        get: function() {
                            return Object.preventExtensions({
                                get options() {
                                    return USER_DEFAULTS;
                                },
                                get templates() {
                                    return templates_1.default;
                                }
                            });
                        },
                        enumerable: false,
                        configurable: true
                    });
                    Choices.prototype.init = function() {
                        if (this.initialised) {
                            return;
                        }
                        this._createTemplates();
                        this._createElements();
                        this._createStructure();
                        this._store.subscribe(this._render);
                        this._render();
                        this._addEventListeners();
                        var shouldDisable = !this.config.addItems || this.passedElement.element.hasAttribute("disabled");
                        if (shouldDisable) {
                            this.disable();
                        }
                        this.initialised = true;
                        var callbackOnInit = this.config.callbackOnInit;
                        if (callbackOnInit && typeof callbackOnInit === "function") {
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
                        this.clearStore();
                        if (this._isSelectElement) {
                            this.passedElement.options = this._presetOptions;
                        }
                        this._templates = templates_1.default;
                        this.initialised = false;
                    };
                    Choices.prototype.enable = function() {
                        if (this.passedElement.isDisabled) {
                            this.passedElement.enable();
                        }
                        if (this.containerOuter.isDisabled) {
                            this._addEventListeners();
                            this.input.enable();
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
                        var id = item.id, _a = item.groupId, groupId = _a === void 0 ? -1 : _a, _b = item.value, value = _b === void 0 ? "" : _b, _c = item.label, label = _c === void 0 ? "" : _c;
                        var group = groupId >= 0 ? this._store.getGroupById(groupId) : null;
                        this._store.dispatch((0, items_1.highlightItem)(id, true));
                        if (runEvent) {
                            this.passedElement.triggerEvent(constants_1.EVENTS.highlightItem, {
                                id: id,
                                value: value,
                                label: label,
                                groupValue: group && group.value ? group.value : null
                            });
                        }
                        return this;
                    };
                    Choices.prototype.unhighlightItem = function(item) {
                        if (!item || !item.id) {
                            return this;
                        }
                        var id = item.id, _a = item.groupId, groupId = _a === void 0 ? -1 : _a, _b = item.value, value = _b === void 0 ? "" : _b, _c = item.label, label = _c === void 0 ? "" : _c;
                        var group = groupId >= 0 ? this._store.getGroupById(groupId) : null;
                        this._store.dispatch((0, items_1.highlightItem)(id, false));
                        this.passedElement.triggerEvent(constants_1.EVENTS.highlightItem, {
                            id: id,
                            value: value,
                            label: label,
                            groupValue: group && group.value ? group.value : null
                        });
                        return this;
                    };
                    Choices.prototype.highlightAll = function() {
                        var _this = this;
                        this._store.items.forEach(function(item) {
                            return _this.highlightItem(item);
                        });
                        return this;
                    };
                    Choices.prototype.unhighlightAll = function() {
                        var _this = this;
                        this._store.items.forEach(function(item) {
                            return _this.unhighlightItem(item);
                        });
                        return this;
                    };
                    Choices.prototype.removeActiveItemsByValue = function(value) {
                        var _this = this;
                        this._store.activeItems.filter(function(item) {
                            return item.value === value;
                        }).forEach(function(item) {
                            return _this._removeItem(item);
                        });
                        return this;
                    };
                    Choices.prototype.removeActiveItems = function(excludedId) {
                        var _this = this;
                        this._store.activeItems.filter(function(_a) {
                            var id = _a.id;
                            return id !== excludedId;
                        }).forEach(function(item) {
                            return _this._removeItem(item);
                        });
                        return this;
                    };
                    Choices.prototype.removeHighlightedItems = function(runEvent) {
                        var _this = this;
                        if (runEvent === void 0) {
                            runEvent = false;
                        }
                        this._store.highlightedActiveItems.forEach(function(item) {
                            _this._removeItem(item);
                            if (runEvent) {
                                _this._triggerChange(item.value);
                            }
                        });
                        return this;
                    };
                    Choices.prototype.showDropdown = function(preventInputFocus) {
                        var _this = this;
                        if (this.dropdown.isActive) {
                            return this;
                        }
                        requestAnimationFrame(function() {
                            _this.dropdown.show();
                            _this.containerOuter.open(_this.dropdown.distanceFromTopWindow);
                            if (!preventInputFocus && _this._canSearch) {
                                _this.input.focus();
                            }
                            _this.passedElement.triggerEvent(constants_1.EVENTS.showDropdown, {});
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
                            _this.passedElement.triggerEvent(constants_1.EVENTS.hideDropdown, {});
                        });
                        return this;
                    };
                    Choices.prototype.getValue = function(valueOnly) {
                        if (valueOnly === void 0) {
                            valueOnly = false;
                        }
                        var values = this._store.activeItems.reduce(function(selectedItems, item) {
                            var itemValue = valueOnly ? item.value : item;
                            selectedItems.push(itemValue);
                            return selectedItems;
                        }, []);
                        return this._isSelectOneElement ? values[0] : values;
                    };
                    Choices.prototype.setValue = function(items) {
                        var _this = this;
                        if (!this.initialised) {
                            return this;
                        }
                        items.forEach(function(value) {
                            return _this._setChoiceOrItem(value);
                        });
                        return this;
                    };
                    Choices.prototype.setChoiceByValue = function(value) {
                        var _this = this;
                        if (!this.initialised || this._isTextElement) {
                            return this;
                        }
                        var choiceValue = Array.isArray(value) ? value : [ value ];
                        choiceValue.forEach(function(val) {
                            return _this._findAndSelectChoiceByValue(val);
                        });
                        return this;
                    };
                    Choices.prototype.setChoices = function(choicesArrayOrFetcher, value, label, replaceChoices) {
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
                        if (!this.initialised) {
                            throw new ReferenceError("setChoices was called on a non-initialized instance of Choices");
                        }
                        if (!this._isSelectElement) {
                            throw new TypeError("setChoices can't be used with INPUT based Choices");
                        }
                        if (typeof value !== "string" || !value) {
                            throw new TypeError("value parameter must be a name of 'value' field in passed objects");
                        }
                        if (replaceChoices) {
                            this.clearChoices();
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
                                    return _this.setChoices(data, value, label, replaceChoices);
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
                        this._startLoading();
                        choicesArrayOrFetcher.forEach(function(groupOrChoice) {
                            if (groupOrChoice.choices) {
                                _this._addGroup({
                                    id: groupOrChoice.id ? parseInt("".concat(groupOrChoice.id), 10) : null,
                                    group: groupOrChoice,
                                    valueKey: value,
                                    labelKey: label
                                });
                            } else {
                                var choice = groupOrChoice;
                                _this._addChoice({
                                    value: choice[value],
                                    label: choice[label],
                                    isSelected: !!choice.selected,
                                    isDisabled: !!choice.disabled,
                                    placeholder: !!choice.placeholder,
                                    customProperties: choice.customProperties
                                });
                            }
                        });
                        this._stopLoading();
                        return this;
                    };
                    Choices.prototype.clearChoices = function() {
                        this._store.dispatch((0, choices_1.clearChoices)());
                        return this;
                    };
                    Choices.prototype.clearStore = function() {
                        this._store.dispatch((0, misc_1.clearAll)());
                        return this;
                    };
                    Choices.prototype.clearInput = function() {
                        var shouldSetInputWidth = !this._isSelectOneElement;
                        this.input.clear(shouldSetInputWidth);
                        if (!this._isTextElement && this._canSearch) {
                            this._isSearching = false;
                            this._store.dispatch((0, choices_1.activateChoices)(true));
                        }
                        return this;
                    };
                    Choices.prototype._render = function() {
                        if (this._store.isLoading()) {
                            return;
                        }
                        this._currentState = this._store.state;
                        var stateChanged = this._currentState.choices !== this._prevState.choices || this._currentState.groups !== this._prevState.groups || this._currentState.items !== this._prevState.items;
                        var shouldRenderChoices = this._isSelectElement;
                        var shouldRenderItems = this._currentState.items !== this._prevState.items;
                        if (!stateChanged) {
                            return;
                        }
                        if (shouldRenderChoices) {
                            this._renderChoices();
                        }
                        if (shouldRenderItems) {
                            this._renderItems();
                        }
                        this._prevState = this._currentState;
                    };
                    Choices.prototype._renderChoices = function() {
                        var _this = this;
                        var _a = this._store, activeGroups = _a.activeGroups, activeChoices = _a.activeChoices;
                        var choiceListFragment = document.createDocumentFragment();
                        this.choiceList.clear();
                        if (this.config.resetScrollPosition) {
                            requestAnimationFrame(function() {
                                return _this.choiceList.scrollToTop();
                            });
                        }
                        if (activeGroups.length >= 1 && !this._isSearching) {
                            var activePlaceholders = activeChoices.filter(function(activeChoice) {
                                return activeChoice.placeholder === true && activeChoice.groupId === -1;
                            });
                            if (activePlaceholders.length >= 1) {
                                choiceListFragment = this._createChoicesFragment(activePlaceholders, choiceListFragment);
                            }
                            choiceListFragment = this._createGroupsFragment(activeGroups, activeChoices, choiceListFragment);
                        } else if (activeChoices.length >= 1) {
                            choiceListFragment = this._createChoicesFragment(activeChoices, choiceListFragment);
                        }
                        var activeItems = this._store.activeItems;
                        if (choiceListFragment.childNodes && choiceListFragment.childNodes.length > 0) {
                            var canAddItem = this._canAddItem(activeItems, this.input.value);
                            if (canAddItem.response) {
                                this.choiceList.append(choiceListFragment);
                                this._highlightChoice();
                            } else {
                                var notice = this._getTemplate("notice", canAddItem.notice);
                                this.choiceList.append(notice);
                            }
                        } else {
                            var canAddChoice = this._canAddChoice(activeItems, this.input.value);
                            var dropdownItem = void 0;
                            if (canAddChoice.response) {
                                dropdownItem = this._getTemplate("notice", canAddChoice.notice);
                            } else if (this._isSearching) {
                                var notice = typeof this.config.noResultsText === "function" ? this.config.noResultsText() : this.config.noResultsText;
                                dropdownItem = this._getTemplate("notice", notice, "no-results");
                            } else {
                                var notice = typeof this.config.noChoicesText === "function" ? this.config.noChoicesText() : this.config.noChoicesText;
                                dropdownItem = this._getTemplate("notice", notice, "no-choices");
                            }
                            this.choiceList.append(dropdownItem);
                        }
                    };
                    Choices.prototype._renderItems = function() {
                        var activeItems = this._store.activeItems || [];
                        this.itemList.clear();
                        var itemListFragment = this._createItemsFragment(activeItems);
                        if (itemListFragment.childNodes) {
                            this.itemList.append(itemListFragment);
                        }
                    };
                    Choices.prototype._createGroupsFragment = function(groups, choices, fragment) {
                        var _this = this;
                        if (fragment === void 0) {
                            fragment = document.createDocumentFragment();
                        }
                        var getGroupChoices = function(group) {
                            return choices.filter(function(choice) {
                                if (_this._isSelectOneElement) {
                                    return choice.groupId === group.id;
                                }
                                return choice.groupId === group.id && (_this.config.renderSelectedChoices === "always" || !choice.selected);
                            });
                        };
                        if (this.config.shouldSort) {
                            groups.sort(this.config.sorter);
                        }
                        var choicesWithoutGroup = choices.filter(function(c) {
                            return c.groupId == -1;
                        });
                        if (choicesWithoutGroup.length > 0) {
                            this._createChoicesFragment(choicesWithoutGroup, fragment, false);
                        }
                        groups.forEach(function(group) {
                            var groupChoices = getGroupChoices(group);
                            if (groupChoices.length >= 1) {
                                var dropdownGroup = _this._getTemplate("choiceGroup", group);
                                fragment.appendChild(dropdownGroup);
                                _this._createChoicesFragment(groupChoices, fragment, true);
                            }
                        });
                        return fragment;
                    };
                    Choices.prototype._createChoicesFragment = function(choices, fragment, withinGroup) {
                        var _this = this;
                        if (fragment === void 0) {
                            fragment = document.createDocumentFragment();
                        }
                        if (withinGroup === void 0) {
                            withinGroup = false;
                        }
                        var _a = this.config, renderSelectedChoices = _a.renderSelectedChoices, searchResultLimit = _a.searchResultLimit, renderChoiceLimit = _a.renderChoiceLimit;
                        var filter = this._isSearching ? utils_1.sortByScore : this.config.sorter;
                        var appendChoice = function(choice) {
                            var shouldRender = renderSelectedChoices === "auto" ? _this._isSelectOneElement || !choice.selected : true;
                            if (shouldRender) {
                                var dropdownItem = _this._getTemplate("choice", choice, _this.config.itemSelectText);
                                fragment.appendChild(dropdownItem);
                            }
                        };
                        var rendererableChoices = choices;
                        if (renderSelectedChoices === "auto" && !this._isSelectOneElement) {
                            rendererableChoices = choices.filter(function(choice) {
                                return !choice.selected;
                            });
                        }
                        var _b = rendererableChoices.reduce(function(acc, choice) {
                            if (choice.placeholder) {
                                acc.placeholderChoices.push(choice);
                            } else {
                                acc.normalChoices.push(choice);
                            }
                            return acc;
                        }, {
                            placeholderChoices: [],
                            normalChoices: []
                        }), placeholderChoices = _b.placeholderChoices, normalChoices = _b.normalChoices;
                        if (this.config.shouldSort || this._isSearching) {
                            normalChoices.sort(filter);
                        }
                        var choiceLimit = rendererableChoices.length;
                        var sortedChoices = this._isSelectOneElement ? __spreadArray(__spreadArray([], placeholderChoices, true), normalChoices, true) : normalChoices;
                        if (this._isSearching) {
                            choiceLimit = searchResultLimit;
                        } else if (renderChoiceLimit && renderChoiceLimit > 0 && !withinGroup) {
                            choiceLimit = renderChoiceLimit;
                        }
                        for (var i = 0; i < choiceLimit; i += 1) {
                            if (sortedChoices[i]) {
                                appendChoice(sortedChoices[i]);
                            }
                        }
                        return fragment;
                    };
                    Choices.prototype._createItemsFragment = function(items, fragment) {
                        var _this = this;
                        if (fragment === void 0) {
                            fragment = document.createDocumentFragment();
                        }
                        var _a = this.config, shouldSortItems = _a.shouldSortItems, sorter = _a.sorter, removeItemButton = _a.removeItemButton;
                        if (shouldSortItems && !this._isSelectOneElement) {
                            items.sort(sorter);
                        }
                        if (this._isTextElement) {
                            this.passedElement.value = items.map(function(_a) {
                                var value = _a.value;
                                return value;
                            }).join(this.config.delimiter);
                        } else {
                            this.passedElement.options = items;
                        }
                        var addItemToFragment = function(item) {
                            var listItem = _this._getTemplate("item", item, removeItemButton);
                            fragment.appendChild(listItem);
                        };
                        items.forEach(addItemToFragment);
                        return fragment;
                    };
                    Choices.prototype._triggerChange = function(value) {
                        if (value === undefined || value === null) {
                            return;
                        }
                        this.passedElement.triggerEvent(constants_1.EVENTS.change, {
                            value: value
                        });
                    };
                    Choices.prototype._selectPlaceholderChoice = function(placeholderChoice) {
                        this._addItem({
                            value: placeholderChoice.value,
                            label: placeholderChoice.label,
                            choiceId: placeholderChoice.id,
                            groupId: placeholderChoice.groupId,
                            placeholder: placeholderChoice.placeholder
                        });
                        this._triggerChange(placeholderChoice.value);
                    };
                    Choices.prototype._handleButtonAction = function(activeItems, element) {
                        if (!activeItems || !element || !this.config.removeItems || !this.config.removeItemButton) {
                            return;
                        }
                        var itemId = element.parentNode && element.parentNode.dataset.id;
                        var itemToRemove = itemId && activeItems.find(function(item) {
                            return item.id === parseInt(itemId, 10);
                        });
                        if (!itemToRemove) {
                            return;
                        }
                        this._removeItem(itemToRemove);
                        this._triggerChange(itemToRemove.value);
                        if (this._isSelectOneElement && this._store.placeholderChoice) {
                            this._selectPlaceholderChoice(this._store.placeholderChoice);
                        }
                    };
                    Choices.prototype._handleItemAction = function(activeItems, element, hasShiftKey) {
                        var _this = this;
                        if (hasShiftKey === void 0) {
                            hasShiftKey = false;
                        }
                        if (!activeItems || !element || !this.config.removeItems || this._isSelectOneElement) {
                            return;
                        }
                        var passedId = element.dataset.id;
                        activeItems.forEach(function(item) {
                            if (item.id === parseInt("".concat(passedId), 10) && !item.highlighted) {
                                _this.highlightItem(item);
                            } else if (!hasShiftKey && item.highlighted) {
                                _this.unhighlightItem(item);
                            }
                        });
                        this.input.focus();
                    };
                    Choices.prototype._handleChoiceAction = function(activeItems, element) {
                        if (!activeItems || !element) {
                            return;
                        }
                        var id = element.dataset.id;
                        var choice = id && this._store.getChoiceById(id);
                        if (!choice) {
                            return;
                        }
                        var passedKeyCode = activeItems[0] && activeItems[0].keyCode ? activeItems[0].keyCode : undefined;
                        var hasActiveDropdown = this.dropdown.isActive;
                        choice.keyCode = passedKeyCode;
                        this.passedElement.triggerEvent(constants_1.EVENTS.choice, {
                            choice: choice
                        });
                        if (!choice.selected && !choice.disabled) {
                            var canAddItem = this._canAddItem(activeItems, choice.value);
                            if (canAddItem.response) {
                                this._addItem({
                                    value: choice.value,
                                    label: choice.label,
                                    choiceId: choice.id,
                                    groupId: choice.groupId,
                                    customProperties: choice.customProperties,
                                    placeholder: choice.placeholder,
                                    keyCode: choice.keyCode
                                });
                                this._triggerChange(choice.value);
                            }
                        }
                        this.clearInput();
                        if (hasActiveDropdown && this._isSelectOneElement) {
                            this.hideDropdown(true);
                            this.containerOuter.focus();
                        }
                    };
                    Choices.prototype._handleBackspace = function(activeItems) {
                        if (!this.config.removeItems || !activeItems) {
                            return;
                        }
                        var lastItem = activeItems[activeItems.length - 1];
                        var hasHighlightedItems = activeItems.some(function(item) {
                            return item.highlighted;
                        });
                        if (this.config.editItems && !hasHighlightedItems && lastItem) {
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
                    Choices.prototype._startLoading = function() {
                        this._store.dispatch((0, misc_1.setIsLoading)(true));
                    };
                    Choices.prototype._stopLoading = function() {
                        this._store.dispatch((0, misc_1.setIsLoading)(false));
                    };
                    Choices.prototype._handleLoadingState = function(setLoading) {
                        if (setLoading === void 0) {
                            setLoading = true;
                        }
                        var placeholderItem = this.itemList.getChild(".".concat(this.config.classNames.placeholder));
                        if (setLoading) {
                            this.disable();
                            this.containerOuter.addLoadingState();
                            if (this._isSelectOneElement) {
                                if (!placeholderItem) {
                                    placeholderItem = this._getTemplate("placeholder", this.config.loadingText);
                                    if (placeholderItem) {
                                        this.itemList.append(placeholderItem);
                                    }
                                } else {
                                    placeholderItem.innerHTML = this.config.loadingText;
                                }
                            } else {
                                this.input.placeholder = this.config.loadingText;
                            }
                        } else {
                            this.enable();
                            this.containerOuter.removeLoadingState();
                            if (this._isSelectOneElement) {
                                if (placeholderItem) {
                                    placeholderItem.innerHTML = this._placeholderValue || "";
                                }
                            } else {
                                this.input.placeholder = this._placeholderValue || "";
                            }
                        }
                    };
                    Choices.prototype._handleSearch = function(value) {
                        if (!this.input.isFocussed) {
                            return;
                        }
                        var choices = this._store.choices;
                        var _a = this.config, searchFloor = _a.searchFloor, searchChoices = _a.searchChoices;
                        var hasUnactiveChoices = choices.some(function(option) {
                            return !option.active;
                        });
                        if (value !== null && typeof value !== "undefined" && value.length >= searchFloor) {
                            var resultCount = searchChoices ? this._searchChoices(value) : 0;
                            this.passedElement.triggerEvent(constants_1.EVENTS.search, {
                                value: value,
                                resultCount: resultCount
                            });
                        } else if (hasUnactiveChoices) {
                            this._isSearching = false;
                            this._store.dispatch((0, choices_1.activateChoices)(true));
                        }
                    };
                    Choices.prototype._canAddChoice = function(activeItems, value) {
                        var canAddItem = this._canAddItem(activeItems, value);
                        canAddItem.response = this.config.addChoices && canAddItem.response;
                        return canAddItem;
                    };
                    Choices.prototype._canAddItem = function(activeItems, value) {
                        var canAddItem = true;
                        var notice = typeof this.config.addItemText === "function" ? this.config.addItemText(value) : this.config.addItemText;
                        if (!this._isSelectOneElement) {
                            var isDuplicateValue = (0, utils_1.existsInArray)(activeItems, value);
                            if (this.config.maxItemCount > 0 && this.config.maxItemCount <= activeItems.length) {
                                canAddItem = false;
                                notice = typeof this.config.maxItemText === "function" ? this.config.maxItemText(this.config.maxItemCount) : this.config.maxItemText;
                            }
                            if (!this.config.duplicateItemsAllowed && isDuplicateValue && canAddItem) {
                                canAddItem = false;
                                notice = typeof this.config.uniqueItemText === "function" ? this.config.uniqueItemText(value) : this.config.uniqueItemText;
                            }
                            if (this._isTextElement && this.config.addItems && canAddItem && typeof this.config.addItemFilter === "function" && !this.config.addItemFilter(value)) {
                                canAddItem = false;
                                notice = typeof this.config.customAddItemText === "function" ? this.config.customAddItemText(value) : this.config.customAddItemText;
                            }
                        }
                        return {
                            response: canAddItem,
                            notice: notice
                        };
                    };
                    Choices.prototype._searchChoices = function(value) {
                        var newValue = typeof value === "string" ? value.trim() : value;
                        var currentValue = typeof this._currentValue === "string" ? this._currentValue.trim() : this._currentValue;
                        if (newValue.length < 1 && newValue === "".concat(currentValue, " ")) {
                            return 0;
                        }
                        var haystack = this._store.searchableChoices;
                        var needle = newValue;
                        var options = Object.assign(this.config.fuseOptions, {
                            keys: __spreadArray([], this.config.searchFields, true),
                            includeMatches: true
                        });
                        var fuse = new fuse_js_1.default(haystack, options);
                        var results = fuse.search(needle);
                        this._currentValue = newValue;
                        this._highlightPosition = 0;
                        this._isSearching = true;
                        this._store.dispatch((0, choices_1.filterChoices)(results));
                        return results.length;
                    };
                    Choices.prototype._addEventListeners = function() {
                        var documentElement = document.documentElement;
                        documentElement.addEventListener("touchend", this._onTouchEnd, true);
                        this.containerOuter.element.addEventListener("keydown", this._onKeyDown, true);
                        this.containerOuter.element.addEventListener("mousedown", this._onMouseDown, true);
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
                            this.containerOuter.element.addEventListener("focus", this._onFocus, {
                                passive: true
                            });
                            this.containerOuter.element.addEventListener("blur", this._onBlur, {
                                passive: true
                            });
                        }
                        this.input.element.addEventListener("keyup", this._onKeyUp, {
                            passive: true
                        });
                        this.input.element.addEventListener("focus", this._onFocus, {
                            passive: true
                        });
                        this.input.element.addEventListener("blur", this._onBlur, {
                            passive: true
                        });
                        if (this.input.element.form) {
                            this.input.element.form.addEventListener("reset", this._onFormReset, {
                                passive: true
                            });
                        }
                        this.input.addEventListeners();
                    };
                    Choices.prototype._removeEventListeners = function() {
                        var documentElement = document.documentElement;
                        documentElement.removeEventListener("touchend", this._onTouchEnd, true);
                        this.containerOuter.element.removeEventListener("keydown", this._onKeyDown, true);
                        this.containerOuter.element.removeEventListener("mousedown", this._onMouseDown, true);
                        documentElement.removeEventListener("click", this._onClick);
                        documentElement.removeEventListener("touchmove", this._onTouchMove);
                        this.dropdown.element.removeEventListener("mouseover", this._onMouseOver);
                        if (this._isSelectOneElement) {
                            this.containerOuter.element.removeEventListener("focus", this._onFocus);
                            this.containerOuter.element.removeEventListener("blur", this._onBlur);
                        }
                        this.input.element.removeEventListener("keyup", this._onKeyUp);
                        this.input.element.removeEventListener("focus", this._onFocus);
                        this.input.element.removeEventListener("blur", this._onBlur);
                        if (this.input.element.form) {
                            this.input.element.form.removeEventListener("reset", this._onFormReset);
                        }
                        this.input.removeEventListeners();
                    };
                    Choices.prototype._onKeyDown = function(event) {
                        var keyCode = event.keyCode;
                        var activeItems = this._store.activeItems;
                        var hasFocusedInput = this.input.isFocussed;
                        var hasActiveDropdown = this.dropdown.isActive;
                        var hasItems = this.itemList.hasChildren();
                        var keyString = String.fromCharCode(keyCode);
                        var wasPrintableChar = /[^\x00-\x1F]/.test(keyString);
                        var BACK_KEY = constants_1.KEY_CODES.BACK_KEY, DELETE_KEY = constants_1.KEY_CODES.DELETE_KEY, ENTER_KEY = constants_1.KEY_CODES.ENTER_KEY, A_KEY = constants_1.KEY_CODES.A_KEY, ESC_KEY = constants_1.KEY_CODES.ESC_KEY, UP_KEY = constants_1.KEY_CODES.UP_KEY, DOWN_KEY = constants_1.KEY_CODES.DOWN_KEY, PAGE_UP_KEY = constants_1.KEY_CODES.PAGE_UP_KEY, PAGE_DOWN_KEY = constants_1.KEY_CODES.PAGE_DOWN_KEY;
                        if (!this._isTextElement && !hasActiveDropdown && wasPrintableChar) {
                            this.showDropdown();
                            if (!this.input.isFocussed) {
                                this.input.value += event.key.toLowerCase();
                            }
                        }
                        switch (keyCode) {
                          case A_KEY:
                            return this._onSelectKey(event, hasItems);

                          case ENTER_KEY:
                            return this._onEnterKey(event, activeItems, hasActiveDropdown);

                          case ESC_KEY:
                            return this._onEscapeKey(hasActiveDropdown);

                          case UP_KEY:
                          case PAGE_UP_KEY:
                          case DOWN_KEY:
                          case PAGE_DOWN_KEY:
                            return this._onDirectionKey(event, hasActiveDropdown);

                          case DELETE_KEY:
                          case BACK_KEY:
                            return this._onDeleteKey(event, activeItems, hasFocusedInput);

                          default:
                        }
                    };
                    Choices.prototype._onKeyUp = function(_a) {
                        var target = _a.target, keyCode = _a.keyCode;
                        var value = this.input.value;
                        var activeItems = this._store.activeItems;
                        var canAddItem = this._canAddItem(activeItems, value);
                        var backKey = constants_1.KEY_CODES.BACK_KEY, deleteKey = constants_1.KEY_CODES.DELETE_KEY;
                        if (this._isTextElement) {
                            var canShowDropdownNotice = canAddItem.notice && value;
                            if (canShowDropdownNotice) {
                                var dropdownItem = this._getTemplate("notice", canAddItem.notice);
                                this.dropdown.element.innerHTML = dropdownItem.outerHTML;
                                this.showDropdown(true);
                            } else {
                                this.hideDropdown(true);
                            }
                        } else {
                            var wasRemovalKeyCode = keyCode === backKey || keyCode === deleteKey;
                            var userHasRemovedValue = wasRemovalKeyCode && target && !target.value;
                            var canReactivateChoices = !this._isTextElement && this._isSearching;
                            var canSearch = this._canSearch && canAddItem.response;
                            if (userHasRemovedValue && canReactivateChoices) {
                                this._isSearching = false;
                                this._store.dispatch((0, choices_1.activateChoices)(true));
                            } else if (canSearch) {
                                this._handleSearch(this.input.rawValue);
                            }
                        }
                        this._canSearch = this.config.searchEnabled;
                    };
                    Choices.prototype._onSelectKey = function(event, hasItems) {
                        var ctrlKey = event.ctrlKey, metaKey = event.metaKey;
                        var hasCtrlDownKeyPressed = ctrlKey || metaKey;
                        if (hasCtrlDownKeyPressed && hasItems) {
                            this._canSearch = false;
                            var shouldHightlightAll = this.config.removeItems && !this.input.value && this.input.element === document.activeElement;
                            if (shouldHightlightAll) {
                                this.highlightAll();
                            }
                        }
                    };
                    Choices.prototype._onEnterKey = function(event, activeItems, hasActiveDropdown) {
                        var target = event.target;
                        var enterKey = constants_1.KEY_CODES.ENTER_KEY;
                        var targetWasButton = target && target.hasAttribute("data-button");
                        var addedItem = false;
                        if (target && target.value) {
                            var value = this.input.value;
                            var canAddItem = this._canAddItem(activeItems, value);
                            var canAddChoice = this._canAddChoice(activeItems, value);
                            if (this._isTextElement && canAddItem.response || !this._isTextElement && canAddChoice.response) {
                                this.hideDropdown(true);
                                this._addItem({
                                    value: value
                                });
                                this._triggerChange(value);
                                this.clearInput();
                                addedItem = true;
                            }
                        }
                        if (targetWasButton) {
                            this._handleButtonAction(activeItems, target);
                            event.preventDefault();
                        }
                        if (hasActiveDropdown) {
                            var highlightedChoice = this.dropdown.getChild(".".concat(this.config.classNames.highlightedState));
                            if (highlightedChoice) {
                                if (addedItem) {
                                    this.unhighlightAll();
                                } else {
                                    if (activeItems[0]) {
                                        activeItems[0].keyCode = enterKey;
                                    }
                                    this._handleChoiceAction(activeItems, highlightedChoice);
                                }
                            }
                            event.preventDefault();
                        } else if (this._isSelectOneElement) {
                            this.showDropdown();
                            event.preventDefault();
                        }
                    };
                    Choices.prototype._onEscapeKey = function(hasActiveDropdown) {
                        if (hasActiveDropdown) {
                            this.hideDropdown(true);
                            this.containerOuter.focus();
                        }
                    };
                    Choices.prototype._onDirectionKey = function(event, hasActiveDropdown) {
                        var keyCode = event.keyCode, metaKey = event.metaKey;
                        var downKey = constants_1.KEY_CODES.DOWN_KEY, pageUpKey = constants_1.KEY_CODES.PAGE_UP_KEY, pageDownKey = constants_1.KEY_CODES.PAGE_DOWN_KEY;
                        if (hasActiveDropdown || this._isSelectOneElement) {
                            this.showDropdown();
                            this._canSearch = false;
                            var directionInt = keyCode === downKey || keyCode === pageDownKey ? 1 : -1;
                            var skipKey = metaKey || keyCode === pageDownKey || keyCode === pageUpKey;
                            var selectableChoiceIdentifier = "[data-choice-selectable]";
                            var nextEl = void 0;
                            if (skipKey) {
                                if (directionInt > 0) {
                                    nextEl = this.dropdown.element.querySelector("".concat(selectableChoiceIdentifier, ":last-of-type"));
                                } else {
                                    nextEl = this.dropdown.element.querySelector(selectableChoiceIdentifier);
                                }
                            } else {
                                var currentEl = this.dropdown.element.querySelector(".".concat(this.config.classNames.highlightedState));
                                if (currentEl) {
                                    nextEl = (0, utils_1.getAdjacentEl)(currentEl, selectableChoiceIdentifier, directionInt);
                                } else {
                                    nextEl = this.dropdown.element.querySelector(selectableChoiceIdentifier);
                                }
                            }
                            if (nextEl) {
                                if (!(0, utils_1.isScrolledIntoView)(nextEl, this.choiceList.element, directionInt)) {
                                    this.choiceList.scrollToChildElement(nextEl, directionInt);
                                }
                                this._highlightChoice(nextEl);
                            }
                            event.preventDefault();
                        }
                    };
                    Choices.prototype._onDeleteKey = function(event, activeItems, hasFocusedInput) {
                        var target = event.target;
                        if (!this._isSelectOneElement && !target.value && hasFocusedInput) {
                            this._handleBackspace(activeItems);
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
                            var isOnScrollbar = this._direction === "ltr" ? event.offsetX >= firstChoice.offsetWidth : event.offsetX < firstChoice.offsetLeft;
                            this._isScrollingOnIe = isOnScrollbar;
                        }
                        if (target === this.input.element) {
                            return;
                        }
                        var item = target.closest("[data-button],[data-item],[data-choice]");
                        if (item instanceof HTMLElement) {
                            var hasShiftKey = event.shiftKey;
                            var activeItems = this._store.activeItems;
                            var dataset = item.dataset;
                            if ("button" in dataset) {
                                this._handleButtonAction(activeItems, item);
                            } else if ("item" in dataset) {
                                this._handleItemAction(activeItems, item, hasShiftKey);
                            } else if ("choice" in dataset) {
                                this._handleChoiceAction(activeItems, item);
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
                        var clickWasWithinContainer = this.containerOuter.element.contains(target);
                        if (clickWasWithinContainer) {
                            if (!this.dropdown.isActive && !this.containerOuter.isDisabled) {
                                if (this._isTextElement) {
                                    if (document.activeElement !== this.input.element) {
                                        this.input.focus();
                                    }
                                } else {
                                    this.showDropdown();
                                    this.containerOuter.focus();
                                }
                            } else if (this._isSelectOneElement && target !== this.input.element && !this.dropdown.element.contains(target)) {
                                this.hideDropdown();
                            }
                        } else {
                            var hasHighlightedItems = this._store.highlightedActiveItems.length > 0;
                            if (hasHighlightedItems) {
                                this.unhighlightAll();
                            }
                            this.containerOuter.removeFocusState();
                            this.hideDropdown(true);
                        }
                    };
                    Choices.prototype._onFocus = function(_a) {
                        var _b;
                        var _this = this;
                        var target = _a.target;
                        var focusWasWithinContainer = target && this.containerOuter.element.contains(target);
                        if (!focusWasWithinContainer) {
                            return;
                        }
                        var focusActions = (_b = {}, _b[constants_1.TEXT_TYPE] = function() {
                            if (target === _this.input.element) {
                                _this.containerOuter.addFocusState();
                            }
                        }, _b[constants_1.SELECT_ONE_TYPE] = function() {
                            _this.containerOuter.addFocusState();
                            if (target === _this.input.element) {
                                _this.showDropdown(true);
                            }
                        }, _b[constants_1.SELECT_MULTIPLE_TYPE] = function() {
                            if (target === _this.input.element) {
                                _this.showDropdown(true);
                                _this.containerOuter.addFocusState();
                            }
                        }, _b);
                        focusActions[this.passedElement.element.type]();
                    };
                    Choices.prototype._onBlur = function(_a) {
                        var _b;
                        var _this = this;
                        var target = _a.target;
                        var blurWasWithinContainer = target && this.containerOuter.element.contains(target);
                        if (blurWasWithinContainer && !this._isScrollingOnIe) {
                            var activeItems = this._store.activeItems;
                            var hasHighlightedItems_1 = activeItems.some(function(item) {
                                return item.highlighted;
                            });
                            var blurActions = (_b = {}, _b[constants_1.TEXT_TYPE] = function() {
                                if (target === _this.input.element) {
                                    _this.containerOuter.removeFocusState();
                                    if (hasHighlightedItems_1) {
                                        _this.unhighlightAll();
                                    }
                                    _this.hideDropdown(true);
                                }
                            }, _b[constants_1.SELECT_ONE_TYPE] = function() {
                                _this.containerOuter.removeFocusState();
                                if (target === _this.input.element || target === _this.containerOuter.element && !_this._canSearch) {
                                    _this.hideDropdown(true);
                                }
                            }, _b[constants_1.SELECT_MULTIPLE_TYPE] = function() {
                                if (target === _this.input.element) {
                                    _this.containerOuter.removeFocusState();
                                    _this.hideDropdown(true);
                                    if (hasHighlightedItems_1) {
                                        _this.unhighlightAll();
                                    }
                                }
                            }, _b);
                            blurActions[this.passedElement.element.type]();
                        } else {
                            this._isScrollingOnIe = false;
                            this.input.element.focus();
                        }
                    };
                    Choices.prototype._onFormReset = function() {
                        this._store.dispatch((0, misc_1.resetTo)(this._initialState));
                    };
                    Choices.prototype._highlightChoice = function(el) {
                        var _this = this;
                        if (el === void 0) {
                            el = null;
                        }
                        var choices = Array.from(this.dropdown.element.querySelectorAll("[data-choice-selectable]"));
                        if (!choices.length) {
                            return;
                        }
                        var passedEl = el;
                        var highlightedChoices = Array.from(this.dropdown.element.querySelectorAll(".".concat(this.config.classNames.highlightedState)));
                        highlightedChoices.forEach(function(choice) {
                            choice.classList.remove(_this.config.classNames.highlightedState);
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
                        passedEl.classList.add(this.config.classNames.highlightedState);
                        passedEl.setAttribute("aria-selected", "true");
                        this.passedElement.triggerEvent(constants_1.EVENTS.highlightChoice, {
                            el: passedEl
                        });
                        if (this.dropdown.isActive) {
                            this.input.setActiveDescendant(passedEl.id);
                            this.containerOuter.setActiveDescendant(passedEl.id);
                        }
                    };
                    Choices.prototype._addItem = function(_a) {
                        var value = _a.value, _b = _a.label, label = _b === void 0 ? null : _b, _c = _a.choiceId, choiceId = _c === void 0 ? -1 : _c, _d = _a.groupId, groupId = _d === void 0 ? -1 : _d, _e = _a.customProperties, customProperties = _e === void 0 ? {} : _e, _f = _a.placeholder, placeholder = _f === void 0 ? false : _f, _g = _a.keyCode, keyCode = _g === void 0 ? -1 : _g;
                        var passedValue = typeof value === "string" ? value.trim() : value;
                        var items = this._store.items;
                        var passedLabel = label || passedValue;
                        var passedOptionId = choiceId || -1;
                        var group = groupId >= 0 ? this._store.getGroupById(groupId) : null;
                        var id = items ? items.length + 1 : 1;
                        if (this.config.prependValue) {
                            passedValue = this.config.prependValue + passedValue.toString();
                        }
                        if (this.config.appendValue) {
                            passedValue += this.config.appendValue.toString();
                        }
                        this._store.dispatch((0, items_1.addItem)({
                            value: passedValue,
                            label: passedLabel,
                            id: id,
                            choiceId: passedOptionId,
                            groupId: groupId,
                            customProperties: customProperties,
                            placeholder: placeholder,
                            keyCode: keyCode
                        }));
                        if (this._isSelectOneElement) {
                            this.removeActiveItems(id);
                        }
                        this.passedElement.triggerEvent(constants_1.EVENTS.addItem, {
                            id: id,
                            value: passedValue,
                            label: passedLabel,
                            customProperties: customProperties,
                            groupValue: group && group.value ? group.value : null,
                            keyCode: keyCode
                        });
                    };
                    Choices.prototype._removeItem = function(item) {
                        var id = item.id, value = item.value, label = item.label, customProperties = item.customProperties, choiceId = item.choiceId, groupId = item.groupId;
                        var group = groupId && groupId >= 0 ? this._store.getGroupById(groupId) : null;
                        if (!id || !choiceId) {
                            return;
                        }
                        this._store.dispatch((0, items_1.removeItem)(id, choiceId));
                        this.passedElement.triggerEvent(constants_1.EVENTS.removeItem, {
                            id: id,
                            value: value,
                            label: label,
                            customProperties: customProperties,
                            groupValue: group && group.value ? group.value : null
                        });
                    };
                    Choices.prototype._addChoice = function(_a) {
                        var value = _a.value, _b = _a.label, label = _b === void 0 ? null : _b, _c = _a.isSelected, isSelected = _c === void 0 ? false : _c, _d = _a.isDisabled, isDisabled = _d === void 0 ? false : _d, _e = _a.groupId, groupId = _e === void 0 ? -1 : _e, _f = _a.customProperties, customProperties = _f === void 0 ? {} : _f, _g = _a.placeholder, placeholder = _g === void 0 ? false : _g, _h = _a.keyCode, keyCode = _h === void 0 ? -1 : _h;
                        if (typeof value === "undefined" || value === null) {
                            return;
                        }
                        var choices = this._store.choices;
                        var choiceLabel = label || value;
                        var choiceId = choices ? choices.length + 1 : 1;
                        var choiceElementId = "".concat(this._baseId, "-").concat(this._idNames.itemChoice, "-").concat(choiceId);
                        this._store.dispatch((0, choices_1.addChoice)({
                            id: choiceId,
                            groupId: groupId,
                            elementId: choiceElementId,
                            value: value,
                            label: choiceLabel,
                            disabled: isDisabled,
                            customProperties: customProperties,
                            placeholder: placeholder,
                            keyCode: keyCode
                        }));
                        if (isSelected) {
                            this._addItem({
                                value: value,
                                label: choiceLabel,
                                choiceId: choiceId,
                                customProperties: customProperties,
                                placeholder: placeholder,
                                keyCode: keyCode
                            });
                        }
                    };
                    Choices.prototype._addGroup = function(_a) {
                        var _this = this;
                        var group = _a.group, id = _a.id, _b = _a.valueKey, valueKey = _b === void 0 ? "value" : _b, _c = _a.labelKey, labelKey = _c === void 0 ? "label" : _c;
                        var groupChoices = (0, utils_1.isType)("Object", group) ? group.choices : Array.from(group.getElementsByTagName("OPTION"));
                        var groupId = id || Math.floor(new Date().valueOf() * Math.random());
                        var isDisabled = group.disabled ? group.disabled : false;
                        if (groupChoices) {
                            this._store.dispatch((0, groups_1.addGroup)({
                                value: group.label,
                                id: groupId,
                                active: true,
                                disabled: isDisabled
                            }));
                            var addGroupChoices = function(choice) {
                                var isOptDisabled = choice.disabled || choice.parentNode && choice.parentNode.disabled;
                                _this._addChoice({
                                    value: choice[valueKey],
                                    label: (0, utils_1.isType)("Object", choice) ? choice[labelKey] : choice.innerHTML,
                                    isSelected: choice.selected,
                                    isDisabled: isOptDisabled,
                                    groupId: groupId,
                                    customProperties: choice.customProperties,
                                    placeholder: choice.placeholder
                                });
                            };
                            groupChoices.forEach(addGroupChoices);
                        } else {
                            this._store.dispatch((0, groups_1.addGroup)({
                                value: group.label,
                                id: group.id,
                                active: false,
                                disabled: group.disabled
                            }));
                        }
                    };
                    Choices.prototype._getTemplate = function(template) {
                        var _a;
                        var args = [];
                        for (var _i = 1; _i < arguments.length; _i++) {
                            args[_i - 1] = arguments[_i];
                        }
                        return (_a = this._templates[template]).call.apply(_a, __spreadArray([ this, this.config ], args, false));
                    };
                    Choices.prototype._createTemplates = function() {
                        var callbackOnCreateTemplates = this.config.callbackOnCreateTemplates;
                        var userTemplates = {};
                        if (callbackOnCreateTemplates && typeof callbackOnCreateTemplates === "function") {
                            userTemplates = callbackOnCreateTemplates.call(this, utils_1.strToEl);
                        }
                        this._templates = (0, deepmerge_1.default)(templates_1.default, userTemplates);
                    };
                    Choices.prototype._createElements = function() {
                        this.containerOuter = new components_1.Container({
                            element: this._getTemplate("containerOuter", this._direction, this._isSelectElement, this._isSelectOneElement, this.config.searchEnabled, this.passedElement.element.type, this.config.labelId),
                            classNames: this.config.classNames,
                            type: this.passedElement.element.type,
                            position: this.config.position
                        });
                        this.containerInner = new components_1.Container({
                            element: this._getTemplate("containerInner"),
                            classNames: this.config.classNames,
                            type: this.passedElement.element.type,
                            position: this.config.position
                        });
                        this.input = new components_1.Input({
                            element: this._getTemplate("input", this._placeholderValue),
                            classNames: this.config.classNames,
                            type: this.passedElement.element.type,
                            preventPaste: !this.config.paste
                        });
                        this.choiceList = new components_1.List({
                            element: this._getTemplate("choiceList", this._isSelectOneElement)
                        });
                        this.itemList = new components_1.List({
                            element: this._getTemplate("itemList", this._isSelectOneElement)
                        });
                        this.dropdown = new components_1.Dropdown({
                            element: this._getTemplate("dropdown"),
                            classNames: this.config.classNames,
                            type: this.passedElement.element.type
                        });
                    };
                    Choices.prototype._createStructure = function() {
                        this.passedElement.conceal();
                        this.containerInner.wrap(this.passedElement.element);
                        this.containerOuter.wrap(this.containerInner.element);
                        if (this._isSelectOneElement) {
                            this.input.placeholder = this.config.searchPlaceholderValue || "";
                        } else if (this._placeholderValue) {
                            this.input.placeholder = this._placeholderValue;
                            this.input.setWidth();
                        }
                        this.containerOuter.element.appendChild(this.containerInner.element);
                        this.containerOuter.element.appendChild(this.dropdown.element);
                        this.containerInner.element.appendChild(this.itemList.element);
                        if (!this._isTextElement) {
                            this.dropdown.element.appendChild(this.choiceList.element);
                        }
                        if (!this._isSelectOneElement) {
                            this.containerInner.element.appendChild(this.input.element);
                        } else if (this.config.searchEnabled) {
                            this.dropdown.element.insertBefore(this.input.element, this.dropdown.element.firstChild);
                        }
                        if (this._isSelectElement) {
                            this._highlightPosition = 0;
                            this._isSearching = false;
                            this._startLoading();
                            this._addPredefinedChoices(this._presetChoices);
                            this._stopLoading();
                        }
                        if (this._isTextElement) {
                            this._addPredefinedItems(this._presetItems);
                        }
                    };
                    Choices.prototype._addPredefinedGroups = function(groups) {
                        var _this = this;
                        var placeholderChoice = this.passedElement.placeholderOption;
                        if (placeholderChoice && placeholderChoice.parentNode && placeholderChoice.parentNode.tagName === "SELECT") {
                            this._addChoice({
                                value: placeholderChoice.value,
                                label: placeholderChoice.innerHTML,
                                isSelected: placeholderChoice.selected,
                                isDisabled: placeholderChoice.disabled,
                                placeholder: true
                            });
                        }
                        groups.forEach(function(group) {
                            return _this._addGroup({
                                group: group,
                                id: group.id || null
                            });
                        });
                    };
                    Choices.prototype._addPredefinedChoices = function(choices) {
                        var _this = this;
                        if (this.config.shouldSort) {
                            choices.sort(this.config.sorter);
                        }
                        var hasSelectedChoice = choices.some(function(choice) {
                            return choice.selected;
                        });
                        var firstEnabledChoiceIndex = choices.findIndex(function(choice) {
                            return choice.disabled === undefined || !choice.disabled;
                        });
                        choices.forEach(function(choice, index) {
                            var _a = choice.value, value = _a === void 0 ? "" : _a, label = choice.label, customProperties = choice.customProperties, placeholder = choice.placeholder;
                            if (_this._isSelectElement) {
                                if (choice.choices) {
                                    _this._addGroup({
                                        group: choice,
                                        id: choice.id || null
                                    });
                                } else {
                                    var shouldPreselect = _this._isSelectOneElement && !hasSelectedChoice && index === firstEnabledChoiceIndex;
                                    var isSelected = shouldPreselect ? true : choice.selected;
                                    var isDisabled = choice.disabled;
                                    _this._addChoice({
                                        value: value,
                                        label: label,
                                        isSelected: !!isSelected,
                                        isDisabled: !!isDisabled,
                                        placeholder: !!placeholder,
                                        customProperties: customProperties
                                    });
                                }
                            } else {
                                _this._addChoice({
                                    value: value,
                                    label: label,
                                    isSelected: !!choice.selected,
                                    isDisabled: !!choice.disabled,
                                    placeholder: !!choice.placeholder,
                                    customProperties: customProperties
                                });
                            }
                        });
                    };
                    Choices.prototype._addPredefinedItems = function(items) {
                        var _this = this;
                        items.forEach(function(item) {
                            if (typeof item === "object" && item.value) {
                                _this._addItem({
                                    value: item.value,
                                    label: item.label,
                                    choiceId: item.id,
                                    customProperties: item.customProperties,
                                    placeholder: item.placeholder
                                });
                            }
                            if (typeof item === "string") {
                                _this._addItem({
                                    value: item
                                });
                            }
                        });
                    };
                    Choices.prototype._setChoiceOrItem = function(item) {
                        var _this = this;
                        var itemType = (0, utils_1.getType)(item).toLowerCase();
                        var handleType = {
                            object: function() {
                                if (!item.value) {
                                    return;
                                }
                                if (!_this._isTextElement) {
                                    _this._addChoice({
                                        value: item.value,
                                        label: item.label,
                                        isSelected: true,
                                        isDisabled: false,
                                        customProperties: item.customProperties,
                                        placeholder: item.placeholder
                                    });
                                } else {
                                    _this._addItem({
                                        value: item.value,
                                        label: item.label,
                                        choiceId: item.id,
                                        customProperties: item.customProperties,
                                        placeholder: item.placeholder
                                    });
                                }
                            },
                            string: function() {
                                if (!_this._isTextElement) {
                                    _this._addChoice({
                                        value: item,
                                        label: item,
                                        isSelected: true,
                                        isDisabled: false
                                    });
                                } else {
                                    _this._addItem({
                                        value: item
                                    });
                                }
                            }
                        };
                        handleType[itemType]();
                    };
                    Choices.prototype._findAndSelectChoiceByValue = function(value) {
                        var _this = this;
                        var choices = this._store.choices;
                        var foundChoice = choices.find(function(choice) {
                            return _this.config.valueComparer(choice.value, value);
                        });
                        if (foundChoice && !foundChoice.selected) {
                            this._addItem({
                                value: foundChoice.value,
                                label: foundChoice.label,
                                choiceId: foundChoice.id,
                                groupId: foundChoice.groupId,
                                customProperties: foundChoice.customProperties,
                                placeholder: foundChoice.placeholder,
                                keyCode: foundChoice.keyCode
                            });
                        }
                    };
                    Choices.prototype._generatePlaceholderValue = function() {
                        if (this._isSelectElement && this.passedElement.placeholderOption) {
                            var placeholderOption = this.passedElement.placeholderOption;
                            return placeholderOption ? placeholderOption.text : null;
                        }
                        var _a = this.config, placeholder = _a.placeholder, placeholderValue = _a.placeholderValue;
                        var dataset = this.passedElement.element.dataset;
                        if (placeholder) {
                            if (placeholderValue) {
                                return placeholderValue;
                            }
                            if (dataset.placeholder) {
                                return dataset.placeholder;
                            }
                        }
                        return null;
                    };
                    return Choices;
                }();
                exports["default"] = Choices;
            },
            613: function(__unused_webpack_module, exports, __webpack_require__) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                var utils_1 = __webpack_require__(799);
                var constants_1 = __webpack_require__(883);
                var Container = function() {
                    function Container(_a) {
                        var element = _a.element, type = _a.type, classNames = _a.classNames, position = _a.position;
                        this.element = element;
                        this.classNames = classNames;
                        this.type = type;
                        this.position = position;
                        this.isOpen = false;
                        this.isFlipped = false;
                        this.isFocussed = false;
                        this.isDisabled = false;
                        this.isLoading = false;
                        this._onFocus = this._onFocus.bind(this);
                        this._onBlur = this._onBlur.bind(this);
                    }
                    Container.prototype.addEventListeners = function() {
                        this.element.addEventListener("focus", this._onFocus);
                        this.element.addEventListener("blur", this._onBlur);
                    };
                    Container.prototype.removeEventListeners = function() {
                        this.element.removeEventListener("focus", this._onFocus);
                        this.element.removeEventListener("blur", this._onBlur);
                    };
                    Container.prototype.shouldFlip = function(dropdownPos) {
                        if (typeof dropdownPos !== "number") {
                            return false;
                        }
                        var shouldFlip = false;
                        if (this.position === "auto") {
                            shouldFlip = !window.matchMedia("(min-height: ".concat(dropdownPos + 1, "px)")).matches;
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
                    Container.prototype.open = function(dropdownPos) {
                        this.element.classList.add(this.classNames.openState);
                        this.element.setAttribute("aria-expanded", "true");
                        this.isOpen = true;
                        if (this.shouldFlip(dropdownPos)) {
                            this.element.classList.add(this.classNames.flippedState);
                            this.isFlipped = true;
                        }
                    };
                    Container.prototype.close = function() {
                        this.element.classList.remove(this.classNames.openState);
                        this.element.setAttribute("aria-expanded", "false");
                        this.removeActiveDescendant();
                        this.isOpen = false;
                        if (this.isFlipped) {
                            this.element.classList.remove(this.classNames.flippedState);
                            this.isFlipped = false;
                        }
                    };
                    Container.prototype.focus = function() {
                        if (!this.isFocussed) {
                            this.element.focus();
                        }
                    };
                    Container.prototype.addFocusState = function() {
                        this.element.classList.add(this.classNames.focusState);
                    };
                    Container.prototype.removeFocusState = function() {
                        this.element.classList.remove(this.classNames.focusState);
                    };
                    Container.prototype.enable = function() {
                        this.element.classList.remove(this.classNames.disabledState);
                        this.element.removeAttribute("aria-disabled");
                        if (this.type === constants_1.SELECT_ONE_TYPE) {
                            this.element.setAttribute("tabindex", "0");
                        }
                        this.isDisabled = false;
                    };
                    Container.prototype.disable = function() {
                        this.element.classList.add(this.classNames.disabledState);
                        this.element.setAttribute("aria-disabled", "true");
                        if (this.type === constants_1.SELECT_ONE_TYPE) {
                            this.element.setAttribute("tabindex", "-1");
                        }
                        this.isDisabled = true;
                    };
                    Container.prototype.wrap = function(element) {
                        (0, utils_1.wrap)(element, this.element);
                    };
                    Container.prototype.unwrap = function(element) {
                        if (this.element.parentNode) {
                            this.element.parentNode.insertBefore(element, this.element);
                            this.element.parentNode.removeChild(this.element);
                        }
                    };
                    Container.prototype.addLoadingState = function() {
                        this.element.classList.add(this.classNames.loadingState);
                        this.element.setAttribute("aria-busy", "true");
                        this.isLoading = true;
                    };
                    Container.prototype.removeLoadingState = function() {
                        this.element.classList.remove(this.classNames.loadingState);
                        this.element.removeAttribute("aria-busy");
                        this.isLoading = false;
                    };
                    Container.prototype._onFocus = function() {
                        this.isFocussed = true;
                    };
                    Container.prototype._onBlur = function() {
                        this.isFocussed = false;
                    };
                    return Container;
                }();
                exports["default"] = Container;
            },
            217: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                var Dropdown = function() {
                    function Dropdown(_a) {
                        var element = _a.element, type = _a.type, classNames = _a.classNames;
                        this.element = element;
                        this.classNames = classNames;
                        this.type = type;
                        this.isActive = false;
                    }
                    Object.defineProperty(Dropdown.prototype, "distanceFromTopWindow", {
                        get: function() {
                            return this.element.getBoundingClientRect().bottom;
                        },
                        enumerable: false,
                        configurable: true
                    });
                    Dropdown.prototype.getChild = function(selector) {
                        return this.element.querySelector(selector);
                    };
                    Dropdown.prototype.show = function() {
                        this.element.classList.add(this.classNames.activeState);
                        this.element.setAttribute("aria-expanded", "true");
                        this.isActive = true;
                        return this;
                    };
                    Dropdown.prototype.hide = function() {
                        this.element.classList.remove(this.classNames.activeState);
                        this.element.setAttribute("aria-expanded", "false");
                        this.isActive = false;
                        return this;
                    };
                    return Dropdown;
                }();
                exports["default"] = Dropdown;
            },
            520: function(__unused_webpack_module, exports, __webpack_require__) {
                var __importDefault = this && this.__importDefault || function(mod) {
                    return mod && mod.__esModule ? mod : {
                        default: mod
                    };
                };
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.WrappedSelect = exports.WrappedInput = exports.List = exports.Input = exports.Container = exports.Dropdown = void 0;
                var dropdown_1 = __importDefault(__webpack_require__(217));
                exports.Dropdown = dropdown_1.default;
                var container_1 = __importDefault(__webpack_require__(613));
                exports.Container = container_1.default;
                var input_1 = __importDefault(__webpack_require__(11));
                exports.Input = input_1.default;
                var list_1 = __importDefault(__webpack_require__(624));
                exports.List = list_1.default;
                var wrapped_input_1 = __importDefault(__webpack_require__(541));
                exports.WrappedInput = wrapped_input_1.default;
                var wrapped_select_1 = __importDefault(__webpack_require__(982));
                exports.WrappedSelect = wrapped_select_1.default;
            },
            11: function(__unused_webpack_module, exports, __webpack_require__) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                var utils_1 = __webpack_require__(799);
                var constants_1 = __webpack_require__(883);
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
                            return (0, utils_1.sanitise)(this.element.value);
                        },
                        set: function(value) {
                            this.element.value = value;
                        },
                        enumerable: false,
                        configurable: true
                    });
                    Object.defineProperty(Input.prototype, "rawValue", {
                        get: function() {
                            return this.element.value;
                        },
                        enumerable: false,
                        configurable: true
                    });
                    Input.prototype.addEventListeners = function() {
                        this.element.addEventListener("paste", this._onPaste);
                        this.element.addEventListener("input", this._onInput, {
                            passive: true
                        });
                        this.element.addEventListener("focus", this._onFocus, {
                            passive: true
                        });
                        this.element.addEventListener("blur", this._onBlur, {
                            passive: true
                        });
                    };
                    Input.prototype.removeEventListeners = function() {
                        this.element.removeEventListener("input", this._onInput);
                        this.element.removeEventListener("paste", this._onPaste);
                        this.element.removeEventListener("focus", this._onFocus);
                        this.element.removeEventListener("blur", this._onBlur);
                    };
                    Input.prototype.enable = function() {
                        this.element.removeAttribute("disabled");
                        this.isDisabled = false;
                    };
                    Input.prototype.disable = function() {
                        this.element.setAttribute("disabled", "");
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
                        if (this.element.value) {
                            this.element.value = "";
                        }
                        if (setWidth) {
                            this.setWidth();
                        }
                        return this;
                    };
                    Input.prototype.setWidth = function() {
                        var _a = this.element, style = _a.style, value = _a.value, placeholder = _a.placeholder;
                        style.minWidth = "".concat(placeholder.length + 1, "ch");
                        style.width = "".concat(value.length + 1, "ch");
                    };
                    Input.prototype.setActiveDescendant = function(activeDescendantID) {
                        this.element.setAttribute("aria-activedescendant", activeDescendantID);
                    };
                    Input.prototype.removeActiveDescendant = function() {
                        this.element.removeAttribute("aria-activedescendant");
                    };
                    Input.prototype._onInput = function() {
                        if (this.type !== constants_1.SELECT_ONE_TYPE) {
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
                exports["default"] = Input;
            },
            624: function(__unused_webpack_module, exports, __webpack_require__) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                var constants_1 = __webpack_require__(883);
                var List = function() {
                    function List(_a) {
                        var element = _a.element;
                        this.element = element;
                        this.scrollPos = this.element.scrollTop;
                        this.height = this.element.offsetHeight;
                    }
                    List.prototype.clear = function() {
                        this.element.innerHTML = "";
                    };
                    List.prototype.append = function(node) {
                        this.element.appendChild(node);
                    };
                    List.prototype.getChild = function(selector) {
                        return this.element.querySelector(selector);
                    };
                    List.prototype.hasChildren = function() {
                        return this.element.hasChildNodes();
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
                        var strength = constants_1.SCROLLING_SPEED;
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
                exports["default"] = List;
            },
            730: function(__unused_webpack_module, exports, __webpack_require__) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                var utils_1 = __webpack_require__(799);
                var WrappedElement = function() {
                    function WrappedElement(_a) {
                        var element = _a.element, classNames = _a.classNames;
                        this.element = element;
                        this.classNames = classNames;
                        if (!(element instanceof HTMLInputElement) && !(element instanceof HTMLSelectElement)) {
                            throw new TypeError("Invalid element passed");
                        }
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
                            this.element.value = value;
                        },
                        enumerable: false,
                        configurable: true
                    });
                    WrappedElement.prototype.conceal = function() {
                        this.element.classList.add(this.classNames.input);
                        this.element.hidden = true;
                        this.element.tabIndex = -1;
                        var origStyle = this.element.getAttribute("style");
                        if (origStyle) {
                            this.element.setAttribute("data-choice-orig-style", origStyle);
                        }
                        this.element.setAttribute("data-choice", "active");
                    };
                    WrappedElement.prototype.reveal = function() {
                        this.element.classList.remove(this.classNames.input);
                        this.element.hidden = false;
                        this.element.removeAttribute("tabindex");
                        var origStyle = this.element.getAttribute("data-choice-orig-style");
                        if (origStyle) {
                            this.element.removeAttribute("data-choice-orig-style");
                            this.element.setAttribute("style", origStyle);
                        } else {
                            this.element.removeAttribute("style");
                        }
                        this.element.removeAttribute("data-choice");
                        this.element.value = this.element.value;
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
                        (0, utils_1.dispatchEvent)(this.element, eventType, data);
                    };
                    return WrappedElement;
                }();
                exports["default"] = WrappedElement;
            },
            541: function(__unused_webpack_module, exports, __webpack_require__) {
                var __extends = this && this.__extends || function() {
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
                    return function(d, b) {
                        if (typeof b !== "function" && b !== null) throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
                        extendStatics(d, b);
                        function __() {
                            this.constructor = d;
                        }
                        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, 
                        new __());
                    };
                }();
                var __importDefault = this && this.__importDefault || function(mod) {
                    return mod && mod.__esModule ? mod : {
                        default: mod
                    };
                };
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                var wrapped_element_1 = __importDefault(__webpack_require__(730));
                var WrappedInput = function(_super) {
                    __extends(WrappedInput, _super);
                    function WrappedInput(_a) {
                        var element = _a.element, classNames = _a.classNames, delimiter = _a.delimiter;
                        var _this = _super.call(this, {
                            element: element,
                            classNames: classNames
                        }) || this;
                        _this.delimiter = delimiter;
                        return _this;
                    }
                    Object.defineProperty(WrappedInput.prototype, "value", {
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
                    return WrappedInput;
                }(wrapped_element_1.default);
                exports["default"] = WrappedInput;
            },
            982: function(__unused_webpack_module, exports, __webpack_require__) {
                var __extends = this && this.__extends || function() {
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
                    return function(d, b) {
                        if (typeof b !== "function" && b !== null) throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
                        extendStatics(d, b);
                        function __() {
                            this.constructor = d;
                        }
                        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, 
                        new __());
                    };
                }();
                var __importDefault = this && this.__importDefault || function(mod) {
                    return mod && mod.__esModule ? mod : {
                        default: mod
                    };
                };
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                var utils_1 = __webpack_require__(799);
                var wrapped_element_1 = __importDefault(__webpack_require__(730));
                var htmlElementGuards_1 = __webpack_require__(858);
                var htmlElementGuards_2 = __webpack_require__(858);
                var WrappedSelect = function(_super) {
                    __extends(WrappedSelect, _super);
                    function WrappedSelect(_a) {
                        var element = _a.element, classNames = _a.classNames, template = _a.template;
                        var _this = _super.call(this, {
                            element: element,
                            classNames: classNames
                        }) || this;
                        _this.template = template;
                        return _this;
                    }
                    Object.defineProperty(WrappedSelect.prototype, "placeholderOption", {
                        get: function() {
                            return this.element.querySelector('option[value=""]') || this.element.querySelector("option[placeholder]");
                        },
                        enumerable: false,
                        configurable: true
                    });
                    Object.defineProperty(WrappedSelect.prototype, "optionGroups", {
                        get: function() {
                            return Array.from(this.element.getElementsByTagName("OPTGROUP"));
                        },
                        enumerable: false,
                        configurable: true
                    });
                    Object.defineProperty(WrappedSelect.prototype, "options", {
                        get: function() {
                            return Array.from(this.element.options);
                        },
                        set: function(options) {
                            var _this = this;
                            var fragment = document.createDocumentFragment();
                            var addOptionToFragment = function(data) {
                                var option = _this.template(data);
                                fragment.appendChild(option);
                            };
                            options.forEach(function(optionData) {
                                return addOptionToFragment(optionData);
                            });
                            this.appendDocFragment(fragment);
                        },
                        enumerable: false,
                        configurable: true
                    });
                    WrappedSelect.prototype.optionsAsChoices = function() {
                        var choices = [];
                        for (var _i = 0, _a = Array.from(this.element.querySelectorAll(":scope > *")); _i < _a.length; _i++) {
                            var e = _a[_i];
                            if ((0, htmlElementGuards_2.isHTMLOption)(e)) {
                                choices.push(this._optionToChoice(e));
                            } else if ((0, htmlElementGuards_1.isHTMLOptgroup)(e)) {
                                choices.push(this._optgroupToChoice(e));
                            }
                        }
                        return choices;
                    };
                    WrappedSelect.prototype._optionToChoice = function(option) {
                        return {
                            value: option.value,
                            label: option.innerHTML,
                            selected: !!option.selected,
                            disabled: option.disabled || this.element.disabled,
                            placeholder: option.value === "" || option.hasAttribute("placeholder"),
                            customProperties: (0, utils_1.parseCustomProperties)(option.dataset.customProperties)
                        };
                    };
                    WrappedSelect.prototype._optgroupToChoice = function(optgroup) {
                        var _this = this;
                        return {
                            label: optgroup.label || "",
                            disabled: !!optgroup.disabled,
                            choices: Array.from(optgroup.querySelectorAll("option")).map(function(option) {
                                return _this._optionToChoice(option);
                            })
                        };
                    };
                    WrappedSelect.prototype.appendDocFragment = function(fragment) {
                        this.element.innerHTML = "";
                        this.element.appendChild(fragment);
                    };
                    return WrappedSelect;
                }(wrapped_element_1.default);
                exports["default"] = WrappedSelect;
            },
            883: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.SCROLLING_SPEED = exports.SELECT_MULTIPLE_TYPE = exports.SELECT_ONE_TYPE = exports.TEXT_TYPE = exports.KEY_CODES = exports.ACTION_TYPES = exports.EVENTS = void 0;
                exports.EVENTS = {
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
                exports.ACTION_TYPES = {
                    ADD_CHOICE: "ADD_CHOICE",
                    FILTER_CHOICES: "FILTER_CHOICES",
                    ACTIVATE_CHOICES: "ACTIVATE_CHOICES",
                    CLEAR_CHOICES: "CLEAR_CHOICES",
                    ADD_GROUP: "ADD_GROUP",
                    ADD_ITEM: "ADD_ITEM",
                    REMOVE_ITEM: "REMOVE_ITEM",
                    HIGHLIGHT_ITEM: "HIGHLIGHT_ITEM",
                    CLEAR_ALL: "CLEAR_ALL",
                    RESET_TO: "RESET_TO",
                    SET_IS_LOADING: "SET_IS_LOADING"
                };
                exports.KEY_CODES = {
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
                exports.TEXT_TYPE = "text";
                exports.SELECT_ONE_TYPE = "select-one";
                exports.SELECT_MULTIPLE_TYPE = "select-multiple";
                exports.SCROLLING_SPEED = 4;
            },
            789: function(__unused_webpack_module, exports, __webpack_require__) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.DEFAULT_CONFIG = exports.DEFAULT_CLASSNAMES = void 0;
                var utils_1 = __webpack_require__(799);
                exports.DEFAULT_CLASSNAMES = {
                    containerOuter: "choices",
                    containerInner: "choices__inner",
                    input: "choices__input",
                    inputCloned: "choices__input--cloned",
                    list: "choices__list",
                    listItems: "choices__list--multiple",
                    listSingle: "choices__list--single",
                    listDropdown: "choices__list--dropdown",
                    item: "choices__item",
                    itemSelectable: "choices__item--selectable",
                    itemDisabled: "choices__item--disabled",
                    itemChoice: "choices__item--choice",
                    placeholder: "choices__placeholder",
                    group: "choices__group",
                    groupHeading: "choices__heading",
                    button: "choices__button",
                    activeState: "is-active",
                    focusState: "is-focused",
                    openState: "is-open",
                    disabledState: "is-disabled",
                    highlightedState: "is-highlighted",
                    selectedState: "is-selected",
                    flippedState: "is-flipped",
                    loadingState: "is-loading",
                    noResults: "has-no-results",
                    noChoices: "has-no-choices"
                };
                exports.DEFAULT_CONFIG = {
                    items: [],
                    choices: [],
                    silent: false,
                    renderChoiceLimit: -1,
                    maxItemCount: -1,
                    addChoices: false,
                    addItems: true,
                    addItemFilter: null,
                    removeItems: true,
                    removeItemButton: false,
                    editItems: false,
                    allowHTML: true,
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
                    sorter: utils_1.sortByAlpha,
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
                        return 'Press Enter to add <b>"'.concat((0, utils_1.sanitise)(value), '"</b>');
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
                    classNames: exports.DEFAULT_CLASSNAMES
                };
            },
            18: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            978: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            948: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            359: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            285: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            533: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            187: function(__unused_webpack_module, exports, __webpack_require__) {
                var __createBinding = this && this.__createBinding || (Object.create ? function(o, m, k, k2) {
                    if (k2 === undefined) k2 = k;
                    var desc = Object.getOwnPropertyDescriptor(m, k);
                    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
                        desc = {
                            enumerable: true,
                            get: function() {
                                return m[k];
                            }
                        };
                    }
                    Object.defineProperty(o, k2, desc);
                } : function(o, m, k, k2) {
                    if (k2 === undefined) k2 = k;
                    o[k2] = m[k];
                });
                var __exportStar = this && this.__exportStar || function(m, exports) {
                    for (var p in m) if (p !== "default" && !Object.prototype.hasOwnProperty.call(exports, p)) __createBinding(exports, m, p);
                };
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                __exportStar(__webpack_require__(18), exports);
                __exportStar(__webpack_require__(978), exports);
                __exportStar(__webpack_require__(948), exports);
                __exportStar(__webpack_require__(359), exports);
                __exportStar(__webpack_require__(285), exports);
                __exportStar(__webpack_require__(533), exports);
                __exportStar(__webpack_require__(287), exports);
                __exportStar(__webpack_require__(132), exports);
                __exportStar(__webpack_require__(837), exports);
                __exportStar(__webpack_require__(598), exports);
                __exportStar(__webpack_require__(369), exports);
                __exportStar(__webpack_require__(37), exports);
                __exportStar(__webpack_require__(47), exports);
                __exportStar(__webpack_require__(923), exports);
                __exportStar(__webpack_require__(876), exports);
            },
            287: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            132: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            837: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            598: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            37: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            369: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            47: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            923: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            876: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            },
            858: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.isHTMLOptgroup = exports.isHTMLOption = void 0;
                var isHTMLOption = function(e) {
                    return e.tagName === "OPTION";
                };
                exports.isHTMLOption = isHTMLOption;
                var isHTMLOptgroup = function(e) {
                    return e.tagName === "OPTGROUP";
                };
                exports.isHTMLOptgroup = isHTMLOptgroup;
            },
            799: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.parseCustomProperties = exports.diff = exports.cloneObject = exports.existsInArray = exports.dispatchEvent = exports.sortByScore = exports.sortByAlpha = exports.strToEl = exports.sanitise = exports.isScrolledIntoView = exports.getAdjacentEl = exports.wrap = exports.isType = exports.getType = exports.generateId = exports.generateChars = exports.getRandomNumber = void 0;
                var getRandomNumber = function(min, max) {
                    return Math.floor(Math.random() * (max - min) + min);
                };
                exports.getRandomNumber = getRandomNumber;
                var generateChars = function(length) {
                    return Array.from({
                        length: length
                    }, function() {
                        return (0, exports.getRandomNumber)(0, 36).toString(36);
                    }).join("");
                };
                exports.generateChars = generateChars;
                var generateId = function(element, prefix) {
                    var id = element.id || element.name && "".concat(element.name, "-").concat((0, 
                    exports.generateChars)(2)) || (0, exports.generateChars)(4);
                    id = id.replace(/(:|\.|\[|\]|,)/g, "");
                    id = "".concat(prefix, "-").concat(id);
                    return id;
                };
                exports.generateId = generateId;
                var getType = function(obj) {
                    return Object.prototype.toString.call(obj).slice(8, -1);
                };
                exports.getType = getType;
                var isType = function(type, obj) {
                    return obj !== undefined && obj !== null && (0, exports.getType)(obj) === type;
                };
                exports.isType = isType;
                var wrap = function(element, wrapper) {
                    if (wrapper === void 0) {
                        wrapper = document.createElement("div");
                    }
                    if (element.parentNode) {
                        if (element.nextSibling) {
                            element.parentNode.insertBefore(wrapper, element.nextSibling);
                        } else {
                            element.parentNode.appendChild(wrapper);
                        }
                    }
                    return wrapper.appendChild(element);
                };
                exports.wrap = wrap;
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
                    return sibling;
                };
                exports.getAdjacentEl = getAdjacentEl;
                var isScrolledIntoView = function(element, parent, direction) {
                    if (direction === void 0) {
                        direction = 1;
                    }
                    if (!element) {
                        return false;
                    }
                    var isVisible;
                    if (direction > 0) {
                        isVisible = parent.scrollTop + parent.offsetHeight >= element.offsetTop + element.offsetHeight;
                    } else {
                        isVisible = element.offsetTop >= parent.scrollTop;
                    }
                    return isVisible;
                };
                exports.isScrolledIntoView = isScrolledIntoView;
                var sanitise = function(value) {
                    if (typeof value !== "string") {
                        return value;
                    }
                    return value.replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;").replace(/"/g, "&quot;");
                };
                exports.sanitise = sanitise;
                exports.strToEl = function() {
                    var tmpEl = document.createElement("div");
                    return function(str) {
                        var cleanedInput = str.trim();
                        tmpEl.innerHTML = cleanedInput;
                        var firldChild = tmpEl.children[0];
                        while (tmpEl.firstChild) {
                            tmpEl.removeChild(tmpEl.firstChild);
                        }
                        return firldChild;
                    };
                }();
                var sortByAlpha = function(_a, _b) {
                    var value = _a.value, _c = _a.label, label = _c === void 0 ? value : _c;
                    var value2 = _b.value, _d = _b.label, label2 = _d === void 0 ? value2 : _d;
                    return label.localeCompare(label2, [], {
                        sensitivity: "base",
                        ignorePunctuation: true,
                        numeric: true
                    });
                };
                exports.sortByAlpha = sortByAlpha;
                var sortByScore = function(a, b) {
                    var _a = a.score, scoreA = _a === void 0 ? 0 : _a;
                    var _b = b.score, scoreB = _b === void 0 ? 0 : _b;
                    return scoreA - scoreB;
                };
                exports.sortByScore = sortByScore;
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
                exports.dispatchEvent = dispatchEvent;
                var existsInArray = function(array, value, key) {
                    if (key === void 0) {
                        key = "value";
                    }
                    return array.some(function(item) {
                        if (typeof value === "string") {
                            return item[key] === value.trim();
                        }
                        return item[key] === value;
                    });
                };
                exports.existsInArray = existsInArray;
                var cloneObject = function(obj) {
                    return JSON.parse(JSON.stringify(obj));
                };
                exports.cloneObject = cloneObject;
                var diff = function(a, b) {
                    var aKeys = Object.keys(a).sort();
                    var bKeys = Object.keys(b).sort();
                    return aKeys.filter(function(i) {
                        return bKeys.indexOf(i) < 0;
                    });
                };
                exports.diff = diff;
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
                exports.parseCustomProperties = parseCustomProperties;
            },
            273: function(__unused_webpack_module, exports) {
                var __spreadArray = this && this.__spreadArray || function(to, from, pack) {
                    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
                        if (ar || !(i in from)) {
                            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
                            ar[i] = from[i];
                        }
                    }
                    return to.concat(ar || Array.prototype.slice.call(from));
                };
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.defaultState = void 0;
                exports.defaultState = [];
                function choices(state, action) {
                    if (state === void 0) {
                        state = exports.defaultState;
                    }
                    if (action === void 0) {
                        action = {};
                    }
                    switch (action.type) {
                      case "ADD_CHOICE":
                        {
                            var addChoiceAction = action;
                            var choice = {
                                id: addChoiceAction.id,
                                elementId: addChoiceAction.elementId,
                                groupId: addChoiceAction.groupId,
                                value: addChoiceAction.value,
                                label: addChoiceAction.label || addChoiceAction.value,
                                disabled: addChoiceAction.disabled || false,
                                selected: false,
                                active: true,
                                score: 9999,
                                customProperties: addChoiceAction.customProperties,
                                placeholder: addChoiceAction.placeholder || false
                            };
                            return __spreadArray(__spreadArray([], state, true), [ choice ], false);
                        }

                      case "ADD_ITEM":
                        {
                            var addItemAction_1 = action;
                            if (addItemAction_1.choiceId > -1) {
                                return state.map(function(obj) {
                                    var choice = obj;
                                    if (choice.id === parseInt("".concat(addItemAction_1.choiceId), 10)) {
                                        choice.selected = true;
                                    }
                                    return choice;
                                });
                            }
                            return state;
                        }

                      case "REMOVE_ITEM":
                        {
                            var removeItemAction_1 = action;
                            if (removeItemAction_1.choiceId && removeItemAction_1.choiceId > -1) {
                                return state.map(function(obj) {
                                    var choice = obj;
                                    if (choice.id === parseInt("".concat(removeItemAction_1.choiceId), 10)) {
                                        choice.selected = false;
                                    }
                                    return choice;
                                });
                            }
                            return state;
                        }

                      case "FILTER_CHOICES":
                        {
                            var filterChoicesAction_1 = action;
                            return state.map(function(obj) {
                                var choice = obj;
                                choice.active = filterChoicesAction_1.results.some(function(_a) {
                                    var item = _a.item, score = _a.score;
                                    if (item.id === choice.id) {
                                        choice.score = score;
                                        return true;
                                    }
                                    return false;
                                });
                                return choice;
                            });
                        }

                      case "ACTIVATE_CHOICES":
                        {
                            var activateChoicesAction_1 = action;
                            return state.map(function(obj) {
                                var choice = obj;
                                choice.active = activateChoicesAction_1.active;
                                return choice;
                            });
                        }

                      case "CLEAR_CHOICES":
                        {
                            return exports.defaultState;
                        }

                      default:
                        {
                            return state;
                        }
                    }
                }
                exports["default"] = choices;
            },
            871: function(__unused_webpack_module, exports) {
                var __spreadArray = this && this.__spreadArray || function(to, from, pack) {
                    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
                        if (ar || !(i in from)) {
                            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
                            ar[i] = from[i];
                        }
                    }
                    return to.concat(ar || Array.prototype.slice.call(from));
                };
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.defaultState = void 0;
                exports.defaultState = [];
                function groups(state, action) {
                    if (state === void 0) {
                        state = exports.defaultState;
                    }
                    if (action === void 0) {
                        action = {};
                    }
                    switch (action.type) {
                      case "ADD_GROUP":
                        {
                            var addGroupAction = action;
                            return __spreadArray(__spreadArray([], state, true), [ {
                                id: addGroupAction.id,
                                value: addGroupAction.value,
                                active: addGroupAction.active,
                                disabled: addGroupAction.disabled
                            } ], false);
                        }

                      case "CLEAR_CHOICES":
                        {
                            return [];
                        }

                      default:
                        {
                            return state;
                        }
                    }
                }
                exports["default"] = groups;
            },
            655: function(__unused_webpack_module, exports, __webpack_require__) {
                var __importDefault = this && this.__importDefault || function(mod) {
                    return mod && mod.__esModule ? mod : {
                        default: mod
                    };
                };
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.defaultState = void 0;
                var redux_1 = __webpack_require__(791);
                var items_1 = __importDefault(__webpack_require__(52));
                var groups_1 = __importDefault(__webpack_require__(871));
                var choices_1 = __importDefault(__webpack_require__(273));
                var loading_1 = __importDefault(__webpack_require__(502));
                var utils_1 = __webpack_require__(799);
                exports.defaultState = {
                    groups: [],
                    items: [],
                    choices: [],
                    loading: false
                };
                var appReducer = (0, redux_1.combineReducers)({
                    items: items_1.default,
                    groups: groups_1.default,
                    choices: choices_1.default,
                    loading: loading_1.default
                });
                var rootReducer = function(passedState, action) {
                    var state = passedState;
                    if (action.type === "CLEAR_ALL") {
                        state = exports.defaultState;
                    } else if (action.type === "RESET_TO") {
                        return (0, utils_1.cloneObject)(action.state);
                    }
                    return appReducer(state, action);
                };
                exports["default"] = rootReducer;
            },
            52: function(__unused_webpack_module, exports) {
                var __spreadArray = this && this.__spreadArray || function(to, from, pack) {
                    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
                        if (ar || !(i in from)) {
                            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
                            ar[i] = from[i];
                        }
                    }
                    return to.concat(ar || Array.prototype.slice.call(from));
                };
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.defaultState = void 0;
                exports.defaultState = [];
                function items(state, action) {
                    if (state === void 0) {
                        state = exports.defaultState;
                    }
                    if (action === void 0) {
                        action = {};
                    }
                    switch (action.type) {
                      case "ADD_ITEM":
                        {
                            var addItemAction = action;
                            var newState = __spreadArray(__spreadArray([], state, true), [ {
                                id: addItemAction.id,
                                choiceId: addItemAction.choiceId,
                                groupId: addItemAction.groupId,
                                value: addItemAction.value,
                                label: addItemAction.label,
                                active: true,
                                highlighted: false,
                                customProperties: addItemAction.customProperties,
                                placeholder: addItemAction.placeholder || false,
                                keyCode: null
                            } ], false);
                            return newState.map(function(obj) {
                                var item = obj;
                                item.highlighted = false;
                                return item;
                            });
                        }

                      case "REMOVE_ITEM":
                        {
                            return state.map(function(obj) {
                                var item = obj;
                                if (item.id === action.id) {
                                    item.active = false;
                                }
                                return item;
                            });
                        }

                      case "HIGHLIGHT_ITEM":
                        {
                            var highlightItemAction_1 = action;
                            return state.map(function(obj) {
                                var item = obj;
                                if (item.id === highlightItemAction_1.id) {
                                    item.highlighted = highlightItemAction_1.highlighted;
                                }
                                return item;
                            });
                        }

                      default:
                        {
                            return state;
                        }
                    }
                }
                exports["default"] = items;
            },
            502: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                exports.defaultState = void 0;
                exports.defaultState = false;
                var general = function(state, action) {
                    if (state === void 0) {
                        state = exports.defaultState;
                    }
                    if (action === void 0) {
                        action = {};
                    }
                    switch (action.type) {
                      case "SET_IS_LOADING":
                        {
                            return action.isLoading;
                        }

                      default:
                        {
                            return state;
                        }
                    }
                };
                exports["default"] = general;
            },
            744: function(__unused_webpack_module, exports, __webpack_require__) {
                var __spreadArray = this && this.__spreadArray || function(to, from, pack) {
                    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
                        if (ar || !(i in from)) {
                            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
                            ar[i] = from[i];
                        }
                    }
                    return to.concat(ar || Array.prototype.slice.call(from));
                };
                var __importDefault = this && this.__importDefault || function(mod) {
                    return mod && mod.__esModule ? mod : {
                        default: mod
                    };
                };
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                var redux_1 = __webpack_require__(791);
                var index_1 = __importDefault(__webpack_require__(655));
                var Store = function() {
                    function Store() {
                        this._store = (0, redux_1.createStore)(index_1.default, window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__());
                    }
                    Store.prototype.subscribe = function(onChange) {
                        this._store.subscribe(onChange);
                    };
                    Store.prototype.dispatch = function(action) {
                        this._store.dispatch(action);
                    };
                    Object.defineProperty(Store.prototype, "state", {
                        get: function() {
                            return this._store.getState();
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
                    Object.defineProperty(Store.prototype, "activeItems", {
                        get: function() {
                            return this.items.filter(function(item) {
                                return item.active === true;
                            });
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
                                return choice.active === true;
                            });
                        },
                        enumerable: false,
                        configurable: true
                    });
                    Object.defineProperty(Store.prototype, "selectableChoices", {
                        get: function() {
                            return this.choices.filter(function(choice) {
                                return choice.disabled !== true;
                            });
                        },
                        enumerable: false,
                        configurable: true
                    });
                    Object.defineProperty(Store.prototype, "searchableChoices", {
                        get: function() {
                            return this.selectableChoices.filter(function(choice) {
                                return choice.placeholder !== true;
                            });
                        },
                        enumerable: false,
                        configurable: true
                    });
                    Object.defineProperty(Store.prototype, "placeholderChoice", {
                        get: function() {
                            return __spreadArray([], this.choices, true).reverse().find(function(choice) {
                                return choice.placeholder === true;
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
                            var _a = this, groups = _a.groups, choices = _a.choices;
                            return groups.filter(function(group) {
                                var isActive = group.active === true && group.disabled === false;
                                var hasActiveOptions = choices.some(function(choice) {
                                    return choice.active === true && choice.disabled === false;
                                });
                                return isActive && hasActiveOptions;
                            }, []);
                        },
                        enumerable: false,
                        configurable: true
                    });
                    Store.prototype.isLoading = function() {
                        return this.state.loading;
                    };
                    Store.prototype.getChoiceById = function(id) {
                        return this.activeChoices.find(function(choice) {
                            return choice.id === parseInt(id, 10);
                        });
                    };
                    Store.prototype.getGroupById = function(id) {
                        return this.groups.find(function(group) {
                            return group.id === id;
                        });
                    };
                    return Store;
                }();
                exports["default"] = Store;
            },
            686: function(__unused_webpack_module, exports) {
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
                var templates = {
                    containerOuter: function(_a, dir, isSelectElement, isSelectOneElement, searchEnabled, passedElementType, labelId) {
                        var containerOuter = _a.classNames.containerOuter;
                        var div = Object.assign(document.createElement("div"), {
                            className: containerOuter
                        });
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
                            }
                        }
                        div.setAttribute("aria-haspopup", "true");
                        div.setAttribute("aria-expanded", "false");
                        if (labelId) {
                            div.setAttribute("aria-labelledby", labelId);
                        }
                        return div;
                    },
                    containerInner: function(_a) {
                        var containerInner = _a.classNames.containerInner;
                        return Object.assign(document.createElement("div"), {
                            className: containerInner
                        });
                    },
                    itemList: function(_a, isSelectOneElement) {
                        var _b = _a.classNames, list = _b.list, listSingle = _b.listSingle, listItems = _b.listItems;
                        return Object.assign(document.createElement("div"), {
                            className: "".concat(list, " ").concat(isSelectOneElement ? listSingle : listItems)
                        });
                    },
                    placeholder: function(_a, value) {
                        var _b;
                        var allowHTML = _a.allowHTML, placeholder = _a.classNames.placeholder;
                        return Object.assign(document.createElement("div"), (_b = {
                            className: placeholder
                        }, _b[allowHTML ? "innerHTML" : "innerText"] = value, _b));
                    },
                    item: function(_a, _b, removeItemButton) {
                        var _c, _d;
                        var allowHTML = _a.allowHTML, _e = _a.classNames, item = _e.item, button = _e.button, highlightedState = _e.highlightedState, itemSelectable = _e.itemSelectable, placeholder = _e.placeholder;
                        var id = _b.id, value = _b.value, label = _b.label, customProperties = _b.customProperties, active = _b.active, disabled = _b.disabled, highlighted = _b.highlighted, isPlaceholder = _b.placeholder;
                        var div = Object.assign(document.createElement("div"), (_c = {
                            className: item
                        }, _c[allowHTML ? "innerHTML" : "innerText"] = label, _c));
                        Object.assign(div.dataset, {
                            item: "",
                            id: id,
                            value: value,
                            customProperties: customProperties
                        });
                        if (active) {
                            div.setAttribute("aria-selected", "true");
                        }
                        if (disabled) {
                            div.setAttribute("aria-disabled", "true");
                        }
                        if (isPlaceholder) {
                            div.classList.add(placeholder);
                        }
                        div.classList.add(highlighted ? highlightedState : itemSelectable);
                        if (removeItemButton) {
                            if (disabled) {
                                div.classList.remove(itemSelectable);
                            }
                            div.dataset.deletable = "";
                            var REMOVE_ITEM_TEXT = "Remove item";
                            var removeButton = Object.assign(document.createElement("button"), (_d = {
                                type: "button",
                                className: button
                            }, _d[allowHTML ? "innerHTML" : "innerText"] = REMOVE_ITEM_TEXT, 
                            _d));
                            removeButton.setAttribute("aria-label", "".concat(REMOVE_ITEM_TEXT, ": '").concat(value, "'"));
                            removeButton.dataset.button = "";
                            div.appendChild(removeButton);
                        }
                        return div;
                    },
                    choiceList: function(_a, isSelectOneElement) {
                        var list = _a.classNames.list;
                        var div = Object.assign(document.createElement("div"), {
                            className: list
                        });
                        if (!isSelectOneElement) {
                            div.setAttribute("aria-multiselectable", "true");
                        }
                        div.setAttribute("role", "listbox");
                        return div;
                    },
                    choiceGroup: function(_a, _b) {
                        var _c;
                        var allowHTML = _a.allowHTML, _d = _a.classNames, group = _d.group, groupHeading = _d.groupHeading, itemDisabled = _d.itemDisabled;
                        var id = _b.id, value = _b.value, disabled = _b.disabled;
                        var div = Object.assign(document.createElement("div"), {
                            className: "".concat(group, " ").concat(disabled ? itemDisabled : "")
                        });
                        div.setAttribute("role", "group");
                        Object.assign(div.dataset, {
                            group: "",
                            id: id,
                            value: value
                        });
                        if (disabled) {
                            div.setAttribute("aria-disabled", "true");
                        }
                        div.appendChild(Object.assign(document.createElement("div"), (_c = {
                            className: groupHeading
                        }, _c[allowHTML ? "innerHTML" : "innerText"] = value, _c)));
                        return div;
                    },
                    choice: function(_a, _b, selectText) {
                        var _c;
                        var allowHTML = _a.allowHTML, _d = _a.classNames, item = _d.item, itemChoice = _d.itemChoice, itemSelectable = _d.itemSelectable, selectedState = _d.selectedState, itemDisabled = _d.itemDisabled, placeholder = _d.placeholder;
                        var id = _b.id, value = _b.value, label = _b.label, groupId = _b.groupId, elementId = _b.elementId, isDisabled = _b.disabled, isSelected = _b.selected, isPlaceholder = _b.placeholder;
                        var div = Object.assign(document.createElement("div"), (_c = {
                            id: elementId
                        }, _c[allowHTML ? "innerHTML" : "innerText"] = label, _c.className = "".concat(item, " ").concat(itemChoice), 
                        _c));
                        if (isSelected) {
                            div.classList.add(selectedState);
                        }
                        if (isPlaceholder) {
                            div.classList.add(placeholder);
                        }
                        div.setAttribute("role", groupId && groupId > 0 ? "treeitem" : "option");
                        Object.assign(div.dataset, {
                            choice: "",
                            id: id,
                            value: value,
                            selectText: selectText
                        });
                        if (isDisabled) {
                            div.classList.add(itemDisabled);
                            div.dataset.choiceDisabled = "";
                            div.setAttribute("aria-disabled", "true");
                        } else {
                            div.classList.add(itemSelectable);
                            div.dataset.choiceSelectable = "";
                        }
                        return div;
                    },
                    input: function(_a, placeholderValue) {
                        var _b = _a.classNames, input = _b.input, inputCloned = _b.inputCloned;
                        var inp = Object.assign(document.createElement("input"), {
                            type: "search",
                            name: "search_terms",
                            className: "".concat(input, " ").concat(inputCloned),
                            autocomplete: "off",
                            autocapitalize: "off",
                            spellcheck: false
                        });
                        inp.setAttribute("role", "textbox");
                        inp.setAttribute("aria-autocomplete", "list");
                        inp.setAttribute("aria-label", placeholderValue);
                        return inp;
                    },
                    dropdown: function(_a) {
                        var _b = _a.classNames, list = _b.list, listDropdown = _b.listDropdown;
                        var div = document.createElement("div");
                        div.classList.add(list, listDropdown);
                        div.setAttribute("aria-expanded", "false");
                        return div;
                    },
                    notice: function(_a, innerText, type) {
                        var _b;
                        var allowHTML = _a.allowHTML, _c = _a.classNames, item = _c.item, itemChoice = _c.itemChoice, noResults = _c.noResults, noChoices = _c.noChoices;
                        if (type === void 0) {
                            type = "";
                        }
                        var classes = [ item, itemChoice ];
                        if (type === "no-choices") {
                            classes.push(noChoices);
                        } else if (type === "no-results") {
                            classes.push(noResults);
                        }
                        return Object.assign(document.createElement("div"), (_b = {}, 
                        _b[allowHTML ? "innerHTML" : "innerText"] = innerText, _b.className = classes.join(" "), 
                        _b));
                    },
                    option: function(_a) {
                        var label = _a.label, value = _a.value, customProperties = _a.customProperties, active = _a.active, disabled = _a.disabled;
                        var opt = new Option(label, value, false, active);
                        if (customProperties) {
                            opt.dataset.customProperties = "".concat(JSON.stringify(customProperties));
                        }
                        opt.disabled = !!disabled;
                        return opt;
                    }
                };
                exports["default"] = templates;
            },
            996: function(module) {
                var isMergeableObject = function isMergeableObject(value) {
                    return isNonNullObject(value) && !isSpecial(value);
                };
                function isNonNullObject(value) {
                    return !!value && typeof value === "object";
                }
                function isSpecial(value) {
                    var stringValue = Object.prototype.toString.call(value);
                    return stringValue === "[object RegExp]" || stringValue === "[object Date]" || isReactElement(value);
                }
                var canUseSymbol = typeof Symbol === "function" && Symbol.for;
                var REACT_ELEMENT_TYPE = canUseSymbol ? Symbol.for("react.element") : 60103;
                function isReactElement(value) {
                    return value.$$typeof === REACT_ELEMENT_TYPE;
                }
                function emptyTarget(val) {
                    return Array.isArray(val) ? [] : {};
                }
                function cloneUnlessOtherwiseSpecified(value, options) {
                    return options.clone !== false && options.isMergeableObject(value) ? deepmerge(emptyTarget(value), value, options) : value;
                }
                function defaultArrayMerge(target, source, options) {
                    return target.concat(source).map(function(element) {
                        return cloneUnlessOtherwiseSpecified(element, options);
                    });
                }
                function getMergeFunction(key, options) {
                    if (!options.customMerge) {
                        return deepmerge;
                    }
                    var customMerge = options.customMerge(key);
                    return typeof customMerge === "function" ? customMerge : deepmerge;
                }
                function getEnumerableOwnPropertySymbols(target) {
                    return Object.getOwnPropertySymbols ? Object.getOwnPropertySymbols(target).filter(function(symbol) {
                        return target.propertyIsEnumerable(symbol);
                    }) : [];
                }
                function getKeys(target) {
                    return Object.keys(target).concat(getEnumerableOwnPropertySymbols(target));
                }
                function propertyIsOnObject(object, property) {
                    try {
                        return property in object;
                    } catch (_) {
                        return false;
                    }
                }
                function propertyIsUnsafe(target, key) {
                    return propertyIsOnObject(target, key) && !(Object.hasOwnProperty.call(target, key) && Object.propertyIsEnumerable.call(target, key));
                }
                function mergeObject(target, source, options) {
                    var destination = {};
                    if (options.isMergeableObject(target)) {
                        getKeys(target).forEach(function(key) {
                            destination[key] = cloneUnlessOtherwiseSpecified(target[key], options);
                        });
                    }
                    getKeys(source).forEach(function(key) {
                        if (propertyIsUnsafe(target, key)) {
                            return;
                        }
                        if (propertyIsOnObject(target, key) && options.isMergeableObject(source[key])) {
                            destination[key] = getMergeFunction(key, options)(target[key], source[key], options);
                        } else {
                            destination[key] = cloneUnlessOtherwiseSpecified(source[key], options);
                        }
                    });
                    return destination;
                }
                function deepmerge(target, source, options) {
                    options = options || {};
                    options.arrayMerge = options.arrayMerge || defaultArrayMerge;
                    options.isMergeableObject = options.isMergeableObject || isMergeableObject;
                    options.cloneUnlessOtherwiseSpecified = cloneUnlessOtherwiseSpecified;
                    var sourceIsArray = Array.isArray(source);
                    var targetIsArray = Array.isArray(target);
                    var sourceAndTargetTypesMatch = sourceIsArray === targetIsArray;
                    if (!sourceAndTargetTypesMatch) {
                        return cloneUnlessOtherwiseSpecified(source, options);
                    } else if (sourceIsArray) {
                        return options.arrayMerge(target, source, options);
                    } else {
                        return mergeObject(target, source, options);
                    }
                }
                deepmerge.all = function deepmergeAll(array, options) {
                    if (!Array.isArray(array)) {
                        throw new Error("first argument should be an array");
                    }
                    return array.reduce(function(prev, next) {
                        return deepmerge(prev, next, options);
                    }, {});
                };
                var deepmerge_1 = deepmerge;
                module.exports = deepmerge_1;
            },
            221: function(__unused_webpack_module, __webpack_exports__, __webpack_require__) {
                __webpack_require__.r(__webpack_exports__);
                __webpack_require__.d(__webpack_exports__, {
                    default: function() {
                        return Fuse;
                    }
                });
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
                            totalWeight += obj.weight;
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
                var Config = {
                    ...BasicOptions,
                    ...MatchOptions,
                    ...FuzzyOptions,
                    ...AdvancedOptions
                };
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
                        this.options = {
                            ...Config,
                            ...options
                        };
                        if (this.options.useExtendedSearch && !true) {}
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
                Fuse.version = "6.6.2";
                Fuse.createIndex = createIndex;
                Fuse.parseIndex = parseIndex;
                Fuse.config = Config;
                {
                    Fuse.parseQuery = parse;
                }
                {
                    register(ExtendedSearch);
                }
            },
            791: function(__unused_webpack_module, __webpack_exports__, __webpack_require__) {
                __webpack_require__.r(__webpack_exports__);
                __webpack_require__.d(__webpack_exports__, {
                    __DO_NOT_USE__ActionTypes: function() {
                        return ActionTypes;
                    },
                    applyMiddleware: function() {
                        return applyMiddleware;
                    },
                    bindActionCreators: function() {
                        return bindActionCreators;
                    },
                    combineReducers: function() {
                        return combineReducers;
                    },
                    compose: function() {
                        return compose;
                    },
                    createStore: function() {
                        return createStore;
                    },
                    legacy_createStore: function() {
                        return legacy_createStore;
                    }
                });
                function _typeof(obj) {
                    "@babel/helpers - typeof";
                    return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function(obj) {
                        return typeof obj;
                    } : function(obj) {
                        return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
                    }, _typeof(obj);
                }
                function _toPrimitive(input, hint) {
                    if (_typeof(input) !== "object" || input === null) return input;
                    var prim = input[Symbol.toPrimitive];
                    if (prim !== undefined) {
                        var res = prim.call(input, hint || "default");
                        if (_typeof(res) !== "object") return res;
                        throw new TypeError("@@toPrimitive must return a primitive value.");
                    }
                    return (hint === "string" ? String : Number)(input);
                }
                function _toPropertyKey(arg) {
                    var key = _toPrimitive(arg, "string");
                    return _typeof(key) === "symbol" ? key : String(key);
                }
                function _defineProperty(obj, key, value) {
                    key = _toPropertyKey(key);
                    if (key in obj) {
                        Object.defineProperty(obj, key, {
                            value: value,
                            enumerable: true,
                            configurable: true,
                            writable: true
                        });
                    } else {
                        obj[key] = value;
                    }
                    return obj;
                }
                function ownKeys(object, enumerableOnly) {
                    var keys = Object.keys(object);
                    if (Object.getOwnPropertySymbols) {
                        var symbols = Object.getOwnPropertySymbols(object);
                        enumerableOnly && (symbols = symbols.filter(function(sym) {
                            return Object.getOwnPropertyDescriptor(object, sym).enumerable;
                        })), keys.push.apply(keys, symbols);
                    }
                    return keys;
                }
                function _objectSpread2(target) {
                    for (var i = 1; i < arguments.length; i++) {
                        var source = null != arguments[i] ? arguments[i] : {};
                        i % 2 ? ownKeys(Object(source), !0).forEach(function(key) {
                            _defineProperty(target, key, source[key]);
                        }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)) : ownKeys(Object(source)).forEach(function(key) {
                            Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key));
                        });
                    }
                    return target;
                }
                function formatProdErrorMessage(code) {
                    return "Minified Redux error #" + code + "; visit https://redux.js.org/Errors?code=" + code + " for the full message or " + "use the non-minified dev environment for full errors. ";
                }
                var $$observable = function() {
                    return typeof Symbol === "function" && Symbol.observable || "@@observable";
                }();
                var randomString = function randomString() {
                    return Math.random().toString(36).substring(7).split("").join(".");
                };
                var ActionTypes = {
                    INIT: "@@redux/INIT" + randomString(),
                    REPLACE: "@@redux/REPLACE" + randomString(),
                    PROBE_UNKNOWN_ACTION: function PROBE_UNKNOWN_ACTION() {
                        return "@@redux/PROBE_UNKNOWN_ACTION" + randomString();
                    }
                };
                function isPlainObject(obj) {
                    if (typeof obj !== "object" || obj === null) return false;
                    var proto = obj;
                    while (Object.getPrototypeOf(proto) !== null) {
                        proto = Object.getPrototypeOf(proto);
                    }
                    return Object.getPrototypeOf(obj) === proto;
                }
                function miniKindOf(val) {
                    if (val === void 0) return "undefined";
                    if (val === null) return "null";
                    var type = typeof val;
                    switch (type) {
                      case "boolean":
                      case "string":
                      case "number":
                      case "symbol":
                      case "function":
                        {
                            return type;
                        }
                    }
                    if (Array.isArray(val)) return "array";
                    if (isDate(val)) return "date";
                    if (isError(val)) return "error";
                    var constructorName = ctorName(val);
                    switch (constructorName) {
                      case "Symbol":
                      case "Promise":
                      case "WeakMap":
                      case "WeakSet":
                      case "Map":
                      case "Set":
                        return constructorName;
                    }
                    return type.slice(8, -1).toLowerCase().replace(/\s/g, "");
                }
                function ctorName(val) {
                    return typeof val.constructor === "function" ? val.constructor.name : null;
                }
                function isError(val) {
                    return val instanceof Error || typeof val.message === "string" && val.constructor && typeof val.constructor.stackTraceLimit === "number";
                }
                function isDate(val) {
                    if (val instanceof Date) return true;
                    return typeof val.toDateString === "function" && typeof val.getDate === "function" && typeof val.setDate === "function";
                }
                function kindOf(val) {
                    var typeOfVal = typeof val;
                    if (false) {}
                    return typeOfVal;
                }
                function createStore(reducer, preloadedState, enhancer) {
                    var _ref2;
                    if (typeof preloadedState === "function" && typeof enhancer === "function" || typeof enhancer === "function" && typeof arguments[3] === "function") {
                        throw new Error(true ? formatProdErrorMessage(0) : 0);
                    }
                    if (typeof preloadedState === "function" && typeof enhancer === "undefined") {
                        enhancer = preloadedState;
                        preloadedState = undefined;
                    }
                    if (typeof enhancer !== "undefined") {
                        if (typeof enhancer !== "function") {
                            throw new Error(true ? formatProdErrorMessage(1) : 0);
                        }
                        return enhancer(createStore)(reducer, preloadedState);
                    }
                    if (typeof reducer !== "function") {
                        throw new Error(true ? formatProdErrorMessage(2) : 0);
                    }
                    var currentReducer = reducer;
                    var currentState = preloadedState;
                    var currentListeners = [];
                    var nextListeners = currentListeners;
                    var isDispatching = false;
                    function ensureCanMutateNextListeners() {
                        if (nextListeners === currentListeners) {
                            nextListeners = currentListeners.slice();
                        }
                    }
                    function getState() {
                        if (isDispatching) {
                            throw new Error(true ? formatProdErrorMessage(3) : 0);
                        }
                        return currentState;
                    }
                    function subscribe(listener) {
                        if (typeof listener !== "function") {
                            throw new Error(true ? formatProdErrorMessage(4) : 0);
                        }
                        if (isDispatching) {
                            throw new Error(true ? formatProdErrorMessage(5) : 0);
                        }
                        var isSubscribed = true;
                        ensureCanMutateNextListeners();
                        nextListeners.push(listener);
                        return function unsubscribe() {
                            if (!isSubscribed) {
                                return;
                            }
                            if (isDispatching) {
                                throw new Error(true ? formatProdErrorMessage(6) : 0);
                            }
                            isSubscribed = false;
                            ensureCanMutateNextListeners();
                            var index = nextListeners.indexOf(listener);
                            nextListeners.splice(index, 1);
                            currentListeners = null;
                        };
                    }
                    function dispatch(action) {
                        if (!isPlainObject(action)) {
                            throw new Error(true ? formatProdErrorMessage(7) : 0);
                        }
                        if (typeof action.type === "undefined") {
                            throw new Error(true ? formatProdErrorMessage(8) : 0);
                        }
                        if (isDispatching) {
                            throw new Error(true ? formatProdErrorMessage(9) : 0);
                        }
                        try {
                            isDispatching = true;
                            currentState = currentReducer(currentState, action);
                        } finally {
                            isDispatching = false;
                        }
                        var listeners = currentListeners = nextListeners;
                        for (var i = 0; i < listeners.length; i++) {
                            var listener = listeners[i];
                            listener();
                        }
                        return action;
                    }
                    function replaceReducer(nextReducer) {
                        if (typeof nextReducer !== "function") {
                            throw new Error(true ? formatProdErrorMessage(10) : 0);
                        }
                        currentReducer = nextReducer;
                        dispatch({
                            type: ActionTypes.REPLACE
                        });
                    }
                    function observable() {
                        var _ref;
                        var outerSubscribe = subscribe;
                        return _ref = {
                            subscribe: function subscribe(observer) {
                                if (typeof observer !== "object" || observer === null) {
                                    throw new Error(true ? formatProdErrorMessage(11) : 0);
                                }
                                function observeState() {
                                    if (observer.next) {
                                        observer.next(getState());
                                    }
                                }
                                observeState();
                                var unsubscribe = outerSubscribe(observeState);
                                return {
                                    unsubscribe: unsubscribe
                                };
                            }
                        }, _ref[$$observable] = function() {
                            return this;
                        }, _ref;
                    }
                    dispatch({
                        type: ActionTypes.INIT
                    });
                    return _ref2 = {
                        dispatch: dispatch,
                        subscribe: subscribe,
                        getState: getState,
                        replaceReducer: replaceReducer
                    }, _ref2[$$observable] = observable, _ref2;
                }
                var legacy_createStore = createStore;
                function warning(message) {
                    if (typeof console !== "undefined" && typeof console.error === "function") {
                        console.error(message);
                    }
                    try {
                        throw new Error(message);
                    } catch (e) {}
                }
                function getUnexpectedStateShapeWarningMessage(inputState, reducers, action, unexpectedKeyCache) {
                    var reducerKeys = Object.keys(reducers);
                    var argumentName = action && action.type === ActionTypes.INIT ? "preloadedState argument passed to createStore" : "previous state received by the reducer";
                    if (reducerKeys.length === 0) {
                        return "Store does not have a valid reducer. Make sure the argument passed " + "to combineReducers is an object whose values are reducers.";
                    }
                    if (!isPlainObject(inputState)) {
                        return "The " + argumentName + ' has unexpected type of "' + kindOf(inputState) + '". Expected argument to be an object with the following ' + ('keys: "' + reducerKeys.join('", "') + '"');
                    }
                    var unexpectedKeys = Object.keys(inputState).filter(function(key) {
                        return !reducers.hasOwnProperty(key) && !unexpectedKeyCache[key];
                    });
                    unexpectedKeys.forEach(function(key) {
                        unexpectedKeyCache[key] = true;
                    });
                    if (action && action.type === ActionTypes.REPLACE) return;
                    if (unexpectedKeys.length > 0) {
                        return "Unexpected " + (unexpectedKeys.length > 1 ? "keys" : "key") + " " + ('"' + unexpectedKeys.join('", "') + '" found in ' + argumentName + ". ") + "Expected to find one of the known reducer keys instead: " + ('"' + reducerKeys.join('", "') + '". Unexpected keys will be ignored.');
                    }
                }
                function assertReducerShape(reducers) {
                    Object.keys(reducers).forEach(function(key) {
                        var reducer = reducers[key];
                        var initialState = reducer(undefined, {
                            type: ActionTypes.INIT
                        });
                        if (typeof initialState === "undefined") {
                            throw new Error(true ? formatProdErrorMessage(12) : 0);
                        }
                        if (typeof reducer(undefined, {
                            type: ActionTypes.PROBE_UNKNOWN_ACTION()
                        }) === "undefined") {
                            throw new Error(true ? formatProdErrorMessage(13) : 0);
                        }
                    });
                }
                function combineReducers(reducers) {
                    var reducerKeys = Object.keys(reducers);
                    var finalReducers = {};
                    for (var i = 0; i < reducerKeys.length; i++) {
                        var key = reducerKeys[i];
                        if (false) {}
                        if (typeof reducers[key] === "function") {
                            finalReducers[key] = reducers[key];
                        }
                    }
                    var finalReducerKeys = Object.keys(finalReducers);
                    var unexpectedKeyCache;
                    if (false) {}
                    var shapeAssertionError;
                    try {
                        assertReducerShape(finalReducers);
                    } catch (e) {
                        shapeAssertionError = e;
                    }
                    return function combination(state, action) {
                        if (state === void 0) {
                            state = {};
                        }
                        if (shapeAssertionError) {
                            throw shapeAssertionError;
                        }
                        if (false) {
                            var warningMessage;
                        }
                        var hasChanged = false;
                        var nextState = {};
                        for (var _i = 0; _i < finalReducerKeys.length; _i++) {
                            var _key = finalReducerKeys[_i];
                            var reducer = finalReducers[_key];
                            var previousStateForKey = state[_key];
                            var nextStateForKey = reducer(previousStateForKey, action);
                            if (typeof nextStateForKey === "undefined") {
                                var actionType = action && action.type;
                                throw new Error(true ? formatProdErrorMessage(14) : 0);
                            }
                            nextState[_key] = nextStateForKey;
                            hasChanged = hasChanged || nextStateForKey !== previousStateForKey;
                        }
                        hasChanged = hasChanged || finalReducerKeys.length !== Object.keys(state).length;
                        return hasChanged ? nextState : state;
                    };
                }
                function bindActionCreator(actionCreator, dispatch) {
                    return function() {
                        return dispatch(actionCreator.apply(this, arguments));
                    };
                }
                function bindActionCreators(actionCreators, dispatch) {
                    if (typeof actionCreators === "function") {
                        return bindActionCreator(actionCreators, dispatch);
                    }
                    if (typeof actionCreators !== "object" || actionCreators === null) {
                        throw new Error(true ? formatProdErrorMessage(16) : 0);
                    }
                    var boundActionCreators = {};
                    for (var key in actionCreators) {
                        var actionCreator = actionCreators[key];
                        if (typeof actionCreator === "function") {
                            boundActionCreators[key] = bindActionCreator(actionCreator, dispatch);
                        }
                    }
                    return boundActionCreators;
                }
                function compose() {
                    for (var _len = arguments.length, funcs = new Array(_len), _key = 0; _key < _len; _key++) {
                        funcs[_key] = arguments[_key];
                    }
                    if (funcs.length === 0) {
                        return function(arg) {
                            return arg;
                        };
                    }
                    if (funcs.length === 1) {
                        return funcs[0];
                    }
                    return funcs.reduce(function(a, b) {
                        return function() {
                            return a(b.apply(void 0, arguments));
                        };
                    });
                }
                function applyMiddleware() {
                    for (var _len = arguments.length, middlewares = new Array(_len), _key = 0; _key < _len; _key++) {
                        middlewares[_key] = arguments[_key];
                    }
                    return function(createStore) {
                        return function() {
                            var store = createStore.apply(void 0, arguments);
                            var _dispatch = function dispatch() {
                                throw new Error(true ? formatProdErrorMessage(15) : 0);
                            };
                            var middlewareAPI = {
                                getState: store.getState,
                                dispatch: function dispatch() {
                                    return _dispatch.apply(void 0, arguments);
                                }
                            };
                            var chain = middlewares.map(function(middleware) {
                                return middleware(middlewareAPI);
                            });
                            _dispatch = compose.apply(void 0, chain)(store.dispatch);
                            return _objectSpread2(_objectSpread2({}, store), {}, {
                                dispatch: _dispatch
                            });
                        };
                    };
                }
                function isCrushed() {}
                if (false) {}
            }
        };
        var __webpack_module_cache__ = {};
        function __webpack_require__(moduleId) {
            var cachedModule = __webpack_module_cache__[moduleId];
            if (cachedModule !== undefined) {
                return cachedModule.exports;
            }
            var module = __webpack_module_cache__[moduleId] = {
                exports: {}
            };
            __webpack_modules__[moduleId].call(module.exports, module, module.exports, __webpack_require__);
            return module.exports;
        }
        !function() {
            __webpack_require__.n = function(module) {
                var getter = module && module.__esModule ? function() {
                    return module["default"];
                } : function() {
                    return module;
                };
                __webpack_require__.d(getter, {
                    a: getter
                });
                return getter;
            };
        }();
        !function() {
            __webpack_require__.d = function(exports, definition) {
                for (var key in definition) {
                    if (__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
                        Object.defineProperty(exports, key, {
                            enumerable: true,
                            get: definition[key]
                        });
                    }
                }
            };
        }();
        !function() {
            __webpack_require__.o = function(obj, prop) {
                return Object.prototype.hasOwnProperty.call(obj, prop);
            };
        }();
        !function() {
            __webpack_require__.r = function(exports) {
                if (typeof Symbol !== "undefined" && Symbol.toStringTag) {
                    Object.defineProperty(exports, Symbol.toStringTag, {
                        value: "Module"
                    });
                }
                Object.defineProperty(exports, "__esModule", {
                    value: true
                });
            };
        }();
        var __webpack_exports__ = {};
        !function() {
            var _scripts_choices__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(373);
            var _scripts_choices__WEBPACK_IMPORTED_MODULE_0___default = __webpack_require__.n(_scripts_choices__WEBPACK_IMPORTED_MODULE_0__);
            var _scripts_interfaces__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(187);
            var _scripts_interfaces__WEBPACK_IMPORTED_MODULE_1___default = __webpack_require__.n(_scripts_interfaces__WEBPACK_IMPORTED_MODULE_1__);
            var _scripts_constants__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(883);
            var _scripts_defaults__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(789);
            var _scripts_templates__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(686);
            __webpack_exports__["default"] = _scripts_choices__WEBPACK_IMPORTED_MODULE_0___default();
        }();
        __webpack_exports__ = __webpack_exports__["default"];
        return __webpack_exports__;
    }();
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
    function o(t) {
        return function(t) {
            if (Array.isArray(t)) return i(t);
        }(t) || function(t) {
            if ("undefined" != typeof Symbol && null != t[Symbol.iterator] || null != t["@@iterator"]) return Array.from(t);
        }(t) || r(t) || function() {
            throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.");
        }();
    }
    function r(t, e) {
        if (t) {
            if ("string" == typeof t) return i(t, e);
            var n = Object.prototype.toString.call(t).slice(8, -1);
            return "Object" === n && t.constructor && (n = t.constructor.name), 
            "Map" === n || "Set" === n ? Array.from(t) : "Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n) ? i(t, e) : void 0;
        }
    }
    function i(t, e) {
        (null == e || e > t.length) && (e = t.length);
        for (var n = 0, a = new Array(e); n < e; n++) a[n] = t[n];
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
            if ("object" !== a(t) || null === t) return t;
            var n = t[Symbol.toPrimitive];
            if (void 0 !== n) {
                var o = n.call(t, e || "default");
                if ("object" !== a(o)) return o;
                throw new TypeError("@@toPrimitive must return a primitive value.");
            }
            return ("string" === e ? String : Number)(t);
        }(t, "string");
        return "symbol" === a(e) ? e : String(e);
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
        var e, n, a;
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
                    return Object.assign(t, function(t, e, n) {
                        return (e = l(e)) in t ? Object.defineProperty(t, e, {
                            value: n,
                            enumerable: !0,
                            configurable: !0,
                            writable: !0
                        }) : t[e] = n, t;
                    }({}, n, e.settings[n]));
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
                return t ? o(new Set(Array.from(document.querySelectorAll('[data-gallery="'.concat(t, '"]')), function(t) {
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
                var e = this, n = document.createElement("template"), a = t.allowedMediaTypes.join("|"), o = this.sources.map(function(t, n) {
                    t = t.replace(/\/$/, "");
                    var o = new RegExp("^(".concat(a, ")"), "i"), r = /^html/.test(t), i = /^image/.test(t);
                    o.test(t) && (t = t.replace(o, ""));
                    var s = e.settings.constrain ? "mw-100 mh-100 h-auto w-auto m-auto top-0 end-0 bottom-0 start-0" : "h-100 w-100", l = new URLSearchParams(t.split("?")[1]), c = "", u = t;
                    if (l.get("caption")) {
                        try {
                            (u = new URL(t)).searchParams.delete("caption"), u = u.toString();
                        } catch (e) {
                            u = t;
                        }
                        c = '<div class="carousel-caption d-none d-md-block" style="z-index:2"><p class="bg-secondary rounded">'.concat(l.get("caption"), "</p></div>");
                    }
                    var d = '<img src="'.concat(u, '" class="d-block ').concat(s, ' img-fluid" style="z-index: 1; object-fit: contain;" />'), h = "", m = e.getInstagramEmbed(t), f = e.getYoutubeLink(t);
                    return e.isEmbed(t) && !i && (f && (t = f, h = 'title="YouTube video player" frameborder="0" allow="accelerometer autoplay clipboard-write encrypted-media gyroscope picture-in-picture"'), 
                    d = m || '<img src="'.concat(t, '" ').concat(h, ' class="d-block w-100" style="object-fit-cover;height:auto" />')), 
                    r && (d = t), '\n\t\t\t\t<div class="carousel-item '.concat(n ? "" : "active", '" style="min-height: 100px">\n\t\t\t\t\t').concat('<div class="position-absolute top-50 start-50 translate-middle text-white"><div class="spinner-border" style="width: 3rem height: 3rem" role="status"></div></div>', '\n\t\t\t\t\t<div class="ratio ratio-16x9" style="background-color: #000;">').concat(d, "</div>\n\t\t\t\t\t").concat(c, "\n\t\t\t\t</div>");
                }).join(""), r = this.sources.length < 2 ? "" : '\n\t\t\t<button id="#lightboxCarousel-'.concat(this.hash, '-prev" class="carousel-control carousel-control-prev h-75 m-auto" style="left:-110px" type="button" data-bs-target="#lightboxCarousel-').concat(this.hash, '" data-bs-slide="prev">\n\t\t\t\t<span class="carousel-control-prev-icon" aria-hidden="true"></span>\n\t\t\t\t<span class="visually-hidden">Previous</span>\n\t\t\t</button>\n\t\t\t<button id="#lightboxCarousel-').concat(this.hash, '-next" class="carousel-control carousel-control-next h-75 m-auto" style="right:-110px" type="button" data-bs-target="#lightboxCarousel-').concat(this.hash, '" data-bs-slide="next">\n\t\t\t\t<span class="carousel-control-next-icon" aria-hidden="true"></span>\n\t\t\t\t<span class="visually-hidden">Next</span>\n\t\t\t</button>'), i = "lightbox-carousel carousel slide";
                "fullscreen" === this.settings.size && (i += " position-absolute w-100 translate-middle top-50 start-50");
                var s = '\n\t\t\t<div class="carousel-indicators" style="bottom: -40px">\n\t\t\t\t'.concat(this.sources.map(function(t, n) {
                    return '\n\t\t\t\t\t<button type="button" data-bs-target="#lightboxCarousel-'.concat(e.hash, '" data-bs-slide-to="').concat(n, '" class="').concat(0 === n ? "active" : "", '" aria-current="').concat(0 === n ? "true" : "false", '" aria-label="Slide ').concat(n + 1, '"></button>\n\t\t\t\t');
                }).join(""), "\n\t\t\t</div>"), l = '\n\t\t\t<div id="lightboxCarousel-'.concat(this.hash, '" class="').concat(i, '" data-bs-ride="carousel" data-bs-interval="').concat(this.carouselOptions.interval, '">\n\t\t\t    <div class="carousel-inner">\n\t\t\t\t\t').concat(o, "\n\t\t\t\t</div>\n\t\t\t    ").concat(s, "\n\t\t\t\t").concat(r, "\n\t\t\t</div>");
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
                var n, a = 0, o = function(t, e) {
                    var n = "undefined" != typeof Symbol && t[Symbol.iterator] || t["@@iterator"];
                    if (!n) {
                        if (Array.isArray(t) || (n = r(t)) || e && t && "number" == typeof t.length) {
                            n && (t = n);
                            var a = 0, o = function() {};
                            return {
                                s: o,
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
                                f: o
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
                    for (o.s(); !(n = o.n()).done; ) {
                        if (n.value.includes(e)) return a;
                        a++;
                    }
                } catch (t) {
                    o.e(t);
                } finally {
                    o.f();
                }
                return 0;
            }
        }, {
            key: "createModal",
            value: function() {
                var t = this, e = document.createElement("template"), n = '\n\t\t\t<div class="modal lightbox fade" id="lightboxModal-'.concat(this.hash, '" tabindex="-1" aria-hidden="true">\n\t\t\t\t<div class="modal-dialog modal-dialog-centered modal-').concat(this.settings.size, '">\n\t\t\t\t\t<div class="modal-content border-0 bg-transparent">\n\t\t\t\t\t\t<div class="modal-body p-0">\n\t\t\t\t\t\t\t<button type="button" class="btn-close position-absolute top-0 end-0 p-3" data-bs-dismiss="modal" aria-label="Close" style="z-index: 2; background: none;">').concat('<svg xmlns="http://www.w3.org/2000/svg" style="position: relative; top: -15px;right:-40px" viewBox="0 0 16 16" fill="#fff"><path d="M.293.293a1 1 0 011.414 0L8 6.586 14.293.293a1 1 0 111.414 1.414L9.414 8l6.293 6.293a1 1 0 01-1.414 1.414L8 9.414l-6.293 6.293a1 1 0 01-1.414-1.414L6.586 8 .293 1.707a1 1 0 010-1.414z"/></svg>', "</button>\n\t\t\t\t\t\t</div>\n\t\t\t\t\t</div>\n\t\t\t\t</div>\n\t\t\t</div>");
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
        }), t;
    }();
    u.allowedEmbedTypes = [ "embed", "youtube", "vimeo", "instagram", "url" ], u.allowedMediaTypes = [].concat(o(u.allowedEmbedTypes), [ "image", "html" ]), 
    u.defaultSelector = '[data-toggle="lightbox"]', u.initialize = function(t) {
        t.preventDefault(), new u(this).show();
    }, document.querySelectorAll(u.defaultSelector).forEach(function(t) {
        return t.addEventListener("click", u.initialize);
    }), "undefined" != typeof window && window.bootstrap && (window.bootstrap.Lightbox = u);
    var d = u;
    window.Lightbox = e.default;
}();

function userCardContent(pop, delay) {
    fetch(pop.dataset.hovercard, {
        method: "GET",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json;charset=utf-8"
        }
    }).then(res => res.json()).then(profileData => {
        var content = (profileData.Avatar ? `<img src="${profileData.Avatar}" class="rounded mx-auto d-block" style="width:75px" alt="" />` : "") + '<ul class="list-unstyled m-0">' + (profileData.Location ? `<li class="px-2 py-1"><i class="fas fa-home me-1"></i>${profileData.Location}</li>` : "") + (profileData.Rank ? `<li class="px-2 py-1"><i class="fas fa-graduation-cap me-1"></i>${profileData.Rank}</li>` : "") + (profileData.Interests ? `<li class="px-2 py-1"><i class="fas fa-running me-1"></i>${profileData.Interests}</li>` : "") + (profileData.Joined ? `<li class="px-2 py-1"><i class="fas fa-user-check me-1"></i>${profileData.Joined}</li>` : "") + (profileData.HomePage ? `<li class="px-2 py-1"><i class="fas fa-globe me-1"></i><a href="${profileData.HomePage}" target="_blank">${profileData.HomePage}</a></li>` : "") + '<li class="px-2 py-1"><i class="far fa-comment me-1"></i>' + profileData.Posts + "</li>" + "</ul>";
        const popover = new bootstrap.Popover(pop, {
            delay: {
                show: delay,
                hide: 100
            },
            trigger: "hover",
            html: true,
            content: content,
            template: '<div class="popover" role="tooltip"><div class="popover-arrow"></div><div class="popover-body p-2"></div></div>'
        });
    }).catch(function(error) {
        console.log(error);
    });
}

var _self = typeof window !== "undefined" ? window : typeof WorkerGlobalScope !== "undefined" && self instanceof WorkerGlobalScope ? self : {};

var Prism = function(_self) {
    var lang = /(?:^|\s)lang(?:uage)?-([\w-]+)(?=\s|$)/i;
    var uniqueId = 0;
    var plainTextGrammar = {};
    var _ = {
        manual: _self.Prism && _self.Prism.manual,
        disableWorkerMessageHandler: _self.Prism && _self.Prism.disableWorkerMessageHandler,
        util: {
            encode: function encode(tokens) {
                if (tokens instanceof Token) {
                    return new Token(tokens.type, encode(tokens.content), tokens.alias);
                } else if (Array.isArray(tokens)) {
                    return tokens.map(encode);
                } else {
                    return tokens.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/\u00a0/g, " ");
                }
            },
            type: function(o) {
                return Object.prototype.toString.call(o).slice(8, -1);
            },
            objId: function(obj) {
                if (!obj["__id"]) {
                    Object.defineProperty(obj, "__id", {
                        value: ++uniqueId
                    });
                }
                return obj["__id"];
            },
            clone: function deepClone(o, visited) {
                visited = visited || {};
                var clone;
                var id;
                switch (_.util.type(o)) {
                  case "Object":
                    id = _.util.objId(o);
                    if (visited[id]) {
                        return visited[id];
                    }
                    clone = {};
                    visited[id] = clone;
                    for (var key in o) {
                        if (o.hasOwnProperty(key)) {
                            clone[key] = deepClone(o[key], visited);
                        }
                    }
                    return clone;

                  case "Array":
                    id = _.util.objId(o);
                    if (visited[id]) {
                        return visited[id];
                    }
                    clone = [];
                    visited[id] = clone;
                    o.forEach(function(v, i) {
                        clone[i] = deepClone(v, visited);
                    });
                    return clone;

                  default:
                    return o;
                }
            },
            getLanguage: function(element) {
                while (element) {
                    var m = lang.exec(element.className);
                    if (m) {
                        return m[1].toLowerCase();
                    }
                    element = element.parentElement;
                }
                return "none";
            },
            setLanguage: function(element, language) {
                element.className = element.className.replace(RegExp(lang, "gi"), "");
                element.classList.add("language-" + language);
            },
            currentScript: function() {
                if (typeof document === "undefined") {
                    return null;
                }
                if ("currentScript" in document && 1 < 2) {
                    return document.currentScript;
                }
                try {
                    throw new Error();
                } catch (err) {
                    var src = (/at [^(\r\n]*\((.*):[^:]+:[^:]+\)$/i.exec(err.stack) || [])[1];
                    if (src) {
                        var scripts = document.getElementsByTagName("script");
                        for (var i in scripts) {
                            if (scripts[i].src == src) {
                                return scripts[i];
                            }
                        }
                    }
                    return null;
                }
            },
            isActive: function(element, className, defaultActivation) {
                var no = "no-" + className;
                while (element) {
                    var classList = element.classList;
                    if (classList.contains(className)) {
                        return true;
                    }
                    if (classList.contains(no)) {
                        return false;
                    }
                    element = element.parentElement;
                }
                return !!defaultActivation;
            }
        },
        languages: {
            plain: plainTextGrammar,
            plaintext: plainTextGrammar,
            text: plainTextGrammar,
            txt: plainTextGrammar,
            extend: function(id, redef) {
                var lang = _.util.clone(_.languages[id]);
                for (var key in redef) {
                    lang[key] = redef[key];
                }
                return lang;
            },
            insertBefore: function(inside, before, insert, root) {
                root = root || _.languages;
                var grammar = root[inside];
                var ret = {};
                for (var token in grammar) {
                    if (grammar.hasOwnProperty(token)) {
                        if (token == before) {
                            for (var newToken in insert) {
                                if (insert.hasOwnProperty(newToken)) {
                                    ret[newToken] = insert[newToken];
                                }
                            }
                        }
                        if (!insert.hasOwnProperty(token)) {
                            ret[token] = grammar[token];
                        }
                    }
                }
                var old = root[inside];
                root[inside] = ret;
                _.languages.DFS(_.languages, function(key, value) {
                    if (value === old && key != inside) {
                        this[key] = ret;
                    }
                });
                return ret;
            },
            DFS: function DFS(o, callback, type, visited) {
                visited = visited || {};
                var objId = _.util.objId;
                for (var i in o) {
                    if (o.hasOwnProperty(i)) {
                        callback.call(o, i, o[i], type || i);
                        var property = o[i];
                        var propertyType = _.util.type(property);
                        if (propertyType === "Object" && !visited[objId(property)]) {
                            visited[objId(property)] = true;
                            DFS(property, callback, null, visited);
                        } else if (propertyType === "Array" && !visited[objId(property)]) {
                            visited[objId(property)] = true;
                            DFS(property, callback, i, visited);
                        }
                    }
                }
            }
        },
        plugins: {},
        highlightAll: function(async, callback) {
            _.highlightAllUnder(document, async, callback);
        },
        highlightAllUnder: function(container, async, callback) {
            var env = {
                callback: callback,
                container: container,
                selector: 'code[class*="language-"], [class*="language-"] code, code[class*="lang-"], [class*="lang-"] code'
            };
            _.hooks.run("before-highlightall", env);
            env.elements = Array.prototype.slice.apply(env.container.querySelectorAll(env.selector));
            _.hooks.run("before-all-elements-highlight", env);
            for (var i = 0, element; element = env.elements[i++]; ) {
                _.highlightElement(element, async === true, env.callback);
            }
        },
        highlightElement: function(element, async, callback) {
            var language = _.util.getLanguage(element);
            var grammar = _.languages[language];
            _.util.setLanguage(element, language);
            var parent = element.parentElement;
            if (parent && parent.nodeName.toLowerCase() === "pre") {
                _.util.setLanguage(parent, language);
            }
            var code = element.textContent;
            var env = {
                element: element,
                language: language,
                grammar: grammar,
                code: code
            };
            function insertHighlightedCode(highlightedCode) {
                env.highlightedCode = highlightedCode;
                _.hooks.run("before-insert", env);
                env.element.innerHTML = env.highlightedCode;
                _.hooks.run("after-highlight", env);
                _.hooks.run("complete", env);
                callback && callback.call(env.element);
            }
            _.hooks.run("before-sanity-check", env);
            parent = env.element.parentElement;
            if (parent && parent.nodeName.toLowerCase() === "pre" && !parent.hasAttribute("tabindex")) {
                parent.setAttribute("tabindex", "0");
            }
            if (!env.code) {
                _.hooks.run("complete", env);
                callback && callback.call(env.element);
                return;
            }
            _.hooks.run("before-highlight", env);
            if (!env.grammar) {
                insertHighlightedCode(_.util.encode(env.code));
                return;
            }
            if (async && _self.Worker) {
                var worker = new Worker(_.filename);
                worker.onmessage = function(evt) {
                    insertHighlightedCode(evt.data);
                };
                worker.postMessage(JSON.stringify({
                    language: env.language,
                    code: env.code,
                    immediateClose: true
                }));
            } else {
                insertHighlightedCode(_.highlight(env.code, env.grammar, env.language));
            }
        },
        highlight: function(text, grammar, language) {
            var env = {
                code: text,
                grammar: grammar,
                language: language
            };
            _.hooks.run("before-tokenize", env);
            if (!env.grammar) {
                throw new Error('The language "' + env.language + '" has no grammar.');
            }
            env.tokens = _.tokenize(env.code, env.grammar);
            _.hooks.run("after-tokenize", env);
            return Token.stringify(_.util.encode(env.tokens), env.language);
        },
        tokenize: function(text, grammar) {
            var rest = grammar.rest;
            if (rest) {
                for (var token in rest) {
                    grammar[token] = rest[token];
                }
                delete grammar.rest;
            }
            var tokenList = new LinkedList();
            addAfter(tokenList, tokenList.head, text);
            matchGrammar(text, tokenList, grammar, tokenList.head, 0);
            return toArray(tokenList);
        },
        hooks: {
            all: {},
            add: function(name, callback) {
                var hooks = _.hooks.all;
                hooks[name] = hooks[name] || [];
                hooks[name].push(callback);
            },
            run: function(name, env) {
                var callbacks = _.hooks.all[name];
                if (!callbacks || !callbacks.length) {
                    return;
                }
                for (var i = 0, callback; callback = callbacks[i++]; ) {
                    callback(env);
                }
            }
        },
        Token: Token
    };
    _self.Prism = _;
    function Token(type, content, alias, matchedStr) {
        this.type = type;
        this.content = content;
        this.alias = alias;
        this.length = (matchedStr || "").length | 0;
    }
    Token.stringify = function stringify(o, language) {
        if (typeof o == "string") {
            return o;
        }
        if (Array.isArray(o)) {
            var s = "";
            o.forEach(function(e) {
                s += stringify(e, language);
            });
            return s;
        }
        var env = {
            type: o.type,
            content: stringify(o.content, language),
            tag: "span",
            classes: [ "token", o.type ],
            attributes: {},
            language: language
        };
        var aliases = o.alias;
        if (aliases) {
            if (Array.isArray(aliases)) {
                Array.prototype.push.apply(env.classes, aliases);
            } else {
                env.classes.push(aliases);
            }
        }
        _.hooks.run("wrap", env);
        var attributes = "";
        for (var name in env.attributes) {
            attributes += " " + name + '="' + (env.attributes[name] || "").replace(/"/g, "&quot;") + '"';
        }
        return "<" + env.tag + ' class="' + env.classes.join(" ") + '"' + attributes + ">" + env.content + "</" + env.tag + ">";
    };
    function matchPattern(pattern, pos, text, lookbehind) {
        pattern.lastIndex = pos;
        var match = pattern.exec(text);
        if (match && lookbehind && match[1]) {
            var lookbehindLength = match[1].length;
            match.index += lookbehindLength;
            match[0] = match[0].slice(lookbehindLength);
        }
        return match;
    }
    function matchGrammar(text, tokenList, grammar, startNode, startPos, rematch) {
        for (var token in grammar) {
            if (!grammar.hasOwnProperty(token) || !grammar[token]) {
                continue;
            }
            var patterns = grammar[token];
            patterns = Array.isArray(patterns) ? patterns : [ patterns ];
            for (var j = 0; j < patterns.length; ++j) {
                if (rematch && rematch.cause == token + "," + j) {
                    return;
                }
                var patternObj = patterns[j];
                var inside = patternObj.inside;
                var lookbehind = !!patternObj.lookbehind;
                var greedy = !!patternObj.greedy;
                var alias = patternObj.alias;
                if (greedy && !patternObj.pattern.global) {
                    var flags = patternObj.pattern.toString().match(/[imsuy]*$/)[0];
                    patternObj.pattern = RegExp(patternObj.pattern.source, flags + "g");
                }
                var pattern = patternObj.pattern || patternObj;
                for (var currentNode = startNode.next, pos = startPos; currentNode !== tokenList.tail; pos += currentNode.value.length, 
                currentNode = currentNode.next) {
                    if (rematch && pos >= rematch.reach) {
                        break;
                    }
                    var str = currentNode.value;
                    if (tokenList.length > text.length) {
                        return;
                    }
                    if (str instanceof Token) {
                        continue;
                    }
                    var removeCount = 1;
                    var match;
                    if (greedy) {
                        match = matchPattern(pattern, pos, text, lookbehind);
                        if (!match || match.index >= text.length) {
                            break;
                        }
                        var from = match.index;
                        var to = match.index + match[0].length;
                        var p = pos;
                        p += currentNode.value.length;
                        while (from >= p) {
                            currentNode = currentNode.next;
                            p += currentNode.value.length;
                        }
                        p -= currentNode.value.length;
                        pos = p;
                        if (currentNode.value instanceof Token) {
                            continue;
                        }
                        for (var k = currentNode; k !== tokenList.tail && (p < to || typeof k.value === "string"); k = k.next) {
                            removeCount++;
                            p += k.value.length;
                        }
                        removeCount--;
                        str = text.slice(pos, p);
                        match.index -= pos;
                    } else {
                        match = matchPattern(pattern, 0, str, lookbehind);
                        if (!match) {
                            continue;
                        }
                    }
                    var from = match.index;
                    var matchStr = match[0];
                    var before = str.slice(0, from);
                    var after = str.slice(from + matchStr.length);
                    var reach = pos + str.length;
                    if (rematch && reach > rematch.reach) {
                        rematch.reach = reach;
                    }
                    var removeFrom = currentNode.prev;
                    if (before) {
                        removeFrom = addAfter(tokenList, removeFrom, before);
                        pos += before.length;
                    }
                    removeRange(tokenList, removeFrom, removeCount);
                    var wrapped = new Token(token, inside ? _.tokenize(matchStr, inside) : matchStr, alias, matchStr);
                    currentNode = addAfter(tokenList, removeFrom, wrapped);
                    if (after) {
                        addAfter(tokenList, currentNode, after);
                    }
                    if (removeCount > 1) {
                        var nestedRematch = {
                            cause: token + "," + j,
                            reach: reach
                        };
                        matchGrammar(text, tokenList, grammar, currentNode.prev, pos, nestedRematch);
                        if (rematch && nestedRematch.reach > rematch.reach) {
                            rematch.reach = nestedRematch.reach;
                        }
                    }
                }
            }
        }
    }
    function LinkedList() {
        var head = {
            value: null,
            prev: null,
            next: null
        };
        var tail = {
            value: null,
            prev: head,
            next: null
        };
        head.next = tail;
        this.head = head;
        this.tail = tail;
        this.length = 0;
    }
    function addAfter(list, node, value) {
        var next = node.next;
        var newNode = {
            value: value,
            prev: node,
            next: next
        };
        node.next = newNode;
        next.prev = newNode;
        list.length++;
        return newNode;
    }
    function removeRange(list, node, count) {
        var next = node.next;
        for (var i = 0; i < count && next !== list.tail; i++) {
            next = next.next;
        }
        node.next = next;
        next.prev = node;
        list.length -= i;
    }
    function toArray(list) {
        var array = [];
        var node = list.head.next;
        while (node !== list.tail) {
            array.push(node.value);
            node = node.next;
        }
        return array;
    }
    if (!_self.document) {
        if (!_self.addEventListener) {
            return _;
        }
        if (!_.disableWorkerMessageHandler) {
            _self.addEventListener("message", function(evt) {
                var message = JSON.parse(evt.data);
                var lang = message.language;
                var code = message.code;
                var immediateClose = message.immediateClose;
                _self.postMessage(_.highlight(code, _.languages[lang], lang));
                if (immediateClose) {
                    _self.close();
                }
            }, false);
        }
        return _;
    }
    var script = _.util.currentScript();
    if (script) {
        _.filename = script.src;
        if (script.hasAttribute("data-manual")) {
            _.manual = true;
        }
    }
    function highlightAutomaticallyCallback() {
        if (!_.manual) {
            _.highlightAll();
        }
    }
    if (!_.manual) {
        var readyState = document.readyState;
        if (readyState === "loading" || readyState === "interactive" && script && script.defer) {
            document.addEventListener("DOMContentLoaded", highlightAutomaticallyCallback);
        } else {
            if (window.requestAnimationFrame) {
                window.requestAnimationFrame(highlightAutomaticallyCallback);
            } else {
                window.setTimeout(highlightAutomaticallyCallback, 16);
            }
        }
    }
    return _;
}(_self);

if (typeof module !== "undefined" && module.exports) {
    module.exports = Prism;
}

if (typeof global !== "undefined") {
    global.Prism = Prism;
}

Prism.languages.markup = {
    comment: {
        pattern: /<!--(?:(?!<!--)[\s\S])*?-->/,
        greedy: true
    },
    prolog: {
        pattern: /<\?[\s\S]+?\?>/,
        greedy: true
    },
    doctype: {
        pattern: /<!DOCTYPE(?:[^>"'[\]]|"[^"]*"|'[^']*')+(?:\[(?:[^<"'\]]|"[^"]*"|'[^']*'|<(?!!--)|<!--(?:[^-]|-(?!->))*-->)*\]\s*)?>/i,
        greedy: true,
        inside: {
            "internal-subset": {
                pattern: /(^[^\[]*\[)[\s\S]+(?=\]>$)/,
                lookbehind: true,
                greedy: true,
                inside: null
            },
            string: {
                pattern: /"[^"]*"|'[^']*'/,
                greedy: true
            },
            punctuation: /^<!|>$|[[\]]/,
            "doctype-tag": /^DOCTYPE/i,
            name: /[^\s<>'"]+/
        }
    },
    cdata: {
        pattern: /<!\[CDATA\[[\s\S]*?\]\]>/i,
        greedy: true
    },
    tag: {
        pattern: /<\/?(?!\d)[^\s>\/=$<%]+(?:\s(?:\s*[^\s>\/=]+(?:\s*=\s*(?:"[^"]*"|'[^']*'|[^\s'">=]+(?=[\s>]))|(?=[\s/>])))+)?\s*\/?>/,
        greedy: true,
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
                        lookbehind: true
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
};

Prism.languages.markup["tag"].inside["attr-value"].inside["entity"] = Prism.languages.markup["entity"];

Prism.languages.markup["doctype"].inside["internal-subset"].inside = Prism.languages.markup;

Prism.hooks.add("wrap", function(env) {
    if (env.type === "entity") {
        env.attributes["title"] = env.content.replace(/&amp;/, "&");
    }
});

Object.defineProperty(Prism.languages.markup.tag, "addInlined", {
    value: function addInlined(tagName, lang) {
        var includedCdataInside = {};
        includedCdataInside["language-" + lang] = {
            pattern: /(^<!\[CDATA\[)[\s\S]+?(?=\]\]>$)/i,
            lookbehind: true,
            inside: Prism.languages[lang]
        };
        includedCdataInside["cdata"] = /^<!\[CDATA\[|\]\]>$/i;
        var inside = {
            "included-cdata": {
                pattern: /<!\[CDATA\[[\s\S]*?\]\]>/i,
                inside: includedCdataInside
            }
        };
        inside["language-" + lang] = {
            pattern: /[\s\S]+/,
            inside: Prism.languages[lang]
        };
        var def = {};
        def[tagName] = {
            pattern: RegExp(/(<__[^>]*>)(?:<!\[CDATA\[(?:[^\]]|\](?!\]>))*\]\]>|(?!<!\[CDATA\[)[\s\S])*?(?=<\/__>)/.source.replace(/__/g, function() {
                return tagName;
            }), "i"),
            lookbehind: true,
            greedy: true,
            inside: inside
        };
        Prism.languages.insertBefore("markup", "cdata", def);
    }
});

Object.defineProperty(Prism.languages.markup.tag, "addAttribute", {
    value: function(attrName, lang) {
        Prism.languages.markup.tag.inside["special-attr"].push({
            pattern: RegExp(/(^|["'\s])/.source + "(?:" + attrName + ")" + /\s*=\s*(?:"[^"]*"|'[^']*'|[^\s'">=]+(?=[\s>]))/.source, "i"),
            lookbehind: true,
            inside: {
                "attr-name": /^[^\s=]+/,
                "attr-value": {
                    pattern: /=[\s\S]+/,
                    inside: {
                        value: {
                            pattern: /(^=\s*(["']|(?!["'])))\S[\s\S]*(?=\2$)/,
                            lookbehind: true,
                            alias: [ lang, "language-" + lang ],
                            inside: Prism.languages[lang]
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
});

Prism.languages.html = Prism.languages.markup;

Prism.languages.mathml = Prism.languages.markup;

Prism.languages.svg = Prism.languages.markup;

Prism.languages.xml = Prism.languages.extend("markup", {});

Prism.languages.ssml = Prism.languages.xml;

Prism.languages.atom = Prism.languages.xml;

Prism.languages.rss = Prism.languages.xml;

(function(Prism) {
    var string = /(?:"(?:\\(?:\r\n|[\s\S])|[^"\\\r\n])*"|'(?:\\(?:\r\n|[\s\S])|[^'\\\r\n])*')/;
    Prism.languages.css = {
        comment: /\/\*[\s\S]*?\*\//,
        atrule: {
            pattern: RegExp("@[\\w-](?:" + /[^;{\s"']|\s+(?!\s)/.source + "|" + string.source + ")*?" + /(?:;|(?=\s*\{))/.source),
            inside: {
                rule: /^@[\w-]+/,
                "selector-function-argument": {
                    pattern: /(\bselector\s*\(\s*(?![\s)]))(?:[^()\s]|\s+(?![\s)])|\((?:[^()]|\([^()]*\))*\))+(?=\s*\))/,
                    lookbehind: true,
                    alias: "selector"
                },
                keyword: {
                    pattern: /(^|[^\w-])(?:and|not|only|or)(?![\w-])/,
                    lookbehind: true
                }
            }
        },
        url: {
            pattern: RegExp("\\burl\\((?:" + string.source + "|" + /(?:[^\\\r\n()"']|\\[\s\S])*/.source + ")\\)", "i"),
            greedy: true,
            inside: {
                function: /^url/i,
                punctuation: /^\(|\)$/,
                string: {
                    pattern: RegExp("^" + string.source + "$"),
                    alias: "url"
                }
            }
        },
        selector: {
            pattern: RegExp("(^|[{}\\s])[^{}\\s](?:[^{};\"'\\s]|\\s+(?![\\s{])|" + string.source + ")*(?=\\s*\\{)"),
            lookbehind: true
        },
        string: {
            pattern: string,
            greedy: true
        },
        property: {
            pattern: /(^|[^-\w\xA0-\uFFFF])(?!\s)[-_a-z\xA0-\uFFFF](?:(?!\s)[-\w\xA0-\uFFFF])*(?=\s*:)/i,
            lookbehind: true
        },
        important: /!important\b/i,
        function: {
            pattern: /(^|[^-a-z0-9])[-a-z0-9]+(?=\()/i,
            lookbehind: true
        },
        punctuation: /[(){};:,]/
    };
    Prism.languages.css["atrule"].inside.rest = Prism.languages.css;
    var markup = Prism.languages.markup;
    if (markup) {
        markup.tag.addInlined("style", "css");
        markup.tag.addAttribute("style", "css");
    }
})(Prism);

Prism.languages.clike = {
    comment: [ {
        pattern: /(^|[^\\])\/\*[\s\S]*?(?:\*\/|$)/,
        lookbehind: true,
        greedy: true
    }, {
        pattern: /(^|[^\\:])\/\/.*/,
        lookbehind: true,
        greedy: true
    } ],
    string: {
        pattern: /(["'])(?:\\(?:\r\n|[\s\S])|(?!\1)[^\\\r\n])*\1/,
        greedy: true
    },
    "class-name": {
        pattern: /(\b(?:class|extends|implements|instanceof|interface|new|trait)\s+|\bcatch\s+\()[\w.\\]+/i,
        lookbehind: true,
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
        lookbehind: true
    } ],
    keyword: [ {
        pattern: /((?:^|\})\s*)catch\b/,
        lookbehind: true
    }, {
        pattern: /(^|[^.]|\.\.\.\s*)\b(?:as|assert(?=\s*\{)|async(?=\s*(?:function\b|\(|[$\w\xA0-\uFFFF]|$))|await|break|case|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally(?=\s*(?:\{|$))|for|from(?=\s*(?:['"]|$))|function|(?:get|set)(?=\s*(?:[#\[$\w\xA0-\uFFFF]|$))|if|implements|import|in|instanceof|interface|let|new|null|of|package|private|protected|public|return|static|super|switch|this|throw|try|typeof|undefined|var|void|while|with|yield)\b/,
        lookbehind: true
    } ],
    function: /#?(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*(?:\.\s*(?:apply|bind|call)\s*)?\()/,
    number: {
        pattern: RegExp(/(^|[^\w$])/.source + "(?:" + (/NaN|Infinity/.source + "|" + /0[bB][01]+(?:_[01]+)*n?/.source + "|" + /0[oO][0-7]+(?:_[0-7]+)*n?/.source + "|" + /0[xX][\dA-Fa-f]+(?:_[\dA-Fa-f]+)*n?/.source + "|" + /\d+(?:_\d+)*n/.source + "|" + /(?:\d+(?:_\d+)*(?:\.(?:\d+(?:_\d+)*)?)?|\.\d+(?:_\d+)*)(?:[Ee][+-]?\d+(?:_\d+)*)?/.source) + ")" + /(?![\w$])/.source),
        lookbehind: true
    },
    operator: /--|\+\+|\*\*=?|=>|&&=?|\|\|=?|[!=]==|<<=?|>>>?=?|[-+*/%&|^!=<>]=?|\.{3}|\?\?=?|\?\.?|[~:]/
});

Prism.languages.javascript["class-name"][0].pattern = /(\b(?:class|extends|implements|instanceof|interface|new)\s+)[\w.\\]+/;

Prism.languages.insertBefore("javascript", "keyword", {
    regex: {
        pattern: RegExp(/((?:^|[^$\w\xA0-\uFFFF."'\])\s]|\b(?:return|yield))\s*)/.source + /\//.source + "(?:" + /(?:\[(?:[^\]\\\r\n]|\\.)*\]|\\.|[^/\\\[\r\n])+\/[dgimyus]{0,7}/.source + "|" + /(?:\[(?:[^[\]\\\r\n]|\\.|\[(?:[^[\]\\\r\n]|\\.|\[(?:[^[\]\\\r\n]|\\.)*\])*\])*\]|\\.|[^/\\\[\r\n])+\/[dgimyus]{0,7}v[dgimyus]{0,7}/.source + ")" + /(?=(?:\s|\/\*(?:[^*]|\*(?!\/))*\*\/)*(?:$|[\r\n,.;:})\]]|\/\/))/.source),
        lookbehind: true,
        greedy: true,
        inside: {
            "regex-source": {
                pattern: /^(\/)[\s\S]+(?=\/[a-z]*$)/,
                lookbehind: true,
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
        lookbehind: true,
        inside: Prism.languages.javascript
    }, {
        pattern: /(^|[^$\w\xA0-\uFFFF])(?!\s)[_$a-z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*=>)/i,
        lookbehind: true,
        inside: Prism.languages.javascript
    }, {
        pattern: /(\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\)\s*=>)/,
        lookbehind: true,
        inside: Prism.languages.javascript
    }, {
        pattern: /((?:\b|\s|^)(?!(?:as|async|await|break|case|catch|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally|for|from|function|get|if|implements|import|in|instanceof|interface|let|new|null|of|package|private|protected|public|return|set|static|super|switch|this|throw|try|typeof|undefined|var|void|while|with|yield)(?![$\w\xA0-\uFFFF]))(?:(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*\s*)\(\s*|\]\s*\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\)\s*\{)/,
        lookbehind: true,
        inside: Prism.languages.javascript
    } ],
    constant: /\b[A-Z](?:[A-Z_]|\dx?)*\b/
});

Prism.languages.insertBefore("javascript", "string", {
    hashbang: {
        pattern: /^#!.*/,
        greedy: true,
        alias: "comment"
    },
    "template-string": {
        pattern: /`(?:\\[\s\S]|\$\{(?:[^{}]|\{(?:[^{}]|\{[^}]*\})*\})+\}|(?!\$\{)[^\\`])*`/,
        greedy: true,
        inside: {
            "template-punctuation": {
                pattern: /^`|`$/,
                alias: "string"
            },
            interpolation: {
                pattern: /((?:^|[^\\])(?:\\{2})*)\$\{(?:[^{}]|\{(?:[^{}]|\{[^}]*\})*\})+\}/,
                lookbehind: true,
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
        lookbehind: true,
        greedy: true,
        alias: "property"
    }
});

Prism.languages.insertBefore("javascript", "operator", {
    "literal-property": {
        pattern: /((?:^|[,{])[ \t]*)(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*:)/m,
        lookbehind: true,
        alias: "property"
    }
});

if (Prism.languages.markup) {
    Prism.languages.markup.tag.addInlined("script", "javascript");
    Prism.languages.markup.tag.addAttribute(/on(?:abort|blur|change|click|composition(?:end|start|update)|dblclick|error|focus(?:in|out)?|key(?:down|up)|load|mouse(?:down|enter|leave|move|out|over|up)|reset|resize|scroll|select|slotchange|submit|unload|wheel)/.source, "javascript");
}

Prism.languages.js = Prism.languages.javascript;

(function(Prism) {
    function replace(pattern, replacements) {
        return pattern.replace(/<<(\d+)>>/g, function(m, index) {
            return "(?:" + replacements[+index] + ")";
        });
    }
    function re(pattern, replacements, flags) {
        return RegExp(replace(pattern, replacements), flags || "");
    }
    function nested(pattern, depthLog2) {
        for (var i = 0; i < depthLog2; i++) {
            pattern = pattern.replace(/<<self>>/g, function() {
                return "(?:" + pattern + ")";
            });
        }
        return pattern.replace(/<<self>>/g, "[^\\s\\S]");
    }
    var keywordKinds = {
        type: "bool byte char decimal double dynamic float int long object sbyte short string uint ulong ushort var void",
        typeDeclaration: "class enum interface record struct",
        contextual: "add alias and ascending async await by descending from(?=\\s*(?:\\w|$)) get global group into init(?=\\s*;) join let nameof not notnull on or orderby partial remove select set unmanaged value when where with(?=\\s*{)",
        other: "abstract as base break case catch checked const continue default delegate do else event explicit extern finally fixed for foreach goto if implicit in internal is lock namespace new null operator out override params private protected public readonly ref return sealed sizeof stackalloc static switch this throw try typeof unchecked unsafe using virtual volatile while yield"
    };
    function keywordsToPattern(words) {
        return "\\b(?:" + words.trim().replace(/ /g, "|") + ")\\b";
    }
    var typeDeclarationKeywords = keywordsToPattern(keywordKinds.typeDeclaration);
    var keywords = RegExp(keywordsToPattern(keywordKinds.type + " " + keywordKinds.typeDeclaration + " " + keywordKinds.contextual + " " + keywordKinds.other));
    var nonTypeKeywords = keywordsToPattern(keywordKinds.typeDeclaration + " " + keywordKinds.contextual + " " + keywordKinds.other);
    var nonContextualKeywords = keywordsToPattern(keywordKinds.type + " " + keywordKinds.typeDeclaration + " " + keywordKinds.other);
    var generic = nested(/<(?:[^<>;=+\-*/%&|^]|<<self>>)*>/.source, 2);
    var nestedRound = nested(/\((?:[^()]|<<self>>)*\)/.source, 2);
    var name = /@?\b[A-Za-z_]\w*\b/.source;
    var genericName = replace(/<<0>>(?:\s*<<1>>)?/.source, [ name, generic ]);
    var identifier = replace(/(?!<<0>>)<<1>>(?:\s*\.\s*<<1>>)*/.source, [ nonTypeKeywords, genericName ]);
    var array = /\[\s*(?:,\s*)*\]/.source;
    var typeExpressionWithoutTuple = replace(/<<0>>(?:\s*(?:\?\s*)?<<1>>)*(?:\s*\?)?/.source, [ identifier, array ]);
    var tupleElement = replace(/[^,()<>[\];=+\-*/%&|^]|<<0>>|<<1>>|<<2>>/.source, [ generic, nestedRound, array ]);
    var tuple = replace(/\(<<0>>+(?:,<<0>>+)+\)/.source, [ tupleElement ]);
    var typeExpression = replace(/(?:<<0>>|<<1>>)(?:\s*(?:\?\s*)?<<2>>)*(?:\s*\?)?/.source, [ tuple, identifier, array ]);
    var typeInside = {
        keyword: keywords,
        punctuation: /[<>()?,.:[\]]/
    };
    var character = /'(?:[^\r\n'\\]|\\.|\\[Uux][\da-fA-F]{1,8})'/.source;
    var regularString = /"(?:\\.|[^\\"\r\n])*"/.source;
    var verbatimString = /@"(?:""|\\[\s\S]|[^\\"])*"(?!")/.source;
    Prism.languages.csharp = Prism.languages.extend("clike", {
        string: [ {
            pattern: re(/(^|[^$\\])<<0>>/.source, [ verbatimString ]),
            lookbehind: true,
            greedy: true
        }, {
            pattern: re(/(^|[^@$\\])<<0>>/.source, [ regularString ]),
            lookbehind: true,
            greedy: true
        } ],
        "class-name": [ {
            pattern: re(/(\busing\s+static\s+)<<0>>(?=\s*;)/.source, [ identifier ]),
            lookbehind: true,
            inside: typeInside
        }, {
            pattern: re(/(\busing\s+<<0>>\s*=\s*)<<1>>(?=\s*;)/.source, [ name, typeExpression ]),
            lookbehind: true,
            inside: typeInside
        }, {
            pattern: re(/(\busing\s+)<<0>>(?=\s*=)/.source, [ name ]),
            lookbehind: true
        }, {
            pattern: re(/(\b<<0>>\s+)<<1>>/.source, [ typeDeclarationKeywords, genericName ]),
            lookbehind: true,
            inside: typeInside
        }, {
            pattern: re(/(\bcatch\s*\(\s*)<<0>>/.source, [ identifier ]),
            lookbehind: true,
            inside: typeInside
        }, {
            pattern: re(/(\bwhere\s+)<<0>>/.source, [ name ]),
            lookbehind: true
        }, {
            pattern: re(/(\b(?:is(?:\s+not)?|as)\s+)<<0>>/.source, [ typeExpressionWithoutTuple ]),
            lookbehind: true,
            inside: typeInside
        }, {
            pattern: re(/\b<<0>>(?=\s+(?!<<1>>|with\s*\{)<<2>>(?:\s*[=,;:{)\]]|\s+(?:in|when)\b))/.source, [ typeExpression, nonContextualKeywords, name ]),
            inside: typeInside
        } ],
        keyword: keywords,
        number: /(?:\b0(?:x[\da-f_]*[\da-f]|b[01_]*[01])|(?:\B\.\d+(?:_+\d+)*|\b\d+(?:_+\d+)*(?:\.\d+(?:_+\d+)*)?)(?:e[-+]?\d+(?:_+\d+)*)?)(?:[dflmu]|lu|ul)?\b/i,
        operator: />>=?|<<=?|[-=]>|([-+&|])\1|~|\?\?=?|[-+*/%&|^!=<>]=?/,
        punctuation: /\?\.?|::|[{}[\];(),.:]/
    });
    Prism.languages.insertBefore("csharp", "number", {
        range: {
            pattern: /\.\./,
            alias: "operator"
        }
    });
    Prism.languages.insertBefore("csharp", "punctuation", {
        "named-parameter": {
            pattern: re(/([(,]\s*)<<0>>(?=\s*:)/.source, [ name ]),
            lookbehind: true,
            alias: "punctuation"
        }
    });
    Prism.languages.insertBefore("csharp", "class-name", {
        namespace: {
            pattern: re(/(\b(?:namespace|using)\s+)<<0>>(?:\s*\.\s*<<0>>)*(?=\s*[;{])/.source, [ name ]),
            lookbehind: true,
            inside: {
                punctuation: /\./
            }
        },
        "type-expression": {
            pattern: re(/(\b(?:default|sizeof|typeof)\s*\(\s*(?!\s))(?:[^()\s]|\s(?!\s)|<<0>>)*(?=\s*\))/.source, [ nestedRound ]),
            lookbehind: true,
            alias: "class-name",
            inside: typeInside
        },
        "return-type": {
            pattern: re(/<<0>>(?=\s+(?:<<1>>\s*(?:=>|[({]|\.\s*this\s*\[)|this\s*\[))/.source, [ typeExpression, identifier ]),
            inside: typeInside,
            alias: "class-name"
        },
        "constructor-invocation": {
            pattern: re(/(\bnew\s+)<<0>>(?=\s*[[({])/.source, [ typeExpression ]),
            lookbehind: true,
            inside: typeInside,
            alias: "class-name"
        },
        "generic-method": {
            pattern: re(/<<0>>\s*<<1>>(?=\s*\()/.source, [ name, generic ]),
            inside: {
                function: re(/^<<0>>/.source, [ name ]),
                generic: {
                    pattern: RegExp(generic),
                    alias: "class-name",
                    inside: typeInside
                }
            }
        },
        "type-list": {
            pattern: re(/\b((?:<<0>>\s+<<1>>|record\s+<<1>>\s*<<5>>|where\s+<<2>>)\s*:\s*)(?:<<3>>|<<4>>|<<1>>\s*<<5>>|<<6>>)(?:\s*,\s*(?:<<3>>|<<4>>|<<6>>))*(?=\s*(?:where|[{;]|=>|$))/.source, [ typeDeclarationKeywords, genericName, name, typeExpression, keywords.source, nestedRound, /\bnew\s*\(\s*\)/.source ]),
            lookbehind: true,
            inside: {
                "record-arguments": {
                    pattern: re(/(^(?!new\s*\()<<0>>\s*)<<1>>/.source, [ genericName, nestedRound ]),
                    lookbehind: true,
                    greedy: true,
                    inside: Prism.languages.csharp
                },
                keyword: keywords,
                "class-name": {
                    pattern: RegExp(typeExpression),
                    greedy: true,
                    inside: typeInside
                },
                punctuation: /[,()]/
            }
        },
        preprocessor: {
            pattern: /(^[\t ]*)#.*/m,
            lookbehind: true,
            alias: "property",
            inside: {
                directive: {
                    pattern: /(#)\b(?:define|elif|else|endif|endregion|error|if|line|nullable|pragma|region|undef|warning)\b/,
                    lookbehind: true,
                    alias: "keyword"
                }
            }
        }
    });
    var regularStringOrCharacter = regularString + "|" + character;
    var regularStringCharacterOrComment = replace(/\/(?![*/])|\/\/[^\r\n]*[\r\n]|\/\*(?:[^*]|\*(?!\/))*\*\/|<<0>>/.source, [ regularStringOrCharacter ]);
    var roundExpression = nested(replace(/[^"'/()]|<<0>>|\(<<self>>*\)/.source, [ regularStringCharacterOrComment ]), 2);
    var attrTarget = /\b(?:assembly|event|field|method|module|param|property|return|type)\b/.source;
    var attr = replace(/<<0>>(?:\s*\(<<1>>*\))?/.source, [ identifier, roundExpression ]);
    Prism.languages.insertBefore("csharp", "class-name", {
        attribute: {
            pattern: re(/((?:^|[^\s\w>)?])\s*\[\s*)(?:<<0>>\s*:\s*)?<<1>>(?:\s*,\s*<<1>>)*(?=\s*\])/.source, [ attrTarget, attr ]),
            lookbehind: true,
            greedy: true,
            inside: {
                target: {
                    pattern: re(/^<<0>>(?=\s*:)/.source, [ attrTarget ]),
                    alias: "keyword"
                },
                "attribute-arguments": {
                    pattern: re(/\(<<0>>*\)/.source, [ roundExpression ]),
                    inside: Prism.languages.csharp
                },
                "class-name": {
                    pattern: RegExp(identifier),
                    inside: {
                        punctuation: /\./
                    }
                },
                punctuation: /[:,]/
            }
        }
    });
    var formatString = /:[^}\r\n]+/.source;
    var mInterpolationRound = nested(replace(/[^"'/()]|<<0>>|\(<<self>>*\)/.source, [ regularStringCharacterOrComment ]), 2);
    var mInterpolation = replace(/\{(?!\{)(?:(?![}:])<<0>>)*<<1>>?\}/.source, [ mInterpolationRound, formatString ]);
    var sInterpolationRound = nested(replace(/[^"'/()]|\/(?!\*)|\/\*(?:[^*]|\*(?!\/))*\*\/|<<0>>|\(<<self>>*\)/.source, [ regularStringOrCharacter ]), 2);
    var sInterpolation = replace(/\{(?!\{)(?:(?![}:])<<0>>)*<<1>>?\}/.source, [ sInterpolationRound, formatString ]);
    function createInterpolationInside(interpolation, interpolationRound) {
        return {
            interpolation: {
                pattern: re(/((?:^|[^{])(?:\{\{)*)<<0>>/.source, [ interpolation ]),
                lookbehind: true,
                inside: {
                    "format-string": {
                        pattern: re(/(^\{(?:(?![}:])<<0>>)*)<<1>>(?=\}$)/.source, [ interpolationRound, formatString ]),
                        lookbehind: true,
                        inside: {
                            punctuation: /^:/
                        }
                    },
                    punctuation: /^\{|\}$/,
                    expression: {
                        pattern: /[\s\S]+/,
                        alias: "language-csharp",
                        inside: Prism.languages.csharp
                    }
                }
            },
            string: /[\s\S]+/
        };
    }
    Prism.languages.insertBefore("csharp", "string", {
        "interpolation-string": [ {
            pattern: re(/(^|[^\\])(?:\$@|@\$)"(?:""|\\[\s\S]|\{\{|<<0>>|[^\\{"])*"/.source, [ mInterpolation ]),
            lookbehind: true,
            greedy: true,
            inside: createInterpolationInside(mInterpolation, mInterpolationRound)
        }, {
            pattern: re(/(^|[^@\\])\$"(?:\\.|\{\{|<<0>>|[^\\"{])*"/.source, [ sInterpolation ]),
            lookbehind: true,
            greedy: true,
            inside: createInterpolationInside(sInterpolation, sInterpolationRound)
        } ],
        char: {
            pattern: RegExp(character),
            greedy: true
        }
    });
    Prism.languages.dotnet = Prism.languages.cs = Prism.languages.csharp;
})(Prism);

Prism.languages.aspnet = Prism.languages.extend("markup", {
    "page-directive": {
        pattern: /<%\s*@.*%>/,
        alias: "tag",
        inside: {
            "page-directive": {
                pattern: /<%\s*@\s*(?:Assembly|Control|Implements|Import|Master(?:Type)?|OutputCache|Page|PreviousPageType|Reference|Register)?|%>/i,
                alias: "tag"
            },
            rest: Prism.languages.markup.tag.inside
        }
    },
    directive: {
        pattern: /<%.*%>/,
        alias: "tag",
        inside: {
            directive: {
                pattern: /<%\s*?[$=%#:]{0,2}|%>/,
                alias: "tag"
            },
            rest: Prism.languages.csharp
        }
    }
});

Prism.languages.aspnet.tag.pattern = /<(?!%)\/?[^\s>\/]+(?:\s+[^\s>\/=]+(?:=(?:("|')(?:\\[\s\S]|(?!\1)[^\\])*\1|[^\s'">=]+))?)*\s*\/?>/;

Prism.languages.insertBefore("inside", "punctuation", {
    directive: Prism.languages.aspnet["directive"]
}, Prism.languages.aspnet.tag.inside["attr-value"]);

Prism.languages.insertBefore("aspnet", "comment", {
    "asp-comment": {
        pattern: /<%--[\s\S]*?--%>/,
        alias: [ "asp", "comment" ]
    }
});

Prism.languages.insertBefore("aspnet", Prism.languages.javascript ? "script" : "tag", {
    "asp-script": {
        pattern: /(<script(?=.*runat=['"]?server\b)[^>]*>)[\s\S]*?(?=<\/script>)/i,
        lookbehind: true,
        alias: [ "asp", "script" ],
        inside: Prism.languages.csharp || {}
    }
});

(function(Prism) {
    var envVars = "\\b(?:BASH|BASHOPTS|BASH_ALIASES|BASH_ARGC|BASH_ARGV|BASH_CMDS|BASH_COMPLETION_COMPAT_DIR|BASH_LINENO|BASH_REMATCH|BASH_SOURCE|BASH_VERSINFO|BASH_VERSION|COLORTERM|COLUMNS|COMP_WORDBREAKS|DBUS_SESSION_BUS_ADDRESS|DEFAULTS_PATH|DESKTOP_SESSION|DIRSTACK|DISPLAY|EUID|GDMSESSION|GDM_LANG|GNOME_KEYRING_CONTROL|GNOME_KEYRING_PID|GPG_AGENT_INFO|GROUPS|HISTCONTROL|HISTFILE|HISTFILESIZE|HISTSIZE|HOME|HOSTNAME|HOSTTYPE|IFS|INSTANCE|JOB|LANG|LANGUAGE|LC_ADDRESS|LC_ALL|LC_IDENTIFICATION|LC_MEASUREMENT|LC_MONETARY|LC_NAME|LC_NUMERIC|LC_PAPER|LC_TELEPHONE|LC_TIME|LESSCLOSE|LESSOPEN|LINES|LOGNAME|LS_COLORS|MACHTYPE|MAILCHECK|MANDATORY_PATH|NO_AT_BRIDGE|OLDPWD|OPTERR|OPTIND|ORBIT_SOCKETDIR|OSTYPE|PAPERSIZE|PATH|PIPESTATUS|PPID|PS1|PS2|PS3|PS4|PWD|RANDOM|REPLY|SECONDS|SELINUX_INIT|SESSION|SESSIONTYPE|SESSION_MANAGER|SHELL|SHELLOPTS|SHLVL|SSH_AUTH_SOCK|TERM|UID|UPSTART_EVENTS|UPSTART_INSTANCE|UPSTART_JOB|UPSTART_SESSION|USER|WINDOWID|XAUTHORITY|XDG_CONFIG_DIRS|XDG_CURRENT_DESKTOP|XDG_DATA_DIRS|XDG_GREETER_DATA_DIR|XDG_MENU_PREFIX|XDG_RUNTIME_DIR|XDG_SEAT|XDG_SEAT_PATH|XDG_SESSION_DESKTOP|XDG_SESSION_ID|XDG_SESSION_PATH|XDG_SESSION_TYPE|XDG_VTNR|XMODIFIERS)\\b";
    var commandAfterHeredoc = {
        pattern: /(^(["']?)\w+\2)[ \t]+\S.*/,
        lookbehind: true,
        alias: "punctuation",
        inside: null
    };
    var insideString = {
        bash: commandAfterHeredoc,
        environment: {
            pattern: RegExp("\\$" + envVars),
            alias: "constant"
        },
        variable: [ {
            pattern: /\$?\(\([\s\S]+?\)\)/,
            greedy: true,
            inside: {
                variable: [ {
                    pattern: /(^\$\(\([\s\S]+)\)\)/,
                    lookbehind: true
                }, /^\$\(\(/ ],
                number: /\b0x[\dA-Fa-f]+\b|(?:\b\d+(?:\.\d*)?|\B\.\d+)(?:[Ee]-?\d+)?/,
                operator: /--|\+\+|\*\*=?|<<=?|>>=?|&&|\|\||[=!+\-*/%<>^&|]=?|[?~:]/,
                punctuation: /\(\(?|\)\)?|,|;/
            }
        }, {
            pattern: /\$\((?:\([^)]+\)|[^()])+\)|`[^`]+`/,
            greedy: true,
            inside: {
                variable: /^\$\(|^`|\)$|`$/
            }
        }, {
            pattern: /\$\{[^}]+\}/,
            greedy: true,
            inside: {
                operator: /:[-=?+]?|[!\/]|##?|%%?|\^\^?|,,?/,
                punctuation: /[\[\]]/,
                environment: {
                    pattern: RegExp("(\\{)" + envVars),
                    lookbehind: true,
                    alias: "constant"
                }
            }
        }, /\$(?:\w+|[#?*!@$])/ ],
        entity: /\\(?:[abceEfnrtv\\"]|O?[0-7]{1,3}|U[0-9a-fA-F]{8}|u[0-9a-fA-F]{4}|x[0-9a-fA-F]{1,2})/
    };
    Prism.languages.bash = {
        shebang: {
            pattern: /^#!\s*\/.*/,
            alias: "important"
        },
        comment: {
            pattern: /(^|[^"{\\$])#.*/,
            lookbehind: true
        },
        "function-name": [ {
            pattern: /(\bfunction\s+)[\w-]+(?=(?:\s*\(?:\s*\))?\s*\{)/,
            lookbehind: true,
            alias: "function"
        }, {
            pattern: /\b[\w-]+(?=\s*\(\s*\)\s*\{)/,
            alias: "function"
        } ],
        "for-or-select": {
            pattern: /(\b(?:for|select)\s+)\w+(?=\s+in\s)/,
            alias: "variable",
            lookbehind: true
        },
        "assign-left": {
            pattern: /(^|[\s;|&]|[<>]\()\w+(?:\.\w+)*(?=\+?=)/,
            inside: {
                environment: {
                    pattern: RegExp("(^|[\\s;|&]|[<>]\\()" + envVars),
                    lookbehind: true,
                    alias: "constant"
                }
            },
            alias: "variable",
            lookbehind: true
        },
        parameter: {
            pattern: /(^|\s)-{1,2}(?:\w+:[+-]?)?\w+(?:\.\w+)*(?=[=\s]|$)/,
            alias: "variable",
            lookbehind: true
        },
        string: [ {
            pattern: /((?:^|[^<])<<-?\s*)(\w+)\s[\s\S]*?(?:\r?\n|\r)\2/,
            lookbehind: true,
            greedy: true,
            inside: insideString
        }, {
            pattern: /((?:^|[^<])<<-?\s*)(["'])(\w+)\2\s[\s\S]*?(?:\r?\n|\r)\3/,
            lookbehind: true,
            greedy: true,
            inside: {
                bash: commandAfterHeredoc
            }
        }, {
            pattern: /(^|[^\\](?:\\\\)*)"(?:\\[\s\S]|\$\([^)]+\)|\$(?!\()|`[^`]+`|[^"\\`$])*"/,
            lookbehind: true,
            greedy: true,
            inside: insideString
        }, {
            pattern: /(^|[^$\\])'[^']*'/,
            lookbehind: true,
            greedy: true
        }, {
            pattern: /\$'(?:[^'\\]|\\[\s\S])*'/,
            greedy: true,
            inside: {
                entity: insideString.entity
            }
        } ],
        environment: {
            pattern: RegExp("\\$?" + envVars),
            alias: "constant"
        },
        variable: insideString.variable,
        function: {
            pattern: /(^|[\s;|&]|[<>]\()(?:add|apropos|apt|apt-cache|apt-get|aptitude|aspell|automysqlbackup|awk|basename|bash|bc|bconsole|bg|bzip2|cal|cargo|cat|cfdisk|chgrp|chkconfig|chmod|chown|chroot|cksum|clear|cmp|column|comm|composer|cp|cron|crontab|csplit|curl|cut|date|dc|dd|ddrescue|debootstrap|df|diff|diff3|dig|dir|dircolors|dirname|dirs|dmesg|docker|docker-compose|du|egrep|eject|env|ethtool|expand|expect|expr|fdformat|fdisk|fg|fgrep|file|find|fmt|fold|format|free|fsck|ftp|fuser|gawk|git|gparted|grep|groupadd|groupdel|groupmod|groups|grub-mkconfig|gzip|halt|head|hg|history|host|hostname|htop|iconv|id|ifconfig|ifdown|ifup|import|install|ip|java|jobs|join|kill|killall|less|link|ln|locate|logname|logrotate|look|lpc|lpr|lprint|lprintd|lprintq|lprm|ls|lsof|lynx|make|man|mc|mdadm|mkconfig|mkdir|mke2fs|mkfifo|mkfs|mkisofs|mknod|mkswap|mmv|more|most|mount|mtools|mtr|mutt|mv|nano|nc|netstat|nice|nl|node|nohup|notify-send|npm|nslookup|op|open|parted|passwd|paste|pathchk|ping|pkill|pnpm|podman|podman-compose|popd|pr|printcap|printenv|ps|pushd|pv|quota|quotacheck|quotactl|ram|rar|rcp|reboot|remsync|rename|renice|rev|rm|rmdir|rpm|rsync|scp|screen|sdiff|sed|sendmail|seq|service|sftp|sh|shellcheck|shuf|shutdown|sleep|slocate|sort|split|ssh|stat|strace|su|sudo|sum|suspend|swapon|sync|sysctl|tac|tail|tar|tee|time|timeout|top|touch|tr|traceroute|tsort|tty|umount|uname|unexpand|uniq|units|unrar|unshar|unzip|update-grub|uptime|useradd|userdel|usermod|users|uudecode|uuencode|v|vcpkg|vdir|vi|vim|virsh|vmstat|wait|watch|wc|wget|whereis|which|who|whoami|write|xargs|xdg-open|yarn|yes|zenity|zip|zsh|zypper)(?=$|[)\s;|&])/,
            lookbehind: true
        },
        keyword: {
            pattern: /(^|[\s;|&]|[<>]\()(?:case|do|done|elif|else|esac|fi|for|function|if|in|select|then|until|while)(?=$|[)\s;|&])/,
            lookbehind: true
        },
        builtin: {
            pattern: /(^|[\s;|&]|[<>]\()(?:\.|:|alias|bind|break|builtin|caller|cd|command|continue|declare|echo|enable|eval|exec|exit|export|getopts|hash|help|let|local|logout|mapfile|printf|pwd|read|readarray|readonly|return|set|shift|shopt|source|test|times|trap|type|typeset|ulimit|umask|unalias|unset)(?=$|[)\s;|&])/,
            lookbehind: true,
            alias: "class-name"
        },
        boolean: {
            pattern: /(^|[\s;|&]|[<>]\()(?:false|true)(?=$|[)\s;|&])/,
            lookbehind: true
        },
        "file-descriptor": {
            pattern: /\B&\d\b/,
            alias: "important"
        },
        operator: {
            pattern: /\d?<>|>\||\+=|=[=~]?|!=?|<<[<-]?|[&\d]?>>|\d[<>]&?|[<>][&=]?|&[>&]?|\|[&|]?/,
            inside: {
                "file-descriptor": {
                    pattern: /^\d/,
                    alias: "important"
                }
            }
        },
        punctuation: /\$?\(\(?|\)\)?|\.\.|[{}[\];\\]/,
        number: {
            pattern: /(^|\s)(?:[1-9]\d*|0)(?:[.,]\d+)?\b/,
            lookbehind: true
        }
    };
    commandAfterHeredoc.inside = Prism.languages.bash;
    var toBeCopied = [ "comment", "function-name", "for-or-select", "assign-left", "parameter", "string", "environment", "function", "keyword", "builtin", "boolean", "file-descriptor", "operator", "punctuation", "number" ];
    var inside = insideString.variable[1].inside;
    for (var i = 0; i < toBeCopied.length; i++) {
        inside[toBeCopied[i]] = Prism.languages.bash[toBeCopied[i]];
    }
    Prism.languages.sh = Prism.languages.bash;
    Prism.languages.shell = Prism.languages.bash;
})(Prism);

Prism.languages.basic = {
    comment: {
        pattern: /(?:!|REM\b).+/i,
        inside: {
            keyword: /^REM/i
        }
    },
    string: {
        pattern: /"(?:""|[!#$%&'()*,\/:;<=>?^\w +\-.])*"/,
        greedy: true
    },
    number: /(?:\b\d+(?:\.\d*)?|\B\.\d+)(?:E[+-]?\d+)?/i,
    keyword: /\b(?:AS|BEEP|BLOAD|BSAVE|CALL(?: ABSOLUTE)?|CASE|CHAIN|CHDIR|CLEAR|CLOSE|CLS|COM|COMMON|CONST|DATA|DECLARE|DEF(?: FN| SEG|DBL|INT|LNG|SNG|STR)|DIM|DO|DOUBLE|ELSE|ELSEIF|END|ENVIRON|ERASE|ERROR|EXIT|FIELD|FILES|FOR|FUNCTION|GET|GOSUB|GOTO|IF|INPUT|INTEGER|IOCTL|KEY|KILL|LINE INPUT|LOCATE|LOCK|LONG|LOOP|LSET|MKDIR|NAME|NEXT|OFF|ON(?: COM| ERROR| KEY| TIMER)?|OPEN|OPTION BASE|OUT|POKE|PUT|READ|REDIM|REM|RESTORE|RESUME|RETURN|RMDIR|RSET|RUN|SELECT CASE|SHARED|SHELL|SINGLE|SLEEP|STATIC|STEP|STOP|STRING|SUB|SWAP|SYSTEM|THEN|TIMER|TO|TROFF|TRON|TYPE|UNLOCK|UNTIL|USING|VIEW PRINT|WAIT|WEND|WHILE|WRITE)(?:\$|\b)/i,
    function: /\b(?:ABS|ACCESS|ACOS|ANGLE|AREA|ARITHMETIC|ARRAY|ASIN|ASK|AT|ATN|BASE|BEGIN|BREAK|CAUSE|CEIL|CHR|CLIP|COLLATE|COLOR|CON|COS|COSH|COT|CSC|DATE|DATUM|DEBUG|DECIMAL|DEF|DEG|DEGREES|DELETE|DET|DEVICE|DISPLAY|DOT|ELAPSED|EPS|ERASABLE|EXLINE|EXP|EXTERNAL|EXTYPE|FILETYPE|FIXED|FP|GO|GRAPH|HANDLER|IDN|IMAGE|IN|INT|INTERNAL|IP|IS|KEYED|LBOUND|LCASE|LEFT|LEN|LENGTH|LET|LINE|LINES|LOG|LOG10|LOG2|LTRIM|MARGIN|MAT|MAX|MAXNUM|MID|MIN|MISSING|MOD|NATIVE|NUL|NUMERIC|OF|OPTION|ORD|ORGANIZATION|OUTIN|OUTPUT|PI|POINT|POINTER|POINTS|POS|PRINT|PROGRAM|PROMPT|RAD|RADIANS|RANDOMIZE|RECORD|RECSIZE|RECTYPE|RELATIVE|REMAINDER|REPEAT|REST|RETRY|REWRITE|RIGHT|RND|ROUND|RTRIM|SAME|SEC|SELECT|SEQUENTIAL|SET|SETTER|SGN|SIN|SINH|SIZE|SKIP|SQR|STANDARD|STATUS|STR|STREAM|STYLE|TAB|TAN|TANH|TEMPLATE|TEXT|THERE|TIME|TIMEOUT|TRACE|TRANSFORM|TRUNCATE|UBOUND|UCASE|USE|VAL|VARIABLE|VIEWPORT|WHEN|WINDOW|WITH|ZER|ZONEWIDTH)(?:\$|\b)/i,
    operator: /<[=>]?|>=?|[+\-*\/^=&]|\b(?:AND|EQV|IMP|NOT|OR|XOR)\b/i,
    punctuation: /[,;:()]/
};

Prism.languages.c = Prism.languages.extend("clike", {
    comment: {
        pattern: /\/\/(?:[^\r\n\\]|\\(?:\r\n?|\n|(?![\r\n])))*|\/\*[\s\S]*?(?:\*\/|$)/,
        greedy: true
    },
    string: {
        pattern: /"(?:\\(?:\r\n|[\s\S])|[^"\\\r\n])*"/,
        greedy: true
    },
    "class-name": {
        pattern: /(\b(?:enum|struct)\s+(?:__attribute__\s*\(\([\s\S]*?\)\)\s*)?)\w+|\b[a-z]\w*_t\b/,
        lookbehind: true
    },
    keyword: /\b(?:_Alignas|_Alignof|_Atomic|_Bool|_Complex|_Generic|_Imaginary|_Noreturn|_Static_assert|_Thread_local|__attribute__|asm|auto|break|case|char|const|continue|default|do|double|else|enum|extern|float|for|goto|if|inline|int|long|register|return|short|signed|sizeof|static|struct|switch|typedef|typeof|union|unsigned|void|volatile|while)\b/,
    function: /\b[a-z_]\w*(?=\s*\()/i,
    number: /(?:\b0x(?:[\da-f]+(?:\.[\da-f]*)?|\.[\da-f]+)(?:p[+-]?\d+)?|(?:\b\d+(?:\.\d*)?|\B\.\d+)(?:e[+-]?\d+)?)[ful]{0,4}/i,
    operator: />>=?|<<=?|->|([-+&|:])\1|[?:~]|[-+*/%&|^!=<>]=?/
});

Prism.languages.insertBefore("c", "string", {
    char: {
        pattern: /'(?:\\(?:\r\n|[\s\S])|[^'\\\r\n]){0,32}'/,
        greedy: true
    }
});

Prism.languages.insertBefore("c", "string", {
    macro: {
        pattern: /(^[\t ]*)#\s*[a-z](?:[^\r\n\\/]|\/(?!\*)|\/\*(?:[^*]|\*(?!\/))*\*\/|\\(?:\r\n|[\s\S]))*/im,
        lookbehind: true,
        greedy: true,
        alias: "property",
        inside: {
            string: [ {
                pattern: /^(#\s*include\s*)<[^>]+>/,
                lookbehind: true
            }, Prism.languages.c["string"] ],
            char: Prism.languages.c["char"],
            comment: Prism.languages.c["comment"],
            "macro-name": [ {
                pattern: /(^#\s*define\s+)\w+\b(?!\()/i,
                lookbehind: true
            }, {
                pattern: /(^#\s*define\s+)\w+\b(?=\()/i,
                lookbehind: true,
                alias: "function"
            } ],
            directive: {
                pattern: /^(#\s*)[a-z]+/,
                lookbehind: true,
                alias: "keyword"
            },
            "directive-hash": /^#/,
            punctuation: /##|\\(?=[\r\n])/,
            expression: {
                pattern: /\S[\s\S]*/,
                inside: Prism.languages.c
            }
        }
    }
});

Prism.languages.insertBefore("c", "function", {
    constant: /\b(?:EOF|NULL|SEEK_CUR|SEEK_END|SEEK_SET|__DATE__|__FILE__|__LINE__|__TIMESTAMP__|__TIME__|__func__|stderr|stdin|stdout)\b/
});

delete Prism.languages.c["boolean"];

(function(Prism) {
    var keyword = /\b(?:alignas|alignof|asm|auto|bool|break|case|catch|char|char16_t|char32_t|char8_t|class|co_await|co_return|co_yield|compl|concept|const|const_cast|consteval|constexpr|constinit|continue|decltype|default|delete|do|double|dynamic_cast|else|enum|explicit|export|extern|final|float|for|friend|goto|if|import|inline|int|int16_t|int32_t|int64_t|int8_t|long|module|mutable|namespace|new|noexcept|nullptr|operator|override|private|protected|public|register|reinterpret_cast|requires|return|short|signed|sizeof|static|static_assert|static_cast|struct|switch|template|this|thread_local|throw|try|typedef|typeid|typename|uint16_t|uint32_t|uint64_t|uint8_t|union|unsigned|using|virtual|void|volatile|wchar_t|while)\b/;
    var modName = /\b(?!<keyword>)\w+(?:\s*\.\s*\w+)*\b/.source.replace(/<keyword>/g, function() {
        return keyword.source;
    });
    Prism.languages.cpp = Prism.languages.extend("c", {
        "class-name": [ {
            pattern: RegExp(/(\b(?:class|concept|enum|struct|typename)\s+)(?!<keyword>)\w+/.source.replace(/<keyword>/g, function() {
                return keyword.source;
            })),
            lookbehind: true
        }, /\b[A-Z]\w*(?=\s*::\s*\w+\s*\()/, /\b[A-Z_]\w*(?=\s*::\s*~\w+\s*\()/i, /\b\w+(?=\s*<(?:[^<>]|<(?:[^<>]|<[^<>]*>)*>)*>\s*::\s*\w+\s*\()/ ],
        keyword: keyword,
        number: {
            pattern: /(?:\b0b[01']+|\b0x(?:[\da-f']+(?:\.[\da-f']*)?|\.[\da-f']+)(?:p[+-]?[\d']+)?|(?:\b[\d']+(?:\.[\d']*)?|\B\.[\d']+)(?:e[+-]?[\d']+)?)[ful]{0,4}/i,
            greedy: true
        },
        operator: />>=?|<<=?|->|--|\+\+|&&|\|\||[?:~]|<=>|[-+*/%&|^!=<>]=?|\b(?:and|and_eq|bitand|bitor|not|not_eq|or|or_eq|xor|xor_eq)\b/,
        boolean: /\b(?:false|true)\b/
    });
    Prism.languages.insertBefore("cpp", "string", {
        module: {
            pattern: RegExp(/(\b(?:import|module)\s+)/.source + "(?:" + /"(?:\\(?:\r\n|[\s\S])|[^"\\\r\n])*"|<[^<>\r\n]*>/.source + "|" + /<mod-name>(?:\s*:\s*<mod-name>)?|:\s*<mod-name>/.source.replace(/<mod-name>/g, function() {
                return modName;
            }) + ")"),
            lookbehind: true,
            greedy: true,
            inside: {
                string: /^[<"][\s\S]+/,
                operator: /:/,
                punctuation: /\./
            }
        },
        "raw-string": {
            pattern: /R"([^()\\ ]{0,16})\([\s\S]*?\)\1"/,
            alias: "string",
            greedy: true
        }
    });
    Prism.languages.insertBefore("cpp", "keyword", {
        "generic-function": {
            pattern: /\b(?!operator\b)[a-z_]\w*\s*<(?:[^<>]|<[^<>]*>)*>(?=\s*\()/i,
            inside: {
                function: /^\w+/,
                generic: {
                    pattern: /<[\s\S]+/,
                    alias: "class-name",
                    inside: Prism.languages.cpp
                }
            }
        }
    });
    Prism.languages.insertBefore("cpp", "operator", {
        "double-colon": {
            pattern: /::/,
            alias: "punctuation"
        }
    });
    Prism.languages.insertBefore("cpp", "class-name", {
        "base-clause": {
            pattern: /(\b(?:class|struct)\s+\w+\s*:\s*)[^;{}"'\s]+(?:\s+[^;{}"'\s]+)*(?=\s*[;{])/,
            lookbehind: true,
            greedy: true,
            inside: Prism.languages.extend("cpp", {})
        }
    });
    Prism.languages.insertBefore("inside", "double-colon", {
        "class-name": /\b[a-z_]\w*\b(?!\s*::)/i
    }, Prism.languages.cpp["base-clause"]);
})(Prism);

(function(Prism) {
    var string = /("|')(?:\\(?:\r\n|[\s\S])|(?!\1)[^\\\r\n])*\1/;
    var selectorInside;
    Prism.languages.css.selector = {
        pattern: Prism.languages.css.selector.pattern,
        lookbehind: true,
        inside: selectorInside = {
            "pseudo-element": /:(?:after|before|first-letter|first-line|selection)|::[-\w]+/,
            "pseudo-class": /:[-\w]+/,
            class: /\.[-\w]+/,
            id: /#[-\w]+/,
            attribute: {
                pattern: RegExp("\\[(?:[^[\\]\"']|" + string.source + ")*\\]"),
                greedy: true,
                inside: {
                    punctuation: /^\[|\]$/,
                    "case-sensitivity": {
                        pattern: /(\s)[si]$/i,
                        lookbehind: true,
                        alias: "keyword"
                    },
                    namespace: {
                        pattern: /^(\s*)(?:(?!\s)[-*\w\xA0-\uFFFF])*\|(?!=)/,
                        lookbehind: true,
                        inside: {
                            punctuation: /\|$/
                        }
                    },
                    "attr-name": {
                        pattern: /^(\s*)(?:(?!\s)[-\w\xA0-\uFFFF])+/,
                        lookbehind: true
                    },
                    "attr-value": [ string, {
                        pattern: /(=\s*)(?:(?!\s)[-\w\xA0-\uFFFF])+(?=\s*$)/,
                        lookbehind: true
                    } ],
                    operator: /[|~*^$]?=/
                }
            },
            "n-th": [ {
                pattern: /(\(\s*)[+-]?\d*[\dn](?:\s*[+-]\s*\d+)?(?=\s*\))/,
                lookbehind: true,
                inside: {
                    number: /[\dn]+/,
                    operator: /[+-]/
                }
            }, {
                pattern: /(\(\s*)(?:even|odd)(?=\s*\))/i,
                lookbehind: true
            } ],
            combinator: />|\+|~|\|\|/,
            punctuation: /[(),]/
        }
    };
    Prism.languages.css["atrule"].inside["selector-function-argument"].inside = selectorInside;
    Prism.languages.insertBefore("css", "property", {
        variable: {
            pattern: /(^|[^-\w\xA0-\uFFFF])--(?!\s)[-_a-z\xA0-\uFFFF](?:(?!\s)[-\w\xA0-\uFFFF])*/i,
            lookbehind: true
        }
    });
    var unit = {
        pattern: /(\b\d+)(?:%|[a-z]+(?![\w-]))/,
        lookbehind: true
    };
    var number = {
        pattern: /(^|[^\w.-])-?(?:\d+(?:\.\d+)?|\.\d+)/,
        lookbehind: true
    };
    Prism.languages.insertBefore("css", "function", {
        operator: {
            pattern: /(\s)[+\-*\/](?=\s)/,
            lookbehind: true
        },
        hexcode: {
            pattern: /\B#[\da-f]{3,8}\b/i,
            alias: "color"
        },
        color: [ {
            pattern: /(^|[^\w-])(?:AliceBlue|AntiqueWhite|Aqua|Aquamarine|Azure|Beige|Bisque|Black|BlanchedAlmond|Blue|BlueViolet|Brown|BurlyWood|CadetBlue|Chartreuse|Chocolate|Coral|CornflowerBlue|Cornsilk|Crimson|Cyan|DarkBlue|DarkCyan|DarkGoldenRod|DarkGr[ae]y|DarkGreen|DarkKhaki|DarkMagenta|DarkOliveGreen|DarkOrange|DarkOrchid|DarkRed|DarkSalmon|DarkSeaGreen|DarkSlateBlue|DarkSlateGr[ae]y|DarkTurquoise|DarkViolet|DeepPink|DeepSkyBlue|DimGr[ae]y|DodgerBlue|FireBrick|FloralWhite|ForestGreen|Fuchsia|Gainsboro|GhostWhite|Gold|GoldenRod|Gr[ae]y|Green|GreenYellow|HoneyDew|HotPink|IndianRed|Indigo|Ivory|Khaki|Lavender|LavenderBlush|LawnGreen|LemonChiffon|LightBlue|LightCoral|LightCyan|LightGoldenRodYellow|LightGr[ae]y|LightGreen|LightPink|LightSalmon|LightSeaGreen|LightSkyBlue|LightSlateGr[ae]y|LightSteelBlue|LightYellow|Lime|LimeGreen|Linen|Magenta|Maroon|MediumAquaMarine|MediumBlue|MediumOrchid|MediumPurple|MediumSeaGreen|MediumSlateBlue|MediumSpringGreen|MediumTurquoise|MediumVioletRed|MidnightBlue|MintCream|MistyRose|Moccasin|NavajoWhite|Navy|OldLace|Olive|OliveDrab|Orange|OrangeRed|Orchid|PaleGoldenRod|PaleGreen|PaleTurquoise|PaleVioletRed|PapayaWhip|PeachPuff|Peru|Pink|Plum|PowderBlue|Purple|RebeccaPurple|Red|RosyBrown|RoyalBlue|SaddleBrown|Salmon|SandyBrown|SeaGreen|SeaShell|Sienna|Silver|SkyBlue|SlateBlue|SlateGr[ae]y|Snow|SpringGreen|SteelBlue|Tan|Teal|Thistle|Tomato|Transparent|Turquoise|Violet|Wheat|White|WhiteSmoke|Yellow|YellowGreen)(?![\w-])/i,
            lookbehind: true
        }, {
            pattern: /\b(?:hsl|rgb)\(\s*\d{1,3}\s*,\s*\d{1,3}%?\s*,\s*\d{1,3}%?\s*\)\B|\b(?:hsl|rgb)a\(\s*\d{1,3}\s*,\s*\d{1,3}%?\s*,\s*\d{1,3}%?\s*,\s*(?:0|0?\.\d+|1)\s*\)\B/i,
            inside: {
                unit: unit,
                number: number,
                function: /[\w-]+(?=\()/,
                punctuation: /[(),]/
            }
        } ],
        entity: /\\[\da-f]{1,8}/i,
        unit: unit,
        number: number
    });
})(Prism);

Prism.languages.git = {
    comment: /^#.*/m,
    deleted: /^[-–].*/m,
    inserted: /^\+.*/m,
    string: /("|')(?:\\.|(?!\1)[^\\\r\n])*\1/,
    command: {
        pattern: /^.*\$ git .*$/m,
        inside: {
            parameter: /\s--?\w+/
        }
    },
    coord: /^@@.*@@$/m,
    "commit-sha1": /^commit \w{40}$/m
};

(function(Prism) {
    var keywords = /\b(?:abstract|assert|boolean|break|byte|case|catch|char|class|const|continue|default|do|double|else|enum|exports|extends|final|finally|float|for|goto|if|implements|import|instanceof|int|interface|long|module|native|new|non-sealed|null|open|opens|package|permits|private|protected|provides|public|record(?!\s*[(){}[\]<>=%~.:,;?+\-*/&|^])|requires|return|sealed|short|static|strictfp|super|switch|synchronized|this|throw|throws|to|transient|transitive|try|uses|var|void|volatile|while|with|yield)\b/;
    var classNamePrefix = /(?:[a-z]\w*\s*\.\s*)*(?:[A-Z]\w*\s*\.\s*)*/.source;
    var className = {
        pattern: RegExp(/(^|[^\w.])/.source + classNamePrefix + /[A-Z](?:[\d_A-Z]*[a-z]\w*)?\b/.source),
        lookbehind: true,
        inside: {
            namespace: {
                pattern: /^[a-z]\w*(?:\s*\.\s*[a-z]\w*)*(?:\s*\.)?/,
                inside: {
                    punctuation: /\./
                }
            },
            punctuation: /\./
        }
    };
    Prism.languages.java = Prism.languages.extend("clike", {
        string: {
            pattern: /(^|[^\\])"(?:\\.|[^"\\\r\n])*"/,
            lookbehind: true,
            greedy: true
        },
        "class-name": [ className, {
            pattern: RegExp(/(^|[^\w.])/.source + classNamePrefix + /[A-Z]\w*(?=\s+\w+\s*[;,=()]|\s*(?:\[[\s,]*\]\s*)?::\s*new\b)/.source),
            lookbehind: true,
            inside: className.inside
        }, {
            pattern: RegExp(/(\b(?:class|enum|extends|implements|instanceof|interface|new|record|throws)\s+)/.source + classNamePrefix + /[A-Z]\w*\b/.source),
            lookbehind: true,
            inside: className.inside
        } ],
        keyword: keywords,
        function: [ Prism.languages.clike.function, {
            pattern: /(::\s*)[a-z_]\w*/,
            lookbehind: true
        } ],
        number: /\b0b[01][01_]*L?\b|\b0x(?:\.[\da-f_p+-]+|[\da-f_]+(?:\.[\da-f_p+-]+)?)\b|(?:\b\d[\d_]*(?:\.[\d_]*)?|\B\.\d[\d_]*)(?:e[+-]?\d[\d_]*)?[dfl]?/i,
        operator: {
            pattern: /(^|[^.])(?:<<=?|>>>?=?|->|--|\+\+|&&|\|\||::|[?:~]|[-+*/%&|^!=<>]=?)/m,
            lookbehind: true
        },
        constant: /\b[A-Z][A-Z_\d]+\b/
    });
    Prism.languages.insertBefore("java", "string", {
        "triple-quoted-string": {
            pattern: /"""[ \t]*[\r\n](?:(?:"|"")?(?:\\.|[^"\\]))*"""/,
            greedy: true,
            alias: "string"
        },
        char: {
            pattern: /'(?:\\.|[^'\\\r\n]){1,6}'/,
            greedy: true
        }
    });
    Prism.languages.insertBefore("java", "class-name", {
        annotation: {
            pattern: /(^|[^.])@\w+(?:\s*\.\s*\w+)*/,
            lookbehind: true,
            alias: "punctuation"
        },
        generics: {
            pattern: /<(?:[\w\s,.?]|&(?!&)|<(?:[\w\s,.?]|&(?!&)|<(?:[\w\s,.?]|&(?!&)|<(?:[\w\s,.?]|&(?!&))*>)*>)*>)*>/,
            inside: {
                "class-name": className,
                keyword: keywords,
                punctuation: /[<>(),.:]/,
                operator: /[?&|]/
            }
        },
        import: [ {
            pattern: RegExp(/(\bimport\s+)/.source + classNamePrefix + /(?:[A-Z]\w*|\*)(?=\s*;)/.source),
            lookbehind: true,
            inside: {
                namespace: className.inside.namespace,
                punctuation: /\./,
                operator: /\*/,
                "class-name": /\w+/
            }
        }, {
            pattern: RegExp(/(\bimport\s+static\s+)/.source + classNamePrefix + /(?:\w+|\*)(?=\s*;)/.source),
            lookbehind: true,
            alias: "static",
            inside: {
                namespace: className.inside.namespace,
                static: /\b\w+$/,
                punctuation: /\./,
                operator: /\*/,
                "class-name": /\w+/
            }
        } ],
        namespace: {
            pattern: RegExp(/(\b(?:exports|import(?:\s+static)?|module|open|opens|package|provides|requires|to|transitive|uses|with)\s+)(?!<keyword>)[a-z]\w*(?:\.[a-z]\w*)*\.?/.source.replace(/<keyword>/g, function() {
                return keywords.source;
            })),
            lookbehind: true,
            inside: {
                punctuation: /\./
            }
        }
    });
})(Prism);

Prism.languages.python = {
    comment: {
        pattern: /(^|[^\\])#.*/,
        lookbehind: true,
        greedy: true
    },
    "string-interpolation": {
        pattern: /(?:f|fr|rf)(?:("""|''')[\s\S]*?\1|("|')(?:\\.|(?!\2)[^\\\r\n])*\2)/i,
        greedy: true,
        inside: {
            interpolation: {
                pattern: /((?:^|[^{])(?:\{\{)*)\{(?!\{)(?:[^{}]|\{(?!\{)(?:[^{}]|\{(?!\{)(?:[^{}])+\})+\})+\}/,
                lookbehind: true,
                inside: {
                    "format-spec": {
                        pattern: /(:)[^:(){}]+(?=\}$)/,
                        lookbehind: true
                    },
                    "conversion-option": {
                        pattern: /![sra](?=[:}]$)/,
                        alias: "punctuation"
                    },
                    rest: null
                }
            },
            string: /[\s\S]+/
        }
    },
    "triple-quoted-string": {
        pattern: /(?:[rub]|br|rb)?("""|''')[\s\S]*?\1/i,
        greedy: true,
        alias: "string"
    },
    string: {
        pattern: /(?:[rub]|br|rb)?("|')(?:\\.|(?!\1)[^\\\r\n])*\1/i,
        greedy: true
    },
    function: {
        pattern: /((?:^|\s)def[ \t]+)[a-zA-Z_]\w*(?=\s*\()/g,
        lookbehind: true
    },
    "class-name": {
        pattern: /(\bclass\s+)\w+/i,
        lookbehind: true
    },
    decorator: {
        pattern: /(^[\t ]*)@\w+(?:\.\w+)*/m,
        lookbehind: true,
        alias: [ "annotation", "punctuation" ],
        inside: {
            punctuation: /\./
        }
    },
    keyword: /\b(?:_(?=\s*:)|and|as|assert|async|await|break|case|class|continue|def|del|elif|else|except|exec|finally|for|from|global|if|import|in|is|lambda|match|nonlocal|not|or|pass|print|raise|return|try|while|with|yield)\b/,
    builtin: /\b(?:__import__|abs|all|any|apply|ascii|basestring|bin|bool|buffer|bytearray|bytes|callable|chr|classmethod|cmp|coerce|compile|complex|delattr|dict|dir|divmod|enumerate|eval|execfile|file|filter|float|format|frozenset|getattr|globals|hasattr|hash|help|hex|id|input|int|intern|isinstance|issubclass|iter|len|list|locals|long|map|max|memoryview|min|next|object|oct|open|ord|pow|property|range|raw_input|reduce|reload|repr|reversed|round|set|setattr|slice|sorted|staticmethod|str|sum|super|tuple|type|unichr|unicode|vars|xrange|zip)\b/,
    boolean: /\b(?:False|None|True)\b/,
    number: /\b0(?:b(?:_?[01])+|o(?:_?[0-7])+|x(?:_?[a-f0-9])+)\b|(?:\b\d+(?:_\d+)*(?:\.(?:\d+(?:_\d+)*)?)?|\B\.\d+(?:_\d+)*)(?:e[+-]?\d+(?:_\d+)*)?j?(?!\w)/i,
    operator: /[-+%=]=?|!=|:=|\*\*?=?|\/\/?=?|<[<=>]?|>[=>]?|[&|^~]/,
    punctuation: /[{}[\];(),.:]/
};

Prism.languages.python["string-interpolation"].inside["interpolation"].inside.rest = Prism.languages.python;

Prism.languages.py = Prism.languages.python;

Prism.languages.scss = Prism.languages.extend("css", {
    comment: {
        pattern: /(^|[^\\])(?:\/\*[\s\S]*?\*\/|\/\/.*)/,
        lookbehind: true
    },
    atrule: {
        pattern: /@[\w-](?:\([^()]+\)|[^()\s]|\s+(?!\s))*?(?=\s+[{;])/,
        inside: {
            rule: /@[\w-]+/
        }
    },
    url: /(?:[-a-z]+-)?url(?=\()/i,
    selector: {
        pattern: /(?=\S)[^@;{}()]?(?:[^@;{}()\s]|\s+(?!\s)|#\{\$[-\w]+\})+(?=\s*\{(?:\}|\s|[^}][^:{}]*[:{][^}]))/,
        inside: {
            parent: {
                pattern: /&/,
                alias: "important"
            },
            placeholder: /%[-\w]+/,
            variable: /\$[-\w]+|#\{\$[-\w]+\}/
        }
    },
    property: {
        pattern: /(?:[-\w]|\$[-\w]|#\{\$[-\w]+\})+(?=\s*:)/,
        inside: {
            variable: /\$[-\w]+|#\{\$[-\w]+\}/
        }
    }
});

Prism.languages.insertBefore("scss", "atrule", {
    keyword: [ /@(?:content|debug|each|else(?: if)?|extend|for|forward|function|if|import|include|mixin|return|use|warn|while)\b/i, {
        pattern: /( )(?:from|through)(?= )/,
        lookbehind: true
    } ]
});

Prism.languages.insertBefore("scss", "important", {
    variable: /\$[-\w]+|#\{\$[-\w]+\}/
});

Prism.languages.insertBefore("scss", "function", {
    "module-modifier": {
        pattern: /\b(?:as|hide|show|with)\b/i,
        alias: "keyword"
    },
    placeholder: {
        pattern: /%[-\w]+/,
        alias: "selector"
    },
    statement: {
        pattern: /\B!(?:default|optional)\b/i,
        alias: "keyword"
    },
    boolean: /\b(?:false|true)\b/,
    null: {
        pattern: /\bnull\b/,
        alias: "keyword"
    },
    operator: {
        pattern: /(\s)(?:[-+*\/%]|[=!]=|<=?|>=?|and|not|or)(?=\s)/,
        lookbehind: true
    }
});

Prism.languages.scss["atrule"].inside.rest = Prism.languages.scss;

Prism.languages.sql = {
    comment: {
        pattern: /(^|[^\\])(?:\/\*[\s\S]*?\*\/|(?:--|\/\/|#).*)/,
        lookbehind: true
    },
    variable: [ {
        pattern: /@(["'`])(?:\\[\s\S]|(?!\1)[^\\])+\1/,
        greedy: true
    }, /@[\w.$]+/ ],
    string: {
        pattern: /(^|[^@\\])("|')(?:\\[\s\S]|(?!\2)[^\\]|\2\2)*\2/,
        greedy: true,
        lookbehind: true
    },
    identifier: {
        pattern: /(^|[^@\\])`(?:\\[\s\S]|[^`\\]|``)*`/,
        greedy: true,
        lookbehind: true,
        inside: {
            punctuation: /^`|`$/
        }
    },
    function: /\b(?:AVG|COUNT|FIRST|FORMAT|LAST|LCASE|LEN|MAX|MID|MIN|MOD|NOW|ROUND|SUM|UCASE)(?=\s*\()/i,
    keyword: /\b(?:ACTION|ADD|AFTER|ALGORITHM|ALL|ALTER|ANALYZE|ANY|APPLY|AS|ASC|AUTHORIZATION|AUTO_INCREMENT|BACKUP|BDB|BEGIN|BERKELEYDB|BIGINT|BINARY|BIT|BLOB|BOOL|BOOLEAN|BREAK|BROWSE|BTREE|BULK|BY|CALL|CASCADED?|CASE|CHAIN|CHAR(?:ACTER|SET)?|CHECK(?:POINT)?|CLOSE|CLUSTERED|COALESCE|COLLATE|COLUMNS?|COMMENT|COMMIT(?:TED)?|COMPUTE|CONNECT|CONSISTENT|CONSTRAINT|CONTAINS(?:TABLE)?|CONTINUE|CONVERT|CREATE|CROSS|CURRENT(?:_DATE|_TIME|_TIMESTAMP|_USER)?|CURSOR|CYCLE|DATA(?:BASES?)?|DATE(?:TIME)?|DAY|DBCC|DEALLOCATE|DEC|DECIMAL|DECLARE|DEFAULT|DEFINER|DELAYED|DELETE|DELIMITERS?|DENY|DESC|DESCRIBE|DETERMINISTIC|DISABLE|DISCARD|DISK|DISTINCT|DISTINCTROW|DISTRIBUTED|DO|DOUBLE|DROP|DUMMY|DUMP(?:FILE)?|DUPLICATE|ELSE(?:IF)?|ENABLE|ENCLOSED|END|ENGINE|ENUM|ERRLVL|ERRORS|ESCAPED?|EXCEPT|EXEC(?:UTE)?|EXISTS|EXIT|EXPLAIN|EXTENDED|FETCH|FIELDS|FILE|FILLFACTOR|FIRST|FIXED|FLOAT|FOLLOWING|FOR(?: EACH ROW)?|FORCE|FOREIGN|FREETEXT(?:TABLE)?|FROM|FULL|FUNCTION|GEOMETRY(?:COLLECTION)?|GLOBAL|GOTO|GRANT|GROUP|HANDLER|HASH|HAVING|HOLDLOCK|HOUR|IDENTITY(?:COL|_INSERT)?|IF|IGNORE|IMPORT|INDEX|INFILE|INNER|INNODB|INOUT|INSERT|INT|INTEGER|INTERSECT|INTERVAL|INTO|INVOKER|ISOLATION|ITERATE|JOIN|KEYS?|KILL|LANGUAGE|LAST|LEAVE|LEFT|LEVEL|LIMIT|LINENO|LINES|LINESTRING|LOAD|LOCAL|LOCK|LONG(?:BLOB|TEXT)|LOOP|MATCH(?:ED)?|MEDIUM(?:BLOB|INT|TEXT)|MERGE|MIDDLEINT|MINUTE|MODE|MODIFIES|MODIFY|MONTH|MULTI(?:LINESTRING|POINT|POLYGON)|NATIONAL|NATURAL|NCHAR|NEXT|NO|NONCLUSTERED|NULLIF|NUMERIC|OFF?|OFFSETS?|ON|OPEN(?:DATASOURCE|QUERY|ROWSET)?|OPTIMIZE|OPTION(?:ALLY)?|ORDER|OUT(?:ER|FILE)?|OVER|PARTIAL|PARTITION|PERCENT|PIVOT|PLAN|POINT|POLYGON|PRECEDING|PRECISION|PREPARE|PREV|PRIMARY|PRINT|PRIVILEGES|PROC(?:EDURE)?|PUBLIC|PURGE|QUICK|RAISERROR|READS?|REAL|RECONFIGURE|REFERENCES|RELEASE|RENAME|REPEAT(?:ABLE)?|REPLACE|REPLICATION|REQUIRE|RESIGNAL|RESTORE|RESTRICT|RETURN(?:ING|S)?|REVOKE|RIGHT|ROLLBACK|ROUTINE|ROW(?:COUNT|GUIDCOL|S)?|RTREE|RULE|SAVE(?:POINT)?|SCHEMA|SECOND|SELECT|SERIAL(?:IZABLE)?|SESSION(?:_USER)?|SET(?:USER)?|SHARE|SHOW|SHUTDOWN|SIMPLE|SMALLINT|SNAPSHOT|SOME|SONAME|SQL|START(?:ING)?|STATISTICS|STATUS|STRIPED|SYSTEM_USER|TABLES?|TABLESPACE|TEMP(?:ORARY|TABLE)?|TERMINATED|TEXT(?:SIZE)?|THEN|TIME(?:STAMP)?|TINY(?:BLOB|INT|TEXT)|TOP?|TRAN(?:SACTIONS?)?|TRIGGER|TRUNCATE|TSEQUAL|TYPES?|UNBOUNDED|UNCOMMITTED|UNDEFINED|UNION|UNIQUE|UNLOCK|UNPIVOT|UNSIGNED|UPDATE(?:TEXT)?|USAGE|USE|USER|USING|VALUES?|VAR(?:BINARY|CHAR|CHARACTER|YING)|VIEW|WAITFOR|WARNINGS|WHEN|WHERE|WHILE|WITH(?: ROLLUP|IN)?|WORK|WRITE(?:TEXT)?|YEAR)\b/i,
    boolean: /\b(?:FALSE|NULL|TRUE)\b/i,
    number: /\b0x[\da-f]+\b|\b\d+(?:\.\d*)?|\B\.\d+\b/i,
    operator: /[-+*\/=%^~]|&&?|\|\|?|!=?|<(?:=>?|<|>)?|>[>=]?|\b(?:AND|BETWEEN|DIV|ILIKE|IN|IS|LIKE|NOT|OR|REGEXP|RLIKE|SOUNDS LIKE|XOR)\b/i,
    punctuation: /[;[\]()`,.]/
};

Prism.languages.vbnet = Prism.languages.extend("basic", {
    comment: [ {
        pattern: /(?:!|REM\b).+/i,
        inside: {
            keyword: /^REM/i
        }
    }, {
        pattern: /(^|[^\\:])'.*/,
        lookbehind: true,
        greedy: true
    } ],
    string: {
        pattern: /(^|[^"])"(?:""|[^"])*"(?!")/,
        lookbehind: true,
        greedy: true
    },
    keyword: /(?:\b(?:ADDHANDLER|ADDRESSOF|ALIAS|AND|ANDALSO|AS|BEEP|BLOAD|BOOLEAN|BSAVE|BYREF|BYTE|BYVAL|CALL(?: ABSOLUTE)?|CASE|CATCH|CBOOL|CBYTE|CCHAR|CDATE|CDBL|CDEC|CHAIN|CHAR|CHDIR|CINT|CLASS|CLEAR|CLNG|CLOSE|CLS|COBJ|COM|COMMON|CONST|CONTINUE|CSBYTE|CSHORT|CSNG|CSTR|CTYPE|CUINT|CULNG|CUSHORT|DATA|DATE|DECIMAL|DECLARE|DEF(?: FN| SEG|DBL|INT|LNG|SNG|STR)|DEFAULT|DELEGATE|DIM|DIRECTCAST|DO|DOUBLE|ELSE|ELSEIF|END|ENUM|ENVIRON|ERASE|ERROR|EVENT|EXIT|FALSE|FIELD|FILES|FINALLY|FOR(?: EACH)?|FRIEND|FUNCTION|GET|GETTYPE|GETXMLNAMESPACE|GLOBAL|GOSUB|GOTO|HANDLES|IF|IMPLEMENTS|IMPORTS|IN|INHERITS|INPUT|INTEGER|INTERFACE|IOCTL|IS|ISNOT|KEY|KILL|LET|LIB|LIKE|LINE INPUT|LOCATE|LOCK|LONG|LOOP|LSET|ME|MKDIR|MOD|MODULE|MUSTINHERIT|MUSTOVERRIDE|MYBASE|MYCLASS|NAME|NAMESPACE|NARROWING|NEW|NEXT|NOT|NOTHING|NOTINHERITABLE|NOTOVERRIDABLE|OBJECT|OF|OFF|ON(?: COM| ERROR| KEY| TIMER)?|OPEN|OPERATOR|OPTION(?: BASE)?|OPTIONAL|OR|ORELSE|OUT|OVERLOADS|OVERRIDABLE|OVERRIDES|PARAMARRAY|PARTIAL|POKE|PRIVATE|PROPERTY|PROTECTED|PUBLIC|PUT|RAISEEVENT|READ|READONLY|REDIM|REM|REMOVEHANDLER|RESTORE|RESUME|RETURN|RMDIR|RSET|RUN|SBYTE|SELECT(?: CASE)?|SET|SHADOWS|SHARED|SHELL|SHORT|SINGLE|SLEEP|STATIC|STEP|STOP|STRING|STRUCTURE|SUB|SWAP|SYNCLOCK|SYSTEM|THEN|THROW|TIMER|TO|TROFF|TRON|TRUE|TRY|TRYCAST|TYPE|TYPEOF|UINTEGER|ULONG|UNLOCK|UNTIL|USHORT|USING|VIEW PRINT|WAIT|WEND|WHEN|WHILE|WIDENING|WITH|WITHEVENTS|WRITE|WRITEONLY|XOR)|\B(?:#CONST|#ELSE|#ELSEIF|#END|#IF))(?:\$|\b)/i,
    punctuation: /[,;:(){}]/
});

Prism.languages["visual-basic"] = {
    comment: {
        pattern: /(?:['‘’]|REM\b)(?:[^\r\n_]|_(?:\r\n?|\n)?)*/i,
        inside: {
            keyword: /^REM/i
        }
    },
    directive: {
        pattern: /#(?:Const|Else|ElseIf|End|ExternalChecksum|ExternalSource|If|Region)(?:\b_[ \t]*(?:\r\n?|\n)|.)+/i,
        alias: "property",
        greedy: true
    },
    string: {
        pattern: /\$?["“”](?:["“”]{2}|[^"“”])*["“”]C?/i,
        greedy: true
    },
    date: {
        pattern: /#[ \t]*(?:\d+([/-])\d+\1\d+(?:[ \t]+(?:\d+[ \t]*(?:AM|PM)|\d+:\d+(?::\d+)?(?:[ \t]*(?:AM|PM))?))?|\d+[ \t]*(?:AM|PM)|\d+:\d+(?::\d+)?(?:[ \t]*(?:AM|PM))?)[ \t]*#/i,
        alias: "number"
    },
    number: /(?:(?:\b\d+(?:\.\d+)?|\.\d+)(?:E[+-]?\d+)?|&[HO][\dA-F]+)(?:[FRD]|U?[ILS])?/i,
    boolean: /\b(?:False|Nothing|True)\b/i,
    keyword: /\b(?:AddHandler|AddressOf|Alias|And(?:Also)?|As|Boolean|ByRef|Byte|ByVal|Call|Case|Catch|C(?:Bool|Byte|Char|Date|Dbl|Dec|Int|Lng|Obj|SByte|Short|Sng|Str|Type|UInt|ULng|UShort)|Char|Class|Const|Continue|Currency|Date|Decimal|Declare|Default|Delegate|Dim|DirectCast|Do|Double|Each|Else(?:If)?|End(?:If)?|Enum|Erase|Error|Event|Exit|Finally|For|Friend|Function|Get(?:Type|XMLNamespace)?|Global|GoSub|GoTo|Handles|If|Implements|Imports|In|Inherits|Integer|Interface|Is|IsNot|Let|Lib|Like|Long|Loop|Me|Mod|Module|Must(?:Inherit|Override)|My(?:Base|Class)|Namespace|Narrowing|New|Next|Not(?:Inheritable|Overridable)?|Object|Of|On|Operator|Option(?:al)?|Or(?:Else)?|Out|Overloads|Overridable|Overrides|ParamArray|Partial|Private|Property|Protected|Public|RaiseEvent|ReadOnly|ReDim|RemoveHandler|Resume|Return|SByte|Select|Set|Shadows|Shared|short|Single|Static|Step|Stop|String|Structure|Sub|SyncLock|Then|Throw|To|Try|TryCast|Type|TypeOf|U(?:Integer|Long|Short)|Until|Using|Variant|Wend|When|While|Widening|With(?:Events)?|WriteOnly|Xor)\b/i,
    operator: /[+\-*/\\^<=>&#@$%!]|\b_(?=[ \t]*[\r\n])/,
    punctuation: /[{}().,:?]/
};

Prism.languages.vb = Prism.languages["visual-basic"];

Prism.languages.vba = Prism.languages["visual-basic"];

(function() {
    if (typeof Prism === "undefined" || typeof document === "undefined" || !document.querySelector) {
        return;
    }
    var LINE_NUMBERS_CLASS = "line-numbers";
    var LINKABLE_LINE_NUMBERS_CLASS = "linkable-line-numbers";
    var NEW_LINE_EXP = /\n(?!$)/g;
    function $$(selector, container) {
        return Array.prototype.slice.call((container || document).querySelectorAll(selector));
    }
    function hasClass(element, className) {
        return element.classList.contains(className);
    }
    function callFunction(func) {
        func();
    }
    var isLineHeightRounded = function() {
        var res;
        return function() {
            if (typeof res === "undefined") {
                var d = document.createElement("div");
                d.style.fontSize = "13px";
                d.style.lineHeight = "1.5";
                d.style.padding = "0";
                d.style.border = "0";
                d.innerHTML = "&nbsp;<br />&nbsp;";
                document.body.appendChild(d);
                res = d.offsetHeight === 38;
                document.body.removeChild(d);
            }
            return res;
        };
    }();
    function getContentBoxTopOffset(parent, child) {
        var parentStyle = getComputedStyle(parent);
        var childStyle = getComputedStyle(child);
        function pxToNumber(px) {
            return +px.substr(0, px.length - 2);
        }
        return child.offsetTop + pxToNumber(childStyle.borderTopWidth) + pxToNumber(childStyle.paddingTop) - pxToNumber(parentStyle.paddingTop);
    }
    function isActiveFor(pre) {
        if (!pre || !/pre/i.test(pre.nodeName)) {
            return false;
        }
        if (pre.hasAttribute("data-line")) {
            return true;
        }
        if (pre.id && Prism.util.isActive(pre, LINKABLE_LINE_NUMBERS_CLASS)) {
            return true;
        }
        return false;
    }
    var scrollIntoView = true;
    Prism.plugins.lineHighlight = {
        highlightLines: function highlightLines(pre, lines, classes) {
            lines = typeof lines === "string" ? lines : pre.getAttribute("data-line") || "";
            var ranges = lines.replace(/\s+/g, "").split(",").filter(Boolean);
            var offset = +pre.getAttribute("data-line-offset") || 0;
            var parseMethod = isLineHeightRounded() ? parseInt : parseFloat;
            var lineHeight = parseMethod(getComputedStyle(pre).lineHeight);
            var hasLineNumbers = Prism.util.isActive(pre, LINE_NUMBERS_CLASS);
            var codeElement = pre.querySelector("code");
            var parentElement = hasLineNumbers ? pre : codeElement || pre;
            var mutateActions = [];
            var lineBreakMatch = codeElement.textContent.match(NEW_LINE_EXP);
            var numberOfLines = lineBreakMatch ? lineBreakMatch.length + 1 : 1;
            var codePreOffset = !codeElement || parentElement == codeElement ? 0 : getContentBoxTopOffset(pre, codeElement);
            ranges.forEach(function(currentRange) {
                var range = currentRange.split("-");
                var start = +range[0];
                var end = +range[1] || start;
                end = Math.min(numberOfLines + offset, end);
                if (end < start) {
                    return;
                }
                var line = pre.querySelector('.line-highlight[data-range="' + currentRange + '"]') || document.createElement("div");
                mutateActions.push(function() {
                    line.setAttribute("aria-hidden", "true");
                    line.setAttribute("data-range", currentRange);
                    line.className = (classes || "") + " line-highlight";
                });
                if (hasLineNumbers && Prism.plugins.lineNumbers) {
                    var startNode = Prism.plugins.lineNumbers.getLine(pre, start);
                    var endNode = Prism.plugins.lineNumbers.getLine(pre, end);
                    if (startNode) {
                        var top = startNode.offsetTop + codePreOffset + "px";
                        mutateActions.push(function() {
                            line.style.top = top;
                        });
                    }
                    if (endNode) {
                        var height = endNode.offsetTop - startNode.offsetTop + endNode.offsetHeight + "px";
                        mutateActions.push(function() {
                            line.style.height = height;
                        });
                    }
                } else {
                    mutateActions.push(function() {
                        line.setAttribute("data-start", String(start));
                        if (end > start) {
                            line.setAttribute("data-end", String(end));
                        }
                        line.style.top = (start - offset - 1) * lineHeight + codePreOffset + "px";
                        line.textContent = new Array(end - start + 2).join(" \n");
                    });
                }
                mutateActions.push(function() {
                    line.style.width = pre.scrollWidth + "px";
                });
                mutateActions.push(function() {
                    parentElement.appendChild(line);
                });
            });
            var id = pre.id;
            if (hasLineNumbers && Prism.util.isActive(pre, LINKABLE_LINE_NUMBERS_CLASS) && id) {
                if (!hasClass(pre, LINKABLE_LINE_NUMBERS_CLASS)) {
                    mutateActions.push(function() {
                        pre.classList.add(LINKABLE_LINE_NUMBERS_CLASS);
                    });
                }
                var start = parseInt(pre.getAttribute("data-start") || "1");
                $$(".line-numbers-rows > span", pre).forEach(function(lineSpan, i) {
                    var lineNumber = i + start;
                    lineSpan.onclick = function() {
                        var hash = id + "." + lineNumber;
                        scrollIntoView = false;
                        location.hash = hash;
                        setTimeout(function() {
                            scrollIntoView = true;
                        }, 1);
                    };
                });
            }
            return function() {
                mutateActions.forEach(callFunction);
            };
        }
    };
    function applyHash() {
        var hash = location.hash.slice(1);
        $$(".temporary.line-highlight").forEach(function(line) {
            line.parentNode.removeChild(line);
        });
        var range = (hash.match(/\.([\d,-]+)$/) || [ , "" ])[1];
        if (!range || document.getElementById(hash)) {
            return;
        }
        var id = hash.slice(0, hash.lastIndexOf("."));
        var pre = document.getElementById(id);
        if (!pre) {
            return;
        }
        if (!pre.hasAttribute("data-line")) {
            pre.setAttribute("data-line", "");
        }
        var mutateDom = Prism.plugins.lineHighlight.highlightLines(pre, range, "temporary ");
        mutateDom();
        if (scrollIntoView) {
            document.querySelector(".temporary.line-highlight").scrollIntoView();
        }
    }
    var fakeTimer = 0;
    Prism.hooks.add("before-sanity-check", function(env) {
        var pre = env.element.parentElement;
        if (!isActiveFor(pre)) {
            return;
        }
        var num = 0;
        $$(".line-highlight", pre).forEach(function(line) {
            num += line.textContent.length;
            line.parentNode.removeChild(line);
        });
        if (num && /^(?: \n)+$/.test(env.code.slice(-num))) {
            env.code = env.code.slice(0, -num);
        }
    });
    Prism.hooks.add("complete", function completeHook(env) {
        var pre = env.element.parentElement;
        if (!isActiveFor(pre)) {
            return;
        }
        clearTimeout(fakeTimer);
        var hasLineNumbers = Prism.plugins.lineNumbers;
        var isLineNumbersLoaded = env.plugins && env.plugins.lineNumbers;
        if (hasClass(pre, LINE_NUMBERS_CLASS) && hasLineNumbers && !isLineNumbersLoaded) {
            Prism.hooks.add("line-numbers", completeHook);
        } else {
            var mutateDom = Prism.plugins.lineHighlight.highlightLines(pre);
            mutateDom();
            fakeTimer = setTimeout(applyHash, 1);
        }
    });
    window.addEventListener("hashchange", applyHash);
    window.addEventListener("resize", function() {
        var actions = $$("pre").filter(isActiveFor).map(function(pre) {
            return Prism.plugins.lineHighlight.highlightLines(pre);
        });
        actions.forEach(callFunction);
    });
})();

(function() {
    if (typeof Prism === "undefined" || typeof document === "undefined") {
        return;
    }
    var PLUGIN_NAME = "line-numbers";
    var NEW_LINE_EXP = /\n(?!$)/g;
    var config = Prism.plugins.lineNumbers = {
        getLine: function(element, number) {
            if (element.tagName !== "PRE" || !element.classList.contains(PLUGIN_NAME)) {
                return;
            }
            var lineNumberRows = element.querySelector(".line-numbers-rows");
            if (!lineNumberRows) {
                return;
            }
            var lineNumberStart = parseInt(element.getAttribute("data-start"), 10) || 1;
            var lineNumberEnd = lineNumberStart + (lineNumberRows.children.length - 1);
            if (number < lineNumberStart) {
                number = lineNumberStart;
            }
            if (number > lineNumberEnd) {
                number = lineNumberEnd;
            }
            var lineIndex = number - lineNumberStart;
            return lineNumberRows.children[lineIndex];
        },
        resize: function(element) {
            resizeElements([ element ]);
        },
        assumeViewportIndependence: true
    };
    function resizeElements(elements) {
        elements = elements.filter(function(e) {
            var codeStyles = getStyles(e);
            var whiteSpace = codeStyles["white-space"];
            return whiteSpace === "pre-wrap" || whiteSpace === "pre-line";
        });
        if (elements.length == 0) {
            return;
        }
        var infos = elements.map(function(element) {
            var codeElement = element.querySelector("code");
            var lineNumbersWrapper = element.querySelector(".line-numbers-rows");
            if (!codeElement || !lineNumbersWrapper) {
                return undefined;
            }
            var lineNumberSizer = element.querySelector(".line-numbers-sizer");
            var codeLines = codeElement.textContent.split(NEW_LINE_EXP);
            if (!lineNumberSizer) {
                lineNumberSizer = document.createElement("span");
                lineNumberSizer.className = "line-numbers-sizer";
                codeElement.appendChild(lineNumberSizer);
            }
            lineNumberSizer.innerHTML = "0";
            lineNumberSizer.style.display = "block";
            var oneLinerHeight = lineNumberSizer.getBoundingClientRect().height;
            lineNumberSizer.innerHTML = "";
            return {
                element: element,
                lines: codeLines,
                lineHeights: [],
                oneLinerHeight: oneLinerHeight,
                sizer: lineNumberSizer
            };
        }).filter(Boolean);
        infos.forEach(function(info) {
            var lineNumberSizer = info.sizer;
            var lines = info.lines;
            var lineHeights = info.lineHeights;
            var oneLinerHeight = info.oneLinerHeight;
            lineHeights[lines.length - 1] = undefined;
            lines.forEach(function(line, index) {
                if (line && line.length > 1) {
                    var e = lineNumberSizer.appendChild(document.createElement("span"));
                    e.style.display = "block";
                    e.textContent = line;
                } else {
                    lineHeights[index] = oneLinerHeight;
                }
            });
        });
        infos.forEach(function(info) {
            var lineNumberSizer = info.sizer;
            var lineHeights = info.lineHeights;
            var childIndex = 0;
            for (var i = 0; i < lineHeights.length; i++) {
                if (lineHeights[i] === undefined) {
                    lineHeights[i] = lineNumberSizer.children[childIndex++].getBoundingClientRect().height;
                }
            }
        });
        infos.forEach(function(info) {
            var lineNumberSizer = info.sizer;
            var wrapper = info.element.querySelector(".line-numbers-rows");
            lineNumberSizer.style.display = "none";
            lineNumberSizer.innerHTML = "";
            info.lineHeights.forEach(function(height, lineNumber) {
                wrapper.children[lineNumber].style.height = height + "px";
            });
        });
    }
    function getStyles(element) {
        if (!element) {
            return null;
        }
        return window.getComputedStyle ? getComputedStyle(element) : element.currentStyle || null;
    }
    var lastWidth = undefined;
    window.addEventListener("resize", function() {
        if (config.assumeViewportIndependence && lastWidth === window.innerWidth) {
            return;
        }
        lastWidth = window.innerWidth;
        resizeElements(Array.prototype.slice.call(document.querySelectorAll("pre." + PLUGIN_NAME)));
    });
    Prism.hooks.add("complete", function(env) {
        if (!env.code) {
            return;
        }
        var code = env.element;
        var pre = code.parentNode;
        if (!pre || !/pre/i.test(pre.nodeName)) {
            return;
        }
        if (code.querySelector(".line-numbers-rows")) {
            return;
        }
        if (!Prism.util.isActive(code, PLUGIN_NAME)) {
            return;
        }
        code.classList.remove(PLUGIN_NAME);
        pre.classList.add(PLUGIN_NAME);
        var match = env.code.match(NEW_LINE_EXP);
        var linesNum = match ? match.length + 1 : 1;
        var lineNumbersWrapper;
        var lines = new Array(linesNum + 1).join("<span></span>");
        lineNumbersWrapper = document.createElement("span");
        lineNumbersWrapper.setAttribute("aria-hidden", "true");
        lineNumbersWrapper.className = "line-numbers-rows";
        lineNumbersWrapper.innerHTML = lines;
        if (pre.hasAttribute("data-start")) {
            pre.style.counterReset = "linenumber " + (parseInt(pre.getAttribute("data-start"), 10) - 1);
        }
        env.element.appendChild(lineNumbersWrapper);
        resizeElements([ pre ]);
        Prism.hooks.run("line-numbers", env);
    });
    Prism.hooks.add("line-numbers", function(env) {
        env.plugins = env.plugins || {};
        env.plugins.lineNumbers = true;
    });
})();

(function() {
    if (typeof Prism === "undefined") {
        return;
    }
    var url = /\b([a-z]{3,7}:\/\/|tel:)[\w\-+%~/.:=&!$'()*,;@]+(?:\?[\w\-+%~/.:=?&!$'()*,;@]*)?(?:#[\w\-+%~/.:#=?&!$'()*,;@]*)?/;
    var email = /\b\S+@[\w.]+[a-z]{2}/;
    var linkMd = /\[([^\]]+)\]\(([^)]+)\)/;
    var candidates = [ "comment", "url", "attr-value", "string" ];
    Prism.plugins.autolinker = {
        processGrammar: function(grammar) {
            if (!grammar || grammar["url-link"]) {
                return;
            }
            Prism.languages.DFS(grammar, function(key, def, type) {
                if (candidates.indexOf(type) > -1 && !Array.isArray(def)) {
                    if (!def.pattern) {
                        def = this[key] = {
                            pattern: def
                        };
                    }
                    def.inside = def.inside || {};
                    if (type == "comment") {
                        def.inside["md-link"] = linkMd;
                    }
                    if (type == "attr-value") {
                        Prism.languages.insertBefore("inside", "punctuation", {
                            "url-link": url
                        }, def);
                    } else {
                        def.inside["url-link"] = url;
                    }
                    def.inside["email-link"] = email;
                }
            });
            grammar["url-link"] = url;
            grammar["email-link"] = email;
        }
    };
    Prism.hooks.add("before-highlight", function(env) {
        Prism.plugins.autolinker.processGrammar(env.grammar);
    });
    Prism.hooks.add("wrap", function(env) {
        if (/-link$/.test(env.type)) {
            env.tag = "a";
            var href = env.content;
            if (env.type == "email-link" && href.indexOf("mailto:") != 0) {
                href = "mailto:" + href;
            } else if (env.type == "md-link") {
                var match = env.content.match(linkMd);
                href = match[2];
                env.content = match[1];
            }
            env.attributes.href = href;
            try {
                env.content = decodeURIComponent(env.content);
            } catch (e) {}
        }
    });
})();

(function() {
    if (typeof Prism === "undefined") {
        return;
    }
    var assign = Object.assign || function(obj1, obj2) {
        for (var name in obj2) {
            if (obj2.hasOwnProperty(name)) {
                obj1[name] = obj2[name];
            }
        }
        return obj1;
    };
    function NormalizeWhitespace(defaults) {
        this.defaults = assign({}, defaults);
    }
    function toCamelCase(value) {
        return value.replace(/-(\w)/g, function(match, firstChar) {
            return firstChar.toUpperCase();
        });
    }
    function tabLen(str) {
        var res = 0;
        for (var i = 0; i < str.length; ++i) {
            if (str.charCodeAt(i) == "\t".charCodeAt(0)) {
                res += 3;
            }
        }
        return str.length + res;
    }
    var settingsConfig = {
        "remove-trailing": "boolean",
        "remove-indent": "boolean",
        "left-trim": "boolean",
        "right-trim": "boolean",
        "break-lines": "number",
        indent: "number",
        "remove-initial-line-feed": "boolean",
        "tabs-to-spaces": "number",
        "spaces-to-tabs": "number"
    };
    NormalizeWhitespace.prototype = {
        setDefaults: function(defaults) {
            this.defaults = assign(this.defaults, defaults);
        },
        normalize: function(input, settings) {
            settings = assign(this.defaults, settings);
            for (var name in settings) {
                var methodName = toCamelCase(name);
                if (name !== "normalize" && methodName !== "setDefaults" && settings[name] && this[methodName]) {
                    input = this[methodName].call(this, input, settings[name]);
                }
            }
            return input;
        },
        leftTrim: function(input) {
            return input.replace(/^\s+/, "");
        },
        rightTrim: function(input) {
            return input.replace(/\s+$/, "");
        },
        tabsToSpaces: function(input, spaces) {
            spaces = spaces | 0 || 4;
            return input.replace(/\t/g, new Array(++spaces).join(" "));
        },
        spacesToTabs: function(input, spaces) {
            spaces = spaces | 0 || 4;
            return input.replace(RegExp(" {" + spaces + "}", "g"), "\t");
        },
        removeTrailing: function(input) {
            return input.replace(/\s*?$/gm, "");
        },
        removeInitialLineFeed: function(input) {
            return input.replace(/^(?:\r?\n|\r)/, "");
        },
        removeIndent: function(input) {
            var indents = input.match(/^[^\S\n\r]*(?=\S)/gm);
            if (!indents || !indents[0].length) {
                return input;
            }
            indents.sort(function(a, b) {
                return a.length - b.length;
            });
            if (!indents[0].length) {
                return input;
            }
            return input.replace(RegExp("^" + indents[0], "gm"), "");
        },
        indent: function(input, tabs) {
            return input.replace(/^[^\S\n\r]*(?=\S)/gm, new Array(++tabs).join("\t") + "$&");
        },
        breakLines: function(input, characters) {
            characters = characters === true ? 80 : characters | 0 || 80;
            var lines = input.split("\n");
            for (var i = 0; i < lines.length; ++i) {
                if (tabLen(lines[i]) <= characters) {
                    continue;
                }
                var line = lines[i].split(/(\s+)/g);
                var len = 0;
                for (var j = 0; j < line.length; ++j) {
                    var tl = tabLen(line[j]);
                    len += tl;
                    if (len > characters) {
                        line[j] = "\n" + line[j];
                        len = tl;
                    }
                }
                lines[i] = line.join("");
            }
            return lines.join("\n");
        }
    };
    if (typeof module !== "undefined" && module.exports) {
        module.exports = NormalizeWhitespace;
    }
    Prism.plugins.NormalizeWhitespace = new NormalizeWhitespace({
        "remove-trailing": true,
        "remove-indent": true,
        "left-trim": true,
        "right-trim": true
    });
    Prism.hooks.add("before-sanity-check", function(env) {
        var Normalizer = Prism.plugins.NormalizeWhitespace;
        if (env.settings && env.settings["whitespace-normalization"] === false) {
            return;
        }
        if (!Prism.util.isActive(env.element, "whitespace-normalization", true)) {
            return;
        }
        if ((!env.element || !env.element.parentNode) && env.code) {
            env.code = Normalizer.normalize(env.code, env.settings);
            return;
        }
        var pre = env.element.parentNode;
        if (!env.code || !pre || pre.nodeName.toLowerCase() !== "pre") {
            return;
        }
        if (env.settings == null) {
            env.settings = {};
        }
        for (var key in settingsConfig) {
            if (Object.hasOwnProperty.call(settingsConfig, key)) {
                var settingType = settingsConfig[key];
                if (pre.hasAttribute("data-" + key)) {
                    try {
                        var value = JSON.parse(pre.getAttribute("data-" + key) || "true");
                        if (typeof value === settingType) {
                            env.settings[key] = value;
                        }
                    } catch (_error) {}
                }
            }
        }
        var children = pre.childNodes;
        var before = "";
        var after = "";
        var codeFound = false;
        for (var i = 0; i < children.length; ++i) {
            var node = children[i];
            if (node == env.element) {
                codeFound = true;
            } else if (node.nodeName === "#text") {
                if (codeFound) {
                    after += node.nodeValue;
                } else {
                    before += node.nodeValue;
                }
                pre.removeChild(node);
                --i;
            }
        }
        if (!env.element.children.length || !Prism.plugins.KeepMarkup) {
            env.code = before + env.code + after;
            env.code = Normalizer.normalize(env.code, env.settings);
        } else {
            var html = before + env.element.innerHTML + after;
            env.element.innerHTML = Normalizer.normalize(html, env.settings);
            env.code = env.element.textContent;
        }
    });
})();

(function() {
    if (typeof Prism === "undefined" || typeof document === "undefined") {
        return;
    }
    var callbacks = [];
    var map = {};
    var noop = function() {};
    Prism.plugins.toolbar = {};
    var registerButton = Prism.plugins.toolbar.registerButton = function(key, opts) {
        var callback;
        if (typeof opts === "function") {
            callback = opts;
        } else {
            callback = function(env) {
                var element;
                if (typeof opts.onClick === "function") {
                    element = document.createElement("button");
                    element.type = "button";
                    element.addEventListener("click", function() {
                        opts.onClick.call(this, env);
                    });
                } else if (typeof opts.url === "string") {
                    element = document.createElement("a");
                    element.href = opts.url;
                } else {
                    element = document.createElement("span");
                }
                if (opts.className) {
                    element.classList.add(opts.className);
                }
                element.textContent = opts.text;
                return element;
            };
        }
        if (key in map) {
            console.warn('There is a button with the key "' + key + '" registered already.');
            return;
        }
        callbacks.push(map[key] = callback);
    };
    function getOrder(element) {
        while (element) {
            var order = element.getAttribute("data-toolbar-order");
            if (order != null) {
                order = order.trim();
                if (order.length) {
                    return order.split(/\s*,\s*/g);
                } else {
                    return [];
                }
            }
            element = element.parentElement;
        }
    }
    var hook = Prism.plugins.toolbar.hook = function(env) {
        var pre = env.element.parentNode;
        if (!pre || !/pre/i.test(pre.nodeName)) {
            return;
        }
        if (pre.parentNode.classList.contains("code-toolbar")) {
            return;
        }
        var wrapper = document.createElement("div");
        wrapper.classList.add("code-toolbar");
        pre.parentNode.insertBefore(wrapper, pre);
        wrapper.appendChild(pre);
        var toolbar = document.createElement("div");
        toolbar.classList.add("toolbar");
        var elementCallbacks = callbacks;
        var order = getOrder(env.element);
        if (order) {
            elementCallbacks = order.map(function(key) {
                return map[key] || noop;
            });
        }
        elementCallbacks.forEach(function(callback) {
            var element = callback(env);
            if (!element) {
                return;
            }
            var item = document.createElement("div");
            item.classList.add("toolbar-item");
            item.appendChild(element);
            toolbar.appendChild(item);
        });
        wrapper.appendChild(toolbar);
    };
    registerButton("label", function(env) {
        var pre = env.element.parentNode;
        if (!pre || !/pre/i.test(pre.nodeName)) {
            return;
        }
        if (!pre.hasAttribute("data-label")) {
            return;
        }
        var element;
        var template;
        var text = pre.getAttribute("data-label");
        try {
            template = document.querySelector("template#" + text);
        } catch (e) {}
        if (template) {
            element = template.content;
        } else {
            if (pre.hasAttribute("data-url")) {
                element = document.createElement("a");
                element.href = pre.getAttribute("data-url");
            } else {
                element = document.createElement("span");
            }
            element.textContent = text;
        }
        return element;
    });
    Prism.hooks.add("complete", hook);
})();

(function() {
    if (typeof Prism === "undefined" || typeof document === "undefined") {
        return;
    }
    if (!Prism.plugins.toolbar) {
        console.warn("Copy to Clipboard plugin loaded before Toolbar plugin.");
        return;
    }
    function registerClipboard(element, copyInfo) {
        element.addEventListener("click", function() {
            copyTextToClipboard(copyInfo);
        });
    }
    function fallbackCopyTextToClipboard(copyInfo) {
        var textArea = document.createElement("textarea");
        textArea.value = copyInfo.getText();
        textArea.style.top = "0";
        textArea.style.left = "0";
        textArea.style.position = "fixed";
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        try {
            var successful = document.execCommand("copy");
            setTimeout(function() {
                if (successful) {
                    copyInfo.success();
                } else {
                    copyInfo.error();
                }
            }, 1);
        } catch (err) {
            setTimeout(function() {
                copyInfo.error(err);
            }, 1);
        }
        document.body.removeChild(textArea);
    }
    function copyTextToClipboard(copyInfo) {
        if (navigator.clipboard) {
            navigator.clipboard.writeText(copyInfo.getText()).then(copyInfo.success, function() {
                fallbackCopyTextToClipboard(copyInfo);
            });
        } else {
            fallbackCopyTextToClipboard(copyInfo);
        }
    }
    function selectElementText(element) {
        window.getSelection().selectAllChildren(element);
    }
    function getSettings(startElement) {
        var settings = {
            copy: "Copy",
            "copy-error": "Press Ctrl+C to copy",
            "copy-success": "Copied!",
            "copy-timeout": 5e3
        };
        var prefix = "data-prismjs-";
        for (var key in settings) {
            var attr = prefix + key;
            var element = startElement;
            while (element && !element.hasAttribute(attr)) {
                element = element.parentElement;
            }
            if (element) {
                settings[key] = element.getAttribute(attr);
            }
        }
        return settings;
    }
    Prism.plugins.toolbar.registerButton("copy-to-clipboard", function(env) {
        var element = env.element;
        var settings = getSettings(element);
        var linkCopy = document.createElement("button");
        linkCopy.className = "copy-to-clipboard-button";
        linkCopy.setAttribute("type", "button");
        var linkSpan = document.createElement("span");
        linkCopy.appendChild(linkSpan);
        setState("copy");
        registerClipboard(linkCopy, {
            getText: function() {
                return element.textContent;
            },
            success: function() {
                setState("copy-success");
                resetText();
            },
            error: function() {
                setState("copy-error");
                setTimeout(function() {
                    selectElementText(element);
                }, 1);
                resetText();
            }
        });
        return linkCopy;
        function resetText() {
            setTimeout(function() {
                setState("copy");
            }, settings["copy-timeout"]);
        }
        function setState(state) {
            linkSpan.textContent = settings[state];
            linkCopy.setAttribute("data-copy-state", state);
        }
    });
})();

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
                          <span><i class="fas fa-fw fa-folder text-warning me-1"></i>${String(data.value)}</span>
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
    if (event.target.parentElement.matches('a[data-bs-toggle="confirm"]')) {
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
                    label: '<i class="fa fa-check"></i> ' + button.dataset.yes,
                    className: "btn-success"
                },
                cancel: {
                    label: '<i class="fa fa-times"></i> ' + button.dataset.no,
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
        if (data.AttachmentList.length === 0) {
            const noText = placeHolder.dataset.notext;
            const li = document.createElement("li");
            li.innerHTML = `<li><div class="alert alert-info text-break" role="alert" style="white-space:normal">${noText}</div></li>`;
            list.appendChild(li);
        }
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
        if (isPageChange) {
            const toggleBtn = document.querySelector(".albums-toggle"), dropdownEl = new bootstrap.Dropdown(toggleBtn);
            dropdownEl.toggle();
        }
    }).catch(function(error) {
        console.log(error);
        document.getElementById("PostAlbumsLoader").style.display = "none";
        placeHolder.textContent = error;
    });
}

function getPaginationData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById("PostAttachmentListPlaceholder"), list = placeHolder.querySelector("ul"), yafUserId = placeHolder.dataset.userid, pagedResults = {}, ajaxUrl = placeHolder.dataset.url + "api/Attachment/GetAttachments";
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
        document.getElementById("PostAttachmentLoader").style.display = "none";
        if (data.AttachmentList.length === 0) {
            const noText = placeHolder.dataset.notext;
            const li = document.createElement("li");
            li.innerHTML = `<li><div class="alert alert-info text-break" role="alert" style="white-space:normal">${noText}</div></li>`;
            list.appendChild(li);
        }
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
        setPageNumber(pageSize, pageNumber, data.TotalRecords, document.getElementById("AttachmentsListPager"), "Attachments", "getPaginationData");
        if (isPageChange) {
            const toggleBtn = document.querySelector(".attachments-toggle"), dropdownEl = new bootstrap.Dropdown(toggleBtn);
            dropdownEl.toggle();
        }
    }).catch(function(error) {
        console.log(error);
        document.getElementById("PostAttachmentLoader").style.display = "none";
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

function getSearchResultsData(pageNumber) {
    var searchInput = document.querySelector(".searchInput").value, searchInputUser = document.querySelector(".searchUserInput").value, searchInputTag = document.querySelector(".searchTagInput").value, placeHolder = document.getElementById("SearchResultsPlaceholder"), ajaxUrl = placeHolder.dataset.url + "api/Search/GetSearchResults", loadModal = new bootstrap.Modal("#loadModal");
    var useDisplayName = document.querySelector(".searchUserInput").dataset.display === "True";
    var pageSize = document.querySelector(".resultsPage").value, titleOnly = document.querySelector(".titleOnly").value, searchWhat = document.querySelector(".searchWhat").value;
    var minimumLength = placeHolder.dataset.minimum;
    var searchForum = document.querySelector(".searchForum").value === "" ? 0 : parseInt(document.querySelector(".searchForum").value);
    var searchText = "";
    if (searchInput.length && searchInput.length >= minimumLength || searchInputUser.length && searchInputUser.length >= minimumLength || searchInputTag.length && searchInputTag.length >= minimumLength) {
        var replace;
        if (searchInput.length && searchInput.length >= minimumLength) {
            if (titleOnly === "1") {
                if (searchWhat === "0") {
                    replace = searchInput;
                    searchText += ` Topic: (${replace.replace(/(^|\s+)/g, "$1+")})`;
                } else if (searchWhat === "1") {
                    searchText += ` Topic: ${searchInput}`;
                } else if (searchWhat === "2") {
                    searchText += ` Topic:"${searchInput}"`;
                }
            } else {
                if (searchWhat === "0") {
                    replace = searchInput;
                    searchText += `(${replace.replace(/(^|\s+)/g, "$1+")})`;
                } else if (searchWhat === "1") {
                    searchText += `${searchInput}`;
                } else if (searchWhat === "2") {
                    searchText += `"${searchInput}"`;
                }
            }
        }
        if (searchInputUser.length && searchInputUser.length >= minimumLength) {
            var author = useDisplayName ? "AuthorDisplay" : "Author";
            if (searchText.length) searchText += " ";
            if (searchInput.length) {
                searchText += `AND ${author}:${searchInputUser}`;
            } else {
                searchText = `+${author}:${searchInputUser}`;
            }
        }
        if (searchInputTag.length && searchInputTag.length >= minimumLength) {
            if (searchText.length) searchText += " ";
            if (searchInput.length) {
                searchText += `AND TopicTags:${searchInputTag}`;
            } else {
                searchText = `+TopicTags:${searchInputTag}`;
            }
        }
        var searchTopic = {};
        searchTopic.ForumId = searchForum;
        searchTopic.PageSize = pageSize;
        searchTopic.Page = pageNumber;
        searchTopic.SearchTerm = searchText;
        empty(placeHolder);
        loadModal.show();
        fetch(ajaxUrl, {
            method: "POST",
            body: JSON.stringify(searchTopic),
            headers: {
                Accept: "application/json",
                "Content-Type": "application/json;charset=utf-8"
            }
        }).then(res => res.json()).then(data => {
            document.getElementById("loadModal").addEventListener("shown.bs.modal", () => {
                loadModal.hide();
            });
            var posted = placeHolder.dataset.posted, by = placeHolder.dataset.by, lastPost = placeHolder.dataset.lastpost, topic = placeHolder.dataset.topic;
            if (data.SearchResults.length === 0) {
                loadModal.hide();
                const noText = placeHolder.dataset.notext;
                const div = document.createElement("div");
                div.innerHTML = `<div class="alert alert-warning text-center mt-3" role="alert">${noText}</div>`;
                placeHolder.appendChild(div);
                empty(document.getElementById("SearchResultsPagerTop"));
                empty(document.getElementById("SearchResultsPagerBottom"));
            } else {
                loadModal.hide();
                data.SearchResults.forEach(dataItem => {
                    var item = document.createElement("div");
                    var tags = " ";
                    if (dataItem.TopicTags) {
                        const topicTags = dataItem.TopicTags.split(",");
                        topicTags.forEach(d => {
                            tags += `<span class='badge text-bg-secondary me-1'><i class='fas fa-tag me-1'></i>${d}</span>`;
                        });
                    }
                    item.innerHTML = `<div class="row"><div class="col"><div class="card border-0 w-100 mb-3"><div class="card-header bg-transparent border-top border-bottom-0 px-0 pb-0 pt-4 topicTitle"><h5> <a title="${topic}" href="${dataItem.TopicUrl}">${dataItem.Topic}</a>&nbsp;<a title="${lastPost}" href="${dataItem.MessageUrl}"><i class="fas fa-external-link-alt"></i></a> <small class="text-body-secondary">(<a href="${dataItem.ForumUrl}">${dataItem.ForumName}</a>)</small></h5></div><div class="card-body px-0"><h6 class="card-subtitle mb-2 text-body-secondary">${data.Description}</h6><p class="card-text messageContent">${dataItem.Message}</p></div><div class="card-footer bg-transparent border-top-0 px-0 py-2"> <small class="text-body-secondary"><span class="fa-stack"><i class="fa fa-calendar-day fa-stack-1x text-secondary"></i><i class="fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse"></i> <i class="fa fa-clock fa-badge text-secondary"></i> </span>${posted} ${dataItem.Posted} <i class="fa fa-user fa-fw text-secondary"></i>${by} ${useDisplayName ? dataItem.UserDisplayName : dataItem.UserName}${tags}</small> </div></div></div></div>`;
                    placeHolder.appendChild(item);
                });
                setSearchPageNumber(pageSize, pageNumber, data.TotalRecords);
            }
        }).catch(function(error) {
            console.log(error);
            document.getElementById("SearchResultsPlaceholder").style.display = "none";
            placeHolder.textContent = error;
        });
    }
}

function setSearchPageNumber(pageSize, pageNumber, total) {
    const pages = Math.ceil(total / pageSize), pagerHolderTop = document.getElementById("SearchResultsPagerTop"), pagerHolderBottom = document.getElementById("SearchResultsPagerBottom"), pagination = document.createElement("ul"), paginationNavTop = document.createElement("nav"), paginationNavBottom = document.createElement("nav");
    paginationNavTop.setAttribute("aria-label", "Search Page Results");
    paginationNavBottom.setAttribute("aria-label", "Search Page Results");
    pagination.classList.add("pagination");
    empty(pagerHolderTop);
    empty(pagerHolderBottom);
    if (pageNumber > 0) {
        const page = document.createElement("li");
        page.classList.add("page-item");
        page.innerHTML = `<a href="javascript:getSearchResultsData(${pageNumber - 1})" class="page-link"><i class="fas fas fa-angle-left" aria-hidden="true"></i></a>`;
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
        page.innerHTML = `<a href="javascript:getSearchResultsData(${0});" class="page-link">1</a>`;
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
            page.innerHTML = `<a href="javascript:getSearchResultsData(${i});" class="page-link">${i + 1}</a>`;
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
        page.innerHTML = `<a href="javascript:getSearchResultsData(${pages - 1})" class="page-link">${pages}</a>`;
        pagination.appendChild(page);
    }
    if (pageNumber < pages - 1) {
        const page = document.createElement("li");
        page.classList.add("page-item");
        page.innerHTML = `<a href="javascript:getSearchResultsData(${pageNumber + 1})" class="page-link"><i class="fas fas fa-angle-right" aria-hidden="true"></i></a>`;
        pagination.appendChild(page);
    }
    paginationNavTop.appendChild(pagination);
    paginationNavBottom.innerHTML = paginationNavTop.innerHTML;
    pagerHolderTop.appendChild(paginationNavTop);
    pagerHolderBottom.appendChild(paginationNavBottom);
}

document.addEventListener("DOMContentLoaded", function() {
    if (document.querySelector(".searchSimilarTopics") != null) {
        const input = document.querySelector(".searchSimilarTopics");
        input.addEventListener("keyup", () => {
            const placeHolder = document.getElementById("SearchResultsPlaceholder"), ajaxUrl = placeHolder.dataset.url + "api/Search/GetSimilarTitles", searchText = input.value;
            if (searchText.length && searchText.length >= 4) {
                const searchTopic = {};
                searchTopic.ForumId = 0;
                searchTopic.PageSize = 0;
                searchTopic.Page = 0;
                searchTopic.SearchTerm = searchText;
                fetch(ajaxUrl, {
                    method: "POST",
                    body: JSON.stringify(searchTopic),
                    headers: {
                        Accept: "application/json",
                        "Content-Type": "application/json;charset=utf-8"
                    }
                }).then(res => res.json()).then(data => {
                    empty(placeHolder);
                    placeHolder.classList.remove("list-group");
                    if (data.TotalRecords > 0) {
                        var list = document.createElement("ul");
                        list.classList.add("list-group");
                        list.classList.add("list-similar");
                        if (data.SearchResults.length > 0) {
                            const markRead = document.getElementById("MarkRead");
                            markRead.classList.remove("d-none");
                            markRead.classList.add("d-block");
                            data.SearchResults.forEach(dataItem => {
                                var li = document.createElement("li");
                                li.classList.add("list-group-item");
                                li.classList.add("list-group-item-action");
                                li.innerHTML = `<a href="${dataItem.TopicUrl}" target="_blank">${dataItem.Topic}</a>`;
                                list.appendChild(li);
                            });
                        }
                        placeHolder.appendChild(list);
                    }
                }).catch(function(error) {
                    console.log(error);
                    placeHolder.textContent = error;
                });
            }
        });
    }
});

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
                containerOuter: "choices w-100"
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
                containerOuter: "choices w-100"
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
            if (event.detail.choice.customProperties) {
                try {
                    json = JSON.parse(event.detail.choice.customProperties);
                } catch (e) {
                    json = event.detail.choice.customProperties;
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