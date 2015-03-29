function ChangeReputationBarColor(value, text, selector) {
    jQuery(selector).html('<div class="ui-progressbar-value ui-widget-header ui-corner-left ReputationBarValue" style="width: ' + value + '%; "></div>');
    jQuery(selector).attr('aria-valuenow', value);

    // 0%
    var reputationbarvalue = '.ReputationBarValue';
    var $repbar = jQuery(selector).children(reputationbarvalue);

    if (value == 0) {
        $repbar.addClass("BarDarkRed");
        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 1-29%
    else if (value < 20) {

        $repbar.addClass("BarRed");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 30-39%
    else if (value < 30) {
        $repbar.addClass("BarOrangeRed");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 40-49%
    else if (value < 40) {
        $repbar.addClass("BarDarkOrange");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 50-59%
    else if (value < 50) {
        $repbar.addClass("BarOrange");
        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 60-69%
    else if (value < 60) {
        $repbar.addClass("BarYellow");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 70-79%
    else if (value < 80) {
        $repbar.addClass("BarLightGreen");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 80-89%
    else if (value < 90) {
        $repbar.addClass("BarGreen");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 90-100%
    else {
        jQuery(selector).html('<div class="ui-progressbar-value ui-widget-header ui-corner-left ui-corner-right ReputationBarValue BarDarkGreen" style="width: ' + value + '%; "><p class="ReputationBarText">' + text + '</p></div>');
    }
}

function ScrollToTop() {
    jQuery('body,html').animate({ scrollTop: 0 }, 820);
    return false;
}


function toggleContainer(id, senderId, showText, hideText) {
    var el = jQuery('#' + id);
    var sender = jQuery('#' + senderId);

    el.toggle(function() {
        sender.attr("title", hideText);
        sender.html(hideText);
        sender.addClass('hide');
    }, function() {
        sender.attr("title", showText);
        sender.html(showText);
        sender.addClass('show');
    });
}

function getEvent(eventobj) {
    if (eventobj.stopPropagation) {
        eventobj.stopPropagation();
        eventobj.preventDefault();
        return eventobj;
    } else {
        window.event.returnValue = false;
        window.event.cancelBubble = true;
        return window.event;
    }
}

function yaf_mouseover() {
    var evt = getEvent(window.event);
    if (evt.srcElement) {
        evt.srcElement.style.cursor = "hand";
    } else if (evt.target) {
        evt.target.style.cursor = "pointer";
    }
}

function yaf_left(obj) {
    return jQuery(obj).position().left;
}

function yaf_top(obj) {
    return jQuery(obj).position().top + jQuery(obj).outerHeight() + 1;
}

function yaf_popit(menuName) {
    var evt = getEvent(window.event);
    var target;

    if (!document.getElementById) {
        throw ('ERROR: missing getElementById');
    }

    if (evt.srcElement)
        target = evt.srcElement;
    else if (evt.target)
        target = evt.target;
    else {
        throw ('ERROR: missing event target');
    }

    var newmenu = document.getElementById(menuName);

    if (window.themenu && window.themenu.id != newmenu.id)
        yaf_hidemenu();

    window.themenu = newmenu;
    if (!window.themenu.style) {
        throw ('ERROR: missing style');
    }

    if (!jQuery(themenu).is(":visible")) {
        var x = yaf_left(target);
        // Make sure the menu stays inside the page
        // offsetWidth or clientWidth?!?
        if (x + jQuery(themenu).outerWidth() + 2 > jQuery(document).width()) {
            x = jQuery(document).width() - jQuery(themenu).outerWidth() - 2;
        }

        themenu.style.left = x + "px";
        themenu.style.top = yaf_top(target) + "px";
        themenu.style.zIndex = 100;

        jQuery(themenu).fadeIn();
    } else {
        yaf_hidemenu();
    }

    return false;
}

function yaf_hidemenu() {
    if (window.themenu) {
        jQuery(window.themenu).fadeOut();
        window.themenu = null;
    }
}

function mouseHover(cell, hover) {
    if (hover) {
        cell.className = "popupitemhover";
        try {
            cell.style.cursor = "pointer";
        } catch(e) {
            cell.style.cursor = "hand";
        }
    } else {
        cell.className = "popupitem";
    }
}

// Generic Functions
jQuery(document).ready(function () {
    
    /// <summary>
    /// Convert user posted image to modal images
    /// </summary>
    jQuery(".postContainer .UserPostedImage,.postContainer_Alt .UserPostedImage").each(function() {
        var image = jQuery(this);

        if (!image.parents('a').length) {
           image.wrap('<a href="' + image.attr("src") + '" class="ceebox" title="' + image.attr("alt") + '"/>');
        }
    });

    jQuery('.postdiv div').has('.attachedImage').addClass('ceebox');

    jQuery(".standardSelectMenu").selectmenu({
        change: function() {
            if (typeof (jQuery(this).attr('onchange')) !== 'undefined') {
                __doPostBack(jQuery(this).attr('name'), '');
            }
        }
    });

    jQuery(".standardSelect2Menu").select2({
        width: '350px'
    });

    if (typeof (jQuery.fn.spinner) !== 'undefined') {
        jQuery('.Numeric').spinner({min: 0});
    }

    jQuery.widget.bridge('uitooltip', $.ui.tooltip);
    jQuery.widget.bridge('uibutton', $.ui.button);

    var dialog = jQuery(".UploadDialog").dialog({
        autoOpen: false,
        width: 530,

        modal: true,
        buttons: {
            Cancel: function () {
                dialog.dialog("close");
            }
        },
        close: function () {
        }
    });

    jQuery(".OpenUploadDialog,.UploadNewFileLine").on("click", function () {
        dialog.dialog("open");
    });

    if (jQuery('#AttachmentsListPager').length) {
        var Attachments_entries = jQuery('#AttachmentsPagerHidden div.result').length;
        jQuery('#AttachmentsListPager').pagination(Attachments_entries, {
            callback: AttachmentsPageSelectCallback,
            items_per_page: 1,
            num_display_entries: 3,
            num_edge_entries: 1,
            prev_class: 'smiliesPagerPrev',
            next_class: 'smiliesPagerNext',
            prev_text: '&laquo;',
            next_text: '&raquo;'
        });
    }

    if (typeof (jQuery.fn.uitooltip) !== 'undefined') {
        jQuery(document).uitooltip({
            items: "[data-url]",
            content: function() {
                var element = $(this);
                var text = element.text();
                var url = element.attr('data-url');
                return "<img alt='" + text + "'  src='" + url + "' style='max-width:300px' />";
            }
        });
    }

    // Show caps lock info on password fields
    jQuery("input[type='password']").keypress(function (e) {
        var s = String.fromCharCode(e.which);
        if (s.toUpperCase() === s && s.toLowerCase() !== s && !e.shiftKey) {
            jQuery('.CapsLockWarning').show();
        }
        else {
            jQuery('.CapsLockWarning').hide();
        }
    });
});

function AttachmentsPageSelectCallback(page_index) {
    var Attachments_content = jQuery('#AttachmentsPagerHidden div.result:eq(' + page_index + ')').clone();
    jQuery('#AttachmentsPagerResult').empty().append(Attachments_content);
    return false;
}

$(function () {
    $.widget("custom.iconselectmenu", $.ui.selectmenu, {
        _renderItem: function (ul, item) {
            var li = $("<li>", { text: item.label });

            if (item.disabled) {
                li.addClass("ui-state-disabled");
            }

            $("<span>", {
                style: item.element.attr("data-style"),
                "class": "ui-icon " + item.element.attr("data-class")
            })
              .appendTo(li);

            return li.appendTo(ul);
        }
    });
});

function toggleNewSelection(source) {
    var isChecked = source.checked;
    $("input[id*='New']").each(function () {
        $(this).attr('checked', false);
    });
    source.checked = isChecked;
}

function toggleOldSelection(source) {
    var isChecked = source.checked;
    $("input[id*='Old']").each(function () {
        $(this).attr('checked', false);
    });
    source.checked = isChecked;
}

function RenderMessageDiff(messageEditedAtText, nothingSelectedText, selectBothText, selectDifferentText) {
    var oldElement = $("input[id*='New']:checked");
    var newElement = $("input[id*='Old']:checked");

    if (newElement.length && oldElement.length) {
        // check if two different messages are selected
        if ($("input[id*='Old']:checked").attr('id').slice(-1) == $("input[id*='New']:checked").attr('id').slice(-1)) {
            alert(selectDifferentText);
        } else {
            var base = difflib.stringAsLines($("input[id*='Old']:checked").parent().next().next().find("input[id*='MessageField']").attr('value'));
            var newtxt = difflib.stringAsLines($("input[id*='New']:checked").parent().next().find("input[id*='MessageField']").attr('value'));
            var sm = new difflib.SequenceMatcher(base, newtxt);
            var opcodes = sm.get_opcodes();

            $("#diffContent").html('<div class="diffContent">' + diffview.buildView({
                baseTextLines: base,
                newTextLines: newtxt,
                opcodes: opcodes,
                baseTextName: messageEditedAtText + oldElement.parent().next().next().next().next().html(),
                newTextName: messageEditedAtText + oldElement.parent().next().next().next().next().html(),
                contextSize: 3,
                viewType: 0
            }).outerHTML + '</div>');
        }
    }
    else if (newElement.length || oldElement.length) {
        alert(selectBothText);
    } else {
        alert(nothingSelectedText);
    }
}

document.onclick = yaf_hidemenu;
if (document.addEventListener) document.addEventListener("click", function(e) { window.event = e; }, true);
if (document.addEventListener) document.addEventListener("mouseover", function(e) { window.event = e; }, true);