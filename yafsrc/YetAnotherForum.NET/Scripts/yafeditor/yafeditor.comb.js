var yafEditor = function(name, urlTitle, urlDescription, urlImageTitle, urlImageDescription, description) {
    this.Name = name;
    this.UrlTitle = urlTitle;
    this.UrlDescription = urlDescription;
    this.UrlImageTitle = urlImageTitle;
    this.UrlImageDescription = urlImageDescription;
    this.Description = description;
    if (document.getElementById("PostAlbumsListPlaceholder") != null) {
        const pageSize = 5;
        const pageNumber = 0;
        getAlbumImagesData(pageSize, pageNumber, false);
    }
    document.querySelector(".BBCodeEditor").addEventListener("keydown", function(e) {
        if (e.ctrlKey && !e.altKey && (e.which == 66 || e.which == 73 || e.which == 85 || e.which == 81 || e.which == 13)) {
            if (e.which == 66) {
                wrapSelection(this, "[b]", "[/b]");
            } else if (e.which == 73) {
                wrapSelection(this, "[i]", "[/i]");
            } else if (e.which == 85) {
                wrapSelection(this, "[u]", "[/u]");
            } else if (e.which == 81) {
                wrapSelection(this, "[quote]", "[/quote]");
            } else if (e.which == 13) {
                if (document.querySelector('[id*="QuickReply"]') != null) {
                    document.querySelector('[id*="QuickReply"]').click();
                } else if (document.querySelector('[id*="PostReply"]') != null) {
                    window.location.href = document.querySelector('[id*="PostReply"]').href;
                }
            }
        }
    });
};

yafEditor.prototype.FormatText = function(command, option) {
    var textObj = document.getElementById(this.Name);
    switch (command) {
      case "bold":
        wrapSelection(textObj, "[b]", "[/b]");
        break;

      case "italic":
        wrapSelection(textObj, "[i]", "[/i]");
        break;

      case "underline":
        wrapSelection(textObj, "[u]", "[/u]");
        break;

      case "highlight":
        wrapSelection(textObj, "[h]", "[/h]");
        break;

      case "code":
        wrapSelection(textObj, "[code]", "[/code]");
        break;

      case "codelang":
        wrapSelection(textObj, "[code=" + option + "]", "[/code]");
        break;

      case "img":
        if (getCurrentSelection(textObj)) {
            wrapSelection(textObj, "[img]", "[/img]");
        } else {
            var descriptionImage = this.Description;
            var titleImage = this.UrlImageDescription;
            bootbox.prompt({
                title: this.UrlImageTitle,
                placeholder: "https://",
                callback: function(url) {
                    if (url !== null && url !== "") {
                        bootbox.prompt({
                            title: titleImage,
                            placeholder: descriptionImage,
                            callback: function(desc) {
                                if (desc !== "" && desc !== null) {
                                    replaceSelection(textObj, "[img=" + url + "]" + desc + "[/img]");
                                } else {
                                    replaceSelection(textObj, "[img]" + url + "[/img]");
                                }
                            }
                        });
                    }
                }
            });
        }
        break;

      case "quote":
        wrapSelection(textObj, "[quote]", "[/quote]");
        break;

      case "justifyleft":
        wrapSelection(textObj, "[left]", "[/left]");
        break;

      case "justifycenter":
        wrapSelection(textObj, "[center]", "[/center]");
        break;

      case "justifyright":
        wrapSelection(textObj, "[right]", "[/right]");
        break;

      case "indent":
        wrapSelection(textObj, "[indent]", "[/indent]");
        break;

      case "outdent":
        if (getCurrentSelection(textObj)) {
            removeFromSelection(textObj, "[indent]", "[/indent]");
        }
        break;

      case "removeFormat":
        if (getCurrentSelection(textObj)) {
            removeFormat(textObj);
        }
        break;

      case "createlink":
        var descriptionUrl = this.Description;
        var titleUrl = this.UrlDescription;
        bootbox.prompt({
            title: this.UrlTitle,
            placeholder: "https://",
            callback: function(url) {
                if (url !== null && url !== "") {
                    if (getCurrentSelection(textObj)) {
                        wrapSelection(textObj, "[url=" + url + "]", "[/url]");
                    } else {
                        bootbox.prompt({
                            title: titleUrl,
                            placeholder: descriptionUrl,
                            callback: function(desc) {
                                if (desc != "" && desc != null) {
                                    replaceSelection(textObj, "[url=" + url + "]" + desc + "[/url]");
                                } else {
                                    replaceSelection(textObj, "[url]" + url + "[/url]");
                                }
                            }
                        });
                    }
                }
            }
        });
        break;

      case "unorderedlist":
        wrapSelection(textObj, "[list][*]", "[/list]");
        break;

      case "orderedlist":
        wrapSelection(textObj, "[list=1][*]", "[/list]");
        break;

      case "color":
        wrapSelection(textObj, "[color=" + option + "]", "[/color]");
        break;

      case "fontsize":
        wrapSelection(textObj, "[size=" + option + "]", "[/size]");
        break;

      case "AlbumImgId":
        replaceSelection(textObj, "[albumimg]" + option + "[/albumimg]");
        break;

      case "attach":
        replaceSelection(textObj, "[attach]" + option + "[/attach]");
        break;

      case "selectAll":
        textObj.select();
        break;

      case "cut":
        if (getCurrentSelection(textObj)) {
            document.execCommand("cut");
        }
        break;

      case "copy":
        if (getCurrentSelection(textObj)) {
            document.execCommand("copy");
        }
        break;

      case "paste":
        navigator.clipboard.readText().then(function(textFromClipboard) {
            textObj.value += textFromClipboard;
        });
        break;

      default:
        wrapSelection(textObj, "[" + command + "]", "[/" + command + "]");
        break;
    }
};

function removeFormat(input) {
    if (input.setSelectionRange) {
        var selectionStart = input.selectionStart;
        var selectionEnd = input.selectionEnd;
        var selectedText = input.value.substring(selectionStart, selectionEnd);
        var regex = /\[.*?\]/g;
        var replacedText = selectedText.replace(regex, "");
        var replacedLength = selectedText.length - replacedText.length;
        input.value = input.value.replace(selectedText, replacedText);
        setSelectionRange(input, selectionStart, selectionEnd - replacedLength);
    }
}

function setSelectionRange(input, selectionStart, selectionEnd) {
    if (input.setSelectionRange) {
        input.focus();
        input.setSelectionRange(selectionStart, selectionEnd);
    } else if (input.createTextRange) {
        var range = input.createTextRange();
        range.collapse(true);
        range.moveEnd("character", selectionEnd);
        range.moveStart("character", selectionStart);
        range.select();
    }
}

function setCaretToPos(input, pos) {
    setSelectionRange(input, pos, pos);
}

function replaceSelection(input, replaceString) {
    if (input.setSelectionRange) {
        var selectionStart = input.selectionStart;
        var selectionEnd = input.selectionEnd;
        input.value = input.value.substring(0, selectionStart) + replaceString + input.value.substring(selectionEnd);
        if (selectionStart != selectionEnd) setSelectionRange(input, selectionStart, selectionStart + replaceString.length); else setCaretToPos(input, selectionStart + replaceString.length);
    } else if (document.selection) {
        input.focus();
        document.selection.createRange().text = replaceString;
    } else {
        input.value += replaceString;
        input.focus();
    }
}

function removeFromSelection(input, preString, postString) {
    if (input.setSelectionRange) {
        var selectionStart = input.selectionStart;
        var selectionEnd = input.selectionEnd;
        var selectedText = input.value.substring(selectionStart, selectionEnd);
        if (selectedText.indexOf(preString) != -1 && selectedText.indexOf(postString) != -1) {
            input.value = input.value.substring(0, selectionStart) + input.value.substring(selectionStart + preString.length, selectionEnd - postString.length) + input.value.substring(selectionEnd);
            if (selectionStart != selectionEnd) {
                setSelectionRange(input, selectionStart, selectionEnd - postString.length - preString.length);
            } else {
                setCaretToPos(input, selectionStart + preString.length);
            }
        }
    }
}

function wrapSelection(input, preString, postString) {
    if (input.setSelectionRange) {
        var selectionStart = input.selectionStart;
        var selectionEnd = input.selectionEnd;
        input.value = input.value.substring(0, selectionStart) + preString + input.value.substring(selectionStart, selectionEnd) + postString + input.value.substring(selectionEnd);
        if (selectionStart != selectionEnd) {
            setSelectionRange(input, selectionStart, preString.length + postString.length + selectionEnd);
        } else {
            setCaretToPos(input, selectionStart + preString.length);
        }
    } else if (document.selection) {
        var sel = document.selection.createRange().text;
        if (sel) {
            document.selection.createRange().text = preString + sel + postString;
            input.focus();
        } else {
            input.value += preString;
            input.focus();
            input.value += postString;
        }
    } else {
        input.value += preString;
        input.focus();
        input.value += postString;
    }
    input.dispatchEvent(new Event("change"));
}

function getCurrentSelection(input) {
    if (input.setSelectionRange) {
        return input.selectionStart != input.selectionEnd;
    } else if (document.selection) {
        var range = document.selection.createRange();
        return range.parentElement() == input && range.text != "";
    } else {
        return false;
    }
}

var doc, bod, I, StateMaker;

addEventListener("load", function() {
    doc = document;
    bod = doc.body;
    I = function(id) {
        return doc.getElementById(id);
    };
    StateMaker = function(initialState) {
        var o = initialState;
        if (o) {
            this.initialState = o;
            this.states = [ o ];
        } else {
            this.states = [];
        }
        this.savedStates = [];
        this.canUndo = this.canRedo = false;
        this.undoneStates = [];
        this.addState = function(state) {
            this.states.push(state);
            this.undoneStates = [];
            this.canUndo = true;
            this.canRedo = false;
            return this;
        };
        this.undo = function() {
            var sl = this.states.length;
            if (this.initialState) {
                if (sl > 1) {
                    this.undoneStates.push(this.states.pop());
                    this.canRedo = true;
                    if (this.states.length < 2) {
                        this.canUndo = false;
                    }
                } else {
                    this.canUndo = false;
                }
            } else if (sl > 0) {
                this.undoneStates.push(this.states.pop());
                this.canRedo = true;
            } else {
                this.canUndo = false;
            }
            return this;
        };
        this.redo = function() {
            if (this.undoneStates.length > 0) {
                this.states.push(this.undoneStates.pop());
                this.canUndo = true;
                if (this.undoneStates.length < 1) {
                    this.canRedo = false;
                }
            } else {
                this.canRedo = false;
            }
            return this;
        };
        this.save = function() {
            this.savedStates = this.states.slice();
            return this;
        };
        this.isSavedState = function() {
            if (JSON.stringify(this.states) !== JSON.stringify(this.savedStates)) {
                return false;
            }
            return true;
        };
    };
    var text = doc.querySelector(".BBCodeEditor"), val, wordCount = 0, words = 0, stateMaker = new StateMaker(), undoButton = I("undo"), redoButton = I("redo"), countField = document.getElementById("editor-Counter"), maxLimit = text.maxLength;
    countField.textContent = maxLimit - text.value.length;
    function onChange(editor) {
        val = editor.value.trim();
        wordCount = val.split(/\s+/).length;
        if (wordCount === words && stateMaker.states.length) {
            stateMaker.states[stateMaker.states.length - 1] = val;
        } else {
            stateMaker.addState(val);
            words = wordCount;
        }
        if (editor.value.length > maxLimit) {
            editor.value = editor.value.substring(0, maxLimit);
        } else {
            countField.textContent = maxLimit - editor.value.length;
        }
    }
    text.addEventListener("change", function(event) {
        onChange(this);
    });
    text.onkeyup = function() {
        onChange(this);
    };
    undoButton.onclick = function() {
        stateMaker.undo();
        val = text.value = (stateMaker.states[stateMaker.states.length - 1] || "").trim();
        text.focus();
        undoButton.disabled = !stateMaker.canUndo;
        redoButton.disabled = !stateMaker.canRedo;
    };
    redoButton.onclick = function() {
        stateMaker.redo();
        val = text.value = (stateMaker.states[stateMaker.states.length - 1] || "").trim();
        text.focus();
        undoButton.disabled = !stateMaker.canUndo;
        redoButton.disabled = !stateMaker.canRedo;
    };
});

var AutoCloseTags = function(textarea) {
    this.textarea = textarea;
    this.autoClosingTags = [ "b", "i", "u", "h", "code", "img", "quote", "left", "center", "right", "indent", "list", "color", "size", "albumimg", "attach", "youtube", "vimeo", "instagram", "twitter", "facebook", "googlewidget", "spoiler", "userlink", "googlemaps", "hide", "group-hide", "hide-thanks", "hide-reply-thanks", "hide-reply", "hide-posts", "dailymotion", "audio" ];
    this.enableAutoCloseTags();
};

AutoCloseTags.prototype = {
    enableAutoCloseTags: function() {
        var self = this;
        this.textarea.addEventListener("keydown", function(event) {
            const keyCode = event.key;
            if (keyCode === "]") {
                const position = this.selectionStart;
                const before = this.value.substr(0, position);
                const after = this.value.substr(this.selectionEnd, this.value.length);
                let tagName;
                try {
                    tagName = before.match(/\[([^\]]+)$/)[1].match(/^([a-z1-6]+)/)[1];
                } catch (e) {
                    return;
                }
                if (-1 === self.autoClosingTags.indexOf(tagName)) return;
                const closeTag = `[/${tagName}]`;
                this.value = before + closeTag + after;
                this.selectionStart = this.selectionEnd = position;
                this.focus();
            }
        });
    }
};

document.addEventListener("DOMContentLoaded", function() {
    const autoCloseTags = new AutoCloseTags(document.querySelector(".BBCodeEditor"));
});

(function($) {
    "use strict";
    var Suggest = function(el, key, options) {
        var that = this;
        this.$element = $(el);
        this.$items = undefined;
        this.options = $.extend(true, {}, $.fn.suggest.defaults, options, this.$element.data(), this.$element.data("options"));
        this.key = key;
        this.isShown = false;
        this.query = "";
        this._queryPos = [];
        this._keyPos = -1;
        this.$dropdown = $("<div />", {
            class: "dropdown suggest " + this.options.dropdownClass,
            html: $("<div />", {
                class: "dropdown-menu",
                role: "menu"
            }),
            "data-key": this.key
        });
        this.load();
    };
    Suggest.prototype = {
        __setListener: function() {
            this.$element.on("suggest.show", $.proxy(this.options.onshow, this)).on("suggest.select", $.proxy(this.options.onselect, this)).on("suggest.lookup", $.proxy(this.options.onlookup, this)).on("keyup", $.proxy(this.__keyup, this));
            return this;
        },
        __getCaretPos: function(posStart) {
            var properties = [ "direction", "boxSizing", "width", "height", "overflowX", "overflowY", "borderTopWidth", "borderRightWidth", "borderBottomWidth", "borderLeftWidth", "paddingTop", "paddingRight", "paddingBottom", "paddingLeft", "fontStyle", "fontVariant", "fontWeight", "fontStretch", "fontSize", "fontSizeAdjust", "lineHeight", "fontFamily", "textAlign", "textTransform", "textIndent", "textDecoration", "letterSpacing", "wordSpacing" ];
            var isFirefox = !(window.mozInnerScreenX == null);
            var getCaretCoordinatesFn = function(element, position, recalculate) {
                var div = document.createElement("div");
                div.id = "input-textarea-caret-position-mirror-div";
                document.body.appendChild(div);
                var style = div.style;
                var computed = window.getComputedStyle ? getComputedStyle(element) : element.currentStyle;
                style.whiteSpace = "pre-wrap";
                if (element.nodeName !== "INPUT") style.wordWrap = "break-word";
                style.position = "absolute";
                style.visibility = "hidden";
                $.each(properties, function(index, value) {
                    style[value] = computed[value];
                });
                if (isFirefox) {
                    style.width = parseInt(computed.width) - 2 + "px";
                    if (element.scrollHeight > parseInt(computed.height)) style.overflowY = "scroll";
                } else {
                    style.overflow = "hidden";
                }
                div.textContent = element.value.substring(0, position);
                if (element.nodeName === "INPUT") div.textContent = div.textContent.replace(/\s/g, " ");
                var span = document.createElement("span");
                span.textContent = element.value.substring(position) || ".";
                div.appendChild(span);
                var coordinates = {
                    top: span.offsetTop + parseInt(computed["borderTopWidth"]),
                    left: span.offsetLeft + parseInt(computed["borderLeftWidth"])
                };
                document.body.removeChild(div);
                return coordinates;
            };
            return getCaretCoordinatesFn(this.$element.get(0), posStart);
        },
        __keyup: function(e) {
            var specialChars = [ 38, 40, 37, 39, 17, 18, 9, 16, 20, 91, 93, 36, 35, 45, 33, 34, 144, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 145, 19 ];
            switch (e.keyCode) {
              case 27:
                this.hide();
                return;

              case 13:
                return true;
            }
            if ($.inArray(e.keyCode, specialChars) !== -1) return;
            var $el = this.$element, val = $el.val(), currentPos = this.__getSelection($el.get(0)).start;
            for (var i = currentPos; i >= 0; i--) {
                var subChar = $.trim(val.substring(i - 1, i));
                if (!subChar && this.options.respectWhitespace) {
                    this.hide();
                    break;
                }
                var isSpaceBefore = $.trim(val.substring(i - 2, i - 1)) == "";
                if (subChar === this.key && (isSpaceBefore || !this.options.respectWhitespace)) {
                    this.query = val.substring(i, currentPos);
                    this._queryPos = [ i, currentPos ];
                    this._keyPos = i;
                    this.lookup(this.query);
                    break;
                }
            }
        },
        __getVisibleItems: function() {
            return this.$items ? this.$items.not(".d-none") : $();
        },
        __build: function() {
            var elems = [], $item, $dropdown = this.$dropdown, that = this;
            var blur = function(e) {
                that.hide();
            };
            $dropdown.on("click", "a.dropdown-item", function(e) {
                e.preventDefault();
                that.__select($(this).index());
                that.$element.focus();
            }).on("mouseover", "a.dropdown-item", function(e) {
                that.$element.off("blur", blur);
            }).on("mouseout", "a.dropdown-item", function(e) {
                that.$element.on("blur", blur);
            });
            this.$element.before($dropdown).on("blur", blur).on("keydown", function(e) {
                var $visibleItems;
                if (that.isShown) {
                    switch (e.keyCode) {
                      case 13:
                      case 9:
                        $visibleItems = that.__getVisibleItems();
                        $visibleItems.each(function(index) {
                            if ($(this).is(".active")) that.__select($(this).index());
                        });
                        return false;
                        break;

                      case 40:
                        $visibleItems = that.__getVisibleItems();
                        if ($visibleItems.last().is(".active")) return false;
                        $visibleItems.each(function(index) {
                            var $this = $(this), $next = $visibleItems.eq(index + 1);
                            if ($this.is(".active")) {
                                if (!$next.is(".d-none")) {
                                    $this.removeClass("active");
                                    $next.addClass("active");
                                }
                                return false;
                            }
                        });
                        return false;

                      case 38:
                        $visibleItems = that.__getVisibleItems();
                        if ($visibleItems.first().is(".active")) return false;
                        $visibleItems.each(function(index) {
                            var $this = $(this), $prev = $visibleItems.eq(index - 1);
                            if ($this.is(".active")) {
                                if (!$prev.is(".d-none")) {
                                    $this.removeClass("active");
                                    $prev.addClass("active");
                                }
                                return false;
                            }
                        });
                        return false;
                    }
                }
            });
        },
        __mapItem: function(dataItem) {
            var itemHtml, that = this, _item = {
                text: "",
                value: "",
                class: ""
            };
            if (this.options.map) {
                dataItem = this.options.map(dataItem);
                if (!dataItem) return false;
            }
            if (dataItem instanceof Object) {
                _item.text = dataItem.text || "";
                _item.value = dataItem.value || "";
                _item.class = dataItem.class || "";
            } else {
                _item.text = dataItem;
                _item.value = dataItem;
            }
            return $("<a />", {
                class: "dropdown-item" + " " + _item.class,
                "data-value": _item.value,
                href: "#",
                html: _item.text
            });
        },
        __select: function(index) {
            var endKey = this.options.endKey || "";
            var $el = this.$element, el = $el.get(0), val = $el.val(), item = this.get(index), setCaretPos = this._keyPos + item.value.length + 1;
            $el.val(val.slice(0, this._keyPos) + item.value + endKey + " " + val.slice(this.__getSelection(el).start));
            $el.blur();
            if (el.setSelectionRange) {
                el.setSelectionRange(setCaretPos, setCaretPos);
            } else if (el.createTextRange) {
                var range = el.createTextRange();
                range.collapse(true);
                range.moveEnd("character", setCaretPos);
                range.moveStart("character", setCaretPos);
                range.select();
            }
            $el.trigger($.extend({
                type: "suggest.select"
            }, this), item);
            this.hide();
        },
        __getSelection: function(el) {
            el.focus();
            return {
                start: el.selectionStart,
                end: el.selectionEnd
            };
        },
        __buildItems: function(data) {
            var $dropdownMenu = this.$dropdown.find(".dropdown-menu");
            $dropdownMenu.empty();
            if (data && data instanceof Array) {
                for (var i in data) {
                    var $item = this.__mapItem(data[i]);
                    if ($item) {
                        $dropdownMenu.append($item);
                    }
                }
            }
            return $dropdownMenu.find("a.dropdown-item");
        },
        __lookup: function(q, $resultItems) {
            var active = $resultItems.eq(0).addClass("active");
            this.$element.trigger($.extend({
                type: "suggest.lookup"
            }, this), [ q, $resultItems ]);
            if ($resultItems && $resultItems.length) {
                this.show();
            } else {
                this.hide();
            }
        },
        __filterData: function(q, data) {
            var options = this.options;
            this.$items.addClass("d-none");
            this.$items.filter(function(index) {
                if (q === "") return index < options.filter.limit;
                var value = $(this).text();
                var selectorValue = $(this).data().value;
                if (!options.filter.casesensitive) {
                    value = value.toLowerCase();
                    q = q.toLowerCase();
                    selectorValue = selectorValue.toLowerCase();
                }
                return value.indexOf(q) != -1 || selectorValue.indexOf(q) != -1;
            }).slice(0, options.filter.limit).removeClass("d-none active");
            return this.__getVisibleItems();
        },
        get: function(index) {
            if (!this.$items) return;
            var $item = this.$items.eq(index);
            return {
                text: $item.text(),
                value: $item.attr("data-value"),
                index: index,
                $element: $item
            };
        },
        lookup: function(q) {
            var options = this.options, that = this, data;
            var provide = function(data) {
                if (that._keyPos !== -1) {
                    if (!that.$items) {
                        that.$items = that.__buildItems(data);
                    }
                    that.__lookup(q, that.__filterData(q, data));
                }
            };
            if (typeof this.options.data === "function") {
                this.$items = undefined;
                data = this.options.data(q, provide);
            } else {
                data = this.options.data;
            }
            if (data && typeof data.promise === "function") {
                data.done(provide);
            } else if (data) {
                provide.call(this, data);
            }
        },
        load: function() {
            this.__setListener();
            this.__build();
        },
        hide: function() {
            this.$dropdown.find(".dropdown-menu").removeClass("show");
            this.isShown = false;
            if (this.$items) {
                this.$items.removeClass("active");
            }
            this._keyPos = -1;
        },
        show: function() {
            var $el = this.$element, $dropdownMenu = this.$dropdown.find(".dropdown-menu"), el = $el.get(0), options = this.options, caretPos, position = {
                top: "auto",
                bottom: "auto",
                left: "auto",
                right: "auto"
            };
            if (!this.isShown) {
                $dropdownMenu.addClass("show");
                if (options.position !== false) {
                    caretPos = this.__getCaretPos(this._keyPos);
                    if (typeof options.position == "string") {
                        switch (options.position) {
                          case "bottom":
                            position.top = $el.outerHeight() - parseFloat($dropdownMenu.css("margin-top"));
                            position.left = 0;
                            position.right = 0;
                            break;

                          case "top":
                            position.top = -($dropdownMenu.outerHeight(true) + parseFloat($dropdownMenu.css("margin-top")));
                            position.left = 0;
                            position.right = 0;
                            break;

                          case "caret":
                            position.top = caretPos.top - el.scrollTop;
                            position.left = caretPos.left - el.scrollLeft;
                            break;
                        }
                    } else {
                        position = $.extend(position, typeof options.position === "function" ? options.position(el, caretPos) : options.position);
                    }
                    $dropdownMenu.css(position);
                }
                this.isShown = true;
                $el.trigger($.extend({
                    type: "suggest.show"
                }, this));
            }
        }
    };
    var old = $.fn.suggest;
    $.fn.suggest = function(arg1) {
        var arg2 = arguments[1], arg3 = arguments[2];
        var createSuggestions = function(el, suggestions) {
            var newData = {};
            $.each(suggestions, function(keyChar, options) {
                var key = keyChar.toString().charAt(0);
                newData[key] = new Suggest(el, key, typeof options === "object" && options);
            });
            return newData;
        };
        return this.each(function() {
            var that = this, $this = $(this), data = $this.data("suggest"), suggestions = {};
            if (typeof arg1 === "string") {
                if (arg1.length == 1) {
                    if (arg2) {
                        if (typeof arg2 === "string") {
                            if (arg1 in data && typeof data[arg1][arg2] !== "undefined") {
                                return data[arg1][arg2].call(data[arg1], arg3);
                            } else {
                                console.error(arg1 + " is not a suggest");
                            }
                        } else {
                            suggestions[arg1] = $.isArray(arg2) || typeof arg2 === "function" ? {
                                data: arg2
                            } : arg2;
                            if (data && arg1 in data) {
                                data[arg1].options = $.extend({}, data[arg1].options, suggestions[arg1]);
                            } else {
                                data = $.extend(data, createSuggestions(this, suggestions));
                            }
                            $this.data("suggest", data);
                        }
                    }
                } else {
                    console.error("you're not initializing suggest properly. arg1 should have length == 1");
                }
            } else {
                if (!data) $this.data("suggest", createSuggestions(this, arg1)); else if (data) {
                    $.each(arg1, function(key, value) {
                        if (key in data === false) {
                            suggestions[key] = value;
                        } else {
                            data[key].options = $.extend({}, data[key].options, value);
                        }
                    });
                    $this.data("suggest", $.extend(data, createSuggestions(that, suggestions)));
                }
            }
        });
    };
    $.fn.suggest.defaults = {
        data: [],
        map: undefined,
        filter: {
            casesensitive: false,
            limit: 5
        },
        dropdownClass: "",
        position: "caret",
        endKey: "",
        respectWhitespace: true,
        onshow: function(e) {},
        onselect: function(e, item) {},
        onlookup: function(e, item) {}
    };
    $.fn.suggest.Constructor = Suggest;
    $.fn.suggest.noConflict = function() {
        $.fn.suggest = old;
        return this;
    };
})(jQuery);