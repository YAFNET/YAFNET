var yafEditor = function(name, urlTitle, urlDescription, urlImageTitle, urlImageDescription, description, mediaTitle, insertNote, typeTitle) {
    this.Name = name;
    this.InsertNote = insertNote;
    this.TypeTitle = typeTitle;
    this.UrlTitle = urlTitle;
    this.UrlDescription = urlDescription;
    this.UrlImageTitle = urlImageTitle;
    this.UrlImageDescription = urlImageDescription;
    this.Description = description;
    this.MediaTitle = mediaTitle;
    document.querySelectorAll(".BBCodeEditor").forEach(editor => {
        const autoCloseTags = new AutoCloseTags(editor);
        const undoManager = new EditorUndoManager(editor);
        editor.addEventListener("keydown", function(e) {
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
                    if (document.getElementById("QuickReplyDialog") != null) {
                        document.querySelector('[data-bs-save*="modal"]').click();
                    } else if (document.querySelector('[formaction*="PostReply"]') != null) {
                        document.querySelector('[formaction*="PostReply"]').click();
                    }
                }
            }
        });
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

      case "strikethrough":
        wrapSelection(textObj, "[s]", "[/s]");
        break;

      case "highlight":
        wrapSelection(textObj, "[h]", "[/h]");
        break;

      case "code":
        wrapSelection(textObj, "[code]", "[/code]");
        break;

      case "codelang":
        wrapSelection(textObj, `[code=${option}]`, "[/code]");
        break;

      case "media":
        {
            if (getCurrentSelection(textObj)) {
                wrapSelection(textObj, "[media]", "[/media]");
            } else {
                bootbox.prompt({
                    title: this.MediaTitle,
                    placeholder: "https://",
                    callback: function(url) {
                        replaceSelection(textObj, `[media]${url}[/media]`);
                    }
                });
            }
        }
        break;

      case "img":
        {
            if (getCurrentSelection(textObj)) {
                wrapSelection(textObj, "[img]", "[/img]");
            } else {
                bootbox.confirm({
                    title: this.UrlImageTitle,
                    message: `<form><div class="mb-3">
                                 <label for="url" class="form-label">${this.UrlImageTitle}</label> 
                                 <input type="text" class="form-control" id="url" placeholder="https://" />
                             </div>
                             <div class="mb-3">
                                 <label for="desc" class="form-label">${this.UrlImageDescription}</label>
                                 <input type="text" class="form-control" id="desc" placeholder="${this.Description}" />
                             </div></form>
                                 `,
                    callback: function(result) {
                        if (result) {
                            const url = document.getElementById("url").value, desc = document.getElementById("desc").value;
                            if (desc !== "" && desc !== null) {
                                replaceSelection(textObj, `[img=${url}]${desc}[/img]`);
                            } else {
                                replaceSelection(textObj, `[img]${url}[/img]`);
                            }
                        }
                    }
                });
            }
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

      case "createNote":
        {
            bootbox.confirm({
                title: this.InsertNote,
                message: `<form><div class="mb-3">
                                 <label for="type" class="form-label">${this.TypeTitle}</label> 
                                 <select class="form-select" id="type" aria-label="note type">
                                     <option value="primary">primary</option>
                                     <option value="secondary">secondary</option>
                                     <option value="success">success</option>
                                     <option value="danger">danger</option>
                                     <option value="warning">warning</option>
                                     <option value="info">info</option>
                                     <option value="light">light</option>
                                     <option value="dark">dark</option>
                                 </select>
                             </div>
                             <div class="mb-3">
                                 <label for="txt" class="form-label">${this.InsertNote}</label>
                                 <textarea cols="20" rows="7" id="txt" class="form-control textarea-input"></textarea>
                             </div></form>
                                 `,
                callback: function(result) {
                    if (result) {
                        const type = document.getElementById("type").value, txt = document.getElementById("txt").value;
                        replaceSelection(textObj, `[note=${type}]${txt}[/note]`);
                    }
                }
            });
        }
        break;

      case "createlink":
        {
            bootbox.confirm({
                title: this.UrlTitle,
                message: `<form><div class="mb-3">
                                 <label for="url" class="form-label">${this.UrlTitle}</label> 
                                 <input type="text" class="form-control" id="url" placeholder="https://" />
                             </div>
                             <div class="mb-3">
                                 <label for="desc" class="form-label">${this.UrlDescription}</label>
                                 <input type="text" class="form-control" id="desc" placeholder="${this.Description}" />
                             </div></form>
                                 `,
                callback: function(result) {
                    if (result) {
                        const url = document.getElementById("url").value, desc = document.getElementById("desc").value;
                        if (desc !== "" && desc != null) {
                            replaceSelection(textObj, `[url=${url}]${desc}[/url]`);
                        } else {
                            replaceSelection(textObj, `[url]${url}[/url]`);
                        }
                    }
                }
            });
        }
        break;

      case "unorderedlist":
        wrapSelection(textObj, "[list][*]", "[/list]");
        break;

      case "orderedlist":
        wrapSelection(textObj, "[list=1][*]", "[/list]");
        break;

      case "color":
        wrapSelection(textObj, `[color=${option}]`, "[/color]");
        break;

      case "font":
        wrapSelection(textObj, `[font=${option}]`, "[/font]");
        break;

      case "fontsize":
        wrapSelection(textObj, `[size=${option}]`, "[/size]");
        break;

      case "albumimg":
        replaceSelection(textObj, `[albumimg]${option}[/albumimg]`);
        break;

      case "attach":
        replaceSelection(textObj, `[attach]${option}[/attach]`);
        break;

      case "email":
        bootbox.confirm({
            title: this.UrlTitle,
            message: `<form><div class="mb-3">
                  <label for="url" class="form-label">${this.UrlTitle}</label> 
                  <input type="text" class="form-control" id="url" placeholder="https://" />
              </div>
              <div class="mb-3">
                  <label for="desc" class="form-label">${this.UrlDescription}</label>
                  <input type="text" class="form-control" id="desc" placeholder="${this.Description}" />
              </div></form>
                  `,
            callback: function(result) {
                if (result) {
                    const url = document.getElementById("url").value, desc = document.getElementById("desc").value;
                    if (desc !== "" && desc != null) {
                        replaceSelection(textObj, `[email=${url}]${desc}[/email]`);
                    } else {
                        replaceSelection(textObj, `[email]${url}[/email]`);
                    }
                }
            }
        });
        break;

      case "userlink":
        replaceSelection(textObj, `[userlink]${option}[/userlink]`);
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
        wrapSelection(textObj, `[${command}]`, `[/${command}]`);
        break;
    }
};

function removeFormat(input) {
    if (input.setSelectionRange) {
        const selectionStart = input.selectionStart;
        const selectionEnd = input.selectionEnd;
        const selectedText = input.value.substring(selectionStart, selectionEnd);
        const regex = /\[.*?\]/g;
        const replacedText = selectedText.replace(regex, "");
        const replacedLength = selectedText.length - replacedText.length;
        input.value = input.value.replace(selectedText, replacedText);
        setSelectionRange(input, selectionStart, selectionEnd - replacedLength);
    }
}

function setSelectionRange(input, selectionStart, selectionEnd) {
    if (input.setSelectionRange) {
        input.focus();
        input.setSelectionRange(selectionStart, selectionEnd);
    } else if (input.createTextRange) {
        const range = input.createTextRange();
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
        const selectionStart = input.selectionStart, selectionEnd = input.selectionEnd;
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
        const selectionStart = input.selectionStart;
        const selectionEnd = input.selectionEnd;
        const selectedText = input.value.substring(selectionStart, selectionEnd);
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
        const selectionStart = input.selectionStart;
        const selectionEnd = input.selectionEnd;
        input.value = input.value.substring(0, selectionStart) + preString + input.value.substring(selectionStart, selectionEnd) + postString + input.value.substring(selectionEnd);
        if (selectionStart != selectionEnd) {
            setSelectionRange(input, selectionStart, preString.length + postString.length + selectionEnd);
        } else {
            setCaretToPos(input, selectionStart + preString.length);
        }
    } else if (document.selection) {
        const sel = document.selection.createRange().text;
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
        const range = document.selection.createRange();
        return range.parentElement() == input && range.text != "";
    } else {
        return false;
    }
}

var doc, I, StateMaker;

doc = document;

I = function(id) {
    return doc.getElementById(id);
};

StateMaker = function(initialState) {
    const o = initialState;
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
        const sl = this.states.length;
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

var EditorUndoManager = function(editor) {
    var text = editor, val, wordCount = 0, words = 0, stateMaker = new StateMaker(), undoButton = I("undo"), redoButton = I("redo"), countField = text.parentElement.parentElement.querySelector("#editor-Counter"), maxLimit = text.maxLength;
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
    text.addEventListener("change", function() {
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
};

var AutoCloseTags = function(textarea) {
    this.textarea = textarea;
    this.autoClosingTags = [ "b", "i", "u", "h", "code", "img", "quote", "left", "center", "right", "indent", "list", "color", "size", "albumimg", "attach", "youtube", "vimeo", "instagram", "twitter", "facebook", "googlewidget", "spoiler", "userlink", "googlemaps", "hide", "group-hide", "hide-thanks", "hide-reply-thanks", "hide-reply", "hide-posts", "dailymotion", "audio", "media" ];
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

function mentions(opts = {}) {
    opts = Object.assign({}, {
        lookup: "lookup",
        id: "",
        selector: "",
        element: null,
        symbol: "@",
        items: [],
        item_template: '<a class="dropdown-item" href="#"><img src="${item.avatar}" alt="avatar" class="me-2 img-thumbnail" style="max-width:20px;max-height:20px" /> ${item.name}</a>',
        onclick: undefined,
        url: ""
    }, opts);
    const $e = opts.id && document.getElementById(opts.id) || opts.selector && document.querySelector(opts.selector) || opts.element;
    if (!$e) return console.error("Invalid element selector", opts);
    var $lookup = document.createElement("div");
    $lookup.classList = `fixed-top autohide ${opts.lookup}`;
    $e.parentNode.insertBefore($lookup, $e.nextSibling);
    $e.addEventListener("keydown", processKey);
    $e.addEventListener("keyup", showLookup);
    $e.addEventListener("click", hideLookup);
    var range, start, end, prevWord;
    var isFixed = false;
    var $el = $lookup.parentNode;
    while ($el && $el.nodeName.toLowerCase() !== "body" && !isFixed) {
        isFixed = window.getComputedStyle($el).getPropertyValue("position").toLowerCase() === "fixed";
        $el = $el.parentElement;
    }
    function showLookup(event) {
        if (event.code === "Escape") {
            return hideLookup();
        }
        var sel = window.getSelection();
        var $parent = $lookup.parentNode.querySelector("textarea");
        var text = sel.anchorNode.querySelector("textarea").value || "";
        var curr = $parent.selectionStart;
        var getLength = arr => arr instanceof Array && arr.length > 0 ? arr[0].length : 0;
        start = curr - getLength(text.slice(0, curr).match(/[\S]+$/g));
        end = curr + getLength(text.slice(curr).match(/^[\S]+/g));
        var word = text.substring(start, end);
        if (!word || word[0] !== opts.symbol) {
            prevWord = "";
            return hideLookup();
        }
        if (word === prevWord) return;
        prevWord = word;
        var pos = {
            x: 0,
            y: 0
        };
        var parentRect = $parent.getBoundingClientRect();
        pos.y = parentRect.top + 30;
        pos.x = parentRect.left + 20;
        $lookup.style.left = pos.x + "px";
        $lookup.style.top = pos.y + "px";
        var query = word.slice(1);
        if (query.length >= 3) {
            fetch(opts.url.replace("{q}", query), {
                method: "GET",
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json;charset=utf-8",
                    RequestVerificationToken: document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            }).then(response => response.json()).then(data => opts.items = data);
        }
        var items = opts.items.filter(e => e.name.toLowerCase().includes(word.slice(1).toLowerCase())).map(item => eval('`<li class="mention-li-nt ${opts.lookup}" data-name = "${item.name}" data-id = "${item.id}">' + opts.item_template + "</li>`"));
        if (!items.length) return hideLookup();
        $lookup.innerHTML = `<ul class="dropdown-menu show">${items.join("")}</ul>`;
        [ ...$lookup.firstElementChild.children ].forEach($el => $el.addEventListener("click", onClick) || $el.addEventListener("mouseenter", onHover));
        $lookup.firstElementChild.children[0].classList.add("active");
        if ($lookup.hasAttribute("hidden")) $lookup.removeAttribute("hidden");
        var bounding = $lookup.getBoundingClientRect();
        if (!(bounding.top >= 0 && bounding.left >= 0 && bounding.bottom <= (window.innerHeight || document.documentElement.clientHeight) && bounding.right <= (window.innerWidth || document.documentElement.clientWidth))) $lookup.style.top = parseInt($lookup.style.top) - 10 - $lookup.clientHeight + "px";
    }
    function hideLookup() {
        if (!$lookup.hasAttribute("hidden")) $lookup.setAttribute("hidden", true);
    }
    function onClick(event) {
        const deleteItem = event.target.classList.contains("mention-li-nt") ? event.target : event.target.parentElement;
        opts.onclick(deleteItem.dataset);
        const $parent = $lookup.parentNode.querySelector("textarea");
        $parent.value = $parent.value.replace($parent.value.substring(start + 1, end), "");
        hideLookup();
    }
    function onHover(event) {
        const $el = event.target.closest(".mention-li-nt");
        if ($el.classList.contains("active")) return;
        [ ...$lookup.firstElementChild.children ].filter($el => $el.classList.contains("active")).forEach($el => $el.classList.remove("active"));
        $el.classList.add("active");
    }
    function processKey(event) {
        const code = event.key;
        if ([ "ArrowUp", "ArrowDown", "Enter" ].indexOf(code) == -1 || $lookup.hasAttribute("hidden")) return;
        event.preventDefault();
        if (code == "Enter") return $lookup.querySelector(".active").click();
        const $children = [ ...$lookup.firstElementChild.children ];
        const curr = $children.findIndex($el => $el.classList.contains("active"));
        $children[curr].classList.remove("active");
        const $next = $children[($children.length + curr + (code == "ArrowUp" ? -1 : 1)) % $children.length];
        $next.classList.add("active");
        $next.scrollIntoView(false);
    }
}