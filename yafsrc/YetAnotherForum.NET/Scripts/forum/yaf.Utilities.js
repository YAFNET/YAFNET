/// <summary>
/// Scrolls to document to the top.
/// </summary>
function ScrollToTop() {
    jQuery('body,html').animate({ scrollTop: 0 }, 820);
    return false;
}

/// <summary>
/// Toggles the container.
/// </summary>
/// <param name="id">The identifier.</param>
/// <param name="senderId">The sender identifier.</param>
/// <param name="showText">The show text.</param>
/// <param name="hideText">The hide text.</param>
/// <returns></returns>
function toggleContainer(id, senderId, showText, hideText) {
    var el = jQuery('#' + id);
    var sender = jQuery('#' + senderId);

    el.toggle(function () {
        sender.attr("title", hideText);
        sender.html(hideText);
        sender.addClass('hide');
    }, function () {
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
        } catch (e) {
            cell.style.cursor = "hand";
        }
    } else {
        cell.className = "popupitem";
    }
}

function formatState(state) {
    if (!state.id) {
        return state.text;
    }
    if ($($(state.element).data("content")).length === 0) {
        return state.text;
    }

    var $state = $($(state.element).data("content"));
    return $state;
};

function doClick(buttonName, e) {
    var key;

    if (window.event) {
        key = window.event.keyCode;
    } else {
        key = e.which;
    }

    if (key == 13) {
        var btn = document.getElementById(buttonName);
        if (btn != null) {
            e.preventDefault();
            btn.click();
            event.keyCode = 0;
        }
    }
}