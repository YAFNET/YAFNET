/*
* Project: Bootstrap Notify = v4.0.0
* Description: Turns standard Bootstrap toasts into "Growl-like" notifications.
* Author: Mouse0270 aka Robert McIntosh
* License: MIT License
* Website: https://github.com/mouse0270/bootstrap-growl
*/

function Notify(content, options) {
    String.format = function () {
        var args = arguments;
        const string = arguments[0];
        return string.replace(/(\{\{\d\}\}|\{\d\})/g,
            function (str) {
                if (str.substring(0, 2) === "{{") return str;
                const num = parseInt(str.match(/\d/)[0]);
                return args[num + 1];
            });
    };

    function isDuplicateNotification(notification) {
        var isDupe = false;

        document.querySelectorAll('[data-notify="container"]').forEach(container => {
            const $el = container,
                title = $el.querySelector('[data-notify="title"]').innerHTML.trim(),
                message = $el.querySelector('[data-notify="message"]').innerHTML.trim();

            // The input string might be different than the actual parsed HTML string!
            // (<br> vs <br /> for example)
            // So we have to force-parse this as HTML here!
            const isSameTitle = title === `${notification.settings.content.title}`.trim(),
                isSameMsg = message === `${notification.settings.content.message}`.trim();
                //isSameType = $el.classList.contains(`alert-${notification.settings.type}`);

            if (isSameTitle && isSameMsg /*&& isSameType*/) {
                //we found the dupe. Set the var and stop checking.
                isDupe = true;
            }
            return !isDupe;
        });

        return isDupe;
    }

    // Create the defaults once
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
        delay: 5000,
        timer: 1000,
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
        template: [
            '<div data-notify="container" class="toast fade m-3" role="alert" aria-live="assertive" aria-atomic="true">',
            '<div class="toast-header">',
            '<span data-notify="icon" class="me-2 text-{0}"></span>',
            '<strong class="me-auto fw-bold" data-notify="title">{1}</strong>',
            '<button type="button" class="ms-2 mb-1 btn-close" data-bs-dismiss="toast" data-notify="dismiss" aria-label="Close">',
            "</button>",
            "</div>",
            '<div class="toast-body" data-notify="message">',
            "{2}",
            '<div class="progress" role="progressbar" data-notify="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100">',
            '<div class="progress-bar bg-{0}" style="width: 0%;"></div>',
            "</div>",
            "</div>"
        ].join("")
    };

    // Setup Content of Notify
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

    //if duplicate messages are not allowed, then only continue if this new message is not a duplicate of one that it already showing
    if (this.settings.allow_duplicates || (!this.settings.allow_duplicates && !isDuplicateNotification(this))) {
        this.init();
    }
}

extend(Notify.prototype,
    {
        init: function () {
            var self = this;
            this.buildNotify();
            if (this.settings.content.icon) {
                this.setIcon();
            }
            this.placement();
            this.bind();

            this.notify = {
                $ele: this.$ele,
                close: function () {
                    self.close();
                }
            };


        },
        update: function (command, update) {
            var commands = {};
            if (typeof command === "string") {
                commands[command] = update;
            } else {
                commands = command;
            }

            for (var cmd in commands) {
                switch (cmd) {
                    /*case "type":
                        this.$ele.classList.remove(`alert-${self.settings.type}`);
                        this.$ele.querySelector('[data-notify="progressbar"] > .progress-bar')
                            .classList.remove(`progress-bar-${self.settings.type}`);
                        self.settings.type = commands[cmd];
                        this.$ele.classList.add(`alert-${commands[cmd]}`)
                            .querySelector('[data-notify="progressbar"] > .progress-bar')
                            .classList.add(`progress-bar-${commands[cmd]}`);
                        break;
                    case "icon":
                        var $icon = this.$ele.querySelector('[data-notify="icon"]');
                        if (self.settings.icon_type.toLowerCase() === "class") {
                            $icon.classList.remove(self.settings.content.icon);
                            $icon.classList.add(commands[cmd]);
                        } else {
                            if ($icon.nodeName !== "IMG") {
                                $icon.querySelector("img");
                            }
                            $icon.src = commands[cmd];
                        }
                        self.settings.content.icon = commands[command];
                        break;
                    case "progress":
                        var newDelay = self.settings.delay - (self.settings.delay * (commands[cmd] / 100));
                        this.$ele.dataset.notifyDelay = newDelay;

                        var progress = this.$ele.querySelector('[data-notify="progressbar"] > div');

                        this.$ele.querySelector('[data-notify="progressbar"]').setAttribute("aria-valuenow", commands[cmd]);
                        progress.style.width = commands[cmd] + "%";
                        break;*/
                    default:
                        this.$ele.querySelector(`[data-notify="${cmd}"]`).innerHTML = commands[cmd];
                }
            }
        },
        buildNotify: function () {
            const content = this.settings.content;
			
			const div = document.createElement("div");
			
            div.innerHTML = String.format(this.settings.template,
                this.settings.type,
                content.title,
                content.message,
                content.url,
                content.target);

            this.$ele = div.firstChild;

            this.$ele.dataset.notifyPosition = this.settings.placement.from + "-" + this.settings.placement.align;

            this.$ele.dataset.bsDelay = this.settings.delay;

            if (!this.settings.allow_dismiss) {
                this.$ele.querySelector('[data-notify="dismiss"]').style.display = "none";
            }

            if ((this.settings.delay <= 0 && !this.settings.showProgressbar) || !this.settings.showProgressbar) {
                if (this.$ele.querySelector('[data-notify="progressbar"]') != null) {
                    this.$ele.querySelector('[data-notify="progressbar"]').remove();
                }
            }
        },
        setIcon: function () {
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
        placement: function () {
            var self = this;

            this.$ele.className += ` ${this.settings.animate.enter}`;

            const toast = new bootstrap.Toast(this.$ele);

            toast.show();

            const pre = ["webkit-", "moz-", "o-", "ms-", ""];
            pre.forEach((prefix) => {
                self.cssText += prefix + "AnimationIterationCount: " + 1;
            });

            // Create Wrapper Container
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
        bind: function () {
            var self = this;

            if (this.$ele.querySelector('[data-notify="dismiss"]') != null) {
                this.$ele.querySelector('[data-notify="dismiss"]').addEventListener("click", () => {
                    self.close();
                });
            }

            if (typeof self.settings.onClick === "function") {
                this.$ele.addEventListener("click",
                    (event) => {
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

                var timer = setInterval(function () {
                    const delay = parseInt(self.$ele.dataset.notifyDelay) - self.settings.timer;
                    if ((self.$ele.dataset.hover === "false" && self.settings.mouse_over === "pause") ||
                        self.settings.mouse_over !== "pause") {
                        const percent = ((self.settings.delay - delay) / self.settings.delay) * 100;
                        self.$ele.dataset.notifyDelay = delay;

                        if (self.settings.showProgressbar) {
                          
                            const div = self.$ele.querySelector('[data-notify="progressbar"] > div');

                            self.$ele.querySelector('[data-notify="progressbar"]').setAttribute("aria-valuenow", percent);

                            div.style.width = percent + "%";
                        }
                    }
                    if (delay <= -(self.settings.timer)) {
                        clearInterval(timer);
                        self.close();
                    }
                },
                    self.settings.timer);
            }
        },
        close: function () {
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
    for (let key in b)
        if (b.hasOwnProperty(key))
            a[key] = b[key];
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

        for (const [key, value] of Object.entries(obj)) {
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