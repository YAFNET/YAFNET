var yafEditor = function (name, urlTitle, urlDescription, urlImageTitle, urlImageDescription, description, mediaTitle, insertNote, typeTitle) {
    this.Name = name;
    this.InsertNote = insertNote;
    this.TypeTitle = typeTitle;
    this.UrlTitle = urlTitle;
    this.UrlDescription = urlDescription;
    this.UrlImageTitle = urlImageTitle;
    this.UrlImageDescription = urlImageDescription;
    this.Description = description;
    this.MediaTitle = mediaTitle;

    document.querySelectorAll('.BBCodeEditor').forEach(editor => {
	    const autoCloseTags = new AutoCloseTags(editor);
	    const undoManager = new EditorUndoManager(editor);

	    editor.addEventListener('keydown', function (e) {
		    if (e.ctrlKey &&
			    !e.altKey &&
			    (e.which == 66 || e.which == 73 || e.which == 85 || e.which == 81 || e.which == 13)) {
			    if (e.which == 66) {
				    wrapSelection(this, '[b]', '[/b]');
			    } else if (e.which == 73) {
				    wrapSelection(this, '[i]', '[/i]');
			    } else if (e.which == 85) {
				    wrapSelection(this, '[u]', '[/u]');
			    } else if (e.which == 81) {
				    wrapSelection(this, '[quote]', '[/quote]');
			    } else if (e.which == 13) {
				    if (document.getElementById('QuickReplyDialog') != null) {
					    document.querySelector('[data-bs-save*="modal"]').click();
				    } else if (document.querySelector('[formaction*="PostReply"]') != null) {
					    document.querySelector('[formaction*="PostReply"]').click();
				    }
			    }
		    }
	    });
    });
};
yafEditor.prototype.FormatText = function (command, option) {
    var textObj = document.getElementById(this.Name);
    switch (command) {
        case 'bold':
            wrapSelection(textObj, '[b]', '[/b]');
            break;
        case 'italic':
            wrapSelection(textObj, '[i]', '[/i]');
            break;
        case 'underline':
            wrapSelection(textObj, '[u]', '[/u]');
            break;
        case 'strikethrough':
            wrapSelection(textObj, '[s]', '[/s]');
            break;
        case 'highlight':
            wrapSelection(textObj, '[h]', '[/h]');
            break;
        case 'code':
            wrapSelection(textObj, '[code]', '[/code]');
            break;
        case 'codelang':
            wrapSelection(textObj, `[code=${option}]`, '[/code]');
            break;
        case 'media':
            {

                if (getCurrentSelection(textObj)) {
                    wrapSelection(textObj, '[media]', '[/media]');
                }
                else {
                    bootbox.prompt({
                        title: this.MediaTitle,
                        placeholder: 'https://',
                        callback: function (url) {
                            replaceSelection(textObj, `[media]${url}[/media]`);
                        }
                    });
                }
            }

            break;
        case 'img':
            {
                if (getCurrentSelection(textObj)) {
                    wrapSelection(textObj, '[img]', '[/img]');
                }
                else {
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
                        callback: function (result) {
                            if (result) {
                                const url = document.getElementById('url').value,
                                    desc = document.getElementById('desc').value;

                                if (desc !== '' && desc !== null) {
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
        case 'quote':
            wrapSelection(textObj, '[quote]', '[/quote]');
            break;
        case 'justifyleft':
            wrapSelection(textObj, '[left]', '[/left]');
            break;
        case 'justifycenter':
            wrapSelection(textObj, '[center]', '[/center]');
            break;
        case 'justifyright':
            wrapSelection(textObj, '[right]', '[/right]');
            break;
        case 'indent':
            wrapSelection(textObj, '[indent]', '[/indent]');
            break;
        case 'outdent':
            if (getCurrentSelection(textObj)) {
                removeFromSelection(textObj, '[indent]', '[/indent]');
            }
            break;
        case 'removeFormat':
            if (getCurrentSelection(textObj)) {
                removeFormat(textObj);
            }
            break;
        case 'createNote':
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
                    callback: function (result) {
                        if (result) {
                            const type = document.getElementById('type').value,
                                txt = document.getElementById('txt').value;

                            replaceSelection(textObj, `[note=${type}]${txt}[/note]`);
                        }

                    }
                });
            }

            break;
        case 'createlink':
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
                    callback: function (result) {
                        if (result) {
                            const url = document.getElementById('url').value,
                                desc = document.getElementById('desc').value;

                            if (desc !== '' && desc != null) {
                                replaceSelection(textObj, `[url=${url}]${desc}[/url]`);
                            } else {
                                replaceSelection(textObj, `[url]${url}[/url]`);
                            }
                        }

                    }
                });
            }

            break;
        case 'unorderedlist':
            wrapSelection(textObj, '[list][*]', '[/list]');
            break;
        case 'orderedlist':
            wrapSelection(textObj, '[list=1][*]', '[/list]');
            break;
        case 'color':
            wrapSelection(textObj, `[color=${option}]`, '[/color]');
            break;
        case 'font':
            wrapSelection(textObj, `[font=${option}]`, '[/font]');
            break;
        case 'fontsize':
            wrapSelection(textObj, `[size=${option}]`, '[/size]');
            break;
        case 'albumimg':
            replaceSelection(textObj, `[albumimg]${option}[/albumimg]`);
            break;
        case 'attach':
            replaceSelection(textObj, `[attach]${option}[/attach]`);
            break;
        case 'email':
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
                callback: function (result) {
                    if (result) {
                        const url = document.getElementById('url').value,
                            desc = document.getElementById('desc').value;

                        if (desc !== '' && desc != null) {
                            replaceSelection(textObj, `[email=${url}]${desc}[/email]`);
                        } else {
                            replaceSelection(textObj, `[email]${url}[/email]`);
                        }
                    }

                }
            });
            break;
        case 'userlink':
            replaceSelection(textObj, `[userlink]${option}[/userlink]`);
            break;
        case 'selectAll':
            textObj.select();
            break;
        case 'cut':
            if (getCurrentSelection(textObj)) {
                document.execCommand('cut');
            }
            break;
        case 'copy':
            if (getCurrentSelection(textObj)) {
                document.execCommand('copy');
            }
            break;
        case 'paste':
            navigator.clipboard.readText().then(function(textFromClipboard) {
                textObj.value += textFromClipboard;
            });
            break;
        default:
            // make custom option
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

        const replacedText = selectedText.replace(regex, '');

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
        range.moveEnd('character', selectionEnd);
        range.moveStart('character', selectionStart);
        range.select();
    }
}

function setCaretToPos(input, pos) {
    setSelectionRange(input, pos, pos);
}

function replaceSelection(input, replaceString) {
    if (input.setSelectionRange) {
        const selectionStart = input.selectionStart,
        selectionEnd = input.selectionEnd;
        input.value = input.value.substring(0, selectionStart)
            + replaceString
            + input.value.substring(selectionEnd);
        if (selectionStart != selectionEnd) // has there been a selection
            setSelectionRange(input, selectionStart, selectionStart +
                replaceString.length);
        else // set caret
            setCaretToPos(input, selectionStart + replaceString.length);
    }
    else if (document.selection) {
        input.focus();
        document.selection.createRange().text = replaceString;
    }
    else {
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

            input.value = input.value.substring(0, selectionStart)
                + input.value.substring(selectionStart + preString.length, selectionEnd - postString.length)
                + input.value.substring(selectionEnd);

            if (selectionStart != selectionEnd) {
                // has there been a selection
                setSelectionRange(input, selectionStart, selectionEnd - postString.length - preString.length);
            }
            else {
                // set caret
                setCaretToPos(input, selectionStart + (preString).length);
            }
        }
    }
}

function wrapSelection(input, preString, postString) {
    if (input.setSelectionRange) {
        const selectionStart = input.selectionStart;
        const selectionEnd = input.selectionEnd;
        input.value = input.value.substring(0, selectionStart)
            + preString
            + input.value.substring(selectionStart, selectionEnd)
            + postString
            + input.value.substring(selectionEnd);
        if (selectionStart != selectionEnd) {
            // has there been a selection
            setSelectionRange(input, selectionStart, preString.length + postString.length + selectionEnd);
        }
        else {
            // set caret
            setCaretToPos(input, selectionStart + (preString).length);
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

    input.dispatchEvent(new Event('change'));
}

function getCurrentSelection(input) {
    if (input.setSelectionRange) {
        return input.selectionStart != input.selectionEnd;
    } else if (document.selection) {
        const range = document.selection.createRange();
        return range.parentElement() == input && range.text != '';
    } else {
        return false;
    }
}