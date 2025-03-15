(function(global, factory) {
    typeof exports === "object" && typeof module !== "undefined" ? module.exports = factory() : typeof define === "function" && define.amd ? define(factory) : (global = typeof globalThis !== "undefined" ? globalThis : global || self, 
    global.bootstrap = factory());
})(this, function() {
    "use strict";
    const elementMap = new Map();
    const Data = {
        set(element, key, instance) {
            if (!elementMap.has(element)) {
                elementMap.set(element, new Map());
            }
            const instanceMap = elementMap.get(element);
            if (!instanceMap.has(key) && instanceMap.size !== 0) {
                console.error(`Bootstrap doesn't allow more than one instance per element. Bound instance: ${Array.from(instanceMap.keys())[0]}.`);
                return;
            }
            instanceMap.set(key, instance);
        },
        get(element, key) {
            if (elementMap.has(element)) {
                return elementMap.get(element).get(key) || null;
            }
            return null;
        },
        remove(element, key) {
            if (!elementMap.has(element)) {
                return;
            }
            const instanceMap = elementMap.get(element);
            instanceMap.delete(key);
            if (instanceMap.size === 0) {
                elementMap.delete(element);
            }
        }
    };
    const MAX_UID = 1e6;
    const MILLISECONDS_MULTIPLIER = 1e3;
    const TRANSITION_END = "transitionend";
    const parseSelector = selector => {
        if (selector && window.CSS && window.CSS.escape) {
            selector = selector.replace(/#([^\s"#']+)/g, (match, id) => `#${CSS.escape(id)}`);
        }
        return selector;
    };
    const toType = object => {
        if (object === null || object === undefined) {
            return `${object}`;
        }
        return Object.prototype.toString.call(object).match(/\s([a-z]+)/i)[1].toLowerCase();
    };
    const getUID = prefix => {
        do {
            prefix += Math.floor(Math.random() * MAX_UID);
        } while (document.getElementById(prefix));
        return prefix;
    };
    const getTransitionDurationFromElement = element => {
        if (!element) {
            return 0;
        }
        let {
            transitionDuration,
            transitionDelay
        } = window.getComputedStyle(element);
        const floatTransitionDuration = Number.parseFloat(transitionDuration);
        const floatTransitionDelay = Number.parseFloat(transitionDelay);
        if (!floatTransitionDuration && !floatTransitionDelay) {
            return 0;
        }
        transitionDuration = transitionDuration.split(",")[0];
        transitionDelay = transitionDelay.split(",")[0];
        return (Number.parseFloat(transitionDuration) + Number.parseFloat(transitionDelay)) * MILLISECONDS_MULTIPLIER;
    };
    const triggerTransitionEnd = element => {
        element.dispatchEvent(new Event(TRANSITION_END));
    };
    const isElement$1 = object => {
        if (!object || typeof object !== "object") {
            return false;
        }
        if (typeof object.jquery !== "undefined") {
            object = object[0];
        }
        return typeof object.nodeType !== "undefined";
    };
    const getElement = object => {
        if (isElement$1(object)) {
            return object.jquery ? object[0] : object;
        }
        if (typeof object === "string" && object.length > 0) {
            return document.querySelector(parseSelector(object));
        }
        return null;
    };
    const isVisible = element => {
        if (!isElement$1(element) || element.getClientRects().length === 0) {
            return false;
        }
        const elementIsVisible = getComputedStyle(element).getPropertyValue("visibility") === "visible";
        const closedDetails = element.closest("details:not([open])");
        if (!closedDetails) {
            return elementIsVisible;
        }
        if (closedDetails !== element) {
            const summary = element.closest("summary");
            if (summary && summary.parentNode !== closedDetails) {
                return false;
            }
            if (summary === null) {
                return false;
            }
        }
        return elementIsVisible;
    };
    const isDisabled = element => {
        if (!element || element.nodeType !== Node.ELEMENT_NODE) {
            return true;
        }
        if (element.classList.contains("disabled")) {
            return true;
        }
        if (typeof element.disabled !== "undefined") {
            return element.disabled;
        }
        return element.hasAttribute("disabled") && element.getAttribute("disabled") !== "false";
    };
    const findShadowRoot = element => {
        if (!document.documentElement.attachShadow) {
            return null;
        }
        if (typeof element.getRootNode === "function") {
            const root = element.getRootNode();
            return root instanceof ShadowRoot ? root : null;
        }
        if (element instanceof ShadowRoot) {
            return element;
        }
        if (!element.parentNode) {
            return null;
        }
        return findShadowRoot(element.parentNode);
    };
    const noop = () => {};
    const reflow = element => {
        element.offsetHeight;
    };
    const getjQuery = () => {
        if (window.jQuery && !document.body.hasAttribute("data-bs-no-jquery")) {
            return window.jQuery;
        }
        return null;
    };
    const DOMContentLoadedCallbacks = [];
    const onDOMContentLoaded = callback => {
        if (document.readyState === "loading") {
            if (!DOMContentLoadedCallbacks.length) {
                document.addEventListener("DOMContentLoaded", () => {
                    for (const callback of DOMContentLoadedCallbacks) {
                        callback();
                    }
                });
            }
            DOMContentLoadedCallbacks.push(callback);
        } else {
            callback();
        }
    };
    const isRTL = () => document.documentElement.dir === "rtl";
    const defineJQueryPlugin = plugin => {
        onDOMContentLoaded(() => {
            const $ = getjQuery();
            if ($) {
                const name = plugin.NAME;
                const JQUERY_NO_CONFLICT = $.fn[name];
                $.fn[name] = plugin.jQueryInterface;
                $.fn[name].Constructor = plugin;
                $.fn[name].noConflict = () => {
                    $.fn[name] = JQUERY_NO_CONFLICT;
                    return plugin.jQueryInterface;
                };
            }
        });
    };
    const execute = (possibleCallback, args = [], defaultValue = possibleCallback) => {
        return typeof possibleCallback === "function" ? possibleCallback(...args) : defaultValue;
    };
    const executeAfterTransition = (callback, transitionElement, waitForTransition = true) => {
        if (!waitForTransition) {
            execute(callback);
            return;
        }
        const durationPadding = 5;
        const emulatedDuration = getTransitionDurationFromElement(transitionElement) + durationPadding;
        let called = false;
        const handler = ({
            target
        }) => {
            if (target !== transitionElement) {
                return;
            }
            called = true;
            transitionElement.removeEventListener(TRANSITION_END, handler);
            execute(callback);
        };
        transitionElement.addEventListener(TRANSITION_END, handler);
        setTimeout(() => {
            if (!called) {
                triggerTransitionEnd(transitionElement);
            }
        }, emulatedDuration);
    };
    const getNextActiveElement = (list, activeElement, shouldGetNext, isCycleAllowed) => {
        const listLength = list.length;
        let index = list.indexOf(activeElement);
        if (index === -1) {
            return !shouldGetNext && isCycleAllowed ? list[listLength - 1] : list[0];
        }
        index += shouldGetNext ? 1 : -1;
        if (isCycleAllowed) {
            index = (index + listLength) % listLength;
        }
        return list[Math.max(0, Math.min(index, listLength - 1))];
    };
    const namespaceRegex = /[^.]*(?=\..*)\.|.*/;
    const stripNameRegex = /\..*/;
    const stripUidRegex = /::\d+$/;
    const eventRegistry = {};
    let uidEvent = 1;
    const customEvents = {
        mouseenter: "mouseover",
        mouseleave: "mouseout"
    };
    const nativeEvents = new Set([ "click", "dblclick", "mouseup", "mousedown", "contextmenu", "mousewheel", "DOMMouseScroll", "mouseover", "mouseout", "mousemove", "selectstart", "selectend", "keydown", "keypress", "keyup", "orientationchange", "touchstart", "touchmove", "touchend", "touchcancel", "pointerdown", "pointermove", "pointerup", "pointerleave", "pointercancel", "gesturestart", "gesturechange", "gestureend", "focus", "blur", "change", "reset", "select", "submit", "focusin", "focusout", "load", "unload", "beforeunload", "resize", "move", "DOMContentLoaded", "readystatechange", "error", "abort", "scroll" ]);
    function makeEventUid(element, uid) {
        return uid && `${uid}::${uidEvent++}` || element.uidEvent || uidEvent++;
    }
    function getElementEvents(element) {
        const uid = makeEventUid(element);
        element.uidEvent = uid;
        eventRegistry[uid] = eventRegistry[uid] || {};
        return eventRegistry[uid];
    }
    function bootstrapHandler(element, fn) {
        return function handler(event) {
            hydrateObj(event, {
                delegateTarget: element
            });
            if (handler.oneOff) {
                EventHandler.off(element, event.type, fn);
            }
            return fn.apply(element, [ event ]);
        };
    }
    function bootstrapDelegationHandler(element, selector, fn) {
        return function handler(event) {
            const domElements = element.querySelectorAll(selector);
            for (let {
                target
            } = event; target && target !== this; target = target.parentNode) {
                for (const domElement of domElements) {
                    if (domElement !== target) {
                        continue;
                    }
                    hydrateObj(event, {
                        delegateTarget: target
                    });
                    if (handler.oneOff) {
                        EventHandler.off(element, event.type, selector, fn);
                    }
                    return fn.apply(target, [ event ]);
                }
            }
        };
    }
    function findHandler(events, callable, delegationSelector = null) {
        return Object.values(events).find(event => event.callable === callable && event.delegationSelector === delegationSelector);
    }
    function normalizeParameters(originalTypeEvent, handler, delegationFunction) {
        const isDelegated = typeof handler === "string";
        const callable = isDelegated ? delegationFunction : handler || delegationFunction;
        let typeEvent = getTypeEvent(originalTypeEvent);
        if (!nativeEvents.has(typeEvent)) {
            typeEvent = originalTypeEvent;
        }
        return [ isDelegated, callable, typeEvent ];
    }
    function addHandler(element, originalTypeEvent, handler, delegationFunction, oneOff) {
        if (typeof originalTypeEvent !== "string" || !element) {
            return;
        }
        let [ isDelegated, callable, typeEvent ] = normalizeParameters(originalTypeEvent, handler, delegationFunction);
        if (originalTypeEvent in customEvents) {
            const wrapFunction = fn => {
                return function(event) {
                    if (!event.relatedTarget || event.relatedTarget !== event.delegateTarget && !event.delegateTarget.contains(event.relatedTarget)) {
                        return fn.call(this, event);
                    }
                };
            };
            callable = wrapFunction(callable);
        }
        const events = getElementEvents(element);
        const handlers = events[typeEvent] || (events[typeEvent] = {});
        const previousFunction = findHandler(handlers, callable, isDelegated ? handler : null);
        if (previousFunction) {
            previousFunction.oneOff = previousFunction.oneOff && oneOff;
            return;
        }
        const uid = makeEventUid(callable, originalTypeEvent.replace(namespaceRegex, ""));
        const fn = isDelegated ? bootstrapDelegationHandler(element, handler, callable) : bootstrapHandler(element, callable);
        fn.delegationSelector = isDelegated ? handler : null;
        fn.callable = callable;
        fn.oneOff = oneOff;
        fn.uidEvent = uid;
        handlers[uid] = fn;
        element.addEventListener(typeEvent, fn, isDelegated);
    }
    function removeHandler(element, events, typeEvent, handler, delegationSelector) {
        const fn = findHandler(events[typeEvent], handler, delegationSelector);
        if (!fn) {
            return;
        }
        element.removeEventListener(typeEvent, fn, Boolean(delegationSelector));
        delete events[typeEvent][fn.uidEvent];
    }
    function removeNamespacedHandlers(element, events, typeEvent, namespace) {
        const storeElementEvent = events[typeEvent] || {};
        for (const [ handlerKey, event ] of Object.entries(storeElementEvent)) {
            if (handlerKey.includes(namespace)) {
                removeHandler(element, events, typeEvent, event.callable, event.delegationSelector);
            }
        }
    }
    function getTypeEvent(event) {
        event = event.replace(stripNameRegex, "");
        return customEvents[event] || event;
    }
    const EventHandler = {
        on(element, event, handler, delegationFunction) {
            addHandler(element, event, handler, delegationFunction, false);
        },
        one(element, event, handler, delegationFunction) {
            addHandler(element, event, handler, delegationFunction, true);
        },
        off(element, originalTypeEvent, handler, delegationFunction) {
            if (typeof originalTypeEvent !== "string" || !element) {
                return;
            }
            const [ isDelegated, callable, typeEvent ] = normalizeParameters(originalTypeEvent, handler, delegationFunction);
            const inNamespace = typeEvent !== originalTypeEvent;
            const events = getElementEvents(element);
            const storeElementEvent = events[typeEvent] || {};
            const isNamespace = originalTypeEvent.startsWith(".");
            if (typeof callable !== "undefined") {
                if (!Object.keys(storeElementEvent).length) {
                    return;
                }
                removeHandler(element, events, typeEvent, callable, isDelegated ? handler : null);
                return;
            }
            if (isNamespace) {
                for (const elementEvent of Object.keys(events)) {
                    removeNamespacedHandlers(element, events, elementEvent, originalTypeEvent.slice(1));
                }
            }
            for (const [ keyHandlers, event ] of Object.entries(storeElementEvent)) {
                const handlerKey = keyHandlers.replace(stripUidRegex, "");
                if (!inNamespace || originalTypeEvent.includes(handlerKey)) {
                    removeHandler(element, events, typeEvent, event.callable, event.delegationSelector);
                }
            }
        },
        trigger(element, event, args) {
            if (typeof event !== "string" || !element) {
                return null;
            }
            const $ = getjQuery();
            const typeEvent = getTypeEvent(event);
            const inNamespace = event !== typeEvent;
            let jQueryEvent = null;
            let bubbles = true;
            let nativeDispatch = true;
            let defaultPrevented = false;
            if (inNamespace && $) {
                jQueryEvent = $.Event(event, args);
                $(element).trigger(jQueryEvent);
                bubbles = !jQueryEvent.isPropagationStopped();
                nativeDispatch = !jQueryEvent.isImmediatePropagationStopped();
                defaultPrevented = jQueryEvent.isDefaultPrevented();
            }
            const evt = hydrateObj(new Event(event, {
                bubbles: bubbles,
                cancelable: true
            }), args);
            if (defaultPrevented) {
                evt.preventDefault();
            }
            if (nativeDispatch) {
                element.dispatchEvent(evt);
            }
            if (evt.defaultPrevented && jQueryEvent) {
                jQueryEvent.preventDefault();
            }
            return evt;
        }
    };
    function hydrateObj(obj, meta = {}) {
        for (const [ key, value ] of Object.entries(meta)) {
            try {
                obj[key] = value;
            } catch (_unused) {
                Object.defineProperty(obj, key, {
                    configurable: true,
                    get() {
                        return value;
                    }
                });
            }
        }
        return obj;
    }
    function normalizeData(value) {
        if (value === "true") {
            return true;
        }
        if (value === "false") {
            return false;
        }
        if (value === Number(value).toString()) {
            return Number(value);
        }
        if (value === "" || value === "null") {
            return null;
        }
        if (typeof value !== "string") {
            return value;
        }
        try {
            return JSON.parse(decodeURIComponent(value));
        } catch (_unused) {
            return value;
        }
    }
    function normalizeDataKey(key) {
        return key.replace(/[A-Z]/g, chr => `-${chr.toLowerCase()}`);
    }
    const Manipulator = {
        setDataAttribute(element, key, value) {
            element.setAttribute(`data-bs-${normalizeDataKey(key)}`, value);
        },
        removeDataAttribute(element, key) {
            element.removeAttribute(`data-bs-${normalizeDataKey(key)}`);
        },
        getDataAttributes(element) {
            if (!element) {
                return {};
            }
            const attributes = {};
            const bsKeys = Object.keys(element.dataset).filter(key => key.startsWith("bs") && !key.startsWith("bsConfig"));
            for (const key of bsKeys) {
                let pureKey = key.replace(/^bs/, "");
                pureKey = pureKey.charAt(0).toLowerCase() + pureKey.slice(1, pureKey.length);
                attributes[pureKey] = normalizeData(element.dataset[key]);
            }
            return attributes;
        },
        getDataAttribute(element, key) {
            return normalizeData(element.getAttribute(`data-bs-${normalizeDataKey(key)}`));
        }
    };
    class Config {
        static get Default() {
            return {};
        }
        static get DefaultType() {
            return {};
        }
        static get NAME() {
            throw new Error('You have to implement the static method "NAME", for each component!');
        }
        _getConfig(config) {
            config = this._mergeConfigObj(config);
            config = this._configAfterMerge(config);
            this._typeCheckConfig(config);
            return config;
        }
        _configAfterMerge(config) {
            return config;
        }
        _mergeConfigObj(config, element) {
            const jsonConfig = isElement$1(element) ? Manipulator.getDataAttribute(element, "config") : {};
            return {
                ...this.constructor.Default,
                ...typeof jsonConfig === "object" ? jsonConfig : {},
                ...isElement$1(element) ? Manipulator.getDataAttributes(element) : {},
                ...typeof config === "object" ? config : {}
            };
        }
        _typeCheckConfig(config, configTypes = this.constructor.DefaultType) {
            for (const [ property, expectedTypes ] of Object.entries(configTypes)) {
                const value = config[property];
                const valueType = isElement$1(value) ? "element" : toType(value);
                if (!new RegExp(expectedTypes).test(valueType)) {
                    throw new TypeError(`${this.constructor.NAME.toUpperCase()}: Option "${property}" provided type "${valueType}" but expected type "${expectedTypes}".`);
                }
            }
        }
    }
    const VERSION = "5.3.3";
    class BaseComponent extends Config {
        constructor(element, config) {
            super();
            element = getElement(element);
            if (!element) {
                return;
            }
            this._element = element;
            this._config = this._getConfig(config);
            Data.set(this._element, this.constructor.DATA_KEY, this);
        }
        dispose() {
            Data.remove(this._element, this.constructor.DATA_KEY);
            EventHandler.off(this._element, this.constructor.EVENT_KEY);
            for (const propertyName of Object.getOwnPropertyNames(this)) {
                this[propertyName] = null;
            }
        }
        _queueCallback(callback, element, isAnimated = true) {
            executeAfterTransition(callback, element, isAnimated);
        }
        _getConfig(config) {
            config = this._mergeConfigObj(config, this._element);
            config = this._configAfterMerge(config);
            this._typeCheckConfig(config);
            return config;
        }
        static getInstance(element) {
            return Data.get(getElement(element), this.DATA_KEY);
        }
        static getOrCreateInstance(element, config = {}) {
            return this.getInstance(element) || new this(element, typeof config === "object" ? config : null);
        }
        static get VERSION() {
            return VERSION;
        }
        static get DATA_KEY() {
            return `bs.${this.NAME}`;
        }
        static get EVENT_KEY() {
            return `.${this.DATA_KEY}`;
        }
        static eventName(name) {
            return `${name}${this.EVENT_KEY}`;
        }
    }
    const getSelector = element => {
        let selector = element.getAttribute("data-bs-target");
        if (!selector || selector === "#") {
            let hrefAttribute = element.getAttribute("href");
            if (!hrefAttribute || !hrefAttribute.includes("#") && !hrefAttribute.startsWith(".")) {
                return null;
            }
            if (hrefAttribute.includes("#") && !hrefAttribute.startsWith("#")) {
                hrefAttribute = `#${hrefAttribute.split("#")[1]}`;
            }
            selector = hrefAttribute && hrefAttribute !== "#" ? hrefAttribute.trim() : null;
        }
        return selector ? selector.split(",").map(sel => parseSelector(sel)).join(",") : null;
    };
    const SelectorEngine = {
        find(selector, element = document.documentElement) {
            return [].concat(...Element.prototype.querySelectorAll.call(element, selector));
        },
        findOne(selector, element = document.documentElement) {
            return Element.prototype.querySelector.call(element, selector);
        },
        children(element, selector) {
            return [].concat(...element.children).filter(child => child.matches(selector));
        },
        parents(element, selector) {
            const parents = [];
            let ancestor = element.parentNode.closest(selector);
            while (ancestor) {
                parents.push(ancestor);
                ancestor = ancestor.parentNode.closest(selector);
            }
            return parents;
        },
        prev(element, selector) {
            let previous = element.previousElementSibling;
            while (previous) {
                if (previous.matches(selector)) {
                    return [ previous ];
                }
                previous = previous.previousElementSibling;
            }
            return [];
        },
        next(element, selector) {
            let next = element.nextElementSibling;
            while (next) {
                if (next.matches(selector)) {
                    return [ next ];
                }
                next = next.nextElementSibling;
            }
            return [];
        },
        focusableChildren(element) {
            const focusables = [ "a", "button", "input", "textarea", "select", "details", "[tabindex]", '[contenteditable="true"]' ].map(selector => `${selector}:not([tabindex^="-"])`).join(",");
            return this.find(focusables, element).filter(el => !isDisabled(el) && isVisible(el));
        },
        getSelectorFromElement(element) {
            const selector = getSelector(element);
            if (selector) {
                return SelectorEngine.findOne(selector) ? selector : null;
            }
            return null;
        },
        getElementFromSelector(element) {
            const selector = getSelector(element);
            return selector ? SelectorEngine.findOne(selector) : null;
        },
        getMultipleElementsFromSelector(element) {
            const selector = getSelector(element);
            return selector ? SelectorEngine.find(selector) : [];
        }
    };
    const enableDismissTrigger = (component, method = "hide") => {
        const clickEvent = `click.dismiss${component.EVENT_KEY}`;
        const name = component.NAME;
        EventHandler.on(document, clickEvent, `[data-bs-dismiss="${name}"]`, function(event) {
            if ([ "A", "AREA" ].includes(this.tagName)) {
                event.preventDefault();
            }
            if (isDisabled(this)) {
                return;
            }
            const target = SelectorEngine.getElementFromSelector(this) || this.closest(`.${name}`);
            const instance = component.getOrCreateInstance(target);
            instance[method]();
        });
    };
    const NAME$f = "alert";
    const DATA_KEY$a = "bs.alert";
    const EVENT_KEY$b = `.${DATA_KEY$a}`;
    const EVENT_CLOSE = `close${EVENT_KEY$b}`;
    const EVENT_CLOSED = `closed${EVENT_KEY$b}`;
    const CLASS_NAME_FADE$5 = "fade";
    const CLASS_NAME_SHOW$8 = "show";
    class Alert extends BaseComponent {
        static get NAME() {
            return NAME$f;
        }
        close() {
            const closeEvent = EventHandler.trigger(this._element, EVENT_CLOSE);
            if (closeEvent.defaultPrevented) {
                return;
            }
            this._element.classList.remove(CLASS_NAME_SHOW$8);
            const isAnimated = this._element.classList.contains(CLASS_NAME_FADE$5);
            this._queueCallback(() => this._destroyElement(), this._element, isAnimated);
        }
        _destroyElement() {
            this._element.remove();
            EventHandler.trigger(this._element, EVENT_CLOSED);
            this.dispose();
        }
        static jQueryInterface(config) {
            return this.each(function() {
                const data = Alert.getOrCreateInstance(this);
                if (typeof config !== "string") {
                    return;
                }
                if (data[config] === undefined || config.startsWith("_") || config === "constructor") {
                    throw new TypeError(`No method named "${config}"`);
                }
                data[config](this);
            });
        }
    }
    enableDismissTrigger(Alert, "close");
    defineJQueryPlugin(Alert);
    const NAME$e = "button";
    const DATA_KEY$9 = "bs.button";
    const EVENT_KEY$a = `.${DATA_KEY$9}`;
    const DATA_API_KEY$6 = ".data-api";
    const CLASS_NAME_ACTIVE$3 = "active";
    const SELECTOR_DATA_TOGGLE$5 = '[data-bs-toggle="button"]';
    const EVENT_CLICK_DATA_API$6 = `click${EVENT_KEY$a}${DATA_API_KEY$6}`;
    class Button extends BaseComponent {
        static get NAME() {
            return NAME$e;
        }
        toggle() {
            this._element.setAttribute("aria-pressed", this._element.classList.toggle(CLASS_NAME_ACTIVE$3));
        }
        static jQueryInterface(config) {
            return this.each(function() {
                const data = Button.getOrCreateInstance(this);
                if (config === "toggle") {
                    data[config]();
                }
            });
        }
    }
    EventHandler.on(document, EVENT_CLICK_DATA_API$6, SELECTOR_DATA_TOGGLE$5, event => {
        event.preventDefault();
        const button = event.target.closest(SELECTOR_DATA_TOGGLE$5);
        const data = Button.getOrCreateInstance(button);
        data.toggle();
    });
    defineJQueryPlugin(Button);
    const NAME$d = "swipe";
    const EVENT_KEY$9 = ".bs.swipe";
    const EVENT_TOUCHSTART = `touchstart${EVENT_KEY$9}`;
    const EVENT_TOUCHMOVE = `touchmove${EVENT_KEY$9}`;
    const EVENT_TOUCHEND = `touchend${EVENT_KEY$9}`;
    const EVENT_POINTERDOWN = `pointerdown${EVENT_KEY$9}`;
    const EVENT_POINTERUP = `pointerup${EVENT_KEY$9}`;
    const POINTER_TYPE_TOUCH = "touch";
    const POINTER_TYPE_PEN = "pen";
    const CLASS_NAME_POINTER_EVENT = "pointer-event";
    const SWIPE_THRESHOLD = 40;
    const Default$c = {
        endCallback: null,
        leftCallback: null,
        rightCallback: null
    };
    const DefaultType$c = {
        endCallback: "(function|null)",
        leftCallback: "(function|null)",
        rightCallback: "(function|null)"
    };
    class Swipe extends Config {
        constructor(element, config) {
            super();
            this._element = element;
            if (!element || !Swipe.isSupported()) {
                return;
            }
            this._config = this._getConfig(config);
            this._deltaX = 0;
            this._supportPointerEvents = Boolean(window.PointerEvent);
            this._initEvents();
        }
        static get Default() {
            return Default$c;
        }
        static get DefaultType() {
            return DefaultType$c;
        }
        static get NAME() {
            return NAME$d;
        }
        dispose() {
            EventHandler.off(this._element, EVENT_KEY$9);
        }
        _start(event) {
            if (!this._supportPointerEvents) {
                this._deltaX = event.touches[0].clientX;
                return;
            }
            if (this._eventIsPointerPenTouch(event)) {
                this._deltaX = event.clientX;
            }
        }
        _end(event) {
            if (this._eventIsPointerPenTouch(event)) {
                this._deltaX = event.clientX - this._deltaX;
            }
            this._handleSwipe();
            execute(this._config.endCallback);
        }
        _move(event) {
            this._deltaX = event.touches && event.touches.length > 1 ? 0 : event.touches[0].clientX - this._deltaX;
        }
        _handleSwipe() {
            const absDeltaX = Math.abs(this._deltaX);
            if (absDeltaX <= SWIPE_THRESHOLD) {
                return;
            }
            const direction = absDeltaX / this._deltaX;
            this._deltaX = 0;
            if (!direction) {
                return;
            }
            execute(direction > 0 ? this._config.rightCallback : this._config.leftCallback);
        }
        _initEvents() {
            if (this._supportPointerEvents) {
                EventHandler.on(this._element, EVENT_POINTERDOWN, event => this._start(event));
                EventHandler.on(this._element, EVENT_POINTERUP, event => this._end(event));
                this._element.classList.add(CLASS_NAME_POINTER_EVENT);
            } else {
                EventHandler.on(this._element, EVENT_TOUCHSTART, event => this._start(event));
                EventHandler.on(this._element, EVENT_TOUCHMOVE, event => this._move(event));
                EventHandler.on(this._element, EVENT_TOUCHEND, event => this._end(event));
            }
        }
        _eventIsPointerPenTouch(event) {
            return this._supportPointerEvents && (event.pointerType === POINTER_TYPE_PEN || event.pointerType === POINTER_TYPE_TOUCH);
        }
        static isSupported() {
            return "ontouchstart" in document.documentElement || navigator.maxTouchPoints > 0;
        }
    }
    const NAME$c = "carousel";
    const DATA_KEY$8 = "bs.carousel";
    const EVENT_KEY$8 = `.${DATA_KEY$8}`;
    const DATA_API_KEY$5 = ".data-api";
    const ARROW_LEFT_KEY$1 = "ArrowLeft";
    const ARROW_RIGHT_KEY$1 = "ArrowRight";
    const TOUCHEVENT_COMPAT_WAIT = 500;
    const ORDER_NEXT = "next";
    const ORDER_PREV = "prev";
    const DIRECTION_LEFT = "left";
    const DIRECTION_RIGHT = "right";
    const EVENT_SLIDE = `slide${EVENT_KEY$8}`;
    const EVENT_SLID = `slid${EVENT_KEY$8}`;
    const EVENT_KEYDOWN$1 = `keydown${EVENT_KEY$8}`;
    const EVENT_MOUSEENTER$1 = `mouseenter${EVENT_KEY$8}`;
    const EVENT_MOUSELEAVE$1 = `mouseleave${EVENT_KEY$8}`;
    const EVENT_DRAG_START = `dragstart${EVENT_KEY$8}`;
    const EVENT_LOAD_DATA_API$3 = `load${EVENT_KEY$8}${DATA_API_KEY$5}`;
    const EVENT_CLICK_DATA_API$5 = `click${EVENT_KEY$8}${DATA_API_KEY$5}`;
    const CLASS_NAME_CAROUSEL = "carousel";
    const CLASS_NAME_ACTIVE$2 = "active";
    const CLASS_NAME_SLIDE = "slide";
    const CLASS_NAME_END = "carousel-item-end";
    const CLASS_NAME_START = "carousel-item-start";
    const CLASS_NAME_NEXT = "carousel-item-next";
    const CLASS_NAME_PREV = "carousel-item-prev";
    const SELECTOR_ACTIVE = ".active";
    const SELECTOR_ITEM = ".carousel-item";
    const SELECTOR_ACTIVE_ITEM = SELECTOR_ACTIVE + SELECTOR_ITEM;
    const SELECTOR_ITEM_IMG = ".carousel-item img";
    const SELECTOR_INDICATORS = ".carousel-indicators";
    const SELECTOR_DATA_SLIDE = "[data-bs-slide], [data-bs-slide-to]";
    const SELECTOR_DATA_RIDE = '[data-bs-ride="carousel"]';
    const KEY_TO_DIRECTION = {
        [ARROW_LEFT_KEY$1]: DIRECTION_RIGHT,
        [ARROW_RIGHT_KEY$1]: DIRECTION_LEFT
    };
    const Default$b = {
        interval: 5e3,
        keyboard: true,
        pause: "hover",
        ride: false,
        touch: true,
        wrap: true
    };
    const DefaultType$b = {
        interval: "(number|boolean)",
        keyboard: "boolean",
        pause: "(string|boolean)",
        ride: "(boolean|string)",
        touch: "boolean",
        wrap: "boolean"
    };
    class Carousel extends BaseComponent {
        constructor(element, config) {
            super(element, config);
            this._interval = null;
            this._activeElement = null;
            this._isSliding = false;
            this.touchTimeout = null;
            this._swipeHelper = null;
            this._indicatorsElement = SelectorEngine.findOne(SELECTOR_INDICATORS, this._element);
            this._addEventListeners();
            if (this._config.ride === CLASS_NAME_CAROUSEL) {
                this.cycle();
            }
        }
        static get Default() {
            return Default$b;
        }
        static get DefaultType() {
            return DefaultType$b;
        }
        static get NAME() {
            return NAME$c;
        }
        next() {
            this._slide(ORDER_NEXT);
        }
        nextWhenVisible() {
            if (!document.hidden && isVisible(this._element)) {
                this.next();
            }
        }
        prev() {
            this._slide(ORDER_PREV);
        }
        pause() {
            if (this._isSliding) {
                triggerTransitionEnd(this._element);
            }
            this._clearInterval();
        }
        cycle() {
            this._clearInterval();
            this._updateInterval();
            this._interval = setInterval(() => this.nextWhenVisible(), this._config.interval);
        }
        _maybeEnableCycle() {
            if (!this._config.ride) {
                return;
            }
            if (this._isSliding) {
                EventHandler.one(this._element, EVENT_SLID, () => this.cycle());
                return;
            }
            this.cycle();
        }
        to(index) {
            const items = this._getItems();
            if (index > items.length - 1 || index < 0) {
                return;
            }
            if (this._isSliding) {
                EventHandler.one(this._element, EVENT_SLID, () => this.to(index));
                return;
            }
            const activeIndex = this._getItemIndex(this._getActive());
            if (activeIndex === index) {
                return;
            }
            const order = index > activeIndex ? ORDER_NEXT : ORDER_PREV;
            this._slide(order, items[index]);
        }
        dispose() {
            if (this._swipeHelper) {
                this._swipeHelper.dispose();
            }
            super.dispose();
        }
        _configAfterMerge(config) {
            config.defaultInterval = config.interval;
            return config;
        }
        _addEventListeners() {
            if (this._config.keyboard) {
                EventHandler.on(this._element, EVENT_KEYDOWN$1, event => this._keydown(event));
            }
            if (this._config.pause === "hover") {
                EventHandler.on(this._element, EVENT_MOUSEENTER$1, () => this.pause());
                EventHandler.on(this._element, EVENT_MOUSELEAVE$1, () => this._maybeEnableCycle());
            }
            if (this._config.touch && Swipe.isSupported()) {
                this._addTouchEventListeners();
            }
        }
        _addTouchEventListeners() {
            for (const img of SelectorEngine.find(SELECTOR_ITEM_IMG, this._element)) {
                EventHandler.on(img, EVENT_DRAG_START, event => event.preventDefault());
            }
            const endCallBack = () => {
                if (this._config.pause !== "hover") {
                    return;
                }
                this.pause();
                if (this.touchTimeout) {
                    clearTimeout(this.touchTimeout);
                }
                this.touchTimeout = setTimeout(() => this._maybeEnableCycle(), TOUCHEVENT_COMPAT_WAIT + this._config.interval);
            };
            const swipeConfig = {
                leftCallback: () => this._slide(this._directionToOrder(DIRECTION_LEFT)),
                rightCallback: () => this._slide(this._directionToOrder(DIRECTION_RIGHT)),
                endCallback: endCallBack
            };
            this._swipeHelper = new Swipe(this._element, swipeConfig);
        }
        _keydown(event) {
            if (/input|textarea/i.test(event.target.tagName)) {
                return;
            }
            const direction = KEY_TO_DIRECTION[event.key];
            if (direction) {
                event.preventDefault();
                this._slide(this._directionToOrder(direction));
            }
        }
        _getItemIndex(element) {
            return this._getItems().indexOf(element);
        }
        _setActiveIndicatorElement(index) {
            if (!this._indicatorsElement) {
                return;
            }
            const activeIndicator = SelectorEngine.findOne(SELECTOR_ACTIVE, this._indicatorsElement);
            activeIndicator.classList.remove(CLASS_NAME_ACTIVE$2);
            activeIndicator.removeAttribute("aria-current");
            const newActiveIndicator = SelectorEngine.findOne(`[data-bs-slide-to="${index}"]`, this._indicatorsElement);
            if (newActiveIndicator) {
                newActiveIndicator.classList.add(CLASS_NAME_ACTIVE$2);
                newActiveIndicator.setAttribute("aria-current", "true");
            }
        }
        _updateInterval() {
            const element = this._activeElement || this._getActive();
            if (!element) {
                return;
            }
            const elementInterval = Number.parseInt(element.getAttribute("data-bs-interval"), 10);
            this._config.interval = elementInterval || this._config.defaultInterval;
        }
        _slide(order, element = null) {
            if (this._isSliding) {
                return;
            }
            const activeElement = this._getActive();
            const isNext = order === ORDER_NEXT;
            const nextElement = element || getNextActiveElement(this._getItems(), activeElement, isNext, this._config.wrap);
            if (nextElement === activeElement) {
                return;
            }
            const nextElementIndex = this._getItemIndex(nextElement);
            const triggerEvent = eventName => {
                return EventHandler.trigger(this._element, eventName, {
                    relatedTarget: nextElement,
                    direction: this._orderToDirection(order),
                    from: this._getItemIndex(activeElement),
                    to: nextElementIndex
                });
            };
            const slideEvent = triggerEvent(EVENT_SLIDE);
            if (slideEvent.defaultPrevented) {
                return;
            }
            if (!activeElement || !nextElement) {
                return;
            }
            const isCycling = Boolean(this._interval);
            this.pause();
            this._isSliding = true;
            this._setActiveIndicatorElement(nextElementIndex);
            this._activeElement = nextElement;
            const directionalClassName = isNext ? CLASS_NAME_START : CLASS_NAME_END;
            const orderClassName = isNext ? CLASS_NAME_NEXT : CLASS_NAME_PREV;
            nextElement.classList.add(orderClassName);
            reflow(nextElement);
            activeElement.classList.add(directionalClassName);
            nextElement.classList.add(directionalClassName);
            const completeCallBack = () => {
                nextElement.classList.remove(directionalClassName, orderClassName);
                nextElement.classList.add(CLASS_NAME_ACTIVE$2);
                activeElement.classList.remove(CLASS_NAME_ACTIVE$2, orderClassName, directionalClassName);
                this._isSliding = false;
                triggerEvent(EVENT_SLID);
            };
            this._queueCallback(completeCallBack, activeElement, this._isAnimated());
            if (isCycling) {
                this.cycle();
            }
        }
        _isAnimated() {
            return this._element.classList.contains(CLASS_NAME_SLIDE);
        }
        _getActive() {
            return SelectorEngine.findOne(SELECTOR_ACTIVE_ITEM, this._element);
        }
        _getItems() {
            return SelectorEngine.find(SELECTOR_ITEM, this._element);
        }
        _clearInterval() {
            if (this._interval) {
                clearInterval(this._interval);
                this._interval = null;
            }
        }
        _directionToOrder(direction) {
            if (isRTL()) {
                return direction === DIRECTION_LEFT ? ORDER_PREV : ORDER_NEXT;
            }
            return direction === DIRECTION_LEFT ? ORDER_NEXT : ORDER_PREV;
        }
        _orderToDirection(order) {
            if (isRTL()) {
                return order === ORDER_PREV ? DIRECTION_LEFT : DIRECTION_RIGHT;
            }
            return order === ORDER_PREV ? DIRECTION_RIGHT : DIRECTION_LEFT;
        }
        static jQueryInterface(config) {
            return this.each(function() {
                const data = Carousel.getOrCreateInstance(this, config);
                if (typeof config === "number") {
                    data.to(config);
                    return;
                }
                if (typeof config === "string") {
                    if (data[config] === undefined || config.startsWith("_") || config === "constructor") {
                        throw new TypeError(`No method named "${config}"`);
                    }
                    data[config]();
                }
            });
        }
    }
    EventHandler.on(document, EVENT_CLICK_DATA_API$5, SELECTOR_DATA_SLIDE, function(event) {
        const target = SelectorEngine.getElementFromSelector(this);
        if (!target || !target.classList.contains(CLASS_NAME_CAROUSEL)) {
            return;
        }
        event.preventDefault();
        const carousel = Carousel.getOrCreateInstance(target);
        const slideIndex = this.getAttribute("data-bs-slide-to");
        if (slideIndex) {
            carousel.to(slideIndex);
            carousel._maybeEnableCycle();
            return;
        }
        if (Manipulator.getDataAttribute(this, "slide") === "next") {
            carousel.next();
            carousel._maybeEnableCycle();
            return;
        }
        carousel.prev();
        carousel._maybeEnableCycle();
    });
    EventHandler.on(window, EVENT_LOAD_DATA_API$3, () => {
        const carousels = SelectorEngine.find(SELECTOR_DATA_RIDE);
        for (const carousel of carousels) {
            Carousel.getOrCreateInstance(carousel);
        }
    });
    defineJQueryPlugin(Carousel);
    const NAME$b = "collapse";
    const DATA_KEY$7 = "bs.collapse";
    const EVENT_KEY$7 = `.${DATA_KEY$7}`;
    const DATA_API_KEY$4 = ".data-api";
    const EVENT_SHOW$6 = `show${EVENT_KEY$7}`;
    const EVENT_SHOWN$6 = `shown${EVENT_KEY$7}`;
    const EVENT_HIDE$6 = `hide${EVENT_KEY$7}`;
    const EVENT_HIDDEN$6 = `hidden${EVENT_KEY$7}`;
    const EVENT_CLICK_DATA_API$4 = `click${EVENT_KEY$7}${DATA_API_KEY$4}`;
    const CLASS_NAME_SHOW$7 = "show";
    const CLASS_NAME_COLLAPSE = "collapse";
    const CLASS_NAME_COLLAPSING = "collapsing";
    const CLASS_NAME_COLLAPSED = "collapsed";
    const CLASS_NAME_DEEPER_CHILDREN = `:scope .${CLASS_NAME_COLLAPSE} .${CLASS_NAME_COLLAPSE}`;
    const CLASS_NAME_HORIZONTAL = "collapse-horizontal";
    const WIDTH = "width";
    const HEIGHT = "height";
    const SELECTOR_ACTIVES = ".collapse.show, .collapse.collapsing";
    const SELECTOR_DATA_TOGGLE$4 = '[data-bs-toggle="collapse"]';
    const Default$a = {
        parent: null,
        toggle: true
    };
    const DefaultType$a = {
        parent: "(null|element)",
        toggle: "boolean"
    };
    class Collapse extends BaseComponent {
        constructor(element, config) {
            super(element, config);
            this._isTransitioning = false;
            this._triggerArray = [];
            const toggleList = SelectorEngine.find(SELECTOR_DATA_TOGGLE$4);
            for (const elem of toggleList) {
                const selector = SelectorEngine.getSelectorFromElement(elem);
                const filterElement = SelectorEngine.find(selector).filter(foundElement => foundElement === this._element);
                if (selector !== null && filterElement.length) {
                    this._triggerArray.push(elem);
                }
            }
            this._initializeChildren();
            if (!this._config.parent) {
                this._addAriaAndCollapsedClass(this._triggerArray, this._isShown());
            }
            if (this._config.toggle) {
                this.toggle();
            }
        }
        static get Default() {
            return Default$a;
        }
        static get DefaultType() {
            return DefaultType$a;
        }
        static get NAME() {
            return NAME$b;
        }
        toggle() {
            if (this._isShown()) {
                this.hide();
            } else {
                this.show();
            }
        }
        show() {
            if (this._isTransitioning || this._isShown()) {
                return;
            }
            let activeChildren = [];
            if (this._config.parent) {
                activeChildren = this._getFirstLevelChildren(SELECTOR_ACTIVES).filter(element => element !== this._element).map(element => Collapse.getOrCreateInstance(element, {
                    toggle: false
                }));
            }
            if (activeChildren.length && activeChildren[0]._isTransitioning) {
                return;
            }
            const startEvent = EventHandler.trigger(this._element, EVENT_SHOW$6);
            if (startEvent.defaultPrevented) {
                return;
            }
            for (const activeInstance of activeChildren) {
                activeInstance.hide();
            }
            const dimension = this._getDimension();
            this._element.classList.remove(CLASS_NAME_COLLAPSE);
            this._element.classList.add(CLASS_NAME_COLLAPSING);
            this._element.style[dimension] = 0;
            this._addAriaAndCollapsedClass(this._triggerArray, true);
            this._isTransitioning = true;
            const complete = () => {
                this._isTransitioning = false;
                this._element.classList.remove(CLASS_NAME_COLLAPSING);
                this._element.classList.add(CLASS_NAME_COLLAPSE, CLASS_NAME_SHOW$7);
                this._element.style[dimension] = "";
                EventHandler.trigger(this._element, EVENT_SHOWN$6);
            };
            const capitalizedDimension = dimension[0].toUpperCase() + dimension.slice(1);
            const scrollSize = `scroll${capitalizedDimension}`;
            this._queueCallback(complete, this._element, true);
            this._element.style[dimension] = `${this._element[scrollSize]}px`;
        }
        hide() {
            if (this._isTransitioning || !this._isShown()) {
                return;
            }
            const startEvent = EventHandler.trigger(this._element, EVENT_HIDE$6);
            if (startEvent.defaultPrevented) {
                return;
            }
            const dimension = this._getDimension();
            this._element.style[dimension] = `${this._element.getBoundingClientRect()[dimension]}px`;
            reflow(this._element);
            this._element.classList.add(CLASS_NAME_COLLAPSING);
            this._element.classList.remove(CLASS_NAME_COLLAPSE, CLASS_NAME_SHOW$7);
            for (const trigger of this._triggerArray) {
                const element = SelectorEngine.getElementFromSelector(trigger);
                if (element && !this._isShown(element)) {
                    this._addAriaAndCollapsedClass([ trigger ], false);
                }
            }
            this._isTransitioning = true;
            const complete = () => {
                this._isTransitioning = false;
                this._element.classList.remove(CLASS_NAME_COLLAPSING);
                this._element.classList.add(CLASS_NAME_COLLAPSE);
                EventHandler.trigger(this._element, EVENT_HIDDEN$6);
            };
            this._element.style[dimension] = "";
            this._queueCallback(complete, this._element, true);
        }
        _isShown(element = this._element) {
            return element.classList.contains(CLASS_NAME_SHOW$7);
        }
        _configAfterMerge(config) {
            config.toggle = Boolean(config.toggle);
            config.parent = getElement(config.parent);
            return config;
        }
        _getDimension() {
            return this._element.classList.contains(CLASS_NAME_HORIZONTAL) ? WIDTH : HEIGHT;
        }
        _initializeChildren() {
            if (!this._config.parent) {
                return;
            }
            const children = this._getFirstLevelChildren(SELECTOR_DATA_TOGGLE$4);
            for (const element of children) {
                const selected = SelectorEngine.getElementFromSelector(element);
                if (selected) {
                    this._addAriaAndCollapsedClass([ element ], this._isShown(selected));
                }
            }
        }
        _getFirstLevelChildren(selector) {
            const children = SelectorEngine.find(CLASS_NAME_DEEPER_CHILDREN, this._config.parent);
            return SelectorEngine.find(selector, this._config.parent).filter(element => !children.includes(element));
        }
        _addAriaAndCollapsedClass(triggerArray, isOpen) {
            if (!triggerArray.length) {
                return;
            }
            for (const element of triggerArray) {
                element.classList.toggle(CLASS_NAME_COLLAPSED, !isOpen);
                element.setAttribute("aria-expanded", isOpen);
            }
        }
        static jQueryInterface(config) {
            const _config = {};
            if (typeof config === "string" && /show|hide/.test(config)) {
                _config.toggle = false;
            }
            return this.each(function() {
                const data = Collapse.getOrCreateInstance(this, _config);
                if (typeof config === "string") {
                    if (typeof data[config] === "undefined") {
                        throw new TypeError(`No method named "${config}"`);
                    }
                    data[config]();
                }
            });
        }
    }
    EventHandler.on(document, EVENT_CLICK_DATA_API$4, SELECTOR_DATA_TOGGLE$4, function(event) {
        if (event.target.tagName === "A" || event.delegateTarget && event.delegateTarget.tagName === "A") {
            event.preventDefault();
        }
        for (const element of SelectorEngine.getMultipleElementsFromSelector(this)) {
            Collapse.getOrCreateInstance(element, {
                toggle: false
            }).toggle();
        }
    });
    defineJQueryPlugin(Collapse);
    var top = "top";
    var bottom = "bottom";
    var right = "right";
    var left = "left";
    var auto = "auto";
    var basePlacements = [ top, bottom, right, left ];
    var start = "start";
    var end = "end";
    var clippingParents = "clippingParents";
    var viewport = "viewport";
    var popper = "popper";
    var reference = "reference";
    var variationPlacements = basePlacements.reduce(function(acc, placement) {
        return acc.concat([ placement + "-" + start, placement + "-" + end ]);
    }, []);
    var placements = [].concat(basePlacements, [ auto ]).reduce(function(acc, placement) {
        return acc.concat([ placement, placement + "-" + start, placement + "-" + end ]);
    }, []);
    var beforeRead = "beforeRead";
    var read = "read";
    var afterRead = "afterRead";
    var beforeMain = "beforeMain";
    var main = "main";
    var afterMain = "afterMain";
    var beforeWrite = "beforeWrite";
    var write = "write";
    var afterWrite = "afterWrite";
    var modifierPhases = [ beforeRead, read, afterRead, beforeMain, main, afterMain, beforeWrite, write, afterWrite ];
    function getNodeName(element) {
        return element ? (element.nodeName || "").toLowerCase() : null;
    }
    function getWindow(node) {
        if (node == null) {
            return window;
        }
        if (node.toString() !== "[object Window]") {
            var ownerDocument = node.ownerDocument;
            return ownerDocument ? ownerDocument.defaultView || window : window;
        }
        return node;
    }
    function isElement(node) {
        var OwnElement = getWindow(node).Element;
        return node instanceof OwnElement || node instanceof Element;
    }
    function isHTMLElement(node) {
        var OwnElement = getWindow(node).HTMLElement;
        return node instanceof OwnElement || node instanceof HTMLElement;
    }
    function isShadowRoot(node) {
        if (typeof ShadowRoot === "undefined") {
            return false;
        }
        var OwnElement = getWindow(node).ShadowRoot;
        return node instanceof OwnElement || node instanceof ShadowRoot;
    }
    function applyStyles(_ref) {
        var state = _ref.state;
        Object.keys(state.elements).forEach(function(name) {
            var style = state.styles[name] || {};
            var attributes = state.attributes[name] || {};
            var element = state.elements[name];
            if (!isHTMLElement(element) || !getNodeName(element)) {
                return;
            }
            Object.assign(element.style, style);
            Object.keys(attributes).forEach(function(name) {
                var value = attributes[name];
                if (value === false) {
                    element.removeAttribute(name);
                } else {
                    element.setAttribute(name, value === true ? "" : value);
                }
            });
        });
    }
    function effect$2(_ref2) {
        var state = _ref2.state;
        var initialStyles = {
            popper: {
                position: state.options.strategy,
                left: "0",
                top: "0",
                margin: "0"
            },
            arrow: {
                position: "absolute"
            },
            reference: {}
        };
        Object.assign(state.elements.popper.style, initialStyles.popper);
        state.styles = initialStyles;
        if (state.elements.arrow) {
            Object.assign(state.elements.arrow.style, initialStyles.arrow);
        }
        return function() {
            Object.keys(state.elements).forEach(function(name) {
                var element = state.elements[name];
                var attributes = state.attributes[name] || {};
                var styleProperties = Object.keys(state.styles.hasOwnProperty(name) ? state.styles[name] : initialStyles[name]);
                var style = styleProperties.reduce(function(style, property) {
                    style[property] = "";
                    return style;
                }, {});
                if (!isHTMLElement(element) || !getNodeName(element)) {
                    return;
                }
                Object.assign(element.style, style);
                Object.keys(attributes).forEach(function(attribute) {
                    element.removeAttribute(attribute);
                });
            });
        };
    }
    const applyStyles$1 = {
        name: "applyStyles",
        enabled: true,
        phase: "write",
        fn: applyStyles,
        effect: effect$2,
        requires: [ "computeStyles" ]
    };
    function getBasePlacement(placement) {
        return placement.split("-")[0];
    }
    var max = Math.max;
    var min = Math.min;
    var round = Math.round;
    function getUAString() {
        var uaData = navigator.userAgentData;
        if (uaData != null && uaData.brands && Array.isArray(uaData.brands)) {
            return uaData.brands.map(function(item) {
                return item.brand + "/" + item.version;
            }).join(" ");
        }
        return navigator.userAgent;
    }
    function isLayoutViewport() {
        return !/^((?!chrome|android).)*safari/i.test(getUAString());
    }
    function getBoundingClientRect(element, includeScale, isFixedStrategy) {
        if (includeScale === void 0) {
            includeScale = false;
        }
        if (isFixedStrategy === void 0) {
            isFixedStrategy = false;
        }
        var clientRect = element.getBoundingClientRect();
        var scaleX = 1;
        var scaleY = 1;
        if (includeScale && isHTMLElement(element)) {
            scaleX = element.offsetWidth > 0 ? round(clientRect.width) / element.offsetWidth || 1 : 1;
            scaleY = element.offsetHeight > 0 ? round(clientRect.height) / element.offsetHeight || 1 : 1;
        }
        var _ref = isElement(element) ? getWindow(element) : window, visualViewport = _ref.visualViewport;
        var addVisualOffsets = !isLayoutViewport() && isFixedStrategy;
        var x = (clientRect.left + (addVisualOffsets && visualViewport ? visualViewport.offsetLeft : 0)) / scaleX;
        var y = (clientRect.top + (addVisualOffsets && visualViewport ? visualViewport.offsetTop : 0)) / scaleY;
        var width = clientRect.width / scaleX;
        var height = clientRect.height / scaleY;
        return {
            width: width,
            height: height,
            top: y,
            right: x + width,
            bottom: y + height,
            left: x,
            x: x,
            y: y
        };
    }
    function getLayoutRect(element) {
        var clientRect = getBoundingClientRect(element);
        var width = element.offsetWidth;
        var height = element.offsetHeight;
        if (Math.abs(clientRect.width - width) <= 1) {
            width = clientRect.width;
        }
        if (Math.abs(clientRect.height - height) <= 1) {
            height = clientRect.height;
        }
        return {
            x: element.offsetLeft,
            y: element.offsetTop,
            width: width,
            height: height
        };
    }
    function contains(parent, child) {
        var rootNode = child.getRootNode && child.getRootNode();
        if (parent.contains(child)) {
            return true;
        } else if (rootNode && isShadowRoot(rootNode)) {
            var next = child;
            do {
                if (next && parent.isSameNode(next)) {
                    return true;
                }
                next = next.parentNode || next.host;
            } while (next);
        }
        return false;
    }
    function getComputedStyle$1(element) {
        return getWindow(element).getComputedStyle(element);
    }
    function isTableElement(element) {
        return [ "table", "td", "th" ].indexOf(getNodeName(element)) >= 0;
    }
    function getDocumentElement(element) {
        return ((isElement(element) ? element.ownerDocument : element.document) || window.document).documentElement;
    }
    function getParentNode(element) {
        if (getNodeName(element) === "html") {
            return element;
        }
        return element.assignedSlot || element.parentNode || (isShadowRoot(element) ? element.host : null) || getDocumentElement(element);
    }
    function getTrueOffsetParent(element) {
        if (!isHTMLElement(element) || getComputedStyle$1(element).position === "fixed") {
            return null;
        }
        return element.offsetParent;
    }
    function getContainingBlock(element) {
        var isFirefox = /firefox/i.test(getUAString());
        var isIE = /Trident/i.test(getUAString());
        if (isIE && isHTMLElement(element)) {
            var elementCss = getComputedStyle$1(element);
            if (elementCss.position === "fixed") {
                return null;
            }
        }
        var currentNode = getParentNode(element);
        if (isShadowRoot(currentNode)) {
            currentNode = currentNode.host;
        }
        while (isHTMLElement(currentNode) && [ "html", "body" ].indexOf(getNodeName(currentNode)) < 0) {
            var css = getComputedStyle$1(currentNode);
            if (css.transform !== "none" || css.perspective !== "none" || css.contain === "paint" || [ "transform", "perspective" ].indexOf(css.willChange) !== -1 || isFirefox && css.willChange === "filter" || isFirefox && css.filter && css.filter !== "none") {
                return currentNode;
            } else {
                currentNode = currentNode.parentNode;
            }
        }
        return null;
    }
    function getOffsetParent(element) {
        var window = getWindow(element);
        var offsetParent = getTrueOffsetParent(element);
        while (offsetParent && isTableElement(offsetParent) && getComputedStyle$1(offsetParent).position === "static") {
            offsetParent = getTrueOffsetParent(offsetParent);
        }
        if (offsetParent && (getNodeName(offsetParent) === "html" || getNodeName(offsetParent) === "body" && getComputedStyle$1(offsetParent).position === "static")) {
            return window;
        }
        return offsetParent || getContainingBlock(element) || window;
    }
    function getMainAxisFromPlacement(placement) {
        return [ "top", "bottom" ].indexOf(placement) >= 0 ? "x" : "y";
    }
    function within(min$1, value, max$1) {
        return max(min$1, min(value, max$1));
    }
    function withinMaxClamp(min, value, max) {
        var v = within(min, value, max);
        return v > max ? max : v;
    }
    function getFreshSideObject() {
        return {
            top: 0,
            right: 0,
            bottom: 0,
            left: 0
        };
    }
    function mergePaddingObject(paddingObject) {
        return Object.assign({}, getFreshSideObject(), paddingObject);
    }
    function expandToHashMap(value, keys) {
        return keys.reduce(function(hashMap, key) {
            hashMap[key] = value;
            return hashMap;
        }, {});
    }
    var toPaddingObject = function toPaddingObject(padding, state) {
        padding = typeof padding === "function" ? padding(Object.assign({}, state.rects, {
            placement: state.placement
        })) : padding;
        return mergePaddingObject(typeof padding !== "number" ? padding : expandToHashMap(padding, basePlacements));
    };
    function arrow(_ref) {
        var _state$modifiersData$;
        var state = _ref.state, name = _ref.name, options = _ref.options;
        var arrowElement = state.elements.arrow;
        var popperOffsets = state.modifiersData.popperOffsets;
        var basePlacement = getBasePlacement(state.placement);
        var axis = getMainAxisFromPlacement(basePlacement);
        var isVertical = [ left, right ].indexOf(basePlacement) >= 0;
        var len = isVertical ? "height" : "width";
        if (!arrowElement || !popperOffsets) {
            return;
        }
        var paddingObject = toPaddingObject(options.padding, state);
        var arrowRect = getLayoutRect(arrowElement);
        var minProp = axis === "y" ? top : left;
        var maxProp = axis === "y" ? bottom : right;
        var endDiff = state.rects.reference[len] + state.rects.reference[axis] - popperOffsets[axis] - state.rects.popper[len];
        var startDiff = popperOffsets[axis] - state.rects.reference[axis];
        var arrowOffsetParent = getOffsetParent(arrowElement);
        var clientSize = arrowOffsetParent ? axis === "y" ? arrowOffsetParent.clientHeight || 0 : arrowOffsetParent.clientWidth || 0 : 0;
        var centerToReference = endDiff / 2 - startDiff / 2;
        var min = paddingObject[minProp];
        var max = clientSize - arrowRect[len] - paddingObject[maxProp];
        var center = clientSize / 2 - arrowRect[len] / 2 + centerToReference;
        var offset = within(min, center, max);
        var axisProp = axis;
        state.modifiersData[name] = (_state$modifiersData$ = {}, _state$modifiersData$[axisProp] = offset, 
        _state$modifiersData$.centerOffset = offset - center, _state$modifiersData$);
    }
    function effect$1(_ref2) {
        var state = _ref2.state, options = _ref2.options;
        var _options$element = options.element, arrowElement = _options$element === void 0 ? "[data-popper-arrow]" : _options$element;
        if (arrowElement == null) {
            return;
        }
        if (typeof arrowElement === "string") {
            arrowElement = state.elements.popper.querySelector(arrowElement);
            if (!arrowElement) {
                return;
            }
        }
        if (!contains(state.elements.popper, arrowElement)) {
            return;
        }
        state.elements.arrow = arrowElement;
    }
    const arrow$1 = {
        name: "arrow",
        enabled: true,
        phase: "main",
        fn: arrow,
        effect: effect$1,
        requires: [ "popperOffsets" ],
        requiresIfExists: [ "preventOverflow" ]
    };
    function getVariation(placement) {
        return placement.split("-")[1];
    }
    var unsetSides = {
        top: "auto",
        right: "auto",
        bottom: "auto",
        left: "auto"
    };
    function roundOffsetsByDPR(_ref, win) {
        var x = _ref.x, y = _ref.y;
        var dpr = win.devicePixelRatio || 1;
        return {
            x: round(x * dpr) / dpr || 0,
            y: round(y * dpr) / dpr || 0
        };
    }
    function mapToStyles(_ref2) {
        var _Object$assign2;
        var popper = _ref2.popper, popperRect = _ref2.popperRect, placement = _ref2.placement, variation = _ref2.variation, offsets = _ref2.offsets, position = _ref2.position, gpuAcceleration = _ref2.gpuAcceleration, adaptive = _ref2.adaptive, roundOffsets = _ref2.roundOffsets, isFixed = _ref2.isFixed;
        var _offsets$x = offsets.x, x = _offsets$x === void 0 ? 0 : _offsets$x, _offsets$y = offsets.y, y = _offsets$y === void 0 ? 0 : _offsets$y;
        var _ref3 = typeof roundOffsets === "function" ? roundOffsets({
            x: x,
            y: y
        }) : {
            x: x,
            y: y
        };
        x = _ref3.x;
        y = _ref3.y;
        var hasX = offsets.hasOwnProperty("x");
        var hasY = offsets.hasOwnProperty("y");
        var sideX = left;
        var sideY = top;
        var win = window;
        if (adaptive) {
            var offsetParent = getOffsetParent(popper);
            var heightProp = "clientHeight";
            var widthProp = "clientWidth";
            if (offsetParent === getWindow(popper)) {
                offsetParent = getDocumentElement(popper);
                if (getComputedStyle$1(offsetParent).position !== "static" && position === "absolute") {
                    heightProp = "scrollHeight";
                    widthProp = "scrollWidth";
                }
            }
            offsetParent = offsetParent;
            if (placement === top || (placement === left || placement === right) && variation === end) {
                sideY = bottom;
                var offsetY = isFixed && offsetParent === win && win.visualViewport ? win.visualViewport.height : offsetParent[heightProp];
                y -= offsetY - popperRect.height;
                y *= gpuAcceleration ? 1 : -1;
            }
            if (placement === left || (placement === top || placement === bottom) && variation === end) {
                sideX = right;
                var offsetX = isFixed && offsetParent === win && win.visualViewport ? win.visualViewport.width : offsetParent[widthProp];
                x -= offsetX - popperRect.width;
                x *= gpuAcceleration ? 1 : -1;
            }
        }
        var commonStyles = Object.assign({
            position: position
        }, adaptive && unsetSides);
        var _ref4 = roundOffsets === true ? roundOffsetsByDPR({
            x: x,
            y: y
        }, getWindow(popper)) : {
            x: x,
            y: y
        };
        x = _ref4.x;
        y = _ref4.y;
        if (gpuAcceleration) {
            var _Object$assign;
            return Object.assign({}, commonStyles, (_Object$assign = {}, _Object$assign[sideY] = hasY ? "0" : "", 
            _Object$assign[sideX] = hasX ? "0" : "", _Object$assign.transform = (win.devicePixelRatio || 1) <= 1 ? "translate(" + x + "px, " + y + "px)" : "translate3d(" + x + "px, " + y + "px, 0)", 
            _Object$assign));
        }
        return Object.assign({}, commonStyles, (_Object$assign2 = {}, _Object$assign2[sideY] = hasY ? y + "px" : "", 
        _Object$assign2[sideX] = hasX ? x + "px" : "", _Object$assign2.transform = "", 
        _Object$assign2));
    }
    function computeStyles(_ref5) {
        var state = _ref5.state, options = _ref5.options;
        var _options$gpuAccelerat = options.gpuAcceleration, gpuAcceleration = _options$gpuAccelerat === void 0 ? true : _options$gpuAccelerat, _options$adaptive = options.adaptive, adaptive = _options$adaptive === void 0 ? true : _options$adaptive, _options$roundOffsets = options.roundOffsets, roundOffsets = _options$roundOffsets === void 0 ? true : _options$roundOffsets;
        var commonStyles = {
            placement: getBasePlacement(state.placement),
            variation: getVariation(state.placement),
            popper: state.elements.popper,
            popperRect: state.rects.popper,
            gpuAcceleration: gpuAcceleration,
            isFixed: state.options.strategy === "fixed"
        };
        if (state.modifiersData.popperOffsets != null) {
            state.styles.popper = Object.assign({}, state.styles.popper, mapToStyles(Object.assign({}, commonStyles, {
                offsets: state.modifiersData.popperOffsets,
                position: state.options.strategy,
                adaptive: adaptive,
                roundOffsets: roundOffsets
            })));
        }
        if (state.modifiersData.arrow != null) {
            state.styles.arrow = Object.assign({}, state.styles.arrow, mapToStyles(Object.assign({}, commonStyles, {
                offsets: state.modifiersData.arrow,
                position: "absolute",
                adaptive: false,
                roundOffsets: roundOffsets
            })));
        }
        state.attributes.popper = Object.assign({}, state.attributes.popper, {
            "data-popper-placement": state.placement
        });
    }
    const computeStyles$1 = {
        name: "computeStyles",
        enabled: true,
        phase: "beforeWrite",
        fn: computeStyles,
        data: {}
    };
    var passive = {
        passive: true
    };
    function effect(_ref) {
        var state = _ref.state, instance = _ref.instance, options = _ref.options;
        var _options$scroll = options.scroll, scroll = _options$scroll === void 0 ? true : _options$scroll, _options$resize = options.resize, resize = _options$resize === void 0 ? true : _options$resize;
        var window = getWindow(state.elements.popper);
        var scrollParents = [].concat(state.scrollParents.reference, state.scrollParents.popper);
        if (scroll) {
            scrollParents.forEach(function(scrollParent) {
                scrollParent.addEventListener("scroll", instance.update, passive);
            });
        }
        if (resize) {
            window.addEventListener("resize", instance.update, passive);
        }
        return function() {
            if (scroll) {
                scrollParents.forEach(function(scrollParent) {
                    scrollParent.removeEventListener("scroll", instance.update, passive);
                });
            }
            if (resize) {
                window.removeEventListener("resize", instance.update, passive);
            }
        };
    }
    const eventListeners = {
        name: "eventListeners",
        enabled: true,
        phase: "write",
        fn: function fn() {},
        effect: effect,
        data: {}
    };
    var hash$1 = {
        left: "right",
        right: "left",
        bottom: "top",
        top: "bottom"
    };
    function getOppositePlacement(placement) {
        return placement.replace(/left|right|bottom|top/g, function(matched) {
            return hash$1[matched];
        });
    }
    var hash = {
        start: "end",
        end: "start"
    };
    function getOppositeVariationPlacement(placement) {
        return placement.replace(/start|end/g, function(matched) {
            return hash[matched];
        });
    }
    function getWindowScroll(node) {
        var win = getWindow(node);
        var scrollLeft = win.pageXOffset;
        var scrollTop = win.pageYOffset;
        return {
            scrollLeft: scrollLeft,
            scrollTop: scrollTop
        };
    }
    function getWindowScrollBarX(element) {
        return getBoundingClientRect(getDocumentElement(element)).left + getWindowScroll(element).scrollLeft;
    }
    function getViewportRect(element, strategy) {
        var win = getWindow(element);
        var html = getDocumentElement(element);
        var visualViewport = win.visualViewport;
        var width = html.clientWidth;
        var height = html.clientHeight;
        var x = 0;
        var y = 0;
        if (visualViewport) {
            width = visualViewport.width;
            height = visualViewport.height;
            var layoutViewport = isLayoutViewport();
            if (layoutViewport || !layoutViewport && strategy === "fixed") {
                x = visualViewport.offsetLeft;
                y = visualViewport.offsetTop;
            }
        }
        return {
            width: width,
            height: height,
            x: x + getWindowScrollBarX(element),
            y: y
        };
    }
    function getDocumentRect(element) {
        var _element$ownerDocumen;
        var html = getDocumentElement(element);
        var winScroll = getWindowScroll(element);
        var body = (_element$ownerDocumen = element.ownerDocument) == null ? void 0 : _element$ownerDocumen.body;
        var width = max(html.scrollWidth, html.clientWidth, body ? body.scrollWidth : 0, body ? body.clientWidth : 0);
        var height = max(html.scrollHeight, html.clientHeight, body ? body.scrollHeight : 0, body ? body.clientHeight : 0);
        var x = -winScroll.scrollLeft + getWindowScrollBarX(element);
        var y = -winScroll.scrollTop;
        if (getComputedStyle$1(body || html).direction === "rtl") {
            x += max(html.clientWidth, body ? body.clientWidth : 0) - width;
        }
        return {
            width: width,
            height: height,
            x: x,
            y: y
        };
    }
    function isScrollParent(element) {
        var _getComputedStyle = getComputedStyle$1(element), overflow = _getComputedStyle.overflow, overflowX = _getComputedStyle.overflowX, overflowY = _getComputedStyle.overflowY;
        return /auto|scroll|overlay|hidden/.test(overflow + overflowY + overflowX);
    }
    function getScrollParent(node) {
        if ([ "html", "body", "#document" ].indexOf(getNodeName(node)) >= 0) {
            return node.ownerDocument.body;
        }
        if (isHTMLElement(node) && isScrollParent(node)) {
            return node;
        }
        return getScrollParent(getParentNode(node));
    }
    function listScrollParents(element, list) {
        var _element$ownerDocumen;
        if (list === void 0) {
            list = [];
        }
        var scrollParent = getScrollParent(element);
        var isBody = scrollParent === ((_element$ownerDocumen = element.ownerDocument) == null ? void 0 : _element$ownerDocumen.body);
        var win = getWindow(scrollParent);
        var target = isBody ? [ win ].concat(win.visualViewport || [], isScrollParent(scrollParent) ? scrollParent : []) : scrollParent;
        var updatedList = list.concat(target);
        return isBody ? updatedList : updatedList.concat(listScrollParents(getParentNode(target)));
    }
    function rectToClientRect(rect) {
        return Object.assign({}, rect, {
            left: rect.x,
            top: rect.y,
            right: rect.x + rect.width,
            bottom: rect.y + rect.height
        });
    }
    function getInnerBoundingClientRect(element, strategy) {
        var rect = getBoundingClientRect(element, false, strategy === "fixed");
        rect.top = rect.top + element.clientTop;
        rect.left = rect.left + element.clientLeft;
        rect.bottom = rect.top + element.clientHeight;
        rect.right = rect.left + element.clientWidth;
        rect.width = element.clientWidth;
        rect.height = element.clientHeight;
        rect.x = rect.left;
        rect.y = rect.top;
        return rect;
    }
    function getClientRectFromMixedType(element, clippingParent, strategy) {
        return clippingParent === viewport ? rectToClientRect(getViewportRect(element, strategy)) : isElement(clippingParent) ? getInnerBoundingClientRect(clippingParent, strategy) : rectToClientRect(getDocumentRect(getDocumentElement(element)));
    }
    function getClippingParents(element) {
        var clippingParents = listScrollParents(getParentNode(element));
        var canEscapeClipping = [ "absolute", "fixed" ].indexOf(getComputedStyle$1(element).position) >= 0;
        var clipperElement = canEscapeClipping && isHTMLElement(element) ? getOffsetParent(element) : element;
        if (!isElement(clipperElement)) {
            return [];
        }
        return clippingParents.filter(function(clippingParent) {
            return isElement(clippingParent) && contains(clippingParent, clipperElement) && getNodeName(clippingParent) !== "body";
        });
    }
    function getClippingRect(element, boundary, rootBoundary, strategy) {
        var mainClippingParents = boundary === "clippingParents" ? getClippingParents(element) : [].concat(boundary);
        var clippingParents = [].concat(mainClippingParents, [ rootBoundary ]);
        var firstClippingParent = clippingParents[0];
        var clippingRect = clippingParents.reduce(function(accRect, clippingParent) {
            var rect = getClientRectFromMixedType(element, clippingParent, strategy);
            accRect.top = max(rect.top, accRect.top);
            accRect.right = min(rect.right, accRect.right);
            accRect.bottom = min(rect.bottom, accRect.bottom);
            accRect.left = max(rect.left, accRect.left);
            return accRect;
        }, getClientRectFromMixedType(element, firstClippingParent, strategy));
        clippingRect.width = clippingRect.right - clippingRect.left;
        clippingRect.height = clippingRect.bottom - clippingRect.top;
        clippingRect.x = clippingRect.left;
        clippingRect.y = clippingRect.top;
        return clippingRect;
    }
    function computeOffsets(_ref) {
        var reference = _ref.reference, element = _ref.element, placement = _ref.placement;
        var basePlacement = placement ? getBasePlacement(placement) : null;
        var variation = placement ? getVariation(placement) : null;
        var commonX = reference.x + reference.width / 2 - element.width / 2;
        var commonY = reference.y + reference.height / 2 - element.height / 2;
        var offsets;
        switch (basePlacement) {
          case top:
            offsets = {
                x: commonX,
                y: reference.y - element.height
            };
            break;

          case bottom:
            offsets = {
                x: commonX,
                y: reference.y + reference.height
            };
            break;

          case right:
            offsets = {
                x: reference.x + reference.width,
                y: commonY
            };
            break;

          case left:
            offsets = {
                x: reference.x - element.width,
                y: commonY
            };
            break;

          default:
            offsets = {
                x: reference.x,
                y: reference.y
            };
        }
        var mainAxis = basePlacement ? getMainAxisFromPlacement(basePlacement) : null;
        if (mainAxis != null) {
            var len = mainAxis === "y" ? "height" : "width";
            switch (variation) {
              case start:
                offsets[mainAxis] = offsets[mainAxis] - (reference[len] / 2 - element[len] / 2);
                break;

              case end:
                offsets[mainAxis] = offsets[mainAxis] + (reference[len] / 2 - element[len] / 2);
                break;
            }
        }
        return offsets;
    }
    function detectOverflow(state, options) {
        if (options === void 0) {
            options = {};
        }
        var _options = options, _options$placement = _options.placement, placement = _options$placement === void 0 ? state.placement : _options$placement, _options$strategy = _options.strategy, strategy = _options$strategy === void 0 ? state.strategy : _options$strategy, _options$boundary = _options.boundary, boundary = _options$boundary === void 0 ? clippingParents : _options$boundary, _options$rootBoundary = _options.rootBoundary, rootBoundary = _options$rootBoundary === void 0 ? viewport : _options$rootBoundary, _options$elementConte = _options.elementContext, elementContext = _options$elementConte === void 0 ? popper : _options$elementConte, _options$altBoundary = _options.altBoundary, altBoundary = _options$altBoundary === void 0 ? false : _options$altBoundary, _options$padding = _options.padding, padding = _options$padding === void 0 ? 0 : _options$padding;
        var paddingObject = mergePaddingObject(typeof padding !== "number" ? padding : expandToHashMap(padding, basePlacements));
        var altContext = elementContext === popper ? reference : popper;
        var popperRect = state.rects.popper;
        var element = state.elements[altBoundary ? altContext : elementContext];
        var clippingClientRect = getClippingRect(isElement(element) ? element : element.contextElement || getDocumentElement(state.elements.popper), boundary, rootBoundary, strategy);
        var referenceClientRect = getBoundingClientRect(state.elements.reference);
        var popperOffsets = computeOffsets({
            reference: referenceClientRect,
            element: popperRect,
            strategy: "absolute",
            placement: placement
        });
        var popperClientRect = rectToClientRect(Object.assign({}, popperRect, popperOffsets));
        var elementClientRect = elementContext === popper ? popperClientRect : referenceClientRect;
        var overflowOffsets = {
            top: clippingClientRect.top - elementClientRect.top + paddingObject.top,
            bottom: elementClientRect.bottom - clippingClientRect.bottom + paddingObject.bottom,
            left: clippingClientRect.left - elementClientRect.left + paddingObject.left,
            right: elementClientRect.right - clippingClientRect.right + paddingObject.right
        };
        var offsetData = state.modifiersData.offset;
        if (elementContext === popper && offsetData) {
            var offset = offsetData[placement];
            Object.keys(overflowOffsets).forEach(function(key) {
                var multiply = [ right, bottom ].indexOf(key) >= 0 ? 1 : -1;
                var axis = [ top, bottom ].indexOf(key) >= 0 ? "y" : "x";
                overflowOffsets[key] += offset[axis] * multiply;
            });
        }
        return overflowOffsets;
    }
    function computeAutoPlacement(state, options) {
        if (options === void 0) {
            options = {};
        }
        var _options = options, placement = _options.placement, boundary = _options.boundary, rootBoundary = _options.rootBoundary, padding = _options.padding, flipVariations = _options.flipVariations, _options$allowedAutoP = _options.allowedAutoPlacements, allowedAutoPlacements = _options$allowedAutoP === void 0 ? placements : _options$allowedAutoP;
        var variation = getVariation(placement);
        var placements$1 = variation ? flipVariations ? variationPlacements : variationPlacements.filter(function(placement) {
            return getVariation(placement) === variation;
        }) : basePlacements;
        var allowedPlacements = placements$1.filter(function(placement) {
            return allowedAutoPlacements.indexOf(placement) >= 0;
        });
        if (allowedPlacements.length === 0) {
            allowedPlacements = placements$1;
        }
        var overflows = allowedPlacements.reduce(function(acc, placement) {
            acc[placement] = detectOverflow(state, {
                placement: placement,
                boundary: boundary,
                rootBoundary: rootBoundary,
                padding: padding
            })[getBasePlacement(placement)];
            return acc;
        }, {});
        return Object.keys(overflows).sort(function(a, b) {
            return overflows[a] - overflows[b];
        });
    }
    function getExpandedFallbackPlacements(placement) {
        if (getBasePlacement(placement) === auto) {
            return [];
        }
        var oppositePlacement = getOppositePlacement(placement);
        return [ getOppositeVariationPlacement(placement), oppositePlacement, getOppositeVariationPlacement(oppositePlacement) ];
    }
    function flip(_ref) {
        var state = _ref.state, options = _ref.options, name = _ref.name;
        if (state.modifiersData[name]._skip) {
            return;
        }
        var _options$mainAxis = options.mainAxis, checkMainAxis = _options$mainAxis === void 0 ? true : _options$mainAxis, _options$altAxis = options.altAxis, checkAltAxis = _options$altAxis === void 0 ? true : _options$altAxis, specifiedFallbackPlacements = options.fallbackPlacements, padding = options.padding, boundary = options.boundary, rootBoundary = options.rootBoundary, altBoundary = options.altBoundary, _options$flipVariatio = options.flipVariations, flipVariations = _options$flipVariatio === void 0 ? true : _options$flipVariatio, allowedAutoPlacements = options.allowedAutoPlacements;
        var preferredPlacement = state.options.placement;
        var basePlacement = getBasePlacement(preferredPlacement);
        var isBasePlacement = basePlacement === preferredPlacement;
        var fallbackPlacements = specifiedFallbackPlacements || (isBasePlacement || !flipVariations ? [ getOppositePlacement(preferredPlacement) ] : getExpandedFallbackPlacements(preferredPlacement));
        var placements = [ preferredPlacement ].concat(fallbackPlacements).reduce(function(acc, placement) {
            return acc.concat(getBasePlacement(placement) === auto ? computeAutoPlacement(state, {
                placement: placement,
                boundary: boundary,
                rootBoundary: rootBoundary,
                padding: padding,
                flipVariations: flipVariations,
                allowedAutoPlacements: allowedAutoPlacements
            }) : placement);
        }, []);
        var referenceRect = state.rects.reference;
        var popperRect = state.rects.popper;
        var checksMap = new Map();
        var makeFallbackChecks = true;
        var firstFittingPlacement = placements[0];
        for (var i = 0; i < placements.length; i++) {
            var placement = placements[i];
            var _basePlacement = getBasePlacement(placement);
            var isStartVariation = getVariation(placement) === start;
            var isVertical = [ top, bottom ].indexOf(_basePlacement) >= 0;
            var len = isVertical ? "width" : "height";
            var overflow = detectOverflow(state, {
                placement: placement,
                boundary: boundary,
                rootBoundary: rootBoundary,
                altBoundary: altBoundary,
                padding: padding
            });
            var mainVariationSide = isVertical ? isStartVariation ? right : left : isStartVariation ? bottom : top;
            if (referenceRect[len] > popperRect[len]) {
                mainVariationSide = getOppositePlacement(mainVariationSide);
            }
            var altVariationSide = getOppositePlacement(mainVariationSide);
            var checks = [];
            if (checkMainAxis) {
                checks.push(overflow[_basePlacement] <= 0);
            }
            if (checkAltAxis) {
                checks.push(overflow[mainVariationSide] <= 0, overflow[altVariationSide] <= 0);
            }
            if (checks.every(function(check) {
                return check;
            })) {
                firstFittingPlacement = placement;
                makeFallbackChecks = false;
                break;
            }
            checksMap.set(placement, checks);
        }
        if (makeFallbackChecks) {
            var numberOfChecks = flipVariations ? 3 : 1;
            var _loop = function _loop(_i) {
                var fittingPlacement = placements.find(function(placement) {
                    var checks = checksMap.get(placement);
                    if (checks) {
                        return checks.slice(0, _i).every(function(check) {
                            return check;
                        });
                    }
                });
                if (fittingPlacement) {
                    firstFittingPlacement = fittingPlacement;
                    return "break";
                }
            };
            for (var _i = numberOfChecks; _i > 0; _i--) {
                var _ret = _loop(_i);
                if (_ret === "break") break;
            }
        }
        if (state.placement !== firstFittingPlacement) {
            state.modifiersData[name]._skip = true;
            state.placement = firstFittingPlacement;
            state.reset = true;
        }
    }
    const flip$1 = {
        name: "flip",
        enabled: true,
        phase: "main",
        fn: flip,
        requiresIfExists: [ "offset" ],
        data: {
            _skip: false
        }
    };
    function getSideOffsets(overflow, rect, preventedOffsets) {
        if (preventedOffsets === void 0) {
            preventedOffsets = {
                x: 0,
                y: 0
            };
        }
        return {
            top: overflow.top - rect.height - preventedOffsets.y,
            right: overflow.right - rect.width + preventedOffsets.x,
            bottom: overflow.bottom - rect.height + preventedOffsets.y,
            left: overflow.left - rect.width - preventedOffsets.x
        };
    }
    function isAnySideFullyClipped(overflow) {
        return [ top, right, bottom, left ].some(function(side) {
            return overflow[side] >= 0;
        });
    }
    function hide(_ref) {
        var state = _ref.state, name = _ref.name;
        var referenceRect = state.rects.reference;
        var popperRect = state.rects.popper;
        var preventedOffsets = state.modifiersData.preventOverflow;
        var referenceOverflow = detectOverflow(state, {
            elementContext: "reference"
        });
        var popperAltOverflow = detectOverflow(state, {
            altBoundary: true
        });
        var referenceClippingOffsets = getSideOffsets(referenceOverflow, referenceRect);
        var popperEscapeOffsets = getSideOffsets(popperAltOverflow, popperRect, preventedOffsets);
        var isReferenceHidden = isAnySideFullyClipped(referenceClippingOffsets);
        var hasPopperEscaped = isAnySideFullyClipped(popperEscapeOffsets);
        state.modifiersData[name] = {
            referenceClippingOffsets: referenceClippingOffsets,
            popperEscapeOffsets: popperEscapeOffsets,
            isReferenceHidden: isReferenceHidden,
            hasPopperEscaped: hasPopperEscaped
        };
        state.attributes.popper = Object.assign({}, state.attributes.popper, {
            "data-popper-reference-hidden": isReferenceHidden,
            "data-popper-escaped": hasPopperEscaped
        });
    }
    const hide$1 = {
        name: "hide",
        enabled: true,
        phase: "main",
        requiresIfExists: [ "preventOverflow" ],
        fn: hide
    };
    function distanceAndSkiddingToXY(placement, rects, offset) {
        var basePlacement = getBasePlacement(placement);
        var invertDistance = [ left, top ].indexOf(basePlacement) >= 0 ? -1 : 1;
        var _ref = typeof offset === "function" ? offset(Object.assign({}, rects, {
            placement: placement
        })) : offset, skidding = _ref[0], distance = _ref[1];
        skidding = skidding || 0;
        distance = (distance || 0) * invertDistance;
        return [ left, right ].indexOf(basePlacement) >= 0 ? {
            x: distance,
            y: skidding
        } : {
            x: skidding,
            y: distance
        };
    }
    function offset(_ref2) {
        var state = _ref2.state, options = _ref2.options, name = _ref2.name;
        var _options$offset = options.offset, offset = _options$offset === void 0 ? [ 0, 0 ] : _options$offset;
        var data = placements.reduce(function(acc, placement) {
            acc[placement] = distanceAndSkiddingToXY(placement, state.rects, offset);
            return acc;
        }, {});
        var _data$state$placement = data[state.placement], x = _data$state$placement.x, y = _data$state$placement.y;
        if (state.modifiersData.popperOffsets != null) {
            state.modifiersData.popperOffsets.x += x;
            state.modifiersData.popperOffsets.y += y;
        }
        state.modifiersData[name] = data;
    }
    const offset$1 = {
        name: "offset",
        enabled: true,
        phase: "main",
        requires: [ "popperOffsets" ],
        fn: offset
    };
    function popperOffsets(_ref) {
        var state = _ref.state, name = _ref.name;
        state.modifiersData[name] = computeOffsets({
            reference: state.rects.reference,
            element: state.rects.popper,
            strategy: "absolute",
            placement: state.placement
        });
    }
    const popperOffsets$1 = {
        name: "popperOffsets",
        enabled: true,
        phase: "read",
        fn: popperOffsets,
        data: {}
    };
    function getAltAxis(axis) {
        return axis === "x" ? "y" : "x";
    }
    function preventOverflow(_ref) {
        var state = _ref.state, options = _ref.options, name = _ref.name;
        var _options$mainAxis = options.mainAxis, checkMainAxis = _options$mainAxis === void 0 ? true : _options$mainAxis, _options$altAxis = options.altAxis, checkAltAxis = _options$altAxis === void 0 ? false : _options$altAxis, boundary = options.boundary, rootBoundary = options.rootBoundary, altBoundary = options.altBoundary, padding = options.padding, _options$tether = options.tether, tether = _options$tether === void 0 ? true : _options$tether, _options$tetherOffset = options.tetherOffset, tetherOffset = _options$tetherOffset === void 0 ? 0 : _options$tetherOffset;
        var overflow = detectOverflow(state, {
            boundary: boundary,
            rootBoundary: rootBoundary,
            padding: padding,
            altBoundary: altBoundary
        });
        var basePlacement = getBasePlacement(state.placement);
        var variation = getVariation(state.placement);
        var isBasePlacement = !variation;
        var mainAxis = getMainAxisFromPlacement(basePlacement);
        var altAxis = getAltAxis(mainAxis);
        var popperOffsets = state.modifiersData.popperOffsets;
        var referenceRect = state.rects.reference;
        var popperRect = state.rects.popper;
        var tetherOffsetValue = typeof tetherOffset === "function" ? tetherOffset(Object.assign({}, state.rects, {
            placement: state.placement
        })) : tetherOffset;
        var normalizedTetherOffsetValue = typeof tetherOffsetValue === "number" ? {
            mainAxis: tetherOffsetValue,
            altAxis: tetherOffsetValue
        } : Object.assign({
            mainAxis: 0,
            altAxis: 0
        }, tetherOffsetValue);
        var offsetModifierState = state.modifiersData.offset ? state.modifiersData.offset[state.placement] : null;
        var data = {
            x: 0,
            y: 0
        };
        if (!popperOffsets) {
            return;
        }
        if (checkMainAxis) {
            var _offsetModifierState$;
            var mainSide = mainAxis === "y" ? top : left;
            var altSide = mainAxis === "y" ? bottom : right;
            var len = mainAxis === "y" ? "height" : "width";
            var offset = popperOffsets[mainAxis];
            var min$1 = offset + overflow[mainSide];
            var max$1 = offset - overflow[altSide];
            var additive = tether ? -popperRect[len] / 2 : 0;
            var minLen = variation === start ? referenceRect[len] : popperRect[len];
            var maxLen = variation === start ? -popperRect[len] : -referenceRect[len];
            var arrowElement = state.elements.arrow;
            var arrowRect = tether && arrowElement ? getLayoutRect(arrowElement) : {
                width: 0,
                height: 0
            };
            var arrowPaddingObject = state.modifiersData["arrow#persistent"] ? state.modifiersData["arrow#persistent"].padding : getFreshSideObject();
            var arrowPaddingMin = arrowPaddingObject[mainSide];
            var arrowPaddingMax = arrowPaddingObject[altSide];
            var arrowLen = within(0, referenceRect[len], arrowRect[len]);
            var minOffset = isBasePlacement ? referenceRect[len] / 2 - additive - arrowLen - arrowPaddingMin - normalizedTetherOffsetValue.mainAxis : minLen - arrowLen - arrowPaddingMin - normalizedTetherOffsetValue.mainAxis;
            var maxOffset = isBasePlacement ? -referenceRect[len] / 2 + additive + arrowLen + arrowPaddingMax + normalizedTetherOffsetValue.mainAxis : maxLen + arrowLen + arrowPaddingMax + normalizedTetherOffsetValue.mainAxis;
            var arrowOffsetParent = state.elements.arrow && getOffsetParent(state.elements.arrow);
            var clientOffset = arrowOffsetParent ? mainAxis === "y" ? arrowOffsetParent.clientTop || 0 : arrowOffsetParent.clientLeft || 0 : 0;
            var offsetModifierValue = (_offsetModifierState$ = offsetModifierState == null ? void 0 : offsetModifierState[mainAxis]) != null ? _offsetModifierState$ : 0;
            var tetherMin = offset + minOffset - offsetModifierValue - clientOffset;
            var tetherMax = offset + maxOffset - offsetModifierValue;
            var preventedOffset = within(tether ? min(min$1, tetherMin) : min$1, offset, tether ? max(max$1, tetherMax) : max$1);
            popperOffsets[mainAxis] = preventedOffset;
            data[mainAxis] = preventedOffset - offset;
        }
        if (checkAltAxis) {
            var _offsetModifierState$2;
            var _mainSide = mainAxis === "x" ? top : left;
            var _altSide = mainAxis === "x" ? bottom : right;
            var _offset = popperOffsets[altAxis];
            var _len = altAxis === "y" ? "height" : "width";
            var _min = _offset + overflow[_mainSide];
            var _max = _offset - overflow[_altSide];
            var isOriginSide = [ top, left ].indexOf(basePlacement) !== -1;
            var _offsetModifierValue = (_offsetModifierState$2 = offsetModifierState == null ? void 0 : offsetModifierState[altAxis]) != null ? _offsetModifierState$2 : 0;
            var _tetherMin = isOriginSide ? _min : _offset - referenceRect[_len] - popperRect[_len] - _offsetModifierValue + normalizedTetherOffsetValue.altAxis;
            var _tetherMax = isOriginSide ? _offset + referenceRect[_len] + popperRect[_len] - _offsetModifierValue - normalizedTetherOffsetValue.altAxis : _max;
            var _preventedOffset = tether && isOriginSide ? withinMaxClamp(_tetherMin, _offset, _tetherMax) : within(tether ? _tetherMin : _min, _offset, tether ? _tetherMax : _max);
            popperOffsets[altAxis] = _preventedOffset;
            data[altAxis] = _preventedOffset - _offset;
        }
        state.modifiersData[name] = data;
    }
    const preventOverflow$1 = {
        name: "preventOverflow",
        enabled: true,
        phase: "main",
        fn: preventOverflow,
        requiresIfExists: [ "offset" ]
    };
    function getHTMLElementScroll(element) {
        return {
            scrollLeft: element.scrollLeft,
            scrollTop: element.scrollTop
        };
    }
    function getNodeScroll(node) {
        if (node === getWindow(node) || !isHTMLElement(node)) {
            return getWindowScroll(node);
        } else {
            return getHTMLElementScroll(node);
        }
    }
    function isElementScaled(element) {
        var rect = element.getBoundingClientRect();
        var scaleX = round(rect.width) / element.offsetWidth || 1;
        var scaleY = round(rect.height) / element.offsetHeight || 1;
        return scaleX !== 1 || scaleY !== 1;
    }
    function getCompositeRect(elementOrVirtualElement, offsetParent, isFixed) {
        if (isFixed === void 0) {
            isFixed = false;
        }
        var isOffsetParentAnElement = isHTMLElement(offsetParent);
        var offsetParentIsScaled = isHTMLElement(offsetParent) && isElementScaled(offsetParent);
        var documentElement = getDocumentElement(offsetParent);
        var rect = getBoundingClientRect(elementOrVirtualElement, offsetParentIsScaled, isFixed);
        var scroll = {
            scrollLeft: 0,
            scrollTop: 0
        };
        var offsets = {
            x: 0,
            y: 0
        };
        if (isOffsetParentAnElement || !isOffsetParentAnElement && !isFixed) {
            if (getNodeName(offsetParent) !== "body" || isScrollParent(documentElement)) {
                scroll = getNodeScroll(offsetParent);
            }
            if (isHTMLElement(offsetParent)) {
                offsets = getBoundingClientRect(offsetParent, true);
                offsets.x += offsetParent.clientLeft;
                offsets.y += offsetParent.clientTop;
            } else if (documentElement) {
                offsets.x = getWindowScrollBarX(documentElement);
            }
        }
        return {
            x: rect.left + scroll.scrollLeft - offsets.x,
            y: rect.top + scroll.scrollTop - offsets.y,
            width: rect.width,
            height: rect.height
        };
    }
    function order(modifiers) {
        var map = new Map();
        var visited = new Set();
        var result = [];
        modifiers.forEach(function(modifier) {
            map.set(modifier.name, modifier);
        });
        function sort(modifier) {
            visited.add(modifier.name);
            var requires = [].concat(modifier.requires || [], modifier.requiresIfExists || []);
            requires.forEach(function(dep) {
                if (!visited.has(dep)) {
                    var depModifier = map.get(dep);
                    if (depModifier) {
                        sort(depModifier);
                    }
                }
            });
            result.push(modifier);
        }
        modifiers.forEach(function(modifier) {
            if (!visited.has(modifier.name)) {
                sort(modifier);
            }
        });
        return result;
    }
    function orderModifiers(modifiers) {
        var orderedModifiers = order(modifiers);
        return modifierPhases.reduce(function(acc, phase) {
            return acc.concat(orderedModifiers.filter(function(modifier) {
                return modifier.phase === phase;
            }));
        }, []);
    }
    function debounce(fn) {
        var pending;
        return function() {
            if (!pending) {
                pending = new Promise(function(resolve) {
                    Promise.resolve().then(function() {
                        pending = undefined;
                        resolve(fn());
                    });
                });
            }
            return pending;
        };
    }
    function mergeByName(modifiers) {
        var merged = modifiers.reduce(function(merged, current) {
            var existing = merged[current.name];
            merged[current.name] = existing ? Object.assign({}, existing, current, {
                options: Object.assign({}, existing.options, current.options),
                data: Object.assign({}, existing.data, current.data)
            }) : current;
            return merged;
        }, {});
        return Object.keys(merged).map(function(key) {
            return merged[key];
        });
    }
    var DEFAULT_OPTIONS = {
        placement: "bottom",
        modifiers: [],
        strategy: "absolute"
    };
    function areValidElements() {
        for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
            args[_key] = arguments[_key];
        }
        return !args.some(function(element) {
            return !(element && typeof element.getBoundingClientRect === "function");
        });
    }
    function popperGenerator(generatorOptions) {
        if (generatorOptions === void 0) {
            generatorOptions = {};
        }
        var _generatorOptions = generatorOptions, _generatorOptions$def = _generatorOptions.defaultModifiers, defaultModifiers = _generatorOptions$def === void 0 ? [] : _generatorOptions$def, _generatorOptions$def2 = _generatorOptions.defaultOptions, defaultOptions = _generatorOptions$def2 === void 0 ? DEFAULT_OPTIONS : _generatorOptions$def2;
        return function createPopper(reference, popper, options) {
            if (options === void 0) {
                options = defaultOptions;
            }
            var state = {
                placement: "bottom",
                orderedModifiers: [],
                options: Object.assign({}, DEFAULT_OPTIONS, defaultOptions),
                modifiersData: {},
                elements: {
                    reference: reference,
                    popper: popper
                },
                attributes: {},
                styles: {}
            };
            var effectCleanupFns = [];
            var isDestroyed = false;
            var instance = {
                state: state,
                setOptions: function setOptions(setOptionsAction) {
                    var options = typeof setOptionsAction === "function" ? setOptionsAction(state.options) : setOptionsAction;
                    cleanupModifierEffects();
                    state.options = Object.assign({}, defaultOptions, state.options, options);
                    state.scrollParents = {
                        reference: isElement(reference) ? listScrollParents(reference) : reference.contextElement ? listScrollParents(reference.contextElement) : [],
                        popper: listScrollParents(popper)
                    };
                    var orderedModifiers = orderModifiers(mergeByName([].concat(defaultModifiers, state.options.modifiers)));
                    state.orderedModifiers = orderedModifiers.filter(function(m) {
                        return m.enabled;
                    });
                    runModifierEffects();
                    return instance.update();
                },
                forceUpdate: function forceUpdate() {
                    if (isDestroyed) {
                        return;
                    }
                    var _state$elements = state.elements, reference = _state$elements.reference, popper = _state$elements.popper;
                    if (!areValidElements(reference, popper)) {
                        return;
                    }
                    state.rects = {
                        reference: getCompositeRect(reference, getOffsetParent(popper), state.options.strategy === "fixed"),
                        popper: getLayoutRect(popper)
                    };
                    state.reset = false;
                    state.placement = state.options.placement;
                    state.orderedModifiers.forEach(function(modifier) {
                        return state.modifiersData[modifier.name] = Object.assign({}, modifier.data);
                    });
                    for (var index = 0; index < state.orderedModifiers.length; index++) {
                        if (state.reset === true) {
                            state.reset = false;
                            index = -1;
                            continue;
                        }
                        var _state$orderedModifie = state.orderedModifiers[index], fn = _state$orderedModifie.fn, _state$orderedModifie2 = _state$orderedModifie.options, _options = _state$orderedModifie2 === void 0 ? {} : _state$orderedModifie2, name = _state$orderedModifie.name;
                        if (typeof fn === "function") {
                            state = fn({
                                state: state,
                                options: _options,
                                name: name,
                                instance: instance
                            }) || state;
                        }
                    }
                },
                update: debounce(function() {
                    return new Promise(function(resolve) {
                        instance.forceUpdate();
                        resolve(state);
                    });
                }),
                destroy: function destroy() {
                    cleanupModifierEffects();
                    isDestroyed = true;
                }
            };
            if (!areValidElements(reference, popper)) {
                return instance;
            }
            instance.setOptions(options).then(function(state) {
                if (!isDestroyed && options.onFirstUpdate) {
                    options.onFirstUpdate(state);
                }
            });
            function runModifierEffects() {
                state.orderedModifiers.forEach(function(_ref) {
                    var name = _ref.name, _ref$options = _ref.options, options = _ref$options === void 0 ? {} : _ref$options, effect = _ref.effect;
                    if (typeof effect === "function") {
                        var cleanupFn = effect({
                            state: state,
                            name: name,
                            instance: instance,
                            options: options
                        });
                        var noopFn = function noopFn() {};
                        effectCleanupFns.push(cleanupFn || noopFn);
                    }
                });
            }
            function cleanupModifierEffects() {
                effectCleanupFns.forEach(function(fn) {
                    return fn();
                });
                effectCleanupFns = [];
            }
            return instance;
        };
    }
    var createPopper$2 = popperGenerator();
    var defaultModifiers$1 = [ eventListeners, popperOffsets$1, computeStyles$1, applyStyles$1 ];
    var createPopper$1 = popperGenerator({
        defaultModifiers: defaultModifiers$1
    });
    var defaultModifiers = [ eventListeners, popperOffsets$1, computeStyles$1, applyStyles$1, offset$1, flip$1, preventOverflow$1, arrow$1, hide$1 ];
    var createPopper = popperGenerator({
        defaultModifiers: defaultModifiers
    });
    const Popper = Object.freeze(Object.defineProperty({
        __proto__: null,
        afterMain: afterMain,
        afterRead: afterRead,
        afterWrite: afterWrite,
        applyStyles: applyStyles$1,
        arrow: arrow$1,
        auto: auto,
        basePlacements: basePlacements,
        beforeMain: beforeMain,
        beforeRead: beforeRead,
        beforeWrite: beforeWrite,
        bottom: bottom,
        clippingParents: clippingParents,
        computeStyles: computeStyles$1,
        createPopper: createPopper,
        createPopperBase: createPopper$2,
        createPopperLite: createPopper$1,
        detectOverflow: detectOverflow,
        end: end,
        eventListeners: eventListeners,
        flip: flip$1,
        hide: hide$1,
        left: left,
        main: main,
        modifierPhases: modifierPhases,
        offset: offset$1,
        placements: placements,
        popper: popper,
        popperGenerator: popperGenerator,
        popperOffsets: popperOffsets$1,
        preventOverflow: preventOverflow$1,
        read: read,
        reference: reference,
        right: right,
        start: start,
        top: top,
        variationPlacements: variationPlacements,
        viewport: viewport,
        write: write
    }, Symbol.toStringTag, {
        value: "Module"
    }));
    const NAME$a = "dropdown";
    const DATA_KEY$6 = "bs.dropdown";
    const EVENT_KEY$6 = `.${DATA_KEY$6}`;
    const DATA_API_KEY$3 = ".data-api";
    const ESCAPE_KEY$2 = "Escape";
    const TAB_KEY$1 = "Tab";
    const ARROW_UP_KEY$1 = "ArrowUp";
    const ARROW_DOWN_KEY$1 = "ArrowDown";
    const RIGHT_MOUSE_BUTTON = 2;
    const EVENT_HIDE$5 = `hide${EVENT_KEY$6}`;
    const EVENT_HIDDEN$5 = `hidden${EVENT_KEY$6}`;
    const EVENT_SHOW$5 = `show${EVENT_KEY$6}`;
    const EVENT_SHOWN$5 = `shown${EVENT_KEY$6}`;
    const EVENT_CLICK_DATA_API$3 = `click${EVENT_KEY$6}${DATA_API_KEY$3}`;
    const EVENT_KEYDOWN_DATA_API = `keydown${EVENT_KEY$6}${DATA_API_KEY$3}`;
    const EVENT_KEYUP_DATA_API = `keyup${EVENT_KEY$6}${DATA_API_KEY$3}`;
    const CLASS_NAME_SHOW$6 = "show";
    const CLASS_NAME_DROPUP = "dropup";
    const CLASS_NAME_DROPEND = "dropend";
    const CLASS_NAME_DROPSTART = "dropstart";
    const CLASS_NAME_DROPUP_CENTER = "dropup-center";
    const CLASS_NAME_DROPDOWN_CENTER = "dropdown-center";
    const SELECTOR_DATA_TOGGLE$3 = '[data-bs-toggle="dropdown"]:not(.disabled):not(:disabled)';
    const SELECTOR_DATA_TOGGLE_SHOWN = `${SELECTOR_DATA_TOGGLE$3}.${CLASS_NAME_SHOW$6}`;
    const SELECTOR_MENU = ".dropdown-menu";
    const SELECTOR_NAVBAR = ".navbar";
    const SELECTOR_NAVBAR_NAV = ".navbar-nav";
    const SELECTOR_VISIBLE_ITEMS = ".dropdown-menu .dropdown-item:not(.disabled):not(:disabled)";
    const PLACEMENT_TOP = isRTL() ? "top-end" : "top-start";
    const PLACEMENT_TOPEND = isRTL() ? "top-start" : "top-end";
    const PLACEMENT_BOTTOM = isRTL() ? "bottom-end" : "bottom-start";
    const PLACEMENT_BOTTOMEND = isRTL() ? "bottom-start" : "bottom-end";
    const PLACEMENT_RIGHT = isRTL() ? "left-start" : "right-start";
    const PLACEMENT_LEFT = isRTL() ? "right-start" : "left-start";
    const PLACEMENT_TOPCENTER = "top";
    const PLACEMENT_BOTTOMCENTER = "bottom";
    const Default$9 = {
        autoClose: true,
        boundary: "clippingParents",
        display: "dynamic",
        offset: [ 0, 2 ],
        popperConfig: null,
        reference: "toggle"
    };
    const DefaultType$9 = {
        autoClose: "(boolean|string)",
        boundary: "(string|element)",
        display: "string",
        offset: "(array|string|function)",
        popperConfig: "(null|object|function)",
        reference: "(string|element|object)"
    };
    class Dropdown extends BaseComponent {
        constructor(element, config) {
            super(element, config);
            this._popper = null;
            this._parent = this._element.parentNode;
            this._menu = SelectorEngine.next(this._element, SELECTOR_MENU)[0] || SelectorEngine.prev(this._element, SELECTOR_MENU)[0] || SelectorEngine.findOne(SELECTOR_MENU, this._parent);
            this._inNavbar = this._detectNavbar();
        }
        static get Default() {
            return Default$9;
        }
        static get DefaultType() {
            return DefaultType$9;
        }
        static get NAME() {
            return NAME$a;
        }
        toggle() {
            return this._isShown() ? this.hide() : this.show();
        }
        show() {
            if (isDisabled(this._element) || this._isShown()) {
                return;
            }
            const relatedTarget = {
                relatedTarget: this._element
            };
            const showEvent = EventHandler.trigger(this._element, EVENT_SHOW$5, relatedTarget);
            if (showEvent.defaultPrevented) {
                return;
            }
            this._createPopper();
            if ("ontouchstart" in document.documentElement && !this._parent.closest(SELECTOR_NAVBAR_NAV)) {
                for (const element of [].concat(...document.body.children)) {
                    EventHandler.on(element, "mouseover", noop);
                }
            }
            this._element.focus();
            this._element.setAttribute("aria-expanded", true);
            this._menu.classList.add(CLASS_NAME_SHOW$6);
            this._element.classList.add(CLASS_NAME_SHOW$6);
            EventHandler.trigger(this._element, EVENT_SHOWN$5, relatedTarget);
        }
        hide() {
            if (isDisabled(this._element) || !this._isShown()) {
                return;
            }
            const relatedTarget = {
                relatedTarget: this._element
            };
            this._completeHide(relatedTarget);
        }
        dispose() {
            if (this._popper) {
                this._popper.destroy();
            }
            super.dispose();
        }
        update() {
            this._inNavbar = this._detectNavbar();
            if (this._popper) {
                this._popper.update();
            }
        }
        _completeHide(relatedTarget) {
            const hideEvent = EventHandler.trigger(this._element, EVENT_HIDE$5, relatedTarget);
            if (hideEvent.defaultPrevented) {
                return;
            }
            if ("ontouchstart" in document.documentElement) {
                for (const element of [].concat(...document.body.children)) {
                    EventHandler.off(element, "mouseover", noop);
                }
            }
            if (this._popper) {
                this._popper.destroy();
            }
            this._menu.classList.remove(CLASS_NAME_SHOW$6);
            this._element.classList.remove(CLASS_NAME_SHOW$6);
            this._element.setAttribute("aria-expanded", "false");
            Manipulator.removeDataAttribute(this._menu, "popper");
            EventHandler.trigger(this._element, EVENT_HIDDEN$5, relatedTarget);
        }
        _getConfig(config) {
            config = super._getConfig(config);
            if (typeof config.reference === "object" && !isElement$1(config.reference) && typeof config.reference.getBoundingClientRect !== "function") {
                throw new TypeError(`${NAME$a.toUpperCase()}: Option "reference" provided type "object" without a required "getBoundingClientRect" method.`);
            }
            return config;
        }
        _createPopper() {
            if (typeof Popper === "undefined") {
                throw new TypeError("Bootstrap's dropdowns require Popper (https://popper.js.org)");
            }
            let referenceElement = this._element;
            if (this._config.reference === "parent") {
                referenceElement = this._parent;
            } else if (isElement$1(this._config.reference)) {
                referenceElement = getElement(this._config.reference);
            } else if (typeof this._config.reference === "object") {
                referenceElement = this._config.reference;
            }
            const popperConfig = this._getPopperConfig();
            this._popper = createPopper(referenceElement, this._menu, popperConfig);
        }
        _isShown() {
            return this._menu.classList.contains(CLASS_NAME_SHOW$6);
        }
        _getPlacement() {
            const parentDropdown = this._parent;
            if (parentDropdown.classList.contains(CLASS_NAME_DROPEND)) {
                return PLACEMENT_RIGHT;
            }
            if (parentDropdown.classList.contains(CLASS_NAME_DROPSTART)) {
                return PLACEMENT_LEFT;
            }
            if (parentDropdown.classList.contains(CLASS_NAME_DROPUP_CENTER)) {
                return PLACEMENT_TOPCENTER;
            }
            if (parentDropdown.classList.contains(CLASS_NAME_DROPDOWN_CENTER)) {
                return PLACEMENT_BOTTOMCENTER;
            }
            const isEnd = getComputedStyle(this._menu).getPropertyValue("--bs-position").trim() === "end";
            if (parentDropdown.classList.contains(CLASS_NAME_DROPUP)) {
                return isEnd ? PLACEMENT_TOPEND : PLACEMENT_TOP;
            }
            return isEnd ? PLACEMENT_BOTTOMEND : PLACEMENT_BOTTOM;
        }
        _detectNavbar() {
            return this._element.closest(SELECTOR_NAVBAR) !== null;
        }
        _getOffset() {
            const {
                offset
            } = this._config;
            if (typeof offset === "string") {
                return offset.split(",").map(value => Number.parseInt(value, 10));
            }
            if (typeof offset === "function") {
                return popperData => offset(popperData, this._element);
            }
            return offset;
        }
        _getPopperConfig() {
            const defaultBsPopperConfig = {
                placement: this._getPlacement(),
                modifiers: [ {
                    name: "preventOverflow",
                    options: {
                        boundary: this._config.boundary
                    }
                }, {
                    name: "offset",
                    options: {
                        offset: this._getOffset()
                    }
                } ]
            };
            if (this._inNavbar || this._config.display === "static") {
                Manipulator.setDataAttribute(this._menu, "popper", "static");
                defaultBsPopperConfig.modifiers = [ {
                    name: "applyStyles",
                    enabled: false
                } ];
            }
            return {
                ...defaultBsPopperConfig,
                ...execute(this._config.popperConfig, [ defaultBsPopperConfig ])
            };
        }
        _selectMenuItem({
            key,
            target
        }) {
            const items = SelectorEngine.find(SELECTOR_VISIBLE_ITEMS, this._menu).filter(element => isVisible(element));
            if (!items.length) {
                return;
            }
            getNextActiveElement(items, target, key === ARROW_DOWN_KEY$1, !items.includes(target)).focus();
        }
        static jQueryInterface(config) {
            return this.each(function() {
                const data = Dropdown.getOrCreateInstance(this, config);
                if (typeof config !== "string") {
                    return;
                }
                if (typeof data[config] === "undefined") {
                    throw new TypeError(`No method named "${config}"`);
                }
                data[config]();
            });
        }
        static clearMenus(event) {
            if (event.button === RIGHT_MOUSE_BUTTON || event.type === "keyup" && event.key !== TAB_KEY$1) {
                return;
            }
            const openToggles = SelectorEngine.find(SELECTOR_DATA_TOGGLE_SHOWN);
            for (const toggle of openToggles) {
                const context = Dropdown.getInstance(toggle);
                if (!context || context._config.autoClose === false) {
                    continue;
                }
                const composedPath = event.composedPath();
                const isMenuTarget = composedPath.includes(context._menu);
                if (composedPath.includes(context._element) || context._config.autoClose === "inside" && !isMenuTarget || context._config.autoClose === "outside" && isMenuTarget) {
                    continue;
                }
                if (context._menu.contains(event.target) && (event.type === "keyup" && event.key === TAB_KEY$1 || /input|select|option|textarea|form/i.test(event.target.tagName))) {
                    continue;
                }
                const relatedTarget = {
                    relatedTarget: context._element
                };
                if (event.type === "click") {
                    relatedTarget.clickEvent = event;
                }
                context._completeHide(relatedTarget);
            }
        }
        static dataApiKeydownHandler(event) {
            const isInput = /input|textarea/i.test(event.target.tagName);
            const isEscapeEvent = event.key === ESCAPE_KEY$2;
            const isUpOrDownEvent = [ ARROW_UP_KEY$1, ARROW_DOWN_KEY$1 ].includes(event.key);
            if (!isUpOrDownEvent && !isEscapeEvent) {
                return;
            }
            if (isInput && !isEscapeEvent) {
                return;
            }
            event.preventDefault();
            const getToggleButton = this.matches(SELECTOR_DATA_TOGGLE$3) ? this : SelectorEngine.prev(this, SELECTOR_DATA_TOGGLE$3)[0] || SelectorEngine.next(this, SELECTOR_DATA_TOGGLE$3)[0] || SelectorEngine.findOne(SELECTOR_DATA_TOGGLE$3, event.delegateTarget.parentNode);
            const instance = Dropdown.getOrCreateInstance(getToggleButton);
            if (isUpOrDownEvent) {
                event.stopPropagation();
                instance.show();
                instance._selectMenuItem(event);
                return;
            }
            if (instance._isShown()) {
                event.stopPropagation();
                instance.hide();
                getToggleButton.focus();
            }
        }
    }
    EventHandler.on(document, EVENT_KEYDOWN_DATA_API, SELECTOR_DATA_TOGGLE$3, Dropdown.dataApiKeydownHandler);
    EventHandler.on(document, EVENT_KEYDOWN_DATA_API, SELECTOR_MENU, Dropdown.dataApiKeydownHandler);
    EventHandler.on(document, EVENT_CLICK_DATA_API$3, Dropdown.clearMenus);
    EventHandler.on(document, EVENT_KEYUP_DATA_API, Dropdown.clearMenus);
    EventHandler.on(document, EVENT_CLICK_DATA_API$3, SELECTOR_DATA_TOGGLE$3, function(event) {
        event.preventDefault();
        Dropdown.getOrCreateInstance(this).toggle();
    });
    defineJQueryPlugin(Dropdown);
    const NAME$9 = "backdrop";
    const CLASS_NAME_FADE$4 = "fade";
    const CLASS_NAME_SHOW$5 = "show";
    const EVENT_MOUSEDOWN = `mousedown.bs.${NAME$9}`;
    const Default$8 = {
        className: "modal-backdrop",
        clickCallback: null,
        isAnimated: false,
        isVisible: true,
        rootElement: "body"
    };
    const DefaultType$8 = {
        className: "string",
        clickCallback: "(function|null)",
        isAnimated: "boolean",
        isVisible: "boolean",
        rootElement: "(element|string)"
    };
    class Backdrop extends Config {
        constructor(config) {
            super();
            this._config = this._getConfig(config);
            this._isAppended = false;
            this._element = null;
        }
        static get Default() {
            return Default$8;
        }
        static get DefaultType() {
            return DefaultType$8;
        }
        static get NAME() {
            return NAME$9;
        }
        show(callback) {
            if (!this._config.isVisible) {
                execute(callback);
                return;
            }
            this._append();
            const element = this._getElement();
            if (this._config.isAnimated) {
                reflow(element);
            }
            element.classList.add(CLASS_NAME_SHOW$5);
            this._emulateAnimation(() => {
                execute(callback);
            });
        }
        hide(callback) {
            if (!this._config.isVisible) {
                execute(callback);
                return;
            }
            this._getElement().classList.remove(CLASS_NAME_SHOW$5);
            this._emulateAnimation(() => {
                this.dispose();
                execute(callback);
            });
        }
        dispose() {
            if (!this._isAppended) {
                return;
            }
            EventHandler.off(this._element, EVENT_MOUSEDOWN);
            this._element.remove();
            this._isAppended = false;
        }
        _getElement() {
            if (!this._element) {
                const backdrop = document.createElement("div");
                backdrop.className = this._config.className;
                if (this._config.isAnimated) {
                    backdrop.classList.add(CLASS_NAME_FADE$4);
                }
                this._element = backdrop;
            }
            return this._element;
        }
        _configAfterMerge(config) {
            config.rootElement = getElement(config.rootElement);
            return config;
        }
        _append() {
            if (this._isAppended) {
                return;
            }
            const element = this._getElement();
            this._config.rootElement.append(element);
            EventHandler.on(element, EVENT_MOUSEDOWN, () => {
                execute(this._config.clickCallback);
            });
            this._isAppended = true;
        }
        _emulateAnimation(callback) {
            executeAfterTransition(callback, this._getElement(), this._config.isAnimated);
        }
    }
    const NAME$8 = "focustrap";
    const DATA_KEY$5 = "bs.focustrap";
    const EVENT_KEY$5 = `.${DATA_KEY$5}`;
    const EVENT_FOCUSIN$2 = `focusin${EVENT_KEY$5}`;
    const EVENT_KEYDOWN_TAB = `keydown.tab${EVENT_KEY$5}`;
    const TAB_KEY = "Tab";
    const TAB_NAV_FORWARD = "forward";
    const TAB_NAV_BACKWARD = "backward";
    const Default$7 = {
        autofocus: true,
        trapElement: null
    };
    const DefaultType$7 = {
        autofocus: "boolean",
        trapElement: "element"
    };
    class FocusTrap extends Config {
        constructor(config) {
            super();
            this._config = this._getConfig(config);
            this._isActive = false;
            this._lastTabNavDirection = null;
        }
        static get Default() {
            return Default$7;
        }
        static get DefaultType() {
            return DefaultType$7;
        }
        static get NAME() {
            return NAME$8;
        }
        activate() {
            if (this._isActive) {
                return;
            }
            if (this._config.autofocus) {
                this._config.trapElement.focus();
            }
            EventHandler.off(document, EVENT_KEY$5);
            EventHandler.on(document, EVENT_FOCUSIN$2, event => this._handleFocusin(event));
            EventHandler.on(document, EVENT_KEYDOWN_TAB, event => this._handleKeydown(event));
            this._isActive = true;
        }
        deactivate() {
            if (!this._isActive) {
                return;
            }
            this._isActive = false;
            EventHandler.off(document, EVENT_KEY$5);
        }
        _handleFocusin(event) {
            const {
                trapElement
            } = this._config;
            if (event.target === document || event.target === trapElement || trapElement.contains(event.target)) {
                return;
            }
            const elements = SelectorEngine.focusableChildren(trapElement);
            if (elements.length === 0) {
                trapElement.focus();
            } else if (this._lastTabNavDirection === TAB_NAV_BACKWARD) {
                elements[elements.length - 1].focus();
            } else {
                elements[0].focus();
            }
        }
        _handleKeydown(event) {
            if (event.key !== TAB_KEY) {
                return;
            }
            this._lastTabNavDirection = event.shiftKey ? TAB_NAV_BACKWARD : TAB_NAV_FORWARD;
        }
    }
    const SELECTOR_FIXED_CONTENT = ".fixed-top, .fixed-bottom, .is-fixed, .sticky-top";
    const SELECTOR_STICKY_CONTENT = ".sticky-top";
    const PROPERTY_PADDING = "padding-right";
    const PROPERTY_MARGIN = "margin-right";
    class ScrollBarHelper {
        constructor() {
            this._element = document.body;
        }
        getWidth() {
            const documentWidth = document.documentElement.clientWidth;
            return Math.abs(window.innerWidth - documentWidth);
        }
        hide() {
            const width = this.getWidth();
            this._disableOverFlow();
            this._setElementAttributes(this._element, PROPERTY_PADDING, calculatedValue => calculatedValue + width);
            this._setElementAttributes(SELECTOR_FIXED_CONTENT, PROPERTY_PADDING, calculatedValue => calculatedValue + width);
            this._setElementAttributes(SELECTOR_STICKY_CONTENT, PROPERTY_MARGIN, calculatedValue => calculatedValue - width);
        }
        reset() {
            this._resetElementAttributes(this._element, "overflow");
            this._resetElementAttributes(this._element, PROPERTY_PADDING);
            this._resetElementAttributes(SELECTOR_FIXED_CONTENT, PROPERTY_PADDING);
            this._resetElementAttributes(SELECTOR_STICKY_CONTENT, PROPERTY_MARGIN);
        }
        isOverflowing() {
            return this.getWidth() > 0;
        }
        _disableOverFlow() {
            this._saveInitialAttribute(this._element, "overflow");
            this._element.style.overflow = "hidden";
        }
        _setElementAttributes(selector, styleProperty, callback) {
            const scrollbarWidth = this.getWidth();
            const manipulationCallBack = element => {
                if (element !== this._element && window.innerWidth > element.clientWidth + scrollbarWidth) {
                    return;
                }
                this._saveInitialAttribute(element, styleProperty);
                const calculatedValue = window.getComputedStyle(element).getPropertyValue(styleProperty);
                element.style.setProperty(styleProperty, `${callback(Number.parseFloat(calculatedValue))}px`);
            };
            this._applyManipulationCallback(selector, manipulationCallBack);
        }
        _saveInitialAttribute(element, styleProperty) {
            const actualValue = element.style.getPropertyValue(styleProperty);
            if (actualValue) {
                Manipulator.setDataAttribute(element, styleProperty, actualValue);
            }
        }
        _resetElementAttributes(selector, styleProperty) {
            const manipulationCallBack = element => {
                const value = Manipulator.getDataAttribute(element, styleProperty);
                if (value === null) {
                    element.style.removeProperty(styleProperty);
                    return;
                }
                Manipulator.removeDataAttribute(element, styleProperty);
                element.style.setProperty(styleProperty, value);
            };
            this._applyManipulationCallback(selector, manipulationCallBack);
        }
        _applyManipulationCallback(selector, callBack) {
            if (isElement$1(selector)) {
                callBack(selector);
                return;
            }
            for (const sel of SelectorEngine.find(selector, this._element)) {
                callBack(sel);
            }
        }
    }
    const NAME$7 = "modal";
    const DATA_KEY$4 = "bs.modal";
    const EVENT_KEY$4 = `.${DATA_KEY$4}`;
    const DATA_API_KEY$2 = ".data-api";
    const ESCAPE_KEY$1 = "Escape";
    const EVENT_HIDE$4 = `hide${EVENT_KEY$4}`;
    const EVENT_HIDE_PREVENTED$1 = `hidePrevented${EVENT_KEY$4}`;
    const EVENT_HIDDEN$4 = `hidden${EVENT_KEY$4}`;
    const EVENT_SHOW$4 = `show${EVENT_KEY$4}`;
    const EVENT_SHOWN$4 = `shown${EVENT_KEY$4}`;
    const EVENT_RESIZE$1 = `resize${EVENT_KEY$4}`;
    const EVENT_CLICK_DISMISS = `click.dismiss${EVENT_KEY$4}`;
    const EVENT_MOUSEDOWN_DISMISS = `mousedown.dismiss${EVENT_KEY$4}`;
    const EVENT_KEYDOWN_DISMISS$1 = `keydown.dismiss${EVENT_KEY$4}`;
    const EVENT_CLICK_DATA_API$2 = `click${EVENT_KEY$4}${DATA_API_KEY$2}`;
    const CLASS_NAME_OPEN = "modal-open";
    const CLASS_NAME_FADE$3 = "fade";
    const CLASS_NAME_SHOW$4 = "show";
    const CLASS_NAME_STATIC = "modal-static";
    const OPEN_SELECTOR$1 = ".modal.show";
    const SELECTOR_DIALOG = ".modal-dialog";
    const SELECTOR_MODAL_BODY = ".modal-body";
    const SELECTOR_DATA_TOGGLE$2 = '[data-bs-toggle="modal"]';
    const Default$6 = {
        backdrop: true,
        focus: true,
        keyboard: true
    };
    const DefaultType$6 = {
        backdrop: "(boolean|string)",
        focus: "boolean",
        keyboard: "boolean"
    };
    class Modal extends BaseComponent {
        constructor(element, config) {
            super(element, config);
            this._dialog = SelectorEngine.findOne(SELECTOR_DIALOG, this._element);
            this._backdrop = this._initializeBackDrop();
            this._focustrap = this._initializeFocusTrap();
            this._isShown = false;
            this._isTransitioning = false;
            this._scrollBar = new ScrollBarHelper();
            this._addEventListeners();
        }
        static get Default() {
            return Default$6;
        }
        static get DefaultType() {
            return DefaultType$6;
        }
        static get NAME() {
            return NAME$7;
        }
        toggle(relatedTarget) {
            return this._isShown ? this.hide() : this.show(relatedTarget);
        }
        show(relatedTarget) {
            if (this._isShown || this._isTransitioning) {
                return;
            }
            const showEvent = EventHandler.trigger(this._element, EVENT_SHOW$4, {
                relatedTarget: relatedTarget
            });
            if (showEvent.defaultPrevented) {
                return;
            }
            this._isShown = true;
            this._isTransitioning = true;
            this._scrollBar.hide();
            document.body.classList.add(CLASS_NAME_OPEN);
            this._adjustDialog();
            this._backdrop.show(() => this._showElement(relatedTarget));
        }
        hide() {
            if (!this._isShown || this._isTransitioning) {
                return;
            }
            const hideEvent = EventHandler.trigger(this._element, EVENT_HIDE$4);
            if (hideEvent.defaultPrevented) {
                return;
            }
            this._isShown = false;
            this._isTransitioning = true;
            this._focustrap.deactivate();
            this._element.classList.remove(CLASS_NAME_SHOW$4);
            this._queueCallback(() => this._hideModal(), this._element, this._isAnimated());
        }
        dispose() {
            EventHandler.off(window, EVENT_KEY$4);
            EventHandler.off(this._dialog, EVENT_KEY$4);
            this._backdrop.dispose();
            this._focustrap.deactivate();
            super.dispose();
        }
        handleUpdate() {
            this._adjustDialog();
        }
        _initializeBackDrop() {
            return new Backdrop({
                isVisible: Boolean(this._config.backdrop),
                isAnimated: this._isAnimated()
            });
        }
        _initializeFocusTrap() {
            return new FocusTrap({
                trapElement: this._element
            });
        }
        _showElement(relatedTarget) {
            if (!document.body.contains(this._element)) {
                document.body.append(this._element);
            }
            this._element.style.display = "block";
            this._element.removeAttribute("aria-hidden");
            this._element.setAttribute("aria-modal", true);
            this._element.setAttribute("role", "dialog");
            this._element.scrollTop = 0;
            const modalBody = SelectorEngine.findOne(SELECTOR_MODAL_BODY, this._dialog);
            if (modalBody) {
                modalBody.scrollTop = 0;
            }
            reflow(this._element);
            this._element.classList.add(CLASS_NAME_SHOW$4);
            const transitionComplete = () => {
                if (this._config.focus) {
                    this._focustrap.activate();
                }
                this._isTransitioning = false;
                EventHandler.trigger(this._element, EVENT_SHOWN$4, {
                    relatedTarget: relatedTarget
                });
            };
            this._queueCallback(transitionComplete, this._dialog, this._isAnimated());
        }
        _addEventListeners() {
            EventHandler.on(this._element, EVENT_KEYDOWN_DISMISS$1, event => {
                if (event.key !== ESCAPE_KEY$1) {
                    return;
                }
                if (this._config.keyboard) {
                    this.hide();
                    return;
                }
                this._triggerBackdropTransition();
            });
            EventHandler.on(window, EVENT_RESIZE$1, () => {
                if (this._isShown && !this._isTransitioning) {
                    this._adjustDialog();
                }
            });
            EventHandler.on(this._element, EVENT_MOUSEDOWN_DISMISS, event => {
                EventHandler.one(this._element, EVENT_CLICK_DISMISS, event2 => {
                    if (this._element !== event.target || this._element !== event2.target) {
                        return;
                    }
                    if (this._config.backdrop === "static") {
                        this._triggerBackdropTransition();
                        return;
                    }
                    if (this._config.backdrop) {
                        this.hide();
                    }
                });
            });
        }
        _hideModal() {
            this._element.style.display = "none";
            this._element.setAttribute("aria-hidden", true);
            this._element.removeAttribute("aria-modal");
            this._element.removeAttribute("role");
            this._isTransitioning = false;
            this._backdrop.hide(() => {
                document.body.classList.remove(CLASS_NAME_OPEN);
                this._resetAdjustments();
                this._scrollBar.reset();
                EventHandler.trigger(this._element, EVENT_HIDDEN$4);
            });
        }
        _isAnimated() {
            return this._element.classList.contains(CLASS_NAME_FADE$3);
        }
        _triggerBackdropTransition() {
            const hideEvent = EventHandler.trigger(this._element, EVENT_HIDE_PREVENTED$1);
            if (hideEvent.defaultPrevented) {
                return;
            }
            const isModalOverflowing = this._element.scrollHeight > document.documentElement.clientHeight;
            const initialOverflowY = this._element.style.overflowY;
            if (initialOverflowY === "hidden" || this._element.classList.contains(CLASS_NAME_STATIC)) {
                return;
            }
            if (!isModalOverflowing) {
                this._element.style.overflowY = "hidden";
            }
            this._element.classList.add(CLASS_NAME_STATIC);
            this._queueCallback(() => {
                this._element.classList.remove(CLASS_NAME_STATIC);
                this._queueCallback(() => {
                    this._element.style.overflowY = initialOverflowY;
                }, this._dialog);
            }, this._dialog);
            this._element.focus();
        }
        _adjustDialog() {
            const isModalOverflowing = this._element.scrollHeight > document.documentElement.clientHeight;
            const scrollbarWidth = this._scrollBar.getWidth();
            const isBodyOverflowing = scrollbarWidth > 0;
            if (isBodyOverflowing && !isModalOverflowing) {
                const property = isRTL() ? "paddingLeft" : "paddingRight";
                this._element.style[property] = `${scrollbarWidth}px`;
            }
            if (!isBodyOverflowing && isModalOverflowing) {
                const property = isRTL() ? "paddingRight" : "paddingLeft";
                this._element.style[property] = `${scrollbarWidth}px`;
            }
        }
        _resetAdjustments() {
            this._element.style.paddingLeft = "";
            this._element.style.paddingRight = "";
        }
        static jQueryInterface(config, relatedTarget) {
            return this.each(function() {
                const data = Modal.getOrCreateInstance(this, config);
                if (typeof config !== "string") {
                    return;
                }
                if (typeof data[config] === "undefined") {
                    throw new TypeError(`No method named "${config}"`);
                }
                data[config](relatedTarget);
            });
        }
    }
    EventHandler.on(document, EVENT_CLICK_DATA_API$2, SELECTOR_DATA_TOGGLE$2, function(event) {
        const target = SelectorEngine.getElementFromSelector(this);
        if ([ "A", "AREA" ].includes(this.tagName)) {
            event.preventDefault();
        }
        EventHandler.one(target, EVENT_SHOW$4, showEvent => {
            if (showEvent.defaultPrevented) {
                return;
            }
            EventHandler.one(target, EVENT_HIDDEN$4, () => {
                if (isVisible(this)) {
                    this.focus();
                }
            });
        });
        const alreadyOpen = SelectorEngine.findOne(OPEN_SELECTOR$1);
        if (alreadyOpen) {
            Modal.getInstance(alreadyOpen).hide();
        }
        const data = Modal.getOrCreateInstance(target);
        data.toggle(this);
    });
    enableDismissTrigger(Modal);
    defineJQueryPlugin(Modal);
    const NAME$6 = "offcanvas";
    const DATA_KEY$3 = "bs.offcanvas";
    const EVENT_KEY$3 = `.${DATA_KEY$3}`;
    const DATA_API_KEY$1 = ".data-api";
    const EVENT_LOAD_DATA_API$2 = `load${EVENT_KEY$3}${DATA_API_KEY$1}`;
    const ESCAPE_KEY = "Escape";
    const CLASS_NAME_SHOW$3 = "show";
    const CLASS_NAME_SHOWING$1 = "showing";
    const CLASS_NAME_HIDING = "hiding";
    const CLASS_NAME_BACKDROP = "offcanvas-backdrop";
    const OPEN_SELECTOR = ".offcanvas.show";
    const EVENT_SHOW$3 = `show${EVENT_KEY$3}`;
    const EVENT_SHOWN$3 = `shown${EVENT_KEY$3}`;
    const EVENT_HIDE$3 = `hide${EVENT_KEY$3}`;
    const EVENT_HIDE_PREVENTED = `hidePrevented${EVENT_KEY$3}`;
    const EVENT_HIDDEN$3 = `hidden${EVENT_KEY$3}`;
    const EVENT_RESIZE = `resize${EVENT_KEY$3}`;
    const EVENT_CLICK_DATA_API$1 = `click${EVENT_KEY$3}${DATA_API_KEY$1}`;
    const EVENT_KEYDOWN_DISMISS = `keydown.dismiss${EVENT_KEY$3}`;
    const SELECTOR_DATA_TOGGLE$1 = '[data-bs-toggle="offcanvas"]';
    const Default$5 = {
        backdrop: true,
        keyboard: true,
        scroll: false
    };
    const DefaultType$5 = {
        backdrop: "(boolean|string)",
        keyboard: "boolean",
        scroll: "boolean"
    };
    class Offcanvas extends BaseComponent {
        constructor(element, config) {
            super(element, config);
            this._isShown = false;
            this._backdrop = this._initializeBackDrop();
            this._focustrap = this._initializeFocusTrap();
            this._addEventListeners();
        }
        static get Default() {
            return Default$5;
        }
        static get DefaultType() {
            return DefaultType$5;
        }
        static get NAME() {
            return NAME$6;
        }
        toggle(relatedTarget) {
            return this._isShown ? this.hide() : this.show(relatedTarget);
        }
        show(relatedTarget) {
            if (this._isShown) {
                return;
            }
            const showEvent = EventHandler.trigger(this._element, EVENT_SHOW$3, {
                relatedTarget: relatedTarget
            });
            if (showEvent.defaultPrevented) {
                return;
            }
            this._isShown = true;
            this._backdrop.show();
            if (!this._config.scroll) {
                new ScrollBarHelper().hide();
            }
            this._element.setAttribute("aria-modal", true);
            this._element.setAttribute("role", "dialog");
            this._element.classList.add(CLASS_NAME_SHOWING$1);
            const completeCallBack = () => {
                if (!this._config.scroll || this._config.backdrop) {
                    this._focustrap.activate();
                }
                this._element.classList.add(CLASS_NAME_SHOW$3);
                this._element.classList.remove(CLASS_NAME_SHOWING$1);
                EventHandler.trigger(this._element, EVENT_SHOWN$3, {
                    relatedTarget: relatedTarget
                });
            };
            this._queueCallback(completeCallBack, this._element, true);
        }
        hide() {
            if (!this._isShown) {
                return;
            }
            const hideEvent = EventHandler.trigger(this._element, EVENT_HIDE$3);
            if (hideEvent.defaultPrevented) {
                return;
            }
            this._focustrap.deactivate();
            this._element.blur();
            this._isShown = false;
            this._element.classList.add(CLASS_NAME_HIDING);
            this._backdrop.hide();
            const completeCallback = () => {
                this._element.classList.remove(CLASS_NAME_SHOW$3, CLASS_NAME_HIDING);
                this._element.removeAttribute("aria-modal");
                this._element.removeAttribute("role");
                if (!this._config.scroll) {
                    new ScrollBarHelper().reset();
                }
                EventHandler.trigger(this._element, EVENT_HIDDEN$3);
            };
            this._queueCallback(completeCallback, this._element, true);
        }
        dispose() {
            this._backdrop.dispose();
            this._focustrap.deactivate();
            super.dispose();
        }
        _initializeBackDrop() {
            const clickCallback = () => {
                if (this._config.backdrop === "static") {
                    EventHandler.trigger(this._element, EVENT_HIDE_PREVENTED);
                    return;
                }
                this.hide();
            };
            const isVisible = Boolean(this._config.backdrop);
            return new Backdrop({
                className: CLASS_NAME_BACKDROP,
                isVisible: isVisible,
                isAnimated: true,
                rootElement: this._element.parentNode,
                clickCallback: isVisible ? clickCallback : null
            });
        }
        _initializeFocusTrap() {
            return new FocusTrap({
                trapElement: this._element
            });
        }
        _addEventListeners() {
            EventHandler.on(this._element, EVENT_KEYDOWN_DISMISS, event => {
                if (event.key !== ESCAPE_KEY) {
                    return;
                }
                if (this._config.keyboard) {
                    this.hide();
                    return;
                }
                EventHandler.trigger(this._element, EVENT_HIDE_PREVENTED);
            });
        }
        static jQueryInterface(config) {
            return this.each(function() {
                const data = Offcanvas.getOrCreateInstance(this, config);
                if (typeof config !== "string") {
                    return;
                }
                if (data[config] === undefined || config.startsWith("_") || config === "constructor") {
                    throw new TypeError(`No method named "${config}"`);
                }
                data[config](this);
            });
        }
    }
    EventHandler.on(document, EVENT_CLICK_DATA_API$1, SELECTOR_DATA_TOGGLE$1, function(event) {
        const target = SelectorEngine.getElementFromSelector(this);
        if ([ "A", "AREA" ].includes(this.tagName)) {
            event.preventDefault();
        }
        if (isDisabled(this)) {
            return;
        }
        EventHandler.one(target, EVENT_HIDDEN$3, () => {
            if (isVisible(this)) {
                this.focus();
            }
        });
        const alreadyOpen = SelectorEngine.findOne(OPEN_SELECTOR);
        if (alreadyOpen && alreadyOpen !== target) {
            Offcanvas.getInstance(alreadyOpen).hide();
        }
        const data = Offcanvas.getOrCreateInstance(target);
        data.toggle(this);
    });
    EventHandler.on(window, EVENT_LOAD_DATA_API$2, () => {
        for (const selector of SelectorEngine.find(OPEN_SELECTOR)) {
            Offcanvas.getOrCreateInstance(selector).show();
        }
    });
    EventHandler.on(window, EVENT_RESIZE, () => {
        for (const element of SelectorEngine.find("[aria-modal][class*=show][class*=offcanvas-]")) {
            if (getComputedStyle(element).position !== "fixed") {
                Offcanvas.getOrCreateInstance(element).hide();
            }
        }
    });
    enableDismissTrigger(Offcanvas);
    defineJQueryPlugin(Offcanvas);
    const ARIA_ATTRIBUTE_PATTERN = /^aria-[\w-]*$/i;
    const DefaultAllowlist = {
        "*": [ "class", "dir", "id", "lang", "role", ARIA_ATTRIBUTE_PATTERN ],
        a: [ "target", "href", "title", "rel" ],
        area: [],
        b: [],
        br: [],
        col: [],
        code: [],
        dd: [],
        div: [],
        dl: [],
        dt: [],
        em: [],
        hr: [],
        h1: [],
        h2: [],
        h3: [],
        h4: [],
        h5: [],
        h6: [],
        i: [],
        img: [ "src", "srcset", "alt", "title", "width", "height" ],
        li: [],
        ol: [],
        p: [],
        pre: [],
        s: [],
        small: [],
        span: [],
        sub: [],
        sup: [],
        strong: [],
        u: [],
        ul: []
    };
    const uriAttributes = new Set([ "background", "cite", "href", "itemtype", "longdesc", "poster", "src", "xlink:href" ]);
    const SAFE_URL_PATTERN = /^(?!javascript:)(?:[a-z0-9+.-]+:|[^&:/?#]*(?:[/?#]|$))/i;
    const allowedAttribute = (attribute, allowedAttributeList) => {
        const attributeName = attribute.nodeName.toLowerCase();
        if (allowedAttributeList.includes(attributeName)) {
            if (uriAttributes.has(attributeName)) {
                return Boolean(SAFE_URL_PATTERN.test(attribute.nodeValue));
            }
            return true;
        }
        return allowedAttributeList.filter(attributeRegex => attributeRegex instanceof RegExp).some(regex => regex.test(attributeName));
    };
    function sanitizeHtml(unsafeHtml, allowList, sanitizeFunction) {
        if (!unsafeHtml.length) {
            return unsafeHtml;
        }
        if (sanitizeFunction && typeof sanitizeFunction === "function") {
            return sanitizeFunction(unsafeHtml);
        }
        const domParser = new window.DOMParser();
        const createdDocument = domParser.parseFromString(unsafeHtml, "text/html");
        const elements = [].concat(...createdDocument.body.querySelectorAll("*"));
        for (const element of elements) {
            const elementName = element.nodeName.toLowerCase();
            if (!Object.keys(allowList).includes(elementName)) {
                element.remove();
                continue;
            }
            const attributeList = [].concat(...element.attributes);
            const allowedAttributes = [].concat(allowList["*"] || [], allowList[elementName] || []);
            for (const attribute of attributeList) {
                if (!allowedAttribute(attribute, allowedAttributes)) {
                    element.removeAttribute(attribute.nodeName);
                }
            }
        }
        return createdDocument.body.innerHTML;
    }
    const NAME$5 = "TemplateFactory";
    const Default$4 = {
        allowList: DefaultAllowlist,
        content: {},
        extraClass: "",
        html: false,
        sanitize: true,
        sanitizeFn: null,
        template: "<div></div>"
    };
    const DefaultType$4 = {
        allowList: "object",
        content: "object",
        extraClass: "(string|function)",
        html: "boolean",
        sanitize: "boolean",
        sanitizeFn: "(null|function)",
        template: "string"
    };
    const DefaultContentType = {
        entry: "(string|element|function|null)",
        selector: "(string|element)"
    };
    class TemplateFactory extends Config {
        constructor(config) {
            super();
            this._config = this._getConfig(config);
        }
        static get Default() {
            return Default$4;
        }
        static get DefaultType() {
            return DefaultType$4;
        }
        static get NAME() {
            return NAME$5;
        }
        getContent() {
            return Object.values(this._config.content).map(config => this._resolvePossibleFunction(config)).filter(Boolean);
        }
        hasContent() {
            return this.getContent().length > 0;
        }
        changeContent(content) {
            this._checkContent(content);
            this._config.content = {
                ...this._config.content,
                ...content
            };
            return this;
        }
        toHtml() {
            const templateWrapper = document.createElement("div");
            templateWrapper.innerHTML = this._maybeSanitize(this._config.template);
            for (const [ selector, text ] of Object.entries(this._config.content)) {
                this._setContent(templateWrapper, text, selector);
            }
            const template = templateWrapper.children[0];
            const extraClass = this._resolvePossibleFunction(this._config.extraClass);
            if (extraClass) {
                template.classList.add(...extraClass.split(" "));
            }
            return template;
        }
        _typeCheckConfig(config) {
            super._typeCheckConfig(config);
            this._checkContent(config.content);
        }
        _checkContent(arg) {
            for (const [ selector, content ] of Object.entries(arg)) {
                super._typeCheckConfig({
                    selector: selector,
                    entry: content
                }, DefaultContentType);
            }
        }
        _setContent(template, content, selector) {
            const templateElement = SelectorEngine.findOne(selector, template);
            if (!templateElement) {
                return;
            }
            content = this._resolvePossibleFunction(content);
            if (!content) {
                templateElement.remove();
                return;
            }
            if (isElement$1(content)) {
                this._putElementInTemplate(getElement(content), templateElement);
                return;
            }
            if (this._config.html) {
                templateElement.innerHTML = this._maybeSanitize(content);
                return;
            }
            templateElement.textContent = content;
        }
        _maybeSanitize(arg) {
            return this._config.sanitize ? sanitizeHtml(arg, this._config.allowList, this._config.sanitizeFn) : arg;
        }
        _resolvePossibleFunction(arg) {
            return execute(arg, [ this ]);
        }
        _putElementInTemplate(element, templateElement) {
            if (this._config.html) {
                templateElement.innerHTML = "";
                templateElement.append(element);
                return;
            }
            templateElement.textContent = element.textContent;
        }
    }
    const NAME$4 = "tooltip";
    const DISALLOWED_ATTRIBUTES = new Set([ "sanitize", "allowList", "sanitizeFn" ]);
    const CLASS_NAME_FADE$2 = "fade";
    const CLASS_NAME_MODAL = "modal";
    const CLASS_NAME_SHOW$2 = "show";
    const SELECTOR_TOOLTIP_INNER = ".tooltip-inner";
    const SELECTOR_MODAL = `.${CLASS_NAME_MODAL}`;
    const EVENT_MODAL_HIDE = "hide.bs.modal";
    const TRIGGER_HOVER = "hover";
    const TRIGGER_FOCUS = "focus";
    const TRIGGER_CLICK = "click";
    const TRIGGER_MANUAL = "manual";
    const EVENT_HIDE$2 = "hide";
    const EVENT_HIDDEN$2 = "hidden";
    const EVENT_SHOW$2 = "show";
    const EVENT_SHOWN$2 = "shown";
    const EVENT_INSERTED = "inserted";
    const EVENT_CLICK$1 = "click";
    const EVENT_FOCUSIN$1 = "focusin";
    const EVENT_FOCUSOUT$1 = "focusout";
    const EVENT_MOUSEENTER = "mouseenter";
    const EVENT_MOUSELEAVE = "mouseleave";
    const AttachmentMap = {
        AUTO: "auto",
        TOP: "top",
        RIGHT: isRTL() ? "left" : "right",
        BOTTOM: "bottom",
        LEFT: isRTL() ? "right" : "left"
    };
    const Default$3 = {
        allowList: DefaultAllowlist,
        animation: true,
        boundary: "clippingParents",
        container: false,
        customClass: "",
        delay: 0,
        fallbackPlacements: [ "top", "right", "bottom", "left" ],
        html: false,
        offset: [ 0, 6 ],
        placement: "top",
        popperConfig: null,
        sanitize: true,
        sanitizeFn: null,
        selector: false,
        template: '<div class="tooltip" role="tooltip">' + '<div class="tooltip-arrow"></div>' + '<div class="tooltip-inner"></div>' + "</div>",
        title: "",
        trigger: "hover focus"
    };
    const DefaultType$3 = {
        allowList: "object",
        animation: "boolean",
        boundary: "(string|element)",
        container: "(string|element|boolean)",
        customClass: "(string|function)",
        delay: "(number|object)",
        fallbackPlacements: "array",
        html: "boolean",
        offset: "(array|string|function)",
        placement: "(string|function)",
        popperConfig: "(null|object|function)",
        sanitize: "boolean",
        sanitizeFn: "(null|function)",
        selector: "(string|boolean)",
        template: "string",
        title: "(string|element|function)",
        trigger: "string"
    };
    class Tooltip extends BaseComponent {
        constructor(element, config) {
            if (typeof Popper === "undefined") {
                throw new TypeError("Bootstrap's tooltips require Popper (https://popper.js.org)");
            }
            super(element, config);
            this._isEnabled = true;
            this._timeout = 0;
            this._isHovered = null;
            this._activeTrigger = {};
            this._popper = null;
            this._templateFactory = null;
            this._newContent = null;
            this.tip = null;
            this._setListeners();
            if (!this._config.selector) {
                this._fixTitle();
            }
        }
        static get Default() {
            return Default$3;
        }
        static get DefaultType() {
            return DefaultType$3;
        }
        static get NAME() {
            return NAME$4;
        }
        enable() {
            this._isEnabled = true;
        }
        disable() {
            this._isEnabled = false;
        }
        toggleEnabled() {
            this._isEnabled = !this._isEnabled;
        }
        toggle() {
            if (!this._isEnabled) {
                return;
            }
            this._activeTrigger.click = !this._activeTrigger.click;
            if (this._isShown()) {
                this._leave();
                return;
            }
            this._enter();
        }
        dispose() {
            clearTimeout(this._timeout);
            EventHandler.off(this._element.closest(SELECTOR_MODAL), EVENT_MODAL_HIDE, this._hideModalHandler);
            if (this._element.getAttribute("data-bs-original-title")) {
                this._element.setAttribute("title", this._element.getAttribute("data-bs-original-title"));
            }
            this._disposePopper();
            super.dispose();
        }
        show() {
            if (this._element.style.display === "none") {
                throw new Error("Please use show on visible elements");
            }
            if (!(this._isWithContent() && this._isEnabled)) {
                return;
            }
            const showEvent = EventHandler.trigger(this._element, this.constructor.eventName(EVENT_SHOW$2));
            const shadowRoot = findShadowRoot(this._element);
            const isInTheDom = (shadowRoot || this._element.ownerDocument.documentElement).contains(this._element);
            if (showEvent.defaultPrevented || !isInTheDom) {
                return;
            }
            this._disposePopper();
            const tip = this._getTipElement();
            this._element.setAttribute("aria-describedby", tip.getAttribute("id"));
            const {
                container
            } = this._config;
            if (!this._element.ownerDocument.documentElement.contains(this.tip)) {
                container.append(tip);
                EventHandler.trigger(this._element, this.constructor.eventName(EVENT_INSERTED));
            }
            this._popper = this._createPopper(tip);
            tip.classList.add(CLASS_NAME_SHOW$2);
            if ("ontouchstart" in document.documentElement) {
                for (const element of [].concat(...document.body.children)) {
                    EventHandler.on(element, "mouseover", noop);
                }
            }
            const complete = () => {
                EventHandler.trigger(this._element, this.constructor.eventName(EVENT_SHOWN$2));
                if (this._isHovered === false) {
                    this._leave();
                }
                this._isHovered = false;
            };
            this._queueCallback(complete, this.tip, this._isAnimated());
        }
        hide() {
            if (!this._isShown()) {
                return;
            }
            const hideEvent = EventHandler.trigger(this._element, this.constructor.eventName(EVENT_HIDE$2));
            if (hideEvent.defaultPrevented) {
                return;
            }
            const tip = this._getTipElement();
            tip.classList.remove(CLASS_NAME_SHOW$2);
            if ("ontouchstart" in document.documentElement) {
                for (const element of [].concat(...document.body.children)) {
                    EventHandler.off(element, "mouseover", noop);
                }
            }
            this._activeTrigger[TRIGGER_CLICK] = false;
            this._activeTrigger[TRIGGER_FOCUS] = false;
            this._activeTrigger[TRIGGER_HOVER] = false;
            this._isHovered = null;
            const complete = () => {
                if (this._isWithActiveTrigger()) {
                    return;
                }
                if (!this._isHovered) {
                    this._disposePopper();
                }
                this._element.removeAttribute("aria-describedby");
                EventHandler.trigger(this._element, this.constructor.eventName(EVENT_HIDDEN$2));
            };
            this._queueCallback(complete, this.tip, this._isAnimated());
        }
        update() {
            if (this._popper) {
                this._popper.update();
            }
        }
        _isWithContent() {
            return Boolean(this._getTitle());
        }
        _getTipElement() {
            if (!this.tip) {
                this.tip = this._createTipElement(this._newContent || this._getContentForTemplate());
            }
            return this.tip;
        }
        _createTipElement(content) {
            const tip = this._getTemplateFactory(content).toHtml();
            if (!tip) {
                return null;
            }
            tip.classList.remove(CLASS_NAME_FADE$2, CLASS_NAME_SHOW$2);
            tip.classList.add(`bs-${this.constructor.NAME}-auto`);
            const tipId = getUID(this.constructor.NAME).toString();
            tip.setAttribute("id", tipId);
            if (this._isAnimated()) {
                tip.classList.add(CLASS_NAME_FADE$2);
            }
            return tip;
        }
        setContent(content) {
            this._newContent = content;
            if (this._isShown()) {
                this._disposePopper();
                this.show();
            }
        }
        _getTemplateFactory(content) {
            if (this._templateFactory) {
                this._templateFactory.changeContent(content);
            } else {
                this._templateFactory = new TemplateFactory({
                    ...this._config,
                    content: content,
                    extraClass: this._resolvePossibleFunction(this._config.customClass)
                });
            }
            return this._templateFactory;
        }
        _getContentForTemplate() {
            return {
                [SELECTOR_TOOLTIP_INNER]: this._getTitle()
            };
        }
        _getTitle() {
            return this._resolvePossibleFunction(this._config.title) || this._element.getAttribute("data-bs-original-title");
        }
        _initializeOnDelegatedTarget(event) {
            return this.constructor.getOrCreateInstance(event.delegateTarget, this._getDelegateConfig());
        }
        _isAnimated() {
            return this._config.animation || this.tip && this.tip.classList.contains(CLASS_NAME_FADE$2);
        }
        _isShown() {
            return this.tip && this.tip.classList.contains(CLASS_NAME_SHOW$2);
        }
        _createPopper(tip) {
            const placement = execute(this._config.placement, [ this, tip, this._element ]);
            const attachment = AttachmentMap[placement.toUpperCase()];
            return createPopper(this._element, tip, this._getPopperConfig(attachment));
        }
        _getOffset() {
            const {
                offset
            } = this._config;
            if (typeof offset === "string") {
                return offset.split(",").map(value => Number.parseInt(value, 10));
            }
            if (typeof offset === "function") {
                return popperData => offset(popperData, this._element);
            }
            return offset;
        }
        _resolvePossibleFunction(arg) {
            return execute(arg, [ this._element ]);
        }
        _getPopperConfig(attachment) {
            const defaultBsPopperConfig = {
                placement: attachment,
                modifiers: [ {
                    name: "flip",
                    options: {
                        fallbackPlacements: this._config.fallbackPlacements
                    }
                }, {
                    name: "offset",
                    options: {
                        offset: this._getOffset()
                    }
                }, {
                    name: "preventOverflow",
                    options: {
                        boundary: this._config.boundary
                    }
                }, {
                    name: "arrow",
                    options: {
                        element: `.${this.constructor.NAME}-arrow`
                    }
                }, {
                    name: "preSetPlacement",
                    enabled: true,
                    phase: "beforeMain",
                    fn: data => {
                        this._getTipElement().setAttribute("data-popper-placement", data.state.placement);
                    }
                } ]
            };
            return {
                ...defaultBsPopperConfig,
                ...execute(this._config.popperConfig, [ defaultBsPopperConfig ])
            };
        }
        _setListeners() {
            const triggers = this._config.trigger.split(" ");
            for (const trigger of triggers) {
                if (trigger === "click") {
                    EventHandler.on(this._element, this.constructor.eventName(EVENT_CLICK$1), this._config.selector, event => {
                        const context = this._initializeOnDelegatedTarget(event);
                        context.toggle();
                    });
                } else if (trigger !== TRIGGER_MANUAL) {
                    const eventIn = trigger === TRIGGER_HOVER ? this.constructor.eventName(EVENT_MOUSEENTER) : this.constructor.eventName(EVENT_FOCUSIN$1);
                    const eventOut = trigger === TRIGGER_HOVER ? this.constructor.eventName(EVENT_MOUSELEAVE) : this.constructor.eventName(EVENT_FOCUSOUT$1);
                    EventHandler.on(this._element, eventIn, this._config.selector, event => {
                        const context = this._initializeOnDelegatedTarget(event);
                        context._activeTrigger[event.type === "focusin" ? TRIGGER_FOCUS : TRIGGER_HOVER] = true;
                        context._enter();
                    });
                    EventHandler.on(this._element, eventOut, this._config.selector, event => {
                        const context = this._initializeOnDelegatedTarget(event);
                        context._activeTrigger[event.type === "focusout" ? TRIGGER_FOCUS : TRIGGER_HOVER] = context._element.contains(event.relatedTarget);
                        context._leave();
                    });
                }
            }
            this._hideModalHandler = () => {
                if (this._element) {
                    this.hide();
                }
            };
            EventHandler.on(this._element.closest(SELECTOR_MODAL), EVENT_MODAL_HIDE, this._hideModalHandler);
        }
        _fixTitle() {
            const title = this._element.getAttribute("title");
            if (!title) {
                return;
            }
            if (!this._element.getAttribute("aria-label") && !this._element.textContent.trim()) {
                this._element.setAttribute("aria-label", title);
            }
            this._element.setAttribute("data-bs-original-title", title);
            this._element.removeAttribute("title");
        }
        _enter() {
            if (this._isShown() || this._isHovered) {
                this._isHovered = true;
                return;
            }
            this._isHovered = true;
            this._setTimeout(() => {
                if (this._isHovered) {
                    this.show();
                }
            }, this._config.delay.show);
        }
        _leave() {
            if (this._isWithActiveTrigger()) {
                return;
            }
            this._isHovered = false;
            this._setTimeout(() => {
                if (!this._isHovered) {
                    this.hide();
                }
            }, this._config.delay.hide);
        }
        _setTimeout(handler, timeout) {
            clearTimeout(this._timeout);
            this._timeout = setTimeout(handler, timeout);
        }
        _isWithActiveTrigger() {
            return Object.values(this._activeTrigger).includes(true);
        }
        _getConfig(config) {
            const dataAttributes = Manipulator.getDataAttributes(this._element);
            for (const dataAttribute of Object.keys(dataAttributes)) {
                if (DISALLOWED_ATTRIBUTES.has(dataAttribute)) {
                    delete dataAttributes[dataAttribute];
                }
            }
            config = {
                ...dataAttributes,
                ...typeof config === "object" && config ? config : {}
            };
            config = this._mergeConfigObj(config);
            config = this._configAfterMerge(config);
            this._typeCheckConfig(config);
            return config;
        }
        _configAfterMerge(config) {
            config.container = config.container === false ? document.body : getElement(config.container);
            if (typeof config.delay === "number") {
                config.delay = {
                    show: config.delay,
                    hide: config.delay
                };
            }
            if (typeof config.title === "number") {
                config.title = config.title.toString();
            }
            if (typeof config.content === "number") {
                config.content = config.content.toString();
            }
            return config;
        }
        _getDelegateConfig() {
            const config = {};
            for (const [ key, value ] of Object.entries(this._config)) {
                if (this.constructor.Default[key] !== value) {
                    config[key] = value;
                }
            }
            config.selector = false;
            config.trigger = "manual";
            return config;
        }
        _disposePopper() {
            if (this._popper) {
                this._popper.destroy();
                this._popper = null;
            }
            if (this.tip) {
                this.tip.remove();
                this.tip = null;
            }
        }
        static jQueryInterface(config) {
            return this.each(function() {
                const data = Tooltip.getOrCreateInstance(this, config);
                if (typeof config !== "string") {
                    return;
                }
                if (typeof data[config] === "undefined") {
                    throw new TypeError(`No method named "${config}"`);
                }
                data[config]();
            });
        }
    }
    defineJQueryPlugin(Tooltip);
    const NAME$3 = "popover";
    const SELECTOR_TITLE = ".popover-header";
    const SELECTOR_CONTENT = ".popover-body";
    const Default$2 = {
        ...Tooltip.Default,
        content: "",
        offset: [ 0, 8 ],
        placement: "right",
        template: '<div class="popover" role="tooltip">' + '<div class="popover-arrow"></div>' + '<h3 class="popover-header"></h3>' + '<div class="popover-body"></div>' + "</div>",
        trigger: "click"
    };
    const DefaultType$2 = {
        ...Tooltip.DefaultType,
        content: "(null|string|element|function)"
    };
    class Popover extends Tooltip {
        static get Default() {
            return Default$2;
        }
        static get DefaultType() {
            return DefaultType$2;
        }
        static get NAME() {
            return NAME$3;
        }
        _isWithContent() {
            return this._getTitle() || this._getContent();
        }
        _getContentForTemplate() {
            return {
                [SELECTOR_TITLE]: this._getTitle(),
                [SELECTOR_CONTENT]: this._getContent()
            };
        }
        _getContent() {
            return this._resolvePossibleFunction(this._config.content);
        }
        static jQueryInterface(config) {
            return this.each(function() {
                const data = Popover.getOrCreateInstance(this, config);
                if (typeof config !== "string") {
                    return;
                }
                if (typeof data[config] === "undefined") {
                    throw new TypeError(`No method named "${config}"`);
                }
                data[config]();
            });
        }
    }
    defineJQueryPlugin(Popover);
    const NAME$2 = "scrollspy";
    const DATA_KEY$2 = "bs.scrollspy";
    const EVENT_KEY$2 = `.${DATA_KEY$2}`;
    const DATA_API_KEY = ".data-api";
    const EVENT_ACTIVATE = `activate${EVENT_KEY$2}`;
    const EVENT_CLICK = `click${EVENT_KEY$2}`;
    const EVENT_LOAD_DATA_API$1 = `load${EVENT_KEY$2}${DATA_API_KEY}`;
    const CLASS_NAME_DROPDOWN_ITEM = "dropdown-item";
    const CLASS_NAME_ACTIVE$1 = "active";
    const SELECTOR_DATA_SPY = '[data-bs-spy="scroll"]';
    const SELECTOR_TARGET_LINKS = "[href]";
    const SELECTOR_NAV_LIST_GROUP = ".nav, .list-group";
    const SELECTOR_NAV_LINKS = ".nav-link";
    const SELECTOR_NAV_ITEMS = ".nav-item";
    const SELECTOR_LIST_ITEMS = ".list-group-item";
    const SELECTOR_LINK_ITEMS = `${SELECTOR_NAV_LINKS}, ${SELECTOR_NAV_ITEMS} > ${SELECTOR_NAV_LINKS}, ${SELECTOR_LIST_ITEMS}`;
    const SELECTOR_DROPDOWN = ".dropdown";
    const SELECTOR_DROPDOWN_TOGGLE$1 = ".dropdown-toggle";
    const Default$1 = {
        offset: null,
        rootMargin: "0px 0px -25%",
        smoothScroll: false,
        target: null,
        threshold: [ .1, .5, 1 ]
    };
    const DefaultType$1 = {
        offset: "(number|null)",
        rootMargin: "string",
        smoothScroll: "boolean",
        target: "element",
        threshold: "array"
    };
    class ScrollSpy extends BaseComponent {
        constructor(element, config) {
            super(element, config);
            this._targetLinks = new Map();
            this._observableSections = new Map();
            this._rootElement = getComputedStyle(this._element).overflowY === "visible" ? null : this._element;
            this._activeTarget = null;
            this._observer = null;
            this._previousScrollData = {
                visibleEntryTop: 0,
                parentScrollTop: 0
            };
            this.refresh();
        }
        static get Default() {
            return Default$1;
        }
        static get DefaultType() {
            return DefaultType$1;
        }
        static get NAME() {
            return NAME$2;
        }
        refresh() {
            this._initializeTargetsAndObservables();
            this._maybeEnableSmoothScroll();
            if (this._observer) {
                this._observer.disconnect();
            } else {
                this._observer = this._getNewObserver();
            }
            for (const section of this._observableSections.values()) {
                this._observer.observe(section);
            }
        }
        dispose() {
            this._observer.disconnect();
            super.dispose();
        }
        _configAfterMerge(config) {
            config.target = getElement(config.target) || document.body;
            config.rootMargin = config.offset ? `${config.offset}px 0px -30%` : config.rootMargin;
            if (typeof config.threshold === "string") {
                config.threshold = config.threshold.split(",").map(value => Number.parseFloat(value));
            }
            return config;
        }
        _maybeEnableSmoothScroll() {
            if (!this._config.smoothScroll) {
                return;
            }
            EventHandler.off(this._config.target, EVENT_CLICK);
            EventHandler.on(this._config.target, EVENT_CLICK, SELECTOR_TARGET_LINKS, event => {
                const observableSection = this._observableSections.get(event.target.hash);
                if (observableSection) {
                    event.preventDefault();
                    const root = this._rootElement || window;
                    const height = observableSection.offsetTop - this._element.offsetTop;
                    if (root.scrollTo) {
                        root.scrollTo({
                            top: height,
                            behavior: "smooth"
                        });
                        return;
                    }
                    root.scrollTop = height;
                }
            });
        }
        _getNewObserver() {
            const options = {
                root: this._rootElement,
                threshold: this._config.threshold,
                rootMargin: this._config.rootMargin
            };
            return new IntersectionObserver(entries => this._observerCallback(entries), options);
        }
        _observerCallback(entries) {
            const targetElement = entry => this._targetLinks.get(`#${entry.target.id}`);
            const activate = entry => {
                this._previousScrollData.visibleEntryTop = entry.target.offsetTop;
                this._process(targetElement(entry));
            };
            const parentScrollTop = (this._rootElement || document.documentElement).scrollTop;
            const userScrollsDown = parentScrollTop >= this._previousScrollData.parentScrollTop;
            this._previousScrollData.parentScrollTop = parentScrollTop;
            for (const entry of entries) {
                if (!entry.isIntersecting) {
                    this._activeTarget = null;
                    this._clearActiveClass(targetElement(entry));
                    continue;
                }
                const entryIsLowerThanPrevious = entry.target.offsetTop >= this._previousScrollData.visibleEntryTop;
                if (userScrollsDown && entryIsLowerThanPrevious) {
                    activate(entry);
                    if (!parentScrollTop) {
                        return;
                    }
                    continue;
                }
                if (!userScrollsDown && !entryIsLowerThanPrevious) {
                    activate(entry);
                }
            }
        }
        _initializeTargetsAndObservables() {
            this._targetLinks = new Map();
            this._observableSections = new Map();
            const targetLinks = SelectorEngine.find(SELECTOR_TARGET_LINKS, this._config.target);
            for (const anchor of targetLinks) {
                if (!anchor.hash || isDisabled(anchor)) {
                    continue;
                }
                const observableSection = SelectorEngine.findOne(decodeURI(anchor.hash), this._element);
                if (isVisible(observableSection)) {
                    this._targetLinks.set(decodeURI(anchor.hash), anchor);
                    this._observableSections.set(anchor.hash, observableSection);
                }
            }
        }
        _process(target) {
            if (this._activeTarget === target) {
                return;
            }
            this._clearActiveClass(this._config.target);
            this._activeTarget = target;
            target.classList.add(CLASS_NAME_ACTIVE$1);
            this._activateParents(target);
            EventHandler.trigger(this._element, EVENT_ACTIVATE, {
                relatedTarget: target
            });
        }
        _activateParents(target) {
            if (target.classList.contains(CLASS_NAME_DROPDOWN_ITEM)) {
                SelectorEngine.findOne(SELECTOR_DROPDOWN_TOGGLE$1, target.closest(SELECTOR_DROPDOWN)).classList.add(CLASS_NAME_ACTIVE$1);
                return;
            }
            for (const listGroup of SelectorEngine.parents(target, SELECTOR_NAV_LIST_GROUP)) {
                for (const item of SelectorEngine.prev(listGroup, SELECTOR_LINK_ITEMS)) {
                    item.classList.add(CLASS_NAME_ACTIVE$1);
                }
            }
        }
        _clearActiveClass(parent) {
            parent.classList.remove(CLASS_NAME_ACTIVE$1);
            const activeNodes = SelectorEngine.find(`${SELECTOR_TARGET_LINKS}.${CLASS_NAME_ACTIVE$1}`, parent);
            for (const node of activeNodes) {
                node.classList.remove(CLASS_NAME_ACTIVE$1);
            }
        }
        static jQueryInterface(config) {
            return this.each(function() {
                const data = ScrollSpy.getOrCreateInstance(this, config);
                if (typeof config !== "string") {
                    return;
                }
                if (data[config] === undefined || config.startsWith("_") || config === "constructor") {
                    throw new TypeError(`No method named "${config}"`);
                }
                data[config]();
            });
        }
    }
    EventHandler.on(window, EVENT_LOAD_DATA_API$1, () => {
        for (const spy of SelectorEngine.find(SELECTOR_DATA_SPY)) {
            ScrollSpy.getOrCreateInstance(spy);
        }
    });
    defineJQueryPlugin(ScrollSpy);
    const NAME$1 = "tab";
    const DATA_KEY$1 = "bs.tab";
    const EVENT_KEY$1 = `.${DATA_KEY$1}`;
    const EVENT_HIDE$1 = `hide${EVENT_KEY$1}`;
    const EVENT_HIDDEN$1 = `hidden${EVENT_KEY$1}`;
    const EVENT_SHOW$1 = `show${EVENT_KEY$1}`;
    const EVENT_SHOWN$1 = `shown${EVENT_KEY$1}`;
    const EVENT_CLICK_DATA_API = `click${EVENT_KEY$1}`;
    const EVENT_KEYDOWN = `keydown${EVENT_KEY$1}`;
    const EVENT_LOAD_DATA_API = `load${EVENT_KEY$1}`;
    const ARROW_LEFT_KEY = "ArrowLeft";
    const ARROW_RIGHT_KEY = "ArrowRight";
    const ARROW_UP_KEY = "ArrowUp";
    const ARROW_DOWN_KEY = "ArrowDown";
    const HOME_KEY = "Home";
    const END_KEY = "End";
    const CLASS_NAME_ACTIVE = "active";
    const CLASS_NAME_FADE$1 = "fade";
    const CLASS_NAME_SHOW$1 = "show";
    const CLASS_DROPDOWN = "dropdown";
    const SELECTOR_DROPDOWN_TOGGLE = ".dropdown-toggle";
    const SELECTOR_DROPDOWN_MENU = ".dropdown-menu";
    const NOT_SELECTOR_DROPDOWN_TOGGLE = `:not(${SELECTOR_DROPDOWN_TOGGLE})`;
    const SELECTOR_TAB_PANEL = '.list-group, .nav, [role="tablist"]';
    const SELECTOR_OUTER = ".nav-item, .list-group-item";
    const SELECTOR_INNER = `.nav-link${NOT_SELECTOR_DROPDOWN_TOGGLE}, .list-group-item${NOT_SELECTOR_DROPDOWN_TOGGLE}, [role="tab"]${NOT_SELECTOR_DROPDOWN_TOGGLE}`;
    const SELECTOR_DATA_TOGGLE = '[data-bs-toggle="tab"], [data-bs-toggle="pill"], [data-bs-toggle="list"]';
    const SELECTOR_INNER_ELEM = `${SELECTOR_INNER}, ${SELECTOR_DATA_TOGGLE}`;
    const SELECTOR_DATA_TOGGLE_ACTIVE = `.${CLASS_NAME_ACTIVE}[data-bs-toggle="tab"], .${CLASS_NAME_ACTIVE}[data-bs-toggle="pill"], .${CLASS_NAME_ACTIVE}[data-bs-toggle="list"]`;
    class Tab extends BaseComponent {
        constructor(element) {
            super(element);
            this._parent = this._element.closest(SELECTOR_TAB_PANEL);
            if (!this._parent) {
                return;
            }
            this._setInitialAttributes(this._parent, this._getChildren());
            EventHandler.on(this._element, EVENT_KEYDOWN, event => this._keydown(event));
        }
        static get NAME() {
            return NAME$1;
        }
        show() {
            const innerElem = this._element;
            if (this._elemIsActive(innerElem)) {
                return;
            }
            const active = this._getActiveElem();
            const hideEvent = active ? EventHandler.trigger(active, EVENT_HIDE$1, {
                relatedTarget: innerElem
            }) : null;
            const showEvent = EventHandler.trigger(innerElem, EVENT_SHOW$1, {
                relatedTarget: active
            });
            if (showEvent.defaultPrevented || hideEvent && hideEvent.defaultPrevented) {
                return;
            }
            this._deactivate(active, innerElem);
            this._activate(innerElem, active);
        }
        _activate(element, relatedElem) {
            if (!element) {
                return;
            }
            element.classList.add(CLASS_NAME_ACTIVE);
            this._activate(SelectorEngine.getElementFromSelector(element));
            const complete = () => {
                if (element.getAttribute("role") !== "tab") {
                    element.classList.add(CLASS_NAME_SHOW$1);
                    return;
                }
                element.removeAttribute("tabindex");
                element.setAttribute("aria-selected", true);
                this._toggleDropDown(element, true);
                EventHandler.trigger(element, EVENT_SHOWN$1, {
                    relatedTarget: relatedElem
                });
            };
            this._queueCallback(complete, element, element.classList.contains(CLASS_NAME_FADE$1));
        }
        _deactivate(element, relatedElem) {
            if (!element) {
                return;
            }
            element.classList.remove(CLASS_NAME_ACTIVE);
            element.blur();
            this._deactivate(SelectorEngine.getElementFromSelector(element));
            const complete = () => {
                if (element.getAttribute("role") !== "tab") {
                    element.classList.remove(CLASS_NAME_SHOW$1);
                    return;
                }
                element.setAttribute("aria-selected", false);
                element.setAttribute("tabindex", "-1");
                this._toggleDropDown(element, false);
                EventHandler.trigger(element, EVENT_HIDDEN$1, {
                    relatedTarget: relatedElem
                });
            };
            this._queueCallback(complete, element, element.classList.contains(CLASS_NAME_FADE$1));
        }
        _keydown(event) {
            if (![ ARROW_LEFT_KEY, ARROW_RIGHT_KEY, ARROW_UP_KEY, ARROW_DOWN_KEY, HOME_KEY, END_KEY ].includes(event.key)) {
                return;
            }
            event.stopPropagation();
            event.preventDefault();
            const children = this._getChildren().filter(element => !isDisabled(element));
            let nextActiveElement;
            if ([ HOME_KEY, END_KEY ].includes(event.key)) {
                nextActiveElement = children[event.key === HOME_KEY ? 0 : children.length - 1];
            } else {
                const isNext = [ ARROW_RIGHT_KEY, ARROW_DOWN_KEY ].includes(event.key);
                nextActiveElement = getNextActiveElement(children, event.target, isNext, true);
            }
            if (nextActiveElement) {
                nextActiveElement.focus({
                    preventScroll: true
                });
                Tab.getOrCreateInstance(nextActiveElement).show();
            }
        }
        _getChildren() {
            return SelectorEngine.find(SELECTOR_INNER_ELEM, this._parent);
        }
        _getActiveElem() {
            return this._getChildren().find(child => this._elemIsActive(child)) || null;
        }
        _setInitialAttributes(parent, children) {
            this._setAttributeIfNotExists(parent, "role", "tablist");
            for (const child of children) {
                this._setInitialAttributesOnChild(child);
            }
        }
        _setInitialAttributesOnChild(child) {
            child = this._getInnerElement(child);
            const isActive = this._elemIsActive(child);
            const outerElem = this._getOuterElement(child);
            child.setAttribute("aria-selected", isActive);
            if (outerElem !== child) {
                this._setAttributeIfNotExists(outerElem, "role", "presentation");
            }
            if (!isActive) {
                child.setAttribute("tabindex", "-1");
            }
            this._setAttributeIfNotExists(child, "role", "tab");
            this._setInitialAttributesOnTargetPanel(child);
        }
        _setInitialAttributesOnTargetPanel(child) {
            const target = SelectorEngine.getElementFromSelector(child);
            if (!target) {
                return;
            }
            this._setAttributeIfNotExists(target, "role", "tabpanel");
            if (child.id) {
                this._setAttributeIfNotExists(target, "aria-labelledby", `${child.id}`);
            }
        }
        _toggleDropDown(element, open) {
            const outerElem = this._getOuterElement(element);
            if (!outerElem.classList.contains(CLASS_DROPDOWN)) {
                return;
            }
            const toggle = (selector, className) => {
                const element = SelectorEngine.findOne(selector, outerElem);
                if (element) {
                    element.classList.toggle(className, open);
                }
            };
            toggle(SELECTOR_DROPDOWN_TOGGLE, CLASS_NAME_ACTIVE);
            toggle(SELECTOR_DROPDOWN_MENU, CLASS_NAME_SHOW$1);
            outerElem.setAttribute("aria-expanded", open);
        }
        _setAttributeIfNotExists(element, attribute, value) {
            if (!element.hasAttribute(attribute)) {
                element.setAttribute(attribute, value);
            }
        }
        _elemIsActive(elem) {
            return elem.classList.contains(CLASS_NAME_ACTIVE);
        }
        _getInnerElement(elem) {
            return elem.matches(SELECTOR_INNER_ELEM) ? elem : SelectorEngine.findOne(SELECTOR_INNER_ELEM, elem);
        }
        _getOuterElement(elem) {
            return elem.closest(SELECTOR_OUTER) || elem;
        }
        static jQueryInterface(config) {
            return this.each(function() {
                const data = Tab.getOrCreateInstance(this);
                if (typeof config !== "string") {
                    return;
                }
                if (data[config] === undefined || config.startsWith("_") || config === "constructor") {
                    throw new TypeError(`No method named "${config}"`);
                }
                data[config]();
            });
        }
    }
    EventHandler.on(document, EVENT_CLICK_DATA_API, SELECTOR_DATA_TOGGLE, function(event) {
        if ([ "A", "AREA" ].includes(this.tagName)) {
            event.preventDefault();
        }
        if (isDisabled(this)) {
            return;
        }
        Tab.getOrCreateInstance(this).show();
    });
    EventHandler.on(window, EVENT_LOAD_DATA_API, () => {
        for (const element of SelectorEngine.find(SELECTOR_DATA_TOGGLE_ACTIVE)) {
            Tab.getOrCreateInstance(element);
        }
    });
    defineJQueryPlugin(Tab);
    const NAME = "toast";
    const DATA_KEY = "bs.toast";
    const EVENT_KEY = `.${DATA_KEY}`;
    const EVENT_MOUSEOVER = `mouseover${EVENT_KEY}`;
    const EVENT_MOUSEOUT = `mouseout${EVENT_KEY}`;
    const EVENT_FOCUSIN = `focusin${EVENT_KEY}`;
    const EVENT_FOCUSOUT = `focusout${EVENT_KEY}`;
    const EVENT_HIDE = `hide${EVENT_KEY}`;
    const EVENT_HIDDEN = `hidden${EVENT_KEY}`;
    const EVENT_SHOW = `show${EVENT_KEY}`;
    const EVENT_SHOWN = `shown${EVENT_KEY}`;
    const CLASS_NAME_FADE = "fade";
    const CLASS_NAME_HIDE = "hide";
    const CLASS_NAME_SHOW = "show";
    const CLASS_NAME_SHOWING = "showing";
    const DefaultType = {
        animation: "boolean",
        autohide: "boolean",
        delay: "number"
    };
    const Default = {
        animation: true,
        autohide: true,
        delay: 5e3
    };
    class Toast extends BaseComponent {
        constructor(element, config) {
            super(element, config);
            this._timeout = null;
            this._hasMouseInteraction = false;
            this._hasKeyboardInteraction = false;
            this._setListeners();
        }
        static get Default() {
            return Default;
        }
        static get DefaultType() {
            return DefaultType;
        }
        static get NAME() {
            return NAME;
        }
        show() {
            const showEvent = EventHandler.trigger(this._element, EVENT_SHOW);
            if (showEvent.defaultPrevented) {
                return;
            }
            this._clearTimeout();
            if (this._config.animation) {
                this._element.classList.add(CLASS_NAME_FADE);
            }
            const complete = () => {
                this._element.classList.remove(CLASS_NAME_SHOWING);
                EventHandler.trigger(this._element, EVENT_SHOWN);
                this._maybeScheduleHide();
            };
            this._element.classList.remove(CLASS_NAME_HIDE);
            reflow(this._element);
            this._element.classList.add(CLASS_NAME_SHOW, CLASS_NAME_SHOWING);
            this._queueCallback(complete, this._element, this._config.animation);
        }
        hide() {
            if (!this.isShown()) {
                return;
            }
            const hideEvent = EventHandler.trigger(this._element, EVENT_HIDE);
            if (hideEvent.defaultPrevented) {
                return;
            }
            const complete = () => {
                this._element.classList.add(CLASS_NAME_HIDE);
                this._element.classList.remove(CLASS_NAME_SHOWING, CLASS_NAME_SHOW);
                EventHandler.trigger(this._element, EVENT_HIDDEN);
            };
            this._element.classList.add(CLASS_NAME_SHOWING);
            this._queueCallback(complete, this._element, this._config.animation);
        }
        dispose() {
            this._clearTimeout();
            if (this.isShown()) {
                this._element.classList.remove(CLASS_NAME_SHOW);
            }
            super.dispose();
        }
        isShown() {
            return this._element.classList.contains(CLASS_NAME_SHOW);
        }
        _maybeScheduleHide() {
            if (!this._config.autohide) {
                return;
            }
            if (this._hasMouseInteraction || this._hasKeyboardInteraction) {
                return;
            }
            this._timeout = setTimeout(() => {
                this.hide();
            }, this._config.delay);
        }
        _onInteraction(event, isInteracting) {
            switch (event.type) {
              case "mouseover":
              case "mouseout":
                {
                    this._hasMouseInteraction = isInteracting;
                    break;
                }

              case "focusin":
              case "focusout":
                {
                    this._hasKeyboardInteraction = isInteracting;
                    break;
                }
            }
            if (isInteracting) {
                this._clearTimeout();
                return;
            }
            const nextElement = event.relatedTarget;
            if (this._element === nextElement || this._element.contains(nextElement)) {
                return;
            }
            this._maybeScheduleHide();
        }
        _setListeners() {
            EventHandler.on(this._element, EVENT_MOUSEOVER, event => this._onInteraction(event, true));
            EventHandler.on(this._element, EVENT_MOUSEOUT, event => this._onInteraction(event, false));
            EventHandler.on(this._element, EVENT_FOCUSIN, event => this._onInteraction(event, true));
            EventHandler.on(this._element, EVENT_FOCUSOUT, event => this._onInteraction(event, false));
        }
        _clearTimeout() {
            clearTimeout(this._timeout);
            this._timeout = null;
        }
        static jQueryInterface(config) {
            return this.each(function() {
                const data = Toast.getOrCreateInstance(this, config);
                if (typeof config === "string") {
                    if (typeof data[config] === "undefined") {
                        throw new TypeError(`No method named "${config}"`);
                    }
                    data[config](this);
                }
            });
        }
    }
    enableDismissTrigger(Toast);
    defineJQueryPlugin(Toast);
    const index_umd = {
        Alert: Alert,
        Button: Button,
        Carousel: Carousel,
        Collapse: Collapse,
        Dropdown: Dropdown,
        Modal: Modal,
        Offcanvas: Offcanvas,
        Popover: Popover,
        ScrollSpy: ScrollSpy,
        Tab: Tab,
        Toast: Toast,
        Tooltip: Tooltip
    };
    return index_umd;
});

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
            const li = document.createElement("li");
            if (noText) {
                noAttachmentsText = noText;
            }
            li.innerHTML = `<li><div class="alert alert-info text-break" role="alert" style="white-space:normal;width:200px">${noAttachmentsText}</div></li>`;
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
        if (isPageChange && document.querySelector(".attachments-toggle") != null) {
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
                    item.innerHTML = `<div class="row"><div class="col"><div class="card border-0 w-100 mb-3"><div class="card-header bg-transparent border-top border-bottom-0 px-0 pb-0 pt-4 topicTitle"><h5> <a title="${topic}" href="${dataItem.TopicUrl}">${dataItem.Topic}</a>&nbsp;<a title="${lastPost}" href="${dataItem.MessageUrl}"><i class="fas fa-external-link-alt"></i></a> <small class="text-body-secondary">(<a href="${dataItem.ForumUrl}">${dataItem.ForumName}</a>)</small></h5></div><div class="card-body px-0"><h6 class="card-subtitle mb-2 text-body-secondary">${dataItem.Description}</h6><p class="card-text messageContent">${dataItem.Message}</p></div><div class="card-footer bg-transparent border-top-0 px-0 py-2"> <small class="text-body-secondary"><span class="fa-stack"><i class="fa fa-calendar-day fa-stack-1x text-secondary"></i><i class="fa fa-clock fa-badge text-secondary"></i> </span>${posted} ${dataItem.Posted} <i class="fa fa-user fa-fw text-secondary"></i>${by} ${useDisplayName ? dataItem.UserDisplayName : dataItem.UserName}${tags}</small> </div></div></div></div>`;
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