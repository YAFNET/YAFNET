var yafEditor = function (name) {
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
                wrapSelection(textObj, "[img]", "[/img]");
            }
            else {
                var imgUrl = prompt("Enter Image Url:", "http://");

                // ask for the Image description text...
                var imgDesc = prompt("Enter Image Description:", "");

                if (imgDesc !== "" && imgDesc != null) {
                    replaceSelection(textObj, "[img=" + imgUrl + "]" + imgDesc + "[/img]");
                }
                else if (imgUrl !== "" && imgUrl != null) {
                    replaceSelection(textObj, "[img]" + imgUrl + "[/img]");
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
            var url = prompt("Enter URL:", "http://");

            if (url != "" && url != null) {
                if (getCurrentSelection(textObj)) {
                    wrapSelection(textObj, "[url=" + url + "]", "[/url]");
                }
                else {
                    // ask for the description text...
                    var desc = prompt("Enter URL Description:", "");
                    if (desc != "" && desc != null)
                        replaceSelection(textObj, "[url=" + url + "]" + desc + "[/url]");
                    else
                        replaceSelection(textObj, "[url]" + url + "[/url]");
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
            replaceSelection(textObj, "[albumimg]" + option + "[/albumimg]");
            break;
        case "attach":
            replaceSelection(textObj, "[attach]" + option + "[/attach]");
            break;
        default:
            // make custom option
            wrapSelection(textObj, "[" + command + "]", "[/" + command + "]");
            break;
    }
};
yafEditor.prototype.AddImage = function () {

    var textObj = document.getElementById(this.Name);

    var imgUrl = prompt("Enter image URL:", "http://");

    // ask for the Image description text...
    var imgDesc = prompt("Enter Image Description:", "");

    if (imgDesc != "" && imgDesc != null) {
        replaceSelection(textObj, "[img=" + imgUrl + "]" + imgDesc + "[/img]");
    }
    else {
        if (imgUrl != "" && imgUrl != null) {
            replaceSelection(textObj, "[img]" + imgUrl + "[/img]");
        }
    }


};
yafEditor.prototype.InsertSmiley = function (code) {
    var textObj = document.getElementById(this.Name);
    replaceSelection(textObj, code);
};

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
        return range.parentElement() == input && range.text != "";
    } else {
        return false;
    }
}

jQuery(document).ready(function() {

    var autoSaveKey = 'autosave_' + window.location + "_" + $(".BBCodeEditor").attr('name');

    CheckForAutoSavedContent($(".BBCodeEditor"), autoSaveKey, 1440);

    // Render Album Images DropDown
    if (jQuery("#PostAlbumsListPlaceholder").length) {
        var pageSize = 5;
        var pageNumber = 0;
        getAlbumImagesData(pageSize, pageNumber, false);
    }

    // Render emojipicker
    $(".BBCodeEditor").emojiPicker({
        width: '300px',
        height: '200px',
        button: false
    });

    $(".btn-group").on("show.bs.dropdown",
        function(event) {
            if (event.relatedTarget.id === "emoji") {
                $(".BBCodeEditor").emojiPicker("toggle");
            }
        });


    var autoSaveKey = 'autosave_' + window.location + "_" + $(this).attr('name');


    $(".BBCodeEditor").change(function() {

        CheckForAutoSavedContent(this, autoSaveKey, 1440);

        startTimer(this);
    });

    $(".BBCodeEditor").keydown(function(e) {
        if (e.ctrlKey &&
            !e.altKey &&
            (e.which == 66 || e.which == 73 || e.which == 85 || e.which == 81 || e.which == 13)) {
            if (e.which == 66) {
                wrapSelection(this, "[b]", "[/b]");
            } else if (e.which == 73) {
                wrapSelection(this, "[i]", "[/i]");
            } else if (e.which == 85) {
                wrapSelection(this, "[u]", "[/u]");
            } else if (e.which == 81) {
                wrapSelection(this, "[quote]", "[/quote]");
            } else if (e.which == 13) {
                if ($('[id *= "QuickReply"]').length) {
                    $('[id *= "QuickReply"]').click();
                } else if ($('[id *= "PostReply"]').length) {
                    window.location.href = $('[id *= "PostReply"]').attr("href");
                }
            }
            return false;
        }
    });

    $("#forum_ctl03_Cancel, #forum_ctl03_PostReply").click(function() {
        RemoveStorage(autoSaveKey);
    });
});

function CheckForAutoSavedContent(editorInstance, autoSaveKey, notOlderThen) {
    // Checks If there is data available and load it
    if (localStorage.getItem(autoSaveKey)) {
        var jsonSavedContent = LoadData(autoSaveKey);

        var autoSavedContent = jsonSavedContent.data;
        var autoSavedContentDate = jsonSavedContent.saveTime;

        var editorLoadedContent = $(editorInstance).val();

        // check if the loaded editor content is the same as the autosaved content
        if (editorLoadedContent == autoSavedContent) {
            localStorage.removeItem(autoSaveKey);
            return;
        }

        // Ignore if autosaved content is older then x minutes
        if (moment(new Date()).diff(new Date(autoSavedContentDate), 'minutes') > notOlderThen) {
            RemoveStorage(autoSaveKey);

            return;
        }

        var confirmMessage = 'An auto-saved message draft for this topic from "{0}" has been found. Would you like to continue with this Draft or delete it?'.replace("{0}",
            moment(autoSavedContentDate).locale("en")
                .format("LLL"));

        if (confirm(confirmMessage)) {
            if (localStorage.getItem(autoSaveKey)) {
                $(editorInstance).val(jsonSavedContent.data);

                RemoveStorage(autoSaveKey);
            }
        } else {
            RemoveStorage(autoSaveKey);
        }
    } 
}

var startTimer = function (editorInstance) {
    
    if (window.autosave_timeOutId == null) {
        var delay =   10;
        window.autosave_timeOutId = setTimeout(function () {
                onTimer(editorInstance);
            },
            delay * 1000);
    }
};

function onTimer(editorInstance) {
    var editor = editorInstance,
        autoSaveKey = 'autosave_' + window.location + "_" + $(editor).attr('name');

    SaveData(autoSaveKey, editor);

    clearTimeout(window.autosave_timeOutId);

    window.autosave_timeOutId = null;
}

function LoadData(autoSaveKey) {
    var compressedJSON = LZString.decompressFromUTF16(localStorage.getItem(autoSaveKey));
    return JSON.parse(compressedJSON);
}

function SaveMessage() {
    var autoSaveKey = 'autosave_' + window.location + "_" + $(".BBCodeEditor").attr('name');
    SaveData(autoSaveKey, $(".BBCodeEditor"));
}


function SaveData(autoSaveKey, editorInstance) {
    var compressedJSON = LZString.compressToUTF16(JSON.stringify({ data: $(editorInstance).val(), saveTime: new Date() }));

    var quotaExceeded = false;

    try {
        localStorage.setItem(autoSaveKey, compressedJSON);
    } catch (e) {
        quotaExceeded = isQuotaExceeded(e);
        if (quotaExceeded) {
            console.log("Browser localStorage is full, clear your storage or Increase database size");
        }
    }

    if (quotaExceeded) {
        alert("Browser localStorage is full, clear your storage or Increase database size");
    } else {
        $.notify({
                message: "Auto Saved",
                icon: "fa fa-check"
            },
            {
                allow_dismiss: true,
                type: "success",
                element: "body",
                position: null,
                placement: {
                    from: "top", align: "center" }
            });
    }
}

function RemoveStorage(autoSaveKey) {
    if (window.autosave_timeOutId) {
        clearTimeout(window.autosave_timeOutId);
    }

    localStorage.removeItem(autoSaveKey);
}

function isQuotaExceeded(e) {
    var quotaExceeded = false;
    if (e) {
        if (e.code) {
            switch (e.code) {
            case 22:
                quotaExceeded = true;
                break;
            case 1014:
                // Firefox
                if (e.name === 'NS_ERROR_DOM_QUOTA_REACHED') {
                    quotaExceeded = true;
                }
                break;
            }
        } else if (e.number === -2147024882) {
            // Internet Explorer 8
            quotaExceeded = true;
        }
    }
    return quotaExceeded;
}

// Copyright (c) 2013 Pieroxy <pieroxy@pieroxy.net>
// This work is free. You can redistribute it and/or modify it
// under the terms of the WTFPL, Version 2
// For more information see LICENSE.txt or http://www.wtfpl.net/
//
// For more information, the home page:
// http://pieroxy.net/blog/pages/lz-string/testing.html
//
// LZ-based compression algorithm, version 1.4.4
var LZString = (function () {

    // private property
    var f = String.fromCharCode;
    var keyStrBase64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    var keyStrUriSafe = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-$";
    var baseReverseDic = {};

    function getBaseValue(alphabet, character) {
        if (!baseReverseDic[alphabet]) {
            baseReverseDic[alphabet] = {};
            for (var i = 0; i < alphabet.length; i++) {
                baseReverseDic[alphabet][alphabet.charAt(i)] = i;
            }
        }
        return baseReverseDic[alphabet][character];
    }

    var LZString = {
        compressToBase64: function (input) {
            if (input == null) return "";
            var res = LZString._compress(input, 6, function (a) { return keyStrBase64.charAt(a); });
            switch (res.length % 4) { // To produce valid Base64
                default: // When could this happen ?
                case 0: return res;
                case 1: return res + "===";
                case 2: return res + "==";
                case 3: return res + "=";
            }
        },

        decompressFromBase64: function (input) {
            if (input == null) return "";
            if (input == "") return null;
            return LZString._decompress(input.length, 32, function (index) { return getBaseValue(keyStrBase64, input.charAt(index)); });
        },

        compressToUTF16: function (input) {
            if (input == null) return "";
            return LZString._compress(input, 15, function (a) { return f(a + 32); }) + " ";
        },

        decompressFromUTF16: function (compressed) {
            if (compressed == null) return "";
            if (compressed == "") return null;
            return LZString._decompress(compressed.length, 16384, function (index) { return compressed.charCodeAt(index) - 32; });
        },

        //compress into uint8array (UCS-2 big endian format)
        compressToUint8Array: function (uncompressed) {
            var compressed = LZString.compress(uncompressed);
            var buf = new Uint8Array(compressed.length * 2); // 2 bytes per character

            for (var i = 0, TotalLen = compressed.length; i < TotalLen; i++) {
                var current_value = compressed.charCodeAt(i);
                buf[i * 2] = current_value >>> 8;
                buf[i * 2 + 1] = current_value % 256;
            }
            return buf;
        },

        //decompress from uint8array (UCS-2 big endian format)
        decompressFromUint8Array: function (compressed) {
            if (compressed === null || compressed === undefined) {
                return LZString.decompress(compressed);
            } else {
                var buf = new Array(compressed.length / 2); // 2 bytes per character
                for (var i = 0, TotalLen = buf.length; i < TotalLen; i++) {
                    buf[i] = compressed[i * 2] * 256 + compressed[i * 2 + 1];
                }

                var result = [];
                buf.forEach(function (c) {
                    result.push(f(c));
                });
                return LZString.decompress(result.join(''));

            }

        },


        //compress into a string that is already URI encoded
        compressToEncodedURIComponent: function (input) {
            if (input == null) return "";
            return LZString._compress(input, 6, function (a) { return keyStrUriSafe.charAt(a); });
        },

        //decompress from an output of compressToEncodedURIComponent
        decompressFromEncodedURIComponent: function (input) {
            if (input == null) return "";
            if (input == "") return null;
            input = input.replace(/ /g, "+");
            return LZString._decompress(input.length, 32, function (index) { return getBaseValue(keyStrUriSafe, input.charAt(index)); });
        },

        compress: function (uncompressed) {
            return LZString._compress(uncompressed, 16, function (a) { return f(a); });
        },
        _compress: function (uncompressed, bitsPerChar, getCharFromInt) {
            if (uncompressed == null) return "";
            var i, value,
                context_dictionary = {},
                context_dictionaryToCreate = {},
                context_c = "",
                context_wc = "",
                context_w = "",
                context_enlargeIn = 2, // Compensate for the first entry which should not count
                context_dictSize = 3,
                context_numBits = 2,
                context_data = [],
                context_data_val = 0,
                context_data_position = 0,
                ii;

            for (ii = 0; ii < uncompressed.length; ii += 1) {
                context_c = uncompressed.charAt(ii);
                if (!Object.prototype.hasOwnProperty.call(context_dictionary, context_c)) {
                    context_dictionary[context_c] = context_dictSize++;
                    context_dictionaryToCreate[context_c] = true;
                }

                context_wc = context_w + context_c;
                if (Object.prototype.hasOwnProperty.call(context_dictionary, context_wc)) {
                    context_w = context_wc;
                } else {
                    if (Object.prototype.hasOwnProperty.call(context_dictionaryToCreate, context_w)) {
                        if (context_w.charCodeAt(0) < 256) {
                            for (i = 0; i < context_numBits; i++) {
                                context_data_val = (context_data_val << 1);
                                if (context_data_position == bitsPerChar - 1) {
                                    context_data_position = 0;
                                    context_data.push(getCharFromInt(context_data_val));
                                    context_data_val = 0;
                                } else {
                                    context_data_position++;
                                }
                            }
                            value = context_w.charCodeAt(0);
                            for (i = 0; i < 8; i++) {
                                context_data_val = (context_data_val << 1) | (value & 1);
                                if (context_data_position == bitsPerChar - 1) {
                                    context_data_position = 0;
                                    context_data.push(getCharFromInt(context_data_val));
                                    context_data_val = 0;
                                } else {
                                    context_data_position++;
                                }
                                value = value >> 1;
                            }
                        } else {
                            value = 1;
                            for (i = 0; i < context_numBits; i++) {
                                context_data_val = (context_data_val << 1) | value;
                                if (context_data_position == bitsPerChar - 1) {
                                    context_data_position = 0;
                                    context_data.push(getCharFromInt(context_data_val));
                                    context_data_val = 0;
                                } else {
                                    context_data_position++;
                                }
                                value = 0;
                            }
                            value = context_w.charCodeAt(0);
                            for (i = 0; i < 16; i++) {
                                context_data_val = (context_data_val << 1) | (value & 1);
                                if (context_data_position == bitsPerChar - 1) {
                                    context_data_position = 0;
                                    context_data.push(getCharFromInt(context_data_val));
                                    context_data_val = 0;
                                } else {
                                    context_data_position++;
                                }
                                value = value >> 1;
                            }
                        }
                        context_enlargeIn--;
                        if (context_enlargeIn == 0) {
                            context_enlargeIn = Math.pow(2, context_numBits);
                            context_numBits++;
                        }
                        delete context_dictionaryToCreate[context_w];
                    } else {
                        value = context_dictionary[context_w];
                        for (i = 0; i < context_numBits; i++) {
                            context_data_val = (context_data_val << 1) | (value & 1);
                            if (context_data_position == bitsPerChar - 1) {
                                context_data_position = 0;
                                context_data.push(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            } else {
                                context_data_position++;
                            }
                            value = value >> 1;
                        }


                    }
                    context_enlargeIn--;
                    if (context_enlargeIn == 0) {
                        context_enlargeIn = Math.pow(2, context_numBits);
                        context_numBits++;
                    }
                    // Add wc to the dictionary.
                    context_dictionary[context_wc] = context_dictSize++;
                    context_w = String(context_c);
                }
            }

            // Output the code for w.
            if (context_w !== "") {
                if (Object.prototype.hasOwnProperty.call(context_dictionaryToCreate, context_w)) {
                    if (context_w.charCodeAt(0) < 256) {
                        for (i = 0; i < context_numBits; i++) {
                            context_data_val = (context_data_val << 1);
                            if (context_data_position == bitsPerChar - 1) {
                                context_data_position = 0;
                                context_data.push(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            } else {
                                context_data_position++;
                            }
                        }
                        value = context_w.charCodeAt(0);
                        for (i = 0; i < 8; i++) {
                            context_data_val = (context_data_val << 1) | (value & 1);
                            if (context_data_position == bitsPerChar - 1) {
                                context_data_position = 0;
                                context_data.push(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            } else {
                                context_data_position++;
                            }
                            value = value >> 1;
                        }
                    } else {
                        value = 1;
                        for (i = 0; i < context_numBits; i++) {
                            context_data_val = (context_data_val << 1) | value;
                            if (context_data_position == bitsPerChar - 1) {
                                context_data_position = 0;
                                context_data.push(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            } else {
                                context_data_position++;
                            }
                            value = 0;
                        }
                        value = context_w.charCodeAt(0);
                        for (i = 0; i < 16; i++) {
                            context_data_val = (context_data_val << 1) | (value & 1);
                            if (context_data_position == bitsPerChar - 1) {
                                context_data_position = 0;
                                context_data.push(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            } else {
                                context_data_position++;
                            }
                            value = value >> 1;
                        }
                    }
                    context_enlargeIn--;
                    if (context_enlargeIn == 0) {
                        context_enlargeIn = Math.pow(2, context_numBits);
                        context_numBits++;
                    }
                    delete context_dictionaryToCreate[context_w];
                } else {
                    value = context_dictionary[context_w];
                    for (i = 0; i < context_numBits; i++) {
                        context_data_val = (context_data_val << 1) | (value & 1);
                        if (context_data_position == bitsPerChar - 1) {
                            context_data_position = 0;
                            context_data.push(getCharFromInt(context_data_val));
                            context_data_val = 0;
                        } else {
                            context_data_position++;
                        }
                        value = value >> 1;
                    }


                }
                context_enlargeIn--;
                if (context_enlargeIn == 0) {
                    context_enlargeIn = Math.pow(2, context_numBits);
                    context_numBits++;
                }
            }

            // Mark the end of the stream
            value = 2;
            for (i = 0; i < context_numBits; i++) {
                context_data_val = (context_data_val << 1) | (value & 1);
                if (context_data_position == bitsPerChar - 1) {
                    context_data_position = 0;
                    context_data.push(getCharFromInt(context_data_val));
                    context_data_val = 0;
                } else {
                    context_data_position++;
                }
                value = value >> 1;
            }

            // Flush the last char
            while (true) {
                context_data_val = (context_data_val << 1);
                if (context_data_position == bitsPerChar - 1) {
                    context_data.push(getCharFromInt(context_data_val));
                    break;
                }
                else context_data_position++;
            }
            return context_data.join('');
        },

        decompress: function (compressed) {
            if (compressed == null) return "";
            if (compressed == "") return null;
            return LZString._decompress(compressed.length, 32768, function (index) { return compressed.charCodeAt(index); });
        },

        _decompress: function (length, resetValue, getNextValue) {
            var dictionary = [],
                next,
                enlargeIn = 4,
                dictSize = 4,
                numBits = 3,
                entry = "",
                result = [],
                i,
                w,
                bits, resb, maxpower, power,
                c,
                data = { val: getNextValue(0), position: resetValue, index: 1 };

            for (i = 0; i < 3; i += 1) {
                dictionary[i] = i;
            }

            bits = 0;
            maxpower = Math.pow(2, 2);
            power = 1;
            while (power != maxpower) {
                resb = data.val & data.position;
                data.position >>= 1;
                if (data.position == 0) {
                    data.position = resetValue;
                    data.val = getNextValue(data.index++);
                }
                bits |= (resb > 0 ? 1 : 0) * power;
                power <<= 1;
            }

            switch (next = bits) {
                case 0:
                    bits = 0;
                    maxpower = Math.pow(2, 8);
                    power = 1;
                    while (power != maxpower) {
                        resb = data.val & data.position;
                        data.position >>= 1;
                        if (data.position == 0) {
                            data.position = resetValue;
                            data.val = getNextValue(data.index++);
                        }
                        bits |= (resb > 0 ? 1 : 0) * power;
                        power <<= 1;
                    }
                    c = f(bits);
                    break;
                case 1:
                    bits = 0;
                    maxpower = Math.pow(2, 16);
                    power = 1;
                    while (power != maxpower) {
                        resb = data.val & data.position;
                        data.position >>= 1;
                        if (data.position == 0) {
                            data.position = resetValue;
                            data.val = getNextValue(data.index++);
                        }
                        bits |= (resb > 0 ? 1 : 0) * power;
                        power <<= 1;
                    }
                    c = f(bits);
                    break;
                case 2:
                    return "";
            }
            dictionary[3] = c;
            w = c;
            result.push(c);
            while (true) {
                if (data.index > length) {
                    return "";
                }

                bits = 0;
                maxpower = Math.pow(2, numBits);
                power = 1;
                while (power != maxpower) {
                    resb = data.val & data.position;
                    data.position >>= 1;
                    if (data.position == 0) {
                        data.position = resetValue;
                        data.val = getNextValue(data.index++);
                    }
                    bits |= (resb > 0 ? 1 : 0) * power;
                    power <<= 1;
                }

                switch (c = bits) {
                    case 0:
                        bits = 0;
                        maxpower = Math.pow(2, 8);
                        power = 1;
                        while (power != maxpower) {
                            resb = data.val & data.position;
                            data.position >>= 1;
                            if (data.position == 0) {
                                data.position = resetValue;
                                data.val = getNextValue(data.index++);
                            }
                            bits |= (resb > 0 ? 1 : 0) * power;
                            power <<= 1;
                        }

                        dictionary[dictSize++] = f(bits);
                        c = dictSize - 1;
                        enlargeIn--;
                        break;
                    case 1:
                        bits = 0;
                        maxpower = Math.pow(2, 16);
                        power = 1;
                        while (power != maxpower) {
                            resb = data.val & data.position;
                            data.position >>= 1;
                            if (data.position == 0) {
                                data.position = resetValue;
                                data.val = getNextValue(data.index++);
                            }
                            bits |= (resb > 0 ? 1 : 0) * power;
                            power <<= 1;
                        }
                        dictionary[dictSize++] = f(bits);
                        c = dictSize - 1;
                        enlargeIn--;
                        break;
                    case 2:
                        return result.join('');
                }

                if (enlargeIn == 0) {
                    enlargeIn = Math.pow(2, numBits);
                    numBits++;
                }

                if (dictionary[c]) {
                    entry = dictionary[c];
                } else {
                    if (c === dictSize) {
                        entry = w + w.charAt(0);
                    } else {
                        return null;
                    }
                }
                result.push(entry);

                // Add w+entry[0] to the dictionary.
                dictionary[dictSize++] = w + entry.charAt(0);
                enlargeIn--;

                w = entry;

                if (enlargeIn == 0) {
                    enlargeIn = Math.pow(2, numBits);
                    numBits++;
                }

            }
        }
    };
    return LZString;
})();