function toggleContainer(id, senderId, showText, hideText){
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
	if(eventobj.stopPropagation) {
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
	if(evt.srcElement) {
		evt.srcElement.style.cursor = "hand";
	} else if(evt.target) {
		evt.target.style.cursor = "pointer";
	}
}

function yaf_left(obj) {
    return $(obj).position().left; 
}

function yaf_top(obj) {
    return $(obj).position().top + $(obj).outerHeight() + 1;
}

function yaf_popit(menuName) {
	var evt = getEvent(window.event);
	var target,newmenu;

	if(!document.getElementById) {
		throw('ERROR: missing getElementById');
		return false;
	}
	
	if(evt.srcElement)
		target = evt.srcElement;
	else if(evt.target)
		target = evt.target;
	else {
		throw('ERROR: missing event target');
		return false;
	}
	
	newmenu = document.getElementById(menuName);

	if(window.themenu && window.themenu.id!=newmenu.id)
		yaf_hidemenu();

	window.themenu = newmenu;
	if(!window.themenu.style) {
		throw('ERROR: missing style');
		return false;
	}

	if (!$(themenu).is(":visible")) {
	    var x = yaf_left(target);
	    // Make sure the menu stays inside the page
	    // offsetWidth or clientWidth?!?
	    if (x + $(themenu).outerWidth() + 2 > $(document).width()) {
	        x = $(document).width() - $(themenu).outerWidth() - 2;
	    }

	    themenu.style.left = x + "px";
	    themenu.style.top = yaf_top(target) + "px";
	    themenu.style.zIndex = 100;

	    $(themenu).fadeIn();
	} else {
	    yaf_hidemenu();
	}

	return false;
}

function yaf_hidemenu() {
	if(window.themenu) {
		$(window.themenu).fadeOut();
		window.themenu = null;
	}
}

function mouseHover(cell,hover) {
	if(hover) {
		cell.className = "popupitemhover";
		try {
			cell.style.cursor = "pointer";
		}
		catch(e) {
			cell.style.cursor = "hand";
		}
	} else {
		cell.className = "popupitem";
	}
}

document.onclick = yaf_hidemenu;
if(document.addEventListener) document.addEventListener("click",function(e){window.event=e;},true);
if(document.addEventListener) document.addEventListener("mouseover",function(e){window.event=e;},true);
