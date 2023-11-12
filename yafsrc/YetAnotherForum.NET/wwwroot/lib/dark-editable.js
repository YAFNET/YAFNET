class DarkEditable {
    constructor(element, options = {}) {
        this._element = { element: null, form: null, load: null, buttons: { success: null, cancel: null } };
        this.element = element;
        this.options = options;

        this.init_options();
        this.init_popover();
        this.init_text();
        this.init_hide_onclick();
        this.init_style();
        if (this.disabled) {
            this.disable();
        }
        this.element.dispatchEvent(new CustomEvent("init"));
    }

    /* INIT METHODS */

    init_options() {
        const get_opt = (name, default_value) => {
            return this[name] = this.element.dataset?.[name] ?? this.options?.[name] ?? default_value;
        };
        const get_opt_bool = (name, default_value) => {
            get_opt(name, default_value);
            if (typeof this[name] != "boolean") {
                if (this[name] == "true") {
                    this[name] = true;
                } else if (this[name] == "false") {
                    this[name] = false;
                } else {
                    this[name] = default_value;
                }
            }
            return this[name];
        };
        //priority date elements
        get_opt("value", this.element.innerHTML);
        get_opt("name", this.element.id);
        get_opt("id", null);
        get_opt("title", "");
        get_opt("type", "text");
        get_opt("emptytext", "Empty");
        get_opt("url", null);
        get_opt("ajaxOptions", {});
        this.ajaxOptions = Object.assign({
            method: "POST",
            dataType: "text"
        }, this.ajaxOptions);
        get_opt_bool("send", true);
        get_opt_bool("disabled", false);
        get_opt_bool("required", false);
        if (this.options?.success && typeof this.options?.success == "function") {
            this.success = this.options.success;
        }
        if (this.options?.error && typeof this.options?.error == "function") {
            this.error = this.options.error;
        }
        switch (this.type) {
            case "select":
                get_opt("source", []);
                if (typeof this.source == "string" && this.source != "") {
                    this.source = JSON.parse(this.source);
                }
                break;
            case "date":
                get_opt("format", "YYYY-MM-DD");
                get_opt("viewformat", "YYYY-MM-DD");
                // if(this.value != empty_value) this.value = moment(this.value).format("YYYY-MM-DD");
                break;
            case "datetime":
                get_opt("format", "YYYY-MM-DD HH:mm");
                get_opt("viewformat", "YYYY-MM-DD HH:mm");
                this.value = moment(this.value).format("YYYY-MM-DDTHH:mm");
                break;
        }
    }

    init_text() {
        const empty_class = "text-danger";
        this.element.classList.remove(empty_class);
        let empty = true;
        switch (this.type) {
            default:
                if (this.value == "") {
                    this.element.innerHTML = this.emptytext;
                } else {
                    this.element.innerHTML = this.value;
                    empty = false;
                }
                break;
            case "select":
                this.element.innerHTML = this.emptytext;
                if (this.value != "" && this.source.length > 0) {
                    this.source.forEach(item => {
                        if (item.value == this.value) {
                            this.element.innerHTML = item.text;
                            empty = false;
                        }
                    });
                }
                break;
            case "date":
            case "datetime":
                if (this.value == "") {
                    this.element.innerHTML = this.emptytext;
                } else {
                    this.element.innerHTML = moment(this.value).format(this.viewformat);
                    empty = false;
                }
                break;
        }
        if (empty) {
            this.element.classList.add(empty_class);
        }
    }

    init_style() {
        this.element.classList.add("dark-editable-element");
    }

    init_hide_onclick() {
        document.addEventListener('click', (e) => {
            const target = e.target;
            if (target === this.popover.tip || target == this.element) return;
            let current = target;
            while (current = current.parentNode) {
                if (current === this.popover.tip) return;
            }
            this.popover.hide();
        });
    }

    init_popover() {
        this.popover = new bootstrap.Popover(this.element, {
            container: "body",
            content: this.route_type(),
            html: true,
            customClass: "dark-editable",
            title: this.title,
        });
        this.element.addEventListener('show.bs.popover', () => {
            this._element.element.value = this.value;
            this.element.dispatchEvent(new CustomEvent("show"));
        });
        this.element.addEventListener('shown.bs.popover', () => {
            this.element.dispatchEvent(new CustomEvent("shown"));
        });
        this.element.addEventListener('hide.bs.popover', () => {
            this.element.dispatchEvent(new CustomEvent("hide"));
        });
        this.element.addEventListener('hidden.bs.popover', () => {
            this.element.dispatchEvent(new CustomEvent("hidden"));
        });
    }

    /* INIT METHODS END */

    route_type() {
        switch (this.type) {
            default:
                throw new Error(`Undefined type`);
            case "text":
            case "password":
            case "email":
            case "url":
            case "tel":
            case "number":
            case "range":
            case "time":
                return this.type_input();
            case "textarea":
                return this.type_textarea();
            case "select":
                return this.type_select();
            case "date":
                return this.type_date();
            case "datetime":
                return this.type_datetime();
        }
    }

    /* TYPES */

    createElement(name) {
        const element = document.createElement(name);
        element.classList.add("form-control");
        if (this.required) {
            element.required = this.required;
        }
        this.add_focus(element);
        return element;
    }

    type_input() {
        const input = this.createElement(`input`);
        input.type = this.type;

        return this.createContainer(input);
    }

    type_textarea() {
        const textarea = this.createElement(`textarea`);

        return this.createContainer(textarea);
    }

    type_select() {
        const select = this.createElement(`select`);
        this.source.forEach(item => {
            const opt = document.createElement(`option`);
            opt.value = item.value;
            opt.innerHTML = item.text;
            select.append(opt);
        });

        return this.createContainer(select);
    }

    type_date() {
        const input = this.createElement(`input`);
        input.type = "date";

        return this.createContainer(input);
    }

    type_datetime() {
        const input = this.createElement(`input`);
        input.type = "datetime-local";

        return this.createContainer(input);
    }

    /* TYPES END */

    /* ADD FOCUS */

    add_focus(element) {
        this.element.addEventListener('shown.bs.popover', function () {
            element.focus();
        });
    }

    /* ADD FOCUS END */

    /* DIV LOAD */

    //true/false
    load(action) {
        if (action) {
            this._element.load.style.display = "block";
        } else {
            this._element.load.style.display = "none";
        }
    }

    /* DIV LOAD END */

    /* CONTAINER DIV */

    createContainer(element) {
        this._element.element = element;
        this._element.form = this.createContainerForm(element);
        this._element.load = this.createContainerLoad();
        this._element.buttons.success = this.createButtonSuccess();
        this._element.buttons.cancel = this.createButtonCancel();
        this._element.form.append(element, this._element.load, this._element.buttons.success, this._element.buttons.cancel);
        return this._element.form;
    }

    createContainerForm(element) {
        const form = document.createElement(`form`);
        form.classList.add("d-flex", "align-items-start");
        form.style.gap = "10px";
        form.addEventListener('submit', async e => {
            e.preventDefault();
            const newValue = element.value;
            if (this.send && this.id && this.url && (this.value != newValue)) {
                this.load(true);
                const response = await this.ajax(newValue);
                if (response.ok) {
                    this.success(response, newValue);
                    this.value = element.value;
                    this.popover.hide();
                    this.init_text();
                } else {
                    this.error(response, newValue);
                }
                this.load(false);
            } else {
                this.value = element.value;
                this.popover.hide();
                this.init_text();
            }
            this.element.dispatchEvent(new CustomEvent("save"));
        });
        return form;
    }

    createContainerLoad() {
        const div = document.createElement(`div`);
        div.style.display = "none";
        div.style.position = "absolute";
        div.style.background = "white";
        div.style.width = "100%";
        div.style.height = "100%";
        div.style.top = 0;
        div.style.left = 0;
        const loader = document.createElement(`div`);
        loader.classList.add("dark-editable-loader");
        div.append(loader);
        return div;
    }

    /* CONTAINER DIV END */

    /* BUTTONS */

    createButton() {
        const button = document.createElement("button");
        button.type = "button";
        button.classList.add("btn", "btn-sm");
        return button;
    }

    createButtonSuccess() {
        const btn_success = this.createButton();
        btn_success.type = "submit";
        btn_success.classList.add("btn-success");
        btn_success.innerHTML = '<i class="fa-solid fa-check"></i>';
        return btn_success;
    }

    createButtonCancel() {
        const btn_cancel = this.createButton();
        btn_cancel.classList.add("btn-danger");
        btn_cancel.innerHTML = '<i class="fa-solid fa-times"></i>';
        btn_cancel.addEventListener("click", () => {
            this.popover.hide();
        });
        return btn_cancel;
    }

    /* BUTTONS END */

    /* AJAX */

    async ajax(new_value) {
        let url = this.url;
        const form = new FormData;
        form.append("id", this.id);
        form.append("name", this.name);
        form.append("value", new_value);
        const option = {};
        option.method = this.ajaxOptions.method;
        if (option.method == "POST") {
            option.body = form;

            option.headers = Object.assign({
                "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
            });
        } else {
            url += "?" + new URLSearchParams(form).toString();
        }

        const response = await fetch(url, option);
        return response;
    }

    success(response, newValue) {

    }

    error(response, newValue) {

    }

    /* AJAX END */

    /* METHODS */

    enable() {
        this.disabled = false;
        this.element.classList.remove("dark-editable-element-disabled");
        this.popover.enable();
    }

    disable() {
        this.disabled = true;
        this.element.classList.add("dark-editable-element-disabled");
        this.popover.disable();
    }

    setValue(value) {
        this.value = value;
        this.init_text();
    }

    getValue() {
        return this.value;
    }

    /* METHODS END */
}