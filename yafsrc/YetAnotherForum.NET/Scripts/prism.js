/* http://prismjs.com/download.html?themes=prism-funky&languages=markup+css+clike+javascript+aspnet+c+csharp+cpp+css-extras+git+java+python+sql&plugins=line-numbers+autolinker */
try {
    self = (typeof window !== 'undefined')
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
        var lang = /\blang(?:uage)?-(?!\*)(\w+)\b/i;

        var _ = self.Prism = {
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
                            return o.map(function (v) { return _.util.clone(v); });
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
                DFS: function (o, callback, type) {
                    for (var i in o) {
                        if (o.hasOwnProperty(i)) {
                            callback.call(o, i, o[i], type || i);

                            if (_.util.type(o[i]) === 'Object') {
                                _.languages.DFS(o[i], callback);
                            }
                            else if (_.util.type(o[i]) === 'Array') {
                                _.languages.DFS(o[i], callback, i);
                            }
                        }
                    }
                }
            },

            highlightAll: function (async, callback) {
                var elements = document.querySelectorAll('code[class*="language-"], [class*="language-"] code, code[class*="lang-"], [class*="lang-"] code');

                for (var i = 0, element; element = elements[i++];) {
                    _.highlightElement(element, async === true, callback);
                }
            },

            highlightElement: function (element, async, callback) {
                // Find language
                var language, grammar, parent = element;

                while (parent && !lang.test(parent.className)) {
                    parent = parent.parentNode;
                }

                if (parent) {
                    language = (parent.className.match(lang) || [, ''])[1];
                    grammar = _.languages[language];
                }

                if (!grammar) {
                    return;
                }

                // Set language on the element, if not present
                element.className = element.className.replace(lang, '').replace(/\s+/g, ' ') + ' language-' + language;

                // Set language on the parent, for styling
                parent = element.parentNode;

                if (/pre/i.test(parent.nodeName)) {
                    parent.className = parent.className.replace(lang, '').replace(/\s+/g, ' ') + ' language-' + language;
                }

                var code = element.textContent;

                if (!code) {
                    return;
                }

                code = code.replace(/^(?:\r?\n|\r)/, '');

                var env = {
                    element: element,
                    language: language,
                    grammar: grammar,
                    code: code
                };

                _.hooks.run('before-highlight', env);

                if (async && self.Worker) {
                    var worker = new Worker(_.filename);

                    worker.onmessage = function (evt) {
                        env.highlightedCode = Token.stringify(JSON.parse(evt.data), language);

                        _.hooks.run('before-insert', env);

                        env.element.innerHTML = env.highlightedCode;

                        callback && callback.call(env.element);
                        _.hooks.run('after-highlight', env);
                    };

                    worker.postMessage(JSON.stringify({
                        language: env.language,
                        code: env.code
                    }));
                }
                else {
                    env.highlightedCode = _.highlight(env.code, env.grammar, env.language);

                    _.hooks.run('before-insert', env);

                    env.element.innerHTML = env.highlightedCode;

                    callback && callback.call(element);

                    _.hooks.run('after-highlight', env);
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
                            lookbehindLength = 0,
                            alias = pattern.alias;

                        pattern = pattern.pattern || pattern;

                        for (var i = 0; i < strarr.length; i++) { // Don’t cache length as it changes during the loop

                            var str = strarr[i];

                            if (strarr.length > text.length) {
                                // Something went terribly wrong, ABORT, ABORT!
                                break tokenloop;
                            }

                            if (str instanceof Token) {
                                continue;
                            }

                            pattern.lastIndex = 0;

                            var match = pattern.exec(str);

                            if (match) {
                                if (lookbehind) {
                                    lookbehindLength = match[1].length;
                                }

                                var from = match.index - 1 + lookbehindLength,
                                    match = match[0].slice(lookbehindLength),
                                    len = match.length,
                                    to = from + len,
                                    before = str.slice(0, from + 1),
                                    after = str.slice(to + 1);

                                var args = [i, 1];

                                if (before) {
                                    args.push(before);
                                }

                                var wrapped = new Token(token, inside ? _.tokenize(match, inside) : match, alias);

                                args.push(wrapped);

                                if (after) {
                                    args.push(after);
                                }

                                Array.prototype.splice.apply(strarr, args);
                            }
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

        var Token = _.Token = function (type, content, alias) {
            this.type = type;
            this.content = content;
            this.alias = alias;
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
                attributes += name + '="' + (env.attributes[name] || '') + '"';
            }

            return '<' + env.tag + ' class="' + env.classes.join(' ') + '" ' + attributes + '>' + env.content + '</' + env.tag + '>';

        };

        if (!self.document) {
            if (!self.addEventListener) {
                // in Node.js
                return self.Prism;
            }
            // In worker
            self.addEventListener('message', function (evt) {
                var message = JSON.parse(evt.data),
                    lang = message.language,
                    code = message.code;

                self.postMessage(JSON.stringify(_.util.encode(_.tokenize(code, _.languages[lang]))));
                self.close();
            }, false);

            return self.Prism;
        }

        // Get current script and highlight
        var script = document.getElementsByTagName('script');

        script = script[script.length - 1];

        if (script) {
            _.filename = script.src;

            if (document.addEventListener && !script.hasAttribute('data-manual')) {
                document.addEventListener('DOMContentLoaded', _.highlightAll);
            }
        }

        return self.Prism;

    })();

    if (typeof module !== 'undefined' && module.exports) {
        module.exports = Prism;
    }
    ;
    Prism.languages.markup = {
        'comment': /<!--[\w\W]*?-->/,
        'prolog': /<\?.+?\?>/,
        'doctype': /<!DOCTYPE.+?>/,
        'cdata': /<!\[CDATA\[[\w\W]*?]]>/i,
        'tag': {
            pattern: /<\/?[\w:-]+\s*(?:\s+[\w:-]+(?:=(?:("|')(\\?[\w\W])*?\1|[^\s'">=]+))?\s*)*\/?>/i,
            inside: {
                'tag': {
                    pattern: /^<\/?[\w:-]+/i,
                    inside: {
                        'punctuation': /^<\/?/,
                        'namespace': /^[\w-]+?:/
                    }
                },
                'attr-value': {
                    pattern: /=(?:('|")[\w\W]*?(\1)|[^\s>]+)/i,
                    inside: {
                        'punctuation': /=|>|"/
                    }
                },
                'punctuation': /\/?>/,
                'attr-name': {
                    pattern: /[\w:-]+/,
                    inside: {
                        'namespace': /^[\w-]+?:/
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
    ;
    Prism.languages.css = {
        'comment': /\/\*[\w\W]*?\*\//,
        'atrule': {
            pattern: /@[\w-]+?.*?(;|(?=\s*\{))/i,
            inside: {
                'punctuation': /[;:]/
            }
        },
        'url': /url\((?:(["'])(\\\n|\\?.)*?\1|.*?)\)/i,
        'selector': /[^\{\}\s][^\{\};]*(?=\s*\{)/,
        'string': /("|')(\\\n|\\?.)*?\1/,
        'property': /(\b|\B)[\w-]+(?=\s*:)/i,
        'important': /\B!important\b/i,
        'punctuation': /[\{\};:]/,
        'function': /[-a-z0-9]+(?=\()/i
    };

    if (Prism.languages.markup) {
        Prism.languages.insertBefore('markup', 'tag', {
            'style': {
                pattern: /<style[\w\W]*?>[\w\W]*?<\/style>/i,
                inside: {
                    'tag': {
                        pattern: /<style[\w\W]*?>|<\/style>/i,
                        inside: Prism.languages.markup.tag.inside
                    },
                    rest: Prism.languages.css
                },
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
        'string': /("|')(\\\n|\\?.)*?\1/,
        'class-name': {
            pattern: /((?:(?:class|interface|extends|implements|trait|instanceof|new)\s+)|(?:catch\s+\())[a-z0-9_\.\\]+/i,
            lookbehind: true,
            inside: {
                punctuation: /(\.|\\)/
            }
        },
        'keyword': /\b(if|else|while|do|for|return|in|instanceof|function|new|try|throw|catch|finally|null|break|continue)\b/,
        'boolean': /\b(true|false)\b/,
        'function': {
            pattern: /[a-z0-9_]+\(/i,
            inside: {
                punctuation: /\(/
            }
        },
        'number': /\b-?(0x[\dA-Fa-f]+|\d*\.?\d+([Ee]-?\d+)?)\b/,
        'operator': /[-+]{1,2}|!|<=?|>=?|={1,3}|&{1,2}|\|?\||\?|\*|\/|~|\^|%/,
        'ignore': /&(lt|gt|amp);/i,
        'punctuation': /[{}[\];(),.:]/
    };
    ;
    Prism.languages.javascript = Prism.languages.extend('clike', {
        'keyword': /\b(break|case|catch|class|const|continue|debugger|default|delete|do|else|enum|export|extends|false|finally|for|function|get|if|implements|import|in|instanceof|interface|let|new|null|package|private|protected|public|return|set|static|super|switch|this|throw|true|try|typeof|var|void|while|with|yield)\b/,
        'number': /\b-?(0x[\dA-Fa-f]+|\d*\.?\d+([Ee][+-]?\d+)?|NaN|-?Infinity)\b/,
        'function': /(?!\d)[a-z0-9_$]+(?=\()/i
    });

    Prism.languages.insertBefore('javascript', 'keyword', {
        'regex': {
            pattern: /(^|[^/])\/(?!\/)(\[.+?]|\\.|[^/\r\n])+\/[gim]{0,3}(?=\s*($|[\r\n,.;})]))/,
            lookbehind: true
        }
    });

    if (Prism.languages.markup) {
        Prism.languages.insertBefore('markup', 'tag', {
            'script': {
                pattern: /<script[\w\W]*?>[\w\W]*?<\/script>/i,
                inside: {
                    'tag': {
                        pattern: /<script[\w\W]*?>|<\/script>/i,
                        inside: Prism.languages.markup.tag.inside
                    },
                    rest: Prism.languages.javascript
                },
                alias: 'language-javascript'
            }
        });
    }
    ;
    Prism.languages.aspnet = Prism.languages.extend('markup', {
        'page-directive tag': {
            pattern: /<%\s*@.*%>/i,
            inside: {
                'page-directive tag': /<%\s*@\s*(?:Assembly|Control|Implements|Import|Master|MasterType|OutputCache|Page|PreviousPageType|Reference|Register)?|%>/i,
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
            pattern: /<script(?=.*runat=['"]?server['"]?)[\w\W]*?>[\w\W]*?<\/script>/i,
            inside: {
                tag: {
                    pattern: /<\/?script\s*(?:\s+[\w:-]+(?:=(?:("|')(\\?[\w\W])*?\1|\w+))?\s*)*\/?>/i,
                    inside: Prism.languages.aspnet.tag.inside
                },
                rest: Prism.languages.csharp || {}
            }
        }
    });

    // Hacks to fix eager tag matching finishing too early: <script src="<% Foo.Bar %>"> => <script src="<% Foo.Bar %>
    if (Prism.languages.aspnet.style) {
        Prism.languages.aspnet.style.inside.tag.pattern = /<\/?style\s*(?:\s+[\w:-]+(?:=(?:("|')(\\?[\w\W])*?\1|\w+))?\s*)*\/?>/i;
        Prism.languages.aspnet.style.inside.tag.inside = Prism.languages.aspnet.tag.inside;
    }
    if (Prism.languages.aspnet.script) {
        Prism.languages.aspnet.script.inside.tag.pattern = Prism.languages.aspnet['asp script'].inside.tag.pattern;
        Prism.languages.aspnet.script.inside.tag.inside = Prism.languages.aspnet.tag.inside;
    };
    Prism.languages.c = Prism.languages.extend('clike', {
        // allow for c multiline strings
        'string': /("|')([^\n\\\1]|\\.|\\\r*\n)*?\1/,
        'keyword': /\b(asm|typeof|inline|auto|break|case|char|const|continue|default|do|double|else|enum|extern|float|for|goto|if|int|long|register|return|short|signed|sizeof|static|struct|switch|typedef|union|unsigned|void|volatile|while)\b/,
        'operator': /[-+]{1,2}|!=?|<{1,2}=?|>{1,2}=?|\->|={1,2}|\^|~|%|&{1,2}|\|?\||\?|\*|\//
    });

    Prism.languages.insertBefore('c', 'string', {
        // property class reused for macro statements
        'property': {
            // allow for multiline macro definitions
            // spaces after the # character compile fine with gcc
            pattern: /((^|\n)\s*)#\s*[a-z]+([^\n\\]|\\.|\\\r*\n)*/i,
            lookbehind: true,
            inside: {
                // highlight the path of the include statement as a string
                'string': {
                    pattern: /(#\s*include\s*)(<.+?>|("|')(\\?.)+?\3)/,
                    lookbehind: true
                }
            }
        }
    });

    delete Prism.languages.c['class-name'];
    delete Prism.languages.c['boolean'];;
    Prism.languages.csharp = Prism.languages.extend('clike', {
        'keyword': /\b(abstract|as|async|await|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|async|await|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b/,
        'string': [
            /@("|')(\1\1|\\\1|\\?(?!\1)[\s\S])*\1/,
            /("|')(\\?.)*?\1/
        ],
        'preprocessor': /^\s*#.*/m,
        'number': /\b-?(0x[\da-f]+|\d*\.?\d+)\b/i
    });
    ;
    Prism.languages.cpp = Prism.languages.extend('c', {
        'keyword': /\b(alignas|alignof|asm|auto|bool|break|case|catch|char|char16_t|char32_t|class|compl|const|constexpr|const_cast|continue|decltype|default|delete|delete\[\]|do|double|dynamic_cast|else|enum|explicit|export|extern|float|for|friend|goto|if|inline|int|long|mutable|namespace|new|new\[\]|noexcept|nullptr|operator|private|protected|public|register|reinterpret_cast|return|short|signed|sizeof|static|static_assert|static_cast|struct|switch|template|this|thread_local|throw|try|typedef|typeid|typename|union|unsigned|using|virtual|void|volatile|wchar_t|while)\b/,
        'boolean': /\b(true|false)\b/,
        'operator': /[-+]{1,2}|!=?|<{1,2}=?|>{1,2}=?|\->|:{1,2}|={1,2}|\^|~|%|&{1,2}|\|?\||\?|\*|\/|\b(and|and_eq|bitand|bitor|not|not_eq|or|or_eq|xor|xor_eq)\b/
    });

    Prism.languages.insertBefore('cpp', 'keyword', {
        'class-name': {
            pattern: /(class\s+)[a-z0-9_]+/i,
            lookbehind: true
        }
    });;
    Prism.languages.css.selector = {
        pattern: /[^\{\}\s][^\{\}]*(?=\s*\{)/,
        inside: {
            'pseudo-element': /:(?:after|before|first-letter|first-line|selection)|::[-\w]+/,
            'pseudo-class': /:[-\w]+(?:\(.*\))?/,
            'class': /\.[-:\.\w]+/,
            'id': /#[-:\.\w]+/
        }
    };

    Prism.languages.insertBefore('css', 'function', {
        'hexcode': /#[\da-f]{3,6}/i,
        'entity': /\\[\da-f]{1,8}/i,
        'number': /[\d%\.]+/
    });;
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
        'comment': /^#.*$/m,

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
         * Regexp to match the changed lines in a git diff output. Check the example above.
         */
        'deleted': /^-(?!-).+$/m,
        'inserted': /^\+(?!\+).+$/m,

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
    ;
    Prism.languages.java = Prism.languages.extend('clike', {
        'keyword': /\b(abstract|continue|for|new|switch|assert|default|goto|package|synchronized|boolean|do|if|private|this|break|double|implements|protected|throw|byte|else|import|public|throws|case|enum|instanceof|return|transient|catch|extends|int|short|try|char|final|interface|static|void|class|finally|long|strictfp|volatile|const|float|native|super|while)\b/,
        'number': /\b0b[01]+\b|\b0x[\da-f]*\.?[\da-fp\-]+\b|\b\d*\.?\d+[e]?[\d]*[df]\b|\b\d*\.?\d+\b/i,
        'operator': {
            pattern: /(^|[^\.])(?:\+=|\+\+?|-=|--?|!=?|<{1,2}=?|>{1,3}=?|==?|&=|&&?|\|=|\|\|?|\?|\*=?|\/=?|%=?|\^=?|:|~)/m,
            lookbehind: true
        }
    });;
    Prism.languages.python = {
        'comment': {
            pattern: /(^|[^\\])#.*?(\r?\n|$)/,
            lookbehind: true
        },
        'string': /"""[\s\S]+?"""|'''[\s\S]+?'''|("|')(\\?.)*?\1/,
        'keyword': /\b(as|assert|break|class|continue|def|del|elif|else|except|exec|finally|for|from|global|if|import|in|is|lambda|pass|print|raise|return|try|while|with|yield)\b/,
        'boolean': /\b(True|False)\b/,
        'number': /\b-?(0[box])?(?:[\da-f]+\.?\d*|\.\d+)(?:e[+-]?\d+)?j?\b/i,
        'operator': /[-+]|<=?|>=?|!|={1,2}|&{1,2}|\|?\||\?|\*|\/|~|\^|%|\b(or|and|not)\b/,
        'punctuation': /[{}[\];(),.:]/
    };

    ;
    Prism.languages.sql = {
        'comment': {
            pattern: /(^|[^\\])(\/\*[\w\W]*?\*\/|((--)|(\/\/)|#).*?(\r?\n|$))/,
            lookbehind: true
        },
        'string': {
            pattern: /(^|[^@])("|')(\\?[\s\S])*?\2/,
            lookbehind: true
        },
        'variable': /@[\w.$]+|@("|'|`)(\\?[\s\S])+?\1/,
        'function': /\b(?:COUNT|SUM|AVG|MIN|MAX|FIRST|LAST|UCASE|LCASE|MID|LEN|ROUND|NOW|FORMAT)(?=\s*\()/i, // Should we highlight user defined functions too?
        'keyword': /\b(?:ACTION|ADD|AFTER|ALGORITHM|ALTER|ANALYZE|APPLY|AS|ASC|AUTHORIZATION|BACKUP|BDB|BEGIN|BERKELEYDB|BIGINT|BINARY|BIT|BLOB|BOOL|BOOLEAN|BREAK|BROWSE|BTREE|BULK|BY|CALL|CASCADE|CASCADED|CASE|CHAIN|CHAR VARYING|CHARACTER VARYING|CHECK|CHECKPOINT|CLOSE|CLUSTERED|COALESCE|COLUMN|COLUMNS|COMMENT|COMMIT|COMMITTED|COMPUTE|CONNECT|CONSISTENT|CONSTRAINT|CONTAINS|CONTAINSTABLE|CONTINUE|CONVERT|CREATE|CROSS|CURRENT|CURRENT_DATE|CURRENT_TIME|CURRENT_TIMESTAMP|CURRENT_USER|CURSOR|DATA|DATABASE|DATABASES|DATETIME|DBCC|DEALLOCATE|DEC|DECIMAL|DECLARE|DEFAULT|DEFINER|DELAYED|DELETE|DENY|DESC|DESCRIBE|DETERMINISTIC|DISABLE|DISCARD|DISK|DISTINCT|DISTINCTROW|DISTRIBUTED|DO|DOUBLE|DOUBLE PRECISION|DROP|DUMMY|DUMP|DUMPFILE|DUPLICATE KEY|ELSE|ENABLE|ENCLOSED BY|END|ENGINE|ENUM|ERRLVL|ERRORS|ESCAPE|ESCAPED BY|EXCEPT|EXEC|EXECUTE|EXIT|EXPLAIN|EXTENDED|FETCH|FIELDS|FILE|FILLFACTOR|FIRST|FIXED|FLOAT|FOLLOWING|FOR|FOR EACH ROW|FORCE|FOREIGN|FREETEXT|FREETEXTTABLE|FROM|FULL|FUNCTION|GEOMETRY|GEOMETRYCOLLECTION|GLOBAL|GOTO|GRANT|GROUP|HANDLER|HASH|HAVING|HOLDLOCK|IDENTITY|IDENTITY_INSERT|IDENTITYCOL|IF|IGNORE|IMPORT|INDEX|INFILE|INNER|INNODB|INOUT|INSERT|INT|INTEGER|INTERSECT|INTO|INVOKER|ISOLATION LEVEL|JOIN|KEY|KEYS|KILL|LANGUAGE SQL|LAST|LEFT|LIMIT|LINENO|LINES|LINESTRING|LOAD|LOCAL|LOCK|LONGBLOB|LONGTEXT|MATCH|MATCHED|MEDIUMBLOB|MEDIUMINT|MEDIUMTEXT|MERGE|MIDDLEINT|MODIFIES SQL DATA|MODIFY|MULTILINESTRING|MULTIPOINT|MULTIPOLYGON|NATIONAL|NATIONAL CHAR VARYING|NATIONAL CHARACTER|NATIONAL CHARACTER VARYING|NATIONAL VARCHAR|NATURAL|NCHAR|NCHAR VARCHAR|NEXT|NO|NO SQL|NOCHECK|NOCYCLE|NONCLUSTERED|NULLIF|NUMERIC|OF|OFF|OFFSETS|ON|OPEN|OPENDATASOURCE|OPENQUERY|OPENROWSET|OPTIMIZE|OPTION|OPTIONALLY|ORDER|OUT|OUTER|OUTFILE|OVER|PARTIAL|PARTITION|PERCENT|PIVOT|PLAN|POINT|POLYGON|PRECEDING|PRECISION|PREV|PRIMARY|PRINT|PRIVILEGES|PROC|PROCEDURE|PUBLIC|PURGE|QUICK|RAISERROR|READ|READS SQL DATA|READTEXT|REAL|RECONFIGURE|REFERENCES|RELEASE|RENAME|REPEATABLE|REPLICATION|REQUIRE|RESTORE|RESTRICT|RETURN|RETURNS|REVOKE|RIGHT|ROLLBACK|ROUTINE|ROWCOUNT|ROWGUIDCOL|ROWS?|RTREE|RULE|SAVE|SAVEPOINT|SCHEMA|SELECT|SERIAL|SERIALIZABLE|SESSION|SESSION_USER|SET|SETUSER|SHARE MODE|SHOW|SHUTDOWN|SIMPLE|SMALLINT|SNAPSHOT|SOME|SONAME|START|STARTING BY|STATISTICS|STATUS|STRIPED|SYSTEM_USER|TABLE|TABLES|TABLESPACE|TEMP(?:ORARY)?|TEMPTABLE|TERMINATED BY|TEXT|TEXTSIZE|THEN|TIMESTAMP|TINYBLOB|TINYINT|TINYTEXT|TO|TOP|TRAN|TRANSACTION|TRANSACTIONS|TRIGGER|TRUNCATE|TSEQUAL|TYPE|TYPES|UNBOUNDED|UNCOMMITTED|UNDEFINED|UNION|UNPIVOT|UPDATE|UPDATETEXT|USAGE|USE|USER|USING|VALUE|VALUES|VARBINARY|VARCHAR|VARCHARACTER|VARYING|VIEW|WAITFOR|WARNINGS|WHEN|WHERE|WHILE|WITH|WITH ROLLUP|WITHIN|WORK|WRITE|WRITETEXT)\b/i,
        'boolean': /\b(?:TRUE|FALSE|NULL)\b/i,
        'number': /\b-?(0x)?\d*\.?[\da-f]+\b/,
        'operator': /\b(?:ALL|AND|ANY|BETWEEN|EXISTS|IN|LIKE|NOT|OR|IS|UNIQUE|CHARACTER SET|COLLATE|DIV|OFFSET|REGEXP|RLIKE|SOUNDS LIKE|XOR)\b|[-+]|!|[=<>]{1,2}|(&){1,2}|\|?\||\?|\*|\//i,
        'punctuation': /[;[\]()`,.]/
    };;
    Prism.hooks.add('after-highlight', function (env) {
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

        if (clsReg.test(env.element.className)) {
            // Remove the class "line-numbers" from the <code>
            env.element.className = env.element.className.replace(clsReg, '');
        }
        if (!clsReg.test(pre.className)) {
            // Add the class "line-numbers" to the <pre>
            pre.className += ' line-numbers';
        }

        var linesNum = (1 + env.code.split('\n').length);
        var lineNumbersWrapper;

        var lines = new Array(linesNum);
        lines = lines.join('<span></span>');

        lineNumbersWrapper = document.createElement('span');
        lineNumbersWrapper.className = 'line-numbers-rows';
        lineNumbersWrapper.innerHTML = lines;

        if (pre.hasAttribute('data-start')) {
            pre.style.counterReset = 'linenumber ' + (parseInt(pre.getAttribute('data-start'), 10) - 1);
        }

        env.element.appendChild(lineNumbersWrapper);

    });;
    (function () {

        if (!self.Prism) {
            return;
        }

        var url = /\b([a-z]{3,7}:\/\/|tel:)[\w\-+%~/.:#=?&amp;]+/,
            email = /\b\S+@[\w.]+[a-z]{2}/,
            linkMd = /\[([^\]]+)]\(([^)]+)\)/,

            // Tokens that may contain URLs and emails
            candidates = ['comment', 'url', 'attr-value', 'string'];

        for (var language in Prism.languages) {
            var tokens = Prism.languages[language];

            Prism.languages.DFS(tokens, function (key, def, type) {
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

            tokens['url-link'] = url;
            tokens['email-link'] = email;
        }

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
    ;

} catch (Exception) {
    // throw if ie bellow 9
}
