/*!
 * jQuery UI Widget 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */

//>>label: Widget
//>>group: Core
//>>description: Provides a factory for creating stateful widgets with a common API.
//>>docs: http://api.jqueryui.com/jQuery.widget/
//>>demos: http://jqueryui.com/widget/

(function (factory) {
    if (typeof define === "function" && define.amd) {

        // AMD. Register as an anonymous module.
        define(["jquery", "./version"], factory);
    } else {

        // Browser globals
        factory(jQuery);
    }
}(function ($) {

    var widgetUuid = 0;
    var widgetSlice = Array.prototype.slice;

    $.cleanData = (function (orig) {
        return function (elems) {
            var events, elem, i;
            for (i = 0; (elem = elems[i]) != null; i++) {
                try {

                    // Only trigger remove when necessary to save time
                    events = $._data(elem, "events");
                    if (events && events.remove) {
                        $(elem).triggerHandler("remove");
                    }

                    // Http://bugs.jquery.com/ticket/8235
                } catch (e) { }
            }
            orig(elems);
        };
    })($.cleanData);

    $.widget = function (name, base, prototype) {
        var existingConstructor, constructor, basePrototype;

        // ProxiedPrototype allows the provided prototype to remain unmodified
        // so that it can be used as a mixin for multiple widgets (#8876)
        var proxiedPrototype = {};

        var namespace = name.split(".")[0];
        name = name.split(".")[1];
        var fullName = namespace + "-" + name;

        if (!prototype) {
            prototype = base;
            base = $.Widget;
        }

        if ($.isArray(prototype)) {
            prototype = $.extend.apply(null, [{}].concat(prototype));
        }

        // Create selector for plugin
        $.expr[":"][fullName.toLowerCase()] = function (elem) {
            return !!$.data(elem, fullName);
        };

        $[namespace] = $[namespace] || {};
        existingConstructor = $[namespace][name];
        constructor = $[namespace][name] = function (options, element) {

            // Allow instantiation without "new" keyword
            if (!this._createWidget) {
                return new constructor(options, element);
            }

            // Allow instantiation without initializing for simple inheritance
            // must use "new" keyword (the code above always passes args)
            if (arguments.length) {
                this._createWidget(options, element);
            }
        };

        // Extend with the existing constructor to carry over any static properties
        $.extend(constructor, existingConstructor, {
            version: prototype.version,

            // Copy the object used to create the prototype in case we need to
            // redefine the widget later
            _proto: $.extend({}, prototype),

            // Track widgets that inherit from this widget in case this widget is
            // redefined after a widget inherits from it
            _childConstructors: []
        });

        basePrototype = new base();

        // We need to make the options hash a property directly on the new instance
        // otherwise we'll modify the options hash on the prototype that we're
        // inheriting from
        basePrototype.options = $.widget.extend({}, basePrototype.options);
        $.each(prototype, function (prop, value) {
            if (!$.isFunction(value)) {
                proxiedPrototype[prop] = value;
                return;
            }
            proxiedPrototype[prop] = (function () {
                function _super() {
                    return base.prototype[prop].apply(this, arguments);
                }

                function _superApply(args) {
                    return base.prototype[prop].apply(this, args);
                }

                return function () {
                    var __super = this._super;
                    var __superApply = this._superApply;
                    var returnValue;

                    this._super = _super;
                    this._superApply = _superApply;

                    returnValue = value.apply(this, arguments);

                    this._super = __super;
                    this._superApply = __superApply;

                    return returnValue;
                };
            })();
        });
        constructor.prototype = $.widget.extend(basePrototype, {

            // TODO: remove support for widgetEventPrefix
            // always use the name + a colon as the prefix, e.g., draggable:start
            // don't prefix for widgets that aren't DOM-based
            widgetEventPrefix: existingConstructor ? (basePrototype.widgetEventPrefix || name) : name
        }, proxiedPrototype, {
                constructor: constructor,
                namespace: namespace,
                widgetName: name,
                widgetFullName: fullName
            });

        // If this widget is being redefined then we need to find all widgets that
        // are inheriting from it and redefine all of them so that they inherit from
        // the new version of this widget. We're essentially trying to replace one
        // level in the prototype chain.
        if (existingConstructor) {
            $.each(existingConstructor._childConstructors, function (i, child) {
                var childPrototype = child.prototype;

                // Redefine the child widget using the same prototype that was
                // originally used, but inherit from the new version of the base
                $.widget(childPrototype.namespace + "." + childPrototype.widgetName, constructor,
                    child._proto);
            });

            // Remove the list of existing child constructors from the old constructor
            // so the old child constructors can be garbage collected
            delete existingConstructor._childConstructors;
        } else {
            base._childConstructors.push(constructor);
        }

        $.widget.bridge(name, constructor);

        return constructor;
    };

    $.widget.extend = function (target) {
        var input = widgetSlice.call(arguments, 1);
        var inputIndex = 0;
        var inputLength = input.length;
        var key;
        var value;

        for (; inputIndex < inputLength; inputIndex++) {
            for (key in input[inputIndex]) {
                value = input[inputIndex][key];
                if (input[inputIndex].hasOwnProperty(key) && value !== undefined) {

                    // Clone objects
                    if ($.isPlainObject(value)) {
                        target[key] = $.isPlainObject(target[key]) ?
                            $.widget.extend({}, target[key], value) :

                            // Don't extend strings, arrays, etc. with objects
                            $.widget.extend({}, value);

                        // Copy everything else by reference
                    } else {
                        target[key] = value;
                    }
                }
            }
        }
        return target;
    };

    $.widget.bridge = function (name, object) {
        var fullName = object.prototype.widgetFullName || name;
        $.fn[name] = function (options) {
            var isMethodCall = typeof options === "string";
            var args = widgetSlice.call(arguments, 1);
            var returnValue = this;

            if (isMethodCall) {

                // If this is an empty collection, we need to have the instance method
                // return undefined instead of the jQuery instance
                if (!this.length && options === "instance") {
                    returnValue = undefined;
                } else {
                    this.each(function () {
                        var methodValue;
                        var instance = $.data(this, fullName);

                        if (options === "instance") {
                            returnValue = instance;
                            return false;
                        }

                        if (!instance) {
                            return $.error("cannot call methods on " + name +
                                " prior to initialization; " +
                                "attempted to call method '" + options + "'");
                        }

                        if (!$.isFunction(instance[options]) || options.charAt(0) === "_") {
                            return $.error("no such method '" + options + "' for " + name +
                                " widget instance");
                        }

                        methodValue = instance[options].apply(instance, args);

                        if (methodValue !== instance && methodValue !== undefined) {
                            returnValue = methodValue && methodValue.jquery ?
                                returnValue.pushStack(methodValue.get()) :
                                methodValue;
                            return false;
                        }
                    });
                }
            } else {

                // Allow multiple hashes to be passed on init
                if (args.length) {
                    options = $.widget.extend.apply(null, [options].concat(args));
                }

                this.each(function () {
                    var instance = $.data(this, fullName);
                    if (instance) {
                        instance.option(options || {});
                        if (instance._init) {
                            instance._init();
                        }
                    } else {
                        $.data(this, fullName, new object(options, this));
                    }
                });
            }

            return returnValue;
        };
    };

    $.Widget = function ( /* options, element */) { };
    $.Widget._childConstructors = [];

    $.Widget.prototype = {
        widgetName: "widget",
        widgetEventPrefix: "",
        defaultElement: "<div>",

        options: {
            classes: {},
            disabled: false,

            // Callbacks
            create: null
        },

        _createWidget: function (options, element) {
            element = $(element || this.defaultElement || this)[0];
            this.element = $(element);
            this.uuid = widgetUuid++;
            this.eventNamespace = "." + this.widgetName + this.uuid;

            this.bindings = $();
            this.hoverable = $();
            this.focusable = $();
            this.classesElementLookup = {};

            if (element !== this) {
                $.data(element, this.widgetFullName, this);
                this._on(true, this.element, {
                    remove: function (event) {
                        if (event.target === element) {
                            this.destroy();
                        }
                    }
                });
                this.document = $(element.style ?

                    // Element within the document
                    element.ownerDocument :

                    // Element is window or document
                    element.document || element);
                this.window = $(this.document[0].defaultView || this.document[0].parentWindow);
            }

            this.options = $.widget.extend({},
                this.options,
                this._getCreateOptions(),
                options);

            this._create();

            if (this.options.disabled) {
                this._setOptionDisabled(this.options.disabled);
            }

            this._trigger("create", null, this._getCreateEventData());
            this._init();
        },

        _getCreateOptions: function () {
            return {};
        },

        _getCreateEventData: $.noop,

        _create: $.noop,

        _init: $.noop,

        destroy: function () {
            var that = this;

            this._destroy();
            $.each(this.classesElementLookup, function (key, value) {
                that._removeClass(value, key);
            });

            // We can probably remove the unbind calls in 2.0
            // all event bindings should go through this._on()
            this.element
                .off(this.eventNamespace)
                .removeData(this.widgetFullName);
            this.widget()
                .off(this.eventNamespace)
                .removeAttr("aria-disabled");

            // Clean up events and states
            this.bindings.off(this.eventNamespace);
        },

        _destroy: $.noop,

        widget: function () {
            return this.element;
        },

        option: function (key, value) {
            var options = key;
            var parts;
            var curOption;
            var i;

            if (arguments.length === 0) {

                // Don't return a reference to the internal hash
                return $.widget.extend({}, this.options);
            }

            if (typeof key === "string") {

                // Handle nested keys, e.g., "foo.bar" => { foo: { bar: ___ } }
                options = {};
                parts = key.split(".");
                key = parts.shift();
                if (parts.length) {
                    curOption = options[key] = $.widget.extend({}, this.options[key]);
                    for (i = 0; i < parts.length - 1; i++) {
                        curOption[parts[i]] = curOption[parts[i]] || {};
                        curOption = curOption[parts[i]];
                    }
                    key = parts.pop();
                    if (arguments.length === 1) {
                        return curOption[key] === undefined ? null : curOption[key];
                    }
                    curOption[key] = value;
                } else {
                    if (arguments.length === 1) {
                        return this.options[key] === undefined ? null : this.options[key];
                    }
                    options[key] = value;
                }
            }

            this._setOptions(options);

            return this;
        },

        _setOptions: function (options) {
            var key;

            for (key in options) {
                this._setOption(key, options[key]);
            }

            return this;
        },

        _setOption: function (key, value) {
            if (key === "classes") {
                this._setOptionClasses(value);
            }

            this.options[key] = value;

            if (key === "disabled") {
                this._setOptionDisabled(value);
            }

            return this;
        },

        _setOptionClasses: function (value) {
            var classKey, elements, currentElements;

            for (classKey in value) {
                currentElements = this.classesElementLookup[classKey];
                if (value[classKey] === this.options.classes[classKey] ||
                    !currentElements ||
                    !currentElements.length) {
                    continue;
                }

                // We are doing this to create a new jQuery object because the _removeClass() call
                // on the next line is going to destroy the reference to the current elements being
                // tracked. We need to save a copy of this collection so that we can add the new classes
                // below.
                elements = $(currentElements.get());
                this._removeClass(currentElements, classKey);

                // We don't use _addClass() here, because that uses this.options.classes
                // for generating the string of classes. We want to use the value passed in from
                // _setOption(), this is the new value of the classes option which was passed to
                // _setOption(). We pass this value directly to _classes().
                elements.addClass(this._classes({
                    element: elements,
                    keys: classKey,
                    classes: value,
                    add: true
                }));
            }
        },

        _setOptionDisabled: function (value) {
            this._toggleClass(this.widget(), this.widgetFullName + "-disabled", null, !!value);

            // If the widget is becoming disabled, then nothing is interactive
            if (value) {
                this._removeClass(this.hoverable, null, "ui-state-hover");
                this._removeClass(this.focusable, null, "ui-state-focus");
            }
        },

        enable: function () {
            return this._setOptions({ disabled: false });
        },

        disable: function () {
            return this._setOptions({ disabled: true });
        },

        _classes: function (options) {
            var full = [];
            var that = this;

            options = $.extend({
                element: this.element,
                classes: this.options.classes || {}
            }, options);

            function processClassString(classes, checkOption) {
                var current, i;
                for (i = 0; i < classes.length; i++) {
                    current = that.classesElementLookup[classes[i]] || $();
                    if (options.add) {
                        current = $($.unique(current.get().concat(options.element.get())));
                    } else {
                        current = $(current.not(options.element).get());
                    }
                    that.classesElementLookup[classes[i]] = current;
                    full.push(classes[i]);
                    if (checkOption && options.classes[classes[i]]) {
                        full.push(options.classes[classes[i]]);
                    }
                }
            }

            this._on(options.element, {
                "remove": "_untrackClassesElement"
            });

            if (options.keys) {
                processClassString(options.keys.match(/\S+/g) || [], true);
            }
            if (options.extra) {
                processClassString(options.extra.match(/\S+/g) || []);
            }

            return full.join(" ");
        },

        _untrackClassesElement: function (event) {
            var that = this;
            $.each(that.classesElementLookup, function (key, value) {
                if ($.inArray(event.target, value) !== -1) {
                    that.classesElementLookup[key] = $(value.not(event.target).get());
                }
            });
        },

        _removeClass: function (element, keys, extra) {
            return this._toggleClass(element, keys, extra, false);
        },

        _addClass: function (element, keys, extra) {
            return this._toggleClass(element, keys, extra, true);
        },

        _toggleClass: function (element, keys, extra, add) {
            add = (typeof add === "boolean") ? add : extra;
            var shift = (typeof element === "string" || element === null),
                options = {
                    extra: shift ? keys : extra,
                    keys: shift ? element : keys,
                    element: shift ? this.element : element,
                    add: add
                };
            options.element.toggleClass(this._classes(options), add);
            return this;
        },

        _on: function (suppressDisabledCheck, element, handlers) {
            var delegateElement;
            var instance = this;

            // No suppressDisabledCheck flag, shuffle arguments
            if (typeof suppressDisabledCheck !== "boolean") {
                handlers = element;
                element = suppressDisabledCheck;
                suppressDisabledCheck = false;
            }

            // No element argument, shuffle and use this.element
            if (!handlers) {
                handlers = element;
                element = this.element;
                delegateElement = this.widget();
            } else {
                element = delegateElement = $(element);
                this.bindings = this.bindings.add(element);
            }

            $.each(handlers, function (event, handler) {
                function handlerProxy() {

                    // Allow widgets to customize the disabled handling
                    // - disabled as an array instead of boolean
                    // - disabled class as method for disabling individual parts
                    if (!suppressDisabledCheck &&
                        (instance.options.disabled === true ||
                            $(this).hasClass("ui-state-disabled"))) {
                        return;
                    }
                    return (typeof handler === "string" ? instance[handler] : handler)
                        .apply(instance, arguments);
                }

                // Copy the guid so direct unbinding works
                if (typeof handler !== "string") {
                    handlerProxy.guid = handler.guid =
                        handler.guid || handlerProxy.guid || $.guid++;
                }

                var match = event.match(/^([\w:-]*)\s*(.*)$/);
                var eventName = match[1] + instance.eventNamespace;
                var selector = match[2];

                if (selector) {
                    delegateElement.on(eventName, selector, handlerProxy);
                } else {
                    element.on(eventName, handlerProxy);
                }
            });
        },

        _off: function (element, eventName) {
            eventName = (eventName || "").split(" ").join(this.eventNamespace + " ") +
                this.eventNamespace;
            element.off(eventName).off(eventName);

            // Clear the stack to avoid memory leaks (#10056)
            this.bindings = $(this.bindings.not(element).get());
            this.focusable = $(this.focusable.not(element).get());
            this.hoverable = $(this.hoverable.not(element).get());
        },

        _delay: function (handler, delay) {
            function handlerProxy() {
                return (typeof handler === "string" ? instance[handler] : handler)
                    .apply(instance, arguments);
            }
            var instance = this;
            return setTimeout(handlerProxy, delay || 0);
        },

        _hoverable: function (element) {
            this.hoverable = this.hoverable.add(element);
            this._on(element, {
                mouseenter: function (event) {
                    this._addClass($(event.currentTarget), null, "ui-state-hover");
                },
                mouseleave: function (event) {
                    this._removeClass($(event.currentTarget), null, "ui-state-hover");
                }
            });
        },

        _focusable: function (element) {
            this.focusable = this.focusable.add(element);
            this._on(element, {
                focusin: function (event) {
                    this._addClass($(event.currentTarget), null, "ui-state-focus");
                },
                focusout: function (event) {
                    this._removeClass($(event.currentTarget), null, "ui-state-focus");
                }
            });
        },

        _trigger: function (type, event, data) {
            var prop, orig;
            var callback = this.options[type];

            data = data || {};
            event = $.Event(event);
            event.type = (type === this.widgetEventPrefix ?
                type :
                this.widgetEventPrefix + type).toLowerCase();

            // The original event may come from any element
            // so we need to reset the target on the new event
            event.target = this.element[0];

            // Copy original event properties over to the new event
            orig = event.originalEvent;
            if (orig) {
                for (prop in orig) {
                    if (!(prop in event)) {
                        event[prop] = orig[prop];
                    }
                }
            }

            this.element.trigger(event, data);
            return !($.isFunction(callback) &&
                callback.apply(this.element[0], [event].concat(data)) === false ||
                event.isDefaultPrevented());
        }
    };

    $.each({ show: "fadeIn", hide: "fadeOut" }, function (method, defaultEffect) {
        $.Widget.prototype["_" + method] = function (element, options, callback) {
            if (typeof options === "string") {
                options = { effect: options };
            }

            var hasOptions;
            var effectName = !options ?
                method :
                options === true || typeof options === "number" ?
                    defaultEffect :
                    options.effect || defaultEffect;

            options = options || {};
            if (typeof options === "number") {
                options = { duration: options };
            }

            hasOptions = !$.isEmptyObject(options);
            options.complete = callback;

            if (options.delay) {
                element.delay(options.delay);
            }

            if (hasOptions && $.effects && $.effects.effect[effectName]) {
                element[method](options);
            } else if (effectName !== method && element[effectName]) {
                element[effectName](options.duration, options.easing, callback);
            } else {
                element.queue(function (next) {
                    $(this)[method]();
                    if (callback) {
                        callback.call(element[0]);
                    }
                    next();
                });
            }
        };
    });

    return $.widget;

}));
/*
 * JavaScript Templates
 * https://github.com/blueimp/JavaScript-Templates
 *
 * Copyright 2011, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 *
 * Inspired by John Resig's JavaScript Micro-Templating:
 * http://ejohn.org/blog/javascript-micro-templating/
 */

/*global document, define, module */

; (function ($) {
    /*'use strict'*/
    var tmpl = function (str, data) {
        var f = !/[^\w\-\.:]/.test(str)
          ? tmpl.cache[str] = tmpl.cache[str] || tmpl(tmpl.load(str))
          : new Function(// eslint-disable-line no-new-func
            tmpl.arg + ',tmpl',
            'var _e=tmpl.encode' + tmpl.helper + ",_s='" +
              str.replace(tmpl.regexp, tmpl.func) + "';return _s;"
          )
        return data ? f(data, tmpl) : function (data) {
            return f(data, tmpl)
        }
    }
    tmpl.cache = {}
    tmpl.load = function (id) {
        return document.getElementById(id).innerHTML
    }
    tmpl.regexp = /([\s'\\])(?!(?:[^{]|\{(?!%))*%\})|(?:\{%(=|#)([\s\S]+?)%\})|(\{%)|(%\})/g
    tmpl.func = function (s, p1, p2, p3, p4, p5) {
        if (p1) { // whitespace, quote and backspace in HTML context
            return {
                '\n': '\\n',
                '\r': '\\r',
                '\t': '\\t',
                ' ': ' '
            }[p1] || '\\' + p1
        }
        if (p2) { // interpolation: {%=prop%}, or unescaped: {%#prop%}
            if (p2 === '=') {
                return "'+_e(" + p3 + ")+'"
            }
            return "'+(" + p3 + "==null?'':" + p3 + ")+'"
        }
        if (p4) { // evaluation start tag: {%
            return "';"
        }
        if (p5) { // evaluation end tag: %}
            return "_s+='"
        }
    }
    tmpl.encReg = /[<>&"'\x00]/g
    tmpl.encMap = {
        '<': '&lt;',
        '>': '&gt;',
        '&': '&amp;',
        '"': '&quot;',
        "'": '&#39;'
    }
    tmpl.encode = function (s) {
        return (s == null ? '' : '' + s).replace(
          tmpl.encReg,
          function (c) {
              return tmpl.encMap[c] || ''
          }
        )
    }
    tmpl.arg = 'o'
    tmpl.helper = ",print=function(s,e){_s+=e?(s==null?'':s):_e(s);}" +
                    ',include=function(s,d){_s+=tmpl(s,d);}'
    if (typeof define === 'function' && define.amd) {
        define(function () {
            return tmpl
        })
    } else if (typeof module === 'object' && module.exports) {
        module.exports = tmpl
    } else {
        $.tmpl = tmpl
    }
}(this))
!function (e) { "use strict"; var t = function (e, i, a) { var o, r, n = document.createElement("img"); if (n.onerror = i, n.onload = function () { !r || a && a.noRevoke || t.revokeObjectURL(r), i && i(t.scale(n, a)) }, t.isInstanceOf("Blob", e) || t.isInstanceOf("File", e)) o = r = t.createObjectURL(e), n._type = e.type; else { if ("string" != typeof e) return !1; o = e, a && a.crossOrigin && (n.crossOrigin = a.crossOrigin) } return o ? (n.src = o, n) : t.readFile(e, function (e) { var t = e.target; t && t.result ? n.src = t.result : i && i(e) }) }, i = window.createObjectURL && window || window.URL && URL.revokeObjectURL && URL || window.webkitURL && webkitURL; t.isInstanceOf = function (e, t) { return Object.prototype.toString.call(t) === "[object " + e + "]" }, t.transformCoordinates = function () { }, t.getTransformedOptions = function (e, t) { var i, a, o, r, n = t.aspectRatio; if (!n) return t; i = {}; for (a in t) t.hasOwnProperty(a) && (i[a] = t[a]); return i.crop = !0, o = e.naturalWidth || e.width, r = e.naturalHeight || e.height, o / r > n ? (i.maxWidth = r * n, i.maxHeight = r) : (i.maxWidth = o, i.maxHeight = o / n), i }, t.renderImageToCanvas = function (e, t, i, a, o, r, n, s, l, d) { return e.getContext("2d").drawImage(t, i, a, o, r, n, s, l, d), e }, t.hasCanvasOption = function (e) { return e.canvas || e.crop || !!e.aspectRatio }, t.scale = function (e, i) { function a() { var e = Math.max((s || y) / y, (l || v) / v); e > 1 && (y *= e, v *= e) } function o() { var e = Math.min((r || y) / y, (n || v) / v); 1 > e && (y *= e, v *= e) } i = i || {}; var r, n, s, l, d, u, c, g, f, h, m, p = document.createElement("canvas"), S = e.getContext || t.hasCanvasOption(i) && p.getContext, b = e.naturalWidth || e.width, x = e.naturalHeight || e.height, y = b, v = x; if (S && (i = t.getTransformedOptions(e, i), c = i.left || 0, g = i.top || 0, i.sourceWidth ? (d = i.sourceWidth, void 0 !== i.right && void 0 === i.left && (c = b - d - i.right)) : d = b - c - (i.right || 0), i.sourceHeight ? (u = i.sourceHeight, void 0 !== i.bottom && void 0 === i.top && (g = x - u - i.bottom)) : u = x - g - (i.bottom || 0), y = d, v = u), r = i.maxWidth, n = i.maxHeight, s = i.minWidth, l = i.minHeight, S && r && n && i.crop ? (y = r, v = n, m = d / u - r / n, 0 > m ? (u = n * d / r, void 0 === i.top && void 0 === i.bottom && (g = (x - u) / 2)) : m > 0 && (d = r * u / n, void 0 === i.left && void 0 === i.right && (c = (b - d) / 2))) : ((i.contain || i.cover) && (s = r = r || s, l = n = n || l), i.cover ? (o(), a()) : (a(), o())), S) { if (f = i.pixelRatio, f > 1 && (p.style.width = y + "px", p.style.height = v + "px", y *= f, v *= f, p.getContext("2d").scale(f, f)), h = i.downsamplingRatio, h > 0 && 1 > h && d > y && u > v) for (; d * h > y;) p.width = d * h, p.height = u * h, t.renderImageToCanvas(p, e, c, g, d, u, 0, 0, p.width, p.height), d = p.width, u = p.height, e = document.createElement("canvas"), e.width = d, e.height = u, t.renderImageToCanvas(e, p, 0, 0, d, u, 0, 0, d, u); return p.width = y, p.height = v, t.transformCoordinates(p, i), t.renderImageToCanvas(p, e, c, g, d, u, 0, 0, y, v) } return e.width = y, e.height = v, e }, t.createObjectURL = function (e) { return i ? i.createObjectURL(e) : !1 }, t.revokeObjectURL = function (e) { return i ? i.revokeObjectURL(e) : !1 }, t.readFile = function (e, t, i) { if (window.FileReader) { var a = new FileReader; if (a.onload = a.onerror = t, i = i || "readAsDataURL", a[i]) return a[i](e), a } return !1 }, "function" == typeof define && define.amd ? define(function () { return t }) : "object" == typeof module && module.exports ? module.exports = t : e.loadImage = t }(window), function (e) { "use strict"; "function" == typeof define && define.amd ? define(["./load-image"], e) : e("object" == typeof module && module.exports ? require("./load-image") : window.loadImage) }(function (e) { "use strict"; var t = e.hasCanvasOption, i = e.transformCoordinates, a = e.getTransformedOptions; e.hasCanvasOption = function (i) { return !!i.orientation || t.call(e, i) }, e.transformCoordinates = function (t, a) { i.call(e, t, a); var o = t.getContext("2d"), r = t.width, n = t.height, s = t.style.width, l = t.style.height, d = a.orientation; if (d && !(d > 8)) switch (d > 4 && (t.width = n, t.height = r, t.style.width = l, t.style.height = s), d) { case 2: o.translate(r, 0), o.scale(-1, 1); break; case 3: o.translate(r, n), o.rotate(Math.PI); break; case 4: o.translate(0, n), o.scale(1, -1); break; case 5: o.rotate(.5 * Math.PI), o.scale(1, -1); break; case 6: o.rotate(.5 * Math.PI), o.translate(0, -n); break; case 7: o.rotate(.5 * Math.PI), o.translate(r, -n), o.scale(-1, 1); break; case 8: o.rotate(-.5 * Math.PI), o.translate(-r, 0) } }, e.getTransformedOptions = function (t, i) { var o, r, n = a.call(e, t, i), s = n.orientation; if (!s || s > 8 || 1 === s) return n; o = {}; for (r in n) n.hasOwnProperty(r) && (o[r] = n[r]); switch (n.orientation) { case 2: o.left = n.right, o.right = n.left; break; case 3: o.left = n.right, o.top = n.bottom, o.right = n.left, o.bottom = n.top; break; case 4: o.top = n.bottom, o.bottom = n.top; break; case 5: o.left = n.top, o.top = n.left, o.right = n.bottom, o.bottom = n.right; break; case 6: o.left = n.top, o.top = n.right, o.right = n.bottom, o.bottom = n.left; break; case 7: o.left = n.bottom, o.top = n.right, o.right = n.top, o.bottom = n.left; break; case 8: o.left = n.bottom, o.top = n.left, o.right = n.top, o.bottom = n.right } return n.orientation > 4 && (o.maxWidth = n.maxHeight, o.maxHeight = n.maxWidth, o.minWidth = n.minHeight, o.minHeight = n.minWidth, o.sourceWidth = n.sourceHeight, o.sourceHeight = n.sourceWidth), o } }), function (e) { "use strict"; "function" == typeof define && define.amd ? define(["./load-image"], e) : e("object" == typeof module && module.exports ? require("./load-image") : window.loadImage) }(function (e) { "use strict"; var t = window.Blob && (Blob.prototype.slice || Blob.prototype.webkitSlice || Blob.prototype.mozSlice); e.blobSlice = t && function () { var e = this.slice || this.webkitSlice || this.mozSlice; return e.apply(this, arguments) }, e.metaDataParsers = { jpeg: { 65505: [] } }, e.parseMetaData = function (t, i, a) { a = a || {}; var o = this, r = a.maxMetaDataSize || 262144, n = {}, s = !(window.DataView && t && t.size >= 12 && "image/jpeg" === t.type && e.blobSlice); (s || !e.readFile(e.blobSlice.call(t, 0, r), function (t) { if (t.target.error) return console.log(t.target.error), void i(n); var r, s, l, d, u = t.target.result, c = new DataView(u), g = 2, f = c.byteLength - 4, h = g; if (65496 === c.getUint16(0)) { for (; f > g && (r = c.getUint16(g), r >= 65504 && 65519 >= r || 65534 === r) ;) { if (s = c.getUint16(g + 2) + 2, g + s > c.byteLength) { console.log("Invalid meta data: Invalid segment size."); break } if (l = e.metaDataParsers.jpeg[r]) for (d = 0; d < l.length; d += 1) l[d].call(o, c, g, s, n, a); g += s, h = g } !a.disableImageHead && h > 6 && (u.slice ? n.imageHead = u.slice(0, h) : n.imageHead = new Uint8Array(u).subarray(0, h)) } else console.log("Invalid JPEG file: Missing JPEG marker."); i(n) }, "readAsArrayBuffer")) && i(n) } }), function (e) { "use strict"; "function" == typeof define && define.amd ? define(["./load-image", "./load-image-meta"], e) : "object" == typeof module && module.exports ? e(require("./load-image"), require("./load-image-meta")) : e(window.loadImage) }(function (e) { "use strict"; e.ExifMap = function () { return this }, e.ExifMap.prototype.map = { Orientation: 274 }, e.ExifMap.prototype.get = function (e) { return this[e] || this[this.map[e]] }, e.getExifThumbnail = function (e, t, i) { var a, o, r; if (!i || t + i > e.byteLength) return void console.log("Invalid Exif data: Invalid thumbnail data."); for (a = [], o = 0; i > o; o += 1) r = e.getUint8(t + o), a.push((16 > r ? "0" : "") + r.toString(16)); return "data:image/jpeg,%" + a.join("%") }, e.exifTagTypes = { 1: { getValue: function (e, t) { return e.getUint8(t) }, size: 1 }, 2: { getValue: function (e, t) { return String.fromCharCode(e.getUint8(t)) }, size: 1, ascii: !0 }, 3: { getValue: function (e, t, i) { return e.getUint16(t, i) }, size: 2 }, 4: { getValue: function (e, t, i) { return e.getUint32(t, i) }, size: 4 }, 5: { getValue: function (e, t, i) { return e.getUint32(t, i) / e.getUint32(t + 4, i) }, size: 8 }, 9: { getValue: function (e, t, i) { return e.getInt32(t, i) }, size: 4 }, 10: { getValue: function (e, t, i) { return e.getInt32(t, i) / e.getInt32(t + 4, i) }, size: 8 } }, e.exifTagTypes[7] = e.exifTagTypes[1], e.getExifValue = function (t, i, a, o, r, n) { var s, l, d, u, c, g, f = e.exifTagTypes[o]; if (!f) return void console.log("Invalid Exif data: Invalid tag type."); if (s = f.size * r, l = s > 4 ? i + t.getUint32(a + 8, n) : a + 8, l + s > t.byteLength) return void console.log("Invalid Exif data: Invalid data offset."); if (1 === r) return f.getValue(t, l, n); for (d = [], u = 0; r > u; u += 1) d[u] = f.getValue(t, l + u * f.size, n); if (f.ascii) { for (c = "", u = 0; u < d.length && (g = d[u], "\x00" !== g) ; u += 1) c += g; return c } return d }, e.parseExifTag = function (t, i, a, o, r) { var n = t.getUint16(a, o); r.exif[n] = e.getExifValue(t, i, a, t.getUint16(a + 2, o), t.getUint32(a + 4, o), o) }, e.parseExifTags = function (e, t, i, a, o) { var r, n, s; if (i + 6 > e.byteLength) return void console.log("Invalid Exif data: Invalid directory offset."); if (r = e.getUint16(i, a), n = i + 2 + 12 * r, n + 4 > e.byteLength) return void console.log("Invalid Exif data: Invalid directory size."); for (s = 0; r > s; s += 1) this.parseExifTag(e, t, i + 2 + 12 * s, a, o); return e.getUint32(n, a) }, e.parseExifData = function (t, i, a, o, r) { if (!r.disableExif) { var n, s, l, d = i + 10; if (1165519206 === t.getUint32(i + 4)) { if (d + 8 > t.byteLength) return void console.log("Invalid Exif data: Invalid segment size."); if (0 !== t.getUint16(i + 8)) return void console.log("Invalid Exif data: Missing byte alignment offset."); switch (t.getUint16(d)) { case 18761: n = !0; break; case 19789: n = !1; break; default: return void console.log("Invalid Exif data: Invalid byte alignment marker.") } if (42 !== t.getUint16(d + 2, n)) return void console.log("Invalid Exif data: Missing TIFF marker."); s = t.getUint32(d + 4, n), o.exif = new e.ExifMap, s = e.parseExifTags(t, d, d + s, n, o), s && !r.disableExifThumbnail && (l = { exif: {} }, s = e.parseExifTags(t, d, d + s, n, l), l.exif[513] && (o.exif.Thumbnail = e.getExifThumbnail(t, d + l.exif[513], l.exif[514]))), o.exif[34665] && !r.disableExifSub && e.parseExifTags(t, d, d + o.exif[34665], n, o), o.exif[34853] && !r.disableExifGps && e.parseExifTags(t, d, d + o.exif[34853], n, o) } } }, e.metaDataParsers.jpeg[65505].push(e.parseExifData) }), function (e) { "use strict"; "function" == typeof define && define.amd ? define(["./load-image", "./load-image-exif"], e) : "object" == typeof module && module.exports ? e(require("./load-image"), require("./load-image-exif")) : e(window.loadImage) }(function (e) { "use strict"; e.ExifMap.prototype.tags = { 256: "ImageWidth", 257: "ImageHeight", 34665: "ExifIFDPointer", 34853: "GPSInfoIFDPointer", 40965: "InteroperabilityIFDPointer", 258: "BitsPerSample", 259: "Compression", 262: "PhotometricInterpretation", 274: "Orientation", 277: "SamplesPerPixel", 284: "PlanarConfiguration", 530: "YCbCrSubSampling", 531: "YCbCrPositioning", 282: "XResolution", 283: "YResolution", 296: "ResolutionUnit", 273: "StripOffsets", 278: "RowsPerStrip", 279: "StripByteCounts", 513: "JPEGInterchangeFormat", 514: "JPEGInterchangeFormatLength", 301: "TransferFunction", 318: "WhitePoint", 319: "PrimaryChromaticities", 529: "YCbCrCoefficients", 532: "ReferenceBlackWhite", 306: "DateTime", 270: "ImageDescription", 271: "Make", 272: "Model", 305: "Software", 315: "Artist", 33432: "Copyright", 36864: "ExifVersion", 40960: "FlashpixVersion", 40961: "ColorSpace", 40962: "PixelXDimension", 40963: "PixelYDimension", 42240: "Gamma", 37121: "ComponentsConfiguration", 37122: "CompressedBitsPerPixel", 37500: "MakerNote", 37510: "UserComment", 40964: "RelatedSoundFile", 36867: "DateTimeOriginal", 36868: "DateTimeDigitized", 37520: "SubSecTime", 37521: "SubSecTimeOriginal", 37522: "SubSecTimeDigitized", 33434: "ExposureTime", 33437: "FNumber", 34850: "ExposureProgram", 34852: "SpectralSensitivity", 34855: "PhotographicSensitivity", 34856: "OECF", 34864: "SensitivityType", 34865: "StandardOutputSensitivity", 34866: "RecommendedExposureIndex", 34867: "ISOSpeed", 34868: "ISOSpeedLatitudeyyy", 34869: "ISOSpeedLatitudezzz", 37377: "ShutterSpeedValue", 37378: "ApertureValue", 37379: "BrightnessValue", 37380: "ExposureBias", 37381: "MaxApertureValue", 37382: "SubjectDistance", 37383: "MeteringMode", 37384: "LightSource", 37385: "Flash", 37396: "SubjectArea", 37386: "FocalLength", 41483: "FlashEnergy", 41484: "SpatialFrequencyResponse", 41486: "FocalPlaneXResolution", 41487: "FocalPlaneYResolution", 41488: "FocalPlaneResolutionUnit", 41492: "SubjectLocation", 41493: "ExposureIndex", 41495: "SensingMethod", 41728: "FileSource", 41729: "SceneType", 41730: "CFAPattern", 41985: "CustomRendered", 41986: "ExposureMode", 41987: "WhiteBalance", 41988: "DigitalZoomRatio", 41989: "FocalLengthIn35mmFilm", 41990: "SceneCaptureType", 41991: "GainControl", 41992: "Contrast", 41993: "Saturation", 41994: "Sharpness", 41995: "DeviceSettingDescription", 41996: "SubjectDistanceRange", 42016: "ImageUniqueID", 42032: "CameraOwnerName", 42033: "BodySerialNumber", 42034: "LensSpecification", 42035: "LensMake", 42036: "LensModel", 42037: "LensSerialNumber", 0: "GPSVersionID", 1: "GPSLatitudeRef", 2: "GPSLatitude", 3: "GPSLongitudeRef", 4: "GPSLongitude", 5: "GPSAltitudeRef", 6: "GPSAltitude", 7: "GPSTimeStamp", 8: "GPSSatellites", 9: "GPSStatus", 10: "GPSMeasureMode", 11: "GPSDOP", 12: "GPSSpeedRef", 13: "GPSSpeed", 14: "GPSTrackRef", 15: "GPSTrack", 16: "GPSImgDirectionRef", 17: "GPSImgDirection", 18: "GPSMapDatum", 19: "GPSDestLatitudeRef", 20: "GPSDestLatitude", 21: "GPSDestLongitudeRef", 22: "GPSDestLongitude", 23: "GPSDestBearingRef", 24: "GPSDestBearing", 25: "GPSDestDistanceRef", 26: "GPSDestDistance", 27: "GPSProcessingMethod", 28: "GPSAreaInformation", 29: "GPSDateStamp", 30: "GPSDifferential", 31: "GPSHPositioningError" }, e.ExifMap.prototype.stringValues = { ExposureProgram: { 0: "Undefined", 1: "Manual", 2: "Normal program", 3: "Aperture priority", 4: "Shutter priority", 5: "Creative program", 6: "Action program", 7: "Portrait mode", 8: "Landscape mode" }, MeteringMode: { 0: "Unknown", 1: "Average", 2: "CenterWeightedAverage", 3: "Spot", 4: "MultiSpot", 5: "Pattern", 6: "Partial", 255: "Other" }, LightSource: { 0: "Unknown", 1: "Daylight", 2: "Fluorescent", 3: "Tungsten (incandescent light)", 4: "Flash", 9: "Fine weather", 10: "Cloudy weather", 11: "Shade", 12: "Daylight fluorescent (D 5700 - 7100K)", 13: "Day white fluorescent (N 4600 - 5400K)", 14: "Cool white fluorescent (W 3900 - 4500K)", 15: "White fluorescent (WW 3200 - 3700K)", 17: "Standard light A", 18: "Standard light B", 19: "Standard light C", 20: "D55", 21: "D65", 22: "D75", 23: "D50", 24: "ISO studio tungsten", 255: "Other" }, Flash: { 0: "Flash did not fire", 1: "Flash fired", 5: "Strobe return light not detected", 7: "Strobe return light detected", 9: "Flash fired, compulsory flash mode", 13: "Flash fired, compulsory flash mode, return light not detected", 15: "Flash fired, compulsory flash mode, return light detected", 16: "Flash did not fire, compulsory flash mode", 24: "Flash did not fire, auto mode", 25: "Flash fired, auto mode", 29: "Flash fired, auto mode, return light not detected", 31: "Flash fired, auto mode, return light detected", 32: "No flash function", 65: "Flash fired, red-eye reduction mode", 69: "Flash fired, red-eye reduction mode, return light not detected", 71: "Flash fired, red-eye reduction mode, return light detected", 73: "Flash fired, compulsory flash mode, red-eye reduction mode", 77: "Flash fired, compulsory flash mode, red-eye reduction mode, return light not detected", 79: "Flash fired, compulsory flash mode, red-eye reduction mode, return light detected", 89: "Flash fired, auto mode, red-eye reduction mode", 93: "Flash fired, auto mode, return light not detected, red-eye reduction mode", 95: "Flash fired, auto mode, return light detected, red-eye reduction mode" }, SensingMethod: { 1: "Undefined", 2: "One-chip color area sensor", 3: "Two-chip color area sensor", 4: "Three-chip color area sensor", 5: "Color sequential area sensor", 7: "Trilinear sensor", 8: "Color sequential linear sensor" }, SceneCaptureType: { 0: "Standard", 1: "Landscape", 2: "Portrait", 3: "Night scene" }, SceneType: { 1: "Directly photographed" }, CustomRendered: { 0: "Normal process", 1: "Custom process" }, WhiteBalance: { 0: "Auto white balance", 1: "Manual white balance" }, GainControl: { 0: "None", 1: "Low gain up", 2: "High gain up", 3: "Low gain down", 4: "High gain down" }, Contrast: { 0: "Normal", 1: "Soft", 2: "Hard" }, Saturation: { 0: "Normal", 1: "Low saturation", 2: "High saturation" }, Sharpness: { 0: "Normal", 1: "Soft", 2: "Hard" }, SubjectDistanceRange: { 0: "Unknown", 1: "Macro", 2: "Close view", 3: "Distant view" }, FileSource: { 3: "DSC" }, ComponentsConfiguration: { 0: "", 1: "Y", 2: "Cb", 3: "Cr", 4: "R", 5: "G", 6: "B" }, Orientation: { 1: "top-left", 2: "top-right", 3: "bottom-right", 4: "bottom-left", 5: "left-top", 6: "right-top", 7: "right-bottom", 8: "left-bottom" } }, e.ExifMap.prototype.getText = function (e) { var t = this.get(e); switch (e) { case "LightSource": case "Flash": case "MeteringMode": case "ExposureProgram": case "SensingMethod": case "SceneCaptureType": case "SceneType": case "CustomRendered": case "WhiteBalance": case "GainControl": case "Contrast": case "Saturation": case "Sharpness": case "SubjectDistanceRange": case "FileSource": case "Orientation": return this.stringValues[e][t]; case "ExifVersion": case "FlashpixVersion": return String.fromCharCode(t[0], t[1], t[2], t[3]); case "ComponentsConfiguration": return this.stringValues[e][t[0]] + this.stringValues[e][t[1]] + this.stringValues[e][t[2]] + this.stringValues[e][t[3]]; case "GPSVersionID": return t[0] + "." + t[1] + "." + t[2] + "." + t[3] } return String(t) }, function (e) { var t, i = e.tags, a = e.map; for (t in i) i.hasOwnProperty(t) && (a[i[t]] = t) }(e.ExifMap.prototype), e.ExifMap.prototype.getAll = function () { var e, t, i = {}; for (e in this) this.hasOwnProperty(e) && (t = this.tags[e], t && (i[t] = this.getText(t))); return i } });

!function(a){/*"use strict"*/;var b=a.HTMLCanvasElement&&a.HTMLCanvasElement.prototype,c=a.Blob&&function(){try{return Boolean(new Blob)}catch(a){return!1}}(),d=c&&a.Uint8Array&&function(){try{return 100===new Blob([new Uint8Array(100)]).size}catch(a){return!1}}(),e=a.BlobBuilder||a.WebKitBlobBuilder||a.MozBlobBuilder||a.MSBlobBuilder,f=(c||e)&&a.atob&&a.ArrayBuffer&&a.Uint8Array&&function(a){var b,f,g,h,i,j;for(b=a.split(",")[0].indexOf("base64")>=0?atob(a.split(",")[1]):decodeURIComponent(a.split(",")[1]),f=new ArrayBuffer(b.length),g=new Uint8Array(f),h=0;h<b.length;h+=1)g[h]=b.charCodeAt(h);return i=a.split(",")[0].split(":")[1].split(";")[0],c?new Blob([d?g:f],{type:i}):(j=new e,j.append(f),j.getBlob(i))};a.HTMLCanvasElement&&!b.toBlob&&(b.mozGetAsFile?b.toBlob=function(a,c,d){d&&b.toDataURL&&f?a(f(this.toDataURL(c,d))):a(this.mozGetAsFile("blob",c))}:b.toDataURL&&f&&(b.toBlob=function(a,b,c){a(f(this.toDataURL(b,c)))})),"function"==typeof define&&define.amd?define(function(){return f}):a.dataURLtoBlob=f}(this);
/*
 * jQuery Iframe Transport Plugin
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2011, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * https://opensource.org/licenses/MIT
 */

/* global define, require */

(function(factory) {
  'use strict';
  if (typeof define === 'function' && define.amd) {
    // Register as an anonymous AMD module:
    define(['jquery'], factory);
  } else if (typeof exports === 'object') {
    // Node/CommonJS:
    factory(require('jquery'));
  } else {
    // Browser globals:
    factory(window.jQuery);
  }
})(function($) {
  'use strict';

  // Helper variable to create unique names for the transport iframes:
  var counter = 0,
    jsonAPI = $,
    jsonParse = 'parseJSON';

  if ('JSON' in window && 'parse' in JSON) {
    jsonAPI = JSON;
    jsonParse = 'parse';
  }

  // The iframe transport accepts four additional options:
  // options.fileInput: a jQuery collection of file input fields
  // options.paramName: the parameter name for the file form data,
  //  overrides the name property of the file input field(s),
  //  can be a string or an array of strings.
  // options.formData: an array of objects with name and value properties,
  //  equivalent to the return data of .serializeArray(), e.g.:
  //  [{name: 'a', value: 1}, {name: 'b', value: 2}]
  // options.initialIframeSrc: the URL of the initial iframe src,
  //  by default set to "javascript:false;"
  $.ajaxTransport('iframe', function(options) {
    if (options.async) {
      // javascript:false as initial iframe src
      // prevents warning popups on HTTPS in IE6:
      // eslint-disable-next-line no-script-url
      var initialIframeSrc = options.initialIframeSrc || 'javascript:false;',
        form,
        iframe,
        addParamChar;
      return {
        send: function(_, completeCallback) {
          form = $('<form style="display:none;"></form>');
          form.attr('accept-charset', options.formAcceptCharset);
          addParamChar = /\?/.test(options.url) ? '&' : '?';
          // XDomainRequest only supports GET and POST:
          if (options.type === 'DELETE') {
            options.url = options.url + addParamChar + '_method=DELETE';
            options.type = 'POST';
          } else if (options.type === 'PUT') {
            options.url = options.url + addParamChar + '_method=PUT';
            options.type = 'POST';
          } else if (options.type === 'PATCH') {
            options.url = options.url + addParamChar + '_method=PATCH';
            options.type = 'POST';
          }
          // IE versions below IE8 cannot set the name property of
          // elements that have already been added to the DOM,
          // so we set the name along with the iframe HTML markup:
          counter += 1;
          iframe = $(
            '<iframe src="' +
              initialIframeSrc +
              '" name="iframe-transport-' +
              counter +
              '"></iframe>'
          ).bind('load', function() {
            var fileInputClones,
              paramNames = $.isArray(options.paramName)
                ? options.paramName
                : [options.paramName];
            iframe.unbind('load').bind('load', function() {
              var response;
              // Wrap in a try/catch block to catch exceptions thrown
              // when trying to access cross-domain iframe contents:
              try {
                response = iframe.contents();
                // Google Chrome and Firefox do not throw an
                // exception when calling iframe.contents() on
                // cross-domain requests, so we unify the response:
                if (!response.length || !response[0].firstChild) {
                  throw new Error();
                }
              } catch (e) {
                response = undefined;
              }
              // The complete callback returns the
              // iframe content document as response object:
              completeCallback(200, 'success', { iframe: response });
              // Fix for IE endless progress bar activity bug
              // (happens on form submits to iframe targets):
              $('<iframe src="' + initialIframeSrc + '"></iframe>').appendTo(
                form
              );
              window.setTimeout(function() {
                // Removing the form in a setTimeout call
                // allows Chrome's developer tools to display
                // the response result
                form.remove();
              }, 0);
            });
            form
              .prop('target', iframe.prop('name'))
              .prop('action', options.url)
              .prop('method', options.type);
            if (options.formData) {
              $.each(options.formData, function(index, field) {
                $('<input type="hidden"/>')
                  .prop('name', field.name)
                  .val(field.value)
                  .appendTo(form);
              });
            }
            if (
              options.fileInput &&
              options.fileInput.length &&
              options.type === 'POST'
            ) {
              fileInputClones = options.fileInput.clone();
              // Insert a clone for each file input field:
              options.fileInput.after(function(index) {
                return fileInputClones[index];
              });
              if (options.paramName) {
                options.fileInput.each(function(index) {
                  $(this).prop('name', paramNames[index] || options.paramName);
                });
              }
              // Appending the file input fields to the hidden form
              // removes them from their original location:
              form
                .append(options.fileInput)
                .prop('enctype', 'multipart/form-data')
                // enctype must be set as encoding for IE:
                .prop('encoding', 'multipart/form-data');
              // Remove the HTML5 form attribute from the input(s):
              options.fileInput.removeAttr('form');
            }
            form.submit();
            // Insert the file input fields at their original location
            // by replacing the clones with the originals:
            if (fileInputClones && fileInputClones.length) {
              options.fileInput.each(function(index, input) {
                var clone = $(fileInputClones[index]);
                // Restore the original name and form properties:
                $(input)
                  .prop('name', clone.prop('name'))
                  .attr('form', clone.attr('form'));
                clone.replaceWith(input);
              });
            }
          });
          form.append(iframe).appendTo(document.body);
        },
        abort: function() {
          if (iframe) {
            // javascript:false as iframe src aborts the request
            // and prevents warning popups on HTTPS in IE6.
            iframe.unbind('load').prop('src', initialIframeSrc);
          }
          if (form) {
            form.remove();
          }
        }
      };
    }
  });

  // The iframe transport returns the iframe content document as response.
  // The following adds converters from iframe to text, json, html, xml
  // and script.
  // Please note that the Content-Type for JSON responses has to be text/plain
  // or text/html, if the browser doesn't include application/json in the
  // Accept header, else IE will show a download dialog.
  // The Content-Type for XML responses on the other hand has to be always
  // application/xml or text/xml, so IE properly parses the XML response.
  // See also
  // https://github.com/blueimp/jQuery-File-Upload/wiki/Setup#content-type-negotiation
  $.ajaxSetup({
    converters: {
      'iframe text': function(iframe) {
        return iframe && $(iframe[0].body).text();
      },
      'iframe json': function(iframe) {
        return iframe && jsonAPI[jsonParse]($(iframe[0].body).text());
      },
      'iframe html': function(iframe) {
        return iframe && $(iframe[0].body).html();
      },
      'iframe xml': function(iframe) {
        var xmlDoc = iframe && iframe[0];
        return xmlDoc && $.isXMLDoc(xmlDoc)
          ? xmlDoc
          : $.parseXML(
              (xmlDoc.XMLDocument && xmlDoc.XMLDocument.xml) ||
                $(xmlDoc.body).html()
            );
      },
      'iframe script': function(iframe) {
        return iframe && $.globalEval($(iframe[0].body).text());
      }
    }
  });
});

/*
 * jQuery File Upload Plugin
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * https://opensource.org/licenses/MIT
 */

/* global define, require */
/* eslint-disable new-cap */

(function(factory) {
  'use strict';
  if (typeof define === 'function' && define.amd) {
    // Register as an anonymous AMD module:
    define(['jquery', 'jquery-ui/ui/widget'], factory);
  } else if (typeof exports === 'object') {
    // Node/CommonJS:
    factory(require('jquery'), require('./vendor/jquery.ui.widget'));
  } else {
    // Browser globals:
    factory(window.jQuery);
  }
})(function($) {
  'use strict';

  // Detect file input support, based on
  // https://viljamis.com/2012/file-upload-support-on-mobile/
  $.support.fileInput = !(
    new RegExp(
      // Handle devices which give false positives for the feature detection:
      '(Android (1\\.[0156]|2\\.[01]))' +
        '|(Windows Phone (OS 7|8\\.0))|(XBLWP)|(ZuneWP)|(WPDesktop)' +
        '|(w(eb)?OSBrowser)|(webOS)' +
        '|(Kindle/(1\\.0|2\\.[05]|3\\.0))'
    ).test(window.navigator.userAgent) ||
    // Feature detection for all other devices:
    $('<input type="file"/>').prop('disabled')
  );

  // The FileReader API is not actually used, but works as feature detection,
  // as some Safari versions (5?) support XHR file uploads via the FormData API,
  // but not non-multipart XHR file uploads.
  // window.XMLHttpRequestUpload is not available on IE10, so we check for
  // window.ProgressEvent instead to detect XHR2 file upload capability:
  $.support.xhrFileUpload = !!(window.ProgressEvent && window.FileReader);
  $.support.xhrFormDataFileUpload = !!window.FormData;

  // Detect support for Blob slicing (required for chunked uploads):
  $.support.blobSlice =
    window.Blob &&
    (Blob.prototype.slice ||
      Blob.prototype.webkitSlice ||
      Blob.prototype.mozSlice);

  /**
   * Helper function to create drag handlers for dragover/dragenter/dragleave
   *
   * @param {string} type Event type
   * @returns {Function} Drag handler
   */
  function getDragHandler(type) {
    var isDragOver = type === 'dragover';
    return function(e) {
      e.dataTransfer = e.originalEvent && e.originalEvent.dataTransfer;
      var dataTransfer = e.dataTransfer;
      if (
        dataTransfer &&
        $.inArray('Files', dataTransfer.types) !== -1 &&
        this._trigger(type, $.Event(type, { delegatedEvent: e })) !== false
      ) {
        e.preventDefault();
        if (isDragOver) {
          dataTransfer.dropEffect = 'copy';
        }
      }
    };
  }

  // The fileupload widget listens for change events on file input fields defined
  // via fileInput setting and paste or drop events of the given dropZone.
  // In addition to the default jQuery Widget methods, the fileupload widget
  // exposes the "add" and "send" methods, to add or directly send files using
  // the fileupload API.
  // By default, files added via file input selection, paste, drag & drop or
  // "add" method are uploaded immediately, but it is possible to override
  // the "add" callback option to queue file uploads.
  $.widget('blueimp.yafFileUpload', {
    options: {
      // The drop target element(s), by the default the complete document.
      // Set to null to disable drag & drop support:
      dropZone: $(document),
      // The paste target element(s), by the default undefined.
      // Set to a DOM node or jQuery object to enable file pasting:
      pasteZone: undefined,
      // The file input field(s), that are listened to for change events.
      // If undefined, it is set to the file input fields inside
      // of the widget element on plugin initialization.
      // Set to null to disable the change listener.
      fileInput: undefined,
      // By default, the file input field is replaced with a clone after
      // each input field change event. This is required for iframe transport
      // queues and allows change events to be fired for the same file
      // selection, but can be disabled by setting the following option to false:
      replaceFileInput: true,
      // The parameter name for the file form data (the request argument name).
      // If undefined or empty, the name property of the file input field is
      // used, or "files[]" if the file input name property is also empty,
      // can be a string or an array of strings:
      paramName: undefined,
      // By default, each file of a selection is uploaded using an individual
      // request for XHR type uploads. Set to false to upload file
      // selections in one request each:
      singleFileUploads: true,
      // To limit the number of files uploaded with one XHR request,
      // set the following option to an integer greater than 0:
      limitMultiFileUploads: undefined,
      // The following option limits the number of files uploaded with one
      // XHR request to keep the request size under or equal to the defined
      // limit in bytes:
      limitMultiFileUploadSize: undefined,
      // Multipart file uploads add a number of bytes to each uploaded file,
      // therefore the following option adds an overhead for each file used
      // in the limitMultiFileUploadSize configuration:
      limitMultiFileUploadSizeOverhead: 512,
      // Set the following option to true to issue all file upload requests
      // in a sequential order:
      sequentialUploads: false,
      // To limit the number of concurrent uploads,
      // set the following option to an integer greater than 0:
      limitConcurrentUploads: undefined,
      // Set the following option to true to force iframe transport uploads:
      forceIframeTransport: false,
      // Set the following option to the location of a redirect url on the
      // origin server, for cross-domain iframe transport uploads:
      redirect: undefined,
      // The parameter name for the redirect url, sent as part of the form
      // data and set to 'redirect' if this option is empty:
      redirectParamName: undefined,
      // Set the following option to the location of a postMessage window,
      // to enable postMessage transport uploads:
      postMessage: undefined,
      // By default, XHR file uploads are sent as multipart/form-data.
      // The iframe transport is always using multipart/form-data.
      // Set to false to enable non-multipart XHR uploads:
      multipart: true,
      // To upload large files in smaller chunks, set the following option
      // to a preferred maximum chunk size. If set to 0, null or undefined,
      // or the browser does not support the required Blob API, files will
      // be uploaded as a whole.
      maxChunkSize: undefined,
      // When a non-multipart upload or a chunked multipart upload has been
      // aborted, this option can be used to resume the upload by setting
      // it to the size of the already uploaded bytes. This option is most
      // useful when modifying the options object inside of the "add" or
      // "send" callbacks, as the options are cloned for each file upload.
      uploadedBytes: undefined,
      // By default, failed (abort or error) file uploads are removed from the
      // global progress calculation. Set the following option to false to
      // prevent recalculating the global progress data:
      recalculateProgress: true,
      // Interval in milliseconds to calculate and trigger progress events:
      progressInterval: 100,
      // Interval in milliseconds to calculate progress bitrate:
      bitrateInterval: 500,
      // By default, uploads are started automatically when adding files:
      autoUpload: true,
      // By default, duplicate file names are expected to be handled on
      // the server-side. If this is not possible (e.g. when uploading
      // files directly to Amazon S3), the following option can be set to
      // an empty object or an object mapping existing filenames, e.g.:
      // { "image.jpg": true, "image (1).jpg": true }
      // If it is set, all files will be uploaded with unique filenames,
      // adding increasing number suffixes if necessary, e.g.:
      // "image (2).jpg"
      uniqueFilenames: undefined,

      // Error and info messages:
      messages: {
        uploadedBytes: 'Uploaded bytes exceed file size'
      },

      // Translation function, gets the message key to be translated
      // and an object with context specific data as arguments:
      i18n: function(message, context) {
        // eslint-disable-next-line no-param-reassign
        message = this.messages[message] || message.toString();
        if (context) {
          $.each(context, function(key, value) {
            // eslint-disable-next-line no-param-reassign
            message = message.replace('{' + key + '}', value);
          });
        }
        return message;
      },

      // Additional form data to be sent along with the file uploads can be set
      // using this option, which accepts an array of objects with name and
      // value properties, a function returning such an array, a FormData
      // object (for XHR file uploads), or a simple object.
      // The form of the first fileInput is given as parameter to the function:
      formData: function(form) {
        return form.serializeArray();
      },

      // The add callback is invoked as soon as files are added to the fileupload
      // widget (via file input selection, drag & drop, paste or add API call).
      // If the singleFileUploads option is enabled, this callback will be
      // called once for each file in the selection for XHR file uploads, else
      // once for each file selection.
      //
      // The upload starts when the submit method is invoked on the data parameter.
      // The data object contains a files property holding the added files
      // and allows you to override plugin options as well as define ajax settings.
      //
      // Listeners for this callback can also be bound the following way:
      // .bind('fileuploadadd', func);
      //
      // data.submit() returns a Promise object and allows to attach additional
      // handlers using jQuery's Deferred callbacks:
      // data.submit().done(func).fail(func).always(func);
      add: function(e, data) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        if (
          data.autoUpload ||
          (data.autoUpload !== false &&
            $(this).yafFileUpload('option', 'autoUpload'))
        ) {
          data.process().done(function() {
            data.submit();
          });
        }
      },

      // Other callbacks:

      // Callback for the submit event of each file upload:
      // submit: function (e, data) {}, // .bind('fileuploadsubmit', func);

      // Callback for the start of each file upload request:
      // send: function (e, data) {}, // .bind('fileuploadsend', func);

      // Callback for successful uploads:
      // done: function (e, data) {}, // .bind('fileuploaddone', func);

      // Callback for failed (abort or error) uploads:
      // fail: function (e, data) {}, // .bind('fileuploadfail', func);

      // Callback for completed (success, abort or error) requests:
      // always: function (e, data) {}, // .bind('fileuploadalways', func);

      // Callback for upload progress events:
      // progress: function (e, data) {}, // .bind('fileuploadprogress', func);

      // Callback for global upload progress events:
      // progressall: function (e, data) {}, // .bind('fileuploadprogressall', func);

      // Callback for uploads start, equivalent to the global ajaxStart event:
      // start: function (e) {}, // .bind('fileuploadstart', func);

      // Callback for uploads stop, equivalent to the global ajaxStop event:
      // stop: function (e) {}, // .bind('fileuploadstop', func);

      // Callback for change events of the fileInput(s):
      // change: function (e, data) {}, // .bind('fileuploadchange', func);

      // Callback for paste events to the pasteZone(s):
      // paste: function (e, data) {}, // .bind('fileuploadpaste', func);

      // Callback for drop events of the dropZone(s):
      // drop: function (e, data) {}, // .bind('fileuploaddrop', func);

      // Callback for dragover events of the dropZone(s):
      // dragover: function (e) {}, // .bind('fileuploaddragover', func);

      // Callback before the start of each chunk upload request (before form data initialization):
      // chunkbeforesend: function (e, data) {}, // .bind('fileuploadchunkbeforesend', func);

      // Callback for the start of each chunk upload request:
      // chunksend: function (e, data) {}, // .bind('fileuploadchunksend', func);

      // Callback for successful chunk uploads:
      // chunkdone: function (e, data) {}, // .bind('fileuploadchunkdone', func);

      // Callback for failed (abort or error) chunk uploads:
      // chunkfail: function (e, data) {}, // .bind('fileuploadchunkfail', func);

      // Callback for completed (success, abort or error) chunk upload requests:
      // chunkalways: function (e, data) {}, // .bind('fileuploadchunkalways', func);

      // The plugin options are used as settings object for the ajax calls.
      // The following are jQuery ajax settings required for the file uploads:
      processData: false,
      contentType: false,
      cache: false,
      timeout: 0
    },

    // A list of options that require reinitializing event listeners and/or
    // special initialization code:
    _specialOptions: [
      'fileInput',
      'dropZone',
      'pasteZone',
      'multipart',
      'forceIframeTransport'
    ],

    _blobSlice:
      $.support.blobSlice &&
      function() {
        var slice = this.slice || this.webkitSlice || this.mozSlice;
        return slice.apply(this, arguments);
      },

    _BitrateTimer: function() {
      this.timestamp = Date.now ? Date.now() : new Date().getTime();
      this.loaded = 0;
      this.bitrate = 0;
      this.getBitrate = function(now, loaded, interval) {
        var timeDiff = now - this.timestamp;
        if (!this.bitrate || !interval || timeDiff > interval) {
          this.bitrate = (loaded - this.loaded) * (1000 / timeDiff) * 8;
          this.loaded = loaded;
          this.timestamp = now;
        }
        return this.bitrate;
      };
    },

    _isXHRUpload: function(options) {
      return (
        !options.forceIframeTransport &&
        ((!options.multipart && $.support.xhrFileUpload) ||
          $.support.xhrFormDataFileUpload)
      );
    },

    _getFormData: function(options) {
      var formData;
      if ($.type(options.formData) === 'function') {
        return options.formData(options.form);
      }
      if ($.isArray(options.formData)) {
        return options.formData;
      }
      if ($.type(options.formData) === 'object') {
        formData = [];
        $.each(options.formData, function(name, value) {
          formData.push({ name: name, value: value });
        });
        return formData;
      }
      return [];
    },

    _getTotal: function(files) {
      var total = 0;
      $.each(files, function(index, file) {
        total += file.size || 1;
      });
      return total;
    },

    _initProgressObject: function(obj) {
      var progress = {
        loaded: 0,
        total: 0,
        bitrate: 0
      };
      if (obj._progress) {
        $.extend(obj._progress, progress);
      } else {
        obj._progress = progress;
      }
    },

    _initResponseObject: function(obj) {
      var prop;
      if (obj._response) {
        for (prop in obj._response) {
          if (Object.prototype.hasOwnProperty.call(obj._response, prop)) {
            delete obj._response[prop];
          }
        }
      } else {
        obj._response = {};
      }
    },

    _onProgress: function(e, data) {
      if (e.lengthComputable) {
        var now = Date.now ? Date.now() : new Date().getTime(),
          loaded;
        if (
          data._time &&
          data.progressInterval &&
          now - data._time < data.progressInterval &&
          e.loaded !== e.total
        ) {
          return;
        }
        data._time = now;
        loaded =
          Math.floor(
            (e.loaded / e.total) * (data.chunkSize || data._progress.total)
          ) + (data.uploadedBytes || 0);
        // Add the difference from the previously loaded state
        // to the global loaded counter:
        this._progress.loaded += loaded - data._progress.loaded;
        this._progress.bitrate = this._bitrateTimer.getBitrate(
          now,
          this._progress.loaded,
          data.bitrateInterval
        );
        data._progress.loaded = data.loaded = loaded;
        data._progress.bitrate = data.bitrate = data._bitrateTimer.getBitrate(
          now,
          loaded,
          data.bitrateInterval
        );
        // Trigger a custom progress event with a total data property set
        // to the file size(s) of the current upload and a loaded data
        // property calculated accordingly:
        this._trigger(
          'progress',
          $.Event('progress', { delegatedEvent: e }),
          data
        );
        // Trigger a global progress event for all current file uploads,
        // including ajax calls queued for sequential file uploads:
        this._trigger(
          'progressall',
          $.Event('progressall', { delegatedEvent: e }),
          this._progress
        );
      }
    },

    _initProgressListener: function(options) {
      var that = this,
        xhr = options.xhr ? options.xhr() : $.ajaxSettings.xhr();
      // Accesss to the native XHR object is required to add event listeners
      // for the upload progress event:
      if (xhr.upload) {
        $(xhr.upload).bind('progress', function(e) {
          var oe = e.originalEvent;
          // Make sure the progress event properties get copied over:
          e.lengthComputable = oe.lengthComputable;
          e.loaded = oe.loaded;
          e.total = oe.total;
          that._onProgress(e, options);
        });
        options.xhr = function() {
          return xhr;
        };
      }
    },

    _deinitProgressListener: function(options) {
      var xhr = options.xhr ? options.xhr() : $.ajaxSettings.xhr();
      if (xhr.upload) {
        $(xhr.upload).unbind('progress');
      }
    },

    _isInstanceOf: function(type, obj) {
      // Cross-frame instanceof check
      return Object.prototype.toString.call(obj) === '[object ' + type + ']';
    },

    _getUniqueFilename: function(name, map) {
      // eslint-disable-next-line no-param-reassign
      name = String(name);
      if (map[name]) {
        // eslint-disable-next-line no-param-reassign
        name = name.replace(/(?: \(([\d]+)\))?(\.[^.]+)?$/, function(
          _,
          p1,
          p2
        ) {
          var index = p1 ? Number(p1) + 1 : 1;
          var ext = p2 || '';
          return ' (' + index + ')' + ext;
        });
        return this._getUniqueFilename(name, map);
      }
      map[name] = true;
      return name;
    },

    _initXHRData: function(options) {
      var that = this,
        formData,
        file = options.files[0],
        // Ignore non-multipart setting if not supported:
        multipart = options.multipart || !$.support.xhrFileUpload,
        paramName =
          $.type(options.paramName) === 'array'
            ? options.paramName[0]
            : options.paramName;
      options.headers = $.extend({}, options.headers);
      if (options.contentRange) {
        options.headers['Content-Range'] = options.contentRange;
      }
      if (!multipart || options.blob || !this._isInstanceOf('File', file)) {
        options.headers['Content-Disposition'] =
          'attachment; filename="' +
          encodeURI(file.uploadName || file.name) +
          '"';
      }
      if (!multipart) {
        options.contentType = file.type || 'application/octet-stream';
        options.data = options.blob || file;
      } else if ($.support.xhrFormDataFileUpload) {
        if (options.postMessage) {
          // window.postMessage does not allow sending FormData
          // objects, so we just add the File/Blob objects to
          // the formData array and let the postMessage window
          // create the FormData object out of this array:
          formData = this._getFormData(options);
          if (options.blob) {
            formData.push({
              name: paramName,
              value: options.blob
            });
          } else {
            $.each(options.files, function(index, file) {
              formData.push({
                name:
                  ($.type(options.paramName) === 'array' &&
                    options.paramName[index]) ||
                  paramName,
                value: file
              });
            });
          }
        } else {
          if (that._isInstanceOf('FormData', options.formData)) {
            formData = options.formData;
          } else {
            formData = new FormData();
            $.each(this._getFormData(options), function(index, field) {
              formData.append(field.name, field.value);
            });
          }
          if (options.blob) {
            formData.append(
              paramName,
              options.blob,
              file.uploadName || file.name
            );
          } else {
            $.each(options.files, function(index, file) {
              // This check allows the tests to run with
              // dummy objects:
              if (
                that._isInstanceOf('File', file) ||
                that._isInstanceOf('Blob', file)
              ) {
                var fileName = file.uploadName || file.name;
                if (options.uniqueFilenames) {
                  fileName = that._getUniqueFilename(
                    fileName,
                    options.uniqueFilenames
                  );
                }
                formData.append(
                  ($.type(options.paramName) === 'array' &&
                    options.paramName[index]) ||
                    paramName,
                  file,
                  fileName
                );
              }
            });
          }
        }
        options.data = formData;
      }
      // Blob reference is not needed anymore, free memory:
      options.blob = null;
    },

    _initIframeSettings: function(options) {
      var targetHost = $('<a></a>')
        .prop('href', options.url)
        .prop('host');
      // Setting the dataType to iframe enables the iframe transport:
      options.dataType = 'iframe ' + (options.dataType || '');
      // The iframe transport accepts a serialized array as form data:
      options.formData = this._getFormData(options);
      // Add redirect url to form data on cross-domain uploads:
      if (options.redirect && targetHost && targetHost !== location.host) {
        options.formData.push({
          name: options.redirectParamName || 'redirect',
          value: options.redirect
        });
      }
    },

    _initDataSettings: function(options) {
      if (this._isXHRUpload(options)) {
        if (!this._chunkedUpload(options, true)) {
          if (!options.data) {
            this._initXHRData(options);
          }
          this._initProgressListener(options);
        }
        if (options.postMessage) {
          // Setting the dataType to postmessage enables the
          // postMessage transport:
          options.dataType = 'postmessage ' + (options.dataType || '');
        }
      } else {
        this._initIframeSettings(options);
      }
    },

    _getParamName: function(options) {
      var fileInput = $(options.fileInput),
        paramName = options.paramName;
      if (!paramName) {
        paramName = [];
        fileInput.each(function() {
          var input = $(this),
            name = input.prop('name') || 'files[]',
            i = (input.prop('files') || [1]).length;
          while (i) {
            paramName.push(name);
            i -= 1;
          }
        });
        if (!paramName.length) {
          paramName = [fileInput.prop('name') || 'files[]'];
        }
      } else if (!$.isArray(paramName)) {
        paramName = [paramName];
      }
      return paramName;
    },

    _initFormSettings: function(options) {
      // Retrieve missing options from the input field and the
      // associated form, if available:
      if (!options.form || !options.form.length) {
        options.form = $(options.fileInput.prop('form'));
        // If the given file input doesn't have an associated form,
        // use the default widget file input's form:
        if (!options.form.length) {
          options.form = $(this.options.fileInput.prop('form'));
        }
      }
      options.paramName = this._getParamName(options);
      if (!options.url) {
        options.url = options.form.prop('action') || location.href;
      }
      // The HTTP request method must be "POST" or "PUT":
      options.type = (
        options.type ||
        ($.type(options.form.prop('method')) === 'string' &&
          options.form.prop('method')) ||
        ''
      ).toUpperCase();
      if (
        options.type !== 'POST' &&
        options.type !== 'PUT' &&
        options.type !== 'PATCH'
      ) {
        options.type = 'POST';
      }
      if (!options.formAcceptCharset) {
        options.formAcceptCharset = options.form.attr('accept-charset');
      }
    },

    _getAJAXSettings: function(data) {
      var options = $.extend({}, this.options, data);
      this._initFormSettings(options);
      this._initDataSettings(options);
      return options;
    },

    // jQuery 1.6 doesn't provide .state(),
    // while jQuery 1.8+ removed .isRejected() and .isResolved():
    _getDeferredState: function(deferred) {
      if (deferred.state) {
        return deferred.state();
      }
      if (deferred.isResolved()) {
        return 'resolved';
      }
      if (deferred.isRejected()) {
        return 'rejected';
      }
      return 'pending';
    },

    // Maps jqXHR callbacks to the equivalent
    // methods of the given Promise object:
    _enhancePromise: function(promise) {
      promise.success = promise.done;
      promise.error = promise.fail;
      promise.complete = promise.always;
      return promise;
    },

    // Creates and returns a Promise object enhanced with
    // the jqXHR methods abort, success, error and complete:
    _getXHRPromise: function(resolveOrReject, context, args) {
      var dfd = $.Deferred(),
        promise = dfd.promise();
      // eslint-disable-next-line no-param-reassign
      context = context || this.options.context || promise;
      if (resolveOrReject === true) {
        dfd.resolveWith(context, args);
      } else if (resolveOrReject === false) {
        dfd.rejectWith(context, args);
      }
      promise.abort = dfd.promise;
      return this._enhancePromise(promise);
    },

    // Adds convenience methods to the data callback argument:
    _addConvenienceMethods: function(e, data) {
      var that = this,
        getPromise = function(args) {
          return $.Deferred()
            .resolveWith(that, args)
            .promise();
        };
      data.process = function(resolveFunc, rejectFunc) {
        if (resolveFunc || rejectFunc) {
          data._processQueue = this._processQueue = (
            this._processQueue || getPromise([this])
          )
            .then(function() {
              if (data.errorThrown) {
                return $.Deferred()
                  .rejectWith(that, [data])
                  .promise();
              }
              return getPromise(arguments);
            })
            .then(resolveFunc, rejectFunc);
        }
        return this._processQueue || getPromise([this]);
      };
      data.submit = function() {
        if (this.state() !== 'pending') {
          data.jqXHR = this.jqXHR =
            that._trigger(
              'submit',
              $.Event('submit', { delegatedEvent: e }),
              this
            ) !== false && that._onSend(e, this);
        }
        return this.jqXHR || that._getXHRPromise();
      };
      data.abort = function() {
        if (this.jqXHR) {
          return this.jqXHR.abort();
        }
        this.errorThrown = 'abort';
        that._trigger('fail', null, this);
        return that._getXHRPromise(false);
      };
      data.state = function() {
        if (this.jqXHR) {
          return that._getDeferredState(this.jqXHR);
        }
        if (this._processQueue) {
          return that._getDeferredState(this._processQueue);
        }
      };
      data.processing = function() {
        return (
          !this.jqXHR &&
          this._processQueue &&
          that._getDeferredState(this._processQueue) === 'pending'
        );
      };
      data.progress = function() {
        return this._progress;
      };
      data.response = function() {
        return this._response;
      };
    },

    // Parses the Range header from the server response
    // and returns the uploaded bytes:
    _getUploadedBytes: function(jqXHR) {
      var range = jqXHR.getResponseHeader('Range'),
        parts = range && range.split('-'),
        upperBytesPos = parts && parts.length > 1 && parseInt(parts[1], 10);
      return upperBytesPos && upperBytesPos + 1;
    },

    // Uploads a file in multiple, sequential requests
    // by splitting the file up in multiple blob chunks.
    // If the second parameter is true, only tests if the file
    // should be uploaded in chunks, but does not invoke any
    // upload requests:
    _chunkedUpload: function(options, testOnly) {
      options.uploadedBytes = options.uploadedBytes || 0;
      var that = this,
        file = options.files[0],
        fs = file.size,
        ub = options.uploadedBytes,
        mcs = options.maxChunkSize || fs,
        slice = this._blobSlice,
        dfd = $.Deferred(),
        promise = dfd.promise(),
        jqXHR,
        upload;
      if (
        !(
          this._isXHRUpload(options) &&
          slice &&
          (ub || ($.type(mcs) === 'function' ? mcs(options) : mcs) < fs)
        ) ||
        options.data
      ) {
        return false;
      }
      if (testOnly) {
        return true;
      }
      if (ub >= fs) {
        file.error = options.i18n('uploadedBytes');
        return this._getXHRPromise(false, options.context, [
          null,
          'error',
          file.error
        ]);
      }
      // The chunk upload method:
      upload = function() {
        // Clone the options object for each chunk upload:
        var o = $.extend({}, options),
          currentLoaded = o._progress.loaded;
        o.blob = slice.call(
          file,
          ub,
          ub + ($.type(mcs) === 'function' ? mcs(o) : mcs),
          file.type
        );
        // Store the current chunk size, as the blob itself
        // will be dereferenced after data processing:
        o.chunkSize = o.blob.size;
        // Expose the chunk bytes position range:
        o.contentRange =
          'bytes ' + ub + '-' + (ub + o.chunkSize - 1) + '/' + fs;
        // Trigger chunkbeforesend to allow form data to be updated for this chunk
        that._trigger('chunkbeforesend', null, o);
        // Process the upload data (the blob and potential form data):
        that._initXHRData(o);
        // Add progress listeners for this chunk upload:
        that._initProgressListener(o);
        jqXHR = (
          (that._trigger('chunksend', null, o) !== false && $.ajax(o)) ||
          that._getXHRPromise(false, o.context)
        )
          .done(function(result, textStatus, jqXHR) {
            ub = that._getUploadedBytes(jqXHR) || ub + o.chunkSize;
            // Create a progress event if no final progress event
            // with loaded equaling total has been triggered
            // for this chunk:
            if (currentLoaded + o.chunkSize - o._progress.loaded) {
              that._onProgress(
                $.Event('progress', {
                  lengthComputable: true,
                  loaded: ub - o.uploadedBytes,
                  total: ub - o.uploadedBytes
                }),
                o
              );
            }
            options.uploadedBytes = o.uploadedBytes = ub;
            o.result = result;
            o.textStatus = textStatus;
            o.jqXHR = jqXHR;
            that._trigger('chunkdone', null, o);
            that._trigger('chunkalways', null, o);
            if (ub < fs) {
              // File upload not yet complete,
              // continue with the next chunk:
              upload();
            } else {
              dfd.resolveWith(o.context, [result, textStatus, jqXHR]);
            }
          })
          .fail(function(jqXHR, textStatus, errorThrown) {
            o.jqXHR = jqXHR;
            o.textStatus = textStatus;
            o.errorThrown = errorThrown;
            that._trigger('chunkfail', null, o);
            that._trigger('chunkalways', null, o);
            dfd.rejectWith(o.context, [jqXHR, textStatus, errorThrown]);
          })
          .always(function() {
            that._deinitProgressListener(o);
          });
      };
      this._enhancePromise(promise);
      promise.abort = function() {
        return jqXHR.abort();
      };
      upload();
      return promise;
    },

    _beforeSend: function(e, data) {
      if (this._active === 0) {
        // the start callback is triggered when an upload starts
        // and no other uploads are currently running,
        // equivalent to the global ajaxStart event:
        this._trigger('start');
        // Set timer for global bitrate progress calculation:
        this._bitrateTimer = new this._BitrateTimer();
        // Reset the global progress values:
        this._progress.loaded = this._progress.total = 0;
        this._progress.bitrate = 0;
      }
      // Make sure the container objects for the .response() and
      // .progress() methods on the data object are available
      // and reset to their initial state:
      this._initResponseObject(data);
      this._initProgressObject(data);
      data._progress.loaded = data.loaded = data.uploadedBytes || 0;
      data._progress.total = data.total = this._getTotal(data.files) || 1;
      data._progress.bitrate = data.bitrate = 0;
      this._active += 1;
      // Initialize the global progress values:
      this._progress.loaded += data.loaded;
      this._progress.total += data.total;
    },

    _onDone: function(result, textStatus, jqXHR, options) {
      var total = options._progress.total,
        response = options._response;
      if (options._progress.loaded < total) {
        // Create a progress event if no final progress event
        // with loaded equaling total has been triggered:
        this._onProgress(
          $.Event('progress', {
            lengthComputable: true,
            loaded: total,
            total: total
          }),
          options
        );
      }
      response.result = options.result = result;
      response.textStatus = options.textStatus = textStatus;
      response.jqXHR = options.jqXHR = jqXHR;
      this._trigger('done', null, options);
    },

    _onFail: function(jqXHR, textStatus, errorThrown, options) {
      var response = options._response;
      if (options.recalculateProgress) {
        // Remove the failed (error or abort) file upload from
        // the global progress calculation:
        this._progress.loaded -= options._progress.loaded;
        this._progress.total -= options._progress.total;
      }
      response.jqXHR = options.jqXHR = jqXHR;
      response.textStatus = options.textStatus = textStatus;
      response.errorThrown = options.errorThrown = errorThrown;
      this._trigger('fail', null, options);
    },

    _onAlways: function(jqXHRorResult, textStatus, jqXHRorError, options) {
      // jqXHRorResult, textStatus and jqXHRorError are added to the
      // options object via done and fail callbacks
      this._trigger('always', null, options);
    },

    _onSend: function(e, data) {
      if (!data.submit) {
        this._addConvenienceMethods(e, data);
      }
      var that = this,
        jqXHR,
        aborted,
        slot,
        pipe,
        options = that._getAJAXSettings(data),
        send = function() {
          that._sending += 1;
          // Set timer for bitrate progress calculation:
          options._bitrateTimer = new that._BitrateTimer();
          jqXHR =
            jqXHR ||
            (
              ((aborted ||
                that._trigger(
                  'send',
                  $.Event('send', { delegatedEvent: e }),
                  options
                ) === false) &&
                that._getXHRPromise(false, options.context, aborted)) ||
              that._chunkedUpload(options) ||
              $.ajax(options)
            )
              .done(function(result, textStatus, jqXHR) {
                that._onDone(result, textStatus, jqXHR, options);
              })
              .fail(function(jqXHR, textStatus, errorThrown) {
                that._onFail(jqXHR, textStatus, errorThrown, options);
              })
              .always(function(jqXHRorResult, textStatus, jqXHRorError) {
                that._deinitProgressListener(options);
                that._onAlways(
                  jqXHRorResult,
                  textStatus,
                  jqXHRorError,
                  options
                );
                that._sending -= 1;
                that._active -= 1;
                if (
                  options.limitConcurrentUploads &&
                  options.limitConcurrentUploads > that._sending
                ) {
                  // Start the next queued upload,
                  // that has not been aborted:
                  var nextSlot = that._slots.shift();
                  while (nextSlot) {
                    if (that._getDeferredState(nextSlot) === 'pending') {
                      nextSlot.resolve();
                      break;
                    }
                    nextSlot = that._slots.shift();
                  }
                }
                if (that._active === 0) {
                  // The stop callback is triggered when all uploads have
                  // been completed, equivalent to the global ajaxStop event:
                  that._trigger('stop');
                }
              });
          return jqXHR;
        };
      this._beforeSend(e, options);
      if (
        this.options.sequentialUploads ||
        (this.options.limitConcurrentUploads &&
          this.options.limitConcurrentUploads <= this._sending)
      ) {
        if (this.options.limitConcurrentUploads > 1) {
          slot = $.Deferred();
          this._slots.push(slot);
          pipe = slot.then(send);
        } else {
          this._sequence = this._sequence.then(send, send);
          pipe = this._sequence;
        }
        // Return the piped Promise object, enhanced with an abort method,
        // which is delegated to the jqXHR object of the current upload,
        // and jqXHR callbacks mapped to the equivalent Promise methods:
        pipe.abort = function() {
          aborted = [undefined, 'abort', 'abort'];
          if (!jqXHR) {
            if (slot) {
              slot.rejectWith(options.context, aborted);
            }
            return send();
          }
          return jqXHR.abort();
        };
        return this._enhancePromise(pipe);
      }
      return send();
    },

    _onAdd: function(e, data) {
      var that = this,
        result = true,
        options = $.extend({}, this.options, data),
        files = data.files,
        filesLength = files.length,
        limit = options.limitMultiFileUploads,
        limitSize = options.limitMultiFileUploadSize,
        overhead = options.limitMultiFileUploadSizeOverhead,
        batchSize = 0,
        paramName = this._getParamName(options),
        paramNameSet,
        paramNameSlice,
        fileSet,
        i,
        j = 0;
      if (!filesLength) {
        return false;
      }
      if (limitSize && files[0].size === undefined) {
        limitSize = undefined;
      }
      if (
        !(options.singleFileUploads || limit || limitSize) ||
        !this._isXHRUpload(options)
      ) {
        fileSet = [files];
        paramNameSet = [paramName];
      } else if (!(options.singleFileUploads || limitSize) && limit) {
        fileSet = [];
        paramNameSet = [];
        for (i = 0; i < filesLength; i += limit) {
          fileSet.push(files.slice(i, i + limit));
          paramNameSlice = paramName.slice(i, i + limit);
          if (!paramNameSlice.length) {
            paramNameSlice = paramName;
          }
          paramNameSet.push(paramNameSlice);
        }
      } else if (!options.singleFileUploads && limitSize) {
        fileSet = [];
        paramNameSet = [];
        for (i = 0; i < filesLength; i = i + 1) {
          batchSize += files[i].size + overhead;
          if (
            i + 1 === filesLength ||
            batchSize + files[i + 1].size + overhead > limitSize ||
            (limit && i + 1 - j >= limit)
          ) {
            fileSet.push(files.slice(j, i + 1));
            paramNameSlice = paramName.slice(j, i + 1);
            if (!paramNameSlice.length) {
              paramNameSlice = paramName;
            }
            paramNameSet.push(paramNameSlice);
            j = i + 1;
            batchSize = 0;
          }
        }
      } else {
        paramNameSet = paramName;
      }
      data.originalFiles = files;
      $.each(fileSet || files, function(index, element) {
        var newData = $.extend({}, data);
        newData.files = fileSet ? element : [element];
        newData.paramName = paramNameSet[index];
        that._initResponseObject(newData);
        that._initProgressObject(newData);
        that._addConvenienceMethods(e, newData);
        result = that._trigger(
          'add',
          $.Event('add', { delegatedEvent: e }),
          newData
        );
        return result;
      });
      return result;
    },

    _replaceFileInput: function(data) {
      var input = data.fileInput,
        inputClone = input.clone(true),
        restoreFocus = input.is(document.activeElement);
      // Add a reference for the new cloned file input to the data argument:
      data.fileInputClone = inputClone;
      $('<form></form>')
        .append(inputClone)[0]
        .reset();
      // Detaching allows to insert the fileInput on another form
      // without loosing the file input value:
      input.after(inputClone).detach();
      // If the fileInput had focus before it was detached,
      // restore focus to the inputClone.
      if (restoreFocus) {
        inputClone.focus();
      }
      // Avoid memory leaks with the detached file input:
      $.cleanData(input.unbind('remove'));
      // Replace the original file input element in the fileInput
      // elements set with the clone, which has been copied including
      // event handlers:
      this.options.fileInput = this.options.fileInput.map(function(i, el) {
        if (el === input[0]) {
          return inputClone[0];
        }
        return el;
      });
      // If the widget has been initialized on the file input itself,
      // override this.element with the file input clone:
      if (input[0] === this.element[0]) {
        this.element = inputClone;
      }
    },

    _handleFileTreeEntry: function(entry, path) {
      var that = this,
        dfd = $.Deferred(),
        entries = [],
        dirReader,
        errorHandler = function(e) {
          if (e && !e.entry) {
            e.entry = entry;
          }
          // Since $.when returns immediately if one
          // Deferred is rejected, we use resolve instead.
          // This allows valid files and invalid items
          // to be returned together in one set:
          dfd.resolve([e]);
        },
        successHandler = function(entries) {
          that
            ._handleFileTreeEntries(entries, path + entry.name + '/')
            .done(function(files) {
              dfd.resolve(files);
            })
            .fail(errorHandler);
        },
        readEntries = function() {
          dirReader.readEntries(function(results) {
            if (!results.length) {
              successHandler(entries);
            } else {
              entries = entries.concat(results);
              readEntries();
            }
          }, errorHandler);
        };
      // eslint-disable-next-line no-param-reassign
      path = path || '';
      if (entry.isFile) {
        if (entry._file) {
          // Workaround for Chrome bug #149735
          entry._file.relativePath = path;
          dfd.resolve(entry._file);
        } else {
          entry.file(function(file) {
            file.relativePath = path;
            dfd.resolve(file);
          }, errorHandler);
        }
      } else if (entry.isDirectory) {
        dirReader = entry.createReader();
        readEntries();
      } else {
        // Return an empty list for file system items
        // other than files or directories:
        dfd.resolve([]);
      }
      return dfd.promise();
    },

    _handleFileTreeEntries: function(entries, path) {
      var that = this;
      return $.when
        .apply(
          $,
          $.map(entries, function(entry) {
            return that._handleFileTreeEntry(entry, path);
          })
        )
        .then(function() {
          return Array.prototype.concat.apply([], arguments);
        });
    },

    _getDroppedFiles: function(dataTransfer) {
      // eslint-disable-next-line no-param-reassign
      dataTransfer = dataTransfer || {};
      var items = dataTransfer.items;
      if (
        items &&
        items.length &&
        (items[0].webkitGetAsEntry || items[0].getAsEntry)
      ) {
        return this._handleFileTreeEntries(
          $.map(items, function(item) {
            var entry;
            if (item.webkitGetAsEntry) {
              entry = item.webkitGetAsEntry();
              if (entry) {
                // Workaround for Chrome bug #149735:
                entry._file = item.getAsFile();
              }
              return entry;
            }
            return item.getAsEntry();
          })
        );
      }
      return $.Deferred()
        .resolve($.makeArray(dataTransfer.files))
        .promise();
    },

    _getSingleFileInputFiles: function(fileInput) {
      // eslint-disable-next-line no-param-reassign
      fileInput = $(fileInput);
      var entries =
          fileInput.prop('webkitEntries') || fileInput.prop('entries'),
        files,
        value;
      if (entries && entries.length) {
        return this._handleFileTreeEntries(entries);
      }
      files = $.makeArray(fileInput.prop('files'));
      if (!files.length) {
        value = fileInput.prop('value');
        if (!value) {
          return $.Deferred()
            .resolve([])
            .promise();
        }
        // If the files property is not available, the browser does not
        // support the File API and we add a pseudo File object with
        // the input value as name with path information removed:
        files = [{ name: value.replace(/^.*\\/, '') }];
      } else if (files[0].name === undefined && files[0].fileName) {
        // File normalization for Safari 4 and Firefox 3:
        $.each(files, function(index, file) {
          file.name = file.fileName;
          file.size = file.fileSize;
        });
      }
      return $.Deferred()
        .resolve(files)
        .promise();
    },

    _getFileInputFiles: function(fileInput) {
      if (!(fileInput instanceof $) || fileInput.length === 1) {
        return this._getSingleFileInputFiles(fileInput);
      }
      return $.when
        .apply($, $.map(fileInput, this._getSingleFileInputFiles))
        .then(function() {
          return Array.prototype.concat.apply([], arguments);
        });
    },

    _onChange: function(e) {
      var that = this,
        data = {
          fileInput: $(e.target),
          form: $(e.target.form)
        };
      this._getFileInputFiles(data.fileInput).always(function(files) {
        data.files = files;
        if (that.options.replaceFileInput) {
          that._replaceFileInput(data);
        }
        if (
          that._trigger(
            'change',
            $.Event('change', { delegatedEvent: e }),
            data
          ) !== false
        ) {
          that._onAdd(e, data);
        }
      });
    },

    _onPaste: function(e) {
      var items =
          e.originalEvent &&
          e.originalEvent.clipboardData &&
          e.originalEvent.clipboardData.items,
        data = { files: [] };
      if (items && items.length) {
        $.each(items, function(index, item) {
          var file = item.getAsFile && item.getAsFile();
          if (file) {
            data.files.push(file);
          }
        });
        if (
          this._trigger(
            'paste',
            $.Event('paste', { delegatedEvent: e }),
            data
          ) !== false
        ) {
          this._onAdd(e, data);
        }
      }
    },

    _onDrop: function(e) {
      e.dataTransfer = e.originalEvent && e.originalEvent.dataTransfer;
      var that = this,
        dataTransfer = e.dataTransfer,
        data = {};
      if (dataTransfer && dataTransfer.files && dataTransfer.files.length) {
        e.preventDefault();
        this._getDroppedFiles(dataTransfer).always(function(files) {
          data.files = files;
          if (
            that._trigger(
              'drop',
              $.Event('drop', { delegatedEvent: e }),
              data
            ) !== false
          ) {
            that._onAdd(e, data);
          }
        });
      }
    },

    _onDragOver: getDragHandler('dragover'),

    _onDragEnter: getDragHandler('dragenter'),

    _onDragLeave: getDragHandler('dragleave'),

    _initEventHandlers: function() {
      if (this._isXHRUpload(this.options)) {
        this._on(this.options.dropZone, {
          dragover: this._onDragOver,
          drop: this._onDrop,
          // event.preventDefault() on dragenter is required for IE10+:
          dragenter: this._onDragEnter,
          // dragleave is not required, but added for completeness:
          dragleave: this._onDragLeave
        });
        this._on(this.options.pasteZone, {
          paste: this._onPaste
        });
      }
      if ($.support.fileInput) {
        this._on(this.options.fileInput, {
          change: this._onChange
        });
      }
    },

    _destroyEventHandlers: function() {
      this._off(this.options.dropZone, 'dragenter dragleave dragover drop');
      this._off(this.options.pasteZone, 'paste');
      this._off(this.options.fileInput, 'change');
    },

    _destroy: function() {
      this._destroyEventHandlers();
    },

    _setOption: function(key, value) {
      var reinit = $.inArray(key, this._specialOptions) !== -1;
      if (reinit) {
        this._destroyEventHandlers();
      }
      this._super(key, value);
      if (reinit) {
        this._initSpecialOptions();
        this._initEventHandlers();
      }
    },

    _initSpecialOptions: function() {
      var options = this.options;
      if (options.fileInput === undefined) {
        options.fileInput = this.element.is('input[type="file"]')
          ? this.element
          : this.element.find('input[type="file"]');
      } else if (!(options.fileInput instanceof $)) {
        options.fileInput = $(options.fileInput);
      }
      if (!(options.dropZone instanceof $)) {
        options.dropZone = $(options.dropZone);
      }
      if (!(options.pasteZone instanceof $)) {
        options.pasteZone = $(options.pasteZone);
      }
    },

    _getRegExp: function(str) {
      var parts = str.split('/'),
        modifiers = parts.pop();
      parts.shift();
      return new RegExp(parts.join('/'), modifiers);
    },

    _isRegExpOption: function(key, value) {
      return (
        key !== 'url' &&
        $.type(value) === 'string' &&
        /^\/.*\/[igm]{0,3}$/.test(value)
      );
    },

    _initDataAttributes: function() {
      var that = this,
        options = this.options,
        data = this.element.data();
      // Initialize options set via HTML5 data-attributes:
      $.each(this.element[0].attributes, function(index, attr) {
        var key = attr.name.toLowerCase(),
          value;
        if (/^data-/.test(key)) {
          // Convert hyphen-ated key to camelCase:
          key = key.slice(5).replace(/-[a-z]/g, function(str) {
            return str.charAt(1).toUpperCase();
          });
          value = data[key];
          if (that._isRegExpOption(key, value)) {
            value = that._getRegExp(value);
          }
          options[key] = value;
        }
      });
    },

    _create: function() {
      this._initDataAttributes();
      this._initSpecialOptions();
      this._slots = [];
      this._sequence = this._getXHRPromise(true);
      this._sending = this._active = 0;
      this._initProgressObject(this);
      this._initEventHandlers();
    },

    // This method is exposed to the widget API and allows to query
    // the number of active uploads:
    active: function() {
      return this._active;
    },

    // This method is exposed to the widget API and allows to query
    // the widget upload progress.
    // It returns an object with loaded, total and bitrate properties
    // for the running uploads:
    progress: function() {
      return this._progress;
    },

    // This method is exposed to the widget API and allows adding files
    // using the fileupload API. The data parameter accepts an object which
    // must have a files property and can contain additional options:
    // .yafFileUpload('add', {files: filesList});
    add: function(data) {
      var that = this;
      if (!data || this.options.disabled) {
        return;
      }
      if (data.fileInput && !data.files) {
        this._getFileInputFiles(data.fileInput).always(function(files) {
          data.files = files;
          that._onAdd(null, data);
        });
      } else {
        data.files = $.makeArray(data.files);
        this._onAdd(null, data);
      }
    },

    // This method is exposed to the widget API and allows sending files
    // using the fileupload API. The data parameter accepts an object which
    // must have a files or fileInput property and can contain additional options:
    // .yafFileUpload('send', {files: filesList});
    // The method returns a Promise object for the file upload call.
    send: function(data) {
      if (data && !this.options.disabled) {
        if (data.fileInput && !data.files) {
          var that = this,
            dfd = $.Deferred(),
            promise = dfd.promise(),
            jqXHR,
            aborted;
          promise.abort = function() {
            aborted = true;
            if (jqXHR) {
              return jqXHR.abort();
            }
            dfd.reject(null, 'abort', 'abort');
            return promise;
          };
          this._getFileInputFiles(data.fileInput).always(function(files) {
            if (aborted) {
              return;
            }
            if (!files.length) {
              dfd.reject();
              return;
            }
            data.files = files;
            jqXHR = that._onSend(null, data);
            jqXHR.then(
              function(result, textStatus, jqXHR) {
                dfd.resolve(result, textStatus, jqXHR);
              },
              function(jqXHR, textStatus, errorThrown) {
                dfd.reject(jqXHR, textStatus, errorThrown);
              }
            );
          });
          return this._enhancePromise(promise);
        }
        data.files = $.makeArray(data.files);
        if (data.files.length) {
          return this._onSend(null, data);
        }
      }
      return this._getXHRPromise(false, data && data.context);
    }
  });
});

/*
 * jQuery File Upload Processing Plugin
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2012, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * https://opensource.org/licenses/MIT
 */

/* global define, require */

(function(factory) {
  'use strict';
  if (typeof define === 'function' && define.amd) {
    // Register as an anonymous AMD module:
    define(['jquery', './jquery.fileupload'], factory);
  } else if (typeof exports === 'object') {
    // Node/CommonJS:
    factory(require('jquery'), require('./jquery.fileupload'));
  } else {
    // Browser globals:
    factory(window.jQuery);
  }
})(function($) {
  'use strict';

  var originalAdd = $.blueimp.yafFileUpload.prototype.options.add;

  // The File Upload Processing plugin extends the fileupload widget
  // with file processing functionality:
  $.widget('blueimp.yafFileUpload', $.blueimp.yafFileUpload, {
    options: {
      // The list of processing actions:
      processQueue: [
        /*
                {
                    action: 'log',
                    type: 'debug'
                }
                */
      ],
          add: function (e, data) {
        var $this = $(this);
        data.process(function() {
          return $this.yafFileUpload('process', data);
        });
        originalAdd.call(this, e, data);
      }
    },

    processActions: {
      /*
            log: function (data, options) {
                console[options.type](
                    'Processing "' + data.files[data.index].name + '"'
                );
            }
            */
    },

    _processFile: function(data, originalData) {
      var that = this,
        // eslint-disable-next-line new-cap
        dfd = $.Deferred().resolveWith(that, [data]),
        chain = dfd.promise();
      this._trigger('process', null, data);
      $.each(data.processQueue, function(i, settings) {
        var func = function(data) {
          if (originalData.errorThrown) {
            // eslint-disable-next-line new-cap
            return $.Deferred()
              .rejectWith(that, [originalData])
              .promise();
          }
          return that.processActions[settings.action].call(
            that,
            data,
            settings
          );
        };
        chain = chain.then(func, settings.always && func);
      });
      chain
        .done(function() {
          that._trigger('processdone', null, data);
          that._trigger('processalways', null, data);
        })
        .fail(function() {
          that._trigger('processfail', null, data);
          that._trigger('processalways', null, data);
        });
      return chain;
    },

    // Replaces the settings of each processQueue item that
    // are strings starting with an "@", using the remaining
    // substring as key for the option map,
    // e.g. "@autoUpload" is replaced with options.autoUpload:
    _transformProcessQueue: function(options) {
      var processQueue = [];
      $.each(options.processQueue, function() {
        var settings = {},
          action = this.action,
          prefix = this.prefix === true ? action : this.prefix;
        $.each(this, function(key, value) {
          if ($.type(value) === 'string' && value.charAt(0) === '@') {
            settings[key] =
              options[
                value.slice(1) ||
                  (prefix
                    ? prefix + key.charAt(0).toUpperCase() + key.slice(1)
                    : key)
              ];
          } else {
            settings[key] = value;
          }
        });
        processQueue.push(settings);
      });
      options.processQueue = processQueue;
    },

    // Returns the number of files currently in the processsing queue:
    processing: function() {
      return this._processing;
    },

    // Processes the files given as files property of the data parameter,
    // returns a Promise object that allows to bind callbacks:
    process: function(data) {
      var that = this,
        options = $.extend({}, this.options, data);
      if (options.processQueue && options.processQueue.length) {
        this._transformProcessQueue(options);
        if (this._processing === 0) {
          this._trigger('processstart');
        }
        $.each(data.files, function(index) {
          var opts = index ? $.extend({}, options) : options,
            func = function() {
              if (data.errorThrown) {
                // eslint-disable-next-line new-cap
                return $.Deferred()
                  .rejectWith(that, [data])
                  .promise();
              }
              return that._processFile(opts, data);
            };
          opts.index = index;
          that._processing += 1;
          that._processingQueue = that._processingQueue
            .then(func, func)
            .always(function() {
              that._processing -= 1;
              if (that._processing === 0) {
                that._trigger('processstop');
              }
            });
        });
      }
      return this._processingQueue;
    },

    _create: function() {
      this._super();
      this._processing = 0;
      // eslint-disable-next-line new-cap
      this._processingQueue = $.Deferred()
        .resolveWith(this)
        .promise();
    }
  });
});

/*
 * jQuery File Upload Image Preview & Resize Plugin
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2013, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * https://opensource.org/licenses/MIT
 */

/* global define, require */

(function(factory) {
  'use strict';
  if (typeof define === 'function' && define.amd) {
    // Register as an anonymous AMD module:
    define([
      'jquery',
      'load-image',
      'load-image-meta',
      'load-image-scale',
      'load-image-exif',
      'canvas-to-blob',
      './jquery.fileupload-process'
    ], factory);
  } else if (typeof exports === 'object') {
    // Node/CommonJS:
    factory(
      require('jquery'),
      require('blueimp-load-image/js/load-image'),
      require('blueimp-load-image/js/load-image-meta'),
      require('blueimp-load-image/js/load-image-scale'),
      require('blueimp-load-image/js/load-image-exif'),
      require('blueimp-canvas-to-blob'),
      require('./jquery.fileupload-process')
    );
  } else {
    // Browser globals:
    factory(window.jQuery, window.loadImage);
  }
})(function($, loadImage) {
  'use strict';

  // Prepend to the default processQueue:
  $.blueimp.yafFileUpload.prototype.options.processQueue.unshift(
    {
      action: 'loadImageMetaData',
      disableImageHead: '@',
      disableExif: '@',
      disableExifThumbnail: '@',
      disableExifSub: '@',
      disableExifGps: '@',
      disabled: '@disableImageMetaDataLoad'
    },
    {
      action: 'loadImage',
      // Use the action as prefix for the "@" options:
      prefix: true,
      fileTypes: '@',
      maxFileSize: '@',
      noRevoke: '@',
      disabled: '@disableImageLoad'
    },
    {
      action: 'resizeImage',
      // Use "image" as prefix for the "@" options:
      prefix: 'image',
      maxWidth: '@',
      maxHeight: '@',
      minWidth: '@',
      minHeight: '@',
      crop: '@',
      orientation: '@',
      forceResize: '@',
      disabled: '@disableImageResize'
    },
    {
      action: 'saveImage',
      quality: '@imageQuality',
      type: '@imageType',
      disabled: '@disableImageResize'
    },
    {
      action: 'saveImageMetaData',
      disabled: '@disableImageMetaDataSave'
    },
    {
      action: 'resizeImage',
      // Use "preview" as prefix for the "@" options:
      prefix: 'preview',
      maxWidth: '@',
      maxHeight: '@',
      minWidth: '@',
      minHeight: '@',
      crop: '@',
      orientation: '@',
      thumbnail: '@',
      canvas: '@',
      disabled: '@disableImagePreview'
    },
    {
      action: 'setImage',
      name: '@imagePreviewName',
      disabled: '@disableImagePreview'
    },
    {
      action: 'deleteImageReferences',
      disabled: '@disableImageReferencesDeletion'
    }
  );

  // The File Upload Resize plugin extends the fileupload widget
  // with image resize functionality:
  $.widget('blueimp.yafFileUpload', $.blueimp.yafFileUpload, {
    options: {
      // The regular expression for the types of images to load:
      // matched against the file type:
      loadImageFileTypes: /^image\/(gif|jpeg|png|svg\+xml)$/,
      // The maximum file size of images to load:
      loadImageMaxFileSize: 10000000, // 10MB
      // The maximum width of resized images:
      imageMaxWidth: 1920,
      // The maximum height of resized images:
      imageMaxHeight: 1080,
      // Defines the image orientation (1-8) or takes the orientation
      // value from Exif data if set to true:
      imageOrientation: false,
      // Define if resized images should be cropped or only scaled:
      imageCrop: false,
      // Disable the resize image functionality by default:
      disableImageResize: true,
      // The maximum width of the preview images:
      previewMaxWidth: 80,
      // The maximum height of the preview images:
      previewMaxHeight: 80,
      // Defines the preview orientation (1-8) or takes the orientation
      // value from Exif data if set to true:
      previewOrientation: true,
      // Create the preview using the Exif data thumbnail:
      previewThumbnail: true,
      // Define if preview images should be cropped or only scaled:
      previewCrop: false,
      // Define if preview images should be resized as canvas elements:
      previewCanvas: true
    },

    processActions: {
      // Loads the image given via data.files and data.index
      // as img element, if the browser supports the File API.
      // Accepts the options fileTypes (regular expression)
      // and maxFileSize (integer) to limit the files to load:
      loadImage: function(data, options) {
        if (options.disabled) {
          return data;
        }
        var that = this,
          file = data.files[data.index],
          // eslint-disable-next-line new-cap
          dfd = $.Deferred();
        if (
          ($.type(options.maxFileSize) === 'number' &&
            file.size > options.maxFileSize) ||
          (options.fileTypes && !options.fileTypes.test(file.type)) ||
          !loadImage(
            file,
            function(img) {
              if (img.src) {
                data.img = img;
              }
              dfd.resolveWith(that, [data]);
            },
            options
          )
        ) {
          return data;
        }
        return dfd.promise();
      },

      // Resizes the image given as data.canvas or data.img
      // and updates data.canvas or data.img with the resized image.
      // Also stores the resized image as preview property.
      // Accepts the options maxWidth, maxHeight, minWidth,
      // minHeight, canvas and crop:
      resizeImage: function(data, options) {
        if (options.disabled || !(data.canvas || data.img)) {
          return data;
        }
        // eslint-disable-next-line no-param-reassign
        options = $.extend({ canvas: true }, options);
        var that = this,
          // eslint-disable-next-line new-cap
          dfd = $.Deferred(),
          img = (options.canvas && data.canvas) || data.img,
          resolve = function(newImg) {
            if (
              newImg &&
              (newImg.width !== img.width ||
                newImg.height !== img.height ||
                options.forceResize)
            ) {
              data[newImg.getContext ? 'canvas' : 'img'] = newImg;
            }
            data.preview = newImg;
            dfd.resolveWith(that, [data]);
          },
          thumbnail;
        if (data.exif) {
          if (options.orientation === true) {
            options.orientation = data.exif.get('Orientation');
          }
          if (options.thumbnail) {
            thumbnail = data.exif.get('Thumbnail');
            if (thumbnail) {
              loadImage(thumbnail, resolve, options);
              return dfd.promise();
            }
          }
          // Prevent orienting the same image twice:
          if (data.orientation) {
            delete options.orientation;
          } else {
            data.orientation = options.orientation;
          }
        }
        if (img) {
          resolve(loadImage.scale(img, options));
          return dfd.promise();
        }
        return data;
      },

      // Saves the processed image given as data.canvas
      // inplace at data.index of data.files:
      saveImage: function(data, options) {
        if (!data.canvas || options.disabled) {
          return data;
        }
        var that = this,
          file = data.files[data.index],
          // eslint-disable-next-line new-cap
          dfd = $.Deferred();
        if (data.canvas.toBlob) {
          data.canvas.toBlob(
            function(blob) {
              if (!blob.name) {
                if (file.type === blob.type) {
                  blob.name = file.name;
                } else if (file.name) {
                  blob.name = file.name.replace(
                    /\.\w+$/,
                    '.' + blob.type.substr(6)
                  );
                }
              }
              // Don't restore invalid meta data:
              if (file.type !== blob.type) {
                delete data.imageHead;
              }
              // Store the created blob at the position
              // of the original file in the files list:
              data.files[data.index] = blob;
              dfd.resolveWith(that, [data]);
            },
            options.type || file.type,
            options.quality
          );
        } else {
          return data;
        }
        return dfd.promise();
      },

      loadImageMetaData: function(data, options) {
        if (options.disabled) {
          return data;
        }
        var that = this,
          // eslint-disable-next-line new-cap
          dfd = $.Deferred();
        loadImage.parseMetaData(
          data.files[data.index],
          function(result) {
            $.extend(data, result);
            dfd.resolveWith(that, [data]);
          },
          options
        );
        return dfd.promise();
      },

      saveImageMetaData: function(data, options) {
        if (
          !(
            data.imageHead &&
            data.canvas &&
            data.canvas.toBlob &&
            !options.disabled
          )
        ) {
          return data;
        }
        var file = data.files[data.index],
          blob = new Blob(
            [
              data.imageHead,
              // Resized images always have a head size of 20 bytes,
              // including the JPEG marker and a minimal JFIF header:
              this._blobSlice.call(file, 20)
            ],
            { type: file.type }
          );
        blob.name = file.name;
        data.files[data.index] = blob;
        return data;
      },

      // Sets the resized version of the image as a property of the
      // file object, must be called after "saveImage":
      setImage: function(data, options) {
        if (data.preview && !options.disabled) {
          data.files[data.index][options.name || 'preview'] = data.preview;
        }
        return data;
      },

      deleteImageReferences: function(data, options) {
        if (!options.disabled) {
          delete data.img;
          delete data.canvas;
          delete data.preview;
          delete data.imageHead;
        }
        return data;
      }
    }
  });
});

/*
 * jQuery File Upload Validation Plugin
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2013, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * https://opensource.org/licenses/MIT
 */

/* global define, require */

(function(factory) {
  'use strict';
  if (typeof define === 'function' && define.amd) {
    // Register as an anonymous AMD module:
    define(['jquery', './jquery.fileupload-process'], factory);
  } else if (typeof exports === 'object') {
    // Node/CommonJS:
    factory(require('jquery'), require('./jquery.fileupload-process'));
  } else {
    // Browser globals:
    factory(window.jQuery);
  }
})(function($) {
  'use strict';

  // Append to the default processQueue:
  $.blueimp.yafFileUpload.prototype.options.processQueue.push({
    action: 'validate',
    // Always trigger this action,
    // even if the previous action was rejected:
    always: true,
    // Options taken from the global options map:
    acceptFileTypes: '@',
    maxFileSize: '@',
    minFileSize: '@',
    maxNumberOfFiles: '@',
    disabled: '@disableValidation'
  });

  // The File Upload Validation plugin extends the fileupload widget
  // with file validation functionality:
  $.widget('blueimp.yafFileUpload', $.blueimp.yafFileUpload, {
    options: {
      /*
            // The regular expression for allowed file types, matches
            // against either file type or file name:
            acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
            // The maximum allowed file size in bytes:
            maxFileSize: 10000000, // 10 MB
            // The minimum allowed file size in bytes:
            minFileSize: undefined, // No minimal file size
            // The limit of files to be uploaded:
            maxNumberOfFiles: 10,
            */

      // Function returning the current number of files,
      // has to be overriden for maxNumberOfFiles validation:
      getNumberOfFiles: $.noop,

      // Error and info messages:
      messages: {
        maxNumberOfFiles: 'Maximum number of files exceeded',
        acceptFileTypes: 'File type not allowed',
        maxFileSize: 'File is too large',
        minFileSize: 'File is too small'
      }
    },

    processActions: {
      validate: function(data, options) {
        if (options.disabled) {
          return data;
        }
        // eslint-disable-next-line new-cap
        var dfd = $.Deferred(),
          settings = this.options,
          file = data.files[data.index],
          fileSize;
        if (options.minFileSize || options.maxFileSize) {
          fileSize = file.size;
        }
        if (
          $.type(options.maxNumberOfFiles) === 'number' &&
          (settings.getNumberOfFiles() || 0) + data.files.length >
            options.maxNumberOfFiles
        ) {
          file.error = settings.i18n('maxNumberOfFiles');
        } else if (
          options.acceptFileTypes &&
          !(
            options.acceptFileTypes.test(file.type) ||
            options.acceptFileTypes.test(file.name)
          )
        ) {
          file.error = settings.i18n('acceptFileTypes');
        } else if (fileSize > options.maxFileSize) {
          file.error = settings.i18n('maxFileSize');
        } else if (
          $.type(fileSize) === 'number' &&
          fileSize < options.minFileSize
        ) {
          file.error = settings.i18n('minFileSize');
        } else {
          delete file.error;
        }
        if (file.error || data.files.error) {
          data.files.error = true;
          dfd.rejectWith(this, [data]);
        } else {
          dfd.resolveWith(this, [data]);
        }
        return dfd.promise();
      }
    }
  });
});

/*
 * jQuery File Upload User Interface Plugin
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * https://opensource.org/licenses/MIT
 */

/* global define, require */

(function(factory) {
  'use strict';
  if (typeof define === 'function' && define.amd) {
    // Register as an anonymous AMD module:
    define([
      'jquery',
      'blueimp-tmpl',
      './jquery.fileupload-image',
      './jquery.fileupload-audio',
      './jquery.fileupload-video',
      './jquery.fileupload-validate'
    ], factory);
  } else if (typeof exports === 'object') {
    // Node/CommonJS:
    factory(
      require('jquery'),
      require('blueimp-tmpl'),
      require('./jquery.fileupload-image'),
      require('./jquery.fileupload-audio'),
      require('./jquery.fileupload-video'),
      require('./jquery.fileupload-validate')
    );
  } else {
    // Browser globals:
    factory(window.jQuery, window.tmpl);
  }
})(function($, tmpl) {
  'use strict';

  $.blueimp.yafFileUpload.prototype._specialOptions.push(
    'filesContainer',
    'uploadTemplateId',
    'downloadTemplateId'
  );

  // The UI version extends the file upload widget
  // and adds complete user interface interaction:
  $.widget('blueimp.yafFileUpload', $.blueimp.yafFileUpload, {
    options: {
      // By default, files added to the widget are uploaded as soon
      // as the user clicks on the start buttons. To enable automatic
      // uploads, set the following option to true:
      autoUpload: false,
      // The class to show/hide UI elements:
      showElementClass: 'in',
      // The ID of the upload template:
      uploadTemplateId: 'template-upload',
      // The ID of the download template:
      downloadTemplateId: 'template-download',
      // The container for the list of files. If undefined, it is set to
      // an element with class "files" inside of the widget element:
      filesContainer: undefined,
      // By default, files are appended to the files container.
      // Set the following option to true, to prepend files instead:
      prependFiles: false,
      // The expected data type of the upload response, sets the dataType
      // option of the $.ajax upload requests:
      dataType: 'json',

      // Error and info messages:
      messages: {
        unknownError: 'Unknown error'
      },

      // Function returning the current number of files,
      // used by the maxNumberOfFiles validation:
      getNumberOfFiles: function() {
        return this.filesContainer.children().not('.processing').length;
      },

      // Callback to retrieve the list of files from the server response:
      getFilesFromResponse: function(data) {
        if (data.result && $.isArray(data.result.files)) {
          return data.result.files;
        }
        return [];
      },

      // The add callback is invoked as soon as files are added to the fileupload
      // widget (via file input selection, drag & drop or add API call).
      // See the basic file upload widget for more information:
      add: function(e, data) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        var $this = $(this),
          that = $this.data('blueimp-yafFileUpload') || $this.data('fileupload'),
          options = that.options;
        data.context = that
          ._renderUpload(data.files)
          .data('data', data)
          .addClass('processing');
        options.filesContainer[options.prependFiles ? 'prepend' : 'append'](
          data.context
        );
        that._forceReflow(data.context);
        that._transition(data.context);
        data
          .process(function() {
            return $this.yafFileUpload('process', data);
          })
          .always(function() {
            data.context
              .each(function(index) {
                $(this)
                  .find('.size')
                  .text(that._formatFileSize(data.files[index].size));
              })
              .removeClass('processing');
            that._renderPreviews(data);
          })
          .done(function() {
            data.context.find('.edit,.start').prop('disabled', false);
            if (
              that._trigger('added', e, data) !== false &&
              (options.autoUpload || data.autoUpload) &&
              data.autoUpload !== false
            ) {
              data.submit();
            }
          })
          .fail(function() {
            if (data.files.error) {
              data.context.each(function(index) {
                var error = data.files[index].error;
                if (error) {
                  $(this)
                    .find('.error')
                    .text(error);
                }
              });
            }
          });
      },
      // Callback for the start of each file upload request:
      send: function(e, data) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        var that =
          $(this).data('blueimp-yafFileUpload') || $(this).data('fileupload');
        if (
          data.context &&
          data.dataType &&
          data.dataType.substr(0, 6) === 'iframe'
        ) {
          // Iframe Transport does not support progress events.
          // In lack of an indeterminate progress bar, we set
          // the progress to 100%, showing the full animated bar:
          data.context
            .find('.progress')
            .addClass(!$.support.transition && 'progress-animated')
            .attr('aria-valuenow', 100)
            .children()
            .first()
            .css('width', '100%');
        }
        return that._trigger('sent', e, data);
      },
      // Callback for successful uploads:
      done: function(e, data) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        var that =
            $(this).data('blueimp-yafFileUpload') || $(this).data('fileupload'),
          getFilesFromResponse =
            data.getFilesFromResponse || that.options.getFilesFromResponse,
          files = getFilesFromResponse(data),
          template,
          deferred;
        if (data.context) {
          data.context.each(function(index) {
            var file = files[index] || { error: 'Empty file upload result' };
            deferred = that._addFinishedDeferreds();
            that._transition($(this)).done(function() {
              var node = $(this);
              template = that._renderDownload([file]).replaceAll(node);
              that._forceReflow(template);
              that._transition(template).done(function() {
                data.context = $(this);
                that._trigger('completed', e, data);
                that._trigger('finished', e, data);
                deferred.resolve();
              });
            });
          });
        } else {
          template = that
            ._renderDownload(files)
            [that.options.prependFiles ? 'prependTo' : 'appendTo'](
              that.options.filesContainer
            );
          that._forceReflow(template);
          deferred = that._addFinishedDeferreds();
          that._transition(template).done(function() {
            data.context = $(this);
            that._trigger('completed', e, data);
            that._trigger('finished', e, data);
            deferred.resolve();
          });
        }
      },
      // Callback for failed (abort or error) uploads:
      fail: function(e, data) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        var that =
            $(this).data('blueimp-yafFileUpload') || $(this).data('fileupload'),
          template,
          deferred;
        if (data.context) {
          data.context.each(function(index) {
            if (data.errorThrown !== 'abort') {
              var file = data.files[index];
              file.error =
                file.error || data.errorThrown || data.i18n('unknownError');
              deferred = that._addFinishedDeferreds();
              that._transition($(this)).done(function() {
                var node = $(this);
                template = that._renderDownload([file]).replaceAll(node);
                that._forceReflow(template);
                that._transition(template).done(function() {
                  data.context = $(this);
                  that._trigger('failed', e, data);
                  that._trigger('finished', e, data);
                  deferred.resolve();
                });
              });
            } else {
              deferred = that._addFinishedDeferreds();
              that._transition($(this)).done(function() {
                $(this).remove();
                that._trigger('failed', e, data);
                that._trigger('finished', e, data);
                deferred.resolve();
              });
            }
          });
        } else if (data.errorThrown !== 'abort') {
          data.context = that
            ._renderUpload(data.files)
            [that.options.prependFiles ? 'prependTo' : 'appendTo'](
              that.options.filesContainer
            )
            .data('data', data);
          that._forceReflow(data.context);
          deferred = that._addFinishedDeferreds();
          that._transition(data.context).done(function() {
            data.context = $(this);
            that._trigger('failed', e, data);
            that._trigger('finished', e, data);
            deferred.resolve();
          });
        } else {
          that._trigger('failed', e, data);
          that._trigger('finished', e, data);
          that._addFinishedDeferreds().resolve();
        }
      },
      // Callback for upload progress events:
      progress: function(e, data) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        var progress = Math.floor((data.loaded / data.total) * 100);
        if (data.context) {
          data.context.each(function() {
            $(this)
              .find('.progress')
              .attr('aria-valuenow', progress)
              .children()
              .first()
              .css('width', progress + '%');
          });
        }
      },
      // Callback for global upload progress events:
      progressall: function(e, data) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        var $this = $(this),
          progress = Math.floor((data.loaded / data.total) * 100),
          globalProgressNode = $this.find('.fileupload-progress'),
          extendedProgressNode = globalProgressNode.find('.progress-extended');
        if (extendedProgressNode.length) {
          extendedProgressNode.html(
            (
              $this.data('blueimp-yafFileUpload') || $this.data('fileupload')
            )._renderExtendedProgress(data)
          );
        }
        globalProgressNode
          .find('.progress')
          .attr('aria-valuenow', progress)
          .children()
          .first()
          .css('width', progress + '%');
      },
      // Callback for uploads start, equivalent to the global ajaxStart event:
      start: function(e) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        var that =
          $(this).data('blueimp-yafFileUpload') || $(this).data('fileupload');
        that._resetFinishedDeferreds();
        that._transition($(this).find('.fileupload-progress')).done(function() {
          that._trigger('started', e);
        });
      },
      // Callback for uploads stop, equivalent to the global ajaxStop event:
      stop: function(e) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        var that =
            $(this).data('blueimp-yafFileUpload') || $(this).data('fileupload'),
          deferred = that._addFinishedDeferreds();
        $.when.apply($, that._getFinishedDeferreds()).done(function() {
          that._trigger('stopped', e);
        });
        that._transition($(this).find('.fileupload-progress')).done(function() {
          $(this)
            .find('.progress')
            .attr('aria-valuenow', '0')
            .children()
            .first()
            .css('width', '0%');
          $(this)
            .find('.progress-extended')
            .html('&nbsp;');
          deferred.resolve();
        });
      },
      processstart: function(e) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        $(this).addClass('fileupload-processing');
      },
      processstop: function(e) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        $(this).removeClass('fileupload-processing');
      },
      // Callback for file deletion:
      destroy: function(e, data) {
        if (e.isDefaultPrevented()) {
          return false;
        }
        var that =
            $(this).data('blueimp-yafFileUpload') || $(this).data('fileupload'),
          removeNode = function() {
            that._transition(data.context).done(function() {
              $(this).remove();
              that._trigger('destroyed', e, data);
            });
          };
        if (data.url) {
          data.dataType = data.dataType || that.options.dataType;
          $.ajax(data)
            .done(removeNode)
            .fail(function() {
              that._trigger('destroyfailed', e, data);
            });
        } else {
          removeNode();
        }
      }
    },

    _resetFinishedDeferreds: function() {
      this._finishedUploads = [];
    },

    _addFinishedDeferreds: function(deferred) {
      // eslint-disable-next-line new-cap
      var promise = deferred || $.Deferred();
      this._finishedUploads.push(promise);
      return promise;
    },

    _getFinishedDeferreds: function() {
      return this._finishedUploads;
    },

    // Link handler, that allows to download files
    // by drag & drop of the links to the desktop:
    _enableDragToDesktop: function() {
      var link = $(this),
        url = link.prop('href'),
        name = link.prop('download'),
        type = 'application/octet-stream';
      link.bind('dragstart', function(e) {
        try {
          e.originalEvent.dataTransfer.setData(
            'DownloadURL',
            [type, name, url].join(':')
          );
        } catch (ignore) {
          // Ignore exceptions
        }
      });
    },

    _formatFileSize: function(bytes) {
      if (typeof bytes !== 'number') {
        return '';
      }
      if (bytes >= 1000000000) {
        return (bytes / 1000000000).toFixed(2) + ' GB';
      }
      if (bytes >= 1000000) {
        return (bytes / 1000000).toFixed(2) + ' MB';
      }
      return (bytes / 1000).toFixed(2) + ' KB';
    },

    _formatBitrate: function(bits) {
      if (typeof bits !== 'number') {
        return '';
      }
      if (bits >= 1000000000) {
        return (bits / 1000000000).toFixed(2) + ' Gbit/s';
      }
      if (bits >= 1000000) {
        return (bits / 1000000).toFixed(2) + ' Mbit/s';
      }
      if (bits >= 1000) {
        return (bits / 1000).toFixed(2) + ' kbit/s';
      }
      return bits.toFixed(2) + ' bit/s';
    },

    _formatTime: function(seconds) {
      var date = new Date(seconds * 1000),
        days = Math.floor(seconds / 86400);
      days = days ? days + 'd ' : '';
      return (
        days +
        ('0' + date.getUTCHours()).slice(-2) +
        ':' +
        ('0' + date.getUTCMinutes()).slice(-2) +
        ':' +
        ('0' + date.getUTCSeconds()).slice(-2)
      );
    },

    _formatPercentage: function(floatValue) {
      return (floatValue * 100).toFixed(2) + ' %';
    },

    _renderExtendedProgress: function(data) {
      return (
        this._formatBitrate(data.bitrate) +
        ' | ' +
        this._formatTime(((data.total - data.loaded) * 8) / data.bitrate) +
        ' | ' +
        this._formatPercentage(data.loaded / data.total) +
        ' | ' +
        this._formatFileSize(data.loaded) +
        ' / ' +
        this._formatFileSize(data.total)
      );
    },

    _renderTemplate: function(func, files) {
      if (!func) {
        return $();
      }
      var result = func({
        files: files,
        formatFileSize: this._formatFileSize,
        options: this.options
      });
      if (result instanceof $) {
        return result;
      }
      return $(this.options.templatesContainer)
        .html(result)
        .children();
    },

    _renderPreviews: function(data) {
      data.context.find('.preview').each(function(index, elm) {
        $(elm)
          .empty()
          .append(data.files[index].preview);
      });
    },

    _renderUpload: function(files) {
      return this._renderTemplate(this.options.uploadTemplate, files);
    },

    _renderDownload: function(files) {
      return this._renderTemplate(this.options.downloadTemplate, files)
        .find('a[download]')
        .each(this._enableDragToDesktop)
        .end();
    },

    _editHandler: function(e) {
      e.preventDefault();
      if (!this.options.edit) return;
      var that = this,
        button = $(e.currentTarget),
        template = button.closest('.template-upload'),
        data = template.data('data'),
        index = button.data().index;
      this.options.edit(data.files[index]).then(function(file) {
        if (!file) return;
        data.files[index] = file;
        data.context.addClass('processing');
        template.find('.edit,.start').prop('disabled', true);
        $(that.element)
          .yafFileUpload('process', data)
          .always(function() {
            template
              .find('.size')
              .text(that._formatFileSize(data.files[index].size));
            data.context.removeClass('processing');
            that._renderPreviews(data);
          })
          .done(function() {
            template.find('.edit,.start').prop('disabled', false);
          })
          .fail(function() {
            template.find('.edit').prop('disabled', false);
            var error = data.files[index].error;
            if (error) {
              template.find('.error').text(error);
            }
          });
      });
    },

    _startHandler: function(e) {
      e.preventDefault();
      var button = $(e.currentTarget),
        template = button.closest('.template-upload'),
        data = template.data('data');
      button.prop('disabled', true);
      if (data && data.submit) {
        data.submit();
      }
    },

    _cancelHandler: function(e) {
      e.preventDefault();
      var template = $(e.currentTarget).closest(
          '.template-upload,.template-download'
        ),
        data = template.data('data') || {};
      data.context = data.context || template;
      if (data.abort) {
        data.abort();
      } else {
        data.errorThrown = 'abort';
        this._trigger('fail', e, data);
      }
    },

    _deleteHandler: function(e) {
      e.preventDefault();
      var button = $(e.currentTarget);
      this._trigger(
        'destroy',
        e,
        $.extend(
          {
            context: button.closest('.template-download'),
            type: 'DELETE'
          },
          button.data()
        )
      );
    },

    _forceReflow: function(node) {
      return $.support.transition && node.length && node[0].offsetWidth;
    },

    _transition: function(node) {
      // eslint-disable-next-line new-cap
      var dfd = $.Deferred();
      if (
        $.support.transition &&
        node.hasClass('fade') &&
        node.is(':visible')
      ) {
        var transitionEndHandler = function(e) {
          // Make sure we don't respond to other transition events
          // in the container element, e.g. from button elements:
          if (e.target === node[0]) {
            node.unbind($.support.transition.end, transitionEndHandler);
            dfd.resolveWith(node);
          }
        };
        node
          .bind($.support.transition.end, transitionEndHandler)
          .toggleClass(this.options.showElementClass);
      } else {
        node.toggleClass(this.options.showElementClass);
        dfd.resolveWith(node);
      }
      return dfd;
    },

    _initButtonBarEventHandlers: function() {
      var fileUploadButtonBar = this.element.find('.fileupload-buttonbar'),
        filesList = this.options.filesContainer;
      this._on(fileUploadButtonBar.find('.start'), {
        click: function(e) {
          e.preventDefault();
          filesList.find('.start').click();
        }
      });
      this._on(fileUploadButtonBar.find('.cancel'), {
        click: function(e) {
          e.preventDefault();
          filesList.find('.cancel').click();
        }
      });
      this._on(fileUploadButtonBar.find('.delete'), {
        click: function(e) {
          e.preventDefault();
          filesList
            .find('.toggle:checked')
            .closest('.template-download')
            .find('.delete')
            .click();
          fileUploadButtonBar.find('.toggle').prop('checked', false);
        }
      });
      this._on(fileUploadButtonBar.find('.toggle'), {
        change: function(e) {
          filesList
            .find('.toggle')
            .prop('checked', $(e.currentTarget).is(':checked'));
        }
      });
    },

    _destroyButtonBarEventHandlers: function() {
      this._off(
        this.element
          .find('.fileupload-buttonbar')
          .find('.start, .cancel, .delete'),
        'click'
      );
      this._off(this.element.find('.fileupload-buttonbar .toggle'), 'change.');
    },

    _initEventHandlers: function() {
      this._super();
      this._on(this.options.filesContainer, {
        'click .edit': this._editHandler,
        'click .start': this._startHandler,
        'click .cancel': this._cancelHandler,
        'click .delete': this._deleteHandler
      });
      this._initButtonBarEventHandlers();
    },

    _destroyEventHandlers: function() {
      this._destroyButtonBarEventHandlers();
      this._off(this.options.filesContainer, 'click');
      this._super();
    },

    _enableFileInputButton: function() {
      this.element
        .find('.fileinput-button input')
        .prop('disabled', false)
        .parent()
        .removeClass('disabled');
    },

    _disableFileInputButton: function() {
      this.element
        .find('.fileinput-button input')
        .prop('disabled', true)
        .parent()
        .addClass('disabled');
    },

    _initTemplates: function() {
      var options = this.options;
      options.templatesContainer = this.document[0].createElement(
        options.filesContainer.prop('nodeName')
      );
      if (tmpl) {
        if (options.uploadTemplateId) {
          options.uploadTemplate = tmpl(options.uploadTemplateId);
        }
        if (options.downloadTemplateId) {
          options.downloadTemplate = tmpl(options.downloadTemplateId);
        }
      }
    },

    _initFilesContainer: function() {
      var options = this.options;
      if (options.filesContainer === undefined) {
        options.filesContainer = this.element.find('.files');
      } else if (!(options.filesContainer instanceof $)) {
        options.filesContainer = $(options.filesContainer);
      }
    },

    _initSpecialOptions: function() {
      this._super();
      this._initFilesContainer();
      this._initTemplates();
    },

    _create: function() {
      this._super();
      this._resetFinishedDeferreds();
      if (!$.support.fileInput) {
        this._disableFileInputButton();
      }
    },

    enable: function() {
      var wasDisabled = false;
      if (this.options.disabled) {
        wasDisabled = true;
      }
      this._super();
      if (wasDisabled) {
        this.element.find('input, button').prop('disabled', false);
        this._enableFileInputButton();
      }
    },

    disable: function() {
      if (!this.options.disabled) {
        this.element.find('input, button').prop('disabled', true);
        this._disableFileInputButton();
      }
      this._super();
    }
  });
});