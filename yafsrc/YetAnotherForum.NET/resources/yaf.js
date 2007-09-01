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
	var x = 0;
	while(obj) {
		x += obj.offsetLeft;
		obj = obj.offsetParent;
	}
	return x;
}

function yaf_top(obj) {
	var y = obj.offsetHeight;
	while(obj) {
		y += obj.offsetTop;
		obj = obj.offsetParent;
	}
	return y;
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

	if(themenu.style.visibility == "hidden") {
		var x = yaf_left(target);
		// Make sure the menu stays inside the page
		// offsetWidth or clientWidth?!?
		if(document.documentElement) {
			if(x + themenu.offsetWidth>document.documentElement.offsetWidth - 20)
				x += target.offsetWidth - themenu.offsetWidth;
		}

		themenu.style.left = x;
		themenu.style.top = yaf_top(target);
		themenu.style.visibility = "visible";
		themenu.style.zIndex = 100;
	} else {
		yaf_hidemenu();
	}

	return false;
}

function yaf_hidemenu() {
	if(window.themenu) {
		window.themenu.style.visibility = "hidden";
		window.themenu = null;
	}
}

function mouseHover(cell,hover) {
	if(hover) {
		cell.className = "postfooter";
		try {
			cell.style.cursor = "pointer";
		}
		catch(e) {
			cell.style.cursor = "hand";
		}
	} else {
		cell.className = "post";
	}
}

document.onclick = yaf_hidemenu;
if(document.addEventListener) document.addEventListener("click",function(e){window.event=e;},true);
if(document.addEventListener) document.addEventListener("mouseover",function(e){window.event=e;},true);
