var XRegExp;
if (XRegExp) {
    throw Error("can't load XRegExp twice in the same frame");
} (function () {
    XRegExp = function (b, c) {
        var d = [],
            currScope = XRegExp.OUTSIDE_CLASS,
            pos = 0,
            context, tokenResult, match, chr, regex;
        if (XRegExp.isRegExp(b)) {
            if (c !== undefined) throw TypeError("can't supply flags when constructing one RegExp from another");
            return clone(b);
        }
        if (isInsideConstructor) throw Error("can't call the XRegExp constructor within token definition functions");
        c = c || "";
        context = {
            hasNamedCapture: false,
            captureNames: [],
            hasFlag: function (a) {
                return c.indexOf(a) > -1;
            },
            setFlag: function (a) {
                c += a;
            }
        };
        while (pos < b.length) {
            tokenResult = runTokens(b, pos, currScope, context);
            if (tokenResult) {
                d.push(tokenResult.output);
                pos += (tokenResult.match[0].length || 1);
            } else {
                if (match = real.exec.call(nativeTokens[currScope], b.slice(pos))) {
                    d.push(match[0]);
                    pos += match[0].length;
                } else {
                    chr = b.charAt(pos);
                    if (chr === "[") currScope = XRegExp.INSIDE_CLASS;
                    else if (chr === "]") currScope = XRegExp.OUTSIDE_CLASS;
                    d.push(chr);
                    pos++;
                }
            }
        }
        regex = RegExp(d.join(""), real.replace.call(c, flagClip, ""));
        regex._xregexp = {
            source: b,
            captureNames: context.hasNamedCapture ? context.captureNames : null
        };
        return regex;
    };
    XRegExp.version = "1.5.0";
    XRegExp.INSIDE_CLASS = 1;
    XRegExp.OUTSIDE_CLASS = 2;
    var j = /\$(?:(\d\d?|[$&`'])|{([$\w]+)})/g,
        flagClip = /[^gimy]+|([\s\S])(?=[\s\S]*\1)/g,
        quantifier = /^(?:[?*+]|{\d+(?:,\d*)?})\??/,
        isInsideConstructor = false,
        tokens = [],
        real = {
            exec: RegExp.prototype.exec,
            test: RegExp.prototype.test,
            match: String.prototype.match,
            replace: String.prototype.replace,
            split: String.prototype.split
        }, compliantExecNpcg = real.exec.call(/()??/, "")[1] === undefined,
        compliantLastIndexIncrement = function () {
            var x = /^/g;
            real.test.call(x, "");
            return !x.lastIndex;
        }(),
        compliantLastIndexReset = function () {
            var x = /x/g;
            real.replace.call("x", x, "");
            return !x.lastIndex;
        }(),
        hasNativeY = RegExp.prototype.sticky !== undefined,
        nativeTokens = {};
    nativeTokens[XRegExp.INSIDE_CLASS] = /^(?:\\(?:[0-3][0-7]{0,2}|[4-7][0-7]?|x[\dA-Fa-f]{2}|u[\dA-Fa-f]{4}|c[A-Za-z]|[\s\S]))/;
    nativeTokens[XRegExp.OUTSIDE_CLASS] = /^(?:\\(?:0(?:[0-3][0-7]{0,2}|[4-7][0-7]?)?|[1-9]\d*|x[\dA-Fa-f]{2}|u[\dA-Fa-f]{4}|c[A-Za-z]|[\s\S])|\(\?[:=!]|[?*+]\?|{\d+(?:,\d*)?}\??)/;
    XRegExp.addToken = function (a, b, c, d) {
        tokens.push({
            pattern: clone(a, "g" + (hasNativeY ? "y" : "")),
            handler: b,
            scope: c || XRegExp.OUTSIDE_CLASS,
            trigger: d || null
        });
    };
    XRegExp.cache = function (a, b) {
        var c = a + "/" + (b || "");
        return XRegExp.cache[c] || (XRegExp.cache[c] = XRegExp(a, b));
    };
    XRegExp.copyAsGlobal = function (a) {
        return clone(a, "g");
    };
    XRegExp.escape = function (a) {
        return a.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
    };
    XRegExp.execAt = function (a, b, c, d) {
        b = clone(b, "g" + ((d && hasNativeY) ? "y" : ""));
        b.lastIndex = c = c || 0;
        var e = b.exec(a);
        if (d) return (e && e.index === c) ? e : null;
        else return e;
    };
    XRegExp.freezeTokens = function () {
        XRegExp.addToken = function () {
            throw Error("can't run addToken after freezeTokens");
        };
    };
    XRegExp.isRegExp = function (o) {
        return Object.prototype.toString.call(o) === "[object RegExp]";
    };
    XRegExp.iterate = function (a, b, c, d) {
        var e = clone(b, "g"),
            i = -1,
            match;
        while (match = e.exec(a)) {
            c.call(d, match, ++i, a, e);
            if (e.lastIndex === match.index) e.lastIndex++;
        }
        if (b.global) b.lastIndex = 0;
    };
    XRegExp.matchChain = function (e, f) {
        return function recurseChain(b, c) {
            var d = f[c].regex ? f[c] : {
                regex: f[c]
            }, regex = clone(d.regex, "g"),
                matches = [],
                i;
            for (i = 0; i < b.length; i++) {
                XRegExp.iterate(b[i], regex, function (a) {
                    matches.push(d.backref ? (a[d.backref] || "") : a[0]);
                });
            }
            return ((c === f.length - 1) || !matches.length) ? matches : recurseChain(matches, c + 1);
        }([e], 0);
    };
    RegExp.prototype.apply = function (a, b) {
        return this.exec(b[0]);
    };
    RegExp.prototype.call = function (a, b) {
        return this.exec(b);
    };
    RegExp.prototype.exec = function (a) {
        var b = real.exec.apply(this, arguments),
            name, r2;
        if (b) {
            if (!compliantExecNpcg && b.length > 1 && indexOf(b, "") > -1 && a) {
                r2 = RegExp(this.source, real.replace.call(getNativeFlags(this), "g", ""));
                real.replace.call(a.toString().slice(b.index), r2, function () {
                    for (var i = 1; i < arguments.length - 2; i++) {
                        if (arguments[i] === undefined) b[i] = undefined;
                    }
                });
            }
            if (this._xregexp && this._xregexp.captureNames) {
                for (var i = 1; i < b.length; i++) {
                    name = this._xregexp.captureNames[i - 1];
                    if (name) b[name] = b[i];
                }
            }
            if (!compliantLastIndexIncrement && this.global && !b[0].length && (this.lastIndex > b.index)) this.lastIndex--;
        }
        return b;
    };
    if (!compliantLastIndexIncrement) {
        RegExp.prototype.test = function (a) {
            var b = real.exec.call(this, a);
            if (b && this.global && !b[0].length && (this.lastIndex > b.index)) this.lastIndex--;
            return !!b;
        };
    }
    String.prototype.match = function (a) {
        if (!XRegExp.isRegExp(a)) a = RegExp(a);
        if (a.global) {
            var b = real.match.apply(this, arguments);
            a.lastIndex = 0;
            return b;
        }
        return a.exec(this);
    };
    String.prototype.replace = function (f, g) {
        var h = XRegExp.isRegExp(f),
            captureNames, result, str;
        if (h && typeof g.valueOf() === "string" && g.indexOf("${") === -1 && compliantLastIndexReset) return real.replace.apply(this, arguments);
        if (!h) f = f + "";
        else if (f._xregexp) captureNames = f._xregexp.captureNames;
        if (typeof g === "function") {
            result = real.replace.call(this, f, function () {
                if (captureNames) {
                    arguments[0] = new String(arguments[0]);
                    for (var i = 0; i < captureNames.length; i++) {
                        if (captureNames[i]) arguments[0][captureNames[i]] = arguments[i + 1];
                    }
                }
                if (h && f.global) f.lastIndex = arguments[arguments.length - 2] + arguments[0].length;
                return g.apply(null, arguments);
            });
        } else {
            str = this + "";
            result = real.replace.call(str, f, function () {
                var e = arguments;
                return real.replace.call(g, j, function (a, b, c) {
                    if (b) {
                        switch (b) {
                            case "$":
                                return "$";
                            case "&":
                                return e[0];
                            case "`":
                                return e[e.length - 1].slice(0, e[e.length - 2]);
                            case "'":
                                return e[e.length - 1].slice(e[e.length - 2] + e[0].length);
                            default:
                                var d = "";
                                b = +b;
                                if (!b) return a;
                                while (b > e.length - 3) {
                                    d = String.prototype.slice.call(b, -1) + d;
                                    b = Math.floor(b / 10);
                                }
                                return (b ? e[b] || "" : "$") + d;
                        }
                    } else {
                        var n = +c;
                        if (n <= e.length - 3) return e[n];
                        n = captureNames ? indexOf(captureNames, c) : -1;
                        return n > -1 ? e[n + 1] : a;
                    }
                });
            });
        } if (h && f.global) f.lastIndex = 0;
        return result;
    };
    String.prototype.split = function (s, a) {
        if (!XRegExp.isRegExp(s)) return real.split.apply(this, arguments);
        var b = this + "",
            output = [],
            lastLastIndex = 0,
            match, lastLength;
        if (a === undefined || +a < 0) {
            a = Infinity;
        } else {
            a = Math.floor(+a);
            if (!a) return [];
        }
        s = XRegExp.copyAsGlobal(s);
        while (match = s.exec(b)) {
            if (s.lastIndex > lastLastIndex) {
                output.push(b.slice(lastLastIndex, match.index));
                if (match.length > 1 && match.index < b.length) Array.prototype.push.apply(output, match.slice(1));
                lastLength = match[0].length;
                lastLastIndex = s.lastIndex;
                if (output.length >= a) break;
            }
            if (s.lastIndex === match.index) s.lastIndex++;
        }
        if (lastLastIndex === b.length) {
            if (!real.test.call(s, "") || lastLength) output.push("");
        } else {
            output.push(b.slice(lastLastIndex));
        }
        return output.length > a ? output.slice(0, a) : output;
    };
    function clone(a, b) {
        if (!XRegExp.isRegExp(a)) throw TypeError("type RegExp expected");
        var x = a._xregexp;
        a = XRegExp(a.source, getNativeFlags(a) + (b || ""));
        if (x) {
            a._xregexp = {
                source: x.source,
                captureNames: x.captureNames ? x.captureNames.slice(0) : null
            };
        }
        return a;
    };
    function getNativeFlags(a) {
        return (a.global ? "g" : "") + (a.ignoreCase ? "i" : "") + (a.multiline ? "m" : "") + (a.extended ? "x" : "") + (a.sticky ? "y" : "");
    };
    function runTokens(a, b, c, d) {
        var i = tokens.length,
            result, match, t;
        isInsideConstructor = true;
        try {
            while (i--) {
                t = tokens[i];
                if ((c & t.scope) && (!t.trigger || t.trigger.call(d))) {
                    t.pattern.lastIndex = b;
                    match = t.pattern.exec(a);
                    if (match && match.index === b) {
                        result = {
                            output: t.handler.call(d, match, c),
                            match: match
                        };
                        break;
                    }
                }
            }
        } catch (err) {
            throw err;
        } finally {
            isInsideConstructor = false;
        }
        return result;
    };
    function indexOf(a, b, c) {
        if (Array.prototype.indexOf) return a.indexOf(b, c);
        for (var i = c || 0; i < a.length; i++) {
            if (a[i] === b) return i;
        }
        return -1;
    };
    XRegExp.addToken(/\(\?#[^)]*\)/, function (a) {
        return real.test.call(quantifier, a.input.slice(a.index + a[0].length)) ? "" : "(?:)";
    });
    XRegExp.addToken(/\((?!\?)/, function () {
        this.captureNames.push(null);
        return "(";
    });
    XRegExp.addToken(/\(\?<([$\w]+)>/, function (a) {
        this.captureNames.push(a[1]);
        this.hasNamedCapture = true;
        return "(";
    });
    XRegExp.addToken(/\\k<([\w$]+)>/, function (a) {
        var b = indexOf(this.captureNames, a[1]);
        return b > -1 ? "\\" + (b + 1) + (isNaN(a.input.charAt(a.index + a[0].length)) ? "" : "(?:)") : a[0];
    });
    XRegExp.addToken(/\[\^?]/, function (a) {
        return a[0] === "[]" ? "\\b\\B" : "[\\s\\S]";
    });
    XRegExp.addToken(/^\(\?([imsx]+)\)/, function (a) {
        this.setFlag(a[1]);
        return "";
    });
    XRegExp.addToken(/(?:\s+|#.*)+/, function (a) {
        return real.test.call(quantifier, a.input.slice(a.index + a[0].length)) ? "" : "(?:)";
    }, XRegExp.OUTSIDE_CLASS, function () {
        return this.hasFlag("x");
    });
    XRegExp.addToken(/\./, function () {
        return "[\\s\\S]";
    }, XRegExp.OUTSIDE_CLASS, function () {
        return this.hasFlag("s");
    });
})();
typeof (exports) != 'undefined' ? exports.XRegExp = XRegExp : null;


if (typeof SyntaxHighlighter == "undefined") var SyntaxHighlighter = function () {
    function r(a, b) {
        a.className.indexOf(b) != -1 || (a.className += " " + b);
    }
    function t(a) {
        return a.indexOf("highlighter_") == 0 ? a : "highlighter_" + a;
    }
    function B(a) {
        return f.vars.highlighters[t(a)];
    }
    function p(a, b, c) {
        if (a == null) return null;
        var d = c != true ? a.childNodes : [a.parentNode],
            h = {
                "#": "id",
                ".": "className"
            }[b.substr(0, 1)] || "nodeName",
            g, i;
        g = h != "nodeName" ? b.substr(1) : b.toUpperCase();
        if ((a[h] || "").indexOf(g) != -1) return a;
        for (a = 0; d && a < d.length && i == null; a++) i = p(d[a], b, c);
        return i;
    }
    function C(a, b) {
        var c = {}, d;
        for (d in a) c[d] = a[d];
        for (d in b) c[d] = b[d];
        return c;
    }
    function w(a, b, c, d) {
        function h(g) {
            g = g || window.event;
            if (!g.target) {
                g.target = g.srcElement;
                g.preventDefault = function () {
                    this.returnValue = false;
                };
            }
            c.call(d || window, g);
        }
        a.attachEvent ? a.attachEvent("on" + b, h) : a.addEventListener(b, h, false);
    }
    function A(a, b) {
        var c = f.vars.discoveredBrushes,
            d = null;
        if (c == null) {
            c = {};
            for (var h in f.brushes) {
                var g = f.brushes[h];
                d = g.aliases;
                if (d != null) {
                    g.brushName = h.toLowerCase();
                    for (g = 0; g < d.length; g++) c[d[g]] = h;
                }
            }
            f.vars.discoveredBrushes = c;
        }
        d = f.brushes[c[a]];
        d == null && b != false && window.alert(f.config.strings.alert + (f.config.strings.noBrush + a));
        return d;
    }
    function v(a, b) {
        for (var c = a.split("\n"), d = 0; d < c.length; d++) c[d] = b(c[d], d);
        return c.join("\n");
    }
    function u(a, b) {
        if (a == null || a.length == 0 || a == "\n") return a;
        a = a.replace(/</g, "&lt;");
        a = a.replace(/ {2,}/g, function (c) {
            for (var d = "", h = 0; h < c.length - 1; h++) d += f.config.space;
            return d + " ";
        });
        if (b != null) a = v(a, function (c) {
            if (c.length == 0) return "";
            var d = "";
            c = c.replace(/^(&nbsp;| )+/, function (h) {
                d = h;
                return "";
            });
            if (c.length == 0) return d;
            return d + '<code class="' + b + '">' + c + "</code>";
        });
        return a;
    }
    function n(a, b) {
        a.split("\n");
        for (var c = "", d = 0; d < 50; d++) c += "                    ";
        return a = v(a, function (h) {
            if (h.indexOf("\t") == -1) return h;
            for (var g = 0;
            (g = h.indexOf("\t")) != -1;) h = h.substr(0, g) + c.substr(0, b - g % b) + h.substr(g + 1, h.length);
            return h;
        });
    }
    function x(a) {
        return a.replace(/^\s+|\s+$/g, "");
    }
    function D(a, b) {
        if (a.index < b.index) return -1;
        else if (a.index > b.index) return 1;
        else if (a.length < b.length) return -1;
        else if (a.length > b.length) return 1;
        return 0;
    }
    function y(a, b) {
        function c(k) {
            return k[0];
        }
        for (var d = null, h = [], g = b.func ? b.func : c;
        (d = b.regex.exec(a)) != null;) {
            var i = g(d, b);
            if (typeof i == "string") i = [new f.Match(i, d.index, b.css)];
            h = h.concat(i);
        }
        return h;
    }
    function E(a) {
        var b = /(.*)((&gt;|&lt;).*)/;
        return a.replace(f.regexLib.url, function (c) {
            var d = "",
                h = null;
            if (h = b.exec(c)) {
                c = h[1];
                d = h[2];
            }
            return '<a href="' + c + '">' + c + "</a>" + d;
        });
    }
    function z() {
        for (var a = document.getElementsByTagName("script"), b = [], c = 0; c < a.length; c++) a[c].type == "syntaxhighlighter" && b.push(a[c]);
        return b;
    }
    function e(a) {
        a = a.target;
        var b = p(a, ".syntaxhighlighter", true);
        a = p(a, ".container", true);
        var c = document.createElement("textarea");
        if (!(!a || !b || p(a, "textarea"))) {
            B(b.id);
            r(b, "source");
            for (var d = a.childNodes, h = [], g = 0; g < d.length; g++) h.push(d[g].innerText || d[g].textContent);
            h = h.join("\r");
            c.appendChild(document.createTextNode(h));
            a.appendChild(c);
            c.focus();
            c.select();
            w(c, "blur", function () {
                c.parentNode.removeChild(c);
                b.className = b.className.replace("source", "");
            });
        }
    }
    if (typeof require != "undefined" && typeof XRegExp == "undefined") XRegExp = require("XRegExp").XRegExp;
    var f = {
        defaults: {
            "class-name": "",
            "first-line": 1,
            "pad-line-numbers": false,
            highlight: null,
            title: null,
            "smart-tabs": true,
            "tab-size": 4,
            gutter: true,
            toolbar: true,
            "quick-code": true,
            collapse: false,
            "auto-links": true,
            light: false,
            "html-script": false
        },
        config: {
            space: "&nbsp;",
            useScriptTags: true,
            bloggerMode: false,
            stripBrs: false,
            tagName: "pre",
            strings: {
                expandSource: "expand source",
                help: "?",
                alert: "SyntaxHighlighter\n\n",
                noBrush: "Can't find brush for: ",
                brushNotHtmlScript: "Brush wasn't configured for html-script option: ",
                aboutDialog: '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml"><head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>About SyntaxHighlighter</title></head><body style="font-family:Geneva,Arial,Helvetica,sans-serif;background-color:#fff;color:#000;font-size:1em;text-align:center;"><div style="text-align:center;margin-top:1.5em;"><div style="font-size:xx-large;">SyntaxHighlighter</div><div style="font-size:.75em;margin-bottom:3em;"><div>version 3.0.87 (November 12 2010)</div><div><a href="http://alexgorbatchev.com/SyntaxHighlighter" target="_blank" style="color:#005896">http://alexgorbatchev.com/SyntaxHighlighter</a></div><div>JavaScript code syntax highlighter.</div><div>Copyright 2004-2010 Alex Gorbatchev.</div></div><div>If you like this script, please <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2930402" style="color:#005896">donate</a> to <br/>keep development active!</div></div></body></html>'
            }
        },
        vars: {
            discoveredBrushes: null,
            highlighters: {}
        },
        brushes: {},
        regexLib: {
            multiLineCComments: /\/\*[\s\S]*?\*\//gm,
            singleLineCComments: /\/\/.*$/gm,
            singleLinePerlComments: /#.*$/gm,
            doubleQuotedString: /"([^\\"\n]|\\.)*"/g,
            singleQuotedString: /'([^\\'\n]|\\.)*'/g,
            multiLineDoubleQuotedString: new XRegExp('"([^\\\\"]|\\\\.)*"', "gs"),
            multiLineSingleQuotedString: new XRegExp("'([^\\\\']|\\\\.)*'", "gs"),
            xmlComments: /(&lt;|<)!--[\s\S]*?--(&gt;|>)/gm,
            url: /\w+:\/\/[\w-.\/?%&=:@;]*/g,
            phpScriptTags: {
                left: /(&lt;|<)\?=?/g,
                right: /\?(&gt;|>)/g
            },
            aspScriptTags: {
                left: /(&lt;|<)%=?/g,
                right: /%(&gt;|>)/g
            },
            scriptScriptTags: {
                left: /(&lt;|<)\s*script.*?(&gt;|>)/gi,
                right: /(&lt;|<)\/\s*script\s*(&gt;|>)/gi
            }
        },
        toolbar: {
            getHtml: function (a) {
                function b(i, k) {
                    return f.toolbar.getButtonHtml(i, k, f.config.strings[k]);
                }
                for (var c = '<div class="toolbar">', d = f.toolbar.items, h = d.list, g = 0; g < h.length; g++) c += (d[h[g]].getHtml || b)(a, h[g]);
                c += "</div>";
                return c;
            },
            getButtonHtml: function (a, b, c) {
                return '<span><a href="#" class="toolbar_item command_' + b + " " + b + '">' + c + "</a></span>";
            },
            handler: function (a) {
                var b = a.target,
                    c = b.className || "";
                b = B(p(b, ".syntaxhighlighter", true).id);
                var d = function (h) {
                    return (h = RegExp(h + "_(\\w+)").exec(c)) ? h[1] : null;
                }("command");
                b && d && f.toolbar.items[d].execute(b);
                a.preventDefault();
            },
            items: {
                list: ["expandSource", "help"],
                expandSource: {
                    getHtml: function (a) {
                        if (a.getParam("collapse") != true) return "";
                        var b = a.getParam("title");
                        return f.toolbar.getButtonHtml(a, "expandSource", b ? b : f.config.strings.expandSource);
                    },
                    execute: function (a) {
                        a = document.getElementById(t(a.id));
                        a.className = a.className.replace("collapsed", "");
                    }
                },
                help: {
                    execute: function () {
                        var a = "scrollbars=0";
                        a += ", left=" + (screen.width - 500) / 2 + ", top=" + (screen.height - 250) / 2 + ", width=500, height=250";
                        a = a.replace(/^,/, "");
                        a = window.open("", "_blank", a);
                        a.focus();
                        var b = a.document;
                        b.write(f.config.strings.aboutDialog);
                        b.close();
                        a.focus();
                    }
                }
            }
        },
        findElements: function (a, b) {
            var c;
            if (b) c = [b];
            else {
                c = document.getElementsByTagName(f.config.tagName);
                for (var d = [], h = 0; h < c.length; h++) d.push(c[h]);
                c = d;
            }
            c = c;
            d = [];
            if (f.config.useScriptTags) c = c.concat(z());
            if (c.length === 0) return d;
            for (h = 0; h < c.length; h++) {
                for (var g = c[h], i = a, k = c[h].className, j = void 0, l = {}, m = new XRegExp("^\\[(?<values>(.*?))\\]$"), s = new XRegExp("(?<name>[\\w-]+)\\s*:\\s*(?<value>[\\w-%#]+|\\[.*?\\]|\".*?\"|'.*?')\\s*;?", "g") ;
                (j = s.exec(k)) != null;) {
                    var o = j.value.replace(/^['"]|['"]$/g, "");
                    if (o != null && m.test(o)) {
                        o = m.exec(o);
                        o = o.values.length > 0 ? o.values.split(/\s*,\s*/) : [];
                    }
                    l[j.name] = o;
                }
                g = {
                    target: g,
                    params: C(i, l)
                };
                g.params.brush != null && d.push(g);
            }
            return d;
        },
        highlight: function (a, b) {
            var c = this.findElements(a, b),
                d = null,
                h = f.config;
            if (c.length !== 0) for (var g = 0; g < c.length; g++) {
                b = c[g];
                var i = b.target,
                    k = b.params,
                    j = k.brush,
                    l;
                if (j != null) {
                    if (k["html-script"] == "true" || f.defaults["html-script"] == true) {
                        d = new f.HtmlScript(j);
                        j = "htmlscript";
                    } else if (d = A(j)) d = new d;
                    else continue;
                    l = i.innerHTML;
                    if (h.useScriptTags) {
                        l = l;
                        var m = x(l),
                            s = false;
                        if (m.indexOf("<![CDATA[") == 0) {
                            m = m.substring(9);
                            s = true;
                        }
                        var o = m.length;
                        if (m.indexOf("]]\>") == o - 3) {
                            m = m.substring(0, o - 3);
                            s = true;
                        }
                        l = s ? m : l;
                    }
                    if ((i.title || "") != "") k.title = i.title;
                    k.brush = j;
                    d.init(k);
                    b = d.getDiv(l);
                    if ((i.id || "") != "") b.id = i.id;
                    i.parentNode.replaceChild(b, i);
                }
            }
        },
        all: function (a) {
            w(window, "load", function () {
                f.highlight(a);
            });
        }
    };
    f.Match = function (a, b, c) {
        this.value = a;
        this.index = b;
        this.length = a.length;
        this.css = c;
        this.brushName = null;
    };
    f.Match.prototype.toString = function () {
        return this.value;
    };
    f.HtmlScript = function (a) {
        function b(j, l) {
            for (var m = 0; m < j.length; m++) j[m].index += l;
        }
        var c = A(a),
            d, h = new f.brushes.Xml,
            g = this,
            i = "getDiv getHtml init".split(" ");
        if (c != null) {
            d = new c;
            for (var k = 0; k < i.length; k++) (function () {
                var j = i[k];
                g[j] = function () {
                    return h[j].apply(h, arguments);
                };
            })();
            d.htmlScript == null ? window.alert(f.config.strings.alert + (f.config.strings.brushNotHtmlScript + a)) : h.regexList.push({
                regex: d.htmlScript.code,
                func: function (j) {
                    for (var l = j.code, m = [], s = d.regexList, o = j.index + j.left.length, F = d.htmlScript, q, G = 0; G < s.length; G++) {
                        q = y(l, s[G]);
                        b(q, o);
                        m = m.concat(q);
                    }
                    if (F.left != null && j.left != null) {
                        q = y(j.left, F.left);
                        b(q, j.index);
                        m = m.concat(q);
                    }
                    if (F.right != null && j.right != null) {
                        q = y(j.right, F.right);
                        b(q, j.index + j[0].lastIndexOf(j.right));
                        m = m.concat(q);
                    }
                    for (j = 0; j < m.length; j++) m[j].brushName = c.brushName;
                    return m;
                }
            });
        }
    };
    f.Highlighter = function () { };
    f.Highlighter.prototype = {
        getParam: function (a, b) {
            var c = this.params[a];
            c = c == null ? b : c;
            var d = {
                "true": true,
                "false": false
            }[c];
            return d == null ? c : d;
        },
        create: function (a) {
            return document.createElement(a);
        },
        findMatches: function (a, b) {
            var c = [];
            if (a != null) for (var d = 0; d < a.length; d++) if (typeof a[d] == "object") c = c.concat(y(b, a[d]));
            return this.removeNestedMatches(c.sort(D));
        },
        removeNestedMatches: function (a) {
            for (var b = 0; b < a.length; b++) if (a[b] !== null) for (var c = a[b], d = c.index + c.length, h = b + 1; h < a.length && a[b] !== null; h++) {
                var g = a[h];
                if (g !== null) if (g.index > d) break;
                else if (g.index == c.index && g.length > c.length) a[b] = null;
                else if (g.index >= c.index && g.index < d) a[h] = null;
                                                                  }
            return a;
        },
        figureOutLineNumbers: function (a) {
            var b = [],
                c = parseInt(this.getParam("first-line"));
            v(a, function (d, h) {
                b.push(h + c);
            });
            return b;
        },
        isLineHighlighted: function (a) {
            var b = this.getParam("highlight", []);
            if (typeof b != "object" && b.push == null) b = [b];
            a: {
                a = a.toString();
                var c = void 0;
                for (c = c = Math.max(c || 0, 0) ; c < b.length; c++) if (b[c] == a) {
                    b = c;
                    break a;
                                                                      }
                b = -1;
               }
            return b != -1;
        },
        getLineHtml: function (a, b, c) {
            a = ["line", "number" + b, "index" + a, "alt" + (b % 2 == 0 ? 1 : 2).toString()];
            this.isLineHighlighted(b) && a.push("highlighted");
            b == 0 && a.push("break");
            return '<div class="' + a.join(" ") + '">' + c + "</div>";
        },
        getLineNumbersHtml: function (a, b) {
            var c = "",
                d = a.split("\n").length,
                h = parseInt(this.getParam("first-line")),
                g = this.getParam("pad-line-numbers");
            if (g == true) g = (h + d - 1).toString().length;
            else if (isNaN(g) == true) g = 0;
            for (var i = 0; i < d; i++) {
                var k = b ? b[i] : h + i,
                    j;
                if (k == 0) j = f.config.space;
                else {
                    j = g;
                    for (var l = k.toString() ; l.length < j;) l = "0" + l;
                    j = l;
                }
                a = j;
                c += this.getLineHtml(i, k, a);
            }
            return c;
        },
        getCodeLinesHtml: function (a, b) {
            a = x(a);
            var c = a.split("\n");
            this.getParam("pad-line-numbers");
            var d = parseInt(this.getParam("first-line"));
            a = "";
            for (var h = this.getParam("brush"), g = 0; g < c.length; g++) {
                var i = c[g],
                    k = /^(&nbsp;|\s)+/.exec(i),
                    j = null,
                    l = b ? b[g] : d + g;
                if (k != null) {
                    j = k[0].toString();
                    i = i.substr(j.length);
                    j = j.replace(" ", f.config.space);
                }
                i = x(i);
                if (i.length == 0) i = f.config.space;
                a += this.getLineHtml(g, l, (j != null ? '<code class="' + h + ' spaces">' + j + "</code>" : "") + i);
            }
            return a;
        },
        getTitleHtml: function (a) {
            return a ? "<caption>" + a + "</caption>" : "";
        },
        getMatchesHtml: function (a, b) {
            function c(l) {
                return (l = l ? l.brushName || g : g) ? l + " " : "";
            }
            for (var d = 0, h = "", g = this.getParam("brush", ""), i = 0; i < b.length; i++) {
                var k = b[i],
                    j;
                if (!(k === null || k.length === 0)) {
                    j = c(k);
                    h += u(a.substr(d, k.index - d), j + "plain") + u(k.value, j + k.css);
                    d = k.index + k.length + (k.offset || 0);
                }
            }
            h += u(a.substr(d), c() + "plain");
            return h;
        },
        getHtml: function (a) {
            var b = "",
                c = ["syntaxhighlighter"],
                d;
            if (this.getParam("light") == true) this.params.toolbar = this.params.gutter = false;
            className = "syntaxhighlighter";
            this.getParam("collapse") == true && c.push("collapsed");
            if ((gutter = this.getParam("gutter")) == false) c.push("nogutter");
            c.push(this.getParam("class-name"));
            c.push(this.getParam("brush"));
            a = a.replace(/^[ ]*[\n]+|[\n]*[ ]*$/g, "").replace(/\r/g, " ");
            b = this.getParam("tab-size");
            if (this.getParam("smart-tabs") == true) a = n(a, b);
            else {
                for (var h = "", g = 0; g < b; g++) h += " ";
                a = a.replace(/\t/g, h);
            }
            a = a;
            a: {
                b = a = a;
                h = /<br\s*\/?>|&lt;br\s*\/?&gt;/gi;
                if (f.config.bloggerMode == true) b = b.replace(h, "\n");
                if (f.config.stripBrs == true) b = b.replace(h, "");
                b = b.split("\n");
                h = /^\s*/;
                g = 1E3;
                for (var i = 0; i < b.length && g > 0; i++) {
                    var k = b[i];
                    if (x(k).length != 0) {
                        k = h.exec(k);
                        if (k == null) {
                            a = a;
                            break a;
                        }
                        g = Math.min(k[0].length, g);
                    }
                }
                if (g > 0) for (i = 0; i < b.length; i++) b[i] = b[i].substr(g);
                a = b.join("\n");
               }
            if (gutter) d = this.figureOutLineNumbers(a);
            b = this.findMatches(this.regexList, a);
            b = this.getMatchesHtml(a, b);
            b = this.getCodeLinesHtml(b, d);
            if (this.getParam("auto-links")) b = E(b);
            typeof navigator != "undefined" && navigator.userAgent && navigator.userAgent.match(/MSIE/) && c.push("ie");
            return b = '<div id="' + t(this.id) + '" class="' + c.join(" ") + '">' + (this.getParam("toolbar") ? f.toolbar.getHtml(this) : "") + '<table border="0" cellpadding="0" cellspacing="0">' + this.getTitleHtml(this.getParam("title")) + "<tbody><tr>" + (gutter ? '<td class="gutter">' + this.getLineNumbersHtml(a) + "</td>" : "") + '<td class="code"><div class="container">' + b + "</div></td></tr></tbody></table></div>";
        },
        getDiv: function (a) {
            if (a === null) a = "";
            this.code = a;
            var b = this.create("div");
            b.innerHTML = this.getHtml(a);
            this.getParam("toolbar") && w(p(b, ".toolbar"), "click", f.toolbar.handler);
            this.getParam("quick-code") && w(p(b, ".code"), "dblclick", e);
            return b;
        },
        init: function (a) {
            this.id = "" + Math.round(Math.random() * 1E6).toString();
            f.vars.highlighters[t(this.id)] = this;
            this.params = C(f.defaults, a || {});
            if (this.getParam("light") == true) this.params.toolbar = this.params.gutter = false;
        },
        getKeywords: function (a) {
            a = a.replace(/^\s+|\s+$/g, "").replace(/\s+/g, "|");
            return "\\b(?:" + a + ")\\b";
        },
        forHtmlScript: function (a) {
            this.htmlScript = {
                left: {
                    regex: a.left,
                    css: "script"
                },
                right: {
                    regex: a.right,
                    css: "script"
                },
                code: new XRegExp("(?<left>" + a.left.source + ")(?<code>.*?)(?<right>" + a.right.source + ")", "sgi")
            };
        }
    };
    return f;
}();
typeof exports != "undefined" && (exports.SyntaxHighlighter = SyntaxHighlighter);

 
;
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'class interface function package';
        var b = '-Infinity ...rest Array as AS3 Boolean break case catch const continue Date decodeURI ' + 'decodeURIComponent default delete do dynamic each else encodeURI encodeURIComponent escape ' + 'extends false final finally flash_proxy for get if implements import in include Infinity ' + 'instanceof int internal is isFinite isNaN isXMLName label namespace NaN native new null ' + 'Null Number Object object_proxy override parseFloat parseInt private protected public ' + 'return set static String super switch this throw true try typeof uint undefined unescape ' + 'use void while with';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\b([\d]+(\.[\d]+)?|0x[a-f0-9]+)\b/gi,
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'color3'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp('var', 'gm'),
            css: 'variable'
        }, {
            regex: new RegExp('trace', 'gm'),
            css: 'color1'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.scriptScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['actionscript3', 'as3'];
    SyntaxHighlighter.brushes.AS3 = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'if fi then elif else for do done until while break continue case function return in eq ne ge le';
        var b = 'alias apropos awk basename bash bc bg builtin bzip2 cal cat cd cfdisk chgrp chmod chown chroot' + 'cksum clear cmp comm command cp cron crontab csplit cut date dc dd ddrescue declare df ' + 'diff diff3 dig dir dircolors dirname dirs du echo egrep eject enable env ethtool eval ' + 'exec exit expand export expr false fdformat fdisk fg fgrep file find fmt fold format ' + 'free fsck ftp gawk getopts grep groups gzip hash head history hostname id ifconfig ' + 'import install join kill less let ln local locate logname logout look lpc lpr lprint ' + 'lprintd lprintq lprm ls lsof make man mkdir mkfifo mkisofs mknod more mount mtools ' + 'mv netstat nice nl nohup nslookup open op passwd paste pathchk ping popd pr printcap ' + 'printenv printf ps pushd pwd quota quotacheck quotactl ram rcp read readonly renice ' + 'remsync rm rmdir rsync screen scp sdiff sed select seq set sftp shift shopt shutdown ' + 'sleep sort source split ssh strace su sudo sum symlink sync tail tar tee test time ' + 'times touch top traceroute trap tr true tsort tty type ulimit umask umount unalias ' + 'uname unexpand uniq units unset unshar useradd usermod users uuencode uudecode v vdir ' + 'vi watch wc whereis which who whoami Wget xargs yes';
        this.regexList = [{
            regex: /^#!.*$/gm,
            css: 'preprocessor bold'
        }, {
            regex: /\/[\w-\/]+/gm,
            css: 'plain'
        }, {
            regex: SyntaxHighlighter.regexLib.singleLinePerlComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'functions'
        }
        ];
    }
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['bash', 'shell'];
    SyntaxHighlighter.brushes.Bash = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'Abs ACos AddSOAPRequestHeader AddSOAPResponseHeader AjaxLink AjaxOnLoad ArrayAppend ArrayAvg ArrayClear ArrayDeleteAt ' + 'ArrayInsertAt ArrayIsDefined ArrayIsEmpty ArrayLen ArrayMax ArrayMin ArraySet ArraySort ArraySum ArraySwap ArrayToList ' + 'Asc ASin Atn BinaryDecode BinaryEncode BitAnd BitMaskClear BitMaskRead BitMaskSet BitNot BitOr BitSHLN BitSHRN BitXor ' + 'Ceiling CharsetDecode CharsetEncode Chr CJustify Compare CompareNoCase Cos CreateDate CreateDateTime CreateObject ' + 'CreateODBCDate CreateODBCDateTime CreateODBCTime CreateTime CreateTimeSpan CreateUUID DateAdd DateCompare DateConvert ' + 'DateDiff DateFormat DatePart Day DayOfWeek DayOfWeekAsString DayOfYear DaysInMonth DaysInYear DE DecimalFormat DecrementValue ' + 'Decrypt DecryptBinary DeleteClientVariable DeserializeJSON DirectoryExists DollarFormat DotNetToCFType Duplicate Encrypt ' + 'EncryptBinary Evaluate Exp ExpandPath FileClose FileCopy FileDelete FileExists FileIsEOF FileMove FileOpen FileRead ' + 'FileReadBinary FileReadLine FileSetAccessMode FileSetAttribute FileSetLastModified FileWrite Find FindNoCase FindOneOf ' + 'FirstDayOfMonth Fix FormatBaseN GenerateSecretKey GetAuthUser GetBaseTagData GetBaseTagList GetBaseTemplatePath ' + 'GetClientVariablesList GetComponentMetaData GetContextRoot GetCurrentTemplatePath GetDirectoryFromPath GetEncoding ' + 'GetException GetFileFromPath GetFileInfo GetFunctionList GetGatewayHelper GetHttpRequestData GetHttpTimeString ' + 'GetK2ServerDocCount GetK2ServerDocCountLimit GetLocale GetLocaleDisplayName GetLocalHostIP GetMetaData GetMetricData ' + 'GetPageContext GetPrinterInfo GetProfileSections GetProfileString GetReadableImageFormats GetSOAPRequest GetSOAPRequestHeader ' + 'GetSOAPResponse GetSOAPResponseHeader GetTempDirectory GetTempFile GetTemplatePath GetTickCount GetTimeZoneInfo GetToken ' + 'GetUserRoles GetWriteableImageFormats Hash Hour HTMLCodeFormat HTMLEditFormat IIf ImageAddBorder ImageBlur ImageClearRect ' + 'ImageCopy ImageCrop ImageDrawArc ImageDrawBeveledRect ImageDrawCubicCurve ImageDrawLine ImageDrawLines ImageDrawOval ' + 'ImageDrawPoint ImageDrawQuadraticCurve ImageDrawRect ImageDrawRoundRect ImageDrawText ImageFlip ImageGetBlob ImageGetBufferedImage ' + 'ImageGetEXIFTag ImageGetHeight ImageGetIPTCTag ImageGetWidth ImageGrayscale ImageInfo ImageNegative ImageNew ImageOverlay ImagePaste ' + 'ImageRead ImageReadBase64 ImageResize ImageRotate ImageRotateDrawingAxis ImageScaleToFit ImageSetAntialiasing ImageSetBackgroundColor ' + 'ImageSetDrawingColor ImageSetDrawingStroke ImageSetDrawingTransparency ImageSharpen ImageShear ImageShearDrawingAxis ImageTranslate ' + 'ImageTranslateDrawingAxis ImageWrite ImageWriteBase64 ImageXORDrawingMode IncrementValue InputBaseN Insert Int IsArray IsBinary ' + 'IsBoolean IsCustomFunction IsDate IsDDX IsDebugMode IsDefined IsImage IsImageFile IsInstanceOf IsJSON IsLeapYear IsLocalHost ' + 'IsNumeric IsNumericDate IsObject IsPDFFile IsPDFObject IsQuery IsSimpleValue IsSOAPRequest IsStruct IsUserInAnyRole IsUserInRole ' + 'IsUserLoggedIn IsValid IsWDDX IsXML IsXmlAttribute IsXmlDoc IsXmlElem IsXmlNode IsXmlRoot JavaCast JSStringFormat LCase Left Len ' + 'ListAppend ListChangeDelims ListContains ListContainsNoCase ListDeleteAt ListFind ListFindNoCase ListFirst ListGetAt ListInsertAt ' + 'ListLast ListLen ListPrepend ListQualify ListRest ListSetAt ListSort ListToArray ListValueCount ListValueCountNoCase LJustify Log ' + 'Log10 LSCurrencyFormat LSDateFormat LSEuroCurrencyFormat LSIsCurrency LSIsDate LSIsNumeric LSNumberFormat LSParseCurrency LSParseDateTime ' + 'LSParseEuroCurrency LSParseNumber LSTimeFormat LTrim Max Mid Min Minute Month MonthAsString Now NumberFormat ParagraphFormat ParseDateTime ' + 'Pi PrecisionEvaluate PreserveSingleQuotes Quarter QueryAddColumn QueryAddRow QueryConvertForGrid QueryNew QuerySetCell QuotedValueList Rand ' + 'Randomize RandRange REFind REFindNoCase ReleaseComObject REMatch REMatchNoCase RemoveChars RepeatString Replace ReplaceList ReplaceNoCase ' + 'REReplace REReplaceNoCase Reverse Right RJustify Round RTrim Second SendGatewayMessage SerializeJSON SetEncoding SetLocale SetProfileString ' + 'SetVariable Sgn Sin Sleep SpanExcluding SpanIncluding Sqr StripCR StructAppend StructClear StructCopy StructCount StructDelete StructFind ' + 'StructFindKey StructFindValue StructGet StructInsert StructIsEmpty StructKeyArray StructKeyExists StructKeyList StructKeyList StructNew ' + 'StructSort StructUpdate Tan TimeFormat ToBase64 ToBinary ToScript ToString Trim UCase URLDecode URLEncodedFormat URLSessionFormat Val ' + 'ValueList VerifyClient Week Wrap Wrap WriteOutput XmlChildPos XmlElemNew XmlFormat XmlGetNodeType XmlNew XmlParse XmlSearch XmlTransform ' + 'XmlValidate Year YesNoFormat';
        var b = 'cfabort cfajaximport cfajaxproxy cfapplet cfapplication cfargument cfassociate cfbreak cfcache cfcalendar ' + 'cfcase cfcatch cfchart cfchartdata cfchartseries cfcol cfcollection cfcomponent cfcontent cfcookie cfdbinfo ' + 'cfdefaultcase cfdirectory cfdiv cfdocument cfdocumentitem cfdocumentsection cfdump cfelse cfelseif cferror ' + 'cfexchangecalendar cfexchangeconnection cfexchangecontact cfexchangefilter cfexchangemail cfexchangetask ' + 'cfexecute cfexit cffeed cffile cfflush cfform cfformgroup cfformitem cfftp cffunction cfgrid cfgridcolumn ' + 'cfgridrow cfgridupdate cfheader cfhtmlhead cfhttp cfhttpparam cfif cfimage cfimport cfinclude cfindex ' + 'cfinput cfinsert cfinterface cfinvoke cfinvokeargument cflayout cflayoutarea cfldap cflocation cflock cflog ' + 'cflogin cfloginuser cflogout cfloop cfmail cfmailparam cfmailpart cfmenu cfmenuitem cfmodule cfNTauthenticate ' + 'cfobject cfobjectcache cfoutput cfparam cfpdf cfpdfform cfpdfformparam cfpdfparam cfpdfsubform cfpod cfpop ' + 'cfpresentation cfpresentationslide cfpresenter cfprint cfprocessingdirective cfprocparam cfprocresult ' + 'cfproperty cfquery cfqueryparam cfregistry cfreport cfreportparam cfrethrow cfreturn cfsavecontent cfschedule ' + 'cfscript cfsearch cfselect cfset cfsetting cfsilent cfslider cfsprydataset cfstoredproc cfswitch cftable ' + 'cftextarea cfthread cfthrow cftimer cftooltip cftrace cftransaction cftree cftreeitem cftry cfupdate cfwddx ' + 'cfwindow cfxml cfzip cfzipparam';
        var c = 'all and any between cross in join like not null or outer some';
        this.regexList = [{
            regex: new RegExp('--(.*)$', 'gm'),
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.xmlComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'functions'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gmi'),
            css: 'color1'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gmi'),
            css: 'keyword'
        }
        ];
    }
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['coldfusion', 'cf'];
    SyntaxHighlighter.brushes.ColdFusion = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'Abs ACos AddSOAPRequestHeader AddSOAPResponseHeader AjaxLink AjaxOnLoad ArrayAppend ArrayAvg ArrayClear ArrayDeleteAt ' + 'ArrayInsertAt ArrayIsDefined ArrayIsEmpty ArrayLen ArrayMax ArrayMin ArraySet ArraySort ArraySum ArraySwap ArrayToList ' + 'Asc ASin Atn BinaryDecode BinaryEncode BitAnd BitMaskClear BitMaskRead BitMaskSet BitNot BitOr BitSHLN BitSHRN BitXor ' + 'Ceiling CharsetDecode CharsetEncode Chr CJustify Compare CompareNoCase Cos CreateDate CreateDateTime CreateObject ' + 'CreateODBCDate CreateODBCDateTime CreateODBCTime CreateTime CreateTimeSpan CreateUUID DateAdd DateCompare DateConvert ' + 'DateDiff DateFormat DatePart Day DayOfWeek DayOfWeekAsString DayOfYear DaysInMonth DaysInYear DE DecimalFormat DecrementValue ' + 'Decrypt DecryptBinary DeleteClientVariable DeserializeJSON DirectoryExists DollarFormat DotNetToCFType Duplicate Encrypt ' + 'EncryptBinary Evaluate Exp ExpandPath FileClose FileCopy FileDelete FileExists FileIsEOF FileMove FileOpen FileRead ' + 'FileReadBinary FileReadLine FileSetAccessMode FileSetAttribute FileSetLastModified FileWrite Find FindNoCase FindOneOf ' + 'FirstDayOfMonth Fix FormatBaseN GenerateSecretKey GetAuthUser GetBaseTagData GetBaseTagList GetBaseTemplatePath ' + 'GetClientVariablesList GetComponentMetaData GetContextRoot GetCurrentTemplatePath GetDirectoryFromPath GetEncoding ' + 'GetException GetFileFromPath GetFileInfo GetFunctionList GetGatewayHelper GetHttpRequestData GetHttpTimeString ' + 'GetK2ServerDocCount GetK2ServerDocCountLimit GetLocale GetLocaleDisplayName GetLocalHostIP GetMetaData GetMetricData ' + 'GetPageContext GetPrinterInfo GetProfileSections GetProfileString GetReadableImageFormats GetSOAPRequest GetSOAPRequestHeader ' + 'GetSOAPResponse GetSOAPResponseHeader GetTempDirectory GetTempFile GetTemplatePath GetTickCount GetTimeZoneInfo GetToken ' + 'GetUserRoles GetWriteableImageFormats Hash Hour HTMLCodeFormat HTMLEditFormat IIf ImageAddBorder ImageBlur ImageClearRect ' + 'ImageCopy ImageCrop ImageDrawArc ImageDrawBeveledRect ImageDrawCubicCurve ImageDrawLine ImageDrawLines ImageDrawOval ' + 'ImageDrawPoint ImageDrawQuadraticCurve ImageDrawRect ImageDrawRoundRect ImageDrawText ImageFlip ImageGetBlob ImageGetBufferedImage ' + 'ImageGetEXIFTag ImageGetHeight ImageGetIPTCTag ImageGetWidth ImageGrayscale ImageInfo ImageNegative ImageNew ImageOverlay ImagePaste ' + 'ImageRead ImageReadBase64 ImageResize ImageRotate ImageRotateDrawingAxis ImageScaleToFit ImageSetAntialiasing ImageSetBackgroundColor ' + 'ImageSetDrawingColor ImageSetDrawingStroke ImageSetDrawingTransparency ImageSharpen ImageShear ImageShearDrawingAxis ImageTranslate ' + 'ImageTranslateDrawingAxis ImageWrite ImageWriteBase64 ImageXORDrawingMode IncrementValue InputBaseN Insert Int IsArray IsBinary ' + 'IsBoolean IsCustomFunction IsDate IsDDX IsDebugMode IsDefined IsImage IsImageFile IsInstanceOf IsJSON IsLeapYear IsLocalHost ' + 'IsNumeric IsNumericDate IsObject IsPDFFile IsPDFObject IsQuery IsSimpleValue IsSOAPRequest IsStruct IsUserInAnyRole IsUserInRole ' + 'IsUserLoggedIn IsValid IsWDDX IsXML IsXmlAttribute IsXmlDoc IsXmlElem IsXmlNode IsXmlRoot JavaCast JSStringFormat LCase Left Len ' + 'ListAppend ListChangeDelims ListContains ListContainsNoCase ListDeleteAt ListFind ListFindNoCase ListFirst ListGetAt ListInsertAt ' + 'ListLast ListLen ListPrepend ListQualify ListRest ListSetAt ListSort ListToArray ListValueCount ListValueCountNoCase LJustify Log ' + 'Log10 LSCurrencyFormat LSDateFormat LSEuroCurrencyFormat LSIsCurrency LSIsDate LSIsNumeric LSNumberFormat LSParseCurrency LSParseDateTime ' + 'LSParseEuroCurrency LSParseNumber LSTimeFormat LTrim Max Mid Min Minute Month MonthAsString Now NumberFormat ParagraphFormat ParseDateTime ' + 'Pi PrecisionEvaluate PreserveSingleQuotes Quarter QueryAddColumn QueryAddRow QueryConvertForGrid QueryNew QuerySetCell QuotedValueList Rand ' + 'Randomize RandRange REFind REFindNoCase ReleaseComObject REMatch REMatchNoCase RemoveChars RepeatString Replace ReplaceList ReplaceNoCase ' + 'REReplace REReplaceNoCase Reverse Right RJustify Round RTrim Second SendGatewayMessage SerializeJSON SetEncoding SetLocale SetProfileString ' + 'SetVariable Sgn Sin Sleep SpanExcluding SpanIncluding Sqr StripCR StructAppend StructClear StructCopy StructCount StructDelete StructFind ' + 'StructFindKey StructFindValue StructGet StructInsert StructIsEmpty StructKeyArray StructKeyExists StructKeyList StructKeyList StructNew ' + 'StructSort StructUpdate Tan TimeFormat ToBase64 ToBinary ToScript ToString Trim UCase URLDecode URLEncodedFormat URLSessionFormat Val ' + 'ValueList VerifyClient Week Wrap Wrap WriteOutput XmlChildPos XmlElemNew XmlFormat XmlGetNodeType XmlNew XmlParse XmlSearch XmlTransform ' + 'XmlValidate Year YesNoFormat';
        var b = 'cfabort cfajaximport cfajaxproxy cfapplet cfapplication cfargument cfassociate cfbreak cfcache cfcalendar ' + 'cfcase cfcatch cfchart cfchartdata cfchartseries cfcol cfcollection cfcomponent cfcontent cfcookie cfdbinfo ' + 'cfdefaultcase cfdirectory cfdiv cfdocument cfdocumentitem cfdocumentsection cfdump cfelse cfelseif cferror ' + 'cfexchangecalendar cfexchangeconnection cfexchangecontact cfexchangefilter cfexchangemail cfexchangetask ' + 'cfexecute cfexit cffeed cffile cfflush cfform cfformgroup cfformitem cfftp cffunction cfgrid cfgridcolumn ' + 'cfgridrow cfgridupdate cfheader cfhtmlhead cfhttp cfhttpparam cfif cfimage cfimport cfinclude cfindex ' + 'cfinput cfinsert cfinterface cfinvoke cfinvokeargument cflayout cflayoutarea cfldap cflocation cflock cflog ' + 'cflogin cfloginuser cflogout cfloop cfmail cfmailparam cfmailpart cfmenu cfmenuitem cfmodule cfNTauthenticate ' + 'cfobject cfobjectcache cfoutput cfparam cfpdf cfpdfform cfpdfformparam cfpdfparam cfpdfsubform cfpod cfpop ' + 'cfpresentation cfpresentationslide cfpresenter cfprint cfprocessingdirective cfprocparam cfprocresult ' + 'cfproperty cfquery cfqueryparam cfregistry cfreport cfreportparam cfrethrow cfreturn cfsavecontent cfschedule ' + 'cfscript cfsearch cfselect cfset cfsetting cfsilent cfslider cfsprydataset cfstoredproc cfswitch cftable ' + 'cftextarea cfthread cfthrow cftimer cftooltip cftrace cftransaction cftree cftreeitem cftry cfupdate cfwddx ' + 'cfwindow cfxml cfzip cfzipparam';
        var c = 'all and any between cross in join like not null or outer some';
        this.regexList = [{
            regex: new RegExp('--(.*)$', 'gm'),
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.xmlComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'functions'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gmi'),
            css: 'color1'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gmi'),
            css: 'keyword'
        }
        ];
    }
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['coldfusion', 'cf'];
    SyntaxHighlighter.brushes.ColdFusion = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var d = 'abstract as base bool break byte case catch char checked class const ' + 'continue decimal default delegate do double else enum event explicit ' + 'extern false finally fixed float for foreach get goto if implicit in int ' + 'interface internal is lock long namespace new null object operator out ' + 'override params private protected public readonly ref return sbyte sealed set ' + 'short sizeof stackalloc static string struct switch this throw true try ' + 'typeof uint ulong unchecked unsafe ushort using virtual void while';
        function fixComments(a, b) {
            var c = (a[0].indexOf("///") == 0) ? 'color1' : 'comments';
            return [new SyntaxHighlighter.Match(a[0], a.index, c)];
        }
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            func: fixComments
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: /@"(?:[^"]|"")*"/g,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /^\s*#.*/gm,
            css: 'preprocessor'
        }, {
            regex: new RegExp(this.getKeywords(d), 'gm'),
            css: 'keyword'
        }, {
            regex: /\bpartial(?=\s+(?:class|interface|struct)\b)/g,
            css: 'keyword'
        }, {
            regex: /\byield(?=\s+(?:return|break)\b)/g,
            css: 'keyword'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['c#', 'c-sharp', 'csharp'];
    SyntaxHighlighter.brushes.CSharp = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        function getKeywordsCSS(a) {
            return '\\b([a-z_]|)' + a.replace(/ /g, '(?=:)\\b|\\b([a-z_\\*]|\\*|)') + '(?=:)\\b';
        };
        function getValuesCSS(a) {
            return '\\b' + a.replace(/ /g, '(?!-)(?!:)\\b|\\b()') + '\:\\b';
        };
        var b = 'ascent azimuth background-attachment background-color background-image background-position ' + 'background-repeat background baseline bbox border-collapse border-color border-spacing border-style border-top ' + 'border-right border-bottom border-left border-top-color border-right-color border-bottom-color border-left-color ' + 'border-top-style border-right-style border-bottom-style border-left-style border-top-width border-right-width ' + 'border-bottom-width border-left-width border-width border bottom cap-height caption-side centerline clear clip color ' + 'content counter-increment counter-reset cue-after cue-before cue cursor definition-src descent direction display ' + 'elevation empty-cells float font-size-adjust font-family font-size font-stretch font-style font-variant font-weight font ' + 'height left letter-spacing line-height list-style-image list-style-position list-style-type list-style margin-top ' + 'margin-right margin-bottom margin-left margin marker-offset marks mathline max-height max-width min-height min-width orphans ' + 'outline-color outline-style outline-width outline overflow padding-top padding-right padding-bottom padding-left padding page ' + 'page-break-after page-break-before page-break-inside pause pause-after pause-before pitch pitch-range play-during position ' + 'quotes right richness size slope src speak-header speak-numeral speak-punctuation speak speech-rate stemh stemv stress ' + 'table-layout text-align top text-decoration text-indent text-shadow text-transform unicode-bidi unicode-range units-per-em ' + 'vertical-align visibility voice-family volume white-space widows width widths word-spacing x-height z-index';
        var c = 'above absolute all always aqua armenian attr aural auto avoid baseline behind below bidi-override black blink block blue bold bolder ' + 'both bottom braille capitalize caption center center-left center-right circle close-quote code collapse compact condensed ' + 'continuous counter counters crop cross crosshair cursive dashed decimal decimal-leading-zero default digits disc dotted double ' + 'embed embossed e-resize expanded extra-condensed extra-expanded fantasy far-left far-right fast faster fixed format fuchsia ' + 'gray green groove handheld hebrew help hidden hide high higher icon inline-table inline inset inside invert italic ' + 'justify landscape large larger left-side left leftwards level lighter lime line-through list-item local loud lower-alpha ' + 'lowercase lower-greek lower-latin lower-roman lower low ltr marker maroon medium message-box middle mix move narrower ' + 'navy ne-resize no-close-quote none no-open-quote no-repeat normal nowrap n-resize nw-resize oblique olive once open-quote outset ' + 'outside overline pointer portrait pre print projection purple red relative repeat repeat-x repeat-y rgb ridge right right-side ' + 'rightwards rtl run-in screen scroll semi-condensed semi-expanded separate se-resize show silent silver slower slow ' + 'small small-caps small-caption smaller soft solid speech spell-out square s-resize static status-bar sub super sw-resize ' + 'table-caption table-cell table-column table-column-group table-footer-group table-header-group table-row table-row-group teal ' + 'text-bottom text-top thick thin top transparent tty tv ultra-condensed ultra-expanded underline upper-alpha uppercase upper-latin ' + 'upper-roman url visible wait white wider w-resize x-fast x-high x-large x-loud x-low x-slow x-small x-soft xx-large xx-small yellow';
        var d = '[mM]onospace [tT]ahoma [vV]erdana [aA]rial [hH]elvetica [sS]ans-serif [sS]erif [cC]ourier mono sans serif';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\#[a-fA-F0-9]{3,6}/g,
            css: 'value'
        }, {
            regex: /(-?\d+)(\.\d+)?(px|em|pt|\:|\%|)/g,
            css: 'value'
        }, {
            regex: /!important/g,
            css: 'color3'
        }, {
            regex: new RegExp(getKeywordsCSS(b), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(getValuesCSS(c), 'g'),
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(d), 'g'),
            css: 'color1'
        }
        ];
        this.forHtmlScript({
            left: /(&lt;|<)\s*style.*?(&gt;|>)/gi,
            right: /(&lt;|<)\/\s*style\s*(&gt;|>)/gi
        });
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['css'];
    SyntaxHighlighter.brushes.CSS = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'abs addr and ansichar ansistring array as asm begin boolean byte cardinal ' + 'case char class comp const constructor currency destructor div do double ' + 'downto else end except exports extended false file finalization finally ' + 'for function goto if implementation in inherited int64 initialization ' + 'integer interface is label library longint longword mod nil not object ' + 'of on or packed pansichar pansistring pchar pcurrency pdatetime pextended ' + 'pint64 pointer private procedure program property pshortstring pstring ' + 'pvariant pwidechar pwidestring protected public published raise real real48 ' + 'record repeat set shl shortint shortstring shr single smallint string then ' + 'threadvar to true try type unit until uses val var varirnt while widechar ' + 'widestring with word write writeln xor';
        this.regexList = [{
            regex: /\(\*[\s\S]*?\*\)/gm,
            css: 'comments'
        }, {
            regex: /{(?!\$)[\s\S]*?}/gm,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\{\$[a-zA-Z]+ .+\}/g,
            css: 'color1'
        }, {
            regex: /\b[\d\.]+\b/g,
            css: 'value'
        }, {
            regex: /\$[a-zA-Z0-9]+\b/g,
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'keyword'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['delphi', 'pascal', 'pas'];
    SyntaxHighlighter.brushes.Delphi = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        this.regexList = [{
            regex: /^\+\+\+.*$/gm,
            css: 'color2'
        }, {
            regex: /^\-\-\-.*$/gm,
            css: 'color2'
        }, {
            regex: /^\s.*$/gm,
            css: 'color1'
        }, {
            regex: /^@@.*@@$/gm,
            css: 'variable'
        }, {
            regex: /^\+[^\+]{1}.*$/gm,
            css: 'string'
        }, {
            regex: /^\-[^\-]{1}.*$/gm,
            css: 'comments'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['diff', 'patch'];
    SyntaxHighlighter.brushes.Diff = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'after and andalso band begin bnot bor bsl bsr bxor ' + 'case catch cond div end fun if let not of or orelse ' + 'query receive rem try when xor' + ' module export import define';
        this.regexList = [{
            regex: new RegExp("[A-Z][A-Za-z0-9_]+", 'g'),
            css: 'constants'
        }, {
            regex: new RegExp("\\%.+", 'gm'),
            css: 'comments'
        }, {
            regex: new RegExp("\\?[A-Za-z0-9_]+", 'g'),
            css: 'preprocessor'
        }, {
            regex: new RegExp("[a-z0-9_]+:[a-z0-9_]+", 'g'),
            css: 'functions'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['erl', 'erlang'];
    SyntaxHighlighter.brushes.Erland = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'as assert break case catch class continue def default do else extends finally ' + 'if in implements import instanceof interface new package property return switch ' + 'throw throws try while public protected private static';
        var b = 'void boolean byte char short int long float double';
        var c = 'null';
        var d = 'allProperties count get size ' + 'collect each eachProperty eachPropertyName eachWithIndex find findAll ' + 'findIndexOf grep inject max min reverseEach sort ' + 'asImmutable asSynchronized flatten intersect join pop reverse subMap toList ' + 'padRight padLeft contains eachMatch toCharacter toLong toUrl tokenize ' + 'eachFile eachFileRecurse eachB yte eachLine readBytes readLine getText ' + 'splitEachLine withReader append encodeBase64 decodeBase64 filterLine ' + 'transformChar transformLine withOutputStream withPrintWriter withStream ' + 'withStreams withWriter withWriterAppend write writeLine ' + 'dump inspect invokeMethod print println step times upto use waitForOrKill ' + 'getText';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /""".*"""/g,
            css: 'string'
        }, {
            regex: new RegExp('\\b([\\d]+(\\.[\\d]+)?|0x[a-f0-9]+)\\b', 'gi'),
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'color1'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gm'),
            css: 'constants'
        }, {
            regex: new RegExp(this.getKeywords(d), 'gm'),
            css: 'functions'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    }
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['groovy'];
    SyntaxHighlighter.brushes.Groovy = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'abstract assert boolean break byte case catch char class const ' + 'continue default do double else enum extends ' + 'false final finally float for goto if implements import ' + 'instanceof int interface long native new null ' + 'package private protected public return ' + 'short static strictfp super switch synchronized this throw throws true ' + 'transient try void volatile while';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: /\/\*([^\*][\s\S]*)?\*\//gm,
            css: 'comments'
        }, {
            regex: /\/\*(?!\*\/)\*[\s\S]*?\*\//gm,
            css: 'preprocessor'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\b([\d]+(\.[\d]+)?|0x[a-f0-9]+)\b/gi,
            css: 'value'
        }, {
            regex: /(?!\@interface\b)\@[\$\w]+\b/g,
            css: 'color1'
        }, {
            regex: /\@interface\b/g,
            css: 'color2'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript({
            left: /(&lt;|<)%[@!=]?/g,
            right: /%(&gt;|>)/g
        });
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['java'];
    SyntaxHighlighter.brushes.Java = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'Boolean Byte Character Double Duration ' + 'Float Integer Long Number Short String Void';
        var b = 'abstract after and as assert at before bind bound break catch class ' + 'continue def delete else exclusive extends false finally first for from ' + 'function if import in indexof init insert instanceof into inverse last ' + 'lazy mixin mod nativearray new not null on or override package postinit ' + 'protected public public-init public-read replace return reverse sizeof ' + 'step super then this throw true try tween typeof var where while with ' + 'attribute let private readonly static trigger';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: /(-?\.?)(\b(\d*\.?\d+|\d+\.?\d*)(e[+-]?\d+)?|0x[a-f\d]+)\b\.?/gi,
            css: 'color2'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'variable'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['jfx', 'javafx'];
    SyntaxHighlighter.brushes.JavaFX = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'break case catch continue ' + 'default delete do else false  ' + 'for function if in instanceof ' + 'new null return super switch ' + 'this throw true try typeof var while with';
        var r = SyntaxHighlighter.regexLib;
        this.regexList = [{
            regex: r.multiLineDoubleQuotedString,
            css: 'string'
        }, {
            regex: r.multiLineSingleQuotedString,
            css: 'string'
        }, {
            regex: r.singleLineCComments,
            css: 'comments'
        }, {
            regex: r.multiLineCComments,
            css: 'comments'
        }, {
            regex: /\s*#.*/gm,
            css: 'preprocessor'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript(r.scriptScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['js', 'jscript', 'javascript'];
    SyntaxHighlighter.brushes.JScript = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'abs accept alarm atan2 bind binmode chdir chmod chomp chop chown chr ' + 'chroot close closedir connect cos crypt defined delete each endgrent ' + 'endhostent endnetent endprotoent endpwent endservent eof exec exists ' + 'exp fcntl fileno flock fork format formline getc getgrent getgrgid ' + 'getgrnam gethostbyaddr gethostbyname gethostent getlogin getnetbyaddr ' + 'getnetbyname getnetent getpeername getpgrp getppid getpriority ' + 'getprotobyname getprotobynumber getprotoent getpwent getpwnam getpwuid ' + 'getservbyname getservbyport getservent getsockname getsockopt glob ' + 'gmtime grep hex index int ioctl join keys kill lc lcfirst length link ' + 'listen localtime lock log lstat map mkdir msgctl msgget msgrcv msgsnd ' + 'oct open opendir ord pack pipe pop pos print printf prototype push ' + 'quotemeta rand read readdir readline readlink readpipe recv rename ' + 'reset reverse rewinddir rindex rmdir scalar seek seekdir select semctl ' + 'semget semop send setgrent sethostent setnetent setpgrp setpriority ' + 'setprotoent setpwent setservent setsockopt shift shmctl shmget shmread ' + 'shmwrite shutdown sin sleep socket socketpair sort splice split sprintf ' + 'sqrt srand stat study substr symlink syscall sysopen sysread sysseek ' + 'system syswrite tell telldir time times tr truncate uc ucfirst umask ' + 'undef unlink unpack unshift utime values vec wait waitpid warn write';
        var b = 'bless caller continue dbmclose dbmopen die do dump else elsif eval exit ' + 'for foreach goto if import last local my next no our package redo ref ' + 'require return sub tie tied unless untie until use wantarray while';
        this.regexList = [{
            regex: new RegExp('#[^!].*$', 'gm'),
            css: 'comments'
        }, {
            regex: new RegExp('^\\s*#!.*$', 'gm'),
            css: 'preprocessor'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp('(\\$|@|%)\\w+', 'g'),
            css: 'variable'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'functions'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.phpScriptTags);
    }
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['perl', 'Perl', 'pl'];
    SyntaxHighlighter.brushes.Perl = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'abs acos acosh addcslashes addslashes ' + 'array_change_key_case array_chunk array_combine array_count_values array_diff ' + 'array_diff_assoc array_diff_key array_diff_uassoc array_diff_ukey array_fill ' + 'array_filter array_flip array_intersect array_intersect_assoc array_intersect_key ' + 'array_intersect_uassoc array_intersect_ukey array_key_exists array_keys array_map ' + 'array_merge array_merge_recursive array_multisort array_pad array_pop array_product ' + 'array_push array_rand array_reduce array_reverse array_search array_shift ' + 'array_slice array_splice array_sum array_udiff array_udiff_assoc ' + 'array_udiff_uassoc array_uintersect array_uintersect_assoc ' + 'array_uintersect_uassoc array_unique array_unshift array_values array_walk ' + 'array_walk_recursive atan atan2 atanh base64_decode base64_encode base_convert ' + 'basename bcadd bccomp bcdiv bcmod bcmul bindec bindtextdomain bzclose bzcompress ' + 'bzdecompress bzerrno bzerror bzerrstr bzflush bzopen bzread bzwrite ceil chdir ' + 'checkdate checkdnsrr chgrp chmod chop chown chr chroot chunk_split class_exists ' + 'closedir closelog copy cos cosh count count_chars date decbin dechex decoct ' + 'deg2rad delete ebcdic2ascii echo empty end ereg ereg_replace eregi eregi_replace error_log ' + 'error_reporting escapeshellarg escapeshellcmd eval exec exit exp explode extension_loaded ' + 'feof fflush fgetc fgetcsv fgets fgetss file_exists file_get_contents file_put_contents ' + 'fileatime filectime filegroup fileinode filemtime fileowner fileperms filesize filetype ' + 'floatval flock floor flush fmod fnmatch fopen fpassthru fprintf fputcsv fputs fread fscanf ' + 'fseek fsockopen fstat ftell ftok getallheaders getcwd getdate getenv gethostbyaddr gethostbyname ' + 'gethostbynamel getimagesize getlastmod getmxrr getmygid getmyinode getmypid getmyuid getopt ' + 'getprotobyname getprotobynumber getrandmax getrusage getservbyname getservbyport gettext ' + 'gettimeofday gettype glob gmdate gmmktime ini_alter ini_get ini_get_all ini_restore ini_set ' + 'interface_exists intval ip2long is_a is_array is_bool is_callable is_dir is_double ' + 'is_executable is_file is_finite is_float is_infinite is_int is_integer is_link is_long ' + 'is_nan is_null is_numeric is_object is_readable is_real is_resource is_scalar is_soap_fault ' + 'is_string is_subclass_of is_uploaded_file is_writable is_writeable mkdir mktime nl2br ' + 'parse_ini_file parse_str parse_url passthru pathinfo print readlink realpath rewind rewinddir rmdir ' + 'round str_ireplace str_pad str_repeat str_replace str_rot13 str_shuffle str_split ' + 'str_word_count strcasecmp strchr strcmp strcoll strcspn strftime strip_tags stripcslashes ' + 'stripos stripslashes stristr strlen strnatcasecmp strnatcmp strncasecmp strncmp strpbrk ' + 'strpos strptime strrchr strrev strripos strrpos strspn strstr strtok strtolower strtotime ' + 'strtoupper strtr strval substr substr_compare';
        var b = 'abstract and array as break case catch cfunction class clone const continue declare default die do ' + 'else elseif enddeclare endfor endforeach endif endswitch endwhile extends final for foreach ' + 'function include include_once global goto if implements interface instanceof namespace new ' + 'old_function or private protected public return require require_once static switch ' + 'throw try use var while xor ';
        var c = '__FILE__ __LINE__ __METHOD__ __FUNCTION__ __CLASS__';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\$\w+/g,
            css: 'variable'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'functions'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gmi'),
            css: 'constants'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.phpScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['php'];
    SyntaxHighlighter.brushes.Php = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() { };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['text', 'plain'];
    SyntaxHighlighter.brushes.Plain = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'Add-Content Add-History Add-Member Add-PSSnapin Clear(-Content)? Clear-Item ' + 'Clear-ItemProperty Clear-Variable Compare-Object ConvertFrom-SecureString Convert-Path ' + 'ConvertTo-Html ConvertTo-SecureString Copy(-Item)? Copy-ItemProperty Export-Alias ' + 'Export-Clixml Export-Console Export-Csv ForEach(-Object)? Format-Custom Format-List ' + 'Format-Table Format-Wide Get-Acl Get-Alias Get-AuthenticodeSignature Get-ChildItem Get-Command ' + 'Get-Content Get-Credential Get-Culture Get-Date Get-EventLog Get-ExecutionPolicy ' + 'Get-Help Get-History Get-Host Get-Item Get-ItemProperty Get-Location Get-Member ' + 'Get-PfxCertificate Get-Process Get-PSDrive Get-PSProvider Get-PSSnapin Get-Service ' + 'Get-TraceSource Get-UICulture Get-Unique Get-Variable Get-WmiObject Group-Object ' + 'Import-Alias Import-Clixml Import-Csv Invoke-Expression Invoke-History Invoke-Item ' + 'Join-Path Measure-Command Measure-Object Move(-Item)? Move-ItemProperty New-Alias ' + 'New-Item New-ItemProperty New-Object New-PSDrive New-Service New-TimeSpan ' + 'New-Variable Out-Default Out-File Out-Host Out-Null Out-Printer Out-String Pop-Location ' + 'Push-Location Read-Host Remove-Item Remove-ItemProperty Remove-PSDrive Remove-PSSnapin ' + 'Remove-Variable Rename-Item Rename-ItemProperty Resolve-Path Restart-Service Resume-Service ' + 'Select-Object Select-String Set-Acl Set-Alias Set-AuthenticodeSignature Set-Content ' + 'Set-Date Set-ExecutionPolicy Set-Item Set-ItemProperty Set-Location Set-PSDebug ' + 'Set-Service Set-TraceSource Set(-Variable)? Sort-Object Split-Path Start-Service ' + 'Start-Sleep Start-Transcript Stop-Process Stop-Service Stop-Transcript Suspend-Service ' + 'Tee-Object Test-Path Trace-Command Update-FormatData Update-TypeData Where(-Object)? ' + 'Write-Debug Write-Error Write(-Host)? Write-Output Write-Progress Write-Verbose Write-Warning';
        var b = 'ac asnp clc cli clp clv cpi cpp cvpa diff epal epcsv fc fl ' + 'ft fw gal gc gci gcm gdr ghy gi gl gm gp gps group gsv ' + 'gsnp gu gv gwmi iex ihy ii ipal ipcsv mi mp nal ndr ni nv oh rdr ' + 'ri rni rnp rp rsnp rv rvpa sal sasv sc select si sl sleep sort sp ' + 'spps spsv sv tee cat cd cp h history kill lp ls ' + 'mount mv popd ps pushd pwd r rm rmdir echo cls chdir del dir ' + 'erase rd ren type % \\?';
        this.regexList = [{
            regex: /#.*$/gm,
            css: 'comments'
        }, {
            regex: /\$[a-zA-Z0-9]+\b/g,
            css: 'value'
        }, {
            regex: /\-[a-zA-Z]+\b/g,
            css: 'keyword'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'keyword'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gmi'),
            css: 'keyword'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['powershell', 'ps'];
    SyntaxHighlighter.brushes.PowerShell = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'and assert break class continue def del elif else ' + 'except exec finally for from global if import in is ' + 'lambda not or pass print raise return try yield while';
        var b = '__import__ abs all any apply basestring bin bool buffer callable ' + 'chr classmethod cmp coerce compile complex delattr dict dir ' + 'divmod enumerate eval execfile file filter float format frozenset ' + 'getattr globals hasattr hash help hex id input int intern ' + 'isinstance issubclass iter len list locals long map max min next ' + 'object oct open ord pow print property range raw_input reduce ' + 'reload repr reversed round set setattr slice sorted staticmethod ' + 'str sum super tuple type type unichr unicode vars xrange zip';
        var c = 'None True False self cls class_';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLinePerlComments,
            css: 'comments'
        }, {
            regex: /^\s*@\w+/gm,
            css: 'decorator'
        }, {
            regex: /(['\"]{3})([^\1])*?\1/gm,
            css: 'comments'
        }, {
            regex: /"(?!")(?:\.|\\\"|[^\""\n])*"/gm,
            css: 'string'
        }, {
            regex: /'(?!')(?:\.|(\\\')|[^\''\n])*'/gm,
            css: 'string'
        }, {
            regex: /\+|\-|\*|\/|\%|=|==/gm,
            css: 'keyword'
        }, {
            regex: /\b\d+\.?\w*/g,
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gmi'),
            css: 'functions'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gm'),
            css: 'color1'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['py', 'python'];
    SyntaxHighlighter.brushes.Python = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'alias and BEGIN begin break case class def define_method defined do each else elsif ' + 'END end ensure false for if in module new next nil not or raise redo rescue retry return ' + 'self super then throw true undef unless until when while yield';
        var b = 'Array Bignum Binding Class Continuation Dir Exception FalseClass File::Stat File Fixnum Fload ' + 'Hash Integer IO MatchData Method Module NilClass Numeric Object Proc Range Regexp String Struct::TMS Symbol ' + 'ThreadGroup Thread Time TrueClass';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLinePerlComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\b[A-Z0-9_]+\b/g,
            css: 'constants'
        }, {
            regex: /:[a-z][A-Za-z0-9_]*/g,
            css: 'color2'
        }, {
            regex: /(\$|@@|@)\w+/g,
            css: 'variable bold'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'color1'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['ruby', 'rails', 'ror', 'rb'];
    SyntaxHighlighter.brushes.Ruby = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        function getKeywordsCSS(a) {
            return '\\b([a-z_]|)' + a.replace(/ /g, '(?=:)\\b|\\b([a-z_\\*]|\\*|)') + '(?=:)\\b';
        };
        function getValuesCSS(a) {
            return '\\b' + a.replace(/ /g, '(?!-)(?!:)\\b|\\b()') + '\:\\b';
        };
        var b = 'ascent azimuth background-attachment background-color background-image background-position ' + 'background-repeat background baseline bbox border-collapse border-color border-spacing border-style border-top ' + 'border-right border-bottom border-left border-top-color border-right-color border-bottom-color border-left-color ' + 'border-top-style border-right-style border-bottom-style border-left-style border-top-width border-right-width ' + 'border-bottom-width border-left-width border-width border bottom cap-height caption-side centerline clear clip color ' + 'content counter-increment counter-reset cue-after cue-before cue cursor definition-src descent direction display ' + 'elevation empty-cells float font-size-adjust font-family font-size font-stretch font-style font-variant font-weight font ' + 'height left letter-spacing line-height list-style-image list-style-position list-style-type list-style margin-top ' + 'margin-right margin-bottom margin-left margin marker-offset marks mathline max-height max-width min-height min-width orphans ' + 'outline-color outline-style outline-width outline overflow padding-top padding-right padding-bottom padding-left padding page ' + 'page-break-after page-break-before page-break-inside pause pause-after pause-before pitch pitch-range play-during position ' + 'quotes right richness size slope src speak-header speak-numeral speak-punctuation speak speech-rate stemh stemv stress ' + 'table-layout text-align top text-decoration text-indent text-shadow text-transform unicode-bidi unicode-range units-per-em ' + 'vertical-align visibility voice-family volume white-space widows width widths word-spacing x-height z-index';
        var c = 'above absolute all always aqua armenian attr aural auto avoid baseline behind below bidi-override black blink block blue bold bolder ' + 'both bottom braille capitalize caption center center-left center-right circle close-quote code collapse compact condensed ' + 'continuous counter counters crop cross crosshair cursive dashed decimal decimal-leading-zero digits disc dotted double ' + 'embed embossed e-resize expanded extra-condensed extra-expanded fantasy far-left far-right fast faster fixed format fuchsia ' + 'gray green groove handheld hebrew help hidden hide high higher icon inline-table inline inset inside invert italic ' + 'justify landscape large larger left-side left leftwards level lighter lime line-through list-item local loud lower-alpha ' + 'lowercase lower-greek lower-latin lower-roman lower low ltr marker maroon medium message-box middle mix move narrower ' + 'navy ne-resize no-close-quote none no-open-quote no-repeat normal nowrap n-resize nw-resize oblique olive once open-quote outset ' + 'outside overline pointer portrait pre print projection purple red relative repeat repeat-x repeat-y rgb ridge right right-side ' + 'rightwards rtl run-in screen scroll semi-condensed semi-expanded separate se-resize show silent silver slower slow ' + 'small small-caps small-caption smaller soft solid speech spell-out square s-resize static status-bar sub super sw-resize ' + 'table-caption table-cell table-column table-column-group table-footer-group table-header-group table-row table-row-group teal ' + 'text-bottom text-top thick thin top transparent tty tv ultra-condensed ultra-expanded underline upper-alpha uppercase upper-latin ' + 'upper-roman url visible wait white wider w-resize x-fast x-high x-large x-loud x-low x-slow x-small x-soft xx-large xx-small yellow';
        var d = '[mM]onospace [tT]ahoma [vV]erdana [aA]rial [hH]elvetica [sS]ans-serif [sS]erif [cC]ourier mono sans serif';
        var e = '!important !default';
        var f = '@import @extend @debug @warn @if @for @while @mixin @include';
        var r = SyntaxHighlighter.regexLib;
        this.regexList = [{
            regex: r.multiLineCComments,
            css: 'comments'
        }, {
            regex: r.singleLineCComments,
            css: 'comments'
        }, {
            regex: r.doubleQuotedString,
            css: 'string'
        }, {
            regex: r.singleQuotedString,
            css: 'string'
        }, {
            regex: /\#[a-fA-F0-9]{3,6}/g,
            css: 'value'
        }, {
            regex: /\b(-?\d+)(\.\d+)?(px|em|pt|\:|\%|)\b/g,
            css: 'value'
        }, {
            regex: /\$\w+/g,
            css: 'variable'
        }, {
            regex: new RegExp(this.getKeywords(e), 'g'),
            css: 'color3'
        }, {
            regex: new RegExp(this.getKeywords(f), 'g'),
            css: 'preprocessor'
        }, {
            regex: new RegExp(getKeywordsCSS(b), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(getValuesCSS(c), 'g'),
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(d), 'g'),
            css: 'color1'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['sass', 'scss'];
    SyntaxHighlighter.brushes.Sass = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function() {
    typeof(require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;

    function Brush() {
        var a = 'val sealed case def true trait implicit forSome import match object null finally super ' + 'override try lazy for var catch throw type extends class while with new final yield abstract ' + 'else do if return protected private this package false';
        var b = '[_:=><%#@]+';
        this.regexList = [{
                regex: SyntaxHighlighter.regexLib.singleLineCComments,
                css: 'comments'
            }, {
                regex: SyntaxHighlighter.regexLib.multiLineCComments,
                css: 'comments'
            }, {
                regex: SyntaxHighlighter.regexLib.multiLineSingleQuotedString,
                css: 'string'
            }, {
                regex: SyntaxHighlighter.regexLib.multiLineDoubleQuotedString,
                css: 'string'
            }, {
                regex: SyntaxHighlighter.regexLib.singleQuotedString,
                css: 'string'
            }, {
                regex: /0x[a-f0-9]+|\d+(\.\d+)?/gi,
                css: 'value'
            }, {
                regex: new RegExp(this.getKeywords(a), 'gm'),
                css: 'keyword'
            }, {
                regex: new RegExp(b, 'gm'),
                css: 'keyword'
            }
        ];
    }

    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['scala'];
    SyntaxHighlighter.brushes.Scala = Brush;
    typeof(exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function() {
    typeof(require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;

    function Brush() {
        var a = 'abs avg case cast coalesce convert count current_timestamp ' + 'current_user day isnull left lower month nullif replace right ' + 'session_user space substring sum system_user upper user year';
        var b = 'absolute action add after alter as asc at authorization begin bigint ' + 'binary bit by cascade char character check checkpoint close collate ' + 'column commit committed connect connection constraint contains continue ' + 'create cube current current_date current_time cursor database date ' + 'deallocate dec decimal declare default delete desc distinct double drop ' + 'dynamic else end end-exec escape except exec execute false fetch first ' + 'float for force foreign forward free from full function global goto grant ' + 'group grouping having hour ignore index inner insensitive insert instead ' + 'int integer intersect into is isolation key last level load local max min ' + 'minute modify move name national nchar next no numeric of off on only ' + 'open option order out output partial password precision prepare primary ' + 'prior privileges procedure public read real references relative repeatable ' + 'restrict return returns revoke rollback rollup rows rule schema scroll ' + 'second section select sequence serializable set size smallint static ' + 'statistics table temp temporary then time timestamp to top transaction ' + 'translation trigger true truncate uncommitted union unique update values ' + 'varchar varying view when where with work';
        var c = 'all and any between cross in join like not null or outer some';
        this.regexList = [{
                regex: /--(.*)$/gm,
                css: 'comments'
            }, {
                regex: SyntaxHighlighter.regexLib.multiLineDoubleQuotedString,
                css: 'string'
            }, {
                regex: SyntaxHighlighter.regexLib.multiLineSingleQuotedString,
                css: 'string'
            }, {
                regex: new RegExp(this.getKeywords(a), 'gmi'),
                css: 'color2'
            }, {
                regex: new RegExp(this.getKeywords(c), 'gmi'),
                css: 'color1'
            }, {
                regex: new RegExp(this.getKeywords(b), 'gmi'),
                css: 'keyword'
            }
        ];
    }

    ;
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['sql'];
    SyntaxHighlighter.brushes.Sql = Brush;
    typeof(exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'AddHandler AddressOf AndAlso Alias And Ansi As Assembly Auto ' + 'Boolean ByRef Byte ByVal Call Case Catch CBool CByte CChar CDate ' + 'CDec CDbl Char CInt Class CLng CObj Const CShort CSng CStr CType ' + 'Date Decimal Declare Default Delegate Dim DirectCast Do Double Each ' + 'Else ElseIf End Enum Erase Error Event Exit False Finally For Friend ' + 'Function Get GetType GoSub GoTo Handles If Implements Imports In ' + 'Inherits Integer Interface Is Let Lib Like Long Loop Me Mod Module ' + 'MustInherit MustOverride MyBase MyClass Namespace New Next Not Nothing ' + 'NotInheritable NotOverridable Object On Option Optional Or OrElse ' + 'Overloads Overridable Overrides ParamArray Preserve Private Property ' + 'Protected Public RaiseEvent ReadOnly ReDim REM RemoveHandler Resume ' + 'Return Select Set Shadows Shared Short Single Static Step Stop String ' + 'Structure Sub SyncLock Then Throw To True Try TypeOf Unicode Until ' + 'Variant When While With WithEvents WriteOnly Xor';
        this.regexList = [{
            regex: /'.*$/gm,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: /^\s*#.*$/gm,
            css: 'preprocessor'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['vb', 'vbnet'];
    SyntaxHighlighter.brushes.Vb = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        function process(a, b) {
            var constructor = SyntaxHighlighter.Match,
                code = a[0],
                tag = new XRegExp('(&lt;|<)[\\s\\/\\?]*(?<name>[:\\w-\\.]+)', 'xg').exec(code),
                result = [];
            if (a.attributes != null) {
                var c, regex = new XRegExp('(?<name> [\\w:\\-\\.]+)' + '\\s*=\\s*' + '(?<value> ".*?"|\'.*?\'|\\w+)', 'xg');
                while ((c = regex.exec(code)) != null) {
                    result.push(new constructor(c.name, a.index + c.index, 'color1'));
                    result.push(new constructor(c.value, a.index + c.index + c[0].indexOf(c.value), 'string'));
                }
            }
            if (tag != null) result.push(new constructor(tag.name, a.index + tag[0].indexOf(tag.name), 'keyword'));
            return result;
        }
        this.regexList = [{
            regex: new XRegExp('(\\&lt;|<)\\!\\[[\\w\\s]*?\\[(.|\\s)*?\\]\\](\\&gt;|>)', 'gm'),
            css: 'color2'
        }, {
            regex: SyntaxHighlighter.regexLib.xmlComments,
            css: 'comments'
        }, {
            regex: new XRegExp('(&lt;|<)[\\s\\/\\?]*(\\w+)(?<attributes>.*?)[\\s\\/\\?]*(&gt;|>)', 'sg'),
            func: process
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['xml', 'xhtml', 'xslt', 'html'];
    SyntaxHighlighter.brushes.Xml = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'ATOM BOOL BOOLEAN BYTE CHAR COLORREF DWORD DWORDLONG DWORD_PTR ' + 'DWORD32 DWORD64 FLOAT HACCEL HALF_PTR HANDLE HBITMAP HBRUSH ' + 'HCOLORSPACE HCONV HCONVLIST HCURSOR HDC HDDEDATA HDESK HDROP HDWP ' + 'HENHMETAFILE HFILE HFONT HGDIOBJ HGLOBAL HHOOK HICON HINSTANCE HKEY ' + 'HKL HLOCAL HMENU HMETAFILE HMODULE HMONITOR HPALETTE HPEN HRESULT ' + 'HRGN HRSRC HSZ HWINSTA HWND INT INT_PTR INT32 INT64 LANGID LCID LCTYPE ' + 'LGRPID LONG LONGLONG LONG_PTR LONG32 LONG64 LPARAM LPBOOL LPBYTE LPCOLORREF ' + 'LPCSTR LPCTSTR LPCVOID LPCWSTR LPDWORD LPHANDLE LPINT LPLONG LPSTR LPTSTR ' + 'LPVOID LPWORD LPWSTR LRESULT PBOOL PBOOLEAN PBYTE PCHAR PCSTR PCTSTR PCWSTR ' + 'PDWORDLONG PDWORD_PTR PDWORD32 PDWORD64 PFLOAT PHALF_PTR PHANDLE PHKEY PINT ' + 'PINT_PTR PINT32 PINT64 PLCID PLONG PLONGLONG PLONG_PTR PLONG32 PLONG64 POINTER_32 ' + 'POINTER_64 PSHORT PSIZE_T PSSIZE_T PSTR PTBYTE PTCHAR PTSTR PUCHAR PUHALF_PTR ' + 'PUINT PUINT_PTR PUINT32 PUINT64 PULONG PULONGLONG PULONG_PTR PULONG32 PULONG64 ' + 'PUSHORT PVOID PWCHAR PWORD PWSTR SC_HANDLE SC_LOCK SERVICE_STATUS_HANDLE SHORT ' + 'SIZE_T SSIZE_T TBYTE TCHAR UCHAR UHALF_PTR UINT UINT_PTR UINT32 UINT64 ULONG ' + 'ULONGLONG ULONG_PTR ULONG32 ULONG64 USHORT USN VOID WCHAR WORD WPARAM WPARAM WPARAM ' + 'char bool short int __int32 __int64 __int8 __int16 long float double __wchar_t ' + 'clock_t _complex _dev_t _diskfree_t div_t ldiv_t _exception _EXCEPTION_POINTERS ' + 'FILE _finddata_t _finddatai64_t _wfinddata_t _wfinddatai64_t __finddata64_t ' + '__wfinddata64_t _FPIEEE_RECORD fpos_t _HEAPINFO _HFILE lconv intptr_t ' + 'jmp_buf mbstate_t _off_t _onexit_t _PNH ptrdiff_t _purecall_handler ' + 'sig_atomic_t size_t _stat __stat64 _stati64 terminate_function ' + 'time_t __time64_t _timeb __timeb64 tm uintptr_t _utimbuf ' + 'va_list wchar_t wctrans_t wctype_t wint_t signed';
        var b = 'break case catch class const __finally __exception __try ' + 'const_cast continue private public protected __declspec ' + 'default delete deprecated dllexport dllimport do dynamic_cast ' + 'else enum explicit extern if for friend goto inline ' + 'mutable naked namespace new noinline noreturn nothrow ' + 'register reinterpret_cast return selectany ' + 'sizeof static static_cast struct switch template this ' + 'thread throw true false try typedef typeid typename union ' + 'using uuid virtual void volatile whcar_t while';
        var c = 'assert isalnum isalpha iscntrl isdigit isgraph islower isprint' + 'ispunct isspace isupper isxdigit tolower toupper errno localeconv ' + 'setlocale acos asin atan atan2 ceil cos cosh exp fabs floor fmod ' + 'frexp ldexp log log10 modf pow sin sinh sqrt tan tanh jmp_buf ' + 'longjmp setjmp raise signal sig_atomic_t va_arg va_end va_start ' + 'clearerr fclose feof ferror fflush fgetc fgetpos fgets fopen ' + 'fprintf fputc fputs fread freopen fscanf fseek fsetpos ftell ' + 'fwrite getc getchar gets perror printf putc putchar puts remove ' + 'rename rewind scanf setbuf setvbuf sprintf sscanf tmpfile tmpnam ' + 'ungetc vfprintf vprintf vsprintf abort abs atexit atof atoi atol ' + 'bsearch calloc div exit free getenv labs ldiv malloc mblen mbstowcs ' + 'mbtowc qsort rand realloc srand strtod strtol strtoul system ' + 'wcstombs wctomb memchr memcmp memcpy memmove memset strcat strchr ' + 'strcmp strcoll strcpy strcspn strerror strlen strncat strncmp ' + 'strncpy strpbrk strrchr strspn strstr strtok strxfrm asctime ' + 'clock ctime difftime gmtime localtime mktime strftime time';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /^ *#.*/gm,
            css: 'preprocessor'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'color1 bold'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gm'),
            css: 'functions bold'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'keyword bold'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['cpp', 'c'];
    SyntaxHighlighter.brushes.Cpp = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();