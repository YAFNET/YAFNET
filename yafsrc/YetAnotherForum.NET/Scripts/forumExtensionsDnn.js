var bootbox = function(b, B) {
    "use strict";
    function P(t) {
        const e = Object.create(null, {
            [Symbol.toStringTag]: {
                value: "Module"
            }
        });
        if (t) {
            for (const o in t) if (o !== "default") {
                const a = Object.getOwnPropertyDescriptor(t, o);
                Object.defineProperty(e, o, a.get ? a : {
                    enumerable: !0,
                    get: () => t[o]
                });
            }
        }
        return e.default = t, Object.freeze(e);
    }
    const w = P(B), _ = {
        OK: "موافق",
        CANCEL: "الغاء",
        CONFIRM: "تأكيد"
    }, H = {
        OK: "OK",
        CANCEL: "İmtina et",
        CONFIRM: "Təsdiq et"
    }, V = {
        OK: "Ок",
        CANCEL: "Отказ",
        CONFIRM: "Потвърждавам"
    }, D = {
        OK: "OK",
        CANCEL: "Zrušit",
        CONFIRM: "Potvrdit"
    }, z = {
        OK: "OK",
        CANCEL: "Annuller",
        CONFIRM: "Accepter"
    }, $ = {
        OK: "OK",
        CANCEL: "Abbrechen",
        CONFIRM: "Akzeptieren"
    }, U = {
        OK: "Εντάξει",
        CANCEL: "Ακύρωση",
        CONFIRM: "Επιβεβαίωση"
    }, G = {
        OK: "OK",
        CANCEL: "Cancel",
        CONFIRM: "OK"
    }, W = {
        OK: "OK",
        CANCEL: "Cancelar",
        CONFIRM: "Aceptar"
    }, Z = {
        OK: "OK",
        CANCEL: "Katkesta",
        CONFIRM: "OK"
    }, J = {
        OK: "OK",
        CANCEL: "Ezeztatu",
        CONFIRM: "Onartu"
    }, X = {
        OK: "قبول",
        CANCEL: "لغو",
        CONFIRM: "تایید"
    }, Q = {
        OK: "OK",
        CANCEL: "Peruuta",
        CONFIRM: "OK"
    }, Y = {
        OK: "OK",
        CANCEL: "Annuler",
        CONFIRM: "Confirmer"
    }, ee = {
        OK: "אישור",
        CANCEL: "ביטול",
        CONFIRM: "אישור"
    }, te = {
        OK: "OK",
        CANCEL: "Odustani",
        CONFIRM: "Potvrdi"
    }, oe = {
        OK: "OK",
        CANCEL: "Mégsem",
        CONFIRM: "Megerősít"
    }, re = {
        OK: "OK",
        CANCEL: "Batal",
        CONFIRM: "OK"
    }, ae = {
        OK: "OK",
        CANCEL: "Annulla",
        CONFIRM: "Conferma"
    }, ne = {
        OK: "OK",
        CANCEL: "キャンセル",
        CONFIRM: "OK"
    }, le = {
        OK: "OK",
        CANCEL: "გაუქმება",
        CONFIRM: "დადასტურება"
    }, ie = {
        OK: "OK",
        CANCEL: "취소",
        CONFIRM: "확인"
    }, ce = {
        OK: "Gerai",
        CANCEL: "Atšaukti",
        CONFIRM: "Patvirtinti"
    }, se = {
        OK: "Labi",
        CANCEL: "Atcelt",
        CONFIRM: "Apstiprināt"
    }, ue = {
        OK: "OK",
        CANCEL: "Annuleren",
        CONFIRM: "Accepteren"
    }, de = {
        OK: "OK",
        CANCEL: "Avbryt",
        CONFIRM: "OK"
    }, fe = {
        OK: "OK",
        CANCEL: "Anuluj",
        CONFIRM: "Potwierdź"
    }, be = {
        OK: "OK",
        CANCEL: "Cancelar",
        CONFIRM: "Confirmar"
    }, pe = {
        OK: "OK",
        CANCEL: "Cancelar",
        CONFIRM: "Sim"
    }, me = {
        OK: "OK",
        CANCEL: "Отмена",
        CONFIRM: "Применить"
    }, Oe = {
        OK: "OK",
        CANCEL: "Zrušiť",
        CONFIRM: "Potvrdiť"
    }, he = {
        OK: "OK",
        CANCEL: "Prekliči",
        CONFIRM: "Potrdi"
    }, Ce = {
        OK: "OK",
        CANCEL: "Anulo",
        CONFIRM: "Prano"
    }, ve = {
        OK: "OK",
        CANCEL: "Avbryt",
        CONFIRM: "OK"
    }, ye = {
        OK: "Sawa",
        CANCEL: "Ghairi",
        CONFIRM: "Thibitisha"
    }, we = {
        OK: "சரி",
        CANCEL: "ரத்து செய்",
        CONFIRM: "உறுதி செய்"
    }, ge = {
        OK: "ตกลง",
        CANCEL: "ยกเลิก",
        CONFIRM: "ยืนยัน"
    }, Ee = {
        OK: "Tamam",
        CANCEL: "İptal",
        CONFIRM: "Onayla"
    }, Ne = {
        OK: "OK",
        CANCEL: "Відміна",
        CONFIRM: "Прийняти"
    }, Ae = {
        OK: "OK",
        CANCEL: "Hủy bỏ",
        CONFIRM: "Xác nhận"
    }, Le = {
        OK: "OK",
        CANCEL: "取消",
        CONFIRM: "确认"
    }, ke = {
        OK: "OK",
        CANCEL: "取消",
        CONFIRM: "確認"
    }, Ke = "6.0.6", xe = {
        ar: _,
        az: H,
        bgBG: V,
        cs: D,
        da: z,
        de: $,
        el: U,
        en: G,
        es: W,
        et: Z,
        eu: J,
        fa: X,
        fi: Q,
        fr: Y,
        he: ee,
        hr: te,
        hu: oe,
        id: re,
        it: ae,
        ja: ne,
        ka: le,
        ko: ie,
        lt: ce,
        lv: se,
        nl: ue,
        no: de,
        pl: fe,
        pt: be,
        ptBR: pe,
        ru: me,
        sk: Oe,
        sl: he,
        sq: Ce,
        sv: ve,
        sw: ye,
        ta: we,
        th: ge,
        tr: Ee,
        uk: Ne,
        vi: Ae,
        zhCN: Le,
        zhTW: ke
    }, Me = {
        dialog: '<div class="bootbox modal" tabindex="-1" role="dialog" aria-hidden="true"><div class="modal-dialog"><div class="modal-content"><div class="modal-body"><div class="bootbox-body"></div></div></div></div></div>',
        header: '<div class="modal-header"><h5 class="modal-title"></h5></div>',
        footer: '<div class="modal-footer"></div>',
        closeButton: '<button type="button" class="bootbox-close-button close btn-close" aria-label="Close"></button>',
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
    }, Ie = {
        locale: "en",
        backdrop: "static",
        animate: !0,
        className: null,
        closeButton: !0,
        show: !0,
        container: "body",
        value: "",
        inputType: "text",
        errorMessage: null,
        swapButtonOrder: !1,
        centerVertical: !1,
        multiple: !1,
        scrollable: !1,
        reusable: !1,
        relatedTarget: null,
        size: null,
        id: null
    };
    function x(t) {
        return /([01][0-9]|2[0-3]):[0-5][0-9]?:[0-5][0-9]/.test(t);
    }
    function M(t) {
        return /(\d{4})-(\d{2})-(\d{2})/.test(t);
    }
    function g(t, e) {
        if (typeof e == "string" && typeof t[e] == "function") t[e](); else {
            const o = typeof e == "string" ? new Event(e, {
                bubbles: !0
            }) : e;
            t.dispatchEvent(o);
        }
    }
    const Se = Ke, C = xe, m = Me, h = Ie;
    function Fe(t) {
        return C[t];
    }
    function Re() {
        return C;
    }
    function qe(t, e) {
        [ "OK", "CANCEL", "CONFIRM" ].forEach((o, a) => {
            if (!e[o]) throw new Error(`Please supply a translation for "${o}"`);
        }), C[t] = {
            OK: e.OK,
            CANCEL: e.CANCEL,
            CONFIRM: e.CONFIRM
        };
    }
    function Te(t) {
        if (t !== "en") delete C[t]; else throw new Error('"en" is used as the default and fallback locale and cannot be removed.');
    }
    function je(t) {
        return t && (t = t.replace("-", "")), I("locale", t);
    }
    function I(...t) {
        let e = {};
        t.length === 2 ? e[t[0]] = t[1] : e = t[0], Object.assign(h, e);
    }
    function Be() {
        document.querySelectorAll(".bootbox").forEach(t => {
            const e = w.Modal.getInstance(t);
            e && e.hide();
        });
    }
    function S(t) {
        return S();
    }
    function E(t) {
        if (w.Modal === void 0) throw new Error('"bootstrap.Modal" is not defined; please double check you have included the Bootstrap JavaScript library. See https://getbootstrap.com/docs/5.3/getting-started/introduction/ for more details.');
        t = Ue(t);
        let e = p(m.dialog), o = e.querySelector(".modal-dialog"), a = e.querySelector(".modal-body"), r = p(m.header), n = p(m.footer), i = t.buttons;
        t.messageForm ? a.querySelector(".bootbox-body").append(t.messageForm) : typeof t.message == "string" ? a.querySelector(".bootbox-body").innerHTML = t.message : a.querySelector(".bootbox-body").append(t.message);
        let u = {};
        if (typeof t.onEscape == "function" && (u.onEscape = t.onEscape), T(t.buttons) > 0) {
            for (const [ l, d ] of Object.entries(i)) {
                let O = p(m.button);
                O.dataset.bbHandler = l;
                var v = d.className.split(" ");
                switch (v.forEach(L => {
                    O.classList.add(L);
                }), l) {
                  case "ok":
                  case "confirm":
                    O.classList.add("bootbox-accept");
                    break;

                  case "cancel":
                    O.classList.add("bootbox-cancel");
                    break;
                }
                O.innerHTML = d.label, d.id && O.setAttribute("id", d.id), d.disabled === !0 && (O.disabled = !0), 
                n.append(O), typeof d.callback == "function" && (u[l] = d.callback);
            }
            a.after(n);
        }
        if (t.animate === !0 && e.classList.add("fade"), t.className && t.className.split(" ").forEach(l => {
            e.classList.add(l);
        }), t.id && e.setAttribute("id", t.id), t.size) switch (t.size) {
          case "small":
          case "sm":
            o.classList.add("modal-sm");
            break;

          case "large":
          case "lg":
            o.classList.add("modal-lg");
            break;

          case "extra-large":
          case "xl":
            o.classList.add("modal-xl");
            break;
        }
        if (t.scrollable && o.classList.add("modal-dialog-scrollable"), t.title || t.closeButton) {
            if (t.title ? r.querySelector(".modal-title").innerHTML = t.title : r.classList.add("border-0"), 
            t.closeButton) {
                let l = p(m.closeButton);
                r.append(l);
            }
            a.before(r);
        }
        if (t.centerVertical && o.classList.add("modal-dialog-centered"), t.reusable || (e.addEventListener("hide.bs.modal", l => {
            l.target === e && (e.removeEventListener("escape.close.bb", () => {}), 
            e.removeEventListener("click", () => {}));
        }, {
            once: !0
        }), e.addEventListener("hidden.bs.modal", l => {
            l.target === e && e.remove();
        }, {
            once: !0
        })), t.onHide) if (typeof t.onHide == "function") e.addEventListener("hide.bs.modal", t.onHide); else throw new Error('Argument supplied to "onHide" must be a function');
        if (t.onHidden) if (typeof t.onHidden == "function") e.addEventListener("hidden.bs.modal", t.onHidden); else throw new Error('Argument supplied to "onHidden" must be a function');
        if (t.onShow) if (typeof t.onShow == "function") A(e, "show.bs.modal", t.onShow); else throw new Error('Argument supplied to "onShow" must be a function');
        if (e.addEventListener("shown.bs.modal", q), t.onShown) if (typeof t.onShown == "function") A(e, "shown.bs.modal", t.onShown); else throw new Error('Argument supplied to "onShown" must be a function');
        if (t.backdrop === !0) {
            let l = !1;
            A(e, "mousedown", d => {
                d.stopPropagation(), l = !0;
            }, ".modal-content"), A(e, "click.dismiss.bs.modal", d => {
                l || d.target !== d.currentTarget || g(e, "escape.close.bb");
            });
        }
        e.addEventListener("escape.close.bb", l => {
            u.onEscape && K(l, e, u.onEscape);
        }), e.addEventListener("click", l => {
            if (l.target.nodeName.toLowerCase() === "button" && !l.target.classList.contains("disabled")) {
                const d = l.target.dataset.bbHandler;
                d !== void 0 && K(l, e, u[d]);
            }
        }), document.addEventListener("click", l => {
            l.target.closest(".bootbox-close-button") && K(l, e, u.onEscape);
        }), e.addEventListener("keyup", l => {
            (l.which === 27 || l.detail.which === 27) && g(e, "escape.close.bb");
        }), typeof t.container == "object" ? t.container.append(e) : document.querySelector(t.container)?.append(e);
        const y = new w.Modal(e, {
            backdrop: t.backdrop,
            keyboard: !1
        });
        return t.show && (t.relatedTarget ? y.show(t.relatedTarget) : y.show()), 
        {
            _element: e,
            _modal: y,
            _options: t
        };
    }
    function Pe(...t) {
        const e = k("alert", [ "ok" ], [ "message", "callback" ], t);
        if (e.callback && typeof e.callback != "function") throw new Error('alert requires the "callback" property to be a function when provided');
        return e.buttons.ok.callback = e.onEscape = function() {
            return typeof e.callback == "function" ? e.callback.call(this) : !0;
        }, E(e);
    }
    function _e(...t) {
        let e;
        if (e = k("confirm", [ "cancel", "confirm" ], [ "message", "callback" ], t), 
        typeof e.callback != "function") throw new Error("confirm requires a callback");
        let o = e.buttons.cancel, a = e.buttons.confirm;
        return a || (e.buttons.confirm = N("confirm", e.locale), a = e.buttons.confirm), 
        o || (e.buttons.cancel = N("cancel", e.locale), o = e.buttons.cancel), o.callback = e.onEscape = function() {
            return e.callback?.call(this, null);
        }, a.callback = function() {
            return e.callback?.call(this, !0);
        }, e.buttons.cancel = o, e.buttons.confirm = a, E(e);
    }
    function He(...t) {
        let e, o, a, r, n, i;
        a = p(m.form), e = k("prompt", [ "cancel", "confirm" ], [ "title", "callback" ], t), 
        e.value || (e.value = h.value), e.inputType || (e.inputType = h.inputType), 
        n = e.show === void 0 ? h.show : e.show, e.show = !1;
        var u = e.buttons.cancel;
        u || (e.buttons.cancel = N("cancel", e.locale), u = e.buttons.cancel), u.callback = e.onEscape = function() {
            return e.callback?.call(this, null);
        }, e.buttons.cancel = u;
        var v = e.buttons.confirm;
        if (v || (e.buttons.confirm = N("confirm", e.locale), v = e.buttons.confirm), 
        v.callback = function() {
            let s;
            if (a.classList.add("was-validated"), e.inputType === "checkbox") {
                const c = Array.from(r.querySelectorAll('input[type="checkbox"]:checked'));
                if (s = Array.from(c).map(f => f.value), e.required === !0 && c.length === 0) return !1;
            } else if (e.inputType === "radio") s = r.querySelector('input[type="radio"]:checked').value; else {
                let c = r;
                if (c.setCustomValidity(""), c.checkValidity && !c.checkValidity()) return e.errorMessage && c.setCustomValidity(e.errorMessage), 
                c.reportValidity && c.reportValidity(), !1;
                e.inputType === "select" && e.multiple === !0 ? s = Array.from(r.querySelectorAll("option:checked")).map(f => f.value) : s = c.value;
            }
            return e.callback?.call(this, s);
        }, e.buttons.confirm = v, !e.title) throw new Error("prompt requires a title");
        if (typeof e.callback != "function") throw new Error("prompt requires a callback");
        var y = m.inputs;
        if (!y[e.inputType]) throw new Error("Invalid prompt type");
        switch (r = p(y[e.inputType]), e.inputType !== "textarea" && r.addEventListener("keydown", function(s) {
            if (s.key === "Enter") {
                s.preventDefault();
                var c = o.querySelector(".bootbox-accept");
                g(c, "click");
            }
        }), e.inputType) {
          case "text":
          case "textarea":
          case "email":
          case "password":
            r.value = e.value.toString(), e.placeholder && r.setAttribute("placeholder", e.placeholder), 
            e.pattern && r.setAttribute("pattern", e.pattern), e.maxlength && r.setAttribute("maxlength", e.maxlength.toString()), 
            e.required && (r.required = !0), e.inputType === "textarea" && e.rows && !isNaN(parseInt(e.rows.toString())) && r.setAttribute("rows", e.rows.toString());
            break;

          case "date":
          case "time":
          case "number":
          case "range":
            if (r.value = e.value.toString(), e.placeholder && r.setAttribute("placeholder", e.placeholder), 
            e.pattern ? r.setAttribute("pattern", e.pattern) : e.inputType === "date" ? r.setAttribute("pattern", "d{4}-d{2}-d{2}") : e.inputType === "time" && r.setAttribute("pattern", "d{2}:d{2}"), 
            e.required && (r.required = !0), e.step) if (typeof e.step == "string" && (e.step === "any" || parseFloat(e.step) > 0)) r.setAttribute("step", e.step); else if (typeof e.step == "number" && !isNaN(e.step) && e.step > 0) r.setAttribute("step", e.step.toString()); else throw new Error('"step" must be a valid positive number or the value "any". See https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#attr-step for more information.');
            Ge(e.inputType, e.min, e.max) && (e.min !== void 0 && r.setAttribute("min", e.min.toString()), 
            e.max !== void 0 && r.setAttribute("max", e.max.toString()));
            break;

          case "select":
            var l = {};
            if (i = e.inputOptions || [], !Array.isArray(i)) throw new Error("Please pass an array of input options");
            if (!i.length) throw new Error('prompt with "inputType" set to "select" requires at least one option');
            e.required && (r.required = !0), e.multiple && (r.multiple = !0);
            for (const [ , s ] of Object.entries(i)) {
                let c = r;
                if (s.value === void 0 || s.text === void 0) throw new Error('each option needs a "value" property and a "text" property');
                if (s.group) {
                    if (!l[s.group]) {
                        var d = p("<optgroup />");
                        d.setAttribute("label", s.group), l[s.group] = {
                            Content: d
                        };
                    }
                    c = l[s.group].Content;
                }
                let f = p(m.option);
                f.setAttribute("value", s.value), f.textContent = s.text, c.append(f);
            }
            for (const [ s, c ] of Object.entries(l)) r.append(c.Content);
            r.value = e.value.toString();
            break;

          case "checkbox":
            var O = Array.isArray(e.value) ? e.value : [ e.value ];
            if (i = e.inputOptions || [], !i.length) throw new Error('prompt with "inputType" set to "checkbox" requires at least one option');
            r = p('<div class="bootbox-checkbox-list"></div>');
            for (const [ s, c ] of Object.entries(i)) {
                if (c.value === void 0 || c.text === void 0) throw new Error('each option needs a "value" property and a "text" property');
                let f = p(m.inputs[e.inputType]);
                f.querySelector("input")?.setAttribute("value", c.value), f.querySelector("label")?.append(`
${c.text}`);
                for (const [ Je, Ze ] of Object.entries(O)) Ze === c.value && f.querySelector("input")?.setAttribute("checked", "true");
                r.append(f);
            }
            break;

          case "radio":
            if (e.value !== void 0 && Array.isArray(e.value)) throw new Error('prompt with "inputType" set to "radio" requires a single, non-array value for "value"');
            if (i = e.inputOptions || [], !i.length) throw new Error('prompt with "inputType" set to "radio" requires at least one option');
            r = p('<div class="bootbox-radiobutton-list"></div>');
            var L = !0;
            for (const [ s, c ] of Object.entries(i)) {
                if (c.value === void 0 || c.text === void 0) throw new Error('each option needs a "value" property and a "text" property');
                let f = p(m.inputs[e.inputType]);
                f.querySelector("input")?.setAttribute("value", c.value), f.querySelector("label")?.append(`
${c.text}`), e.value !== void 0 && c.value === e.value && (f.querySelector("input").checked = !0, 
                L = !1), r.append(f);
            }
            L && r.querySelector('input[type="radio"]')?.setAttribute("checked", "true");
            break;
        }
        if (a.append(r), a.addEventListener("submit", s => {
            s.preventDefault(), s.stopPropagation(), a.classList.remove("was-validated"), 
            o.querySelector(".bootbox-accept")?.click();
        }), e.message && e.message.trim() !== "") {
            let s = p(m.promptMessage).innerHTML = e.message;
            a.prepend(s), e.messageForm = a;
        } else e.messageForm = a;
        const j = E(e);
        o = j._element, o.removeEventListener("shown.bs.modal", q), o.addEventListener("shown.bs.modal", () => {
            r.focus();
        });
        const We = j._modal;
        return n === !0 && We.show(), o;
    }
    function F(...t) {
        const e = {};
        let o = !1, a = 0;
        const r = t.length;
        Object.prototype.toString.call(t[0]) === "[object Boolean]" && (o = t[0], 
        a++);
        const n = i => {
            for (const u in i) Object.prototype.hasOwnProperty.call(i, u) && (o && Object.prototype.toString.call(i[u]) === "[object Object]" ? e[u] = F(!0, e[u], i[u]) : e[u] = i[u]);
        };
        for (;a < r; a++) {
            const i = t[a];
            n(i);
        }
        return e;
    }
    function Ve(t, e) {
        const o = t.length;
        let a = {};
        if (o < 1 || o > 2) throw new Error("Invalid argument length");
        return o === 2 || typeof t[0] == "string" ? (a[e[0]] = t[0], a[e[1]] = t[1]) : a = t[0], 
        a;
    }
    function De(t, e, o) {
        return F({}, t, Ve(e, o));
    }
    function k(t, e, o, a) {
        let r;
        a && a[0] && (r = a[0].locale || h.locale, (a[0].swapButtonOrder || h.swapButtonOrder) && (e = e.reverse()));
        const n = {
            className: `bootbox-${t}`,
            buttons: $e(e, r),
            show: !0,
            closeButton: !0,
            animate: !0,
            locale: "en",
            swapButtonOrder: !1,
            scrollable: !1,
            reusable: !1,
            centerVertical: !1
        };
        return ze(De(n, a, o), e);
    }
    function ze(t, e) {
        const o = {};
        for (const [ a, r ] of Object.entries(e)) o[r] = !0;
        for (const [ a ] of Object.entries(t.buttons)) if (o[a] === void 0) throw new Error(`button key "${a}" is not allowed (options are ${e.join(" ")})`);
        return t;
    }
    function $e(t, e) {
        const o = {};
        for (let a = 0, r = t.length; a < r; a++) {
            const n = t[a], i = n.toLowerCase(), u = n.toUpperCase();
            o[i] = {
                label: R(u, e),
                className: ""
            };
        }
        return o;
    }
    function N(t, e) {
        return {
            label: R(t.toUpperCase(), e),
            className: ""
        };
    }
    function R(t, e) {
        const o = C[e];
        return o ? o[t] : C.en[t];
    }
    function Ue(t) {
        let e, o;
        if (!t.message && !t.messageForm) throw new Error('"message" option must not be null or an empty string.');
        t = Object.assign({}, h, t), t.backdrop ? t.backdrop = typeof t.backdrop == "string" && t.backdrop.toLowerCase() === "static" ? "static" : !0 : t.backdrop = t.backdrop === !1 || t.backdrop === 0 ? !1 : "static", 
        t.buttons || (t.buttons = {}), e = t.buttons, o = T(e);
        let a = 0;
        for (let [ r, n ] of Object.entries(e)) {
            if (typeof n == "function" && (n = e[r] = {
                callback: n,
                label: "",
                className: ""
            }), Object.prototype.toString.call(n).replace(/^\[object (.+)\]$/, "$1").toLowerCase() !== "object") throw new Error(`button with key "${r}" must be an object`);
            if (n.label || (n.label = r), !n.className) {
                let i = !1;
                t.swapButtonOrder ? i = a === 0 : i = a === o - 1, o <= 2 && i ? n.className = "btn-primary" : n.className = "btn-secondary";
            }
            a++;
        }
        return t;
    }
    function q(t) {
        const e = t?.data?.dialog?.querySelector(".bootbox-accept");
        e && g(e, "focus");
    }
    function T(t) {
        return Object.keys(t).length;
    }
    function K(t, e, o) {
        t.stopPropagation(), t.preventDefault(), !(typeof o == "function" && o.call(e, t) === !1) && e && w.Modal.getInstance(e)?.hide();
    }
    function Ge(t, e, o) {
        let a = !1, r = !0, n = !0;
        if (t === "date") e !== void 0 && !(r = M(e)) ? console.warn('Invalid "min" date format for input type "date".') : o !== void 0 && !(n = M(o)) && console.warn('Invalid "max" date format for input type "date".'); else if (t === "time") {
            if (e !== void 0 && !(r = x(e))) throw new Error('"min" is not a valid time.');
            if (o !== void 0 && !(n = x(o))) throw new Error('"max" is not a valid time.');
        } else {
            if (e !== void 0 && isNaN(Number(e))) throw r = !1, new Error('"min" must be a valid number.');
            if (o !== void 0 && isNaN(Number(o))) throw n = !1, new Error('"max" must be a valid number.');
        }
        if (r && n) {
            if (typeof e == "number" && typeof o == "number" && o < e) throw new Error('"max" must be greater than or equal to "min".');
            if (typeof e == "string" && typeof o == "string" && o < e) throw new Error('"max" must be greater than or equal to "min".');
            a = !0;
        }
        return a;
    }
    function p(t) {
        const e = document.createElement("template");
        return e.innerHTML = t.trim(), e.content.children[0];
    }
    function A(t, e, o, a) {
        if (a) {
            const r = n => {
                if (!n.target) return;
                const i = n.target.closest(a);
                i && o.call(i, n);
            };
            return t.addEventListener(e, r), r;
        } else {
            const r = n => {
                o.call(t, n);
            };
            return t.addEventListener(e, r), r;
        }
    }
    return b.VERSION = Se, b.addLocale = qe, b.alert = Pe, b.confirm = _e, b.dialog = E, 
    b.getAllLocales = Re, b.getLocale = Fe, b.hideAll = Be, b.init = S, b.prompt = He, 
    b.removeLocale = Te, b.setDefaults = I, b.setLocale = je, Object.defineProperty(b, Symbol.toStringTag, {
        value: "Module"
    }), b;
}({}, bootstrap);

var DarkEditable = function(l) {
    "use strict";
    var h = document.createElement("style");
    h.textContent = `.dark-editable-element{border-bottom:dashed 1px #0088cc;text-decoration:none;cursor:pointer}.dark-editable-element-disabled{border-bottom:none;cursor:default}.dark-editable-element-empty{font-style:italic;color:#d14}.dark-editable{max-width:none}.dark-editable-loader{font-size:5px;left:50%;top:50%;width:1em;height:1em;border-radius:50%;position:relative;text-indent:-9999em;-webkit-animation:load5 1.1s infinite ease;animation:load5 1.1s infinite ease;-webkit-transform:translateZ(0);transform:translateZ(0)}@-webkit-keyframes load5{0%,to{-webkit-box-shadow:0em -2.6em 0em 0em #000000,1.8em -1.8em 0 0em rgba(0,0,0,.2),2.5em 0em 0 0em rgba(0,0,0,.2),1.75em 1.75em 0 0em rgba(0,0,0,.2),0em 2.5em 0 0em rgba(0,0,0,.2),-1.8em 1.8em 0 0em rgba(0,0,0,.2),-2.6em 0em 0 0em rgba(0,0,0,.5),-1.8em -1.8em 0 0em rgba(0,0,0,.7);box-shadow:0 -2.6em #000,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #00000080,-1.8em -1.8em #000000b3}12.5%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.7),1.8em -1.8em 0 0em #000000,2.5em 0em 0 0em rgba(0,0,0,.2),1.75em 1.75em 0 0em rgba(0,0,0,.2),0em 2.5em 0 0em rgba(0,0,0,.2),-1.8em 1.8em 0 0em rgba(0,0,0,.2),-2.6em 0em 0 0em rgba(0,0,0,.2),-1.8em -1.8em 0 0em rgba(0,0,0,.5);box-shadow:0 -2.6em #000000b3,1.8em -1.8em #000,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #00000080}25%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.5),1.8em -1.8em 0 0em rgba(0,0,0,.7),2.5em 0em 0 0em #000000,1.75em 1.75em 0 0em rgba(0,0,0,.2),0em 2.5em 0 0em rgba(0,0,0,.2),-1.8em 1.8em 0 0em rgba(0,0,0,.2),-2.6em 0em 0 0em rgba(0,0,0,.2),-1.8em -1.8em 0 0em rgba(0,0,0,.2);box-shadow:0 -2.6em #00000080,1.8em -1.8em #000000b3,2.5em 0 #000,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}37.5%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.2),1.8em -1.8em 0 0em rgba(0,0,0,.5),2.5em 0em 0 0em rgba(0,0,0,.7),1.75em 1.75em 0 0em #000000,0em 2.5em 0 0em rgba(0,0,0,.2),-1.8em 1.8em 0 0em rgba(0,0,0,.2),-2.6em 0em 0 0em rgba(0,0,0,.2),-1.8em -1.8em 0 0em rgba(0,0,0,.2);box-shadow:0 -2.6em #0003,1.8em -1.8em #00000080,2.5em 0 #000000b3,1.75em 1.75em #000,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}50%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.2),1.8em -1.8em 0 0em rgba(0,0,0,.2),2.5em 0em 0 0em rgba(0,0,0,.5),1.75em 1.75em 0 0em rgba(0,0,0,.7),0em 2.5em 0 0em #000000,-1.8em 1.8em 0 0em rgba(0,0,0,.2),-2.6em 0em 0 0em rgba(0,0,0,.2),-1.8em -1.8em 0 0em rgba(0,0,0,.2);box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #00000080,1.75em 1.75em #000000b3,0 2.5em #000,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}62.5%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.2),1.8em -1.8em 0 0em rgba(0,0,0,.2),2.5em 0em 0 0em rgba(0,0,0,.2),1.75em 1.75em 0 0em rgba(0,0,0,.5),0em 2.5em 0 0em rgba(0,0,0,.7),-1.8em 1.8em 0 0em #000000,-2.6em 0em 0 0em rgba(0,0,0,.2),-1.8em -1.8em 0 0em rgba(0,0,0,.2);box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #00000080,0 2.5em #000000b3,-1.8em 1.8em #000,-2.6em 0 #0003,-1.8em -1.8em #0003}75%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.2),1.8em -1.8em 0 0em rgba(0,0,0,.2),2.5em 0em 0 0em rgba(0,0,0,.2),1.75em 1.75em 0 0em rgba(0,0,0,.2),0em 2.5em 0 0em rgba(0,0,0,.5),-1.8em 1.8em 0 0em rgba(0,0,0,.7),-2.6em 0em 0 0em #000000,-1.8em -1.8em 0 0em rgba(0,0,0,.2);box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #00000080,-1.8em 1.8em #000000b3,-2.6em 0 #000,-1.8em -1.8em #0003}87.5%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.2),1.8em -1.8em 0 0em rgba(0,0,0,.2),2.5em 0em 0 0em rgba(0,0,0,.2),1.75em 1.75em 0 0em rgba(0,0,0,.2),0em 2.5em 0 0em rgba(0,0,0,.2),-1.8em 1.8em 0 0em rgba(0,0,0,.5),-2.6em 0em 0 0em rgba(0,0,0,.7),-1.8em -1.8em 0 0em #000000;box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #00000080,-2.6em 0 #000000b3,-1.8em -1.8em #000}}@keyframes load5{0%,to{-webkit-box-shadow:0em -2.6em 0em 0em #000000,1.8em -1.8em 0 0em rgba(0,0,0,.2),2.5em 0em 0 0em rgba(0,0,0,.2),1.75em 1.75em 0 0em rgba(0,0,0,.2),0em 2.5em 0 0em rgba(0,0,0,.2),-1.8em 1.8em 0 0em rgba(0,0,0,.2),-2.6em 0em 0 0em rgba(0,0,0,.5),-1.8em -1.8em 0 0em rgba(0,0,0,.7);box-shadow:0 -2.6em #000,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #00000080,-1.8em -1.8em #000000b3}12.5%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.7),1.8em -1.8em 0 0em #000000,2.5em 0em 0 0em rgba(0,0,0,.2),1.75em 1.75em 0 0em rgba(0,0,0,.2),0em 2.5em 0 0em rgba(0,0,0,.2),-1.8em 1.8em 0 0em rgba(0,0,0,.2),-2.6em 0em 0 0em rgba(0,0,0,.2),-1.8em -1.8em 0 0em rgba(0,0,0,.5);box-shadow:0 -2.6em #000000b3,1.8em -1.8em #000,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #00000080}25%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.5),1.8em -1.8em 0 0em rgba(0,0,0,.7),2.5em 0em 0 0em #000000,1.75em 1.75em 0 0em rgba(0,0,0,.2),0em 2.5em 0 0em rgba(0,0,0,.2),-1.8em 1.8em 0 0em rgba(0,0,0,.2),-2.6em 0em 0 0em rgba(0,0,0,.2),-1.8em -1.8em 0 0em rgba(0,0,0,.2);box-shadow:0 -2.6em #00000080,1.8em -1.8em #000000b3,2.5em 0 #000,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}37.5%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.2),1.8em -1.8em 0 0em rgba(0,0,0,.5),2.5em 0em 0 0em rgba(0,0,0,.7),1.75em 1.75em 0 0em #000000,0em 2.5em 0 0em rgba(0,0,0,.2),-1.8em 1.8em 0 0em rgba(0,0,0,.2),-2.6em 0em 0 0em rgba(0,0,0,.2),-1.8em -1.8em 0 0em rgba(0,0,0,.2);box-shadow:0 -2.6em #0003,1.8em -1.8em #00000080,2.5em 0 #000000b3,1.75em 1.75em #000,0 2.5em #0003,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}50%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.2),1.8em -1.8em 0 0em rgba(0,0,0,.2),2.5em 0em 0 0em rgba(0,0,0,.5),1.75em 1.75em 0 0em rgba(0,0,0,.7),0em 2.5em 0 0em #000000,-1.8em 1.8em 0 0em rgba(0,0,0,.2),-2.6em 0em 0 0em rgba(0,0,0,.2),-1.8em -1.8em 0 0em rgba(0,0,0,.2);box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #00000080,1.75em 1.75em #000000b3,0 2.5em #000,-1.8em 1.8em #0003,-2.6em 0 #0003,-1.8em -1.8em #0003}62.5%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.2),1.8em -1.8em 0 0em rgba(0,0,0,.2),2.5em 0em 0 0em rgba(0,0,0,.2),1.75em 1.75em 0 0em rgba(0,0,0,.5),0em 2.5em 0 0em rgba(0,0,0,.7),-1.8em 1.8em 0 0em #000000,-2.6em 0em 0 0em rgba(0,0,0,.2),-1.8em -1.8em 0 0em rgba(0,0,0,.2);box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #00000080,0 2.5em #000000b3,-1.8em 1.8em #000,-2.6em 0 #0003,-1.8em -1.8em #0003}75%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.2),1.8em -1.8em 0 0em rgba(0,0,0,.2),2.5em 0em 0 0em rgba(0,0,0,.2),1.75em 1.75em 0 0em rgba(0,0,0,.2),0em 2.5em 0 0em rgba(0,0,0,.5),-1.8em 1.8em 0 0em rgba(0,0,0,.7),-2.6em 0em 0 0em #000000,-1.8em -1.8em 0 0em rgba(0,0,0,.2);box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #00000080,-1.8em 1.8em #000000b3,-2.6em 0 #000,-1.8em -1.8em #0003}87.5%{-webkit-box-shadow:0em -2.6em 0em 0em rgba(0,0,0,.2),1.8em -1.8em 0 0em rgba(0,0,0,.2),2.5em 0em 0 0em rgba(0,0,0,.2),1.75em 1.75em 0 0em rgba(0,0,0,.2),0em 2.5em 0 0em rgba(0,0,0,.2),-1.8em 1.8em 0 0em rgba(0,0,0,.5),-2.6em 0em 0 0em rgba(0,0,0,.7),-1.8em -1.8em 0 0em #000000;box-shadow:0 -2.6em #0003,1.8em -1.8em #0003,2.5em 0 #0003,1.75em 1.75em #0003,0 2.5em #0003,-1.8em 1.8em #00000080,-2.6em 0 #000000b3,-1.8em -1.8em #000}}
/*$vite$:1*/`, document.head.appendChild(h);
    class r {
        context;
        constructor(e) {
            if (this.constructor === r) throw new Error("It's abstract class");
            this.context = e;
        }
        event_show() {
            if (this.context.typeElement.hideError(), !this.context.typeElement.element) throw new Error("Element is missing!");
            this.context.typeElement.element.value = this.context.getValue(), this.context.element.dispatchEvent(new CustomEvent("show", {
                detail: {
                    DarkEditable: this.context
                }
            }));
        }
        event_shown() {
            this.context.element.dispatchEvent(new CustomEvent("shown", {
                detail: {
                    DarkEditable: this.context
                }
            }));
        }
        event_hide() {
            this.context.element.dispatchEvent(new CustomEvent("hide", {
                detail: {
                    DarkEditable: this.context
                }
            }));
        }
        event_hidden() {
            this.context.element.dispatchEvent(new CustomEvent("hidden", {
                detail: {
                    DarkEditable: this.context
                }
            }));
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
    class d extends r {
        popover = null;
        init() {
            const e = {
                container: "body",
                content: this.context.typeElement.create(),
                html: !0,
                customClass: "dark-editable",
                title: this.context.options.title
            };
            this.popover = new l.Popover(this.context.element, Object.assign(e, this.context.options.popoverOptions)), 
            this.context.element.addEventListener("show.bs.popover", () => {
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
                let m = s.parentNode;
                for (;m; ) {
                    if (m === this.popover.tip) return;
                    m = m.parentNode;
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
    class b extends r {
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
    class n {
        context;
        element = null;
        error = null;
        form = null;
        load = null;
        buttons = {
            success: null,
            cancel: null
        };
        constructor(e) {
            if (this.constructor === n) throw new Error("It's abstract class");
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
                    let m;
                    try {
                        const o = await this.ajax(s);
                        o.ok ? m = await this.context.success(o, s) : m = await this.context.error(o, s) || `${o.status} ${o.statusText}`;
                    } catch (o) {
                        console.error(o), m = o;
                    }
                    m ? (this.setError(m), this.showError()) : (this.setError(""), 
                    this.hideError(), this.context.setValue(this.getValue()), this.context.modeElement.hide(), 
                    this.initText()), this.hideLoad();
                } else this.context.setValue(this.getValue()), this.context.modeElement.hide(), 
                this.initText();
                this.context.element.dispatchEvent(new CustomEvent("save", {
                    detail: {
                        DarkEditable: this.context
                    }
                }));
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
            let t = this.context.options.url;
            if (!t) throw new Error("URL is required!");
            if (!this.context.options.id) throw new Error("pk is required!");
            if (!this.context.options.name) throw new Error("Name is required!");
            const s = new FormData();
            if (s.append("id", this.context.options.id), s.append("name", this.context.options.name), 
            s.append("value", e), this.context.options.ajaxOptions?.method === "GET") {
                const o = [];
                s.forEach((a, f) => {
                    o.push(`${f}=${a}`);
                }), t += "?" + o.join("&");
            }
            const m = {
                ...this.context.options.ajaxOptions
            };
            return m.body = s, fetch(t, m);
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
    class p extends n {
        create() {
            const e = this.createElement("input");
            e.type = typeof this.context.options.type == "string" ? this.context.options.type : "text";
            const {
                options: t = {}
            } = this.context;
            e.type = typeof t.type == "string" ? t.type : "text";
            const s = t.attributes || {}, m = [ "step", "min", "max", "minlength", "maxlength", "pattern", "placeholder", "required", "readonly", "disabled", "autocomplete", "autofocus", "name", "value" ];
            for (const [ o, a ] of Object.entries(s)) m.includes(o) && a !== void 0 && e.setAttribute(o, String(a));
            return this.createContainer(e);
        }
    }
    class u extends n {
        create() {
            const e = this.createElement("textarea");
            return this.createContainer(e);
        }
    }
    class x extends n {
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
    class c extends n {
        create() {
            const e = this.createElement("input");
            return e.type = "date", this.createContainer(e);
        }
        initText() {
            const e = this.context.getValue();
            return e === "" ? (this.context.element.innerHTML = this.context.options.emptytext || "", 
            !0) : (this.context.element.innerHTML = e, !1);
        }
        initOptions() {
            this.context.setValue(this.context.getValue());
        }
    }
    class g extends c {
        create() {
            const e = this.createElement("input");
            return e.type = "datetime-local", this.createContainer(e);
        }
        initOptions() {
            this.context.setValue(this.context.getValue());
        }
    }
    class w {
        element;
        options;
        typeElement;
        modeElement;
        constructor(e, t = {}) {
            this.element = e, this.options = {
                ...t
            }, this.init_options(), this.typeElement = this.route_type(), this.typeElement.initOptions(), 
            this.modeElement = this.route_mode(), this.modeElement.init(), this.setValue(this.element.innerHTML), 
            this.init_style(), this.options.disabled && this.disable(), this.element.dispatchEvent(new CustomEvent("init", {
                detail: {
                    DarkEditable: this
                }
            }));
        }
        get_opt(e, t) {
            return this.options[e] = this.element.dataset?.[e] ?? this.options?.[e] ?? t;
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
            this.get_opt("value", this.element.innerHTML), this.get_opt("name", this.element.id), 
            this.get_opt("id", null), this.get_opt("title", ""), this.get_opt("type", "text"), 
            this.get_opt("emptytext", "Empty"), this.get_opt("placeholder", this.element.getAttribute("placeholder")), 
            this.get_opt("mode", "popup"), this.get_opt("url", null), this.get_opt("ajaxOptions", {}), 
            this.options.ajaxOptions = Object.assign({
                method: "POST",
                dataType: "text",
                headers: {
                    RequestVerificationToken: document.querySelector('input[name="__RequestVerificationToken"]')?.value
                }
            }, this.options.ajaxOptions), this.get_opt_bool("send", !0), this.get_opt_bool("disabled", !1), 
            this.get_opt_bool("required", !1), this.get_opt_bool("showbuttons", !0), 
            this.options?.success && typeof this.options?.success == "function" && (this.success = this.options.success), 
            this.options?.error && typeof this.options?.error == "function" && (this.error = this.options.error), 
            this.get_opt("attributes", {}), this.get_opt("popoverOptions", {});
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
                return new d(this);

              case "inline":
                return new b(this);
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
                return new p(this);

              case "textarea":
                return new u(this);

              case "select":
                return new x(this);

              case "date":
                return new c(this);

              case "datetime":
                return new g(this);
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
        getOption(e) {
            return this.options[e] ?? null;
        }
    }
    return w;
}(bootstrap);

var Notify = function(c) {
    "use strict";
    var $ = Object.defineProperty;
    var S = (c, o, d) => o in c ? $(c, o, {
        enumerable: !0,
        configurable: !0,
        writable: !0,
        value: d
    }) : c[o] = d;
    var m = (c, o, d) => S(c, typeof o != "symbol" ? o + "" : o, d);
    var o = document.createElement("style");
    o.textContent = `.animated{-webkit-animation-duration:1s;animation-duration:1s;-webkit-animation-fill-mode:both;animation-fill-mode:both}@media (print),(prefers-reduced-motion: reduce){.animated{-webkit-animation-duration:1ms!important;animation-duration:1ms!important;-webkit-transition-duration:1ms!important;transition-duration:1ms!important;-webkit-animation-iteration-count:1!important;animation-iteration-count:1!important}}@-webkit-keyframes fadeInDown{0%{opacity:0;-webkit-transform:translate3d(0,-100%,0);transform:translate3d(0,-100%,0)}to{opacity:1;-webkit-transform:translate3d(0,0,0);transform:translateZ(0)}}@keyframes fadeInDown{0%{opacity:0;-webkit-transform:translate3d(0,-100%,0);transform:translate3d(0,-100%,0)}to{opacity:1;-webkit-transform:translate3d(0,0,0);transform:translateZ(0)}}.fadeInDown{-webkit-animation-name:fadeInDown;animation-name:fadeInDown}@-webkit-keyframes fadeOutUp{0%{opacity:1}to{opacity:0;-webkit-transform:translate3d(0,-100%,0);transform:translate3d(0,-100%,0)}}@keyframes fadeOutUp{0%{opacity:1}to{opacity:0;-webkit-transform:translate3d(0,-100%,0);transform:translate3d(0,-100%,0)}}.fadeOutUp{-webkit-animation-name:fadeOutUp;animation-name:fadeOutUp}
/*$vite$:1*/`, document.head.appendChild(o);
    function d(n) {
        const t = Object.create(null, {
            [Symbol.toStringTag]: {
                value: "Module"
            }
        });
        if (n) {
            for (const e in n) if (e !== "default") {
                const s = Object.getOwnPropertyDescriptor(n, e);
                Object.defineProperty(t, e, s.get ? s : {
                    enumerable: !0,
                    get: () => n[e]
                });
            }
        }
        return t.default = n, Object.freeze(t);
    }
    const w = d(c);
    class v {
        constructor(t, e) {
            m(this, "$ele", document.createElement("div"));
            m(this, "settings");
            m(this, "_defaults");
            m(this, "animations");
            m(this, "notify");
            const s = {
                element: "body",
                type: "info",
                allow_dismiss: !0,
                allow_duplicates: !0,
                newest_on_top: !0,
                showProgressbar: !1,
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
                onShow: void 0,
                onShown: null,
                onClose: void 0,
                onClosed: null,
                onClick: null,
                icon_type: "class",
                offset: {
                    x: 0,
                    y: 0
                },
                template: [ '<div data-notify="container" class="toast fade m-3" role="alert" aria-live="assertive" aria-atomic="true">', '<div class="toast-header">', '<span data-notify="icon" class="me-2 text-{0}"></span>', '<strong class="me-auto fw-bold" data-notify="title">{1}</strong>', '<button type="button" class="ms-2 mb-1 btn-close" data-bs-dismiss="toast" data-notify="dismiss" aria-label="Close">', "</button>", "</div>", '<div class="toast-body" data-notify="message">', "{2}", '<div class="progress" role="progressbar" data-notify="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100">', '<div class="progress-bar bg-{0}" style="width: 0%;"></div>', "</div>", "</div>" ].join("")
            };
            this.settings = s;
            const i = {
                content: {
                    message: typeof t == "object" ? t.message : t,
                    title: typeof t == "object" && t.title ? t.title : "",
                    icon: typeof t == "object" && t.icon ? t.icon : ""
                }
            };
            e = f({}, i, e), this.settings = f({}, s, e), this._defaults = s, this.animations = {
                start: "webkitAnimationStart oanimationstart MSAnimationStart animationstart",
                end: "webkitAnimationEnd oanimationend MSAnimationEnd animationend"
            }, typeof this.settings.offset == "number" && (this.settings.offset = {
                x: this.settings.offset,
                y: this.settings.offset
            }), (this.settings.allow_duplicates || !this.settings.allow_duplicates && !this.isDuplicateNotification(this)) && this.init();
        }
        isDuplicateNotification(t) {
            let e = !1;
            return document.querySelectorAll('[data-notify="container"]').forEach(s => {
                var h, u, y, p, g, b;
                const i = ((h = s.querySelector('[data-notify="title"]')) == null ? void 0 : h.innerHTML.trim()) ?? "", r = ((u = s.querySelector('[data-notify="message"]')) == null ? void 0 : u.innerHTML.trim()) ?? "", a = i === ((p = (y = t.settings.content) == null ? void 0 : y.title) == null ? void 0 : p.trim()), l = r === ((b = (g = t.settings.content) == null ? void 0 : g.message) == null ? void 0 : b.trim());
                return a && l && (e = !0), !e;
            }), e;
        }
        init() {
            var t = this;
            this.buildNotify(), this.settings.content && this.settings.content.icon && this.setIcon(this.settings.content.icon), 
            this.placement(), this.bind(), this.notify = {
                $ele: this.$ele,
                close() {
                    t.close();
                }
            };
        }
        update(t, e) {
            const s = typeof t == "string" ? {
                [t]: e
            } : t;
            for (const i in s) {
                const r = this.$ele.querySelector(`[data-notify="${i}"]`);
                r && (r.innerHTML = s[i]);
            }
        }
        buildNotify() {
            const t = this.settings.content, e = document.createElement("div");
            if (e.innerHTML = this.formatTemplate(this.settings.template, this.settings.type, t.title, t.message), 
            this.$ele = e.firstChild, this.$ele.dataset.notifyPosition = `${this.settings.placement.from}-${this.settings.placement.align}`, 
            this.$ele.dataset.bsDelay = this.settings.delay.toString(), !this.settings.allow_dismiss) {
                const s = this.$ele.querySelector('[data-notify="dismiss"]');
                s && (s.style.display = "none");
            }
            (this.settings.delay <= 0 && !this.settings.showProgressbar || !this.settings.showProgressbar) && this.$ele.querySelector('[data-notify="progressbar"]') && this.$ele.querySelector('[data-notify="progressbar"]').remove();
        }
        setIcon(t) {
            if (this.settings.icon_type && this.settings.icon_type.toLowerCase() === "class") this.$ele.querySelector('[data-notify="icon"]').className += ` ${t}`; else if (this.$ele.querySelector('[data-notify="icon"]').nodeName === "IMG") {
                const e = this.$ele.querySelector('[data-notify="icon"]');
                e.src = this.settings.content.icon, e.className = "me-2";
            } else {
                const e = document.createElement("img");
                e.src = `${this.settings.content.icon}`, e.alt = "Notify Icon", 
                e.className = "me-2", this.$ele.querySelector('[data-notify="icon"]').append(e);
            }
        }
        placement() {
            const t = this;
            if (this.$ele.className += ` ${this.settings.animate.enter}`, new w.Toast(this.$ele).show(), 
            document.querySelector(".toast-container") == null) {
                const i = document.createElement("div");
                switch (i.className = "toast-container position-fixed", this.settings.placement.from) {
                  case "top":
                    i.className += " top-0";
                    break;

                  case "bottom":
                    i.className += " bottom-0";
                    break;
                }
                switch (this.settings.placement.align) {
                  case "left":
                    i.className += " start-0";
                    break;

                  case "right":
                    i.className += " end-0";
                    break;

                  case "center":
                    i.className += " start-50 translate-middle-x";
                    break;
                }
                document.querySelector(this.settings.element).append(i);
            }
            const s = document.querySelector(".toast-container");
            s && (this.settings.newest_on_top ? s.prepend(this.$ele) : s.append(this.$ele)), 
            typeof t.settings.onShow == "function" && t.settings.onShow.call(this.$ele);
        }
        bind() {
            var t = this;
            const e = this.$ele.querySelector('[data-notify="dismiss"]');
            if (e && e.addEventListener("click", () => {
                t.close();
            }), t.settings.onClick && this.$ele.addEventListener("click", i => {
                i.target !== t.$ele.querySelector('[data-notify="dismiss"]') && t.settings.onClick.call(this);
            }), this.$ele.addEventListener("mouseover", () => {
                this.$ele.dataset.hover = "true";
            }), this.$ele.addEventListener("mouseout", () => {
                this.$ele.dataset.hover = "false";
            }), this.$ele.dataset.hover = "false", this.settings.delay && this.settings.delay > 0) {
                t.$ele.dataset.notifyDelay = t.settings.delay.toString();
                var s = setInterval(() => {
                    const i = this.settings.delay - this.settings.timer;
                    if (this.$ele.dataset.hover === "false" && this.settings.mouse_over === "pause" || this.settings.mouse_over !== "pause") {
                        const r = (this.settings.delay - i) / this.settings.delay * 100;
                        if (this.$ele.dataset.notifyDelay = i.toString(), this.settings.showProgressbar) {
                            const a = this.$ele.querySelector('[data-notify="progressbar"] > div');
                            a.setAttribute("aria-valuenow", r.toString()), a.style.width = r + "%";
                        }
                    }
                    i <= -t.settings.timer && (clearInterval(s), this.close());
                }, t.settings.timer);
            }
        }
        close() {
            const t = this;
            this.$ele.dataset.closing = "true", this.$ele.className = `toast ${this.settings.animate.exit}`, 
            t.settings.onClose && t.settings.onClose.call(this.$ele), t.$ele.remove();
        }
        formatTemplate(...t) {
            return t[0].replace(/(\{\{\d\}\}|\{\d\})/g, s => {
                if (s.substring(0, 2) === "{{") return s;
                const i = parseInt(s.match(/\d/)[0]);
                return t[i + 1];
            });
        }
    }
    function f(...n) {
        const t = {};
        let e = !1, s = 0;
        const i = n.length;
        Object.prototype.toString.call(n[0]) === "[object Boolean]" && (e = n[0], 
        s++);
        const r = a => {
            for (const l in a) Object.prototype.hasOwnProperty.call(a, l) && (e && Object.prototype.toString.call(a[l]) === "[object Object]" ? t[l] = f(!0, t[l], a[l]) : t[l] = a[l]);
        };
        for (;s < i; s++) {
            const a = n[s];
            r(a);
        }
        return t;
    }
    return v;
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
    var getChoiceForOutput = function(choice, keyCode) {
        return {
            id: choice.id,
            highlighted: choice.highlighted,
            labelClass: choice.labelClass,
            labelDescription: unwrapStringForRaw(choice.labelDescription),
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
    var resolveNoticeFunction = function(fn, value, item) {
        return typeof fn === "function" ? fn(sanitise(value), unwrapStringForRaw(value), item) : fn;
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
        Container.prototype.addInvalidState = function() {
            addClassesToElement(this.element, this.classNames.invalidState);
        };
        Container.prototype.removeInvalidState = function() {
            removeClassesFromElement(this.element, this.classNames.invalidState);
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
                labelDescription: typeof option.dataset.labelDescription !== "undefined" ? {
                    trusted: option.dataset.labelDescription
                } : undefined,
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
        button: [ "choices__button", "btn-close" ],
        activeState: [ "is-active" ],
        focusState: [ "is-focused" ],
        openState: [ "is-open" ],
        disabledState: [ "is-disabled" ],
        highlightedState: [ "is-highlighted" ],
        selectedState: [ "is-selected" ],
        flippedState: [ "is-flipped" ],
        loadingState: [ "is-loading" ],
        invalidState: [ "is-invalid" ],
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
        searchDisabledChoices: false,
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
        searchRenderSelectedChoices: false,
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
                var context = this._context;
                return this.choices.filter(function(choice) {
                    return !choice.placeholder && (context.searchDisabledChoices || !choice.disabled);
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
            enumerable: true,
            configurable: true,
            writable: true
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
            r % 2 ? ownKeys(Object(t), true).forEach(function(r) {
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
            var i = e.call(t, r);
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
    function baseToString(value) {
        if (typeof value == "string") {
            return value;
        }
        let result = value + "";
        return result == "0" && 1 / value == -Infinity ? "-0" : result;
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
        ignoreDiacritics: false,
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
    const stripDiacritics = String.prototype.normalize ? str => str.normalize("NFD").replace(/[\u0300-\u036F\u0483-\u0489\u0591-\u05BD\u05BF\u05C1\u05C2\u05C4\u05C5\u05C7\u0610-\u061A\u064B-\u065F\u0670\u06D6-\u06DC\u06DF-\u06E4\u06E7\u06E8\u06EA-\u06ED\u0711\u0730-\u074A\u07A6-\u07B0\u07EB-\u07F3\u07FD\u0816-\u0819\u081B-\u0823\u0825-\u0827\u0829-\u082D\u0859-\u085B\u08D3-\u08E1\u08E3-\u0903\u093A-\u093C\u093E-\u094F\u0951-\u0957\u0962\u0963\u0981-\u0983\u09BC\u09BE-\u09C4\u09C7\u09C8\u09CB-\u09CD\u09D7\u09E2\u09E3\u09FE\u0A01-\u0A03\u0A3C\u0A3E-\u0A42\u0A47\u0A48\u0A4B-\u0A4D\u0A51\u0A70\u0A71\u0A75\u0A81-\u0A83\u0ABC\u0ABE-\u0AC5\u0AC7-\u0AC9\u0ACB-\u0ACD\u0AE2\u0AE3\u0AFA-\u0AFF\u0B01-\u0B03\u0B3C\u0B3E-\u0B44\u0B47\u0B48\u0B4B-\u0B4D\u0B56\u0B57\u0B62\u0B63\u0B82\u0BBE-\u0BC2\u0BC6-\u0BC8\u0BCA-\u0BCD\u0BD7\u0C00-\u0C04\u0C3E-\u0C44\u0C46-\u0C48\u0C4A-\u0C4D\u0C55\u0C56\u0C62\u0C63\u0C81-\u0C83\u0CBC\u0CBE-\u0CC4\u0CC6-\u0CC8\u0CCA-\u0CCD\u0CD5\u0CD6\u0CE2\u0CE3\u0D00-\u0D03\u0D3B\u0D3C\u0D3E-\u0D44\u0D46-\u0D48\u0D4A-\u0D4D\u0D57\u0D62\u0D63\u0D82\u0D83\u0DCA\u0DCF-\u0DD4\u0DD6\u0DD8-\u0DDF\u0DF2\u0DF3\u0E31\u0E34-\u0E3A\u0E47-\u0E4E\u0EB1\u0EB4-\u0EB9\u0EBB\u0EBC\u0EC8-\u0ECD\u0F18\u0F19\u0F35\u0F37\u0F39\u0F3E\u0F3F\u0F71-\u0F84\u0F86\u0F87\u0F8D-\u0F97\u0F99-\u0FBC\u0FC6\u102B-\u103E\u1056-\u1059\u105E-\u1060\u1062-\u1064\u1067-\u106D\u1071-\u1074\u1082-\u108D\u108F\u109A-\u109D\u135D-\u135F\u1712-\u1714\u1732-\u1734\u1752\u1753\u1772\u1773\u17B4-\u17D3\u17DD\u180B-\u180D\u1885\u1886\u18A9\u1920-\u192B\u1930-\u193B\u1A17-\u1A1B\u1A55-\u1A5E\u1A60-\u1A7C\u1A7F\u1AB0-\u1ABE\u1B00-\u1B04\u1B34-\u1B44\u1B6B-\u1B73\u1B80-\u1B82\u1BA1-\u1BAD\u1BE6-\u1BF3\u1C24-\u1C37\u1CD0-\u1CD2\u1CD4-\u1CE8\u1CED\u1CF2-\u1CF4\u1CF7-\u1CF9\u1DC0-\u1DF9\u1DFB-\u1DFF\u20D0-\u20F0\u2CEF-\u2CF1\u2D7F\u2DE0-\u2DFF\u302A-\u302F\u3099\u309A\uA66F-\uA672\uA674-\uA67D\uA69E\uA69F\uA6F0\uA6F1\uA802\uA806\uA80B\uA823-\uA827\uA880\uA881\uA8B4-\uA8C5\uA8E0-\uA8F1\uA8FF\uA926-\uA92D\uA947-\uA953\uA980-\uA983\uA9B3-\uA9C0\uA9E5\uAA29-\uAA36\uAA43\uAA4C\uAA4D\uAA7B-\uAA7D\uAAB0\uAAB2-\uAAB4\uAAB7\uAAB8\uAABE\uAABF\uAAC1\uAAEB-\uAAEF\uAAF5\uAAF6\uABE3-\uABEA\uABEC\uABED\uFB1E\uFE00-\uFE0F\uFE20-\uFE2F]/g, "") : str => str;
    class BitapSearch {
        constructor(pattern, {
            location = Config.location,
            threshold = Config.threshold,
            distance = Config.distance,
            includeMatches = Config.includeMatches,
            findAllMatches = Config.findAllMatches,
            minMatchCharLength = Config.minMatchCharLength,
            isCaseSensitive = Config.isCaseSensitive,
            ignoreDiacritics = Config.ignoreDiacritics,
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
                ignoreDiacritics: ignoreDiacritics,
                ignoreLocation: ignoreLocation
            };
            pattern = isCaseSensitive ? pattern : pattern.toLowerCase();
            pattern = ignoreDiacritics ? stripDiacritics(pattern) : pattern;
            this.pattern = pattern;
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
                ignoreDiacritics,
                includeMatches
            } = this.options;
            text = isCaseSensitive ? text : text.toLowerCase();
            text = ignoreDiacritics ? stripDiacritics(text) : text;
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
            ignoreDiacritics = Config.ignoreDiacritics,
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
                ignoreDiacritics: ignoreDiacritics,
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
            ignoreDiacritics = Config.ignoreDiacritics,
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
                ignoreDiacritics: ignoreDiacritics,
                includeMatches: includeMatches,
                minMatchCharLength: minMatchCharLength,
                findAllMatches: findAllMatches,
                ignoreLocation: ignoreLocation,
                location: location,
                threshold: threshold,
                distance: distance
            };
            pattern = isCaseSensitive ? pattern : pattern.toLowerCase();
            pattern = ignoreDiacritics ? stripDiacritics(pattern) : pattern;
            this.pattern = pattern;
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
                isCaseSensitive,
                ignoreDiacritics
            } = this.options;
            text = isCaseSensitive ? text : text.toLowerCase();
            text = ignoreDiacritics ? stripDiacritics(text) : text;
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
            if (this.options.useExtendedSearch && false);
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
    Fuse.version = "7.1.0";
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
            dataset.labelDescription = unwrapStringForRaw(labelDescription);
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
                var eventChoice = getChoiceForOutput(choice);
                setElementHtml(removeButton, true, resolveNoticeFunction(removeItemIconText, choice.value, eventChoice));
                var REMOVE_ITEM_LABEL = resolveNoticeFunction(removeItemLabelText, choice.value, eventChoice);
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
                div.setAttribute("aria-selected", choice.selected ? "true" : "false");
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
            inp.name = "search";
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
            config.searchEnabled = !isText && config.searchEnabled;
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
            this._onChange = this._onChange.bind(this);
            this._onInvalid = this._onInvalid.bind(this);
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
                this.passedElement.triggerEvent(EventType.highlightItem, getChoiceForOutput(choice));
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
                this.passedElement.triggerEvent(EventType.unhighlightItem, getChoiceForOutput(choice));
            }
            return this;
        };
        Choices.prototype.highlightAll = function() {
            var _this = this;
            this._store.withTxn(function() {
                _this._store.items.forEach(function(item) {
                    if (!item.highlighted) {
                        _this._store.dispatch(highlightItem(item, true));
                        _this.passedElement.triggerEvent(EventType.highlightItem, getChoiceForOutput(item));
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
                        _this.passedElement.triggerEvent(EventType.highlightItem, getChoiceForOutput(item));
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
                var activeElement = _this.choiceList.element.querySelector(getClassNamesSelector(_this.config.classNames.selectedState));
                if (activeElement !== null && !isScrolledIntoView(activeElement, _this.choiceList.element)) {
                    activeElement.scrollIntoView();
                }
            });
            return this;
        };
        Choices.prototype.hideDropdown = function(preventInputBlur) {
            var _this = this;
            if (!this.dropdown.isActive) {
                return this;
            }
            this._removeHighlightedChoices();
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
            var values = this._store.items.map(function(item) {
                return valueOnly ? item.value : getChoiceForOutput(item);
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
                this.passedElement.triggerEvent(EventType.removeItem, getChoiceForOutput(choice));
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
            var renderLimit = isSearching ? config.searchResultLimit : config.renderChoiceLimit;
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
                    return !choice.placeholder && (isSearching ? (config.searchRenderSelectedChoices || !choice.selected) && !!choice.rank : config.renderSelectedChoices || !choice.selected);
                });
            };
            var showLabel = config.appendGroupInSearch && isSearching;
            var selectableChoices = false;
            var highlightedEl = null;
            var renderChoices = function(choices, withinGroup) {
                if (isSearching) {
                    choices.sort(sortByRank);
                } else if (config.shouldSort) {
                    choices.sort(config.sorter);
                }
                var choiceLimit = choices.length;
                choiceLimit = !withinGroup && renderLimit > 0 && choiceLimit > renderLimit ? renderLimit : choiceLimit;
                choiceLimit--;
                choices.every(function(choice, index) {
                    var dropdownItem = choice.choiceEl || _this._templates.choice(config, choice, config.itemSelectText, showLabel && choice.group ? choice.group.label : undefined);
                    choice.choiceEl = dropdownItem;
                    fragment.appendChild(dropdownItem);
                    if (isSearching || !choice.selected) {
                        selectableChoices = true;
                    } else if (!highlightedEl) {
                        highlightedEl = dropdownItem;
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
                    }), false);
                }
                if (activeGroups.length && !isSearching) {
                    if (config.shouldSort) {
                        activeGroups.sort(config.sorter);
                    }
                    renderChoices(activeChoices.filter(function(choice) {
                        return !choice.placeholder && !choice.group;
                    }), false);
                    activeGroups.forEach(function(group) {
                        var groupChoices = renderableChoices(group.choices);
                        if (groupChoices.length) {
                            if (group.label) {
                                var dropdownGroup = group.groupEl || _this._templates.choiceGroup(_this.config, group);
                                group.groupEl = dropdownGroup;
                                dropdownGroup.remove();
                                fragment.appendChild(dropdownGroup);
                            }
                            renderChoices(groupChoices, true);
                        }
                    });
                } else {
                    renderChoices(renderableChoices(activeChoices), false);
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
            this._highlightChoice(highlightedEl);
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
            return getChoiceForOutput(choice, keyCode);
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
            var id = element && parseDataSetId(element.closest("[data-id]"));
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
                notice = resolveNoticeFunction(config.customAddItemText, value, undefined);
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
                        notice = resolveNoticeFunction(config.uniqueItemText, value, undefined);
                    }
                }
            }
            if (canAddItem) {
                notice = resolveNoticeFunction(config.addItemText, value, undefined);
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
            var passedElement = this.passedElement.element;
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
            if (passedElement.hasAttribute("required")) {
                passedElement.addEventListener("change", this._onChange, {
                    passive: true
                });
                passedElement.addEventListener("invalid", this._onInvalid, {
                    passive: true
                });
            }
            this.input.addEventListeners();
        };
        Choices.prototype._removeEventListeners = function() {
            var documentElement = this._docRoot;
            var outerElement = this.containerOuter.element;
            var inputElement = this.input.element;
            var passedElement = this.passedElement.element;
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
            if (passedElement.hasAttribute("required")) {
                passedElement.removeEventListener("change", this._onChange);
                passedElement.removeEventListener("invalid", this._onInvalid);
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
            if (!(target instanceof Element)) {
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
                    if (!this.config.searchEnabled) {
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
        Choices.prototype._onChange = function(event) {
            if (!event.target.checkValidity()) {
                return;
            }
            this.containerOuter.removeInvalidState();
        };
        Choices.prototype._onInvalid = function() {
            this.containerOuter.addInvalidState();
        };
        Choices.prototype._removeHighlightedChoices = function() {
            var highlightedState = this.config.classNames.highlightedState;
            var highlightedChoices = Array.from(this.dropdown.element.querySelectorAll(getClassNamesSelector(highlightedState)));
            highlightedChoices.forEach(function(choice) {
                removeClassesFromElement(choice, highlightedState);
                choice.setAttribute("aria-selected", "false");
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
            this._removeHighlightedChoices();
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
                var eventChoice = getChoiceForOutput(item);
                this.passedElement.triggerEvent(EventType.addItem, eventChoice);
                if (userTriggered) {
                    this.passedElement.triggerEvent(EventType.choice, eventChoice);
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
            this.passedElement.triggerEvent(EventType.removeItem, getChoiceForOutput(item));
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
            containerOuter.element.appendChild(containerInner.element);
            containerOuter.element.appendChild(dropdownElement);
            containerInner.element.appendChild(this.itemList.element);
            dropdownElement.appendChild(this.choiceList.element);
            if (this._isSelectOneElement) {
                this.input.placeholder = this.config.searchPlaceholderValue || "";
                if (this.config.searchEnabled) {
                    dropdownElement.insertBefore(this.input.element, dropdownElement.firstChild);
                }
            } else {
                if (!this._isSelectMultipleElement || this.config.searchEnabled) {
                    containerInner.element.appendChild(this.input.element);
                }
                if (this._placeholderValue) {
                    this.input.placeholder = this._placeholderValue;
                }
                this.input.setWidth();
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
        Choices.version = "11.2.0";
        return Choices;
    }();
    return Choices;
});

var lightbox = function(y) {
    "use strict";
    function f(l) {
        const t = Object.create(null, {
            [Symbol.toStringTag]: {
                value: "Module"
            }
        });
        if (l) {
            for (const e in l) if (e !== "default") {
                const a = Object.getOwnPropertyDescriptor(l, e);
                Object.defineProperty(t, e, a.get ? a : {
                    enumerable: !0,
                    get: () => l[e]
                });
            }
        }
        return t.default = l, Object.freeze(t);
    }
    const r = f(y);
    class i {
        hash;
        settings;
        modalOptions;
        carouselOptions;
        el;
        src;
        sources;
        type;
        carouselElement;
        modalElement;
        modal;
        carousel;
        static allowedEmbedTypes = [ "embed", "youtube", "vimeo", "instagram", "url" ];
        static allowedMediaTypes = [ ...i.allowedEmbedTypes, "image", "html" ];
        static defaultSelector = '[data-toggle="lightbox"]';
        constructor(t, e = {}) {
            this.hash = this.randomHash(), this.settings = Object.assign({}, r.Modal.Default, r.Carousel.Default, {
                interval: !1,
                target: '[data-toggle="lightbox"]',
                gallery: "",
                size: "xl",
                constrain: !0
            }, e), this.modalOptions = this.setOptionsFromSettings(r.Modal.Default), 
            this.carouselOptions = this.setOptionsFromSettings(r.Carousel.Default), 
            typeof t == "string" && (this.settings.target = t, t = document.querySelector(t)), 
            this.el = t, this.type = t.dataset.type || "", t.dataset.size && (this.settings.size = t.dataset.size), 
            this.src = this.getSrc(t), this.sources = this.getGalleryItems(), this.createCarousel(), 
            this.createModal();
        }
        show() {
            document.body.appendChild(this.modalElement), this.modal.show();
        }
        hide() {
            this.modal.hide();
        }
        setOptionsFromSettings(t) {
            return Object.keys(t).reduce((e, a) => Object.assign(e, {
                [a]: this.settings[a]
            }), {});
        }
        getSrc(t) {
            let e = t.dataset.src || t.dataset.remote || t.href || "https://placehold.co/1600x900";
            if (t.dataset.type === "html" || t.dataset.type === "image") return e;
            /https?:\/\//.test(e) || (e = window.location.origin + e);
            const a = new URL(e);
            return (t.dataset.footer || t.dataset.caption) && a.searchParams.set("caption", t.dataset.footer || t.dataset.caption || ""), 
            a.toString();
        }
        getGalleryItems() {
            let t;
            if (this.settings.gallery) {
                if (Array.isArray(this.settings.gallery)) return this.settings.gallery;
                t = this.settings.gallery;
            } else this.el.dataset.gallery && (t = this.el.dataset.gallery);
            return t ? [ ...new Set(Array.from(document.querySelectorAll(`[data-gallery="${t}"]`), a => {
                const o = a;
                return `${o.dataset.type || ""}${this.getSrc(o)}`;
            })) ] : [ `${this.type || ""}${this.src}` ];
        }
        getYoutubeId(t) {
            const e = t.match(/^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/);
            return e && e[2].length === 11 ? e[2] : !1;
        }
        getYoutubeLink(t) {
            const e = this.getYoutubeId(t);
            if (!e) return !1;
            const a = t.split("?"), o = a.length > 1 ? `?${a[1]}` : "";
            return `https://www.youtube.com/embed/${e}${o}`;
        }
        getInstagramEmbed(t) {
            if (/instagram/.test(t)) return t += /\/embed$/.test(t) ? "" : "/embed", 
            `<iframe src="${t}" class="start-50 translate-middle-x" style="max-width: 500px" frameborder="0" scrolling="no" allowtransparency="true"></iframe>`;
        }
        isEmbed(t) {
            const a = new RegExp(`(${i.allowedEmbedTypes.join("|")})`).test(t), o = /\.(png|jpe?g|gif|svg|webp)/i.test(t) || this.el.dataset.type === "image";
            return a || !o;
        }
        createCarousel() {
            const t = document.createElement("template"), e = i.allowedMediaTypes.join("|"), a = this.sources.map((s, n) => {
                s = s.replace(/\/$/, "");
                const h = new RegExp(`^(${e})`, "i"), x = /^html/.test(s), E = /^image/.test(s);
                h.test(s) && (s = s.replace(h, ""));
                const S = this.settings.constrain ? "mw-100 mh-100 h-auto w-auto m-auto top-0 end-0 bottom-0 start-0" : "h-100 w-100", u = new URLSearchParams(s.split("?")[1]);
                let m = "", d = s;
                if (u.get("caption")) try {
                    let b = new URL(s);
                    b.searchParams.delete("caption"), d = b.toString(), m = `<div class="carousel-caption d-none d-md-block" style="z-index:2"><p class="bg-secondary rounded">${u.get("caption")}</p></div>`;
                } catch {
                    d = s;
                }
                let c = `<img src="${d}" class="d-block ${S} img-fluid" style="z-index: 1; object-fit: contain;" />`, p = "";
                const C = this.getInstagramEmbed(s), g = this.getYoutubeLink(s);
                return this.isEmbed(s) && !E && (g && (s = g, p = 'title="YouTube video player" frameborder="0" allow="accelerometer autoplay clipboard-write encrypted-media gyroscope picture-in-picture"'), 
                c = C || `<img src="${s}" ${p} class="d-block mw-100 mh-100 h-auto w-auto m-auto top-0 end-0 bottom-0 start-0 img-fluid" style="z-index: 1; object-fit: contain;" />`), 
                x && (c = s), `
          <div class="carousel-item ${n ? "" : "active"}" style="min-height: 100px">
            <div class="position-absolute top-50 start-50 translate-middle text-white"><div class="spinner-border" style="width: 3rem; height: 3rem" role="status"></div></div>
            <div class="ratio ratio-16x9" style="background-color: #000;">${c}</div>
            ${m}
          </div>`;
            }).join(""), o = this.sources.length < 2 ? "" : `
        <button id="#lightboxCarousel-${this.hash}-prev" class="carousel-control-prev" type="button" data-bs-target="#lightboxCarousel-${this.hash}" data-bs-slide="prev">
          <span class="btn btn-primary carousel-control-prev-icon" aria-hidden="true"></span>
          <span class="visually-hidden">Previous</span>
        </button>
        <button id="#lightboxCarousel-${this.hash}-next" class="carousel-control-next" type="button" data-bs-target="#lightboxCarousel-${this.hash}" data-bs-slide="next">
          <span class="btn btn-primary carousel-control-next-icon" aria-hidden="true"></span>
          <span class="visually-hidden">Next</span>
        </button>`, v = `
      <div class="carousel-indicators" style="bottom: -40px">
        ${this.sources.map((s, n) => `
            <button type="button" data-bs-target="#lightboxCarousel-${this.hash}" data-bs-slide-to="${n}" class="${n === 0 ? "active" : ""}" aria-current="${n === 0 ? "true" : "false"}" aria-label="Slide ${n + 1}"></button>`).join("")}
      </div>`;
            t.innerHTML = `
      <div id="lightboxCarousel-${this.hash}" class="lightbox-carousel carousel slide" data-bs-ride="carousel" data-bs-interval="${this.carouselOptions.interval}">
        <div class="carousel-inner">${a}</div>
        ${v}
        ${o}
      </div>`.trim(), this.carouselElement = t.content.firstChild;
            const w = {
                ...this.carouselOptions,
                keyboard: !1
            };
            this.carousel = new r.Carousel(this.carouselElement, w);
            const $ = this.type && this.type !== "image" ? this.type + this.src : this.src;
            return this.carousel.to(this.findGalleryItemIndex(this.sources, $)), 
            this.carouselOptions.keyboard === !0 && document.addEventListener("keydown", s => {
                if (s.code === "ArrowLeft") return document.getElementById(`#lightboxCarousel-${this.hash}-prev`)?.click(), 
                !1;
                if (s.code === "ArrowRight") return document.getElementById(`#lightboxCarousel-${this.hash}-next`)?.click(), 
                !1;
            }), this.carousel;
        }
        findGalleryItemIndex(t, e) {
            return t.findIndex(a => a.includes(e)) || 0;
        }
        createModal() {
            const t = document.createElement("template");
            return t.innerHTML = `
      <div class="modal lightbox fade" id="lightboxModal-${this.hash}" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-${this.settings.size}">
          <div class="modal-content border-0 bg-transparent">
            <div class="modal-body p-0">
              <button type="button" class="btn-close position-absolute p-3" data-bs-dismiss="modal" aria-label="Close" style="top: -15px;right:-40px"></button>
            </div>
          </div>
        </div>
      </div>`.trim(), this.modalElement = t.content.firstChild, this.modalElement.querySelector(".modal-body")?.appendChild(this.carouselElement), 
            this.modalElement.addEventListener("hidden.bs.modal", () => this.modalElement.remove()), 
            this.modalElement.querySelector("[data-bs-dismiss]")?.addEventListener("click", () => this.modal.hide()), 
            this.modal = new r.Modal(this.modalElement, this.modalOptions), this.modal;
        }
        randomHash(t = 8) {
            return Array.from({
                length: t
            }, () => Math.floor(Math.random() * 36).toString(36)).join("");
        }
        static initialize(t) {
            t.preventDefault(), new i(this).show();
        }
    }
    return document.querySelectorAll(i.defaultSelector).forEach(l => l.addEventListener("click", i.initialize)), 
    typeof window < "u" && window.bootstrap && (window.bootstrap.Lightbox = i), 
    i;
}(bootstrap);

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
                if (document.currentScript && document.currentScript.tagName === "SCRIPT" && 1 < 2) {
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

Prism.languages.json = {
    property: {
        pattern: /(^|[^\\])"(?:\\.|[^\\"\r\n])*"(?=\s*:)/,
        lookbehind: true,
        greedy: true
    },
    string: {
        pattern: /(^|[^\\])"(?:\\.|[^\\"\r\n])*"(?!\s*:)/,
        lookbehind: true,
        greedy: true
    },
    comment: {
        pattern: /\/\/.*|\/\*[\s\S]*?(?:\*\/|$)/,
        greedy: true
    },
    number: /-?\b\d+(?:\.\d+)?(?:e[+-]?\d+)?\b/i,
    punctuation: /[{}[\],]/,
    operator: /:/,
    boolean: /\b(?:false|true)\b/,
    null: {
        pattern: /\bnull\b/,
        alias: "keyword"
    }
};

Prism.languages.webmanifest = Prism.languages.json;

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
                        <i class="fas fa-tag align-middle me-1"></i>${String(label)}
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
                                 <div class="${String(classNames.item)} ${String(data.highlighted ? classNames.highlightedState : classNames.itemSelectable)} ${String(data.placeholder ? classNames.placeholder : "")}"
                                      data-item data-id="${String(data.id)}" data-value="${String(data.value)}"
                                      ${String(data.active ? 'aria-selected="true"' : "")} ${String(data.disabled ? 'aria-disabled="true"' : "")}>
                                    <span><i class="fas fa-comments text-secondary me-1"></i>${String(data.label)}</span>
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
                                      <span><i class="fas fa-comments text-secondary me-1"></i>${String(data.label)}</span>
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
                          <span><i class="fas fa-folder text-warning me-1"></i>${String(data.label)}</span>
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
    const placeHolder = document.getElementById("PostAlbumsListPlaceholder"), list = placeHolder.querySelector(".AlbumsList"), yafUserId = placeHolder.dataset.userid, pagedResults = {}, ajaxUrl = placeHolder.dataset.url + "api/Album/GetAlbumImages";
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
        list.innerHTML = "";
        document.getElementById("PostAlbumsLoader").style.display = "none";
        data.AttachmentList.forEach(dataItem => {
            var li = document.createElement("div");
            li.classList.add("col-6");
            li.classList.add("col-sm-4");
            li.classList.add("p-1");
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

function getPaginationData(pageSize, pageNumber, isPageChange) {
    const placeHolder = document.getElementById("PostAttachmentListPlaceholder"), list = placeHolder.querySelector(".AttachmentList"), yafUserId = placeHolder.dataset.userid, pagedResults = {}, ajaxUrl = placeHolder.dataset.url + "api/Attachment/GetAttachments";
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
        list.innerHTML = "";
        document.getElementById("PostAttachmentLoader").style.display = "none";
        if (data.AttachmentList.length === 0) {
            const li = document.createElement("div");
            li.classList.add("col");
            const noText = placeHolder.dataset.notext;
            const noAttachmentsText = noText || "";
            li.innerHTML = `<div class="alert alert-info text-break" role="alert">${noAttachmentsText}</div>`;
            list.appendChild(li);
        }
        data.AttachmentList.forEach(dataItem => {
            var li = document.createElement("div");
            li.classList.add("col-6");
            li.classList.add("col-sm-4");
            li.style.cursor = "pointer";
            li.setAttribute("onclick", dataItem.OnClick);
            li.innerHTML = dataItem.IconImage;
            list.appendChild(li);
        });
        renderAttachPreview(".attachments-preview");
        setPageNumber(pageSize, pageNumber, data.TotalRecords, document.getElementById("AttachmentsListPager"), "Attachments", "getPaginationData");
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
                    item.innerHTML = `<div class="row"><div class="col"><div class="card border-0 w-100 mb-3"><div class="card-header bg-transparent border-top border-bottom-0 px-0 pb-0 pt-4 topicTitle"><h5> <a title="${topic}" href="${dataItem.TopicUrl}">${dataItem.Topic}</a>&nbsp;<a title="${lastPost}" href="${dataItem.MessageUrl}"><i class="fas fa-external-link-alt"></i></a> <small class="text-body-secondary">(<a href="${dataItem.ForumUrl}">${dataItem.ForumName}</a>)</small></h5></div><div class="card-body px-0"><h6 class="card-subtitle mb-2 text-body-secondary">${dataItem.Description}</h6><p class="card-text messageContent">${dataItem.Message}</p></div><div class="card-footer bg-transparent border-top-0 px-0 py-2"> <small class="text-body-secondary"><span class="fa-stack"><i class="fa fa-calendar-day fa-stack-1x text-secondary"></i><i class="fa fa-clock fa-badge text-secondary"></i> </span>${posted} ${dataItem.Posted} <i class="fa fa-user text-secondary"></i>${by} ${useDisplayName ? dataItem.UserDisplayName : dataItem.UserName}${tags}</small> </div></div></div></div>`;
                    placeHolder.appendChild(item);
                });
                setSearchPageNumber(pageSize, pageNumber, data.TotalRecords);
                for (const el of document.querySelectorAll('[data-toggle="lightbox"]')) {
                    const lightBox = window.bootstrap.Lightbox;
                    el.addEventListener("click", lightBox.initialize);
                }
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
                        list.classList.add("list-group", "list-similar");
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
    const loginButton = document.querySelector("a.btn-login,input.btn-login, .btn-spinner");
    if (loginButton) {
        loginButton.addEventListener("click", () => {
            loginButton.innerHTML = "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";
            loginButton.classList.add("disabled");
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
                                 <div class="${String(classNames.item)} ${String(data.highlighted ? classNames.highlightedState : classNames.itemSelectable)} ${String(data.placeholder ? classNames.placeholder : "")}"
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
                            link.innerHTML = `<i class="fas fa-quote-left"></i>&nbsp;${contextMenu.dataset.quote}`;
                            contextMenu.appendChild(link);
                        }
                        const linkSearch = document.createElement("a");
                        linkSearch.classList.add("dropdown-item");
                        linkSearch.classList.add("item-search");
                        linkSearch.href = `javascript:copyToClipBoard('${selectedText}')`;
                        linkSearch.innerHTML = `<i class="fas fa-clipboard"></i>&nbsp;${contextMenu.dataset.copy}`;
                        contextMenu.appendChild(linkSearch);
                        const divider = document.createElement("div");
                        divider.classList.add("dropdown-divider");
                        divider.classList.add("selected-divider");
                        contextMenu.appendChild(linkSearch);
                        const linkSelected = document.createElement("a");
                        linkSelected.classList.add("dropdown-item");
                        linkSelected.classList.add("item-search");
                        linkSelected.href = `javascript:searchText('${selectedText}')`;
                        linkSelected.innerHTML = `<i class="fas fa-search"></i>&nbsp;${contextMenu.dataset.search} "${selectedText}"`;
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
                        link.innerHTML = `<i class="fas fa-quote-left"></i>&nbsp;${contextMenu.dataset.quote}`;
                        contextMenu.appendChild(link);
                    }
                    const linkSearch = document.createElement("a");
                    linkSearch.classList.add("dropdown-item");
                    linkSearch.classList.add("item-search");
                    linkSearch.href = `javascript:copyToClipBoard('${selectedText}')`;
                    linkSearch.innerHTML = `<i class="fas fa-clipboard"></i>&nbsp;${contextMenu.dataset.copy}`;
                    contextMenu.appendChild(linkSearch);
                    const divider = document.createElement("div");
                    divider.classList.add("dropdown-divider");
                    divider.classList.add("selected-divider");
                    contextMenu.appendChild(linkSearch);
                    const linkSelected = document.createElement("a");
                    linkSelected.classList.add("dropdown-item");
                    linkSelected.classList.add("item-search");
                    linkSelected.href = `javascript:searchText('${selectedText}')`;
                    linkSelected.innerHTML = `<i class="fas fa-search"></i>&nbsp;${contextMenu.dataset.search} "${selectedText}"`;
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