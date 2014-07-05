var yafEditor = function(name) {
    this.Name = name;
};
yafEditor.prototype.FormatText = function (command, option) {
    switch (command) {
        case "bold":
            wrapSelection("[b]", "[/b]");
            break;
        case "italic":
            wrapSelection("[i]", "[/i]");
            break;
        case "underline":
            wrapSelection("[u]", "[/u]");
            break;
        case "highlight":
            wrapSelection("[h]", "[/h]");
            break;
        case "code":
            wrapSelection("[code]", "[/code]");
            break;
        case "codelang":
            wrapSelection("[code=" + option + "]", "[/code]");
            break;
        case "img":
            if (window["editorCM"].getSelection()) {
                wrapSelection('[img]', '[/img]');
            }
            else {
                var imgUrl = prompt('Enter Image Url:', 'http://');

                // ask for the Image description text...
                var imgDesc = prompt('Enter Image Description:', '');

                if (imgDesc != '' && imgDesc != null && imgUrl != '' && imgUrl != null) {
                    window["editorCM"].replaceSelection('[img=' + imgUrl + ']' + imgDesc + '[/img]', "around");
                }
                else if (imgUrl != '' && imgUrl != null) {
                    window["editorCM"].replaceSelection('[img]' + imgUrl + '[/img]', "around");
                }
            }
            break;
        case "quote":
            wrapSelection("[quote]", "[/quote]");
            break;
        case "justifyleft":
            wrapSelection("[left]", "[/left]");
            break;
        case "justifycenter":
            wrapSelection("[center]", "[/center]");
            break;
        case "justifyright":
            wrapSelection("[right]", "[/right]");
            break;
        case "indent":
            wrapSelection("[indent]", "[/indent]");
            break;
        case "outdent":
            if (window["editorCM"].getSelection()) {
                removeFromSelection("[indent]", "[/indent]");
            }
            break;
        case "createlink":
            var url = prompt('Enter URL:', 'http://');

            if (url != '' && url != null) {
                if (window["editorCM"].getSelection()) {
                    wrapSelection('[url=' + url + ']', '[/url]');
                }
                else {
                    // ask for the description text...
                    var desc = prompt('Enter URL Description:', '');
                    if (desc != '' && desc != null)
                        window["editorCM"].replaceSelection('[url=' + url + ']' + desc + '[/url]', "around");
                    else
                        window["editorCM"].replaceSelection('[url]' + url + '[/url]', "around");
                }
            }
            break;
        case "unorderedlist":
            wrapSelection("[list][*]", "[/list]");
            break;
        case "orderedlist":
            wrapSelection("[list=1][*]", "[/list]");
            break;
        case "color":
            wrapSelection("[color=" + option + "]", "[/color]");
            break;
        case "fontsize":
            wrapSelection("[size=" + option + "]", "[/size]");
            break;
        case "AlbumImgId":
            window["editorCM"].replaceSelection('[albumimg]' + option + '[/albumimg]', "around");
            break;
        default:
            // make custom option
            wrapSelection("[" + command + "]", "[/" + command + "]");
            break;
    }
};
yafEditor.prototype.AddImage = function () {

    var imgUrl = prompt('Enter image URL:', 'http://');

    // ask for the Image description text...
    var imgDesc = prompt('Enter Image Description:', '');

    if (imgDesc != '' && imgDesc != null) {
        window["editorCM"].replaceSelection('[img=' + imgUrl + ']' + imgDesc + '[/img]', "around");
    }
    else {
        if (imgUrl != '' && imgUrl != null) {
            window["editorCM"].replaceSelection('[img]' + imgUrl + '[/img]', "around");
        }
    }


};
yafEditor.prototype.InsertSmiley = function(code) {
    window["editorCM"].replaceSelection(code, "around");
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

function wrapSelection(preString, postString) {
    var currentSelection = window["editorCM"].getSelection();

    if (currentSelection) {
        window["editorCM"].replaceSelection(preString + currentSelection + postString, "around");
    } else {
        window["editorCM"].replaceSelection(preString + postString, "around");
    }
}

function removeFromSelection(preString, postString) {
    var currentSelection = window["editorCM"].getSelection();

    window["editorCM"].replaceSelection(currentSelection.replace(preString, '').replace(postString, ''), "around");
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
    window["editorCM"] = CodeMirror.fromTextArea($('.BBCodeEditor,.basicBBCodeEditor')[0], {
        mode: "bbcode",
        tabSize: 2,
        indentUnit: 2,
        indentWithTabs: false,
        lineNumbers: true,
        lineWrapping: true,
        extraKeys: {
            "Ctrl-B": function(codeMirror_Editor) {
                wrapSelection('[b]', '[/b]');
            },
            "Ctrl-I": function(codeMirror_Editor) {
                wrapSelection('[i]', '[/i]');
            },
            "Ctrl-U": function(codeMirror_Editor) {
                wrapSelection('[u]', '[/u]');
            },
            "Ctrl-Q": function(codeMirror_Editor) {
                wrapSelection('[quote]', '[/quote]');
            },
            "Ctrl-Enter": function(codeMirror_Editor) {
                if ($('[id *= "QuickReply"]').length) {
                    $('[id *= "QuickReply"]').click();
                } else if ($('[id *= "PostReply"]').length) {
                    window.location.href = $('[id *= "PostReply"]').attr('href');
                }
            }
        }
    });
});