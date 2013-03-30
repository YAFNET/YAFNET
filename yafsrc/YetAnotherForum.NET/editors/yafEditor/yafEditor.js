var yafEditor = function(name) {
    this.Name = name;
};
yafEditor.prototype.FormatText = function (command, option) {
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
                wrapSelection(textObj, '[img]', '[/img]');
            }
            else {
                var imgUrl = prompt('Enter Image Url:', 'http://');

                // ask for the Image description text...
                var imgDesc = prompt('Enter Image Description:', '');

                if (imgDesc != '' && imgDesc != null) {
                    replaceSelection(textObj, '[img=' + imgUrl + ']' + imgDesc + '[/img]');
                }
                else {
                    replaceSelection(textObj, '[img]' + imgUrl + '[/img]');
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
        case "createlink":
            var url = prompt('Enter URL:', 'http://');

            if (url != '' && url != null) {
                if (getCurrentSelection(textObj)) {
                    wrapSelection(textObj, '[url=' + url + ']', '[/url]');
                }
                else {
                    // ask for the description text...
                    var desc = prompt('Enter URL Description:', '');
                    if (desc != '' && desc != null)
                        replaceSelection(textObj, '[url=' + url + ']' + desc + '[/url]');
                    else
                        replaceSelection(textObj, '[url]' + url + '[/url]');
                }
            }
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
            replaceSelection(textObj, '[albumimg]' + option + '[/albumimg]');
            break;
        default:
            // make custom option
            wrapSelection(textObj, "[" + command + "]", "[/" + command + "]");
            break;
    }
};
yafEditor.prototype.AddImage = function () {

    var textObj = document.getElementById(this.Name);

    var imgUrl = prompt('Enter image URL:', 'http://');

    // ask for the Image description text...
    var imgDesc = prompt('Enter Image Description:', '');

    if (imgDesc != '' && imgDesc != null) {
        replaceSelection(textObj, '[img=' + imgUrl + ']' + imgDesc + '[/img]');
    }
    else {
        if (imgUrl != '' && imgUrl != null) {
            replaceSelection(textObj, '[img]' + imgUrl + '[/img]');
        }
    }


};
yafEditor.prototype.InsertSmiley = function(code) {
    var textObj = document.getElementById(this.Name);
    replaceSelection(textObj, code);
};

function Button_Load(img) {
    img.className = "ButtonOut";
    img.onmouseover = function() { Button_Over(this); };
    img.onmouseout = function() { Button_Out(this); };
}

function Button_Over(img) {
    if (typeof (img._enabled) == "boolean" && !img._enabled)
        img.className = "ButtonOff";
    else
        img.className = "ButtonOver";
}

function Button_Out(img) {
    if (typeof (img._enabled) == "boolean" && !img._enabled)
        img.className = "ButtonOff";
    else if (typeof (img._selected) == "boolean" && img._selected)
        img.className = "ButtonChecked";
    else
        img.className = "ButtonOut";
}

function Button_SetState(doc, name, cmd) {
    var img = document.getElementById(name);
    try {
        img._selected = doc.queryCommandState(cmd);
    }
    catch (e) {
        img._selected = false;
    }
    img._enabled = doc.queryCommandEnabled(cmd);

    if (!img._enabled)
        img.className = "ButtonOff";
    else if (img._selected)
        img.className = "ButtonChecked";
    else
        img.className = "ButtonOut";
}

function storeCaret(input) {
    if (input.createTextRange) {
        input.caretPos = document.selection.createRange().duplicate();
    }
}

function setSelectionRange(input, selectionStart, selectionEnd) {
    if (input.setSelectionRange) {
        input.focus();
        input.setSelectionRange(selectionStart, selectionEnd);
    } else if (input.createTextRange) {
        var range = input.createTextRange();
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
        var selectionStart = input.selectionStart;
        var selectionEnd = input.selectionEnd;
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
        var selectionStart = input.selectionStart;
        var selectionEnd = input.selectionEnd;
		
		var selectedText = input.value.substring(selectionStart, selectionEnd);
		
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
        var selectionStart = input.selectionStart;
        var selectionEnd = input.selectionEnd;
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
}

function getCurrentSelection(input) {
    if (input.setSelectionRange) {
        return input.selectionStart != input.selectionEnd;
    } else if (document.selection) {
        var range = document.selection.createRange();
        return range.parentElement() == input && range.text != '';
    } else {
        return false;
    }
}

function AlbumsPageSelectCallback(page_index) {
    var Albums_content = jQuery('#AlbumsPagerHidden div.result:eq(' + page_index + ')').clone();
    jQuery('#AlbumsPagerResult').empty().append(Albums_content);
    return false;
}
jQuery(document).ready(function () {
    if (jQuery('#AlbumsListPager').length) {
        var Albums_entries = jQuery('#AlbumsPagerHidden div.result').length;
        jQuery('#AlbumsListPager').pagination(Albums_entries, {
            callback: AlbumsPageSelectCallback,
            items_per_page: 1,
            num_display_entries: 3,
            num_edge_entries: 1,
            prev_class: 'smiliesPagerPrev',
            next_class: 'smiliesPagerNext',
            prev_text: '&laquo;',
            next_text: '&raquo;'
        });
    }
});
$(document).ready(function () {
    $('.BBCodeEditor').keydown(function (e) {
        if (e.ctrlKey && !e.altKey && (e.which == 66 || e.which == 73 || e.which == 85 || e.which == 81)) {
            if (e.which == 66) {
                wrapSelection(this, '[b]', '[/b]');
            } else if (e.which == 73) {
                wrapSelection(this, '[i]', '[/i]');
            } else if (e.which == 85) {
                wrapSelection(this, '[u]', '[/u]');
            } else if (e.which == 81) {
                wrapSelection(this, '[quote]', '[/quote]');
            }
            return false;
        }
    });
});