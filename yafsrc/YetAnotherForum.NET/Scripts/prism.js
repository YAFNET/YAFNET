/* http://prismjs.com/download.html?themes=prism-funky&languages=markup+css+clike+javascript+aspnet+bash+c+csharp+cpp+css-extras+git+java+python+sql&plugins=line-numbers+autolinker */
try {
    var _self = (typeof window !== 'undefined')
	? window   // if in browser
	: (
		(typeof WorkerGlobalScope !== 'undefined' && self instanceof WorkerGlobalScope)
		? self // if in worker
		: {}   // if in node js
	);

    /**
     * Prism: Lightweight, robust, elegant syntax highlighting
     * MIT license http://www.opensource.org/licenses/mit-license.php/
     * @author Lea Verou http://lea.verou.me
     */

    var Prism = (function () {

        // Private helper vars
        var lang = /\blang(?:uage)?-(\w+)\b/i;
        var uniqueId = 0;

        var _ = _self.Prism = {
            util: {
                encode: function (tokens) {
                    if (tokens instanceof Token) {
                        return new Token(tokens.type, _.util.encode(tokens.content), tokens.alias);
                    } else if (_.util.type(tokens) === 'Array') {
                        return tokens.map(_.util.encode);
                    } else {
                        return tokens.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/\u00a0/g, ' ');
                    }
                },

                type: function (o) {
                    return Object.prototype.toString.call(o).match(/\[object (\w+)\]/)[1];
                },

                objId: function (obj) {
                    if (!obj['__id']) {
                        Object.defineProperty(obj, '__id', { value: ++uniqueId });
                    }
                    return obj['__id'];
                },

                // Deep clone a language definition (e.g. to extend it)
                clone: function (o) {
                    var type = _.util.type(o);

                    switch (type) {
                        case 'Object':
                            var clone = {};

                            for (var key in o) {
                                if (o.hasOwnProperty(key)) {
                                    clone[key] = _.util.clone(o[key]);
                                }
                            }

                            return clone;

                        case 'Array':
                            // Check for existence for IE8
                            return o.map && o.map(function (v) { return _.util.clone(v); });
                    }

                    return o;
                }
            },

            languages: {
                extend: function (id, redef) {
                    var lang = _.util.clone(_.languages[id]);

                    for (var key in redef) {
                        lang[key] = redef[key];
                    }

                    return lang;
                },

                /**
                 * Insert a token before another token in a language literal
                 * As this needs to recreate the object (we cannot actually insert before keys in object literals),
                 * we cannot just provide an object, we need anobject and a key.
                 * @param inside The key (or language id) of the parent
                 * @param before The key to insert before. If not provided, the function appends instead.
                 * @param insert Object with the key/value pairs to insert
                 * @param root The object that contains `inside`. If equal to Prism.languages, it can be omitted.
                 */
                insertBefore: function (inside, before, insert, root) {
                    root = root || _.languages;
                    var grammar = root[inside];

                    if (arguments.length == 2) {
                        insert = arguments[1];

                        for (var newToken in insert) {
                            if (insert.hasOwnProperty(newToken)) {
                                grammar[newToken] = insert[newToken];
                            }
                        }

                        return grammar;
                    }

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

                            ret[token] = grammar[token];
                        }
                    }

                    // Update references in other language definitions
                    _.languages.DFS(_.languages, function (key, value) {
                        if (value === root[inside] && key != inside) {
                            this[key] = ret;
                        }
                    });

                    return root[inside] = ret;
                },

                // Traverse a language definition with Depth First Search
                DFS: function (o, callback, type, visited) {
                    visited = visited || {};
                    for (var i in o) {
                        if (o.hasOwnProperty(i)) {
                            callback.call(o, i, o[i], type || i);

                            if (_.util.type(o[i]) === 'Object' && !visited[_.util.objId(o[i])]) {
                                visited[_.util.objId(o[i])] = true;
                                _.languages.DFS(o[i], callback, null, visited);
                            }
                            else if (_.util.type(o[i]) === 'Array' && !visited[_.util.objId(o[i])]) {
                                visited[_.util.objId(o[i])] = true;
                                _.languages.DFS(o[i], callback, i, visited);
                            }
                        }
                    }
                }
            },
            plugins: {},

            highlightAll: function (async, callback) {
                var env = {
                    callback: callback,
                    selector: 'code[class*="language-"], [class*="language-"] code, code[class*="lang-"], [class*="lang-"] code'
                };

                _.hooks.run("before-highlightall", env);

                var elements = env.elements || document.querySelectorAll(env.selector);

                for (var i = 0, element; element = elements[i++];) {
                    _.highlightElement(element, async === true, env.callback);
                }
            },

            highlightElement: function (element, async, callback) {
                // Find language
                var language, grammar, parent = element;

                while (parent && !lang.test(parent.className)) {
                    parent = parent.parentNode;
                }

                if (parent) {
                    language = (parent.className.match(lang) || [, ''])[1].toLowerCase();
                    grammar = _.languages[language];
                }

                // Set language on the element, if not present
                element.className = element.className.replace(lang, '').replace(/\s+/g, ' ') + ' language-' + language;

                // Set language on the parent, for styling
                parent = element.parentNode;

                if (/pre/i.test(parent.nodeName)) {
                    parent.className = parent.className.replace(lang, '').replace(/\s+/g, ' ') + ' language-' + language;
                }

                var code = element.textContent;

                var env = {
                    element: element,
                    language: language,
                    grammar: grammar,
                    code: code
                };

                _.hooks.run('before-sanity-check', env);

                if (!env.code || !env.grammar) {
                    _.hooks.run('complete', env);
                    return;
                }

                _.hooks.run('before-highlight', env);

                if (async && _self.Worker) {
                    var worker = new Worker(_.filename);

                    worker.onmessage = function (evt) {
                        env.highlightedCode = evt.data;

                        _.hooks.run('before-insert', env);

                        env.element.innerHTML = env.highlightedCode;

                        callback && callback.call(env.element);
                        _.hooks.run('after-highlight', env);
                        _.hooks.run('complete', env);
                    };

                    worker.postMessage(JSON.stringify({
                        language: env.language,
                        code: env.code,
                        immediateClose: true
                    }));
                }
                else {
                    env.highlightedCode = _.highlight(env.code, env.grammar, env.language);

                    _.hooks.run('before-insert', env);

                    env.element.innerHTML = env.highlightedCode;

                    callback && callback.call(element);

                    _.hooks.run('after-highlight', env);
                    _.hooks.run('complete', env);
                }
            },

            highlight: function (text, grammar, language) {
                var tokens = _.tokenize(text, grammar);
                return Token.stringify(_.util.encode(tokens), language);
            },

            tokenize: function (text, grammar, language) {
                var Token = _.Token;

                var strarr = [text];

                var rest = grammar.rest;

                if (rest) {
                    for (var token in rest) {
                        grammar[token] = rest[token];
                    }

                    delete grammar.rest;
                }

                tokenloop: for (var token in grammar) {
                    if (!grammar.hasOwnProperty(token) || !grammar[token]) {
                        continue;
                    }

                    var patterns = grammar[token];
                    patterns = (_.util.type(patterns) === "Array") ? patterns : [patterns];

                    for (var j = 0; j < patterns.length; ++j) {
                        var pattern = patterns[j],
                            inside = pattern.inside,
                            lookbehind = !!pattern.lookbehind,
                            greedy = !!pattern.greedy,
                            lookbehindLength = 0,
                            alias = pattern.alias;

                        if (greedy && !pattern.pattern.global) {
                            // Without the global flag, lastIndex won't work
                            var flags = pattern.pattern.toString().match(/[imuy]*$/)[0];
                            pattern.pattern = RegExp(pattern.pattern.source, flags + "g");
                        }

                        pattern = pattern.pattern || pattern;

                        // Don’t cache length as it changes during the loop
                        for (var i = 0, pos = 0; i < strarr.length; pos += (strarr[i].matchedStr || strarr[i]).length, ++i) {

                            var str = strarr[i];

                            if (strarr.length > text.length) {
                                // Something went terribly wrong, ABORT, ABORT!
                                break tokenloop;
                            }

                            if (str instanceof Token) {
                                continue;
                            }

                            pattern.lastIndex = 0;

                            var match = pattern.exec(str),
                                delNum = 1;

                            // Greedy patterns can override/remove up to two previously matched tokens
                            if (!match && greedy && i != strarr.length - 1) {
                                pattern.lastIndex = pos;
                                match = pattern.exec(text);
                                if (!match) {
                                    break;
                                }

                                var from = match.index + (lookbehind ? match[1].length : 0),
                                    to = match.index + match[0].length,
                                    k = i,
                                    p = pos;

                                for (var len = strarr.length; k < len && p < to; ++k) {
                                    p += (strarr[k].matchedStr || strarr[k]).length;
                                    // Move the index i to the element in strarr that is closest to from
                                    if (from >= p) {
                                        ++i;
                                        pos = p;
                                    }
                                }

                                /*
                                 * If strarr[i] is a Token, then the match starts inside another Token, which is invalid
                                 * If strarr[k - 1] is greedy we are in conflict with another greedy pattern
                                 */
                                if (strarr[i] instanceof Token || strarr[k - 1].greedy) {
                                    continue;
                                }

                                // Number of tokens to delete and replace with the new match
                                delNum = k - i;
                                str = text.slice(pos, p);
                                match.index -= pos;
                            }

                            if (!match) {
                                continue;
                            }

                            if (lookbehind) {
                                lookbehindLength = match[1].length;
                            }

                            var from = match.index + lookbehindLength,
                                match = match[0].slice(lookbehindLength),
                                to = from + match.length,
                                before = str.slice(0, from),
                                after = str.slice(to);

                            var args = [i, delNum];

                            if (before) {
                                args.push(before);
                            }

                            var wrapped = new Token(token, inside ? _.tokenize(match, inside) : match, alias, match, greedy);

                            args.push(wrapped);

                            if (after) {
                                args.push(after);
                            }

                            Array.prototype.splice.apply(strarr, args);
                        }
                    }
                }

                return strarr;
            },

            hooks: {
                all: {},

                add: function (name, callback) {
                    var hooks = _.hooks.all;

                    hooks[name] = hooks[name] || [];

                    hooks[name].push(callback);
                },

                run: function (name, env) {
                    var callbacks = _.hooks.all[name];

                    if (!callbacks || !callbacks.length) {
                        return;
                    }

                    for (var i = 0, callback; callback = callbacks[i++];) {
                        callback(env);
                    }
                }
            }
        };

        var Token = _.Token = function (type, content, alias, matchedStr, greedy) {
            this.type = type;
            this.content = content;
            this.alias = alias;
            // Copy of the full string this token was created from
            this.matchedStr = matchedStr || null;
            this.greedy = !!greedy;
        };

        Token.stringify = function (o, language, parent) {
            if (typeof o == 'string') {
                return o;
            }

            if (_.util.type(o) === 'Array') {
                return o.map(function (element) {
                    return Token.stringify(element, language, o);
                }).join('');
            }

            var env = {
                type: o.type,
                content: Token.stringify(o.content, language, parent),
                tag: 'span',
                classes: ['token', o.type],
                attributes: {},
                language: language,
                parent: parent
            };

            if (env.type == 'comment') {
                env.attributes['spellcheck'] = 'true';
            }

            if (o.alias) {
                var aliases = _.util.type(o.alias) === 'Array' ? o.alias : [o.alias];
                Array.prototype.push.apply(env.classes, aliases);
            }

            _.hooks.run('wrap', env);

            var attributes = '';

            for (var name in env.attributes) {
                attributes += (attributes ? ' ' : '') + name + '="' + (env.attributes[name] || '') + '"';
            }

            return '<' + env.tag + ' class="' + env.classes.join(' ') + '"' + (attributes ? ' ' + attributes : '') + '>' + env.content + '</' + env.tag + '>';

        };

        if (!_self.document) {
            if (!_self.addEventListener) {
                // in Node.js
                return _self.Prism;
            }
            // In worker
            _self.addEventListener('message', function (evt) {
                var message = JSON.parse(evt.data),
                    lang = message.language,
                    code = message.code,
                    immediateClose = message.immediateClose;

                _self.postMessage(_.highlight(code, _.languages[lang], lang));
                if (immediateClose) {
                    _self.close();
                }
            }, false);

            return _self.Prism;
        }

        //Get current script and highlight
        var script = document.currentScript || [].slice.call(document.getElementsByTagName("script")).pop();

        if (script) {
            _.filename = script.src;

            if (document.addEventListener && !script.hasAttribute('data-manual')) {
                if (document.readyState !== "loading") {
                    if (window.requestAnimationFrame) {
                        window.requestAnimationFrame(_.highlightAll);
                    } else {
                        window.setTimeout(_.highlightAll, 16);
                    }
                }
                else {
                    document.addEventListener('DOMContentLoaded', _.highlightAll);
                }
            }
        }

        return _self.Prism;

    })();

    if (typeof module !== 'undefined' && module.exports) {
        module.exports = Prism;
    }

    // hack for components to work correctly in node.js
    if (typeof global !== 'undefined') {
        global.Prism = Prism;
    }
    ;
    Prism.languages.markup = {
        'comment': /<!--[\w\W]*?-->/,
        'prolog': /<\?[\w\W]+?\?>/,
        'doctype': /<!DOCTYPE[\w\W]+?>/,
        'cdata': /<!\[CDATA\[[\w\W]*?]]>/i,
        'tag': {
            pattern: /<\/?(?!\d)[^\s>\/=$<]+(?:\s+[^\s>\/=]+(?:=(?:("|')(?:\\\1|\\?(?!\1)[\w\W])*\1|[^\s'">=]+))?)*\s*\/?>/i,
            inside: {
                'tag': {
                    pattern: /^<\/?[^\s>\/]+/i,
                    inside: {
                        'punctuation': /^<\/?/,
                        'namespace': /^[^\s>\/:]+:/
                    }
                },
                'attr-value': {
                    pattern: /=(?:('|")[\w\W]*?(\1)|[^\s>]+)/i,
                    inside: {
                        'punctuation': /[=>"']/
                    }
                },
                'punctuation': /\/?>/,
                'attr-name': {
                    pattern: /[^\s>\/]+/,
                    inside: {
                        'namespace': /^[^\s>\/:]+:/
                    }
                }

            }
        },
        'entity': /&#?[\da-z]{1,8};/i
    };

    // Plugin to make entity title show the real entity, idea by Roman Komarov
    Prism.hooks.add('wrap', function (env) {

        if (env.type === 'entity') {
            env.attributes['title'] = env.content.replace(/&amp;/, '&');
        }
    });

    Prism.languages.xml = Prism.languages.markup;
    Prism.languages.html = Prism.languages.markup;
    Prism.languages.mathml = Prism.languages.markup;
    Prism.languages.svg = Prism.languages.markup;

    Prism.languages.css = {
        'comment': /\/\*[\w\W]*?\*\//,
        'atrule': {
            pattern: /@[\w-]+?.*?(;|(?=\s*\{))/i,
            inside: {
                'rule': /@[\w-]+/
                // See rest below
            }
        },
        'url': /url\((?:(["'])(\\(?:\r\n|[\w\W])|(?!\1)[^\\\r\n])*\1|.*?)\)/i,
        'selector': /[^\{\}\s][^\{\};]*?(?=\s*\{)/,
        'string': /("|')(\\(?:\r\n|[\w\W])|(?!\1)[^\\\r\n])*\1/,
        'property': /(\b|\B)[\w-]+(?=\s*:)/i,
        'important': /\B!important\b/i,
        'function': /[-a-z0-9]+(?=\()/i,
        'punctuation': /[(){};:]/
    };

    Prism.languages.css['atrule'].inside.rest = Prism.util.clone(Prism.languages.css);

    if (Prism.languages.markup) {
        Prism.languages.insertBefore('markup', 'tag', {
            'style': {
                pattern: /(<style[\w\W]*?>)[\w\W]*?(?=<\/style>)/i,
                lookbehind: true,
                inside: Prism.languages.css,
                alias: 'language-css'
            }
        });

        Prism.languages.insertBefore('inside', 'attr-value', {
            'style-attr': {
                pattern: /\s*style=("|').*?\1/i,
                inside: {
                    'attr-name': {
                        pattern: /^\s*style/i,
                        inside: Prism.languages.markup.tag.inside
                    },
                    'punctuation': /^\s*=\s*['"]|['"]\s*$/,
                    'attr-value': {
                        pattern: /.+/i,
                        inside: Prism.languages.css
                    }
                },
                alias: 'language-css'
            }
        }, Prism.languages.markup.tag);
    };
    Prism.languages.clike = {
        'comment': [
            {
                pattern: /(^|[^\\])\/\*[\w\W]*?\*\//,
                lookbehind: true
            },
            {
                pattern: /(^|[^\\:])\/\/.*/,
                lookbehind: true
            }
        ],
        'string': {
            pattern: /(["'])(\\(?:\r\n|[\s\S])|(?!\1)[^\\\r\n])*\1/,
            greedy: true
        },
        'class-name': {
            pattern: /((?:\b(?:class|interface|extends|implements|trait|instanceof|new)\s+)|(?:catch\s+\())[a-z0-9_\.\\]+/i,
            lookbehind: true,
            inside: {
                punctuation: /(\.|\\)/
            }
        },
        'keyword': /\b(if|else|while|do|for|return|in|instanceof|function|new|try|throw|catch|finally|null|break|continue)\b/,
        'boolean': /\b(true|false)\b/,
        'function': /[a-z0-9_]+(?=\()/i,
        'number': /\b-?(?:0x[\da-f]+|\d*\.?\d+(?:e[+-]?\d+)?)\b/i,
        'operator': /--?|\+\+?|!=?=?|<=?|>=?|==?=?|&&?|\|\|?|\?|\*|\/|~|\^|%/,
        'punctuation': /[{}[\];(),.:]/
    };

    Prism.languages.javascript = Prism.languages.extend('clike', {
        'keyword': /\b(as|async|await|break|case|catch|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally|for|from|function|get|if|implements|import|in|instanceof|interface|let|new|null|of|package|private|protected|public|return|set|static|super|switch|this|throw|try|typeof|var|void|while|with|yield)\b/,
        'number': /\b-?(0x[\dA-Fa-f]+|0b[01]+|0o[0-7]+|\d*\.?\d+([Ee][+-]?\d+)?|NaN|Infinity)\b/,
        // Allow for all non-ASCII characters (See http://stackoverflow.com/a/2008444)
        'function': /[_$a-zA-Z\xA0-\uFFFF][_$a-zA-Z0-9\xA0-\uFFFF]*(?=\()/i,
        'operator': /--?|\+\+?|!=?=?|<=?|>=?|==?=?|&&?|\|\|?|\?|\*\*?|\/|~|\^|%|\.{3}/
    });

    Prism.languages.insertBefore('javascript', 'keyword', {
        'regex': {
            pattern: /(^|[^/])\/(?!\/)(\[.+?]|\\.|[^/\\\r\n])+\/[gimyu]{0,5}(?=\s*($|[\r\n,.;})]))/,
            lookbehind: true,
            greedy: true
        }
    });

    Prism.languages.insertBefore('javascript', 'string', {
        'template-string': {
            pattern: /`(?:\\\\|\\?[^\\])*?`/,
            greedy: true,
            inside: {
                'interpolation': {
                    pattern: /\$\{[^}]+\}/,
                    inside: {
                        'interpolation-punctuation': {
                            pattern: /^\$\{|\}$/,
                            alias: 'punctuation'
                        },
                        rest: Prism.languages.javascript
                    }
                },
                'string': /[\s\S]+/
            }
        }
    });

    if (Prism.languages.markup) {
        Prism.languages.insertBefore('markup', 'tag', {
            'script': {
                pattern: /(<script[\w\W]*?>)[\w\W]*?(?=<\/script>)/i,
                lookbehind: true,
                inside: Prism.languages.javascript,
                alias: 'language-javascript'
            }
        });
    }

    Prism.languages.js = Prism.languages.javascript;
    Prism.languages.aspnet = Prism.languages.extend('markup', {
        'page-directive tag': {
            pattern: /<%\s*@.*%>/i,
            inside: {
                'page-directive tag': /<%\s*@\s*(?:Assembly|Control|Implements|Import|Master(?:Type)?|OutputCache|Page|PreviousPageType|Reference|Register)?|%>/i,
                rest: Prism.languages.markup.tag.inside
            }
        },
        'directive tag': {
            pattern: /<%.*%>/i,
            inside: {
                'directive tag': /<%\s*?[$=%#:]{0,2}|%>/i,
                rest: Prism.languages.csharp
            }
        }
    });
    // Regexp copied from prism-markup, with a negative look-ahead added
    Prism.languages.aspnet.tag.pattern = /<(?!%)\/?[^\s>\/]+(?:\s+[^\s>\/=]+(?:=(?:("|')(?:\\\1|\\?(?!\1)[\w\W])*\1|[^\s'">=]+))?)*\s*\/?>/i;

    // match directives of attribute value foo="<% Bar %>"
    Prism.languages.insertBefore('inside', 'punctuation', {
        'directive tag': Prism.languages.aspnet['directive tag']
    }, Prism.languages.aspnet.tag.inside["attr-value"]);

    Prism.languages.insertBefore('aspnet', 'comment', {
        'asp comment': /<%--[\w\W]*?--%>/
    });

    // script runat="server" contains csharp, not javascript
    Prism.languages.insertBefore('aspnet', Prism.languages.javascript ? 'script' : 'tag', {
        'asp script': {
            pattern: /(<script(?=.*runat=['"]?server['"]?)[\w\W]*?>)[\w\W]*?(?=<\/script>)/i,
            lookbehind: true,
            inside: Prism.languages.csharp || {}
        }
    });
    (function (Prism) {
        var insideString = {
            variable: [
                // Arithmetic Environment
                {
                    pattern: /\$?\(\([\w\W]+?\)\)/,
                    inside: {
                        // If there is a $ sign at the beginning highlight $(( and )) as variable
                        variable: [{
                            pattern: /(^\$\(\([\w\W]+)\)\)/,
                            lookbehind: true
                        },
                            /^\$\(\(/,
                        ],
                        number: /\b-?(?:0x[\dA-Fa-f]+|\d*\.?\d+(?:[Ee]-?\d+)?)\b/,
                        // Operators according to https://www.gnu.org/software/bash/manual/bashref.html#Shell-Arithmetic
                        operator: /--?|-=|\+\+?|\+=|!=?|~|\*\*?|\*=|\/=?|%=?|<<=?|>>=?|<=?|>=?|==?|&&?|&=|\^=?|\|\|?|\|=|\?|:/,
                        // If there is no $ sign at the beginning highlight (( and )) as punctuation
                        punctuation: /\(\(?|\)\)?|,|;/
                    }
                },
                // Command Substitution
                {
                    pattern: /\$\([^)]+\)|`[^`]+`/,
                    inside: {
                        variable: /^\$\(|^`|\)$|`$/
                    }
                },
                /\$(?:[a-z0-9_#\?\*!@]+|\{[^}]+\})/i
            ],
        };

        Prism.languages.bash = {
            'shebang': {
                pattern: /^#!\s*\/bin\/bash|^#!\s*\/bin\/sh/,
                alias: 'important'
            },
            'comment': {
                pattern: /(^|[^"{\\])#.*/,
                lookbehind: true
            },
            'string': [
                //Support for Here-Documents https://en.wikipedia.org/wiki/Here_document
                {
                    pattern: /((?:^|[^<])<<\s*)(?:"|')?(\w+?)(?:"|')?\s*\r?\n(?:[\s\S])*?\r?\n\2/g,
                    lookbehind: true,
                    greedy: true,
                    inside: insideString
                },
                {
                    pattern: /(["'])(?:\\\\|\\?[^\\])*?\1/g,
                    greedy: true,
                    inside: insideString
                }
            ],
            'variable': insideString.variable,
            // Originally based on http://ss64.com/bash/
            'function': {
                pattern: /(^|\s|;|\||&)(?:alias|apropos|apt-get|aptitude|aspell|awk|basename|bash|bc|bg|builtin|bzip2|cal|cat|cd|cfdisk|chgrp|chmod|chown|chroot|chkconfig|cksum|clear|cmp|comm|command|cp|cron|crontab|csplit|cut|date|dc|dd|ddrescue|df|diff|diff3|dig|dir|dircolors|dirname|dirs|dmesg|du|egrep|eject|enable|env|ethtool|eval|exec|expand|expect|export|expr|fdformat|fdisk|fg|fgrep|file|find|fmt|fold|format|free|fsck|ftp|fuser|gawk|getopts|git|grep|groupadd|groupdel|groupmod|groups|gzip|hash|head|help|hg|history|hostname|htop|iconv|id|ifconfig|ifdown|ifup|import|install|jobs|join|kill|killall|less|link|ln|locate|logname|logout|look|lpc|lpr|lprint|lprintd|lprintq|lprm|ls|lsof|make|man|mkdir|mkfifo|mkisofs|mknod|more|most|mount|mtools|mtr|mv|mmv|nano|netstat|nice|nl|nohup|notify-send|npm|nslookup|open|op|passwd|paste|pathchk|ping|pkill|popd|pr|printcap|printenv|printf|ps|pushd|pv|pwd|quota|quotacheck|quotactl|ram|rar|rcp|read|readarray|readonly|reboot|rename|renice|remsync|rev|rm|rmdir|rsync|screen|scp|sdiff|sed|seq|service|sftp|shift|shopt|shutdown|sleep|slocate|sort|source|split|ssh|stat|strace|su|sudo|sum|suspend|sync|tail|tar|tee|test|time|timeout|times|touch|top|traceroute|trap|tr|tsort|tty|type|ulimit|umask|umount|unalias|uname|unexpand|uniq|units|unrar|unshar|uptime|useradd|userdel|usermod|users|uuencode|uudecode|v|vdir|vi|vmstat|wait|watch|wc|wget|whereis|which|who|whoami|write|xargs|xdg-open|yes|zip)(?=$|\s|;|\||&)/,
                lookbehind: true
            },
            'keyword': {
                pattern: /(^|\s|;|\||&)(?:let|:|\.|if|then|else|elif|fi|for|break|continue|while|in|case|function|select|do|done|until|echo|exit|return|set|declare)(?=$|\s|;|\||&)/,
                lookbehind: true
            },
            'boolean': {
                pattern: /(^|\s|;|\||&)(?:true|false)(?=$|\s|;|\||&)/,
                lookbehind: true
            },
            'operator': /&&?|\|\|?|==?|!=?|<<<?|>>|<=?|>=?|=~/,
            'punctuation': /\$?\(\(?|\)\)?|\.\.|[{}[\];]/
        };

        var inside = insideString.variable[1].inside;
        inside['function'] = Prism.languages.bash['function'];
        inside.keyword = Prism.languages.bash.keyword;
        inside.boolean = Prism.languages.bash.boolean;
        inside.operator = Prism.languages.bash.operator;
        inside.punctuation = Prism.languages.bash.punctuation;
    })(Prism);

    Prism.languages.c = Prism.languages.extend('clike', {
        'keyword': /\b(asm|typeof|inline|auto|break|case|char|const|continue|default|do|double|else|enum|extern|float|for|goto|if|int|long|register|return|short|signed|sizeof|static|struct|switch|typedef|union|unsigned|void|volatile|while)\b/,
        'operator': /\-[>-]?|\+\+?|!=?|<<?=?|>>?=?|==?|&&?|\|?\||[~^%?*\/]/,
        'number': /\b-?(?:0x[\da-f]+|\d*\.?\d+(?:e[+-]?\d+)?)[ful]*\b/i
    });

    Prism.languages.insertBefore('c', 'string', {
        'macro': {
            // allow for multiline macro definitions
            // spaces after the # character compile fine with gcc
            pattern: /(^\s*)#\s*[a-z]+([^\r\n\\]|\\.|\\(?:\r\n?|\n))*/im,
            lookbehind: true,
            alias: 'property',
            inside: {
                // highlight the path of the include statement as a string
                'string': {
                    pattern: /(#\s*include\s*)(<.+?>|("|')(\\?.)+?\3)/,
                    lookbehind: true
                },
                // highlight macro directives as keywords
                'directive': {
                    pattern: /(#\s*)\b(define|elif|else|endif|error|ifdef|ifndef|if|import|include|line|pragma|undef|using)\b/,
                    lookbehind: true,
                    alias: 'keyword'
                }
            }
        },
        // highlight predefined macros as constants
        'constant': /\b(__FILE__|__LINE__|__DATE__|__TIME__|__TIMESTAMP__|__func__|EOF|NULL|stdin|stdout|stderr)\b/
    });

    delete Prism.languages.c['class-name'];
    delete Prism.languages.c['boolean'];

    Prism.languages.csharp = Prism.languages.extend('clike', {
        'keyword': /\b(abstract|as|async|await|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|async|await|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b/,
        'string': [
            /@("|')(\1\1|\\\1|\\?(?!\1)[\s\S])*\1/,
            /("|')(\\?.)*?\1/
        ],
        'number': /\b-?(0x[\da-f]+|\d*\.?\d+f?)\b/i
    });

    Prism.languages.insertBefore('csharp', 'keyword', {
        'generic-method': {
            pattern: /[a-z0-9_]+\s*<[^>\r\n]+?>\s*(?=\()/i,
            alias: 'function',
            inside: {
                keyword: Prism.languages.csharp.keyword,
                punctuation: /[<>(),.:]/
            }
        },
        'preprocessor': {
            pattern: /(^\s*)#.*/m,
            lookbehind: true,
            alias: 'property',
            inside: {
                // highlight preprocessor directives as keywords
                'directive': {
                    pattern: /(\s*#)\b(define|elif|else|endif|endregion|error|if|line|pragma|region|undef|warning)\b/,
                    lookbehind: true,
                    alias: 'keyword'
                }
            }
        }
    });

    Prism.languages.cpp = Prism.languages.extend('c', {
        'keyword': /\b(alignas|alignof|asm|auto|bool|break|case|catch|char|char16_t|char32_t|class|compl|const|constexpr|const_cast|continue|decltype|default|delete|do|double|dynamic_cast|else|enum|explicit|export|extern|float|for|friend|goto|if|inline|int|long|mutable|namespace|new|noexcept|nullptr|operator|private|protected|public|register|reinterpret_cast|return|short|signed|sizeof|static|static_assert|static_cast|struct|switch|template|this|thread_local|throw|try|typedef|typeid|typename|union|unsigned|using|virtual|void|volatile|wchar_t|while)\b/,
        'boolean': /\b(true|false)\b/,
        'operator': /[-+]{1,2}|!=?|<{1,2}=?|>{1,2}=?|\->|:{1,2}|={1,2}|\^|~|%|&{1,2}|\|?\||\?|\*|\/|\b(and|and_eq|bitand|bitor|not|not_eq|or|or_eq|xor|xor_eq)\b/
    });

    Prism.languages.insertBefore('cpp', 'keyword', {
        'class-name': {
            pattern: /(class\s+)[a-z0-9_]+/i,
            lookbehind: true
        }
    });
    Prism.languages.css.selector = {
        pattern: /[^\{\}\s][^\{\}]*(?=\s*\{)/,
        inside: {
            'pseudo-element': /:(?:after|before|first-letter|first-line|selection)|::[-\w]+/,
            'pseudo-class': /:[-\w]+(?:\(.*\))?/,
            'class': /\.[-:\.\w]+/,
            'id': /#[-:\.\w]+/,
            'attribute': /\[[^\]]+\]/
        }
    };

    Prism.languages.insertBefore('css', 'function', {
        'hexcode': /#[\da-f]{3,6}/i,
        'entity': /\\[\da-f]{1,8}/i,
        'number': /[\d%\.]+/
    });
    Prism.languages.git = {
        /*
         * A simple one line comment like in a git status command
         * For instance:
         * $ git status
         * # On branch infinite-scroll
         * # Your branch and 'origin/sharedBranches/frontendTeam/infinite-scroll' have diverged,
         * # and have 1 and 2 different commits each, respectively.
         * nothing to commit (working directory clean)
         */
        'comment': /^#.*/m,

        /*
         * Regexp to match the changed lines in a git diff output. Check the example below.
         */
        'deleted': /^[-–].*/m,
        'inserted': /^\+.*/m,

        /*
         * a string (double and simple quote)
         */
        'string': /("|')(\\?.)*?\1/m,

        /*
         * a git command. It starts with a random prompt finishing by a $, then "git" then some other parameters
         * For instance:
         * $ git add file.txt
         */
        'command': {
            pattern: /^.*\$ git .*$/m,
            inside: {
                /*
                 * A git command can contain a parameter starting by a single or a double dash followed by a string
                 * For instance:
                 * $ git diff --cached
                 * $ git log -p
                 */
                'parameter': /\s(--|-)\w+/m
            }
        },

        /*
         * Coordinates displayed in a git diff command
         * For instance:
         * $ git diff
         * diff --git file.txt file.txt
         * index 6214953..1d54a52 100644
         * --- file.txt
         * +++ file.txt
         * @@ -1 +1,2 @@
         * -Here's my tetx file
         * +Here's my text file
         * +And this is the second line
         */
        'coord': /^@@.*@@$/m,

        /*
         * Match a "commit [SHA1]" line in a git log output.
         * For instance:
         * $ git log
         * commit a11a14ef7e26f2ca62d4b35eac455ce636d0dc09
         * Author: lgiraudel
         * Date:   Mon Feb 17 11:18:34 2014 +0100
         *
         *     Add of a new line
         */
        'commit_sha1': /^commit \w{40}$/m
    };

    Prism.languages.java = Prism.languages.extend('clike', {
        'keyword': /\b(abstract|continue|for|new|switch|assert|default|goto|package|synchronized|boolean|do|if|private|this|break|double|implements|protected|throw|byte|else|import|public|throws|case|enum|instanceof|return|transient|catch|extends|int|short|try|char|final|interface|static|void|class|finally|long|strictfp|volatile|const|float|native|super|while)\b/,
        'number': /\b0b[01]+\b|\b0x[\da-f]*\.?[\da-fp\-]+\b|\b\d*\.?\d+(?:e[+-]?\d+)?[df]?\b/i,
        'operator': {
            pattern: /(^|[^.])(?:\+[+=]?|-[-=]?|!=?|<<?=?|>>?>?=?|==?|&[&=]?|\|[|=]?|\*=?|\/=?|%=?|\^=?|[?:~])/m,
            lookbehind: true
        }
    });

    Prism.languages.insertBefore('java', 'function', {
        'annotation': {
            alias: 'punctuation',
            pattern: /(^|[^.])@\w+/,
            lookbehind: true
        }
    });

    Prism.languages.python = {
        'triple-quoted-string': {
            pattern: /"""[\s\S]+?"""|'''[\s\S]+?'''/,
            alias: 'string'
        },
        'comment': {
            pattern: /(^|[^\\])#.*/,
            lookbehind: true
        },
        'string': {
            pattern: /("|')(?:\\\\|\\?[^\\\r\n])*?\1/,
            greedy: true
        },
        'function': {
            pattern: /((?:^|\s)def[ \t]+)[a-zA-Z_][a-zA-Z0-9_]*(?=\()/g,
            lookbehind: true
        },
        'class-name': {
            pattern: /(\bclass\s+)[a-z0-9_]+/i,
            lookbehind: true
        },
        'keyword': /\b(?:as|assert|async|await|break|class|continue|def|del|elif|else|except|exec|finally|for|from|global|if|import|in|is|lambda|pass|print|raise|return|try|while|with|yield)\b/,
        'boolean': /\b(?:True|False)\b/,
        'number': /\b-?(?:0[bo])?(?:(?:\d|0x[\da-f])[\da-f]*\.?\d*|\.\d+)(?:e[+-]?\d+)?j?\b/i,
        'operator': /[-+%=]=?|!=|\*\*?=?|\/\/?=?|<[<=>]?|>[=>]?|[&|^~]|\b(?:or|and|not)\b/,
        'punctuation': /[{}[\];(),.:]/
    };

    Prism.languages.sql = {
        'comment': {
            pattern: /(^|[^\\])(?:\/\*[\w\W]*?\*\/|(?:--|\/\/|#).*)/,
            lookbehind: true
        },
        'string': {
            pattern: /(^|[^@\\])("|')(?:\\?[\s\S])*?\2/,
            lookbehind: true
        },
        'variable': /@[\w.$]+|@("|'|`)(?:\\?[\s\S])+?\1/,
        'function': /\b(?:COUNT|SUM|AVG|MIN|MAX|FIRST|LAST|UCASE|LCASE|MID|LEN|ROUND|NOW|FORMAT)(?=\s*\()/i, // Should we highlight user defined functions too?
        'keyword': /\b(?:ACTION|ADD|AFTER|ALGORITHM|ALL|ALTER|ANALYZE|ANY|APPLY|AS|ASC|AUTHORIZATION|AUTO_INCREMENT|BACKUP|BDB|BEGIN|BERKELEYDB|BIGINT|BINARY|BIT|BLOB|BOOL|BOOLEAN|BREAK|BROWSE|BTREE|BULK|BY|CALL|CASCADED?|CASE|CHAIN|CHAR VARYING|CHARACTER (?:SET|VARYING)|CHARSET|CHECK|CHECKPOINT|CLOSE|CLUSTERED|COALESCE|COLLATE|COLUMN|COLUMNS|COMMENT|COMMIT|COMMITTED|COMPUTE|CONNECT|CONSISTENT|CONSTRAINT|CONTAINS|CONTAINSTABLE|CONTINUE|CONVERT|CREATE|CROSS|CURRENT(?:_DATE|_TIME|_TIMESTAMP|_USER)?|CURSOR|DATA(?:BASES?)?|DATE(?:TIME)?|DBCC|DEALLOCATE|DEC|DECIMAL|DECLARE|DEFAULT|DEFINER|DELAYED|DELETE|DELIMITER(?:S)?|DENY|DESC|DESCRIBE|DETERMINISTIC|DISABLE|DISCARD|DISK|DISTINCT|DISTINCTROW|DISTRIBUTED|DO|DOUBLE(?: PRECISION)?|DROP|DUMMY|DUMP(?:FILE)?|DUPLICATE KEY|ELSE|ENABLE|ENCLOSED BY|END|ENGINE|ENUM|ERRLVL|ERRORS|ESCAPE(?:D BY)?|EXCEPT|EXEC(?:UTE)?|EXISTS|EXIT|EXPLAIN|EXTENDED|FETCH|FIELDS|FILE|FILLFACTOR|FIRST|FIXED|FLOAT|FOLLOWING|FOR(?: EACH ROW)?|FORCE|FOREIGN|FREETEXT(?:TABLE)?|FROM|FULL|FUNCTION|GEOMETRY(?:COLLECTION)?|GLOBAL|GOTO|GRANT|GROUP|HANDLER|HASH|HAVING|HOLDLOCK|IDENTITY(?:_INSERT|COL)?|IF|IGNORE|IMPORT|INDEX|INFILE|INNER|INNODB|INOUT|INSERT|INT|INTEGER|INTERSECT|INTO|INVOKER|ISOLATION LEVEL|JOIN|KEYS?|KILL|LANGUAGE SQL|LAST|LEFT|LIMIT|LINENO|LINES|LINESTRING|LOAD|LOCAL|LOCK|LONG(?:BLOB|TEXT)|MATCH(?:ED)?|MEDIUM(?:BLOB|INT|TEXT)|MERGE|MIDDLEINT|MODIFIES SQL DATA|MODIFY|MULTI(?:LINESTRING|POINT|POLYGON)|NATIONAL(?: CHAR VARYING| CHARACTER(?: VARYING)?| VARCHAR)?|NATURAL|NCHAR(?: VARCHAR)?|NEXT|NO(?: SQL|CHECK|CYCLE)?|NONCLUSTERED|NULLIF|NUMERIC|OFF?|OFFSETS?|ON|OPEN(?:DATASOURCE|QUERY|ROWSET)?|OPTIMIZE|OPTION(?:ALLY)?|ORDER|OUT(?:ER|FILE)?|OVER|PARTIAL|PARTITION|PERCENT|PIVOT|PLAN|POINT|POLYGON|PRECEDING|PRECISION|PREV|PRIMARY|PRINT|PRIVILEGES|PROC(?:EDURE)?|PUBLIC|PURGE|QUICK|RAISERROR|READ(?:S SQL DATA|TEXT)?|REAL|RECONFIGURE|REFERENCES|RELEASE|RENAME|REPEATABLE|REPLICATION|REQUIRE|RESTORE|RESTRICT|RETURNS?|REVOKE|RIGHT|ROLLBACK|ROUTINE|ROW(?:COUNT|GUIDCOL|S)?|RTREE|RULE|SAVE(?:POINT)?|SCHEMA|SELECT|SERIAL(?:IZABLE)?|SESSION(?:_USER)?|SET(?:USER)?|SHARE MODE|SHOW|SHUTDOWN|SIMPLE|SMALLINT|SNAPSHOT|SOME|SONAME|START(?:ING BY)?|STATISTICS|STATUS|STRIPED|SYSTEM_USER|TABLES?|TABLESPACE|TEMP(?:ORARY|TABLE)?|TERMINATED BY|TEXT(?:SIZE)?|THEN|TIMESTAMP|TINY(?:BLOB|INT|TEXT)|TOP?|TRAN(?:SACTIONS?)?|TRIGGER|TRUNCATE|TSEQUAL|TYPES?|UNBOUNDED|UNCOMMITTED|UNDEFINED|UNION|UNIQUE|UNPIVOT|UPDATE(?:TEXT)?|USAGE|USE|USER|USING|VALUES?|VAR(?:BINARY|CHAR|CHARACTER|YING)|VIEW|WAITFOR|WARNINGS|WHEN|WHERE|WHILE|WITH(?: ROLLUP|IN)?|WORK|WRITE(?:TEXT)?)\b/i,
        'boolean': /\b(?:TRUE|FALSE|NULL)\b/i,
        'number': /\b-?(?:0x)?\d*\.?[\da-f]+\b/,
        'operator': /[-+*\/=%^~]|&&?|\|?\||!=?|<(?:=>?|<|>)?|>[>=]?|\b(?:AND|BETWEEN|IN|LIKE|NOT|OR|IS|DIV|REGEXP|RLIKE|SOUNDS LIKE|XOR)\b/i,
        'punctuation': /[;[\]()`,.]/
    };
    (function () {

        if (typeof self === 'undefined' || !self.Prism || !self.document) {
            return;
        }

        Prism.hooks.add('complete', function (env) {
            if (!env.code) {
                return;
            }

            // works only for <code> wrapped inside <pre> (not inline)
            var pre = env.element.parentNode;
            var clsReg = /\s*\bline-numbers\b\s*/;
            if (
                !pre || !/pre/i.test(pre.nodeName) ||
                // Abort only if nor the <pre> nor the <code> have the class
                (!clsReg.test(pre.className) && !clsReg.test(env.element.className))
            ) {
                return;
            }

            if (env.element.querySelector(".line-numbers-rows")) {
                // Abort if line numbers already exists
                return;
            }

            if (clsReg.test(env.element.className)) {
                // Remove the class "line-numbers" from the <code>
                env.element.className = env.element.className.replace(clsReg, '');
            }
            if (!clsReg.test(pre.className)) {
                // Add the class "line-numbers" to the <pre>
                pre.className += ' line-numbers';
            }

            var match = env.code.match(/\n(?!$)/g);
            var linesNum = match ? match.length + 1 : 1;
            var lineNumbersWrapper;

            var lines = new Array(linesNum + 1);
            lines = lines.join('<span></span>');

            lineNumbersWrapper = document.createElement('span');
            lineNumbersWrapper.setAttribute('aria-hidden', 'true');
            lineNumbersWrapper.className = 'line-numbers-rows';
            lineNumbersWrapper.innerHTML = lines;

            if (pre.hasAttribute('data-start')) {
                pre.style.counterReset = 'linenumber ' + (parseInt(pre.getAttribute('data-start'), 10) - 1);
            }

            env.element.appendChild(lineNumbersWrapper);

        });

    }());
    (function () {

        if (
            typeof self !== 'undefined' && !self.Prism ||
            typeof global !== 'undefined' && !global.Prism
        ) {
            return;
        }

        var url = /\b([a-z]{3,7}:\/\/|tel:)[\w\-+%~/.:#=?&amp;]+/,
            email = /\b\S+@[\w.]+[a-z]{2}/,
            linkMd = /\[([^\]]+)]\(([^)]+)\)/,

            // Tokens that may contain URLs and emails
            candidates = ['comment', 'url', 'attr-value', 'string'];

        Prism.plugins.autolinker = {
            processGrammar: function (grammar) {
                // Abort if grammar has already been processed
                if (!grammar || grammar['url-link']) {
                    return;
                }
                Prism.languages.DFS(grammar, function (key, def, type) {
                    if (candidates.indexOf(type) > -1 && Prism.util.type(def) !== 'Array') {
                        if (!def.pattern) {
                            def = this[key] = {
                                pattern: def
                            };
                        }

                        def.inside = def.inside || {};

                        if (type == 'comment') {
                            def.inside['md-link'] = linkMd;
                        }
                        if (type == 'attr-value') {
                            Prism.languages.insertBefore('inside', 'punctuation', { 'url-link': url }, def);
                        }
                        else {
                            def.inside['url-link'] = url;
                        }

                        def.inside['email-link'] = email;
                    }
                });
                grammar['url-link'] = url;
                grammar['email-link'] = email;
            }
        };

        Prism.hooks.add('before-highlight', function (env) {
            Prism.plugins.autolinker.processGrammar(env.grammar);
        });

        Prism.hooks.add('wrap', function (env) {
            if (/-link$/.test(env.type)) {
                env.tag = 'a';

                var href = env.content;

                if (env.type == 'email-link' && href.indexOf('mailto:') != 0) {
                    href = 'mailto:' + href;
                }
                else if (env.type == 'md-link') {
                    // Markdown
                    var match = env.content.match(linkMd);

                    href = match[2];
                    env.content = match[1];
                }

                env.attributes.href = href;
            }
        });

    })();

} catch (Exception) {
    // throw if ie bellow 9
}
