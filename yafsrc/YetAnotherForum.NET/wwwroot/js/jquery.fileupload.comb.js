(function(factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        define([ "jquery" ], factory);
    } else if (typeof exports === "object") {
        factory(require("jquery"));
    } else {
        factory(window.jQuery);
    }
})(function($) {
;    "use strict";
    $.ui = $.ui || {};
    $.ui.version = "1.12.1";
    if (!$.expr.pseudos) {
        $.expr.pseudos = $.expr[":"];
    }
    if (!$.uniqueSort) {
        $.uniqueSort = $.unique;
    }
    var widgetUuid = 0;
    var widgetHasOwnProperty = Array.prototype.hasOwnProperty;
    var widgetSlice = Array.prototype.slice;
    $.cleanData = function(orig) {
        return function(elems) {
            var events, elem, i;
            for (i = 0; (elem = elems[i]) != null; i++) {
                events = $._data(elem, "events");
                if (events && events.remove) {
                    $(elem).triggerHandler("remove");
                }
            }
            orig(elems);
        };
    }($.cleanData);
    $.widget = function(name, base, prototype) {
        var existingConstructor, constructor, basePrototype;
        var proxiedPrototype = {};
        var namespace = name.split(".")[0];
        name = name.split(".")[1];
        var fullName = namespace + "-" + name;
        if (!prototype) {
            prototype = base;
            base = $.Widget;
        }
        if ($.isArray(prototype)) {
            prototype = $.extend.apply(null, [ {} ].concat(prototype));
        }
        $.expr.pseudos[fullName.toLowerCase()] = function(elem) {
            return !!$.data(elem, fullName);
        };
        $[namespace] = $[namespace] || {};
        existingConstructor = $[namespace][name];
        constructor = $[namespace][name] = function(options, element) {
            if (!this._createWidget) {
                return new constructor(options, element);
            }
            if (arguments.length) {
                this._createWidget(options, element);
            }
        };
        $.extend(constructor, existingConstructor, {
            version: prototype.version,
            _proto: $.extend({}, prototype),
            _childConstructors: []
        });
        basePrototype = new base();
        basePrototype.options = $.widget.extend({}, basePrototype.options);
        $.each(prototype, function(prop, value) {
            if (!$.isFunction(value)) {
                proxiedPrototype[prop] = value;
                return;
            }
            proxiedPrototype[prop] = function() {
                function _super() {
                    return base.prototype[prop].apply(this, arguments);
                }
                function _superApply(args) {
                    return base.prototype[prop].apply(this, args);
                }
                return function() {
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
            }();
        });
        constructor.prototype = $.widget.extend(basePrototype, {
            widgetEventPrefix: existingConstructor ? basePrototype.widgetEventPrefix || name : name
        }, proxiedPrototype, {
            constructor: constructor,
            namespace: namespace,
            widgetName: name,
            widgetFullName: fullName
        });
        if (existingConstructor) {
            $.each(existingConstructor._childConstructors, function(i, child) {
                var childPrototype = child.prototype;
                $.widget(childPrototype.namespace + "." + childPrototype.widgetName, constructor, child._proto);
            });
            delete existingConstructor._childConstructors;
        } else {
            base._childConstructors.push(constructor);
        }
        $.widget.bridge(name, constructor);
        return constructor;
    };
    $.widget.extend = function(target) {
        var input = widgetSlice.call(arguments, 1);
        var inputIndex = 0;
        var inputLength = input.length;
        var key;
        var value;
        for (;inputIndex < inputLength; inputIndex++) {
            for (key in input[inputIndex]) {
                value = input[inputIndex][key];
                if (widgetHasOwnProperty.call(input[inputIndex], key) && value !== undefined) {
                    if ($.isPlainObject(value)) {
                        target[key] = $.isPlainObject(target[key]) ? $.widget.extend({}, target[key], value) : $.widget.extend({}, value);
                    } else {
                        target[key] = value;
                    }
                }
            }
        }
        return target;
    };
    $.widget.bridge = function(name, object) {
        var fullName = object.prototype.widgetFullName || name;
        $.fn[name] = function(options) {
            var isMethodCall = typeof options === "string";
            var args = widgetSlice.call(arguments, 1);
            var returnValue = this;
            if (isMethodCall) {
                if (!this.length && options === "instance") {
                    returnValue = undefined;
                } else {
                    this.each(function() {
                        var methodValue;
                        var instance = $.data(this, fullName);
                        if (options === "instance") {
                            returnValue = instance;
                            return false;
                        }
                        if (!instance) {
                            return $.error("cannot call methods on " + name + " prior to initialization; " + "attempted to call method '" + options + "'");
                        }
                        if (!$.isFunction(instance[options]) || options.charAt(0) === "_") {
                            return $.error("no such method '" + options + "' for " + name + " widget instance");
                        }
                        methodValue = instance[options].apply(instance, args);
                        if (methodValue !== instance && methodValue !== undefined) {
                            returnValue = methodValue && methodValue.jquery ? returnValue.pushStack(methodValue.get()) : methodValue;
                            return false;
                        }
                    });
                }
            } else {
                if (args.length) {
                    options = $.widget.extend.apply(null, [ options ].concat(args));
                }
                this.each(function() {
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
    $.Widget = function() {};
    $.Widget._childConstructors = [];
    $.Widget.prototype = {
        widgetName: "widget",
        widgetEventPrefix: "",
        defaultElement: "<div>",
        options: {
            classes: {},
            disabled: false,
            create: null
        },
        _createWidget: function(options, element) {
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
                    remove: function(event) {
                        if (event.target === element) {
                            this.destroy();
                        }
                    }
                });
                this.document = $(element.style ? element.ownerDocument : element.document || element);
                this.window = $(this.document[0].defaultView || this.document[0].parentWindow);
            }
            this.options = $.widget.extend({}, this.options, this._getCreateOptions(), options);
            this._create();
            if (this.options.disabled) {
                this._setOptionDisabled(this.options.disabled);
            }
            this._trigger("create", null, this._getCreateEventData());
            this._init();
        },
        _getCreateOptions: function() {
            return {};
        },
        _getCreateEventData: $.noop,
        _create: $.noop,
        _init: $.noop,
        destroy: function() {
            var that = this;
            this._destroy();
            $.each(this.classesElementLookup, function(key, value) {
                that._removeClass(value, key);
            });
            this.element.off(this.eventNamespace).removeData(this.widgetFullName);
            this.widget().off(this.eventNamespace).removeAttr("aria-disabled");
            this.bindings.off(this.eventNamespace);
        },
        _destroy: $.noop,
        widget: function() {
            return this.element;
        },
        option: function(key, value) {
            var options = key;
            var parts;
            var curOption;
            var i;
            if (arguments.length === 0) {
                return $.widget.extend({}, this.options);
            }
            if (typeof key === "string") {
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
        _setOptions: function(options) {
            var key;
            for (key in options) {
                this._setOption(key, options[key]);
            }
            return this;
        },
        _setOption: function(key, value) {
            if (key === "classes") {
                this._setOptionClasses(value);
            }
            this.options[key] = value;
            if (key === "disabled") {
                this._setOptionDisabled(value);
            }
            return this;
        },
        _setOptionClasses: function(value) {
            var classKey, elements, currentElements;
            for (classKey in value) {
                currentElements = this.classesElementLookup[classKey];
                if (value[classKey] === this.options.classes[classKey] || !currentElements || !currentElements.length) {
                    continue;
                }
                elements = $(currentElements.get());
                this._removeClass(currentElements, classKey);
                elements.addClass(this._classes({
                    element: elements,
                    keys: classKey,
                    classes: value,
                    add: true
                }));
            }
        },
        _setOptionDisabled: function(value) {
            this._toggleClass(this.widget(), this.widgetFullName + "-disabled", null, !!value);
            if (value) {
                this._removeClass(this.hoverable, null, "ui-state-hover");
                this._removeClass(this.focusable, null, "ui-state-focus");
            }
        },
        enable: function() {
            return this._setOptions({
                disabled: false
            });
        },
        disable: function() {
            return this._setOptions({
                disabled: true
            });
        },
        _classes: function(options) {
            var full = [];
            var that = this;
            options = $.extend({
                element: this.element,
                classes: this.options.classes || {}
            }, options);
            function bindRemoveEvent() {
                options.element.each(function(_, element) {
                    var isTracked = $.map(that.classesElementLookup, function(elements) {
                        return elements;
                    }).some(function(elements) {
                        return elements.is(element);
                    });
                    if (!isTracked) {
                        that._on($(element), {
                            remove: "_untrackClassesElement"
                        });
                    }
                });
            }
            function processClassString(classes, checkOption) {
                var current, i;
                for (i = 0; i < classes.length; i++) {
                    current = that.classesElementLookup[classes[i]] || $();
                    if (options.add) {
                        bindRemoveEvent();
                        current = $($.uniqueSort(current.get().concat(options.element.get())));
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
            if (options.keys) {
                processClassString(options.keys.match(/\S+/g) || [], true);
            }
            if (options.extra) {
                processClassString(options.extra.match(/\S+/g) || []);
            }
            return full.join(" ");
        },
        _untrackClassesElement: function(event) {
            var that = this;
            $.each(that.classesElementLookup, function(key, value) {
                if ($.inArray(event.target, value) !== -1) {
                    that.classesElementLookup[key] = $(value.not(event.target).get());
                }
            });
            this._off($(event.target));
        },
        _removeClass: function(element, keys, extra) {
            return this._toggleClass(element, keys, extra, false);
        },
        _addClass: function(element, keys, extra) {
            return this._toggleClass(element, keys, extra, true);
        },
        _toggleClass: function(element, keys, extra, add) {
            add = typeof add === "boolean" ? add : extra;
            var shift = typeof element === "string" || element === null, options = {
                extra: shift ? keys : extra,
                keys: shift ? element : keys,
                element: shift ? this.element : element,
                add: add
            };
            options.element.toggleClass(this._classes(options), add);
            return this;
        },
        _on: function(suppressDisabledCheck, element, handlers) {
            var delegateElement;
            var instance = this;
            if (typeof suppressDisabledCheck !== "boolean") {
                handlers = element;
                element = suppressDisabledCheck;
                suppressDisabledCheck = false;
            }
            if (!handlers) {
                handlers = element;
                element = this.element;
                delegateElement = this.widget();
            } else {
                element = delegateElement = $(element);
                this.bindings = this.bindings.add(element);
            }
            $.each(handlers, function(event, handler) {
                function handlerProxy() {
                    if (!suppressDisabledCheck && (instance.options.disabled === true || $(this).hasClass("ui-state-disabled"))) {
                        return;
                    }
                    return (typeof handler === "string" ? instance[handler] : handler).apply(instance, arguments);
                }
                if (typeof handler !== "string") {
                    handlerProxy.guid = handler.guid = handler.guid || handlerProxy.guid || $.guid++;
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
        _off: function(element, eventName) {
            eventName = (eventName || "").split(" ").join(this.eventNamespace + " ") + this.eventNamespace;
            element.off(eventName);
            this.bindings = $(this.bindings.not(element).get());
            this.focusable = $(this.focusable.not(element).get());
            this.hoverable = $(this.hoverable.not(element).get());
        },
        _delay: function(handler, delay) {
            var instance = this;
            function handlerProxy() {
                return (typeof handler === "string" ? instance[handler] : handler).apply(instance, arguments);
            }
            return setTimeout(handlerProxy, delay || 0);
        },
        _hoverable: function(element) {
            this.hoverable = this.hoverable.add(element);
            this._on(element, {
                mouseenter: function(event) {
                    this._addClass($(event.currentTarget), null, "ui-state-hover");
                },
                mouseleave: function(event) {
                    this._removeClass($(event.currentTarget), null, "ui-state-hover");
                }
            });
        },
        _focusable: function(element) {
            this.focusable = this.focusable.add(element);
            this._on(element, {
                focusin: function(event) {
                    this._addClass($(event.currentTarget), null, "ui-state-focus");
                },
                focusout: function(event) {
                    this._removeClass($(event.currentTarget), null, "ui-state-focus");
                }
            });
        },
        _trigger: function(type, event, data) {
            var prop, orig;
            var callback = this.options[type];
            data = data || {};
            event = $.Event(event);
            event.type = (type === this.widgetEventPrefix ? type : this.widgetEventPrefix + type).toLowerCase();
            event.target = this.element[0];
            orig = event.originalEvent;
            if (orig) {
                for (prop in orig) {
                    if (!(prop in event)) {
                        event[prop] = orig[prop];
                    }
                }
            }
            this.element.trigger(event, data);
            return !($.isFunction(callback) && callback.apply(this.element[0], [ event ].concat(data)) === false || event.isDefaultPrevented());
        }
    };
    $.each({
        show: "fadeIn",
        hide: "fadeOut"
    }, function(method, defaultEffect) {
        $.Widget.prototype["_" + method] = function(element, options, callback) {
            if (typeof options === "string") {
                options = {
                    effect: options
                };
            }
            var hasOptions;
            var effectName = !options ? method : options === true || typeof options === "number" ? defaultEffect : options.effect || defaultEffect;
            options = options || {};
            if (typeof options === "number") {
                options = {
                    duration: options
                };
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
                element.queue(function(next) {
                    $(this)[method]();
                    if (callback) {
                        callback.call(element[0]);
                    }
                    next();
                });
            }
        };
    });
});

(function($) {
    "use strict";
    var tmpl = function(str, data) {
        var f = !/[^\w\-.:]/.test(str) ? tmpl.cache[str] = tmpl.cache[str] || tmpl(tmpl.load(str)) : new Function(tmpl.arg + ",tmpl", "var _e=tmpl.encode" + tmpl.helper + ",_s='" + str.replace(tmpl.regexp, tmpl.func) + "';return _s;");
        return data ? f(data, tmpl) : function(data) {
            return f(data, tmpl);
        };
    };
    tmpl.cache = {};
    tmpl.load = function(id) {
        return document.getElementById(id).innerHTML;
    };
    tmpl.regexp = /([\s'\\])(?!(?:[^{]|\{(?!%))*%\})|(?:\{%(=|#)([\s\S]+?)%\})|(\{%)|(%\})/g;
    tmpl.func = function(s, p1, p2, p3, p4, p5) {
        if (p1) {
            return {
                "\n": "\\n",
                "\r": "\\r",
                "\t": "\\t",
                " ": " "
            }[p1] || "\\" + p1;
        }
        if (p2) {
            if (p2 === "=") {
                return "'+_e(" + p3 + ")+'";
            }
            return "'+(" + p3 + "==null?'':" + p3 + ")+'";
        }
        if (p4) {
            return "';";
        }
        if (p5) {
            return "_s+='";
        }
    };
    tmpl.encReg = /[<>&"'\x00]/g;
    tmpl.encMap = {
        "<": "&lt;",
        ">": "&gt;",
        "&": "&amp;",
        '"': "&quot;",
        "'": "&#39;"
    };
    tmpl.encode = function(s) {
        return (s == null ? "" : "" + s).replace(tmpl.encReg, function(c) {
            return tmpl.encMap[c] || "";
        });
    };
    tmpl.arg = "o";
    tmpl.helper = ",print=function(s,e){_s+=e?(s==null?'':s):_e(s);}" + ",include=function(s,d){_s+=tmpl(s,d);}";
    if (typeof define === "function" && define.amd) {
        define(function() {
            return tmpl;
        });
    } else if (typeof module === "object" && module.exports) {
        module.exports = tmpl;
    } else {
        $.tmpl = tmpl;
    }
})(this);

!!function(c) {
    "use strict";
    var t = c.URL || c.webkitURL;
    function f(e) {
        return !!t && t.createObjectURL(e);
    }
    function i(e) {
        return !!t && t.revokeObjectURL(e);
    }
    function u(e, t) {
        !e || "blob:" !== e.slice(0, 5) || t && t.noRevoke || i(e);
    }
    function d(e, t, i, a) {
        if (!c.FileReader) return !1;
        var n = new FileReader();
        n.onload = function() {
            t.call(n, this.result);
        }, i && (n.onabort = n.onerror = function() {
            i.call(n, this.error);
        });
        a = n[a || "readAsDataURL"];
        return a ? (a.call(n, e), n) : void 0;
    }
    function g(e, t) {
        return Object.prototype.toString.call(t) === "[object " + e + "]";
    }
    function m(s, e, l) {
        function t(i, a) {
            var n, r = document.createElement("img");
            function o(e, t) {
                i !== a ? e instanceof Error ? a(e) : ((t = t || {}).image = e, 
                i(t)) : i && i(e, t);
            }
            function e(e, t) {
                t && c.console && console.log(t), e && g("Blob", e) ? n = f(s = e) : (n = s, 
                l && l.crossOrigin && (r.crossOrigin = l.crossOrigin)), r.src = n;
            }
            return r.onerror = function(e) {
                u(n, l), a && a.call(r, e);
            }, r.onload = function() {
                u(n, l);
                var e = {
                    originalWidth: r.naturalWidth || r.width,
                    originalHeight: r.naturalHeight || r.height
                };
                try {
                    m.transform(r, l, o, s, e);
                } catch (t) {
                    a && a(t);
                }
            }, "string" == typeof s ? (m.requiresMetaData(l) ? m.fetchBlob(s, e, l) : e(), 
            r) : g("Blob", s) || g("File", s) ? (n = f(s)) ? (r.src = n, r) : d(s, function(e) {
                r.src = e;
            }, a) : void 0;
        }
        return c.Promise && "function" != typeof e ? (l = e, new Promise(t)) : t(e, e);
    }
    m.requiresMetaData = function(e) {
        return e && e.meta;
    }, m.fetchBlob = function(e, t) {
        t();
    }, m.transform = function(e, t, i, a, n) {
        i(e, n);
    }, m.global = c, m.readFile = d, m.isInstanceOf = g, m.createObjectURL = f, 
    m.revokeObjectURL = i, "function" == typeof define && define.amd ? define(function() {
        return m;
    }) : "object" == typeof module && module.exports ? module.exports = m : c.loadImage = m;
}("undefined" != typeof window && window || this), function(e) {
    "use strict";
    "function" == typeof define && define.amd ? define([ "./load-image" ], e) : "object" == typeof module && module.exports ? e(require("./load-image")) : e(window.loadImage);
}(function(E) {
    "use strict";
    var r = E.transform;
    E.createCanvas = function(e, t, i) {
        if (i && E.global.OffscreenCanvas) return new OffscreenCanvas(e, t);
        i = document.createElement("canvas");
        return i.width = e, i.height = t, i;
    }, E.transform = function(e, t, i, a, n) {
        r.call(E, E.scale(e, t, n), t, i, a, n);
    }, E.transformCoordinates = function() {}, E.getTransformedOptions = function(e, t) {
        var i, a, n, r = t.aspectRatio;
        if (!r) return t;
        for (a in i = {}, t) Object.prototype.hasOwnProperty.call(t, a) && (i[a] = t[a]);
        return i.crop = !0, r < (n = e.naturalWidth || e.width) / (e = e.naturalHeight || e.height) ? (i.maxWidth = e * r, 
        i.maxHeight = e) : (i.maxWidth = n, i.maxHeight = n / r), i;
    }, E.drawImage = function(e, t, i, a, n, r, o, s, l) {
        t = t.getContext("2d");
        return !1 === l.imageSmoothingEnabled ? (t.msImageSmoothingEnabled = !1, 
        t.imageSmoothingEnabled = !1) : l.imageSmoothingQuality && (t.imageSmoothingQuality = l.imageSmoothingQuality), 
        t.drawImage(e, i, a, n, r, 0, 0, o, s), t;
    }, E.requiresCanvas = function(e) {
        return e.canvas || e.crop || !!e.aspectRatio;
    }, E.scale = function(e, t, i) {
        t = t || {}, i = i || {};
        var a, n, r, o, s, l, c, f, u, d, g, m = e.getContext || E.requiresCanvas(t) && !!E.global.HTMLCanvasElement, h = e.naturalWidth || e.width, p = e.naturalHeight || e.height, A = h, b = p;
        function y() {
            var e = Math.max((r || A) / A, (o || b) / b);
            1 < e && (A *= e, b *= e);
        }
        function S() {
            var e = Math.min((a || A) / A, (n || b) / b);
            e < 1 && (A *= e, b *= e);
        }
        if (m && (c = (t = E.getTransformedOptions(e, t, i)).left || 0, f = t.top || 0, 
        t.sourceWidth ? (s = t.sourceWidth, t.right !== undefined && t.left === undefined && (c = h - s - t.right)) : s = h - c - (t.right || 0), 
        t.sourceHeight ? (l = t.sourceHeight, t.bottom !== undefined && t.top === undefined && (f = p - l - t.bottom)) : l = p - f - (t.bottom || 0), 
        A = s, b = l), a = t.maxWidth, n = t.maxHeight, r = t.minWidth, o = t.minHeight, 
        m && a && n && t.crop ? (d = s / l - (A = a) / (b = n)) < 0 ? (l = n * s / a, 
        t.top === undefined && t.bottom === undefined && (f = (p - l) / 2)) : 0 < d && (s = a * l / n, 
        t.left === undefined && t.right === undefined && (c = (h - s) / 2)) : ((t.contain || t.cover) && (r = a = a || r, 
        o = n = n || o), t.cover ? (S(), y()) : (y(), S())), m) {
            if (1 < (m = t.pixelRatio) && (!e.style.width || Math.floor(parseFloat(e.style.width, 10)) !== Math.floor(h / m)) && (A *= m, 
            b *= m), E.orientationCropBug && !e.getContext && (c || f || s !== h || l !== p) && (d = e, 
            e = E.createCanvas(h, p, !0), E.drawImage(d, e, 0, 0, h, p, h, p, t)), 
            0 < (u = t.downsamplingRatio) && u < 1 && A < s && b < l) for (;A < s * u; ) g = E.createCanvas(s * u, l * u, !0), 
            E.drawImage(e, g, c, f, s, l, g.width, g.height, t), f = c = 0, s = g.width, 
            l = g.height, e = g;
            return g = E.createCanvas(A, b), E.transformCoordinates(g, t, i), 1 < m && (g.style.width = g.width / m + "px"), 
            E.drawImage(e, g, c, f, s, l, A, b, t).setTransform(1, 0, 0, 1, 0, 0), 
            g;
        }
        return e.width = A, e.height = b, e;
    };
}), function(e) {
    "use strict";
    "function" == typeof define && define.amd ? define([ "./load-image" ], e) : "object" == typeof module && module.exports ? e(require("./load-image")) : e(window.loadImage);
}(function(o) {
    "use strict";
    var s = o.global, l = o.transform, a = s.Blob && (Blob.prototype.slice || Blob.prototype.webkitSlice || Blob.prototype.mozSlice), m = s.ArrayBuffer && ArrayBuffer.prototype.slice || function(e, t) {
        t = t || this.byteLength - e;
        e = new Uint8Array(this, e, t), t = new Uint8Array(t);
        return t.set(e), t.buffer;
    }, h = {
        jpeg: {
            65505: [],
            65517: []
        }
    };
    function c(t, e, u, d) {
        var g = this;
        function i(c, f) {
            if (!(s.DataView && a && t && 12 <= t.size && "image/jpeg" === t.type)) return c(d);
            var e = u.maxMetaDataSize || 262144;
            o.readFile(a.call(t, 0, e), function(e) {
                var t = new DataView(e);
                if (65496 !== t.getUint16(0)) return f(new Error("Invalid JPEG file: Missing JPEG marker."));
                for (var i, a, n, r, o = 2, s = t.byteLength - 4, l = o; o < s && (65504 <= (i = t.getUint16(o)) && i <= 65519 || 65534 === i); ) {
                    if (o + (a = t.getUint16(o + 2) + 2) > t.byteLength) {
                        console.log("Invalid JPEG metadata: Invalid segment size.");
                        break;
                    }
                    if ((n = h.jpeg[i]) && !u.disableMetaDataParsers) for (r = 0; r < n.length; r += 1) n[r].call(g, t, o, a, d, u);
                    l = o += a;
                }
                !u.disableImageHead && 6 < l && (d.imageHead = m.call(e, 0, l)), 
                c(d);
            }, f, "readAsArrayBuffer") || c(d);
        }
        return u = u || {}, s.Promise && "function" != typeof e ? (d = u = e || {}, 
        new Promise(i)) : (d = d || {}, i(e, e));
    }
    function n(e, t, i) {
        return e && t && i ? new Blob([ i, a.call(e, t.byteLength) ], {
            type: "image/jpeg"
        }) : null;
    }
    o.transform = function(t, i, a, n, r) {
        o.requiresMetaData(i) ? c(n, function(e) {
            e !== r && (s.console && console.log(e), e = r), l.call(o, t, i, a, n, e);
        }, i, r = r || {}) : l.apply(o, arguments);
    }, o.blobSlice = a, o.bufferSlice = m, o.replaceHead = function(t, i, a) {
        var e = {
            maxMetaDataSize: 256,
            disableMetaDataParsers: !0
        };
        if (!a && s.Promise) return c(t, e).then(function(e) {
            return n(t, e.imageHead, i);
        });
        c(t, function(e) {
            a(n(t, e.imageHead, i));
        }, e);
    }, o.parseMetaData = c, o.metaDataParsers = h;
}), function(e) {
    "use strict";
    "function" == typeof define && define.amd ? define([ "./load-image" ], e) : "object" == typeof module && module.exports ? e(require("./load-image")) : e(window.loadImage);
}(function(e) {
    "use strict";
    var r = e.global;
    r.fetch && r.Request && r.Response && r.Response.prototype.blob ? e.fetchBlob = function(e, t, i) {
        function a(e) {
            return e.blob();
        }
        if (r.Promise && "function" != typeof t) return fetch(new Request(e, t)).then(a);
        fetch(new Request(e, i)).then(a).then(t)["catch"](function(e) {
            t(null, e);
        });
    } : r.XMLHttpRequest && "" === new XMLHttpRequest().responseType && (e.fetchBlob = function(e, t, n) {
        function i(t, i) {
            n = n || {};
            var a = new XMLHttpRequest();
            a.open(n.method || "GET", e), n.headers && Object.keys(n.headers).forEach(function(e) {
                a.setRequestHeader(e, n.headers[e]);
            }), a.withCredentials = "include" === n.credentials, a.responseType = "blob", 
            a.onload = function() {
                t(a.response);
            }, a.onerror = a.onabort = a.ontimeout = function(e) {
                t === i ? i(null, e) : i(e);
            }, a.send(n.body);
        }
        return r.Promise && "function" != typeof t ? (n = t, new Promise(i)) : i(t, t);
    });
}), function(e) {
    "use strict";
    "function" == typeof define && define.amd ? define([ "./load-image", "./load-image-scale", "./load-image-meta" ], e) : "object" == typeof module && module.exports ? e(require("./load-image"), require("./load-image-scale"), require("./load-image-meta")) : e(window.loadImage);
}(function(f) {
    "use strict";
    var t, i, r = f.transform, a = f.requiresCanvas, n = f.requiresMetaData, l = f.transformCoordinates, u = f.getTransformedOptions;
    function o(e, t) {
        e = e && e.orientation;
        return !0 === e && !f.orientation || 1 === e && f.orientation || (!t || f.orientation) && 1 < e && e < 9;
    }
    function d(e, t) {
        return e !== t && (1 === e && 1 < t && t < 9 || 1 < e && e < 9);
    }
    function g(e, t) {
        if (1 < t && t < 9) switch (e) {
          case 2:
          case 4:
            return 4 < t;

          case 5:
          case 7:
            return t % 2 == 0;

          case 6:
          case 8:
            return 2 === t || 4 === t || 5 === t || 7 === t;
        }
    }
    (t = f).global.document && ((i = document.createElement("img")).onload = function() {
        var e;
        t.orientation = 2 === i.width && 3 === i.height, t.orientation && ((e = t.createCanvas(1, 1, !0).getContext("2d")).drawImage(i, 1, 1, 1, 1, 0, 0, 1, 1), 
        t.orientationCropBug = "255,255,255,255" !== e.getImageData(0, 0, 1, 1).data.toString());
    }, i.src = "data:image/jpeg;base64,/9j/4QAiRXhpZgAATU0AKgAAAAgAAQESAAMAAAABAAYAAAAAAAD/2wCEAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAf/AABEIAAIAAwMBEQACEQEDEQH/xABRAAEAAAAAAAAAAAAAAAAAAAAKEAEBAQADAQEAAAAAAAAAAAAGBQQDCAkCBwEBAAAAAAAAAAAAAAAAAAAAABEBAAAAAAAAAAAAAAAAAAAAAP/aAAwDAQACEQMRAD8AG8T9NfSMEVMhQvoP3fFiRZ+MTHDifa/95OFSZU5OzRzxkyejv8ciEfhSceSXGjS8eSdLnZc2HDm4M3BxcXwH/9k="), 
    f.requiresCanvas = function(e) {
        return o(e) || a.call(f, e);
    }, f.requiresMetaData = function(e) {
        return o(e, !0) || n.call(f, e);
    }, f.transform = function(e, t, n, i, a) {
        r.call(f, e, t, function(e, t) {
            var i, a;
            !t || 4 < (a = f.orientation && t.exif && t.exif.get("Orientation")) && a < 9 && (i = t.originalWidth, 
            a = t.originalHeight, t.originalWidth = a, t.originalHeight = i), n(e, t);
        }, i, a);
    }, f.getTransformedOptions = function(e, t, i) {
        var a = u.call(f, e, t), e = i.exif && i.exif.get("Orientation"), t = a.orientation, i = f.orientation && e;
        if (!d(t = !0 === t ? e : t, i)) return a;
        var n, r = a.top, o = a.right, s = a.bottom, l = a.left, c = {};
        for (n in a) Object.prototype.hasOwnProperty.call(a, n) && (c[n] = a[n]);
        if ((4 < (c.orientation = t) && !(4 < i) || t < 5 && 4 < i) && (c.maxWidth = a.maxHeight, 
        c.maxHeight = a.maxWidth, c.minWidth = a.minHeight, c.minHeight = a.minWidth, 
        c.sourceWidth = a.sourceHeight, c.sourceHeight = a.sourceWidth), 1 < i) {
            switch (i) {
              case 2:
                o = a.left, l = a.right;
                break;

              case 3:
                r = a.bottom, o = a.left, s = a.top, l = a.right;
                break;

              case 4:
                r = a.bottom, s = a.top;
                break;

              case 5:
                r = a.left, o = a.bottom, s = a.right, l = a.top;
                break;

              case 6:
                r = a.left, o = a.top, s = a.right, l = a.bottom;
                break;

              case 7:
                r = a.right, o = a.top, s = a.left, l = a.bottom;
                break;

              case 8:
                r = a.right, o = a.bottom, s = a.left, l = a.top;
            }
            g(t, i) && (e = r, i = o, r = s, o = l, s = e, l = i);
        }
        switch (c.top = r, c.right = o, c.bottom = s, c.left = l, t) {
          case 2:
            c.right = l, c.left = o;
            break;

          case 3:
            c.top = s, c.right = l, c.bottom = r, c.left = o;
            break;

          case 4:
            c.top = s, c.bottom = r;
            break;

          case 5:
            c.top = l, c.right = s, c.bottom = o, c.left = r;
            break;

          case 6:
            c.top = o, c.right = s, c.bottom = l, c.left = r;
            break;

          case 7:
            c.top = o, c.right = r, c.bottom = l, c.left = s;
            break;

          case 8:
            c.top = l, c.right = r, c.bottom = o, c.left = s;
        }
        return c;
    }, f.transformCoordinates = function(e, t, i) {
        l.call(f, e, t, i);
        t = t.orientation, i = f.orientation && i.exif && i.exif.get("Orientation");
        if (d(t, i)) {
            var a = e.getContext("2d"), n = e.width, r = e.height, o = n, s = r;
            switch ((4 < t && !(4 < i) || t < 5 && 4 < i) && (e.width = r, e.height = n), 
            4 < t && (o = r, s = n), i) {
              case 2:
                a.translate(o, 0), a.scale(-1, 1);
                break;

              case 3:
                a.translate(o, s), a.rotate(Math.PI);
                break;

              case 4:
                a.translate(0, s), a.scale(1, -1);
                break;

              case 5:
                a.rotate(-.5 * Math.PI), a.scale(-1, 1);
                break;

              case 6:
                a.rotate(-.5 * Math.PI), a.translate(-o, 0);
                break;

              case 7:
                a.rotate(-.5 * Math.PI), a.translate(-o, s), a.scale(1, -1);
                break;

              case 8:
                a.rotate(.5 * Math.PI), a.translate(0, -s);
            }
            switch (g(t, i) && (a.translate(o, s), a.rotate(Math.PI)), t) {
              case 2:
                a.translate(n, 0), a.scale(-1, 1);
                break;

              case 3:
                a.translate(n, r), a.rotate(Math.PI);
                break;

              case 4:
                a.translate(0, r), a.scale(1, -1);
                break;

              case 5:
                a.rotate(.5 * Math.PI), a.scale(1, -1);
                break;

              case 6:
                a.rotate(.5 * Math.PI), a.translate(0, -r);
                break;

              case 7:
                a.rotate(.5 * Math.PI), a.translate(n, -r), a.scale(-1, 1);
                break;

              case 8:
                a.rotate(-.5 * Math.PI), a.translate(-n, 0);
            }
        }
    };
}), function(e) {
    "use strict";
    "function" == typeof define && define.amd ? define([ "./load-image", "./load-image-meta" ], e) : "object" == typeof module && module.exports ? e(require("./load-image"), require("./load-image-meta")) : e(window.loadImage);
}(function(n) {
    "use strict";
    function m(e) {
        e && (Object.defineProperty(this, "map", {
            value: this.ifds[e].map
        }), Object.defineProperty(this, "tags", {
            value: this.tags && this.tags[e] || {}
        }));
    }
    m.prototype.ifds = {
        ifd1: {
            name: "Thumbnail",
            map: m.prototype.map = {
                Orientation: 274,
                Thumbnail: "ifd1",
                Blob: 513,
                Exif: 34665,
                GPSInfo: 34853,
                Interoperability: 40965
            }
        },
        34665: {
            name: "Exif",
            map: {}
        },
        34853: {
            name: "GPSInfo",
            map: {}
        },
        40965: {
            name: "Interoperability",
            map: {}
        }
    }, m.prototype.get = function(e) {
        return this[e] || this[this.map[e]];
    };
    var h = {
        1: {
            getValue: function(e, t) {
                return e.getUint8(t);
            },
            size: 1
        },
        2: {
            getValue: function(e, t) {
                return String.fromCharCode(e.getUint8(t));
            },
            size: 1,
            ascii: !0
        },
        3: {
            getValue: function(e, t, i) {
                return e.getUint16(t, i);
            },
            size: 2
        },
        4: {
            getValue: function(e, t, i) {
                return e.getUint32(t, i);
            },
            size: 4
        },
        5: {
            getValue: function(e, t, i) {
                return e.getUint32(t, i) / e.getUint32(t + 4, i);
            },
            size: 8
        },
        9: {
            getValue: function(e, t, i) {
                return e.getInt32(t, i);
            },
            size: 4
        },
        10: {
            getValue: function(e, t, i) {
                return e.getInt32(t, i) / e.getInt32(t + 4, i);
            },
            size: 8
        }
    };
    function p(e, t, i) {
        return (!e || e[i]) && (!t || !0 !== t[i]);
    }
    function A(e, t, i, a, n, r, o, s) {
        var l, c, f, u, d, g;
        if (i + 6 > e.byteLength) console.log("Invalid Exif data: Invalid directory offset."); else {
            if (!((c = i + 2 + 12 * (l = e.getUint16(i, a))) + 4 > e.byteLength)) {
                for (f = 0; f < l; f += 1) p(o, s, d = e.getUint16(u = i + 2 + 12 * f, a)) && (g = function(e, t, i, a, n, r) {
                    var o, s, l, c, f, u = h[a];
                    if (u) {
                        if (!((o = 4 < (a = u.size * n) ? t + e.getUint32(i + 8, r) : i + 8) + a > e.byteLength)) {
                            if (1 === n) return u.getValue(e, o, r);
                            for (s = [], l = 0; l < n; l += 1) s[l] = u.getValue(e, o + l * u.size, r);
                            if (u.ascii) {
                                for (c = "", l = 0; l < s.length && "\0" !== (f = s[l]); l += 1) c += f;
                                return c;
                            }
                            return s;
                        }
                        console.log("Invalid Exif data: Invalid data offset.");
                    } else console.log("Invalid Exif data: Invalid tag type.");
                }(e, t, u, e.getUint16(u + 2, a), e.getUint32(u + 4, a), a), n[d] = g, 
                r && (r[d] = u));
                return e.getUint32(c, a);
            }
            console.log("Invalid Exif data: Invalid directory size.");
        }
    }
    h[7] = h[1], n.parseExifData = function(l, e, t, c, i) {
        if (!i.disableExif) {
            var f, u = i.includeExifTags, d = i.excludeExifTags || {
                34665: {
                    37500: !0
                }
            }, g = e + 10;
            if (1165519206 === l.getUint32(e + 4)) if (g + 8 > l.byteLength) console.log("Invalid Exif data: Invalid segment size."); else if (0 === l.getUint16(e + 8)) {
                switch (l.getUint16(g)) {
                  case 18761:
                    f = !0;
                    break;

                  case 19789:
                    f = !1;
                    break;

                  default:
                    return void console.log("Invalid Exif data: Invalid byte alignment marker.");
                }
                42 === l.getUint16(g + 2, f) ? (e = l.getUint32(g + 4, f), c.exif = new m(), 
                i.disableExifOffsets || (c.exifOffsets = new m(), c.exifTiffOffset = g, 
                c.exifLittleEndian = f), (e = A(l, g, g + e, f, c.exif, c.exifOffsets, u, d)) && p(u, d, "ifd1") && (c.exif.ifd1 = e, 
                c.exifOffsets && (c.exifOffsets.ifd1 = g + e)), Object.keys(c.exif.ifds).forEach(function(e) {
                    var t, i, a, n, r, o, s;
                    i = e, a = l, n = g, r = f, o = u, s = d, (e = (t = c).exif[i]) && (t.exif[i] = new m(i), 
                    t.exifOffsets && (t.exifOffsets[i] = new m(i)), A(a, n, n + e, r, t.exif[i], t.exifOffsets && t.exifOffsets[i], o && o[i], s && s[i]));
                }), (e = c.exif.ifd1) && e[513] && (e[513] = function(e, t, i) {
                    if (i) {
                        if (!(t + i > e.byteLength)) return new Blob([ n.bufferSlice.call(e.buffer, t, t + i) ], {
                            type: "image/jpeg"
                        });
                        console.log("Invalid Exif data: Invalid thumbnail data.");
                    }
                }(l, g + e[513], e[514]))) : console.log("Invalid Exif data: Missing TIFF marker.");
            } else console.log("Invalid Exif data: Missing byte alignment offset.");
        }
    }, n.metaDataParsers.jpeg[65505].push(n.parseExifData), n.exifWriters = {
        274: function(e, t, i) {
            var a = t.exifOffsets[274];
            return a && new DataView(e, a + 8, 2).setUint16(0, i, t.exifLittleEndian), 
            e;
        }
    }, n.writeExifData = function(e, t, i, a) {
        return n.exifWriters[t.exif.map[i]](e, t, a);
    }, n.ExifMap = m;
}), function(e) {
    "use strict";
    "function" == typeof define && define.amd ? define([ "./load-image", "./load-image-exif" ], e) : "object" == typeof module && module.exports ? e(require("./load-image"), require("./load-image-exif")) : e(window.loadImage);
}(function(e) {
    "use strict";
    var n = e.ExifMap.prototype;
    n.tags = {
        256: "ImageWidth",
        257: "ImageHeight",
        258: "BitsPerSample",
        259: "Compression",
        262: "PhotometricInterpretation",
        274: "Orientation",
        277: "SamplesPerPixel",
        284: "PlanarConfiguration",
        530: "YCbCrSubSampling",
        531: "YCbCrPositioning",
        282: "XResolution",
        283: "YResolution",
        296: "ResolutionUnit",
        273: "StripOffsets",
        278: "RowsPerStrip",
        279: "StripByteCounts",
        513: "JPEGInterchangeFormat",
        514: "JPEGInterchangeFormatLength",
        301: "TransferFunction",
        318: "WhitePoint",
        319: "PrimaryChromaticities",
        529: "YCbCrCoefficients",
        532: "ReferenceBlackWhite",
        306: "DateTime",
        270: "ImageDescription",
        271: "Make",
        272: "Model",
        305: "Software",
        315: "Artist",
        33432: "Copyright",
        34665: {
            36864: "ExifVersion",
            40960: "FlashpixVersion",
            40961: "ColorSpace",
            40962: "PixelXDimension",
            40963: "PixelYDimension",
            42240: "Gamma",
            37121: "ComponentsConfiguration",
            37122: "CompressedBitsPerPixel",
            37500: "MakerNote",
            37510: "UserComment",
            40964: "RelatedSoundFile",
            36867: "DateTimeOriginal",
            36868: "DateTimeDigitized",
            36880: "OffsetTime",
            36881: "OffsetTimeOriginal",
            36882: "OffsetTimeDigitized",
            37520: "SubSecTime",
            37521: "SubSecTimeOriginal",
            37522: "SubSecTimeDigitized",
            33434: "ExposureTime",
            33437: "FNumber",
            34850: "ExposureProgram",
            34852: "SpectralSensitivity",
            34855: "PhotographicSensitivity",
            34856: "OECF",
            34864: "SensitivityType",
            34865: "StandardOutputSensitivity",
            34866: "RecommendedExposureIndex",
            34867: "ISOSpeed",
            34868: "ISOSpeedLatitudeyyy",
            34869: "ISOSpeedLatitudezzz",
            37377: "ShutterSpeedValue",
            37378: "ApertureValue",
            37379: "BrightnessValue",
            37380: "ExposureBias",
            37381: "MaxApertureValue",
            37382: "SubjectDistance",
            37383: "MeteringMode",
            37384: "LightSource",
            37385: "Flash",
            37396: "SubjectArea",
            37386: "FocalLength",
            41483: "FlashEnergy",
            41484: "SpatialFrequencyResponse",
            41486: "FocalPlaneXResolution",
            41487: "FocalPlaneYResolution",
            41488: "FocalPlaneResolutionUnit",
            41492: "SubjectLocation",
            41493: "ExposureIndex",
            41495: "SensingMethod",
            41728: "FileSource",
            41729: "SceneType",
            41730: "CFAPattern",
            41985: "CustomRendered",
            41986: "ExposureMode",
            41987: "WhiteBalance",
            41988: "DigitalZoomRatio",
            41989: "FocalLengthIn35mmFilm",
            41990: "SceneCaptureType",
            41991: "GainControl",
            41992: "Contrast",
            41993: "Saturation",
            41994: "Sharpness",
            41995: "DeviceSettingDescription",
            41996: "SubjectDistanceRange",
            42016: "ImageUniqueID",
            42032: "CameraOwnerName",
            42033: "BodySerialNumber",
            42034: "LensSpecification",
            42035: "LensMake",
            42036: "LensModel",
            42037: "LensSerialNumber"
        },
        34853: {
            0: "GPSVersionID",
            1: "GPSLatitudeRef",
            2: "GPSLatitude",
            3: "GPSLongitudeRef",
            4: "GPSLongitude",
            5: "GPSAltitudeRef",
            6: "GPSAltitude",
            7: "GPSTimeStamp",
            8: "GPSSatellites",
            9: "GPSStatus",
            10: "GPSMeasureMode",
            11: "GPSDOP",
            12: "GPSSpeedRef",
            13: "GPSSpeed",
            14: "GPSTrackRef",
            15: "GPSTrack",
            16: "GPSImgDirectionRef",
            17: "GPSImgDirection",
            18: "GPSMapDatum",
            19: "GPSDestLatitudeRef",
            20: "GPSDestLatitude",
            21: "GPSDestLongitudeRef",
            22: "GPSDestLongitude",
            23: "GPSDestBearingRef",
            24: "GPSDestBearing",
            25: "GPSDestDistanceRef",
            26: "GPSDestDistance",
            27: "GPSProcessingMethod",
            28: "GPSAreaInformation",
            29: "GPSDateStamp",
            30: "GPSDifferential",
            31: "GPSHPositioningError"
        },
        40965: {
            1: "InteroperabilityIndex"
        }
    }, n.tags.ifd1 = n.tags, n.stringValues = {
        ExposureProgram: {
            0: "Undefined",
            1: "Manual",
            2: "Normal program",
            3: "Aperture priority",
            4: "Shutter priority",
            5: "Creative program",
            6: "Action program",
            7: "Portrait mode",
            8: "Landscape mode"
        },
        MeteringMode: {
            0: "Unknown",
            1: "Average",
            2: "CenterWeightedAverage",
            3: "Spot",
            4: "MultiSpot",
            5: "Pattern",
            6: "Partial",
            255: "Other"
        },
        LightSource: {
            0: "Unknown",
            1: "Daylight",
            2: "Fluorescent",
            3: "Tungsten (incandescent light)",
            4: "Flash",
            9: "Fine weather",
            10: "Cloudy weather",
            11: "Shade",
            12: "Daylight fluorescent (D 5700 - 7100K)",
            13: "Day white fluorescent (N 4600 - 5400K)",
            14: "Cool white fluorescent (W 3900 - 4500K)",
            15: "White fluorescent (WW 3200 - 3700K)",
            17: "Standard light A",
            18: "Standard light B",
            19: "Standard light C",
            20: "D55",
            21: "D65",
            22: "D75",
            23: "D50",
            24: "ISO studio tungsten",
            255: "Other"
        },
        Flash: {
            0: "Flash did not fire",
            1: "Flash fired",
            5: "Strobe return light not detected",
            7: "Strobe return light detected",
            9: "Flash fired, compulsory flash mode",
            13: "Flash fired, compulsory flash mode, return light not detected",
            15: "Flash fired, compulsory flash mode, return light detected",
            16: "Flash did not fire, compulsory flash mode",
            24: "Flash did not fire, auto mode",
            25: "Flash fired, auto mode",
            29: "Flash fired, auto mode, return light not detected",
            31: "Flash fired, auto mode, return light detected",
            32: "No flash function",
            65: "Flash fired, red-eye reduction mode",
            69: "Flash fired, red-eye reduction mode, return light not detected",
            71: "Flash fired, red-eye reduction mode, return light detected",
            73: "Flash fired, compulsory flash mode, red-eye reduction mode",
            77: "Flash fired, compulsory flash mode, red-eye reduction mode, return light not detected",
            79: "Flash fired, compulsory flash mode, red-eye reduction mode, return light detected",
            89: "Flash fired, auto mode, red-eye reduction mode",
            93: "Flash fired, auto mode, return light not detected, red-eye reduction mode",
            95: "Flash fired, auto mode, return light detected, red-eye reduction mode"
        },
        SensingMethod: {
            1: "Undefined",
            2: "One-chip color area sensor",
            3: "Two-chip color area sensor",
            4: "Three-chip color area sensor",
            5: "Color sequential area sensor",
            7: "Trilinear sensor",
            8: "Color sequential linear sensor"
        },
        SceneCaptureType: {
            0: "Standard",
            1: "Landscape",
            2: "Portrait",
            3: "Night scene"
        },
        SceneType: {
            1: "Directly photographed"
        },
        CustomRendered: {
            0: "Normal process",
            1: "Custom process"
        },
        WhiteBalance: {
            0: "Auto white balance",
            1: "Manual white balance"
        },
        GainControl: {
            0: "None",
            1: "Low gain up",
            2: "High gain up",
            3: "Low gain down",
            4: "High gain down"
        },
        Contrast: {
            0: "Normal",
            1: "Soft",
            2: "Hard"
        },
        Saturation: {
            0: "Normal",
            1: "Low saturation",
            2: "High saturation"
        },
        Sharpness: {
            0: "Normal",
            1: "Soft",
            2: "Hard"
        },
        SubjectDistanceRange: {
            0: "Unknown",
            1: "Macro",
            2: "Close view",
            3: "Distant view"
        },
        FileSource: {
            3: "DSC"
        },
        ComponentsConfiguration: {
            0: "",
            1: "Y",
            2: "Cb",
            3: "Cr",
            4: "R",
            5: "G",
            6: "B"
        },
        Orientation: {
            1: "Original",
            2: "Horizontal flip",
            3: "Rotate 180° CCW",
            4: "Vertical flip",
            5: "Vertical flip + Rotate 90° CW",
            6: "Rotate 90° CW",
            7: "Horizontal flip + Rotate 90° CW",
            8: "Rotate 90° CCW"
        }
    }, n.getText = function(e) {
        var t = this.get(e);
        switch (e) {
          case "LightSource":
          case "Flash":
          case "MeteringMode":
          case "ExposureProgram":
          case "SensingMethod":
          case "SceneCaptureType":
          case "SceneType":
          case "CustomRendered":
          case "WhiteBalance":
          case "GainControl":
          case "Contrast":
          case "Saturation":
          case "Sharpness":
          case "SubjectDistanceRange":
          case "FileSource":
          case "Orientation":
            return this.stringValues[e][t];

          case "ExifVersion":
          case "FlashpixVersion":
            return t ? String.fromCharCode(t[0], t[1], t[2], t[3]) : void 0;

          case "ComponentsConfiguration":
            return t ? this.stringValues[e][t[0]] + this.stringValues[e][t[1]] + this.stringValues[e][t[2]] + this.stringValues[e][t[3]] : void 0;

          case "GPSVersionID":
            return t ? t[0] + "." + t[1] + "." + t[2] + "." + t[3] : void 0;
        }
        return String(t);
    }, n.getAll = function() {
        var e, t, i = {};
        for (e in this) Object.prototype.hasOwnProperty.call(this, e) && ((t = this[e]) && t.getAll ? i[this.ifds[e].name] = t.getAll() : (t = this.tags[e]) && (i[t] = this.getText(t)));
        return i;
    }, n.getName = function(e) {
        var t = this.tags[e];
        return "object" == typeof t ? this.ifds[e].name : t;
    }, function() {
        var e, t, i, a = n.tags;
        for (e in a) if (Object.prototype.hasOwnProperty.call(a, e)) if (t = n.ifds[e]) for (e in i = a[e]) Object.prototype.hasOwnProperty.call(i, e) && (t.map[i[e]] = Number(e)); else n.map[a[e]] = Number(e);
    }();
}), function(e) {
    "use strict";
    "function" == typeof define && define.amd ? define([ "./load-image", "./load-image-meta" ], e) : "object" == typeof module && module.exports ? e(require("./load-image"), require("./load-image-meta")) : e(window.loadImage);
}(function(e) {
    "use strict";
    function l() {}
    function u(e, t, i, a, n) {
        return "binary" === t.types[e] ? new Blob([ i.buffer.slice(a, a + n) ]) : "Uint16" === t.types[e] ? i.getUint16(a) : function(e, t, i) {
            for (var a = "", n = t + i, r = t; r < n; r += 1) a += String.fromCharCode(e.getUint8(r));
            return a;
        }(i, a, n);
    }
    function c(e, t, i, a, n, r) {
        for (var o, s, l, c = t + i, f = t; f < c; ) 28 === e.getUint8(f) && 2 === e.getUint8(f + 1) && (o = e.getUint8(f + 2), 
        n && !n[o] || r && r[o] || (s = e.getInt16(f + 3), l = u(o, a.iptc, e, f + 5, s), 
        a.iptc[o] = (s = a.iptc[o], l = l, s === undefined ? l : s instanceof Array ? (s.push(l), 
        s) : [ s, l ]), a.iptcOffsets && (a.iptcOffsets[o] = f))), f += 1;
    }
    l.prototype.map = {
        ObjectName: 5
    }, l.prototype.types = {
        0: "Uint16",
        200: "Uint16",
        201: "Uint16",
        202: "binary"
    }, l.prototype.get = function(e) {
        return this[e] || this[this.map[e]];
    }, e.parseIptcData = function(e, t, i, a, n) {
        if (!n.disableIptc) for (var r = t + i; t + 8 < r; ) {
            if (o = t, 943868237 === (s = e).getUint32(o) && 1028 === s.getUint16(o + 4)) {
                var o = (o = t, (s = (s = e).getUint8(o + 7)) % 2 != 0 && (s += 1), 
                s = 0 === s ? 4 : s), s = t + 8 + o;
                if (r < s) {
                    console.log("Invalid IPTC data: Invalid segment offset.");
                    break;
                }
                o = e.getUint16(t + 6 + o);
                if (r < t + o) {
                    console.log("Invalid IPTC data: Invalid segment size.");
                    break;
                }
                return a.iptc = new l(), n.disableIptcOffsets || (a.iptcOffsets = new l()), 
                void c(e, s, o, a, n.includeIptcTags, n.excludeIptcTags || {
                    202: !0
                });
            }
            t += 1;
        }
    }, e.metaDataParsers.jpeg[65517].push(e.parseIptcData), e.IptcMap = l;
}), function(e) {
    "use strict";
    "function" == typeof define && define.amd ? define([ "./load-image", "./load-image-iptc" ], e) : "object" == typeof module && module.exports ? e(require("./load-image"), require("./load-image-iptc")) : e(window.loadImage);
}(function(e) {
    "use strict";
    var a = e.IptcMap.prototype;
    a.tags = {
        0: "ApplicationRecordVersion",
        3: "ObjectTypeReference",
        4: "ObjectAttributeReference",
        5: "ObjectName",
        7: "EditStatus",
        8: "EditorialUpdate",
        10: "Urgency",
        12: "SubjectReference",
        15: "Category",
        20: "SupplementalCategories",
        22: "FixtureIdentifier",
        25: "Keywords",
        26: "ContentLocationCode",
        27: "ContentLocationName",
        30: "ReleaseDate",
        35: "ReleaseTime",
        37: "ExpirationDate",
        38: "ExpirationTime",
        40: "SpecialInstructions",
        42: "ActionAdvised",
        45: "ReferenceService",
        47: "ReferenceDate",
        50: "ReferenceNumber",
        55: "DateCreated",
        60: "TimeCreated",
        62: "DigitalCreationDate",
        63: "DigitalCreationTime",
        65: "OriginatingProgram",
        70: "ProgramVersion",
        75: "ObjectCycle",
        80: "Byline",
        85: "BylineTitle",
        90: "City",
        92: "Sublocation",
        95: "State",
        100: "CountryCode",
        101: "Country",
        103: "OriginalTransmissionReference",
        105: "Headline",
        110: "Credit",
        115: "Source",
        116: "CopyrightNotice",
        118: "Contact",
        120: "Caption",
        121: "LocalCaption",
        122: "Writer",
        125: "RasterizedCaption",
        130: "ImageType",
        131: "ImageOrientation",
        135: "LanguageIdentifier",
        150: "AudioType",
        151: "AudioSamplingRate",
        152: "AudioSamplingResolution",
        153: "AudioDuration",
        154: "AudioOutcue",
        184: "JobID",
        185: "MasterDocumentID",
        186: "ShortDocumentID",
        187: "UniqueDocumentID",
        188: "OwnerID",
        200: "ObjectPreviewFileFormat",
        201: "ObjectPreviewFileVersion",
        202: "ObjectPreviewData",
        221: "Prefs",
        225: "ClassifyState",
        228: "SimilarityIndex",
        230: "DocumentNotes",
        231: "DocumentHistory",
        232: "ExifCameraInfo",
        255: "CatalogSets"
    }, a.stringValues = {
        10: {
            0: "0 (reserved)",
            1: "1 (most urgent)",
            2: "2",
            3: "3",
            4: "4",
            5: "5 (normal urgency)",
            6: "6",
            7: "7",
            8: "8 (least urgent)",
            9: "9 (user-defined priority)"
        },
        75: {
            a: "Morning",
            b: "Both Morning and Evening",
            p: "Evening"
        },
        131: {
            L: "Landscape",
            P: "Portrait",
            S: "Square"
        }
    }, a.getText = function(e) {
        var t = this.get(e), e = this.map[e], e = this.stringValues[e];
        return e ? e[t] : String(t);
    }, a.getAll = function() {
        var e, t, i = {};
        for (e in this) Object.prototype.hasOwnProperty.call(this, e) && (t = this.tags[e]) && (i[t] = this.getText(t));
        return i;
    }, a.getName = function(e) {
        return this.tags[e];
    }, function() {
        var e, t = a.tags, i = a.map || {};
        for (e in t) Object.prototype.hasOwnProperty.call(t, e) && (i[t[e]] = Number(e));
    }();
});

(function(window) {
    "use strict";
    var CanvasPrototype = window.HTMLCanvasElement && window.HTMLCanvasElement.prototype;
    var hasBlobConstructor = window.Blob && function() {
        try {
            return Boolean(new Blob());
        } catch (e) {
            return false;
        }
    }();
    var hasArrayBufferViewSupport = hasBlobConstructor && window.Uint8Array && function() {
        try {
            return new Blob([ new Uint8Array(100) ]).size === 100;
        } catch (e) {
            return false;
        }
    }();
    var BlobBuilder = window.BlobBuilder || window.WebKitBlobBuilder || window.MozBlobBuilder || window.MSBlobBuilder;
    var dataURIPattern = /^data:((.*?)(;charset=.*?)?)(;base64)?,/;
    var dataURLtoBlob = (hasBlobConstructor || BlobBuilder) && window.atob && window.ArrayBuffer && window.Uint8Array && function(dataURI) {
        var matches, mediaType, isBase64, dataString, byteString, arrayBuffer, intArray, i, bb;
        matches = dataURI.match(dataURIPattern);
        if (!matches) {
            throw new Error("invalid data URI");
        }
        mediaType = matches[2] ? matches[1] : "text/plain" + (matches[3] || ";charset=US-ASCII");
        isBase64 = !!matches[4];
        dataString = dataURI.slice(matches[0].length);
        if (isBase64) {
            byteString = atob(dataString);
        } else {
            byteString = decodeURIComponent(dataString);
        }
        arrayBuffer = new ArrayBuffer(byteString.length);
        intArray = new Uint8Array(arrayBuffer);
        for (i = 0; i < byteString.length; i += 1) {
            intArray[i] = byteString.charCodeAt(i);
        }
        if (hasBlobConstructor) {
            return new Blob([ hasArrayBufferViewSupport ? intArray : arrayBuffer ], {
                type: mediaType
            });
        }
        bb = new BlobBuilder();
        bb.append(arrayBuffer);
        return bb.getBlob(mediaType);
    };
    if (window.HTMLCanvasElement && !CanvasPrototype.toBlob) {
        if (CanvasPrototype.mozGetAsFile) {
            CanvasPrototype.toBlob = function(callback, type, quality) {
                var self = this;
                setTimeout(function() {
                    if (quality && CanvasPrototype.toDataURL && dataURLtoBlob) {
                        callback(dataURLtoBlob(self.toDataURL(type, quality)));
                    } else {
                        callback(self.mozGetAsFile("blob", type));
                    }
                });
            };
        } else if (CanvasPrototype.toDataURL && dataURLtoBlob) {
            if (CanvasPrototype.msToBlob) {
                CanvasPrototype.toBlob = function(callback, type, quality) {
                    var self = this;
                    setTimeout(function() {
                        if ((type && type !== "image/png" || quality) && CanvasPrototype.toDataURL && dataURLtoBlob) {
                            callback(dataURLtoBlob(self.toDataURL(type, quality)));
                        } else {
                            callback(self.msToBlob(type));
                        }
                    });
                };
            } else {
                CanvasPrototype.toBlob = function(callback, type, quality) {
                    var self = this;
                    setTimeout(function() {
                        callback(dataURLtoBlob(self.toDataURL(type, quality)));
                    });
                };
            }
        }
    }
    if (typeof define === "function" && define.amd) {
        define(function() {
            return dataURLtoBlob;
        });
    } else if (typeof module === "object" && module.exports) {
        module.exports = dataURLtoBlob;
    } else {
        window.dataURLtoBlob = dataURLtoBlob;
    }
})(window);

(function(factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        define([ "jquery" ], factory);
    } else if (typeof exports === "object") {
        factory(require("jquery"));
    } else {
        factory(window.jQuery);
    }
})(function($) {
    "use strict";
    var counter = 0, jsonAPI = $, jsonParse = "parseJSON";
    if ("JSON" in window && "parse" in JSON) {
        jsonAPI = JSON;
        jsonParse = "parse";
    }
    $.ajaxTransport("iframe", function(options) {
        if (options.async) {
            var initialIframeSrc = options.initialIframeSrc || "javascript:false;", form, iframe, addParamChar;
            return {
                send: function(_, completeCallback) {
                    form = $('<form style="display:none;"></form>');
                    form.attr("accept-charset", options.formAcceptCharset);
                    addParamChar = /\?/.test(options.url) ? "&" : "?";
                    if (options.type === "DELETE") {
                        options.url = options.url + addParamChar + "_method=DELETE";
                        options.type = "POST";
                    } else if (options.type === "PUT") {
                        options.url = options.url + addParamChar + "_method=PUT";
                        options.type = "POST";
                    } else if (options.type === "PATCH") {
                        options.url = options.url + addParamChar + "_method=PATCH";
                        options.type = "POST";
                    }
                    counter += 1;
                    iframe = $('<iframe src="' + initialIframeSrc + '" name="iframe-transport-' + counter + '"></iframe>').on("load", function() {
                        var fileInputClones, paramNames = $.isArray(options.paramName) ? options.paramName : [ options.paramName ];
                        iframe.off("load").on("load", function() {
                            var response;
                            try {
                                response = iframe.contents();
                                if (!response.length || !response[0].firstChild) {
                                    throw new Error();
                                }
                            } catch (e) {
                                response = undefined;
                            }
                            completeCallback(200, "success", {
                                iframe: response
                            });
                            $('<iframe src="' + initialIframeSrc + '"></iframe>').appendTo(form);
                            window.setTimeout(function() {
                                form.remove();
                            }, 0);
                        });
                        form.prop("target", iframe.prop("name")).prop("action", options.url).prop("method", options.type);
                        if (options.formData) {
                            $.each(options.formData, function(index, field) {
                                $('<input type="hidden"/>').prop("name", field.name).val(field.value).appendTo(form);
                            });
                        }
                        if (options.fileInput && options.fileInput.length && options.type === "POST") {
                            fileInputClones = options.fileInput.clone();
                            options.fileInput.after(function(index) {
                                return fileInputClones[index];
                            });
                            if (options.paramName) {
                                options.fileInput.each(function(index) {
                                    $(this).prop("name", paramNames[index] || options.paramName);
                                });
                            }
                            form.append(options.fileInput).prop("enctype", "multipart/form-data").prop("encoding", "multipart/form-data");
                            options.fileInput.removeAttr("form");
                        }
                        window.setTimeout(function() {
                            form.submit();
                            if (fileInputClones && fileInputClones.length) {
                                options.fileInput.each(function(index, input) {
                                    var clone = $(fileInputClones[index]);
                                    $(input).prop("name", clone.prop("name")).attr("form", clone.attr("form"));
                                    clone.replaceWith(input);
                                });
                            }
                        }, 0);
                    });
                    form.append(iframe).appendTo(document.body);
                },
                abort: function() {
                    if (iframe) {
                        iframe.off("load").prop("src", initialIframeSrc);
                    }
                    if (form) {
                        form.remove();
                    }
                }
            };
        }
    });
    $.ajaxSetup({
        converters: {
            "iframe text": function(iframe) {
                return iframe && $(iframe[0].body).text();
            },
            "iframe json": function(iframe) {
                return iframe && jsonAPI[jsonParse]($(iframe[0].body).text());
            },
            "iframe html": function(iframe) {
                return iframe && $(iframe[0].body).html();
            },
            "iframe xml": function(iframe) {
                var xmlDoc = iframe && iframe[0];
                return xmlDoc && $.isXMLDoc(xmlDoc) ? xmlDoc : $.parseXML(xmlDoc.XMLDocument && xmlDoc.XMLDocument.xml || $(xmlDoc.body).html());
            },
            "iframe script": function(iframe) {
                return iframe && $.globalEval($(iframe[0].body).text());
            }
        }
    });
});

(function(factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        define([ "jquery", "jquery-ui/ui/widget" ], factory);
    } else if (typeof exports === "object") {
        factory(require("jquery"), require("./vendor/jquery.ui.widget"));
    } else {
        factory(window.jQuery);
    }
})(function($) {
    "use strict";
    $.support.fileInput = !(new RegExp("(Android (1\\.[0156]|2\\.[01]))" + "|(Windows Phone (OS 7|8\\.0))|(XBLWP)|(ZuneWP)|(WPDesktop)" + "|(w(eb)?OSBrowser)|(webOS)" + "|(Kindle/(1\\.0|2\\.[05]|3\\.0))").test(window.navigator.userAgent) || $('<input type="file"/>').prop("disabled"));
    $.support.xhrFileUpload = !!(window.ProgressEvent && window.FileReader);
    $.support.xhrFormDataFileUpload = !!window.FormData;
    $.support.blobSlice = window.Blob && (Blob.prototype.slice || Blob.prototype.webkitSlice || Blob.prototype.mozSlice);
    function getDragHandler(type) {
        var isDragOver = type === "dragover";
        return function(e) {
            e.dataTransfer = e.originalEvent && e.originalEvent.dataTransfer;
            var dataTransfer = e.dataTransfer;
            if (dataTransfer && $.inArray("Files", dataTransfer.types) !== -1 && this._trigger(type, $.Event(type, {
                delegatedEvent: e
            })) !== false) {
                e.preventDefault();
                if (isDragOver) {
                    dataTransfer.dropEffect = "copy";
                }
            }
        };
    }
    $.widget("blueimp.yafFileUpload", {
        options: {
            dropZone: $(document),
            pasteZone: undefined,
            fileInput: undefined,
            replaceFileInput: true,
            paramName: undefined,
            singleFileUploads: true,
            limitMultiFileUploads: undefined,
            limitMultiFileUploadSize: undefined,
            limitMultiFileUploadSizeOverhead: 512,
            sequentialUploads: false,
            limitConcurrentUploads: undefined,
            forceIframeTransport: false,
            redirect: undefined,
            redirectParamName: undefined,
            postMessage: undefined,
            multipart: true,
            maxChunkSize: undefined,
            uploadedBytes: undefined,
            recalculateProgress: true,
            progressInterval: 100,
            bitrateInterval: 500,
            autoUpload: true,
            uniqueFilenames: undefined,
            messages: {
                uploadedBytes: "Uploaded bytes exceed file size"
            },
            i18n: function(message, context) {
                message = this.messages[message] || message.toString();
                if (context) {
                    $.each(context, function(key, value) {
                        message = message.replace("{" + key + "}", value);
                    });
                }
                return message;
            },
            formData: function(form) {
                return form.serializeArray();
            },
            add: function(e, data) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                if (data.autoUpload || data.autoUpload !== false && $(this).fileupload("option", "autoUpload")) {
                    data.process().done(function() {
                        data.submit();
                    });
                }
            },
            processData: false,
            contentType: false,
            cache: false,
            timeout: 0
        },
        _promisePipe: function() {
            var parts = $.fn.jquery.split(".");
            return Number(parts[0]) > 1 || Number(parts[1]) > 7 ? "then" : "pipe";
        }(),
        _specialOptions: [ "fileInput", "dropZone", "pasteZone", "multipart", "forceIframeTransport" ],
        _blobSlice: $.support.blobSlice && function() {
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
                    this.bitrate = (loaded - this.loaded) * (1e3 / timeDiff) * 8;
                    this.loaded = loaded;
                    this.timestamp = now;
                }
                return this.bitrate;
            };
        },
        _isXHRUpload: function(options) {
            return !options.forceIframeTransport && (!options.multipart && $.support.xhrFileUpload || $.support.xhrFormDataFileUpload);
        },
        _getFormData: function(options) {
            var formData;
            if ($.type(options.formData) === "function") {
                return options.formData(options.form);
            }
            if ($.isArray(options.formData)) {
                return options.formData;
            }
            if ($.type(options.formData) === "object") {
                formData = [];
                $.each(options.formData, function(name, value) {
                    formData.push({
                        name: name,
                        value: value
                    });
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
                var now = Date.now ? Date.now() : new Date().getTime(), loaded;
                if (data._time && data.progressInterval && now - data._time < data.progressInterval && e.loaded !== e.total) {
                    return;
                }
                data._time = now;
                loaded = Math.floor(e.loaded / e.total * (data.chunkSize || data._progress.total)) + (data.uploadedBytes || 0);
                this._progress.loaded += loaded - data._progress.loaded;
                this._progress.bitrate = this._bitrateTimer.getBitrate(now, this._progress.loaded, data.bitrateInterval);
                data._progress.loaded = data.loaded = loaded;
                data._progress.bitrate = data.bitrate = data._bitrateTimer.getBitrate(now, loaded, data.bitrateInterval);
                this._trigger("progress", $.Event("progress", {
                    delegatedEvent: e
                }), data);
                this._trigger("progressall", $.Event("progressall", {
                    delegatedEvent: e
                }), this._progress);
            }
        },
        _initProgressListener: function(options) {
            var that = this, xhr = options.xhr ? options.xhr() : $.ajaxSettings.xhr();
            if (xhr.upload) {
                $(xhr.upload).on("progress", function(e) {
                    var oe = e.originalEvent;
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
                $(xhr.upload).off("progress");
            }
        },
        _isInstanceOf: function(type, obj) {
            return Object.prototype.toString.call(obj) === "[object " + type + "]";
        },
        _getUniqueFilename: function(name, map) {
            name = String(name);
            if (map[name]) {
                name = name.replace(/(?: \(([\d]+)\))?(\.[^.]+)?$/, function(_, p1, p2) {
                    var index = p1 ? Number(p1) + 1 : 1;
                    var ext = p2 || "";
                    return " (" + index + ")" + ext;
                });
                return this._getUniqueFilename(name, map);
            }
            map[name] = true;
            return name;
        },
        _initXHRData: function(options) {
            var that = this, formData, file = options.files[0], multipart = options.multipart || !$.support.xhrFileUpload, paramName = $.type(options.paramName) === "array" ? options.paramName[0] : options.paramName;
            options.headers = $.extend({}, options.headers);
            if (options.contentRange) {
                options.headers["Content-Range"] = options.contentRange;
            }
            if (!multipart || options.blob || !this._isInstanceOf("File", file)) {
                options.headers["Content-Disposition"] = 'attachment; filename="' + encodeURI(file.uploadName || file.name) + '"';
            }
            if (!multipart) {
                options.contentType = file.type || "application/octet-stream";
                options.data = options.blob || file;
            } else if ($.support.xhrFormDataFileUpload) {
                if (options.postMessage) {
                    formData = this._getFormData(options);
                    if (options.blob) {
                        formData.push({
                            name: paramName,
                            value: options.blob
                        });
                    } else {
                        $.each(options.files, function(index, file) {
                            formData.push({
                                name: $.type(options.paramName) === "array" && options.paramName[index] || paramName,
                                value: file
                            });
                        });
                    }
                } else {
                    if (that._isInstanceOf("FormData", options.formData)) {
                        formData = options.formData;
                    } else {
                        formData = new FormData();
                        $.each(this._getFormData(options), function(index, field) {
                            formData.append(field.name, field.value);
                        });
                    }
                    if (options.blob) {
                        formData.append(paramName, options.blob, file.uploadName || file.name);
                    } else {
                        $.each(options.files, function(index, file) {
                            if (that._isInstanceOf("File", file) || that._isInstanceOf("Blob", file)) {
                                var fileName = file.uploadName || file.name;
                                if (options.uniqueFilenames) {
                                    fileName = that._getUniqueFilename(fileName, options.uniqueFilenames);
                                }
                                formData.append($.type(options.paramName) === "array" && options.paramName[index] || paramName, file, fileName);
                            }
                        });
                    }
                }
                options.data = formData;
            }
            options.blob = null;
        },
        _initIframeSettings: function(options) {
            var targetHost = $("<a></a>").prop("href", options.url).prop("host");
            options.dataType = "iframe " + (options.dataType || "");
            options.formData = this._getFormData(options);
            if (options.redirect && targetHost && targetHost !== location.host) {
                options.formData.push({
                    name: options.redirectParamName || "redirect",
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
                    options.dataType = "postmessage " + (options.dataType || "");
                }
            } else {
                this._initIframeSettings(options);
            }
        },
        _getParamName: function(options) {
            var fileInput = $(options.fileInput), paramName = options.paramName;
            if (!paramName) {
                paramName = [];
                fileInput.each(function() {
                    var input = $(this), name = input.prop("name") || "files[]", i = (input.prop("files") || [ 1 ]).length;
                    while (i) {
                        paramName.push(name);
                        i -= 1;
                    }
                });
                if (!paramName.length) {
                    paramName = [ fileInput.prop("name") || "files[]" ];
                }
            } else if (!$.isArray(paramName)) {
                paramName = [ paramName ];
            }
            return paramName;
        },
        _initFormSettings: function(options) {
            if (!options.form || !options.form.length) {
                options.form = $(options.fileInput.prop("form"));
                if (!options.form.length) {
                    options.form = $(this.options.fileInput.prop("form"));
                }
            }
            options.paramName = this._getParamName(options);
            if (!options.url) {
                options.url = options.form.prop("action") || location.href;
            }
            options.type = (options.type || $.type(options.form.prop("method")) === "string" && options.form.prop("method") || "").toUpperCase();
            if (options.type !== "POST" && options.type !== "PUT" && options.type !== "PATCH") {
                options.type = "POST";
            }
            if (!options.formAcceptCharset) {
                options.formAcceptCharset = options.form.attr("accept-charset");
            }
        },
        _getAJAXSettings: function(data) {
            var options = $.extend({}, this.options, data);
            this._initFormSettings(options);
            this._initDataSettings(options);
            return options;
        },
        _getDeferredState: function(deferred) {
            if (deferred.state) {
                return deferred.state();
            }
            if (deferred.isResolved()) {
                return "resolved";
            }
            if (deferred.isRejected()) {
                return "rejected";
            }
            return "pending";
        },
        _enhancePromise: function(promise) {
            promise.success = promise.done;
            promise.error = promise.fail;
            promise.complete = promise.always;
            return promise;
        },
        _getXHRPromise: function(resolveOrReject, context, args) {
            var dfd = $.Deferred(), promise = dfd.promise();
            context = context || this.options.context || promise;
            if (resolveOrReject === true) {
                dfd.resolveWith(context, args);
            } else if (resolveOrReject === false) {
                dfd.rejectWith(context, args);
            }
            promise.abort = dfd.promise;
            return this._enhancePromise(promise);
        },
        _addConvenienceMethods: function(e, data) {
            var that = this, getPromise = function(args) {
                return $.Deferred().resolveWith(that, args).promise();
            };
            data.process = function(resolveFunc, rejectFunc) {
                if (resolveFunc || rejectFunc) {
                    data._processQueue = this._processQueue = (this._processQueue || getPromise([ this ]))[that._promisePipe](function() {
                        if (data.errorThrown) {
                            return $.Deferred().rejectWith(that, [ data ]).promise();
                        }
                        return getPromise(arguments);
                    })[that._promisePipe](resolveFunc, rejectFunc);
                }
                return this._processQueue || getPromise([ this ]);
            };
            data.submit = function() {
                if (this.state() !== "pending") {
                    data.jqXHR = this.jqXHR = that._trigger("submit", $.Event("submit", {
                        delegatedEvent: e
                    }), this) !== false && that._onSend(e, this);
                }
                return this.jqXHR || that._getXHRPromise();
            };
            data.abort = function() {
                if (this.jqXHR) {
                    return this.jqXHR.abort();
                }
                this.errorThrown = "abort";
                that._trigger("fail", null, this);
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
                return !this.jqXHR && this._processQueue && that._getDeferredState(this._processQueue) === "pending";
            };
            data.progress = function() {
                return this._progress;
            };
            data.response = function() {
                return this._response;
            };
        },
        _getUploadedBytes: function(jqXHR) {
            var range = jqXHR.getResponseHeader("Range"), parts = range && range.split("-"), upperBytesPos = parts && parts.length > 1 && parseInt(parts[1], 10);
            return upperBytesPos && upperBytesPos + 1;
        },
        _chunkedUpload: function(options, testOnly) {
            options.uploadedBytes = options.uploadedBytes || 0;
            var that = this, file = options.files[0], fs = file.size, ub = options.uploadedBytes, mcs = options.maxChunkSize || fs, slice = this._blobSlice, dfd = $.Deferred(), promise = dfd.promise(), jqXHR, upload;
            if (!(this._isXHRUpload(options) && slice && (ub || ($.type(mcs) === "function" ? mcs(options) : mcs) < fs)) || options.data) {
                return false;
            }
            if (testOnly) {
                return true;
            }
            if (ub >= fs) {
                file.error = options.i18n("uploadedBytes");
                return this._getXHRPromise(false, options.context, [ null, "error", file.error ]);
            }
            upload = function() {
                var o = $.extend({}, options), currentLoaded = o._progress.loaded;
                o.blob = slice.call(file, ub, ub + ($.type(mcs) === "function" ? mcs(o) : mcs), file.type);
                o.chunkSize = o.blob.size;
                o.contentRange = "bytes " + ub + "-" + (ub + o.chunkSize - 1) + "/" + fs;
                that._trigger("chunkbeforesend", null, o);
                that._initXHRData(o);
                that._initProgressListener(o);
                jqXHR = (that._trigger("chunksend", null, o) !== false && $.ajax(o) || that._getXHRPromise(false, o.context)).done(function(result, textStatus, jqXHR) {
                    ub = that._getUploadedBytes(jqXHR) || ub + o.chunkSize;
                    if (currentLoaded + o.chunkSize - o._progress.loaded) {
                        that._onProgress($.Event("progress", {
                            lengthComputable: true,
                            loaded: ub - o.uploadedBytes,
                            total: ub - o.uploadedBytes
                        }), o);
                    }
                    options.uploadedBytes = o.uploadedBytes = ub;
                    o.result = result;
                    o.textStatus = textStatus;
                    o.jqXHR = jqXHR;
                    that._trigger("chunkdone", null, o);
                    that._trigger("chunkalways", null, o);
                    if (ub < fs) {
                        upload();
                    } else {
                        dfd.resolveWith(o.context, [ result, textStatus, jqXHR ]);
                    }
                }).fail(function(jqXHR, textStatus, errorThrown) {
                    o.jqXHR = jqXHR;
                    o.textStatus = textStatus;
                    o.errorThrown = errorThrown;
                    that._trigger("chunkfail", null, o);
                    that._trigger("chunkalways", null, o);
                    dfd.rejectWith(o.context, [ jqXHR, textStatus, errorThrown ]);
                }).always(function() {
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
                this._trigger("start");
                this._bitrateTimer = new this._BitrateTimer();
                this._progress.loaded = this._progress.total = 0;
                this._progress.bitrate = 0;
            }
            this._initResponseObject(data);
            this._initProgressObject(data);
            data._progress.loaded = data.loaded = data.uploadedBytes || 0;
            data._progress.total = data.total = this._getTotal(data.files) || 1;
            data._progress.bitrate = data.bitrate = 0;
            this._active += 1;
            this._progress.loaded += data.loaded;
            this._progress.total += data.total;
        },
        _onDone: function(result, textStatus, jqXHR, options) {
            var total = options._progress.total, response = options._response;
            if (options._progress.loaded < total) {
                this._onProgress($.Event("progress", {
                    lengthComputable: true,
                    loaded: total,
                    total: total
                }), options);
            }
            response.result = options.result = result;
            response.textStatus = options.textStatus = textStatus;
            response.jqXHR = options.jqXHR = jqXHR;
            this._trigger("done", null, options);
        },
        _onFail: function(jqXHR, textStatus, errorThrown, options) {
            var response = options._response;
            if (options.recalculateProgress) {
                this._progress.loaded -= options._progress.loaded;
                this._progress.total -= options._progress.total;
            }
            response.jqXHR = options.jqXHR = jqXHR;
            response.textStatus = options.textStatus = textStatus;
            response.errorThrown = options.errorThrown = errorThrown;
            this._trigger("fail", null, options);
        },
        _onAlways: function(jqXHRorResult, textStatus, jqXHRorError, options) {
            this._trigger("always", null, options);
        },
        _onSend: function(e, data) {
            if (!data.submit) {
                this._addConvenienceMethods(e, data);
            }
            var that = this, jqXHR, aborted, slot, pipe, options = that._getAJAXSettings(data), send = function() {
                that._sending += 1;
                options._bitrateTimer = new that._BitrateTimer();
                jqXHR = jqXHR || ((aborted || that._trigger("send", $.Event("send", {
                    delegatedEvent: e
                }), options) === false) && that._getXHRPromise(false, options.context, aborted) || that._chunkedUpload(options) || $.ajax(options)).done(function(result, textStatus, jqXHR) {
                    that._onDone(result, textStatus, jqXHR, options);
                }).fail(function(jqXHR, textStatus, errorThrown) {
                    that._onFail(jqXHR, textStatus, errorThrown, options);
                }).always(function(jqXHRorResult, textStatus, jqXHRorError) {
                    that._deinitProgressListener(options);
                    that._onAlways(jqXHRorResult, textStatus, jqXHRorError, options);
                    that._sending -= 1;
                    that._active -= 1;
                    if (options.limitConcurrentUploads && options.limitConcurrentUploads > that._sending) {
                        var nextSlot = that._slots.shift();
                        while (nextSlot) {
                            if (that._getDeferredState(nextSlot) === "pending") {
                                nextSlot.resolve();
                                break;
                            }
                            nextSlot = that._slots.shift();
                        }
                    }
                    if (that._active === 0) {
                        that._trigger("stop");
                    }
                });
                return jqXHR;
            };
            this._beforeSend(e, options);
            if (this.options.sequentialUploads || this.options.limitConcurrentUploads && this.options.limitConcurrentUploads <= this._sending) {
                if (this.options.limitConcurrentUploads > 1) {
                    slot = $.Deferred();
                    this._slots.push(slot);
                    pipe = slot[that._promisePipe](send);
                } else {
                    this._sequence = this._sequence[that._promisePipe](send, send);
                    pipe = this._sequence;
                }
                pipe.abort = function() {
                    aborted = [ undefined, "abort", "abort" ];
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
            var that = this, result = true, options = $.extend({}, this.options, data), files = data.files, filesLength = files.length, limit = options.limitMultiFileUploads, limitSize = options.limitMultiFileUploadSize, overhead = options.limitMultiFileUploadSizeOverhead, batchSize = 0, paramName = this._getParamName(options), paramNameSet, paramNameSlice, fileSet, i, j = 0;
            if (!filesLength) {
                return false;
            }
            if (limitSize && files[0].size === undefined) {
                limitSize = undefined;
            }
            if (!(options.singleFileUploads || limit || limitSize) || !this._isXHRUpload(options)) {
                fileSet = [ files ];
                paramNameSet = [ paramName ];
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
                    if (i + 1 === filesLength || batchSize + files[i + 1].size + overhead > limitSize || limit && i + 1 - j >= limit) {
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
                newData.files = fileSet ? element : [ element ];
                newData.paramName = paramNameSet[index];
                that._initResponseObject(newData);
                that._initProgressObject(newData);
                that._addConvenienceMethods(e, newData);
                result = that._trigger("add", $.Event("add", {
                    delegatedEvent: e
                }), newData);
                return result;
            });
            return result;
        },
        _replaceFileInput: function(data) {
            var input = data.fileInput, inputClone = input.clone(true), restoreFocus = input.is(document.activeElement);
            data.fileInputClone = inputClone;
            $("<form></form>").append(inputClone)[0].reset();
            input.after(inputClone).detach();
            if (restoreFocus) {
                inputClone.trigger("focus");
            }
            $.cleanData(input.off("remove"));
            this.options.fileInput = this.options.fileInput.map(function(i, el) {
                if (el === input[0]) {
                    return inputClone[0];
                }
                return el;
            });
            if (input[0] === this.element[0]) {
                this.element = inputClone;
            }
        },
        _handleFileTreeEntry: function(entry, path) {
            var that = this, dfd = $.Deferred(), entries = [], dirReader, errorHandler = function(e) {
                if (e && !e.entry) {
                    e.entry = entry;
                }
                dfd.resolve([ e ]);
            }, successHandler = function(entries) {
                that._handleFileTreeEntries(entries, path + entry.name + "/").done(function(files) {
                    dfd.resolve(files);
                }).fail(errorHandler);
            }, readEntries = function() {
                dirReader.readEntries(function(results) {
                    if (!results.length) {
                        successHandler(entries);
                    } else {
                        entries = entries.concat(results);
                        readEntries();
                    }
                }, errorHandler);
            };
            path = path || "";
            if (entry.isFile) {
                if (entry._file) {
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
                dfd.resolve([]);
            }
            return dfd.promise();
        },
        _handleFileTreeEntries: function(entries, path) {
            var that = this;
            return $.when.apply($, $.map(entries, function(entry) {
                return that._handleFileTreeEntry(entry, path);
            }))[this._promisePipe](function() {
                return Array.prototype.concat.apply([], arguments);
            });
        },
        _getDroppedFiles: function(dataTransfer) {
            dataTransfer = dataTransfer || {};
            var items = dataTransfer.items;
            if (items && items.length && (items[0].webkitGetAsEntry || items[0].getAsEntry)) {
                return this._handleFileTreeEntries($.map(items, function(item) {
                    var entry;
                    if (item.webkitGetAsEntry) {
                        entry = item.webkitGetAsEntry();
                        if (entry) {
                            entry._file = item.getAsFile();
                        }
                        return entry;
                    }
                    return item.getAsEntry();
                }));
            }
            return $.Deferred().resolve($.makeArray(dataTransfer.files)).promise();
        },
        _getSingleFileInputFiles: function(fileInput) {
            fileInput = $(fileInput);
            var entries = fileInput.prop("entries"), files, value;
            if (entries && entries.length) {
                return this._handleFileTreeEntries(entries);
            }
            files = $.makeArray(fileInput.prop("files"));
            if (!files.length) {
                value = fileInput.prop("value");
                if (!value) {
                    return $.Deferred().resolve([]).promise();
                }
                files = [ {
                    name: value.replace(/^.*\\/, "")
                } ];
            } else if (files[0].name === undefined && files[0].fileName) {
                $.each(files, function(index, file) {
                    file.name = file.fileName;
                    file.size = file.fileSize;
                });
            }
            return $.Deferred().resolve(files).promise();
        },
        _getFileInputFiles: function(fileInput) {
            if (!(fileInput instanceof $) || fileInput.length === 1) {
                return this._getSingleFileInputFiles(fileInput);
            }
            return $.when.apply($, $.map(fileInput, this._getSingleFileInputFiles))[this._promisePipe](function() {
                return Array.prototype.concat.apply([], arguments);
            });
        },
        _onChange: function(e) {
            var that = this, data = {
                fileInput: $(e.target),
                form: $(e.target.form)
            };
            this._getFileInputFiles(data.fileInput).always(function(files) {
                data.files = files;
                if (that.options.replaceFileInput) {
                    that._replaceFileInput(data);
                }
                if (that._trigger("change", $.Event("change", {
                    delegatedEvent: e
                }), data) !== false) {
                    that._onAdd(e, data);
                }
            });
        },
        _onPaste: function(e) {
            var items = e.originalEvent && e.originalEvent.clipboardData && e.originalEvent.clipboardData.items, data = {
                files: []
            };
            if (items && items.length) {
                $.each(items, function(index, item) {
                    var file = item.getAsFile && item.getAsFile();
                    if (file) {
                        data.files.push(file);
                    }
                });
                if (this._trigger("paste", $.Event("paste", {
                    delegatedEvent: e
                }), data) !== false) {
                    this._onAdd(e, data);
                }
            }
        },
        _onDrop: function(e) {
            e.dataTransfer = e.originalEvent && e.originalEvent.dataTransfer;
            var that = this, dataTransfer = e.dataTransfer, data = {};
            if (dataTransfer && dataTransfer.files && dataTransfer.files.length) {
                e.preventDefault();
                this._getDroppedFiles(dataTransfer).always(function(files) {
                    data.files = files;
                    if (that._trigger("drop", $.Event("drop", {
                        delegatedEvent: e
                    }), data) !== false) {
                        that._onAdd(e, data);
                    }
                });
            }
        },
        _onDragOver: getDragHandler("dragover"),
        _onDragEnter: getDragHandler("dragenter"),
        _onDragLeave: getDragHandler("dragleave"),
        _initEventHandlers: function() {
            if (this._isXHRUpload(this.options)) {
                this._on(this.options.dropZone, {
                    dragover: this._onDragOver,
                    drop: this._onDrop,
                    dragenter: this._onDragEnter,
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
            this._off(this.options.dropZone, "dragenter dragleave dragover drop");
            this._off(this.options.pasteZone, "paste");
            this._off(this.options.fileInput, "change");
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
                options.fileInput = this.element.is('input[type="file"]') ? this.element : this.element.find('input[type="file"]');
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
            var parts = str.split("/"), modifiers = parts.pop();
            parts.shift();
            return new RegExp(parts.join("/"), modifiers);
        },
        _isRegExpOption: function(key, value) {
            return key !== "url" && $.type(value) === "string" && /^\/.*\/[igm]{0,3}$/.test(value);
        },
        _initDataAttributes: function() {
            var that = this, options = this.options, data = this.element.data();
            $.each(this.element[0].attributes, function(index, attr) {
                var key = attr.name.toLowerCase(), value;
                if (/^data-/.test(key)) {
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
        active: function() {
            return this._active;
        },
        progress: function() {
            return this._progress;
        },
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
        send: function(data) {
            if (data && !this.options.disabled) {
                if (data.fileInput && !data.files) {
                    var that = this, dfd = $.Deferred(), promise = dfd.promise(), jqXHR, aborted;
                    promise.abort = function() {
                        aborted = true;
                        if (jqXHR) {
                            return jqXHR.abort();
                        }
                        dfd.reject(null, "abort", "abort");
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
                        jqXHR.then(function(result, textStatus, jqXHR) {
                            dfd.resolve(result, textStatus, jqXHR);
                        }, function(jqXHR, textStatus, errorThrown) {
                            dfd.reject(jqXHR, textStatus, errorThrown);
                        });
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

(function(factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        define([ "jquery", "./jquery.fileupload" ], factory);
    } else if (typeof exports === "object") {
        factory(require("jquery"), require("./jquery.fileupload"));
    } else {
        factory(window.jQuery);
    }
})(function($) {
    "use strict";
    var originalAdd = $.blueimp.yafFileUpload.prototype.options.add;
    $.widget("blueimp.yafFileUpload", $.blueimp.yafFileUpload, {
        options: {
            processQueue: [],
            add: function(e, data) {
                var $this = $(this);
                data.process(function() {
                    return $this.yafFileUpload("process", data);
                });
                originalAdd.call(this, e, data);
            }
        },
        processActions: {},
        _processFile: function(data, originalData) {
            var that = this, dfd = $.Deferred().resolveWith(that, [ data ]), chain = dfd.promise();
            this._trigger("process", null, data);
            $.each(data.processQueue, function(i, settings) {
                var func = function(data) {
                    if (originalData.errorThrown) {
                        return $.Deferred().rejectWith(that, [ originalData ]).promise();
                    }
                    return that.processActions[settings.action].call(that, data, settings);
                };
                chain = chain[that._promisePipe](func, settings.always && func);
            });
            chain.done(function() {
                that._trigger("processdone", null, data);
                that._trigger("processalways", null, data);
            }).fail(function() {
                that._trigger("processfail", null, data);
                that._trigger("processalways", null, data);
            });
            return chain;
        },
        _transformProcessQueue: function(options) {
            var processQueue = [];
            $.each(options.processQueue, function() {
                var settings = {}, action = this.action, prefix = this.prefix === true ? action : this.prefix;
                $.each(this, function(key, value) {
                    if ($.type(value) === "string" && value.charAt(0) === "@") {
                        settings[key] = options[value.slice(1) || (prefix ? prefix + key.charAt(0).toUpperCase() + key.slice(1) : key)];
                    } else {
                        settings[key] = value;
                    }
                });
                processQueue.push(settings);
            });
            options.processQueue = processQueue;
        },
        processing: function() {
            return this._processing;
        },
        process: function(data) {
            var that = this, options = $.extend({}, this.options, data);
            if (options.processQueue && options.processQueue.length) {
                this._transformProcessQueue(options);
                if (this._processing === 0) {
                    this._trigger("processstart");
                }
                $.each(data.files, function(index) {
                    var opts = index ? $.extend({}, options) : options, func = function() {
                        if (data.errorThrown) {
                            return $.Deferred().rejectWith(that, [ data ]).promise();
                        }
                        return that._processFile(opts, data);
                    };
                    opts.index = index;
                    that._processing += 1;
                    that._processingQueue = that._processingQueue[that._promisePipe](func, func).always(function() {
                        that._processing -= 1;
                        if (that._processing === 0) {
                            that._trigger("processstop");
                        }
                    });
                });
            }
            return this._processingQueue;
        },
        _create: function() {
            this._super();
            this._processing = 0;
            this._processingQueue = $.Deferred().resolveWith(this).promise();
        }
    });
});

(function(factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        define([ "jquery", "load-image", "load-image-meta", "load-image-scale", "load-image-exif", "load-image-orientation", "canvas-to-blob", "./jquery.fileupload-process" ], factory);
    } else if (typeof exports === "object") {
        factory(require("jquery"), require("blueimp-load-image/js/load-image"), require("blueimp-load-image/js/load-image-meta"), require("blueimp-load-image/js/load-image-scale"), require("blueimp-load-image/js/load-image-exif"), require("blueimp-load-image/js/load-image-orientation"), require("blueimp-canvas-to-blob"), require("./jquery.fileupload-process"));
    } else {
        factory(window.jQuery, window.loadImage);
    }
})(function($, loadImage) {
    "use strict";
    $.blueimp.yafFileUpload.prototype.options.processQueue.unshift({
        action: "loadImageMetaData",
        maxMetaDataSize: "@",
        disableImageHead: "@",
        disableMetaDataParsers: "@",
        disableExif: "@",
        disableExifOffsets: "@",
        includeExifTags: "@",
        excludeExifTags: "@",
        disableIptc: "@",
        disableIptcOffsets: "@",
        includeIptcTags: "@",
        excludeIptcTags: "@",
        disabled: "@disableImageMetaDataLoad"
    }, {
        action: "loadImage",
        prefix: true,
        fileTypes: "@",
        maxFileSize: "@",
        noRevoke: "@",
        disabled: "@disableImageLoad"
    }, {
        action: "resizeImage",
        prefix: "image",
        maxWidth: "@",
        maxHeight: "@",
        minWidth: "@",
        minHeight: "@",
        crop: "@",
        orientation: "@",
        forceResize: "@",
        disabled: "@disableImageResize",
        imageSmoothingQuality: "@imageSmoothingQuality"
    }, {
        action: "saveImage",
        quality: "@imageQuality",
        type: "@imageType",
        disabled: "@disableImageResize"
    }, {
        action: "saveImageMetaData",
        disabled: "@disableImageMetaDataSave"
    }, {
        action: "resizeImage",
        prefix: "preview",
        maxWidth: "@",
        maxHeight: "@",
        minWidth: "@",
        minHeight: "@",
        crop: "@",
        orientation: "@",
        thumbnail: "@",
        canvas: "@",
        disabled: "@disableImagePreview"
    }, {
        action: "setImage",
        name: "@imagePreviewName",
        disabled: "@disableImagePreview"
    }, {
        action: "deleteImageReferences",
        disabled: "@disableImageReferencesDeletion"
    });
    $.widget("blueimp.yafFileUpload", $.blueimp.yafFileUpload, {
        options: {
            loadImageFileTypes: /^image\/(gif|jpeg|png|svg\+xml)$/,
            loadImageMaxFileSize: 1e7,
            imageMaxWidth: 1920,
            imageMaxHeight: 1080,
            imageOrientation: true,
            imageCrop: false,
            disableImageResize: true,
            previewMaxWidth: 80,
            previewMaxHeight: 80,
            previewOrientation: true,
            previewThumbnail: true,
            previewCrop: false,
            previewCanvas: true
        },
        processActions: {
            loadImage: function(data, options) {
                if (options.disabled) {
                    return data;
                }
                var that = this, file = data.files[data.index], dfd = $.Deferred();
                if ($.type(options.maxFileSize) === "number" && file.size > options.maxFileSize || options.fileTypes && !options.fileTypes.test(file.type) || !loadImage(file, function(img) {
                    if (img.src) {
                        data.img = img;
                    }
                    dfd.resolveWith(that, [ data ]);
                }, options)) {
                    return data;
                }
                return dfd.promise();
            },
            resizeImage: function(data, options) {
                if (options.disabled || !(data.canvas || data.img)) {
                    return data;
                }
                options = $.extend({
                    canvas: true
                }, options);
                var that = this, dfd = $.Deferred(), img = options.canvas && data.canvas || data.img, resolve = function(newImg) {
                    if (newImg && (newImg.width !== img.width || newImg.height !== img.height || options.forceResize)) {
                        data[newImg.getContext ? "canvas" : "img"] = newImg;
                    }
                    data.preview = newImg;
                    dfd.resolveWith(that, [ data ]);
                }, thumbnail, thumbnailBlob;
                if (data.exif && options.thumbnail) {
                    thumbnail = data.exif.get("Thumbnail");
                    thumbnailBlob = thumbnail && thumbnail.get("Blob");
                    if (thumbnailBlob) {
                        options.orientation = data.exif.get("Orientation");
                        loadImage(thumbnailBlob, resolve, options);
                        return dfd.promise();
                    }
                }
                if (data.orientation) {
                    delete options.orientation;
                } else {
                    data.orientation = options.orientation || loadImage.orientation;
                }
                if (img) {
                    resolve(loadImage.scale(img, options, data));
                    return dfd.promise();
                }
                return data;
            },
            saveImage: function(data, options) {
                if (!data.canvas || options.disabled) {
                    return data;
                }
                var that = this, file = data.files[data.index], dfd = $.Deferred();
                if (data.canvas.toBlob) {
                    data.canvas.toBlob(function(blob) {
                        if (!blob.name) {
                            if (file.type === blob.type) {
                                blob.name = file.name;
                            } else if (file.name) {
                                blob.name = file.name.replace(/\.\w+$/, "." + blob.type.substr(6));
                            }
                        }
                        if (file.type !== blob.type) {
                            delete data.imageHead;
                        }
                        data.files[data.index] = blob;
                        dfd.resolveWith(that, [ data ]);
                    }, options.type || file.type, options.quality);
                } else {
                    return data;
                }
                return dfd.promise();
            },
            loadImageMetaData: function(data, options) {
                if (options.disabled) {
                    return data;
                }
                var that = this, dfd = $.Deferred();
                loadImage.parseMetaData(data.files[data.index], function(result) {
                    $.extend(data, result);
                    dfd.resolveWith(that, [ data ]);
                }, options);
                return dfd.promise();
            },
            saveImageMetaData: function(data, options) {
                if (!(data.imageHead && data.canvas && data.canvas.toBlob && !options.disabled)) {
                    return data;
                }
                var that = this, file = data.files[data.index], dfd = $.Deferred();
                if (data.orientation === true && data.exifOffsets) {
                    loadImage.writeExifData(data.imageHead, data, "Orientation", 1);
                }
                loadImage.replaceHead(file, data.imageHead, function(blob) {
                    blob.name = file.name;
                    data.files[data.index] = blob;
                    dfd.resolveWith(that, [ data ]);
                });
                return dfd.promise();
            },
            setImage: function(data, options) {
                if (data.preview && !options.disabled) {
                    data.files[data.index][options.name || "preview"] = data.preview;
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

(function(factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        define([ "jquery", "./jquery.fileupload-process" ], factory);
    } else if (typeof exports === "object") {
        factory(require("jquery"), require("./jquery.fileupload-process"));
    } else {
        factory(window.jQuery);
    }
})(function($) {
    "use strict";
    $.blueimp.yafFileUpload.prototype.options.processQueue.push({
        action: "validate",
        always: true,
        acceptFileTypes: "@",
        maxFileSize: "@",
        minFileSize: "@",
        maxNumberOfFiles: "@",
        disabled: "@disableValidation"
    });
    $.widget("blueimp.yafFileUpload", $.blueimp.yafFileUpload, {
        options: {
            getNumberOfFiles: $.noop,
            messages: {
                maxNumberOfFiles: "Maximum number of files exceeded",
                acceptFileTypes: "File type not allowed",
                maxFileSize: "File is too large",
                minFileSize: "File is too small"
            }
        },
        processActions: {
            validate: function(data, options) {
                if (options.disabled) {
                    return data;
                }
                var dfd = $.Deferred(), settings = this.options, file = data.files[data.index], fileSize;
                if (options.minFileSize || options.maxFileSize) {
                    fileSize = file.size;
                }
                if ($.type(options.maxNumberOfFiles) === "number" && (settings.getNumberOfFiles() || 0) + data.files.length > options.maxNumberOfFiles) {
                    file.error = settings.i18n("maxNumberOfFiles");
                } else if (options.acceptFileTypes && !options.acceptFileTypes.test(file.name)) {
                    file.error = settings.i18n("acceptFileTypes");
                } else if (fileSize > options.maxFileSize) {
                    file.error = settings.i18n("maxFileSize");
                } else if ($.type(fileSize) === "number" && fileSize < options.minFileSize) {
                    file.error = settings.i18n("minFileSize");
                } else {
                    delete file.error;
                }
                if (file.error || data.files.error) {
                    data.files.error = true;
                    dfd.rejectWith(this, [ data ]);
                } else {
                    dfd.resolveWith(this, [ data ]);
                }
                return dfd.promise();
            }
        }
    });
});

(function(factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        define([ "jquery", "blueimp-tmpl", "./jquery.fileupload-image", "./jquery.fileupload-audio", "./jquery.fileupload-video", "./jquery.fileupload-validate" ], factory);
    } else if (typeof exports === "object") {
        factory(require("jquery"), require("blueimp-tmpl"), require("./jquery.fileupload-image"), require("./jquery.fileupload-audio"), require("./jquery.fileupload-video"), require("./jquery.fileupload-validate"));
    } else {
        factory(window.jQuery, window.tmpl);
    }
})(function($, tmpl) {
    "use strict";
    $.blueimp.yafFileUpload.prototype._specialOptions.push("filesContainer", "uploadTemplateId", "downloadTemplateId");
    $.widget("blueimp.yafFileUpload", $.blueimp.yafFileUpload, {
        options: {
            autoUpload: false,
            showElementClass: "in",
            uploadTemplateId: "template-upload",
            downloadTemplateId: "template-download",
            filesContainer: undefined,
            prependFiles: false,
            dataType: "json",
            messages: {
                unknownError: "Unknown error"
            },
            getNumberOfFiles: function() {
                return this.filesContainer.children().not(".processing").length;
            },
            getFilesFromResponse: function(data) {
                if (data.result && $.isArray(data.result.files)) {
                    return data.result.files;
                }
                return [];
            },
            add: function(e, data) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                var $this = $(this), that = $this.data("blueimp-yafFileUpload") || $this.data("fileupload"), options = that.options;
                data.context = that._renderUpload(data.files).data("data", data).addClass("processing");
                options.filesContainer[options.prependFiles ? "prepend" : "append"](data.context);
                that._forceReflow(data.context);
                that._transition(data.context);
                data.process(function() {
                    return $this.yafFileUpload("process", data);
                }).always(function() {
                    data.context.each(function(index) {
                        $(this).find(".size").text(that._formatFileSize(data.files[index].size));
                    }).removeClass("processing");
                    that._renderPreviews(data);
                }).done(function() {
                    data.context.find(".edit,.start").prop("disabled", false);
                    if (that._trigger("added", e, data) !== false && (options.autoUpload || data.autoUpload) && data.autoUpload !== false) {
                        data.submit();
                    }
                }).fail(function() {
                    if (data.files.error) {
                        data.context.each(function(index) {
                            var error = data.files[index].error;
                            if (error) {
                                $(this).find(".error").text(error);
                            }
                        });
                    }
                });
            },
            send: function(e, data) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                var that = $(this).data("blueimp-yafFileUpload") || $(this).data("fileupload");
                if (data.context && data.dataType && data.dataType.substr(0, 6) === "iframe") {
                    data.context.find(".progress").addClass(!$.support.transition && "progress-animated").attr("aria-valuenow", 100).children().first().css("width", "100%");
                }
                return that._trigger("sent", e, data);
            },
            done: function(e, data) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                var that = $(this).data("blueimp-yafFileUpload") || $(this).data("fileupload"), getFilesFromResponse = data.getFilesFromResponse || that.options.getFilesFromResponse, files = getFilesFromResponse(data), template, deferred;
                if (data.context) {
                    data.context.each(function(index) {
                        var file = files[index] || {
                            error: "Empty file upload result"
                        };
                        deferred = that._addFinishedDeferreds();
                        that._transition($(this)).done(function() {
                            var node = $(this);
                            template = that._renderDownload([ file ]).replaceAll(node);
                            that._forceReflow(template);
                            that._transition(template).done(function() {
                                data.context = $(this);
                                that._trigger("completed", e, data);
                                that._trigger("finished", e, data);
                                deferred.resolve();
                            });
                        });
                    });
                } else {
                    template = that._renderDownload(files)[that.options.prependFiles ? "prependTo" : "appendTo"](that.options.filesContainer);
                    that._forceReflow(template);
                    deferred = that._addFinishedDeferreds();
                    that._transition(template).done(function() {
                        data.context = $(this);
                        that._trigger("completed", e, data);
                        that._trigger("finished", e, data);
                        deferred.resolve();
                    });
                }
            },
            fail: function(e, data) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                var that = $(this).data("blueimp-yafFileUpload") || $(this).data("fileupload"), template, deferred;
                if (data.context) {
                    data.context.each(function(index) {
                        if (data.errorThrown !== "abort") {
                            var file = data.files[index];
                            file.error = file.error || data.errorThrown || data.i18n("unknownError");
                            deferred = that._addFinishedDeferreds();
                            that._transition($(this)).done(function() {
                                var node = $(this);
                                template = that._renderDownload([ file ]).replaceAll(node);
                                that._forceReflow(template);
                                that._transition(template).done(function() {
                                    data.context = $(this);
                                    that._trigger("failed", e, data);
                                    that._trigger("finished", e, data);
                                    deferred.resolve();
                                });
                            });
                        } else {
                            deferred = that._addFinishedDeferreds();
                            that._transition($(this)).done(function() {
                                $(this).remove();
                                that._trigger("failed", e, data);
                                that._trigger("finished", e, data);
                                deferred.resolve();
                            });
                        }
                    });
                } else if (data.errorThrown !== "abort") {
                    data.context = that._renderUpload(data.files)[that.options.prependFiles ? "prependTo" : "appendTo"](that.options.filesContainer).data("data", data);
                    that._forceReflow(data.context);
                    deferred = that._addFinishedDeferreds();
                    that._transition(data.context).done(function() {
                        data.context = $(this);
                        that._trigger("failed", e, data);
                        that._trigger("finished", e, data);
                        deferred.resolve();
                    });
                } else {
                    that._trigger("failed", e, data);
                    that._trigger("finished", e, data);
                    that._addFinishedDeferreds().resolve();
                }
            },
            progress: function(e, data) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                var progress = Math.floor(data.loaded / data.total * 100);
                if (data.context) {
                    data.context.each(function() {
                        $(this).find(".progress").attr("aria-valuenow", progress).children().first().css("width", progress + "%");
                    });
                }
            },
            progressall: function(e, data) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                var $this = $(this), progress = Math.floor(data.loaded / data.total * 100), globalProgressNode = $this.find(".fileupload-progress"), extendedProgressNode = globalProgressNode.find(".progress-extended");
                if (extendedProgressNode.length) {
                    extendedProgressNode.html(($this.data("blueimp-yafFileUpload") || $this.data("fileupload"))._renderExtendedProgress(data));
                }
                globalProgressNode.find(".progress").attr("aria-valuenow", progress).children().first().css("width", progress + "%");
            },
            start: function(e) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                var that = $(this).data("blueimp-yafFileUpload") || $(this).data("fileupload");
                that._resetFinishedDeferreds();
                that._transition($(this).find(".fileupload-progress")).done(function() {
                    that._trigger("started", e);
                });
            },
            stop: function(e) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                var that = $(this).data("blueimp-yafFileUpload") || $(this).data("fileupload"), deferred = that._addFinishedDeferreds();
                $.when.apply($, that._getFinishedDeferreds()).done(function() {
                    that._trigger("stopped", e);
                });
                that._transition($(this).find(".fileupload-progress")).done(function() {
                    $(this).find(".progress").attr("aria-valuenow", "0").children().first().css("width", "0%");
                    $(this).find(".progress-extended").html("&nbsp;");
                    deferred.resolve();
                });
            },
            processstart: function(e) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                $(this).addClass("fileupload-processing");
            },
            processstop: function(e) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                $(this).removeClass("fileupload-processing");
            },
            destroy: function(e, data) {
                if (e.isDefaultPrevented()) {
                    return false;
                }
                var that = $(this).data("blueimp-yafFileUpload") || $(this).data("fileupload"), removeNode = function() {
                    that._transition(data.context).done(function() {
                        $(this).remove();
                        that._trigger("destroyed", e, data);
                    });
                };
                if (data.url) {
                    data.dataType = data.dataType || that.options.dataType;
                    $.ajax(data).done(removeNode).fail(function() {
                        that._trigger("destroyfailed", e, data);
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
            var promise = deferred || $.Deferred();
            this._finishedUploads.push(promise);
            return promise;
        },
        _getFinishedDeferreds: function() {
            return this._finishedUploads;
        },
        _enableDragToDesktop: function() {
            var link = $(this), url = link.prop("href"), name = link.prop("download"), type = "application/octet-stream";
            link.on("dragstart", function(e) {
                try {
                    e.originalEvent.dataTransfer.setData("DownloadURL", [ type, name, url ].join(":"));
                } catch (ignore) {}
            });
        },
        _formatFileSize: function(bytes) {
            if (typeof bytes !== "number") {
                return "";
            }
            if (bytes >= 1e9) {
                return (bytes / 1e9).toFixed(2) + " GB";
            }
            if (bytes >= 1e6) {
                return (bytes / 1e6).toFixed(2) + " MB";
            }
            return (bytes / 1e3).toFixed(2) + " KB";
        },
        _formatBitrate: function(bits) {
            if (typeof bits !== "number") {
                return "";
            }
            if (bits >= 1e9) {
                return (bits / 1e9).toFixed(2) + " Gbit/s";
            }
            if (bits >= 1e6) {
                return (bits / 1e6).toFixed(2) + " Mbit/s";
            }
            if (bits >= 1e3) {
                return (bits / 1e3).toFixed(2) + " kbit/s";
            }
            return bits.toFixed(2) + " bit/s";
        },
        _formatTime: function(seconds) {
            var date = new Date(seconds * 1e3), days = Math.floor(seconds / 86400);
            days = days ? days + "d " : "";
            return days + ("0" + date.getUTCHours()).slice(-2) + ":" + ("0" + date.getUTCMinutes()).slice(-2) + ":" + ("0" + date.getUTCSeconds()).slice(-2);
        },
        _formatPercentage: function(floatValue) {
            return (floatValue * 100).toFixed(2) + " %";
        },
        _renderExtendedProgress: function(data) {
            return this._formatBitrate(data.bitrate) + " | " + this._formatTime((data.total - data.loaded) * 8 / data.bitrate) + " | " + this._formatPercentage(data.loaded / data.total) + " | " + this._formatFileSize(data.loaded) + " / " + this._formatFileSize(data.total);
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
            return $(this.options.templatesContainer).html(result).children();
        },
        _renderPreviews: function(data) {
            data.context.find(".preview").each(function(index, elm) {
                $(elm).empty().append(data.files[index].preview);
            });
        },
        _renderUpload: function(files) {
            return this._renderTemplate(this.options.uploadTemplate, files);
        },
        _renderDownload: function(files) {
            return this._renderTemplate(this.options.downloadTemplate, files).find("a[download]").each(this._enableDragToDesktop).end();
        },
        _editHandler: function(e) {
            e.preventDefault();
            if (!this.options.edit) return;
            var that = this, button = $(e.currentTarget), template = button.closest(".template-upload"), data = template.data("data"), index = button.data().index;
            this.options.edit(data.files[index]).then(function(file) {
                if (!file) return;
                data.files[index] = file;
                data.context.addClass("processing");
                template.find(".edit,.start").prop("disabled", true);
                $(that.element).fileupload("process", data).always(function() {
                    template.find(".size").text(that._formatFileSize(data.files[index].size));
                    data.context.removeClass("processing");
                    that._renderPreviews(data);
                }).done(function() {
                    template.find(".edit,.start").prop("disabled", false);
                }).fail(function() {
                    template.find(".edit").prop("disabled", false);
                    var error = data.files[index].error;
                    if (error) {
                        template.find(".error").text(error);
                    }
                });
            });
        },
        _startHandler: function(e) {
            e.preventDefault();
            var button = $(e.currentTarget), template = button.closest(".template-upload"), data = template.data("data");
            button.prop("disabled", true);
            if (data && data.submit) {
                data.submit();
            }
        },
        _cancelHandler: function(e) {
            e.preventDefault();
            var template = $(e.currentTarget).closest(".template-upload,.template-download"), data = template.data("data") || {};
            data.context = data.context || template;
            if (data.abort) {
                data.abort();
            } else {
                data.errorThrown = "abort";
                this._trigger("fail", e, data);
            }
        },
        _deleteHandler: function(e) {
            e.preventDefault();
            var button = $(e.currentTarget);
            this._trigger("destroy", e, $.extend({
                context: button.closest(".template-download"),
                type: "DELETE"
            }, button.data()));
        },
        _forceReflow: function(node) {
            return $.support.transition && node.length && node[0].offsetWidth;
        },
        _transition: function(node) {
            var dfd = $.Deferred();
            if ($.support.transition && node.hasClass("fade") && node.is(":visible")) {
                var transitionEndHandler = function(e) {
                    if (e.target === node[0]) {
                        node.off($.support.transition.end, transitionEndHandler);
                        dfd.resolveWith(node);
                    }
                };
                node.on($.support.transition.end, transitionEndHandler).toggleClass(this.options.showElementClass);
            } else {
                node.toggleClass(this.options.showElementClass);
                dfd.resolveWith(node);
            }
            return dfd;
        },
        _initButtonBarEventHandlers: function() {
            var fileUploadButtonBar = this.element.find(".fileupload-buttonbar"), filesList = this.options.filesContainer;
            this._on(fileUploadButtonBar.find(".start"), {
                click: function(e) {
                    e.preventDefault();
                    filesList.find(".start").trigger("click");
                }
            });
            this._on(fileUploadButtonBar.find(".cancel"), {
                click: function(e) {
                    e.preventDefault();
                    filesList.find(".cancel").trigger("click");
                }
            });
            this._on(fileUploadButtonBar.find(".delete"), {
                click: function(e) {
                    e.preventDefault();
                    filesList.find(".toggle:checked").closest(".template-download").find(".delete").trigger("click");
                    fileUploadButtonBar.find(".toggle").prop("checked", false);
                }
            });
            this._on(fileUploadButtonBar.find(".toggle"), {
                change: function(e) {
                    filesList.find(".toggle").prop("checked", $(e.currentTarget).is(":checked"));
                }
            });
        },
        _destroyButtonBarEventHandlers: function() {
            this._off(this.element.find(".fileupload-buttonbar").find(".start, .cancel, .delete"), "click");
            this._off(this.element.find(".fileupload-buttonbar .toggle"), "change.");
        },
        _initEventHandlers: function() {
            this._super();
            this._on(this.options.filesContainer, {
                "click .edit": this._editHandler,
                "click .start": this._startHandler,
                "click .cancel": this._cancelHandler,
                "click .delete": this._deleteHandler
            });
            this._initButtonBarEventHandlers();
        },
        _destroyEventHandlers: function() {
            this._destroyButtonBarEventHandlers();
            this._off(this.options.filesContainer, "click");
            this._super();
        },
        _enableFileInputButton: function() {
            this.element.find(".fileinput-button input").prop("disabled", false).parent().removeClass("disabled");
        },
        _disableFileInputButton: function() {
            this.element.find(".fileinput-button input").prop("disabled", true).parent().addClass("disabled");
        },
        _initTemplates: function() {
            var options = this.options;
            options.templatesContainer = this.document[0].createElement(options.filesContainer.prop("nodeName"));
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
                options.filesContainer = this.element.find(".files");
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
                this.element.find("input, button").prop("disabled", false);
                this._enableFileInputButton();
            }
        },
        disable: function() {
            if (!this.options.disabled) {
                this.element.find("input, button").prop("disabled", true);
                this._disableFileInputButton();
            }
            this._super();
        }
    });
});